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

using Avalonia;
using Avalonia.Media;
using static ShareX.Icons.IconGeometry;
using static ShareX.Icons.IconShape;
using Icon = ShareX.Icons.ImageEffectIcons;

namespace ShareX.Icons;

/// <summary>The shapes of the <see cref="ImageEffectIcons"/>.</summary>
/// <remarks>
/// Drawn like the <see cref="AppIcons"/>: a 24x24 grid, 2px strokes with round caps and joins, content within
/// 2..22. Shading is the Foreground at partial opacity, used where the effect is about tone or light.
/// </remarks>
internal static class ImageEffectIconDefinitions
{
    private const double Shade = 0.35;

    private const string SquareFrame = "M3 5.5A2.5 2.5 0 0 1 5.5 3h13A2.5 2.5 0 0 1 21 5.5v13a2.5 2.5 0 0 1-2.5 2.5h-13A2.5 2.5 0 0 1 3 18.5z";
    private const string Wave = "q2.5-2.5 5 0t5 0 5 0 5 0";
    private const string DropletPath = "M12 2.5s-6.5 7-6.5 11.75a6.5 6.5 0 0 0 13 0C18.5 9.5 12 2.5 12 2.5z";
    private const string CubePath = "M12 2.5 3 7v10l9 4.5 9-4.5V7z";
    private const string CubeEdges = "M3 7l9 4.5L21 7M12 11.5v10";
    private const string ThermometerTube = "M10 4a2 2 0 0 1 4 0v10.5a4 4 0 1 1-4 0z";

    public static IEnumerable<IconGroup> CreateGroups() => [CreateIcons()];

