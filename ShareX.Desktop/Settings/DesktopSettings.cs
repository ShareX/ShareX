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

using Newtonsoft.Json;
using ShareX.Desktop.Commands;
using System;
using System.IO;

namespace ShareX.Desktop.Settings;

/// <summary>
/// Options of the cross-platform application. Uploader accounts and credentials stay in UploadersConfig.json,
/// which uses the same format as the Windows application.
/// </summary>
public sealed class DesktopSettings
{
    public const string FileName = "DesktopSettings.json";

    /// <summary>Where screenshots are saved. Empty uses the Pictures folder.</summary>
    public string ScreenshotsFolder { get; set; } = "";

    /// <summary>Group files in a yyyy-MM sub folder, like the Windows application.</summary>
    public bool GroupByMonth { get; set; } = true;

    public bool SaveToFile { get; set; } = true;

    public bool CopyImageToClipboard { get; set; } = true;

    public bool UploadImage { get; set; }

    public bool CopyUrlAfterUpload { get; set; } = true;

    public bool OpenEditorAfterCapture { get; set; }

    public bool ShowNotifications { get; set; } = true;

    public bool IncludeCursor { get; set; }

    /// <summary>
    /// The ImageDestination member used for screenshots, for example Imgur, AmazonS3 or CustomImageUploader.
    /// Empty by default so nothing is ever sent to a third party until the user chooses where.
    /// </summary>
    public string ImageUploader { get; set; } = "";

    /// <summary>The FileDestination member used for files that are not images.</summary>
    public string FileUploader { get; set; } = "";

    /// <summary>Format for file names. {0} is the capture time.</summary>
    public string FileNameFormat { get; set; } = "ShareX_{0:yyyyMMdd_HHmmss}";

    public AfterCapture ToAfterCapture(AfterCaptureOverrides overrides) => new AfterCapture(
        overrides.Save ?? SaveToFile,
        overrides.CopyImage ?? CopyImageToClipboard,
        overrides.Upload ?? UploadImage,
        overrides.Edit ?? OpenEditorAfterCapture,
        overrides.Notify ?? ShowNotifications);

    public static DesktopSettings Load(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<DesktopSettings>(File.ReadAllText(path)) ?? new DesktopSettings();
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // A damaged settings file must not stop capturing. Defaults apply and the next save replaces it.
        }

        return new DesktopSettings();
    }

    public void Save(string path)
    {
        string? folder = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonConvert.SerializeObject(this, Formatting.Indented));
        File.Move(temporary, path, overwrite: true);
    }
}

/// <summary>The resolved actions for one capture.</summary>
public sealed record AfterCapture(bool Save, bool CopyImage, bool Upload, bool Edit, bool Notify);
