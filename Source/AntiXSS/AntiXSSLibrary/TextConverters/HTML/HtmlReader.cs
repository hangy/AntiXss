// ***************************************************************
// <copyright file="HtmlReader.cs" company="Microsoft">
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

    

    
    
    
    internal enum HtmlTokenKind
    {
        
        Text,

        
        StartTag,
        
        EndTag,
        
        
        
        
        
        
        
        
        EmptyElementTag,                

#if PRIVATEBUILD
        
        
        
        
        
        EmptyScopeMark,
#endif
        
        
        
        
        SpecialTag,

#if PRIVATEBUILD
        
        
        
        Restart,
#endif

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        OverlappedClose,
        
        OverlappedReopen,
    }

    
    
    
    
    internal class HtmlReader : IRestartable, IResultsFeedback, IDisposable
    {
        

        private Encoding inputEncoding;
        private bool detectEncodingFromByteOrderMark;
#if PRIVATEBUILD
        private bool detectEncodingFromMetaTag;
        private bool detectEncodingFromXmlDeclaration;
#endif
        private bool normalizeInputHtml;

        internal bool testBoundaryConditions = false;
        internal int inputBufferSize = 4096;

        private Stream testTraceStream = null;
        private bool testTraceShowTokenNum = true;
        private int testTraceStopOnTokenNum = 0;

        private Stream testNormalizerTraceStream = null;
        private bool testNormalizerTraceShowTokenNum = true;
        private int testNormalizerTraceStopOnTokenNum = 0;

        private bool locked;

        

        private object input;

        private IHtmlParser parser;

        private HtmlTokenId parserTokenId;
        private HtmlToken parserToken;

        private HtmlToken nextParserToken;

        private int depth;

        private StringBuildSink stringBuildSink;

        private int currentAttribute;

        private bool literalTags;

        private enum State : byte
        {
            Disposed,                   
            EndOfFile,                  
            Begin,                      

            Text,                       
            EndText,                    

            OverlappedClose,            
            OverlappedReopen,           

            SpecialTag,                 
            EndSpecialTag,              
#if PRIVATEBUILD
            EmptyScopeMark,             
#endif
            BeginTag,                   
            ReadTagName,                
            EndTagName,                 
            BeginAttribute,             
            ReadAttributeName,          
            EndAttributeName,           
            BeginAttributeValue,        
            ReadAttributeValue,         
            EndAttribute,               
            ReadTag,                    
            EndTag,                     
        }

        private State state;
        private HtmlTokenKind tokenKind;

        
        
        
        
        
        
        
        public HtmlReader(Stream input, System.Text.Encoding inputEncoding)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (!input.CanRead)
            {
                throw new ArgumentException("input stream must support reading");
            }

            this.input = input;
            this.inputEncoding = inputEncoding;
            this.state = State.Begin;
        }

        
        
        
        
        
        
        public HtmlReader(TextReader input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            this.input = input;
            this.inputEncoding = Encoding.Unicode;
            this.state = State.Begin;
        }

        
        

        
        
        
        
        
        
        
        
        
        public System.Text.Encoding InputEncoding
        {
            get { return this.inputEncoding; }
            set { this.AssertNotLocked(); this.inputEncoding = value; }
        }

        
        
        
        
        
        
        
        
        public bool DetectEncodingFromByteOrderMark
        {
            get { return this.detectEncodingFromByteOrderMark; }
            set { this.AssertNotLocked(); this.detectEncodingFromByteOrderMark = value; }
        }
#if PRIVATEBUILD
        
        
        

        
        
        
        
        
        
        
        public bool DetectEncodingFromMetaTag
        {
            get { return this.detectEncodingFromMetaTag; }
            set { this.AssertNotLocked(); this.detectEncodingFromMetaTag = value; }
        }

        
        
        
        public bool DetectEncodingFromXmlDeclaration
        {
            get { return this.detectEncodingFromXmlDeclaration; }
            set { this.AssertNotLocked(); this.detectEncodingFromXmlDeclaration = value; }
        }
