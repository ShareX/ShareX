#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace HelpersLib
{
    public static class Helpers
    {
        public const string Numbers = "0123456789"; // 48 ... 57
        public const string AlphabetCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 65 ... 90
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz"; // 97 ... 122
        public const string Alphanumeric = Numbers + AlphabetCapital + Alphabet;
        public const string URLCharacters = Alphanumeric + "-._~"; // 45 46 95 126
        public const string URLPathCharacters = URLCharacters + "/"; // 47
        public const string ValidURLCharacters = URLPathCharacters + ":?#[]@!$&'()*+,;= ";

        private static readonly object randomLock = new object();
        private static readonly Random random = new Random();

        public static readonly Version OSVersion = Environment.OSVersion.Version;

        public static int Random(int max)
        {
            lock (randomLock)
            {
                return random.Next(max + 1);
            }
        }

        public static int Random(int min, int max)
        {
            lock (randomLock)
            {
                return random.Next(min, max + 1);
            }
        }

        public static string GetFilenameExtension(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && filePath.Contains('.'))
            {
                int pos = filePath.LastIndexOf('.');

                if (pos <= filePath.Length)
                {
                    return filePath.Substring(pos + 1);
                }
            }

            return string.Empty;
        }

        private static bool IsValidFile(string filePath, Type enumType)
        {
            string ext = GetFilenameExtension(filePath);

            if (!string.IsNullOrEmpty(ext))
            {
                return Enum.GetNames(enumType).Any(x => ext.Equals(x, StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        public static bool IsImageFile(string filePath)
        {
            return IsValidFile(filePath, typeof(ImageFileExtensions));
        }

        public static bool IsTextFile(string filePath)
        {
            return IsValidFile(filePath, typeof(TextFileExtensions));
        }

        public static EDataType FindDataType(string filePath)
        {
            if (IsImageFile(filePath))
            {
                return EDataType.Image;
            }

            if (IsTextFile(filePath))
            {
                return EDataType.Text;
            }

            return EDataType.File;
        }

        public static string AddZeroes(int number, int digits = 2)
        {
            return number.ToString().PadLeft(digits, '0');
        }

        public static string HourTo12(int hour)
        {
            if (hour == 0)
            {
                return 12.ToString();
            }

            if (hour > 12)
            {
                return AddZeroes(hour - 12);
            }

            return AddZeroes(hour);
        }

        public static char GetRandomChar(string chars)
        {
            return chars[Random(chars.Length - 1)];
        }

        public static string GetRandomString(string chars, int length)
        {
            StringBuilder sb = new StringBuilder();

            while (length-- > 0)
            {
                sb.Append(GetRandomChar(chars));
            }

            return sb.ToString();
        }

        public static string GetRandomNumber(int length)
        {
            return GetRandomString(Numbers, length);
        }

        public static string GetRandomAlphanumeric(int length)
        {
            return GetRandomString(Alphanumeric, length);
        }

        public static string GetRandomKey(int length = 5, int count = 3, char separator = '-')
        {
            return Enumerable.Range(1, (length + 1) * count - 1).Aggregate("", (x, index) => x += index % (length + 1) == 0 ? separator : GetRandomChar(Alphanumeric));
        }

        public static string GetAllCharacters()
        {
            return Encoding.UTF8.GetString(Enumerable.Range(1, 255).Select(i => (byte)i).ToArray());
        }

        public static string GetValidFileName(string fileName)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            return new string(fileName.Where(c => !invalidFileNameChars.Contains(c)).ToArray());
        }

        public static string GetValidFolderPath(string folderPath)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            return new string(folderPath.Where(c => !invalidPathChars.Contains(c)).ToArray());
        }

        public static string GetValidFilePath(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            return GetValidFolderPath(folderPath) + Path.DirectorySeparatorChar + GetValidFileName(fileName);
        }

        public static string GetValidURL(string url, bool replaceSpace = false)
        {
            if (replaceSpace) url = url.Replace(' ', '_');
            return new string(url.Where(c => ValidURLCharacters.Contains(c)).ToArray());
        }

        public static string GetXMLValue(string input, string tag)
        {
            return Regex.Match(input, String.Format("(?<={0}>).+?(?=</{0})", tag)).Value;
        }

        public static string CombineURL(string url1, string url2)
        {
            bool url1Empty = string.IsNullOrEmpty(url1);
            bool url2Empty = string.IsNullOrEmpty(url2);

            if (url1Empty && url2Empty)
            {
                return string.Empty;
            }

            if (url1Empty)
            {
                return url2;
            }

            if (url2Empty)
            {
                return url1;
            }

            if (url1.EndsWith("/"))
            {
                url1 = url1.Substring(0, url1.Length - 1);
            }

            if (url2.StartsWith("/"))
            {
                url2 = url2.Remove(0, 1);
            }

            return url1 + "/" + url2;
        }

        public static string CombineURL(params string[] urls)
        {
            return urls.Aggregate(CombineURL);
        }

        public static string GetMimeType(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLower();

                if (!string.IsNullOrEmpty(ext))
                {
                    string mimeType = MimeTypes.GetMimeType(ext);

                    if (!string.IsNullOrEmpty(mimeType))
                    {
                        return mimeType;
                    }

                    using (RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext))
                    {
                        if (regKey != null && regKey.GetValue("Content Type") != null)
                        {
                            mimeType = regKey.GetValue("Content Type").ToString();

                            if (!string.IsNullOrEmpty(mimeType))
                            {
                                return mimeType;
                            }
                        }
                    }
                }
            }

            return MimeTypes.DefaultMimeType;
        }

        public static string[] GetEnumDescriptions<T>()
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>().Select(x => x.GetDescription()).ToArray();
        }

        public static int GetEnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        public static T GetEnumFromIndex<T>(int i)
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(i);
        }

        public static string[] GetEnumNamesProper<T>()
        {
            string[] names = Enum.GetNames(typeof(T));
            string[] newNames = new string[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                newNames[i] = GetProperName(names[i]);
            }

            return newNames;
        }

        // Example: "TopLeft" becomes "Top left"
        public static string GetProperName(string name)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (i > 0 && char.IsUpper(c))
                {
                    sb.Append(' ');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string GetProperExtension(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                int dot = filePath.LastIndexOf('.');

                if (dot >= 0)
                {
                    string ext = filePath.Substring(dot + 1);
                    return ext.ToLowerInvariant();
                }
            }

            return null;
        }

        public static string Encode(string text, string unreservedCharacters)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text)
                {
                    if (unreservedCharacters.Contains(c))
                    {
                        result.Append(c);
                    }
                    else
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());

                        foreach (byte b in bytes)
                        {
                            result.AppendFormat(CultureInfo.InvariantCulture, "%{0:X2}", b);
                        }
                    }
                }
            }

            return result.ToString();
        }

        public static string URLEncode(string text)
        {
            return Encode(text, URLCharacters);
        }

        public static string URLPathEncode(string text)
        {
            return Encode(text, URLPathCharacters);
        }

        public static void OpenFolder(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                if (Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else
                {
                    MessageBox.Show("Folder not exist:\r\n" + folderPath, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static void OpenFolderWithFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
            }
        }

        public static bool CheckVersion(Version currentVersion, Version latestVersion)
        {
            return NormalizeVersion(latestVersion).CompareTo(NormalizeVersion(currentVersion)) > 0;
        }

        private static Version NormalizeVersion(Version version)
        {
            return new Version(Math.Max(version.Major, 0), Math.Max(version.Minor, 0), Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }

        public static bool IsWindowsXP()
        {
            return OSVersion.Major == 5 && OSVersion.Minor == 1;
        }

        public static bool IsWindowsXPOrGreater()
        {
            return (OSVersion.Major == 5 && OSVersion.Minor >= 1) || OSVersion.Major > 5;
        }

        public static bool IsWindowsVista()
        {
            return OSVersion.Major == 6;
        }

        public static bool IsWindowsVistaOrGreater()
        {
            return OSVersion.Major >= 6;
        }

        public static bool IsWindows7()
        {
            return OSVersion.Major == 6 && OSVersion.Minor == 1;
        }

        public static bool IsWindows7OrGreater()
        {
            return (OSVersion.Major == 6 && OSVersion.Minor >= 1) || OSVersion.Major > 6;
        }

        public static bool IsWindows8()
        {
            return OSVersion.Major == 6 && OSVersion.Minor == 2;
        }

        public static bool IsWindows8OrGreater()
        {
            return (OSVersion.Major == 6 && OSVersion.Minor >= 2) || OSVersion.Major > 6;
        }

        public static bool IsDefaultInstallDir()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Application.ExecutablePath.StartsWith(path);
        }

        public static void OpenURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        Process.Start(url);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e, "LoadBrowserAsync(" + url + ") failed");
                    }
                });
            }
        }

        public static bool IsValidURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                return !url.StartsWith("file://") && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            }

            return false;
        }

        public static bool IsValidURLRegex(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            // https://gist.github.com/729294
            string pattern =
                "^" +
                // protocol identifier
                "(?:(?:https?|ftp)://)" +
                // user:pass authentication
                "(?:\\S+(?::\\S*)?@)?" +
                "(?:" +
                // IP address exclusion
                // private & local networks
                "(?!10(?:\\.\\d{1,3}){3})" +
                "(?!127(?:\\.\\d{1,3}){3})" +
                "(?!169\\.254(?:\\.\\d{1,3}){2})" +
                "(?!192\\.168(?:\\.\\d{1,3}){2})" +
                "(?!172\\.(?:1[6-9]|2\\d|3[0-1])(?:\\.\\d{1,3}){2})" +
                // IP address dotted notation octets
                // excludes loopback network 0.0.0.0
                // excludes reserved space >= 224.0.0.0
                // excludes network & broacast addresses
                // (first & last IP address of each class)
                "(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])" +
                "(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}" +
                "(?:\\.(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))" +
                "|" +
                // host name
                "(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)" +
                // domain name
                "(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*" +
                // TLD identifier
                "(?:\\.(?:[a-z\\u00a1-\\uffff]{2,}))" +
                ")" +
                // port number
                "(?::\\d{2,5})?" +
                // resource path
                "(?:/[^\\s]*)?" +
                "$";

            return Regex.IsMatch(url.Trim(), pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;

            string pattern = @"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)";

            return Regex.IsMatch(ip.Trim(), pattern);
        }

        public static string GetUniqueFilePath(string filepath)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;
        }

        public static string ProperTimeSpan(TimeSpan ts)
        {
            string time = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            int hours = (int)ts.TotalHours;
            if (hours > 0) time = hours + ":" + time;
            return time;
        }

        public static void AsyncJob(Action thread, Action threadCompleted)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sender, e) => thread();
            bw.RunWorkerCompleted += (sender, e) => threadCompleted();
            bw.RunWorkerAsync();
        }

        public static object Clone(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(ms);
            }
        }

        public static void PlaySoundAsync(Stream stream)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                using (stream)
                using (SoundPlayer soundPlayer = new SoundPlayer(stream))
                {
                    soundPlayer.PlaySync();
                }
            });
        }

        public static bool BrowseFile(string title, TextBox tb, string initialDirectory = "")
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = title;

                try
                {
                    string path = tb.Text;

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Path.GetDirectoryName(path);

                        if (Directory.Exists(path))
                        {
                            ofd.InitialDirectory = path;
                        }
                    }
                }
                finally
                {
                    if (string.IsNullOrEmpty(ofd.InitialDirectory) && !string.IsNullOrEmpty(initialDirectory))
                    {
                        ofd.InitialDirectory = initialDirectory;
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    tb.Text = ofd.FileName;
                    return true;
                }
            }

            return false;
        }

        public static bool BrowseFolder(string title, TextBox tb, string initialDirectory = "")
        {
            using (FolderSelectDialog fsd = new FolderSelectDialog())
            {
                fsd.Title = title;

                string path = tb.Text;

                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    fsd.InitialDirectory = path;
                }
                else if (!string.IsNullOrEmpty(initialDirectory))
                {
                    fsd.InitialDirectory = initialDirectory;
                }

                if (fsd.ShowDialog())
                {
                    tb.Text = fsd.FileName;
                    return true;
                }
            }

            return false;
        }

        public static bool WaitWhile(Func<bool> check, int interval, int timeout = -1)
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (check())
            {
                if (timeout >= 0 && timer.ElapsedMilliseconds >= timeout)
                {
                    return false;
                }

                Thread.Sleep(interval);
            }

            return true;
        }

        public static void WaitWhileAsync(Func<bool> check, int interval, int timeout, Action onSuccess, int waitStart = 0)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                if (waitStart > 0)
                {
                    Thread.Sleep(waitStart);
                }

                e.Result = WaitWhile(check, interval, timeout);
            };
            bw.RunWorkerCompleted += (sender, e) =>
            {
                if ((bool)e.Result) onSuccess();
            };
            bw.RunWorkerAsync();
        }

        public static bool IsFileLocked(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static void CreateDirectoryIfNotExist(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string directoryName = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
        }

        public static void BackupFileMonthly(string filepath, string destinationFolder)
        {
            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                string newFilename = string.Format("{0}-{1:yyyy-MM}{2}", filename, DateTime.Now, extension);
                string newFilepath = Path.Combine(destinationFolder, newFilename);

                if (!File.Exists(newFilepath))
                {
                    CreateDirectoryIfNotExist(newFilepath);
                    File.Copy(filepath, newFilepath, false);
                }
            }
        }

        public static void BackupFileWeekly(string filepath, string destinationFolder)
        {
            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                string filename = Path.GetFileNameWithoutExtension(filepath);
                DateTime dateTime = DateTime.Now;
                string extension = Path.GetExtension(filepath);
                string newFilename = string.Format("{0}-{1:yyyy-MM}-W{2:00}{3}", filename, dateTime, dateTime.WeekOfYear(), extension);
                string newFilepath = Path.Combine(destinationFolder, newFilename);

                if (!File.Exists(newFilepath))
                {
                    CreateDirectoryIfNotExist(newFilepath);
                    File.Copy(filepath, newFilepath, false);
                }
            }
        }

        public static string GetUniqueID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string HtmlEncode(string text)
        {
            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
            StringBuilder result = new StringBuilder(chars.Length + (int)(chars.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);

                if (value > 127)
                {
                    result.AppendFormat("&#{0};", value);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static Point GetPosition(ContentAlignment placement, Point offset, Size backgroundSize, Size objectSize)
        {
            int midX = backgroundSize.Width / 2 - objectSize.Width / 2;
            int midY = backgroundSize.Height / 2 - objectSize.Height / 2;
            int right = backgroundSize.Width - objectSize.Width;
            int bottom = backgroundSize.Height - objectSize.Height;

            switch (placement)
            {
                default:
                case ContentAlignment.TopLeft:
                    return new Point(offset.X, offset.Y);
                case ContentAlignment.TopCenter:
                    return new Point(midX, offset.Y);
                case ContentAlignment.TopRight:
                    return new Point(right - offset.X, offset.Y);
                case ContentAlignment.MiddleLeft:
                    return new Point(offset.X, midY);
                case ContentAlignment.MiddleCenter:
                    return new Point(midX, midY);
                case ContentAlignment.MiddleRight:
                    return new Point(right - offset.X, midY);
                case ContentAlignment.BottomLeft:
                    return new Point(offset.X, bottom - offset.Y);
                case ContentAlignment.BottomCenter:
                    return new Point(midX, bottom - offset.Y);
                case ContentAlignment.BottomRight:
                    return new Point(right - offset.X, bottom - offset.Y);
            }
        }

        public static Size MeasureText(string text, Font font)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return g.MeasureString(text, font).ToSize();
            }
        }

        public static Size MeasureText(string text, Font font, int width)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return g.MeasureString(text, font, width).ToSize();
            }
        }

        public static string GetURLFilename(string url)
        {
            Uri uri = new Uri(url);

            try
            {
                return Path.GetFileName(uri.LocalPath);
            }
            catch { }

            return null;
        }
    }
}