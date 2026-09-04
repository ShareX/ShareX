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
using System.Linq;

namespace ShareX;

public sealed class ShareXClickerGame
{
    public const double StoreUnlockAt = 10;
    public const double EmpireGoalAt = 10_000_000;
    public static readonly IReadOnlyList<int> BuildingMilestones = [10, 25, 50, 100];

    public static readonly IReadOnlyList<ShareXClickerBuilding> Buildings =
    [
        new("auto-clicker", "Auto Clicker", LucideIcons.mouse_pointer_click, 15, 1, 10),
        new("sharex-bot", "ShareX Bot", LucideIcons.bot, 100, 6, 100),
        new("screenshot-station", "Screenshot Station", LucideIcons.monitor, 600, 30, 500),
        new("upload-server", "Upload Server", LucideIcons.server, 3500, 150, 2500),
        new("screenshot-factory", "Screenshot Factory", LucideIcons.factory, 20000, 800, 10000),
        new("sharex-datacenter", "ShareX Datacenter", LucideIcons.database, 120000, 4000, 50000),
        new("capture-cloud", "Capture Cloud", LucideIcons.cloud, 700000, 20000, 250000),
        new("sharex-satellite", "ShareX Satellite", LucideIcons.satellite, 4000000, 100000, 1000000)
    ];

    public static readonly IReadOnlyList<ShareXClickerUpgrade> Upgrades =
    [
        new("better-click", "Better Click", LucideIcons.mouse_pointer_click, 50, 25, "+1 base logo per click", 1, 1, 1),
        new("double-click", "Double Click", LucideIcons.mouse, 500, 250, "2x manual click value", 0, 2, 1),
        new("workflow-automation", "Workflow Automation", LucideIcons.zap, 2500, 1000, "2x generator output", 0, 1, 2),
        new("pixel-perfect", "Pixel Perfect", LucideIcons.target, 15000, 7500, "+4 base logos per click", 4, 1, 1),
        new("parallel-uploads", "Parallel Uploads", LucideIcons.cloud_upload, 75000, 35000, "3x generator output", 0, 1, 3),
        new("quantum-compression", "Quantum Compression", LucideIcons.sparkles, 1000000, 500000, "2x all logo production", 0, 2, 2)
    ];

    private readonly ShareXClickerState _state;
    public ShareXClickerGame(ShareXClickerState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        NormalizeState();
    }

    public ShareXClickerState State => _state;
    public double LogosPerSecond => CalculateLogosPerSecond();
    public double ClickValue => (1 + PurchasedUpgrades.Sum(x => x.ClickBonus)) * PurchasedUpgrades.Aggregate(1d, (value, upgrade) => value * upgrade.ClickMultiplier);
    public double ProductionMultiplier => PurchasedUpgrades.Aggregate(1d, (value, upgrade) => value * upgrade.ProductionMultiplier);
    public long TotalBuildings => Buildings.Sum(x => (long)GetOwned(x));

    private IEnumerable<ShareXClickerUpgrade> PurchasedUpgrades => Upgrades.Where(IsPurchased);

    public double Click()
    {
        double clickGain = ClickValue;
        AddLogos(clickGain);
        if (_state.ManualClicks < long.MaxValue)
        {
            _state.ManualClicks++;
        }
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

    public bool CanBuy(ShareXClickerBuilding building, int quantity = 1)
    {
        int owned = GetOwned(building);
        return Buildings.Contains(building) && quantity > 0 && quantity <= int.MaxValue - owned && IsUnlocked(building) && _state.Logos >= GetBuildingCost(building, quantity);
    }

    public bool BuyBuilding(ShareXClickerBuilding building, int quantity = 1)
    {
        if (!CanBuy(building, quantity))
        {
            return false;
        }

        double cost = GetBuildingCost(building, quantity);
        _state.Logos -= cost;
        _state.Buildings[building.Id] = GetOwned(building) + quantity;
        return true;
    }

    public bool IsUnlocked(ShareXClickerBuilding building) => _state.LifetimeLogos >= building.UnlockAt;

    public int GetOwned(ShareXClickerBuilding building) => _state.Buildings.TryGetValue(building.Id, out int owned) ? Math.Max(owned, 0) : 0;

    public double GetBuildingCost(ShareXClickerBuilding building, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return 0;
        }

        int owned = GetOwned(building);
        if (quantity > int.MaxValue - owned)
        {
            return double.MaxValue;
        }

        double total = 0;
        for (int i = 0; i < quantity; i++)
        {
            double itemCost = Math.Ceiling(building.BaseCost * Math.Pow(1.15, owned + i));
            if (double.IsInfinity(itemCost) || total > double.MaxValue - itemCost)
            {
                return double.MaxValue;
            }

            total += itemCost;
        }

        return total;
    }

