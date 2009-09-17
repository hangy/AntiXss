// ***************************************************************
// <copyright file="HtmlParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;

    

    internal interface IHtmlParser
    {
        

        HtmlToken Token { get; }

        HtmlTokenId Parse();
        void SetRestartConsumer(IRestartable restartConsumer);
    }

    

    internal class HtmlParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        

        #pragma warning disable 0429 
        private const int ParseThresholdMax = (HtmlNameData.MAX_ENTITY_NAME > HtmlNameData.MAX_NAME ? HtmlNameData.MAX_ENTITY_NAME : HtmlNameData.MAX_NAME) + 2;
        #pragma warning restore 0429

        private ConverterInput input;
        private bool endOfFile;

        
        private bool literalTags;               
        private HtmlNameIndex literalTagNameId;           
        private bool literalEntities;           
        private bool plaintext;                 

        
        
        
        private bool parseConditionals = false;

        
        private ParseState parseState = ParseState.Text;

        private char[] parseBuffer;
        private int parseStart;
        private int parseCurrent;
        private int parseEnd;
        private int parseThreshold = 1;

        private bool slowParse = true;

        private char scanQuote;
        private char valueQuote;
        private CharClass lastCharClass;
        private int nameLength;

        
        private HtmlTokenBuilder tokenBuilder;
        private HtmlToken token;

        private IRestartable restartConsumer;
        private bool detectEncodingFromMetaTag;

        
        private short[] hashValuesTable;

        private bool rightMeta;
        private Encoding newEncoding;

        private SavedParserState savedState;

        
        public HtmlParser(
                ConverterInput input,
                bool detectEncodingFromMetaTag,
                bool preformatedText,
                int maxRuns,
                int maxAttrs,
                bool testBoundaryConditions)
        {
            this.input = input;
            this.detectEncodingFromMetaTag = detectEncodingFromMetaTag;
            input.SetRestartConsumer(this);

            this.tokenBuilder = new HtmlTokenBuilder(null, maxRuns, maxAttrs, testBoundaryConditions);

            this.token = this.tokenBuilder.Token;

            this.plaintext = preformatedText;
            this.literalEntities = preformatedText;
        }

        
        protected enum ParseState : byte
        {
            Text,
            TagStart,
            TagNamePrefix,
            TagName,
            TagWsp,
            AttrNameStart,
            AttrNamePrefix,
            AttrName,
            AttrWsp,
            AttrValueWsp,
            AttrValue,
            EmptyTagEnd,
            TagEnd,
            TagSkip,
            CommentStart,
            Comment,
            Conditional,
            CommentConditional,
            Bang,
            Dtd,
            Asp,
        }

        
        public HtmlToken Token
        {
            get
            {
                return this.token;
            }
        }

        
        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        
        private void Reinitialize()
        {
            this.endOfFile = false;

            this.literalTags = false;
            this.literalTagNameId = HtmlNameIndex._NOTANAME;
            this.literalEntities = false;
            this.plaintext = false;

            this.parseState = ParseState.Text;

            this.parseBuffer = null;
            this.parseStart = 0;
            this.parseCurrent = 0;
            this.parseEnd = 0;
            this.parseThreshold = 1;

            this.slowParse = true;  

            this.scanQuote = '\0';
            this.valueQuote = '\0';
            this.lastCharClass = 0;
            this.nameLength = 0;

            this.tokenBuilder.Reset();
            this.tokenBuilder.MakeEmptyToken(HtmlTokenId.Restart);
        }

        
        public bool ParsingFragment
        {
            get { return this.savedState != null && this.savedState.StateSaved; }
        }

        
        public void PushFragment(ConverterInput fragmentInput, bool literalTextInput)
        {
            if (this.savedState == null)
            {
                this.savedState = new SavedParserState();
            }

            this.savedState.PushState(this, fragmentInput, literalTextInput);
        }

        
        public void PopFragment()
        {
            this.savedState.PopState(this);
        }

        
        private class SavedParserState
        {
            private ConverterInput input;
            private bool endOfFile;
            private ParseState parseState;
            private bool slowParse;
            private bool literalTags;
            private HtmlNameIndex literalTagNameId;
            private bool literalEntities;
            private bool plaintext;
            private char[] parseBuffer;
            private int parseStart;
            private int parseCurrent;
            private int parseEnd;
            private int parseThreshold;

            public bool StateSaved { get { return this.input != null; } }

            public void PushState(HtmlParser parser, ConverterInput newInput, bool literalTextInput)
            {
                InternalDebug.Assert(!this.StateSaved);
                InternalDebug.Assert(parser.parseState == ParseState.Text || parser.parseState == ParseState.TagStart);

                this.input = parser.input;
                this.endOfFile = parser.endOfFile;
                this.parseState = parser.parseState;
                this.slowParse = parser.slowParse;
                this.literalTags = parser.literalTags;
                this.literalTagNameId = parser.literalTagNameId;
                this.literalEntities = parser.literalEntities;
                this.plaintext = parser.plaintext;
                this.parseBuffer = parser.parseBuffer;
                this.parseStart = parser.parseStart;
                this.parseCurrent = parser.parseCurrent;
                this.parseEnd = parser.parseEnd;
                this.parseThreshold = parser.parseThreshold;

                parser.input = newInput;
                parser.endOfFile = false;
                parser.parseState = ParseState.Text;
                parser.slowParse = true;
                parser.literalTags = literalTextInput;
                parser.literalTagNameId = HtmlNameIndex.PlainText;
                parser.literalEntities = literalTextInput;
                parser.plaintext = literalTextInput;
                parser.parseBuffer = null;
                parser.parseStart = 0;
                parser.parseCurrent = 0;
                parser.parseEnd = 0;
                parser.parseThreshold = 1;
            }

            public void PopState(HtmlParser parser)
            {
                InternalDebug.Assert(this.StateSaved);
                InternalDebug.Assert(parser.endOfFile);

                parser.input = this.input;
                parser.endOfFile = this.endOfFile;
                parser.parseState = this.parseState;
                parser.slowParse = this.slowParse;
                parser.literalTags = this.literalTags;
                parser.literalTagNameId = this.literalTagNameId;
                parser.literalEntities = this.literalEntities;
                parser.plaintext = this.plaintext;
                parser.parseBuffer = this.parseBuffer;
                parser.parseStart = this.parseStart;
                parser.parseCurrent = this.parseCurrent;
                parser.parseEnd = this.parseEnd;
                parser.parseThreshold = this.parseThreshold;

                this.input = null;
                this.parseBuffer = null;
            }
        }

        
        bool IRestartable.CanRestart()
        {
            return this.restartConsumer != null && this.restartConsumer.CanRestart();
        }

        
        void IRestartable.Restart()
        {
            if (this.restartConsumer != null)
            {
                this.restartConsumer.Restart();
            }

            this.Reinitialize();
        }

        
        void IRestartable.DisableRestart()
        {
            if (this.restartConsumer != null)
            {
                this.restartConsumer.DisableRestart();
                this.restartConsumer = null;
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(this.input is IReusable);

            ((IReusable)this.input).Initialize(newSourceOrDestination);

            this.Reinitialize();

            this.input.SetRestartConsumer(this);
        }

        
        public void Initialize(string fragment, bool preformatedText)
        {
            InternalDebug.Assert(this.input is ConverterBufferInput);

            (this.input as ConverterBufferInput).Initialize(fragment);

            this.Reinitialize();

            this.plaintext = preformatedText;
            this.literalEntities = preformatedText;
        }

        
        void IDisposable.Dispose()
        {
            if (this.input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)this.input).Dispose();
            }

            this.input = null;
            this.restartConsumer = null;
            this.parseBuffer = null;
            this.token = null;
            this.tokenBuilder = null;
            this.hashValuesTable = null;

            GC.SuppressFinalize(this);
        }

        
        public HtmlTokenId Parse()
        {
            if (this.slowParse)
            {
                return this.ParseSlow();
            }

            
            
            
            
            
            
            InternalDebug.Assert(this.parseBuffer[this.parseCurrent] == '<' && this.parseState == ParseState.TagStart);

            if (this.tokenBuilder.Valid)
            {
                

                
                InternalDebug.Assert(!this.tokenBuilder.IncompleteTag);

                this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                this.parseStart = this.parseCurrent;

                this.tokenBuilder.Reset();
            }

            
            

            char[] parseBuffer = this.parseBuffer;
            int parseCurrent = this.parseCurrent;

            int runStart = parseCurrent;
            bool endTag = false;

            char ch = parseBuffer[++parseCurrent];

            if (ch == '/')
            {
                endTag = true;
                ch = parseBuffer[++parseCurrent];
            }

            CharClass charClass = ParseSupport.GetCharClass(ch);

            if (!ParseSupport.AlphaCharacter(charClass))
            {
                
                goto StartSlowly;
            }

            this.tokenBuilder.StartTag(HtmlNameIndex.Unknown, runStart);
            if (endTag)
            {
                this.tokenBuilder.SetEndTag();
            }

            
            
            this.tokenBuilder.DebugPrepareToAddMoreRuns(6);

            this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

            this.tokenBuilder.StartTagName();

            int nameLength = 0;

            runStart = parseCurrent;

            this.parseState = ParseState.TagNamePrefix;

            do
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));

            if (ch == ':')
            {
                
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);

                
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);

                this.tokenBuilder.EndTagNamePrefix();

                nameLength = parseCurrent + 1 - runStart;
                runStart = parseCurrent + 1;

                
                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));

                this.parseState = ParseState.TagName;
            }

            if (parseCurrent != runStart)
            {
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                nameLength += parseCurrent - runStart;
            }

            if (!ParseSupport.HtmlEndTagNameCharacter(charClass))
            {
                
                goto ContinueSlowly;
            }

            this.tokenBuilder.EndTagName(nameLength);

    ScanNextAttributeOrEnd:

            this.tokenBuilder.AssertPreparedToAddMoreRuns(2);

            

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
            }

    CheckEndOfTag:        

            this.tokenBuilder.AssertPreparedToAddMoreRuns(1);

            if (ch == '>' || (ch == '/' && parseBuffer[parseCurrent + 1] == '>'))
            {
                

                runStart = parseCurrent;

                if (ch == '/')
                {
                    parseCurrent ++;
                    this.tokenBuilder.SetEmptyScope();
                }
                parseCurrent ++;

                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);
                this.tokenBuilder.EndTag(true);

                InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                if (parseBuffer[parseCurrent] == '<')    
                {
                    this.parseState = ParseState.TagStart;
                }
                else
                {
                    this.parseState = ParseState.Text;
                    this.slowParse = true;
                }

                this.parseCurrent = parseCurrent;

                this.HandleSpecialTag();

                return this.token.TokenId;
            }

            this.parseState = ParseState.TagWsp;
            if (!ParseSupport.HtmlSimpleAttrNameCharacter(charClass) ||
                !this.tokenBuilder.CanAddAttribute() ||
                !this.tokenBuilder.PrepareToAddMoreRuns(11))
            {
                
                goto ContinueSlowly;
            }

            
            

            this.tokenBuilder.StartAttribute();

            nameLength = 0;

            runStart = parseCurrent;

            this.parseState = ParseState.AttrNamePrefix;

            do
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));

            if (ch == ':')
            {
                
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);

                
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);

                this.tokenBuilder.EndAttributeNamePrefix();

                nameLength = parseCurrent + 1 - runStart;
                runStart = parseCurrent + 1;

                
                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));

                this.parseState = ParseState.AttrName;
            }

            if (parseCurrent != runStart)
            {
                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                nameLength += parseCurrent - runStart;
            }

            if (!ParseSupport.HtmlEndAttrNameCharacter(charClass))
            {
                
                goto ContinueSlowly;
            }

            this.tokenBuilder.EndAttributeName(nameLength);

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);

                this.parseState = ParseState.AttrWsp;
                if (ParseSupport.InvalidUnicodeCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }
            }

            if (ch != '=')
            {
                

                this.tokenBuilder.EndAttribute();

                goto CheckEndOfTag;
            }

            this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, parseCurrent, parseCurrent + 1);

            
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                this.tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);

                this.parseState = ParseState.AttrValueWsp;
                if (ParseSupport.InvalidUnicodeCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }
            }

            if (ParseSupport.QuoteCharacter(charClass))
            {
                this.valueQuote = ch;

                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, parseCurrent, parseCurrent + 1);

                this.tokenBuilder.StartValue();
                this.tokenBuilder.SetValueQuote(this.valueQuote);

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass))
                {
                    runStart = parseCurrent;

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass));

                    
                    this.tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
                }

                if (ch != this.valueQuote)
                {
                    this.scanQuote = this.valueQuote;
                    this.parseState = ParseState.AttrValue;
                    
                    goto ContinueSlowly;
                }

                this.valueQuote = '\0';

                this.tokenBuilder.EndValue();

                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, parseCurrent, parseCurrent + 1);

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                this.tokenBuilder.EndAttribute();

                goto ScanNextAttributeOrEnd;
            }
            else if (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass))
            {
                this.tokenBuilder.StartValue();

                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass));

                this.tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, runStart, parseCurrent);

                this.parseState = ParseState.AttrValue;
                if (!ParseSupport.HtmlEndAttrUnquotedValueCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }

                this.tokenBuilder.EndValue();

                this.tokenBuilder.EndAttribute();

                goto ScanNextAttributeOrEnd;
            }

            
            

            this.parseState = ParseState.AttrValueWsp;

    ContinueSlowly:

            this.parseCurrent = parseCurrent;
            this.lastCharClass = ParseSupport.GetCharClass(parseBuffer[parseCurrent - 1]);
            this.nameLength = nameLength;

    StartSlowly:

            this.slowParse = true;
            return this.ParseSlow();
        }

        
        public HtmlTokenId ParseSlow()
        {
            InternalDebug.Assert(this.parseCurrent >= this.parseStart);

            if (this.tokenBuilder.Valid)
            {
                

                if (this.tokenBuilder.IncompleteTag)
                {
                    int newBase = this.tokenBuilder.RewindTag();

                    
                    

                    InternalDebug.Assert(this.parseCurrent >= newBase);

                    this.input.ReportProcessed(newBase - this.parseStart);
                    this.parseStart = newBase;
                }
                else
                {
                    this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                    this.parseStart = this.parseCurrent;

                    this.tokenBuilder.Reset();
                }
            }

            while (true)
            {
                InternalDebug.Assert(this.parseThreshold > 0);

                

                bool forceFlushToken = false;

                if (this.parseCurrent + this.parseThreshold > this.parseEnd)
                {
                    
                    
                    
                    
                    

                    if (!this.endOfFile)
                    {
                        if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
                        {
                            

                            
                            InternalDebug.Assert(!this.tokenBuilder.Valid);

                            return HtmlTokenId.None;
                        }

                        
                        

                        this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);

                        ConverterDecodingInput decodingInput = this.input as ConverterDecodingInput;

                        if (decodingInput != null && decodingInput.EncodingChanged)
                        {
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            

                            
                            decodingInput.EncodingChanged = false;

                            
                            
                            

                            
                            return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, decodingInput.Encoding.CodePage);
                        }

                        if (this.input.EndOfFile)
                        {
                            this.endOfFile = true;
                        }

                        
                        if (!this.endOfFile && this.parseEnd - this.parseStart < this.input.MaxTokenSize)
                        {
                            
                            continue;
                        }
                    }

                    
                    forceFlushToken = true;
                }

                
                InternalDebug.Assert(this.parseEnd > this.parseCurrent || forceFlushToken);

                

                

                char ch = this.parseBuffer[this.parseCurrent];
                CharClass charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.InvalidUnicodeCharacter(charClass) || this.parseThreshold > 1)
                {
                    
                    
                    
                    
                    

                    bool aboveThreshold = this.SkipInvalidCharacters(ref ch, ref charClass, ref this.parseCurrent);

                    
                    
                    if (this.token.IsEmpty)
                    {
                        this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                        this.parseStart = this.parseCurrent;

                        if (this.tokenBuilder.IncompleteTag)
                        {
                            
                            this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);
                        }
                    }

                    if (!aboveThreshold)
                    {
                        

                        if (!forceFlushToken)
                        {
                            
                            continue;
                        }

                        

                        if (this.parseCurrent == this.parseEnd && !this.tokenBuilder.IsStarted && this.endOfFile)
                        {
                            
                            break;
                        }

                        
                    }

                    
                    this.parseThreshold = 1;
                }

                if (this.ParseStateMachine(ch, charClass, forceFlushToken))
                {
                    return this.token.TokenId;
                }
            }

            return this.tokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
        }

        
        public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
        {
            char chT;
            CharClass charClassT;

            HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            char[] parseBuffer = this.parseBuffer;
            int parseCurrent = this.parseCurrent;
            int parseEnd = this.parseEnd;

            int runStart = parseCurrent;

            InternalDebug.Assert(parseBuffer[parseCurrent] == ch);

            switch (this.parseState)
            {
                

                case ParseState.Text:

                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        if (ch == '<' && !this.plaintext)
                        {
                            
                            this.parseState = ParseState.TagStart;
                            goto case ParseState.TagStart;
                        }

                    ContinueText:

                        tokenBuilder.StartText(runStart);

                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(3);

                        this.ParseText(ch, charClass, ref parseCurrent);

                        if (this.token.IsEmpty && !forceFlushToken)
                        {
                            
                            InternalDebug.Assert(parseCurrent == runStart);
                            InternalDebug.Assert(this.parseState != ParseState.Text || this.parseThreshold > 1);

                            tokenBuilder.Reset();     
                            this.slowParse = true;
                            break;  
                        }

                        

                        tokenBuilder.EndText();

                        this.parseCurrent = parseCurrent;

                        return true;

                

                case ParseState.TagStart:

                        InternalDebug.Assert(ch == '<');
                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(4);

                        chT = parseBuffer[parseCurrent + 1];        
                        charClassT = ParseSupport.GetCharClass(chT);

                        bool endTag = false;

                        if (chT == '/')
                        {
                            

                            chT = parseBuffer[parseCurrent + 2];    
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                
                                
                                

                                if (!this.endOfFile || parseCurrent + 2 < parseEnd)
                                {
                                    

                                    this.parseThreshold = 3;    
                                    break;  
                                }

                                
                            }

                            parseCurrent ++;

                            endTag = true;
                        }
                        else if (!ParseSupport.AlphaCharacter(charClassT) || this.literalTags)
                        {
                            if (chT == '!')
                            {
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                

                                this.parseState = ParseState.CommentStart;
                                goto case ParseState.CommentStart;
                            }
                            else if (chT == '?' && !this.literalTags)
                            {
                                parseCurrent += 2;

                                tokenBuilder.StartTag(HtmlNameIndex._DTD, runStart);

                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                tokenBuilder.StartTagText();

                                this.lastCharClass = charClassT;

                                ch = parseBuffer[parseCurrent];
                                charClass = ParseSupport.GetCharClass(ch);

                                runStart = parseCurrent;

                                this.parseState = ParseState.Dtd;
                                goto case ParseState.Dtd;
                            }
                            else if (chT == '%')
                            {
                                parseCurrent += 2;

                                tokenBuilder.StartTag(HtmlNameIndex._ASP, runStart);

                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                tokenBuilder.StartTagText();

                                ch = parseBuffer[parseCurrent];
                                charClass = ParseSupport.GetCharClass(ch);

                                runStart = parseCurrent;

                                this.parseState = ParseState.Asp;
                                goto case ParseState.Asp;
                            }
                            else if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                

                                if (!this.endOfFile || parseCurrent + 1 < parseEnd)
                                {
                                    

                                    this.parseThreshold = 2;    
                                    break;  
                                }

                                
                            }

                            
                            
                            

                            
                            

                            
                            this.parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        
                        

                        parseCurrent ++;

                        this.lastCharClass = charClass;

                        ch = chT;
                        charClass = charClassT;

                        tokenBuilder.StartTag(HtmlNameIndex.Unknown, runStart);
                        if (endTag)
                        {
                            tokenBuilder.SetEndTag();
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                        this.nameLength = 0;
                        tokenBuilder.StartTagName();

                        runStart = parseCurrent;

                        this.parseState = ParseState.TagNamePrefix;
                        goto case ParseState.TagNamePrefix;

                

                case ParseState.TagNamePrefix:

                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.Name))
                        {
                            
                            
                            

                            goto SplitTag;
                        }

                        ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagNamePrefix);

                        if (parseCurrent != runStart)
                        {
                            this.nameLength += (parseCurrent - runStart);

                            if (this.literalTags && (this.nameLength > HtmlNameData.MAX_TAG_NAME || ch == '<'))
                            {
                                
                                
                                

                                goto RejectTag;
                            }

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        if (ch != ':')
                        {
                            goto EndTagName;
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
                        this.nameLength ++;

                        this.tokenBuilder.EndTagNamePrefix();

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        this.parseState = ParseState.TagName;
                        goto case ParseState.TagName;

                

                case ParseState.TagName:

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.Name))
                        {
                            
                            
                            

                            goto SplitTag;
                        }

                        ch = this.ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagName);

                        if (parseCurrent != runStart)
                        {
                            this.nameLength += (parseCurrent - runStart);

                            if (this.literalTags && (this.nameLength > HtmlNameData.MAX_TAG_NAME || ch == '<'))
                            {
                                
                                
                                

                                goto RejectTag;
                            }

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                EndTagName:

                        

                        tokenBuilder.EndTagName(this.nameLength);

                        InternalDebug.Assert(!this.literalTags || this.token.IsEndTag);

                        if (this.literalTags && this.token.NameIndex != this.literalTagNameId)
                        {
                            
                            

                            goto RejectTag;
                        }

                        

                        runStart = parseCurrent;

                        if (ch == '>')
                        {
                            this.parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            this.parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        
                        

                        this.lastCharClass = charClass;

                        this.parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.TagWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        if (ch == '>')
                        {
                            this.parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            this.parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        this.parseState = ParseState.AttrNameStart;
                        goto case ParseState.AttrNameStart;

                case ParseState.AttrNameStart:

                        if (!tokenBuilder.CanAddAttribute() || !tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        this.nameLength = 0;

                        tokenBuilder.StartAttribute();

                        this.parseState = ParseState.AttrNamePrefix;
                        goto case ParseState.AttrNamePrefix;

                

                case ParseState.AttrNamePrefix:

                        InternalDebug.Assert(this.valueQuote == '\0');

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrNamePrefix);

                        if (parseCurrent != runStart)
                        {
                            this.nameLength += (parseCurrent - runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        if (ch != ':')
                        {
                            goto EndAttributeName;
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
                        this.nameLength ++;

                        this.tokenBuilder.EndAttributeNamePrefix();

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        this.parseState = ParseState.AttrName;
                        goto case ParseState.AttrName;

                

                case ParseState.AttrName:

                        InternalDebug.Assert(this.valueQuote == '\0');

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrName);

                        if (parseCurrent != runStart)
                        {
                            this.nameLength += (parseCurrent - runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                EndAttributeName:

                        tokenBuilder.EndAttributeName(this.nameLength);

                        runStart = parseCurrent;

                         if (ch == '=')
                        {
                            goto HandleAttrEqual;
                        }

                        InternalDebug.Assert(ch == '>' || ch == '/' || ParseSupport.WhitespaceCharacter(charClass));

                        this.lastCharClass = charClass;

                        this.parseState = ParseState.AttrWsp;
                        goto case ParseState.AttrWsp;

                

                case ParseState.AttrWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        if (ch != '=')
                        {
                            
                            tokenBuilder.EndAttribute();

                            InternalDebug.Assert(this.valueQuote == '\0');

                            if (ch == '>')
                            {
                                this.parseState = ParseState.TagEnd;
                                goto case ParseState.TagEnd;
                            }
                            else if (ch == '/')
                            {
                                this.parseState = ParseState.EmptyTagEnd;
                                goto case ParseState.EmptyTagEnd;
                            }

                            this.parseState = ParseState.AttrNameStart;
                            goto case ParseState.AttrNameStart;
                        }

                HandleAttrEqual:

                        InternalDebug.Assert(ch == '=');

                        this.lastCharClass = charClass;

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, runStart, parseCurrent);

                        runStart = parseCurrent;

                        this.parseState = ParseState.AttrValueWsp;
                        goto case ParseState.AttrValueWsp;

                

                case ParseState.AttrValueWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        InternalDebug.Assert(this.valueQuote == '\0');

                        if (ParseSupport.QuoteCharacter(charClass))
                        {
                            if (ch == this.scanQuote)
                            {
                                this.scanQuote = '\0';
                            }
                            else if (this.scanQuote == '\0')
                            {
                                InternalDebug.Assert(ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass));
                                this.scanQuote = ch;
                            }

                            this.valueQuote = ch;

                            this.lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, runStart, parseCurrent);

                            runStart = parseCurrent;
                        }

                        tokenBuilder.StartValue();

                        if (this.valueQuote != '\0')
                        {
                            tokenBuilder.SetValueQuote(this.valueQuote);
                        }

                        this.parseState = ParseState.AttrValue;
                        goto case ParseState.AttrValue;
                        
                

                case ParseState.AttrValue:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.AttrValue))  
                        {
                            goto SplitTag;
                        }

                        if (!this.ParseAttributeText(ch, charClass, ref parseCurrent))
                        {
                            goto SplitTag;
                        }

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        if (ParseSupport.InvalidUnicodeCharacter(charClass) || this.parseThreshold > 1)
                        {
                            goto HandleTagEOB;
                        }

                        tokenBuilder.EndValue();

                        runStart = parseCurrent;

                        if (ch == this.valueQuote)
                        {
                            this.lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, runStart, parseCurrent);

                            this.valueQuote = '\0';

                            runStart = parseCurrent;
                        }

                        tokenBuilder.EndAttribute();

                        InternalDebug.Assert(this.valueQuote == '\0');

                        if (ch == '>')
                        {
                            this.parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            this.parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        this.parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.EmptyTagEnd:

                        InternalDebug.Assert(ch == '/');

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        chT = parseBuffer[parseCurrent + 1];        
                        charClassT = ParseSupport.GetCharClass(chT);

                        if (chT == '>')
                        {
                            

                            tokenBuilder.SetEmptyScope();

                            parseCurrent ++;

                            this.lastCharClass = charClass;

                            ch = chT;
                            charClass = charClassT;

                            this.parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClassT) && (!this.endOfFile || parseCurrent + 1 < parseEnd))
                        {
                            this.parseThreshold = 2;
                            goto HandleTagEOB;
                        }

                        

                        this.lastCharClass = charClass;

                        parseCurrent ++;

                        ch = chT;
                        charClass = charClassT;

                        runStart = parseCurrent;

                        this.parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.TagEnd:

                        InternalDebug.Assert(ch == '>');

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagSuffix))
                        {
                            goto SplitTag;
                        }

                        this.lastCharClass = charClass;

                        parseCurrent ++;

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);

                        if (this.scanQuote != '\0')
                        {
                            runStart = parseCurrent;

                            ch = parseBuffer[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            this.parseState = ParseState.TagSkip;
                            goto case ParseState.TagSkip;
                        }

                        
                        tokenBuilder.EndTag(true);

                        InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                        if (parseBuffer[parseCurrent] == '<')   
                        {
                            this.parseState = ParseState.TagStart;
                            this.slowParse = false;             
                        }
                        else
                        {
                            this.parseState = ParseState.Text;
                        }

                        this.parseCurrent = parseCurrent;

                        this.HandleSpecialTag();

                        return true;

                

                case ParseState.TagSkip:

                        

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagText))
                        {
                            goto SplitTag;
                        }

                        ch = this.ScanSkipTag(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            
                            goto HandleTagEOB;
                        }

                        InternalDebug.Assert(ch == '>' && this.scanQuote == '\0');

                        parseCurrent ++;

                        
                        tokenBuilder.EndTag(true);

                        InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                        if (parseBuffer[parseCurrent] == '<')    
                        {
                            this.parseState = ParseState.TagStart;
                            this.slowParse = false;
                        }
                        else
                        {
                            this.parseState = ParseState.Text;
                        }

                        this.parseCurrent = parseCurrent;

                        this.HandleSpecialTag();

                        return true;

                
                

                HandleTagEOB:

                        if (!forceFlushToken || parseCurrent + this.parseThreshold < parseEnd)
                        {
                            

                            break;  
                        }

                        

                        if (this.endOfFile)
                        {
                            if (parseCurrent < parseEnd)
                            {
                                

                                if (this.ScanForInternalInvalidCharacters(parseCurrent))
                                {
                                    
                                    break;  
                                }

                                
                                
                                parseCurrent = parseEnd;
                            }

                            

                            if (this.token.IsTagBegin)
                            {
                                

                                goto RejectTag;
                            }

                            
                            

                            tokenBuilder.EndTag(true);

                            this.parseCurrent = parseCurrent;

                            this.HandleSpecialTag();

                            this.parseState = ParseState.Text;

                            return true;
                        }

                        
                        

                SplitTag:

                        if (this.literalTags && this.token.NameIndex == HtmlNameIndex.Unknown)
                        {
                            
                            
                            
                            InternalDebug.Assert(this.token.IsTagBegin);
                            goto RejectTag;
                        }

                        

                        tokenBuilder.EndTag(false);

                        this.parseCurrent = parseCurrent;

                        this.HandleSpecialTag();

                        return true;

                RejectTag:

                        

                        
                        
                        InternalDebug.Assert(this.token.IsTagBegin);

                        parseCurrent = this.parseStart;

                        this.scanQuote = this.valueQuote = '\0';

                        tokenBuilder.Reset();

                        runStart = parseCurrent;

                        

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        InternalDebug.Assert(ch == '<');

                        this.parseState = ParseState.Text;
                        goto ContinueText;

                

                case ParseState.CommentStart:                   

                        
                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(3);

                        InternalDebug.Assert(ch == '<');
                        InternalDebug.Assert(parseBuffer[parseCurrent + 1] == '!');
                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        

                        int pos = 2;

                        chT = parseBuffer[parseCurrent + pos];                

                        if (chT == '-')
                        {
                            pos ++;     

                            chT = parseBuffer[parseCurrent + pos];            

                            if (chT == '-')
                            {
                                

                                pos ++;     
                                
                                chT = parseBuffer[parseCurrent + pos];        

                                if (chT == '[' && this.parseConditionals)
                                {
                                    
                                    
                                    
                                    
                                    

                                    parseCurrent += 5;

                                    tokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                    tokenBuilder.StartTagText();

                                    ch = parseBuffer[parseCurrent];
                                    charClass = ParseSupport.GetCharClass(ch);

                                    runStart = parseCurrent;

                                    this.parseState = ParseState.CommentConditional;
                                    goto case ParseState.CommentConditional;
                                }
                                else if (chT == '>')
                                {
                                    

                                    parseCurrent += 5;

                                    tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent - 1);
                                    tokenBuilder.StartTagText();
                                    tokenBuilder.EndTagText();
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 1, parseCurrent);

                                    tokenBuilder.EndTag(true);

                                    InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                                    this.parseState = ParseState.Text;
                                    
                                    this.parseCurrent = parseCurrent;

                                    return true;
                                }
                                else if (chT == '-')
                                {
                                    pos ++;     

                                    chT = parseBuffer[parseCurrent + pos];    

                                    if (chT == '>')
                                    {
                                        

                                        parseCurrent += 6;

                                        tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent - 2);
                                        tokenBuilder.StartTagText();
                                        tokenBuilder.EndTagText();
                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 2, parseCurrent);

                                        tokenBuilder.EndTag(true);

                                        InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                                        this.parseState = ParseState.Text;
                                        
                                        this.parseCurrent = parseCurrent;

                                        return true;
                                    }
                                }

                                charClassT = ParseSupport.GetCharClass(chT);

                                if (!ParseSupport.InvalidUnicodeCharacter(charClassT))
                                {
                                    parseCurrent += 4;

                                    tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                    tokenBuilder.StartTagText();

                                    ch = parseBuffer[parseCurrent];
                                    charClass = ParseSupport.GetCharClass(ch);

                                    runStart = parseCurrent;

                                    this.parseState = ParseState.Comment;
                                    goto case ParseState.Comment;
                                }
                            }
                        }
                        else if (chT == '[' && this.parseConditionals)
                        {
                            
                            
                            
                            
                            

                            parseCurrent += 3;

                            tokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                            tokenBuilder.StartTagText();

                            ch = parseBuffer[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            runStart = parseCurrent;

                            this.parseState = ParseState.Conditional;
                            goto case ParseState.Conditional;
                        }

                        
                        

                        charClassT = ParseSupport.GetCharClass(chT);

                        if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                        {
                            

                            

                            if (!this.endOfFile || parseCurrent + pos < parseEnd)
                            {
                                

                                this.parseThreshold = pos + 1;
                                break;  
                            }

                            

                            InternalDebug.Assert(ch == '<');

                            this.parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        

                        if (this.literalTags)
                        {
                            
                            this.parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        parseCurrent += 2;

                        tokenBuilder.StartTag(HtmlNameIndex._BANG, runStart);

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                        tokenBuilder.StartTagText();

                        this.lastCharClass = ParseSupport.GetCharClass('!');

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        this.parseState = ParseState.Bang;
                        goto case ParseState.Bang;

                
                
                
                

                case ParseState.CommentConditional:             
                case ParseState.Conditional:                    
                case ParseState.Comment:                        
                case ParseState.Bang:                           
                case ParseState.Asp:                            
                case ParseState.Dtd:                            

                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagText))
                        {
                            goto SplitTag;
                        }

                        while (!ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            

                            if (ParseSupport.QuoteCharacter(charClass))
                            {
                                if (ch == this.scanQuote)
                                {
                                    this.scanQuote = '\0';
                                }
                                else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                                {
                                    this.scanQuote = ch;
                                }
                            }
                            else if (ParseSupport.HtmlSuffixCharacter(charClass))
                            {
                                int addToTextCnt, tagSuffixCnt;
                                bool endScan;

                                if (this.CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan))
                                {
                                    if (!endScan)
                                    {
                                        

                                        InternalDebug.Assert(tagSuffixCnt == 0);

                                        parseCurrent += addToTextCnt;

                                        this.lastCharClass = charClass;
                                        InternalDebug.Assert(!ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass));

                                        ch = parseBuffer[parseCurrent];
                                        charClass = ParseSupport.GetCharClass(ch);

                                        continue;   
                                    }

                                    this.scanQuote = '\0';

                                    

                                    parseCurrent += addToTextCnt;

                                    if (parseCurrent != runStart)
                                    {
                                        tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);
                                    }

                                    tokenBuilder.EndTagText();

                                    if (tagSuffixCnt != 0)
                                    {
                                        runStart = parseCurrent;

                                        parseCurrent += tagSuffixCnt;

                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);
                                    }

                                    tokenBuilder.EndTag(true);

                                    InternalDebug.Assert(this.scanQuote == '\0' && this.valueQuote == '\0');

                                    this.parseState = ParseState.Text;

                                    this.parseCurrent = parseCurrent;

                                    return true;
                                }

                                
                                
                                
                                

                                InternalDebug.Assert(tagSuffixCnt >= 1);

                                parseCurrent += addToTextCnt;
                                this.parseThreshold = tagSuffixCnt + 1;

                                break; 
                            }

                            this.lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);

                            if (!tokenBuilder.PrepareToAddMoreRuns(2))
                            {
                                
                                goto SplitTag;
                            }
                        }

                        InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent + this.parseThreshold - 1])));

                        if (forceFlushToken && parseCurrent + this.parseThreshold > parseEnd)
                        {
                            
                            

                            if (this.endOfFile && parseCurrent < parseEnd)
                            {
                                
                                
                                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, parseCurrent, parseEnd);

                                parseCurrent = parseEnd;
                            }

                            tokenBuilder.EndTag(this.endOfFile);

                            this.parseCurrent = parseCurrent;

                            return true;
                        }

                        
                        break;  

                

                default:
                        InternalDebug.Assert(false);  
                        this.parseCurrent = parseCurrent;
                        throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException("internal error: invalid parse state");

            }

            this.parseCurrent = parseCurrent;
            return false;
        }

        
        private static void ProcessNumericEntityValue(int entityValue, out int literal)
        {
            if (entityValue < 0x10000)
            {
                if (0x80 <= entityValue && entityValue <= 0x9F)
                {
                    literal = ParseSupport.Latin1MappingInUnicodeControlArea(entityValue);
                }
                else if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass((char)entityValue)))
                {
                    literal = '?';
                }
                else
                {
                    literal = entityValue;
                }
            }
            else if (entityValue < 0x110000)
            {
                literal = entityValue;
            }
            else
            {
                literal = '?';
            }
        }

        
        private static bool FindEntityByHashName(short hash, char[] buffer, int nameOffset, int nameLength, out int entityValue)
        {
            entityValue = 0;

            bool found = false;
            HtmlEntityIndex nameIndex = HtmlNameData.entityHashTable[hash];

            if (nameIndex > 0)
            {
                do
                {
                    if (HtmlNameData.entities[(int)nameIndex].name.Length == nameLength)
                    {
                        int i;

                        

                        for (i = 0; i < nameLength; i++)
                        {
                            if (HtmlNameData.entities[(int)nameIndex].name[i] != (char)buffer[nameOffset + i])
                            {
                                break;
                            }
                        }

                        if (i == nameLength)
                        {
                            entityValue = (int)HtmlNameData.entities[(int)nameIndex].value;
                            found = true;
                            break;
                        }
                    }

                    
                    
                    nameIndex ++;
                }
                while (HtmlNameData.entities[(int)nameIndex].hash == hash);

            }
            
            return found;
        }

        
        private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
        {
            

            int current = parseCurrent;
            int end = this.parseEnd;

            while (ParseSupport.InvalidUnicodeCharacter(charClass) && current < end)
            {
                ch = this.parseBuffer[++current];
                charClass = ParseSupport.GetCharClass(ch);
            }

            
            

            if (this.parseThreshold > 1 && current + 1 < end)
            {
                InternalDebug.Assert(!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(this.parseBuffer[current])));

                int dst = current + 1;
                int src = dst;

                while (src < end && dst < current + this.parseThreshold)
                {
                    char chT = this.parseBuffer[src];
                    CharClass classT = ParseSupport.GetCharClass(chT);

                    if (!ParseSupport.InvalidUnicodeCharacter(classT))
                    {
                        if (src != dst)
                        {
                            InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(this.parseBuffer[dst])));

                            this.parseBuffer[dst] = chT;        
                            this.parseBuffer[src] = '\0';       
                        }

                        dst ++;
                    }

                    src ++;
                }

                if (src == end)
                {
                    end = this.parseEnd = this.input.RemoveGap(dst, end);
                }
            }

            

            parseCurrent = current;

            
            return (current + this.parseThreshold <= end);
        }

        
        private char ScanTagName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == this.scanQuote)
                    {
                        this.scanQuote = '\0';
                    }
                    else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                    {
                        this.scanQuote = ch;
                    }
                }
                else if (ch == '<' && this.literalTags)
                {
                    
                    

                    break;
                }

                this.lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanAttrName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == this.scanQuote)
                    {
                        this.scanQuote = '\0';
                    }
                    else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                    {
                        this.scanQuote = ch;
                    }

                    if (ch != '`')
                    {
                        
                        

                        parseBuffer[parseCurrent] = '?';
                    }
                }

                this.lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.WhitespaceCharacter(charClass))
            {
                this.lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.HtmlTextCharacter(charClass))
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanAttrValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.HtmlAttrValueCharacter(charClass))
            {
                this.lastCharClass = charClass;
                
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanSkipTag(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;

            while (!ParseSupport.InvalidUnicodeCharacter(charClass) && (ch != '>' || this.scanQuote != '\0'))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == this.scanQuote)
                    {
                        this.scanQuote = '\0';
                    }
                    else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                    {
                        this.scanQuote = ch;
                    }
                }

                this.lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private bool ScanForInternalInvalidCharacters(int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;
            char ch;

            do
            {
                ch = parseBuffer[parseCurrent++];
            }
            while (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch)));

            parseCurrent--;

            InternalDebug.Assert(parseCurrent <= this.parseEnd);

            return parseCurrent < this.parseEnd;
        }

        
        private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;
            HtmlTokenBuilder tokenBuilder = this.tokenBuilder;

            int firstRunStart = parseCurrent;
            int runStart = firstRunStart;

            
            tokenBuilder.AssertPreparedToAddMoreRuns(3);
            tokenBuilder.AssertCurrentRunPosition(runStart);

            do
            {
                ch = this.ScanText(ch, ref charClass, ref parseCurrent);

                if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    if (parseCurrent != runStart)
                    {
                        

                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        runStart = parseCurrent;
                    }

                    if (ch == ' ')
                    {
                        char chT = parseBuffer[parseCurrent + 1];
                        CharClass charClassT = ParseSupport.GetCharClass(chT);

                        if (!ParseSupport.WhitespaceCharacter(charClassT))
                        {
                            

                            ch = chT;
                            charClass = charClassT;

                            parseCurrent ++;

                            tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                            runStart = parseCurrent;

                            continue;
                        }
                    }

                    

                    this.ParseWhitespace(ch, charClass, ref parseCurrent);

                    if (this.parseThreshold > 1)
                    {
                        
                        break;
                    }

                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                else if (ch == '<')
                {
                    
                    

                    if (this.plaintext || firstRunStart == parseCurrent)
                    {
                        

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        
                        tokenBuilder.AssertPreparedToAddMoreRuns(3);
                        continue;
                    }

                    

                    if (parseCurrent != runStart)
                    {
                        
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    this.parseState = ParseState.TagStart;
                    this.slowParse = this.literalTags;

                    
                    break;
                }
                else if (ch == '&')
                {
                    if (this.literalEntities)
                    {
                        

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        
                        tokenBuilder.AssertPreparedToAddMoreRuns(3);
                        continue;
                    }

                    

                    int literal;
                    int consume;

                    if (this.DecodeEntity(parseCurrent, false, out literal, out consume))
                    {
                        InternalDebug.Assert(consume != 0);

                        if (consume == 1)
                        {
                            
                            

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            
                            tokenBuilder.AssertPreparedToAddMoreRuns(3);
                            continue;
                        }

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        
                        

                        if (literal <= 0xFFFF && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)literal)))
                        {
                            

                            switch ((char)literal)
                            {
                                case ' ':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.Space, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\r':
                                        
                                        
                                        
                                        tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\n':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\t':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.Tabulation, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                default:
                                        tokenBuilder.AddLiteralTextRun(RunTextType.UnusualWhitespace, parseCurrent, parseCurrent + consume, literal);
                                        break;
                            }
                        }
                        else
                        {
                            if (literal == 0xA0)
                            {
                                tokenBuilder.AddLiteralTextRun(RunTextType.Nbsp, parseCurrent, parseCurrent + consume, literal);
                            }
                            else
                            {
                                tokenBuilder.AddLiteralTextRun(RunTextType.NonSpace, parseCurrent, parseCurrent + consume, literal);
                            }
                        }

                        parseCurrent += consume;

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else
                    {
                        
                        

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        
                        this.parseThreshold = HtmlNameData.MAX_ENTITY_NAME + 2;
                        
                        
                        break;
                    }
                }
                else if (ParseSupport.NbspCharacter(charClass))
                {
                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    runStart = parseCurrent;

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.NbspCharacter(charClass));

                    tokenBuilder.AddTextRun(RunTextType.Nbsp, runStart, parseCurrent);
                }
                else
                {
                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));

                    

                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    if (parseCurrent >= parseEnd)
                    {
                        
                        break;
                    }

                    
                    

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && parseCurrent < parseEnd);
                }

                
                runStart = parseCurrent;
            }
            while (tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Text));
        }

        
        private bool ParseAttributeText(char ch, CharClass charClass, ref int parseCurrent)
        {
            int runStart = parseCurrent;
            char[] parseBuffer = this.parseBuffer;
            HtmlTokenBuilder tokenBuilder = this.tokenBuilder;
            int consume;

            while (true)
            {
                tokenBuilder.AssertCurrentRunPosition(runStart);
                tokenBuilder.AssertPreparedToAddMoreRuns(2);

                ch = this.ScanAttrValue(ch, ref charClass, ref parseCurrent);

                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == this.scanQuote)
                    {
                        this.scanQuote = '\0';
                    }
                    else if (this.scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(this.lastCharClass))
                    {
                        this.scanQuote = ch;
                    }

                    this.lastCharClass = charClass;

                    if (ch == this.valueQuote)
                    {
                        

                        break;
                    }

                    
                    parseCurrent ++;
                }
                else if (ch == '&')
                {
                    

                    this.lastCharClass = charClass;

                    
                    

                    int literal;
                    
                    if (this.DecodeEntity(parseCurrent, true, out literal, out consume))
                    {
                        InternalDebug.Assert(consume != 0);

                        if (consume == 1)
                        {
                            
                            

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            continue;
                        }

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
                        }

                        tokenBuilder.AddLiteralRun(RunTextType.Unknown, HtmlRunKind.AttrValue, parseCurrent, parseCurrent + consume, literal);

                        parseCurrent += consume;

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2))
                        {
                            return false;
                        }

                        runStart = parseCurrent;
                    }
                    else
                    {
                        
                        

                        
                        this.parseThreshold = HtmlNameData.MAX_ENTITY_NAME + 2;

                        break;
                    }
                }
                else if (ch == '>')
                {
                    this.lastCharClass = charClass;

                    if (this.valueQuote == '\0')
                    {
                        break;
                    }

                    if (this.scanQuote == '\0')
                    {
                        
                        InternalDebug.Assert(this.valueQuote != 0);

                        this.valueQuote = '\0';
                        break;
                    }

                    parseCurrent ++;
                }
                else if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    this.lastCharClass = charClass;

                    if (this.valueQuote == '\0')
                    {
                        break;
                    }

                    
