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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    /// <summary>
    /// Asks for the text of a Text (Background) box, like the ShareX 21 text input dialog.
    /// </summary>
    public partial class TextInputDialogViewModel : ObservableObject
    {
        private readonly Action<string> _onOk;
        private readonly Action _onCancel;
        private bool _isClosed;

        [ObservableProperty]
        private string _text;

        public IRelayCommand OkCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public TextInputDialogViewModel(string text, Action<string> onOk, Action onCancel)
        {
            _text = text;
            _onOk = onOk;
            _onCancel = onCancel;
            OkCommand = new RelayCommand(Ok);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Ok()
        {
            if (_isClosed) return;
            _isClosed = true;
            _onOk(Text ?? string.Empty);
        }

        /// <summary>
        /// Also called when the dialog is closed another way, such as Escape or a click outside it.
        /// </summary>
        internal void Cancel()
        {
            if (_isClosed) return;
            _isClosed = true;
            _onCancel();
        }
    }
}
