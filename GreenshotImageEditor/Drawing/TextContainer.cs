/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
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
        [NonSerialized]
        private bool fontInvalidated = true;
        // If makeUndoable is true the next text-change will make the change undoable.
        // This is set to true AFTER the first change is made, as there is already a "add element" on the undo stack
        private bool makeUndoable;
        [NonSerialized]
        private Font _font;
        public Font Font
        {
            get
            {
                return _font;
            }
        }

        [NonSerialized]
        private TextBox _textBox;

        /// <summary>
        /// The StringFormat object is not serializable!!
        /// </summary>
        [NonSerialized]
        private StringFormat _stringFormat = new StringFormat();
        public StringFormat StringFormat
        {
            get
            {
                return _stringFormat;
            }
        }
        private string text;
        // there is a binding on the following property!
        public string Text
        {
            get { return text; }
            set { ChangeText(value, true); }
        }

        internal void ChangeText(string newText, bool allowUndoable)
        {
            if ((text == null && newText != null) || !text.Equals(newText))
            {
                if (makeUndoable && allowUndoable)
                {
                    makeUndoable = false;
                    _parent.MakeUndoable(new TextChangeMemento(this), false);
                }
                text = newText;
                OnPropertyChanged("Text");
            }
        }

        public TextContainer(Surface parent)
            : base(parent)
        {
            Init();
        }

        protected override void InitializeFields()
        {
            AddField(GetType(), FieldType.LINE_THICKNESS, 2);
            AddField(GetType(), FieldType.LINE_COLOR, DefaultLineColor);
            AddField(GetType(), FieldType.SHADOW, false);
            AddField(GetType(), FieldType.FONT_ITALIC, false);
            AddField(GetType(), FieldType.FONT_BOLD, false);
            AddField(GetType(), FieldType.FILL_COLOR, Color.White);
            AddField(GetType(), FieldType.FONT_FAMILY, FontFamily.GenericSansSerif.Name);
            AddField(GetType(), FieldType.FONT_SIZE, 20f);
            AddField(GetType(), FieldType.TEXT_HORIZONTAL_ALIGNMENT, StringAlignment.Center);
            AddField(GetType(), FieldType.TEXT_VERTICAL_ALIGNMENT, VerticalAlignment.CENTER);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Init();
            UpdateFormat();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_font != null)
                {
                    _font.Dispose();
                    _font = null;
                }
                if (_stringFormat != null)
                {
                    _stringFormat.Dispose();
                    _stringFormat = null;
                }
                if (_textBox != null)
                {
                    _textBox.Dispose();
                    _textBox = null;
                }
            }
            base.Dispose(disposing);
        }

        private void Init()
        {
            _stringFormat = new StringFormat();
            _stringFormat.Trimming = StringTrimming.EllipsisWord;
            fontInvalidated = true;
            CreateTextBox();
            PropertyChanged += TextContainer_PropertyChanged;
            FieldChanged += TextContainer_FieldChanged;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            if (_textBox != null && _textBox.Visible)
            {
                _textBox.Invalidate();
            }
        }

        public void FitToText()
        {
            UpdateFormat();
            Size textSize = TextRenderer.MeasureText(text, _font);
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            Width = textSize.Width + lineThickness;
            Height = textSize.Height + lineThickness;
        }

        private void TextContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Selected"))
            {
                if (!Selected && _textBox.Visible)
                {
                    HideTextBox();
                }
                else if (Selected && Status == EditStatus.DRAWING)
                {
                    ShowTextBox();
                }
            }
            if (_textBox.Visible)
            {
                UpdateTextBoxPosition();
                UpdateTextBoxFormat();
                _textBox.Invalidate();
            }
        }

        private void TextContainer_FieldChanged(object sender, FieldChangedEventArgs e)
        {
            if (_textBox.Visible)
            {
                UpdateTextBoxFormat();
                _textBox.Invalidate();
            }
            else
            {
                UpdateFormat();
                //Invalidate();
            }
            _font.Dispose();
            _font = null;
            fontInvalidated = true;
        }

        public override void OnDoubleClick()
        {
            ShowTextBox();
        }

        private void CreateTextBox()
        {
            _textBox = new TextBox();

            _textBox.ImeMode = ImeMode.On;
            _textBox.Multiline = true;
            _textBox.AcceptsTab = true;
            _textBox.AcceptsReturn = true;
            _textBox.DataBindings.Add("Text", this, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
            _textBox.LostFocus += textBox_LostFocus;
            _textBox.KeyDown += textBox_KeyDown;
            _textBox.BorderStyle = BorderStyle.FixedSingle;
            _textBox.Visible = false;
        }

        private void ShowTextBox()
        {
            _parent.KeysLocked = true;
            _parent.Controls.Add(_textBox);
            EnsureTextBoxContrast();
            _textBox.Show();
            _textBox.Focus();
        }

        /// <summary>
        /// Makes textbox background dark if text color is very bright
        /// </summary>
        private void EnsureTextBoxContrast()
        {
            Color lc = GetFieldValueAsColor(FieldType.LINE_COLOR);
            if (lc.R > 203 && lc.G > 203 && lc.B > 203)
            {
                _textBox.BackColor = Color.FromArgb(51, 51, 51);
            }
            else
            {
                _textBox.BackColor = Color.White;
            }
        }

        private void HideTextBox()
        {
            _parent.Focus();
            _textBox.Hide();
            _parent.KeysLocked = false;
            _parent.Controls.Remove(_textBox);
        }

        /// <summary>
        /// Make sure the size of the font is scaled
        /// </summary>
        /// <param name="matrix"></param>
        public override void Transform(Matrix matrix)
        {
            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            int widthBefore = rect.Width;
            int heightBefore = rect.Height;

            // Transform this container
            base.Transform(matrix);
            rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);

            int widthAfter = rect.Width;
            int heightAfter = rect.Height;
            float factor = (((float)widthAfter / widthBefore) + ((float)heightAfter / heightBefore)) / 2;

            float fontSize = GetFieldValueAsFloat(FieldType.FONT_SIZE);
            fontSize *= factor;
            SetFieldValue(FieldType.FONT_SIZE, fontSize);

            fontInvalidated = true;
        }

        protected void UpdateFormat()
        {
            string fontFamily = GetFieldValueAsString(FieldType.FONT_FAMILY);
            bool fontBold = GetFieldValueAsBool(FieldType.FONT_BOLD);
            bool fontItalic = GetFieldValueAsBool(FieldType.FONT_ITALIC);
            float fontSize = GetFieldValueAsFloat(FieldType.FONT_SIZE);
            try
            {
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
                        _font = new Font(fam, fontSize, fs, GraphicsUnit.Pixel);
                    }
                    fontInvalidated = false;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("fontFamily", fontFamily);
                ex.Data.Add("fontBold", fontBold);
                ex.Data.Add("fontItalic", fontItalic);
                ex.Data.Add("fontSize", fontSize);
                throw;
            }

            _stringFormat.Alignment = (StringAlignment)GetFieldValue(FieldType.TEXT_HORIZONTAL_ALIGNMENT);
            _stringFormat.LineAlignment = (StringAlignment)GetFieldValue(FieldType.TEXT_VERTICAL_ALIGNMENT);
        }

        private void UpdateTextBoxPosition()
        {
            Rectangle absRectangle = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            _textBox.Left = absRectangle.Left;
            _textBox.Top = absRectangle.Top;
            _textBox.Width = absRectangle.Width;
            _textBox.Height = absRectangle.Height;
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
            _textBox.ForeColor = lineColor;
            _textBox.Font = _font;
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
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            bool drawShadow = shadow && (fillColor == Color.Transparent || fillColor == Color.Empty);

            DrawText(graphics, rect, lineThickness, lineColor, drawShadow, _stringFormat, text, _font);
        }

        /// <summary>
        /// This method can be used from other containers
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="drawingRectange"></param>
        /// <param name="lineThickness"></param>
        /// <param name="fontColor"></param>
        /// <param name="stringFormat"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public static void DrawText(Graphics graphics, Rectangle drawingRectange, int lineThickness, Color fontColor, bool drawShadow, StringFormat stringFormat, string text, Font font)
        {
            int textOffset = (lineThickness > 0) ? (int)Math.Ceiling(lineThickness / 2d) : 0;
            // draw shadow before anything else
            if (drawShadow)
            {
                int basealpha = 100;
                int alpha = basealpha;
                int steps = 5;
                int currentStep = 1;
                while (currentStep <= steps)
                {
                    int offset = currentStep;
                    Rectangle shadowRect = GuiRectangle.GetGuiRectangle(drawingRectange.Left + offset, drawingRectange.Top + offset, drawingRectange.Width, drawingRectange.Height);
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

            if (lineThickness > 0)
            {
                drawingRectange.Inflate(-textOffset, -textOffset);
            }
            using (Brush fontBrush = new SolidBrush(fontColor))
            {
                if (stringFormat != null)
                {
                    graphics.DrawString(text, font, fontBrush, drawingRectange, stringFormat);
                }
                else
                {
                    graphics.DrawString(text, font, fontBrush, drawingRectange);
                }
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