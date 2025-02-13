#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace ShareX.HelpersLib
{
    public static class Helpers
    {
        public const string Numbers = "0123456789"; // 48 ... 57
        public const string AlphabetCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 65 ... 90
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz"; // 97 ... 122
        public const string Alphanumeric = Numbers + AlphabetCapital + Alphabet;
        public const string AlphanumericInverse = Numbers + Alphabet + AlphabetCapital;
        public const string Hexadecimal = Numbers + "ABCDEF";
        public const string Base58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"; // https://en.wikipedia.org/wiki/Base58
        public const string Base56 = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz"; // A variant, Base56, excludes 1 (one) and o (lowercase o) compared to Base 58.

        public static readonly Version OSVersion = Environment.OSVersion.Version;

        private static Cursor[] cursorList;

        public static Cursor[] CursorList
        {
            get
            {
                if (cursorList == null)
                {
                    cursorList = new Cursor[] {
                        Cursors.AppStarting, Cursors.Arrow, Cursors.Cross, Cursors.Default, Cursors.Hand, Cursors.Help,
                        Cursors.HSplit, Cursors.IBeam, Cursors.No, Cursors.NoMove2D, Cursors.NoMoveHoriz, Cursors.NoMoveVert,
                        Cursors.PanEast, Cursors.PanNE, Cursors.PanNorth, Cursors.PanNW, Cursors.PanSE, Cursors.PanSouth,
                        Cursors.PanSW, Cursors.PanWest, Cursors.SizeAll, Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE,
                        Cursors.SizeWE, Cursors.UpArrow, Cursors.VSplit, Cursors.WaitCursor
                    };
                }

                return cursorList;
            }
        }

        public static string AddZeroes(string input, int digits = 2)
        {
            return input.PadLeft(digits, '0');
        }

        public static string AddZeroes(int number, int digits = 2)
        {
            return AddZeroes(number.ToString(), digits);
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
            return chars[RandomCrypto.Next(chars.Length - 1)];
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
            return Enumerable.Range(1, ((length + 1) * count) - 1).Aggregate("", (x, index) => x += index % (length + 1) == 0 ? separator : GetRandomChar(Alphanumeric));
        }

        public static string GetAllCharacters()
        {
            return Encoding.UTF8.GetString(Enumerable.Range(1, 255).Select(i => (byte)i).ToArray());
        }

        public static string GetRandomLine(string text)
        {
            string[] lines = text.Trim().Lines();

            if (lines != null && lines.Length > 0)
            {
                return RandomCrypto.Pick(lines);
            }

            return null;
        }

        public static string GetRandomLineFromFile(string filePath)
        {
            string text = File.ReadAllText(filePath, Encoding.UTF8);
            return GetRandomLine(text);
        }

        public static string GetValidURL(string url, bool replaceSpace = false)
        {
            if (replaceSpace) url = url.Replace(' ', '_');
            return HttpUtility.UrlPathEncode(url);
        }

        public static string GetXMLValue(string input, string tag)
        {
            return Regex.Match(input, string.Format("(?<={0}>).+?(?=</{0})", tag)).Value;
        }

        public static T[] GetEnums<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static string[] GetEnumDescriptions<T>(int skip = 0)
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>().Skip(skip).Select(x => x.GetDescription()).ToArray();
        }

        /*public static string[] GetLocalizedEnumDescriptions<T>()
        {
            Assembly assembly = typeof(T).Assembly;
            string resourcePath = assembly.GetName().Name + ".Properties.Resources";
            ResourceManager resourceManager = new ResourceManager(resourcePath, assembly);
            return GetLocalizedEnumDescriptions<T>(resourceManager);
        }*/

        public static string[] GetLocalizedEnumDescriptions<T>()
        {
            return GetLocalizedEnumDescriptions<T>(Resources.ResourceManager);
        }

        public static string[] GetLocalizedEnumDescriptions<T>(ResourceManager resourceManager)
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>().Select(x => x.GetLocalizedDescription(resourceManager)).ToArray();
        }

        public static int GetEnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        public static T GetEnumFromIndex<T>(int i)
        {
            return GetEnums<T>()[i];
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

        // returns a list of public static fields of the class type (similar to enum values)
        public static T[] GetValueFields<T>()
        {
            List<T> res = new List<T>();
            foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fi.FieldType != typeof(T)) continue;
                res.Add((T)fi.GetValue(null));
            }
            return res.ToArray();
        }

        // Example: "TopLeft" becomes "Top left"
        // Example2: "Rotate180" becomes "Rotate 180"
        public static string GetProperName(string name, bool keepCase = false)
        {
            StringBuilder sb = new StringBuilder();

            bool number = false;

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (i > 0 && (char.IsUpper(c) || (!number && char.IsNumber(c))))
                {
                    sb.Append(' ');

                    if (keepCase)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(char.ToLowerInvariant(c));
                    }
                }
                else
                {
                    sb.Append(c);
                }

                number = char.IsNumber(c);
            }

            return sb.ToString();
        }

        public static string GetApplicationVersion(bool includeRevision = false)
        {
            Version version = Version.Parse(Application.ProductVersion);
            string result = $"{version.Major}.{version.Minor}.{version.Build}";
            if (includeRevision)
            {
                result = $"{result}.{version.Revision}";
            }
            return result;
        }

        /// <summary>
        /// If version1 newer than version2 = 1
        /// If version1 equal to version2 = 0
        /// If version1 older than version2 = -1
        /// </summary>
        public static int CompareVersion(string version1, string version2, bool ignoreRevision = false)
        {
            return NormalizeVersion(version1, ignoreRevision).CompareTo(NormalizeVersion(version2, ignoreRevision));
        }

        /// <summary>
        /// If version1 newer than version2 = 1
        /// If version1 equal to version2 = 0
        /// If version1 older than version2 = -1
        /// </summary>
        public static int CompareVersion(Version version1, Version version2, bool ignoreRevision = false)
        {
            return version1.Normalize(ignoreRevision).CompareTo(version2.Normalize(ignoreRevision));
        }

        /// <summary>
        /// If version newer than ApplicationVersion = 1
        /// If version equal to ApplicationVersion = 0
        /// If version older than ApplicationVersion = -1
        /// </summary>
        public static int CompareApplicationVersion(string version, bool includeRevision = false)
        {
            return CompareVersion(version, GetApplicationVersion(includeRevision));
        }

        public static Version NormalizeVersion(string version, bool ignoreRevision = false)
        {
            return Version.Parse(version).Normalize(ignoreRevision);
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

        public static bool IsWindows10OrGreater(int build = -1)
        {
            return OSVersion.Major >= 10 && OSVersion.Build >= build;
        }

        public static bool IsWindows11OrGreater(int build = -1)
        {
            build = Math.Max(22000, build);
            return OSVersion.Major >= 10 && OSVersion.Build >= build;
        }

        public static bool IsDefaultInstallDir()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Application.ExecutablePath.StartsWith(path);
        }

        public static bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;

            string pattern = @"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)";

            return Regex.IsMatch(ip.Trim(), pattern);
        }

        public static string ProperTimeSpan(TimeSpan ts)
        {
            string time = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            int hours = (int)ts.TotalHours;
            if (hours > 0) time = hours + ":" + time;
            return time;
        }

        public static void PlaySoundAsync(Stream stream)
        {
            if (stream != null)
            {
                Task.Run(() =>
                {
                    using (stream)
                    using (SoundPlayer soundPlayer = new SoundPlayer(stream))
                    {
                        soundPlayer.PlaySync();
                    }
                });
            }
        }

        public static void PlaySoundAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Task.Run(() =>
                {
                    using (SoundPlayer soundPlayer = new SoundPlayer(filePath))
                    {
                        soundPlayer.PlaySync();
                    }
                });
            }
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

        public static async Task WaitWhileAsync(Func<bool> check, int interval, int timeout, Action onSuccess, int waitStart = 0)
        {
            bool result = false;

            await Task.Run(() =>
            {
                if (waitStart > 0)
                {
                    Thread.Sleep(waitStart);
                }

                result = WaitWhile(check, interval, timeout);
            });

            if (result) onSuccess();
        }

        public static string GetUniqueID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static Point GetPosition(ContentAlignment placement, int offset, Size backgroundSize, Size objectSize)
        {
            return GetPosition(placement, new Point(offset, offset), backgroundSize, objectSize);
        }

        public static Point GetPosition(ContentAlignment placement, int offset, Rectangle background, Size objectSize)
        {
            return GetPosition(placement, new Point(offset, offset), background, objectSize);
        }

        public static Point GetPosition(ContentAlignment placement, Point offset, Rectangle background, Size objectSize)
        {
            Point position = GetPosition(placement, offset, background.Size, objectSize);

            return new Point(background.X + position.X, background.Y + position.Y);
        }

        public static Point GetPosition(ContentAlignment placement, Point offset, Size backgroundSize, Size objectSize)
        {
            int midX = (int)Math.Round((backgroundSize.Width / 2f) - (objectSize.Width / 2f));
            int midY = (int)Math.Round((backgroundSize.Height / 2f) - (objectSize.Height / 2f));
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

        public static string SendPing(string host)
        {
            return SendPing(host, 1);
        }

        public static string SendPing(string host, int count)
        {
            string[] status = new string[count];

            using (Ping ping = new Ping())
            {
                PingReply reply;
                //byte[] buffer = Encoding.ASCII.GetBytes(new string('a', 32));
                for (int i = 0; i < count; i++)
                {
                    reply = ping.Send(host, 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        status[i] = reply.RoundtripTime.ToString() + " ms";
                    }
                    else
                    {
                        status[i] = "Timeout";
                    }
                    Thread.Sleep(100);
                }
            }

            return string.Join(", ", status);
        }

        public static void SetDefaultUICulture(CultureInfo culture)
        {
            Type type = typeof(CultureInfo);

            try
            {
                // .NET 4.0
                type.InvokeMember("s_userDefaultUICulture", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static, null, culture, new object[] { culture });
            }
            catch
            {
                try
                {
                    // .NET 2.0
                    type.InvokeMember("m_userDefaultUICulture", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static, null, culture, new object[] { culture });
                }
                catch
                {
                    DebugHelper.WriteLine("SetDefaultUICulture failed: " + culture.DisplayName);
                }
            }
        }

        public static bool IsAdministrator()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool IsMemberOfAdministratorsGroup()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                    return principal.UserClaims.Any(x => x.Value.Contains(sid.Value));
                }
            }
            catch
            {
            }

            return false;
        }

        public static string RepeatGenerator(int count, Func<string> generator)
        {
            string result = "";
            for (int x = count; x > 0; x--)
            {
                result += generator();
            }
            return result;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static IEnumerable<T> GetInstances<T>() where T : class
        {
            Type baseType = typeof(T);
            Assembly assembly = baseType.Assembly;
            return assembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(baseType) && t.GetConstructor(Type.EmptyTypes) != null).
                Select(t => Activator.CreateInstance(t) as T);
        }

        public static IEnumerable<Type> FindSubclassesOf<T>()
        {
            Type baseType = typeof(T);
            Assembly assembly = baseType.Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public static string GetOperatingSystemProductName(bool includeBit = false)
        {
            string productName = RegistryHelpers.GetValueString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", RegistryHive.LocalMachine);

            if (string.IsNullOrEmpty(productName))
            {
                productName = Environment.OSVersion.VersionString;
            }

            if (includeBit)
            {
                string bit;

                if (Environment.Is64BitOperatingSystem)
                {
                    bit = "64";
                }
                else
                {
                    bit = "32";
                }

                productName = $"{productName} ({bit}-bit)";
            }

            return productName;
        }

        public static Cursor CreateCursor(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return new Cursor(ms);
            }
        }

        public static string EscapeCLIText(string text)
        {
            string escapedText = text.Replace("\\", "\\\\").Replace("\"", "\\\"");
            return $"\"{escapedText}\"";
        }

        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in bytes)
            {
                sb.Append(string.Format("{0:x2}", x));
            }
            return sb.ToString();
        }

        public static byte[] ComputeSHA256(byte[] data)
        {
            using (SHA256Managed hashAlgorithm = new SHA256Managed())
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }

        public static byte[] ComputeSHA256(Stream stream, int bufferSize = 1024 * 32)
        {
            BufferedStream bufferedStream = new BufferedStream(stream, bufferSize);

            using (SHA256Managed hashAlgorithm = new SHA256Managed())
            {
                return hashAlgorithm.ComputeHash(bufferedStream);
            }
        }

        public static byte[] ComputeSHA256(string data)
        {
            return ComputeSHA256(Encoding.UTF8.GetBytes(data));
        }

        public static byte[] ComputeHMACSHA256(byte[] data, byte[] key)
        {
            using (HMACSHA256 hashAlgorithm = new HMACSHA256(key))
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }

        public static byte[] ComputeHMACSHA256(string data, string key)
        {
            return ComputeHMACSHA256(Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(key));
        }

        public static byte[] ComputeHMACSHA256(byte[] data, string key)
        {
            return ComputeHMACSHA256(data, Encoding.UTF8.GetBytes(key));
        }

        public static byte[] ComputeHMACSHA256(string data, byte[] key)
        {
            return ComputeHMACSHA256(Encoding.UTF8.GetBytes(data), key);
        }

        public static string SafeStringFormat(string format, params object[] args)
        {
            return SafeStringFormat(null, format, args);
        }

        public static string SafeStringFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                if (provider != null)
                {
                    return string.Format(provider, format, args);
                }

                return string.Format(format, args);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return format;
        }

        public static string NumberToLetters(int num)
        {
            string result = "";
            while (--num >= 0)
            {
                result = (char)('A' + (num % 26)) + result;
                num /= 26;
            }
            return result;
        }

        private static string GetNextRomanNumeralStep(ref int num, int step, string numeral)
        {
            string result = "";
            if (num >= step)
            {
                result = numeral.Repeat(num / step);
                num %= step;
            }
            return result;
        }

        public static string NumberToRomanNumeral(int num)
        {
            string result = "";
            result += GetNextRomanNumeralStep(ref num, 1000, "M");
            result += GetNextRomanNumeralStep(ref num, 900, "CM");
            result += GetNextRomanNumeralStep(ref num, 500, "D");
            result += GetNextRomanNumeralStep(ref num, 400, "CD");
            result += GetNextRomanNumeralStep(ref num, 100, "C");
            result += GetNextRomanNumeralStep(ref num, 90, "XC");
            result += GetNextRomanNumeralStep(ref num, 50, "L");
            result += GetNextRomanNumeralStep(ref num, 40, "XL");
            result += GetNextRomanNumeralStep(ref num, 10, "X");
            result += GetNextRomanNumeralStep(ref num, 9, "IX");
            result += GetNextRomanNumeralStep(ref num, 5, "V");
            result += GetNextRomanNumeralStep(ref num, 4, "IV");
            result += GetNextRomanNumeralStep(ref num, 1, "I");
            return result;
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static bool TryFixHandCursor()
        {
            try
            {
                // https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Cursors.cs,423
                typeof(Cursors).GetField("hand", BindingFlags.NonPublic | BindingFlags.Static)
                    .SetValue(null, new Cursor(NativeMethods.LoadCursor(IntPtr.Zero, NativeConstants.IDC_HAND)));

                return true;
            }
            catch
            {
                // If it fails, we'll just have to live with the old hand.
                return false;
            }
        }

        public static bool IsTabletMode()
        {
            //int state = NativeMethods.GetSystemMetrics(SystemMetric.SM_CONVERTIBLESLATEMODE);
            //return state == 0;

            try
            {
                int result = (int)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ImmersiveShell", "TabletMode", 0);
                return result > 0;
            }
            catch
            {
            }

            return false;
        }

        public static string JSONFormat(string json, Newtonsoft.Json.Formatting formatting)
        {
            return JToken.Parse(json).ToString(formatting);
        }

        public static string XMLFormat(string xml)
        {
            using (MemoryStream ms = new MemoryStream())
            using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;

                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                document.WriteContentTo(writer);
                writer.Flush();
                ms.Flush();
                ms.Position = 0;
                StreamReader sReader = new StreamReader(ms);
                return sReader.ReadToEnd();
            }
        }

        public static Icon GetProgressIcon(int percentage)
        {
            return GetProgressIcon(percentage, Color.FromArgb(16, 116, 193));
        }

        public static Icon GetProgressIcon(int percentage, Color color)
        {
            percentage = percentage.Clamp(0, 100);

            Size size = SystemInformation.SmallIconSize;

            using (Bitmap bmp = new Bitmap(size.Width, size.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(39, 39, 39)))
                {
                    g.FillRectangle(brush, 0, 0, size.Width, size.Height);
                }

                int y = (int)(size.Height * (percentage / 100f));

                if (y > 0)
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, 0, size.Height - y, size.Width, y);
                    }

                    if (y < size.Height)
                    {
                        using (Pen pen = new Pen(ColorHelpers.LighterColor(color, 0.3f)))
                        {
                            g.DrawLine(pen, 0, size.Height - y, size.Width - 1, size.Height - y);
                        }
                    }
                }

                using (Font font = new Font("Arial", 10))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    percentage = percentage.Clamp(0, 99);

                    g.DrawString(percentage.ToString(), font, Brushes.White, size.Width / 2f, size.Height / 2f, sf);
                }

                bmp.SetPixel(0, 0, Color.Transparent);
                bmp.SetPixel(bmp.Width - 1, 0, Color.Transparent);
                bmp.SetPixel(0, bmp.Height - 1, Color.Transparent);
                bmp.SetPixel(bmp.Width - 1, bmp.Height - 1, Color.Transparent);

                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        public static string GetChecksum(string filePath)
        {
            using (SHA256Managed hashAlgorithm = new SHA256Managed())
            {
                return GetChecksum(filePath, hashAlgorithm);
            }
        }

        public static string GetChecksum(string filePath, HashAlgorithm hashAlgorithm)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                byte[] hash = hashAlgorithm.ComputeHash(fs);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public static string CreateChecksumFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string outputFilePath = filePath + ".sha256";
                string checksum = GetChecksum(filePath);
                string fileName = Path.GetFileName(filePath);
                string content = $"{checksum}  {fileName}";

                File.WriteAllText(outputFilePath, content);

                return outputFilePath;
            }

            return null;
        }

        public static Task ForEachAsync<T>(IEnumerable<T> inputEnumerable, Func<T, Task> asyncProcessor, int maxDegreeOfParallelism)
        {
            SemaphoreSlim throttler = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);

            IEnumerable<Task> tasks = inputEnumerable.Select(async input =>
            {
                await throttler.WaitAsync();

                try
                {
                    await asyncProcessor(input);
                }
                finally
                {
                    throttler.Release();
                }
            });

            return Task.WhenAll(tasks);
        }

        public static void LockCursorToWindow(Form form)
        {
            form.Activated += (sender, e) => Cursor.Clip = form.Bounds;
            form.Deactivate += (sender, e) => Cursor.Clip = Rectangle.Empty;
        }

        public static bool IsDefaultSettings<T>(IEnumerable<T> current, IEnumerable<T> source, Func<T, T, bool> predicate)
        {
            if (current != null && current.Count() > 0)
            {
                return current.All(x => source.Any(y => predicate(x, y)));
            }

            return true;
        }

        public static string GetDesktopWallpaperFilePath()
        {
            byte[] transcodedImageCache = (byte[])RegistryHelpers.GetValue(@"Control Panel\Desktop", "TranscodedImageCache");
            byte[] transcodedImageCacheDest = new byte[transcodedImageCache.Length - 24];
            Array.Copy(transcodedImageCache, 24, transcodedImageCacheDest, 0, transcodedImageCacheDest.Length);
            string wallpaperFilePath = Encoding.Unicode.GetString(transcodedImageCacheDest);
            return wallpaperFilePath.TrimEnd('\0');
        }

        public static IEnumerable<int> Range(int from, int to, int increment = 1)
        {
            if (increment == 0)
            {
                throw new ArgumentException("Increment cannot be zero.", nameof(increment));
            }

            if (from == to)
            {
                yield return from;
                yield break;
            }

            increment = Math.Abs(increment);

            if (from < to)
            {
                for (int i = from; i <= to; i += increment)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = from; i >= to; i -= increment)
                {
                    yield return i;
                }
            }
        }
    }
}