// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlSupport.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;

    internal static class HtmlSupport
    {
        public const int HtmlNestingLimit = 4096;
        public const int MaxAttributeSize = 4096;
        public const int MaxCssPropertySize = 4096;
        public const int MaxNumberOfNonInlineStyles = 128;

        public static readonly byte[] UnsafeAsciiMap =
        {
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0,
            0,
            0x02,
            0x02,
            0,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0,
            0x02,
              0x03,
            0x02,
            0x02,
            0x02,
              0x03,
            0x02,
            0x02,
            0x02,
            0x02,
              0x03,
            0,
            0,
            0,
            0x02,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0x02,
            0x02,
              0x03,
            0x02,
              0x03,
            0x02,
            0x02,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0x02,
            0x02,
            0x02,
            0x02,
            0,
            0x02,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0x02,
            0x02,
            0x02,
            0x02,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
            0x03,
        };

        public static readonly HtmlEntityIndex[] EntityMap =
        {
#if false
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.quot,                                                  
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.amp,                                                   
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.lt,                                                    
            0,                                                                      
            HtmlEntityIndex.gt,                                                    
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
#endif
            HtmlEntityIndex.nbsp,
            HtmlEntityIndex.iexcl,
            HtmlEntityIndex.cent,
            HtmlEntityIndex.pound,
            HtmlEntityIndex.curren,
            HtmlEntityIndex.yen,
            HtmlEntityIndex.brvbar,
            HtmlEntityIndex.sect,
            HtmlEntityIndex.uml,
            HtmlEntityIndex.copy,
            HtmlEntityIndex.ordf,
            HtmlEntityIndex.laquo,
            HtmlEntityIndex.not,
            HtmlEntityIndex.shy,
            HtmlEntityIndex.reg,
            HtmlEntityIndex.macr,
            HtmlEntityIndex.deg,
            HtmlEntityIndex.plusmn,
            HtmlEntityIndex.sup2,
            HtmlEntityIndex.sup3,
            HtmlEntityIndex.acute,
            HtmlEntityIndex.micro,
            HtmlEntityIndex.para,
            HtmlEntityIndex.middot,
            HtmlEntityIndex.cedil,
            HtmlEntityIndex.sup1,
            HtmlEntityIndex.ordm,
            HtmlEntityIndex.raquo,
            HtmlEntityIndex.frac14,
            HtmlEntityIndex.frac12,
            HtmlEntityIndex.frac34,
            HtmlEntityIndex.iquest,
            HtmlEntityIndex.Agrave,
            HtmlEntityIndex.Aacute,
            HtmlEntityIndex.Acirc,
            HtmlEntityIndex.Atilde,
            HtmlEntityIndex.Auml,
            HtmlEntityIndex.Aring,
            HtmlEntityIndex.AElig,
            HtmlEntityIndex.Ccedil,
            HtmlEntityIndex.Egrave,
            HtmlEntityIndex.Eacute,
            HtmlEntityIndex.Ecirc,
            HtmlEntityIndex.Euml,
            HtmlEntityIndex.Igrave,
            HtmlEntityIndex.Iacute,
            HtmlEntityIndex.Icirc,
            HtmlEntityIndex.Iuml,
            HtmlEntityIndex.ETH,
            HtmlEntityIndex.Ntilde,
            HtmlEntityIndex.Ograve,
            HtmlEntityIndex.Oacute,
            HtmlEntityIndex.Ocirc,
            HtmlEntityIndex.Otilde,
            HtmlEntityIndex.Ouml,
            HtmlEntityIndex.times,
            HtmlEntityIndex.Oslash,
            HtmlEntityIndex.Ugrave,
            HtmlEntityIndex.Uacute,
            HtmlEntityIndex.Ucirc,
            HtmlEntityIndex.Uuml,
            HtmlEntityIndex.Yacute,
            HtmlEntityIndex.THORN,
            HtmlEntityIndex.szlig,
            HtmlEntityIndex.agrave,
            HtmlEntityIndex.aacute,
            HtmlEntityIndex.acirc,
            HtmlEntityIndex.atilde,
            HtmlEntityIndex.auml,
            HtmlEntityIndex.aring,
            HtmlEntityIndex.aelig,
            HtmlEntityIndex.ccedil,
            HtmlEntityIndex.egrave,
            HtmlEntityIndex.eacute,
            HtmlEntityIndex.ecirc,
            HtmlEntityIndex.euml,
            HtmlEntityIndex.igrave,
            HtmlEntityIndex.iacute,
            HtmlEntityIndex.icirc,
            HtmlEntityIndex.iuml,
            HtmlEntityIndex.eth,
            HtmlEntityIndex.ntilde,
            HtmlEntityIndex.ograve,
            HtmlEntityIndex.oacute,
            HtmlEntityIndex.ocirc,
            HtmlEntityIndex.otilde,
            HtmlEntityIndex.ouml,
            HtmlEntityIndex.divide,
            HtmlEntityIndex.oslash,
            HtmlEntityIndex.ugrave,
            HtmlEntityIndex.uacute,
            HtmlEntityIndex.ucirc,
            HtmlEntityIndex.uuml,
            HtmlEntityIndex.yacute,
            HtmlEntityIndex.thorn,
            HtmlEntityIndex.yuml,
        };

        [Flags]
        public enum NumberParseFlags
        {
            Integer = 0x0001,
            Float = 0x0002,
            AbsoluteLength = 0x0004,
            EmExLength = 0x0008,
            Percentage = 0x0010,
            Multiple = 0x0020,
            HtmlFontUnits = 0x0040,

            NonNegative = 0x2000,
            StyleSheetProperty = 0x4000,
            Strict = 0x8000,

            Length = AbsoluteLength | EmExLength | Percentage,
            NonNegativeLength = AbsoluteLength | EmExLength | Percentage | NonNegative,
            FontSize = AbsoluteLength | EmExLength | Percentage | HtmlFontUnits | NonNegative,
        }

        public static PropertyValue ParseNumber(BufferString value, NumberParseFlags parseFlags)
        {
            bool isValidNumber = false;
            bool isSigned = false;
            bool isNegative = false;
            ulong result = 0;
            int exponent = 0;
            int scientificExponent = 0;
            bool floatNumber = false;
            int offset = 0;
            int end = value.Length;

            while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                offset++;
            }

            if (offset == end)
            {
                return PropertyValue.Null;
            }

            if (offset < end && (value[offset] == '-' || value[offset] == '+'))
            {
                isSigned = true;
                isNegative = (value[offset] == '-');
                offset ++;
            }

            while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                isValidNumber = true;
                if (result < ulong.MaxValue / 10 - 9)
                {
                    result = unchecked(result * 10u + (uint)(value[offset] - '0'));
                }
                else
                {
                    exponent++;
                }
                offset++;
            }

            if (offset < end && value[offset] == '.')
            {
                floatNumber = true;
                offset++;

                while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
                {
                    isValidNumber = true;

                    if (result < ulong.MaxValue / 10 - 9)
                    {
                        result = unchecked(result * 10u + (uint)(value[offset] - '0'));
                        exponent--;
                    }

                    offset++;
                }

                if (exponent >= 0 && 0 != (parseFlags & NumberParseFlags.Strict))
                {
                    return PropertyValue.Null;
                }
            }

            if (!isValidNumber)
            {
                return PropertyValue.Null;
            }

            while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                offset++;
            }

            if (offset < end && (value[offset] | '\x20') == 'e')
            {
                if (offset + 1 < end && (value[offset + 1] == '-' || value[offset + 1] == '+' || ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset + 1]))))
                {
                    floatNumber = true;
                    offset ++;

                    bool isNegativeScientificExponent = false;

                    if (value[offset] == '-' || value[offset] == '+')
                    {
                        isNegativeScientificExponent = (value[offset] == '-');
                        offset ++;
                    }

                    while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
                    {
                        scientificExponent = unchecked(scientificExponent * 10 + (value[offset++] - '0'));
                    }

                    if (isNegativeScientificExponent)
                    {
                        scientificExponent = -scientificExponent;
                    }

                    while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
                    {
                        offset++;
                    }
                }
            }

            uint mul = floatNumber ? 10000u : 1;
            uint div = 1;
            PropertyType units = floatNumber ? PropertyType.Fractional : PropertyType.Integer;
            bool recognized = false;
            int typeLength = 0;

            if (offset + 1 < end)
            {
                if ((value[offset] | '\x20') == 'p')
                {
                    if ((value[offset + 1] | '\x20') == 'c')
                    {
                        mul = 8 * 20 * 12;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 't')
                    {
                        mul = 8 * 20;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 'x')
                    {
                        mul = 8 * 20 * 72;
                        div = 120;
                        units = PropertyType.Pixels;
                        recognized = true;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'e')
                {
                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        mul = 8 * 20;
                        div = 1;
                        units = PropertyType.Ems;
                        recognized = true;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 'x')
                    {
                        mul = 8 * 20;
                        div = 1;
                        units = PropertyType.Exs;
                        recognized = true;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'i')
                {
                    if ((value[offset + 1] | '\x20') == 'n')
                    {
                        mul = 8 * 20 * 72;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'c')
                {
                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        mul = 8 * 20 * 72 * 100;
                        div = 254;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'm')
                {
                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        mul = 8 * 20 * 72 * 10;
                        div = 254;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
            }

            if (!recognized && offset < end)
            {
                if (value[offset] == '%')
                {
                    mul = 10000;
                    div = 1;
                    units = PropertyType.Percentage;
                    recognized = true;
                    typeLength = 1;
                }
                else if (value[offset] == '*')
                {
                    mul = 1;
                    div = 1;
                    units = PropertyType.Multiple;
                    recognized = true;
                    typeLength = 1;
                }
            }

            offset += typeLength;

            if (offset < end)
            {
                while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
                {
                    offset++;
                }

                if (offset < end)
                {
                    if (0 != (parseFlags & (NumberParseFlags.Strict | NumberParseFlags.StyleSheetProperty)))
                    {
                        return PropertyValue.Null;
                    }
                }
            }

            if (result != 0)
            {
                int actualExponent = exponent + scientificExponent;

                if (actualExponent > 0)
                {
                    if (actualExponent > 20)
                    {
                        actualExponent = 0;
                        result = ulong.MaxValue;
                    }
                    else
                    {
                        while (actualExponent != 0)
                        {
                            if (result > ulong.MaxValue / 10)
                            {
                                actualExponent = 0;
                                result = ulong.MaxValue;
                                break;
                            }
                            else
                            {
                                result = unchecked(result * 10);
                            }
                            actualExponent --;
                        }
                    }
                }
                else if (actualExponent < -10)
                {
                    if (actualExponent < -21)
                    {
                        actualExponent = 0;
                        result = 0;
                    }
                    else
                    {
                        while (actualExponent != -10)
                        {
                            result /= 10;
                            actualExponent ++;
                        }
                    }
                }

                result *= mul;
                result /= div;

                while (actualExponent != 0)
                {
                    result /= 10;
                    actualExponent ++;
                }

                if (result > PropertyValue.ValueMax)
                {
                    result = PropertyValue.ValueMax;
                }
            }

            int intValue = unchecked((int)result);
            if (isNegative)
            {
                intValue = -intValue;
            }

            if (units == PropertyType.Integer)
            {
                if (0 != (parseFlags & NumberParseFlags.Integer))
                {
                }
                else if (0 != (parseFlags & NumberParseFlags.HtmlFontUnits))
                {
                    if (isSigned)
                    {
                        if (intValue < -7)
                        {
                            intValue = -7;
                        }
                        else if (intValue > 7)
                        {
                            intValue = 7;
                        }
                        units = PropertyType.RelHtmlFontUnits;
                    }
                    else
                    {
                        if (intValue < 1)
                        {
                            intValue = 1;
                        }
                        else if (intValue > 7)
                        {
                            intValue = 7;
                        }
                        units = PropertyType.HtmlFontUnits;
                    }
                }
                else if (0 != (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    result = result * (8 * 20 * 72) / 120;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Pixels;
                }
                else if (0 != (parseFlags & NumberParseFlags.Float))
                {
                    result *= 10000;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Fractional;
                }
                else
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Fractional)
            {
                if (0 != (parseFlags & NumberParseFlags.Float))
                {
                }
                else if (0 != (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    result = result * (8 * 20 * 72) / 120 / 10000;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Pixels;
                }
                else
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.AbsLength || units == PropertyType.Pixels)
            {
                if (0 == (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Ems || units == PropertyType.Exs)
            {
                if (0 == (parseFlags & NumberParseFlags.EmExLength))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Percentage)
            {
                if (0 == (parseFlags & NumberParseFlags.Percentage))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Multiple)
            {
                if (0 == (parseFlags & NumberParseFlags.Multiple))
                {
                    return PropertyValue.Null;
                }
            }

            if (intValue < 0 && 0 != (parseFlags & NumberParseFlags.NonNegative) && units != PropertyType.RelHtmlFontUnits)
            {
                return PropertyValue.Null;
            }

            return new PropertyValue(units, intValue);
        }
    }
}
