// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)

using System.ComponentModel;
using System.Drawing.Design;

namespace ShareX.ImageListView;

/// <summary>
/// Displays a open file dialog box on the property grid.
/// </summary>
internal class OpenFileDialogEditor : UITypeEditor
{
    #region UITypeEditor Overrides
    /// <summary>
    /// Gets the edit style.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The edit style.</returns>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => context != null && context.Instance != null ? UITypeEditorEditStyle.Modal : UITypeEditorEditStyle.None;

    //[RefreshProperties(RefreshProperties.All)]
    /// <summary>
    /// Edits the value.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="value">The value.</param>
    /// <returns>New value.</returns>
    public override object EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider != null && context != null && context.Instance != null)
        {
            using OpenFileDialog dlg = new();
            string filename = value != null ? (string)value : "";

            dlg.FileName = filename;
            dlg.Title = "Select " + context?.PropertyDescriptor?.DisplayName;
            dlg.Filter = "All image files (*.bmp, *.gif, *.jpg, *.jepg, *.jpe, *.jif, *.png, *.tif, *.tiff, *.tga)|" +
                "*.bmp;*.gif;*.jpg;*.jepg;*.jpe;*.jif;*.png;*.tif;*.tiff;*.tga|" +
                "BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg, *.jepg, *.jpe, *.jif)|*.jpg;*.jepg;*.jpe;*.jif|" +
                "PNG (*.png)|*.png|TIFF (*.tif, *.tiff)|*.tif;*.tiff|TGA (*.tga)|*.tga|All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
                filename = dlg.FileName;

            return filename;
        }

        return EditValue(provider, value);
    }
    #endregion
}
