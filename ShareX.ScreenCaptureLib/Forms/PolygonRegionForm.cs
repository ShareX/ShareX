#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class PolygonRegionForm : BaseRegionForm
    {
        private List<NodeObject> nodes;
        private bool isAreaCreated;
        private Rectangle currentArea;

        public PolygonRegionForm()
        {
            nodes = new List<NodeObject>();

            MouseDown += PolygonRegionForm_MouseDown;
            MouseDoubleClick += PolygonRegionForm_MouseDoubleClick;
        }

        private void PolygonRegionForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (drawableObjects.Cast<NodeObject>().Any(node => node.IsCursorHover || node.IsDragging))
                {
                    return;
                }

                if (nodes.Count == 0)
                {
                    CreateNode();
                }

                CreateNode();

                isAreaCreated = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (isAreaCreated)
                {
                    foreach (NodeObject node in nodes)
                    {
                        if (node.IsCursorHover)
                        {
                            nodes.Remove(node);
                            drawableObjects.Remove(node);
                            return;
                        }
                    }

                    isAreaCreated = false;
                    nodes.Clear();
                    drawableObjects.Clear();
                }
                else
                {
                    Close(RegionResult.Close);
                }
            }
        }

        private void PolygonRegionForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Close(RegionResult.Region);
            }
        }

        protected override void Update()
        {
            base.Update();

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].Visible && nodes[i].IsDragging)
                {
                    ActivateNode(nodes[i]);
                    break;
                }
            }

            if (nodes.Count > 2)
            {
                RectangleF rect = regionFillPath.GetBounds();
                currentArea = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width + 1, (int)rect.Height + 1);
            }
        }

        protected override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            regionFillPath = new GraphicsPath();

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                regionFillPath.AddLine(nodes[i].Position, nodes[i + 1].Position);
            }

            if (nodes.Count > 2)
            {
                regionFillPath.CloseFigure();

                if (Config.UseDimming)
                {
                    using (Region region = new Region(regionFillPath))
                    {
                        g.Clip = region;
                        g.FillRectangle(lightBackgroundBrush, ScreenRectangle0Based);
                        g.ResetClip();
                    }
                }

                g.DrawRectangleProper(borderPen, currentArea);
            }

            if (nodes.Count > 1)
            {
                g.DrawPath(borderPen, regionFillPath);
                g.DrawPath(borderDotPen, regionFillPath);
            }

            base.Draw(g);
        }

        private void CreateNode()
        {
            NodeObject newNode = new NodeObject() { Shape = NodeShape.Diamond };
            ActivateNode(newNode);
            nodes.Add(newNode);
            drawableObjects.Add(newNode);
        }

        private void ActivateNode(NodeObject node)
        {
            node.Position = InputManager.MousePosition0Based;
            node.Visible = true;
            node.IsDragging = true;
        }
    }
}