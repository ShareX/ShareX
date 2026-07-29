#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib;

public partial class ParserSelectWindow : Window
{
    private const int ItemsPerColumn = 10;

    public string[] Texts { get; }
    public string SelectedText { get; private set; }

    public ParserSelectWindow() : this([""])
    {
    }

    public ParserSelectWindow(IEnumerable<string> texts)
    {
        Texts = texts.Where(text => !string.IsNullOrEmpty(text)).ToArray();
        SelectedText = Texts.FirstOrDefault() ?? string.Empty;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        CreateButtons();

        Opened += OnOpened;
    }

    private void CreateButtons()
    {
        List<Button> buttons = [];

        for (int offset = 0; offset < Texts.Length; offset += ItemsPerColumn)
        {
            StackPanel column = new()
            {
                Spacing = 6
            };

            int count = Math.Min(ItemsPerColumn, Texts.Length - offset);
            for (int index = 0; index < count; index++)
            {
                string text = Texts[offset + index];
                Button button = new()
                {
                    Content = text
                };
                button.Classes.Add("parser-option");
                button.Click += (_, _) => Select(text);
                column.Children.Add(button);
                buttons.Add(button);
            }

            ColumnsPanel.Children.Add(column);
        }

        ColumnsPanel.Measure(Size.Infinity);
        double maxButtonWidth = buttons.Count > 0
            ? buttons.Max(button => Math.Ceiling(button.DesiredSize.Width))
            : 0;

        foreach (Button button in buttons)
        {
            button.Width = maxButtonWidth;
        }
    }

    private void OnOpened(object? sender, EventArgs e) => Activate();

    private void Select(string text)
    {
        SelectedText = text;
        Close();
    }
}
