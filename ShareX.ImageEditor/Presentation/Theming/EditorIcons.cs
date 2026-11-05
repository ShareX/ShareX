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

using ShareX.Icons;
using ShareX.ImageEditor.Core.Annotations;

namespace ShareX.ImageEditor.Presentation.Theming
{
    public static class EditorIcons
    {
        public const string ToolSelect = ImageEditorIcons.Select;
        public const string ToolRectangle = ImageEditorIcons.Rectangle;
        public const string ToolEllipse = ImageEditorIcons.Ellipse;
        public const string ToolLine = ImageEditorIcons.Line;
        public const string ToolArrow = ImageEditorIcons.Arrow;
        public const string ToolFreehand = ImageEditorIcons.Freehand;
        public const string ToolHighlight = ImageEditorIcons.Highlight;
        public const string ToolText = ImageEditorIcons.Text;
        public const string ToolSpeechBalloon = ImageEditorIcons.SpeechBalloon;
        public const string ToolStep = ImageEditorIcons.Step;
        public const string ToolCursor = ImageEditorIcons.Cursor;
        public const string ToolBlur = ImageEditorIcons.Blur;
        public const string ToolPixelate = ImageEditorIcons.Pixelate;
        public const string ToolMagnify = ImageEditorIcons.Magnify;
        public const string ToolSpotlight = ImageEditorIcons.Spotlight;
        public const string ToolSmartEraser = ImageEditorIcons.SmartEraser;
        public const string ToolCrop = ImageEditorIcons.Crop;
        public const string ToolCutOut = ImageEditorIcons.CutOut;
        public const string ToolImage = ImageEditorIcons.Image;
        public const string ToolClone = ImageEditorIcons.Clone;
        public const string ToolEmoji = ImageEditorIcons.Emoji;
        public const string ToolEmojiSunglasses = ImageEditorIcons.EmojiSunglasses;

        public const string ActionUndo = AppIcons.Undo;
        public const string ActionRedo = AppIcons.Redo;
        public const string ActionDelete = AppIcons.Trash;
        public const string ActionClearAll = AppIcons.ClearAll;
        public const string ActionCut = AppIcons.Scissors;
        public const string ActionCopy = AppIcons.Clipboard;
        public const string ActionPaste = AppIcons.ClipboardPaste;
        public const string ActionDuplicate = AppIcons.Duplicate;
        public const string ActionBringToFront = AppIcons.BringToFront;
        public const string ActionBringForward = AppIcons.BringForward;
        public const string ActionSendBackward = AppIcons.SendBackward;
        public const string ActionSendToBack = AppIcons.SendToBack;
        public const string ActionSave = AppIcons.Save;
        public const string ActionSaveAs = AppIcons.SaveAs;
        public const string ActionPrint = AppIcons.Printer;
        public const string ActionDownload = AppIcons.Download;
        public const string ActionPinToScreen = AppIcons.Pin;
        public const string ActionUpload = AppIcons.CloudUpload;
        public const string ActionCancel = AppIcons.Close;
        public const string ActionContinue = AppIcons.Play;
        public const string ActionReset = AppIcons.Refresh;
        public const string ActionRotateLeft = AppIcons.RotateLeft;
        public const string ActionRotateRight = AppIcons.RotateRight;
        public const string ActionToggleToolbars = AppIcons.ChevronsUpDown;
        public const string Achievement = AppIcons.Trophy;

        public const string FormatBold = AppIcons.Bold;
        public const string FormatItalic = AppIcons.Italic;
        public const string MenuTheme = AppIcons.Theme;
        public const string ToggleSpeechBalloonTail = AppIcons.MessageCheck;
        public const string ToggleEllipseShape = ToolEllipse;
        public const string PanelBackground = AppIcons.Wallpaper;
        public const string PanelEffects = AppIcons.Sparkles;
        public const string LayerFlatten = AppIcons.Layers;
        public const string ChevronDown = AppIcons.ChevronDown;
        public const string Zoom = AppIcons.ZoomIn;
        public const string ScreenColorPicker = AppIcons.ScreenColorPicker;

        public const string FileMenu = AppIcons.File;
        public const string FileNew = AppIcons.FileAdd;
        public const string FileOpen = AppIcons.FolderOpen;
        public const string FileSave = AppIcons.Save;
        public const string FileSaveAs = AppIcons.SaveAs;
        public const string FileExit = AppIcons.Exit;

        public static string ForTool(EditorTool tool) => tool switch
        {
            EditorTool.Select => ToolSelect,
            EditorTool.Rectangle => ToolRectangle,
            EditorTool.Ellipse => ToolEllipse,
            EditorTool.Line => ToolLine,
            EditorTool.Arrow => ToolArrow,
            EditorTool.Freehand => ToolFreehand,
            EditorTool.Text => ToolText,
            EditorTool.SpeechBalloon => ToolSpeechBalloon,
            EditorTool.Step => ToolStep,
            EditorTool.Cursor => ToolCursor,
            EditorTool.Blur => ToolBlur,
            EditorTool.Pixelate => ToolPixelate,
            EditorTool.Magnify => ToolMagnify,
            EditorTool.Spotlight => ToolSpotlight,
            EditorTool.SmartEraser => ToolSmartEraser,
            EditorTool.Highlight => ToolHighlight,
            EditorTool.Crop => ToolCrop,
            EditorTool.CutOut => ToolCutOut,
            EditorTool.Image => ToolImage,
            EditorTool.Clone => ToolClone,
            EditorTool.Emoji => ToolEmoji,
            _ => ToolSelect
        };
    }
}
