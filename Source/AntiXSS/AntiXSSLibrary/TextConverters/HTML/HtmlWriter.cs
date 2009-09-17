// ***************************************************************
// <copyright file="HtmlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

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

        
        
        
        
        
        
        public HtmlWriter(Stream output, System.Text.Encoding outputEncoding)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            if (outputEncoding == null)
            {
                throw new ArgumentNullException("outputEncoding");
            }

            this.output = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    false,      
                                    outputEncoding,
                                    false,  
                                    false,
                                    null);
            this.autoNewLines = true;
        }

        
        
        
        
        
        public HtmlWriter(TextWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            this.output = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    false);     
            this.autoNewLines = true;
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

        
        internal bool CanAcceptMore
        {
            get { return this.output.CanAcceptMore; }
        }

        
        internal bool IsTagOpen
        {
            get { return this.outputState != OutputState.OutsideTag; }
        }

        
        internal int LineLength
        {
            get { return this.lineLength; }
        }

        
        internal int LiteralWhitespaceNesting
        {
            get { return this.literalWhitespaceNesting; }
        }

        
        
        
        
        public HtmlWriterState WriterState
        {
            get
            {
                return this.outputState == OutputState.OutsideTag ? HtmlWriterState.Default :
                        this.outputState < OutputState.WritingAttributeName ? HtmlWriterState.Tag :
                            HtmlWriterState.Attribute;
            }
        }

        
        internal bool IsCopyPending
        {
            get { return this.copyPending; }
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

        
        
        
        
        
        public void WriteTag(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (reader.TagId != HtmlTagId.Unknown)
            {
                this.WriteTagBegin(HtmlNameData.tagIndex[(int)reader.TagId], null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
            }
            else
            {
                this.WriteTagBegin(HtmlNameIndex.Unknown, null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
                reader.WriteTagNameTo(this.WriteTagName());
            }

            this.isEmptyScopeTag = (reader.TokenKind == HtmlTokenKind.EmptyElementTag);

            if (reader.TokenKind == HtmlTokenKind.StartTag ||
                reader.TokenKind == HtmlTokenKind.EmptyElementTag)
            {
                HtmlAttributeReader attrReader = reader.AttributeReader;

                while (attrReader.ReadNext())
                {
                    if (attrReader.Id != HtmlAttributeId.Unknown)
                    {
                        this.OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)attrReader.Id]].name);
                    }
                    else
                    {
                        attrReader.WriteNameTo(this.WriteAttributeName());
                    }

                    if (attrReader.HasValue)
                    {
                        attrReader.WriteValueTo(this.WriteAttributeValue());
                    }

                    this.OutputAttributeEnd();

                    this.outputState = OutputState.BeforeAttribute;
                }
            }
        }

        
        
        
        
        
        public void WriteStartTag(HtmlTagId id)
        {
            this.WriteTag(id, false);
        }

        
        
        
        
        
        public void WriteStartTag(string name)
        {
            this.WriteTag(name, false);
        }

        
        
        
        
        
        public void WriteEndTag(HtmlTagId id)
        {
            this.WriteTag(id, true);
            this.WriteTagEnd();             
        }

        
        
        
        
        
        public void WriteEndTag(string name)
        {
            this.WriteTag(name, true);
            this.WriteTagEnd();             
        }

        
        
        
        
        
        public void WriteEmptyElementTag(HtmlTagId id)
        {
            this.WriteTag(id, false);
            this.isEmptyScopeTag = true;
        }

        
        
        
        
        
        public void WriteEmptyElementTag(string name)
        {
            this.WriteTag(name, false);
            this.isEmptyScopeTag = true;
        }

        
        private void WriteTag(HtmlTagId id, bool isEndTag)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.tagIndex.Length)
            {
                throw new ArgumentException(Strings.TagIdInvalid, "id");
            }

            if (id == HtmlTagId.Unknown)
            {
                throw new ArgumentException(Strings.TagIdIsUnknown, "id");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            this.WriteTagBegin(HtmlNameData.tagIndex[(int)id], null, isEndTag, false, false);
        }

        
        private void WriteTag(string name, bool isEndTag)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.TagNameIsEmpty, "name");
            }

            

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            HtmlNameIndex nameIndex = LookupName(name);

            if (nameIndex != HtmlNameIndex.Unknown)
            {
                
                name = null;
            }

            this.WriteTagBegin(nameIndex, name, isEndTag, false, false);
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

        
        internal void WriteEmptyElementTag(HtmlNameIndex nameIndex)
        {
            this.WriteTagBegin(nameIndex, null, true, false, false);
            this.isEmptyScopeTag = true;
        }

        
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

        
        
        
        
        
        
        public void WriteAttribute(HtmlAttributeId id, string value)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);

            if (value != null)
            {
                this.OutputAttributeValue(value);
                this.OutputAttributeEnd();
            }

            
            this.outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        public void WriteAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            

            
            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(name);

            if (value != null)
            {
                this.OutputAttributeValue(value);
                this.OutputAttributeEnd();
            }

            
            this.outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        
        public void WriteAttribute(HtmlAttributeId id, char[] buffer, int index, int count)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);

            this.OutputAttributeValue(buffer, index, count);

            this.OutputAttributeEnd();

            
            this.outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        
        
        public void WriteAttribute(string name, char[] buffer, int index, int count)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(name);

            this.OutputAttributeValue(buffer, index, count);

            this.OutputAttributeEnd();

            
            this.outputState = OutputState.BeforeAttribute;
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

        internal void WriteAttribute(HtmlNameIndex nameIndex, BufferString value)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(this.outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!this.isEndTag);

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
            this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
            this.OutputAttributeEnd();

            this.outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        public void WriteAttribute(HtmlAttributeReader attributeReader)
        {
            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            attributeReader.WriteNameTo(this.WriteAttributeName());
            if (attributeReader.HasValue)
            {
                attributeReader.WriteValueTo(this.WriteAttributeValue());
            }
            this.OutputAttributeEnd();

            this.outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        public void WriteAttributeName(HtmlAttributeId id)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);
        }

        
        
        
        
        
        
        public void WriteAttributeName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState > OutputState.BeforeAttribute)
            {
                this.OutputAttributeEnd();
            }

            this.OutputAttributeName(name);
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

        
        
        
        
        
        public void WriteAttributeName(HtmlAttributeReader attributeReader)
        {
            if (this.outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            attributeReader.WriteNameTo(this.WriteAttributeName());
        }

        
        
        
        
        
        
        
        public void WriteAttributeValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            this.OutputAttributeValue(value);
        }

        internal void WriteAttributeValue(BufferString value)
        {
            InternalDebug.Assert(!this.copyPending && this.outputState >= OutputState.WritingAttributeName);

            this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
        }

        
        
        
        
        
        
        
        
        
        public void WriteAttributeValue(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (this.outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            this.OutputAttributeValue(buffer, index, count);
        }

        
        
        
        
        
        public void WriteAttributeValue(HtmlAttributeReader attributeReader)
        {
            if (this.outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (this.outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (attributeReader.HasValue)
            {
                attributeReader.WriteValueTo(this.WriteAttributeValue());
            }
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

        
        
        
        
        
        public void WriteText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (value.Length != 0)
            {
                if (this.lastWhitespace)
                {
                    this.OutputLastWhitespace(value[0]);
                }

                

                this.output.Write(value, this);
                this.lineLength += value.Length;
                this.textLineLength += value.Length;

                this.allowWspBeforeFollowingTag = false;
            }
        }

        
        
        
        
        
        
        
        public void WriteText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            

            this.WriteTextInternal(buffer, index, count);
        }

        
        internal void WriteText(char ch)
        {
            InternalDebug.Assert(!this.copyPending);

            
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (this.lastWhitespace)
            {
                this.OutputLastWhitespace(ch);
            }

            this.output.Write(ch, this);
            this.lineLength++;
            this.textLineLength++;
            this.allowWspBeforeFollowingTag = false;
        }

        
        
        
        
        
        public void WriteText(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            reader.WriteTextTo(this.WriteText());
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public void WriteMarkupText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
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

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void WriteMarkupText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
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
                this.OutputLastWhitespace(buffer[index]);
            }

            

            this.output.Write(buffer, index, count, null);
            this.lineLength += count;
            
            this.allowWspBeforeFollowingTag = false;
        }


        
        internal void WriteMarkupText(char ch)
        {
            
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (this.lastWhitespace)
            {
                this.OutputLastWhitespace(ch);
            }

            this.output.Write(ch, null);
            this.lineLength++;
            
            this.allowWspBeforeFollowingTag = false;
        }

        
        
        
        
        
        
        public void WriteMarkupText(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (this.copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            reader.WriteMarkupTextTo(this.WriteMarkupText());
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

        
        internal ITextSinkEx WriteMarkupText()
        {
            
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            if (this.lastWhitespace)
            {
                this.output.Write(' ');
                this.lineLength++;
                this.lastWhitespace = false;
            }

            this.fallback = null;
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

        
        internal void WriteTabulation(int count)
        {
            this.WriteSpace((this.textLineLength / 8 * 8 + 8 * count) - this.textLineLength);
        }

        
        internal void WriteSpace(int count)
        {
            InternalDebug.Assert(this.outputState == OutputState.OutsideTag);

            if (this.literalWhitespaceNesting == 0)
            {
                if (this.lineLength == 0 && count == 1)
                {
                    
                    
                    this.output.Write('\xA0', this);
                    return;
                }

                if (this.lastWhitespace)
                {
                    this.lineLength++;
                    this.output.Write('\xA0', this);
                }

                this.lineLength += count - 1;
                this.textLineLength += count - 1;

                while (0 != --count)
                {
                    this.output.Write('\xA0', this);
                }

                this.lastWhitespace = true;
                this.allowWspBeforeFollowingTag = false;
            }
            else
            {
                while (0 != count--)
                {
                    this.output.Write(' ');
                }

                this.lineLength += count;
                this.textLineLength += count;

                this.lastWhitespace = false;
                this.allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void WriteNbsp(int count)
        {
            InternalDebug.Assert(this.outputState == OutputState.OutsideTag);

            if (this.lastWhitespace)
            {
                this.OutputLastWhitespace('\xA0');
            }

            this.lineLength += count;
            this.textLineLength += count;
            while (0 != count--)
            {
                this.output.Write('\xA0', this);
            }

            this.allowWspBeforeFollowingTag = false;
        }

        
        internal void WriteTextInternal(char[] buffer, int index, int count)
        {
            InternalDebug.Assert(buffer != null);
            InternalDebug.Assert(index >= 0 && index <= buffer.Length);
            InternalDebug.Assert(count >= 0 && count <= buffer.Length - index);
            InternalDebug.Assert(!this.copyPending);

            InternalDebug.Assert(this.outputState == OutputState.OutsideTag);

            if (count != 0)
            {
                if (this.lastWhitespace)
                {
                    this.OutputLastWhitespace(buffer[index]);
                }

                InternalDebug.Assert(!this.lastWhitespace);

                this.output.Write(buffer, index, count, this);
                this.lineLength += count;
                this.textLineLength += count;

                this.allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void StartTextChunk()
        {
            if (this.outputState != OutputState.OutsideTag)
            {
                this.WriteTagEnd();
            }

            this.lastWhitespace = false;
        }

        
        internal void EndTextChunk()
        {
            if (this.lastWhitespace)
            {
                this.OutputLastWhitespace('\n');
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

                this.output.Write("\r\n");
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

        
        internal static HtmlNameIndex LookupName(string name)
        {
            if (name.Length <= HtmlNameData.MAX_NAME)
            {
                short hash = (short)(((uint)HashCode.CalculateLowerCase(name) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                

                int nameIndex = (int)HtmlNameData.nameHashTable[hash];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        string currentName = HtmlNameData.names[nameIndex].name;

                        if (currentName.Length == name.Length)
                        {
                            if (currentName[0] == ParseSupport.ToLowerCase(name[0]))
                            {
                                if (name.Length == 1 || currentName.Equals(name, StringComparison.OrdinalIgnoreCase))
                                {
                                    return (HtmlNameIndex)nameIndex;
                                }
                            }
                        }

                        
                        
                    }
                    while (HtmlNameData.names[++nameIndex].hash == hash);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        
        bool IRestartable.CanRestart()
        {
            if (this.output is IRestartable)
            {
                return ((IRestartable)this.output).CanRestart();
            }

            return false;
        }

        
        void IRestartable.Restart()
        {
            if (this.output is IRestartable)
            {
                ((IRestartable)this.output).Restart();
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
            if (this.output is IRestartable)
            {
                ((IRestartable)this.output).DisableRestart();
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

        
        

        
        public void Close()
        {
            this.Dispose(true);
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

                    if (this.output is IDisposable)
                    {
                        ((IDisposable)this.output).Dispose();
                    }
                }

                GC.SuppressFinalize(this);
            }

            this.output = null;
        }
    }
}
