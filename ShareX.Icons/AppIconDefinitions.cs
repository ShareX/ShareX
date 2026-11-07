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

/// <summary>The shapes of the <see cref="AppIcons"/>.</summary>
/// <remarks>
/// Drawn like the <see cref="ImageEditorIcons"/>: a 24x24 grid, 2px strokes with round caps and joins, content
/// within 2..22. Related icons share their parts (page, folder, cloud, image, frame, brackets), and variants add
/// a glyph inside the part or an overlay, such as a badge in the bottom-right corner or a slash, which cuts a gap
/// out of the shapes under it. Shading is the Foreground at partial opacity.
/// </remarks>
internal static partial class AppIconDefinitions
{
    // Overlays drawn over other shapes: a badge in the bottom-right corner, or a slash for "off".
    private const string BadgeUp = "M18 22v-7.5M14.75 17.5 18 14.25l3.25 3.25";
    private const string BadgeDown = "M18 14v7.5M14.75 18.5 18 21.75l3.25-3.25";
    private const string BadgeShare = "M14.5 21.5l7-7M16.5 14.5h5v5";
    private const string BadgePen = "M14.25 22l.6-2.75 5-5a1.45 1.45 0 0 1 2.05 2.05l-5 5z";
    private const string BadgeCheck = "m14.75 18 2.25 2.25 4.25-4.25";
    private const string BadgeClock = "M18 14a4 4 0 1 0 0 8 4 4 0 1 0 0-8zM18 16v2.25l1.5 1";
    private const string BadgeConvert = "M15.75 21.5V15M14 16.75l1.75-1.75 1.75 1.75M20.25 15v6.5M18.5 19.75l1.75 1.75 1.75-1.75";
    private const string Slash = "M2.5 2.5l19 19";
    private const string CaptionSlash = "M2.5 2.5l10 10";

    // The gap left around an overlay: its 2px line plus 1.5px on each side.
    private const double OverlayHalo = 5;

    // The round gap cut for a badge in the bottom-right corner.
    private const double BadgeGap = 6.25;

    // How strongly the corner pixels of the title bar frames are drawn.
    private const double SoftCornerOpacity = 0.45;

    // Shared parts.
    private const string PagePath = "M14 2H7a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7z";
    private const string PageFold = "M14 2v4a1 1 0 0 0 1 1h4";
    private const string FolderPath = "M2 6.5A2.5 2.5 0 0 1 4.5 4h3.6a2 2 0 0 1 1.6.8L11 6.5h8.5A2.5 2.5 0 0 1 22 9v8.5a2.5 2.5 0 0 1-2.5 2.5h-15A2.5 2.5 0 0 1 2 17.5z";
    private const string Brackets = "M3 8V5.5A2.5 2.5 0 0 1 5.5 3H8M16 3h2.5A2.5 2.5 0 0 1 21 5.5V8M21 16v2.5a2.5 2.5 0 0 1-2.5 2.5H16M8 21H5.5A2.5 2.5 0 0 1 3 18.5V16";
    private const string ClipboardBoard = "M8 4H6.5A2.5 2.5 0 0 0 4 6.5v13A2.5 2.5 0 0 0 6.5 22h11a2.5 2.5 0 0 0 2.5-2.5v-13A2.5 2.5 0 0 0 17.5 4H16";
    private const string BubblePath = "M6 3.5h12a4 4 0 0 1 4 4v6a4 4 0 0 1-4 4h-6.5L6 21v-3.5a4 4 0 0 1-4-4v-6a4 4 0 0 1 4-4z";
    private const string Tray = "M3 15v3.5A2.5 2.5 0 0 0 5.5 21h13a2.5 2.5 0 0 0 2.5-2.5V15";
    private const string FloppyPath = "M5.5 3h10.3a2 2 0 0 1 1.4.6l3.2 3.2a2 2 0 0 1 .6 1.4v10.3a2.5 2.5 0 0 1-2.5 2.5h-13A2.5 2.5 0 0 1 3 18.5v-13A2.5 2.5 0 0 1 5.5 3z";
    private const string ShieldPath = "M12 2.5 4 5.5V11c0 5 3.4 8.8 8 10.5 4.6-1.7 8-5.5 8-10.5V5.5z";

    public static IEnumerable<IconGroup> CreateGroups() => [CreateIcons(), CreateControlIcons(), CreateCaptionIcons()];

