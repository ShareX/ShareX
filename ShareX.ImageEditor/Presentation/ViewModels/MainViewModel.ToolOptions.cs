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
using CommunityToolkit.Mvvm.ComponentModel;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using System.Reflection;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private static EditorTool? _sessionLastUsedAnnotationTool;
        private const string DefaultAnnotationFontFamily = "Segoe UI";
        private const string SpotlightBlurOptionPropertyName = "SpotlightBlur";
        private static readonly IReadOnlyList<string> _availableFontFamilies = BuildAvailableFontFamilies();
        private static readonly IReadOnlyList<TextHorizontalAlignment> _availableTextHorizontalAlignments = BuildAvailableTextHorizontalAlignments();
        private static readonly IReadOnlyList<BorderStyle> _availableBorderStyles = BuildAvailableBorderStyles();
        private static readonly IReadOnlyList<ArrowStyle> _availableArrowStyles = BuildAvailableArrowStyles();
        private static readonly IReadOnlyList<CursorType> _availableCursorTypes = BuildAvailableCursorTypes();
        private static readonly IReadOnlyList<int> _availableStepStartNumbers = Enumerable.Range(1, 10).ToArray();
        private static readonly IReadOnlyList<StepType> _availableStepTypes = BuildAvailableStepTypes();
        private static readonly PropertyInfo? _spotlightBlurOptionProperty = typeof(ImageEditorOptions).GetProperty(SpotlightBlurOptionPropertyName);

        public IReadOnlyList<string> AvailableFontFamilies => _availableFontFamilies;
        public IReadOnlyList<TextHorizontalAlignment> AvailableTextHorizontalAlignments => _availableTextHorizontalAlignments;
        public IReadOnlyList<BorderStyle> AvailableBorderStyles => _availableBorderStyles;
        public IReadOnlyList<ArrowStyle> AvailableArrowStyles => _availableArrowStyles;
        public IReadOnlyList<CursorType> AvailableCursorTypes => _availableCursorTypes;
        public IReadOnlyList<int> AvailableStepStartNumbers => _availableStepStartNumbers;
        public IReadOnlyList<StepType> AvailableStepTypes => _availableStepTypes;

        [ObservableProperty]
        private string _selectedColor = "#EF4444";

        // Add a brush version for the dropdown control
        public IBrush SelectedColorBrush
        {
            get => new SolidColorBrush(Color.Parse(SelectedColor));
            set
            {
                if (value is SolidColorBrush solidBrush)
                {
                    SelectedColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                }
            }
        }

        // Color value for Avalonia ColorPicker binding
        public Color SelectedColorValue
        {
            get => Color.Parse(SelectedColor);
            set => SelectedColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        partial void OnSelectedColorChanged(string value)
        {
            OnPropertyChanged(nameof(SelectedColorBrush));
            OnPropertyChanged(nameof(SelectedColorValue));
            UpdateOptionsFromSelectedColor();
        }

        private void UpdateOptionsFromSelectedColor()
        {
            var color = SelectedColorValue;
            switch (ActiveTool)
            {
                case EditorTool.Select:
                    if (SelectedAnnotation != null)
                    {
                        // TODO: Update SelectedAnnotation color if needed
                    }
                    else
                    {
                        // Fallback to update generic options if no annotation is selected but tool is active?
                        // Or just update default options.
                        UpdateDefaultOptionsColor(color);
                    }
                    break;
                default:
                    UpdateDefaultOptionsColor(color);
                    break;
            }
        }

        private void UpdateDefaultOptionsColor(Color color)
        {
            switch (ActiveTool)
            {
                case EditorTool.Step:
                    Options.StepBorderColor = color;
                    break;
                case EditorTool.SpeechBalloon:
                    Options.SpeechBalloonBorderColor = color;
                    break;
                case EditorTool.Text:
                    Options.TextBorderColor = color;
                    break;
                default:
                    Options.BorderColor = color;
                    break;
            }
        }

        [ObservableProperty]
        private int _strokeWidth = 4;

        partial void OnStrokeWidthChanged(int value)
        {
            if (ActiveTool == EditorTool.Step)
            {
                Options.StepThickness = value;
            }
            else if (ActiveTool == EditorTool.SpeechBalloon)
            {
                Options.SpeechBalloonThickness = value;
            }
            else if (ActiveTool == EditorTool.Text)
            {
                Options.TextThickness = value;
            }
            else if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                if (SelectedAnnotation is NumberAnnotation) Options.StepThickness = value;
                else if (SelectedAnnotation is SpeechBalloonAnnotation) Options.SpeechBalloonThickness = value;
                else if (SelectedAnnotation is TextAnnotation) Options.TextThickness = value;
                else if (SelectedAnnotation is SmartEraserAnnotation) return;
                else Options.Thickness = value;
            }
            else
            {
                Options.Thickness = value;
            }
        }

        [ObservableProperty]
        private int _cornerRadius = 4;

        partial void OnCornerRadiusChanged(int value)
        {
            int clamped = Math.Max(0, value);
            if (clamped != value)
            {
                CornerRadius = clamped;
                return;
            }

            bool appliesToCornerRadius = ActiveTool is EditorTool.Rectangle or EditorTool.SpeechBalloon;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                appliesToCornerRadius = SelectedAnnotation is RectangleAnnotation or SpeechBalloonAnnotation;
            }

            if (appliesToCornerRadius)
            {
                Options.CornerRadius = value;
            }
        }

        // Tool-specific options
        [ObservableProperty]
        private string _fillColor = "#00000000"; // Transparent by default

        // Add a brush version for the fill color dropdown control
        public IBrush FillColorBrush
        {
            get => new SolidColorBrush(Color.Parse(FillColor));
            set
            {
                if (value is SolidColorBrush solidBrush)
                {
                    FillColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                }
            }
        }

        // Color value for Avalonia ColorPicker binding
        public Color FillColorValue
        {
            get => Color.Parse(FillColor);
            set => FillColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        partial void OnFillColorChanged(string value)
        {
            OnPropertyChanged(nameof(FillColorBrush));
            OnPropertyChanged(nameof(FillColorValue));
            UpdateOptionsFromFillColor();
        }

        private void UpdateOptionsFromFillColor()
        {
            var color = FillColorValue;
            switch (ActiveTool)
            {
                case EditorTool.Step:
                    Options.StepFillColor = color;
                    break;
                case EditorTool.SpeechBalloon:
                    Options.SpeechBalloonFillColor = color;
                    break;
                case EditorTool.Highlight:
                    Options.HighlightFillColor = color;
                    break;
                default:
                    Options.FillColor = color;
                    break;
            }
        }

        [ObservableProperty]
        private string _textColor = "#FF000000"; // Black by default

        public IBrush TextColorBrush
        {
            get => new SolidColorBrush(Color.Parse(TextColor));
            set
            {
                if (value is SolidColorBrush solidBrush)
                {
                    TextColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                }
            }
        }

        public Color TextColorValue
        {
            get => Color.Parse(TextColor);
            set => TextColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        partial void OnTextColorChanged(string value)
        {
            OnPropertyChanged(nameof(TextColorBrush));
            OnPropertyChanged(nameof(TextColorValue));
            UpdateOptionsFromTextColor();
        }

        private void UpdateOptionsFromTextColor()
        {
            var color = TextColorValue;

            // Determine which option to update based on active tool or selected annotation
            if (ActiveTool == EditorTool.Step)
            {
                Options.StepTextColor = color;
            }
            else if (ActiveTool == EditorTool.SpeechBalloon)
            {
                Options.SpeechBalloonTextColor = color;
            }
            else if (ActiveTool == EditorTool.Text)
            {
                Options.TextTextColor = color;
            }
            else if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                if (SelectedAnnotation is NumberAnnotation)
                {
                    Options.StepTextColor = color;
                }
                else if (SelectedAnnotation is SpeechBalloonAnnotation)
                {
                    Options.SpeechBalloonTextColor = color;
                }
                else if (SelectedAnnotation is TextAnnotation)
                {
                    Options.TextTextColor = color;
                }
            }
        }

        [ObservableProperty]
        private float _fontSize = 30;

        [ObservableProperty]
        private int _stepStartNumber = 1;

        [ObservableProperty]
        private StepType _selectedStepType = StepType.Numeric;

        [ObservableProperty]
        private TextHorizontalAlignment _selectedTextHorizontalAlignment = TextHorizontalAlignment.Center;

        partial void OnStepStartNumberChanged(int value)
        {
            int clamped = Math.Clamp(value, 1, 10);
            if (clamped != value)
            {
                StepStartNumber = clamped;
                return;
            }

            NumberCounter = clamped;
        }

        partial void OnSelectedStepTypeChanged(StepType value)
        {
            StepType normalizedStepType = NormalizeStepType(value);
            if (normalizedStepType != value)
            {
                SelectedStepType = normalizedStepType;
                return;
            }

            Options.StepType = normalizedStepType;
        }

        partial void OnSelectedTextHorizontalAlignmentChanged(TextHorizontalAlignment value)
        {
            TextHorizontalAlignment normalizedAlignment = NormalizeTextHorizontalAlignment(value);
            if (normalizedAlignment != value)
            {
                SelectedTextHorizontalAlignment = normalizedAlignment;
                return;
            }

            bool isText = ActiveTool == EditorTool.Text;
            bool isSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isText = SelectedAnnotation is TextAnnotation;
                isSpeechBalloon = SelectedAnnotation is SpeechBalloonAnnotation;
            }

            if (isText)
            {
                Options.TextHorizontalAlignment = normalizedAlignment;
            }
            else if (isSpeechBalloon)
            {
                Options.SpeechBalloonTextHorizontalAlignment = normalizedAlignment;
            }
        }

        partial void OnFontSizeChanged(float value)
        {
            bool isStep = ActiveTool == EditorTool.Step;
            bool isSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                if (SelectedAnnotation is NumberAnnotation) isStep = true;
                if (SelectedAnnotation is SpeechBalloonAnnotation) isSpeechBalloon = true;
            }

            if (isStep)
            {
                Options.StepFontSize = value;
            }
            else if (isSpeechBalloon)
            {
                Options.SpeechBalloonFontSize = value;
            }
            else
            {
                Options.TextFontSize = value;
            }
        }

        [ObservableProperty]
        private string _selectedFontFamily = DefaultAnnotationFontFamily;

        [ObservableProperty]
        private BorderStyle _selectedBorderStyle = BorderStyle.Solid;

        [ObservableProperty]
        private ArrowStyle _selectedArrowStyle = ArrowStyle.Classic;

        [ObservableProperty]
        private CursorType _selectedCursorType = CursorType.Default;

        partial void OnSelectedFontFamilyChanged(string value)
        {
            string normalizedFontFamily = NormalizeFontFamily(value);
            if (!string.Equals(normalizedFontFamily, value, StringComparison.Ordinal))
            {
                SelectedFontFamily = normalizedFontFamily;
                return;
            }

            bool isText = ActiveTool == EditorTool.Text;
            bool isSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isText = SelectedAnnotation is TextAnnotation;
                isSpeechBalloon = SelectedAnnotation is SpeechBalloonAnnotation;
            }

            if (isText)
            {
                Options.TextFontFamily = normalizedFontFamily;
            }
            else if (isSpeechBalloon)
            {
                Options.SpeechBalloonFontFamily = normalizedFontFamily;
            }
        }

        partial void OnSelectedBorderStyleChanged(BorderStyle value)
        {
            BorderStyle normalizedBorderStyle = NormalizeBorderStyle(value);
            if (normalizedBorderStyle != value)
            {
                SelectedBorderStyle = normalizedBorderStyle;
                return;
            }

            bool supportsBorderStyle = ActiveTool is EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Freehand;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                supportsBorderStyle = SelectedAnnotation.ToolType is EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Freehand;
            }

            if (supportsBorderStyle)
            {
                Options.BorderStyle = normalizedBorderStyle;
            }
        }

        partial void OnSelectedArrowStyleChanged(ArrowStyle value)
        {
            ArrowStyle normalizedArrowStyle = NormalizeArrowStyle(value);
            if (normalizedArrowStyle != value)
            {
                SelectedArrowStyle = normalizedArrowStyle;
                return;
            }

            bool isArrow = ActiveTool == EditorTool.Arrow;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isArrow = SelectedAnnotation is ArrowAnnotation;
            }

            if (isArrow)
            {
                Options.ArrowStyle = normalizedArrowStyle;
            }
        }

        partial void OnSelectedCursorTypeChanged(CursorType value)
        {
            CursorType normalizedCursorType = NormalizeCursorType(value);
            if (normalizedCursorType != value)
            {
                SelectedCursorType = normalizedCursorType;
                return;
            }

            bool isCursor = ActiveTool == EditorTool.Cursor;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isCursor = SelectedAnnotation is CursorAnnotation;
            }

            if (isCursor)
            {
                Options.CursorType = normalizedCursorType;
            }
        }

        private bool _isLoadingOptions;

        [ObservableProperty]
        private float _effectStrength = 1;

        private const float MinEffectStrength = 1;
        private const float MaxBlurStrength = 200;
        private const float MaxPixelateStrength = 200;
        private const float MaxMagnifyStrength = 10;
        private const float MaxSpotlightStrength = 100;

        public static float GetMaxEffectStrength(EditorTool tool) => tool switch
        {
            EditorTool.Blur => MaxBlurStrength,
            EditorTool.Pixelate => MaxPixelateStrength,
            EditorTool.Magnify => MaxMagnifyStrength,
            EditorTool.Spotlight => MaxSpotlightStrength,
            _ => 30
        };

        private EditorTool GetEffectiveStrengthTool()
        {
            if (ActiveTool == EditorTool.Select && _selectedAnnotation != null)
            {
                return _selectedAnnotation.ToolType;
            }

            return ActiveTool;
        }

        public float EffectStrengthMaximum => GetMaxEffectStrength(GetEffectiveStrengthTool());

        public float SpotlightBlurMaximum => MaxBlurStrength;

        partial void OnEffectStrengthChanged(float value)
        {
            var clamped = Math.Clamp(value, MinEffectStrength, EffectStrengthMaximum);
            if (Math.Abs(clamped - value) > float.Epsilon)
            {
                EffectStrength = clamped;
                return;
            }

            // Don't write back to Options while we're loading values for a tool switch,
            // otherwise the clamped stale value overwrites the correct stored option.
            if (_isLoadingOptions)
            {
                return;
            }

            switch (GetEffectiveStrengthTool())
            {
                case EditorTool.Blur:
                    Options.BlurStrength = value;
                    break;
                case EditorTool.Pixelate:
                    Options.PixelateStrength = value;
                    break;
                case EditorTool.Magnify:
                    Options.MagnifierStrength = value;
                    break;
                case EditorTool.Spotlight:
                    Options.SpotlightStrength = value;
                    break;
            }
        }

        [ObservableProperty]
        private float _spotlightBlur;

        partial void OnSpotlightBlurChanged(float value)
        {
            float clamped = Math.Clamp(value, 0, SpotlightBlurMaximum);
            if (Math.Abs(clamped - value) > float.Epsilon)
            {
                SpotlightBlur = clamped;
                return;
            }

            if (_isLoadingOptions)
            {
                return;
            }

            bool isSpotlight = ActiveTool == EditorTool.Spotlight;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation is SpotlightAnnotation)
            {
                isSpotlight = true;
            }

            if (isSpotlight)
            {
                SetSpotlightBlurOption(value);
            }
        }

        private float GetSpotlightBlurOption()
        {
            object? value = _spotlightBlurOptionProperty?.GetValue(Options);

            return value switch
            {
                float floatValue => floatValue,
                double doubleValue => (float)doubleValue,
                int intValue => intValue,
                _ => 0
            };
        }

        private void SetSpotlightBlurOption(float value)
        {
            if (_spotlightBlurOptionProperty == null || !_spotlightBlurOptionProperty.CanWrite)
            {
                return;
            }

            if (_spotlightBlurOptionProperty.PropertyType == typeof(float))
            {
                _spotlightBlurOptionProperty.SetValue(Options, value);
            }
            else if (_spotlightBlurOptionProperty.PropertyType == typeof(double))
            {
                _spotlightBlurOptionProperty.SetValue(Options, (double)value);
            }
            else if (_spotlightBlurOptionProperty.PropertyType == typeof(int))
            {
                _spotlightBlurOptionProperty.SetValue(Options, (int)MathF.Round(value));
            }
        }

        [ObservableProperty]
        private bool _effectEllipse;

        partial void OnEffectEllipseChanged(bool value)
        {
            bool isMagnify = ActiveTool == EditorTool.Magnify;
            bool isSpotlight = ActiveTool == EditorTool.Spotlight;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isMagnify = SelectedAnnotation is MagnifyAnnotation;
                isSpotlight = SelectedAnnotation is SpotlightAnnotation;
            }

            if (isMagnify)
            {
                Options.MagnifierEllipse = value;
            }
            else if (isSpotlight)
            {
                Options.SpotlightEllipse = value;
            }
        }

        [ObservableProperty]
        private bool _shadowEnabled = true;

        partial void OnShadowEnabledChanged(bool value)
        {
            Options.Shadow = value;
        }

        [ObservableProperty]
        private string _shadowColor = Annotation.DefaultShadowColorHex;

        public IBrush ShadowColorBrush
        {
            get => new SolidColorBrush(Color.Parse(ShadowColor));
            set
            {
                if (value is SolidColorBrush solidBrush)
                {
                    ShadowColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                }
            }
        }

        public Color ShadowColorValue
        {
            get => Color.Parse(ShadowColor);
            set => ShadowColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        partial void OnShadowColorChanged(string value)
        {
            OnPropertyChanged(nameof(ShadowColorBrush));
            OnPropertyChanged(nameof(ShadowColorValue));

            if (Color.TryParse(value, out Color color))
            {
                Options.ShadowColor = color;
            }
        }

        [ObservableProperty]
        private double _shadowBlurRadius = Annotation.DefaultShadowBlurRadius;

        partial void OnShadowBlurRadiusChanged(double value)
        {
            double clamped = Math.Max(0, value);
            if (Math.Abs(clamped - value) > double.Epsilon)
            {
                ShadowBlurRadius = clamped;
                return;
            }

            Options.ShadowBlurRadius = clamped;
        }

        [ObservableProperty]
        private double _shadowOpacity = Annotation.DefaultShadowOpacity;

        partial void OnShadowOpacityChanged(double value)
        {
            double clamped = Math.Clamp(value, 0, 1);
            if (Math.Abs(clamped - value) > double.Epsilon)
            {
                ShadowOpacity = clamped;
                return;
            }

            Options.ShadowOpacity = clamped;
        }

        [ObservableProperty]
        private double _shadowOffsetX = Annotation.DefaultShadowOffsetX;

        partial void OnShadowOffsetXChanged(double value)
        {
            Options.ShadowOffsetX = value;
        }

        [ObservableProperty]
        private double _shadowOffsetY = Annotation.DefaultShadowOffsetY;

        partial void OnShadowOffsetYChanged(double value)
        {
            Options.ShadowOffsetY = value;
        }

        [ObservableProperty]
        private bool _speechBalloonTail = true;

        partial void OnSpeechBalloonTailChanged(bool value)
        {
            bool appliesToSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;
            bool appliesToStep = ActiveTool == EditorTool.Step;

            if (ActiveTool == EditorTool.Select)
            {
                appliesToSpeechBalloon = SelectedAnnotation is SpeechBalloonAnnotation;
                appliesToStep = SelectedAnnotation is NumberAnnotation;
            }

            if (appliesToSpeechBalloon)
            {
                Options.SpeechBalloonTail = value;
            }
            else if (appliesToStep)
            {
                Options.StepTail = value;
            }
        }

        [ObservableProperty]
        private bool _textBold = true;

        partial void OnTextBoldChanged(bool value)
        {
            bool isText = ActiveTool == EditorTool.Text;
            bool isSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;
            bool isStep = ActiveTool == EditorTool.Step;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isText = SelectedAnnotation is TextAnnotation;
                isSpeechBalloon = SelectedAnnotation is SpeechBalloonAnnotation;
                isStep = SelectedAnnotation is NumberAnnotation;
            }

            if (isText)
            {
                Options.TextBold = value;
            }
            else if (isSpeechBalloon)
            {
                Options.SpeechBalloonTextBold = value;
            }
            else if (isStep)
            {
                Options.StepTextBold = value;
            }
        }

        [ObservableProperty]
        private bool _textItalic;

        partial void OnTextItalicChanged(bool value)
        {
            bool isText = ActiveTool == EditorTool.Text;
            bool isSpeechBalloon = ActiveTool == EditorTool.SpeechBalloon;

            if (ActiveTool == EditorTool.Select && SelectedAnnotation != null)
            {
                isText = SelectedAnnotation is TextAnnotation;
                isSpeechBalloon = SelectedAnnotation is SpeechBalloonAnnotation;
            }

            if (isText)
            {
                Options.TextItalic = value;
            }
            else if (isSpeechBalloon)
            {
                Options.SpeechBalloonTextItalic = value;
            }
        }

        // Visibility computed properties based on ActiveTool
        public bool ShowBorderColor => ActiveTool switch
        {
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow or EditorTool.Freehand or EditorTool.SpeechBalloon or EditorTool.Text or EditorTool.Step => true,
                _ => false
            },
            EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow or EditorTool.Freehand or EditorTool.SpeechBalloon or EditorTool.Text or EditorTool.Step => true,
            _ => false
        };

        public bool ShowFillColor => ActiveTool switch
        {
            EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.SpeechBalloon or EditorTool.Step or EditorTool.Highlight => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.SpeechBalloon or EditorTool.Step or EditorTool.Highlight => true,
                _ => false
            },
            _ => false
        };

        public bool ShowTextColor => ActiveTool switch
        {
            EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
            EditorTool.Select => SelectedAnnotation?.ToolType switch
            {
                EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
                _ => false
            },
            _ => false
        };

        public bool ShowThickness => ActiveTool switch
        {
            EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow
                or EditorTool.Freehand or EditorTool.SpeechBalloon or EditorTool.Step or EditorTool.Text => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow
                    or EditorTool.Freehand or EditorTool.SpeechBalloon or EditorTool.Step or EditorTool.Text => true,
                _ => false
            },
            _ => false
        };

        public bool ShowFontSize => ActiveTool switch
        {
            EditorTool.Text or EditorTool.Step or EditorTool.SpeechBalloon => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Text or EditorTool.Step or EditorTool.SpeechBalloon => true,
                _ => false
            },
            _ => false
        };

        public bool ShowStepStartNumber => ActiveTool == EditorTool.Step;

        public bool ShowStepType => ActiveTool == EditorTool.Step;

        public bool ShowTextHorizontalAlignment => ActiveTool switch
        {
            EditorTool.Text or EditorTool.SpeechBalloon => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Text or EditorTool.SpeechBalloon => true,
                _ => false
            },
            _ => false
        };

        public bool ShowFontFamily => ActiveTool switch
        {
            EditorTool.Text or EditorTool.SpeechBalloon => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Text or EditorTool.SpeechBalloon => true,
                _ => false
            },
            _ => false
        };

        public bool ShowBorderStyle => ActiveTool switch
        {
            EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Freehand => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Freehand => true,
                _ => false
            },
            _ => false
        };

        public bool ShowArrowStyle => ActiveTool switch
        {
            EditorTool.Arrow => true,
            EditorTool.Select => _selectedAnnotation is ArrowAnnotation,
            _ => false
        };

        public bool ShowCursorType => ActiveTool switch
        {
            EditorTool.Cursor => true,
            EditorTool.Select => _selectedAnnotation is CursorAnnotation,
            _ => false
        };

        public bool ShowCornerRadius => ActiveTool switch
        {
            EditorTool.Rectangle or EditorTool.SpeechBalloon => true,
            EditorTool.Select => _selectedAnnotation?.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.SpeechBalloon => true,
                _ => false
            },
            _ => false
        };

        public bool ShowStrength => ActiveTool switch
        {
            EditorTool.Blur or EditorTool.Pixelate or EditorTool.Magnify or EditorTool.Spotlight => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Blur or EditorTool.Pixelate or EditorTool.Magnify or EditorTool.Spotlight => true,
                _ => false
            },
            _ => false
        };

        public bool ShowSpotlightBlur => ActiveTool switch
        {
            EditorTool.Spotlight => true,
            EditorTool.Select => SelectedAnnotation is SpotlightAnnotation,
            _ => false
        };

        public bool ShowEffectEllipse => ActiveTool switch
        {
            EditorTool.Magnify or EditorTool.Spotlight => true,
            EditorTool.Select => SelectedAnnotation is MagnifyAnnotation or SpotlightAnnotation,
            _ => false
        };

        public bool ShowShadow => ActiveTool switch
        {
            EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow
                or EditorTool.Freehand or EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Rectangle or EditorTool.Ellipse or EditorTool.Line or EditorTool.Arrow
                    or EditorTool.Freehand or EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
                _ => false
            },
            _ => false
        };

        public bool ShowSpeechBalloonTail => ActiveTool switch
        {
            EditorTool.SpeechBalloon or EditorTool.Step => true,
            EditorTool.Select => SelectedAnnotation is SpeechBalloonAnnotation or NumberAnnotation,
            _ => false
        };

        public bool ShowTextStyle => ActiveTool switch
        {
            EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Text or EditorTool.SpeechBalloon or EditorTool.Step => true,
                _ => false
            },
            _ => false
        };

        public bool ShowTextItalic => ActiveTool switch
        {
            EditorTool.Text or EditorTool.SpeechBalloon => true,
            EditorTool.Select => _selectedAnnotation != null && _selectedAnnotation.ToolType switch
            {
                EditorTool.Text or EditorTool.SpeechBalloon => true,
                _ => false
            },
            _ => false
        };

        public string ActiveToolIcon
        {
            get
            {
                var tool = ActiveTool;
                if (tool == EditorTool.Select && _selectedAnnotation != null)
                {
                    tool = _selectedAnnotation.ToolType;
                }

                return EditorIcons.ForTool(tool);
            }
        }

        /// <summary>
        /// Returns the display name of the active tool (or selected shape's tool in Select mode).
        /// </summary>
        public string ActiveToolName
        {
            get
            {
                var tool = ActiveTool;
                if (tool == EditorTool.Select && _selectedAnnotation != null)
                {
                    tool = _selectedAnnotation.ToolType;
                }

                return tool switch
                {
                    EditorTool.Select => Strings.ToolbarCustomizationItemViewModel_Select,
                    EditorTool.Rectangle => Strings.ToolbarCustomizationItemViewModel_Rectangle,
                    EditorTool.Ellipse => Strings.ToolbarCustomizationItemViewModel_Ellipse,
                    EditorTool.Line => Strings.ToolbarCustomizationItemViewModel_Line,
                    EditorTool.Arrow => Strings.ToolbarCustomizationItemViewModel_Arrow,
                    EditorTool.Freehand => Strings.ToolbarCustomizationItemViewModel_Freehand,
                    EditorTool.Text => Strings.ToolbarCustomizationItemViewModel_Text,
                    EditorTool.Cursor => Strings.ToolbarCustomizationItemViewModel_Cursor,
                    EditorTool.Emoji => Strings.ToolbarCustomizationItemViewModel_Emoji,
                    EditorTool.SpeechBalloon => Strings.ToolbarCustomizationItemViewModel_SpeechBalloon,
                    EditorTool.Step => Strings.ToolbarCustomizationItemViewModel_Step,
                    EditorTool.Blur => Strings.ToolbarCustomizationItemViewModel_Blur,
                    EditorTool.Pixelate => Strings.ToolbarCustomizationItemViewModel_Pixelate,
                    EditorTool.Magnify => Strings.ToolbarCustomizationItemViewModel_Magnify,
                    EditorTool.Spotlight => Strings.ToolbarCustomizationItemViewModel_Spotlight,
                    EditorTool.SmartEraser => Strings.ToolbarCustomizationItemViewModel_SmartEraser,
                    EditorTool.Highlight => Strings.ToolbarCustomizationItemViewModel_Highlight,
                    EditorTool.Crop => Strings.ToolbarCustomizationItemViewModel_Crop,
                    EditorTool.CutOut => Strings.ToolbarCustomizationItemViewModel_CutOut,
                    EditorTool.Image => Strings.ToolbarCustomizationItemViewModel_Image,
                    _ => Strings.ToolbarCustomizationItemViewModel_Select
                };
            }
        }

        // Track selected annotation for Select tool visibility logic
        private Annotation? _selectedAnnotation;
        public Annotation? SelectedAnnotation
        {
            get => _selectedAnnotation;
            set
            {
                if (SetProperty(ref _selectedAnnotation, value))
                {
                    UpdateToolOptionsVisibility();
                }
            }
        }

        private void UpdateToolOptionsVisibility()
        {
            OnPropertyChanged(nameof(ShowBorderColor));
            OnPropertyChanged(nameof(ShowFillColor));
            OnPropertyChanged(nameof(ShowTextColor));
            OnPropertyChanged(nameof(ShowThickness));
            OnPropertyChanged(nameof(ShowFontSize));
            OnPropertyChanged(nameof(ShowStepStartNumber));
            OnPropertyChanged(nameof(ShowStepType));
            OnPropertyChanged(nameof(ShowTextHorizontalAlignment));
            OnPropertyChanged(nameof(ShowFontFamily));
            OnPropertyChanged(nameof(ShowBorderStyle));
            OnPropertyChanged(nameof(ShowArrowStyle));
            OnPropertyChanged(nameof(ShowCursorType));
            OnPropertyChanged(nameof(ShowCornerRadius));
            OnPropertyChanged(nameof(ShowStrength));
            OnPropertyChanged(nameof(ShowSpotlightBlur));
            OnPropertyChanged(nameof(ShowEffectEllipse));
            OnPropertyChanged(nameof(ShowTextStyle));
            OnPropertyChanged(nameof(ShowTextItalic));
            OnPropertyChanged(nameof(ShowShadow));
            OnPropertyChanged(nameof(ShowSpeechBalloonTail));
            OnPropertyChanged(nameof(ActiveToolIcon));
            OnPropertyChanged(nameof(ActiveToolName));
            OnPropertyChanged(nameof(EffectStrengthMaximum));
            OnPropertyChanged(nameof(ShowToolOptionsSeparator));
        }

        public bool ShowToolOptionsSeparator => ShowBorderColor || ShowFillColor || ShowTextColor || ShowThickness || ShowFontSize || ShowStepStartNumber || ShowStepType || ShowTextHorizontalAlignment || ShowFontFamily || ShowBorderStyle || ShowArrowStyle || ShowCursorType || ShowCornerRadius || ShowStrength || ShowSpotlightBlur || ShowEffectEllipse || ShowTextStyle || ShowShadow || ShowSpeechBalloonTail;

        [ObservableProperty]
        private EditorTool _activeTool = EditorTool.Rectangle;

        partial void OnActiveToolChanged(EditorTool value)
        {
            RememberAnnotationToolIfEligible(value);
            UpdateToolbarActiveStates();

            _isLoadingOptions = true;
            try
            {
                // Update the maximum FIRST so the UI control (slider/numeric)
                // accepts the new tool's value range before we set the value.
                // Without this, the control clamps the new value to the OLD maximum
                // via its two-way binding (e.g. Spotlight default 15 clamped to
                // Magnify's old max of 10).
                OnPropertyChanged(nameof(EffectStrengthMaximum));

                LoadOptionsForTool(value);
                UpdateToolOptionsVisibility();
            }
            finally
            {
                _isLoadingOptions = false;
            }
        }

        private static bool IsRememberableAnnotationTool(EditorTool tool) => tool is
            EditorTool.Rectangle or
            EditorTool.Ellipse or
            EditorTool.Line or
            EditorTool.Arrow or
            EditorTool.Freehand or
            EditorTool.Text or
            EditorTool.SpeechBalloon or
            EditorTool.Step or
            EditorTool.Cursor or
            EditorTool.Highlight or
            EditorTool.SmartEraser or
            EditorTool.Blur or
            EditorTool.Pixelate or
            EditorTool.Magnify or
            EditorTool.Spotlight;

        private static EditorTool SanitizeRememberedAnnotationTool(EditorTool tool)
        {
            return IsRememberableAnnotationTool(tool) ? tool : EditorTool.Rectangle;
        }

        private EditorTool GetInitialAnnotationTool()
        {
            EditorTool rememberedTool = Options.LastUsedAnnotationTool;

            if (!IsRememberableAnnotationTool(rememberedTool) && _sessionLastUsedAnnotationTool.HasValue)
            {
                rememberedTool = _sessionLastUsedAnnotationTool.Value;
            }

            return SanitizeRememberedAnnotationTool(rememberedTool);
        }

        private void RememberAnnotationToolIfEligible(EditorTool tool)
        {
            if (!IsRememberableAnnotationTool(tool))
            {
                return;
            }

            Options.LastUsedAnnotationTool = tool;
            _sessionLastUsedAnnotationTool = tool;
        }

        private void LoadOptionsForTool(EditorTool tool)
        {
            // Prevent property change callbacks from overwriting options while loading
            // We can just set fields directly or use a flag, but setting properties is safer for UI updates.
            // However, setting properties triggers On...Changed which calls UpdateOptionsFrom...
            // Use a flag to suppress updates back to Options?
            // Actually, if we set the property to the value from Options, updating Options back to the same value is harmless.

            ShadowColorValue = Options.ShadowColor;
            ShadowBlurRadius = Options.ShadowBlurRadius;
            ShadowOpacity = Options.ShadowOpacity;
            ShadowOffsetX = Options.ShadowOffsetX;
            ShadowOffsetY = Options.ShadowOffsetY;

            switch (tool)
            {
                case EditorTool.Rectangle:
                case EditorTool.Ellipse:
                case EditorTool.Line:
                    SelectedColorValue = Options.BorderColor;
                    FillColorValue = Options.FillColor;
                    StrokeWidth = Options.Thickness;
                    CornerRadius = Options.CornerRadius;
                    ShadowEnabled = Options.Shadow;
                    SelectedBorderStyle = NormalizeBorderStyle(Options.BorderStyle);
                    FontSize = Options.TextFontSize;
                    break;
                case EditorTool.Freehand:
                    SelectedColorValue = Options.BorderColor;
                    FillColorValue = Options.FillColor;
                    StrokeWidth = Options.Thickness;
                    CornerRadius = Options.CornerRadius;
                    ShadowEnabled = Options.Shadow;
                    SelectedBorderStyle = NormalizeBorderStyle(Options.BorderStyle);
                    FontSize = Options.TextFontSize;
                    break;
                case EditorTool.Arrow:
                    SelectedColorValue = Options.BorderColor;
                    FillColorValue = Options.FillColor;
                    StrokeWidth = Options.Thickness;
                    CornerRadius = Options.CornerRadius;
                    ShadowEnabled = Options.Shadow;
                    FontSize = Options.TextFontSize;
                    SelectedArrowStyle = NormalizeArrowStyle(Options.ArrowStyle);
                    break;
                case EditorTool.Cursor:
                    SelectedCursorType = NormalizeCursorType(Options.CursorType);
                    break;
                case EditorTool.Text:
                    SelectedColorValue = Options.TextBorderColor;
                    TextColorValue = Options.TextTextColor;
                    StrokeWidth = Options.TextThickness;
                    ShadowEnabled = Options.Shadow;
                    FontSize = Options.TextFontSize;
                    SelectedFontFamily = NormalizeFontFamily(Options.TextFontFamily);
                    SelectedTextHorizontalAlignment = NormalizeTextHorizontalAlignment(Options.TextHorizontalAlignment);
                    TextBold = Options.TextBold;
                    TextItalic = Options.TextItalic;
                    break;
                case EditorTool.SpeechBalloon:
                    SelectedColorValue = Options.SpeechBalloonBorderColor;
                    FillColorValue = Options.SpeechBalloonFillColor;
                    TextColorValue = Options.SpeechBalloonTextColor;
                    StrokeWidth = Options.SpeechBalloonThickness;
                    CornerRadius = Options.CornerRadius;
                    ShadowEnabled = Options.Shadow;
                    SpeechBalloonTail = Options.SpeechBalloonTail;
                    FontSize = Options.SpeechBalloonFontSize;
                    SelectedFontFamily = NormalizeFontFamily(Options.SpeechBalloonFontFamily);
                    SelectedTextHorizontalAlignment = NormalizeTextHorizontalAlignment(Options.SpeechBalloonTextHorizontalAlignment);
                    TextBold = Options.SpeechBalloonTextBold;
                    TextItalic = Options.SpeechBalloonTextItalic;
                    break;
                case EditorTool.Step:
                    SelectedColorValue = Options.StepBorderColor;
                    FillColorValue = Options.StepFillColor;
                    TextColorValue = Options.StepTextColor;
                    StrokeWidth = Options.StepThickness;
                    ShadowEnabled = Options.Shadow;
                    SpeechBalloonTail = Options.StepTail;
                    FontSize = Options.StepFontSize;
                    SelectedStepType = NormalizeStepType(Options.StepType);
                    TextBold = Options.StepTextBold;
                    TextItalic = Options.TextItalic;
                    break;
                case EditorTool.Highlight:
                    FillColorValue = Options.HighlightFillColor;
                    break;
                case EditorTool.Blur:
                    EffectStrength = Options.BlurStrength;
                    break;
                case EditorTool.Pixelate:
                    EffectStrength = Options.PixelateStrength;
                    break;
                case EditorTool.Magnify:
                    EffectStrength = Options.MagnifierStrength;
                    EffectEllipse = Options.MagnifierEllipse;
                    break;
                case EditorTool.Spotlight:
                    EffectStrength = Options.SpotlightStrength;
                    SpotlightBlur = GetSpotlightBlurOption();
                    EffectEllipse = Options.SpotlightEllipse;
                    break;
            }
        }

        private static IReadOnlyList<string> BuildAvailableFontFamilies()
        {
            try
            {
                string[] fontFamilies = FontManager.Current.SystemFonts
                    .Select(fontFamily => fontFamily.Name)
                    .Where(fontFamily => !string.IsNullOrWhiteSpace(fontFamily))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(fontFamily => fontFamily, StringComparer.CurrentCultureIgnoreCase)
                    .ToArray();

                if (fontFamilies.Length > 0)
                {
                    return fontFamilies;
                }
            }
            catch
            {
            }

            return new[] { DefaultAnnotationFontFamily };
        }

        private static IReadOnlyList<ArrowStyle> BuildAvailableArrowStyles()
        {
            return new[]
            {
                ArrowStyle.Classic,
                ArrowStyle.Double,
                ArrowStyle.Modern,
                ArrowStyle.Basic,
                ArrowStyle.Line
            };
        }

        private static IReadOnlyList<BorderStyle> BuildAvailableBorderStyles()
        {
            return new[]
            {
                BorderStyle.Solid,
                BorderStyle.Dash,
                BorderStyle.Dot,
                BorderStyle.DashDot,
                BorderStyle.DashDotDot
            };
        }

        private static IReadOnlyList<TextHorizontalAlignment> BuildAvailableTextHorizontalAlignments()
        {
            return new[]
            {
                TextHorizontalAlignment.Left,
                TextHorizontalAlignment.Center,
                TextHorizontalAlignment.Right
            };
        }

        private static IReadOnlyList<CursorType> BuildAvailableCursorTypes()
        {
            return new[]
            {
                CursorType.AppStarting,
                CursorType.Arrow,
                CursorType.Cross,
                CursorType.Default,
                CursorType.Hand,
                CursorType.Help,
                CursorType.HSplit,
                CursorType.IBeam,
                CursorType.No,
                CursorType.NoMove2D,
                CursorType.NoMoveHoriz,
                CursorType.NoMoveVert,
                CursorType.PanEast,
                CursorType.PanNE,
                CursorType.PanNorth,
                CursorType.PanNW,
                CursorType.PanSE,
                CursorType.PanSouth,
                CursorType.PanSW,
                CursorType.PanWest,
                CursorType.SizeAll,
                CursorType.SizeNESW,
                CursorType.SizeNS,
                CursorType.SizeNWSE,
                CursorType.SizeWE,
                CursorType.UpArrow,
                CursorType.VSplit,
                CursorType.WaitCursor
            };
        }

        private static IReadOnlyList<StepType> BuildAvailableStepTypes()
        {
            return new[]
            {
                StepType.Numeric,
                StepType.UppercaseLetter,
                StepType.LowercaseLetter,
                StepType.UppercaseRoman,
                StepType.LowercaseRoman
            };
        }

        private static string NormalizeFontFamily(string? fontFamily)
        {
            return string.IsNullOrWhiteSpace(fontFamily) ? DefaultAnnotationFontFamily : fontFamily;
        }

        private static ArrowStyle NormalizeArrowStyle(ArrowStyle arrowStyle)
        {
            return Enum.IsDefined(arrowStyle) ? arrowStyle : ArrowStyle.Classic;
        }

        private static BorderStyle NormalizeBorderStyle(BorderStyle borderStyle)
        {
            return Enum.IsDefined(borderStyle) ? borderStyle : BorderStyle.Solid;
        }

        private static CursorType NormalizeCursorType(CursorType cursorType)
        {
            return Enum.IsDefined(cursorType) ? cursorType : CursorType.Default;
        }

        private static TextHorizontalAlignment NormalizeTextHorizontalAlignment(TextHorizontalAlignment alignment)
        {
            return Enum.IsDefined(alignment) ? alignment : TextHorizontalAlignment.Center;
        }

        private static StepType NormalizeStepType(StepType stepType)
        {
            return Enum.IsDefined(stepType) ? stepType : StepType.Numeric;
        }
    }
}