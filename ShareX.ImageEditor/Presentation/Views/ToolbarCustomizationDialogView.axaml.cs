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
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Controls;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    /// <summary>
    /// Lets the toolbar be laid out by dragging: items and spacers move along the mock toolbar, available items are
    /// dragged onto it, anything dragged from the toolbar onto the available items is taken off it, and the right edge
    /// of a spacer is dragged to change its width.
    /// </summary>
    public partial class ToolbarCustomizationDialogView : UserControl
    {
        private const double DragThreshold = 4;
        private const double ToolbarDropMargin = 12;
        private const string SecondaryHotkeyTag = "Secondary";
        private const int SpacerWidthKeyStep = 4;

        // The dialog widens with its host so the toolbar row is shown at full size when there is room.
        private const double MinDialogWidth = 600;
        private const double MaxDialogWidth = 980;
        private const double HostMargin = 32;

        private enum DropKind
        {
            None,
            Toolbar,
            Palette
        }

        private readonly Border _dialogBorder;
        private readonly Canvas _dragLayer;
        private readonly Border _dragGhost;
        private readonly Border _dragGhostSpacer;
        private readonly Avalonia.Controls.Shapes.Line _dragGhostDash;
        private readonly IconPresenter _dragGhostIcon;
        private readonly Border _dropIndicator;
        private readonly Border _toolbarDropArea;
        private readonly Border _paletteDropArea;
        private readonly ItemsControl _toolbarSlotsControl;
        private readonly ScrollViewer _layoutScrollViewer;

        private Button? _pressedButton;
        private ToolbarLayoutSlot? _pressedSlot;
        private int _pressedToolbarIndex = -1;
        private IPointer? _pressedPointer;
        private Point _pressPoint;
        private bool _isDragging;
        private DropKind _dropKind;
        private int _dropIndex;

        private ToolbarLayoutSlot? _resizingSlot;
        private Button? _resizingButton;
        private int _resizingIndex = -1;
        private int _resizeStartWidth;
        private double _resizeStartX;
        private double _resizeScale = 1;
        private IPointer? _resizingPointer;

        public ToolbarCustomizationDialogView()
        {
            AvaloniaXamlLoader.Load(this);

            _dialogBorder = this.FindControl<Border>("DialogBorder")!;
            _dragLayer = this.FindControl<Canvas>("DragLayer")!;
            _dragGhost = this.FindControl<Border>("DragGhost")!;
            _dragGhostSpacer = this.FindControl<Border>("DragGhostSpacer")!;
            _dragGhostDash = this.FindControl<Avalonia.Controls.Shapes.Line>("DragGhostDash")!;
            _dragGhostIcon = this.FindControl<IconPresenter>("DragGhostIcon")!;
            _dropIndicator = this.FindControl<Border>("DropIndicator")!;
            _toolbarDropArea = this.FindControl<Border>("ToolbarDropArea")!;
            _paletteDropArea = this.FindControl<Border>("PaletteDropArea")!;
            _toolbarSlotsControl = this.FindControl<ItemsControl>("ToolbarSlotsControl")!;
            _layoutScrollViewer = this.FindControl<ScrollViewer>("LayoutScrollViewer")!;

            AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
            AddHandler(PointerPressedEvent, OnPreviewPointerPressed, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnPreviewPointerMoved, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, OnPreviewPointerReleased, RoutingStrategies.Tunnel);
            PointerCaptureLost += OnPointerCaptureLost;
        }

        private ToolbarCustomizationDialogViewModel? ViewModel => DataContext as ToolbarCustomizationDialogViewModel;

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = double.IsInfinity(availableSize.Width)
                ? MaxDialogWidth
                : Math.Clamp(availableSize.Width - HostMargin, MinDialogWidth, MaxDialogWidth);

            if (!_dialogBorder.Width.Equals(width))
            {
                _dialogBorder.Width = width;
            }

            return base.MeasureOverride(availableSize);
        }

        private void OnSlotClick(object? sender, RoutedEventArgs e)
        {
            if (ViewModel is { } vm && sender is Control { DataContext: ToolbarLayoutSlot { Item: { } item } })
            {
                vm.SelectedItem = item;
            }
        }

        private void OnPaletteTileDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (ViewModel is { } vm && sender is Control { DataContext: ToolbarLayoutSlot { Item: { } item } })
            {
                vm.PlaceItem(item, vm.ToolbarSlots.Count);
                e.Handled = true;
            }
        }

        private void OnMoveLeftClick(object? sender, RoutedEventArgs e)
        {
            ViewModel?.MoveLeft(ViewModel.SelectedItem);
        }

        private void OnMoveRightClick(object? sender, RoutedEventArgs e)
        {
            ViewModel?.MoveRight(ViewModel.SelectedItem);
        }

        #region Dragging

        private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_isDragging || _resizingSlot != null)
            {
                return;
            }

            ClearPress();

            if (TryStartResize(e))
            {
                return;
            }

            if (ViewModel is not { } vm || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
                FindSlotButton(e.Source) is not { DataContext: ToolbarLayoutSlot slot } button)
            {
                return;
            }

            _pressedButton = button;
            _pressedSlot = slot;
            _pressedToolbarIndex = button.Classes.Contains("toolbar-slot") ? vm.ToolbarSlots.IndexOf(slot) : -1;
            _pressedPointer = e.Pointer;
            _pressPoint = e.GetPosition(this);
        }

        private void OnPreviewPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_resizingSlot != null && e.Pointer == _resizingPointer)
            {
                double offset = (e.GetPosition(this).X - _resizeStartX) / _resizeScale;
                _resizingSlot.SpacerWidth = (int)Math.Round(_resizeStartWidth + offset);
                e.Handled = true;
                return;
            }

            if (_pressedSlot == null || e.Pointer != _pressedPointer)
            {
                return;
            }

            if (!_isDragging)
            {
                if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    ClearPress();
                    return;
                }

                Vector offset = e.GetPosition(this) - _pressPoint;
                if (Math.Abs(offset.X) < DragThreshold && Math.Abs(offset.Y) < DragThreshold)
                {
                    return;
                }

                StartDrag(e.Pointer);
            }

            UpdateDrag(e.GetPosition(_dragLayer));
            e.Handled = true;
        }

        private void OnPreviewPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_resizingSlot != null)
            {
                EndResize(commit: true);
                e.Handled = true;
                return;
            }

            if (!_isDragging)
            {
                // A press without a drag is a click, which the slot button handles.
                ClearPress();
                return;
            }

            UpdateDrag(e.GetPosition(_dragLayer));
            DropKind dropKind = _dropKind;
            int dropIndex = _dropIndex;
            ToolbarLayoutSlot? slot = _pressedSlot;
            int fromIndex = _pressedToolbarIndex;

            EndDrag();
            e.Handled = true;

            if (ViewModel is not { } vm || slot == null)
            {
                return;
            }

            if (dropKind == DropKind.Toolbar)
            {
                if (slot.Item is { } item)
                {
                    vm.PlaceItem(item, dropIndex);
                }
                else
                {
                    vm.PlaceSpacer(dropIndex, fromIndex >= 0 ? fromIndex : null, slot.ShowLine);
                }
            }
            else if (dropKind == DropKind.Palette)
            {
                vm.RemoveSlot(fromIndex);
            }
        }

        private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            if (_isDragging)
            {
                EndDrag();
            }

            if (_resizingSlot != null)
            {
                EndResize(commit: false);
            }
        }

        /// <summary>Starts resizing when a spacer on the toolbar is pressed by the grip at its right edge.</summary>
        private bool TryStartResize(PointerPressedEventArgs e)
        {
            if (ViewModel is not { } vm || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
                FindAncestor<Border>(e.Source, "spacer-grip") is null ||
                FindSlotButton(e.Source) is not { DataContext: ToolbarLayoutSlot { IsSpacer: true } slot } button)
            {
                return false;
            }

            int index = vm.ToolbarSlots.IndexOf(slot);
            if (index < 0)
            {
                return false;
            }

            _resizingSlot = slot;
            _resizingButton = button;
            _resizingIndex = index;
            _resizeStartWidth = slot.SpacerWidth;
            _resizeStartX = e.GetPosition(this).X;

            // The row may be shrunk to fit. Pointer movement is converted with the scale at the start, because the row
            // shrinks further as the spacer widens and following the live scale would make the spacer grow on its own.
            _resizeScale = button.Bounds.Width > 0 ? GetBounds(button).Width / button.Bounds.Width : 1;
            if (!(_resizeScale > 0))
            {
                _resizeScale = 1;
            }
            _resizingPointer = e.Pointer;

            // Capturing here and handling the press keeps the spacer button from being pressed or clicked.
            button.Classes.Add("resizing");
            e.Pointer.Capture(this);
            e.Handled = true;
            return true;
        }

        private void EndResize(bool commit)
        {
            ToolbarLayoutSlot? slot = _resizingSlot;
            IPointer? pointer = _resizingPointer;
            int index = _resizingIndex;
            int startWidth = _resizeStartWidth;

            _resizingButton?.Classes.Remove("resizing");
            _resizingSlot = null;
            _resizingButton = null;
            _resizingIndex = -1;
            _resizingPointer = null;

            if (slot != null)
            {
                if (commit)
                {
                    ViewModel?.SetSpacerWidth(index, slot.SpacerWidth);
                }
                else
                {
                    slot.SpacerWidth = startWidth;
                }
            }

            if (pointer?.Captured == this)
            {
                pointer.Capture(null);
            }
        }

        private void StartDrag(IPointer pointer)
        {
            if (_pressedSlot == null)
            {
                return;
            }

            _isDragging = true;

            if (ViewModel is { } vm && _pressedSlot.Item is { } item)
            {
                vm.SelectedItem = item;
            }

            // Taking the pointer from the slot button keeps it from raising Click when the drag ends.
            pointer.Capture(this);
            _pressedButton?.Classes.Add("dragging");

            _dragGhostIcon.Icon = _pressedSlot.Icon;
            _dragGhostIcon.IsVisible = _pressedSlot.IsItem;
            _dragGhostSpacer.IsVisible = _pressedSlot.IsSpacer && _pressedSlot.ShowLine;
            _dragGhostDash.IsVisible = _pressedSlot.IsSpacer && !_pressedSlot.ShowLine;
            _dragGhost.IsVisible = true;
        }

        private void UpdateDrag(Point point)
        {
            // Hang the dragged item below the pointer so the drop indicator at the pointer stays visible.
            Canvas.SetLeft(_dragGhost, point.X - _dragGhost.Width / 2);
            Canvas.SetTop(_dragGhost, point.Y + 14);

            _dropKind = DropKind.None;
            _dropIndicator.IsVisible = false;
            _paletteDropArea.Classes.Remove("drop-target");

            Rect visibleArea = GetBounds(_layoutScrollViewer);

            if (GetBounds(_toolbarDropArea).Inflate(ToolbarDropMargin).Intersect(visibleArea).Contains(point))
            {
                Rect indicator = FindToolbarGap(point, out _dropIndex);
                _dropKind = DropKind.Toolbar;
                _dropIndicator.Height = indicator.Height;
                Canvas.SetLeft(_dropIndicator, indicator.X);
                Canvas.SetTop(_dropIndicator, indicator.Y);
                _dropIndicator.IsVisible = true;
            }
            else if (_pressedToolbarIndex >= 0 && _pressedSlot is { CanRemove: true } &&
                GetBounds(_paletteDropArea).Intersect(visibleArea).Contains(point))
            {
                _dropKind = DropKind.Palette;
                _paletteDropArea.Classes.Add("drop-target");
            }
        }

        /// <summary>
        /// Finds the gap between toolbar slots nearest to <paramref name="point"/>, as the index of the slot after it,
        /// and returns where to draw the drop indicator.
        /// </summary>
        private Rect FindToolbarGap(Point point, out int slotIndex)
        {
            List<Rect> slots = new();
            int count = ViewModel?.ToolbarSlots.Count ?? 0;

            for (int i = 0; i < count; i++)
            {
                if (_toolbarSlotsControl.ContainerFromIndex(i) is not Control container)
                {
                    break;
                }

                slots.Add(GetBounds(container));
            }

            if (slots.Count == 0)
            {
                Rect area = GetBounds(_toolbarDropArea);
                slotIndex = 0;
                return new Rect(area.X + 4, area.Y + 4, _dropIndicator.Width, Math.Max(area.Height - 8, 0));
            }

            // The slots normally share one row; find the row nearest the pointer in case they ever do not.
            int rowStart = 0;
            int bestRowStart = 0;
            int bestRowEnd = 0;
            double bestDistance = double.MaxValue;

            for (int i = 1; i <= slots.Count; i++)
            {
                if (i < slots.Count && Math.Abs(slots[i].Y - slots[rowStart].Y) < 1)
                {
                    continue;
                }

                Rect first = slots[rowStart];
                double distance = point.Y < first.Top ? first.Top - point.Y : point.Y > first.Bottom ? point.Y - first.Bottom : 0;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestRowStart = rowStart;
                    bestRowEnd = i - 1;
                }

                rowStart = i;
            }

            Rect row = slots[bestRowStart];
            double indicatorHalfWidth = _dropIndicator.Width / 2;

            for (int i = bestRowStart; i <= bestRowEnd; i++)
            {
                if (point.X < slots[i].Center.X)
                {
                    slotIndex = i;
                    return new Rect(slots[i].Left - indicatorHalfWidth - 0.5, row.Y, _dropIndicator.Width, row.Height);
                }
            }

            slotIndex = bestRowEnd + 1;
            return new Rect(slots[bestRowEnd].Right - indicatorHalfWidth + 0.5, row.Y, _dropIndicator.Width, row.Height);
        }

        /// <summary>Gets where <paramref name="control"/> is drawn on the drag layer, including any shrink-to-fit scaling.</summary>
        private Rect GetBounds(Control control)
        {
            Point? topLeft = control.TranslatePoint(default, _dragLayer);
            Point? bottomRight = control.TranslatePoint(new Point(control.Bounds.Width, control.Bounds.Height), _dragLayer);
            return topLeft is { } first && bottomRight is { } second ? new Rect(first, second) : default;
        }

        private void EndDrag()
        {
            bool wasDragging = _isDragging;
            IPointer? pointer = _pressedPointer;

            _isDragging = false;
            _pressedButton?.Classes.Remove("dragging");
            _dragGhost.IsVisible = false;
            _dropIndicator.IsVisible = false;
            _paletteDropArea.Classes.Remove("drop-target");
            _dropKind = DropKind.None;
            ClearPress();

            if (wasDragging && pointer?.Captured == this)
            {
                pointer.Capture(null);
            }
        }

        private void ClearPress()
        {
            _pressedButton = null;
            _pressedSlot = null;
            _pressedToolbarIndex = -1;
            _pressedPointer = null;
        }

        private Button? FindSlotButton(object? source) => FindAncestor<Button>(source, "slot");

        private T? FindAncestor<T>(object? source, string className) where T : Control
        {
            for (Visual? visual = source as Visual; visual != null && visual != this; visual = visual.GetVisualParent())
            {
                if (visual is T control && control.Classes.Contains(className))
                {
                    return control;
                }
            }

            return null;
        }

        #endregion Dragging

        private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
        {
            if (_isDragging || _resizingSlot != null)
            {
                if (e.Key == Key.Escape)
                {
                    if (_isDragging)
                    {
                        EndDrag();
                    }
                    else
                    {
                        EndResize(commit: false);
                    }

                    e.Handled = true;
                }

                return;
            }

            if (e.Source is Button { DataContext: ToolbarLayoutSlot slot } slotButton && slotButton.Classes.Contains("toolbar-slot"))
            {
                OnToolbarSlotKeyDown(slot, e);
                return;
            }

            if (e.Source is not TextBox { DataContext: ToolbarCustomizationItemViewModel { IsHotkeyEditable: true } item } textBox)
            {
                return;
            }

            bool isSecondary = textBox.Tag is SecondaryHotkeyTag;

            if (e.Key is Key.Back or Key.Delete || e.Key.ToString().Equals("Backspace", StringComparison.OrdinalIgnoreCase))
            {
                SetHotkey(item, isSecondary, "");
                e.Handled = true;
                return;
            }

            if (IsModifierKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            SetHotkey(item, isSecondary, ToolbarHotkeyHelper.Format(e.Key, e.KeyModifiers));
            e.Handled = true;
        }

        /// <summary>
        /// Delete takes the focused slot off the toolbar, Ctrl+Left or Ctrl+Right moves the focused item, and Shift+Left
        /// or Shift+Right narrows or widens the focused spacer.
        /// </summary>
        private void OnToolbarSlotKeyDown(ToolbarLayoutSlot slot, KeyEventArgs e)
        {
            if (ViewModel is not { } vm)
            {
                return;
            }

            int index = vm.ToolbarSlots.IndexOf(slot);

            if (e.Key == Key.Delete && slot.CanRemove)
            {
                vm.RemoveSlot(index);
                FocusToolbarSlot(Math.Min(index, vm.ToolbarSlots.Count - 1));
                e.Handled = true;
            }
            else if (slot.Item is { } item && e.KeyModifiers == KeyModifiers.Control && e.Key is Key.Left or Key.Right)
            {
                if (e.Key == Key.Left)
                {
                    vm.MoveLeft(item);
                }
                else
                {
                    vm.MoveRight(item);
                }

                FocusToolbarSlot(vm.IndexOfSlot(item));
                e.Handled = true;
            }
            else if (slot.IsSpacer && e.KeyModifiers == KeyModifiers.Shift && e.Key is Key.Left or Key.Right)
            {
                int step = e.Key == Key.Left ? -SpacerWidthKeyStep : SpacerWidthKeyStep;
                vm.SetSpacerWidth(index, slot.SpacerWidth + step);
                e.Handled = true;
            }
        }

        private void FocusToolbarSlot(int index)
        {
            if (index < 0)
            {
                return;
            }

            // The slots are rebuilt after each change, so wait for the new buttons to be created.
            Dispatcher.UIThread.Post(() =>
            {
                if (_toolbarSlotsControl.ContainerFromIndex(index) is Control container &&
                    container.GetVisualDescendants().OfType<Button>().FirstOrDefault() is { } button)
                {
                    button.Focus(NavigationMethod.Directional);
                }
            }, DispatcherPriority.Loaded);
        }

        private static void SetHotkey(ToolbarCustomizationItemViewModel item, bool isSecondary, string hotkey)
        {
            if (isSecondary)
            {
                item.SecondaryHotkey = hotkey;
            }
            else
            {
                item.Hotkey = hotkey;
            }
        }

        private static bool IsModifierKey(Key key)
        {
            return key is Key.LeftShift or Key.RightShift
                or Key.LeftCtrl or Key.RightCtrl
                or Key.LeftAlt or Key.RightAlt
                or Key.LWin or Key.RWin;
        }
    }
}
