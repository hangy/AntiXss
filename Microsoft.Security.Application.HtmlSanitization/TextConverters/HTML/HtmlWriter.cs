// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlWriter.cs" company="Microsoft Corporation">
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
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    internal enum HtmlWriterState
    {
        Default,

        Tag,

        Attribute,
    }

    internal class HtmlWriter : IRestartable, IFallback, IDisposable, ITextSinkEx
    {
        private ConverterOutput output;
        private OutputState outputState;

        private bool filterHtml;

        private bool autoNewLines;

        private bool allowWspBeforeFollowingTag;
        private bool lastWhitespace;

        private int lineLength;
        private int longestLineLength;
        private int textLineLength;

        private int literalWhitespaceNesting;
        private bool literalTags;
        private bool literalEntities;
        private bool cssEscaping;

        private IFallback fallback;

        private HtmlNameIndex tagNameIndex;
        private HtmlNameIndex previousTagNameIndex;
        private bool isEndTag;
        private bool isEmptyScopeTag;

        private bool copyPending;

        internal enum OutputState
        {
            OutsideTag,
            TagStarted,
            WritingUnstructuredTagContent,
            WritingTagName,
            BeforeAttribute,
            WritingAttributeName,
            AfterAttributeName,
            WritingAttributeValue,
        }

        internal HtmlWriter(ConverterOutput output, bool filterHtml, bool autoNewLines)
        {
            this.output = output;
            this.filterHtml = filterHtml;
            this.autoNewLines = autoNewLines;
        }

        internal bool HasEncoding
        {
            get { return this.output is ConverterEncodingOutput; }
        }

        internal bool CodePageSameAsInput
        {
            get
            {
                InternalDebug.Assert(this.output is ConverterEncodingOutput);
                return (this.output as ConverterEncodingOutput).CodePageSameAsInput;
            }
        }

        internal Encoding Encoding
        {
            get
            {
                InternalDebug.Assert(this.output is ConverterEncodingOutput);
                return (this.output as ConverterEncodingOutput).Encoding;
            }

            set
            {
                InternalDebug.Assert(this.output is ConverterEncodingOutput);
                (this.output as ConverterEncodingOutput).Encoding = value;
            }
        }

        internal bool IsTagOpen
        {
            get { return this.outputState != OutputState.OutsideTag; }
        }

        public void Flush()
        {
            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            this.output.Flush();
        }

        internal void SetCopyPending(bool copyPending)
        {
            this.copyPending = copyPending;
        }

        internal void WriteStartTag(HtmlNameIndex nameIndex)
        {
            this.WriteTagBegin(nameIndex, null, false, false, false);
        }

        internal void WriteEndTag(HtmlNameIndex nameIndex)
        {
            this.WriteTagBegin(nameIndex, null, true, false, false);
            this.WriteTagEnd();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        internal void WriteTagBegin(HtmlNameIndex nameIndex, string name, bool isEndTag, bool allowWspLeft, bool allowWspRight)
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

#if !BETTER_FUZZING
            if (this.literalTags && nameIndex >= HtmlNameIndex.Unknown && (!isEndTag || nameIndex != this.tagNameIndex))
            {
                throw new InvalidOperationException(Strings.CannotWriteOtherTagsInsideElement(HtmlNameData.names[(int)this.tagNameIndex].name));
            }
#endif
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;

            if (nameIndex > HtmlNameIndex.Unknown)
            {
                this.isEmptyScopeTag = (HtmlDtd.tags[(int)tagIndex].scope == HtmlDtd.TagScope.EMPTY);

                if (isEndTag && this.isEmptyScopeTag)
                {
                    if (HtmlDtd.tags[(int)tagIndex].unmatchedSubstitute != HtmlTagIndex._IMPLICIT_BEGIN)
                    {
                        this.output.Write("<!-- </");
                        this.lineLength += 7;
                        if (nameIndex > HtmlNameIndex.Unknown)
                        {
                            this.output.Write(HtmlNameData.names[(int)nameIndex].name);
                            this.lineLength += HtmlNameData.names[(int)nameIndex].name.Length;
                        }
                        else
                        {
                            this.output.Write(name != null ? name : "???");
                            this.lineLength += name != null ? name.Length : 3;
                        }
                        this.output.Write("> ");
                        this.lineLength += 2;
                        this.tagNameIndex = HtmlNameIndex._COMMENT;
                        this.outputState = OutputState.WritingUnstructuredTagContent;

                        return;
                    }

                    isEndTag = false;
                }
            }

            InternalDebug.Assert(0 == (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Entities) ||
                                0 != (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Tags));

            if (this.autoNewLines && this.literalWhitespaceNesting == 0)
            {
                bool hadWhitespaceBeforeTag = this.lastWhitespace;
                HtmlDtd.TagFill tagFill = HtmlDtd.tags[(int)tagIndex].fill;

                if (this.lineLength != 0)
                {
                    HtmlDtd.TagFmt tagFmt = HtmlDtd.tags[(int)tagIndex].fmt;

                    if ((!isEndTag && tagFmt.LB == HtmlDtd.FmtCode.BRK) ||
                        (isEndTag && tagFmt.LE == HtmlDtd.FmtCode.BRK) ||
                        (this.lineLength > 80 &&
                        (this.lastWhitespace ||
                        this.allowWspBeforeFollowingTag ||
                        (!isEndTag && tagFill.LB == HtmlDtd.FillCode.EAT) ||
                        (isEndTag && tagFill.LE == HtmlDtd.FillCode.EAT))))
                    {
                        if (this.lineLength > this.longestLineLength)
                        {
                            this.longestLineLength = this.lineLength;
                        }

                        this.output.Write("\r\n");
                        this.lineLength = 0;
                        this.lastWhitespace = false;
                    }
                }

                this.allowWspBeforeFollowingTag = ((!isEndTag && tagFill.RB == HtmlDtd.FillCode.EAT) ||
                                                (isEndTag && tagFill.RE == HtmlDtd.FillCode.EAT) ||
                                                hadWhitespaceBeforeTag &&
                                                ((!isEndTag && tagFill.RB == HtmlDtd.FillCode.NUL) ||
                                                (isEndTag && tagFill.RE == HtmlDtd.FillCode.NUL))) &&
                                                (nameIndex != HtmlNameIndex.Body || !isEndTag);
            }

            if (this.lastWhitespace)
            {
                this.output.Write(' ');
                this.lineLength++;
                this.lastWhitespace = false;
            }

            if (HtmlDtd.tags[(int)tagIndex].blockElement || tagIndex == HtmlTagIndex.BR)
            {
                this.textLineLength = 0;
            }

            this.output.Write('<');
            this.lineLength++;

            if (nameIndex >= HtmlNameIndex.Unknown)
            {
                if (isEndTag)
                {
                    if (0 != (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Tags))
                    {
                        this.literalTags = false;

                        this.literalEntities = false;
                        this.cssEscaping = false;
                    }

                    if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                    {
                        this.literalWhitespaceNesting--;
                    }

                    this.output.Write('/');
                    this.lineLength++;
                }

                if (nameIndex != HtmlNameIndex.Unknown)
                {
                    this.output.Write(HtmlNameData.names[(int)nameIndex].name);
                    this.lineLength += HtmlNameData.names[(int)nameIndex].name.Length;

                    this.outputState = OutputState.BeforeAttribute;
                }
                else
                {
                    if (name != null)
                    {
                        this.output.Write(name);
                        this.lineLength += name.Length;

                        this.outputState = OutputState.BeforeAttribute;
                    }
                    else
                    {
                        this.outputState = OutputState.TagStarted;
                    }

                    this.isEmptyScopeTag = false;
                }
            }
            else
            {
                this.previousTagNameIndex = this.tagNameIndex;

                if (nameIndex == HtmlNameIndex._COMMENT)
                {
                    this.output.Write("!--");
                    this.lineLength += 3;
                }
                else if (nameIndex == HtmlNameIndex._ASP)
                {
                    this.output.Write('%');
                    this.lineLength++;
                }
                else if (nameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    this.output.Write("!--[");
                    this.lineLength += 4;
                }
                else if (nameIndex == HtmlNameIndex._DTD)
                {
                    this.output.Write('?');
                    this.lineLength++;
                }
                else
                {
                    this.output.Write('!');
                    this.lineLength++;
                }

                this.outputState = OutputState.WritingUnstructuredTagContent;

                this.isEmptyScopeTag = true;
            }

            this.tagNameIndex = nameIndex;
            this.isEndTag = isEndTag;
        }

        internal void WriteTagEnd()
        {
            this.WriteTagEnd(this.isEmptyScopeTag);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        internal void WriteTagEnd(bool emptyScopeTag)
        {
            InternalDebug.Assert(this.outputState != OutputState.OutsideTag);
            InternalDebug.Assert(!this.lastWhitespace);
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)this.tagNameIndex].tagIndex;

            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            if (this.tagNameIndex > HtmlNameIndex.Unknown)
            {
                this.output.Write('>');
                this.lineLength++;
            }
            else
            {
                if (this.tagNameIndex == HtmlNameIndex._COMMENT)
                {
                    this.output.Write("-->");
                    this.lineLength += 3;
                }
                else if (this.tagNameIndex == HtmlNameIndex._ASP)
                {
                    this.output.Write("%>");
                    this.lineLength += 2;
                }
                else if (this.tagNameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    this.output.Write("]-->");
                    this.lineLength += 4;
                }
                else if (this.tagNameIndex == HtmlNameIndex.Unknown && emptyScopeTag)
                {
                    this.output.Write(" />");
                    this.lineLength += 3;
                }
                else
                {
                    this.output.Write('>');
                    this.lineLength++;
                }

                this.tagNameIndex = this.previousTagNameIndex;
            }

            if (this.isEndTag &&
                (tagIndex == HtmlTagIndex.LI || tagIndex == HtmlTagIndex.DD || tagIndex == HtmlTagIndex.DT))
            {
                this.lineLength = 0;
            }

            if (this.autoNewLines && this.literalWhitespaceNesting == 0)
            {
                HtmlDtd.TagFmt tagFmt = HtmlDtd.tags[(int)tagIndex].fmt;
                HtmlDtd.TagFill tagFill = HtmlDtd.tags[(int)tagIndex].fill;

                if ((!this.isEndTag && tagFmt.RB == HtmlDtd.FmtCode.BRK) ||
                    (this.isEndTag && tagFmt.RE == HtmlDtd.FmtCode.BRK) ||
                        (this.lineLength > 80 &&
                        (this.allowWspBeforeFollowingTag ||
                        (!this.isEndTag && tagFill.RB == HtmlDtd.FillCode.EAT) ||
                        (this.isEndTag && tagFill.RE == HtmlDtd.FillCode.EAT))))
                {
                    if (this.lineLength > this.longestLineLength)
                    {
                        this.longestLineLength = this.lineLength;
                    }

                    this.output.Write("\r\n");
                    this.lineLength = 0;
                }
            }

            if (!this.isEndTag && !emptyScopeTag)
            {
                HtmlDtd.Literal literal = HtmlDtd.tags[(int)tagIndex].literal;

                if (0 != (literal & HtmlDtd.Literal.Tags))
                {
                    this.literalTags = true;
                    this.literalEntities = (0 != (literal & HtmlDtd.Literal.Entities));
                    this.cssEscaping = (tagIndex == HtmlTagIndex.Style);
                }

                if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                {
                    this.literalWhitespaceNesting++;
                }
            }

            this.outputState = OutputState.OutsideTag;
        }

        internal void WriteAttribute(HtmlNameIndex nameIndex, string value)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(this.outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!this.isEndTag);

            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);

            if (value != null)
            {
                this.OutputAttributeValue(value);
                this.OutputAttributeEnd();
            }

            this.outputState = OutputState.BeforeAttribute;
        }

        internal void WriteAttributeName(HtmlNameIndex nameIndex)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(this.outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!this.isEndTag);

            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
        }

        internal void WriteAttributeValueInternal(string value)
        {
            InternalDebug.Assert(value != null);
            InternalDebug.Assert(this.outputState >= OutputState.WritingAttributeName);

            this.OutputAttributeValue(value);
        }

        internal void WriteAttributeValueInternal(char[] buffer, int index, int count)
        {
            InternalDebug.Assert(buffer != null && index >= 0 && index < buffer.Length && count >= 0 && count <= buffer.Length - index);
            InternalDebug.Assert(this.outputState >= OutputState.WritingAttributeName);

            this.OutputAttributeValue(buffer, index, count);
        }

        public void WriteMarkupText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (this.lastWhitespace)
            {
                this.OutputLastWhitespace(value[0]);
            }

            this.output.Write(value, null);
            this.lineLength += value.Length;

            this.allowWspBeforeFollowingTag = false;
        }

        internal ITextSinkEx WriteUnstructuredTagContent()
        {
            InternalDebug.Assert(this.outputState == OutputState.WritingUnstructuredTagContent);

            this.fallback = null;
            return this;
        }

        internal ITextSinkEx WriteTagName()
        {
            InternalDebug.Assert(this.outputState == OutputState.TagStarted ||
                                this.outputState == OutputState.WritingTagName);

            this.outputState = OutputState.WritingTagName;

            this.fallback = null;
            return this;
        }

        internal ITextSinkEx WriteAttributeName()
        {
            InternalDebug.Assert(this.outputState >= OutputState.WritingTagName);

            if (this.outputState != OutputState.WritingAttributeName)
            {
                if (this.outputState > OutputState.BeforeAttribute)
                {
                    this.OutputAttributeEnd();
                }
#if false



                if (this.lineLength > 255 && this.autoNewLines)
                {
                    
                    

                    if (this.lineLength > this.longestLineLength)
                    {
                        this.longestLineLength = this.lineLength;
                    }

                    this.output.Write("\r\n");
                    this.lineLength = 0;
                }
#endif
                this.output.Write(' ');
                this.lineLength++;
            }

            this.outputState = OutputState.WritingAttributeName;

            this.fallback = null;
            return this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        internal ITextSinkEx WriteAttributeValue()
        {
            InternalDebug.Assert(this.outputState >= OutputState.WritingAttributeName);

            if (this.outputState != OutputState.WritingAttributeValue)
            {
                this.output.Write("=\"");
                this.lineLength += 2;
            }

            this.outputState = OutputState.WritingAttributeValue;

            this.fallback = this;
            return this;
        }

        internal ITextSinkEx WriteText()
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            this.allowWspBeforeFollowingTag = false;

            if (this.lastWhitespace)
            {
                InternalDebug.Assert(ParseSupport.FarEastNonHanguelChar('\x3000'));
                this.OutputLastWhitespace('\x3000');
            }

            this.fallback = this;
            return this;
        }

        internal void WriteNewLine()
        {
            this.WriteNewLine(false);
        }

        internal void WriteNewLine(bool optional)
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (!optional || (this.lineLength != 0 && this.literalWhitespaceNesting == 0))
            {
                if (this.lineLength > this.longestLineLength)
                {
                    this.longestLineLength = this.lineLength;
                }

                this.output.Write("\r\n");
                this.lineLength = 0;
                this.lastWhitespace = false;
                this.allowWspBeforeFollowingTag = false;
            }
        }

        internal void WriteAutoNewLine()
        {
            this.WriteNewLine(false);
        }

        internal void WriteAutoNewLine(bool optional)
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (this.autoNewLines && (!optional || (this.lineLength != 0 && this.literalWhitespaceNesting == 0)))
            {
                if (this.lineLength > this.longestLineLength)
                {
                    this.longestLineLength = this.lineLength;
                }

                this.output.Write("\r\n");
                this.lineLength = 0;
                this.lastWhitespace = false;
                this.allowWspBeforeFollowingTag = false;
            }
        }

        internal void WriteCollapsedWhitespace()
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            this.lastWhitespace = true;
            this.allowWspBeforeFollowingTag = false;
        }

        private void OutputLastWhitespace(char nextChar)
        {
            if (this.lineLength > 255 && this.autoNewLines)
            {
                if (this.lineLength > this.longestLineLength)
                {
                    this.longestLineLength = this.lineLength;
                }

                this.lineLength = 0;

                if (ParseSupport.FarEastNonHanguelChar(nextChar))
                {
                    this.output.Write(' ');
                    this.lineLength++;
                }
            }
            else
            {
                this.output.Write(' ');
                this.lineLength++;
            }

            this.textLineLength++;
            this.lastWhitespace = false;
        }

        private void OutputAttributeName(string name)
        {
#if false



            if (this.lineLength > 255 && this.autoNewLines)
            {
                
                

                if (this.lineLength > this.longestLineLength)
                {
                    this.longestLineLength = this.lineLength;
                }

                this.output.Write("\r\n");
                this.lineLength = 0;
            }
#endif
            this.output.Write(' ');
            this.output.Write(name);
            this.lineLength += name.Length + 1;

            this.outputState = OutputState.AfterAttributeName;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        private void OutputAttributeValue(string value)
        {
            InternalDebug.Assert(this.outputState > OutputState.BeforeAttribute);

            if (this.outputState < OutputState.WritingAttributeValue)
            {
                this.output.Write("=\"");
                this.lineLength += 2;
            }

            this.output.Write(value, this);
            this.lineLength += value.Length;

            this.outputState = OutputState.WritingAttributeValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        private void OutputAttributeValue(char[] value, int index, int count)
        {
            InternalDebug.Assert(this.outputState > OutputState.BeforeAttribute);

            if (this.outputState < OutputState.WritingAttributeValue)
            {
                this.output.Write("=\"");
                this.lineLength += 2;
            }

            this.output.Write(value, index, count, this);
            this.lineLength += count;

            this.outputState = OutputState.WritingAttributeValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Exchange.Data.TextConverters.ConverterOutput.Write(System.String)")]
        private void OutputAttributeEnd()
        {
            InternalDebug.Assert(this.outputState > OutputState.BeforeAttribute);

            if (this.outputState < OutputState.WritingAttributeValue)
            {
                this.output.Write("=\"");
                this.lineLength += 2;
            }

            this.output.Write('\"');
            this.lineLength++;
        }

        bool IRestartable.CanRestart()
        {
            if (this.output is IRestartable restartable)
            {
                return restartable.CanRestart();
            }

            return false;
        }

        void IRestartable.Restart()
        {
            if (this.output is IRestartable restartable)
            {
                restartable.Restart();
            }

            this.allowWspBeforeFollowingTag = false;
            this.lastWhitespace = false;
            this.lineLength = 0;
            this.longestLineLength = 0;

            this.literalWhitespaceNesting = 0;
            this.literalTags = false;
            this.literalEntities = false;
            this.cssEscaping = false;

            this.tagNameIndex = HtmlNameIndex._NOTANAME;
            this.previousTagNameIndex = HtmlNameIndex._NOTANAME;

            this.isEndTag = false;
            this.isEmptyScopeTag = false;
            this.copyPending = false;

            this.outputState = OutputState.OutsideTag;
        }

        void IRestartable.DisableRestart()
        {
            if (this.output is IRestartable restartable)
            {
                restartable.DisableRestart();
            }
        }

        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (this.literalEntities)
            {
                unsafeAsciiMask = 0x00;
                return null;
            }

            if (this.filterHtml)
            {
                unsafeAsciiMask = 0x01;
            }
            else
            {
                unsafeAsciiMask = 0x01;
            }

            return HtmlSupport.UnsafeAsciiMap;
        }

        bool IFallback.HasUnsafeUnicode()
        {
            return this.filterHtml;
        }

        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return this.filterHtml && charset.StartsWith("x-", StringComparison.OrdinalIgnoreCase);
        }

        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return this.filterHtml &&
                ((byte)(ch & 0xFF) == (byte)'<' ||
                (byte)((ch >> 8) & 0xFF) == (byte)'<' ||

                (!isFirstChar && ch == '\uFEFF') ||
                Char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.PrivateUse);
        }

        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (this.literalEntities)
            {
                if (this.cssEscaping)
                {
                    uint value = (uint)ch;
                    int len = (value < 0x10) ? 1 : (value < 0x100) ? 2 : (value < 0x1000) ? 3 : 4;
                    if (outputEnd - outputBufferCount < len + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '\\';

                    int offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        uint digit = value & 0xF;
                        outputBuffer[--offset] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ' ';
                }
                else
                {
                    if (outputEnd - outputBufferCount < 1)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = this.filterHtml ? '?' : ch;
                }
            }
            else
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
                    int len = (value < 10) ? 1 : (value < 100) ? 2 : (value < 1000) ? 3 : (value < 10000) ? 4 : 5;
                    if (outputEnd - outputBufferCount < len + 3)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';

                    int offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        uint digit = value % 10;
                        outputBuffer[--offset] = (char)(digit + '0');
                        value /= 10;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ';';
                }
            }

            return true;
        }

        bool ITextSink.IsEnough { get { return false; } }

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            this.lineLength += count;
            this.textLineLength += count;
            this.output.Write(buffer, offset, count, this.fallback);
        }

        void ITextSink.Write(int ucs32Char)
        {
            this.lineLength++;
            this.textLineLength++;
            this.output.Write(ucs32Char, this.fallback);
        }

        void ITextSinkEx.Write(string text)
        {
            this.lineLength += text.Length;
            this.textLineLength += text.Length;
            this.output.Write(text, this.fallback);
        }

        void ITextSinkEx.WriteNewLine()
        {
            if (this.lineLength > this.longestLineLength)
            {
                this.longestLineLength = this.lineLength;
            }

            this.output.Write("\r\n");
            this.lineLength = 0;
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.output != null)
                {
                    if (!this.copyPending)
                    {
                        this.Flush();
                    }

                    if (this.output is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                GC.SuppressFinalize(this);
            }

            this.output = null;
        }
    }
}
