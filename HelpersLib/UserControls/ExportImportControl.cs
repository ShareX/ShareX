#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib.UserControls
{
    public partial class ExportImportControl : UserControl
    {
        public delegate object ExportEventHandler();
        public event ExportEventHandler ExportRequested;

        public delegate void ImportEventHandler(object obj);
        public event ImportEventHandler ImportRequested;

        // Can't use generic class because not works in form designer
        public Type ObjectType { get; set; }

        public ExportImportControl()
        {
            InitializeComponent();
        }

        private void btnExport_MouseUp(object sender, MouseEventArgs e)
        {
            cmsExport.Show(btnExport, e.Location);
        }

        private void btnImport_MouseUp(object sender, MouseEventArgs e)
        {
            cmsImport.Show(btnImport, e.Location);
        }

        public string Export(object obj)
        {
            if (obj != null)
            {
                try
                {
                    StringBuilder sb = new StringBuilder(256);
                    StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture);

                    using (JsonTextWriter textWriter = new JsonTextWriter(stringWriter))
                    {
                        textWriter.Formatting = Formatting.Indented;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                        serializer.Converters.Add(new StringEnumConverter());
                        serializer.TypeNameHandling = TypeNameHandling.Auto;
                        serializer.Serialize(textWriter, obj, ObjectType);
                    }

                    return stringWriter.ToString();
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    MessageBox.Show("Export failed.\n\n" + e.ToString(), "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return null;
        }

        private void tsmiExportClipboard_Click(object sender, EventArgs e)
        {
            if (ExportRequested != null)
            {
                object obj = ExportRequested();

                string json = Export(obj);

                if (!string.IsNullOrEmpty(json) && ClipboardHelpers.CopyText(json))
                {
                    MessageBox.Show("Settings copied to your clipboard.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tsmiExportFile_Click(object sender, EventArgs e)
        {
            if (ExportRequested != null)
            {
                object obj = ExportRequested();

                string json = Export(obj);

                if (!string.IsNullOrEmpty(json))
                {
                    using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Settings(*.json)|*.json" })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(sfd.FileName, json, Encoding.UTF8);
                        }
                    }
                }
            }
        }

        public object Import(string json)
        {
            try
            {
                using (JsonTextReader textReader = new JsonTextReader(new StringReader(json)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.Error += (sender, e) => e.ErrorContext.Handled = true;
                    return serializer.Deserialize(textReader, ObjectType);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        private void tsmiImportClipboard_Click(object sender, EventArgs e)
        {
            if (ImportRequested != null)
            {
                if (Clipboard.ContainsText())
                {
                    string json = Clipboard.GetText();

                    if (!string.IsNullOrEmpty(json))
                    {
                        object obj = Import(json);

                        if (obj != null)
                        {
                            ImportRequested(obj);
                        }
                    }
                }
            }
        }

        private void tsmiImportFile_Click(object sender, EventArgs e)
        {
            if (ImportRequested != null)
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Settings(*.json)|*.json" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string json = File.ReadAllText(ofd.FileName, Encoding.UTF8);

                        if (!string.IsNullOrEmpty(json))
                        {
                            object obj = Import(json);

                            if (obj != null)
                            {
                                ImportRequested(obj);
                            }
                        }
                    }
                }
            }
        }
    }
}