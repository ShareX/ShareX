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

using ShareX.AvaloniaUI.Theming;
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

        public const string ActionUndo = LucideIcons.undo_2;
        public const string ActionRedo = LucideIcons.redo_2;
        public const string ActionDelete = LucideIcons.trash_2;
        public const string ActionClearAll = LucideIcons.brush_cleaning;
        public const string ActionCut = LucideIcons.scissors;
        public const string ActionCopy = LucideIcons.clipboard;
        public const string ActionPaste = LucideIcons.clipboard_paste;
        public const string ActionDuplicate = LucideIcons.copy_plus;
        public const string ActionBringToFront = LucideIcons.bring_to_front;
        public const string ActionBringForward = LucideIcons.move_up;
        public const string ActionSendBackward = LucideIcons.move_down;
        public const string ActionSendToBack = LucideIcons.send_to_back;
        public const string ActionSave = LucideIcons.save;
        public const string ActionSaveAs = LucideIcons.save_all;
        public const string ActionPrint = LucideIcons.printer;
        public const string ActionDownload = LucideIcons.download;
        public const string ActionPinToScreen = LucideIcons.pin;
        public const string ActionUpload = LucideIcons.cloud_upload;
        public const string ActionCancel = LucideIcons.x;
        public const string ActionContinue = LucideIcons.play;
        public const string ActionReset = LucideIcons.refresh_cw;
        public const string ActionRotateLeft = LucideIcons.rotate_ccw_square;
        public const string ActionRotateRight = LucideIcons.rotate_cw_square;
        public const string ActionToggleToolbars = LucideIcons.chevrons_up_down;
        public const string Achievement = LucideIcons.trophy;

        public const string FormatBold = LucideIcons.bold;
        public const string FormatItalic = LucideIcons.italic;
        public const string MenuTheme = LucideIcons.moon_star;
        public const string ToggleSpeechBalloonTail = LucideIcons.message_square_check;
        public const string ToggleEllipseShape = ToolEllipse;
        public const string PanelBackground = LucideIcons.wallpaper;
        public const string PanelEffects = LucideIcons.sparkles;
        public const string LayerFlatten = LucideIcons.layers_2;
        public const string ChevronDown = LucideIcons.chevron_down;
        public const string Zoom = LucideIcons.zoom_in;
        public const string ScreenColorPicker = LucideIcons.pipette;

        public const string FileMenu = LucideIcons.file;
        public const string FileNew = LucideIcons.file_plus;
        public const string FileOpen = LucideIcons.folder_open;
        public const string FileSave = LucideIcons.save;
        public const string FileSaveAs = LucideIcons.save_all;
        public const string FileExit = LucideIcons.log_out;

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
