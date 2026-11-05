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

namespace ShareX.Icons;

/// <summary>
/// Keys of the main window icons: its menus, thumbnail and context menus, and title bar. They are drawn in the
/// same style as the <see cref="ImageEditorIcons"/>; see <see cref="AppIconDefinitions"/>.
/// </summary>
public static class AppIcons
{
    private const string Prefix = "app/";

    // Menu items with an icon of their own.
    public const string AfterCaptureTasks = Prefix + "after-capture-tasks";
    public const string AfterUploadTasks = Prefix + "after-upload-tasks";
    public const string Destinations = Prefix + "destinations";
    public const string DestinationSettings = Prefix + "destination-settings";
    public const string CustomUploaderSettings = Prefix + "custom-uploader-settings";
    public const string ScreenshotsFolder = Prefix + "screenshots-folder";
    public const string ImageHistory = Prefix + "image-history";
    public const string Follow = Prefix + "follow";
    public const string Discord = Prefix + "discord";
    public const string RestartAsAdmin = Prefix + "restart-as-admin";
    public const string RecentItems = Prefix + "recent-items";
    public const string ScreenRecording = Prefix + "screen-recording";
    public const string AutoCapture = Prefix + "auto-capture";
    public const string DragDropUpload = Prefix + "drag-drop-upload";
    public const string UploadClipboard = Prefix + "upload-clipboard";
    public const string MouseHighlighter = Prefix + "mouse-highlighter";
    public const string ScreenColorPicker = Prefix + "screen-color-picker";
    public const string BackgroundRemover = Prefix + "background-remover";
    public const string IconConverter = Prefix + "icon-converter";
    public const string ImageBeautifier = Prefix + "image-beautifier";
    public const string ImageComparer = Prefix + "image-comparer";
    public const string ImageConverter = Prefix + "image-converter";
    public const string ImageEditor = Prefix + "image-editor";
    public const string ImageThumbnailer = Prefix + "image-thumbnailer";
    public const string AnimatedGifMaker = Prefix + "animated-gif-maker";
    public const string VideoConverter = Prefix + "video-converter";
    public const string RemoteStorageBrowser = Prefix + "remote-storage-browser";
    public const string ClipboardViewer = Prefix + "clipboard-viewer";
    public const string MonitorTest = Prefix + "monitor-test";
    public const string ClipboardImage = Prefix + "clipboard-image";
    public const string ClipboardLink = Prefix + "clipboard-link";

    // Capture.
    public const string Camera = Prefix + "camera";
    public const string Fullscreen = Prefix + "fullscreen";
    public const string Window = Prefix + "window";
    public const string Monitor = Prefix + "monitor";
    public const string Region = Prefix + "region";
    public const string LastRegion = Prefix + "last-region";
    public const string Video = Prefix + "video";
    public const string Film = Prefix + "film";
    public const string ScrollingCapture = Prefix + "scrolling-capture";
    public const string Clock = Prefix + "clock";
    public const string Pointer = Prefix + "pointer";
    public const string Timer = Prefix + "timer";
    public const string TimerOff = Prefix + "timer-off";

    // Upload, links and sharing.
    public const string Upload = Prefix + "upload";
    public const string Download = Prefix + "download";
    public const string CloudUpload = Prefix + "cloud-upload";
    public const string CloudOff = Prefix + "cloud-off";
    public const string Link = Prefix + "link";
    public const string ShortLink = Prefix + "short-link";
    public const string ExternalLink = Prefix + "external-link";
    public const string Share = Prefix + "share";
    public const string QrCode = Prefix + "qr-code";

    // Files and folders.
    public const string File = Prefix + "file";
    public const string FileText = Prefix + "file-text";
    public const string FileImage = Prefix + "file-image";
    public const string FileVideo = Prefix + "file-video";
    public const string FileCode = Prefix + "file-code";
    public const string FileUpload = Prefix + "file-upload";
    public const string FileDelete = Prefix + "file-delete";
    public const string Files = Prefix + "files";
    public const string Folder = Prefix + "folder";
    public const string FolderOpen = Prefix + "folder-open";
    public const string FolderUpload = Prefix + "folder-upload";
    public const string FolderBookmark = Prefix + "folder-bookmark";
    public const string FolderTree = Prefix + "folder-tree";
    public const string FilePath = Prefix + "file-path";
    public const string Save = Prefix + "save";
    public const string SaveAs = Prefix + "save-as";
    public const string Printer = Prefix + "printer";
    public const string Trash = Prefix + "trash";

