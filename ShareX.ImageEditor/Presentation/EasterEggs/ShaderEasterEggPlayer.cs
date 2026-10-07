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

using System.Diagnostics;

namespace ShareX.ImageEditor.Presentation.EasterEggs;

internal sealed class ShaderEasterEggPlayer : IDisposable
{
    private readonly ShaderEasterEggOverlay _overlay;
    private readonly IShaderEasterEggEffect _effectDefinition;
    private bool _isPlaying;

    public ShaderEasterEggPlayer(ShaderEasterEggOverlay overlay, IShaderEasterEggEffect effectDefinition)
    {
        _overlay = overlay;
        _effectDefinition = effectDefinition;
    }

    public bool IsPlaying => _isPlaying;

    public bool Play()
    {
        if (_isPlaying)
        {
            return true;
        }

        try
        {
            _overlay.IsVisible = true;
            if (!_overlay.Start(_effectDefinition))
            {
                Stop();
                return false;
            }

            _isPlaying = true;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to start image editor easter egg: {ex}");
            Stop();
            return false;
        }
    }

    public void Stop()
    {
        _isPlaying = false;
        _overlay.Stop();
        _overlay.IsVisible = false;
        _overlay.IsHitTestVisible = false;
    }

    public void Dispose()
    {
        Stop();
        _overlay.Dispose();
    }
}