    private static IconGroup CreateIcons() => new(24, IconStyle.Rounded)
    {
        // Menu items with an icon of their own.
        {
            Icon.Workflows,
            Stroke(Rectangle(2, 2.5, 8, 6, 2)),
            Stroke(Rectangle(14, 9, 8, 6, 2)),
            Stroke(Rectangle(2, 15.5, 8, 6, 2)),
            Stroke("M10 5.5h5a3 3 0 0 1 3 3v.5M18 15v.5a3 3 0 0 1-3 3h-5")
        },
        {
            Icon.AfterCaptureTasks,
            Badge(ImageShapes(), Stroke(BadgeCheck))
        },
        {
            Icon.AfterUploadTasks,
            Badge([Stroke(CloudGeometry())], Stroke(BadgeCheck))
        },
        {
            Icon.Destinations,
            GlobeShapes()
        },
        {
            Icon.DestinationSettings,
            Overlay(GlobeShapes(), Circle(18, 18, 6.75), Fill(BadgeGear()))
        },
        // A cloud with code inside: an uploader you define yourself.
        {
            Icon.CustomUploaderSettings,
            Stroke(CloudGeometry()),
            Stroke("m10.25 12.25-2 2.25 2 2.25M14.75 12.25l2 2.25-2 2.25")
        },
        {
            Icon.ScreenshotsFolder,
            Stroke(FolderPath),
            Fill(Circle(15.5, 11, 1.4)),
            Stroke("m6 17 3.5-3.5 3.5 3.5 1.5-1.5 2.5 2.5")
        },
        {
            Icon.ImageHistory,
            Badge(ImageShapes(), Stroke(BadgeClock))
        },
        // A person with a plus.
        {
            Icon.Follow,
            Stroke(Circle(9.5, 7.5, 4)),
            Stroke("M2.5 21v-.5a6 6 0 0 1 6-6h2a6 6 0 0 1 6 6v.5"),
            Stroke("M19.5 8v6M16.5 11h6")
        },
        // A chat bubble with a message being typed.
        {
            Icon.Discord,
            Stroke("M7.7 19.9A9 9 0 1 0 4.1 16.3L3 21z"),
            Fill(Circle(8, 12, 1.3)),
            Fill(Circle(12, 12, 1.3)),
            Fill(Circle(16, 12, 1.3))
        },
        // A shield with a restart arrow.
        {
            Icon.RestartAsAdmin,
            Stroke(ShieldPath),
            Stroke("M15.25 12a3.25 3.25 0 1 1-.95-2.3M14.3 7.2v2.5h-2.5")
        },
        {
            Icon.RecentItems,
            Badge([Stroke("M3 6h16M3 12h9M3 18h7")], Stroke(BadgeClock))
        },
        // The screen with a record dot.
        {
            Icon.ScreenRecording,
            [.. MonitorShapes(), Fill(Circle(12, 10, 3))]
        },
        {
            Icon.AutoCapture,
            Badge(CameraShapes(), Stroke(BadgeClock))
        },
        // A drop zone with an arrow into it.
        {
            Icon.DragDropUpload,
            Stroke(Rectangle(3, 3, 18, 18, 3), dashes: [2, 3.3]),
            Stroke("M12 16.5v-8M8.5 12 12 8.5 15.5 12")
        },
        {
            Icon.UploadClipboard,
            [.. ClipboardShapes(), Stroke("M12 18v-7M9 14l3-3 3 3")]
        },
        // A pointer at the center of a highlight.
        {
            Icon.MouseHighlighter,
            Fill(Circle(9.5, 9.5, 7), 0.3),
            Stroke("M9.5 9.5v12.15l3.08-2.93 2.03 4.65 2.25-.98-2.03-4.57h4.2z", 1.75)
        },
        // An eyedropper: squeeze bulb, collar and a glass tube holding the picked color.
        {
            Icon.ScreenColorPicker,
            Fill("M10 12 6 16l-1.5 3.5L8 18l4-4z", 0.35),
            Stroke("M12 8.5 15.75 4.75a2.47 2.47 0 0 1 3.5 3.5L15.5 12"),
            Stroke("M11.25 7.75l5 5"),
            Stroke("M12.75 9.25 6 16l-1.5 3.5L8 18l6.75-6.75")
        },
        {
            Icon.BackgroundRemover,
            Stroke("M10 21.2 6.56 17.76c-.8-.8-.8-2 0-2.72l7.68-7.68c.8-.8 2-.8 2.72 0l4.48 4.48c.8.8.8 2 0 2.72L14.8 21.2M22 21.2H10M8.4 13.2l7.2 7.2"),
            Fill(Sparkle(5.5, 5.5, 4.5))
        },
        // An app icon tile being converted.
        {
            Icon.IconConverter,
            Badge([Stroke(Rectangle(3, 3, 18, 18, 5.5)), Fill(Sparkle(10, 10, 4))], Stroke(BadgeConvert))
        },
        // A picture with a sparkle badge.
        {
            Icon.ImageBeautifier,
            Overlay(ImageShapes(), Circle(18.5, 18.5, 6), Fill(Sparkle(18.5, 18.5, 4.75)))
        },
        // A picture split into before and after, with a slider handle.
        {
            Icon.ImageComparer,
            Fill(Subtract(Rectangle(3, 3, 18, 18, 3), Rectangle(12, 0, 12, 24)), 0.3),
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke("M12 1v22"),
            Fill(Circle(12, 12, 2.75))
        },
        {
            Icon.ImageConverter,
            Badge(ImageShapes(), Stroke(BadgeConvert))
        },
        {
            Icon.ImageEditor,
            Overlay(ImageShapes(), BadgePen)
        },
        // A picture shrunk to a thumbnail in its corner.
        {
            Icon.ImageThumbnailer,
            Stroke(Rectangle(2, 2, 20, 20, 3)),
            Fill(Rectangle(12, 12, 6, 6, 1), 0.35),
            Stroke(Rectangle(12, 12, 6, 6, 1)),
            Stroke("M6.5 6.5l3.5 3.5M10 6.5V10H6.5")
        },
        // Animation frames with a play button.
        {
            Icon.AnimatedGifMaker,
            Stroke(Rectangle(2, 7, 15, 15, 2.5)),
            Stroke("M6.5 3H19a3 3 0 0 1 3 3v12.5"),
            Stroke("M7.75 11.75v5.5a.5.5 0 0 0 .77.42l4.1-2.75a.5.5 0 0 0 0-.84l-4.1-2.75a.5.5 0 0 0-.77.42z")
        },
        {
            Icon.VideoConverter,
            Badge([Stroke(Rectangle(2, 4, 20, 16, 3)), Stroke("M8 8.75v6.5a.5.5 0 0 0 .77.42l4.9-3.25a.5.5 0 0 0 0-.84l-4.9-3.25a.5.5 0 0 0-.77.42z")], Stroke(BadgeConvert))
        },
        {
            Icon.RemoteStorageBrowser,
            Badge([Stroke(FolderPath)], Stroke(BadgeCloud()))
        },
        {
            Icon.ClipboardViewer,
            [.. ClipboardShapes(), Stroke("M7.25 14.5s1.75-3.25 4.75-3.25 4.75 3.25 4.75 3.25-1.75 3.25-4.75 3.25-4.75-3.25-4.75-3.25z"), Fill(Circle(12, 14.5, 1.25))]
        },
        // The screen showing a test pattern.
        {
            Icon.MonitorTest,
            [.. MonitorShapes(), Fill(Rectangle(5, 6, 4.67, 8, 0.75)), Fill(Rectangle(9.67, 6, 4.67, 8), 0.6), Fill(Rectangle(14.33, 6, 4.67, 8, 0.75), 0.3)]
        },
        {
            Icon.ClipboardImage,
            [.. ClipboardShapes(), Fill(Circle(14.75, 11, 1.25)), Stroke("m7.5 19 3.5-3.5 3.5 3.5M14 17l1-1 1.5 1.5")]
        },
        {
            Icon.ClipboardLink,
            [.. ClipboardShapes(), Stroke("M10.25 12H9.5a2.5 2.5 0 0 0 0 5h.75M13.75 12h.75a2.5 2.5 0 0 1 0 5h-.75M10.5 14.5h3")]
        },

        // Capture.
        {
            Icon.Camera,
            CameraShapes()
        },
        // The whole screen selected.
        {
            Icon.Fullscreen,
            [.. MonitorShapes(), Fill(Rectangle(5, 6, 14, 8, 1), 0.35)]
        },
        {
            Icon.Window,
            WindowShapes()
        },
        {
            Icon.Monitor,
            MonitorShapes()
        },
        {
            Icon.Region,
            Stroke(Brackets),
            Stroke("M12 9v6M9 12h6")
        },
        // The region brackets around a transparency checkerboard.
        {
            Icon.RegionTransparent,
            Stroke(Brackets),
            Fill(Rectangle(7, 7, 5, 5, 1)),
            Fill(Rectangle(12, 7, 5, 5, 1), 0.35),
            Fill(Rectangle(7, 12, 5, 5, 1), 0.35),
            Fill(Rectangle(12, 12, 5, 5, 1))
        },
        // The region brackets around a repeat arrow.
        {
            Icon.LastRegion,
            Stroke(Brackets),
            Stroke("M7.75 12a4.25 4.25 0 1 0 1.25-3M9 5.75V9h3.25")
        },
        {
            Icon.Video,
            Stroke(Rectangle(2, 6, 14, 12, 2.5)),
            Stroke("M16 10.25l4.45-2.8A1 1 0 0 1 22 8.3v7.4a1 1 0 0 1-1.55.85L16 13.75")
        },
        // A film strip with a frame in the middle.
        {
            Icon.Film,
            FilmShapes()
        },
        // A tall page with arrows down the length of it.
        {
            Icon.ScrollingCapture,
            Stroke(Rectangle(5, 2, 14, 20, 3)),
            Stroke("m9 8 3 3 3-3M9 13l3 3 3-3")
        },
        {
            Icon.Clock,
            Stroke(Circle(12, 12, 10)),
            Stroke("M12 6.5V12l3.5 2")
        },
        {
            Icon.Pointer,
            Stroke("M5.65 2.75v16.2l4.1-3.9 2.7 6.2 3-1.3-2.7-6.1h5.6z", 1.75)
        },
        {
            Icon.Timer,
            TimerShapes()
        },
        {
            Icon.TimerOff,
            Overlay(TimerShapes(), Slash)
        },

        // Upload, links and sharing.
        {
            Icon.Upload,
            Stroke(Tray),
            Stroke("M12 16V3.5M7.5 8 12 3.5 16.5 8")
        },
        {
            Icon.Download,
            Stroke(Tray),
            Stroke("M12 3.5V16M7.5 11.5 12 16l4.5-4.5")
        },
        {
            Icon.CloudUpload,
            Overlay([Stroke(CloudGeometry())], "M12 22v-9M8.75 16.25 12 13l3.25 3.25")
        },
        {
            Icon.CloudOff,
            Overlay([Stroke(CloudGeometry())], Slash)
        },
        {
            Icon.Link,
            Stroke("M10 13.5a4 4 0 0 0 5.66.5l3.5-3.5a4 4 0 0 0-5.66-5.66l-1.25 1.25M14 10.5a4 4 0 0 0-5.66-.5l-3.5 3.5a4 4 0 0 0 5.66 5.66l1.25-1.25")
        },
        {
            Icon.ShortLink,
            Stroke("M9 7H7a5 5 0 0 0 0 10h2M15 7h2a5 5 0 0 1 0 10h-2M8.5 12h7")
        },
        {
            Icon.ExternalLink,
            Stroke("M12 4H6.5A2.5 2.5 0 0 0 4 6.5v11A2.5 2.5 0 0 0 6.5 20h11a2.5 2.5 0 0 0 2.5-2.5V12"),
            Stroke("M10.5 13.5 20 4M14.5 4H20v5.5")
        },
        {
            Icon.Share,
            Stroke(Circle(18, 5.5, 2.75)),
            Stroke(Circle(6, 12, 2.75)),
            Stroke(Circle(18, 18.5, 2.75)),
            Stroke("M8.4 10.65l7.2-3.8M8.4 13.35l7.2 3.8")
        },
        {
            Icon.QrCode,
            Stroke(Rectangle(3, 3, 7, 7, 1.5)),
            Stroke(Rectangle(14, 3, 7, 7, 1.5)),
            Stroke(Rectangle(3, 14, 7, 7, 1.5)),
            Fill(Rectangle(13, 13, 3.5, 3.5, 0.75)),
            Fill(Rectangle(18.5, 13, 3.5, 3.5, 0.75)),
            Fill(Rectangle(13, 18.5, 3.5, 3.5, 0.75)),
            Fill(Rectangle(18.5, 18.5, 3.5, 3.5, 0.75))
        },

        // Files and folders: the page or folder with a glyph inside.
        {
            Icon.File,
            PageShapes()
        },
        {
            Icon.FileText,
            [.. PageShapes(), Stroke("M8.5 12h4M8.5 15.25h7M8.5 18.5h7")]
        },
        {
            Icon.FileImage,
            [.. PageShapes(), Fill(Circle(9.75, 11.5, 1.5)), Stroke("m5 19.5 4.5-4.5 2.5 2.5 2-2 4 4")]
        },
        {
            Icon.FileVideo,
            [.. PageShapes(), Stroke("M10 11.75v6.5a.5.5 0 0 0 .77.42l4.9-3.25a.5.5 0 0 0 0-.84l-4.9-3.25a.5.5 0 0 0-.77.42z")]
        },
        {
            Icon.FileCode,
            [.. PageShapes(), Stroke("M10 12.25 7.75 15 10 17.75M14 12.25 16.25 15 14 17.75")]
        },
        {
            Icon.FileUpload,
            [.. PageShapes(), Stroke("M12 18.5v-7M9 14.5l3-3 3 3")]
        },
        {
            Icon.FileDelete,
            [.. PageShapes(), Stroke("m9.5 12.5 5 5M14.5 12.5l-5 5")]
        },
        // A page in front of the corner of another.
        {
            Icon.Files,
            Stroke("M17 5h-6.5A2.5 2.5 0 0 0 8 7.5v12a2.5 2.5 0 0 0 2.5 2.5h8a2.5 2.5 0 0 0 2.5-2.5V9z"),
            Stroke("M17 5v3a1 1 0 0 0 1 1h3"),
            Stroke("M4 16.5v-12A2.5 2.5 0 0 1 6.5 2h7")
        },
        {
            Icon.Folder,
            Stroke(FolderPath)
        },
        {
            Icon.FolderOpen,
            Stroke("M4 20a2 2 0 0 1-2-2V6.5A2.5 2.5 0 0 1 4.5 4h3.6a2 2 0 0 1 1.6.8L11 6.5h6.5A2.5 2.5 0 0 1 20 9v1.5"),
            Stroke("M4 20h13.35a2 2 0 0 0 1.9-1.38l2.3-6.6a1.15 1.15 0 0 0-1.1-1.52H8.35a2 2 0 0 0-1.9 1.38z")
        },
        {
            Icon.FolderUpload,
            Stroke(FolderPath),
            Stroke("M12 16.75v-6M9 13.25l3-3 3 3")
        },
        {
            Icon.FolderBookmark,
            Stroke(FolderPath),
            Stroke("M10 9.5h4v7l-2-1.5-2 1.5z")
        },
        // A trunk with two small folders branching off it.
        {
            Icon.FolderTree,
            Stroke("M4 3v12.25a2.5 2.5 0 0 0 2.5 2.5H12M4 7.75h8"),
            Stroke(SmallFolder(12.5, 3.5)),
            Stroke(SmallFolder(12.5, 13.5))
        },
        // A route from one point to another.
        {
            Icon.FilePath,
            Stroke(Circle(5.5, 18.5, 2.5)),
            Stroke(Circle(18.5, 5.5, 2.5)),
            Stroke("M8 18.5h8a3.25 3.25 0 0 0 0-6.5H8a3.25 3.25 0 0 1 0-6.5h8")
        },
        {
            Icon.Save,
            SaveShapes()
        },
        {
            Icon.SaveAs,
            Overlay(SaveShapes(), BadgePen)
        },
        {
            Icon.Printer,
            Stroke("M6 9V4.5A1.5 1.5 0 0 1 7.5 3h9A1.5 1.5 0 0 1 18 4.5V9"),
            Stroke("M6 18H4.5A2.5 2.5 0 0 1 2 15.5v-4A2.5 2.5 0 0 1 4.5 9h15a2.5 2.5 0 0 1 2.5 2.5v4a2.5 2.5 0 0 1-2.5 2.5H18"),
            Stroke(Rectangle(6, 14, 12, 7, 1))
        },
        {
            Icon.Trash,
            Stroke("M3.5 6h17M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"),
            Stroke("M5.5 6l.9 13.6A2.5 2.5 0 0 0 8.9 22h6.2a2.5 2.5 0 0 0 2.5-2.4L18.5 6"),
            Stroke("M10 10.5v7M14 10.5v7")
        },

        // Clipboard and text.
        {
            Icon.Clipboard,
            ClipboardShapes()
        },
        {
            Icon.ClipboardCopy,
            [.. ClipboardShapes(), Stroke("M12 10v7M9 14l3 3 3-3")]
        },
        {
            Icon.ClipboardList,
            [.. ClipboardShapes(), Stroke("M8.5 11h7M8.5 14.5h7M8.5 18h4")]
        },
        {
            Icon.Copy,
            Stroke("M16 4.5A2.5 2.5 0 0 0 13.5 2h-9A2.5 2.5 0 0 0 2 4.5v9A2.5 2.5 0 0 0 4.5 16"),
            Stroke(Rectangle(8, 8, 14, 14, 2.5))
        },
        {
            Icon.Code,
            Stroke("m8 6.5-5.5 5.5L8 17.5M16 6.5l5.5 5.5-5.5 5.5")
        },
        {
            Icon.Braces,
            Stroke("M8 3H7a2 2 0 0 0-2 2v4.5a2.5 2.5 0 0 1-2.5 2.5A2.5 2.5 0 0 1 5 14.5V19a2 2 0 0 0 2 2h1M16 3h1a2 2 0 0 1 2 2v4.5a2.5 2.5 0 0 0 2.5 2.5 2.5 2.5 0 0 0-2.5 2.5V19a2 2 0 0 1-2 2h-1")
        },
        {
            Icon.Message,
            Stroke(BubblePath),
            Stroke("M7 9h10M7 12.5h6")
        },
        // A reply bubble behind a message bubble.
        {
            Icon.Messages,
            Stroke("M8.5 7V5.5A2.5 2.5 0 0 1 11 3h8.5A2.5 2.5 0 0 1 22 5.5v6a2.5 2.5 0 0 1-2.5 2.5v3l-2.5-2.5"),
            Stroke("M4.5 8h8A2.5 2.5 0 0 1 15 10.5v5a2.5 2.5 0 0 1-2.5 2.5H8l-4 3v-3h.5A2.5 2.5 0 0 1 2 15.5v-5A2.5 2.5 0 0 1 4.5 8z")
        },
        {
            Icon.MessageShare,
            Overlay([Stroke(BubblePath)], BadgeShare)
        },
        {
            Icon.TextRecognition,
            Stroke(Brackets),
            Stroke("M7.5 9h9M7.5 12h9M7.5 15h5.5")
        },

        // Images and media.
        {
            Icon.Image,
            ImageShapes()
        },
        // A picture in front of another.
        {
            Icon.Images,
            Stroke(Rectangle(2, 7, 15, 15, 2.5)),
            Stroke("m2 18.5 4-4 7.5 7.5"),
            Fill(Circle(11.75, 11.75, 1.5)),
            Stroke("M7 3h12.5A2.5 2.5 0 0 1 22 5.5V17")
        },
        {
            Icon.ImageUpload,
            Overlay(ImageShapes(), BadgeUp)
        },
        {
            Icon.ImageDownload,
            Overlay(ImageShapes(), BadgeDown)
        },
        {
            Icon.Layers,
            Stroke("M12 2.75 2.5 7.75 12 12.75l9.5-5z"),
            Stroke("M2.5 12.25 12 17.25l9.5-5M2.5 16.75 12 21.75l9.5-5")
        },
        // Two pictures overlapping, shaded where they join.
        {
            Icon.Combine,
            Fill(Rectangle(9, 9, 6, 6), 0.35),
            Stroke(Rectangle(2, 2, 13, 13, 2.5)),
            Stroke(Rectangle(9, 9, 13, 13, 2.5))
        },
        // A picture cut into four tiles.
        {
            Icon.Split,
            Stroke(Rectangle(3, 3, 7.5, 7.5, 2)),
            Stroke(Rectangle(13.5, 3, 7.5, 7.5, 2)),
            Stroke(Rectangle(3, 13.5, 7.5, 7.5, 2)),
            Stroke(Rectangle(13.5, 13.5, 7.5, 7.5, 2))
        },
        {
            Icon.Rows,
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke("M3 12h18")
        },
        {
            Icon.Columns,
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke("M12 3v18")
        },
        // A small picture growing into a frame.
        {
            Icon.Resize,
            Stroke(Rectangle(3, 13, 8, 8, 1.5)),
            Stroke("M3 9V5.5A2.5 2.5 0 0 1 5.5 3h13A2.5 2.5 0 0 1 21 5.5v13a2.5 2.5 0 0 1-2.5 2.5H15"),
            Stroke("M13.5 10.5l4-4M13.5 6.5h4v4")
        },
        {
            Icon.Shrink,
            Stroke("M14 10l6.5-6.5M14 4.5V10h5.5M10 14l-6.5 6.5M4.5 14H10v5.5")
        },
        {
            Icon.Refresh,
            Stroke("M20.5 11A8.5 8.5 0 0 0 5.4 6.6L3.5 8.5M3.5 3.5v5h5"),
            Stroke("M3.5 13a8.5 8.5 0 0 0 15.1 4.4l1.9-1.9M20.5 20.5v-5h-5")
        },
        {
            Icon.Eye,
            Stroke("M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12z"),
            Stroke(Circle(12, 12, 3))
        },
        {
            Icon.Sparkles,
            Stroke(Sparkle(10, 13, 8)),
            Fill(Sparkle(18.5, 5, 3)),
            Fill(Sparkle(19.5, 18, 2))
        },
        {
            Icon.MagicWand,
            Stroke("M3 21 14 10"),
            Stroke(Sparkle(17.5, 6.5, 4)),
            Fill(Sparkle(8.5, 4.5, 2)),
            Fill(Sparkle(19.5, 16, 2))
        },
        {
            Icon.Stamp,
            Stroke("M5 21.5h14"),
            Stroke("M5 17.5a2 2 0 0 1 2-2h3.5V12a3.5 3.5 0 1 1 3 0v3.5H17a2 2 0 0 1 2 2V18.5H5z")
        },
        {
            Icon.Eraser,
            Stroke("M8.75 21 3.6 15.85a2 2 0 0 1 0-2.83l9.4-9.4a2 2 0 0 1 2.83 0l4.57 4.57a2 2 0 0 1 0 2.83L12.4 21"),
            Stroke("M21 21H8.75M7.5 9.5l7 7")
        },
        // A pen over the line it draws.
        {
            Icon.Annotate,
            Stroke("M3.5 18.5l.75-3.5L14.6 4.65a2.12 2.12 0 0 1 3 3L7.25 18z"),
            Stroke("M12.5 21h8")
        },
        {
            Icon.Clapperboard,
            Stroke("M3 10h18v8.5a2.5 2.5 0 0 1-2.5 2.5h-13A2.5 2.5 0 0 1 3 18.5z"),
            Stroke("M3 10V6.5A2.5 2.5 0 0 1 5.5 4h13A2.5 2.5 0 0 1 21 6.5V10"),
            Stroke("M8 4l2.5 6M13.5 4 16 10")
        },
        {
            Icon.Scissors,
            ScissorsShapes()
        },
        {
            Icon.ScissorsFrame,
            Stroke(Rectangle(2, 2, 20, 20, 3)),
            Stroke(Circle(8, 8.5, 2)),
            Stroke(Circle(8, 15.5, 2)),
            Stroke("M9.7 9.6 17 15.5M9.7 14.4 17 8.5")
        },
        {
            Icon.CirclePlay,
            Stroke(Circle(12, 12, 10)),
            Stroke("M10 8.75v6.5a.5.5 0 0 0 .77.42l4.9-3.25a.5.5 0 0 0 0-.84l-4.9-3.25a.5.5 0 0 0-.77.42z")
        },
        {
            Icon.Play,
            Stroke("M7 4.6v14.8a1 1 0 0 0 1.53.85l11.84-7.4a1 1 0 0 0 0-1.7L8.53 3.75A1 1 0 0 0 7 4.6z")
        },

        // Tools.
        {
            Icon.Wrench,
            Stroke(WrenchGeometry())
        },
        {
            Icon.Palette,
            Stroke("M12 2.5a9.5 9.5 0 0 0 0 19c1.15 0 2-.9 2-2 0-.5-.2-.95-.5-1.3a2 2 0 0 1 1.5-3.35h2.25A4.25 4.25 0 0 0 21.5 10.6C21.5 6 17.25 2.5 12 2.5z"),
            Fill(Circle(7.25, 11, 1.4)),
            Fill(Circle(10, 6.75, 1.4)),
            Fill(Circle(15, 7, 1.4))
        },
        {
            Icon.Pin,
            PinShapes()
        },
        {
            Icon.Unpin,
            Overlay(PinShapes(), Slash)
        },
        {
            Icon.Ruler,
            Stroke(Rotate(Rectangle(1, 8.5, 22, 7, 1.5), -45, 12, 12)),
            Stroke(Rotate(Path("M6 8.5v3M9.5 8.5v2M13 8.5v3M16.5 8.5v2"), -45, 12, 12))
        },
        {
            Icon.Robot,
            Stroke(Rectangle(4, 8, 16, 12, 3)),
            Stroke("M12 8V5"),
            Fill(Circle(12, 3.75, 1.5)),
            Fill(Circle(9, 13.5, 1.5)),
            Fill(Circle(15, 13.5, 1.5)),
            Stroke("M1.5 12.5v3M22.5 12.5v3")
        },
        {
            Icon.Hash,
            Stroke("M4 9h16M4 15h16M10 3 8 21M16 3l-2 18")
        },
        {
            Icon.Tag,
            Stroke("M2.5 4.5v6.17a2 2 0 0 0 .59 1.42l8.33 8.32a2 2 0 0 0 2.83 0l6.17-6.17a2 2 0 0 0 0-2.83L12.09 3.09A2 2 0 0 0 10.67 2.5H4.5a2 2 0 0 0-2 2z"),
            Fill(Circle(7.5, 7.5, 1.5))
        },
        {
            Icon.Search,
            Stroke(Circle(10.5, 10.5, 7)),
            Stroke("m20.5 20.5-5-5")
        },
        // The region brackets around a magnifier.
        {
            Icon.InspectWindow,
            Stroke(Brackets),
            Stroke(Circle(11, 11, 3.5)),
            Stroke("m13.75 13.75 2.75 2.75")
        },
        // A window without its frame.
        {
            Icon.BorderlessWindow,
            Stroke(Rectangle(2, 4, 20, 16, 2.5), dashes: [2, 3.3]),
            Stroke(Rectangle(6.5, 8.5, 11, 7, 1.5))
        },
        {
            Icon.Activity,
            Stroke("M2 12h4l3-8 6 16 3-8h4")
        },
        {
            Icon.Terminal,
            Stroke(Rectangle(2, 4, 20, 16, 2.5)),
            Stroke("m6.5 9.5 3 2.5-3 2.5M12.5 15h5")
        },

        // Settings and application.
        {
            Icon.Settings,
            Stroke(GearGeometry()),
            Stroke(Circle(12, 12, 3))
        },
        {
            Icon.Sliders,
            Stroke("M3 6h10M17 6h4M3 12h4M11 12h10M3 18h12M19 18h2"),
            Stroke(Circle(15, 6, 2)),
            Stroke(Circle(9, 12, 2)),
            Stroke(Circle(17, 18, 2))
        },
        {
            Icon.Keyboard,
            KeyboardShapes()
        },
        {
            Icon.KeyboardOff,
            Overlay(KeyboardShapes(), Slash)
        },
        {
            Icon.History,
            Stroke("M3.5 12A8.5 8.5 0 1 0 6 6L3.5 8.5M3.5 4v4.5H8"),
            Stroke("M12 7.5V12l3 2")
        },
        {
            Icon.BarChart,
            Stroke("M3 3v15.5A2.5 2.5 0 0 0 5.5 21H21"),
            Stroke("M8 17v-5M12.5 17V7M17 17v-8")
        },
        {
            Icon.Bug,
            Stroke(Rectangle(7, 8, 10, 13, 5)),
            Stroke("M9.5 8.5V8a2.5 2.5 0 0 1 5 0v.5"),
            Stroke("M12 12.5V21M7 13H3.5M7 17.5H4M17 13h3.5M17 17.5h3M8 4.5l1.5 1.5M16 4.5 14.5 6")
        },
        {
            Icon.Heart,
            Stroke("M12 20s-8.5-4.8-8.5-11A4.8 4.8 0 0 1 12 6.4 4.8 4.8 0 0 1 20.5 9c0 6.2-8.5 11-8.5 11z")
        },
        {
            Icon.Info,
            Stroke(Circle(12, 12, 10)),
            Stroke("M12 11v5.5"),
            Fill(Circle(12, 7.75, 1.25))
        },
        {
            Icon.Shield,
            Stroke(ShieldPath)
        },
        // A bar of buttons.
        {
            Icon.Toolbar,
            Stroke(Rectangle(2, 7, 20, 10, 3)),
            Fill(Circle(7, 12, 1.5)),
            Fill(Circle(12, 12, 1.5)),
            Fill(Circle(17, 12, 1.5))
        },
        {
            Icon.Menu,
            Stroke("M4 6h16M4 12h16M4 18h16")
        },
        {
            Icon.Exit,
            Stroke("M9 3H6a3 3 0 0 0-3 3v12a3 3 0 0 0 3 3h3"),
            Stroke("M16 7l5 5-5 5M21 12H9")
        },

        // States and actions.
        {
            Icon.Edit,
            Stroke("M4 20l.9-4.1L15.6 5.2a2.3 2.3 0 0 1 3.2 3.2L8.1 19.1z"),
            Stroke("M13.5 7.3l3.2 3.2")
        },
        {
            Icon.Check,
            Stroke("M4.5 12.5l5 5 10-10")
        },
        {
            Icon.CircleCheck,
            Stroke(Circle(12, 12, 10)),
            Stroke("m8 12.25 2.75 2.75L16 9.75")
        },
        {
            Icon.CircleX,
            Stroke(Circle(12, 12, 10)),
            Stroke("m9 9 6 6M15 9l-6 6")
        },
        {
            Icon.CircleStop,
            Stroke(Circle(12, 12, 10)),
            Fill(Rectangle(8.5, 8.5, 7, 7, 1.5))
        },
        {
            Icon.Circle,
            Stroke(Circle(12, 12, 8))
        },
        {
            Icon.Warning,
            Stroke("M10.27 3.95 2.4 17.6a2 2 0 0 0 1.73 3h15.74a2 2 0 0 0 1.73-3L13.73 3.95a2 2 0 0 0-3.46 0z"),
            Stroke("M12 9v4.5"),
            Fill(Circle(12, 16.75, 1.25))
        },
        {
            Icon.ClearList,
            Stroke("M3 6h18M3 12h12M3 18h8"),
            Stroke("m15.5 15.5 5 5M20.5 15.5l-5 5")
        },
        {
            Icon.ChevronRight,
            Stroke("m9 5 7 7-7 7")
        },

        {
            Icon.Sun,
            Stroke(Circle(12, 12, 4)),
            Stroke("M12 2v2.5M12 19.5V22M2 12h2.5M19.5 12H22M4.9 4.9l1.75 1.75M17.35 17.35l1.75 1.75M4.9 19.1l1.75-1.75M17.35 6.65l1.75-1.75")
        },
        {
            Icon.Close,
            Stroke("M6 6l12 12M18 6 6 18")
        }
    };

