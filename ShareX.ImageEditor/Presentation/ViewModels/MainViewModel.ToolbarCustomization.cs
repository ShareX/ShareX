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

using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ToolbarCustomizationItemViewModel> _toolbarItems = new();
        private readonly ObservableCollection<ToolbarCustomizationItemViewModel> _visibleToolbarItems = new();
        private bool _hostToolbarToolsActive = true;

        public ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel> ToolbarItems { get; private set; } = null!;
        public ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel> VisibleToolbarItems { get; private set; } = null!;
        public event EventHandler? FileMenuRequested;

        private void InitializeToolbarCustomization()
        {
            ToolbarItems = new ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel>(_toolbarItems);
            VisibleToolbarItems = new ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel>(_visibleToolbarItems);
            ApplyToolbarCustomizationItems(
                ToolbarCustomizationItemViewModel.CreateFromOptions(Options.ToolbarItems),
                persist: Options.ToolbarItems.Count == 0);
        }

        [RelayCommand]
        private void OpenToolbarCustomizationDialog()
        {
            if (IsModalOpen)
            {
                return;
            }

            var dialog = new ToolbarCustomizationDialogViewModel(
                _toolbarItems,
                items =>
                {
                    ApplyToolbarCustomizationItems(items, persist: true);
                    CloseModalCommand.Execute(null);
                },
                () => CloseModalCommand.Execute(null));

            ModalContent = dialog;
            IsModalOpen = true;
        }

        internal bool TrySelectToolForToolbarHotkey(Key key, KeyModifiers modifiers)
        {
            foreach (ToolbarCustomizationItemViewModel item in _toolbarItems)
            {
                if (item.IsVisible && item.IsHotkeyEditable && ToolbarHotkeyHelper.Matches(item.Hotkey, key, modifiers))
                {
                    ExecuteToolbarItem(item);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applies a host-specific toolbar filter without persisting it to user settings.
        /// Hosts such as region capture can expose the shared annotation tools while
        /// excluding image-mutating editor commands whose coordinate semantics do not apply.
        /// </summary>
        public void SetHostToolbarFilter(Func<ToolbarCustomizationItemViewModel, bool>? predicate)
        {
            foreach (ToolbarCustomizationItemViewModel item in _toolbarItems)
            {
                item.IsVisible = predicate?.Invoke(item) ?? true;
            }

            RefreshVisibleToolbarItems();
            OnPropertyChanged(nameof(ToolbarItems));
            OnPropertyChanged(nameof(VisibleToolbarItems));
        }

        /// <summary>Lets an embedding host display its own mutually-exclusive tool mode.</summary>
        public void SetHostToolbarToolsActive(bool active)
        {
            if (_hostToolbarToolsActive == active)
            {
                return;
            }

            _hostToolbarToolsActive = active;
            RefreshToolbarItemActiveStates();
        }

        internal void ExecuteToolbarItem(ToolbarCustomizationItemViewModel item)
        {
            if (item.Tool.HasValue)
            {
                SelectToolCommand.Execute(item.Tool.Value);
                return;
            }

            switch (item.Id)
            {
                case ToolbarCustomizationItemViewModel.FileItemId:
                    FileMenuRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case ToolbarCustomizationItemViewModel.BackgroundItemId:
                    ToggleSettingsPanelCommand.Execute(null);
                    break;
                case ToolbarCustomizationItemViewModel.ImageEffectsItemId:
                    if (ToggleEffectsPanelCommand.CanExecute(null))
                    {
                        ToggleEffectsPanelCommand.Execute(null);
                    }
                    break;
            }
        }

        private void ApplyToolbarCustomizationItems(IEnumerable<ToolbarCustomizationItemViewModel> items, bool persist)
        {
            _toolbarItems.Clear();

            foreach (ToolbarCustomizationItemViewModel item in items.Select(item => item.Clone()))
            {
                _toolbarItems.Add(item);
            }

            UpdateToolbarActiveStates();
            RefreshVisibleToolbarItems();
            OnPropertyChanged(nameof(ToolbarItems));
            OnPropertyChanged(nameof(VisibleToolbarItems));

            if (persist)
            {
                Options.ToolbarItems = _toolbarItems.Select(item => item.ToOptions()).ToList();
            }
        }

        private void UpdateToolbarActiveStates()
        {
            foreach (ToolbarCustomizationItemViewModel item in _toolbarItems)
            {
                item.IsActive = item.Tool.HasValue
                    ? _hostToolbarToolsActive && item.Tool.Value == ActiveTool
                    : item.Id switch
                    {
                        ToolbarCustomizationItemViewModel.BackgroundItemId => IsSettingsPanelOpen,
                        ToolbarCustomizationItemViewModel.ImageEffectsItemId => IsEffectsPanelOpen,
                        _ => false
                    };
            }
        }

        private void RefreshVisibleToolbarItems()
        {
            _visibleToolbarItems.Clear();

            foreach (ToolbarCustomizationItemViewModel item in _toolbarItems.Where(item => item.IsVisible))
            {
                _visibleToolbarItems.Add(item);
            }
        }

        partial void OnIsEffectsPanelOpenChanged(bool value)
        {
            RefreshToolbarItemActiveStates();
        }

        partial void OnEffectsPanelContentChanged(object? value)
        {
            RefreshToolbarItemActiveStates();
        }

        private void RefreshToolbarItemActiveStates()
        {
            UpdateToolbarActiveStates();
            OnPropertyChanged(nameof(ToolbarItems));
            OnPropertyChanged(nameof(VisibleToolbarItems));
        }
    }
}
