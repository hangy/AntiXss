// ***************************************************************
// <copyright file="TextParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ... 
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;


    internal class TextParser : IDisposable
    {

        protected ConverterInput input;
        protected bool endOfFile;

        protected char[] parseBuffer;
        protected int parseStart;
        protected int parseCurrent;
        protected int parseEnd;
        protected int parseThreshold = 1;

        // current token
        protected TextTokenBuilder tokenBuilder;
        protected TextToken token;

        protected bool unwrapFlowed;
        protected bool unwrapDelSpace;

        // line state
        protected bool lastSpace;
        protected int lineCount;

        // 2646 decoding state
        protected bool quotingExpected = true;
        protected int quotingLevel;
        protected int lastLineQuotingLevel;
        protected bool lastLineFlowed;
        protected bool signaturePossible = true;


        public TextParser(
                ConverterInput input,
                bool unwrapFlowed,
                bool unwrapDelSp,
                int maxRuns,
                bool testBoundaryConditions)
        {
            this.input = input;

            this.tokenBuilder = new TextTokenBuilder(null, maxRuns, testBoundaryConditions);

            this.token = this.tokenBuilder.Token;

            this.unwrapFlowed = unwrapFlowed;
            this.unwrapDelSpace = unwrapDelSp;
        }


        public TextToken Token
        {
            get 
            {
                InternalDebug.Assert(this.tokenBuilder.Valid);
                return this.token;
            }
        }


        public void Initialize(string fragment)
        {
            InternalDebug.Assert(this.input is ConverterBufferInput);

            (this.input as ConverterBufferInput).Initialize(fragment);

            this.endOfFile = false;

            this.parseBuffer = null;
            this.parseStart = 0;
            this.parseCurrent = 0;
            this.parseEnd = 0;
            this.parseThreshold = 1;

            this.tokenBuilder.Reset();

            this.lastSpace = false;
            this.lineCount = 0;

            this.quotingExpected = true;
            this.quotingLevel = 0;
            this.lastLineQuotingLevel = 0;
            this.lastLineFlowed = false;
            this.signaturePossible = true;
        }


        public TextTokenId Parse()
        {
            char ch, chT;
            CharClass charClass, charClassT;
            bool forceFlushToken;
            int runStart;

            if (this.tokenBuilder.Valid)
            {
                // start the new token

                this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                this.parseStart = this.parseCurrent;

                this.tokenBuilder.Reset();

                InternalDebug.Assert(this.tokenBuilder.TotalLength == 0);
            }

            while (true)
            {
                InternalDebug.Assert(this.parseThreshold > 0);

                // try to read and decode more input data if necessary

                forceFlushToken = false;

                if (this.parseCurrent + this.parseThreshold > this.parseEnd)
                {
                    if (!this.endOfFile)
                    {
                        if (!this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd))
                        {
                            // cannot decode more data until next input chunk is available

                            // we may have incomplete token at this point
                            InternalDebug.Assert(!this.tokenBuilder.Valid);

                            return TextTokenId.None;
                        }

                        // NOTE: in case of success, ReadMore can move the token in the buffer and / or
                        // switch to a new buffer

                        this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);

                        ConverterDecodingInput decodingInput = this.input as ConverterDecodingInput;

                        if (decodingInput != null && decodingInput.EncodingChanged)
                        {


                            // reset the flag as required by ConverterInput protocol
                            decodingInput.EncodingChanged = false;

                            // signal encoding change to the caller
                            return this.tokenBuilder.MakeEmptyToken(TextTokenId.EncodingChange, decodingInput.Encoding.CodePage);
                        }

                        if (this.input.EndOfFile)
                        {
                            this.endOfFile = true;
                        }

                        if (!this.endOfFile && this.parseEnd - this.parseStart < this.input.MaxTokenSize)
                        {
                            // we have successfuly read "something", ensure this something is above threshold
                            continue;
                        }
                    }

                    // end of file or token is too long, need to flush the token as is (split)
                    forceFlushToken = true;
                }

                // we should have read something unless this is EOF
                InternalDebug.Assert(this.parseEnd > this.parseCurrent || forceFlushToken);

                // compact, so that the next character (or parseThreshold next characters) are valid.

                // get the next input character

                ch = this.parseBuffer[this.parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.InvalidUnicodeCharacter(charClass) || this.parseThreshold > 1)
                {



                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && this.parseCurrent < this.parseEnd)
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }


                    if (this.parseThreshold > 1 && this.parseCurrent + 1 < this.parseEnd)
                    {
                        InternalDebug.Assert(this.parseCurrent == this.parseStart);
                        InternalDebug.Assert(!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(this.parseBuffer[this.parseCurrent])));

                        int src = this.parseCurrent + 1;
                        int dst = this.parseCurrent + 1;

                        while (src < this.parseEnd && dst < this.parseCurrent + this.parseThreshold)
                        {
                            chT = this.parseBuffer[src];
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (!ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                if (src != dst)
                                {
                                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(this.parseBuffer[dst])));

                                    this.parseBuffer[dst] = chT;        // move source character
                                    this.parseBuffer[src] = '\0';       // replace source character with invalid (zero)
                                }

                                dst ++;
                            }
                            
                            src ++;
                        }

                        if (src == this.parseEnd && this.parseCurrent + this.parseThreshold > dst)
                        {
                             Array.Copy(this.parseBuffer, this.parseCurrent, this.parseBuffer, this.parseEnd - (dst - this.parseCurrent), dst - this.parseCurrent);

                            this.parseCurrent = this.parseEnd - (dst - this.parseCurrent);

                            // reporting all invalid characters consumed
                            this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                            this.parseStart = this.parseCurrent;
                        }
                    }


                    if (this.parseCurrent + this.parseThreshold > this.parseEnd)
                    {
                        // we still below threshold...

                        if (!forceFlushToken)
                        {
                            // go back and try to read more
                            continue;
                        }

                        // this is the end of file

                        if (this.parseCurrent == this.parseEnd && !this.tokenBuilder.IsStarted && this.endOfFile)
                        {
                            // EOF and token is empty, just return EOF token
                            break;
                        }

                        // this is the end of file, we cannot make it above threshold but still have some input data. 
                    }

                    // reset the threshold to its default value
                    this.parseThreshold = 1;
                }

                // now parse the buffer content

                runStart = this.parseCurrent;

                InternalDebug.Assert(!this.tokenBuilder.IsStarted);

                this.tokenBuilder.StartText(runStart);

                while (this.tokenBuilder.PrepareToAddMoreRuns(9, runStart, RunKind.Text))
                {
                    while (ParseSupport.TextUriCharacter(charClass))
                    {
                        ch = this.parseBuffer[++this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }

                    if (ParseSupport.TextNonUriCharacter(charClass))
                    {
                        if (this.parseCurrent != runStart)
                        {
                            // we have nonempty NWSP run

                            this.AddTextRun(RunTextType.NonSpace, runStart, this.parseCurrent);
                        }

                        runStart = this.parseCurrent;

                        do
                        {
                            ch = this.parseBuffer[++this.parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.NbspCharacter(charClass));

                        this.AddTextRun(RunTextType.NonSpace, runStart, this.parseCurrent);
                    }
                    else if (ParseSupport.WhitespaceCharacter(charClass))
                    {
                        if (this.parseCurrent != runStart)
                        {
                            // we have nonempty NWSP run

                            this.AddTextRun(RunTextType.NonSpace, runStart, this.parseCurrent);
                        }

                        runStart = this.parseCurrent;

                        if (ch == ' ')
                        {
                            // ordinary space

                            chT = this.parseBuffer[this.parseCurrent + 1];
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (!ParseSupport.WhitespaceCharacter(charClassT))
                            {
                                // add single space to text run

                                ch = chT;
                                charClass = charClassT;

                                this.parseCurrent ++;

                                this.AddTextRun(RunTextType.Space, runStart, this.parseCurrent);

                                runStart = this.parseCurrent;

                                continue;
                            }
                        }

                        // this is a potentially collapsable whitespace, accumulate whitespace run(s)  

                        this.ParseWhitespace(ch, charClass);

                        if (this.parseThreshold > 1)
                        {
                            // terminate the text parse loop to read more data
                            break;
                        }

                        runStart = this.parseCurrent;

                        ch = this.parseBuffer[this.parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else if (ParseSupport.NbspCharacter(charClass))
                    {
                        if (this.parseCurrent != runStart)
                        {
                            this.AddTextRun(RunTextType.NonSpace, runStart, this.parseCurrent);
                        }

                        runStart = this.parseCurrent;

                        do
                        {
                            ch = this.parseBuffer[++this.parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.NbspCharacter(charClass));

                        this.AddTextRun(RunTextType.Nbsp, runStart, this.parseCurrent);
                    }
                    else
                    {
                        InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));

                        // finish the "non-whitespace" run

                        if (this.parseCurrent != runStart)
                        {
                            this.AddTextRun(RunTextType.NonSpace, runStart, this.parseCurrent);
                        }

                        if (this.parseCurrent >= this.parseEnd)
                        {
                            // end of available input (EOB), flush the current text token
                            break;
                        }

                        // this is just an embedded invalid character, skip any such invalid 
                        // characters and try to continue collecting text

                        do
                        {
                            ch = this.parseBuffer[++this.parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.InvalidUnicodeCharacter(charClass) && this.parseCurrent < this.parseEnd);
                    }

                    // prepare for a new run

                    runStart = this.parseCurrent;
                }

                if (this.token.IsEmpty)
                {
                    // text token is empty, we need more data

                    this.tokenBuilder.Reset();     // reset open text...

                    // reporting everything below parseCurrent consumed
                    this.input.ReportProcessed(this.parseCurrent - this.parseStart);
                    this.parseStart = this.parseCurrent;
                    continue;
                }

                // finish the current text token, return anything we have collected so far

                this.tokenBuilder.EndText();

                return (TextTokenId)this.token.TokenId;
            }

            return this.tokenBuilder.MakeEmptyToken(TextTokenId.EndOfFile);
        }



        void IDisposable.Dispose()
        {
            if (this.input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)this.input).Dispose();
            }

            this.input = null;
            this.parseBuffer = null;
            this.token = null;
            this.tokenBuilder = null;

            GC.SuppressFinalize(this);
        }


        private void ParseWhitespace(char ch, CharClass charClass)
        {
            CharClass charClassT;
            int runStart = this.parseCurrent;

            // parse whitespace

            do
            {
                switch (ch)
                {
                    case ' ':
                            do
                            {
                                ch = this.parseBuffer[++this.parseCurrent];
                            }
                            while (ch == ' ');

                            this.AddTextRun(RunTextType.Space, runStart, this.parseCurrent);
                            break;

                    case '\r':

                            if (this.parseBuffer[this.parseCurrent + 1] != '\n')
                            {
                                charClassT = ParseSupport.GetCharClass(this.parseBuffer[this.parseCurrent + 1]);

                                if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                                {
                                    if (!this.endOfFile || this.parseCurrent + 1 < this.parseEnd)
                                    {

                                        this.parseThreshold = 2;
                                        break;
                                    }

                                    // EOF - there is no LF
                                }

                                // valid character that is not LF or EOF - no need to skip LF
                            }
                            else
                            {
                                // skip LF
                                this.parseCurrent ++;
                            }

                            ch = this.parseBuffer[++this.parseCurrent];

                            this.AddTextRun(RunTextType.NewLine, runStart, this.parseCurrent);
                            break;

                    case '\n':

                            // this is a standalone LF, count as CRLF

                            ch = this.parseBuffer[++this.parseCurrent];

                            this.AddTextRun(RunTextType.NewLine, runStart, this.parseCurrent);
                            break;

                    case '\t':

                            do
                            {
                                ch = this.parseBuffer[++this.parseCurrent];
                            }
                            while (ch == '\t');

                            this.AddTextRun(RunTextType.Tabulation, runStart, this.parseCurrent);
                            break;

                    default:

                            InternalDebug.Assert(ch == '\v' || ch == '\f');

                            do
                            {
                                ch = this.parseBuffer[++this.parseCurrent];
                            }
                            while (ch == '\v' || ch == '\f');

                            this.AddTextRun(RunTextType.UnusualWhitespace, runStart, this.parseCurrent);
                            break;
                }

                charClass = ParseSupport.GetCharClass(ch);

                runStart = this.parseCurrent;
            }
            while (ParseSupport.WhitespaceCharacter(charClass) && this.tokenBuilder.PrepareToAddMoreRuns(4, runStart, RunKind.Text) && this.parseThreshold == 1);
        }

        private void AddTextRun(RunTextType textType, int runStart, int runEnd)
        {
            if (!this.unwrapFlowed)
            {
                // just add the run
                this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                return;
            }

            // do unwrapping preprocessing
            this.AddTextRunUnwrap(textType, runStart, runEnd);
        }


        private void AddTextRunUnwrap(RunTextType textType, int runStart, int runEnd)
        {
            switch (textType)
            {
                case RunTextType.NewLine:

                        if (!this.lastSpace || (this.signaturePossible && this.lineCount == 3))
                        {
                            this.lastLineFlowed = false;
                            this.tokenBuilder.AddTextRun(textType, runStart, runEnd);
                        }
                        else
                        {
                            this.lastLineFlowed = true;
                        }

                        this.lineCount = 0;
                        this.lastSpace = false;
                        this.signaturePossible = true;
                        this.quotingExpected = true;

                        this.lastLineQuotingLevel = this.quotingLevel;
                        this.quotingLevel = 0;
                        break;

                case RunTextType.Space:
                case RunTextType.UnusualWhitespace:

                        if (this.quotingExpected)
                        {
                            InternalDebug.Assert(this.lineCount == 0);

                            runStart ++;    // skip first space at the beginning of line

                            // need to add invalid run to keep it contigous
                            this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);

                            if (this.lastLineQuotingLevel != this.quotingLevel)
                            {
                                if (this.lastLineFlowed)
                                {
                                    this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                }

                                this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                                this.lastLineQuotingLevel = this.quotingLevel;
                            }

                            this.quotingExpected = false;
                        }

                        InternalDebug.Assert(this.lineCount != 0 || this.lastLineQuotingLevel == this.quotingLevel);

                        if (runStart != runEnd)
                        {
                            this.lineCount += runEnd - runStart;
                            this.lastSpace = true;

                            this.tokenBuilder.AddTextRun(textType, runStart, runEnd);

                            if (this.lineCount != 3 || runEnd - runStart != 1)
                            {
                                this.signaturePossible = false;
                            }
                        }

                        break;

                case RunTextType.Tabulation:

                        if (this.quotingExpected)
                        {
                            InternalDebug.Assert(this.lineCount == 0);

                            if (this.lastLineQuotingLevel != this.quotingLevel)
                            {
                                if (this.lastLineFlowed)
                                {
                                    this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                }

                                this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                                this.lastLineQuotingLevel = this.quotingLevel;
                            }

                            this.quotingExpected = false;
                        }

                        InternalDebug.Assert(this.lineCount != 0 || this.lastLineQuotingLevel == this.quotingLevel);

                        this.tokenBuilder.AddTextRun(textType, runStart, runEnd);

                        this.lineCount += runEnd - runStart;

                        this.lastSpace = false;
                        this.signaturePossible = false;

                        break;

                case RunTextType.Nbsp:
                case RunTextType.NonSpace:

                        if (this.quotingExpected)
                        {
                            InternalDebug.Assert(this.lineCount == 0);

                            while (runStart != runEnd && this.parseBuffer[runStart] == '>')
                            {
                                this.quotingLevel ++;
                                runStart ++;
                            }

                            // we may have skipped some stuff, in such case need to add invalid run to keep it contigous
                            this.tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);

                            if (runStart != runEnd)
                            {
                                // end of quotation, check the quoting level

                                if (this.lastLineQuotingLevel != this.quotingLevel)
                                {
                                    if (this.lastLineFlowed)
                                    {
                                        this.tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                    }

                                    this.tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, this.quotingLevel);
                                    this.lastLineQuotingLevel = this.quotingLevel;
                                }

                                this.quotingExpected = false;
                            }
                        }

                        if (runStart != runEnd)
                        {
                            InternalDebug.Assert(this.lineCount != 0 || this.lastLineQuotingLevel == this.quotingLevel);

                            this.tokenBuilder.AddTextRun(textType, runStart, runEnd);

                            this.lineCount += runEnd - runStart;
                            this.lastSpace = false;

                            if (this.lineCount > 2 || this.parseBuffer[runStart] != '-' || (runEnd - runStart == 2 && this.parseBuffer[runStart + 1] != '-'))
                            {
                                this.signaturePossible = false;
                            }
                        }

                        break;

                default:

                        InternalDebug.Assert(false, "unexpected run textType");
                        break;
            }
        }
    }
}

