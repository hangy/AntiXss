// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsciiEncoderFallback.cs" company="Microsoft Corporation">
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
// </copyright>
// <summary>
//   The fallback ASCII encoder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Text;

    using Internal;

    /// <summary>
    /// The fallback ASCII encoder.
    /// </summary>
    internal class AsciiEncoderFallback : EncoderFallback
    {
        /// <summary>
        /// Gets the maximum character count.
        /// </summary>
        public override int MaxCharCount
        {
            get
            {
                return AsciiFallbackBuffer.MaxCharCount;
            }
        }

        /// <summary>
        /// Encodes the specified character.
        /// </summary>
        /// <param name="charUnknown">
        /// The character to encode.
        /// </param>
        /// <returns>
        /// The encoded character.
        /// </returns>
        public static string GetCharacterFallback(char charUnknown)
        {
            if (charUnknown <= 0x153 &&
                charUnknown >= 0x82)
            {
                switch ((int)charUnknown)
                {
                    case 0x0083:
                        return "f";

                    case 0x0085:
                        return "...";

                    case 0x008B:
                        return "<";

                    case 0x008C:
                        return "OE";

                    case 0x009B:
                        return ">";

                    case 0x009C:
                        return "oe";

                    case 0x0082:
                    case 0x0091:
                    case 0x0092:
                        return "'";

                    case 0x0084:
                    case 0x0093:
                    case 0x0094:
                        return "\"";

                    case 0x0095:
                        return "*";

                    case 0x0096:
                        return "-";

                    case 0x0097:
                        return "-";

                    case 0x0098:
                        return "~";

                    case 0x0099:
                        return "(tm)";

                    case 0x00A0:
                        return " ";

                    case 0x00A2:
                        return "c";

                    case 0x00A4:
                        return "$";

                    case 0x00A5:
                        return "Y";

                    case 0x00A6:
                        return "|";

                    case 0x00A9:
                        return "(c)";

                    case 0x00AB:
                        return "<";

                    case 0x00AD:
                        return string.Empty;

                    case 0x00AE:
                        return "(r)";

                    case 0x00B2:
                        return "^2";

                    case 0x00B3:
                        return "^3";

                    case 0x00B7:
                        return "*";

                    case 0x00B8:
                        return ",";

                    case 0x00B9:
                        return "^1";

                    case 0x00BB:
                        return ">";

                    case 0x00BC:
                        return "(1/4)";

                    case 0x00BD:
                        return "(1/2)";

                    case 0x00BE:
                        return "(3/4)";

                    case 0x00C6:
                        return "AE";

                    case 0x00E6:
                        return "ae";

                    case 0x0132:
                        return "IJ";

                    case 0x0133:
                        return "ij";

                    case 0x0152:
                        return "OE";

                    case 0x0153:
                        return "oe";
                }
            }
            else if (charUnknown >= 0x2002 &&
                     charUnknown <= 0x2122)
            {
                switch ((int)charUnknown)
                {
                    case 0x2002:
                    case 0x2003:
                        return " ";

                    case 0x2011:
                        return "-";

                    case 0x2013:
                    case 0x2014:
                        return "-";

                    case 0x2018:
                    case 0x2019:
                    case 0x201A:
                        return "'";

                    case 0x201C:
                    case 0x201D:
                    case 0x201E:
                        return "\"";

                    case 0x2022:
                        return "*";

                    case 0x2026:
                        return "...";

                    case 0x2039:
                        return "<";

                    case 0x203A:
                        return ">";

                    case 0x20AC:
                        return "EUR";

                    case 0x2122:
                        return "(tm)";
                }
            }
            else if (charUnknown >= 0x2639 &&
                     charUnknown <= 0x263A)
            {
                switch ((int)charUnknown)
                {
                    case 0x263A:
                        return ":)";
                    case 0x2639:
                        return ":(";
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a fall back encoding buffer.
        /// </summary>
        /// <returns>
        /// A new <see cref="AsciiFallbackBuffer"/>.
        /// </returns>
        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new AsciiFallbackBuffer();
        }

        /// <summary>
        /// A buffer for the <see cref="AsciiEncoderFallback"/>.
        /// </summary>
        private sealed class AsciiFallbackBuffer : EncoderFallbackBuffer
        {
            /// <summary>
            /// The fallback index.
            /// </summary>
            private int fallbackIndex;

            /// <summary>
            /// The fallback string.
            /// </summary>
            private string fallbackString;

            /// <summary>
            /// Gets the maximum character count for the buffer.
            /// </summary>
            public static int MaxCharCount
            {
                get
                {
                    return 5;
                }
            }

            /// <summary>
            /// Gets the remaining number of characters in the buffer.
            /// </summary>
            public override int Remaining
            {
                get
                {
                    return this.fallbackString == null ? 0 : this.fallbackString.Length - this.fallbackIndex;
                }
            }

            /// <summary>
            /// Encodes the specified character using the fallback encoder.
            /// </summary>
            /// <param name="charUnknown">
            /// The unknown character to encode.
            /// </param>
            /// <param name="index">
            /// The index position in the buffer to encode into.
            /// </param>
            /// <returns>
            /// The encoded character.
            /// </returns>
            public override bool Fallback(char charUnknown, int index)
            {
                InternalDebug.Assert(this.fallbackString == null || this.fallbackIndex == this.fallbackString.Length);

                this.fallbackIndex = 0;

                this.fallbackString = GetCharacterFallback(charUnknown) ?? "?";

                return true;
            }

            /// <summary>
            /// Encodes the specified high/low character combination using the fallback encoder.
            /// </summary>
            /// <param name="charUnknownHigh">
            /// The high byte of the character to encode.
            /// </param>
            /// <param name="charUnknownLow">
            /// The low byte of the character to encode.
            /// </param>
            /// <param name="index">
            /// The index position in the buffer to encode into.
            /// </param>
            /// <returns>
            /// The encoded character.
            /// </returns>
            public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
            {
                InternalDebug.Assert(Char.IsHighSurrogate(charUnknownHigh) && Char.IsLowSurrogate(charUnknownLow));
                InternalDebug.Assert(this.fallbackString == null || this.fallbackIndex == this.fallbackString.Length);

                this.fallbackIndex = 0;

                this.fallbackString = "?";

                return true;
            }

            /// <summary>
            /// Gets the next character in the buffer.
            /// </summary>
            /// <returns>
            /// The next character in the buffer.
            /// </returns>
            public override char GetNextChar()
            {
                if (this.fallbackString == null ||
                    this.fallbackIndex == this.fallbackString.Length)
                {
                    return '\0';
                }

                return this.fallbackString[this.fallbackIndex++];
            }

            /// <summary>
            /// Moves to the previous character in the buffer.
            /// </summary>
            /// <returns>
            /// True if the move was sucessful, otherwise false.
            /// </returns>
            public override bool MovePrevious()
            {
                if (this.fallbackIndex > 0)
                {
                    this.fallbackIndex--;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Resets the buffer.
            /// </summary>
            public override void Reset()
            {
                this.fallbackString = "?";
                this.fallbackIndex = 0;
            }
        }
    }
}