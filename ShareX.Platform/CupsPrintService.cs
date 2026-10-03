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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>Printing through CUPS, which Linux and macOS both use: pages are written as PNG files and sent as one job with lp.</summary>
/// <remarks>
/// There is no printer dialog: jobs go to the printer named in ShareX's print settings, or the default printer. The CUPS image
/// filter fits each page to the paper, so pages rendered for a slightly different paper size still print whole.
/// </remarks>
public sealed class CupsPrintService : IPrintService
{
    private static readonly TimeSpan QueryTimeout = TimeSpan.FromSeconds(10);
    // Countries that use US Letter; the rest of the world uses A4.
    private static readonly HashSet<string> LetterTerritories = new(StringComparer.OrdinalIgnoreCase) { "US", "CA", "MX", "PH", "CL", "CO", "VE", "CR", "GT", "PR", "PA", "SV", "DO", "BZ" };

    private readonly ICommandRunner runner;
    private readonly LinuxDistribution distribution;
    private readonly Func<string, string?> getEnvironment;
    private readonly string temporaryDirectory;

    public CupsPrintService(PlatformInfo info, ICommandRunner runner)
        : this(runner, info.Distribution ?? LinuxDistribution.Unknown, Environment.GetEnvironmentVariable, Path.GetTempPath())
    {
    }

    internal CupsPrintService(ICommandRunner runner, LinuxDistribution distribution, Func<string, string?> getEnvironment, string temporaryDirectory)
    {
        this.runner = runner;
        this.distribution = distribution;
        this.getEnvironment = getEnvironment;
        this.temporaryDirectory = temporaryDirectory;
    }

    public FeatureSupport Support => runner.Exists("lp") ? FeatureSupport.Supported : LinuxPackages.Missing(distribution, LinuxTool.Cups);

    public FeatureSupport DialogSupport { get; } = FeatureSupport.NotSupported(
        "There is no printer dialog here. ShareX prints to the printer named in its print settings, or to the default printer.");

    public bool IsPrinterInstalled(string printerName) =>
        !string.IsNullOrWhiteSpace(printerName) && runner.Exists("lpstat") && Run("lpstat", ["-p", printerName])?.Success == true;

    public PrintPageSetup GetPageSetup(PrintOptions options)
    {
        List<string> arguments = new List<string>();

        if (!string.IsNullOrEmpty(options.PrinterName))
        {
            arguments.Add("-p");
            arguments.Add(options.PrinterName);
        }

        CommandResult? result = runner.Exists("lpoptions") ? Run("lpoptions", arguments) : null;
        return ParseMedia(result?.Success == true ? result.StandardOutputText : null) ?? GetLocalePaper();
    }

    /// <summary>Jobs go to the chosen or default printer without asking.</summary>
    public bool ShowPrintDialog(PrintOptions options) => true;

    public void Print(PrintOptions options, string documentName, Func<PrintPageSetup, int, PrintedPage?> renderPage)
    {
        PrintPageSetup setup = GetPageSetup(options);
        string directory = Path.Combine(temporaryDirectory, "sharex-print-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            List<string> files = new List<string>();

            for (int index = 0; ; index++)
            {
                PrintedPage? page = renderPage(setup, index);

                if (page == null)
                {
                    break;
                }

                string file = Path.Combine(directory, $"page-{index + 1:D4}.png");
                File.WriteAllBytes(file, PngCodec.Encode(page.Image));
                files.Add(file);

                if (!page.HasMorePages)
                {
                    break;
                }
            }

            if (files.Count == 0)
            {
                return;
            }

            // lp sends the files to the spooler before it exits, so they can be deleted afterwards.
            CommandResult result = Run("lp", CreatePrintArguments(options, documentName, files), TimeSpan.FromMinutes(2))
                ?? throw new IOException("lp did not finish.");

            if (!result.Success)
            {
                throw new IOException("Printing failed: " + result.StandardError.Trim());
            }
        }
        finally
        {
            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    internal static List<string> CreatePrintArguments(PrintOptions options, string documentName, IReadOnlyList<string> files)
    {
        List<string> arguments = new List<string>();

        if (!string.IsNullOrEmpty(options.PrinterName))
        {
            arguments.Add("-d");
            arguments.Add(options.PrinterName);
        }

        arguments.Add("-n");
        arguments.Add(Math.Max(1, options.Copies).ToString(CultureInfo.InvariantCulture));
        arguments.Add("-t");
        arguments.Add(string.IsNullOrWhiteSpace(documentName) ? "ShareX" : documentName);
        arguments.Add("-o");
        arguments.Add("fit-to-page");
        arguments.Add("--");
        arguments.AddRange(files);
        return arguments;
    }

    /// <summary>Reads the media option from lpoptions output, for example "media=iso_a4_210x297mm" or "media=Letter".</summary>
    internal static PrintPageSetup? ParseMedia(string? lpoptionsOutput)
    {
        if (string.IsNullOrEmpty(lpoptionsOutput))
        {
            return null;
        }

        foreach (string option in lpoptionsOutput.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!option.StartsWith("media=", StringComparison.OrdinalIgnoreCase) && !option.StartsWith("PageSize=", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string media = option[(option.IndexOf('=') + 1)..].ToLowerInvariant();

            if (media.Contains("a4"))
            {
                return PrintPageSetup.A4;
            }

            if (media.Contains("letter"))
            {
                return PrintPageSetup.Letter;
            }
        }

        return null;
    }

    /// <summary>The paper size for the user's region, from LC_PAPER, LC_ALL or LANG (for example "en_US.UTF-8").</summary>
    internal PrintPageSetup GetLocalePaper()
    {
        string? locale = getEnvironment("LC_PAPER") ?? getEnvironment("LC_ALL") ?? getEnvironment("LANG");

        if (!string.IsNullOrEmpty(locale))
        {
            int underscore = locale.IndexOf('_');

            if (underscore >= 0 && locale.Length >= underscore + 3)
            {
                return LetterTerritories.Contains(locale.Substring(underscore + 1, 2)) ? PrintPageSetup.Letter : PrintPageSetup.A4;
            }
        }

        return PrintPageSetup.A4;
    }

    private CommandResult? Run(string command, IReadOnlyList<string> arguments, TimeSpan? timeout = null)
    {
        try
        {
            Task<CommandResult> task = Task.Run(() => runner.RunAsync(command, arguments, timeout: timeout ?? QueryTimeout));
            return task.Wait((timeout ?? QueryTimeout) + TimeSpan.FromSeconds(5)) ? task.Result : null;
        }
        catch (AggregateException e) when (e.InnerException is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }
}