    // Clipboard and text.
    public const string Clipboard = Prefix + "clipboard";
    public const string ClipboardCopy = Prefix + "clipboard-copy";
    public const string ClipboardList = Prefix + "clipboard-list";
    public const string Copy = Prefix + "copy";
    public const string Code = Prefix + "code";
    public const string Braces = Prefix + "braces";
    public const string Message = Prefix + "message";
    public const string Messages = Prefix + "messages";
    public const string MessageShare = Prefix + "message-share";
    public const string TextRecognition = Prefix + "text-recognition";

    // Images and media.
    public const string Image = Prefix + "image";
    public const string Images = Prefix + "images";
    public const string ImageUpload = Prefix + "image-upload";
    public const string ImageDownload = Prefix + "image-download";
    public const string Layers = Prefix + "layers";
    public const string Combine = Prefix + "combine";
    public const string Split = Prefix + "split";
    public const string Rows = Prefix + "rows";
    public const string Columns = Prefix + "columns";
    public const string Resize = Prefix + "resize";
    public const string Shrink = Prefix + "shrink";
    public const string Refresh = Prefix + "refresh";
    public const string Eye = Prefix + "eye";
    public const string Sparkles = Prefix + "sparkles";
    public const string MagicWand = Prefix + "magic-wand";
    public const string Stamp = Prefix + "stamp";
    public const string Eraser = Prefix + "eraser";
    public const string Annotate = Prefix + "annotate";
    public const string Clapperboard = Prefix + "clapperboard";
    public const string Scissors = Prefix + "scissors";
    public const string ScissorsFrame = Prefix + "scissors-frame";
    public const string CirclePlay = Prefix + "circle-play";
    public const string Play = Prefix + "play";

    // Tools.
    public const string Wrench = Prefix + "wrench";
    public const string Palette = Prefix + "palette";
    public const string Pin = Prefix + "pin";
    public const string Unpin = Prefix + "unpin";
    public const string Ruler = Prefix + "ruler";
    public const string Robot = Prefix + "robot";
    public const string Hash = Prefix + "hash";
    public const string Tag = Prefix + "tag";
    public const string Search = Prefix + "search";
    public const string InspectWindow = Prefix + "inspect-window";
    public const string BorderlessWindow = Prefix + "borderless-window";
    public const string Activity = Prefix + "activity";
    public const string Terminal = Prefix + "terminal";

    // Settings and application.
    public const string Settings = Prefix + "settings";
    public const string Sliders = Prefix + "sliders";
    public const string Keyboard = Prefix + "keyboard";
    public const string KeyboardOff = Prefix + "keyboard-off";
    public const string Workflows = Prefix + "workflows";
    public const string History = Prefix + "history";
    public const string BarChart = Prefix + "bar-chart";
    public const string Bug = Prefix + "bug";
    public const string Heart = Prefix + "heart";
    public const string Info = Prefix + "info";
    public const string Shield = Prefix + "shield";
    public const string Toolbar = Prefix + "toolbar";
    public const string Menu = Prefix + "menu";
    public const string Exit = Prefix + "exit";

    // States and actions.
    public const string Edit = Prefix + "edit";
    public const string Check = Prefix + "check";
    public const string CircleCheck = Prefix + "circle-check";
    public const string CircleX = Prefix + "circle-x";
    public const string CircleStop = Prefix + "circle-stop";
    public const string Circle = Prefix + "circle";
    public const string Warning = Prefix + "warning";
    public const string ClearList = Prefix + "clear-list";
    public const string ChevronRight = Prefix + "chevron-right";

    public const string Sun = Prefix + "sun";
    public const string Close = Prefix + "close";

    // Title bar buttons, drawn on a 16px pixel grid to be shown at 16px.
    public const string CaptionMoon = Prefix + "caption-moon";
    public const string CaptionSun = Prefix + "caption-sun";
    public const string CaptionPin = Prefix + "caption-pin";
    public const string CaptionUnpin = Prefix + "caption-unpin";
    public const string CaptionMinimize = Prefix + "caption-minimize";
    public const string CaptionMaximize = Prefix + "caption-maximize";
    public const string CaptionRestore = Prefix + "caption-restore";
    public const string CaptionClose = Prefix + "caption-close";