    private static IconGroup CreateIcons() => new(24, IconStyle.Rounded)
    {
        // Light and lenses.
        {
            Icon.Aperture,
            [Stroke(Circle(12, 12, 10)), .. ApertureBlades()]
        },
        {
            Icon.Sparkle,
            Stroke(Sparkle(12, 12, 9.5))
        },
        // A soft glow around a bright core.
        {
            Icon.SoftGlow,
            Fill(Circle(12, 12, 10), 0.2),
            Fill(Circle(12, 12, 7), Shade),
            Stroke(Circle(12, 12, 4))
        },
        {
            Icon.Sunrise,
            Stroke("M3 18h18M6.5 18a5.5 5.5 0 0 1 11 0"),
            Stroke("M12 8.5V2.5M9.5 5 12 2.5 14.5 5"),
            Stroke("M4 12l1.5 1.5M20 12l-1.5 1.5M8 21.5h8")
        },
        {
            Icon.Sunset,
            Stroke("M3 18h18M6.5 18a5.5 5.5 0 0 1 11 0"),
            Stroke("M12 2.5V8.5M9.5 6 12 8.5 14.5 6"),
            Stroke("M4 12l1.5 1.5M20 12l-1.5 1.5M8 21.5h8")
        },
        {
            Icon.Eclipse,
            Fill(Subtract(Circle(12, 12, 10), Circle(16.5, 9, 8)), Shade),
            Stroke(Circle(12, 12, 10))
        },
        {
            Icon.Flashlight,
            Stroke("M7 2.5h10v3.5l-2.5 4V21a1 1 0 0 1-1 1h-3a1 1 0 0 1-1-1V10L7 6z"),
            Stroke("M7 6h10"),
            Fill(Circle(12, 14, 1.25))
        },
        {
            Icon.Sphere,
            Stroke(Circle(12, 12, 10)),
            Stroke("M6.5 10.5A5.75 5.75 0 0 1 10.5 6.5"),
            Fill(Ellipse(4, 15, 16, 5), 0.2)
        },
        {
            Icon.Glasses,
            Fill(Circle(6.5, 14, 2.5), Shade),
            Stroke(Circle(6.5, 14, 4)),
            Stroke(Circle(17.5, 14, 4)),
            Stroke("M10.5 14a1.5 1.5 0 0 1 3 0M2.5 14 4.5 7M21.5 14l-2-7")
        },

        // Color and tone.
        {
            Icon.Rainbow,
            Stroke("M2 19a10 10 0 0 1 20 0M5.5 19a6.5 6.5 0 0 1 13 0M9 19a3 3 0 0 1 6 0")
        },
        {
            Icon.Droplet,
            Stroke(DropletPath)
        },
        {
            Icon.Droplets,
            Stroke("M8.5 6.5S4 11.5 4 14.75a4.5 4.5 0 0 0 9 0C13 11.5 8.5 6.5 8.5 6.5z"),
            Stroke("M17 2.5s-3 3.25-3 5.5a3 3 0 0 0 6 0c0-2.25-3-5.5-3-5.5z")
        },
        {
            Icon.Invert,
            Fill("M21 7v11.5a2.5 2.5 0 0 1-2.5 2.5H7z"),
            Stroke(SquareFrame)
        },
        // A color swapped for another.
        {
            Icon.Replace,
            Fill(Rectangle(2, 2, 9, 9, 2), Shade),
            Stroke(Rectangle(2, 2, 9, 9, 2)),
            Stroke(Rectangle(13, 13, 9, 9, 2)),
            Stroke("M14 5.5h2.5A2.5 2.5 0 0 1 19 8v2M17 8l2 2 2-2M10 18.5H7.5A2.5 2.5 0 0 1 5 16v-2M3 16l2-2 2 2")
        },
        // Red, green and blue overlapping.
        {
            Icon.RgbSplit,
            Stroke(Circle(9, 9, 5.5)),
            Stroke(Circle(15, 9, 5.5)),
            Stroke(Circle(12, 14.5, 5.5))
        },
        {
            Icon.Flame,
            Stroke("M12 2.5c1 3.5 5.5 5.5 5.5 11.25a5.5 5.5 0 0 1-11 0c0-2.5 1.5-4 2.5-5 .3 1.6 1.2 2.5 2.25 2.5C10.5 8.5 11 5.5 12 2.5z")
        },
        {
            Icon.Thermometer,
            Stroke(ThermometerTube),
            Stroke("M12 9v8.5"),
            Fill(Circle(12, 17.5, 1.75))
        },
        {
            Icon.ThermometerSun,
            Stroke("M14 4a2 2 0 0 1 4 0v10.5a4 4 0 1 1-4 0z"),
            Fill(Circle(16, 17.5, 1.5)),
            Stroke(Circle(6.5, 8.5, 2.5)),
            Stroke("M6.5 2.5v1.5M6.5 13v1.5M1.5 8.5H3M10 8.5h1.5")
        },
        {
            Icon.Coffee,
            Stroke("M4 9h12v6a5 5 0 0 1-5 5H9a5 5 0 0 1-5-5z"),
            Stroke("M16 10.5h1.5a2.5 2.5 0 0 1 0 5H16"),
            Stroke("M8 2.5c-1 1.5 1 2.5 0 4.5M12 2.5c-1 1.5 1 2.5 0 4.5")
        },
        {
            Icon.Gauge,
            Stroke("M4.2 18.5A9 9 0 1 1 19.8 18.5"),
            Stroke("m12 14 4-5"),
            Fill(Circle(12, 14, 1.75))
        },
        {
            Icon.SlidersVertical,
            Stroke("M6 3v10M6 17v4M12 3v4M12 11v10M18 3v12M18 19v2"),
            Stroke(Circle(6, 15, 2)),
            Stroke(Circle(12, 9, 2)),
            Stroke(Circle(18, 17, 2))
        },
        {
            Icon.Binary,
            Stroke(Rectangle(4, 2.5, 5, 8, 2.5)),
            Stroke("M15 3.5l2-1v8M15 10.5h4"),
            Stroke("M5 14.5l2-1v8M5 21.5h4"),
            Stroke(Rectangle(15, 13.5, 5, 8, 2.5))
        },
        {
            Icon.Sigma,
            Stroke("M18 7V4H6l6 8-6 8h12v-3")
        },
        {
            Icon.SortDescending,
            Stroke("M3 5h18M3 10h14M3 15h10M3 20h6")
        },

        // Texture and pattern.
        {
            Icon.Waves,
            Stroke("M2 6.5" + Wave + "M2 12" + Wave + "M2 17.5" + Wave)
        },
        {
            Icon.Hexagon,
            Stroke("M12 2.5 20.25 7.25v9.5L12 21.5l-8.25-4.75v-9.5z")
        },
        {
            Icon.Triangle,
            Stroke("M10.27 4.45 2.9 17.5a2 2 0 0 0 1.73 3h14.74a2 2 0 0 0 1.73-3L13.73 4.45a2 2 0 0 0-3.46 0z")
        },
        {
            Icon.Diamond,
            Stroke("M10.6 3.4a2 2 0 0 1 2.8 0l7.2 7.2a2 2 0 0 1 0 2.8l-7.2 7.2a2 2 0 0 1-2.8 0l-7.2-7.2a2 2 0 0 1 0-2.8z")
        },
        {
            Icon.Gem,
            Stroke("M6 3h12l4 6-10 12.5L2 9z"),
            Stroke("M2 9h20M12 21.5 8.5 9 11 3M12 21.5 15.5 9 13 3")
        },
        {
            Icon.CircleDot,
            Stroke(Circle(12, 12, 10)),
            Fill(Circle(12, 12, 3))
        },
        // Two of the four squares filled.
        {
            Icon.Checkerboard,
            Fill(Subtract(Path(SquareFrame), Union(Rectangle(12, 0, 12, 12), Rectangle(0, 12, 12, 12))), 0.6),
            Stroke(SquareFrame)
        },
        {
            Icon.Blinds,
            Stroke("M3 3.5h18M5 8h14M5 12.5h14M5 17h14"),
            Stroke("M4 3.5v14M20 3.5v14")
        },
        {
            Icon.SprayCan,
            Stroke(Rectangle(4, 10, 10, 12, 2.5)),
            Stroke(Rectangle(6.5, 6, 5, 4, 1)),
            Fill(Circle(16.5, 4, 1)),
            Fill(Circle(20, 3, 1)),
            Fill(Circle(19, 7, 1)),
            Fill(Circle(16.5, 9.5, 1)),
            Fill(Circle(21, 10.5, 1))
        },
        {
            Icon.Fog,
            Stroke("M3 7h5M11 7h10M3 12h12M18 12h3M3 17h7M13 17h8")
        },
        {
            Icon.Wind,
            Stroke("M2.5 8.5h11a3 3 0 1 0-3-3M2.5 15.5h14a3 3 0 1 1-3 3M2.5 12h7")
        },
        // Lines of equal brightness.
        {
            Icon.Contours,
            Stroke("M12 3c5 0 9 3.5 9 8.5S17 21 11.5 21 3 17 3 11.5 7 3 12 3z"),
            Stroke("M12 8c2.75 0 4.5 1.75 4.5 4S14.5 16.5 12 16.5 7.5 14.75 7.5 12 9.25 8 12 8z"),
            Fill(Circle(12, 12.25, 1.25))
        },
        {
            Icon.Grass,
            Stroke("M2 21h20"),
            Stroke("M5 21c0-4 1-8 3.5-10.5M9 21c0-5.5 2-10 5.5-13M14 21c0-4 1.5-7 4.5-9M19 21c0-2.5.75-4.5 2-5.5")
        },
        {
            Icon.CircuitBoard,
            Stroke(SquareFrame),
            Stroke("M8 3v5.5l3.5 3.5M16 21v-5.5L12.5 12M3 15.5h4.5M21 8.5h-4.5"),
            Fill(Circle(12, 12, 1.75))
        },

        // Borders and frames.
        {
            Icon.Frame,
            Stroke(SquareFrame),
            Stroke(Rectangle(7, 7, 10, 10, 1))
        },
        {
            Icon.BrickWall,
            Stroke(SquareFrame),
            Stroke("M3 9h18M3 15h18M9 3v6M15 9v6M9 15v6")
        },
        {
            Icon.Fence,
            Stroke(SquareFrame),
            Stroke("M3 12 12 3M3 21 21 3M12 21l9-9M12 3l9 9M3 3l18 18M3 12l9 9")
        },
        {
            Icon.SquareDashed,
            Stroke(Rectangle(3, 3, 18, 18, 3), dashes: [2, 3.3])
        },
        // One corner rounded, the rest of the square dashed.
        {
            Icon.RoundedCorner,
            Stroke("M3 21V11a8 8 0 0 1 8-8h10"),
            Stroke("M21 3v18H3", dashes: [2, 3.3])
        },
        {
            Icon.TornEdge,
            Stroke("M4 3h16v13l-2.67 2.5-2.66-2.5L12 18.5 9.33 16l-2.66 2.5L4 16z"),
            Stroke("M8 8h8M8 11.5h5")
        },
        {
            Icon.Crown,
            Stroke("M3 7.5l4.5 4.5L12 4.5l4.5 7.5L21 7.5l-2 11H5z"),
            Stroke("M5 21.5h14")
        },
        {
            Icon.Scroll,
            Stroke("M19 17V5a2 2 0 0 0-2-2H5.5"),
            Stroke("M8 21h11a2 2 0 0 0 2-2v-2H10v2a2 2 0 1 1-4 0V5a2 2 0 1 0-4 0v3h4")
        },
        {
            Icon.CandyCane,
            Stroke(Halo("M8.5 21V9.5a4 4 0 0 1 8 0v1", 5)),
            Stroke("M6 17.5l5-1.75M6 13l5-1.75M12.25 5.75l3.25 4.5")
        },
        {
            Icon.Clover,
            Stroke(Union(Circle(12, 7, 4.25), Circle(7, 13.5, 4.25), Circle(17, 13.5, 4.25))),
            Stroke("M12 15v6.5")
        },
        {
            Icon.Sword,
            Stroke("M14.5 17.5 3 6V3h3l11.5 11.5"),
            Stroke("M13 19l6-6M16 16l4.5 4.5M19.5 21.5l2-2")
        },
        {
            Icon.Newspaper,
            Stroke("M5 21h14a2 2 0 0 0 2-2V5a2 2 0 0 0-2-2H9a2 2 0 0 0-2 2v14a2 2 0 0 1-4 0v-9h4"),
            Stroke("M11 7.5h6M11 11.5h6M11 15.5h4")
        },

        // Shape and geometry.
        {
            Icon.FlipHorizontal,
            Stroke("M12 2.5v19", dashes: [2, 3.3]),
            Stroke("M9 5.5v13L2.5 12z"),
            Fill("M15 5.5v13l6.5-6.5z", Shade),
            Stroke("M15 5.5v13l6.5-6.5z")
        },
        {
            Icon.FlipVertical,
            Stroke("M2.5 12h19", dashes: [2, 3.3]),
            Stroke("M5.5 9h13L12 2.5z"),
            Fill("M5.5 15h13L12 21.5z", Shade),
            Stroke("M5.5 15h13L12 21.5z")
        },
        {
            Icon.RotateClockwise,
            Stroke("M20.5 12A8.5 8.5 0 1 1 18 6l2.5 2.5M20.5 4v4.5H16")
        },
        {
            Icon.Rotate3d,
            Stroke("M12 6.5 7.5 9v5.5L12 17l4.5-2.5V9z"),
            Stroke("M7.5 9 12 11.5 16.5 9M12 11.5V17"),
            Stroke("M4.5 9.5C2.25 10.6 2 12.4 3 13.6c1.9 2.3 8 3.3 13.5 2.4M14.5 13.5l2.5 2.5-2.5 2.5")
        },
        {
            Icon.MoveDiagonal,
            Stroke("M5 19 19 5M5 13v6h6M13 5h6v6")
        },
        {
            Icon.Cube,
            Stroke(CubePath),
            Stroke(CubeEdges)
        },
        {
            Icon.Cylinder,
            Stroke(Ellipse(4, 2.5, 16, 5)),
            Stroke("M4 5v14c0 1.4 3.6 2.5 8 2.5s8-1.1 8-2.5V5")
        },
        {
            Icon.Orbit,
            Stroke(Circle(12, 12, 3)),
            Stroke(Rotate(Ellipse(1.5, 7.5, 21, 9), -30, 12, 12)),
            Fill(Circle(20, 7.5, 1.75))
        },
        {
            Icon.Pinwheel,
            [.. Enumerable.Range(0, 4).Select(i => Stroke(Rotate(Path("M12 12c0-4.5 2.5-8.5 7.5-8.5.5 5-3 8.5-7.5 8.5z"), i * 90, 12, 12)))]
        },
        // A page with its corner curled over.
        {
            Icon.PageCurl,
            Stroke("M3 5.5A2.5 2.5 0 0 1 5.5 3h13A2.5 2.5 0 0 1 21 5.5V14l-7 7H5.5A2.5 2.5 0 0 1 3 18.5z"),
            Fill("M21 14h-4.5a2.5 2.5 0 0 0-2.5 2.5V21z", Shade),
            Stroke("M21 14h-4.5a2.5 2.5 0 0 0-2.5 2.5V21")
        },
        // A picture cut in two, its halves shifted apart.
        {
            Icon.Slice,
            Stroke("M3 10.5v-5A2.5 2.5 0 0 1 5.5 3H16a2.5 2.5 0 0 1 2.5 2.5v5z"),
            Stroke("M5.5 13.5v5A2.5 2.5 0 0 0 8 21h10.5a2.5 2.5 0 0 0 2.5-2.5v-5z")
        },
        {
            Icon.Shapes,
            Stroke("M8.5 2.5 3.5 11h10z"),
            Stroke(Rectangle(13, 13, 8.5, 8.5, 1.5)),
            Stroke(Circle(7.5, 17.5, 4))
        },
        {
            Icon.Compass,
            Stroke(Circle(12, 4.5, 2)),
            Stroke("M11 6.5 5 21M13 6.5 19 21M6.5 17a10 10 0 0 0 11 0")
        },
        {
            Icon.Map,
            Stroke("M9 4 3 6.5V20l6-2.5 6 2.5 6-2.5V4l-6 2.5z"),
            Stroke("M9 4v13.5M15 6.5V20")
        },

        // Tools and materials.
        {
            Icon.PaintBrush,
            Stroke("M21.5 2.5 12.5 11.5M10.75 10.25l3 3"),
            Stroke("M9.5 13c-2.75 0-4.5 1.75-4.5 4.25 0 1.6-1 2.6-2.5 3.25 1 .75 2.5 1 3.75 1 3.25 0 5.25-2.25 5.25-5.25z")
        },
        {
            Icon.ChalkBrush,
            Stroke(Rotate(Rectangle(10, 1.5, 5, 14, 1.5), 45, 12.5, 8.5)),
            Stroke("M3 21c2-1.5 3.5 0 5.5-1s3-2 5-1 3 .5 4.5-.5")
        },
        {
            Icon.PaintRoller,
            Stroke(Rectangle(3, 3, 15, 6, 2)),
            Stroke("M18 6h2a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1h-7.5a1 1 0 0 0-1 1v3"),
            Stroke(Rectangle(9.5, 15, 3, 7, 1))
        },
        {
            Icon.PaintBucket,
            Stroke("M4.9 11.1 11 5l7.5 7.5-6.1 6.1a2 2 0 0 1-2.8 0l-4.7-4.7a2 2 0 0 1 0-2.8z"),
            Stroke("M4.5 12.5h14M9 7 6.5 4.5"),
            Stroke("M21 15.5s1.5 2 1.5 3a1.5 1.5 0 0 1-3 0c0-1 1.5-3 1.5-3z")
        },
        {
            Icon.Axe,
            Stroke("M20 6.5 3.5 21"),
            Fill("M14.5 11.5 8 10.5Q8.5 4 15 2.5L18.5 8z", Shade),
            Stroke("M14.5 11.5 8 10.5Q8.5 4 15 2.5L18.5 8z")
        },
        {
            Icon.Glass,
            Stroke("M5 3h14l-1.75 16.25A2 2 0 0 1 15.26 21H8.74a2 2 0 0 1-1.99-1.75z"),
            Fill("M6.1 11h11.8l-.95 8.1a1 1 0 0 1-1 .9H8.05a1 1 0 0 1-1-.9z", Shade),
            Stroke("M6 11q1.5-1.5 3 0t3 0 3 0 3 0")
        },
        {
            Icon.Cpu,
            Stroke(Rectangle(5, 5, 14, 14, 2)),
            Stroke(Rectangle(9, 9, 6, 6, 1)),
            Stroke("M9 2v3M15 2v3M9 19v3M15 19v3M2 9h3M2 15h3M19 9h3M19 15h3")
        },
        {
            Icon.Tv,
            Stroke(Rectangle(2, 7, 20, 14, 2.5)),
            Stroke("m7.5 2.5 4.5 4.5 4.5-4.5"),
            Fill(Circle(7, 12, 1)),
            Fill(Circle(11, 15, 1)),
            Fill(Circle(16.5, 11.5, 1)),
            Fill(Circle(14.5, 17, 1))
        },
        {
            Icon.Videotape,
            Stroke(Rectangle(2, 4, 20, 16, 2.5)),
            Stroke(Circle(8, 12, 2.5)),
            Stroke(Circle(16, 12, 2.5)),
            Stroke("M8 14.5h8")
        },
        {
            Icon.TextCursor,
            Stroke("M9 4h1.5A1.5 1.5 0 0 1 12 5.5v13a1.5 1.5 0 0 1-1.5 1.5H9M15 4h-1.5A1.5 1.5 0 0 0 12 5.5M15 20h-1.5a1.5 1.5 0 0 1-1.5-1.5M9.5 12h5")
        },
        {
            Icon.Sticker,
            Stroke("M3 5.5A2.5 2.5 0 0 1 5.5 3h13A2.5 2.5 0 0 1 21 5.5V14l-7 7H5.5A2.5 2.5 0 0 1 3 18.5z"),
            Stroke("M21 14h-4.5a2.5 2.5 0 0 0-2.5 2.5V21"),
            Fill(Circle(9, 9.5, 1.25)),
            Fill(Circle(15, 9.5, 1.25)),
            Stroke("M8.5 13.5a4.5 4.5 0 0 0 4 1.5")
        },
        {
            Icon.PartyPopper,
            Stroke("M3 21 7 9l8 8z"),
            Stroke("M13 7.5c1-1 1-2.5 0-3.5M16.5 11c1-1 2.5-1 3.5 0"),
            Fill(Circle(14.5, 2.75, 1)),
            Fill(Circle(19.5, 5, 1)),
            Fill(Circle(21, 13.5, 1)),
            Fill(Circle(17, 7, 1))
        },
        {
            Icon.Ghost,
            Stroke("M5 21V10a7 7 0 0 1 14 0v11l-2.33-2-2.34 2L12 19l-2.33 2-2.34-2z"),
            Fill(Circle(9.5, 10, 1.25)),
            Fill(Circle(14.5, 10, 1.25))
        },
        {
            Icon.ScanFace,
            Stroke("M3 8V5.5A2.5 2.5 0 0 1 5.5 3H8M16 3h2.5A2.5 2.5 0 0 1 21 5.5V8M21 16v2.5a2.5 2.5 0 0 1-2.5 2.5H16M8 21H5.5A2.5 2.5 0 0 1 3 18.5V16"),
            Fill(Circle(9.5, 10, 1.25)),
            Fill(Circle(14.5, 10, 1.25)),
            Stroke("M9 14.5a4 4 0 0 0 6 0")
        },
        {
            Icon.Shuffle,
            Stroke("M2 18h3.5a4 4 0 0 0 3.3-1.75L15.2 6.75A4 4 0 0 1 18.5 5H22M19 2l3 3-3 3"),
            Stroke("M2 6h3.5a4 4 0 0 1 3.3 1.75l.45.65M14.75 15.6l.45.65A4 4 0 0 0 18.5 18H22M19 15l3 3-3 3")
        },

        // Weather.
        {
            Icon.Snowflake,
            [.. Enumerable.Range(0, 3).Select(i => Stroke(Rotate(Path("M12 2v20M9.5 3.5 12 5.5l2.5-2M9.5 20.5l2.5-2 2.5 2"), i * 60, 12, 12)))]
        },
        {
            Icon.CloudRain,
            Stroke(SmallCloud()),
            Stroke("M8 19.5v2M12 19.5v2M16 19.5v2")
        },
        {
            Icon.CloudSnow,
            Stroke(SmallCloud()),
            Fill(Circle(8, 20.5, 1.1)),
            Fill(Circle(12, 21, 1.1)),
            Fill(Circle(16, 20.5, 1.1))
        },
        {
            Icon.CloudMoon,
            Stroke("M15 2.5a5 5 0 0 0 6.5 6.5 6 6 0 0 1-3 3.5"),
            Stroke(Union(Circle(7, 16.5, 3.5), Circle(11.5, 13.5, 4.5), Circle(16, 17, 3), Rectangle(7, 17, 9, 3)))
        }
    };

    // A cloud with room under it for rain or snow.
    private static Geometry SmallCloud() =>
        Union(Circle(7, 12, 4.5), Circle(12.5, 7.5, 5.5), Circle(17.5, 12, 4.5), Rectangle(7, 12, 10.5, 4.5));

    // Six blades, each running from a corner of the inner hexagon past the next corner to the rim.
    private static IEnumerable<IconShape> ApertureBlades()
    {
        const double center = 12, inner = 4.5, rim = 10;

        for (int i = 0; i < 6; i++)
        {
            Point start = Corner(i), next = Corner(i + 1);
            Vector delta = next - start;
            Vector direction = delta / delta.Length;
            Vector offset = start - new Point(center, center);
            double along = Vector.Dot(offset, direction);
            double length = -along + Math.Sqrt(along * along - Vector.Dot(offset, offset) + rim * rim);
            Point end = start + direction * length;
            yield return Stroke(FormattedPath($"M{start.X:0.##} {start.Y:0.##}L{end.X:0.##} {end.Y:0.##}"));
        }

        static Point Corner(int index)
        {
            double angle = Math.PI / 3 * index - Math.PI / 6;
            return new Point(center + inner * Math.Cos(angle), center + inner * Math.Sin(angle));
        }
    }
}