    public int GetMaxAffordableQuantity(ShareXClickerBuilding building)
    {
        if (!IsUnlocked(building))
        {
            return 0;
        }

        int quantity = 0;
        int owned = GetOwned(building);
        double total = 0;
        while (owned + quantity < int.MaxValue)
        {
            double itemCost = Math.Ceiling(building.BaseCost * Math.Pow(1.15, owned + quantity));
            if (double.IsInfinity(itemCost) || total > _state.Logos - itemCost)
            {
                break;
            }

            total += itemCost;
            quantity++;
        }

        return quantity;
    }

    public double GetBuildingProductionPerSecond(ShareXClickerBuilding building)
    {
        return building.ProductionPerSecond * GetBuildingMilestoneMultiplier(building) * ProductionMultiplier;
    }

    public double GetBuildingTotalProductionPerSecond(ShareXClickerBuilding building) => GetOwned(building) * GetBuildingProductionPerSecond(building);

    public int GetBuildingMilestoneMultiplier(ShareXClickerBuilding building)
    {
        int owned = GetOwned(building);
        int completedMilestones = BuildingMilestones.Count(x => owned >= x);
        return 1 << completedMilestones;
    }

    public int? GetNextBuildingMilestone(ShareXClickerBuilding building)
    {
        int owned = GetOwned(building);
        foreach (int milestone in BuildingMilestones)
        {
            if (owned < milestone)
            {
                return milestone;
            }
        }

        return null;
    }

    public bool IsUnlocked(ShareXClickerUpgrade upgrade) => _state.LifetimeLogos >= upgrade.UnlockAt;

    public bool IsPurchased(ShareXClickerUpgrade upgrade) => _state.PurchasedUpgrades.Contains(upgrade.Id);

    public bool CanBuy(ShareXClickerUpgrade upgrade) => Upgrades.Contains(upgrade) && IsUnlocked(upgrade) && !IsPurchased(upgrade) && _state.Logos >= upgrade.Cost;

    public bool BuyUpgrade(ShareXClickerUpgrade upgrade)
    {
        if (!CanBuy(upgrade))
        {
            return false;
        }

        _state.Logos -= upgrade.Cost;
        _state.PurchasedUpgrades.Add(upgrade.Id);
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
            total += GetBuildingTotalProductionPerSecond(building);
        }

        return total;
    }

    private void NormalizeState()
    {
        _state.Buildings ??= [];
        _state.PurchasedUpgrades ??= [];
        _state.Logos = IsValidAmount(_state.Logos) ? Math.Max(0, _state.Logos) : 0;
        _state.LifetimeLogos = IsValidAmount(_state.LifetimeLogos) ? Math.Max(0, _state.LifetimeLogos) : 0;
        _state.LifetimeLogos = Math.Max(_state.Logos, _state.LifetimeLogos);
        _state.ManualClicks = Math.Max(0, _state.ManualClicks);
        _state.StoreDiscovered |= _state.LifetimeLogos >= StoreUnlockAt;
    }

    private static bool IsValidAmount(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}

public sealed record ShareXClickerBuilding(string Id, string Name, string Icon, double BaseCost, double ProductionPerSecond, double UnlockAt);
public sealed record ShareXClickerUpgrade(string Id, string Name, string Icon, double Cost, double UnlockAt, string Effect,
    double ClickBonus, double ClickMultiplier, double ProductionMultiplier);

public static class ShareXClickerNumberFormatter
{
    private static readonly string[] Suffixes = ["", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc"];

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
