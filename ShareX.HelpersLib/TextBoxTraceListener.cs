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
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class TextBoxTraceListener : TraceListener
    {
        private TextBox textBox;

        public TextBoxTraceListener(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public override void Write(string message)
        {
            textBox.InvokeSafe(() =>
            {
                string text = string.Format("{0} - {1}", DateTime.Now.ToLongTimeString(), message);
                textBox.AppendText(text);
            });
        }

        public override void WriteLine(string message)
        {
            Write(message + "\r\n");
        }
    }
}