#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Helpers = ShareX.HelpersLib.Helpers;

namespace ShareX;

/// <summary>
/// Small, self-contained controller for the About window's hidden clicker.
/// </summary>
public sealed class ShareXClickerControl : IDisposable
{
    private const int MaxParticles = 200;
    private const double MaxPassiveParticlesPerSecond = 140;
    private const double PassiveAnimationIntervalSeconds = 1;
    private readonly ShareXClickerGame _game;
    private readonly Image _logoImage;
    private readonly Canvas _overlay;
    private readonly Canvas _particleOverlay;
    private readonly Control _normalContent;
    private readonly ContentControl _storeHost;
    private readonly IBrush? _panelBackground;
    private readonly IBrush? _accentBrush;
    private readonly IReadOnlyList<Control> _aboutTextControls;
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new();
    private readonly List<FloatingText> _floatingTexts = [];
    private readonly List<LogoParticle> _particles = [];
    private readonly List<StoreRow> _buildingRows = [];
    private readonly List<StoreRow> _upgradeRows = [];
    private readonly List<Button> _buyModeButtons = [];
    private readonly StackPanel _statsPanel;
    private readonly TextBlock _counter;
    private readonly TextBlock _rateText;
    private readonly TextBlock _goalText;
    private readonly ProgressBar _goalProgress;
    private long _lastTick;
    private double _pressUntil;
    private double _uiRefreshElapsed;
    private double _passiveVisualLogos;
    private double _passiveVisualCooldown;
    private double _passiveParticleCarry;
    private bool _disposed;
    private bool _activated;
    private bool _storeVisible;
    private bool _empireCelebrated;
    private BuyMode _buyMode = BuyMode.One;
    private string _displayedBuildingIds = "";
    private string _displayedUpgradeIds = "";