#if false
                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, runStart, parseCurrent);
                    }

                    
                    this.ParseWhitespace(ch, charClass, ref parseCurrent);

                    runStart = parseCurrent;

                    if (this.parseThreshold > 1)
                    {
                        
                        break;
                    }

                    
                    if (!tokenBuilder.PrepareToAddMoreRuns(2))
                    {
                        return false;
                    }
#else
                    parseCurrent ++;
#endif
                }
                else
                {
                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));
                    break;
                }

                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            if (parseCurrent != runStart)
            {
                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
            }

            return true;
        }

        
        private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
        {
            CharClass charClassT;
            int runStart = parseCurrent;
            char[] parseBuffer = this.parseBuffer;
            HtmlTokenBuilder tokenBuilder = this.tokenBuilder;

            tokenBuilder.AssertPreparedToAddMoreRuns(1);
            tokenBuilder.AssertCurrentRunPosition(runStart);

            InternalDebug.Assert(ParseSupport.WhitespaceCharacter(charClass));

            

            do
            {
                switch (ch)
                {
                    case ' ':
                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == ' ');

                            tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                            break;

                    case '\r':

                            if (parseBuffer[parseCurrent + 1] != '\n')
                            {
                                charClassT = ParseSupport.GetCharClass(parseBuffer[parseCurrent + 1]);

                                if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                                {
                                    if (!this.endOfFile || parseCurrent + 1 < this.parseEnd)
                                    {
                                        
                                        
                                        

                                        this.parseThreshold = 2;
                                        break;
                                    }

                                    
                                }

                                
                            }
                            else
                            {
                                
                                parseCurrent ++;
                            }

                            ch = parseBuffer[++parseCurrent];

                            tokenBuilder.AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\n':

                            

                            ch = parseBuffer[++parseCurrent];

                            tokenBuilder.AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\t':

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\t');

                            tokenBuilder.AddTextRun(RunTextType.Tabulation, runStart, parseCurrent);
                            break;

                    default:

                            InternalDebug.Assert(ch == '\f' || ch == '\v');

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\f' || ch == '\v');

                            tokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, runStart, parseCurrent);
                            break;
                }

                charClass = ParseSupport.GetCharClass(ch);

                runStart = parseCurrent;
            }
            while (ParseSupport.WhitespaceCharacter(charClass) && tokenBuilder.PrepareToAddMoreRuns(1) && this.parseThreshold == 1);
        }

        
        private bool CheckSuffix(int parseCurrent, char ch, out int addToTextCnt, out int tagSuffixCnt, out bool endScan)
        {
            InternalDebug.Assert(this.parseBuffer[parseCurrent] == ch);

            addToTextCnt = 1;
            tagSuffixCnt = 0;
            endScan = false;

            char chT;

            switch (this.parseState)
            {
                case ParseState.Asp:

                        if (ch != '%')
                        {
                            
                            return true;
                        }

                        chT = this.parseBuffer[parseCurrent + 1];

                        if (chT == '>')
                        {
                            

                            addToTextCnt = 0;
                            tagSuffixCnt = 2;
                            endScan = true;

                            return true;
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            
                            return true;
                        }

                        

                        addToTextCnt = 0;
                        tagSuffixCnt = 1;

                        return false;

                case ParseState.Bang:
                case ParseState.Dtd:

                        if (ch == '>' && this.scanQuote == '\0')
                        {
                            addToTextCnt = 0;
                            tagSuffixCnt = 1;
                            endScan = true;
                        }

                        return true;

                case ParseState.Comment:

                        if (ch != '-')
                        {
                            
                            return true;
                        }

                        int nonDash = parseCurrent;

                        do
                        {
                            chT = this.parseBuffer[++nonDash];
                        }
                        while (chT == '-');

                        if (chT == '>' && nonDash - parseCurrent >= 2)
                        {
                            

                            if (this.parseState == ParseState.CommentConditional)
                            {
                                this.parseState = ParseState.Comment;

                                this.tokenBuilder.AbortConditional(true);
                            }

                            addToTextCnt = nonDash - parseCurrent - 2;
                            tagSuffixCnt = 3;
                            endScan = true;

                            return true;
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            

                            addToTextCnt = nonDash - parseCurrent;

                            return true;
                        }

                        

                        addToTextCnt = (nonDash - parseCurrent > 2) ? (nonDash - parseCurrent - 2) : 0;
                        tagSuffixCnt = nonDash - parseCurrent - addToTextCnt;    

                        return false;

                case ParseState.Conditional:
                case ParseState.CommentConditional:

                        if (ch == '>')
                        {
                            
                            

                            this.parseState = (this.parseState == ParseState.CommentConditional) ? ParseState.Comment : ParseState.Bang;

                            this.tokenBuilder.AbortConditional(this.parseState == ParseState.Comment);

                            
                            return this.CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan);
                        }

                        if (ch == '-' && this.parseState == ParseState.CommentConditional)
                        {
                            

                            goto case ParseState.Comment;
                        }

                        if (ch != ']')
                        {
                            return true;
                        }

                        

                        
                        
                        

                        chT = this.parseBuffer[parseCurrent + 1];

                        if (chT == '>')
                        {
                            

                            addToTextCnt = 0;
                            tagSuffixCnt = 2;
                            endScan = true;

                            return true;
                        }

                        int numOkChars = 1;

                        if (chT == '-')
                        {
                            numOkChars ++;

                            chT = this.parseBuffer[parseCurrent + 2];

                            if (chT == '-')
                            {
                                numOkChars ++;

                                chT = this.parseBuffer[parseCurrent + 3];

                                if (chT == '>')
                                {
                                    

                                    addToTextCnt = 0;
                                    tagSuffixCnt = 4;
                                    endScan = true;

                                    return true;
                                }
                            }
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            addToTextCnt = numOkChars;

                            return true;
                        }

                        addToTextCnt = 0;
                        tagSuffixCnt = numOkChars;

                        return false;

                default:
                        InternalDebug.Assert(false);
                        break;
            }

            return true;
        }

        
        private bool DecodeEntity(int parseCurrent, bool inAttribute, out int literal, out int consume)
        {
            

            
            
            
            
            
            
            

            

            
            
            
            
            
            

            
            
            

            char[] parseBuffer = this.parseBuffer;

            InternalDebug.Assert(parseBuffer[parseCurrent] == '&');

            char chT;
            CharClass charClassT;
            int entityStart = parseCurrent + 1;
            int entityCurrent = entityStart;
            int charCount = 0;
            int entityValue = 0;

            chT = parseBuffer[entityCurrent];
            charClassT = ParseSupport.GetCharClass(chT);

            if (chT == '#')
            {
                

                chT = parseBuffer[++entityCurrent];
                charClassT = ParseSupport.GetCharClass(chT);

                if (chT == 'x' || chT == 'X')
                {
                    

                    chT = parseBuffer[++entityCurrent];
                    charClassT = ParseSupport.GetCharClass(chT);

                    while (ParseSupport.HexCharacter(charClassT))
                    {
                        charCount ++;
                        entityValue = (entityValue << 4) + ParseSupport.CharToHex(chT);

                        chT = parseBuffer[++entityCurrent];
                        charClassT = ParseSupport.GetCharClass(chT);
                    }

                    
                    

                    
                    
                    
                    
                    

                    
                    

                    if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || 
                        (this.endOfFile && entityCurrent >= this.parseEnd) ||
                        charCount > 6)            
                    {
                        if ((inAttribute || chT == ';') && entityValue != 0 && charCount <= 6)
                        {
                            ProcessNumericEntityValue(entityValue, out literal);

                            consume = entityCurrent - parseCurrent;

                            if (chT == ';')
                            {
                                consume ++;
                            }

                            return true;
                        }
                        else
                        {
                            
                            
                            

                            literal = 0;
                            consume = 1;

                            return true;
                        }
                    }

                    
                    

                }
                else
                {
                    

                    while (ParseSupport.NumericCharacter(charClassT))
                    {
                        charCount ++;
                        entityValue = (entityValue * 10) + ParseSupport.CharToDecimal(chT);

                        entityCurrent ++;

                        chT = parseBuffer[entityCurrent];
                        charClassT = ParseSupport.GetCharClass(chT);
                    }

                    
                    
                    

                    if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || 
                        (this.endOfFile && entityCurrent >= this.parseEnd) ||
                        charCount > 7)
                    {
                        if (entityValue != 0 && charCount <= 7)
                        {
                            

                            ProcessNumericEntityValue(entityValue, out literal);

                            consume = entityCurrent - parseCurrent;

                            if (chT == ';')
                            {
                                
                                consume ++;
                            }

                            return true;
                        }
                        else
                        {
                            

                            literal = 0;
                            consume = 1;

                            return true;
                        }
                    }

                    
                    
                }
            }
            else
            {
                short[] hashValues = this.hashValuesTable;
                if (hashValues == null)
                {
                    hashValues = this.hashValuesTable = new short[HtmlNameData.MAX_ENTITY_NAME];
                }

                HashCode hashCode = new HashCode(true);
                short hashValue;
                int entityValueT;

                while (ParseSupport.HtmlEntityCharacter(charClassT) && charCount < HtmlNameData.MAX_ENTITY_NAME)
                {
                    hashValue = (short)(((uint)hashCode.AdvanceAndFinalizeHash(chT) ^ HtmlNameData.ENTITY_HASH_MODIFIER) % HtmlNameData.ENTITY_HASH_SIZE);
                    hashValues[charCount++] = hashValue;

                    entityCurrent ++;

                    chT = parseBuffer[entityCurrent];
                    charClassT = ParseSupport.GetCharClass(chT);
                }

                if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || (this.endOfFile && entityCurrent >= this.parseEnd))
                {
                    

                    
                    
                    
                    
                    

                    if (charCount > 1)      
                    {
                        if (FindEntityByHashName(hashValues[charCount - 1], parseBuffer, entityStart, charCount, out entityValueT) && 
                            (chT == ';' || entityValueT <= 255))
                        {
                            

                            entityValue = entityValueT;
                        }
                        else if (!inAttribute)
                        {
                            for (int i = charCount - 2; i >= 0; i--)
                            {
                                if (FindEntityByHashName(hashValues[i], parseBuffer, entityStart, i + 1, out entityValueT) && 
                                    entityValueT <= 255)
                                {
                                    

                                    entityValue = entityValueT;
                                    charCount = i + 1;
                                    break;
                                }
                            }
                        }

                        if (entityValue != 0)
                        {
                            
                            InternalDebug.Assert(entityValue <= 0xFFFF);

                            literal = entityValue;
                            consume = charCount + 1;

                            if (parseBuffer[entityStart + charCount] == ';')
                            {
                                consume ++;
                            }

                            return true;
                        }
                    }

                    

                    literal = 0;
                    consume = 1;

                    return true;
                }
            }

            literal = 0;
            consume = 0;

            return false;
        }

        
        private void HandleSpecialTag()
        {
            if (HtmlNameData.names[(int)this.token.NameIndex].literalTag)
            {
                this.literalTags = !this.token.IsEndTag;
                this.literalTagNameId = this.literalTags ? this.token.NameIndex : HtmlNameIndex.Unknown;

                if (HtmlNameData.names[(int)this.token.NameIndex].literalEnt)
                {
                    
                    
                    
                    
                    
                    
                    
                    
                    

                    this.literalEntities = !(this.token.IsEndTag || this.token.IsEmptyScope);
                }

                this.slowParse = this.slowParse || this.literalTags;
            }

            switch (this.token.NameIndex)
            {
                case HtmlNameIndex.Meta:

                        if (this.input is ConverterDecodingInput && this.detectEncodingFromMetaTag && ((IRestartable)this).CanRestart())
                        {
                            
                            

                            
                            
                            
                            
                            
                            

                            if (this.token.IsTagBegin)
                            {
                                this.rightMeta = false;
                                this.newEncoding = null;
                            }

                            this.token.Attributes.Rewind();

                            int attrToParse = -1;
                            bool isHttpEquivContent = false;

                            foreach (HtmlAttribute attr in this.token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.HttpEquiv)
                                {
                                    if (attr.Value.CaseInsensitiveCompareEqual("content-type") ||
                                        attr.Value.CaseInsensitiveCompareEqual("charset"))
                                    {
                                        this.rightMeta = true;
                                        if (attrToParse != -1)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        
                                        break;
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Content)
                                {
                                    attrToParse = attr.Index;
                                    isHttpEquivContent = true;
                                    if (this.rightMeta)
                                    {
                                        break;
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Charset)
                                {
                                    attrToParse = attr.Index;
                                    isHttpEquivContent = false;
                                    this.rightMeta = true;
                                    break;
                                }
                            }

                            if (attrToParse != -1)
                            {
                                string strToParse = this.token.Attributes[attrToParse].Value.GetString(100);
                                string charset = CharsetFromString(strToParse, isHttpEquivContent);

                                if (charset != null)
                                {
                                    Charset.TryGetEncoding(charset, out this.newEncoding);

                                    
                                    
                                    
                                }
                            }

                            if (this.rightMeta && this.newEncoding != null)
                            {
                                (this.input as ConverterDecodingInput).RestartWithNewEncoding(this.newEncoding);
                            }

                            this.token.Attributes.Rewind();
                        }
                        break;

                    case HtmlNameIndex.PlainText:

                        if (!this.token.IsEndTag)
                        {
                            
                            
                            

                            this.plaintext = true;
                            this.literalEntities = true;

                            if (this.token.IsTagEnd)
                            {
                                
                                

                                this.parseState = ParseState.Text;
                            }
                        }

                        break;
            }
        }

        
        private static string CharsetFromString(string arg, bool lookForWordCharset)
        {
            int i = 0;

            while (i < arg.Length)
            {
                
                while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                {
                    i ++;
                }

                if (i == arg.Length)
                {
                    break;
                }

                if (!lookForWordCharset || 
                    (arg.Length - i >= 7 && string.Equals(arg.Substring(i, 7), "charset", StringComparison.OrdinalIgnoreCase)))
                {
                    if (lookForWordCharset)
                    {
                        i = arg.IndexOf('=', i + 7);

                        if (i < 0)
                        {
                            break;
                        }

                        i ++;

                        
                        while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                        {
                            i ++;
                        }

                        if (i == arg.Length)
                        {
                            break;
                        }
                    }

                    int j = i;

                    while (j < arg.Length && arg[j] != ';' && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[j])))
                    {
                        j ++;
                    }

                    return arg.Substring(i, j - i);
                }

                i = arg.IndexOf(';', i);

                if (i < 0)
                {
                    break;
                }

                i ++;
            }

            return null;
        }
    }
}

