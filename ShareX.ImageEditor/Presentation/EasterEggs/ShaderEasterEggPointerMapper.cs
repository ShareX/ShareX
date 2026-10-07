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
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace ShareX.ImageEditor.Presentation.EasterEggs;

/// <summary>
/// Applies the shader's inverse display mapping before hit testing and pointer routing.
/// The subscription is scoped to this editor's input root and is released with the effect.
/// </summary>
internal sealed class ShaderEasterEggPointerMapper : IDisposable
{
    private readonly Control _host;
    private readonly IShaderEasterEggEffect _effect;
    private readonly IDisposable? _subscription;

    public ShaderEasterEggPointerMapper(Control host, IShaderEasterEggEffect effect)
    {
        _host = host;
        _effect = effect;
        IInputManager? inputManager = AvaloniaLocator.Current.GetService<IInputManager>();
        _subscription = inputManager?.PreProcess.Subscribe(new AnonymousObserver<RawInputEventArgs>(OnInput));
    }

    private void OnInput(RawInputEventArgs input)
    {
        TopLevel? root = TopLevel.GetTopLevel(_host);
        if (input is not RawPointerEventArgs pointer || input.Handled ||
            root == null || !ReferenceEquals(input.Root.FocusRoot, root) ||
            pointer.Type == RawPointerEventType.LeaveWindow ||
            _host.Bounds.Width <= 0 || _host.Bounds.Height <= 0)
        {
            return;
        }

        Matrix? rootToLocal = root.TransformToVisual(_host);
        if (rootToLocal == null || !rootToLocal.Value.TryInvert(out Matrix localToRoot))
        {
            return;
        }

        Size bounds = _host.Bounds.Size;
        Point position = pointer.Position * rootToLocal.Value;
        // Captured drawing gestures continue outside the window.
        bool isCaptured = pointer.Device is IPointerDevice pointerDevice &&
            pointerDevice.TryGetPointer(pointer)?.Captured is Visual captured &&
            captured.GetVisualAncestors().Contains(_host.Parent);
        if (!isCaptured && !new Rect(bounds).Contains(position))
        {
            return;
        }

        pointer.Position = _effect.MapToSource(position, bounds) * localToRoot;

        // Pen/touch coalesced points must use the same mapping as the current point, otherwise
        // a freehand stroke jumps between screen and source coordinates.
        if (pointer.IntermediatePoints is { } intermediatePoints)
        {
            pointer.IntermediatePoints = new Lazy<IReadOnlyList<RawPointerPoint>?>(() =>
                intermediatePoints.Value?.Select(point =>
                {
                    point.Position = _effect.MapToSource(point.Position * rootToLocal.Value, bounds) * localToRoot;
                    return point;
                }).ToArray());
        }
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}