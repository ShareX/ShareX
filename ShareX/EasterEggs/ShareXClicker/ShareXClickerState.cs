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

using System.Collections.Generic;

namespace ShareX;

public sealed class ShareXClickerState
{
    public double Logos;
    public double LifetimeLogos;
    public Dictionary<string, int> Buildings = [];
    public bool BetterClickPurchased;
    public bool DoubleClickPurchased;
    public bool StoreDiscovered;
    public long LastActiveUtcTicks;
}
