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

using ShareX.HelpersLib;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class CutOutTool : BaseTool
    {
        public override ShapeType ShapeType { get; } = ShapeType.ToolCutOut;

        public override bool LimitRectangleToInsideCanvas { get; } = true;

        public bool IsHorizontalTrim => Rectangle.Width >= Options.MinimumSize && Rectangle.Width > Rectangle.Height;
        public bool IsVerticalTrim => Rectangle.Height >= Options.MinimumSize && Rectangle.Height >= Rectangle.Width;

        public override bool IsValidShape
        {
            get
            {
                if (!IsHorizontalTrim && !IsVerticalTrim) return false;
                if (IsHorizontalTrim && Rectangle.Left <= Manager.Form.CanvasRectangle.Left && Rectangle.Right >= Manager.Form.CanvasRectangle.Right) return false;
                if (IsVerticalTrim && Rectangle.Top <= Manager.Form.CanvasRectangle.Top && Rectangle.Bottom >= Manager.Form.CanvasRectangle.Bottom) return false;
                return true;
            }
        }

        public RectangleF CutOutRectangle
        {
            get
            {
                if (IsHorizontalTrim)
                {
                    return new RectangleF(Rectangle.X, Manager.Form.CanvasRectangle.Y, Rectangle.Width, Manager.Form.CanvasRectangle.Height);
                }
                if (IsVerticalTrim)
                {
                    return new RectangleF(Manager.Form.CanvasRectangle.X, Rectangle.Y, Manager.Form.CanvasRectangle.Width, Rectangle.Height);
                }
                return RectangleF.Empty;
            }
        }

        private ImageEditorButton confirmButton, cancelButton;
        private Size buttonSize = new Size(80, 40);
        private int buttonOffset = 15;

        public override void ShowNodes()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (confirmButton != null && cancelButton != null)
            {
                if (IsVerticalTrim)
                {
                    float spaceBelow = Manager.Form.ClientArea.Bottom - Rectangle.Bottom;
                    bool positionBelow = spaceBelow >= buttonSize.Height + 2 * buttonOffset;
                    float buttonsTop = positionBelow ? Rectangle.Bottom + buttonOffset : Rectangle.Top - buttonOffset - buttonSize.Height;
                    float buttonsLeft = Rectangle.Left + Rectangle.Width / 2 - (2 * buttonSize.Width + buttonOffset) / 2;
                    float buttonsRight = buttonsLeft + 2 * buttonSize.Width + buttonOffset;
                    bool overflowsLeft = buttonsLeft < Manager.Form.ClientArea.Left + buttonOffset;
                    bool overflowsRight = buttonsRight >= Manager.Form.ClientArea.Right - buttonOffset;
                    if (overflowsLeft && overflowsRight)
                    {
                        // can't fix
                    }
                    else if (overflowsLeft)
                    {
                        buttonsLeft = Manager.Form.ClientArea.Left + buttonOffset;
                    }
                    else if (overflowsRight)
                    {
                        buttonsRight = Manager.Form.ClientArea.Right - buttonOffset;
                        buttonsLeft = buttonsRight - 2 * buttonSize.Width - buttonOffset;
                    }
                    confirmButton.Rectangle = new RectangleF(buttonsLeft, buttonsTop, buttonSize.Width, buttonSize.Height);
                    cancelButton.Rectangle = confirmButton.Rectangle.LocationOffset(buttonSize.Width + buttonOffset, 0);
                }
                else
                {
                    float spaceRight = Manager.Form.ClientArea.Right - Rectangle.Right;
                    bool positionRight = spaceRight >= buttonSize.Width + 2 * buttonOffset;
                    float buttonsLeft = positionRight ? Rectangle.Right + buttonOffset : Rectangle.Left - buttonOffset - buttonSize.Width;
                    float buttonsTop = Rectangle.Top + Rectangle.Height / 2 - (2 * buttonSize.Height + buttonOffset) / 2;
                    float buttonsBottom = buttonsTop + 2 * buttonSize.Height + buttonOffset;
                    bool overflowsTop = buttonsTop < Manager.Form.ClientArea.Top + buttonOffset;
                    bool overflowsBottom = buttonsBottom >= Manager.Form.ClientArea.Bottom - buttonOffset;
                    if (overflowsTop && overflowsBottom)
                    {
                        // can't fix
                    }
                    else if (overflowsTop)
                    {
                        buttonsTop = Manager.Form.ClientArea.Top + buttonOffset;
                    }
                    else if (overflowsBottom)
                    {
                        buttonsBottom = Manager.Form.ClientArea.Bottom - buttonOffset;
                        buttonsTop = buttonsBottom - 2 * buttonSize.Height - buttonOffset;
                    }
                    confirmButton.Rectangle = new RectangleF(buttonsLeft, buttonsTop, buttonSize.Width, buttonSize.Height);
                    cancelButton.Rectangle = confirmButton.Rectangle.LocationOffset(0, buttonSize.Height + buttonOffset);
                }
            }
        }

        public override void OnDraw(Graphics g)
        {
            using (Image selectionHighlightPattern = ImageHelpers.CreateCheckerPattern(1, 1, Color.FromArgb(128, Color.LightGray), Color.FromArgb(128, Color.Gray)))
            using (Brush selectionHighlightBrush = new TextureBrush(selectionHighlightPattern, WrapMode.Tile))
            {
                g.FillRectangle(selectionHighlightBrush, CutOutRectangle);
            }
        }

        public override void OnCreated()
        {
            confirmButton = new ImageEditorButton()
            {
                Text = "\u2714",
                ButtonColor = Color.ForestGreen,
                Rectangle = new Rectangle(new Point(), buttonSize),
                Visible = true
            };
            confirmButton.MouseDown += ConfirmButton_MousePressed;
            confirmButton.MouseEnter += () => Manager.Form.Cursor = Cursors.Hand;
            confirmButton.MouseLeave += () => Manager.Form.SetDefaultCursor();
            Manager.DrawableObjects.Add(confirmButton);

            cancelButton = new ImageEditorButton()
            {
                Text = "\u2716",
                ButtonColor = Color.FromArgb(227, 45, 45),
                Rectangle = new Rectangle(new Point(), buttonSize),
                Visible = true
            };
            cancelButton.MouseDown += CancelButton_MousePressed;
            cancelButton.MouseEnter += () => Manager.Form.Cursor = Cursors.Hand;
            cancelButton.MouseLeave += () => Manager.Form.SetDefaultCursor();
            Manager.DrawableObjects.Add(cancelButton);
        }

        private void ConfirmButton_MousePressed(object sender, MouseEventArgs e)
        {
            Manager.CutOut(Rectangle);
            Remove();
        }

        private void CancelButton_MousePressed(object sender, MouseEventArgs e)
        {
            Remove();
        }

        public override void Remove()
        {
            base.Remove();

            if (Options.SwitchToSelectionToolAfterDrawing)
            {
                Manager.CurrentTool = ShapeType.ToolSelect;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if ((confirmButton != null && confirmButton.IsCursorHover) || (cancelButton != null && cancelButton.IsCursorHover))
            {
                Manager.Form.SetDefaultCursor();
            }

            if (confirmButton != null)
            {
                Manager.DrawableObjects.Remove(confirmButton);
            }

            if (cancelButton != null)
            {
                Manager.DrawableObjects.Remove(cancelButton);
            }
        }
    }
}