    public ShareXClickerControl(ShareXClickerState state, Image logoImage, Canvas overlay, Canvas particleOverlay, Control normalContent,
        ContentControl storeHost, IBrush? panelBackground, IReadOnlyList<Control> aboutTextControls)
    {
        _game = new ShareXClickerGame(state);
        _logoImage = logoImage ?? throw new ArgumentNullException(nameof(logoImage));
        _overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
        _particleOverlay = particleOverlay ?? throw new ArgumentNullException(nameof(particleOverlay));
        _normalContent = normalContent ?? throw new ArgumentNullException(nameof(normalContent));
        _storeHost = storeHost ?? throw new ArgumentNullException(nameof(storeHost));
        _panelBackground = panelBackground;
        _accentBrush = Application.Current?.Resources["ShareX.Brush.Accent"] as IBrush;
        _aboutTextControls = aboutTextControls ?? throw new ArgumentNullException(nameof(aboutTextControls));

        _counter = new TextBlock
        {
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = _accentBrush,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        _rateText = new TextBlock
        {
            FontSize = 12,
            Opacity = 0.75,
            TextAlignment = TextAlignment.Center
        };
        _goalText = new TextBlock
        {
            FontSize = 11,
            Opacity = 0.75,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        _goalProgress = new ProgressBar
        {
            Height = 4,
            Minimum = 0,
            Maximum = 1
        };
        _statsPanel = new StackPanel
        {
            Width = 230,
            Spacing = 4,
            IsVisible = false,
            Children =
            {
                _counter,
                _rateText,
                _goalText,
                _goalProgress
            }
        };
        Canvas.SetLeft(_statsPanel, 5);
        Canvas.SetTop(_statsPanel, 225);
        _overlay.Children.Add(_statsPanel);

        _logoImage.Cursor = new Cursor(StandardCursorType.Hand);
        _logoImage.Focusable = true;
        _logoImage.RenderTransformOrigin = RelativePoint.Center;
        _logoImage.PointerPressed += OnLogoPointerPressed;
        _logoImage.KeyDown += OnLogoKeyDown;

        RefreshUi();
        _lastTick = Stopwatch.GetTimestamp();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnTimerTick;
    }

    private void OnLogoPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_disposed || !e.GetCurrentPoint(_logoImage).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point position = e.GetPosition(_overlay);
        Point particleOrigin = e.GetPosition(_particleOverlay);
        ClickLogo(position, particleOrigin);
        _logoImage.Focus();
        e.Handled = true;
    }

    private void OnLogoKeyDown(object? sender, KeyEventArgs e)
    {
        if (_disposed || (e.Key != Key.Enter && e.Key != Key.Space))
        {
            return;
        }

        Point logoCenter = new(_logoImage.Bounds.Width / 2, _logoImage.Bounds.Height / 2);
        Point overlayPosition = _logoImage.TranslatePoint(logoCenter, _overlay) ?? new Point(120, 130);
        Point particleOrigin = _logoImage.TranslatePoint(logoCenter, _particleOverlay) ?? new Point(120, 130);
        ClickLogo(overlayPosition, particleOrigin);
        e.Handled = true;
    }

    private void ClickLogo(Point position, Point particleOrigin)
    {
        double clickGain = _game.Click();
        Helpers.PlaySound(Resources.ActionCompletedSound);
        if (!_activated)
        {
            _activated = true;
            _passiveVisualCooldown = PassiveAnimationIntervalSeconds;
            _lastTick = Stopwatch.GetTimestamp();
            _timer.Start();
        }
        HideAboutText();
        _statsPanel.IsVisible = true;
        _pressUntil = ElapsedSeconds() + 0.12;
        AddFloatingText(position, clickGain);
        AddParticles((int)Math.Ceiling(clickGain), particleOrigin);

        if (_game.State.StoreDiscovered && !_storeVisible)
        {
            ShowStore();
        }

        RefreshUi();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_disposed)
        {
            return;
        }

        long now = Stopwatch.GetTimestamp();
        double elapsedSeconds = (now - _lastTick) / (double)Stopwatch.Frequency;
        _lastTick = now;
        if (elapsedSeconds <= 0 || elapsedSeconds > 2)
        {
            elapsedSeconds = 0;
        }

        double passiveEarned = _game.Tick(TimeSpan.FromSeconds(elapsedSeconds));
        double nowSeconds = ElapsedSeconds();
        _logoImage.RenderTransform = nowSeconds < _pressUntil ? new ScaleTransform(0.93, 0.93) : null;
        UpdateEffects(elapsedSeconds);
        AddPassiveAnimations(passiveEarned, elapsedSeconds);

        _uiRefreshElapsed += elapsedSeconds;
        if (_uiRefreshElapsed >= 0.2)
        {
            _uiRefreshElapsed = 0;
            RefreshUi();
        }

    }

    private void ShowStore()
    {
        _storeVisible = true;
        HideAboutText();
        _normalContent.IsVisible = false;
        _storeHost.IsVisible = true;
        RebuildStoreRows();
    }

