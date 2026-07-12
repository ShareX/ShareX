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

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using System.Diagnostics;
using Vortice.DXGI;

namespace ShareX.Tools;

public sealed class BackgroundRemovalModel
{
    public required string FilePath { get; init; }
    public required string FileName { get; init; }
    public required long FileSize { get; init; }

    public string DisplayName => $"{FileName} ({FormatFileSize(FileSize)})";

    public override string ToString() => DisplayName;

    private static string FormatFileSize(long size)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double value = size;
        int unitIndex = 0;

        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return unitIndex == 0 ? $"{value:0} {units[unitIndex]}" : $"{value:0.#} {units[unitIndex]}";
    }
}

public enum BackgroundRemovalDevice
{
    Auto,
    GPU,
    CPU
}

public sealed record BackgroundRemovalResult(
    SKBitmap Image,
    bool IsSessionCached,
    string ExecutionDevice,
    long SessionSetupMilliseconds,
    long PreprocessingMilliseconds,
    long InferenceMilliseconds,
    long PostprocessingMilliseconds);

public sealed class BackgroundRemovalService : IDisposable
{
    private static readonly float[] Mean = [0.485f, 0.456f, 0.406f];
    private static readonly float[] StandardDeviation = [0.229f, 0.224f, 0.225f];
    private static readonly Lazy<DirectMLAdapter> PreferredDirectMLAdapter = new(FindPreferredDirectMLAdapter);
    private readonly Lock _sessionLock = new();
    private SessionCacheKey? _sessionKey;
    private InferenceSession? _session;
    private string? _executionDevice;
    private bool _isDisposed;

    public BackgroundRemovalResult RemoveBackground(SKBitmap source, BackgroundRemovalModel model, BackgroundRemovalDevice device)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(model);

        if (source.Width <= 0 || source.Height <= 0)
        {
            throw new InvalidOperationException("Selected image is empty.");
        }

