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
    private readonly Action<IReadOnlyList<ToolbarCustomizationItemViewModel>> _onSave;
    private readonly Action _onCancel;
    private string _validationMessage = "";
    private ToolbarCustomizationItemViewModel? _selectedItem;

    public ToolbarCustomizationDialogViewModel(
        IEnumerable<ToolbarCustomizationItemViewModel> sourceItems,
        Action<IReadOnlyList<ToolbarCustomizationItemViewModel>> onSave,
        Action onCancel)
    {
        _onSave = onSave;
        _onCancel = onCancel;
        Items = new ObservableCollection<ToolbarCustomizationItemViewModel>(sourceItems.Select(item => item.Clone()));
        Items.CollectionChanged += OnItemsCollectionChanged;

        foreach (ToolbarCustomizationItemViewModel item in Items)
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }

        ResetCommand = new RelayCommand(Reset);
        OkCommand = new RelayCommand(Ok, () => CanSave);
        CancelCommand = new RelayCommand(Cancel);

        RefreshMoveStates();
        RefreshValidation();
    }

    public ObservableCollection<ToolbarCustomizationItemViewModel> Items { get; }
    public IRelayCommand ResetCommand { get; }
    public IRelayCommand OkCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public ToolbarCustomizationItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
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

    public void MoveUp(ToolbarCustomizationItemViewModel? item)
    {
        if (item == null)
        {
            return;
        }

        int index = Items.IndexOf(item);
        if (index <= 0)
        {
            return;
        }

        Items.Move(index, index - 1);
        SelectedItem = item;
        RefreshMoveStates();
    }

    public void MoveDown(ToolbarCustomizationItemViewModel? item)
    {
        if (item == null)
        {
            return;
        }

        int index = Items.IndexOf(item);
        if (index < 0 || index >= Items.Count - 1)
        {
            return;
        }

        Items.Move(index, index + 1);
        SelectedItem = item;
        RefreshMoveStates();
    }

    private void Reset()
    {
        foreach (ToolbarCustomizationItemViewModel item in Items)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
        }

        Items.Clear();

        foreach (ToolbarCustomizationItemViewModel item in ToolbarCustomizationItemViewModel.CreateDefaultItems())
        {
            Items.Add(item);
        }

        SelectedItem = Items.FirstOrDefault();
        RefreshMoveStates();
        RefreshValidation();
    }

    private void Ok()
    {
        if (!CanSave)
        {
            return;
        }

        foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => item.IsHotkeyEditable && !string.IsNullOrWhiteSpace(item.Hotkey)))
        {
            item.Hotkey = ToolbarHotkeyHelper.Normalize(item.Hotkey);
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

        RefreshMoveStates();
        RefreshValidation();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ToolbarCustomizationItemViewModel.Hotkey))
        {
            RefreshValidation();
        }
    }

    private void RefreshMoveStates()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].CanMoveUp = i > 0;
            Items[i].CanMoveDown = i < Items.Count - 1;
        }
    }

    private void RefreshValidation()
    {
        Dictionary<string, ToolbarCustomizationItemViewModel> usedHotkeys = new(StringComparer.OrdinalIgnoreCase);

        foreach (ToolbarCustomizationItemViewModel item in Items.Where(item => item.IsHotkeyEditable))
        {
            string hotkey = item.Hotkey.Trim();
            if (hotkey.Length == 0)
            {
                continue;
            }

            if (!ToolbarHotkeyHelper.TryParse(hotkey, out _, out _))
            {
                ValidationMessage = string.Format(Strings.ToolbarCustomizationDialogView_InvalidHotkey, item.Name);
                return;
            }

            string normalizedHotkey = ToolbarHotkeyHelper.Normalize(hotkey);
            if (usedHotkeys.TryGetValue(normalizedHotkey, out ToolbarCustomizationItemViewModel? existingItem))
            {
                ValidationMessage = $"{item.Name} and {existingItem.Name} use the same hotkey ({normalizedHotkey}).";
                return;
            }

            usedHotkeys[normalizedHotkey] = item;
        }

        ValidationMessage = "";
    }
}