    // The title bar buttons: a 16x16 grid shown at 16px, with 1px lines on pixel centers and straight line ends on
    // pixel edges, so they stay sharp. The window buttons fill 3..13; the moon, sun and pin are about 11px.
    private static IconGroup CreateCaptionIcons() => new(16, IconStyle.Fine)
    {
        {
            Icon.CaptionMoon,
            Stroke("M7.88 2.5A5.5 5.5 0 1 0 13.5 8.12 4 4 0 0 1 7.88 2.5z")
        },
        {
            Icon.CaptionSun,
            Stroke(Circle(7.5, 7.5, 3)),
            Stroke("M7.5 1v2M7.5 12v2M1 7.5h2M12 7.5h2M3.25 3.25l1.25 1.25M10.5 10.5l1.25 1.25M3.25 11.75l1.25-1.25M10.5 4.5l1.25-1.25")
        },
        {
            Icon.CaptionPin,
            CaptionPinShapes()
        },
        {
            Icon.CaptionUnpin,
            Overlay(CaptionPinShapes(), Halo(CaptionSlash, 3), Stroke(CaptionSlash))
        },
        {
            Icon.CaptionMinimize,
            Stroke("M3 8.5h10")
        },
        {
            Icon.CaptionMaximize,
            SoftFrame(3, 3, 10, 10)
        },
        {
            Icon.CaptionRestore,
            [.. SoftFrame(3, 5, 8, 8), .. SoftFrame(5, 3, 8, 8, hidden: Rectangle(3, 5, 8, 8))]
        },
        {
            Icon.CaptionClose,
            Stroke("M3.5 3.5l9 9M12.5 3.5l-9 9")
        }
    };