        string modelPath = model.FilePath;
        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException("The background removal model file is missing.", modelPath);
        }

        lock (_sessionLock)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            Stopwatch sessionStopwatch = Stopwatch.StartNew();
            SessionLookupResult sessionResult = GetOrCreateSession(modelPath, device);
            sessionStopwatch.Stop();
            return RunModel(source, sessionResult, sessionStopwatch.ElapsedMilliseconds);
        }
    }

    private SessionLookupResult GetOrCreateSession(string modelPath, BackgroundRemovalDevice device)
    {
        FileInfo modelFile = new(modelPath);
        SessionCacheKey cacheKey = new(modelFile.FullName, modelFile.Length, modelFile.LastWriteTimeUtc.Ticks, device);

        if (_sessionKey == cacheKey && _session != null && _executionDevice != null)
        {
            return new SessionLookupResult(_session, true, _executionDevice);
        }

        _session?.Dispose();
        SessionCreationResult creationResult = CreateSession(modelPath, device);
        _session = creationResult.Session;
        _executionDevice = creationResult.ExecutionDevice;
        _sessionKey = cacheKey;
        return new SessionLookupResult(_session, false, _executionDevice);
    }

    private static SessionCreationResult CreateSession(string modelPath, BackgroundRemovalDevice device)
    {
        if (device == BackgroundRemovalDevice.CPU)
        {
            return new SessionCreationResult(new InferenceSession(modelPath), "CPU");
        }

        if (device == BackgroundRemovalDevice.GPU)
        {
            return CreateDirectMLSession(modelPath);
        }

        try
        {
            return CreateDirectMLSession(modelPath);
        }
        catch (Exception ex) when (ex is OnnxRuntimeException or DllNotFoundException or EntryPointNotFoundException or NotSupportedException)
        {
            return new SessionCreationResult(new InferenceSession(modelPath), "CPU (GPU unavailable)");
        }
    }

    private static SessionCreationResult CreateDirectMLSession(string modelPath)
    {
        DirectMLAdapter adapter = PreferredDirectMLAdapter.Value;
        using SessionOptions sessionOptions = new()
        {
            EnableMemoryPattern = false,
            ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
        };

        sessionOptions.AppendExecutionProvider_DML(adapter.DeviceId);
        return new SessionCreationResult(new InferenceSession(modelPath, sessionOptions), adapter.Name);
    }

    private static BackgroundRemovalResult RunModel(SKBitmap source, SessionLookupResult sessionResult, long sessionSetupMilliseconds)
    {
        InferenceSession session = sessionResult.Session;
        string inputName = session.InputMetadata.Keys.FirstOrDefault()
            ?? throw new InvalidOperationException("The ONNX model does not expose an input tensor.");

        int inputWidth;
        int inputHeight;
        bool channelsLast;
        ResolveInputShape(session.InputMetadata[inputName].Dimensions, out inputWidth, out inputHeight, out channelsLast);

        Stopwatch phaseStopwatch = Stopwatch.StartNew();
        using SKBitmap inputBitmap = ResizeBitmap(source, inputWidth, inputHeight);
        DenseTensor<float> inputTensor = CreateInputTensor(inputBitmap, channelsLast);
        phaseStopwatch.Stop();
        long preprocessingMilliseconds = phaseStopwatch.ElapsedMilliseconds;

        phaseStopwatch.Restart();
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = session.Run(
        [
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        ]);

        Tensor<float> outputTensor = outputs.FirstOrDefault()?.AsTensor<float>()
            ?? throw new InvalidOperationException("The ONNX model did not return a usable output tensor.");
        phaseStopwatch.Stop();
        long inferenceMilliseconds = phaseStopwatch.ElapsedMilliseconds;

        phaseStopwatch.Restart();
        using SKBitmap modelMask = CreateMaskBitmap(outputTensor);
        using SKBitmap resizedMask = ResizeBitmap(modelMask, source.Width, source.Height);
        SKBitmap result = ApplyAlphaMask(source, resizedMask);
        phaseStopwatch.Stop();

        return new BackgroundRemovalResult(
            result,
            sessionResult.IsCached,
            sessionResult.ExecutionDevice,
            sessionSetupMilliseconds,
            preprocessingMilliseconds,
            inferenceMilliseconds,
            phaseStopwatch.ElapsedMilliseconds);
    }

    private static DirectMLAdapter FindPreferredDirectMLAdapter()
    {
        try
        {
            using IDXGIFactory1 factory = Vortice.DXGI.DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            DirectMLAdapter? preferredAdapter = null;
            ulong largestDedicatedMemory = 0;

            for (uint index = 0; ; index++)
            {
                if (factory.EnumAdapters1(index, out IDXGIAdapter1 adapter).Failure)
                {
                    break;
                }

                using (adapter)
                {
                    AdapterDescription1 description = adapter.Description1;
                    if ((description.Flags & AdapterFlags.Software) != 0)
                    {
                        continue;
                    }

                    ulong dedicatedMemory = description.DedicatedVideoMemory;
                    if (preferredAdapter == null || dedicatedMemory > largestDedicatedMemory)
                    {
                        preferredAdapter = new DirectMLAdapter((int)index, description.Description);
                        largestDedicatedMemory = dedicatedMemory;
                    }
                }
            }

            return preferredAdapter ?? new DirectMLAdapter(0, "GPU 0");
        }
        catch
        {
            return new DirectMLAdapter(0, "GPU 0");
        }
    }

    public void Dispose()
    {
        lock (_sessionLock)
        {
            if (_isDisposed)
            {
                return;
            }

            _session?.Dispose();
            _session = null;
            _sessionKey = null;
            _executionDevice = null;
            _isDisposed = true;
        }
    }

    private readonly record struct SessionLookupResult(InferenceSession Session, bool IsCached, string ExecutionDevice);

    private readonly record struct SessionCreationResult(InferenceSession Session, string ExecutionDevice);

    private readonly record struct DirectMLAdapter(int DeviceId, string Name);

    private readonly record struct SessionCacheKey(
        string ModelPath,
        long FileSize,
        long LastWriteTimeUtcTicks,
        BackgroundRemovalDevice Device);

    private static void ResolveInputShape(IReadOnlyList<int> dimensions, out int width, out int height, out bool channelsLast)
    {
        width = 1024;
        height = 1024;
        channelsLast = false;

        if (dimensions.Count != 4)
        {
            return;
        }

        int channelIndex = dimensions[1] == 3 ? 1 : dimensions[3] == 3 ? 3 : 1;
        channelsLast = channelIndex == 3;

        int heightIndex = channelsLast ? 1 : 2;
        int widthIndex = channelsLast ? 2 : 3;

        if (dimensions[heightIndex] > 0)
        {
            height = dimensions[heightIndex];
        }

        if (dimensions[widthIndex] > 0)
        {
            width = dimensions[widthIndex];
        }
    }

    private static unsafe DenseTensor<float> CreateInputTensor(SKBitmap bitmap, bool channelsLast)
    {
        DenseTensor<float> tensor = channelsLast
            ? new DenseTensor<float>([1, bitmap.Height, bitmap.Width, 3])
            : new DenseTensor<float>([1, 3, bitmap.Height, bitmap.Width]);
        Span<float> tensorValues = tensor.Buffer.Span;
        int pixelCount = bitmap.Width * bitmap.Height;
        byte* pixels = (byte*)bitmap.GetPixels();

        for (int y = 0; y < bitmap.Height; y++)
        {
            byte* row = pixels + (y * bitmap.RowBytes);

            for (int x = 0; x < bitmap.Width; x++)
            {
                byte* pixel = row + (x * 4);
                float alphaScale = pixel[3] is > 0 and < 255 ? 255f / pixel[3] : 1f;
                float r = ((Math.Min(255f, pixel[2] * alphaScale) / 255f) - Mean[0]) / StandardDeviation[0];
                float g = ((Math.Min(255f, pixel[1] * alphaScale) / 255f) - Mean[1]) / StandardDeviation[1];
                float b = ((Math.Min(255f, pixel[0] * alphaScale) / 255f) - Mean[2]) / StandardDeviation[2];
                int pixelIndex = (y * bitmap.Width) + x;

                if (channelsLast)
                {
                    int tensorIndex = pixelIndex * 3;
                    tensorValues[tensorIndex] = r;
                    tensorValues[tensorIndex + 1] = g;
                    tensorValues[tensorIndex + 2] = b;
                }
                else
                {
                    tensorValues[pixelIndex] = r;
                    tensorValues[pixelCount + pixelIndex] = g;
                    tensorValues[(pixelCount * 2) + pixelIndex] = b;
                }
            }
        }

        return tensor;
    }

    private static unsafe SKBitmap CreateMaskBitmap(Tensor<float> outputTensor)
    {
        ResolveOutputShape(outputTensor.Dimensions, out int width, out int height);

        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException("The ONNX model returned an invalid mask size.");
        }

        float[] values = outputTensor.ToArray();
        int pixelCount = width * height;

        if (values.Length < pixelCount)
        {
            throw new InvalidOperationException("The ONNX model returned an incomplete mask.");
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < pixelCount; i++)
        {
            float value = values[i];
            if (value < min) min = value;
            if (value > max) max = value;
        }

        if (min < 0f)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < pixelCount; i++)
            {
                float value = 1f / (1f + MathF.Exp(-values[i]));
                values[i] = value;
                if (value < min) min = value;
                if (value > max) max = value;
            }
        }

        float range = Math.Max(0.000001f, max - min);
        SKBitmap mask = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        byte* maskPixels = (byte*)mask.GetPixels();

        for (int y = 0; y < height; y++)
        {
            byte* row = maskPixels + (y * mask.RowBytes);

            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;
                float normalized = Math.Clamp((values[index] - min) / range, 0f, 1f);
                byte alpha = (byte)MathF.Round(normalized * 255f);
                byte* pixel = row + (x * 4);
                pixel[0] = 0;
                pixel[1] = 0;
                pixel[2] = 0;
                pixel[3] = alpha;
            }
        }

        return mask;
    }

    private static void ResolveOutputShape(ReadOnlySpan<int> dimensions, out int width, out int height)
    {
        width = 1;
        height = 1;

        if (dimensions.Length >= 4)
        {
            bool channelsLast = dimensions[^1] == 1 && dimensions[^3] > 1;
            height = channelsLast ? dimensions[^3] : dimensions[^2];
            width = channelsLast ? dimensions[^2] : dimensions[^1];
        }
        else if (dimensions.Length >= 2)
        {
            height = dimensions[^2];
            width = dimensions[^1];
        }
    }

    private static SKBitmap ApplyAlphaMask(SKBitmap source, SKBitmap mask)
    {
        SKBitmap output = new(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul));

        using SKCanvas canvas = new(output);
        using SKImage sourceImage = SKImage.FromBitmap(source);
        using SKImage maskImage = SKImage.FromBitmap(mask);
        using SKPaint maskPaint = new() { BlendMode = SKBlendMode.DstIn };
        canvas.Clear(SKColors.Transparent);
        canvas.DrawImage(sourceImage, 0, 0);
        canvas.DrawImage(maskImage, 0, 0, maskPaint);

        return output;
    }

    private static SKBitmap ResizeBitmap(SKBitmap source, int width, int height)
    {
        if (source.Width == width && source.Height == height &&
            source.ColorType == SKColorType.Bgra8888 &&
            source.AlphaType is SKAlphaType.Premul or SKAlphaType.Opaque)
        {
            return source.Copy();
        }

        SKBitmap resized = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));

        using SKCanvas canvas = new(resized);
        using SKPaint paint = new() { IsAntialias = true };
        using SKImage sourceImage = SKImage.FromBitmap(source);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawImage(sourceImage, new SKRect(0, 0, width, height), new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);

        return resized;
    }
}
