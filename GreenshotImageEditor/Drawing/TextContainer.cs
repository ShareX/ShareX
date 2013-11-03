/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.Drawing.Fields;
using Greenshot.Helpers;
using Greenshot.Memento;
using Greenshot.Plugin;
using Greenshot.Plugin.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Represents a textbox (extends RectangleContainer for border/background support
    /// </summary>
    [Serializable]
    public class TextContainer : RectangleContainer, ITextContainer
    {
        private bool fontInvalidated = true;
        // If makeUndoable is true the next text-change will make the change undoable.
        // This is set to true AFTER the first change is made, as there is already a "add element" on the undo stack
        private bool makeUndoable = false;
        private Font font;

        /// <summary>
        /// The StringFormat object is not serializable!!
        /// </summary>
        [NonSerialized]
        private StringFormat stringFormat;

        private string text;
        // there is a binding on the following property!
        public string Text
        {
            get { return text; }
            set
            {
                ChangeText(value, true);
            }
        }

        internal void ChangeText(string newText, bool allowUndoable)
        {
            if ((text == null && newText != null) || !text.Equals(newText))
            {
                if (makeUndoable && allowUndoable)
                {
                    makeUndoable = false;
                    parent.MakeUndoable(new TextChangeMemento(this), false);
                }
                text = newText;
                OnPropertyChanged("Text");
            }
        }

        [NonSerialized]
        private TextBox textBox;

        public TextContainer(Surface parent)
            : base(parent)
        {
            Init();
            AddField(GetType(), FieldType.LINE_THICKNESS, 2);
            AddField(GetType(), FieldType.LINE_COLOR, Color.Red);
            AddField(GetType(), FieldType.SHADOW, true);
            AddField(GetType(), FieldType.FONT_ITALIC, false);
            AddField(GetType(), FieldType.FONT_BOLD, false);
            AddField(GetType(), FieldType.FILL_COLOR, Color.Transparent);
            AddField(GetType(), FieldType.FONT_FAMILY, FontFamily.GenericSansSerif.Name);
            AddField(GetType(), FieldType.FONT_SIZE, 11f);
            AddField(GetType(), FieldType.TEXT_HORIZONTAL_ALIGNMENT, HorizontalAlignment.Center);
            AddField(GetType(), FieldType.TEXT_VERTICAL_ALIGNMENT, VerticalAlignment.CENTER);
            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.EllipsisWord;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            stringFormat = new StringFormat();
            Init();
            UpdateFormat();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (font != null)
                {
                    font.Dispose();
                    font = null;
                }
                if (stringFormat != null)
                {
                    stringFormat.Dispose();
                    stringFormat = null;
                }
                if (textBox != null)
                {
                    textBox.Dispose();
                    textBox = null;
                }
            }
            base.Dispose(disposing);
        }

        private void Init()
        {
            CreateTextBox();
            PropertyChanged += TextContainer_PropertyChanged;
            FieldChanged += TextContainer_FieldChanged;
        }

        public void FitToText()
        {
            UpdateFormat();
            Size textSize = TextRenderer.MeasureText(text, font);
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            Width = textSize.Width + lineThickness;
            Height = textSize.Height + lineThickness;
        }

        private void TextContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Selected"))
            {
                if (!Selected && textBox.Visible)
                {
                    HideTextBox();
                }
                else if (Selected && Status == EditStatus.DRAWING)
                {
                    ShowTextBox();
                }
            }
            if (textBox.Visible)
            {
                UpdateTextBoxPosition();
                UpdateTextBoxFormat();
                textBox.Invalidate();
            }
        }

        private void TextContainer_FieldChanged(object sender, FieldChangedEventArgs e)
        {
            if (textBox.Visible)
            {
                UpdateTextBoxFormat();
                textBox.Invalidate();
            }
            else
            {
                UpdateFormat();
                //Invalidate();
            }
            font.Dispose();
            font = null;
            fontInvalidated = true;
        }

        public override void OnDoubleClick()
        {
            ShowTextBox();
            textBox.Focus();
        }

        private void CreateTextBox()
        {
            textBox = new TextBox();
            textBox.ImeMode = ImeMode.On;
            textBox.Multiline = true;
            textBox.AcceptsTab = true;
            textBox.AcceptsReturn = true;
            textBox.DataBindings.Add("Text", this, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox.LostFocus += textBox_LostFocus;
            textBox.KeyDown += textBox_KeyDown;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Visible = false;
        }

        private void ShowTextBox()
        {
            parent.KeysLocked = true;
            parent.Controls.Add(textBox);
            textBox.Show();
            textBox.Focus();
        }

        private void HideTextBox()
        {
            parent.Focus();
            textBox.Hide();
            parent.KeysLocked = false;
            parent.Controls.Remove(textBox);
        }

        private void UpdateFormat()
        {
            string fontFamily = GetFieldValueAsString(FieldType.FONT_FAMILY);
            bool fontBold = GetFieldValueAsBool(FieldType.FONT_BOLD);
            bool fontItalic = GetFieldValueAsBool(FieldType.FONT_ITALIC);
            float fontSize = GetFieldValueAsFloat(FieldType.FONT_SIZE);

            if (fontInvalidated && fontFamily != null && fontSize != 0)
            {
                FontStyle fs = FontStyle.Regular;

                bool hasStyle = false;
                using (FontFamily fam = new FontFamily(fontFamily))
                {
                    bool boldAvailable = fam.IsStyleAvailable(FontStyle.Bold);
                    if (fontBold && boldAvailable)
                    {
                        fs |= FontStyle.Bold;
                        hasStyle = true;
                    }

                    bool italicAvailable = fam.IsStyleAvailable(FontStyle.Italic);
                    if (fontItalic && italicAvailable)
                    {
                        fs |= FontStyle.Italic;
                        hasStyle = true;
                    }

                    if (!hasStyle)
                    {
                        bool regularAvailable = fam.IsStyleAvailable(FontStyle.Regular);
                        if (regularAvailable)
                        {
                            fs = FontStyle.Regular;
                        }
                        else
                        {
                            if (boldAvailable)
                            {
                                fs = FontStyle.Bold;
                            }
                            else if (italicAvailable)
                            {
                                fs = FontStyle.Italic;
                            }
                        }
                    }
                    font = new Font(fam, fontSize, fs, GraphicsUnit.Pixel);
                }
                fontInvalidated = false;
            }

            stringFormat.Alignment = (StringAlignment)GetFieldValue(FieldType.TEXT_HORIZONTAL_ALIGNMENT);
            stringFormat.LineAlignment = (StringAlignment)GetFieldValue(FieldType.TEXT_VERTICAL_ALIGNMENT);
        }

        private void UpdateTextBoxPosition()
        {
            textBox.Left = Left;
            textBox.Top = Top;
            textBox.Width = Width;
            textBox.Height = Height;
        }

        public override void ApplyBounds(RectangleF newBounds)
        {
            base.ApplyBounds(newBounds);
            UpdateTextBoxPosition();
        }

        private void UpdateTextBoxFormat()
        {
            UpdateFormat();
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            textBox.ForeColor = lineColor;
            textBox.Font = font;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            // ESC and Enter/Return (w/o Shift) hide text editor
            if (e.KeyCode == Keys.Escape || ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && e.Modifiers == Keys.None))
            {
                HideTextBox();
                e.SuppressKeyPress = true;
            }
        }

        private void textBox_LostFocus(object sender, EventArgs e)
        {
            // next change will be made undoable
            makeUndoable = true;
            HideTextBox();
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            base.Draw(graphics, rm);
            UpdateFormat();
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.None;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;

            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            if (Selected && rm == RenderMode.EDIT)
            {
                DrawSelectionBorder(graphics, rect);
            }

            if (text == null || text.Length == 0)
            {
                return;
            }

            // we only draw the shadow if there is no background
            bool shadow = GetFieldValueAsBool(FieldType.SHADOW);
            Color fillColor = GetFieldValueAsColor(FieldType.FILL_COLOR);
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            int textOffset = (lineThickness > 0) ? (int)Math.Ceiling(lineThickness / 2d) : 0;
            // draw shadow before anything else
            if (shadow && (fillColor == Color.Transparent || fillColor == Color.Empty))
            {
                int basealpha = 100;
                int alpha = basealpha;
                int steps = 5;
                int currentStep = 1;
                while (currentStep <= steps)
                {
                    int offset = currentStep;
                    Rectangle shadowRect = GuiRectangle.GetGuiRectangle(Left + offset, Top + offset, Width, Height);
                    if (lineThickness > 0)
                    {
                        shadowRect.Inflate(-textOffset, -textOffset);
                    }
                    using (Brush fontBrush = new SolidBrush(Color.FromArgb(alpha, 100, 100, 100)))
                    {
                        graphics.DrawString(text, font, fontBrush, shadowRect, stringFormat);
                        currentStep++;
                        alpha = alpha - basealpha / steps;
                    }
                }
            }
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            Rectangle fontRect = rect;
            if (lineThickness > 0)
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                fontRect.Inflate(-textOffset, -textOffset);
            }
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (Brush fontBrush = new SolidBrush(lineColor))
            {
                graphics.DrawString(text, font, fontBrush, fontRect, stringFormat);
            }
        }

        public override bool ClickableAt(int x, int y)
        {
            Rectangle r = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            r.Inflate(5, 5);
            return r.Contains(x, y);
        }
    }
}