#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : IDisposable
    {
        private bool _disposed;

        /// <summary>Releases images owned by this view model. The attached editor core is borrowed.</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            (ToolbarAdapter as IDisposable)?.Dispose();
            _editorCore = null;
            ModalContent = null;
            IsModalOpen = false;
            var resources = new HashSet<IDisposable>(ReferenceEqualityComparer.Instance);
            foreach (var resource in new IDisposable?[] { _previewImage, _backgroundBitmap, _currentSourceImage,
                _originalSourceImage, _preEffectImage, _latestEffectPreviewImage, _rotateCustomAngleOriginalBitmap })
            {
                if (resource != null) resources.Add(resource);
            }
            _previewImage = null;
            _backgroundBitmap = null;
            _currentSourceImage = null;
            _originalSourceImage = null;
            _preEffectImage = null;
            _latestEffectPreviewImage = null;
            _rotateCustomAngleOriginalBitmap = null;
            foreach (var resource in resources) resource.Dispose();
        }
    }
}
