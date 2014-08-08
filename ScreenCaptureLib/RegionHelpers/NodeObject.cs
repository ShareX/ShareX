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

using HelpersLib;
using System.Drawing;

namespace ScreenCaptureLib
{
    internal class NodeObject : DrawableObject
    {
        private PointF position;

        public PointF Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Rectangle = new RectangleF(position.X - (NodeSize - 1) / 2, position.Y - (NodeSize - 1) / 2, NodeSize, NodeSize);
            }
        }

        public float NodeSize { get; set; }

        public NodeShape Shape { get; set; }

        public NodeObject(float x = 0, float y = 0)
        {
            NodeSize = 13;
            Shape = NodeShape.Square;
            Position = new PointF(x, y);
        }

        public override void Draw(Graphics g)
        {
            Rectangle rect = new Rectangle((int)Rectangle.X, (int)Rectangle.Y, (int)Rectangle.Width - 1, (int)Rectangle.Height - 1);

            switch (Shape)
            {
                case NodeShape.Square:
                    g.DrawRectangle(Pens.White, rect.RectangleOffset(-1));
                    g.DrawRectangle(Pens.Black, rect);
                    break;
                case NodeShape.Circle:
                    g.DrawEllipse(Pens.White, rect.RectangleOffset(-1));
                    g.DrawEllipse(Pens.Black, rect);
                    break;
            }
        }
    }
}