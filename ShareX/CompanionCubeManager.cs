#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public static class CompanionCubeManager
    {
        public static bool IsActive { get; set; }
        public static int CubeCount { get; } = 50;
        public static List<CompanionCube> Cubes { get; private set; }

        private static CompanionCubesForm cubesForm;
        private static Timer timer;
        private static Stopwatch startTime;
        private static TimeSpan previousElapsed;
        private static Rectangle screen;

        public static void Toggle()
        {
            if (!IsActive)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public static void Start()
        {
            if (!IsActive)
            {
                IsActive = true;
                screen = CaptureHelpers.GetScreenWorkingArea();

                if (cubesForm != null) cubesForm.Close();
                cubesForm = new CompanionCubesForm();
                cubesForm.MouseClick += CubesForm_MouseClick;
                cubesForm.Show();

                Cubes = new List<CompanionCube>(CubeCount);

                for (int i = 0; i < CubeCount; i++)
                {
                    CompanionCube cube = new CompanionCube(MathHelpers.Random(50, 100), MathHelpers.Random(200, 500));
                    cube.Location = new Point(MathHelpers.Random(screen.X, screen.X + screen.Width - cube.Size.Width),
                        MathHelpers.Random(screen.Y - cube.Size.Height - 500, screen.Y - cube.Size.Height));
                    Cubes.Add(cube);
                }

                previousElapsed = TimeSpan.Zero;
                startTime = Stopwatch.StartNew();

                timer = new Timer();
                timer.Interval = 10;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private static void CubesForm_MouseClick(object sender, MouseEventArgs e)
        {
            CompanionCube cube = Cubes.FirstOrDefault(x => x.Rectangle.Contains(e.Location));

            if (cube != null)
            {
                cube.IsActive = false;
            }
        }

        public static void Stop()
        {
            if (IsActive)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                if (cubesForm != null)
                {
                    cubesForm.Close();
                    cubesForm = null;
                }

                if (Cubes != null)
                {
                    Cubes.Clear();
                    Cubes = null;
                }

                IsActive = false;
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = startTime.Elapsed - previousElapsed;
            previousElapsed = startTime.Elapsed;

            foreach (CompanionCube cube in Cubes)
            {
                if (!cube.IsActive)
                {
                    continue;
                }

                int velocityY = (int)(cube.Speed * elapsed.TotalSeconds);
                Point newLocation = new Point(cube.Location.X, cube.Location.Y + velocityY);
                Rectangle newRectangle = new Rectangle(newLocation, cube.Size);
                bool intersect = false;

                foreach (CompanionCube cube2 in Cubes)
                {
                    if (cube != cube2 && cube2.IsActive && (newRectangle.IntersectsWith(cube2.Rectangle) || cube.Rectangle.IntersectsWith(cube2.Rectangle)))
                    {
                        intersect = true;
                        newLocation = new Point(cube.Location.X, cube2.Location.Y - cube.Size.Height);
                        break;
                    }
                }

                if (!intersect && newLocation.Y + cube.Size.Height > screen.Y + screen.Height)
                {
                    newLocation = new Point(cube.Location.X, screen.Height - cube.Size.Height);
                }

                cube.Location = newLocation;
            }

            DrawCubes();
        }

        private static void DrawCubes()
        {
            using (Bitmap companionCube = Resources.CompanionCube)
            using (Bitmap surface = new Bitmap(screen.Width, screen.Height))
            using (Graphics g = Graphics.FromImage(surface))
            {
                foreach (CompanionCube cube in Cubes)
                {
                    if (cube.IsActive)
                    {
                        g.DrawImage(companionCube, new Rectangle(CaptureHelpers.ScreenToClient(cube.Location), cube.Size));
                    }
                }

                cubesForm.SelectBitmap(surface);
            }
        }
    }
}