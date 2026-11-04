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
/// Post-processes the live editor on the composition thread. The source is a snapshot of the
/// current render surface, which stays on the GPU when Avalonia uses a GPU-backed renderer.
/// </summary>
internal sealed class ShaderEasterEggOverlay : Control, IDisposable
{
    private enum HandlerCommand
    {
        Start,
        Stop,
        UpdateBounds
    }

    private readonly record struct ShaderPayload(
        HandlerCommand Command,
        IShaderEasterEggEffect? Effect = null,
        Size Bounds = default);

    private CompositionCustomVisual? _customVisual;
    private IShaderEasterEggEffect? _activeEffect;
    private Size _lastSentBounds;

    public bool Start(IShaderEasterEggEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);
        if (_customVisual == null)
        {
            return false;
        }

        // Validate before covering the editor. The composition thread compiles its own
        // runtime effect and owns its eventual disposal.
        using SKRuntimeEffect? validation = SKRuntimeEffect.CreateShader(effect.ShaderSource, out string errors);
        if (validation == null)
        {
            Debug.WriteLine($"Unable to compile image editor easter-egg shader: {errors}");
            return false;
        }

        _activeEffect = effect;
        _customVisual.SendHandlerMessage(new ShaderPayload(HandlerCommand.Start, effect, Bounds.Size));
        return true;
    }

    public void Stop()
    {
        _activeEffect = null;
        _customVisual?.SendHandlerMessage(new ShaderPayload(HandlerCommand.Stop));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        Compositor? compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor == null)
        {
            return;
        }

        _customVisual = compositor.CreateCustomVisual(new ShaderCompositionHandler());
        ElementComposition.SetElementChildVisual(this, _customVisual);
        LayoutUpdated += OnLayoutUpdated;

        _lastSentBounds = default;
        SendBounds();
        if (_activeEffect != null)
        {
            Start(_activeEffect);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        LayoutUpdated -= OnLayoutUpdated;
        Stop();
        ElementComposition.SetElementChildVisual(this, null);
        _customVisual = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        SendBounds();
    }

    private void SendBounds()
    {
        if (_customVisual == null || Bounds.Width <= 0 || Bounds.Height <= 0 ||
            _lastSentBounds == Bounds.Size)
        {
            return;
        }

        _lastSentBounds = Bounds.Size;
        _customVisual.Size = new Vector2((float)Bounds.Width, (float)Bounds.Height);
        _customVisual.SendHandlerMessage(new ShaderPayload(HandlerCommand.UpdateBounds, Bounds: Bounds.Size));
    }

    public void Dispose()
    {
        Stop();
    }

    private sealed class ShaderCompositionHandler : CompositionCustomVisualHandler
    {
        private IShaderEasterEggEffect? _definition;
        private SKRuntimeEffect? _effect;
        private SKRuntimeEffectUniforms? _uniforms;
        private SKRuntimeEffectChildren? _children;
        private SKPaint? _paint;
        private Size _bounds;
        private TimeSpan _startedAt;
        private bool _running;

        public override void OnMessage(object message)
        {
            if (message is not ShaderPayload payload)
            {
                return;
            }

            switch (payload.Command)
            {
                case HandlerCommand.Start:
                    Start(payload);
                    break;
                case HandlerCommand.Stop:
                    ReleaseResources();
                    Invalidate();
                    break;
                case HandlerCommand.UpdateBounds:
                    _bounds = payload.Bounds;
                    Invalidate();
                    break;
            }
        }

        public override void OnAnimationFrameUpdate()
        {
            if (_running)
            {
                // Invalidate the whole editor so the surface always contains fresh underlying
                // content, even when only the CRT animation changed since the last frame.
                Invalidate();
                RegisterForNextAnimationFrameUpdate();
            }
        }

        public override void OnRender(ImmediateDrawingContext context)
        {
            if (!_running || _effect == null || _uniforms == null || _children == null || _paint == null ||
                _definition == null || _bounds.Width <= 0 || _bounds.Height <= 0)
            {
                return;
            }

            ISkiaSharpApiLeaseFeature? leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
            {
                return;
            }

            using (context.PushClip(new Rect(_bounds)))
            using (ISkiaSharpApiLease lease = leaseFeature.Lease())
            {
                SKSurface? surface = lease.SkSurface;
                if (surface == null)
                {
                    return;
                }

                SKCanvas canvas = lease.SkCanvas;
                SKMatrix sourceMatrix = canvas.TotalMatrix;
                SKRect deviceBounds = sourceMatrix.MapRect(SKRect.Create((float)_bounds.Width, (float)_bounds.Height));
                SKRectI sourceRect = new((int)Math.Floor(deviceBounds.Left), (int)Math.Floor(deviceBounds.Top),
                    (int)Math.Ceiling(deviceBounds.Right), (int)Math.Ceiling(deviceBounds.Bottom));
                sourceRect.Intersect(canvas.DeviceClipBounds);
                if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
                {
                    return;
                }

                // Capture before drawing ourselves. Cropping avoids retaining a whole window's
                // texture when the editor is hosted in a smaller workspace.
                using SKImage sourceImage = surface.Snapshot(sourceRect);
                sourceMatrix.TransX -= sourceRect.Left;
                sourceMatrix.TransY -= sourceRect.Top;
                if (!sourceMatrix.TryInvert(out SKMatrix imageToLocal))
                {
                    return;
                }

                using SKShader sourceShader = SKShader.CreateImage(sourceImage,
                    SKShaderTileMode.Clamp, SKShaderTileMode.Clamp,
                    new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None), imageToLocal);
                _children["source"] = sourceShader;

                double elapsed = Math.Max(0, (CompositionNow - _startedAt).TotalSeconds);
                _uniforms["resolution"] = new SKPoint((float)_bounds.Width, (float)_bounds.Height);
                _uniforms["pixelSize"] = new SKPoint(
                    (float)(_bounds.Width / deviceBounds.Width),
                    (float)(_bounds.Height / deviceBounds.Height));
                _uniforms["time"] = (float)(elapsed % 3600);
                _uniforms["powerOn"] = (float)Math.Clamp(elapsed / 0.65, 0, 1);
                _definition.UpdateUniforms(_uniforms, _bounds);

                using SKShader shader = _effect.ToShader(_uniforms, _children);
                _paint.Shader = shader;
                _paint.Color = SKColors.White.WithAlpha((byte)Math.Round(lease.CurrentOpacity * 255));
                canvas.DrawRect(0, 0, (float)_bounds.Width, (float)_bounds.Height, _paint);
                // Do not retain frame textures in the reusable paint/children objects.
                _paint.Shader = null;
                _children["source"] = null;
            }
        }

        private void Start(ShaderPayload payload)
        {
            ReleaseResources();
            if (payload.Effect == null)
            {
                return;
            }

            _effect = SKRuntimeEffect.CreateShader(payload.Effect.ShaderSource, out string errors);
            if (_effect == null)
            {
                Debug.WriteLine($"Unable to compile image editor easter-egg shader: {errors}");
                return;
            }

            _definition = payload.Effect;
            _uniforms = new SKRuntimeEffectUniforms(_effect);
            _children = new SKRuntimeEffectChildren(_effect);
            _paint = new SKPaint { IsAntialias = false };
            _bounds = payload.Bounds;
            _startedAt = CompositionNow;
            _running = true;
            Invalidate();
            RegisterForNextAnimationFrameUpdate();
        }

        private void ReleaseResources()
        {
            _running = false;
            _definition = null;
            _paint?.Dispose();
            _paint = null;
            _children?.Dispose();
            _children = null;
            _uniforms?.Dispose();
            _uniforms = null;
            _effect?.Dispose();
            _effect = null;
        }
    }
}