    /// <summary><paramref name="shapes"/> with a gap cut around <paramref name="overlay"/>, which is drawn on top.</summary>
    private static IconShape[] Overlay(IconShape[] shapes, string overlay) =>
        Overlay(shapes, Halo(overlay, OverlayHalo), Stroke(overlay));

    /// <summary><paramref name="shapes"/> with the <paramref name="gap"/> area cut out, and <paramref name="overlay"/> drawn in it.</summary>
    private static IconShape[] Overlay(IconShape[] shapes, Geometry gap, params IconShape[] overlay) =>
        [.. shapes.Select(shape => shape.Except(gap)), .. overlay];

    private static IconShape[] PageShapes() => [Stroke(PagePath), Stroke(PageFold)];

    private static IconShape[] ClipboardShapes() => [Stroke(ClipboardBoard), Stroke(Rectangle(8, 2, 8, 4, 1))];

    private static IconShape[] ImageShapes() =>
    [
        Stroke(Rectangle(3, 3, 18, 18, 3)),
        Stroke("m3 16.5 5.5-5.5 9.5 9.5M14 16.5l3-3 4 4"),
        Fill(Circle(15.5, 8.5, 1.75))
    ];

    /// <summary><paramref name="shapes"/> with a round gap cut in the bottom-right corner, and <paramref name="badge"/> drawn in it.</summary>
    private static IconShape[] Badge(IconShape[] shapes, params IconShape[] badge) => Overlay(shapes, Circle(18, 18, BadgeGap), badge);

