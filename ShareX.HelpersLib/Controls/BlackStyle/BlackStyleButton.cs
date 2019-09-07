#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [DefaultEvent("MouseClick")]
    public class BlackStyleButton : Control, IButtonControl
    {
        // the devdocs I copied directly from the net472 reference sources to System.Windows.Forms.Button to also allow registering this as a "default" button.
        // I am going to need help with the state images of default button and default cancel buttons on a form.
        /// <devdoc>
        ///     The dialog result that will be sent to the parent dialog form when
        ///     we are clicked.
        /// </devdoc>
        private DialogResult dialogResult;
        private string text;
        private bool isHover;
        private bool isEnabled = true; // default to true.
        private LinearGradientBrush backgroundBrush;
        private LinearGradientBrush backgroundHoverBrush;
        private LinearGradientBrush backgroundDisabledBrush;
        private LinearGradientBrush innerBorderBrush;
        private LinearGradientBrush innerBorderDisabledBrush;
        private Pen innerBorderPen;
        private Pen innerBorderDisabledPen;
        private Pen borderPen;
        private Pen borderDisabledPen;

        public BlackStyleButton()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.ForeColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 8.25f, GraphicsUnit.Point);
            this.borderPen = new Pen(Color.FromArgb(30, 30, 30));
            this.borderDisabledPen = new Pen(Color.FromArgb(142, 142, 142));
            this.Prepare();
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override string Text
        {
            get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;

                    this.Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(DialogResult.None)]
        [Description("TODO: Add real DialogResult description from original System.Windows.Forms.Button.")]
        public virtual DialogResult DialogResult
        {
            get => this.dialogResult;

            set
            {
                // if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                // {
                //     throw new InvalidEnumArgumentException("value", (int)value, typeof(DialogResult));
                // }
                this.dialogResult = value;
            }
        }

        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize => new Size(75, 23);

        private bool IsDefault { get; set; }

        /// <devdoc>
        ///    <para>
        ///       Notifies the <see cref='ThemedButton'/>
        ///       whether it is the default button so that it can adjust its appearance
        ///       accordingly.
        ///
        ///    </para>
        /// </devdoc>
        public virtual void NotifyDefault(bool value)
        {
            if (this.IsDefault != value)
            {
                this.IsDefault = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Generates a <see cref='Control.Click'/> event for a
        ///       button.
        ///    </para>
        /// </devdoc>
        public void PerformClick()
        {
            if (this.CanSelect)
            {
                // TODO: Paint button down.
                // bool validate = ValidateActiveControl(out var validatedControlAllowsFocusChange);
                // if (!ValidationCancelled && (validate || validatedControlAllowsFocusChange))
                // {
                //     // Paint in raised state...
                //     ResetFlagsandPaint();
                this.OnClick(EventArgs.Empty);

                // }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            var g = pe.Graphics;

            this.DrawBackground(g);

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.DrawText(g);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            this.isHover = true;
            this.Invalidate();
        }

        /// <devdoc>
        ///    <para>
        ///       This method actually raises the Click event. Inheriting classes should
        ///       override this if they wish to be notified of a Click event. (This is far
        ///       preferable to actually adding an event handler.) They should not,
        ///       however, forget to call base.onClick(e); before exiting, to ensure that
        ///       other recipients do actually get the event.
        ///
        ///    </para>
        /// </devdoc>
        protected override void OnClick(EventArgs e)
        {
#pragma warning disable IDISP001 // Dispose created.
            var form = this.FindForm();
#pragma warning restore IDISP001 // Dispose created.
            if (form != null)
            {
                form.DialogResult = this.dialogResult;
            }

            // accessibility stuff
            // this.AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            // this.AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            base.OnClick(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            this.isEnabled = this.Enabled;

            this.Prepare();
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.backgroundBrush?.Dispose();
                this.backgroundHoverBrush?.Dispose();
                this.backgroundDisabledBrush?.Dispose();
                this.innerBorderBrush?.Dispose();
                this.innerBorderDisabledBrush?.Dispose();
                this.innerBorderPen?.Dispose();
                this.innerBorderDisabledPen?.Dispose();
                this.borderPen?.Dispose();
                this.borderDisabledPen?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.isHover = false;
            this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Prepare();
        }

        private void Prepare()
        {
            this.backgroundBrush?.Dispose();
            this.backgroundBrush = new LinearGradientBrush(new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4), Color.FromArgb(105, 105, 105), Color.FromArgb(65, 65, 65), LinearGradientMode.Vertical);
            this.backgroundHoverBrush?.Dispose();
            this.backgroundHoverBrush = new LinearGradientBrush(new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4), Color.FromArgb(115, 115, 115), Color.FromArgb(75, 75, 75), LinearGradientMode.Vertical);
            this.backgroundDisabledBrush?.Dispose();
            this.backgroundDisabledBrush = new LinearGradientBrush(new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4), Color.FromArgb(180, 180, 180), Color.FromArgb(160, 160, 160), LinearGradientMode.Vertical);
            this.innerBorderBrush?.Dispose();
            this.innerBorderBrush = new LinearGradientBrush(new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - 2), Color.FromArgb(125, 125, 125), Color.FromArgb(75, 75, 75), LinearGradientMode.Vertical);
            this.innerBorderDisabledBrush?.Dispose();
            this.innerBorderDisabledBrush = new LinearGradientBrush(new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - 2), Color.FromArgb(190, 190, 190), Color.FromArgb(165, 165, 165), LinearGradientMode.Vertical);
            this.innerBorderPen?.Dispose();
            this.innerBorderPen = new Pen(this.innerBorderBrush);
            this.innerBorderDisabledPen?.Dispose();
            this.innerBorderDisabledPen = new Pen(this.innerBorderDisabledBrush);
        }

        private void DrawBackground(Graphics g)
        {
            if (this.isEnabled)
            {
                // enabled state.
                if (this.isHover)
                {
                    g.FillRectangle(this.backgroundHoverBrush, new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4));
                }
                else
                {
                    g.FillRectangle(this.backgroundBrush, new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4));
                }

                g.DrawRectangle(this.innerBorderPen, new Rectangle(1, 1, this.ClientSize.Width - 3, this.ClientSize.Height - 3));
                g.DrawRectangle(this.borderPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
            else
            {
                // disabled state.
                g.FillRectangle(this.backgroundDisabledBrush, new Rectangle(2, 2, this.ClientSize.Width - 4, this.ClientSize.Height - 4));
                g.DrawRectangle(this.innerBorderDisabledPen, new Rectangle(1, 1, this.ClientSize.Width - 3, this.ClientSize.Height - 3));
                g.DrawRectangle(this.borderDisabledPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }

        private void DrawText(Graphics g)
        {
            if (this.isEnabled)
            {
                TextRenderer.DrawText(g, this.Text, this.Font, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y + 1, this.ClientRectangle.Width, this.ClientRectangle.Height), Color.Black);
                TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor);
            }
            else
            {
                TextRenderer.DrawText(g, this.Text, this.Font, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y + 1, this.ClientRectangle.Width, this.ClientRectangle.Height), Color.FromArgb(127, 127, 127));
                TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, Color.White);
            }
        }
    }
}
