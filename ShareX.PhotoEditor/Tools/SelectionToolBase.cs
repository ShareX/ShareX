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

using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public enum MarqueeStyle
{
    Normal,
    FixedRatio,
    FixedSize
}

/// <summary>Shared behavior for tools that create selections: mode, feather, anti-alias and moving the outline.</summary>
public abstract class SelectionToolBase : ToolBase
{
    private SKPoint _moveStart;
    private PixelSurface? _moveOriginal;

    protected SelectionToolBase(IEditorHost host) : base(host)
    {
    }

    public SelectionMode Mode { get; set; } = SelectionMode.Replace;

    public float Feather { get; set; }

    public bool AntiAlias { get; set; } = true;

    protected bool IsMovingSelection => _moveOriginal != null;

    /// <summary>Resolves the selection mode from the options bar and the modifier keys held at press time.</summary>
    protected SelectionMode ResolveMode(ToolInput input)
    {
        if (input.Shift && input.Alt) return SelectionMode.Intersect;
        if (input.Shift) return SelectionMode.Add;
        if (input.Alt) return SelectionMode.Subtract;
        return Mode;
    }

    /// <summary>Starts dragging the selection outline when pressing inside an existing selection in replace mode.</summary>
    protected bool TryBeginMove(ToolInput input)
    {
        if (Doc?.Selection == null || ResolveMode(input) != SelectionMode.Replace) return false;
        if (Doc.SelectionAt(input.DocPixel.X, input.DocPixel.Y) < 0.5f) return false;
        _moveStart = input.Doc;
        _moveOriginal = Doc.Selection;
        return true;
    }

    protected void UpdateMove(ToolInput input)
    {
        if (_moveOriginal == null || Doc == null) return;
        SKPoint delta = input.Doc - _moveStart;
        PixelSurface moved = MaskOps.Transform(_moveOriginal, SKMatrix.CreateTranslation(MathF.Round(delta.X), MathF.Round(delta.Y)), Doc.Width, Doc.Height, false);
        Doc.SetSelection(moved);
        Host.InvalidateOverlay();
    }

    protected void EndMove()
    {
        if (_moveOriginal == null) return;
        _moveOriginal = null;
        PushHistory("Move Selection");
    }

    /// <summary>Combines a shape mask with the current selection and records history.</summary>
    protected void ApplySelection(PixelSurface shape, SelectionMode mode, string historyName)
    {
        if (Doc == null) return;
        if (Feather > 0) MaskOps.Blur(shape, Feather);
        PixelSurface? result = MaskOps.Combine(Doc.Selection, shape, mode);
        if (!ReferenceEquals(result, shape) && !ReferenceEquals(result, Doc.Selection)) shape.Dispose();
        Doc.SetSelection(result);
        PushHistory(historyName);
        Host.InvalidateCanvas();
    }

    protected void ApplyPath(SKPath path, SelectionMode mode, string historyName)
    {
        if (Doc == null) return;
        ApplySelection(MaskOps.FromPath(path, Doc.Width, Doc.Height, AntiAlias), mode, historyName);
    }

    protected void BuildCommonOptions(OptionsBuilder options, bool feather = true, bool antiAlias = true)
    {
        options.SelectionMode(() => Mode, m => Mode = m);
        options.Separator();
        if (feather) options.Number("Feather:", 0, 1000, () => Feather, v => Feather = (float)v, "0.#", "px", 0.5);
        if (antiAlias) options.Check("Anti-alias", () => AntiAlias, v => AntiAlias = v);
    }

    /// <summary>Clicking without dragging deselects (in replace mode), like the reference application.</summary>
    protected void DeselectOnClick(SelectionMode mode)
    {
        if (Doc?.Selection != null && mode == SelectionMode.Replace)
        {
            Doc.SetSelection(null);
            PushHistory("Deselect");
            Host.InvalidateCanvas();
        }
    }
}
