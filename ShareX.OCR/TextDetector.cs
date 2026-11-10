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

namespace ShareX.OCR;

// Finds text lines with a DB (differentiable binarization) model. The model returns a text probability for every pixel,
// and each connected area above the threshold becomes one rotated box.
internal sealed class TextDetector : IDisposable
{
    // Images are scaled up until their short side reaches this, and down until their long side fits the max.
    private const int MinSideLength = 64;
    private const int MaxSideLength = 4000;
    // The model works on sizes that are a multiple of its downsampling stride.
    private const int SizeMultiple = 32;
    private const float MinBoxSize = 3;

    private readonly InferenceSession _session;
    private readonly string _inputName;
    private readonly float[] _mean;
    private readonly float[] _std;
    private readonly float _threshold;
    private readonly float _boxThreshold;
    private readonly float _unclipRatio;
    private readonly int _maxCandidates;

    public TextDetector(string modelFolder, SessionOptions sessionOptions)
    {
        ModelConfig config = ModelConfig.Load(Path.Combine(modelFolder, "inference.yml"));
        _mean = config.GetFloats("mean", [0.485f, 0.456f, 0.406f]);
        _std = config.GetFloats("std", [0.229f, 0.224f, 0.225f]);
        _threshold = config.GetFloat("thresh", 0.3f);
        _boxThreshold = config.GetFloat("box_thresh", 0.6f);
        _unclipRatio = config.GetFloat("unclip_ratio", 1.5f);
        _maxCandidates = config.GetInt("max_candidates", 1000);

        _session = new InferenceSession(GetModelPath(modelFolder), sessionOptions);
        _inputName = _session.InputMetadata.Keys.First();
    }

    // Returns the boxes in source image coordinates, each as top-left, top-right, bottom-right, bottom-left.
    public List<SKPoint[]> Detect(SKImage image)
    {
        (int inputWidth, int inputHeight) = GetInputSize(image.Width, image.Height);
        DenseTensor<float> input = new([1, 3, inputHeight, inputWidth]);

        using (SKBitmap resized = ImageTensor.Resize(image, inputWidth, inputHeight))
        {
            ImageTensor.Fill(input.Buffer.Span, 0, inputWidth, inputHeight, resized, _mean, _std);
        }

        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = _session.Run([NamedOnnxValue.CreateFromTensor(_inputName, input)]);
        Tensor<float> output = outputs.First().AsTensor<float>();
        ReadOnlySpan<int> dimensions = output.Dimensions;
        int mapHeight = dimensions[^2];
        int mapWidth = dimensions[^1];
        float[] probabilities = output is DenseTensor<float> dense ? dense.Buffer.ToArray() : output.ToArray();

        float scaleX = (float)image.Width / mapWidth;
        float scaleY = (float)image.Height / mapHeight;
        List<SKPoint[]> boxes = [];

        foreach (SKPoint[] box in FindBoxes(probabilities, mapWidth, mapHeight))
        {
            SKPoint[] scaled = new SKPoint[4];

            for (int i = 0; i < 4; i++)
            {
                scaled[i] = new SKPoint(
                    Math.Clamp(box[i].X * scaleX, 0, image.Width - 1),
                    Math.Clamp(box[i].Y * scaleY, 0, image.Height - 1));
            }

            if (SKPoint.Distance(scaled[0], scaled[1]) > MinBoxSize && SKPoint.Distance(scaled[0], scaled[3]) > MinBoxSize)
            {
                boxes.Add(scaled);
            }
        }

        return boxes;
    }

    private static (int Width, int Height) GetInputSize(int width, int height)
    {
        float ratio = 1f;

        if (Math.Min(width, height) < MinSideLength)
        {
            ratio = (float)MinSideLength / Math.Min(width, height);
        }

        if (Math.Max(width, height) * ratio > MaxSideLength)
        {
            ratio = (float)MaxSideLength / Math.Max(width, height);
        }

        int RoundToMultiple(float value) => Math.Max((int)Math.Round(value / SizeMultiple) * SizeMultiple, SizeMultiple);

        return (RoundToMultiple(width * ratio), RoundToMultiple(height * ratio));
    }

