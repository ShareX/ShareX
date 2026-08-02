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
    public enum InsertImagePlacement
    {
        Center,
        CanvasExpandDown,
        CanvasExpandRight
    }

    public partial class InsertImageDialogViewModel : ObservableObject
    {
        public string Title => Strings.InsertImageDialogView_Title;
        public string Description => Strings.InsertImageDialogView_Description;
        public string ImageSummary { get; }

        public IRelayCommand InsertCenterCommand { get; }
        public IRelayCommand InsertBelowCommand { get; }
        public IRelayCommand InsertRightCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public InsertImageDialogViewModel(int imageWidth, int imageHeight, Action<InsertImagePlacement> onSelect, Action onCancel)
        {
            ImageSummary = string.Format(Strings.InsertImageDialogView_ImageSummary, imageWidth, imageHeight);

            InsertCenterCommand = new RelayCommand(() => onSelect(InsertImagePlacement.Center));
            InsertBelowCommand = new RelayCommand(() => onSelect(InsertImagePlacement.CanvasExpandDown));
            InsertRightCommand = new RelayCommand(() => onSelect(InsertImagePlacement.CanvasExpandRight));
            CancelCommand = new RelayCommand(onCancel);
        }
    }
}
