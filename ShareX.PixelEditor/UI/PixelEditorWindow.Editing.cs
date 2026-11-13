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
using Avalonia.Platform.Storage;
using ShareX.PixelEditor.Tools;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI;

public sealed partial class PixelEditorWindow
{
    private ToolBase? _springReturn;
    private List<int> _playSequence = [];
    private int _playIndex;

    // ------------------------------------------------------------------ painting

    /// <summary>The layer tools paint on, or null (with a status message) when the active layer cannot be edited.</summary>
    public PixelLayer? EditableLayer(bool quiet = false)
    {
        Sprite? sprite = Sprite;
        if (sprite == null) return null;
        string? problem = sprite.ActiveLayer is not PixelLayer ? Strings.Status_SelectPixelLayer
            : sprite.ActiveLayer.IsEffectivelyLocked ? Strings.Status_LayerLocked
            : !sprite.ActiveLayer.IsEffectivelyVisible ? Strings.Status_LayerHidden
            : null;
        if (problem != null)
        {
            if (!quiet) SetStatus(problem);
            return null;
        }
        return (PixelLayer)sprite.ActiveLayer!;
    }

    /// <summary>Starts an edit of the active cel. The painter uses the foreground color, or the background color for the right button.</summary>
    public Painter? CreatePainter(bool secondary)
    {
        Sprite? sprite = Sprite;
        PixelLayer? layer = EditableLayer();
        if (sprite == null || layer == null || _view == null) return null;
        StopPlayback();
        CommitFloating();
        SetStatus(null);
        PixelBuffer? before = sprite.GetBuffer(layer, sprite.ActiveFrame);
        PixelBuffer original = before == null ? new PixelBuffer(sprite.Width, sprite.Height) : before.Frozen ? before : before.Clone();
        PixelBuffer target = sprite.GetWritable(layer, sprite.ActiveFrame);
        return new Painter(target, original)
        {
            Color = secondary ? BackgroundColor : ForegroundColor,
            Clip = sprite.Selection,
            Symmetry = Settings.Symmetry,
            AxisX2 = _view.AxisX2,
            AxisY2 = _view.AxisY2,
            TiledX = Settings.Tiled is TiledMode.Horizontal or TiledMode.Both,
            TiledY = Settings.Tiled is TiledMode.Vertical or TiledMode.Both,
            Palette = sprite.Palette
        };
    }

    /// <summary>Records a history state after a change and refreshes the interface.</summary>
    public void Commit(string historyName)
    {
        if (Sprite == null) return;
        // Pixels painted on tilemap layers go back into their tiles before the state is recorded.
        Tilemaps.Sync(Sprite, Settings.TileEditsShared);
        Sprite.History.Push(historyName);
        InvalidateCanvas();
        RefreshPanels();
    }

    /// <summary>Drops changes made since the last history state.</summary>
    public void RevertEdit()
    {
        if (Sprite == null) return;
        Sprite.History.Revert();
        InvalidateCanvas();
    }

    public void AfterHistoryChange()
    {
        Floating = null;
        Canvas.AbortTool();
        Tools.Active.Cancel();
        Tools.Active.Activate();
        InvalidateCanvas();
        RefreshPanels();
        RefreshOptions();
    }

    public void SetSelection(SelectionMask? mask)
    {
        if (Sprite == null || ReferenceEquals(Sprite.Selection, mask)) return;
        bool had = Sprite.Selection != null;
        Sprite.Selection = mask;
        Sprite.History.Push(mask == null ? Strings.History_Deselect : had ? Strings.History_ModifySelection : Strings.History_Select);
        (_dock.Get("history") as Panels.HistoryPanel)?.Refresh();
        UpdateTitle();
        RefreshTabs();
        Canvas.InvalidateVisual();
    }

    /// <summary>The color at a pixel, of the whole image or of the active layer only.</summary>
    public uint? SampleColor(int x, int y, bool allLayers)
    {
        Sprite? sprite = Sprite;
        if (sprite == null || _view == null) return null;
        if (Settings.Tiled is TiledMode.Horizontal or TiledMode.Both) x = ((x % sprite.Width) + sprite.Width) % sprite.Width;
        if (Settings.Tiled is TiledMode.Vertical or TiledMode.Both) y = ((y % sprite.Height) + sprite.Height) % sprite.Height;
        if (!sprite.Bounds.Contains(x, y)) return null;
        if (allLayers || sprite.ActiveLayer is not PixelLayer layer) return _view.Composite[x, y];
        return sprite.GetBuffer(layer, sprite.ActiveFrame)?[x, y] ?? 0;
    }

    // ------------------------------------------------------------------ floating selection

    /// <summary>Pixels being moved or transformed; they are part of the cel but not yet of the history.</summary>
    public Floating? Floating { get; private set; }