    private static IconShape[] CameraShapes() =>
    [
        Stroke("M2 9a3 3 0 0 1 3-3h2l1.55-2.3A1.5 1.5 0 0 1 9.8 3h4.4a1.5 1.5 0 0 1 1.25.7L17 6h2a3 3 0 0 1 3 3v9a3 3 0 0 1-3 3H5a3 3 0 0 1-3-3z"),
        Stroke(Circle(12, 13.5, 4))
    ];

    private static IconShape[] GlobeShapes() => [Stroke(Circle(12, 12, 10)), Stroke(Ellipse(7.5, 2, 9, 20)), Stroke("M2.5 12h19")];

    private static IconShape[] MonitorShapes() => [Stroke(Rectangle(2, 3, 20, 14, 2.5)), Stroke("M8 21h8M12 17v4")];

    private static IconShape[] WindowShapes() =>
    [
        Stroke(Rectangle(2, 4, 20, 16, 2.5)),
        Stroke("M2 9h20"),
        Fill(Circle(5.5, 6.5, 1)),
        Fill(Circle(8.5, 6.5, 1))
    ];

    private static IconShape[] FilmShapes() =>
    [
        Stroke(Rectangle(2, 4, 20, 16, 2.5)),
        Stroke(Rectangle(7.5, 8.5, 9, 7, 1)),
        Fill(Circle(4.75, 8, 0.9)),
        Fill(Circle(4.75, 12, 0.9)),
        Fill(Circle(4.75, 16, 0.9)),
        Fill(Circle(19.25, 8, 0.9)),
        Fill(Circle(19.25, 12, 0.9)),
        Fill(Circle(19.25, 16, 0.9))
    ];

