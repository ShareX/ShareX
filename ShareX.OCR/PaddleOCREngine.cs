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
using SkiaSharp;
using System.Runtime.InteropServices;

namespace ShareX.OCR;

// Offline OCR with the PaddleOCR PP-OCRv6 models: a detection model finds the text lines and a recognition model
// reads each one. The models are multilingual, so there is no language to choose.
public sealed class PaddleOCREngine : IDisposable
{
    // Models in the order they are preferred, the biggest and most accurate first. The small models ship with ShareX;
    // a user who downloads the medium models into the models folder gets them instead. The tiny models are used only
    // when the small ones have been removed. Detection and recognition are picked separately, so either one can be
    // replaced on its own.
    private static readonly string[] DetectionModelNames = ["PP-OCRv6_medium_det", "PP-OCRv6_small_det", "PP-OCRv6_tiny_det"];
    private static readonly string[] RecognitionModelNames = ["PP-OCRv6_medium_rec", "PP-OCRv6_small_rec", "PP-OCRv6_tiny_rec"];
    private const string BuiltInDetectionModelName = "PP-OCRv6_small_det";
    private const string BuiltInRecognitionModelName = "PP-OCRv6_small_rec";

    // Hugging Face names the ONNX downloads with this suffix, so a folder copied from there as-is is found too.
    private const string DownloadFolderSuffix = "_onnx";

    // Recognized text below this confidence is usually an icon or noise read as characters.
    public const float DefaultMinScore = 0.5f;

    private static readonly Lock SharedLock = new();
    private static PaddleOCREngine? _shared;

    private readonly TextDetector _detector;
    private readonly TextRecognizer _recognizer;

    // The folders of the models in use.
    public string DetectionModelFolder { get; }
    public string RecognitionModelFolder { get; }

    // The models are copied next to the application under this folder.
    public static string DefaultModelsFolder => Path.Combine(AppContext.BaseDirectory, "Models");

    private static readonly Lazy<bool> Available = new(() =>
        FindModelFolder(DefaultModelsFolder, DetectionModelNames) != null &&
        FindModelFolder(DefaultModelsFolder, RecognitionModelNames) != null &&
        NativeLibrary.TryLoad("onnxruntime", typeof(InferenceSession).Assembly, null, out _));

    // Whether the models and the ONNX runtime are here. The runtime has no Intel Mac (osx-x64) build.
    public static bool IsAvailable => Available.Value;

    // Loading the models takes a moment, so one engine is kept for the life of the process.
    public static PaddleOCREngine Shared
    {
        get
        {
            lock (SharedLock)
            {
                return _shared ??= new PaddleOCREngine(DefaultModelsFolder);
            }
        }
    }

    public PaddleOCREngine(string modelsFolder)
    {
        using SessionOptions sessionOptions = new()
        {
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
        };

        DetectionModelFolder = FindModelFolder(modelsFolder, DetectionModelNames) ?? Path.Combine(modelsFolder, BuiltInDetectionModelName);
        RecognitionModelFolder = FindModelFolder(modelsFolder, RecognitionModelNames) ?? Path.Combine(modelsFolder, BuiltInRecognitionModelName);
        _detector = new TextDetector(DetectionModelFolder, sessionOptions);

        try
        {
            _recognizer = new TextRecognizer(RecognitionModelFolder, sessionOptions);
        }
        catch
        {
            _detector.Dispose();
            throw;
        }
    }

    // Returns the first model in the list that has both its files. Null when none does, so the caller can name the
    // built-in model and let loading report it as missing.
    private static string? FindModelFolder(string modelsFolder, string[] modelNames)
    {
        foreach (string modelName in modelNames)
        {
            foreach (string folderName in (string[])[modelName, modelName + DownloadFolderSuffix])
            {
                string folder = Path.Combine(modelsFolder, folderName);

                if (File.Exists(Path.Combine(folder, "inference.onnx")) && File.Exists(Path.Combine(folder, "inference.yml")))
                {
                    return folder;
                }
            }
        }

        return null;
    }

    public OCRResult Recognize(SKBitmap bitmap, float minScore = DefaultMinScore)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        if (bitmap.Width <= 0 || bitmap.Height <= 0)
        {
            return new OCRResult([]);
        }

        using SKBitmap source = ImageTensor.ToOpaqueBgra(bitmap);
        using SKImage image = SKImage.FromBitmap(source);
        List<SKPoint[]> boxes = _detector.Detect(image);

        if (boxes.Count == 0)
        {
            return new OCRResult([]);
        }

        (string Text, float Score)[] texts = _recognizer.Recognize(image, boxes);
        List<OCRTextRegion> regions = [];

        for (int i = 0; i < boxes.Count; i++)
        {
            string text = texts[i].Text.Trim();

            if (text.Length > 0 && texts[i].Score >= minScore)
            {
                regions.Add(new OCRTextRegion(boxes[i], text, texts[i].Score));
            }
        }

        return new OCRResult(regions);
    }

    public void Dispose()
    {
        _detector.Dispose();
        _recognizer.Dispose();
    }
}