    /// <summary>Lifts the selected pixels (or the whole cel) so they can be moved. Returns false when there is nothing to lift.</summary>
    public bool BeginFloating(bool copy)
    {
        Sprite? sprite = Sprite;
        PixelLayer? layer = EditableLayer();
        if (sprite == null || layer == null) return false;
        StopPlayback();
        PixelBuffer? buffer = sprite.GetBuffer(layer, sprite.ActiveFrame);
        if (buffer == null)
        {
            SetStatus(Strings.Status_NothingToMove);
            return false;
        }
        SelectionMask? selection = sprite.Selection;
        SKRectI bounds;
        PixelBuffer source, baseImage = buffer.Clone();
        bool[]? sourceMask = null;
        if (selection != null)
        {
            bounds = selection.Bounds;
            source = new PixelBuffer(bounds.Width, bounds.Height);
            sourceMask = new bool[bounds.Width * bounds.Height];
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    int i = y * sprite.Width + x;
                    if (!selection.Data[i]) continue;
                    int s = (y - bounds.Top) * bounds.Width + (x - bounds.Left);
                    sourceMask[s] = true;
                    source.Pixels[s] = buffer.Pixels[i];
                    if (!copy) baseImage.Pixels[i] = 0;
                }
            }
        }
        else
        {
            if (buffer.ContentBounds() is not SKRectI content)
            {
                SetStatus(Strings.Status_NothingToMove);
                return false;
            }
            bounds = content;
            source = buffer.Crop(bounds);
            if (!copy) baseImage.Clear();
        }
        Floating = new Floating
        {
            Source = source,
            SourceMask = sourceMask,
            Base = baseImage,
            Layer = layer,
            Frame = sprite.ActiveFrame,
            X = bounds.Left,
            Y = bounds.Top,
            Width = bounds.Width,
            Height = bounds.Height,
            Selects = selection != null,
            HistoryName = Strings.History_Move
        };
        RenderFloating();
        RefreshOptions();
        return true;
    }

    /// <summary>Places an image on the active cel as a movable selection and switches to the Move tool.</summary>
    public void PasteFloating(PixelBuffer image, int x, int y, string historyName)
    {
        Sprite? sprite = Sprite;
        PixelLayer? layer = EditableLayer();
        if (sprite == null || layer == null) return;
        CommitFloating();
        StopPlayback();
        PixelBuffer? buffer = sprite.GetBuffer(layer, sprite.ActiveFrame);
        Floating = new Floating
        {
            Source = image,
            Base = buffer?.Clone() ?? new PixelBuffer(sprite.Width, sprite.Height),
            Layer = layer,
            Frame = sprite.ActiveFrame,
            X = x,
            Y = y,
            Width = image.Width,
            Height = image.Height,
            Selects = true,
            HistoryName = historyName
        };
        RenderFloating();
        Tools.Select(ToolId.Move);
        RefreshOptions();
    }

    public void RenderFloating()
    {
        Sprite? sprite = Sprite;
        if (sprite == null || Floating is not Floating f) return;
        PixelBuffer target = sprite.GetWritable(f.Layer, f.Frame);
        Array.Copy(f.Base.Pixels, target.Pixels, target.Pixels.Length);
        bool[]? covered = f.Selects ? new bool[sprite.Width * sprite.Height] : null;
        // Rotations off the right angles read from an enlarged, edge-smoothed copy so lines stay clean.
        bool smooth = Settings.SmoothRotation && Math.Abs(f.Angle % 90) > 0.01;
        if (smooth) f.Smooth ??= Effects.UpscaleForRotation(f.Source, f.SourceMask);
        Effects.DrawTransformed(target, f.Source, f.SourceMask, f.X, f.Y, f.Width, f.Height, f.Angle, f.FlipX, f.FlipY, covered, smooth ? f.Smooth!.Value.Image : null, smooth ? f.Smooth!.Value.Mask : null);
        if (covered != null)
        {
            SelectionMask mask = new(sprite.Width, sprite.Height, covered);
            sprite.Selection = mask.IsEmpty ? null : mask;
        }
        InvalidateCanvas();
    }

    /// <summary>Drops the floating pixels where they are and records the change.</summary>
    public void CommitFloating()
    {
        if (Floating is not Floating f) return;
        Floating = null;
        Commit(f.HistoryName);
        RefreshOptions();
    }

    public void CancelFloating()
    {
        if (Floating == null) return;
        Floating = null;
        RevertEdit();
        RefreshPanels();
        RefreshOptions();
    }

    // ------------------------------------------------------------------ frames, layers and playback

    public void GoToFrame(int frame, bool extendRange = false)
    {
        Sprite? sprite = Sprite;
        if (sprite == null) return;
        CommitFloating();
        frame = Math.Clamp(frame, 0, sprite.Frames.Count - 1);
        sprite.SetActiveFrame(frame, extendRange);
        InvalidateCanvas();
        _timeline.Refresh();
        _timeline.ScrollToFrame(frame);
        (_dock.Get("layers") as Panels.LayersPanel)?.Refresh();
        RefreshStatus();
    }

    public void SetActiveLayer(Layer layer)
    {
        Sprite? sprite = Sprite;
        if (sprite == null || sprite.ActiveLayer == layer) return;
        CommitFloating();
        sprite.ActiveLayer = layer;
        _dock.Get("layers")?.Refresh();
        _timeline.Refresh();
        Canvas.InvalidateVisual();
    }

    public bool IsPlaying => _playTimer.IsEnabled;

    public void TogglePlayback()
    {
        if (IsPlaying) StopPlayback();
        else StartPlayback();
    }

    public void StartPlayback()
    {
        Sprite? sprite = Sprite;
        if (sprite == null || _view == null || sprite.Frames.Count < 2) return;
        CommitFloating();
        _playSequence = SpriteOps.PlaybackSequence(sprite, Settings.PlayTagOnly);
        _playIndex = Math.Max(0, _playSequence.IndexOf(sprite.ActiveFrame));
        _view.PlayFrame = _playSequence[_playIndex];
        _playTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(10, sprite.Frames[_playSequence[_playIndex]].Duration));
        _playTimer.Start();
        _timeline.Refresh();
    }

    public void StopPlayback()
    {
        if (!IsPlaying) return;
        _playTimer.Stop();
        if (_view != null) _view.PlayFrame = null;
        InvalidateCanvas();
        _timeline.Refresh();
        RefreshStatus();
    }

    private void AdvancePlayback()
    {
        Sprite? sprite = Sprite;
        if (sprite == null || _view == null || _playSequence.Count == 0)
        {
            StopPlayback();
            return;
        }
        _playIndex++;
        if (_playIndex >= _playSequence.Count)
        {
            if (!Settings.LoopPlayback)
            {
                StopPlayback();
                return;
            }
            _playIndex = 0;
        }
        int frame = Math.Clamp(_playSequence[_playIndex], 0, sprite.Frames.Count - 1);
        _view.PlayFrame = frame;
        _playTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(10, sprite.Frames[frame].Duration));
        InvalidateCanvas();
        _timeline.InvalidatePlayhead();
        RefreshStatus();
    }

    // ------------------------------------------------------------------ keyboard

    private static bool IsTextInput(object? source)
    {
        for (StyledElement? e = source as StyledElement; e != null; e = e.Parent)
        {
            if (e is TextBox || e is NumericUpDown || e is ComboBox { IsDropDownOpen: true }) return true;
        }
        return false;
    }

    private void OnKeyDownTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (IsTextInput(e.Source)) return;
        if (IsBusy)
        {
            e.Handled = true;
            return;
        }
        // While a stroke or drag is in progress only modifier keys, Escape and Enter reach the tool; commands wait for the release.
        if (Canvas.IsToolPointerDown && e.Key is not (Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or Key.Space or Key.Escape or Key.Enter))
        {
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Space && !Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = true;
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Space)
        {
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Tab && e.KeyModifiers == KeyModifiers.None)
        {
            TogglePanels();
            e.Handled = true;
            return;
        }
        // Alt temporarily switches painting tools to the Eyedropper.
        if (e.Key is Key.LeftAlt or Key.RightAlt)
        {
            if (_springReturn == null && !Canvas.IsToolPointerDown && Tools.Active is PaintToolBase)
            {
                _springReturn = Tools.Active;
                Tools.Select(ToolId.Eyedropper);
            }
            e.Handled = true;
            return;
        }
        if (Tools.Active.KeyDown(e.Key, e.KeyModifiers))
        {
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Escape && Canvas.IsToolPointerDown)
        {
            Canvas.AbortTool();
            InvalidateCanvas();
            e.Handled = true;
            return;
        }
        EditorCommand? command = CommandRegistry.Match(e.Key, e.KeyModifiers, Settings);
        if (command != null)
        {
            Execute(command.Id);
            e.Handled = true;
            return;
        }
        if (Tools.HandleShortcut(e.Key, e.KeyModifiers)) e.Handled = true;
    }

    private void OnKeyUpTunnel(object? sender, KeyEventArgs e)
    {
        LastModifiers = e.KeyModifiers;
        if (e.Key == Key.Space && Canvas.SpaceHeld)
        {
            Canvas.SpaceHeld = false;
            e.Handled = true;
        }
        if (e.Key is Key.LeftAlt or Key.RightAlt)
        {
            if (_springReturn != null)
            {
                Tools.Select(_springReturn);
                _springReturn = null;
            }
            e.Handled = true;
        }
    }

    // ------------------------------------------------------------------ drag and drop

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        List<string> paths = e.DataTransfer.TryGetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => !string.IsNullOrEmpty(p)).Select(p => p!).ToList() ?? [];
        foreach (string path in paths.Where(SpriteFile.IsSupported)) EditorCommands.OpenFile(this, path);
    }
}
