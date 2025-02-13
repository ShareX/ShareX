#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Manina.Windows.Forms;
using ShareX.HelpersLib;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public class HistoryImageListViewRenderer : ImageListView.ImageListViewRenderer
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

                Color hoverColor;
                if (ShareXResources.IsDarkTheme)
                {
                    hoverColor = ColorHelpers.LighterColor(ShareXResources.Theme.LightBackgroundColor, 0.1f);
                }
                else
                {
                    hoverColor = ColorHelpers.DarkerColor(ShareXResources.Theme.LightBackgroundColor, 0.1f);
                }
                ImageListView.Colors.SelectedColor1 = ImageListView.Colors.HoverColor1 = ImageListView.Colors.UnFocusedColor1 =
                    ImageListView.Colors.SelectedColor2 = ImageListView.Colors.HoverColor2 = ImageListView.Colors.UnFocusedColor2 = hoverColor;
            }
            else
            {
                ImageListView.Colors.BackColor = SystemColors.Control;
                ImageListView.Colors.SelectedColor1 = ImageListView.Colors.HoverColor1 = ImageListView.Colors.UnFocusedColor1 =
                    ImageListView.Colors.SelectedColor2 = ImageListView.Colors.HoverColor2 = ImageListView.Colors.UnFocusedColor2 = SystemColors.ControlLight;
            }
        }

        public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
        {
            Clip = false;

            Size itemPadding = new Size(4, 4);
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
                using (Brush bItemBack = new SolidBrush(ImageListView.Colors.BackColor))
                {
                    g.FillRectangle(bItemBack, bounds);
                }
            }
            else
            {
                using (Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor))
                {
                    g.FillRectangle(bItemBack, bounds);
                }
            }

            if ((state & ItemState.Disabled) != ItemState.None) // Paint background Disabled
            {
                using (Brush bDisabled = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.DisabledColor1, ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bDisabled, bounds);
                }
            }
            else if ((ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) ||
                (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) && ((state & ItemState.Hovered) != ItemState.None))) // Paint background Selected
            {
                using (Brush bSelected = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bSelected, bounds);
                }
            }
            else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) // Paint background unfocused
            {
                using (Brush bGray64 = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bGray64, bounds);
                }
            }

            // Paint background Hovered
            if ((state & ItemState.Hovered) != ItemState.None)
            {
                using (Brush bHovered = new LinearGradientBrush(bounds.Offset(1), ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bHovered, bounds);
                }
            }

            // Draw the image
            Image img = item.GetCachedImage(CachedImageType.Thumbnail);
            if (img != null)
            {
                Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(imageBounds.Location + itemPadding, ImageListView.ThumbnailSize));
                g.DrawImage(img, pos);
            }

            // Draw item text
            Color foreColor = ImageListView.Colors.ForeColor;
            if ((state & ItemState.Disabled) != ItemState.None)
            {
                foreColor = ImageListView.Colors.DisabledForeColor;
            }
            else if ((state & ItemState.Selected) != ItemState.None)
            {
                if (ImageListView.Focused)
                {
                    foreColor = ImageListView.Colors.SelectedForeColor;
                }
                else
                {
                    foreColor = ImageListView.Colors.UnFocusedForeColor;
                }
            }

            Rectangle rt = new Rectangle(bounds.Left, bounds.Top + (2 * itemPadding.Height) + ImageListView.ThumbnailSize.Height, bounds.Width, szt.Height);
            TextFormatFlags flags;

            if ((state & ItemState.Hovered) != ItemState.None)
            {
                flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoClipping;
            }
            else
            {
                flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;
            }

            TextRenderer.DrawText(g, text, ImageListView.Font, rt, foreColor, flags);
        }
    }
}