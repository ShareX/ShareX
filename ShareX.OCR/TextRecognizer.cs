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

    public RecognizedText[] Recognize(SKImage image, IReadOnlyList<SKPoint[]> boxes)
    {
        LineImage[] lines = [.. boxes.Select(GetLineImage)];
        RecognizedText[] results = new RecognizedText[boxes.Count];

        // Lines of similar width are batched together so less of each batch is padding.
        int[] order = [.. Enumerable.Range(0, lines.Length).OrderBy(i => lines[i].AspectRatio)];

        for (int batchStart = 0; batchStart < order.Length; batchStart += BatchSize)
        {
            int[] batch = order[batchStart..Math.Min(batchStart + BatchSize, order.Length)];
            RecognizeBatch(image, lines, batch, results);
        }

        return results;
    }

    private void RecognizeBatch(SKImage image, LineImage[] lines, int[] batch, RecognizedText[] results)
    {
        float maxAspectRatio = Math.Max((float)_minInputWidth / _inputHeight, batch.Max(i => lines[i].AspectRatio));
        int inputWidth = Math.Min((int)(_inputHeight * maxAspectRatio), MaxInputWidth);
        DenseTensor<float> input = new([batch.Length, 3, _inputHeight, inputWidth]);
        int[] widths = new int[batch.Length];

        for (int i = 0; i < batch.Length; i++)
        {
            LineImage line = lines[batch[i]];
            int width = Math.Clamp((int)Math.Ceiling(_inputHeight * line.AspectRatio), 1, inputWidth);
            widths[i] = width;
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
            // The steps cover the whole input, padding included, and the line fills only the first part of it.
            float stepShare = (float)inputWidth / steps / widths[i];
            results[batch[i]] = Decode(probabilities.AsSpan(i * steps * classes, steps * classes), steps, classes, stepShare);
        }
    }

    // Greedy CTC decoding: take the likeliest class at each step, merge repeats, drop blanks. Each character is placed
    // at the middle of the steps that read it, as a share of the line length, so the text can be found in the image.
    private RecognizedText Decode(ReadOnlySpan<float> probabilities, int steps, int classes, float stepShare)
    {
        StringBuilder text = new();
        List<(int Length, int FirstStep, int LastStep)> tokens = [];
        float scoreSum = 0;
        int scoreCount = 0;
        int previous = -1;
        bool previousRead = false;

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

            if (best == previous && previousRead)
            {
                tokens[^1] = tokens[^1] with { LastStep = step };
            }
            else if (best != 0 && best != previous && best < _characters.Length)
            {
                string character = _characters[best];
                text.Append(character);
                tokens.Add((character.Length, step, step));
                scoreSum += stepProbabilities[best];
                scoreCount++;
                previousRead = true;
            }
            else
            {
                previousRead = false;
            }

            previous = best;
        }

        float score = scoreCount > 0 ? scoreSum / scoreCount : 0;

        if (tokens.Count == 0)
        {
            return new RecognizedText(string.Empty, score, []);
        }

        // A dictionary entry can be more than one UTF-16 unit long, so every unit of it gets the span of the entry.
        OCRCharacterSpan[] tokenSpans = GetSpans([.. tokens.Select(token => (token.FirstStep + token.LastStep + 1) / 2f * stepShare)]);
        List<OCRCharacterSpan> spans = new(text.Length);

        for (int i = 0; i < tokens.Count; i++)
        {
            spans.AddRange(Enumerable.Repeat(tokenSpans[i], tokens[i].Length));
        }

        // Trimmed the same way as string.Trim, so the spans stay lined up with the characters.
        string value = text.ToString();
        int start = 0;
        int end = value.Length;

        while (start < end && char.IsWhiteSpace(value[start]))
        {
            start++;
        }

        while (end > start && char.IsWhiteSpace(value[end - 1]))
        {
            end--;
        }

        return new RecognizedText(value[start..end], score, [.. spans.GetRange(start, end - start)]);
    }

    // Where to sample a box from: its origin corner and the edges that become the line's x and y axes.
    private static LineImage GetLineImage(SKPoint[] box)
    {
        (SKPoint origin, SKPoint xEdge, SKPoint yEdge, float aspectRatio) = Geometry.GetReadingAxis(box);
        return new LineImage(origin, xEdge, yEdge, aspectRatio);
    }

    // Each character spans from halfway to the character before it to halfway to the character after it. The first
    // and last characters reach out by the same distance on their outer side.
    private static OCRCharacterSpan[] GetSpans(List<float> centers)
    {
        if (centers.Count == 1)
        {
            return [new OCRCharacterSpan(0, 1)];
        }

        OCRCharacterSpan[] spans = new OCRCharacterSpan[centers.Count];

        for (int i = 0; i < centers.Count; i++)
        {
            float start = i > 0 ? (centers[i - 1] + centers[i]) / 2 : centers[0] - ((centers[1] - centers[0]) / 2);
            float end = i < centers.Count - 1 ? (centers[i] + centers[i + 1]) / 2 : centers[i] + ((centers[i] - centers[i - 1]) / 2);
            spans[i] = new OCRCharacterSpan(Math.Clamp(start, 0, 1), Math.Clamp(end, 0, 1));
        }

        return spans;
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    private readonly record struct LineImage(SKPoint Origin, SKPoint XEdge, SKPoint YEdge, float AspectRatio);
}

// The text read from one box, with where each of its characters sits along the box.
internal readonly record struct RecognizedText(string Text, float Score, OCRCharacterSpan[] Characters);
