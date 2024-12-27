#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using ShareX.HelpersLib.Extensions;
using ShareX.HelpersLib.NameParser;
using ShareX.UploadersLib.CustomUploader;
using ShareX.UploadersLib.Helpers;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib;

public partial class CustomUploaderSyntaxTestForm : Form
{
    private ResponseInfo testResponseInfo;

    public CustomUploaderSyntaxTestForm() : this(null, null)
    {
    }

    public CustomUploaderSyntaxTestForm(ResponseInfo responseInfo, string urlSyntax)
    {
        InitializeComponent();

        testResponseInfo = responseInfo;

        if (testResponseInfo == null)
        {
            testResponseInfo = new ResponseInfo()
            {
                ResponseText = "{\r\n    \"status\": 200,\r\n    \"data\": {\r\n        \"link\": \"https:\\/\\/example.com\\/image.png\"\r\n    }\r\n}",
                ResponseURL = "https://example.com/upload"
            };
        }

        if (string.IsNullOrEmpty(urlSyntax))
        {
            urlSyntax = "{json:data.link}";
        }

        rtbResponseText.Text = testResponseInfo.ResponseText;
        rtbURLSyntax.Text = urlSyntax;
        rtbURLSyntax.Select(rtbURLSyntax.TextLength, 0);

        CodeMenuItem[] outputCodeMenuItems = new CodeMenuItem[]
        {
            new("{response}", "Response text"),
            new("{responseurl}", "Response/Redirection URL"),
            new("{header:header_name}", "Response header"),
            new("{json:path}", "Parse JSON response using JSONPath"),
            new("{xml:path}", "Parse XML response using XPath"),
            new("{regex:pattern|group}", "Parse response using Regex"),
            new("{filename}", "File name used when uploading"),
            new("{random:input1|input2}", "Random selection from list"),
            new("{select:input1|input2}", "Lets user to select one input from list"),
            new("{prompt:title|default_value}", "Lets user to input text"),
            new("{base64:input}", "Base64 encode input")
        };

        new CodeMenu(rtbURLSyntax, outputCodeMenuItems)
        {
            MenuLocationOffset = new Point(5, -3)
        };

        rtbURLSyntax.AddContextMenu();

        ShareXResources.ApplyTheme(this, true);

        CustomUploaderSyntaxHighlight(rtbURLSyntax);
        UpdatePreview();
    }

    private void CustomUploaderSyntaxHighlight(RichTextBox rtb)
    {
        string text = rtb.Text;

        if (!string.IsNullOrEmpty(text))
        {
            int start = rtb.SelectionStart;
            int length = rtb.SelectionLength;
            rtb.BeginUpdate();

            rtb.SelectionStart = 0;
            rtb.SelectionLength = rtb.TextLength;
            rtb.SelectionColor = rtb.ForeColor;

            ShareXCustomUploaderSyntaxParser parser = new();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == parser.SyntaxStart || c == parser.SyntaxEnd || c == parser.SyntaxParameterStart ||
                    c == parser.SyntaxParameterDelimiter || c == parser.SyntaxEscape)
                {
                    rtb.SelectionStart = i;
                    rtb.SelectionLength = 1;
                    rtb.SelectionColor = Color.Lime;
                }
            }

            rtb.SelectionStart = start;
            rtb.SelectionLength = length;
            rtb.EndUpdate();
        }
    }

    private string ParseSyntax(ResponseInfo responseInfo, string urlSyntax)
    {
        if (responseInfo == null || string.IsNullOrEmpty(urlSyntax))
        {
            return null;
        }

        ShareXCustomUploaderSyntaxParser parser = new()
        {
            FileName = "example.png",
            ResponseInfo = responseInfo,
            URLEncode = true
        };

        return parser.Parse(urlSyntax);
    }

    private void UpdatePreview()
    {
        try
        {
            testResponseInfo.ResponseText = rtbResponseText.Text;
            string result = ParseSyntax(testResponseInfo, rtbURLSyntax.Text);
            txtResult.Text = result;
        } catch (Exception ex)
        {
            txtResult.Text = "Error\r\n" + ex.Message;
        }
    }

    private void txtResponseText_TextChanged(object sender, EventArgs e)
    {
        UpdatePreview();
    }

    private void rtbURLSyntax_TextChanged(object sender, EventArgs e)
    {
        CustomUploaderSyntaxHighlight(rtbURLSyntax);
        UpdatePreview();
    }
}