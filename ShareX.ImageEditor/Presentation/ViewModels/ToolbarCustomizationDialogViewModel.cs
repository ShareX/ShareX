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

using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Localization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.ViewModels;

public sealed class ToolbarCustomizationDialogViewModel : ViewModelBase
{
    /// <summary>An item, or a spacer of the given width and line when <see cref="Item"/> is null.</summary>
    private readonly record struct LayoutToken(ToolbarCustomizationItemViewModel? Item, int SpacerWidth = 0, bool ShowLine = true)
    {
        public bool IsSpacer => Item == null;
    }

    private readonly Action<IReadOnlyList<ToolbarCustomizationItemViewModel>> _onSave;
    private readonly Action _onCancel;
    private readonly Func<IReadOnlyList<ToolbarCustomizationItemViewModel>> _createDefaultItems;
    private readonly Dictionary<string, int> _defaultOrder;
    private string _validationMessage = "";
    private ToolbarCustomizationItemViewModel? _selectedItem;
    private ToolbarCustomizationItemViewModel? _selectedSpacerItem;
    private bool _isUpdatingLayout;

    public ToolbarCustomizationDialogViewModel(
        IEnumerable<ToolbarCustomizationItemViewModel> sourceItems,
        Action<IReadOnlyList<ToolbarCustomizationItemViewModel>> onSave,
        Action onCancel,
        Func<IReadOnlyList<ToolbarCustomizationItemViewModel>>? createDefaultItems = null)
    {
        _onSave = onSave;
        _onCancel = onCancel;
        _createDefaultItems = createDefaultItems ?? ToolbarCustomizationItemViewModel.CreateDefaultItems;
        _defaultOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (ToolbarCustomizationItemViewModel item in _createDefaultItems())
        {
            _defaultOrder.TryAdd(item.Id, _defaultOrder.Count);
        }

        Items = new ObservableCollection<ToolbarCustomizationItemViewModel>(sourceItems.Select(item => item.Clone()));
        Items.CollectionChanged += OnItemsCollectionChanged;

        foreach (ToolbarCustomizationItemViewModel item in Items)
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }

        ResetCommand = new RelayCommand(Reset);
        OkCommand = new RelayCommand(Ok, () => CanSave);
        CancelCommand = new RelayCommand(Cancel);

