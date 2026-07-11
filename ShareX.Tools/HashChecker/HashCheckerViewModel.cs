#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools.Infrastructure;

namespace ShareX.Tools.HashChecker;

public sealed record HashAlgorithmItem(HashCheckerAlgorithm Algorithm, string DisplayName);

public sealed partial class HashCheckerViewModel : ViewModelBase, IDisposable
{
    public static IReadOnlyList<HashAlgorithmItem> Algorithms { get; } =
    [
        new(HashCheckerAlgorithm.Crc32, "CRC-32"),
        new(HashCheckerAlgorithm.Md5, "MD5"),
        new(HashCheckerAlgorithm.Sha1, "SHA-1"),
        new(HashCheckerAlgorithm.Sha256, "SHA-256"),
        new(HashCheckerAlgorithm.Sha384, "SHA-384"),
        new(HashCheckerAlgorithm.Sha512, "SHA-512")
    ];

    private readonly HashCalculationHandler _hashCalculationHandler;
    private readonly Action? _playNotificationSound;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCheck))]
    private string _firstFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCheck))]
    private string _secondFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCheck))]
    [NotifyPropertyChangedFor(nameof(ResultLabel))]
    [NotifyPropertyChangedFor(nameof(TargetLabel))]
    private bool _compareTwoFiles;

    [ObservableProperty]
    private HashAlgorithmItem _selectedAlgorithm = Algorithms[3];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasComparison))]
    [NotifyPropertyChangedFor(nameof(HashesMatch))]
    [NotifyPropertyChangedFor(nameof(HashesDiffer))]
    private string _resultHash = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasComparison))]
    [NotifyPropertyChangedFor(nameof(HashesMatch))]
    [NotifyPropertyChangedFor(nameof(HashesDiffer))]
    private string _targetHash = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanCheck))]
    private bool _isWorking;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public HashCheckerViewModel(
        HashCalculationHandler hashCalculationHandler,
        Action? playNotificationSound = null,
        string? filePath = null)
    {
        _hashCalculationHandler = hashCalculationHandler;
        _playNotificationSound = playNotificationSound;
        _firstFilePath = filePath ?? string.Empty;
    }

    public Func<string, string?, Task<string?>>? SelectFileRequested { get; set; }

    public bool IsIdle => !IsWorking;
    public bool CanCheck => !IsWorking && File.Exists(FirstFilePath) && (!CompareTwoFiles || File.Exists(SecondFilePath));
    public string ResultLabel => CompareTwoFiles ? "First file hash" : "Result";
    public string TargetLabel => CompareTwoFiles ? "Second file hash" : "Target hash";
    public bool HasComparison => !string.IsNullOrWhiteSpace(ResultHash) && !string.IsNullOrWhiteSpace(TargetHash);
    public bool HashesMatch => HasComparison && ResultHash.Equals(TargetHash.Trim(), StringComparison.OrdinalIgnoreCase);
    public bool HashesDiffer => HasComparison && !HashesMatch;

    [RelayCommand]
    private async Task BrowseFirstFileAsync()
    {
        if (!IsIdle || SelectFileRequested == null)
        {
            return;
        }

        string? filePath = await SelectFileRequested("Select file", FirstFilePath);
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            FirstFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task BrowseSecondFileAsync()
    {
        if (!IsIdle || SelectFileRequested == null)
        {
            return;
        }

        string? filePath = await SelectFileRequested("Select second file", SecondFilePath);
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            SecondFilePath = filePath;
        }
    }

    public async Task StartAutomaticallyAsync()
    {
        if (File.Exists(FirstFilePath))
        {
            await CheckAsync();
        }
    }

    [RelayCommand]
    private async Task CheckAsync()
    {
        if (!CanCheck)
        {
            StatusText = CompareTwoFiles ? "Select two existing files." : "Select an existing file.";
            return;
        }

        IsWorking = true;
        Progress = 0;
        ResultHash = string.Empty;
        if (CompareTwoFiles)
        {
            TargetHash = string.Empty;
        }
        StatusText = "Calculating hash...";
        _cancellationTokenSource = new CancellationTokenSource();
        IProgress<double> progress = new Progress<double>(value => Progress = Math.Clamp(value, 0, 100));

        try
        {
            string? result = await _hashCalculationHandler(
                FirstFilePath,
                SelectedAlgorithm.Algorithm,
                progress,
                _cancellationTokenSource.Token);

            if (string.IsNullOrWhiteSpace(result))
            {
                StatusText = _cancellationTokenSource.IsCancellationRequested ? "Hash check stopped." : "Unable to calculate the file hash.";
                return;
            }

            ResultHash = result.ToUpperInvariant();

            if (CompareTwoFiles)
            {
                Progress = 0;
                StatusText = "Calculating second file hash...";
                string? secondResult = await _hashCalculationHandler(
                    SecondFilePath,
                    SelectedAlgorithm.Algorithm,
                    progress,
                    _cancellationTokenSource.Token);

                if (string.IsNullOrWhiteSpace(secondResult))
                {
                    StatusText = _cancellationTokenSource.IsCancellationRequested
                        ? "Hash check stopped."
                        : "Unable to calculate the second file hash.";
                    return;
                }

                TargetHash = secondResult.ToUpperInvariant();
            }

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                Progress = 100;
                StatusText = CompareTwoFiles
                    ? (HashesMatch ? "The files have matching hashes." : "The files have different hashes.")
                    : "Hash calculation complete.";
                _playNotificationSound?.Invoke();
            }
        }
        catch (OperationCanceledException)
        {
            Progress = 0;
            StatusText = "Hash check stopped.";
        }
        catch (Exception ex)
        {
            Progress = 0;
            StatusText = $"Hash calculation failed: {ex.Message}";
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsWorking = false;
        }
    }

    [RelayCommand]
    private void Stop()
    {
        if (IsWorking)
        {
            StatusText = "Stopping hash check...";
            _cancellationTokenSource?.Cancel();
        }
    }

    public void SetDroppedFile(string filePath, bool secondFile)
    {
        if (!IsIdle)
        {
            return;
        }

        if (secondFile)
        {
            SecondFilePath = filePath;
        }
        else
        {
            FirstFilePath = filePath;
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
