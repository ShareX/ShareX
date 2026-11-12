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
using System.Text;

namespace ShareX.OCR;

// Reads the text of one line image with a CTC model. Each line is scaled to the model height and the model returns
// a character probability for every horizontal step.
internal sealed class TextRecognizer : IDisposable
{
    private const int BatchSize = 6;
    private const int MaxInputWidth = 3200;
    // Lines this much taller than wide are vertical text and are turned to read left to right.
    private const float VerticalTextRatio = 1.5f;
    private static readonly float[] Mean = [0.5f, 0.5f, 0.5f];
    private static readonly float[] Std = [0.5f, 0.5f, 0.5f];

    private readonly InferenceSession _session;
    private readonly string _inputName;
    private readonly int _inputHeight;
    private readonly int _minInputWidth;
    // Index 0 is the CTC blank, followed by the dictionary and a space.
    private readonly string[] _characters;

    public TextRecognizer(string modelFolder, SessionOptions sessionOptions)
    {
        ModelConfig config = ModelConfig.Load(Path.Combine(modelFolder, "inference.yml"));
        List<string> dictionary = config.GetList("character_dict");

        if (dictionary.Count == 0)
        {
            throw new InvalidDataException("The OCR recognition model configuration has no character dictionary.");
        }

        _characters = [string.Empty, .. dictionary, " "];
        List<string> imageShape = config.GetList("image_shape");
        _inputHeight = imageShape.Count == 3 && int.TryParse(imageShape[1], out int height) ? height : 48;
        _minInputWidth = imageShape.Count == 3 && int.TryParse(imageShape[2], out int width) ? width : 320;

        _session = new InferenceSession(TextDetector.GetModelPath(modelFolder), sessionOptions);
        _inputName = _session.InputMetadata.Keys.First();
    }

    public (string Text, float Score)[] Recognize(SKImage image, IReadOnlyList<SKPoint[]> boxes)
    {
        LineImage[] lines = [.. boxes.Select(GetLineImage)];
        (string Text, float Score)[] results = new (string, float)[boxes.Count];

        // Lines of similar width are batched together so less of each batch is padding.
        int[] order = [.. Enumerable.Range(0, lines.Length).OrderBy(i => lines[i].AspectRatio)];

        for (int batchStart = 0; batchStart < order.Length; batchStart += BatchSize)
        {
            int[] batch = order[batchStart..Math.Min(batchStart + BatchSize, order.Length)];
            RecognizeBatch(image, lines, batch, results);
        }

        return results;
    }

    private void RecognizeBatch(SKImage image, LineImage[] lines, int[] batch, (string Text, float Score)[] results)
    {
        float maxAspectRatio = Math.Max((float)_minInputWidth / _inputHeight, batch.Max(i => lines[i].AspectRatio));
        int inputWidth = Math.Min((int)(_inputHeight * maxAspectRatio), MaxInputWidth);
        DenseTensor<float> input = new([batch.Length, 3, _inputHeight, inputWidth]);

        for (int i = 0; i < batch.Length; i++)
        {
            LineImage line = lines[batch[i]];
            int width = Math.Clamp((int)Math.Ceiling(_inputHeight * line.AspectRatio), 1, inputWidth);
            using SKBitmap crop = ImageTensor.Crop(image, line.Origin, line.XEdge, line.YEdge, width, _inputHeight);
            ImageTensor.Fill(input.Buffer.Span, i, inputWidth, _inputHeight, crop, Mean, Std);
        }

        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = _session.Run([NamedOnnxValue.CreateFromTensor(_inputName, input)]);
        Tensor<float> output = outputs.First().AsTensor<float>();
        int steps = output.Dimensions[1];
        int classes = output.Dimensions[2];
        float[] probabilities = output is DenseTensor<float> dense ? dense.Buffer.ToArray() : output.ToArray();

        for (int i = 0; i < batch.Length; i++)
        {
            results[batch[i]] = Decode(probabilities.AsSpan(i * steps * classes, steps * classes), steps, classes);
        }
    }

    // Greedy CTC decoding: take the likeliest class at each step, merge repeats, drop blanks.
    private (string Text, float Score) Decode(ReadOnlySpan<float> probabilities, int steps, int classes)
    {
        StringBuilder text = new();
        float scoreSum = 0;
        int scoreCount = 0;
        int previous = -1;

        for (int step = 0; step < steps; step++)
        {
            ReadOnlySpan<float> stepProbabilities = probabilities.Slice(step * classes, classes);
            int best = 0;

            for (int c = 1; c < classes; c++)
            {
                if (stepProbabilities[c] > stepProbabilities[best])
                {
                    best = c;
                }
            }

            if (best != 0 && best != previous && best < _characters.Length)
            {
                text.Append(_characters[best]);
                scoreSum += stepProbabilities[best];
                scoreCount++;
            }

            previous = best;
        }

        return (text.ToString(), scoreCount > 0 ? scoreSum / scoreCount : 0);
    }

    // Where to sample a box from: its origin corner and the edges that become the line's x and y axes.
    private static LineImage GetLineImage(SKPoint[] box)
    {
        float width = Math.Max(SKPoint.Distance(box[0], box[1]), SKPoint.Distance(box[3], box[2]));
        float height = Math.Max(SKPoint.Distance(box[0], box[3]), SKPoint.Distance(box[1], box[2]));
        width = Math.Max(width, 1);
        height = Math.Max(height, 1);

        if (height / width >= VerticalTextRatio)
        {
            // Turned a quarter counter-clockwise: the top-right corner becomes the origin and the right edge the x axis.
            return new LineImage(box[1], box[2] - box[1], box[0] - box[1], height / width);
        }

        return new LineImage(box[0], box[1] - box[0], box[3] - box[0], width / height);
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    private readonly record struct LineImage(SKPoint Origin, SKPoint XEdge, SKPoint YEdge, float AspectRatio);
}
