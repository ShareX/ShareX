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
using ShareX.ImageEditor.Localization;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class ConfirmationDialogViewModel : ObservableObject
    {
        public string Title { get; }
        public string Message { get; }

        public IRelayCommand YesCommand { get; }
        public IRelayCommand NoCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public ConfirmationDialogViewModel(Action onYes, Action onNo, Action onCancel,
            string? title = null,
            string? message = null)
        {
            Title = title ?? Strings.ConfirmationDialogView_Title;
            Message = message ?? Strings.ConfirmationDialogView_Message;
            YesCommand = new RelayCommand(onYes);
            NoCommand = new RelayCommand(onNo);
            CancelCommand = new RelayCommand(onCancel);
        }
    }
}
