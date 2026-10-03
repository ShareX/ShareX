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

using ShareX.Platform;
using ShareX.Platform.Windows;
using SkiaSharp;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using System.Security.Cryptography;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class WindowsSdkArtifactTests
{
    private const string OcrTypeName = "ShareX.Platform.Windows.WindowsOcrService";

    [WindowsSdkArtifactFact]
    public void WindowsApplicationGetsTheSdkAssemblyAndPortableHostsKeepTheirOwnAssembly()
    {
        string sdkAssembly = GetSdkAssembly();
        string applicationAssembly = Path.Combine(GetApplicationDirectory(), "ShareX.Platform.Windows.dll");
        string portableAssembly = typeof(WindowsPlatformServices).Assembly.Location;
        Assert.True(File.Exists(sdkAssembly), $"Build the Windows SDK platform target: {sdkAssembly}");
        Assert.True(File.Exists(applicationAssembly), $"Build the Windows application: {applicationAssembly}");
        Assert.False(ContainsOcrType(portableAssembly), "The net10.0 host must use the portable platform assembly.");
        Assert.True(ContainsOcrType(sdkAssembly), "The SDK target must contain Windows.Media.Ocr support.");
        Assert.True(ContainsOcrType(applicationAssembly), "The Windows application must retain its native OCR implementation.");
        Assert.Equal(SHA256.HashData(File.ReadAllBytes(sdkAssembly)), SHA256.HashData(File.ReadAllBytes(applicationAssembly)));
        Assert.NotEqual(SHA256.HashData(File.ReadAllBytes(portableAssembly)), SHA256.HashData(File.ReadAllBytes(sdkAssembly)));
    }

    [WindowsSdkArtifactFact]
    public async Task WindowsApplicationRecognizesSyntheticTextWithItsNativeOcrService()
    {
        if (!OperatingSystem.IsWindows()) return;
        NativeApplicationContext context = new(GetApplicationDirectory());
        try
        {
            Assembly assembly = context.LoadFromAssemblyPath(Path.Combine(GetApplicationDirectory(), "ShareX.Platform.Windows.dll"));
            Type platformType = assembly.GetType("ShareX.Platform.Windows.WindowsPlatformServices", throwOnError: true)!;
            using IPlatformServices services = (IPlatformServices)Activator.CreateInstance(platformType)!;
            Assert.Equal(OcrTypeName, services.Ocr.GetType().FullName);
            Assert.True(services.Ocr.Support.IsSupported);
            IReadOnlyList<OcrLanguage> languages = services.Ocr.GetLanguages();
            Assert.NotEmpty(languages);
            OcrLanguage language = languages.FirstOrDefault(value => value.Tag.StartsWith("en", StringComparison.OrdinalIgnoreCase)) ?? languages[0];
            byte[] image = RenderSyntheticText();
            Assert.Equal("SHAREX TEST" + Environment.NewLine + "123", (await services.Ocr.RecognizeAsync(image, language.Tag, false)).Trim());
            Assert.Equal("SHAREX TEST 123", (await services.Ocr.RecognizeAsync(image, language.Tag, true)).Trim());
        }
        finally
        {
            context.Unload();
        }
    }

    private static byte[] RenderSyntheticText()
    {
        using SKBitmap image = new(640, 220);
        using SKCanvas canvas = new(image);
        using SKTypeface typeface = SKTypeface.FromFamilyName("Arial");
        using SKFont font = new(typeface, 48);
        using SKPaint paint = new() { Color = SKColors.Black, IsAntialias = true };
        canvas.Clear(SKColors.White);
        canvas.DrawText("SHAREX TEST", 30, 80, font, paint);
        canvas.DrawText("123", 30, 170, font, paint);
        using SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
        return png.ToArray();
    }

    private static bool ContainsOcrType(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using PEReader reader = new(stream);
        MetadataReader metadata = reader.GetMetadataReader();
        return metadata.TypeDefinitions.Any(handle =>
        {
            TypeDefinition type = metadata.GetTypeDefinition(handle);
            return metadata.GetString(type.Namespace) + "." + metadata.GetString(type.Name) == OcrTypeName;
        });
    }

    private static string GetSdkAssembly() => Path.GetFullPath(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_SDK_PLATFORM_ASSEMBLY")!);
    private static string GetApplicationDirectory() => Path.GetFullPath(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY")!);

    private sealed class NativeApplicationContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public NativeApplicationContext(string directory) : base(isCollectible: true) =>
            resolver = new AssemblyDependencyResolver(Path.Combine(directory, "ShareX.dll"));

        protected override Assembly? Load(AssemblyName name)
        {
            // Use the test host's portable contracts so the SDK implementation can be exercised through them.
            if (name.Name == typeof(IPlatformServices).Assembly.GetName().Name) return typeof(IPlatformServices).Assembly;
            string? path = resolver.ResolveAssemblyToPath(name);
            return path == null ? null : LoadFromAssemblyPath(path);
        }

        protected override IntPtr LoadUnmanagedDll(string name)
        {
            string? path = resolver.ResolveUnmanagedDllToPath(name);
            return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}

public sealed class WindowsSdkArtifactFactAttribute : FactAttribute
{
    public WindowsSdkArtifactFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires the Windows application and its SDK platform target.";
        else if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_SDK_PLATFORM_ASSEMBLY")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY")))
            Skip = "Set SHAREX_TEST_WINDOWS_SDK_PLATFORM_ASSEMBLY and SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY after building the solution.";
    }
}
