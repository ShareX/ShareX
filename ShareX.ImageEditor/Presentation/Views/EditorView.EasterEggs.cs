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

using Avalonia.Controls;
using Avalonia.Input;
using ShareX.ImageEditor.Presentation.EasterEggs;
using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views;

public partial class EditorView
{
    private EditorEasterEggController? _easterEggController;

    private void InitializeEasterEggs()
    {
        ShaderEasterEggOverlay? overlay = this.FindControl<ShaderEasterEggOverlay>("EasterEggOverlay");
        if (overlay != null)
        {
            // Replacing this effect is the only editor-adapter change required for a new animation.
            _easterEggController = new EditorEasterEggController(
                overlay,
                GetSnapshot,
                new InfiniteFoldEffect(),
                ShowEasterEggNotification);
        }
    }

    private void ShowEasterEggNotification()
    {
        if (DataContext is MainViewModel vm)
        {
            vm.ShowNotification(Strings.EditorView_AchievementUnlocked, EditorIcons.Achievement);
        }
    }

    private bool HandleEasterEggKeyDown(KeyEventArgs e)
    {
        if (_easterEggController?.HandleKeyDown(e.Key, e.KeyModifiers) != true)
        {
            return false;
        }

        e.Handled = true;
        return true;
    }

    private void StopEasterEggs()
    {
        _easterEggController?.Stop();
    }
}