    private static IconShape[] TimerShapes() =>
    [
        Stroke(Circle(12, 13.5, 8)),
        Stroke("M10 2.5h4M12 13.5V9.5M18.5 7l1.5-1.5")
    ];

    private static IconShape[] SaveShapes() =>
    [
        Stroke(FloppyPath),
        Stroke("M7 3v4a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V3"),
        Stroke("M7 21v-5a1 1 0 0 1 1-1h8a1 1 0 0 1 1 1v5")
    ];

    private static IconShape[] ScissorsShapes() =>
    [
        Stroke(Circle(6, 6.5, 3)),
        Stroke(Circle(6, 17.5, 3)),
        Stroke("M8.5 8.25 20.5 18M8.5 15.75 20.5 6")
    ];

    // A 1px outline filled on whole pixels, with its corner pixels drawn faint so the corners look slightly rounded
    // while the sides stay sharp. The parts under hidden, such as another frame in front, are left out.
    private static IconShape[] SoftFrame(double x, double y, double width, double height, Geometry? hidden = null)
    {
        Geometry corners = Union(Rectangle(x, y, 1, 1), Rectangle(x + width - 1, y, 1, 1),
            Rectangle(x, y + height - 1, 1, 1), Rectangle(x + width - 1, y + height - 1, 1, 1));
        Geometry sides = Subtract(Subtract(Rectangle(x, y, width, height), Rectangle(x + 1, y + 1, width - 2, height - 2)), corners);

        if (hidden != null)
        {
            sides = Subtract(sides, hidden);
            corners = Subtract(corners, hidden);
        }

        return [Fill(sides), Fill(corners, SoftCornerOpacity)];
    }

