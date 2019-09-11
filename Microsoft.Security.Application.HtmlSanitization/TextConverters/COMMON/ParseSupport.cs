// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseSupport.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;

    [Flags]
    internal enum CharClass : uint
    {
        Invalid = 0,
        NotInterestingText = 0x00000001,
        Control = 0x00000002,
        Whitespace = 0x00000004,
        Alpha = 0x00000008,
        Numeric = 0x00000010,
        Backslash = 0x00000020,
        LessThan = 0x00000040,
        Equals = 0x00000080,
        GreaterThan = 0x00000100,
        Solidus = 0x00000200,
        Ampersand = 0x00000400,
        Nbsp = 0x00000800,
        Comma = 0x00001000,
        SingleQuote = 0x00002000,
        DoubleQuote = 0x00004000,
        GraveAccent = 0x00008000,
        Circumflex = 0x00010000,
        VerticalLine = 0x00020000,
        Parentheses = 0x00040000,
        CurlyBrackets = 0x00080000,
        SquareBrackets = 0x00100000,
        Tilde = 0x00200000,
        Colon = 0x00400000,
        UniqueMask = 0x00FFFFFF,

        AlphaHex = 0x80000000,
        HtmlSuffix = 0x40000000,
        RtfInteresting = 0x20000000,
        OverlappedMask = 0xFF000000,

        Quote = SingleQuote | DoubleQuote | GraveAccent,
        Brackets = CurlyBrackets | SquareBrackets,
        NonWhitespaceText = UniqueMask & ~(Whitespace | Nbsp),
        NonWhitespaceNonControlText = NonWhitespaceText & ~Control,
        HtmlNonWhitespaceText = NonWhitespaceText & ~(LessThan | Ampersand),
        NonWhitespaceNonUri = LessThan | GreaterThan | Brackets | DoubleQuote | GraveAccent | Ampersand | Circumflex | VerticalLine | Tilde,
        NonWhitespaceUri = UniqueMask & ~(Whitespace | Nbsp | NonWhitespaceNonUri),

        HtmlTagName = UniqueMask & ~(Whitespace | GreaterThan | Solidus),
        HtmlTagNamePrefix = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Colon),
        HtmlAttrName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals),
        HtmlAttrNamePrefix = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals | Colon),
        HtmlAttrValue = UniqueMask & ~(Whitespace | GreaterThan | Quote | Ampersand),
        HtmlScanQuoteSensitive = Whitespace | Equals,
        HtmlEntity = Alpha | Numeric,

        HtmlSimpleTagName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Quote | LessThan | Colon),
        HtmlEndTagName = Whitespace | GreaterThan | Solidus,
        HtmlSimpleAttrName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals | Quote | Colon),
        HtmlEndAttrName = Whitespace | GreaterThan | Solidus | Equals,
        HtmlSimpleAttrQuotedValue = UniqueMask & ~(Ampersand | Quote),
        HtmlSimpleAttrUnquotedValue = UniqueMask & ~(Whitespace | GreaterThan | Quote | Ampersand),
        HtmlEndAttrUnquotedValue = Whitespace | GreaterThan,

        Hex = Numeric | AlphaHex,
    }

    [Flags]
    internal enum DbcsLeadBits : byte
    {
        Lead1361 = 0x01,
        Lead10001 = 0x02,
        Lead10002 = 0x04,
        Lead10003 = 0x08,
        Lead10008 = 0x10,
        Lead932 = 0x20,
        Lead9XX = 0x40,
    }

    internal static class ParseSupport
    {
        private static readonly char[] latin1MappingInUnicodeControlArea =
        {
            (char) 0x20ac,
            (char) 0x0081,
            (char) 0x201a,
            (char) 0x0192,
            (char) 0x201e,
            (char) 0x2026,
            (char) 0x2020,
            (char) 0x2021,
            (char) 0x02c6,
            (char) 0x2030,
            (char) 0x0160,
            (char) 0x2039,
            (char) 0x0152,
            (char) 0x008d,
            (char) 0x017d,
            (char) 0x008f,
            (char) 0x0090,
            (char) 0x2018,
            (char) 0x2019,
            (char) 0x201c,
            (char) 0x201d,
            (char) 0x2022,
            (char) 0x2013,
            (char) 0x2014,
            (char) 0x02dc,
            (char) 0x2122,
            (char) 0x0161,
            (char) 0x203a,
            (char) 0x0153,
            (char) 0x009d,
            (char) 0x017e,
            (char) 0x0178
        };

        private static readonly byte[] charToHexTable =
        {
            0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        private static readonly CharClass[] lowCharClass =
        {
            CharClass.Invalid | CharClass.RtfInteresting,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Whitespace,
            CharClass.Whitespace,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Whitespace,
            CharClass.NotInterestingText,
            CharClass.DoubleQuote,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText | CharClass.HtmlSuffix,
            CharClass.Ampersand,
            CharClass.SingleQuote,
            CharClass.Parentheses,
            CharClass.Parentheses,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.Comma,
            CharClass.NotInterestingText | CharClass.HtmlSuffix,
            CharClass.NotInterestingText,
            CharClass.Solidus,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Colon,
            CharClass.NotInterestingText,
            CharClass.LessThan,
            CharClass.Equals,
            CharClass.GreaterThan | CharClass.HtmlSuffix,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.SquareBrackets,
            CharClass.Backslash | CharClass.RtfInteresting,
            CharClass.SquareBrackets | CharClass.HtmlSuffix,
            CharClass.Circumflex,
            CharClass.NotInterestingText,
            CharClass.GraveAccent,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha | CharClass.AlphaHex,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.CurlyBrackets | CharClass.RtfInteresting,
            CharClass.VerticalLine,
            CharClass.CurlyBrackets | CharClass.RtfInteresting,
            CharClass.Tilde,
            CharClass.Control,

            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,

            CharClass.Nbsp,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
        };

        public static int CharToDecimal(char ch)
        {
            unchecked
            {
                InternalDebug.Assert(NumericCharacter(GetCharClass(ch)));
                return (int)(ch - '0');
            }
        }

        public static int CharToHex(char ch)
        {
            InternalDebug.Assert(HexCharacter(GetCharClass(ch)));
            return (int)charToHexTable[ch & 0x1F];
        }

        public static char HighSurrogateCharFromUcs4(int ich)
        {
            return (char)(0xd800 + ((ich - 0x10000) >> 10));
        }

        public static char LowSurrogateCharFromUcs4(int ich)
        {
            return (char)(0xdc00 + (ich & 0x3ff));
        }

        public static bool IsCharClassOneOf(CharClass charClass, CharClass charClassSet)
        {
            return (charClass & charClassSet) != 0;
        }

        public static bool InvalidUnicodeCharacter(CharClass charClass)
        {
            return (charClass & CharClass.UniqueMask) == 0;
        }

        public static bool HtmlTextCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlNonWhitespaceText) != 0;
        }

        public static bool WhitespaceCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Whitespace) != 0;
        }

        public static bool WhitespaceCharacter(char ch)
        {
            return (GetCharClass(ch) & CharClass.Whitespace) != 0;
        }

        public static bool NbspCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Nbsp) != 0;
        }

        public static bool AlphaCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Alpha) != 0;
        }

        public static bool QuoteCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Quote) != 0;
        }

        public static bool HtmlAttrValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrValue) != 0;
        }

        public static bool HtmlScanQuoteSensitiveCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlScanQuoteSensitive) != 0;
        }

        public static bool HtmlSimpleTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleTagName) != 0;
        }

        public static bool HtmlEndTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndTagName) != 0;
        }

        public static bool HtmlSimpleAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrName) != 0;
        }

        public static bool HtmlEndAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrName) != 0;
        }

        public static bool HtmlSimpleAttrQuotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrQuotedValue) != 0;
        }

        public static bool HtmlSimpleAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrUnquotedValue) != 0;
        }

        public static bool HtmlEndAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrUnquotedValue) != 0;
        }

        public static bool NumericCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Numeric) != 0;
        }

        public static bool HexCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Hex) != 0;
        }

        public static bool HtmlEntityCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEntity) != 0;
        }

        public static bool HtmlSuffixCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSuffix) != 0;
        }

        public static CharClass GetCharClass(char ch)
        {
            return (int)ch <= 0xFF/*lowCharClass.Length*/ ? lowCharClass[(int)ch] : GetHighCharClass(ch);
        }

        public static CharClass GetHighCharClass(char ch)
        {
            return ch < (char)0xFDD0 ? CharClass.NotInterestingText :
                    (!((char)0xFFF9 <= ch && ch <= (char)0xFFFD) && !((char)0xFDF0 <= ch && ch <= (char)0xFFEF))
                        ? CharClass.Invalid : CharClass.NotInterestingText;
        }

        public static bool IsUpperCase(char ch)
        {
            return (uint)(ch - 'A') <= ('Z' - 'A');
        }

        public static char ToLowerCase(char ch)
        {
            return IsUpperCase(ch) ? (char)(ch + ('a' - 'A')) : ch;
        }

        public static int Latin1MappingInUnicodeControlArea(int value)
        {
            InternalDebug.Assert(0x80 <= value && value <= 0x9F);
            return (int)ParseSupport.latin1MappingInUnicodeControlArea[value - 0x80];
        }

        public static bool TwoFarEastNonHanguelChars(char ch1, char ch2)
        {
            if (ch1 < (char)0x3000 || ch2 < (char)0x3000)
            {
                return false;
            }

            return !HanguelRange(ch1) && !HanguelRange(ch2);
        }

        public static bool FarEastNonHanguelChar(char ch)
        {
            return ch >= (char)0x3000 && !HanguelRange(ch);
        }

        private static bool HanguelRange(char ch)
        {
            InternalDebug.Assert(ch >= (char)0x3000);

            return /*ch > 0x10ff &&*/
                (/*(0x1100 <= ch && ch <= 0x11f9) ||*/
                (0x3130 <= ch && ch <= 0x318f) ||
                (0xac00 <= ch && ch <= 0xd7a3) ||
                (0xffa1 <= ch && ch <= 0xffdc));
        }
    }
}

