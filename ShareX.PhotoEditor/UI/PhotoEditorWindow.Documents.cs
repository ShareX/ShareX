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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PhotoEditor.Integration;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI;

public sealed partial class PhotoEditorWindow
{
    public void OpenDocument(PhotoDocument doc, string? historyName = null)
    {
        doc.History.MaxStates = Math.Clamp(Settings.HistoryStates, 5, 1000);
        doc.History.Initialize(historyName ?? Strings.EditorCommands_Open);
        doc.IsDirty = false;
        DocumentView view = new(doc);
        doc.StructureChanged += () => Canvas.InvalidateOutline();
        _views.Add(view);
        ActivateView(view);
    }

    public void ActivateView(DocumentView? view)
    {
        if (IsTextEditing) EndTextEditing(true);
        if (_view == view) return;
        Tools.Active.Deactivate();
        _view = view;
        Tools.Active.Activate();
        Canvas.InvalidateOutline();
        RefreshTabs();
        RefreshPanels();
        RefreshOptions();
        InvalidateCanvas();
        UpdateTitle();
        ApplyVisibility();
    }

    /// <summary>The document opened by a ShareX after capture task; its flattened image is handed back to the task when it closes.</summary>
    public PhotoDocument? TaskDocument { get; set; }

    /// <summary>The flattened task document, captured when it closed.</summary>
    public SKBitmap? TaskResult { get; private set; }

    private void CaptureTaskResult(DocumentView view)
    {
        if (TaskDocument == null || view.Document != TaskDocument) return;
        TaskResult?.Dispose();
        TaskResult = Compositor.Flatten(view.Document);
        TaskDocument = null;
    }

    public async Task<bool> CloseViewAsync(DocumentView view)
    {
        if (IsBusy) return false;
        // The task document continues to ShareX after capture tasks, so closing it needs no save prompt.
        if (view.Document.IsDirty && view.Document != TaskDocument)
        {
            ActivateView(view);
            bool? save = await DialogBuilder.ConfirmAsync(this, Strings.PhotoEditorWindow_PhotoEditor, string.Format(Strings.PhotoEditorWindow_SaveChangesPrompt, view.Document.DisplayName.TrimEnd('*')), Strings.EditorCommands_Save, Strings.PhotoEditorWindow_DontSave);
            if (save == null) return false;
            if (save == true && !await EditorCommands.SaveAsync(this, false)) return false;
        }
        CaptureTaskResult(view);
        int index = _views.IndexOf(view);
        _views.Remove(view);
        view.Dispose();
        view.Document.Dispose();
        if (_view == view)
        {
            _view = null;
            DocumentView? next = _views.Count > 0 ? _views[Math.Clamp(index, 0, _views.Count - 1)] : null;
            if (next != null) ActivateView(next);
            else
            {
                // No documents left: clear panels, options and status like an empty workspace.
                RefreshPanels();
                RefreshOptions();
                RefreshStatus();
                ApplyVisibility();
                UpdateTitle();
            }
        }
        RefreshTabs();
        InvalidateCanvas();
        return true;
    }

    private void RefreshTabs()
    {
        _tabZoom = _view?.Zoom ?? 0;
        _tabs.Children.Clear();
        foreach (DocumentView view in _views)
        {
            bool active = view == _view;
            DocumentView captured = view;
            PhotoDocument doc = view.Document;
            string layer = doc.ActiveLayer?.Name ?? Strings.LayerNames_Background;
            string mode = doc.EditingMask && doc.ActiveLayer?.Mask != null ? Strings.PhotoEditorWindow_ModeLayerMask : doc.QuickMask ? Strings.PhotoEditorWindow_ModeQuickMask : "RGB/8";
            string text = $"{doc.DisplayName.TrimEnd('*')} @ {view.Zoom * 100:0.#}% ({layer}, {mode}){(doc.IsDirty ? " *" : string.Empty)}";

            Button close = new() { Classes = { "icon" }, Content = new IconView(PhotoEditorIcons.Close, 10), Padding = new Thickness(3), Width = 16, Height = 16, VerticalAlignment = VerticalAlignment.Center, Opacity = active ? 1 : 0.6 };
            ToolTip.SetTip(close, Strings.EditorCommands_Close);
            close.Click += async (_, e) =>
            {
                e.Handled = true;
                await CloseViewAsync(captured);
            };
            StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            content.Children.Add(new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = active ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))
            });
            content.Children.Add(close);
            // Document tabs: the active tab joins the canvas area, inactive tabs sit on the darker strip.
            Border tab = new()
            {
                Child = content,
                Padding = new Thickness(12, 0, 6, 0),
                Height = 28,
                Margin = new Thickness(0, 0, 1, 0),
                Background = active ? new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32)) : new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A)),
                BorderBrush = active ? OverlayContext.AccentBrush : Brushes.Transparent,
                BorderThickness = new Thickness(0, active ? 2 : 0, 0, 0),
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            ToolTip.SetTip(tab, doc.FilePath ?? doc.Title);
            tab.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(tab).Properties.IsMiddleButtonPressed)
                {
                    _ = CloseViewAsync(captured);
                    return;
                }
                ActivateView(captured);
            };
            _tabs.Children.Add(tab);
        }
    }

    private void UpdateTitle()
    {
        Title = Document != null ? string.Format(Strings.PhotoEditorWindow_DocumentWindowTitle, Document.DisplayName) : Strings.PhotoEditorWindow_WindowTitle;
        _titleText.Text = Document?.DisplayName ?? Strings.PhotoEditorWindow_PhotoEditor;
    }
}
