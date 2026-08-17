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
using ShareX.ImageEditor.Presentation.Controls;

namespace ShareX.ImageEditor.Presentation.Views;

public partial class EditorBuiltInToolbars : UserControl
{
    public AnnotationToolbar AnnotationToolbar { get; private set; } = null!;

    public event EventHandler<double>? ZoomChanged;
    public event EventHandler? ZoomToFitRequested;

    public EditorBuiltInToolbars()
    {
        AvaloniaXamlLoader.Load(this);
        AnnotationToolbar = this.FindControl<AnnotationToolbar>("AnnotationToolbarControl")!;
    }

    public void OpenFileMenu() => AnnotationToolbar.OpenFileMenu();

    private void OnZoomChanged(object? sender, double zoom) => ZoomChanged?.Invoke(this, zoom);

    private void OnZoomToFitRequested(object? sender, EventArgs e) => ZoomToFitRequested?.Invoke(this, EventArgs.Empty);
}