#endif
        
        
        
        
        
        
        
        
        public bool NormalizeHtml
        {
            get { return this.normalizeInputHtml; }
            set { this.AssertNotLocked(); this.normalizeInputHtml = value; }
        }

        
        

        
        
        
        
        
        
        public HtmlTokenKind TokenKind
        {
            get
            {
                this.AssertInToken();
                return this.tokenKind;
            }
        }

        
        
        
        
        
        
        
        public bool ReadNextToken()
        {
            AssertNotDisposed();

            if (this.state == State.EndOfFile)
            {
                return false;
            }

            if (!this.locked)
            {
                this.InitializeAndLock();
            }

            if (this.state == State.Text)
            {
                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Text || (this.literalTags && this.parserTokenId == HtmlTokenId.Tag && this.parserToken.TagIndex < HtmlTagIndex.Unknown));

                

                do
                {
                    this.ParseToken();
                }
                while (this.parserTokenId == HtmlTokenId.Text ||
                    (this.literalTags && this.parserTokenId == HtmlTokenId.Tag && this.parserToken.TagIndex < HtmlTagIndex.Unknown));
            }
            else if (this.state >= State.SpecialTag)
            {
                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag);

                
                

                while (!this.parserToken.IsTagEnd)
                {
                    this.ParseToken();
                }
#if PRIVATEBUILD
                if (HtmlDtd.tags[(int)this.parserToken.TagIndex].scope != HtmlDtd.TagScope.EMPTY && this.parserToken.IsEmptyScope)
                {
                    

                    if (this.state != State.EmptyScopeMark)
                    {
                        

                        this.state = State.EmptyScopeMark;
                        this.tokenKind = HtmlTokenKind.EmptyScopeMark;
                        return true;
                    }
                }
