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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Skia;
using SkiaSharp;
using System.Diagnostics;
using System.Numerics;

namespace ShareX.ImageEditor.Presentation.EasterEggs;

/// <summary>
/// Composition-thread shader host. Rendering leases Avalonia's Skia canvas so a GPU-backed
/// renderer executes runtime effects on the GPU without touching the editor's raster surface.
/// </summary>
internal sealed class ShaderEasterEggOverlay : Control, IDisposable
{
    private enum HandlerCommand
    {
        Start,
        Stop,
        UpdateBounds,
        Dispose
    }

    private readonly record struct ShaderPayload(
        HandlerCommand Command,
        string? ShaderSource = null,
        byte[]? EncodedSource = null,
        TimeSpan Duration = default,
        Size Bounds = default);

    private CompositionCustomVisual? _customVisual;
    private ShaderPayload? _pendingStart;
    private Size _lastSentBounds;

    public bool Start(IShaderEasterEggEffect effect, SKBitmap snapshot)
    {
        ArgumentNullException.ThrowIfNull(effect);
        ArgumentNullException.ThrowIfNull(snapshot);

        try
        {
            using SKImage image = SKImage.FromBitmap(snapshot);
            using SKData? encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            if (encoded == null)
            {
                return false;
            }

            var payload = new ShaderPayload(
                HandlerCommand.Start,
                effect.ShaderSource,
                encoded.ToArray(),
                effect.Duration,
                Bounds.Size);

            _pendingStart = payload;
            _customVisual?.SendHandlerMessage(payload);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to prepare image editor easter-egg source: {ex}");
            return false;
        }
        finally
        {
            snapshot.Dispose();
        }
    }

