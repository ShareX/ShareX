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

using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;

namespace ShareX;

public sealed class ShareXClickerGame
{
    public const double StoreUnlockAt = 10;
    public static readonly IReadOnlyList<ShareXClickerBuilding> Buildings =
    [
        new("auto-clicker", "Auto Clicker", LucideIcons.mouse_pointer_click, 15, 1, 10),
        new("sharex-bot", "ShareX Bot", LucideIcons.bot, 100, 1, 100),
        new("screenshot-station", "Screenshot Station", LucideIcons.monitor, 550, 5, 500),
        new("upload-server", "Upload Server", LucideIcons.server, 3000, 25, 2500),
        new("screenshot-factory", "Screenshot Factory", LucideIcons.factory, 12000, 100, 10000),
        new("sharex-datacenter", "ShareX Datacenter", LucideIcons.database, 60000, 450, 50000),
        new("capture-cloud", "Capture Cloud", LucideIcons.cloud, 300000, 2000, 250000),
        new("sharex-satellite", "ShareX Satellite", LucideIcons.satellite, 1250000, 10000, 1000000)
    ];

    public static readonly IReadOnlyList<ShareXClickerUpgrade> Upgrades =
    [
        new("better-click", "Better Click", LucideIcons.mouse_pointer_click, 50, "+1 logo per click"),
        new("double-click", "Double Click", LucideIcons.mouse, 500, "2x manual click value")
    ];

    private readonly ShareXClickerState _state;
    public ShareXClickerGame(ShareXClickerState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        NormalizeState();
    }

    public ShareXClickerState State => _state;
    public double LogosPerSecond => CalculateLogosPerSecond();
    public double ClickValue => (_state.BetterClickPurchased ? 2 : 1) * (_state.DoubleClickPurchased ? 2 : 1);
    public double Click()
    {
        double clickGain = ClickValue;
        AddLogos(clickGain);
        _state.StoreDiscovered |= _state.LifetimeLogos >= StoreUnlockAt;
        return clickGain;
    }

    public double Tick(TimeSpan elapsed)
    {
        if (elapsed <= TimeSpan.Zero || LogosPerSecond <= 0)
        {
            return 0;
        }

        double earned = LogosPerSecond * elapsed.TotalSeconds;
        AddLogos(earned);
        _state.StoreDiscovered |= _state.LifetimeLogos >= StoreUnlockAt;
        return earned;
    }

    public bool CanBuy(ShareXClickerBuilding building) => IsUnlocked(building) && _state.Logos >= GetBuildingCost(building);

    public bool BuyBuilding(ShareXClickerBuilding building)
    {
        double cost = GetBuildingCost(building);
        if (!IsUnlocked(building) || _state.Logos < cost)
        {
            return false;
        }

        _state.Logos -= cost;
        _state.Buildings[building.Id] = GetOwned(building) + 1;
        return true;
    }

    public bool IsUnlocked(ShareXClickerBuilding building) => _state.LifetimeLogos >= building.UnlockAt;

    public int GetOwned(ShareXClickerBuilding building) => _state.Buildings.TryGetValue(building.Id, out int owned) ? Math.Max(owned, 0) : 0;

    public double GetBuildingCost(ShareXClickerBuilding building) => Math.Ceiling(building.BaseCost * Math.Pow(1.15, GetOwned(building)));

    public bool IsPurchased(ShareXClickerUpgrade upgrade) => upgrade.Id switch
    {
        "better-click" => _state.BetterClickPurchased,
        "double-click" => _state.DoubleClickPurchased,
        _ => false
    };

    public bool CanBuy(ShareXClickerUpgrade upgrade) => !IsPurchased(upgrade) && _state.Logos >= upgrade.Cost;

    public bool BuyUpgrade(ShareXClickerUpgrade upgrade)
    {
        if (!CanBuy(upgrade))
        {
            return false;
        }

        _state.Logos -= upgrade.Cost;
        switch (upgrade.Id)
        {
            case "better-click": _state.BetterClickPurchased = true; break;
            case "double-click": _state.DoubleClickPurchased = true; break;
            default: return false;
        }

        return true;
    }

    private void AddLogos(double amount)
    {
        if (amount <= 0 || double.IsNaN(amount) || double.IsInfinity(amount))
        {
            return;
        }

        _state.Logos += amount;
        _state.LifetimeLogos += amount;
    }

    private double CalculateLogosPerSecond()
    {
        double total = 0;
        foreach (ShareXClickerBuilding building in Buildings)
        {
            total += GetOwned(building) * building.ProductionPerSecond;
        }

        return total;
    }

    private void NormalizeState()
    {
        _state.Buildings ??= [];
        _state.Logos = IsValidAmount(_state.Logos) ? Math.Max(0, _state.Logos) : 0;
        _state.LifetimeLogos = IsValidAmount(_state.LifetimeLogos) ? Math.Max(0, _state.LifetimeLogos) : 0;
    }

    private static bool IsValidAmount(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}

public sealed record ShareXClickerBuilding(string Id, string Name, string Icon, double BaseCost, double ProductionPerSecond, double UnlockAt);
public sealed record ShareXClickerUpgrade(string Id, string Name, string Icon, double Cost, string Effect);
public static class ShareXClickerNumberFormatter
{
    private static readonly string[] Suffixes = ["", "K", "M", "B"];

    public static string Format(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return "0";

        int suffixIndex = 0;
        double scaled = Math.Max(0, value);
        while (scaled >= 1000 && suffixIndex < Suffixes.Length - 1)
        {
            scaled /= 1000;
            suffixIndex++;
        }

        return suffixIndex == 0 ? Math.Floor(scaled).ToString("0") : scaled.ToString(scaled >= 100 ? "0" : "0.0") + Suffixes[suffixIndex];
    }
}