    // Controls and actions.
    public const string Plus = Prefix + "plus";
    public const string Minus = Prefix + "minus";
    public const string ChevronDown = Prefix + "chevron-down";
    public const string ChevronUp = Prefix + "chevron-up";
    public const string ChevronLeft = Prefix + "chevron-left";
    public const string ChevronsUpDown = Prefix + "chevrons-up-down";
    public const string ArrowUp = Prefix + "arrow-up";
    public const string ArrowDown = Prefix + "arrow-down";
    public const string ArrowRight = Prefix + "arrow-right";
    public const string ArrowRightToLine = Prefix + "arrow-right-to-line";
    public const string ArrowLeftToLine = Prefix + "arrow-left-to-line";
    public const string ArrowRightFromLine = Prefix + "arrow-right-from-line";
    public const string ArrowDownToLine = Prefix + "arrow-down-to-line";
    public const string Pause = Prefix + "pause";
    public const string Stop = Prefix + "stop";
    public const string CirclePause = Prefix + "circle-pause";
    public const string StepForward = Prefix + "step-forward";
    public const string StepBack = Prefix + "step-back";
    public const string Reset = Prefix + "reset";
    public const string Undo = Prefix + "undo";
    public const string Redo = Prefix + "redo";
    public const string Star = Prefix + "star";
    public const string Crop = Prefix + "crop";
    public const string FolderSearch = Prefix + "folder-search";
    public const string Zap = Prefix + "zap";
    public const string ClipboardPaste = Prefix + "clipboard-paste";
    public const string ScanLine = Prefix + "scan-line";
    public const string Help = Prefix + "help";
    public const string Expand = Prefix + "expand";
    public const string Loading = Prefix + "loading";
    public const string Language = Prefix + "language";
    public const string Filter = Prefix + "filter";
    public const string Duplicate = Prefix + "duplicate";
    public const string CircleDashed = Prefix + "circle-dashed";
    public const string Bell = Prefix + "bell";
    public const string Volume = Prefix + "volume";
    public const string Mute = Prefix + "mute";
    public const string VideoOff = Prefix + "video-off";
    public const string AlignLeft = Prefix + "align-left";
    public const string AlignCenter = Prefix + "align-center";
    public const string AlignRight = Prefix + "align-right";
    public const string Flask = Prefix + "flask";
    public const string Server = Prefix + "server";
    public const string Plug = Prefix + "plug";
    public const string Network = Prefix + "network";
    public const string Grid = Prefix + "grid";
    public const string FileEdit = Prefix + "file-edit";
    public const string Factory = Prefix + "factory";
    public const string Database = Prefix + "database";
    public const string DatabaseBackup = Prefix + "database-backup";
    public const string ZoomIn = Prefix + "zoom-in";
    public const string Wallpaper = Prefix + "wallpaper";
    public const string Trophy = Prefix + "trophy";
    public const string Target = Prefix + "target";
    public const string Swatches = Prefix + "swatches";
    public const string Theme = Prefix + "theme";
    public const string BringToFront = Prefix + "bring-to-front";
    public const string SendToBack = Prefix + "send-to-back";
    public const string BringForward = Prefix + "bring-forward";
    public const string SendBackward = Prefix + "send-backward";
    public const string Send = Prefix + "send";
    public const string Reply = Prefix + "reply";
    public const string SearchCheck = Prefix + "search-check";
    public const string QrScan = Prefix + "qr-scan";
    public const string ScanEye = Prefix + "scan-eye";
    public const string Satellite = Prefix + "satellite";
    public const string RotateLeft = Prefix + "rotate-left";
    public const string RotateRight = Prefix + "rotate-right";
    public const string Rocket = Prefix + "rocket";
    public const string PictureInPicture = Prefix + "picture-in-picture";
    public const string Package = Prefix + "package";
    public const string MoveHorizontal = Prefix + "move-horizontal";
    public const string Mouse = Prefix + "mouse";
    public const string Microphone = Prefix + "microphone";
    public const string MessageCheck = Prefix + "message-check";
    public const string Locate = Prefix + "locate";
    public const string ListRestart = Prefix + "list-restart";
    public const string Lightbulb = Prefix + "lightbulb";
    public const string Dashboard = Prefix + "dashboard";
    public const string Landmark = Prefix + "landmark";
    public const string Key = Prefix + "key";
    public const string Bold = Prefix + "bold";
    public const string Italic = Prefix + "italic";
    public const string ImageAdd = Prefix + "image-add";
    public const string Grip = Prefix + "grip";
    public const string FolderAdd = Prefix + "folder-add";
    public const string FileSearch = Prefix + "file-search";
    public const string FileAdd = Prefix + "file-add";
    public const string FileJson = Prefix + "file-json";
    public const string FileDownload = Prefix + "file-download";
    public const string Crosshair = Prefix + "crosshair";
    public const string Contrast = Prefix + "contrast";
    public const string Cloud = Prefix + "cloud";
    public const string ClipboardClear = Prefix + "clipboard-clear";
    public const string LineChart = Prefix + "line-chart";
    public const string ClearAll = Prefix + "clear-all";
    public const string Blocks = Prefix + "blocks";
    public const string Blend = Prefix + "blend";
    public const string Audio = Prefix + "audio";
    public const string PointerClick = Prefix + "pointer-click";
    public const string Lock = Prefix + "lock";
}
