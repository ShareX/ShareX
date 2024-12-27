using ShareX.HelpersLib;
using ShareX.HelpersLib.Extensions;
using ShareX.HelpersLib.Helpers;
using ShareX.ImageListView;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HistoryLib;

public class HistoryImageListViewRenderer : ImageListView.ImageListView.ImageListViewRenderer
{
    public override void InitializeGraphics(Graphics g)
    {
        base.InitializeGraphics(g);

        ItemDrawOrder = ItemDrawOrder.NormalSelectedHovered;

        if (ShareXResources.UseCustomTheme)
        {
            ImageListView.BackColor = ShareXResources.Theme.BackgroundColor;
            ImageListView.Colors.BackColor = ShareXResources.Theme.LightBackgroundColor;
            ImageListView.Colors.BorderColor = ShareXResources.Theme.BorderColor;
            ImageListView.Colors.ForeColor = ShareXResources.Theme.TextColor;
            ImageListView.Colors.SelectedForeColor = ShareXResources.Theme.TextColor;
            ImageListView.Colors.UnFocusedForeColor = ShareXResources.Theme.TextColor;

            System.Drawing.Color hoverColor = ShareXResources.IsDarkTheme
                ? ColorHelpers.LighterColor(ShareXResources.Theme.LightBackgroundColor, 0.1f)
                : ColorHelpers.DarkerColor(ShareXResources.Theme.LightBackgroundColor, 0.1f);
            ImageListView.Colors.SelectedColor1 = ImageListView.Colors.HoverColor1 = ImageListView.Colors.UnFocusedColor1 =
                ImageListView.Colors.SelectedColor2 = ImageListView.Colors.HoverColor2 = ImageListView.Colors.UnFocusedColor2 = hoverColor;
        } else
        {
            ImageListView.Colors.BackColor = SystemColors.Control;
            ImageListView.Colors.SelectedColor1 = ImageListView.Colors.HoverColor1 = ImageListView.Colors.UnFocusedColor1 =
                ImageListView.Colors.SelectedColor2 = ImageListView.Colors.HoverColor2 = ImageListView.Colors.UnFocusedColor2 = SystemColors.ControlLight;
        }
    }

    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        Clip = false;

        Size itemPadding = new(4, 4);
        Rectangle imageBounds = bounds;

        string text = Path.GetFileNameWithoutExtension(item.Text);
        Size szt = TextRenderer.MeasureText(text, ImageListView.Font);
        int textWidth = szt.Width + (itemPadding.Width * 2);

        if ((state & ItemState.Hovered) != ItemState.None && textWidth > bounds.Width)
        {
            bounds = new Rectangle(bounds.X + (bounds.Width / 2) - (textWidth / 2), bounds.Y, textWidth, bounds.Height);
        }

        // Paint background
        if (ImageListView.Enabled)
        {
            using System.Drawing.Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor);
            g.FillRectangle(bItemBack, bounds);
        } else
        {
            using System.Drawing.Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor);
            g.FillRectangle(bItemBack, bounds);
        }

        if ((state & ItemState.Disabled) != ItemState.None) // Paint background Disabled
        {
            using LinearGradientBrush bDisabled = new(bounds.Offset(1), ImageListView.Colors.DisabledColor1, ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bDisabled, bounds);
        } else if ((ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) ||
              (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) && ((state & ItemState.Hovered) != ItemState.None))) // Paint background Selected
        {
            using System.Drawing.Brush bSelected = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bSelected, bounds);
        } else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) // Paint background unfocused
        {
            using System.Drawing.Brush bGray64 = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bGray64, bounds);
        }

        // Paint background Hovered
        if ((state & ItemState.Hovered) != ItemState.None)
        {
            using System.Drawing.Brush bHovered = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical);
            g.FillRectangle(bHovered, bounds);
        }

        // Draw the image
        Image img = item.GetCachedImage(CachedImageType.Thumbnail);
        if (img != null)
        {
            Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(imageBounds.Location + itemPadding, ImageListView.ThumbnailSize));
            g.DrawImage(img, pos);
        }

        // Draw item text
        System.Drawing.Color foreColor = ImageListView.Colors.ForeColor;
        if ((state & ItemState.Disabled) != ItemState.None)
        {
            foreColor = ImageListView.Colors.DisabledForeColor;
        } else if ((state & ItemState.Selected) != ItemState.None)
        {
            foreColor = ImageListView.Focused ? ImageListView.Colors.SelectedForeColor : ImageListView.Colors.UnFocusedForeColor;
        }

        Rectangle rt = new(bounds.Left, bounds.Top + (2 * itemPadding.Height) + ImageListView.ThumbnailSize.Height, bounds.Width, szt.Height);
        TextFormatFlags flags = (state & ItemState.Hovered) != ItemState.None
            ? TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoClipping
            : TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;
        TextRenderer.DrawText(g, text, ImageListView.Font, rt, foreColor, flags);
    }
}