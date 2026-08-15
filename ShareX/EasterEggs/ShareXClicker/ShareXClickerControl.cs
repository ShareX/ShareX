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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    private readonly TextBlock _counter;
    private long _lastTick;
    private double _pressUntil;
    private double _uiRefreshElapsed;
    private double _passiveVisualLogos;
    private double _passiveVisualCooldown;
    private double _passiveParticleCarry;
    private bool _disposed;
    private bool _activated;
    private bool _storeVisible;
    private string _displayedBuildingIds = "";

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
            Width = 230,
            FontSize = 22,
            LineHeight = 34,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
        };
        Canvas.SetLeft(_counter, 5);
        Canvas.SetTop(_counter, 278);
        _overlay.Children.Add(_counter);

        _logoImage.Cursor = new Cursor(StandardCursorType.Hand);
        _logoImage.RenderTransformOrigin = RelativePoint.Center;
        _logoImage.PointerPressed += OnLogoPointerPressed;

        RefreshUi();
        _lastTick = Stopwatch.GetTimestamp();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnLogoPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_disposed || !e.GetCurrentPoint(_logoImage).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point position = e.GetPosition(_overlay);
        double clickGain = _game.Click();
        if (!_activated)
        {
            _activated = true;
            _passiveVisualCooldown = PassiveAnimationIntervalSeconds;
        }
        HideAboutText();
        _counter.IsVisible = true;
        _pressUntil = ElapsedSeconds() + 0.12;
        AddFloatingText(position, clickGain);
        AddParticles((int)Math.Ceiling(clickGain));

        if (_game.State.StoreDiscovered && !_storeVisible)
        {
            ShowStore();
        }

        RefreshUi();
        e.Handled = true;
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

        StackPanel panel = new() { Spacing = 8, Margin = new Thickness(4, 0, 4, 0) };
        panel.Children.Add(new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 2,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 7,
                    Children = { Icon(LucideIcons.landmark, 18), new TextBlock { Text = "ShareX Clicker", FontSize = 18, Foreground = _accentBrush } }
                },
                new TextBlock { Text = "Build a tiny screenshot empire.", FontSize = 12, Opacity = 0.7, TextAlignment = TextAlignment.Center }
            }
        });
        panel.Children.Add(new TextBlock { Text = "Upgrades", FontSize = 17, Margin = new Thickness(0, 5, 0, 0) });

        foreach (ShareXClickerUpgrade upgrade in ShareXClickerGame.Upgrades)
        {
            StoreRow row = CreateUpgradeRow(upgrade);
            _upgradeRows.Add(row);
            panel.Children.Add(row.Container);
        }

        panel.Children.Add(new TextBlock { Text = "Generators", FontSize = 17, Margin = new Thickness(0, 5, 0, 0) });
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
    }

    private IEnumerable<ShareXClickerBuilding> BuildingsToDisplay()
    {
        return ShareXClickerGame.Buildings.Where(_game.IsUnlocked)
            .Concat(ShareXClickerGame.Buildings.Where(x => !_game.IsUnlocked(x)).Take(2));
    }

    private StoreRow CreateBuildingRow(ShareXClickerBuilding building)
    {
        TextBlock title = new() { FontSize = 16, TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center };
        TextBlock details = new() { FontSize = 13, Opacity = 0.75, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Right, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        Button buyButton = new() { Width = 88, VerticalAlignment = VerticalAlignment.Center };
        buyButton.Click += (_, _) =>
        {
            if (_game.BuyBuilding(building))
            {
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
        TextBlock title = new() { FontSize = 16, TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center };
        TextBlock details = new() { FontSize = 13, Opacity = 0.75, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Right, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        Button buyButton = new() { Width = 88, VerticalAlignment = VerticalAlignment.Center };
        buyButton.Click += (_, _) =>
        {
            if (_game.BuyUpgrade(upgrade))
            {
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
            Padding = new Thickness(14, 6, 8, 6),
            CornerRadius = new CornerRadius(4),
            Background = background,
            Child = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("26,210,*,Auto"),
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
        _counter.Text = $"{ShareXClickerNumberFormatter.Format(_game.State.Logos)} logos\n+{ShareXClickerNumberFormatter.Format(_game.LogosPerSecond)} / sec";
        if (!_storeVisible)
        {
            return;
        }

        string buildingIds = string.Join(",", BuildingsToDisplay().Select(x => x.Id));
        if (buildingIds != _displayedBuildingIds)
        {
            RebuildStoreRows();
        }

        foreach (StoreRow row in _upgradeRows)
        {
            ShareXClickerUpgrade upgrade = row.Upgrade!;
            bool purchased = _game.IsPurchased(upgrade);
            row.Title.Text = upgrade.Name;
            row.Details.Text = upgrade.Effect;
            row.Price.Text = ShareXClickerNumberFormatter.Format(upgrade.Cost);
            row.Button.Content = purchased ? "Owned" : row.PriceContent;
            row.Button.IsEnabled = _game.CanBuy(upgrade);
        }

        foreach (StoreRow row in _buildingRows)
        {
            ShareXClickerBuilding building = row.Building!;
            bool unlocked = _game.IsUnlocked(building);
            row.Icon.Text = unlocked ? building.Icon : LucideIcons.@lock;
            row.Title.Text = $"{building.Name} ({_game.GetOwned(building)})";
            row.Details.Text = unlocked
                ? $"+{ShareXClickerNumberFormatter.Format(building.ProductionPerSecond)}/sec each"
                : $"Unlock at {ShareXClickerNumberFormatter.Format(building.UnlockAt)} logos";
            row.Price.Text = ShareXClickerNumberFormatter.Format(_game.GetBuildingCost(building));
            row.Button.Content = unlocked ? row.PriceContent : "Locked";
            row.Button.IsEnabled = _game.CanBuy(building);
            row.Container.Opacity = unlocked ? 1 : 0.55;
        }
    }

    private void AddFloatingText(Point position, double amount)
    {
        TextBlock text = new()
        {
            Text = "+" + ShareXClickerNumberFormatter.Format(amount),
            FontSize = 24,
            IsHitTestVisible = false
        };
        double left = Math.Clamp(position.X - 12 + _random.Next(-10, 11), 0, 205);
        Canvas.SetLeft(text, left);
        Canvas.SetTop(text, Math.Clamp(position.Y - 16, 50, 260));
        _overlay.Children.Add(text);
        _floatingTexts.Add(new FloatingText(text, Canvas.GetTop(text)));
    }

    private void AddParticles(int count)
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
            Point logoOrigin = _logoImage.TranslatePoint(default, _particleOverlay) ?? new Point(40, 50);
            double angle = _random.NextDouble() * Math.PI * 2;
            double distance = Math.Sqrt(_random.NextDouble());
            double startX = logoOrigin.X + 80 + Math.Cos(angle) * distance * 65;
            double startY = logoOrigin.Y + 80 + Math.Sin(angle) * distance * 65;
            double lifetime = 2.6 + _random.NextDouble() * 0.6;
            LogoParticle particle = new(image, startX, startY, _random.NextDouble() * 140 - 70, -40 - _random.NextDouble() * 75,
                _random.NextDouble() * 200 - 100, lifetime);
            _particles.Add(particle);
            Canvas.SetLeft(image, particle.X);
            Canvas.SetTop(image, particle.Y);
            _particleOverlay.Children.Add(image);
        }
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
            floating.Control.Opacity = Math.Max(0, 1 - floating.Age / 0.65);
            Canvas.SetTop(floating.Control, floating.StartTop - floating.Age * 34);
            if (floating.Age >= 0.65)
            {
                _overlay.Children.Remove(floating.Control);
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
            if (particle.Age >= particle.Lifetime || particle.Y >= _particleOverlay.Bounds.Height - particle.Control.Height || particle.X < -20 || particle.X > 320)
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
        _logoImage.RenderTransform = null;
        foreach (FloatingText text in _floatingTexts) _overlay.Children.Remove(text.Control);
        foreach (LogoParticle particle in _particles) _particleOverlay.Children.Remove(particle.Control);
        _floatingTexts.Clear();
        _particles.Clear();
        _overlay.Children.Remove(_counter);
    }

    private sealed class FloatingText(TextBlock control, double startTop)
    {
        public TextBlock Control { get; } = control;
        public double StartTop { get; } = startTop;
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
}
