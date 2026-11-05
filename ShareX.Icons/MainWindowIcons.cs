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
/// same style as the <see cref="ImageEditorIcons"/>; see <see cref="MainWindowIconDefinitions"/>.
/// </summary>
public static class MainWindowIcons
{
    private const string Prefix = "main-window/";

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
    public const string Cloud = Prefix + "cloud";
    public const string CloudUpload = Prefix + "cloud-upload";
    public const string CloudSettings = Prefix + "cloud-settings";
    public const string CloudOff = Prefix + "cloud-off";
    public const string Server = Prefix + "server";
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
    public const string ChatBubble = Prefix + "chat-bubble";
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
    public const string Convert = Prefix + "convert";
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
    public const string Eyedropper = Prefix + "eyedropper";
    public const string PointerClick = Prefix + "pointer-click";
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

    // Title bar.
    public const string Moon = Prefix + "moon";
    public const string Sun = Prefix + "sun";
    public const string Minimize = Prefix + "minimize";
    public const string Maximize = Prefix + "maximize";
    public const string Restore = Prefix + "restore";
    public const string Close = Prefix + "close";
}