    private static IconShape[] CaptionPinShapes() =>
    [
        Stroke("M4 2.5h7M5.5 2.5v4l-2 2v1h8v-1l-2-2v-4"),
        Stroke("M7.5 10v4")
    ];

    private static IconShape[] PinShapes() =>
    [
        Stroke("M8.5 3h7M10 3v5.25l-3.35 3.6a1.5 1.5 0 0 0-.4 1.02v1.63h11.5v-1.63a1.5 1.5 0 0 0-.4-1.02L14 8.25V3"),
        Stroke("M12 14.5v7")
    ];

    private static IconShape[] KeyboardShapes() =>
    [
        Stroke(Rectangle(2, 5, 20, 14, 2.5)),
        Fill(Circle(6.5, 9.5, 1.1)),
        Fill(Circle(10.25, 9.5, 1.1)),
        Fill(Circle(13.75, 9.5, 1.1)),
        Fill(Circle(17.5, 9.5, 1.1)),
        Stroke("M7.5 14.5h9")
    ];

    // Three overlapping circles on a flat base.
    private static Geometry CloudGeometry() =>
        Union(Circle(7, 15, 4.5), Circle(12.5, 10.5, 5.5), Circle(17.5, 15, 4.5), Rectangle(7, 15, 10.5, 4.5));

