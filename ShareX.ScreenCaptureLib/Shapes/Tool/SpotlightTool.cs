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

using ShareX.HelpersLib;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class SpotlightTool : BaseTool
    {
        public override ShapeType ShapeType { get; } = ShapeType.ToolSpotlight;

        public override bool LimitRectangleToInsideCanvas { get; } = true;
        public int Dim { get; set; }
        public int Blur { get; set; }
        public bool Ellipse { get; set; }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();
            Dim = AnnotationOptions.SpotlightDim;
            Blur = AnnotationOptions.SpotlightBlur;
            Ellipse = AnnotationOptions.SpotlightEllipse;
        }

        public override void OnConfigSave()
        {
            base.OnConfigSave();
            AnnotationOptions.SpotlightDim = Dim;
            AnnotationOptions.SpotlightBlur = Blur;
            AnnotationOptions.SpotlightEllipse = Ellipse;
        }

        public override void OnDraw(Graphics g)
        {
            if (IsValidShape)
            {
                Manager.DrawRegionArea(g, Rectangle, true, Manager.Options.ShowInfo, Ellipse);
                g.DrawCross(Pens.Black, Rectangle.Center().Add(-1, -1), 10);
                g.DrawCross(Pens.White, Rectangle.Center(), 10);
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();

            if (IsValidShape)
            {
                Manager.Form.Cursor = Cursors.WaitCursor;

                Manager.SpotlightArea(Rectangle, Dim, Blur, Ellipse);

                Manager.Form.SetDefaultCursor();
            }

            Remove();
        }

        public override void Remove()
        {
            base.Remove();

            if (Options.SwitchToSelectionToolAfterDrawing)
            {
                Manager.CurrentTool = ShapeType.ToolSelect;
            }
        }
    }
}