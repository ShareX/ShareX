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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class AnalyzeImageOptionsWindow : Window
{
    private readonly AnalyzeImageOptionsViewModel _viewModel;

    public AnalyzeImageOptionsWindow()
        : this(new AnalyzeImageOptions(), _ => Task.FromResult(new AnalyzeImageConnectionResult(false, string.Empty)),
            _ => Task.FromResult<IReadOnlyList<string>>([]))
    {
    }

    public AnalyzeImageOptionsWindow(AnalyzeImageOptions options,
        AnalyzeImageTestConnectionHandler testConnection,
        AnalyzeImageLoadModelsHandler loadModels,
        Action<AnalyzeImageOptions>? optionsChanged = null)
    {
        _viewModel = new AnalyzeImageOptionsViewModel(options, testConnection, loadModels, optionsChanged);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.CloseRequested = saved => Close(saved);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
