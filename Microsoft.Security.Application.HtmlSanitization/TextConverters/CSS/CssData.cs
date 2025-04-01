// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssData.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//    
// </summary>

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
    using System;
    using Microsoft.Exchange.Data.Internal;

    internal enum CssNameIndex : byte
    {
        Unknown = 0,
        ScrollbarArrowColor = 1,
        WhiteSpace = 2,
        LineBreak = 3,
        Orphans = 4,
        WritingMode = 5,
        Scrollbar3dLightColor = 6,
        TextAutospace = 7,
        VerticalAlign = 8,
        BorderRight = 9,
        Bottom = 10,
        FontFamily = 11,
        LineHeight = 12,
        BorderBottom = 13,
        ScrollbarBaseColor = 14,
        MinWidth = 15,
        BackgroundColor = 16,
        BorderTopStyle = 17,
        EmptyCells = 18,
        ListStyleType = 19,
        TextAlign = 20,
        FontWeight = 21,
        OutlineWidth = 22,
        CaptionSide = 23,
        ScrollbarShadowColor = 24,
        Clip = 25,
        Volume = 26,
        MarginLeft = 27,
        BorderTopWidth = 28,
        Azimuth = 29,
        UnicodeBidi = 30,
        Float = 31,
        LayoutFlow = 32,
        MinHeight = 33,
        Content = 34,
        Padding = 35,
        BorderBottomWidth = 36,
        Visibility = 37,
        Overflow = 38,
        TableLayout = 39,
        BorderLeftColor = 40,
        Pitch = 41,
        Pause = 42,
        OverflowY = 43,
        ScrollbarHighlightColor = 44,
        Height = 45,
        WordWrap = 46,
        Top = 47,
        ListStyle = 48,
        Margin = 49,
        TextKashidaSpace = 50,
        VoiceFamily = 51,
        CueBefore = 52,
        Clear = 53,
        TextOverflow = 54,
        BorderBottomStyle = 55,
        BorderColor = 56,
        TextDecoration = 57,
        Display = 58,
        CounterReset = 59,
        MarginBottom = 60,
        BorderStyle = 61,
        LayoutGrid = 62,
        Quotes = 63,
        Accelerator = 64,
        Border = 65,
        Zoom = 66,
        OutlineStyle = 67,
        Width = 68,
        Color = 69,
        PageBreakInside = 70,
        PitchRange = 71,
        BorderCollapse = 72,
        Speak = 73,
        Cue = 74,
        Left = 75,
        LayoutGridMode = 76,
        SpeakPunctuation = 77,
        LayoutGridLine = 78,
        BorderSpacing = 79,
        TextTransform = 80,
        BorderRightWidth = 81,
        PageBreakBefore = 82,
        TextIndent = 83,
        LayoutGridChar = 84,
        SpeechRate = 85,
        PauseBefore = 86,
        ScrollbarFaceColor = 87,
        PlayDuring = 88,
        WordBreak = 89,
        BorderBottomColor = 90,
        MarginRight = 91,
        SpeakNumeral = 92,
        TextJustify = 93,
        PaddingRight = 94,
        BorderRightStyle = 95,
        CounterIncrement = 96,
        TextUnderlinePosition = 97,
        WordSpacing = 98,
        Background = 99,
        OverflowX = 100,
        BorderWidth = 101,
        Widows = 102,
        ZIndex = 103,
        BorderTopColor = 104,
        MaxWidth = 105,
        ScrollbarDarkshadowColor = 106,
        CueAfter = 107,
        SpeakHeader = 108,
        Direction = 109,
        FontVariant = 110,
        Richness = 111,
        Stress = 112,
        Font = 113,
        Elevation = 114,
        Outline = 115,
        BorderRightColor = 116,
        FontStyle = 117,
        MarginTop = 118,
        BorderLeft = 119,
        ListStylePosition = 120,
        OutlineColor = 121,
        BorderLeftWidth = 122,
        PaddingBottom = 123,
        LayoutGridType = 124,
        PageBreakAfter = 125,
        FontSize = 126,
        Position = 127,
        BorderLeftStyle = 128,
        PaddingTop = 129,
        PaddingLeft = 130,
        Right = 131,
        PauseAfter = 132,
        MaxHeight = 133,
        LetterSpacing = 134,
        BorderTop = 135,
        Max
    }

    internal static class CssData
    {
        public const short MAX_NAME = 26;
        public const short MAX_TAG_NAME = 26;

        public struct NameDef
        {
            public short hash;
            public string name;
            public CssNameIndex publicNameId;

            public NameDef(short hash, string name, CssNameIndex publicNameId)
            {
                this.hash = hash;
                this.name = name;
                this.publicNameId = publicNameId;
            }
        }

        public enum FilterAction : byte
        {
            Unknown,
            Drop,
            Keep,
            CheckContent,
        }

        public struct FilterActionEntry
        {
            public FilterAction propertyAction;

            public FilterActionEntry(FilterAction propertyAction)
            {
                this.propertyAction = propertyAction;
            }
        }

        public const short NAME_HASH_SIZE = 329;
        public const int NAME_HASH_MODIFIER = 0x2;

        public static CssNameIndex[] nameHashTable =
        [
            CssNameIndex.ScrollbarArrowColor,
            0,
            CssNameIndex.WhiteSpace,
            0,
            CssNameIndex.LineBreak,
            CssNameIndex.Orphans,
            0,
            0,
            0,
            0,
            CssNameIndex.WritingMode,
            0,
            CssNameIndex.Scrollbar3dLightColor,
            0,
            CssNameIndex.TextAutospace,
            CssNameIndex.VerticalAlign,
            0,
            0,
            CssNameIndex.BorderRight,
            0,
            0,
            CssNameIndex.Bottom,
            CssNameIndex.LineHeight,
            0,
            0,
            CssNameIndex.BorderBottom,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.ScrollbarBaseColor,
            CssNameIndex.MinWidth,
            CssNameIndex.BackgroundColor,
            0,
            CssNameIndex.BorderTopStyle,
            0,
            CssNameIndex.EmptyCells,
            0,
            CssNameIndex.ListStyleType,
            0,
            0,
            0,
            0,
            CssNameIndex.TextAlign,
            0,
            CssNameIndex.FontWeight,
            0,
            CssNameIndex.OutlineWidth,
            CssNameIndex.CaptionSide,
            CssNameIndex.ScrollbarShadowColor,
            0,
            0,
            0,
            CssNameIndex.Clip,
            0,
            CssNameIndex.MarginLeft,
            CssNameIndex.BorderTopWidth,
            0,
            0,
            CssNameIndex.Azimuth,
            CssNameIndex.Float,
            0,
            0,
            0,
            CssNameIndex.LayoutFlow,
            CssNameIndex.MinHeight,
            CssNameIndex.Content,
            0,
            CssNameIndex.Padding,
            CssNameIndex.BorderBottomWidth,
            0,
            0,
            CssNameIndex.Visibility,
            0,
            CssNameIndex.Overflow,
            CssNameIndex.BorderLeftColor,
            0,
            0,
            CssNameIndex.Pitch,
            CssNameIndex.Pause,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.OverflowY,
            0,
            0,
            0,
            CssNameIndex.ScrollbarHighlightColor,
            0,
            CssNameIndex.Height,
            0,
            CssNameIndex.WordWrap,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.Top,
            CssNameIndex.ListStyle,
            0,
            CssNameIndex.Margin,
            0,
            CssNameIndex.TextKashidaSpace,
            CssNameIndex.VoiceFamily,
            CssNameIndex.CueBefore,
            CssNameIndex.Clear,
            0,
            0,
            0,
            CssNameIndex.TextOverflow,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.BorderBottomStyle,
            0,
            0,
            CssNameIndex.BorderColor,
            CssNameIndex.TextDecoration,
            CssNameIndex.Display,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.CounterReset,
            CssNameIndex.MarginBottom,
            CssNameIndex.BorderStyle,
            0,
            0,
            0,
            CssNameIndex.LayoutGrid,
            CssNameIndex.Quotes,
            0,
            0,
            0,
            CssNameIndex.Accelerator,
            CssNameIndex.Border,
            0,
            0,
            CssNameIndex.Zoom,
            0,
            0,
            CssNameIndex.OutlineStyle,
            0,
            CssNameIndex.Width,
            0,
            CssNameIndex.Color,
            0,
            0,
            0,
            0,
            CssNameIndex.PageBreakInside,
            0,
            CssNameIndex.PitchRange,
            CssNameIndex.BorderCollapse,
            CssNameIndex.Cue,
            0,
            CssNameIndex.Left,
            CssNameIndex.LayoutGridMode,
            0,
            0,
            CssNameIndex.SpeakPunctuation,
            CssNameIndex.LayoutGridLine,
            0,
            0,
            0,
            0,
            CssNameIndex.BorderSpacing,
            0,
            CssNameIndex.TextTransform,
            0,
            0,
            0,
            CssNameIndex.BorderRightWidth,
            CssNameIndex.PageBreakBefore,
            CssNameIndex.TextIndent,
            CssNameIndex.LayoutGridChar,
            CssNameIndex.SpeechRate,
            CssNameIndex.PauseBefore,
            0,
            CssNameIndex.ScrollbarFaceColor,
            0,
            0,
            0,
            CssNameIndex.PlayDuring,
            0,
            0,
            CssNameIndex.WordBreak,
            CssNameIndex.BorderBottomColor,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.MarginRight,
            0,
            0,
            CssNameIndex.SpeakNumeral,
            0,
            0,
            0,
            0,
            CssNameIndex.TextJustify,
            CssNameIndex.PaddingRight,
            CssNameIndex.BorderRightStyle,
            0,
            0,
            CssNameIndex.CounterIncrement,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.TextUnderlinePosition,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.WordSpacing,
            0,
            0,
            CssNameIndex.Background,
            0,
            CssNameIndex.OverflowX,
            CssNameIndex.BorderWidth,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.ZIndex,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.MaxWidth,
            0,
            0,
            0,
            0,
            CssNameIndex.ScrollbarDarkshadowColor,
            0,
            0,
            0,
            CssNameIndex.CueAfter,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.SpeakHeader,
            0,
            CssNameIndex.Direction,
            CssNameIndex.FontVariant,
            0,
            CssNameIndex.Richness,
            0,
            0,
            0,
            0,
            0,
            0,
            CssNameIndex.Font,
            0,
            0,
            0,
            CssNameIndex.Outline,
            0,
            0,
            0,
            CssNameIndex.BorderRightColor,
            0,
            CssNameIndex.FontStyle,
            CssNameIndex.MarginTop,
            0,
            0,
            CssNameIndex.BorderLeft,
            0,
            0,
            CssNameIndex.ListStylePosition,
            0,
            CssNameIndex.BorderLeftWidth,
            0,
            0,
            0,
            0,
            CssNameIndex.PaddingBottom,
            0,
            CssNameIndex.LayoutGridType,
            CssNameIndex.PageBreakAfter,
            0,
            0,
            CssNameIndex.FontSize,
            0,
            CssNameIndex.Position,
            CssNameIndex.BorderLeftStyle,
            CssNameIndex.PaddingLeft,
            0,
            0,
            CssNameIndex.Right,
            CssNameIndex.PauseAfter,
            CssNameIndex.MaxHeight,
            0,
            0,
            CssNameIndex.LetterSpacing,
            0,
            0,
            0,
            0,
            CssNameIndex.BorderTop,
        ];

        public static NameDef[] names =
        [
            new NameDef(0, null, CssNameIndex.Unknown),
            new NameDef(0, "scrollbar-arrow-color", CssNameIndex.ScrollbarArrowColor),
            new NameDef(2, "white-space", CssNameIndex.WhiteSpace),
            new NameDef(4, "line-break", CssNameIndex.LineBreak),
            new NameDef(5, "orphans", CssNameIndex.Orphans),
            new NameDef(10, "writing-mode", CssNameIndex.WritingMode),
            new NameDef(12, "scrollbar-3dlight-color", CssNameIndex.Scrollbar3dLightColor),
            new NameDef(14, "text-autospace", CssNameIndex.TextAutospace),
            new NameDef(15, "vertical-align", CssNameIndex.VerticalAlign),
            new NameDef(18, "border-right", CssNameIndex.BorderRight),
            new NameDef(21, "bottom", CssNameIndex.Bottom),
            new NameDef(21, "font-family", CssNameIndex.FontFamily),
            new NameDef(22, "line-height", CssNameIndex.LineHeight),
            new NameDef(25, "border-bottom", CssNameIndex.BorderBottom),
            new NameDef(32, "scrollbar-base-color", CssNameIndex.ScrollbarBaseColor),
            new NameDef(33, "min-width", CssNameIndex.MinWidth),
            new NameDef(34, "background-color", CssNameIndex.BackgroundColor),
            new NameDef(36, "border-top-style", CssNameIndex.BorderTopStyle),
            new NameDef(38, "empty-cells", CssNameIndex.EmptyCells),
            new NameDef(40, "list-style-type", CssNameIndex.ListStyleType),
            new NameDef(45, "text-align", CssNameIndex.TextAlign),
            new NameDef(47, "font-weight", CssNameIndex.FontWeight),
            new NameDef(49, "outline-width", CssNameIndex.OutlineWidth),
            new NameDef(50, "caption-side", CssNameIndex.CaptionSide),
            new NameDef(51, "scrollbar-shadow-color", CssNameIndex.ScrollbarShadowColor),
            new NameDef(55, "clip", CssNameIndex.Clip),
            new NameDef(55, "volume", CssNameIndex.Volume),
            new NameDef(57, "margin-left", CssNameIndex.MarginLeft),
            new NameDef(58, "border-top-width", CssNameIndex.BorderTopWidth),
            new NameDef(61, "azimuth", CssNameIndex.Azimuth),
            new NameDef(61, "unicode-bidi", CssNameIndex.UnicodeBidi),
            new NameDef(62, "float", CssNameIndex.Float),
            new NameDef(66, "layout-flow", CssNameIndex.LayoutFlow),
            new NameDef(67, "min-height", CssNameIndex.MinHeight),
            new NameDef(68, "content", CssNameIndex.Content),
            new NameDef(70, "padding", CssNameIndex.Padding),
            new NameDef(71, "border-bottom-width", CssNameIndex.BorderBottomWidth),
            new NameDef(74, "visibility", CssNameIndex.Visibility),
            new NameDef(76, "overflow", CssNameIndex.Overflow),
            new NameDef(76, "table-layout", CssNameIndex.TableLayout),
            new NameDef(77, "border-left-color", CssNameIndex.BorderLeftColor),
            new NameDef(80, "pitch", CssNameIndex.Pitch),
            new NameDef(81, "pause", CssNameIndex.Pause),
            new NameDef(89, "overflow-y", CssNameIndex.OverflowY),
            new NameDef(93, "scrollbar-highlight-color", CssNameIndex.ScrollbarHighlightColor),
            new NameDef(95, "height", CssNameIndex.Height),
            new NameDef(97, "word-wrap", CssNameIndex.WordWrap),
            new NameDef(104, "top", CssNameIndex.Top),
            new NameDef(105, "list-style", CssNameIndex.ListStyle),
            new NameDef(107, "margin", CssNameIndex.Margin),
            new NameDef(109, "text-kashida-space", CssNameIndex.TextKashidaSpace),
            new NameDef(110, "voice-family", CssNameIndex.VoiceFamily),
            new NameDef(111, "cue-before", CssNameIndex.CueBefore),
            new NameDef(112, "clear", CssNameIndex.Clear),
            new NameDef(116, "text-overflow", CssNameIndex.TextOverflow),
            new NameDef(125, "border-bottom-style", CssNameIndex.BorderBottomStyle),
            new NameDef(128, "border-color", CssNameIndex.BorderColor),
            new NameDef(129, "text-decoration", CssNameIndex.TextDecoration),
            new NameDef(130, "display", CssNameIndex.Display),
            new NameDef(136, "counter-reset", CssNameIndex.CounterReset),
            new NameDef(137, "margin-bottom", CssNameIndex.MarginBottom),
            new NameDef(138, "border-style", CssNameIndex.BorderStyle),
            new NameDef(142, "layout-grid", CssNameIndex.LayoutGrid),
            new NameDef(143, "quotes", CssNameIndex.Quotes),
            new NameDef(147, "accelerator", CssNameIndex.Accelerator),
            new NameDef(148, "border", CssNameIndex.Border),
            new NameDef(151, "zoom", CssNameIndex.Zoom),
            new NameDef(154, "outline-style", CssNameIndex.OutlineStyle),
            new NameDef(156, "width", CssNameIndex.Width),
            new NameDef(158, "color", CssNameIndex.Color),
            new NameDef(163, "page-break-inside", CssNameIndex.PageBreakInside),
            new NameDef(165, "pitch-range", CssNameIndex.PitchRange),
            new NameDef(166, "border-collapse", CssNameIndex.BorderCollapse),
            new NameDef(166, "speak", CssNameIndex.Speak),
            new NameDef(167, "cue", CssNameIndex.Cue),
            new NameDef(169, "left", CssNameIndex.Left),
            new NameDef(170, "layout-grid-mode", CssNameIndex.LayoutGridMode),
            new NameDef(173, "speak-punctuation", CssNameIndex.SpeakPunctuation),
            new NameDef(174, "layout-grid-line", CssNameIndex.LayoutGridLine),
            new NameDef(179, "border-spacing", CssNameIndex.BorderSpacing),
            new NameDef(181, "text-transform", CssNameIndex.TextTransform),
            new NameDef(185, "border-right-width", CssNameIndex.BorderRightWidth),
            new NameDef(186, "page-break-before", CssNameIndex.PageBreakBefore),
            new NameDef(187, "text-indent", CssNameIndex.TextIndent),
            new NameDef(188, "layout-grid-char", CssNameIndex.LayoutGridChar),
            new NameDef(189, "speech-rate", CssNameIndex.SpeechRate),
            new NameDef(190, "pause-before", CssNameIndex.PauseBefore),
            new NameDef(192, "scrollbar-face-color", CssNameIndex.ScrollbarFaceColor),
            new NameDef(196, "play-during", CssNameIndex.PlayDuring),
            new NameDef(199, "word-break", CssNameIndex.WordBreak),
            new NameDef(200, "border-bottom-color", CssNameIndex.BorderBottomColor),
            new NameDef(208, "margin-right", CssNameIndex.MarginRight),
            new NameDef(211, "speak-numeral", CssNameIndex.SpeakNumeral),
            new NameDef(216, "text-justify", CssNameIndex.TextJustify),
            new NameDef(217, "padding-right", CssNameIndex.PaddingRight),
            new NameDef(218, "border-right-style", CssNameIndex.BorderRightStyle),
            new NameDef(221, "counter-increment", CssNameIndex.CounterIncrement),
            new NameDef(227, "text-underline-position", CssNameIndex.TextUnderlinePosition),
            new NameDef(233, "word-spacing", CssNameIndex.WordSpacing),
            new NameDef(236, "background", CssNameIndex.Background),
            new NameDef(238, "overflow-x", CssNameIndex.OverflowX),
            new NameDef(239, "border-width", CssNameIndex.BorderWidth),
            new NameDef(239, "widows", CssNameIndex.Widows),
            new NameDef(245, "z-index", CssNameIndex.ZIndex),
            new NameDef(245, "border-top-color", CssNameIndex.BorderTopColor),
            new NameDef(252, "max-width", CssNameIndex.MaxWidth),
            new NameDef(257, "scrollbar-darkshadow-color", CssNameIndex.ScrollbarDarkshadowColor),
            new NameDef(261, "cue-after", CssNameIndex.CueAfter),
            new NameDef(269, "speak-header", CssNameIndex.SpeakHeader),
            new NameDef(271, "direction", CssNameIndex.Direction),
            new NameDef(272, "font-variant", CssNameIndex.FontVariant),
            new NameDef(274, "richness", CssNameIndex.Richness),
            new NameDef(274, "stress", CssNameIndex.Stress),
            new NameDef(281, "font", CssNameIndex.Font),
            new NameDef(281, "elevation", CssNameIndex.Elevation),
            new NameDef(285, "outline", CssNameIndex.Outline),
            new NameDef(289, "border-right-color", CssNameIndex.BorderRightColor),
            new NameDef(291, "font-style", CssNameIndex.FontStyle),
            new NameDef(292, "margin-top", CssNameIndex.MarginTop),
            new NameDef(295, "border-left", CssNameIndex.BorderLeft),
            new NameDef(298, "list-style-position", CssNameIndex.ListStylePosition),
            new NameDef(298, "outline-color", CssNameIndex.OutlineColor),
            new NameDef(300, "border-left-width", CssNameIndex.BorderLeftWidth),
            new NameDef(305, "padding-bottom", CssNameIndex.PaddingBottom),
            new NameDef(307, "layout-grid-type", CssNameIndex.LayoutGridType),
            new NameDef(308, "page-break-after", CssNameIndex.PageBreakAfter),
            new NameDef(311, "font-size", CssNameIndex.FontSize),
            new NameDef(313, "position", CssNameIndex.Position),
            new NameDef(314, "border-left-style", CssNameIndex.BorderLeftStyle),
            new NameDef(314, "padding-top", CssNameIndex.PaddingTop),
            new NameDef(315, "padding-left", CssNameIndex.PaddingLeft),
            new NameDef(318, "right", CssNameIndex.Right),
            new NameDef(319, "pause-after", CssNameIndex.PauseAfter),
            new NameDef(320, "max-height", CssNameIndex.MaxHeight),
            new NameDef(323, "letter-spacing", CssNameIndex.LetterSpacing),
            new NameDef(328, "border-top", CssNameIndex.BorderTop),
            new NameDef(329, null, CssNameIndex.Unknown),
        ];

        public static FilterActionEntry[] filterInstructions =
        [
            new FilterActionEntry(FilterAction.Drop),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.CheckContent),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.CheckContent),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Drop),
        ];
    }
}