    public void Stop()
    {
        _pendingStart = null;
        _customVisual?.SendHandlerMessage(new ShaderPayload(HandlerCommand.Stop));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        CompositionVisual? elementVisual = ElementComposition.GetElementVisual(this);
        Compositor? compositor = elementVisual?.Compositor;
        if (compositor == null)
        {
            return;
        }

        _customVisual = compositor.CreateCustomVisual(new ShaderCompositionHandler());
        _customVisual.Size = new Vector2((float)Bounds.Width, (float)Bounds.Height);
        ElementComposition.SetElementChildVisual(this, _customVisual);
        LayoutUpdated += OnLayoutUpdated;

        _lastSentBounds = default;
        SendBounds();
        if (_pendingStart is ShaderPayload payload)
        {
            _customVisual.SendHandlerMessage(payload);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        LayoutUpdated -= OnLayoutUpdated;
        if (_customVisual != null)
        {
            _customVisual.SendHandlerMessage(new ShaderPayload(HandlerCommand.Dispose));
        }

        _customVisual = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        SendBounds();
    }

    private void SendBounds()
    {
        if (_customVisual == null || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        if (_lastSentBounds == Bounds.Size)
        {
            return;
        }

        _lastSentBounds = Bounds.Size;
        _customVisual.Size = new Vector2((float)Bounds.Width, (float)Bounds.Height);
        _customVisual.SendHandlerMessage(new ShaderPayload(
            HandlerCommand.UpdateBounds,
            Bounds: Bounds.Size));
    }

    public void Dispose()
    {
        Stop();
        if (_customVisual != null)
        {
            _customVisual.SendHandlerMessage(new ShaderPayload(HandlerCommand.Dispose));
        }
    }

    private sealed class ShaderCompositionHandler : CompositionCustomVisualHandler
    {
        private readonly object _syncRoot = new();

        private SKRuntimeEffect? _effect;
        private SKRuntimeEffectUniforms? _uniforms;
        private SKRuntimeEffectChildren? _children;
        private SKBitmap? _sourceBitmap;
        private SKImage? _sourceImage;
        private SKShader? _sourceShader;
        private Size _bounds;
        private TimeSpan _duration;
        private TimeSpan _startedAt;
        private bool _running;

        public override void OnMessage(object message)
        {
            if (message is not ShaderPayload payload)
            {
                return;
            }

            lock (_syncRoot)
            {
                switch (payload.Command)
                {
                    case HandlerCommand.Start:
                        Start(payload);
                        break;
                    case HandlerCommand.Stop:
                        _running = false;
                        ReleaseResources();
                        break;
                    case HandlerCommand.UpdateBounds:
                        _bounds = payload.Bounds;
                        break;
                    case HandlerCommand.Dispose:
                        _running = false;
                        ReleaseResources();
                        break;
                }
            }
        }

        public override void OnAnimationFrameUpdate()
        {
            if (!_running)
            {
                return;
            }

            Invalidate();
            RegisterForNextAnimationFrameUpdate();
        }

        public override void OnRender(ImmediateDrawingContext context)
        {
            lock (_syncRoot)
            {
                if (!_running || _effect == null || _uniforms == null ||
                    _children == null || _sourceBitmap == null)
                {
                    return;
                }

                ISkiaSharpApiLeaseFeature? leaseFeature =
                    context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
                if (leaseFeature == null)
                {
                    return;
                }

                Size bounds = _bounds.Width > 0 && _bounds.Height > 0
                    ? _bounds
                    : new Size(_sourceBitmap.Width, _sourceBitmap.Height);
                double scaleX = bounds.Width / _sourceBitmap.Width;
                double scaleY = bounds.Height / _sourceBitmap.Height;

                using (context.PushClip(new Rect(bounds)))
                using (context.PushPostTransform(Matrix.CreateScale(scaleX, scaleY)))
                using (ISkiaSharpApiLease lease = leaseFeature.Lease())
                {
                    Draw(lease.SkCanvas);
                }
            }
        }

        private void Start(ShaderPayload payload)
        {
            ReleaseResources();
            if (string.IsNullOrWhiteSpace(payload.ShaderSource) || payload.EncodedSource == null)
            {
                return;
            }

            _effect = SKRuntimeEffect.CreateShader(payload.ShaderSource, out string errors);
            if (_effect == null)
            {
                Debug.WriteLine($"Unable to compile image editor easter-egg shader: {errors}");
                return;
            }

            _sourceBitmap = SKBitmap.Decode(payload.EncodedSource);
            if (_sourceBitmap == null)
            {
                Debug.WriteLine("Unable to decode image editor easter-egg source image.");
                ReleaseResources();
                return;
            }

            _sourceImage = SKImage.FromBitmap(_sourceBitmap);
            _sourceShader = SKShader.CreateImage(
                _sourceImage,
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
            _uniforms = new SKRuntimeEffectUniforms(_effect);
            _children = new SKRuntimeEffectChildren(_effect);
            _children["source"] = _sourceShader;
            _bounds = payload.Bounds;
            _duration = payload.Duration;
            _startedAt = CompositionNow;
            _running = true;
            RegisterForNextAnimationFrameUpdate();
        }

        private void Draw(SKCanvas canvas)
        {
            if (_effect == null || _uniforms == null || _children == null ||
                _sourceBitmap == null)
            {
                return;
            }

            float elapsed = (float)(CompositionNow - _startedAt).TotalSeconds;
            float duration = Math.Max((float)_duration.TotalSeconds, float.Epsilon);
            float progress = Math.Clamp(elapsed / duration, 0f, 1f);
            if (progress >= 1f)
            {
                _running = false;
                return;
            }

            _uniforms["resolution"] = new SKPoint(_sourceBitmap.Width, _sourceBitmap.Height);
            _uniforms["time"] = elapsed;
            _uniforms["progress"] = progress;

            using SKShader shader = _effect.ToShader(_uniforms, _children);
            using var paint = new SKPaint
            {
                Shader = shader,
                IsAntialias = false
            };
            canvas.DrawRect(0, 0, _sourceBitmap.Width, _sourceBitmap.Height, paint);
        }

        private void ReleaseResources()
        {
            _children?.Dispose();
            _children = null;
            _uniforms?.Dispose();
            _uniforms = null;
            _sourceShader?.Dispose();
            _sourceShader = null;
            _sourceImage?.Dispose();
            _sourceImage = null;
            _sourceBitmap?.Dispose();
            _sourceBitmap = null;
            _effect?.Dispose();
            _effect = null;
        }
    }
}