    // A body with six teeth.
    private static Geometry GearGeometry() =>
        Union([Circle(12, 12, 7), .. Enumerable.Range(0, 6).Select(i => Rotate(Rectangle(10.25, 1.75, 3.5, 6, 1), i * 60, 12, 12))]);

    // A small cloud badge in the bottom-right corner.
    private static Geometry BadgeCloud() =>
        Union(Circle(16.25, 19.25, 2.25), Circle(18.75, 17.25, 2.75), Circle(20.5, 19.75, 1.75), Rectangle(16.25, 19.25, 4.25, 2.25));

    // A solid gear badge in the bottom-right corner, with a hole in the middle.
    private static Geometry BadgeGear() =>
        Subtract(
            Union([Circle(18, 18, 3.25), .. Enumerable.Range(0, 6).Select(i => Rotate(Rectangle(17, 12.75, 2, 3, 0.5), i * 60, 18, 18))]),
            Circle(18, 18, 1.25));

    // A round head with a slot, on a handle running down to the left.
    private static Geometry WrenchGeometry() =>
        Subtract(
            Union(Circle(16, 8, 5.5), Rotate(Rectangle(14, 8, 4, 14.5, 2), 45, 16, 8)),
            Rotate(Rectangle(14.25, 0, 3.5, 8, 1), 45, 16, 8));

    // A small folder, 9 wide and 8.5 tall.
    private static Geometry SmallFolder(double x, double y) => FormattedPath(
        $"M{x} {y + 1.5}a1.5 1.5 0 0 1 1.5-1.5h2l1.2 1.5h2.8a1.5 1.5 0 0 1 1.5 1.5v4a1.5 1.5 0 0 1-1.5 1.5h-6a1.5 1.5 0 0 1-1.5-1.5z");
}
