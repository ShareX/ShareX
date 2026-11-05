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

using Avalonia.Media;
using static ShareX.Icons.IconGeometry;
using static ShareX.Icons.IconShape;
using Icon = ShareX.Icons.AppIcons;

namespace ShareX.Icons;

internal static partial class AppIconDefinitions
{
    private const string BadgePlus = "M18 14.5v7M14.5 18h7";
    private const string BadgeSearch = "M17.25 13.5a3 3 0 1 0 0 6 3 3 0 1 0 0-6zM19.5 17.75l2.25 2.25";
    private const string BadgeArrowRight = "M14 18h7.5M18.5 15l3 3-3 3";
    private const string BadgeRestart = "M21.5 18a3.5 3.5 0 1 1-1.03-2.47M21 12.75v2.75h-2.75";
    private const string VideoBody = "M2 8.5A2.5 2.5 0 0 1 4.5 6h9A2.5 2.5 0 0 1 16 8.5v7a2.5 2.5 0 0 1-2.5 2.5h-9A2.5 2.5 0 0 1 2 15.5z";
    private const string VideoLens = "M16 10.25l4.45-2.8A1 1 0 0 1 22 8.3v7.4a1 1 0 0 1-1.55.85L16 13.75";
    private const string SpeakerPath = "M4 9h3.5L12 5v14l-4.5-4H4a1 1 0 0 1-1-1v-4a1 1 0 0 1 1-1z";
    private const string DatabaseBody = "M4 5v14c0 1.4 3.6 2.5 8 2.5s8-1.1 8-2.5V5M4 12c0 1.4 3.6 2.5 8 2.5s8-1.1 8-2.5";

