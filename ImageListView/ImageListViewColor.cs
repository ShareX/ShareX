// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)
//
// Theme support coded by Robby

using System.ComponentModel;
using System.Reflection;

namespace ShareX.ImageListView;

/// <summary>
/// Represents the color palette of the image list view.
/// </summary>
[TypeConverter(typeof(ImageListViewColorTypeConverter))]
public class ImageListViewColor
{
    #region Member Variables
    // control background color
    Color mControlBackColor;
    Color mDisabledBackColor;

    // item colors
    Color mBackColor;
    Color mBorderColor;
    Color mUnFocusedColor1;
    Color mUnFocusedColor2;
    Color mUnFocusedBorderColor;
    Color mUnFocusedForeColor;
    Color mForeColor;
    Color mHoverColor1;
    Color mHoverColor2;
    Color mHoverBorderColor;
    Color mInsertionCaretColor;
    Color mSelectedColor1;
    Color mSelectedColor2;
    Color mSelectedBorderColor;
    Color mSelectedForeColor;
    Color mDisabledColor1;
    Color mDisabledColor2;
    Color mDisabledBorderColor;
    Color mDisabledForeColor;

    // thumbnail & pane
    Color mImageInnerBorderColor;
    Color mImageOuterBorderColor;

    // details view
    Color mCellForeColor;
    Color mColumnHeaderBackColor1;
    Color mColumnHeaderBackColor2;
    Color mColumnHeaderForeColor;
    Color mColumnHeaderHoverColor1;
    Color mColumnHeaderHoverColor2;
    Color mColumnSelectColor;
    Color mColumnSeparatorColor;
    Color mAlternateBackColor;
    Color mAlternateCellForeColor;

    // pane
    Color mPaneBackColor;
    Color mPaneSeparatorColor;
    Color mPaneLabelColor;

