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
    // A Windows Runtime type that only the Windows SDK target compiles.
    private const string SdkOnlyTypeName = "ShareX.Platform.Windows.WindowsNativeScreenRecorderService";

    [WindowsSdkArtifactFact]
    public void WindowsApplicationGetsTheSdkAssemblyAndPortableHostsKeepTheirOwnAssembly()
    {
        string sdkAssembly = GetSdkAssembly();
        string applicationAssembly = Path.Combine(GetApplicationDirectory(), "ShareX.Platform.Windows.dll");
        string portableAssembly = typeof(WindowsPlatformServices).Assembly.Location;
        Assert.True(File.Exists(sdkAssembly), $"Build the Windows SDK platform target: {sdkAssembly}");
        Assert.True(File.Exists(applicationAssembly), $"Build the Windows application: {applicationAssembly}");
        Assert.False(ContainsSdkOnlyType(portableAssembly), "The net10.0 host must use the portable platform assembly.");
        Assert.True(ContainsSdkOnlyType(sdkAssembly), "The SDK target must contain the Windows Runtime services.");
        Assert.True(ContainsSdkOnlyType(applicationAssembly), "The Windows application must retain its Windows Runtime services.");
        Assert.Equal(SHA256.HashData(File.ReadAllBytes(sdkAssembly)), SHA256.HashData(File.ReadAllBytes(applicationAssembly)));
        Assert.NotEqual(SHA256.HashData(File.ReadAllBytes(portableAssembly)), SHA256.HashData(File.ReadAllBytes(sdkAssembly)));
    }

    private static bool ContainsSdkOnlyType(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using PEReader reader = new(stream);
        MetadataReader metadata = reader.GetMetadataReader();
        return metadata.TypeDefinitions.Any(handle =>
        {
            TypeDefinition type = metadata.GetTypeDefinition(handle);
            return metadata.GetString(type.Namespace) + "." + metadata.GetString(type.Name) == SdkOnlyTypeName;
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
