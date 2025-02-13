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

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace ShareX.HelpersLib
{
    internal class PrintTextHelper
    {
        private const int Eos = -1;
        private const int NewLine = -2;

        private string text = "";
        private Font font;
        private int offset;
        private int page;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        public void BeginPrint()
        {
            offset = 0;
            page = 1;
        }

        public void PrintPage(PrintPageEventArgs e)
        {
            float pagewidth = e.MarginBounds.Width * 3.0f;
            float pageheight = e.MarginBounds.Height * 3.0f;

            float textwidth = 0.0f;
            float textheight = 0.0f;

            float offsetx = e.MarginBounds.Left * 3.0f;
            float offsety = e.MarginBounds.Top * 3.0f;

            float x = offsetx;
            float y = offsety;

            StringBuilder line = new StringBuilder(256);
            StringFormat sf = StringFormat.GenericTypographic;
            sf.FormatFlags = StringFormatFlags.DisplayFormatControl;
            sf.SetTabStops(0.0f, new float[] { 300.0f });

            RectangleF r;

            Graphics g = e.Graphics;
            g.PageUnit = GraphicsUnit.Document;

            SizeF size = g.MeasureString("X", font, 1, sf);
            float lineheight = size.Height;

            // make sure we can print at least 1 line (font too big?)
            if (lineheight + (lineheight * 3) > pageheight)
            {
                // cannot print at least 1 line and footer
                g.Dispose();

                e.HasMorePages = false;

                return;
            }

            // don't include footer
            pageheight -= lineheight * 3;

            // last whitespace in line buffer
            int lastws = -1;

            // next character
            int c;

            while (true)
            {
                // get next character
                c = NextChar();

                // append c to line if not NewLine or Eos
                if ((c != NewLine) && (c != Eos))
                {
                    char ch = Convert.ToChar(c);
                    line.Append(ch);

                    // if ch is whitespace, remember pos and continue
                    if (ch == ' ' || ch == '\t')
                    {
                        lastws = line.Length - 1;
                        continue;
                    }
                }

                // measure string if line is not empty
                if (line.Length > 0)
                {
                    size = g.MeasureString(line.ToString(), font, int.MaxValue, StringFormat.GenericTypographic);
                    textwidth = size.Width;
                }

                // draw line if line is full, if NewLine or if last line
                if (c == Eos || (textwidth > pagewidth) || (c == NewLine))
                {
                    if (textwidth > pagewidth)
                    {
                        if (lastws != -1)
                        {
                            offset -= line.Length - lastws - 1;
                            line.Length = lastws + 1;
                        }
                        else
                        {
                            line.Length--;
                            offset--;
                        }
                    }

                    // there's something to draw
                    if (line.Length > 0)
                    {
                        r = new RectangleF(x, y, pagewidth, lineheight);
                        sf.Alignment = StringAlignment.Near;
                        g.DrawString(line.ToString(), font, Brushes.Black, r, sf);
                    }

                    // increase ypos
                    y += lineheight;
                    textheight += lineheight;

                    // empty line buffer
                    line.Length = 0;
                    textwidth = 0.0f;
                    lastws = -1;
                }

                // if next line doesn't fit on page anymore, exit loop
                if (textheight > (pageheight - lineheight) || c == Eos)
                {
                    break;
                }
            }

            // print footer
            x = offsetx;
            y = offsety + pageheight + (lineheight * 2);
            r = new RectangleF(x, y, pagewidth, lineheight);
            sf.Alignment = StringAlignment.Center;
            g.DrawString(page.ToString(), font, Brushes.Black, r, sf);

            g.Dispose();

            page++;

            e.HasMorePages = c != Eos;
        }

        private bool NextCharIsNewLine()
        {
            int nl = Environment.NewLine.Length;
            int tl = text.Length - offset;

            if (tl < nl) return false;

            string newline = Environment.NewLine;

            for (int i = 0; i < nl; i++)
            {
                if (text[offset + i] != newline[i])
                    return false;
            }

            return true;
        }

        private int NextChar()
        {
            if (offset >= text.Length)
                return -1;

            if (NextCharIsNewLine())
            {
                offset += Environment.NewLine.Length;
                return -2;
            }

            return text[offset++];
        }
    }
}