        RebuildLayout();
        SelectedItem = Items.FirstOrDefault(item => item.IsVisible);
        RefreshValidation();
    }

    /// <summary>Every item in toolbar order. Once the layout is edited, hidden items follow the visible ones.</summary>
    public ObservableCollection<ToolbarCustomizationItemViewModel> Items { get; }

    /// <summary>The visible items and the spacers between them, as the toolbar shows them.</summary>
    public ObservableCollection<ToolbarLayoutSlot> ToolbarSlots { get; } = new();

    /// <summary>A spacer with a line, one without, and the hidden items, all of which can be dragged onto the toolbar.</summary>
    public ObservableCollection<ToolbarLayoutSlot> AvailableSlots { get; } = new();

    public bool HasHiddenItems => AvailableSlots.Any(slot => slot.IsItem);

    public IRelayCommand ResetCommand { get; }
    public IRelayCommand OkCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public ToolbarCustomizationItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnPropertyChanged(nameof(HasSelectedItem));

                if (value != null)
                {
                    SelectedSpacerItem = null;
                }

                RefreshSelection();
            }
        }
    }

    public bool HasSelectedItem => SelectedItem != null;

    /// <summary>
    /// The item after the selected spacer, which stores the spacer's width, or null when no spacer is selected. Only
    /// one of this and <see cref="SelectedItem"/> is set at a time.
    /// </summary>
    public ToolbarCustomizationItemViewModel? SelectedSpacerItem
    {
        get => _selectedSpacerItem;
        private set
        {
            if (SetProperty(ref _selectedSpacerItem, value))
            {
                OnPropertyChanged(nameof(HasSelectedSpacer));
                OnPropertyChanged(nameof(SelectedSpacerName));

                if (value != null)
                {
                    SelectedItem = null;
                }

                RefreshSelection();
            }
        }
    }

    public bool HasSelectedSpacer => SelectedSpacerItem != null;

    public string SelectedSpacerName => SelectedSpacerItem?.ShowSpacerLine == false
        ? Strings.ToolbarCustomizationDialogView_BlankSpacer
        : Strings.ToolbarCustomizationDialogView_Spacer;

    /// <summary>Selects the spacer at <paramref name="slotIndex"/> on the toolbar so its width can be typed in.</summary>
    public void SelectSpacer(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < ToolbarSlots.Count - 1 && ToolbarSlots[slotIndex].IsSpacer &&
            ToolbarSlots[slotIndex + 1].Item is { } item)
        {
            SelectedSpacerItem = item;
        }
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        private set
        {
            if (SetProperty(ref _validationMessage, value))
            {
                OnPropertyChanged(nameof(HasValidationMessage));
                OnPropertyChanged(nameof(CanSave));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public bool HasValidationMessage => !string.IsNullOrEmpty(ValidationMessage);
    public bool CanSave => !HasValidationMessage;

    /// <summary>
    /// Puts <paramref name="item"/> on the toolbar before the slot at <paramref name="slotIndex"/>, or at the end when
    /// the index equals the slot count. An item already on the toolbar moves there.
    /// </summary>
    public void PlaceItem(ToolbarCustomizationItemViewModel item, int slotIndex)
    {
        List<LayoutToken> tokens = GetLayoutTokens();
        int oldIndex = tokens.FindIndex(token => token.Item == item);

        if (oldIndex >= 0)
        {
            tokens.RemoveAt(oldIndex);

            if (oldIndex < slotIndex)
            {
                slotIndex--;
            }
        }

        tokens.Insert(Math.Clamp(slotIndex, 0, tokens.Count), new LayoutToken(item));
        ApplyLayoutTokens(tokens);
        SelectedItem = item;
    }

    /// <summary>
    /// Puts a spacer before the slot at <paramref name="slotIndex"/> and selects it. When <paramref
    /// name="fromSlotIndex"/> is a spacer on the toolbar, that spacer moves, keeping its width and line, instead of a
    /// new one being added with <paramref name="showLine"/>.
    /// </summary>
    public void PlaceSpacer(int slotIndex, int? fromSlotIndex = null, bool showLine = true)
    {
        List<LayoutToken> tokens = GetLayoutTokens();
        LayoutToken spacer = new(null, ShowLine: showLine);

        if (fromSlotIndex is int fromIndex && fromIndex >= 0 && fromIndex < tokens.Count && tokens[fromIndex].IsSpacer)
        {
            spacer = tokens[fromIndex];
            tokens.RemoveAt(fromIndex);

            if (fromIndex < slotIndex)
            {
                slotIndex--;
            }
        }

        slotIndex = Math.Clamp(slotIndex, 0, tokens.Count);
        tokens.Insert(slotIndex, spacer);

        // The spacer is stored on the next item, merging with any spacers in between. At the end it is dropped.
        ToolbarCustomizationItemViewModel? nextItem = tokens.Skip(slotIndex + 1).FirstOrDefault(token => !token.IsSpacer).Item;
        ApplyLayoutTokens(tokens);

        if (nextItem != null)
        {
            SelectedSpacerItem = nextItem;
        }
    }

    /// <summary>Sets the extra space of the spacer at <paramref name="slotIndex"/>.</summary>
    public void SetSpacerWidth(int slotIndex, int width)
    {
        if (slotIndex < 0 || slotIndex >= ToolbarSlots.Count - 1 || !ToolbarSlots[slotIndex].IsSpacer ||
            ToolbarSlots[slotIndex + 1].Item is not { } item)
        {
            return;
        }

        // The item after a spacer stores its width.
        item.SpacerWidth = width;
        ToolbarSlots[slotIndex].SpacerWidth = item.SpacerWidth;
    }

    /// <summary>Takes the spacer or item at <paramref name="slotIndex"/> off the toolbar. Items taken off are hidden.</summary>
    public void RemoveSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= ToolbarSlots.Count || !ToolbarSlots[slotIndex].CanRemove)
        {
            return;
        }

        List<LayoutToken> tokens = GetLayoutTokens();
        tokens.RemoveAt(slotIndex);
        ApplyLayoutTokens(tokens);
    }

    public void MoveLeft(ToolbarCustomizationItemViewModel? item)
    {
        int index = IndexOfSlot(item);
        if (item != null && index > 0)
        {
            PlaceItem(item, index - 1);
        }
    }

    public void MoveRight(ToolbarCustomizationItemViewModel? item)
    {
        int index = IndexOfSlot(item);
        if (item != null && index >= 0 && index < ToolbarSlots.Count - 1)
        {
            PlaceItem(item, index + 2);
        }
    }

    public int IndexOfSlot(ToolbarCustomizationItemViewModel? item)
    {
        if (item == null)
        {
            return -1;
        }

        for (int i = 0; i < ToolbarSlots.Count; i++)
        {
            if (ToolbarSlots[i].Item == item)
            {
                return i;
            }
        }

        return -1;
    }

    private List<LayoutToken> GetLayoutTokens()
    {
        return ToolbarSlots.Select(slot => new LayoutToken(slot.Item, slot.SpacerWidth, slot.ShowLine)).ToList();
    }

    /// <summary>
    /// Writes a toolbar layout back to the items. A spacer becomes <see
    /// cref="ToolbarCustomizationItemViewModel.BeginGroup"/> and <see cref="ToolbarCustomizationItemViewModel.SpacerWidth"/>
    /// on the item after it, so neighboring spacers merge into the widest of them, with a line if either has one, and a
    /// spacer at the end, with no item after it, is dropped. Items left out of the layout are hidden.
    /// </summary>
    private void ApplyLayoutTokens(IReadOnlyList<LayoutToken> tokens)
    {
        _isUpdatingLayout = true;

        try
        {
            List<ToolbarCustomizationItemViewModel> order = new();
            bool beginGroup = false;
            int spacerWidth = 0;
            bool showLine = true;

            foreach (LayoutToken token in tokens)
            {
                if (token.Item is not { } item)
                {
                    spacerWidth = beginGroup ? Math.Max(spacerWidth, token.SpacerWidth) : token.SpacerWidth;
                    showLine = beginGroup ? showLine || token.ShowLine : token.ShowLine;
                    beginGroup = true;
                    continue;
                }

                item.IsVisible = true;
                item.BeginGroup = beginGroup;
                item.SpacerWidth = beginGroup ? spacerWidth : 0;
                item.ShowSpacerLine = !beginGroup || showLine;
                beginGroup = false;
                order.Add(item);
            }

            HashSet<ToolbarCustomizationItemViewModel> visibleItems = new(order);
            foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => !visibleItems.Contains(item)).OrderBy(GetDefaultOrder).ToList())
            {
                item.IsVisible = false;
                item.BeginGroup = false;
                item.SpacerWidth = 0;
                item.ShowSpacerLine = true;
                order.Add(item);
            }

            for (int i = 0; i < order.Count; i++)
            {
                int currentIndex = Items.IndexOf(order[i]);
                if (currentIndex != i)
                {
                    Items.Move(currentIndex, i);
                }
            }
        }
        finally
        {
            _isUpdatingLayout = false;
        }

        RebuildLayout();
    }

    private int GetDefaultOrder(ToolbarCustomizationItemViewModel item)
    {
        return _defaultOrder.TryGetValue(item.Id, out int index) ? index : int.MaxValue;
    }

    private void RebuildLayout()
    {
        ToolbarSlots.Clear();

        foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => item.IsVisible))
        {
            if (item.BeginGroup)
            {
                ToolbarSlots.Add(new ToolbarLayoutSlot(null, item.SpacerWidth, item.ShowSpacerLine));
            }

            ToolbarSlots.Add(new ToolbarLayoutSlot(item));
        }

        AvailableSlots.Clear();
        AvailableSlots.Add(new ToolbarLayoutSlot(null));
        AvailableSlots.Add(new ToolbarLayoutSlot(null, showLine: false));

        foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => !item.IsVisible).OrderBy(GetDefaultOrder))
        {
            AvailableSlots.Add(new ToolbarLayoutSlot(item));
        }

        OnPropertyChanged(nameof(HasHiddenItems));
        RefreshSelection();
    }

    private void Reset()
    {
        _isUpdatingLayout = true;

        try
        {
            // Clear() reports no old items, so stop listening to them first.
            foreach (ToolbarCustomizationItemViewModel item in Items)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }

            Items.Clear();

            foreach (ToolbarCustomizationItemViewModel item in _createDefaultItems())
            {
                Items.Add(item);
            }
        }
        finally
        {
            _isUpdatingLayout = false;
        }

        RebuildLayout();
        SelectedItem = Items.FirstOrDefault(item => item.IsVisible);
        RefreshValidation();
    }

    private void Ok()
    {
        if (!CanSave)
        {
            return;
        }

        foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => item.IsHotkeyEditable))
        {
            if (!string.IsNullOrWhiteSpace(item.Hotkey))
            {
                item.Hotkey = ToolbarHotkeyHelper.Normalize(item.Hotkey);
            }

            if (!string.IsNullOrWhiteSpace(item.SecondaryHotkey))
            {
                item.SecondaryHotkey = ToolbarHotkeyHelper.Normalize(item.SecondaryHotkey);
            }
        }

        _onSave(Items.Select(item => item.Clone()).ToList());
    }

    private void Cancel()
    {
        _onCancel();
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (ToolbarCustomizationItemViewModel item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (ToolbarCustomizationItemViewModel item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }

        if (!_isUpdatingLayout)
        {
            RebuildLayout();
            RefreshValidation();
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ToolbarCustomizationItemViewModel.Hotkey) or nameof(ToolbarCustomizationItemViewModel.SecondaryHotkey))
        {
            RefreshValidation();
        }
        else if (e.PropertyName is nameof(ToolbarCustomizationItemViewModel.IsVisible) or nameof(ToolbarCustomizationItemViewModel.BeginGroup)
            && !_isUpdatingLayout)
        {
            RebuildLayout();
        }
        else if (e.PropertyName is nameof(ToolbarCustomizationItemViewModel.SpacerWidth) or nameof(ToolbarCustomizationItemViewModel.ShowSpacerLine)
            && sender is ToolbarCustomizationItemViewModel item)
        {
            // Update the spacer in place so the slot being resized keeps its button.
            int index = IndexOfSlot(item);
            if (index > 0 && ToolbarSlots[index - 1].IsSpacer)
            {
                ToolbarSlots[index - 1].SpacerWidth = item.SpacerWidth;
                ToolbarSlots[index - 1].ShowLine = item.ShowSpacerLine;
            }
        }
    }

    private void RefreshSelection()
    {
        // A selected spacer that was moved, merged or removed is no longer selected.
        if (SelectedSpacerItem is { } spacerItem && !(spacerItem.IsVisible && spacerItem.BeginGroup))
        {
            SelectedSpacerItem = null;
            return;
        }

        foreach (ToolbarLayoutSlot slot in AvailableSlots)
        {
            slot.IsSelected = slot.Item != null && slot.Item == SelectedItem;
        }

        for (int i = 0; i < ToolbarSlots.Count; i++)
        {
            ToolbarLayoutSlot slot = ToolbarSlots[i];
            slot.IsSelected = slot.Item is { } item
                ? item == SelectedItem
                : SelectedSpacerItem != null && i + 1 < ToolbarSlots.Count && ToolbarSlots[i + 1].Item == SelectedSpacerItem;
        }
    }

    private void RefreshValidation()
    {
        Dictionary<string, ToolbarCustomizationItemViewModel> usedHotkeys = new(StringComparer.OrdinalIgnoreCase);

        foreach (ToolbarCustomizationItemViewModel item in Items)
        {
            foreach (string hotkey in item.Hotkeys)
            {
                if (!ToolbarHotkeyHelper.TryParse(hotkey, out _, out _))
                {
                    ValidationMessage = string.Format(Strings.ToolbarCustomizationDialogView_InvalidHotkey, item.Name);
                    return;
                }

                string normalizedHotkey = ToolbarHotkeyHelper.Normalize(hotkey);
                if (usedHotkeys.TryGetValue(normalizedHotkey, out ToolbarCustomizationItemViewModel? existingItem) && existingItem != item)
                {
                    ValidationMessage = string.Format(Strings.ToolbarCustomizationDialogView_DuplicateHotkey, item.Name, existingItem.Name, normalizedHotkey);
                    return;
                }

                usedHotkeys[normalizedHotkey] = item;
            }
        }

        ValidationMessage = "";
    }
}