    // selection rectangle
    Color mSelectionRectangleColor1;
    Color mSelectionRectangleColor2;
    Color mSelectionRectangleBorderColor;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the background color of the ImageListView control.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color of the ImageListView control.")]
    [DefaultValue(typeof(Color), "Window")]
    public Color ControlBackColor
    {
        get { return mControlBackColor; }
        set { mControlBackColor = value; }
    }
    /// <summary>
    /// Gets or sets the background color of the ImageListView control in its disabled state.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color of the ImageListView control in its disabled state.")]
    [DefaultValue(typeof(Color), "Control")]
    public Color DisabledBackColor
    {
        get { return mDisabledBackColor; }
        set { mDisabledBackColor = value; }
    }
    /// <summary>
    /// Gets or sets the background color of the ImageListViewItem.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color of the ImageListViewItem.")]
    [DefaultValue(typeof(Color), "Window")]
    public Color BackColor
    {
        get { return mBackColor; }
        set { mBackColor = value; }
    }
    /// <summary>
    /// Gets or sets the background color of alternating cells in Details View.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the background color of alternating cells in Details View.")]
    [DefaultValue(typeof(Color), "Window")]
    public Color AlternateBackColor
    {
        get { return mAlternateBackColor; }
        set { mAlternateBackColor = value; }
    }
    /// <summary>
    /// Gets or sets the border color of the ImageListViewItem.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border color of the ImageListViewItem.")]
    [DefaultValue(typeof(Color), "64, 128, 128, 128")]
    public Color BorderColor
    {
        get { return mBorderColor; }
        set { mBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the foreground color of the ImageListViewItem.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the foreground color of the ImageListViewItem.")]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color ForeColor
    {
        get { return mForeColor; }
        set { mForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color1 of the ImageListViewItem if the control is not focused.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color1 of the ImageListViewItem if the control is not focused.")]
    [DefaultValue(typeof(Color), "16, 128, 128, 128")]
    public Color UnFocusedColor1
    {
        get { return mUnFocusedColor1; }
        set { mUnFocusedColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color2 of the ImageListViewItem if the control is not focused.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color2 of the ImageListViewItem if the control is not focused.")]
    [DefaultValue(typeof(Color), "64, 128, 128, 128")]
    public Color UnFocusedColor2
    {
        get { return mUnFocusedColor2; }
        set { mUnFocusedColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the border color of the ImageListViewItem if the control is not focused.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border color of the ImageListViewItem if the control is not focused.")]
    [DefaultValue(typeof(Color), "128, 128, 128, 128")]
    public Color UnFocusedBorderColor
    {
        get { return mUnFocusedBorderColor; }
        set { mUnFocusedBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the fore color of the ImageListViewItem if the control is not focused.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the fore color of the ImageListViewItem if the control is not focused.")]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color UnFocusedForeColor
    {
        get { return mUnFocusedForeColor; }
        set { mUnFocusedForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color1 if the ImageListViewItem is hovered.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color1 if the ImageListViewItem is hovered.")]
    [DefaultValue(typeof(Color), "8, 10, 36, 106")]
    public Color HoverColor1
    {
        get { return mHoverColor1; }
        set { mHoverColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color2 if the ImageListViewItem is hovered.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color2 if the ImageListViewItem is hovered.")]
    [DefaultValue(typeof(Color), "64, 10, 36, 106")]
    public Color HoverColor2
    {
        get { return mHoverColor2; }
        set { mHoverColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the border color of the ImageListViewItem if the item is hovered.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border color of the ImageListViewItem if the item is hovered.")]
    [DefaultValue(typeof(Color), "64, 10, 36, 106")]
    public Color HoverBorderColor
    {
        get { return mHoverBorderColor; }
        set { mHoverBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the color of the insertion caret.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the color of the insertion caret.")]
    [DefaultValue(typeof(Color), "Highlight")]
    public Color InsertionCaretColor
    {
        get { return mInsertionCaretColor; }
        set { mInsertionCaretColor = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color1 if the ImageListViewItem is selected.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color1 if the ImageListViewItem is selected.")]
    [DefaultValue(typeof(Color), "16, 10, 36, 106")]
    public Color SelectedColor1
    {
        get { return mSelectedColor1; }
        set { mSelectedColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color2 if the ImageListViewItem is selected.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color2 if the ImageListViewItem is selected.")]
    [DefaultValue(typeof(Color), "128, 10, 36, 106")]
    public Color SelectedColor2
    {
        get { return mSelectedColor2; }
        set { mSelectedColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the border color of the ImageListViewItem if the item is selected.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border color of the ImageListViewItem if the item is selected.")]
    [DefaultValue(typeof(Color), "128, 10, 36, 106")]
    public Color SelectedBorderColor
    {
        get { return mSelectedBorderColor; }
        set { mSelectedBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the fore color of the ImageListViewItem if the item is selected.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the fore color of the ImageListViewItem if the item is selected.")]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color SelectedForeColor
    {
        get { return mSelectedForeColor; }
        set { mSelectedForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color1 if the ImageListViewItem is disabled.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color1 if the ImageListViewItem is disabled.")]
    [DefaultValue(typeof(Color), "0, 128, 128, 128")]
    public Color DisabledColor1
    {
        get { return mDisabledColor1; }
        set { mDisabledColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color2 if the ImageListViewItem is disabled.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background gradient color2 if the ImageListViewItem is disabled.")]
    [DefaultValue(typeof(Color), "32, 128, 128, 128")]
    public Color DisabledColor2
    {
        get { return mDisabledColor2; }
        set { mDisabledColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the border color of the ImageListViewItem if the item is disabled.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the border color of the ImageListViewItem if the item is disabled.")]
    [DefaultValue(typeof(Color), "32, 128, 128, 128")]
    public Color DisabledBorderColor
    {
        get { return mDisabledBorderColor; }
        set { mDisabledBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the fore color of the ImageListViewItem if the item is disabled.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the fore color of the ImageListViewItem if the item is disabled.")]
    [DefaultValue(typeof(Color), "128, 128, 128")]
    public Color DisabledForeColor
    {
        get { return mDisabledForeColor; }
        set { mDisabledForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color1 of the column header.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the cells background color1 of the column header.")]
    [DefaultValue(typeof(Color), "32, 212, 208, 200")]
    public Color ColumnHeaderBackColor1
    {
        get { return mColumnHeaderBackColor1; }
        set { mColumnHeaderBackColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background gradient color2 of the column header.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the cells background color2 of the column header.")]
    [DefaultValue(typeof(Color), "196, 212, 208, 200")]
    public Color ColumnHeaderBackColor2
    {
        get { return mColumnHeaderBackColor2; }
        set { mColumnHeaderBackColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the background hover gradient color1 of the column header.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the background hover color1 of the column header.")]
    [DefaultValue(typeof(Color), "16, 10, 36, 106")]
    public Color ColumnHeaderHoverColor1
    {
        get { return mColumnHeaderHoverColor1; }
        set { mColumnHeaderHoverColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background hover gradient color2 of the column header.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the background hover color2 of the column header.")]
    [DefaultValue(typeof(Color), "64, 10, 36, 106")]
    public Color ColumnHeaderHoverColor2
    {
        get { return mColumnHeaderHoverColor2; }
        set { mColumnHeaderHoverColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the cells foreground color of the column header text.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the cells foreground color of the column header text.")]
    [DefaultValue(typeof(Color), "WindowText")]
    public Color ColumnHeaderForeColor
    {
        get { return mColumnHeaderForeColor; }
        set { mColumnHeaderForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the cells background color if column is selected in Details View.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the cells background color if column is selected in Details View.")]
    [DefaultValue(typeof(Color), "16, 128, 128, 128")]
    public Color ColumnSelectColor
    {
        get { return mColumnSelectColor; }
        set { mColumnSelectColor = value; }
    }
    /// <summary>
    /// Gets or sets the color of the separator in Details View.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the color of the separator in Details View.")]
    [DefaultValue(typeof(Color), "32, 128, 128, 128")]
    public Color ColumnSeparatorColor
    {
        get { return mColumnSeparatorColor; }
        set { mColumnSeparatorColor = value; }
    }
    /// <summary>
    /// Gets or sets the foreground color of the cell text in Details View.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the foreground color of the cell text in Details View.")]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color CellForeColor
    {
        get { return mCellForeColor; }
        set { mCellForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the foreground color of alternating cells text in Details View.
    /// </summary>
    [Category("Appearance Details View"), Description("Gets or sets the foreground color of alternating cells text in Details View.")]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color AlternateCellForeColor
    {
        get { return mAlternateCellForeColor; }
        set { mAlternateCellForeColor = value; }
    }
    /// <summary>
    /// Gets or sets the background color of the image pane.
    /// </summary>
    [Category("Appearance Pane View"), Description("Gets or sets the background color of the image pane.")]
    [DefaultValue(typeof(Color), "16, 128, 128, 128")]
    public Color PaneBackColor
    {
        get { return mPaneBackColor; }
        set { mPaneBackColor = value; }
    }
    /// <summary>
    /// Gets or sets the separator line color between image pane and thumbnail view.
    /// </summary>
    [Category("Appearance Pane View"), Description("Gets or sets the separator line color between image pane and thumbnail view.")]
    [DefaultValue(typeof(Color), "128, 128, 128, 128")]
    public Color PaneSeparatorColor
    {
        get { return mPaneSeparatorColor; }
        set { mPaneSeparatorColor = value; }
    }
    /// <summary>
    /// Gets or sets the color of labels in pane view.
    /// </summary>
    [Category("Appearance Pane View"), Description("Gets or sets the color of labels in pane view.")]
    [DefaultValue(typeof(Color), "196, 0, 0, 0")]
    public Color PaneLabelColor
    {
        get { return mPaneLabelColor; }
        set { mPaneLabelColor = value; }
    }
    /// <summary>
    /// Gets or sets the image inner border color for thumbnails and pane.
    /// </summary>
    [Category("Appearance Image"), Description("Gets or sets the image inner border color for thumbnails and pane.")]
    [DefaultValue(typeof(Color), "128, 255, 255, 255")]
    public Color ImageInnerBorderColor
    {
        get { return mImageInnerBorderColor; }
        set { mImageInnerBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the image outer border color for thumbnails and pane.
    /// </summary>
    [Category("Appearance Image"), Description("Gets or sets the image outer border color for thumbnails and pane.")]
    [DefaultValue(typeof(Color), "128, 128, 128, 128")]
    public Color ImageOuterBorderColor
    {
        get { return mImageOuterBorderColor; }
        set { mImageOuterBorderColor = value; }
    }
    /// <summary>
    /// Gets or sets the background color1 of the selection rectangle.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color1 of the selection rectangle.")]
    [DefaultValue(typeof(Color), "128, 10, 36, 106")]
    public Color SelectionRectangleColor1
    {
        get { return mSelectionRectangleColor1; }
        set { mSelectionRectangleColor1 = value; }
    }
    /// <summary>
    /// Gets or sets the background color2 of the selection rectangle.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the background color2 of the selection rectangle.")]
    [DefaultValue(typeof(Color), "128, 10, 36, 106")]
    public Color SelectionRectangleColor2
    {
        get { return mSelectionRectangleColor2; }
        set { mSelectionRectangleColor2 = value; }
    }
    /// <summary>
    /// Gets or sets the color of the selection rectangle border.
    /// </summary>
    [Category("Appearance"), Description("Gets or sets the color of the selection rectangle border.")]
    [DefaultValue(typeof(Color), "Highlight")]
    public Color SelectionRectangleBorderColor
    {
        get { return mSelectionRectangleBorderColor; }
        set { mSelectionRectangleBorderColor = value; }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the ImageListViewColor class.
    /// </summary>
    public ImageListViewColor()
    {
        // control
        mControlBackColor = SystemColors.Window;
        mDisabledBackColor = SystemColors.Control;

        // item
        mBackColor = SystemColors.Window;
        mForeColor = SystemColors.ControlText;

        mBorderColor = Color.FromArgb(64, SystemColors.GrayText);

        mUnFocusedColor1 = Color.FromArgb(16, SystemColors.GrayText);
        mUnFocusedColor2 = Color.FromArgb(64, SystemColors.GrayText);
        mUnFocusedBorderColor = Color.FromArgb(128, SystemColors.GrayText);
        mUnFocusedForeColor = SystemColors.ControlText;

        mHoverColor1 = Color.FromArgb(8, SystemColors.Highlight);
        mHoverColor2 = Color.FromArgb(64, SystemColors.Highlight);
        mHoverBorderColor = Color.FromArgb(64, SystemColors.Highlight);

        mSelectedColor1 = Color.FromArgb(16, SystemColors.Highlight);
        mSelectedColor2 = Color.FromArgb(128, SystemColors.Highlight);
        mSelectedBorderColor = Color.FromArgb(128, SystemColors.Highlight);
        mSelectedForeColor = SystemColors.ControlText;

        mDisabledColor1 = Color.FromArgb(0, SystemColors.GrayText);
        mDisabledColor2 = Color.FromArgb(32, SystemColors.GrayText);
        mDisabledBorderColor = Color.FromArgb(32, SystemColors.GrayText);
        mDisabledForeColor = Color.FromArgb(128, 128, 128);

        mInsertionCaretColor = SystemColors.Highlight;

        // thumbnails
        mImageInnerBorderColor = Color.FromArgb(128, Color.White);
        mImageOuterBorderColor = Color.FromArgb(128, Color.Gray);

        // details view
        mColumnHeaderBackColor1 = Color.FromArgb(32, SystemColors.Control);
        mColumnHeaderBackColor2 = Color.FromArgb(196, SystemColors.Control);
        mColumnHeaderHoverColor1 = Color.FromArgb(16, SystemColors.Highlight);
        mColumnHeaderHoverColor2 = Color.FromArgb(64, SystemColors.Highlight);
        mColumnHeaderForeColor = SystemColors.WindowText;
        mColumnSelectColor = Color.FromArgb(16, SystemColors.GrayText);
        mColumnSeparatorColor = Color.FromArgb(32, SystemColors.GrayText);
        mCellForeColor = SystemColors.ControlText;
        mAlternateBackColor = SystemColors.Window;
        mAlternateCellForeColor = SystemColors.ControlText;

        // image pane
        mPaneBackColor = Color.FromArgb(16, SystemColors.GrayText);
        mPaneSeparatorColor = Color.FromArgb(128, SystemColors.GrayText);
        mPaneLabelColor = Color.FromArgb(196, Color.Black);

        // selection rectangle
        mSelectionRectangleColor1 = Color.FromArgb(128, SystemColors.Highlight);
        mSelectionRectangleColor2 = Color.FromArgb(128, SystemColors.Highlight);
        mSelectionRectangleBorderColor = SystemColors.Highlight;
    }

    /// <summary>
    /// Initializes a new instance of the ImageListViewColor class
    /// from its string representation.
    /// </summary>
    /// <param name="definition">String representation of the object.</param>
    public ImageListViewColor(string definition)
        : this()
    {
        try
        {
            // First check if the color matches a predefined color setting
            foreach (MemberInfo info in typeof(ImageListViewColor).GetMembers(BindingFlags.Static | BindingFlags.Public))
            {
                if (info.MemberType == MemberTypes.Property)
                {
                    PropertyInfo propertyInfo = (PropertyInfo)info;
                    if (propertyInfo.PropertyType == typeof(ImageListViewColor))
                    {
                        // If the color setting is equal to a preset value
                        // return the preset
                        if (definition == string.Format("({0})", propertyInfo.Name) ||
                            definition == propertyInfo.Name)
                        {
                            ImageListViewColor presetValue = (ImageListViewColor)propertyInfo.GetValue(null, null);
                            CopyFrom(presetValue);
                            return;
                        }
                    }
                }
            }

            // Convert color values
            foreach (string line in definition.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                // Read the color setting
                string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                string name = pair[0].Trim();
                Color color = Color.FromName(pair[1].Trim());
                // Set the property value
                PropertyInfo property = typeof(ImageListViewColor).GetProperty(name);
                property.SetValue(this, color, null);
            }
        } catch
        {
            throw new ArgumentException("Invalid string format", "definition");
        }
    }
    #endregion

    #region Instance Methods
    /// <summary>
    /// Copies color values from the given object.
    /// </summary>
    /// <param name="source">The source object.</param>
    public void CopyFrom(ImageListViewColor source)
    {
        foreach (PropertyInfo info in typeof(ImageListViewColor).GetProperties())
        {
            // Walk through color properties
            if (info.PropertyType == typeof(Color))
            {
                Color color = (Color)info.GetValue(source, null);
                info.SetValue(this, color, null);
            }
        }
    }
    #endregion

    #region Static Members
    /// <summary>
    /// Represents the default color theme.
    /// </summary>
    public static ImageListViewColor Default { get { return GetDefaultTheme(); } }
    /// <summary>
    /// Represents the noir color theme.
    /// </summary>
    public static ImageListViewColor Noir { get { return GetNoirTheme(); } }
    /// <summary>
    /// Represents the mandarin color theme.
    /// </summary>
    public static ImageListViewColor Mandarin { get { return GetMandarinTheme(); } }

    /// <summary>
    /// Sets the color palette to default colors.
    /// </summary>
    private static ImageListViewColor GetDefaultTheme()
    {
        return new ImageListViewColor();
    }
    /// <summary>
    /// Sets the color palette to mandarin colors.
    /// </summary>
    private static ImageListViewColor GetMandarinTheme()
    {
        ImageListViewColor c = new();

        // control
        c.ControlBackColor = Color.White;
        c.DisabledBackColor = Color.FromArgb(220, 220, 220);

        // item
        c.BackColor = Color.White;
        c.ForeColor = Color.FromArgb(60, 60, 60);
        c.BorderColor = Color.FromArgb(187, 190, 183);

        c.UnFocusedColor1 = Color.FromArgb(235, 235, 235);
        c.UnFocusedColor2 = Color.FromArgb(217, 217, 217);
        c.UnFocusedBorderColor = Color.FromArgb(168, 169, 161);
        c.UnFocusedForeColor = Color.FromArgb(40, 40, 40);

        c.HoverColor1 = Color.Transparent;
        c.HoverColor2 = Color.Transparent;
        c.HoverBorderColor = Color.Transparent;

        c.SelectedColor1 = Color.FromArgb(244, 125, 77);
        c.SelectedColor2 = Color.FromArgb(235, 110, 60);
        c.SelectedBorderColor = Color.FromArgb(240, 119, 70);
        c.SelectedForeColor = Color.White;

        c.DisabledColor1 = Color.FromArgb(217, 217, 217);
        c.DisabledColor2 = Color.FromArgb(197, 197, 197);
        c.DisabledBorderColor = Color.FromArgb(128, 128, 128);
        c.DisabledForeColor = Color.FromArgb(128, 128, 128);

        c.InsertionCaretColor = Color.FromArgb(240, 119, 70);

        // thumbnails & pane
        c.ImageInnerBorderColor = Color.Transparent;
        c.ImageOuterBorderColor = Color.White;

        // details view
        c.CellForeColor = Color.FromArgb(60, 60, 60);
        c.ColumnHeaderBackColor1 = Color.FromArgb(247, 247, 247);
        c.ColumnHeaderBackColor2 = Color.FromArgb(235, 235, 235);
        c.ColumnHeaderHoverColor1 = Color.White;
        c.ColumnHeaderHoverColor2 = Color.FromArgb(245, 245, 245);
        c.ColumnHeaderForeColor = Color.FromArgb(60, 60, 60);
        c.ColumnSelectColor = Color.FromArgb(34, 128, 128, 128);
        c.ColumnSeparatorColor = Color.FromArgb(106, 128, 128, 128);
        c.mAlternateBackColor = Color.FromArgb(234, 234, 234);
        c.mAlternateCellForeColor = Color.FromArgb(40, 40, 40);

        // image pane
        c.PaneBackColor = Color.White;
        c.PaneSeparatorColor = Color.FromArgb(216, 216, 216);
        c.PaneLabelColor = Color.FromArgb(156, 156, 156);

        // selection rectangle
        c.SelectionRectangleColor1 = Color.FromArgb(64, 240, 116, 68);
        c.SelectionRectangleColor2 = Color.FromArgb(64, 240, 116, 68);
        c.SelectionRectangleBorderColor = Color.FromArgb(240, 119, 70);

        return c;
    }
    /// <summary>
    /// Sets the color palette to noir colors.
    /// </summary>
    private static ImageListViewColor GetNoirTheme()
    {
        ImageListViewColor c = new();

        // control
        c.ControlBackColor = Color.Black;
        c.DisabledBackColor = Color.Black;

        // item
        c.BackColor = Color.FromArgb(0x31, 0x31, 0x31);
        c.ForeColor = Color.LightGray;

        c.BorderColor = Color.DarkGray;

        c.UnFocusedColor1 = Color.FromArgb(16, SystemColors.GrayText);
        c.UnFocusedColor2 = Color.FromArgb(64, SystemColors.GrayText);
        c.UnFocusedBorderColor = Color.FromArgb(128, SystemColors.GrayText);
        c.UnFocusedForeColor = Color.LightGray;

        c.HoverColor1 = Color.FromArgb(64, Color.White);
        c.HoverColor2 = Color.FromArgb(16, Color.White);
        c.HoverBorderColor = Color.FromArgb(64, SystemColors.Highlight);

        c.SelectedColor1 = Color.FromArgb(64, 96, 160);
        c.SelectedColor2 = Color.FromArgb(64, 64, 96, 160);
        c.SelectedBorderColor = Color.FromArgb(128, SystemColors.Highlight);
        c.SelectedForeColor = Color.LightGray;

        c.DisabledColor1 = Color.FromArgb(0, SystemColors.GrayText);
        c.DisabledColor2 = Color.FromArgb(32, SystemColors.GrayText);
        c.DisabledBorderColor = Color.FromArgb(96, SystemColors.GrayText);
        c.DisabledForeColor = Color.LightGray;

        c.InsertionCaretColor = Color.FromArgb(96, 144, 240);

        // thumbnails & pane
        c.ImageInnerBorderColor = Color.FromArgb(128, Color.White);
        c.ImageOuterBorderColor = Color.FromArgb(128, Color.Gray);

        // details view
        c.CellForeColor = Color.WhiteSmoke;
        c.ColumnHeaderBackColor1 = Color.FromArgb(32, 128, 128, 128);
        c.ColumnHeaderBackColor2 = Color.FromArgb(196, 128, 128, 128);
        c.ColumnHeaderHoverColor1 = Color.FromArgb(64, 96, 144, 240);
        c.ColumnHeaderHoverColor2 = Color.FromArgb(196, 96, 144, 240);
        c.ColumnHeaderForeColor = Color.White;
        c.ColumnSelectColor = Color.FromArgb(96, 128, 128, 128);
        c.ColumnSeparatorColor = Color.Gold;
        c.AlternateBackColor = Color.FromArgb(0x31, 0x31, 0x31);
        c.AlternateCellForeColor = Color.WhiteSmoke;

        // image pane
        c.PaneBackColor = Color.FromArgb(0x31, 0x31, 0x31);
        c.PaneSeparatorColor = Color.Gold;
        c.PaneLabelColor = SystemColors.GrayText;

        // selection rectangke
        c.SelectionRectangleColor1 = Color.FromArgb(160, 96, 144, 240);
        c.SelectionRectangleColor2 = Color.FromArgb(32, 96, 144, 240);
        c.SelectionRectangleBorderColor = Color.FromArgb(64, 96, 144, 240);

        return c;
    }
    #endregion

    #region System.Object Overrides
    /// <summary>
    /// Determines whether all color values of the specified 
    /// ImageListViewColor are equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>true if the two instances have the same color values; 
    /// otherwise false.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null)
            throw new NullReferenceException();

        ImageListViewColor other = obj as ImageListViewColor;
        if (other == null) return false;

        foreach (PropertyInfo info in typeof(ImageListViewColor).GetProperties())
        {
            // Walk through color properties
            if (info.PropertyType == typeof(Color))
            {
                // Compare colors
                Color color1 = (Color)info.GetValue(this, null);
                Color color2 = (Color)info.GetValue(other, null);

                if (color1 != color2) return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in 
    /// hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Returns a string that represents this instance.
    /// </summary>
    /// <returns>
    /// A string that represents this instance.
    /// </returns>
    public override string ToString()
    {
        ImageListViewColor colors = this;

        // First check if the color matches a predefined color setting
        foreach (MemberInfo info in typeof(ImageListViewColor).GetMembers(BindingFlags.Static | BindingFlags.Public))
        {
            if (info.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = (PropertyInfo)info;
                if (propertyInfo.PropertyType == typeof(ImageListViewColor))
                {
                    ImageListViewColor presetValue = (ImageListViewColor)propertyInfo.GetValue(null, null);
                    // If the color setting is equal to a preset value
                    // return the name of the preset
                    if (colors.Equals(presetValue))
                        return string.Format("({0})", propertyInfo.Name);
                }
            }
        }

        // Serialize all colors which are different from the default setting
        List<string> lines = new();
        foreach (PropertyInfo info in typeof(ImageListViewColor).GetProperties())
        {
            // Walk through color properties
            if (info.PropertyType == typeof(Color))
            {
                // Get property name
                string name = info.Name;
                // Get the current value
                Color color = (Color)info.GetValue(colors, null);
                // Find the default value atribute
                Attribute[] attributes = (Attribute[])info.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attributes.Length != 0)
                {
                    // Get the default value
                    DefaultValueAttribute attribute = (DefaultValueAttribute)attributes[0];
                    Color defaultColor = (Color)attribute.Value;
                    // Serialize only if colors are different
                    if (color != defaultColor)
                    {
                        lines.Add(string.Format("{0} = {1}", name, color.Name));
                    }
                }
            }
        }

        return string.Join("; ", lines.ToArray());
    }
    #endregion
}