#endif
                if (this.parserToken.TagIndex <= HtmlTagIndex.Unknown ||                                
                    this.parserToken.IsEndTag ||                                                        
                    HtmlDtd.tags[(int)this.parserToken.TagIndex].scope == HtmlDtd.TagScope.EMPTY ||     
                    this.parserToken.IsEmptyScope)
                {
                    
                }
                else
                {
                    
                    this.depth ++;
                }

                if (!this.parserToken.IsEndTag &&
                    0 != (HtmlDtd.tags[(int)this.parserToken.TagIndex].literal & HtmlDtd.Literal.Tags))
                {
                    this.literalTags = true;
                }

                this.ParseToken();
            }
            else
            {
                

                if (this.state == State.OverlappedClose)
                {
                    
                    this.depth -= this.parserToken.Argument;
                }

                this.ParseToken();
            }

            while (true)
            {
                switch (this.parserTokenId)
                {
                    case HtmlTokenId.Text:

                            this.state = State.Text;
                            this.tokenKind = HtmlTokenKind.Text;

                            this.parserToken.Text.Rewind();
                            break;

                    case HtmlTokenId.Tag:

                            InternalDebug.Assert(this.parserToken.IsTagBegin);

                            if (this.parserToken.TagIndex < HtmlTagIndex.Unknown)
                            {
                                if (this.literalTags)
                                {
                                    
                                    this.state = State.Text;
                                    this.tokenKind = HtmlTokenKind.Text;
                                }
                                else
                                {
                                    this.state = State.SpecialTag;
                                    this.tokenKind = HtmlTokenKind.SpecialTag;
                                }

                                this.parserToken.Text.Rewind();
                            }
                            else
                            {
                                if (this.parserToken.TagIndex == HtmlTagIndex.TC)
                                {
                                    
                                    
                                    

                                    InternalDebug.Assert(this.parserToken.IsTagEnd);

                                    this.ParseToken();
                                    continue;
                                }

                                if (this.parserToken.IsTagNameEmpty && this.parserToken.TagIndex == HtmlTagIndex.Unknown)
                                {
                                    
                                    
                                    
                                    InternalDebug.Assert(this.parserToken.IsEndTag);

                                    this.state = State.SpecialTag;
                                    this.tokenKind = HtmlTokenKind.SpecialTag;

                                    this.parserToken.Text.Rewind();
                                }
                                else
                                {
                                    this.state = State.BeginTag;

                                    if (this.parserToken.IsEndTag)
                                    {
                                        this.tokenKind = HtmlTokenKind.EndTag;

                                        if (0 != (HtmlDtd.tags[(int)this.parserToken.TagIndex].literal & HtmlDtd.Literal.Tags))
                                        {
                                            this.literalTags = false;
                                        }
                                    }
                                    else if (this.parserToken.TagIndex > HtmlTagIndex.Unknown &&
                                            HtmlDtd.tags[(int)this.parserToken.TagIndex].scope == HtmlDtd.TagScope.EMPTY)
                                    {
                                        this.tokenKind = HtmlTokenKind.EmptyElementTag;
                                    }
                                    else
                                    {
                                        this.tokenKind = HtmlTokenKind.StartTag;
                                    }

                                    this.parserToken.Text.Rewind();

                                    
                                    
                                    
                                    if (this.parserToken.IsEndTag && this.parserToken.TagIndex != HtmlTagIndex.Unknown)
                                    {
                                        this.depth --;
                                    }
                                }
                            }
                            break;

                    case HtmlTokenId.OverlappedClose:

                            

                            

                            this.state = State.OverlappedClose;
                            this.tokenKind = HtmlTokenKind.OverlappedClose;
                            break;

                    case HtmlTokenId.OverlappedReopen:

                            

                            
                            this.depth += this.parserToken.Argument;

                            this.state = State.OverlappedReopen;
                            this.tokenKind = HtmlTokenKind.OverlappedReopen;
                            break;

                    case HtmlTokenId.EncodingChange:

                            
                            
                            this.ParseToken();
                            continue;

                    case HtmlTokenId.Restart:

#if PRIVATEBUILD
                            
                            this.state = State.Begin;
                            this.tokenKind = HtmlTokenKind.Restart;
                            break;
#else
                            InternalDebug.Assert(false);
                            continue;
#endif
                    default: 

                            InternalDebug.Assert(this.parserTokenId == HtmlTokenId.EndOfFile);

                            this.state = State.EndOfFile;
                            return false;
                }

                break; 
            }

            return true;
        }

        
        
        
        
        
        
        
        
        
        public int Depth
        {
            get
            {
                this.AssertNotDisposed();
                return this.depth;
            }
        }

        
        
        
        
        
        public int OverlappedDepth
        {
            get
            {
                if (this.state != State.OverlappedClose && this.state != State.OverlappedReopen)
                {
                    this.AssertInToken();
                    throw new InvalidOperationException("Reader must be positioned on OverlappedClose or OverlappedReopen token");
                }

                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.OverlappedClose || this.parserTokenId == HtmlTokenId.OverlappedReopen);
                InternalDebug.Assert(this.tokenKind == HtmlTokenKind.OverlappedClose || this.tokenKind == HtmlTokenKind.OverlappedReopen);

                return this.parserToken.Argument;
            }
        }

        
        
        
        
        
        
        
        public HtmlTagId TagId
        {
            get
            {
                this.AssertInTag();
                return HtmlNameData.names[(int)this.parserToken.NameIndex].publicTagId;
            }
        }

        
        
        
        
        
        
        
        public bool TagInjectedByNormalizer
        {
            get
            {
                this.AssertInTag();

                if (this.state != State.BeginTag)
                {
                    throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
                }

                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag);

                return this.parserToken.Argument == 1;
            }
        }

        
        
        
        
        
        
        
        
        
        public bool TagNameIsLong
        {
            get
            {
                this.AssertInTag();

                if (this.state != State.BeginTag)
                {
                    throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
                }

                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag);

                return this.parserToken.NameIndex == HtmlNameIndex.Unknown && this.parserToken.IsTagNameBegin && !this.parserToken.IsTagNameEnd;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public string ReadTagName()
        {
            if (this.state != State.BeginTag)
            {
                this.AssertInTag();
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag && this.parserToken.IsTagBegin);

            string name;
            
            if (this.parserToken.NameIndex != HtmlNameIndex.Unknown)
            {
                
                name = HtmlNameData.names[(int)this.parserToken.NameIndex].name;
            }
            else
            {
                InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag && this.parserToken.IsTagBegin && this.parserToken.NameIndex == HtmlNameIndex.Unknown);

                if (this.parserToken.IsTagNameEnd)
                {
                    return this.parserToken.Name.GetString(int.MaxValue);
                }

                InternalDebug.Assert(!this.parserToken.IsTagEnd);

                StringBuildSink sbSink = this.GetStringBuildSink();

                this.parserToken.Name.WriteTo(sbSink);

                do
                {
                    this.ParseToken();

                    this.parserToken.Name.WriteTo(sbSink);
                }
                while (!this.parserToken.IsTagNameEnd);

                name = sbSink.ToString();
            }

            
            this.state = State.EndTagName;

            
            return name;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadTagName(char[] buffer, int offset, int count)
        {
            this.AssertInTag();
            if (this.state > State.EndTagName)
            {
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            int lengthRead = 0;

            if (this.state != State.EndTagName)
            {
                if (this.state == State.BeginTag)
                {
                    this.state = State.ReadTagName;
                    this.parserToken.Name.Rewind();
                }

                while (count != 0)
                {
                    int chunkLength = this.parserToken.Name.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (this.parserToken.IsTagNameEnd)
                        {
                            this.state = State.EndTagName;
                            break;
                        }

                        this.ParseToken();
                        this.parserToken.Name.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void WriteTagNameTo(ITextSink sink)
        {
            if (this.state != State.BeginTag)
            {
                this.AssertInTag();
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            while (true)
            {
                this.parserToken.Name.WriteTo(sink);

                if (this.parserToken.IsTagNameEnd)
                {
                    this.state = State.EndTagName;
                    break;
                }

                this.ParseToken();
            }
        }

        
        
        
        
        
        
        public HtmlAttributeReader AttributeReader
        {
            get
            {
                this.AssertInTag();
                if (this.state == State.ReadTag)
                {
                    throw new InvalidOperationException("Cannot read attributes after reading tag as a markup text");
                }

                return new HtmlAttributeReader(this);
            }
        }

        
        
        
        
        
        
        
        
        public int ReadText(char[] buffer, int offset, int count)
        {
            if (this.state == State.EndText)
            {
                return 0;
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.state != State.Text)
            {
                this.AssertInToken();
                throw new InvalidOperationException("Reader must be positioned on a Text token");
            }

            int lengthRead = 0;

            while (count != 0)
            {
                int chunkLength = this.parserToken.Text.Read(buffer, offset, count);

                if (chunkLength == 0)
                {
                    HtmlTokenId nextToken = this.PreviewNextToken();
                    
                    if (nextToken != HtmlTokenId.Text &&
                        (!this.literalTags || nextToken != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                    {
                        this.state = State.EndText;
                        break;
                    }

                    this.ParseToken();
                    this.parserToken.Text.Rewind();
                    continue;
                }

                offset += chunkLength;
                count -= chunkLength;
                lengthRead += chunkLength;
            }

            return lengthRead;
        }

        
        internal void WriteTextTo(ITextSink sink)
        {
            if (this.state != State.Text)
            {
                this.AssertInToken();
                throw new InvalidOperationException("Reader must be positioned on a Text token");
            }

            while (true)
            {
                this.parserToken.Text.WriteTo(sink);

                HtmlTokenId nextToken = this.PreviewNextToken();
                
                if (nextToken != HtmlTokenId.Text &&
                    (!this.literalTags || nextToken != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                {
                    this.state = State.EndText;
                    break;
                }

                this.ParseToken();
            }
        }

        
        
        
        
        
        
        
        
        
        
        public int ReadMarkupText(char[] buffer, int offset, int count)
        {
            if (this.state == State.EndTag || this.state == State.EndSpecialTag || this.state == State.EndText)
            {
                return 0;
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.state == State.BeginTag)
            {
                this.state = State.ReadTag;
            }
            else if (this.state != State.SpecialTag && this.state != State.ReadTag && this.state != State.Text)
            {
                this.AssertInToken();

                if (this.state > State.BeginTag)
                {
                    throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
                }

                throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
            }

            int lengthRead = 0;

            while (count != 0)
            {
                int chunkLength = this.parserToken.Text.ReadOriginal(buffer, offset, count);

                if (chunkLength == 0)
                {
                    if (this.state == State.SpecialTag)
                    {
                        if (this.parserToken.IsTagEnd)
                        {
                            this.state = State.EndSpecialTag;
                            break;
                        }
                    }
                    else if (this.state == State.ReadTag)
                    {
                        if (this.parserToken.IsTagEnd)
                        {
                            this.state = State.EndTag;
                            break;
                        }
                    }
                    else 
                    {
                        HtmlTokenId nextToken = this.PreviewNextToken();
                        
                        if (nextToken != HtmlTokenId.Text &&
                            (!this.literalTags || nextToken != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                        {
                            this.state = State.EndText;
                            break;
                        }
                    }

                    this.ParseToken();
                    this.parserToken.Text.Rewind();
                    continue;
                }

                offset += chunkLength;
                count -= chunkLength;
                lengthRead += chunkLength;
            }

            return lengthRead;
        }

        
        internal void WriteMarkupTextTo(ITextSink sink)
        {
            if (this.state == State.BeginTag)
            {
                this.state = State.ReadTag;
            }
            else if (this.state != State.SpecialTag && this.state != State.ReadTag && this.state != State.Text)
            {
                this.AssertInToken();

                if (this.state > State.BeginTag)
                {
                    throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
                }

                throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
            }

            while (true)
            {
                this.parserToken.Text.WriteOriginalTo(sink);

                if (this.state == State.SpecialTag)
                {
                    if (this.parserToken.IsTagEnd)
                    {
                        this.state = State.EndSpecialTag;
                        break;
                    }
                }
                else if (this.state == State.ReadTag)
                {
                    if (this.parserToken.IsTagEnd)
                    {
                        this.state = State.EndTag;
                        break;
                    }
                }
                else 
                {
                    HtmlTokenId nextToken = this.PreviewNextToken();
                    
                    if (nextToken != HtmlTokenId.Text &&
                        (!this.literalTags || nextToken != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                    {
                        this.state = State.EndText;
                        break;
                    }
                }

                this.ParseToken();
            }
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

                if (this.parser != null && this.parser is IDisposable)
                {
                    ((IDisposable)this.parser).Dispose();
                }

                if (this.input != null && this.input is IDisposable)
                {
                    ((IDisposable)this.input).Dispose();
                }

                GC.SuppressFinalize(this);
            }

            this.parser = null;
            this.input = null;
            this.stringBuildSink = null;
            this.parserToken = null;
            this.nextParserToken = null;
            this.state = State.Disposed;
        }

        

        internal HtmlReader SetInputEncoding(System.Text.Encoding value)
        {
            this.InputEncoding = value;
            return this;
        }

        internal HtmlReader SetDetectEncodingFromByteOrderMark(bool value)
        {
            this.DetectEncodingFromByteOrderMark = value;
            return this;
        }

#if PRIVATEBUILD
        internal HtmlReader SetDetectEncodingFromMetaTag(bool value)
        {
            this.DetectEncodingFromMetaTag = value;
            return this;
        }
#endif
        internal HtmlReader SetNormalizeHtml(bool value)
        {
            this.NormalizeHtml = value;
            return this;
        }

        internal HtmlReader SetTestBoundaryConditions(bool value)
        {
            this.testBoundaryConditions = value;
            return this;
        }

        internal HtmlReader SetTestTraceStream(Stream value)
        {
            this.testTraceStream = value;
            return this;
        }

        internal HtmlReader SetTestTraceShowTokenNum(bool value)
        {
            this.testTraceShowTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestTraceStopOnTokenNum(int value)
        {
            this.testTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceStream(Stream value)
        {
            this.testNormalizerTraceStream = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceShowTokenNum(bool value)
        {
            this.testNormalizerTraceShowTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceStopOnTokenNum(int value)
        {
            this.testNormalizerTraceStopOnTokenNum = value;
            return this;
        }

        
        private void InitializeAndLock()
        {
            this.locked = true;

            ConverterInput converterInput;

            if (this.input is Stream)
            {
                if (this.inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                converterInput = new ConverterDecodingInput(
                                    (Stream)this.input, 
                                    false,
                                    this.inputEncoding, 
                                    this.detectEncodingFromByteOrderMark, 
                                    
                                    TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), 
                                    TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), 
                                    
                                    this.inputBufferSize,                       
                                    this.testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null); 
            }
            else
            {
                converterInput = new ConverterUnicodeInput(
                                    input, 
                                    false,
                                    
                                    TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), 
                                    this.testBoundaryConditions,
                                    null); 

            }

            HtmlParser preParser = new HtmlParser(
                                    converterInput,
#if PRIVATEBUILD
                                    this.detectEncodingFromMetaTag,
#else
                                    false,
#endif
                                    false, 
                                    
                                    TextConvertersDefaults.MaxTokenRuns(this.testBoundaryConditions),
                                    
                                    TextConvertersDefaults.MaxHtmlAttributes(this.testBoundaryConditions), 
                                    this.testBoundaryConditions);

            if (this.normalizeInputHtml)
            {
                this.parser = new HtmlNormalizingParser(
                                        preParser, 
                                        null, 
                                        false, 
                                        TextConvertersDefaults.MaxHtmlNormalizerNesting(this.testBoundaryConditions),
                                        this.testBoundaryConditions,
                                        this.testNormalizerTraceStream, 
                                        this.testNormalizerTraceShowTokenNum, 
                                        this.testNormalizerTraceStopOnTokenNum);
            }
            else
            {
                this.parser = preParser;
            }

#if PRIVATEBUILD
            if (this.detectEncodingFromMetaTag)
            {
                this.parser.SetRestartConsumer(this);
            }
#endif
        }

        
        bool IRestartable.CanRestart()
        {
            return false;
        }

        
        void IRestartable.Restart()
        {
        }

        
        void IRestartable.DisableRestart()
        {
        }

        
        void IResultsFeedback.Set(ConfigParameter parameterId, object val)
        {
            switch (parameterId)
            {
                case ConfigParameter.InputEncoding:
                        this.inputEncoding = (System.Text.Encoding) val;
                        break;
            }
        }

        
        private void ParseToken()
        {
            if (this.nextParserToken != null)
            {
                this.parserToken = this.nextParserToken;
                this.parserTokenId = this.parserToken.TokenId;
                this.nextParserToken = null;
                return;
            }

            this.parserTokenId = this.parser.Parse();
            this.parserToken = this.parser.Token;

        }

        
        private HtmlTokenId PreviewNextToken()
        {
            if (this.nextParserToken == null)
            {
                this.parser.Parse();
                this.nextParserToken = this.parser.Token;

            }

            return this.nextParserToken.TokenId;
        }

        
        private StringBuildSink GetStringBuildSink()
        {
            if (this.stringBuildSink == null)
            {
                this.stringBuildSink = new StringBuildSink();
            }

            this.stringBuildSink.Reset(int.MaxValue);
            return this.stringBuildSink;
        }

        
        internal bool AttributeReader_ReadNextAttribute()
        {
            if (this.state == State.EndTag)
            {
                return false;
            }

            this.AssertInTag();

            if (this.state == State.ReadTag)
            {
                throw new InvalidOperationException("Cannot read attributes after reading tag as markup text");
            }

            do
            {
                if (this.state >= State.BeginTag &&
                    this.state < State.BeginAttribute)
                {
                    

                    while (this.parserToken.Attributes.Count == 0 && !this.parserToken.IsTagEnd)
                    {
                        this.ParseToken();
                    }

                    if (this.parserToken.Attributes.Count == 0)
                    {
                        InternalDebug.Assert(this.parserToken.IsTagEnd);

                        this.state = State.EndTag;
                        return false;
                    }

                    this.currentAttribute = 0;
                    InternalDebug.Assert(this.parserToken.Attributes[0].IsAttrBegin);

                    this.state = State.BeginAttribute;
                }
                else
                {
                    InternalDebug.Assert(this.currentAttribute < this.parserToken.Attributes.Count);

                    if (++this.currentAttribute == this.parserToken.Attributes.Count)
                    {
                        

                        if (this.parserToken.IsTagEnd)
                        {
                            this.state = State.EndTag;
                            return false;
                        }

                        while (true)
                        {
                            this.ParseToken();

                            InternalDebug.Assert(!this.parserToken.IsTagBegin);

                            if (this.parserToken.Attributes.Count != 0 && (this.parserToken.Attributes[0].IsAttrBegin || this.parserToken.Attributes.Count > 1))
                            {
                                
                                break;
                            }

                            
                            if (this.parserToken.IsTagEnd)
                            {
                                this.state = State.EndTag;
                                return false;
                            }
                        }

                        this.currentAttribute = 0;

                        if (!this.parserToken.Attributes[0].IsAttrBegin)
                        {
                            
                            
                            this.currentAttribute++;
                            InternalDebug.Assert(this.currentAttribute < this.parserToken.Attributes.Count);
                        }
                    }
                }

                
                
                
                if (!this.parserToken.Attributes[this.currentAttribute].IsAttrEmptyName)
                {
                    break;
                }
            }
            while (true);

            this.state = State.BeginAttribute;
            return true;
        }

        
        internal HtmlAttributeId AttributeReader_GetCurrentAttributeId()
        {
            this.AssertInAttribute();

            return HtmlNameData.names[(int)this.parserToken.Attributes[this.currentAttribute].NameIndex].publicAttributeId;
        }

        
        internal bool AttributeReader_CurrentAttributeNameIsLong()
        {
            if (this.state != State.BeginAttribute)
            {
                this.AssertInAttribute();
                throw new InvalidOperationException();
            }

            return this.parserToken.Attributes[this.currentAttribute].NameIndex == HtmlNameIndex.Unknown &&
                    this.parserToken.Attributes[this.currentAttribute].IsAttrBegin && !this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd;
        }

        
        internal string AttributeReader_ReadCurrentAttributeName()
        {
            if (this.state != State.BeginAttribute)
            {
                this.AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            InternalDebug.Assert(this.parserToken.Attributes[this.currentAttribute].IsAttrBegin);

            string name;
            
            if (this.parserToken.Attributes[this.currentAttribute].NameIndex != HtmlNameIndex.Unknown)
            {
                
                name = HtmlNameData.names[(int)this.parserToken.Attributes[this.currentAttribute].NameIndex].name;
            }
            else
            {
                InternalDebug.Assert(this.parserToken.Attributes[this.currentAttribute].IsAttrBegin && this.parserToken.Attributes[this.currentAttribute].NameIndex == HtmlNameIndex.Unknown);

                if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
                {
                    return this.parserToken.Attributes[this.currentAttribute].Name.GetString(int.MaxValue);
                }

                InternalDebug.Assert(!this.parserToken.IsTagEnd && !this.parserToken.Attributes[this.currentAttribute].IsAttrEnd);

                StringBuildSink sbSink = this.GetStringBuildSink();

                this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(sbSink);

                do
                {
                    this.ParseToken();

                    InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                        this.parserToken.Attributes.Count != 0 &&
                        !this.parserToken.Attributes[0].IsAttrBegin);

                    this.currentAttribute = 0;

                    this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(sbSink);
                }
                while (!this.parserToken.Attributes[0].IsAttrNameEnd);

                name = sbSink.ToString();
            }

            
            this.state = State.EndAttributeName;

            
            return name;
        }

        
        internal int AttributeReader_ReadCurrentAttributeName(char[] buffer, int offset, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.state < State.BeginAttribute || this.state > State.EndAttributeName)
            {
                this.AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            int lengthRead = 0;

            if (this.state != State.EndAttributeName)
            {
                if (this.state == State.BeginAttribute)
                {
                    this.state = State.ReadAttributeName;
                    this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
                }

                while (count != 0)
                {
                    int chunkLength = this.parserToken.Attributes[this.currentAttribute].Name.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
                        {
                            this.state = State.EndAttributeName;
                            break;
                        }

                        this.ParseToken();

                        InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                            this.parserToken.Attributes.Count != 0 &&
                            !this.parserToken.Attributes[0].IsAttrBegin);

                        this.currentAttribute = 0;
                        this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void AttributeReader_WriteCurrentAttributeNameTo(ITextSink sink)
        {
            if (this.state != State.BeginAttribute)
            {
                this.AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            while (true)
            {
                this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(sink);

                if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
                {
                    this.state = State.EndAttributeName;
                    break;
                }

                this.ParseToken();

                InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                    this.parserToken.Attributes.Count != 0 &&
                    !this.parserToken.Attributes[0].IsAttrBegin);

                this.currentAttribute = 0;
            }
        }

        
        internal bool AttributeReader_CurrentAttributeHasValue()
        {
            if (this.state != State.BeginAttributeValue)
            {
                this.AssertInAttribute();

                if (this.state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!this.SkipToAttributeValue())
                {
                    this.state = State.EndAttributeName;
                    return false;
                }

                this.state = State.BeginAttributeValue;
            }

            return true;
        }

        
        internal bool AttributeReader_CurrentAttributeValueIsLong()
        {
            if (this.state != State.BeginAttributeValue)
            {
                this.AssertInAttribute();

                if (this.state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!this.SkipToAttributeValue())
                {
                    this.state = State.EndAttributeName;
                    return false;
                }

                this.state = State.BeginAttributeValue;
            }

            return this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin && !this.parserToken.Attributes[this.currentAttribute].IsAttrEnd;
        }

        
        internal string AttributeReader_ReadCurrentAttributeValue()
        {
            if (this.state != State.BeginAttributeValue)
            {
                this.AssertInAttribute();

                if (this.state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!this.SkipToAttributeValue())
                {
                    this.state = State.EndAttribute;
                    return null;
                }

                
            }

            InternalDebug.Assert(this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin);
            
            InternalDebug.Assert(this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin);

            if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
            {
                return this.parserToken.Attributes[this.currentAttribute].Value.GetString(int.MaxValue);
            }

            InternalDebug.Assert(!this.parserToken.IsTagEnd && !this.parserToken.Attributes[this.currentAttribute].IsAttrEnd);

            StringBuildSink sbSink = this.GetStringBuildSink();

            this.parserToken.Attributes[this.currentAttribute].Value.WriteTo(sbSink);

            do
            {
                this.ParseToken();

                InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                    this.parserToken.Attributes.Count != 0 &&
                    !this.parserToken.Attributes[0].IsAttrBegin);

                this.currentAttribute = 0;

                this.parserToken.Attributes[0].Value.WriteTo(sbSink);
            }
            while (!this.parserToken.Attributes[0].IsAttrEnd);

            
            this.state = State.EndAttribute;

            return sbSink.ToString();
        }

        
        internal int AttributeReader_ReadCurrentAttributeValue(char[] buffer, int offset, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            this.AssertInAttribute();

            if (this.state < State.BeginAttributeValue)
            {
                
                if (!this.SkipToAttributeValue())
                {
                    this.state = State.EndAttribute;
                    return 0;
                }

                this.state = State.BeginAttributeValue;
            }

            int lengthRead = 0;

            if (this.state != State.EndAttribute)
            {
                if (this.state == State.BeginAttributeValue)
                {
                    this.state = State.ReadAttributeValue;
                    this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
                }

                while (count != 0)
                {
                    int chunkLength = this.parserToken.Attributes[this.currentAttribute].Value.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
                        {
                            this.state = State.EndAttribute;
                            break;
                        }

                        this.ParseToken();

                        InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                            this.parserToken.Attributes.Count != 0 &&
                            !this.parserToken.Attributes[0].IsAttrBegin);

                        this.currentAttribute = 0;
                        this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void AttributeReader_WriteCurrentAttributeValueTo(ITextSink sink)
        {
            if (this.state != State.BeginAttributeValue)
            {
                this.AssertInAttribute();

                if (this.state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!this.SkipToAttributeValue())
                {
                    this.state = State.EndAttribute;
                    return;
                }

                
            }

            while (true)
            {
                this.parserToken.Attributes[this.currentAttribute].Value.WriteTo(sink);

                if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
                {
                    this.state = State.EndAttribute;
                    break;
                }

                this.ParseToken();

                InternalDebug.Assert(!this.parserToken.IsTagBegin && 
                    this.parserToken.Attributes.Count != 0 &&
                    !this.parserToken.Attributes[0].IsAttrBegin);

                this.currentAttribute = 0;
            }
        }

        
        private bool SkipToAttributeValue()
        {
            if (!this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin)
            {
                if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
                {
                    
                    return false;
                }

                InternalDebug.Assert(!this.parserToken.IsTagEnd);

                do
                {
                    this.ParseToken();
                    InternalDebug.Assert(!this.parserToken.IsTagBegin && this.parserToken.Attributes.Count != 0 && !this.parserToken.Attributes[0].IsAttrBegin);
                }
                while (!this.parserToken.Attributes[0].IsAttrValueBegin && !this.parserToken.Attributes[0].IsAttrEnd);

                if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
                {
                    
                    return false;
                }
            }

            return true;
        }

        
        private void AssertNotLocked()
        {
            this.AssertNotDisposed();
            if (this.locked)
            {
                throw new InvalidOperationException("Cannot set reader properties after reading a first token");
            }
        }

        
        private void AssertNotDisposed()
        {
            if (this.state == State.Disposed)
            {
                throw new ObjectDisposedException("HtmlReader");
            }
        }

        
        private void AssertInToken()
        {
            if (this.state <= State.Begin)
            {
                this.AssertNotDisposed();
                throw new InvalidOperationException("Reader must be positioned inside a valid token");
            }
        }

        
        private void AssertInTag()
        {
            if (this.state < State.BeginTag)
            {
                this.AssertInToken();
                throw new InvalidOperationException("Reader must be positioned inside a StartTag, EndTag or EmptyElementTag token");
            }

            InternalDebug.Assert(this.parserTokenId == HtmlTokenId.Tag && this.parserToken.TagIndex >= HtmlTagIndex.Unknown);
        }

        
        private void AssertInAttribute()
        {
            if (this.state < State.BeginAttribute || this.state > State.EndAttribute)
            {
                this.AssertInTag();
                throw new InvalidOperationException("Reader must be positioned inside attribute");
            }

            InternalDebug.Assert(this.currentAttribute < this.parserToken.Attributes.Count);
        }
    }

    
    
    
    
    internal struct HtmlAttributeReader
    {
        private HtmlReader reader;

        
        internal HtmlAttributeReader(HtmlReader reader)
        {
            this.reader = reader;
        }

        
        
        
        public bool ReadNext()
        {
            return this.reader.AttributeReader_ReadNextAttribute();
        }

        
        
        
        
        
        
        
        public HtmlAttributeId Id
        {
            get
            {
                return this.reader.AttributeReader_GetCurrentAttributeId();
            }
        }

        
        
        
        
        
        
        
        
        
        public bool NameIsLong
        {
            get
            {
                return this.reader.AttributeReader_CurrentAttributeNameIsLong();
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public string ReadName()
        {
            return this.reader.AttributeReader_ReadCurrentAttributeName();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadName(char[] buffer, int offset, int count)          
        {
            return this.reader.AttributeReader_ReadCurrentAttributeName(buffer, offset, count);
        }

        
        internal void WriteNameTo(ITextSink sink)
        {
            this.reader.AttributeReader_WriteCurrentAttributeNameTo(sink);
        }

        
        
        
        
        
        
        
        
        public bool HasValue
        {
            get
            {
                return this.reader.AttributeReader_CurrentAttributeHasValue();
            }
        }

        
        
        
        
        
        
        
        public bool ValueIsLong
        {
            get
            {
                return this.reader.AttributeReader_CurrentAttributeValueIsLong();
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public string ReadValue()
        {
            return this.reader.AttributeReader_ReadCurrentAttributeValue();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadValue(char[] buffer, int offset, int count)         
        {
            return this.reader.AttributeReader_ReadCurrentAttributeValue(buffer, offset, count);
        }

        
        internal void WriteValueTo(ITextSink sink)
        {
            this.reader.AttributeReader_WriteCurrentAttributeValueTo(sink);
        }

        
    }
}
