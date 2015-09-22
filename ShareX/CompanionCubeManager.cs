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
        public static int CubeCount { get; } = 100;

        private static List<CompanionCubeForm> cubes;
        private static Timer timer;
        private static Stopwatch startTime;
        private static TimeSpan previousElapsed;
        private static Rectangle screen;

        static CompanionCubeManager()
        {
            cubes = new List<CompanionCubeForm>();
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
        }

        public static void Toggle()
        {
            if (!IsActive)
            {
                IsActive = true;

                screen = CaptureHelpers.GetScreenWorkingArea();

                for (int i = 0; i < CubeCount; i++)
                {
                    CompanionCubeForm cube = new CompanionCubeForm(MathHelpers.Random(100, 500), MathHelpers.Random(50, 100));
                    cube.Location = new Point(MathHelpers.Random(screen.X, screen.X + screen.Width - cube.CubeSize),
                        MathHelpers.Random(screen.Y - cube.CubeSize - 500, screen.Y - cube.CubeSize));
                    cube.Show();
                    Console.WriteLine(cube.Location);
                    cubes.Add(cube);
                }

                startTime = Stopwatch.StartNew();
                timer.Start();
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = startTime.Elapsed - previousElapsed;
            previousElapsed = startTime.Elapsed;

            foreach (CompanionCubeForm cube in cubes)
            {
                int velocityY = (int)(cube.Gravity * elapsed.TotalSeconds);
                Point newLoc = new Point(cube.Location.X, cube.Location.Y + velocityY);
                Rectangle newRect = new Rectangle(newLoc, new Size(cube.CubeSize, cube.CubeSize));
                bool intersect = false;

                foreach (CompanionCubeForm cube2 in cubes)
                {
                    if (cube != cube2 && newRect.IntersectsWith(cube2.CubeRectangle))
                    {
                        intersect = true;
                        newLoc = new Point(cube.Location.X, cube2.Location.Y - cube.CubeSize);
                        break;
                    }
                }

                if (!intersect && newLoc.Y + cube.CubeSize > screen.Y + screen.Height)
                {
                    newLoc = new Point(cube.Location.X, screen.Height - cube.CubeSize);
                }

                if (cube.Location != newLoc)
                {
                    cube.Location = newLoc;
                }
            }
        }
    }
}