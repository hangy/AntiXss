// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextOutput.cs" company="Microsoft Corporation">
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
//    Manages the preparation of encoding output.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;

    /// <summary>
    /// Image rendering callbak delegate.
    /// </summary>
    /// <param name="attachmentUrl">The attachement URL.</param>
    /// <param name="approximateRenderingPosition">The approximate rendering position.</param>
    /// <returns>
    /// <c>true</c> when image rendering callback is successful; otherwise <c>false</c>.
    /// </returns>
    internal delegate bool ImageRenderingCallbackInternal(string attachmentUrl, int approximateRenderingPosition);

    /// <summary>
    /// Manages the preparation of encoding output.
    /// </summary>
    internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
    {
        /// <summary>
        /// Array of white space characters.
        /// </summary>
        private static readonly char[] Whitespaces = { ' ', '\t', '\r', '\n', '\f' };

        #region Private Variables
        /// <summary>
        /// The encoded output.
        /// </summary>
        private ConverterOutput output;

        /// <summary>
        /// Use line wrapping.
        /// </summary>
        private bool lineWrapping;

        /// <summary>
        /// Use the text-plain format parameter.
        /// </summary>
        private bool rfc2646;

        /// <summary>
        /// The offset of the longest non wrapped paragraph found.
        /// </summary>
        private int longestNonWrappedParagraph;

        /// <summary>
        /// The offset before the wrap.
        /// </summary>
        private int wrapBeforePosition;

        /// <summary>
        /// Preserve trailing spaces.
        /// </summary>
        private bool preserveTrailingSpace;

        /// <summary>
        /// Preserve the tabs.
        /// </summary>
        private bool preserveTabulation;

        /// <summary>
        /// Preserve the non break spaces.
        /// </summary>
        private bool preserveNbsp;

        /// <summary>
        /// The length of the line.
        /// </summary>
        private int lineLength;

        /// <summary>
        /// The length of the line before the soft wrap.
        /// </summary>
        private int lineLengthBeforeSoftWrap;

        /// <summary>
        /// The lenght flushed.
        /// </summary>
        private int flushedLength;

        /// <summary>
        /// Number of tail spaces.
        /// </summary>
        private int tailSpace;

        /// <summary>
        /// The break opportunity.
        /// </summary>
        private int breakOpportunity;

        /// <summary>
        /// The next break opportunity.
        /// </summary>
        private int nextBreakOpportunity;

        /// <summary>
        /// The level of quotes.
        /// </summary>
        private int quotingLevel;

        /// <summary>
        /// True if buffer is wrapped.
        /// </summary>
        private bool wrapped;

        /// <summary>
        /// The wrap buffer.
        /// </summary>
        private char[] wrapBuffer;

        /// <summary>
        /// Possible signature.
        /// </summary>
        private bool signaturePossible = true;

        /// <summary>
        /// True if new lines.
        /// </summary>
        private bool anyNewlines;

        /// <summary>
        /// True if end of paragraph found.
        /// </summary>
        private bool endParagraph;

        /// <summary>
        /// When <c>true</c> write this object; otherwise, write null.
        /// </summary>
        private bool fallbacks;

        /// <summary>
        /// True if escaping html.
        /// </summary>
        private bool htmlEscape;

        /// <summary>
        /// The anchor URL.
        /// </summary>
        private string anchorUrl;

        /// <summary>
        /// The line position.
        /// </summary>
        private int linePosition;

        /// <summary>
        /// The image rendering callback delegate.
        /// </summary>
        private ImageRenderingCallbackInternal imageRenderingCallback;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutput"/> class.
        /// </summary>
        /// <param name="output">The converted output.</param>
        /// <param name="lineWrapping">if set to <c>true</c> allow line wrapping.</param>
        /// <param name="flowed">if set to <c>true</c> allow flowing.</param>
        /// <param name="wrapBeforePosition">The wrap before position.</param>
        /// <param name="longestNonWrappedParagraph">The longest non wrapped paragraph.</param>
        /// <param name="imageRenderingCallback">The image rendering callback.</param>
        /// <param name="fallbacks">if set to <c>true</c> allow fallbacks.</param>
        /// <param name="htmlEscape">if set to <c>true</c> escape HTML.</param>
        /// <param name="preserveSpace">if set to <c>true</c> preserve spaces.</param>
        /// <param name="testTraceStream">The test trace stream.</param>
        public TextOutput(
                ConverterOutput output,
                bool lineWrapping,
                bool flowed,
                int wrapBeforePosition,
                int longestNonWrappedParagraph,
                ImageRenderingCallbackInternal imageRenderingCallback,
                bool fallbacks,
                bool htmlEscape,
                bool preserveSpace,
                Stream testTraceStream)
        {
            this.rfc2646 = flowed;
            this.lineWrapping = lineWrapping;
            this.wrapBeforePosition = wrapBeforePosition;
            this.longestNonWrappedParagraph = longestNonWrappedParagraph;

            if (!this.lineWrapping)
            {
                this.preserveTrailingSpace = preserveSpace;
                this.preserveTabulation = preserveSpace;
                this.preserveNbsp = preserveSpace;
            }

            this.output = output;

            this.fallbacks = fallbacks;
            this.htmlEscape = htmlEscape;

            this.imageRenderingCallback = imageRenderingCallback;
            this.wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
        }

        /// <summary>
        /// Sets the anchor Url.
        /// </summary>
        /// <value>The anchor Url.</value>
        internal string AnchorUrl
        {
            set
            {
                this.anchorUrl = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether output code page is same as input.
        /// </summary>
        /// <value>
        /// <c>true</c> if the output code page is the same as input; otherwise, <c>false</c>.
        /// </value>
        internal bool OutputCodePageSameAsInput
        {
            get
            {
                if (this.output is ConverterEncodingOutput)
                {
                    return (this.output as ConverterEncodingOutput).CodePageSameAsInput;
                }

                return false;
            }
        }

        /// <summary>
        /// Sets the output encoding.
        /// </summary>
        /// <value>The output encoding.</value>
        internal Encoding OutputEncoding
        {
            set
            {
                if (this.output is ConverterEncodingOutput)
                {
                    (this.output as ConverterEncodingOutput).Encoding = value;
                    return;
                }

                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether image rendering callback is defined.
        /// </summary>
        /// <value>
        /// <c>true</c> if image rendering callback is defined; otherwise, <c>false</c>.
        /// </value>
        internal bool ImageRenderingCallbackDefined
        {
            get
            {
                return this.imageRenderingCallback != null;
            }
        }

        /// <summary>
        /// Gets the unsafe ASCII map.
        /// </summary>
        /// <param name="unsafeAsciiMask">The unsafe ASCII mask.</param>
        /// <returns>
        /// The unsafe ASCII mask byte[].
        /// </returns>
        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (this.htmlEscape)
            {
                unsafeAsciiMask = 0x01;
                return HtmlSupport.UnsafeAsciiMap;
            }

            unsafeAsciiMask = 0;
            return null;
        }

        /// <summary>
        /// Determines whether has unsafe unicode.
        /// </summary>
        /// <returns>
        /// <c>true</c> if has unsafe unicode; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.HasUnsafeUnicode()
        {
            return this.htmlEscape;
        }

        /// <summary>
        /// Treat the non ASCII as unsafe.
        /// </summary>
        /// <param name="charset">The charset.</param>
        /// <returns>
        /// Always returns False
        /// </returns>
        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return false;
        }

        /// <summary>
        /// Determines whether is unsafe unicode.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <param name="isFirstChar">if set to <c>true</c> is first character.</param>
        /// <returns>
        /// <c>true</c> if is unsafe unicode; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return this.htmlEscape &&
                ((byte)(ch & 0xFF) == (byte)'<' ||
                (byte)((ch >> 8) & 0xFF) == (byte)'<');
        }

        /// <summary>
        /// Encodes the character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputBufferCount">The output buffer count.</param>
        /// <param name="outputEnd">The output end.</param>
        /// <returns>
        /// <c>true</c> if encoding is successful; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (this.htmlEscape)
            {
                HtmlEntityIndex namedEntityId = 0;

                if (ch <= '>')
                {
                    if (ch == '>')
                    {
                        namedEntityId = HtmlEntityIndex.gt;
                    }
                    else if (ch == '<')
                    {
                        namedEntityId = HtmlEntityIndex.lt;
                    }
                    else if (ch == '&')
                    {
                        namedEntityId = HtmlEntityIndex.amp;
                    }
                    else if (ch == '\"')
                    {
                        namedEntityId = HtmlEntityIndex.quot;
                    }
                }
                else if ((char)0xA0 <= ch && ch <= (char)0xFF)
                {
                    namedEntityId = HtmlSupport.EntityMap[(int)ch - 0xA0];
                }

                if ((int)namedEntityId != 0)
                {
                    string strQuote = HtmlNameData.entities[(int)namedEntityId].name;

                    if (outputEnd - outputBufferCount < strQuote.Length + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    strQuote.CopyTo(0, outputBuffer, outputBufferCount, strQuote.Length);
                    outputBufferCount += strQuote.Length;
                    outputBuffer[outputBufferCount++] = ';';
                }
                else
                {
                    uint value = (uint)ch;
                    int len = (value < 0x10) ? 1 : (value < 0x100) ? 2 : (value < 0x1000) ? 3 : 4;
                    if (outputEnd - outputBufferCount < len + 4)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';
                    outputBuffer[outputBufferCount++] = 'x';

                    int offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        uint digit = value & 0xF;
                        outputBuffer[--offset] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }

                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ';';
                }
            }
            else
            {
                string substitute = AsciiEncoderFallback.GetCharacterFallback(ch);

                if (substitute != null)
                {
                    if (outputEnd - outputBufferCount < substitute.Length)
                    {
                        return false;
                    }

                    substitute.CopyTo(0, outputBuffer, outputBufferCount, substitute.Length);
                    outputBufferCount += substitute.Length;
                }
                else
                {
                    InternalDebug.Assert(outputEnd - outputBufferCount > 0);
                    outputBuffer[outputBufferCount++] = ch;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.output != null /*&& this.output is IDisposable*/)
            {
                ((IDisposable)this.output).Dispose();
            }

            this.output = null;
            this.wrapBuffer = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines whether this instance can restart.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can restart; otherwise, <c>false</c>.
        /// </returns>
        bool IRestartable.CanRestart()
        {
            if (this.output is IRestartable)
            {
                return ((IRestartable)this.output).CanRestart();
            }

            return false;
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            ((IRestartable)this.output).Restart();

            this.Reinitialize();
        }

        /// <summary>
        /// Disables the restart.
        /// </summary>
        void IRestartable.DisableRestart()
        {
            if (this.output is IRestartable)
            {
                ((IRestartable)this.output).DisableRestart();
            }
        }

        /// <summary>
        /// Initializes the specified new source or destination.
        /// </summary>
        /// <param name="newSourceOrDestination">The new source or destination.</param>
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(this.output is IReusable);

            ((IReusable)this.output).Initialize(newSourceOrDestination);

            this.Reinitialize();
        }

        /// <summary>
        /// Add line terminator to output.
        /// </summary>
        internal void CloseDocument()
        {
            if (!this.anyNewlines)
            {
                this.output.Write("\r\n");
            }

            this.endParagraph = false;
        }

        /// <summary>
        /// Add new line to the output.
        /// </summary>
        internal void OutputNewLine()
        {
            if (this.lineWrapping)
            {
                this.FlushLine('\n');

                if (this.signaturePossible && this.lineLength == 2 && this.tailSpace == 1)
                {
                    this.output.Write(' ');
                    this.lineLength++;
                }
            }
            else if (this.preserveTrailingSpace && this.tailSpace != 0)
            {
                this.FlushTailSpace();
            }

            if (!this.endParagraph)
            {
                this.output.Write("\r\n");
                this.anyNewlines = true;
                this.linePosition += 2;
            }

            this.linePosition += this.lineLength;

            this.lineLength = 0;
            this.lineLengthBeforeSoftWrap = 0;
            this.flushedLength = 0;
            this.tailSpace = 0;
            this.breakOpportunity = 0;
            this.nextBreakOpportunity = 0;
            this.wrapped = false;
            
            this.signaturePossible = true;
        }

        /// <summary>
        /// Add a tab to the output.
        /// </summary>
        /// <param name="count">The count.</param>
        internal void OutputTabulation(int count)
        {
            if (this.preserveTabulation)
            {
                while (count != 0)
                {
                    this.OutputNonspace("\t", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                int tabPosition = ((this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace) / 8 * 8) + (8 * count);
                count = tabPosition - (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace);

                this.OutputSpace(count);
            }
        }

        /// <summary>
        /// Add spaces to the output.
        /// </summary>
        /// <param name="count">The number of spaces to add.</param>
        internal void OutputSpace(int count)
        {
            InternalDebug.Assert(count != 0);

            if (this.lineWrapping)
            {
                if (this.breakOpportunity == 0 || this.lineLength + this.tailSpace <= this.WrapBeforePosition())
                {
                    this.breakOpportunity = this.lineLength + this.tailSpace;

                    InternalDebug.Assert(this.breakOpportunity >= 0);

                    if (this.lineLength + this.tailSpace < this.WrapBeforePosition() && count > 1)
                    {
                        this.breakOpportunity += Math.Min(this.WrapBeforePosition() - (this.lineLength + this.tailSpace), count - 1);
                    }

                    if (this.breakOpportunity < this.lineLength + this.tailSpace + count - 1)
                    {
                        this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
                    }

                    if (this.lineLength > this.flushedLength)
                    {
                        this.FlushLine(' ');
                    }
                }
                else
                {
                    this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
                }
            }

            this.tailSpace += count;

            InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
        }

        /// <summary>
        /// Add non breaking spaces to the output.
        /// </summary>
        /// <param name="count">The number of non breaking spaces to add.</param>
        internal void OutputNbsp(int count)
        {
            if (this.preserveNbsp)
            {
                while (count != 0)
                {
                    this.OutputNonspace("\xA0", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                this.tailSpace += count;
            }
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="buffer">The buffer to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (!this.lineWrapping && !this.endParagraph && textMapping == TextMapping.Unicode)
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                this.lineLength += count;
            }
            else
            {
                this.OutputNonspaceImpl(buffer, offset, count, textMapping);
            }
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="length">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
        {
            if (textMapping != TextMapping.Unicode)
            {
                for (int i = offset; i < length; i++)
                {
                    this.MapAndOutputSymbolCharacter(text[i], textMapping);
                }

                return;
            }

            if (this.endParagraph)
            {
                InternalDebug.Assert(this.lineLength == 0);

                this.output.Write("\r\n");
                this.linePosition += 2;
                this.anyNewlines = true;
                this.endParagraph = false;
            }

            if (this.lineWrapping)
            {
                if (length != 0)
                {
                    this.WrapPrepareToAppendNonspace(length);

                    if (this.breakOpportunity == 0)
                    {
                        this.FlushLine(text[offset]);

                        this.output.Write(text, offset, length, this.fallbacks ? this : null);

                        this.flushedLength += length;
                    }
                    else
                    {
                        text.CopyTo(offset, this.wrapBuffer, this.lineLength - this.flushedLength, length);
                    }

                    this.lineLength += length;

                    if (this.lineLength > 2 || text[offset] != '-' || (length == 2 && text[offset + 1] != '-'))
                    {
                        this.signaturePossible = false;
                    }
                }
            }
            else
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(text, offset, length, this.fallbacks ? this : null);

                this.lineLength += length;
            }
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="ucs32Literal">The ucs32 literal to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(int ucs32Literal, TextMapping textMapping)
        {
            if (textMapping != TextMapping.Unicode)
            {
                this.MapAndOutputSymbolCharacter((char)ucs32Literal, textMapping);
                return;
            }

            if (this.endParagraph)
            {
                InternalDebug.Assert(this.lineLength == 0);

                this.output.Write("\r\n");
                this.linePosition += 2;
                this.anyNewlines = true;
                this.endParagraph = false;
            }

            if (this.lineWrapping)
            {
                int count = Token.LiteralLength(ucs32Literal);
                this.WrapPrepareToAppendNonspace(count);

                if (this.breakOpportunity == 0)
                {
                    this.FlushLine(Token.LiteralFirstChar(ucs32Literal));

                    this.output.Write(ucs32Literal, this.fallbacks ? this : null);

                    this.flushedLength += count;
                }
                else
                {
                    this.wrapBuffer[this.lineLength - this.flushedLength] = Token.LiteralFirstChar(ucs32Literal);
                    if (count != 1)
                    {
                        this.wrapBuffer[this.lineLength - this.flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
                    }
                }

                this.lineLength += count;

                if (this.lineLength > 2 || count != 1 || (char)ucs32Literal != '-')
                {
                    this.signaturePossible = false;
                }
            }
            else
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(ucs32Literal, this.fallbacks ? this : null);

                this.lineLength += Token.LiteralLength(ucs32Literal);
            }
        }

        /// <summary>
        /// Flushes the converted output.
        /// </summary>
        internal void Flush()
        {
            if (this.lineWrapping)
            {
                if (this.lineLength != 0)
                {
                    this.FlushLine('\r');

                    this.OutputNewLine();
                }
            }
            else if (this.lineLength != 0)
            {
                this.OutputNewLine();
            }

            this.output.Flush();
        }

        /// <summary>
        /// Perform image encoding.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="imageAltText">The image alt text.</param>
        /// <param name="wdthPixels">The image width in pixels.</param>
        /// <param name="heightPixels">The image height in pixels.</param>
        internal void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
        {
            if (this.imageRenderingCallback != null && this.imageRenderingCallback(imageUrl, this.RenderingPosition()))
            {
                this.OutputSpace(1);
            }
            else
            {
                if ((wdthPixels == 0 || wdthPixels >= 8) && (heightPixels == 0 || heightPixels >= 8))
                {
                    bool addSpace = this.tailSpace != 0;

                    this.OutputNonspace("[", TextMapping.Unicode);

                    if (!string.IsNullOrEmpty(imageAltText))
                    {
                        int offset = 0;
                        while (offset != imageAltText.Length)
                        {
                            int nextOffset = imageAltText.IndexOfAny(Whitespaces, offset);

                            if (nextOffset == -1)
                            {
                                InternalDebug.Assert(imageAltText.Length - offset > 0);
                                this.OutputNonspace(imageAltText, offset, imageAltText.Length - offset, TextMapping.Unicode);
                                break;
                            }

                            if (nextOffset != offset)
                            {
                                this.OutputNonspace(imageAltText, offset, nextOffset - offset, TextMapping.Unicode);
                            }

                            if (imageAltText[offset] == '\t')
                            {
                                this.OutputTabulation(1);
                            }
                            else
                            {
                                this.OutputSpace(1);
                            }

                            offset = nextOffset + 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(imageUrl))
                    {
                        if (imageUrl.Contains("/") &&
                            !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            imageUrl = "X";
                        }
                        else if (imageUrl.IndexOf(' ') != -1)
                        {
                            imageUrl = imageUrl.Replace(" ", "%20");
                        }

                        this.OutputNonspace(imageUrl, TextMapping.Unicode);
                    }
                    else
                    {
                        this.OutputNonspace("X", TextMapping.Unicode);
                    }

                    this.OutputNonspace("]", TextMapping.Unicode);

                    if (addSpace)
                    {
                        this.OutputSpace(1);
                    }
                }
            }
        }

        /// <summary>
        /// Add non spaces to the output.
        /// </summary>
        /// <param name="text">The text the replace with non spaces.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(string text, TextMapping textMapping)
        {
            this.OutputNonspace(text, 0, text.Length, textMapping);
        }

        /// <summary>
        /// Closes the anchor.
        /// </summary>
        internal void CloseAnchor()
        {
            if (!String.IsNullOrEmpty(this.anchorUrl))
            {
                bool addSpace = this.tailSpace != 0;

                string urlString = this.anchorUrl;

                if (urlString.IndexOf(' ') != -1)
                {
                    urlString = urlString.Replace(" ", "%20");
                }

                this.OutputNonspace("<", TextMapping.Unicode);
                this.OutputNonspace(urlString, TextMapping.Unicode);
                this.OutputNonspace(">", TextMapping.Unicode);

                if (addSpace)
                {
                    this.OutputSpace(1);
                }

                this.CancelAnchor();
            }
        }

        /// <summary>
        /// Resets the anchor to null.
        /// </summary>
        internal void CancelAnchor()
        {
            this.anchorUrl = null;
        }

        /// <summary>
        /// Renderings the position.
        /// </summary>
        /// <returns>
        /// Offset for the rendering position.
        /// </returns>
        private int RenderingPosition()
        {
            return this.linePosition + this.lineLength + this.tailSpace;
        }

        /// <summary>
        /// Reinitializes this instance.
        /// </summary>
        private void Reinitialize()
        {
            this.anchorUrl = null;
            this.linePosition = 0;
            this.lineLength = 0;
            this.lineLengthBeforeSoftWrap = 0;
            this.flushedLength = 0;
            this.tailSpace = 0;
            this.breakOpportunity = 0;
            this.nextBreakOpportunity = 0;
            this.quotingLevel = 0;
            this.wrapped = false;
            this.signaturePossible = true;
            this.anyNewlines = false;
            this.endParagraph = false;
        }

        /// <summary>
        /// Perform encoding while handling end of paragraph and line wrapping.
        /// </summary>
        /// <param name="buffer">The buffer to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (count != 0)
            {
                if (textMapping != TextMapping.Unicode)
                {
                    for (int i = 0; i < count; i++)
                    {
                        this.MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
                    }

                    return;
                }

                if (this.endParagraph)
                {
                    InternalDebug.Assert(this.lineLength == 0);

                    this.output.Write("\r\n");
                    this.linePosition += 2;
                    this.anyNewlines = true;
                    this.endParagraph = false;
                }

                if (this.lineWrapping)
                {
                    this.WrapPrepareToAppendNonspace(count);

                    if (this.breakOpportunity == 0)
                    {
                        this.FlushLine(buffer[offset]);

                        this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                        this.flushedLength += count;
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset * 2, this.wrapBuffer, (this.lineLength - this.flushedLength) * 2, count * 2);
                    }

                    this.lineLength += count;

                    if (this.lineLength > 2 || buffer[offset] != '-' || (count == 2 && buffer[offset + 1] != '-'))
                    {
                        this.signaturePossible = false;
                    }
                }
                else
                {
                    if (this.tailSpace != 0)
                    {
                        this.FlushTailSpace();
                    }

                    this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                    this.lineLength += count;
                }
            }
        }

        /// <summary>
        /// Finds offset before the wrap.
        /// </summary>
        /// <returns>
        /// The offset before the wrap position.
        /// </returns>
        private int WrapBeforePosition()
        {
            return this.wrapBeforePosition - (this.rfc2646 ? this.quotingLevel + 1 : 0);
        }

        /// <summary>
        /// Find the Longest non wrapped paragraph.
        /// </summary>
        /// <returns>
        /// The offset at the beginning of the paragraph.
        /// </returns>
        private int LongestNonWrappedParagraph()
        {
            return this.longestNonWrappedParagraph - (this.rfc2646 ? this.quotingLevel + 1 : 0);
        }

        /// <summary>
        /// Wrap in preparation to append non space.
        /// </summary>
        /// <param name="count">The count.</param>
        private void WrapPrepareToAppendNonspace(int count)
        {
            InternalDebug.Assert(this.lineWrapping);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity > this.breakOpportunity);

            while (this.breakOpportunity != 0 &&
                this.lineLength + this.tailSpace + count
                    > (this.wrapped ? this.WrapBeforePosition() : this.LongestNonWrappedParagraph()))
            {
                InternalDebug.Assert(this.breakOpportunity >= this.flushedLength);

                if (this.flushedLength == 0 && this.rfc2646)
                {
                    for (int i = 0; i < this.quotingLevel; i++)
                    {
                        this.output.Write('>');
                    }

                    if (this.quotingLevel != 0 || this.wrapBuffer[0] == '>' || this.wrapBuffer[0] == ' ')
                    {
                        this.output.Write(' ');
                    }
                }

                if (this.breakOpportunity >= this.lineLength)
                {
                    InternalDebug.Assert(this.tailSpace >= this.breakOpportunity + 1 - this.lineLength);
                    InternalDebug.Assert(this.flushedLength == this.lineLength);

                    do
                    {
                        if (this.lineLength - this.flushedLength == this.wrapBuffer.Length)
                        {
                            this.output.Write(this.wrapBuffer, 0, this.wrapBuffer.Length, this.fallbacks ? this : null);
                            this.flushedLength += this.wrapBuffer.Length;
                        }

                        this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';

                        this.lineLength++;
                        this.tailSpace--;
                    }
                    while (this.lineLength != this.breakOpportunity + 1);
                }

                this.output.Write(this.wrapBuffer, 0, this.breakOpportunity + 1 - this.flushedLength, this.fallbacks ? this : null);

                this.anyNewlines = true;
                this.output.Write("\r\n");

                this.wrapped = true;
                this.lineLengthBeforeSoftWrap += this.breakOpportunity + 1;

                this.linePosition += this.breakOpportunity + 1 + 2;
                this.lineLength -= this.breakOpportunity + 1;

                InternalDebug.Assert(this.lineLength >= 0);

                int oldFlushedLength = this.flushedLength;
                this.flushedLength = 0;

                if (this.lineLength != 0)
                {
                    if (this.nextBreakOpportunity == 0 ||
                        this.nextBreakOpportunity - (this.breakOpportunity + 1) >= this.lineLength ||
                        this.nextBreakOpportunity - (this.breakOpportunity + 1) == 0)
                    {
                        if (this.rfc2646)
                        {
                            for (int i = 0; i < this.quotingLevel; i++)
                            {
                                this.output.Write('>');
                            }

                            if (this.quotingLevel != 0 || this.wrapBuffer[this.breakOpportunity + 1 - oldFlushedLength] == '>' || this.wrapBuffer[this.breakOpportunity + 1 - oldFlushedLength] == ' ')
                            {
                                this.output.Write(' ');
                            }
                        }

                        this.output.Write(this.wrapBuffer, this.breakOpportunity + 1 - oldFlushedLength, this.lineLength, this.fallbacks ? this : null);
                        this.flushedLength = this.lineLength;
                    }
                    else
                    {
                        Buffer.BlockCopy(this.wrapBuffer, (this.breakOpportunity + 1 - oldFlushedLength) * 2, this.wrapBuffer, 0, this.lineLength * 2);
                    }
                }

                if (this.nextBreakOpportunity != 0)
                {
                    InternalDebug.Assert(this.nextBreakOpportunity > this.breakOpportunity);

                    this.breakOpportunity = this.nextBreakOpportunity - (this.breakOpportunity + 1);

                    InternalDebug.Assert(this.breakOpportunity >= 0);
                    InternalDebug.Assert(this.breakOpportunity < this.lineLength || this.tailSpace >= this.breakOpportunity + 1 - this.lineLength);

                    if (this.breakOpportunity > this.WrapBeforePosition())
                    {
                        if (this.lineLength < this.WrapBeforePosition())
                        {
                            InternalDebug.Assert(this.tailSpace != 0);

                            this.nextBreakOpportunity = this.breakOpportunity;
                            this.breakOpportunity = this.WrapBeforePosition();
                        }
                        else if (this.breakOpportunity > this.lineLength)
                        {
                            InternalDebug.Assert(this.tailSpace != 0);

                            this.nextBreakOpportunity = this.breakOpportunity;
                            this.breakOpportunity = this.lineLength;
                        }
                        else
                        {
                            this.nextBreakOpportunity = 0;
                        }

                        InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
                        InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
                    }
                    else
                    {
                        this.nextBreakOpportunity = 0;
                    }
                }
                else
                {
                    this.breakOpportunity = 0;
                }

                InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
                InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
            }

            if (this.tailSpace != 0)
            {
                if (this.breakOpportunity == 0)
                {
                    InternalDebug.Assert(this.lineLength == this.flushedLength);

                    if (this.flushedLength == 0 && this.rfc2646)
                    {
                        for (int i = 0; i < this.quotingLevel; i++)
                        {
                            this.output.Write('>');
                        }

                        this.output.Write(' ');
                    }

                    this.flushedLength += this.tailSpace;
                    this.FlushTailSpace();
                }
                else
                {
                    InternalDebug.Assert(this.lineLength + this.tailSpace + count - this.flushedLength
                                                <= this.LongestNonWrappedParagraph());

                    do
                    {
                        this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';

                        this.lineLength++;
                        this.tailSpace--;
                    }
                    while (this.tailSpace != 0);
                }
            }

            InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
        }

        /// <summary>
        /// Flushes the line.
        /// </summary>
        /// <param name="nextChar">The next char.</param>
        private void FlushLine(char nextChar)
        {
            if (this.flushedLength == 0 && this.rfc2646)
            {
                for (int i = 0; i < this.quotingLevel; i++)
                {
                    this.output.Write('>');
                }

                char firstChar = this.lineLength != 0 ? this.wrapBuffer[0] : nextChar;
                if (this.quotingLevel != 0 || firstChar == '>' || firstChar == ' ')
                {
                    this.output.Write(' ');
                }
            }

            if (this.lineLength != this.flushedLength)
            {
                this.output.Write(this.wrapBuffer, 0, this.lineLength - this.flushedLength, this.fallbacks ? this : null);
                this.flushedLength = this.lineLength;
            }
        }

        /// <summary>
        /// Add spaces to the end of the output.
        /// </summary>
        private void FlushTailSpace()
        {
            InternalDebug.Assert(this.tailSpace != 0);

            this.lineLength += this.tailSpace;
            do
            {
                this.output.Write(' ');
                this.tailSpace--;
            }
            while (this.tailSpace != 0);
        }

        /// <summary>
        /// Encodes the symbol character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
        {
            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                this.OutputNonspace((int)ch, TextMapping.Unicode);
                return;
            }

            string substitute = null;

            if (textMapping == TextMapping.Wingdings)
            {
                switch ((int)ch)
                {
                    case 74:
                        substitute = "\x263A";
                        break;
                    case 75:
                        substitute = ":|";
                        break;
                    case 76:
                        substitute = "\x2639";
                        break;
                    case 216:
                        substitute = ">";
                        break;
                    case 223:
                        substitute = "<--";
                        break;
                    case 224:
                        substitute = "-->";
                        break;
                    case 231:
                        substitute = "<==";
                        break;
                    case 232:
                        substitute = "==>";
                        break;
                    case 239:
                        substitute = "<=";
                        break;
                    case 240:
                        substitute = "=>";
                        break;
                    case 243:
                        substitute = "<=>";
                        break;
                }
            }

            if (substitute == null)
            {
                substitute = "\x2022";
            }

            this.OutputNonspace(substitute, TextMapping.Unicode);
        }
    }
}