    private static IconGroup CreateControlIcons() => new(24, IconStyle.Rounded)
    {
        // Directions.
        {
            Icon.Plus,
            Stroke("M12 5v14M5 12h14")
        },
        {
            Icon.Minus,
            Stroke("M5 12h14")
        },
        {
            Icon.ChevronDown,
            Stroke("m6 9 6 6 6-6")
        },
        {
            Icon.ChevronUp,
            Stroke("m6 15 6-6 6 6")
        },
        {
            Icon.ChevronLeft,
            Stroke("m15 5-7 7 7 7")
        },
        {
            Icon.ChevronsUpDown,
            Stroke("m7 9.5 5-5 5 5M7 14.5l5 5 5-5")
        },
        {
            Icon.ArrowUp,
            Stroke("M12 20V4M6 10l6-6 6 6")
        },
        {
            Icon.ArrowDown,
            Stroke("M12 4v16M6 14l6 6 6-6")
        },
        {
            Icon.ArrowRight,
            Stroke("M4 12h16M14 6l6 6-6 6")
        },
        {
            Icon.ArrowRightToLine,
            Stroke("M3 12h12M10 7l5 5-5 5M20 4v16")
        },
        {
            Icon.ArrowLeftToLine,
            Stroke("M21 12H9M14 7l-5 5 5 5M4 4v16")
        },
        {
            Icon.ArrowRightFromLine,
            Stroke("M4 4v16M9 12h12M16 7l5 5-5 5")
        },
        {
            Icon.ArrowDownToLine,
            Stroke("M12 3v12M7 10l5 5 5-5M4 21h16")
        },
        {
            Icon.MoveHorizontal,
            Stroke("M3 12h18M7 8l-4 4 4 4M17 8l4 4-4 4")
        },

        // Playback.
        {
            Icon.Pause,
            Stroke(Rectangle(6, 4.5, 4, 15, 1.5)),
            Stroke(Rectangle(14, 4.5, 4, 15, 1.5))
        },
        {
            Icon.Stop,
            Stroke(Rectangle(5, 5, 14, 14, 3))
        },
        {
            Icon.CirclePause,
            Stroke(Circle(12, 12, 10)),
            Stroke("M10 9v6M14 9v6")
        },
        {
            Icon.StepForward,
            Stroke("M5.5 5.6v12.8a.8.8 0 0 0 1.24.67l9.6-6.4a.8.8 0 0 0 0-1.34l-9.6-6.4A.8.8 0 0 0 5.5 5.6z"),
            Stroke("M19.5 5v14")
        },
        {
            Icon.StepBack,
            Stroke("M18.5 5.6v12.8a.8.8 0 0 1-1.24.67l-9.6-6.4a.8.8 0 0 1 0-1.34l9.6-6.4a.8.8 0 0 1 1.24.67z"),
            Stroke("M4.5 5v14")
        },
        {
            Icon.Volume,
            Stroke(SpeakerPath),
            Stroke("M15.5 9a4 4 0 0 1 0 6M18.5 6a8 8 0 0 1 0 12")
        },
        {
            Icon.Mute,
            Stroke(SpeakerPath),
            Stroke("m16 9.5 5 5M21 9.5l-5 5")
        },
        {
            Icon.VideoOff,
            Overlay([Stroke(VideoBody), Stroke(VideoLens)], Slash)
        },
        {
            Icon.Microphone,
            Stroke(Rectangle(8.5, 2, 7, 12.5, 3.5)),
            Stroke("M5 11a7 7 0 0 0 14 0M12 18v3.5M8.5 21.5h7")
        },
        {
            Icon.Audio,
            Stroke("M3 10v4M7.5 6v12M12 3v18M16.5 7.5v9M21 10v4")
        },
        {
            Icon.Expand,
            Stroke("M15 3h6v6M9 21H3v-6M21 3l-7 7M3 21l7-7")
        },

        // History and state.
        {
            Icon.Reset,
            Stroke("M3.5 12A8.5 8.5 0 1 0 6 6L3.5 8.5M3.5 4v4.5H8")
        },
        {
            Icon.Undo,
            Stroke("M9 14 4 9l5-5M4 9h10.5a5.5 5.5 0 0 1 0 11H11")
        },
        {
            Icon.Redo,
            Stroke("m15 14 5-5-5-5M20 9H9.5a5.5 5.5 0 0 0 0 11H13")
        },
        {
            Icon.Loading,
            Stroke("M21 12a9 9 0 1 1-6.2-8.55")
        },
        {
            Icon.CircleDashed,
            Stroke(Circle(12, 12, 9), dashes: [1.5, 3.2])
        },
        {
            Icon.Help,
            Stroke(Circle(12, 12, 10)),
            Stroke("M9.25 9.25a2.85 2.85 0 0 1 5.5 1c0 1.9-2.75 2.4-2.75 4.25"),
            Fill(Circle(12, 17.5, 1.25))
        },
        {
            Icon.Bell,
            Stroke("M6 16.5V11a6 6 0 0 1 12 0v5.5l1.5 2h-15z"),
            Stroke("M10 21.5a2.3 2.3 0 0 0 4 0")
        },
        {
            Icon.Star,
            Stroke(Star(12, 12.75, 10, 4.4))
        },
        {
            Icon.Trophy,
            Stroke("M7 3.5h10v6a5 5 0 0 1-10 0z"),
            Stroke("M7 5.5H5a2.5 2.5 0 0 0 0 5h2.25M17 5.5h2a2.5 2.5 0 0 1 0 5h-2.25"),
            Stroke("M12 14.5v3.5M8 21h8M9.5 18h5")
        },
        {
            Icon.Target,
            Stroke(Circle(12, 12, 10)),
            Stroke(Circle(12, 12, 5.5)),
            Fill(Circle(12, 12, 1.75))
        },
        {
            Icon.Locate,
            Stroke(Circle(12, 12, 7)),
            Fill(Circle(12, 12, 2.5)),
            Stroke("M12 2v3M12 19v3M2 12h3M19 12h3")
        },
        {
            Icon.Crosshair,
            Stroke(Circle(12, 12, 9)),
            Stroke("M12 2v5.5M12 16.5V22M2 12h5.5M16.5 12H22")
        },
        {
            Icon.Lightbulb,
            Stroke("M8.5 15.5c0-1.5-.6-2.4-1.6-3.6A6 6 0 1 1 17 11.9c-1 1.2-1.5 2.1-1.5 3.6z"),
            Stroke("M9 18.5h6M10.5 21.5h3")
        },
        {
            Icon.Zap,
            Stroke("M13.5 2 4.5 13.5h7l-1 8.5 9-11.5h-7z")
        },
        {
            Icon.Theme,
            Stroke(Circle(12, 12, 4.5)),
            Fill("M12 7.5a4.5 4.5 0 0 0 0 9z"),
            Stroke("M12 2v2M12 20v2M2 12h2M20 12h2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4")
        },

        // Editing.
        {
            Icon.Crop,
            Stroke("M6 2v14a2 2 0 0 0 2 2h14M18 22V8a2 2 0 0 0-2-2H2")
        },
        {
            Icon.Duplicate,
            Stroke("M16 4.5A2.5 2.5 0 0 0 13.5 2h-9A2.5 2.5 0 0 0 2 4.5v9A2.5 2.5 0 0 0 4.5 16"),
            Stroke(Rectangle(8, 8, 14, 14, 2.5)),
            Stroke("M15 12v6M12 15h6")
        },
        {
            Icon.ZoomIn,
            Stroke(Circle(10.5, 10.5, 7)),
            Stroke("m20.5 20.5-5-5M10.5 7.5v6M7.5 10.5h6")
        },
        {
            Icon.Filter,
            Stroke("M3 4.5h18l-7 8.5v6l-4 2v-8z")
        },
        {
            Icon.AlignLeft,
            Stroke("M3 6h18M3 12h11M3 18h14")
        },
        {
            Icon.AlignCenter,
            Stroke("M3 6h18M7 12h10M5 18h14")
        },
        {
            Icon.AlignRight,
            Stroke("M3 6h18M10 12h11M7 18h14")
        },
        {
            Icon.Bold,
            Stroke("M7 4h6a4 4 0 0 1 0 8H7zM7 12h7a4 4 0 0 1 0 8H7z")
        },
        {
            Icon.Italic,
            Stroke("M10 4h9M5 20h9M15 4 9 20")
        },
        // The front square shaded, above the one behind it.
        {
            Icon.BringToFront,
            Overlay([Stroke(Rectangle(9, 9, 13, 13, 2.5))], Rectangle(0, 0, 17, 17, 4), Fill(Rectangle(2, 2, 13, 13, 2.5), 0.35), Stroke(Rectangle(2, 2, 13, 13, 2.5)))
        },
        // The back square shaded, under the one in front.
        {
            Icon.SendToBack,
            Overlay([Fill(Rectangle(9, 9, 13, 13, 2.5), 0.35), Stroke(Rectangle(9, 9, 13, 13, 2.5))], Rectangle(0, 0, 17, 17, 4), Stroke(Rectangle(2, 2, 13, 13, 2.5)))
        },
        {
            Icon.BringForward,
            Stroke("M12 13V3.5M8 7.5l4-4 4 4"),
            Stroke(Rectangle(4, 16, 16, 5, 2))
        },
        {
            Icon.SendBackward,
            Stroke(Rectangle(4, 3, 16, 5, 2)),
            Stroke("M12 11v9.5M8 16.5l4 4 4-4")
        },
        {
            Icon.RotateLeft,
            Stroke(Rectangle(9.5, 10, 11.5, 11.5, 2.5)),
            Stroke("M16 6h-5.5a5 5 0 0 0-5 5v3M8 11.5 5.5 14 3 11.5")
        },
        {
            Icon.RotateRight,
            Stroke(Rectangle(3, 10, 11.5, 11.5, 2.5)),
            Stroke("M8 6h5.5a5 5 0 0 1 5 5v3M16 11.5l2.5 2.5 2.5-2.5")
        },
        {
            Icon.ClearAll,
            Stroke("M14.5 2.5 12 8.5"),
            Stroke("M7.5 8.5h9a1.5 1.5 0 0 1 1.5 1.5v1.5H6V10a1.5 1.5 0 0 1 1.5-1.5z"),
            Stroke("M6.5 11.5 5 21.5h14l-1.5-10M10 21.5l.5-4M14 21.5l-.5-4")
        },
        {
            Icon.Contrast,
            Stroke(Circle(12, 12, 9.5)),
            Fill("M12 2.5a9.5 9.5 0 0 1 0 19z")
        },
        {
            Icon.Blend,
            Fill(Subtract(Circle(9, 12, 6.5), Subtract(Circle(9, 12, 6.5), Circle(15, 12, 6.5))), 0.35),
            Stroke(Circle(9, 12, 6.5)),
            Stroke(Circle(15, 12, 6.5))
        },
        {
            Icon.Swatches,
            Fill(Rectangle(3, 3, 8, 8, 2)),
            Fill(Rectangle(13, 3, 8, 8, 2), 0.65),
            Fill(Rectangle(3, 13, 8, 8, 2), 0.4),
            Stroke(Rectangle(13, 13, 8, 8, 2))
        },
        {
            Icon.Wallpaper,
            Fill(Rectangle(3, 3, 18, 18, 3), 0.3),
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke("m3 16.5 5.5-5.5 9.5 9.5"),
            Fill(Circle(15.5, 8.5, 1.75))
        },
        {
            Icon.Grid,
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke("M9 3v18M15 3v18M3 9h18M3 15h18")
        },
        {
            Icon.Grip,
            Fill(Circle(9, 6, 1.5)),
            Fill(Circle(15, 6, 1.5)),
            Fill(Circle(9, 12, 1.5)),
            Fill(Circle(15, 12, 1.5)),
            Fill(Circle(9, 18, 1.5)),
            Fill(Circle(15, 18, 1.5))
        },
        {
            Icon.PointerClick,
            Stroke("M10 10v11.25l3-2.85 2 4.5 2.2-.95-2-4.45h4.1z", 1.75),
            Stroke("M10 2.5V5M2.5 10H5M4.7 4.7l1.75 1.75M15.3 4.7l-1.75 1.75M4.7 15.3l1.75-1.75")
        },
        {
            Icon.Lock,
            Stroke(Rectangle(4, 10.5, 16, 11, 2.5)),
            Stroke("M7.5 10.5V7a4.5 4.5 0 0 1 9 0v3.5")
        },
        {
            Icon.Mouse,
            Stroke(Rectangle(5, 2, 14, 20, 7)),
            Stroke("M12 6v4")
        },

        // Files, folders and data.
        {
            Icon.FileAdd,
            [.. PageShapes(), Stroke("M12 11.5v7M8.5 15h7")]
        },
        {
            Icon.FileDownload,
            [.. PageShapes(), Stroke("M12 11.5v7M9 15.5l3 3 3-3")]
        },
        {
            Icon.FileJson,
            [.. PageShapes(), Stroke("M10 11.5a1 1 0 0 0-1 1v1.5l-1 1 1 1v1.5a1 1 0 0 0 1 1M14 11.5a1 1 0 0 1 1 1v1.5l1 1-1 1v1.5a1 1 0 0 1-1 1")]
        },
        {
            Icon.FileEdit,
            Overlay(PageShapes(), BadgePen)
        },
        {
            Icon.FileSearch,
            Badge(PageShapes(), Stroke(BadgeSearch))
        },
        {
            Icon.FolderAdd,
            Stroke(FolderPath),
            Stroke("M12 10.25v6.5M8.75 13.5h6.5")
        },
        {
            Icon.FolderSearch,
            Badge([Stroke(FolderPath)], Stroke(BadgeSearch))
        },
        {
            Icon.ImageAdd,
            Badge(ImageShapes(), Stroke(BadgePlus))
        },
        {
            Icon.ClipboardPaste,
            Badge(ClipboardShapes(), Stroke(BadgeArrowRight))
        },
        {
            Icon.ClipboardClear,
            [.. ClipboardShapes(), Stroke("m9.5 11.5 5 5M14.5 11.5l-5 5")]
        },
        {
            Icon.Database,
            DatabaseShapes()
        },
        {
            Icon.DatabaseBackup,
            Badge(DatabaseShapes(), Stroke(BadgeRestart))
        },
        {
            Icon.ListRestart,
            Badge([Stroke("M3 6h16M3 12h9M3 18h7")], Stroke(BadgeRestart))
        },
        {
            Icon.Package,
            Stroke("M12 2.5 3 7v10l9 4.5 9-4.5V7z"),
            Stroke("M3 7l9 4.5L21 7M12 11.5v10M7.5 4.75l9 4.5")
        },
        {
            Icon.Blocks,
            Stroke(Rectangle(3, 13, 8, 8, 2)),
            Stroke(Rectangle(13, 13, 8, 8, 2)),
            Stroke(Rectangle(3, 3, 8, 8, 2)),
            Stroke(Rotate(Rectangle(13.5, 2.5, 7, 7, 2), 20, 17, 6))
        },

        // Connections and services.
        {
            Icon.Cloud,
            Stroke(CloudGeometry())
        },
        {
            Icon.Server,
            Stroke(Rectangle(2, 3, 20, 8, 2.5)),
            Stroke(Rectangle(2, 13, 20, 8, 2.5)),
            Fill(Circle(6.5, 7, 1.25)),
            Fill(Circle(6.5, 17, 1.25)),
            Stroke("M11 7h6.5M11 17h6.5")
        },
        {
            Icon.Plug,
            Stroke("M9 2.5V7M15 2.5V7"),
            Stroke("M6 7h12v4a6 6 0 0 1-12 0z"),
            Stroke("M12 17v4.5")
        },
        {
            Icon.Network,
            Stroke(Rectangle(9, 2, 6, 5, 1.5)),
            Stroke(Rectangle(2, 17, 6, 5, 1.5)),
            Stroke(Rectangle(16, 17, 6, 5, 1.5)),
            Stroke("M12 7v5M5 17v-2.5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2V17")
        },
        {
            Icon.Send,
            Stroke("M21.5 2.5 2.5 10l7.5 3.5 3.5 7.5z"),
            Stroke("M10 13.5 21.5 2.5")
        },
        {
            Icon.Reply,
            Stroke("M9 17l-5-5 5-5M4 12h11a5 5 0 0 1 5 5v2")
        },
        {
            Icon.Language,
            Stroke("M3 5h10M8 3v2M5.5 5c.6 3.6 2.6 6.2 5.5 7.6M10.5 5c-.6 3.6-2.6 6.2-6 7.6"),
            Stroke("M12.5 21l4.25-10 4.25 10M14 17.5h5.5")
        },
        {
            Icon.MessageCheck,
            Stroke(BubblePath),
            Stroke("m8.5 10.5 2.5 2.5 4.5-4.5")
        },

        // Search and scanning.
        {
            Icon.SearchCheck,
            Stroke(Circle(10.5, 10.5, 7)),
            Stroke("m20.5 20.5-5-5"),
            Stroke("m7.5 10.5 2 2 3.5-3.5")
        },
        {
            Icon.ScanLine,
            Stroke(Brackets),
            Stroke("M7 12h10")
        },
        {
            Icon.ScanEye,
            Stroke(Brackets),
            Stroke("M6.5 12s2-3.5 5.5-3.5 5.5 3.5 5.5 3.5-2 3.5-5.5 3.5S6.5 12 6.5 12z"),
            Fill(Circle(12, 12, 1.5))
        },
        {
            Icon.QrScan,
            Stroke(Brackets),
            Fill(Rectangle(8, 8, 3.25, 3.25, 0.75)),
            Fill(Rectangle(12.75, 8, 3.25, 3.25, 0.75)),
            Fill(Rectangle(8, 12.75, 3.25, 3.25, 0.75)),
            Fill(Rectangle(12.75, 12.75, 3.25, 3.25, 0.75), 0.45)
        },

        // Windows and layout.
        {
            Icon.PictureInPicture,
            Stroke(Rectangle(2, 4, 20, 16, 2.5)),
            Fill(Rectangle(11.5, 11.5, 7, 5, 1), 0.35),
            Stroke(Rectangle(11.5, 11.5, 7, 5, 1))
        },
        {
            Icon.Dashboard,
            Stroke(Rectangle(3, 3, 8, 10, 2)),
            Stroke(Rectangle(13, 3, 8, 6, 2)),
            Stroke(Rectangle(13, 11, 8, 10, 2)),
            Stroke(Rectangle(3, 15, 8, 6, 2))
        },
        {
            Icon.LineChart,
            Stroke("M3 3v15.5A2.5 2.5 0 0 0 5.5 21H21"),
            Stroke("m7 15 4-5 3.5 3 5-6.5")
        },

        // Objects.
        {
            Icon.Flask,
            Fill("M6.9 15.5h10.2l2.15 3.4A1.15 1.15 0 0 1 18.3 20.6H5.7a1.15 1.15 0 0 1-.95-1.7z", 0.35),
            Stroke("M9 3h6M10 3v6.5L4.6 18.5A1.6 1.6 0 0 0 6 21h12a1.6 1.6 0 0 0 1.4-2.5L14 9.5V3")
        },
        {
            Icon.Key,
            Stroke(Circle(7.5, 15.5, 4.5)),
            Stroke("M10.75 12.25 21 2M17 6l3 3M14.25 8.75l2.5 2.5")
        },
        {
            Icon.Rocket,
            Stroke("M9.5 14.5 7 12c1.5-4.5 5.5-9 13.5-9.5C20 10.5 16 14.5 12 17z"),
            Stroke("M7 12H3.5L6 8.5h4.5M12 17v3.5l3.5-2.5V13.5"),
            Stroke("M6 15.5c-1.5 1.25-2 4.5-2 4.5s3.25-.5 4.5-2"),
            Stroke(Circle(15, 9, 1.75))
        },
        {
            Icon.Satellite,
            Stroke(Rectangle(9, 9, 6, 7, 1.5)),
            Stroke(Rectangle(1.5, 9.5, 5.5, 6, 1)),
            Stroke(Rectangle(17, 9.5, 5.5, 6, 1)),
            Stroke("M4.25 9.5v6M19.75 9.5v6M7 12.5h2M15 12.5h2M12 9V5.5"),
            Fill(Circle(12, 4.25, 1.5))
        },
        {
            Icon.Landmark,
            Stroke("M3 21h18M4.5 18h15M6 18v-7M10 18v-7M14 18v-7M18 18v-7"),
            Stroke("M3 9 12 3.5 21 9z")
        },
        {
            Icon.Factory,
            Stroke("M3 21V10l5 3v-3l5 3v-3l5 3V4h3v17z"),
            Stroke("M7 17h2M12 17h2")
        }
    };

    private static IconShape[] DatabaseShapes() => [Stroke(Ellipse(4, 2.5, 16, 5)), Stroke(DatabaseBody)];
}
