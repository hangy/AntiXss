// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatProperty.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Exchange.Data.Internal;

    internal enum PropertyId : byte
    {
        Null = 0,
        FirstFlag = 1,
        Bold = FirstFlag,       
        Italic,                 
        Underline,              
        Subscript,              
        Superscript,            
        Strikethrough,          
        SmallCaps,              
        Capitalize,             
        RightToLeft,            
        Preformatted,           
        NoBreak,                
        Visible,                
        Overloaded1,            
        Overloaded2,            
        Overloaded3,            
        MergedCell,             
        TableLayoutFixed = Overloaded1,     
        SelectMultiple = Overloaded1,       
        OptionSelected = Overloaded1,       
        ReadOnly = Overloaded1,             
        TableBorderCollapse = Overloaded2,  
        Disabled = Overloaded2,             
        Checked = Overloaded3,              
        LastFlag = 16,
        FontColor,              
        FontSize,               
        FontFace,               
        TextAlignment,          
        FirstLineIndent,        
        BlockAlignment,         
        HorizontalAlignment = BlockAlignment, 
        VerticalAlignment = BlockAlignment, 
        BackColor,              
        Display,                
        Language,               
        UnicodeBiDi,            
        Width,                  
        Height,                 
        Margins,                
        TopMargin = Margins,    
        RightMargin,            
        BottomMargin,           
        LeftMargin,             
        Paddings,               
        TopPadding = Paddings,  
        RightPadding,           
        BottomPadding,          
        LeftPadding,            
        BorderWidths,           
        TopBorderWidth = BorderWidths, 
        RightBorderWidth,       
        BottomBorderWidth,      
        LeftBorderWidth,        
        BorderStyles,           
        TopBorderStyle = BorderStyles, 
        RightBorderStyle,       
        BottomBorderStyle,      
        LeftBorderStyle,        
        BorderColors,           
        TopBorderColor = BorderColors, 
        RightBorderColor,       
        BottomBorderColor,      
        LeftBorderColor,        
        ListLevel,              
        ListStyle,              
        ListStart,              
        NumColumns,             
        NumRows,                
        TableShowEmptyCells,    
        TableCaptionSideTop,    
        TableCellNoWrap,        
        TableBorderSpacingVertical, 
        TableBorderSpacingHorizontal, 
        TableBorder,            
        TableFrame,             
        TableRules,             
        TableCellSpacing,       
        TableCellPadding,       
        BookmarkName,           
        HyperlinkUrl,           
        HyperlinkTarget,        
        ImageUrl,               
        ImageAltText,           
        QuotingLevelDelta,      
        MaxValue,    
        ImageBorder = TableBorder,
        IFrameUrl = ImageUrl,
        FormAction = HyperlinkUrl,
        FormMethod = HyperlinkTarget,
        FormEncodingType = ImageUrl,
        FormAcceptContentTypes = ImageAltText,
        FormAcceptCharsets = QuotingLevelDelta,
        Label = ImageAltText,
        OptionValue = QuotingLevelDelta,
        InputValue = QuotingLevelDelta,
        InputType = TableBorder,
        InputSize = TableFrame,
        InputMaxLength = TableRules,
        ButtonType = InputType,
        Prompt = ImageAltText,
    }

    internal enum Side
    {
        Top,
        Right,
        Bottom,
        Left,
    }

    internal enum PropertyType : byte
    {
        Null,                       
        Calculated,                 
        Bool,                       
        String,                     
        MultiValue,                 
        Enum,                       
        Color,                      
        Integer,                    
        Fractional,                 
        Percentage,                 
        AbsLength,                  
        RelLength,                  
        Pixels,                     
        Ems,                        
        Exs,                        
        HtmlFontUnits,              
        RelHtmlFontUnits,           
        Multiple,                   
        Milliseconds,               
        kHz,                        
        Degrees,                    
        FirstLength = AbsLength,
        LastLength = Multiple,
    }

    internal struct RGBT
    {
        private uint        rawValue;

        public RGBT(uint rawValue)
        {
            InternalDebug.Assert(rawValue <= 0x07FFFFFF);
            this.rawValue = rawValue;
        }

        public bool     IsTransparent { get { return this.Transparency == 7; } }
        public uint     Transparency { get { return (rawValue >> 24) & 0x07; } }
        public uint     Red { get { return (rawValue >> 16) & 0xFF; } }
        public uint     Green { get { return (rawValue >> 8) & 0xFF; } }
        public uint     Blue { get { return rawValue & 0xFF; } }
        public override string ToString()
        {
            return this.IsTransparent ? "transparent" :
                "rgb(" + this.Red.ToString() + ", " + this.Green.ToString() + ", " + this.Blue.ToString() + ")" + (this.Transparency != 0 ? "+t" + this.Transparency.ToString() : "");
        }
    }

    internal struct PropertyValue
    {
        private const uint  TypeMask = 0xF8000000;
        private const int   TypeShift = 27;
        private const uint  ValueMask = 0x07FFFFFF;
        private const int   ValueShift = 5;

        public const int    ValueMax = 0x03FFFFFF;
        public const int    ValueMin = -ValueMax;

        private uint        rawValue;

        public static readonly PropertyValue Null = new();
        public static readonly PropertyValue True = new(true);
        public static readonly PropertyValue False = new(false);

        internal static readonly int[] sizesInTwips = [151, 200, 240, 271, 360, 480, 720];
        internal static readonly int[] maxSizesInTwips = [160, 220, 260, 320, 420, 620];

        public PropertyValue(bool value)
        {
            this.rawValue = ComposeRawValue(value);
        }

        

        public PropertyValue(PropertyType type, int value)
        {
            this.rawValue = ComposeRawValue(type, value);
        }     

        private static uint ComposeRawValue(bool value)
        {
            return (GetRawType(PropertyType.Bool) | (value ? 1u : 0u));
        }

        private static uint ComposeRawValue(PropertyType type, int value)
        {
            InternalDebug.Assert(ValueMin <= value && value <= ValueMax);
            return (((uint)type<<TypeShift) | ((uint)value & ValueMask));
        }

        public uint             RawType         { get { return (this.rawValue & TypeMask); } }

        public static uint GetRawType(PropertyType type)
        { 
            return ((uint)type << TypeShift); 
        }

        public PropertyType     Type            { get { return (PropertyType) ((this.rawValue & TypeMask) >> TypeShift); } }
        public int              Value           { get { return ((int)((this.rawValue & ValueMask) << ValueShift) >> ValueShift); } }     
        public uint             UnsignedValue   { get { return this.rawValue & ValueMask; } }

        public bool             IsAbsRelLength  { get { return this.RawType == GetRawType(PropertyType.AbsLength) ||
                                                            this.RawType == GetRawType(PropertyType.RelLength) ||
                                                            this.RawType == GetRawType(PropertyType.Pixels); } }

        public int              StringHandle    { get { InternalDebug.Assert(this.Type == PropertyType.String); return this.Value; } }
        public int              MultiValueHandle { get { InternalDebug.Assert(this.Type == PropertyType.MultiValue); return this.Value; } }
        public bool             Bool            { get { InternalDebug.Assert(this.Type == PropertyType.Bool); return this.UnsignedValue != 0; } }
        public int              Enum            { get { InternalDebug.Assert(this.Type == PropertyType.Enum); return (int)this.UnsignedValue; } }
        public RGBT             Color           { get { InternalDebug.Assert(this.Type == PropertyType.Color); return new RGBT(this.UnsignedValue); } }
        public float            Percentage      { get { InternalDebug.Assert(this.Type == PropertyType.Percentage); return (float) this.Value / (10000.0f); } }
        public float            Fractional      { get { InternalDebug.Assert(this.Type == PropertyType.Fractional); return (float) this.Value / (10000.0f); } }
        public int              Integer         { get { InternalDebug.Assert(this.Type == PropertyType.Integer); return this.Value; } }
        public int              Milliseconds    { get { InternalDebug.Assert(this.Type == PropertyType.Milliseconds); return this.Value; } }
        public int              kHz             { get { InternalDebug.Assert(this.Type == PropertyType.kHz); return this.Value; } }
        public int              Degrees         { get { InternalDebug.Assert(this.Type == PropertyType.Degrees); return this.Value; } }
        public float            Points          { get { InternalDebug.Assert(this.IsAbsRelLength); return (float) this.Value / (8.0f * 20.0f) ; } }
        public float            Inches          { get { InternalDebug.Assert(this.IsAbsRelLength); return (float) this.Value / (8.0f * 20.0f * 72.0f); } }
        public float            Millimeters     { get { InternalDebug.Assert(this.IsAbsRelLength); return (float) this.Value / ((8.0f * 20.0f * 72.0f) / 25.4f); } }
        public int              HtmlFontUnits   { get { InternalDebug.Assert(this.Type == PropertyType.HtmlFontUnits); return this.Value; } }
        public float            Pixels          { get { InternalDebug.Assert(this.IsAbsRelLength); return (float) this.Value / ((8.0f * 20.0f * 72.0f) / 120.0f); } }
        public int              PixelsInteger   { get { InternalDebug.Assert(this.IsAbsRelLength); return this.Value / 96; } }
        public float            Ems             { get { InternalDebug.Assert(this.Type == PropertyType.Ems); return (float) this.Value / (8.0f * 20.0f); } }
        public float            Exs             { get { InternalDebug.Assert(this.Type == PropertyType.Exs); return (float) this.Value / (8.0f * 20.0f); } }
        public int              RelativeHtmlFontUnits { get { InternalDebug.Assert(this.Type == PropertyType.RelHtmlFontUnits); return this.Value; } }

        public override string ToString()
        {
            switch (this.Type)
            {
                case PropertyType.Null:                 return "null";
                case PropertyType.Calculated:           return "calculated";
                case PropertyType.Bool:                 return this.Bool.ToString();
                case PropertyType.String:               return "string: " + this.StringHandle.ToString();
                case PropertyType.MultiValue:           return "multi: " + this.MultiValueHandle.ToString();
                case PropertyType.Enum:                 return "enum: " + this.Enum.ToString();
                case PropertyType.Color:                return "color: " + this.Color.ToString();
                case PropertyType.Integer:              return this.Integer.ToString() + " (integer)";
                case PropertyType.Fractional:           return this.Fractional.ToString() + " (fractional)";
                case PropertyType.Percentage:           return this.Percentage.ToString() + "%";
                case PropertyType.AbsLength:            return this.Points.ToString() + "pt (" + this.Inches.ToString() + "in, " + this.Millimeters.ToString() + "mm) (abs)";
                case PropertyType.RelLength:            return this.Points.ToString() + "pt (" + this.Inches.ToString() + "in, " + this.Millimeters.ToString() + "mm) (rel)";
                case PropertyType.HtmlFontUnits:        return this.HtmlFontUnits.ToString() + " (html font units)";
                case PropertyType.RelHtmlFontUnits:     return this.RelativeHtmlFontUnits.ToString() + " (relative html font units)";
                case PropertyType.Multiple:             return this.Integer.ToString() + "*";
                case PropertyType.Pixels:               return this.Pixels.ToString() + "px";
                case PropertyType.Ems:                  return this.Ems.ToString() + "em";
                case PropertyType.Exs:                  return this.Exs.ToString() + "ex";
                case PropertyType.Milliseconds:         return this.Milliseconds.ToString() + "ms";
                case PropertyType.kHz:                  return this.kHz.ToString() + "kHz";
                case PropertyType.Degrees:              return this.Degrees.ToString() + "deg";
            }

            return "unknown value type";
        }

        
        public static bool operator ==(PropertyValue x, PropertyValue y) 
        {
            return x.rawValue == y.rawValue;
        }

        
        public static bool operator !=(PropertyValue x, PropertyValue y) 
        {
            return x.rawValue != y.rawValue;
        }

        public override bool Equals(object obj)
        {
            return (obj is PropertyValue) && this.rawValue == ((PropertyValue)obj).rawValue;
        }

        public override int GetHashCode()
        {
            return (int)this.rawValue;
        }
    }

    

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct Property
    {
        private PropertyId id;
        private PropertyValue value;

        public static readonly Property Null = new();

        public override string ToString()
        {
            return this.id.ToString() + " = " + this.value.ToString();
        }
    }

    

    internal enum ListStyle
    {
        None,
        Bullet,
        Decimal,
        LowerAlpha,
        UpperAlpha,
        LowerRoman,
        UpperRoman,
#if false
        Disc,
        Bullet = Disc,
        Circle,
        Square,
        Decimal,
        DecimalLeadingZero,
        LowerRoman,
        UpperRoman,
        LowerGreek,
        LowerAlpha,
        LowerLatin,
        UpperAlpha,
        UpperLatin,
        Hebrew,
        Armenian,
        Georgian,
        CjkIdeographic,
        Hiragana,
        Katakana,
        HiraganaIroha,
        KatakanaIroha,
        None,
#endif
    }

    internal enum Align
    {
        Top = 0,
        Middle = 1,
        Center = Middle,
        Bottom = 2,
        Left = 3,
        Right = 4,
        BaseLine = 5,
        Justify = 6,
        Sub = 7,
        Super = 8,
        TextTop = 9,
        TextBottom = 10,
        Char = 11,
    }

    internal enum TextAlign
    {
        Left = Align.Left,
        Center = Align.Center,
        Right = Align.Right,
        Justify = Align.Justify,
    }

    internal enum BlockAlign
    {
        Top = Align.Top,
        Middle = Align.Middle,
        Bottom = Align.Bottom,
        Left = Align.Left,
        Right = Align.Right,
        Baseline = Align.BaseLine,
    }

    internal enum BlockHorizontalAlign
    {
        Left = Align.Left,
        Center = Align.Center,
        Right = Align.Right,
    }

    internal enum BlockVerticalAlign
    {
        Top = Align.Top,
        Middle = Align.Middle,
        Bottom = Align.Bottom,
        Baseline = Align.BaseLine,
    }

    internal enum LinkTarget
    {
        Self,
        Top,
        Blank,
        Parent
    }

    internal enum BorderStyle
    {
        None,
        Hidden,
        Dotted,
        Dashed,
        Solid,
        Double,
        Groove,
        Ridge,
        Inset,
        Outset,
    }

    internal enum UnicodeBiDi : byte
    {
        Normal,
        Embed,
        Override,
    }

    internal enum Display
    {
        None,
        Inline,
        Block,
        ListItem,
        RunIn,
        InlineBlock,
        Table,
        InlineTable,
        TableRowGroup,
        TableHeaderGroup,
        TableFooterGroup,
        TableRow,
        TableColumnGroup,
        TableColumn,
        TableCell,
        TableCaption,
    }
}
