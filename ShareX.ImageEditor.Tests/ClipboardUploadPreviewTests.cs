using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using SkiaSharp;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class ClipboardUploadPreviewTests
{
    [ClipboardPreviewTheory]
    [InlineData("text")]
    [InlineData("image-priority")]
    [InlineData("png-alpha")]
    [InlineData("close-before-transfer")]
    [InlineData("close-during-bitmap")]
    [InlineData("late-error")]
    public async Task ActualDialogOwnsItsAsynchronousSnapshot(string mode)
    {
        ProcessStartInfo start = new("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        start.ArgumentList.Add(typeof(NativeProcessFixture).Assembly.Location);
        start.ArgumentList.Add("--clipboard-upload-preview");
        start.ArgumentList.Add(mode);
        using Process child = Assert.IsType<Process>(Process.Start(start));
        Task<string> output = child.StandardOutput.ReadToEndAsync(), error = child.StandardError.ReadToEndAsync();
        try
        {
            await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
            string diagnostics = await output.WaitAsync(TimeSpan.FromSeconds(2)) + await error.WaitAsync(TimeSpan.FromSeconds(2));
            Assert.True(child.ExitCode == 0, diagnostics);
            Assert.Contains("clipboard preview passed: " + mode, diagnostics);
        }
        finally
        {
            if (!child.HasExited)
            {
                child.Kill(entireProcessTree: true);
                await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
    }

    internal static int RunFixture(string[] args)
    {
        try
        {
            string source = Path.GetFullPath(Environment.GetEnvironmentVariable("SHAREX_TEST_APPLICATION_DIRECTORY")!);
            AssemblyLoadContext.Default.Resolving += (_, name) =>
            {
                string path = Path.Combine(source, name.Name + ".dll");
                return File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : null;
            };
            Assembly app = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(source, "ShareX.dll"));
            Type type = app.GetType("ShareX.ClipboardUploadWindow", throwOnError: true)!;
            Type settings = app.GetType("ShareX.TaskSettings", throwOnError: true)!;
            AvaloniaBootstrapper.EnsureInitialized();
            DesktopServices.Run(async () =>
            {
                // Never show/focus this fixture or read the real clipboard. Exercise the actual
                // production dialog with a controlled provider and generated bitmap only.
                Window window = (Window)Activator.CreateInstance(type, Activator.CreateInstance(settings), false)!;
                Button upload = window.FindControl<Button>("UploadButton")!;
                Assert.False(upload.IsEnabled);
                MethodInfo load = type.GetMethod("LoadClipboardContentAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
                TaskCompletionSource<IAsyncDataTransfer?> read = new(TaskCreationOptions.RunContinuationsAsynchronously);
                Task loading = (Task)load.Invoke(window, [new Func<Task<IAsyncDataTransfer?>>(() => read.Task), PngFormat])!;
                Assert.False(loading.IsCompleted);
                Assert.False(upload.IsEnabled);
                await VerifyAsync(args.Single(), window, upload, loading, read);
                window.Close();
                AvaloniaBootstrapper.Shutdown();
                return true;
            });
            Console.WriteLine("clipboard preview passed: " + args.Single());
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }

    private static async Task VerifyAsync(string mode, Window window, Button upload, Task loading,
        TaskCompletionSource<IAsyncDataTransfer?> read)
    {
        const string text = "ShareX synthetic controlled clipboard";
        TaskCompletionSource<object?> bitmapRead = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using SKBitmap pixels = new(3, 2, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        pixels.Erase(new SKColor(17, 83, 211, mode == "png-alpha" ? (byte)79 : (byte)255));
        using SKImage image = SKImage.FromBitmap(pixels);
        using SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream stream = new(png.ToArray());
        Bitmap bitmap = new(stream);
        TestTransfer transfer = new(mode == "text" ? [DataFormat.Text] : mode == "png-alpha" ? [PngFormat, DataFormat.Bitmap, DataFormat.Text] : [DataFormat.Bitmap, DataFormat.Text],
            format => format == PngFormat ? Task.FromResult<object?>(png.ToArray()) : format == DataFormat.Bitmap ? bitmapRead.Task : Task.FromResult<object?>(text));
        try
        {
            if (mode is "close-before-transfer" or "late-error") window.Close();
            if (mode == "late-error") read.SetException(new IOException("Synthetic late clipboard failure."));
            else read.SetResult(transfer);
            if (mode == "close-during-bitmap")
            {
                await transfer.BitmapRequested.Task;
                window.Close();
            }
            bitmapRead.SetResult(bitmap);
            await loading;
            if (mode.StartsWith("close-", StringComparison.Ordinal) || mode == "late-error")
            {
                Assert.False(upload.IsEnabled);
                Assert.Null(window.GetType().GetField("_clipboardContent", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window));
                Assert.False(window.FindControl<TextBox>("TextPreview")!.IsVisible);
                Assert.False(window.FindControl<TextBlock>("EmptyPreview")!.IsVisible);
            }
            else
            {
                Assert.True(upload.IsEnabled);
                if (mode == "text") Assert.Equal(text, window.FindControl<TextBox>("TextPreview")!.Text);
                else
                {
                    Assert.True(window.FindControl<Border>("ImagePreviewContainer")!.IsVisible);
                    Assert.False(window.FindControl<TextBox>("TextPreview")!.IsVisible);
                    SKBitmap snapshot = Assert.IsType<SKBitmap>(window.GetType().GetField("_clipboardContent", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window));
                    using SKBitmap reference = SkiaImageHelpers.ByteArrayToBitmap(png.ToArray());
                    Assert.Equal(reference.GetPixel(0, 0), snapshot.GetPixel(0, 0));
                    if (mode == "png-alpha") Assert.False(transfer.BitmapRequested.Task.IsCompleted);
                    Assert.Equal(3, snapshot.Width);
                    Assert.Equal(2, snapshot.Height);
                    window.Close();
                    Assert.Equal(IntPtr.Zero, snapshot.Handle);
                }
            }
            Assert.Equal(mode == "late-error" ? 0 : 1, transfer.DisposeCount);
            if (mode is "image-priority" or "close-during-bitmap")
                Assert.Throws<ObjectDisposedException>(() => bitmap.Save(Stream.Null, PngBitmapEncoderOptions.Default));
        }
        finally { bitmap.Dispose(); }
    }

    private static readonly DataFormat<byte[]> PngFormat = DataFormat.CreateBytesPlatformFormat("image/png");

    private sealed class TestTransfer : IAsyncDataTransfer, IAsyncDataTransferItem
    {
        private readonly Func<DataFormat, Task<object?>> _read;
        public TestTransfer(DataFormat[] formats, Func<DataFormat, Task<object?>> read) { Formats = formats; _read = read; }
        public IReadOnlyList<DataFormat> Formats { get; }
        public IReadOnlyList<IAsyncDataTransferItem> Items => [this];
        public int DisposeCount { get; private set; }
        public TaskCompletionSource<bool> BitmapRequested { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task<object?> TryGetRawAsync(DataFormat format)
        {
            if (format == DataFormat.Bitmap) BitmapRequested.TrySetResult(true);
            return _read(format);
        }
        public void Dispose() => DisposeCount++;
    }
}

public sealed class ClipboardPreviewTheoryAttribute : TheoryAttribute
{
    public ClipboardPreviewTheoryAttribute()
    {
        if (Environment.GetEnvironmentVariable("SHAREX_TEST_DESKTOP") != "1" ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_APPLICATION_DIRECTORY")))
            Skip = "Set SHAREX_TEST_DESKTOP=1 and SHAREX_TEST_APPLICATION_DIRECTORY to the real app build for isolated clipboard dialog lifetime cases.";
    }
}