    private void RebuildStoreRows()
    {
        _buildingRows.Clear();
        _upgradeRows.Clear();
        _buyModeButtons.Clear();

        StackPanel panel = new() { Spacing = 10, Margin = new Thickness(4, 0, 4, 0) };
        StackPanel buyModeSelector = CreateBuyModeSelector();
        Grid.SetColumn(buyModeSelector, 1);
        panel.Children.Add(new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children =
            {
                new StackPanel
                {
                    Spacing = 2,
                    Children =
                    {
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 7,
                            Children = { Icon(LucideIcons.landmark, 18), new TextBlock { Text = "ShareX Clicker", FontSize = 18, Foreground = _accentBrush } }
                        },
                        new TextBlock { Text = "Build a tiny screenshot empire.", FontSize = 12, Opacity = 0.7 }
                    }
                },
                buyModeSelector
            }
        });

        panel.Children.Add(CreateSectionHeading("Upgrades", LucideIcons.sparkles));

        foreach (ShareXClickerUpgrade upgrade in UpgradesToDisplay())
        {
            StoreRow row = CreateUpgradeRow(upgrade);
            _upgradeRows.Add(row);
            panel.Children.Add(row.Container);
        }

        panel.Children.Add(CreateSectionHeading("Generators", LucideIcons.factory));
        foreach (ShareXClickerBuilding building in BuildingsToDisplay())
        {
            StoreRow row = CreateBuildingRow(building);
            _buildingRows.Add(row);
            panel.Children.Add(row.Container);
        }

        _storeHost.Content = new Border
        {
            Padding = new Thickness(18, 12, 8, 12),
            Child = new ScrollViewer
            {
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = panel
            }
        };
        _displayedBuildingIds = string.Join(",", _buildingRows.Select(x => x.Id));
        _displayedUpgradeIds = string.Join(",", _upgradeRows.Select(x => x.Id));
        UpdateBuyModeButtons();
    }

    private IEnumerable<ShareXClickerBuilding> BuildingsToDisplay()
    {
        return ShareXClickerGame.Buildings.Where(_game.IsUnlocked)
            .Concat(ShareXClickerGame.Buildings.Where(x => !_game.IsUnlocked(x)).Take(2));
    }

    private IEnumerable<ShareXClickerUpgrade> UpgradesToDisplay()
    {
        return ShareXClickerGame.Upgrades.Where(_game.IsUnlocked)
            .Concat(ShareXClickerGame.Upgrades.Where(x => !_game.IsUnlocked(x)).Take(1));
    }

    private StackPanel CreateBuyModeSelector()
    {
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        foreach ((BuyMode mode, string label) in new[] { (BuyMode.One, "x1"), (BuyMode.Ten, "x10"), (BuyMode.Max, "Max") })
        {
            Button button = new()
            {
                Content = label,
                Width = mode == BuyMode.Max ? 48 : 40,
                Height = 30,
                Padding = new Thickness(4),
                Tag = mode
            };
            ToolTip.SetTip(button, $"Buy {label}");
            button.Click += (_, _) =>
            {
                _buyMode = mode;
                UpdateBuyModeButtons();
                RefreshUi();
            };
            _buyModeButtons.Add(button);
            buttons.Children.Add(button);
        }

        return new StackPanel
        {
            Spacing = 3,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children =
            {
                new TextBlock { Text = "Generator amount", FontSize = 11, Opacity = 0.7, HorizontalAlignment = HorizontalAlignment.Right },
                buttons
            }
        };
    }

    private StackPanel CreateSectionHeading(string text, string glyph)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 7,
            Margin = new Thickness(0, 6, 0, 0),
            Children =
            {
                Icon(glyph, 16),
                new TextBlock { Text = text, FontSize = 16 }
            }
        };
    }

    private void UpdateBuyModeButtons()
    {
        foreach (Button button in _buyModeButtons)
        {
            button.Classes.Set("active", button.Tag is BuyMode mode && mode == _buyMode);
        }
    }

    private StoreRow CreateBuildingRow(ShareXClickerBuilding building)
    {
        TextBlock title = new() { FontSize = 15, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
        TextBlock details = new() { FontSize = 13, Opacity = 0.75, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Right, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        Button buyButton = new() { MinWidth = 94, VerticalAlignment = VerticalAlignment.Center };
        buyButton.Click += (_, _) =>
        {
            int quantity = GetBuyQuantity(building);
            int previousOwned = _game.GetOwned(building);
            if (_game.BuyBuilding(building, quantity))
            {
                int currentOwned = _game.GetOwned(building);
                int? reachedMilestone = ShareXClickerGame.BuildingMilestones.LastOrDefault(x => previousOwned < x && currentOwned >= x);
                string message = reachedMilestone > 0
                    ? $"{building.Name} {_game.GetBuildingMilestoneMultiplier(building)}x boost!"
                    : $"Bought {quantity} {building.Name}";
                CelebratePurchase(buyButton, message, reachedMilestone > 0 ? 24 : 8);
                RefreshUi();
            }
        };

        TextBlock rowIcon = Icon(building.Icon, 18);
        StackPanel priceContent = CreatePriceContent(out TextBlock priceText);
        buyButton.Content = priceContent;
        return new StoreRow(building.Id, building, null, CreateRow(rowIcon, title, details, buyButton, _panelBackground), rowIcon, title, details, buyButton, priceContent, priceText);
    }

    private StoreRow CreateUpgradeRow(ShareXClickerUpgrade upgrade)
    {
        TextBlock title = new() { FontSize = 15, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
        TextBlock details = new() { FontSize = 13, Opacity = 0.75, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Right, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        Button buyButton = new() { MinWidth = 94, VerticalAlignment = VerticalAlignment.Center };
        buyButton.Click += (_, _) =>
        {
            if (_game.BuyUpgrade(upgrade))
            {
                CelebratePurchase(buyButton, upgrade.Name + " unlocked!", 20);
                RefreshUi();
            }
        };

        TextBlock rowIcon = Icon(upgrade.Icon, 18);
        StackPanel priceContent = CreatePriceContent(out TextBlock priceText);
        buyButton.Content = priceContent;
        return new StoreRow(upgrade.Id, null, upgrade, CreateRow(rowIcon, title, details, buyButton, _panelBackground), rowIcon, title, details, buyButton, priceContent, priceText);
    }

    private StackPanel CreatePriceContent(out TextBlock priceText)
    {
        priceText = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
        Image logo = new() { Source = _logoImage.Source, Width = 12, Height = 12, Stretch = Stretch.Uniform, VerticalAlignment = VerticalAlignment.Center };
        RenderOptions.SetBitmapInterpolationMode(logo, BitmapInterpolationMode.HighQuality);
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children =
            {
                logo,
                priceText
            }
        };
    }

    private static Border CreateRow(TextBlock icon, TextBlock title, TextBlock details, Button buyButton, IBrush? background)
    {
        Border buttonHost = new() { Padding = new Thickness(8, 0, 0, 0), Child = buyButton };
        Grid.SetColumn(title, 1);
        Grid.SetColumn(details, 2);
        Grid.SetColumn(buttonHost, 3);

        return new Border
        {
            Padding = new Thickness(12, 7, 8, 7),
            CornerRadius = new CornerRadius(6),
            Background = background,
            Child = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("26,1.4*,1.2*,Auto"),
                Children =
                {
                    icon,
                    title,
                    details,
                    buttonHost
                }
            }
        };
    }

    private void RefreshUi()
    {
        _counter.Text = $"{ShareXClickerNumberFormatter.Format(_game.State.Logos)} logos";
        _rateText.Text = $"+{ShareXClickerNumberFormatter.Format(_game.LogosPerSecond)}/sec  •  {ShareXClickerNumberFormatter.Format(_game.ClickValue)} per click";
        ToolTip.SetTip(_statsPanel, $"{ShareXClickerNumberFormatter.Format(_game.State.LifetimeLogos)} lifetime logos • {_game.State.ManualClicks:N0} clicks • {_game.TotalBuildings:N0} generators");
        UpdateGoal();
        if (!_storeVisible)
        {
            return;
        }

        string buildingIds = string.Join(",", BuildingsToDisplay().Select(x => x.Id));
        string upgradeIds = string.Join(",", UpgradesToDisplay().Select(x => x.Id));
        if (buildingIds != _displayedBuildingIds || upgradeIds != _displayedUpgradeIds)
        {
            RebuildStoreRows();
        }

        foreach (StoreRow row in _upgradeRows)
        {
            ShareXClickerUpgrade upgrade = row.Upgrade!;
            bool unlocked = _game.IsUnlocked(upgrade);
            bool purchased = _game.IsPurchased(upgrade);
            row.Icon.Text = unlocked ? upgrade.Icon : LucideIcons.@lock;
            row.Title.Text = upgrade.Name;
            row.Details.Text = unlocked ? upgrade.Effect : $"Unlock at {ShareXClickerNumberFormatter.Format(upgrade.UnlockAt)} logos";
            row.Price.Text = ShareXClickerNumberFormatter.Format(upgrade.Cost);
            row.Button.Content = purchased ? "Owned" : unlocked ? row.PriceContent : "Locked";
            row.Button.IsEnabled = _game.CanBuy(upgrade);
            row.Container.Opacity = unlocked ? 1 : 0.55;
        }

        foreach (StoreRow row in _buildingRows)
        {
            ShareXClickerBuilding building = row.Building!;
            bool unlocked = _game.IsUnlocked(building);
            row.Icon.Text = unlocked ? building.Icon : LucideIcons.@lock;
            row.Title.Text = $"{building.Name} ({_game.GetOwned(building)})";
            row.Details.Text = GetBuildingDetails(building, unlocked);
            int quantity = GetBuyQuantity(building);
            int displayedQuantity = Math.Max(1, quantity);
            double cost = _game.GetBuildingCost(building, displayedQuantity);
            row.Price.Text = displayedQuantity > 1
                ? $"x{displayedQuantity}  {ShareXClickerNumberFormatter.Format(cost)}"
                : ShareXClickerNumberFormatter.Format(cost);
            row.Button.Content = unlocked ? row.PriceContent : "Locked";
            row.Button.IsEnabled = quantity > 0 && _game.CanBuy(building, quantity);
            row.Container.Opacity = unlocked ? 1 : 0.55;
        }
    }

    private int GetBuyQuantity(ShareXClickerBuilding building) => _buyMode switch
    {
        BuyMode.Ten => 10,
        BuyMode.Max => _game.GetMaxAffordableQuantity(building),
        _ => 1
    };

    private string GetBuildingDetails(ShareXClickerBuilding building, bool unlocked)
    {
        if (!unlocked)
        {
            return $"Unlock at {ShareXClickerNumberFormatter.Format(building.UnlockAt)} logos";
        }

        double each = _game.GetBuildingProductionPerSecond(building);
        int? nextMilestone = _game.GetNextBuildingMilestone(building);
        return nextMilestone.HasValue
            ? $"+{ShareXClickerNumberFormatter.Format(each)}/sec each • 2x at {nextMilestone}"
            : $"+{ShareXClickerNumberFormatter.Format(each)}/sec each • max boost";
    }

    private void UpdateGoal()
    {
        double lifetime = _game.State.LifetimeLogos;
        if (lifetime < ShareXClickerGame.StoreUnlockAt)
        {
            UpdateGoalProgress("Store", ShareXClickerGame.StoreUnlockAt, 0);
            return;
        }

        (string name, double target)? nextGoal = ShareXClickerGame.Buildings
            .Where(x => !_game.IsUnlocked(x))
            .Select(x => (x.Name, x.UnlockAt))
            .Concat(ShareXClickerGame.Upgrades.Where(x => !_game.IsUnlocked(x)).Select(x => (x.Name, x.UnlockAt)))
            .OrderBy(x => x.UnlockAt)
            .FirstOrDefault();

        if (nextGoal.HasValue && nextGoal.Value.target > 0)
        {
            double previousTarget = ShareXClickerGame.Buildings.Where(_game.IsUnlocked).Select(x => x.UnlockAt)
                .Concat(ShareXClickerGame.Upgrades.Where(_game.IsUnlocked).Select(x => x.UnlockAt))
                .DefaultIfEmpty(ShareXClickerGame.StoreUnlockAt)
                .Max();
            UpdateGoalProgress(nextGoal.Value.name, nextGoal.Value.target, previousTarget);
            return;
        }

        if (lifetime < ShareXClickerGame.EmpireGoalAt)
        {
            UpdateGoalProgress("Complete the empire", ShareXClickerGame.EmpireGoalAt, 1_000_000);
            return;
        }

        _goalText.Text = "Screenshot empire complete!";
        _goalProgress.Value = 1;
        if (_activated && !_empireCelebrated)
        {
            _empireCelebrated = true;
            Point center = new(_particleOverlay.Bounds.Width / 2, _particleOverlay.Bounds.Height / 2);
            AddParticles(80, center);
            AddFloatingText(_particleOverlay, center, "Empire complete!", 24, 1.5);
        }
    }

    private void UpdateGoalProgress(string name, double target, double previousTarget)
    {
        double remaining = Math.Max(0, target - _game.State.LifetimeLogos);
        _goalText.Text = $"Next: {name} • {ShareXClickerNumberFormatter.Format(remaining)} to go";
        _goalProgress.Value = Math.Clamp((_game.State.LifetimeLogos - previousTarget) / Math.Max(1, target - previousTarget), 0, 1);
    }

    private void AddFloatingText(Point position, double amount)
    {
        AddFloatingText(_overlay, position, "+" + ShareXClickerNumberFormatter.Format(amount), 24);
    }

    private void AddFloatingText(Canvas canvas, Point position, string content, double fontSize, double lifetime = 0.65)
    {
        TextBlock text = new()
        {
            Text = content,
            FontSize = fontSize,
            Foreground = _accentBrush,
            IsHitTestVisible = false,
            TextAlignment = TextAlignment.Center
        };
        double estimatedWidth = Math.Max(40, content.Length * fontSize * 0.55);
        double left = Math.Clamp(position.X - estimatedWidth / 2 + _random.Next(-8, 9), 0, Math.Max(0, canvas.Bounds.Width - estimatedWidth));
        Canvas.SetLeft(text, left);
        Canvas.SetTop(text, Math.Clamp(position.Y - 16, 8, Math.Max(8, canvas.Bounds.Height - 40)));
        canvas.Children.Add(text);
        _floatingTexts.Add(new FloatingText(text, canvas, Canvas.GetTop(text), lifetime));
    }

    private void CelebratePurchase(Control control, string message, int particleCount)
    {
        Point localCenter = new(control.Bounds.Width / 2, control.Bounds.Height / 2);
        Point origin = control.TranslatePoint(localCenter, _particleOverlay) ?? new Point(_particleOverlay.Bounds.Width / 2, 80);
        AddParticles(particleCount, origin);
        AddFloatingText(_particleOverlay, origin, message, 17, 1.1);
        Helpers.PlaySound(Resources.ActionCompletedSound);
    }

    private void AddParticles(int count, Point? origin = null)
    {
        for (int i = 0; i < Math.Min(count, MaxParticles); i++)
        {
            if (_particles.Count >= MaxParticles)
            {
                RemoveParticle(_particles[0]);
            }

            Image image = new()
            {
                Source = _logoImage.Source,
                Width = 15,
                Height = 15,
                Stretch = Stretch.Uniform,
                IsHitTestVisible = false,
                RenderTransformOrigin = RelativePoint.Center
            };
            Point start = origin ?? RandomLogoPoint();
            double lifetime = 2.6 + _random.NextDouble() * 0.6;
            LogoParticle particle = new(image, start.X, start.Y, _random.NextDouble() * 140 - 70, -40 - _random.NextDouble() * 75,
                _random.NextDouble() * 200 - 100, lifetime);
            _particles.Add(particle);
            Canvas.SetLeft(image, particle.X);
            Canvas.SetTop(image, particle.Y);
            _particleOverlay.Children.Add(image);
        }
    }

    private Point RandomLogoPoint()
    {
        Point logoOrigin = _logoImage.TranslatePoint(default, _particleOverlay) ?? new Point(40, 50);
        double angle = _random.NextDouble() * Math.PI * 2;
        double distance = Math.Sqrt(_random.NextDouble());
        return new Point(logoOrigin.X + 80 + Math.Cos(angle) * distance * 65, logoOrigin.Y + 80 + Math.Sin(angle) * distance * 65);
    }

    private void AddPassiveAnimations(double earned, double elapsedSeconds)
    {
        if (!_activated || earned <= 0)
        {
            return;
        }

        AddPassiveParticles(earned, elapsedSeconds);
        _passiveVisualLogos += earned;
        _passiveVisualCooldown = Math.Max(0, _passiveVisualCooldown - elapsedSeconds);
        if (_passiveVisualLogos < 1 || _passiveVisualCooldown > 0)
        {
            return;
        }

        double displayedAmount = Math.Floor(_passiveVisualLogos);
        _passiveVisualLogos -= displayedAmount;
        _passiveVisualCooldown = PassiveAnimationIntervalSeconds;
        AddFloatingText(new Point(120 + _random.Next(-35, 36), 135 + _random.Next(-20, 21)), displayedAmount);
    }

    private void AddPassiveParticles(double earned, double elapsedSeconds)
    {
        _passiveParticleCarry += Math.Min(earned, MaxPassiveParticlesPerSecond * elapsedSeconds);
        int particleCount = (int)Math.Floor(_passiveParticleCarry);
        if (particleCount <= 0)
        {
            return;
        }

        _passiveParticleCarry -= particleCount;
        AddParticles(particleCount);
    }

    private void HideAboutText()
    {
        foreach (Control control in _aboutTextControls)
        {
            control.IsVisible = false;
        }
    }

    private void UpdateEffects(double elapsedSeconds)
    {
        for (int i = _floatingTexts.Count - 1; i >= 0; i--)
        {
            FloatingText floating = _floatingTexts[i];
            floating.Age += elapsedSeconds;
            floating.Control.Opacity = Math.Max(0, 1 - floating.Age / floating.Lifetime);
            Canvas.SetTop(floating.Control, floating.StartTop - floating.Age * 34);
            if (floating.Age >= floating.Lifetime)
            {
                floating.Canvas.Children.Remove(floating.Control);
                _floatingTexts.RemoveAt(i);
            }
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            LogoParticle particle = _particles[i];
            particle.Age += elapsedSeconds;
            particle.X += particle.VelocityX * elapsedSeconds;
            particle.Y += particle.VelocityY * elapsedSeconds;
            particle.VelocityY += 500 * elapsedSeconds;
            particle.Control.Opacity = particle.Age < particle.Lifetime - 0.35
                ? 1
                : Math.Max(0, (particle.Lifetime - particle.Age) / 0.35);
            particle.Control.RenderTransform = new RotateTransform(particle.Rotation * particle.Age);
            Canvas.SetLeft(particle.Control, particle.X);
            Canvas.SetTop(particle.Control, particle.Y);
            if (particle.Age >= particle.Lifetime || particle.Y >= _particleOverlay.Bounds.Height - particle.Control.Height || particle.X < -20 || particle.X > _particleOverlay.Bounds.Width + 20)
            {
                RemoveParticle(particle);
            }
        }
    }

    private TextBlock Icon(string glyph, double size)
    {
        TextBlock icon = new() { Text = glyph, FontSize = size, VerticalAlignment = VerticalAlignment.Center, Foreground = _accentBrush };
        icon.Classes.Add("icon");
        return icon;
    }

    private static double ElapsedSeconds() => Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

    private void RemoveParticle(LogoParticle particle)
    {
        _particleOverlay.Children.Remove(particle.Control);
        _particles.Remove(particle);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _logoImage.PointerPressed -= OnLogoPointerPressed;
        _logoImage.KeyDown -= OnLogoKeyDown;
        _logoImage.RenderTransform = null;
        foreach (FloatingText text in _floatingTexts) text.Canvas.Children.Remove(text.Control);
        foreach (LogoParticle particle in _particles) _particleOverlay.Children.Remove(particle.Control);
        _floatingTexts.Clear();
        _particles.Clear();
        _overlay.Children.Remove(_statsPanel);
    }

    private sealed class FloatingText(TextBlock control, Canvas canvas, double startTop, double lifetime)
    {
        public TextBlock Control { get; } = control;
        public Canvas Canvas { get; } = canvas;
        public double StartTop { get; } = startTop;
        public double Lifetime { get; } = lifetime;
        public double Age { get; set; }
    }

    private sealed class LogoParticle(Image control, double x, double y, double velocityX, double velocityY, double rotation, double lifetime)
    {
        public Image Control { get; } = control;
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
        public double VelocityX { get; } = velocityX;
        public double VelocityY { get; set; } = velocityY;
        public double Rotation { get; } = rotation;
        public double Lifetime { get; } = lifetime;
        public double Age { get; set; }
    }

    private sealed class StoreRow(string id, ShareXClickerBuilding? building, ShareXClickerUpgrade? upgrade, Border container, TextBlock icon, TextBlock title, TextBlock details, Button button, StackPanel priceContent, TextBlock price)
    {
        public string Id { get; } = id;
        public ShareXClickerBuilding? Building { get; } = building;
        public ShareXClickerUpgrade? Upgrade { get; } = upgrade;
        public Border Container { get; } = container;
        public TextBlock Icon { get; } = icon;
        public TextBlock Title { get; } = title;
        public TextBlock Details { get; } = details;
        public Button Button { get; } = button;
        public StackPanel PriceContent { get; } = priceContent;
        public TextBlock Price { get; } = price;
    }

    private enum BuyMode
    {
        One,
        Ten,
        Max
    }
}
