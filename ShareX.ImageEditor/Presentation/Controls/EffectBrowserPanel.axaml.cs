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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Core.ImageEffects;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Effects;
using System.Collections.ObjectModel;
using System.Text;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public sealed class EffectDialogRequestedEventArgs : EventArgs
    {
        public string EffectId { get; }
        public EffectDialogRequestedEventArgs(string effectId) => EffectId = effectId;
    }

    public partial class EffectCategory : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        private readonly bool _showCount;
        private readonly bool _keepVisibleWhenEmpty;
        private readonly string? _headerHint;

        public ObservableCollection<EffectItem> AllEffects { get; } = new();

        [ObservableProperty]
        private ObservableCollection<EffectItem> _visibleEffects = new();

        [ObservableProperty]
        private bool _isVisible = true;

        public EffectCategory(string name, bool showCount = true, bool keepVisibleWhenEmpty = false, string? headerHint = null)
        {
            Name = name;
            _showCount = showCount;
            _keepVisibleWhenEmpty = keepVisibleWhenEmpty;
            _headerHint = headerHint;
        }

        public string HeaderText
        {
            get
            {
                return _showCount ? $"{Name} ({AllEffects.Count})" : Name;
            }
        }

        public string HeaderHint => _headerHint ?? string.Empty;

        public bool HasHeaderHint => !string.IsNullOrWhiteSpace(_headerHint);

        partial void OnNameChanged(string value)
        {
            OnPropertyChanged(nameof(HeaderText));
        }

        public EffectItem AddEffect(string name, string icon, string description, Action execute, string? effectId = null, bool keepSorted = true)
        {
            var effect = new EffectItem(name, icon, description, execute, effectId);
            Insert(AllEffects, effect, keepSorted);
            Insert(VisibleEffects, effect, keepSorted);
            OnPropertyChanged(nameof(HeaderText));
            return effect;
        }

        public EffectItem AddEffectCopy(EffectItem effect, bool keepSorted = true)
            => AddEffect(effect.Name, effect.Icon, effect.Description, effect.ExecuteAction, effect.EffectId, keepSorted)
                .WithExecuteObserver(effect.ExecuteObserver);

        public void ClearEffects()
        {
            AllEffects.Clear();
            VisibleEffects.Clear();
            OnPropertyChanged(nameof(HeaderText));
        }

        public bool RemoveEffectsById(string effectId)
        {
            bool removed = false;

            for (int i = AllEffects.Count - 1; i >= 0; i--)
            {
                if (string.Equals(AllEffects[i].EffectId, effectId, StringComparison.OrdinalIgnoreCase))
                {
                    AllEffects.RemoveAt(i);
                    removed = true;
                }
            }

            for (int i = VisibleEffects.Count - 1; i >= 0; i--)
            {
                if (string.Equals(VisibleEffects[i].EffectId, effectId, StringComparison.OrdinalIgnoreCase))
                {
                    VisibleEffects.RemoveAt(i);
                }
            }

            if (removed)
            {
                OnPropertyChanged(nameof(HeaderText));
            }

            return removed;
        }

        private static void Insert(ObservableCollection<EffectItem> target, EffectItem effect, bool keepSorted)
        {
            if (!keepSorted)
            {
                target.Add(effect);
                return;
            }

            int index = 0;
            while (index < target.Count &&
                   string.Compare(target[index].Name, effect.Name, StringComparison.OrdinalIgnoreCase) <= 0)
            {
                index++;
            }

            target.Insert(index, effect);
        }

        public void Filter(string searchText)
        {
            VisibleEffects.Clear();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                foreach (var effect in AllEffects) VisibleEffects.Add(effect);
            }
            else
            {
                foreach (var effect in AllEffects)
                {
                    if (effect.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        effect.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        VisibleEffects.Add(effect);
                    }
                }
            }

            IsVisible = VisibleEffects.Count > 0 || (_keepVisibleWhenEmpty && string.IsNullOrWhiteSpace(searchText));
        }
    }

    public partial class EffectItem : ObservableObject
    {
        [ObservableProperty]
        private string _effectId;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _icon;

        [ObservableProperty]
        private string _description;

        public Action ExecuteAction { get; }
        public Action<EffectItem>? ExecuteObserver { get; set; }

        public EffectItem(string name, string icon, string description, Action executeAction, string? effectId = null)
        {
            EffectId = string.IsNullOrWhiteSpace(effectId) ? NormalizeEffectId(name) : effectId;
            Name = name;
            Icon = icon;
            Description = description;
            ExecuteAction = executeAction;
        }

        public EffectItem WithExecuteObserver(Action<EffectItem>? executeObserver)
        {
            ExecuteObserver = executeObserver;
            return this;
        }

        public static string NormalizeEffectId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(value.Length);
            bool lastWasUnderscore = false;

            foreach (char c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    lastWasUnderscore = false;
                    continue;
                }

                if (!lastWasUnderscore)
                {
                    sb.Append('_');
                    lastWasUnderscore = true;
                }
            }

            while (sb.Length > 0 && sb[0] == '_')
            {
                sb.Remove(0, 1);
            }

            while (sb.Length > 0 && sb[^1] == '_')
            {
                sb.Length--;
            }

            return sb.ToString();
        }

        [RelayCommand]
        private void Execute()
        {
            ExecuteAction?.Invoke();
            ExecuteObserver?.Invoke(this);
        }
    }

    public partial class EffectBrowserPanel : UserControl
    {
        private const int MaxRecentEffects = 10;
        private static readonly FontFamily IconFontFamily = new("avares://ShareX.Avalonia/Assets#lucide");

        private static readonly Dictionary<string, string> EffectAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["resize"] = "resize_image",
            ["canvas"] = "resize_canvas",
            ["crop"] = "crop_image",
            ["auto_crop"] = "auto_crop_image",
            // Back-compat for older persisted ids (e.g. favorites/recent) which exposed a broken host shortcut.
            ["editor_auto_crop"] = "auto_crop_image",
            ["rotate"] = "rotate_custom_angle",
            ["rotate_90"] = "rotate_90_clockwise",
            ["rotate_90_cc"] = "rotate_90_counter_clockwise"
        };

        public event EventHandler<EffectDialogRequestedEventArgs>? EffectDialogRequested;

        public ObservableCollection<EffectCategory> Categories { get; } = new();

        private readonly EffectCategory _recentCategory = new(Strings.EffectBrowserPanel_Recent, headerHint: Strings.EffectBrowserPanel_RecentHint);
        private readonly EffectCategory _favoritesCategory = new(Strings.EffectBrowserPanel_Favorites, headerHint: Strings.EffectBrowserPanel_FavoritesHint);
        private readonly Dictionary<string, EffectItem> _allEffectsById = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _recentEffectIds = new();
        private readonly HashSet<string> _favoriteEffectIds = new(StringComparer.OrdinalIgnoreCase);
        private ImageEditorOptions? _options;

        public EffectBrowserPanel()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeEffects();
            BuildEffectLookup();
            LoadFavoriteEffects(ImageEditorOptions.DefaultFavoriteEffects, persistToOptions: false);

            var categoriesControl = this.FindControl<ItemsControl>("CategoriesControl");
            if (categoriesControl != null)
            {
                categoriesControl.ItemsSource = Categories;
            }

            UpdateSearchWatermark();
        }

        public void SetOptions(ImageEditorOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            LoadRecentEffects(options.RecentEffects, persistToOptions: true);
            LoadFavoriteEffects(options.FavoriteEffects, persistToOptions: true);
        }

        public IReadOnlyList<MenuItem> CreateFavoriteMenuItems()
        {
            return _favoritesCategory.AllEffects
                .OrderBy(effect => effect.Name, StringComparer.OrdinalIgnoreCase)
                .Select(CreateFavoriteMenuItem)
                .ToList();
        }

        private void OnSearchTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            ApplyCurrentFilter();
        }

        public void FocusSearchBox()
        {
            var searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox != null)
            {
                // Clear existing search
                searchBox.Text = string.Empty;
                // Focus using Dispatcher to ensure it happens after layout/visibility logic
                System.Threading.Tasks.Task.Delay(50).ContinueWith(_ =>
                {
                    Dispatcher.UIThread.Post(() => searchBox.Focus(), DispatcherPriority.Input);
                });
            }
        }

        private void RaiseDialog(string effectId)
        {
            var args = new EffectDialogRequestedEventArgs(effectId);
            Dispatcher.UIThread.Post(() => EffectDialogRequested?.Invoke(this, args));
        }

        private static MenuItem CreateFavoriteMenuItem(EffectItem effect)
        {
            var menuItem = new MenuItem
            {
                Header = effect.Name,
                Command = effect.ExecuteCommand
            };

            if (!string.IsNullOrWhiteSpace(effect.Icon))
            {
                var iconBlock = new TextBlock
                {
                    Text = effect.Icon,
                    FontFamily = IconFontFamily,
                    FontSize = 16,
                    FontWeight = FontWeight.Normal
                };

                menuItem.Icon = iconBlock;
            }

            return menuItem;
        }

        private void OnEffectItemPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not EffectItem effect)
            {
                return;
            }

            var point = e.GetCurrentPoint(button);
            if (!point.Properties.IsRightButtonPressed)
            {
                return;
            }

            if (_recentCategory.AllEffects.Contains(effect))
            {
                if (RemoveRecent(effect.EffectId))
                {
                    ApplyCurrentFilter();
                    PersistRecentToOptions();
                }

                e.Handled = true;
                return;
            }

            if (TryToggleFavorite(effect))
            {
                ApplyCurrentFilter();
                PersistFavoritesToOptions();
            }

            e.Handled = true;
        }

        private void BuildEffectLookup()
        {
            _allEffectsById.Clear();

            foreach (var category in Categories)
            {
                if (IsPinnedCategory(category))
                {
                    continue;
                }

                foreach (var effect in category.AllEffects)
                {
                    if (!string.IsNullOrWhiteSpace(effect.EffectId))
                    {
                        effect.ExecuteObserver = RegisterRecentEffect;
                        _allEffectsById[effect.EffectId] = effect;
                    }
                }
            }
        }

        private bool IsPinnedCategory(EffectCategory category)
        {
            return ReferenceEquals(category, _recentCategory) || ReferenceEquals(category, _favoritesCategory);
        }

        private void LoadRecentEffects(IEnumerable<string>? recentEffectIds, bool persistToOptions)
        {
            _recentEffectIds.Clear();

            if (recentEffectIds != null)
            {
                foreach (string recentEffectId in recentEffectIds)
                {
                    if (!TryResolveEffect(recentEffectId, out EffectItem effect))
                    {
                        continue;
                    }

                    if (_recentEffectIds.Any(effectId => string.Equals(effectId, effect.EffectId, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    _recentEffectIds.Add(effect.EffectId);

                    if (_recentEffectIds.Count >= MaxRecentEffects)
                    {
                        break;
                    }
                }
            }

            RebuildRecentCategory();
            ApplyCurrentFilter();

            if (persistToOptions)
            {
                PersistRecentToOptions();
            }
        }

        private void LoadFavoriteEffects(IEnumerable<string>? favoriteEffectIds, bool persistToOptions)
        {
            _favoriteEffectIds.Clear();
            _favoritesCategory.ClearEffects();

            if (favoriteEffectIds != null)
            {
                foreach (string favoriteEffectId in favoriteEffectIds)
                {
                    if (TryResolveEffect(favoriteEffectId, out EffectItem effect))
                    {
                        TryAddFavorite(effect);
                    }
                }
            }

            ApplyCurrentFilter();

            if (persistToOptions)
            {
                PersistFavoritesToOptions();
            }
        }

        private bool TryResolveEffect(string value, out EffectItem effect)
        {
            effect = null!;

            string normalizedId = EffectItem.NormalizeEffectId(value);
            if (string.IsNullOrWhiteSpace(normalizedId))
            {
                return false;
            }

            if (EffectAliases.TryGetValue(normalizedId, out string? alias))
            {
                normalizedId = alias;
            }

            if (_allEffectsById.TryGetValue(normalizedId, out EffectItem? resolvedEffect) && resolvedEffect != null)
            {
                effect = resolvedEffect;
                return true;
            }

            return false;
        }

        private bool TryAddFavorite(EffectItem effect)
        {
            if (!_favoriteEffectIds.Add(effect.EffectId))
            {
                return false;
            }

            _favoritesCategory.AddEffectCopy(effect, keepSorted: false);
            return true;
        }

        private bool TryToggleFavorite(EffectItem effect)
        {
            if (_favoriteEffectIds.Contains(effect.EffectId))
            {
                return RemoveFavorite(effect.EffectId);
            }

            return TryAddFavorite(effect);
        }

        private bool RemoveFavorite(string effectId)
        {
            if (!_favoriteEffectIds.Remove(effectId))
            {
                return false;
            }

            _favoritesCategory.RemoveEffectsById(effectId);
            return true;
        }

        private bool RemoveRecent(string effectId)
        {
            int removedCount = _recentEffectIds.RemoveAll(id => string.Equals(id, effectId, StringComparison.OrdinalIgnoreCase));
            if (removedCount == 0)
            {
                return false;
            }

            RebuildRecentCategory();
            return true;
        }

        private void RegisterRecentEffect(EffectItem effect)
        {
            if (string.IsNullOrWhiteSpace(effect.EffectId))
            {
                return;
            }

            _recentEffectIds.RemoveAll(effectId => string.Equals(effectId, effect.EffectId, StringComparison.OrdinalIgnoreCase));
            _recentEffectIds.Insert(0, effect.EffectId);

            if (_recentEffectIds.Count > MaxRecentEffects)
            {
                _recentEffectIds.RemoveRange(MaxRecentEffects, _recentEffectIds.Count - MaxRecentEffects);
            }

            RebuildRecentCategory();
            ApplyCurrentFilter();
            PersistRecentToOptions();
        }

        private void RebuildRecentCategory()
        {
            _recentCategory.ClearEffects();

            foreach (string recentEffectId in _recentEffectIds)
            {
                if (TryResolveEffect(recentEffectId, out EffectItem effect))
                {
                    _recentCategory.AddEffectCopy(effect, keepSorted: false);
                }
            }
        }

        private void ApplyCurrentFilter()
        {
            var searchBox = this.FindControl<TextBox>("SearchBox");
            string searchText = searchBox?.Text ?? string.Empty;

            foreach (var category in Categories)
            {
                category.Filter(searchText);
            }
        }

        private void UpdateSearchWatermark()
        {
            var searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox == null)
            {
                return;
            }

            int totalEffectCount = Categories
                .Where(category => !IsPinnedCategory(category))
                .Sum(category => category.AllEffects.Count);

            searchBox.PlaceholderText = string.Format(Strings.EffectBrowserPanel_SearchCount, totalEffectCount);
        }

        private void PersistRecentToOptions()
        {
            if (_options == null)
            {
                return;
            }

            _options.RecentEffects = _recentEffectIds.ToList();
        }

        private void PersistFavoritesToOptions()
        {
            if (_options == null)
            {
                return;
            }

            _options.FavoriteEffects = _favoritesCategory.AllEffects
                .Select(effect => effect.EffectId)
                .Where(effectId => !string.IsNullOrWhiteSpace(effectId))
                .ToList();
        }

        private void InitializeEffects()
        {
            Categories.Add(_recentCategory);
            Categories.Add(_favoritesCategory);

            foreach (ImageEffectCategory categoryEnum in Enum.GetValues<ImageEffectCategory>())
            {
                var category = new EffectCategory(EffectBrowserLocalization.GetCategoryName(categoryEnum));
                AddCatalogDrivenEffects(category, categoryEnum);
                AddEditorOperations(category, categoryEnum);
                Categories.Add(category);
            }
        }

        private void AddEditorOperations(EffectCategory category, ImageEffectCategory targetCategory)
        {
            foreach (EditorOperationDefinition operation in EditorOperationCatalog.GetByCategory(targetCategory))
            {
                category.AddEffect(
                    EffectBrowserLocalization.GetEffectBrowserLabel(operation.Id, operation.BrowserLabel),
                    operation.Icon,
                    operation.Description,
                    () => RaiseDialog(operation.Id),
                    operation.Id);
            }
        }

        private void AddCatalogDrivenEffects(EffectCategory category, ImageEffectCategory targetCategory)
        {
            foreach (EffectDefinition definition in ImageEffectCatalog.GetByCategory(targetCategory))
            {
                // Skip effects that are registered as editor operations — they are added separately.
                if (EditorOperationCatalog.TryGetDefinition(definition.Id, out _))
                {
                    continue;
                }

                category.AddEffect(
                    EffectBrowserLocalization.GetEffectBrowserLabel(definition.Id, definition.BrowserLabel),
                    definition.Icon,
                    definition.Description,
                    () => RaiseDialog(definition.Id),
                    definition.Id);
            }
        }
    }
}