    private IEnumerable<SKPoint[]> FindBoxes(float[] probabilities, int width, int height)
    {
        bool[] visited = new bool[probabilities.Length];
        Stack<int> pending = new();
        int candidates = 0;

        for (int start = 0; start < probabilities.Length && candidates < _maxCandidates; start++)
        {
            if (visited[start] || probabilities[start] <= _threshold)
            {
                continue;
            }

            candidates++;
            List<SKPoint> edgePixels = CollectRegion(probabilities, visited, pending, start, width, height);
            (SKPoint[] box, float shortSide) = Geometry.MinAreaRect(Geometry.ConvexHull(edgePixels));

            if (shortSide < MinBoxSize || GetBoxScore(probabilities, width, height, box) < _boxThreshold)
            {
                continue;
            }

            (SKPoint[] expanded, float expandedShortSide) = Geometry.Unclip(box, _unclipRatio);

            if (expandedShortSide < MinBoxSize + 2)
            {
                continue;
            }

            yield return expanded;
        }
    }

    // Flood fills one 8-connected region of text pixels and returns its edge pixels, which have the same convex hull
    // as the whole region.
    private List<SKPoint> CollectRegion(float[] probabilities, bool[] visited, Stack<int> pending, int start, int width, int height)
    {
        List<SKPoint> edgePixels = [];
        visited[start] = true;
        pending.Push(start);

        bool IsText(int x, int y) => x >= 0 && y >= 0 && x < width && y < height && probabilities[(y * width) + x] > _threshold;

        while (pending.Count > 0)
        {
            int index = pending.Pop();
            int x = index % width;
            int y = index / width;

            if (!IsText(x - 1, y) || !IsText(x + 1, y) || !IsText(x, y - 1) || !IsText(x, y + 1))
            {
                edgePixels.Add(new SKPoint(x, y));
            }

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if ((dx != 0 || dy != 0) && IsText(nx, ny))
                    {
                        int neighbor = (ny * width) + nx;

                        if (!visited[neighbor])
                        {
                            visited[neighbor] = true;
                            pending.Push(neighbor);
                        }
                    }
                }
            }
        }

        return edgePixels;
    }

    // The mean probability of the pixels inside the box, which filters out weak detections.
    private static float GetBoxScore(float[] probabilities, int width, int height, SKPoint[] box)
    {
        SKRect bounds = Geometry.GetBounds(box);
        int left = Math.Clamp((int)Math.Floor(bounds.Left), 0, width - 1);
        int right = Math.Clamp((int)Math.Ceiling(bounds.Right), 0, width - 1);
        int top = Math.Clamp((int)Math.Floor(bounds.Top), 0, height - 1);
        int bottom = Math.Clamp((int)Math.Ceiling(bounds.Bottom), 0, height - 1);
        double sum = 0;
        int count = 0;

        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                if (IsInside(box, x, y))
                {
                    sum += probabilities[(y * width) + x];
                    count++;
                }
            }
        }

        return count > 0 ? (float)(sum / count) : 0;
    }

    // Whether a pixel center is inside the convex box, counting pixels within half a pixel of an edge like a filled polygon does.
    private static bool IsInside(SKPoint[] box, float x, float y)
    {
        int sign = 0;

        for (int i = 0; i < 4; i++)
        {
            SKPoint a = box[i];
            SKPoint b = box[(i + 1) % 4];
            SKPoint edge = b - a;
            float length = edge.Length;

            if (length < 1e-6f)
            {
                continue;
            }

            float distance = ((edge.X * (y - a.Y)) - (edge.Y * (x - a.X))) / length;

            if (Math.Abs(distance) <= 0.5f)
            {
                continue;
            }

            int side = Math.Sign(distance);

            if (sign == 0)
            {
                sign = side;
            }
            else if (side != sign)
            {
                return false;
            }
        }

        return true;
    }

    internal static string GetModelPath(string modelFolder)
    {
        string modelPath = Path.Combine(modelFolder, "inference.onnx");

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException("The OCR model file is missing.", modelPath);
        }

        return modelPath;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
