// ***************************************************************
// <copyright file="Token.cs" company="Microsoft">
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
    using System.Runtime.InteropServices;
    using Microsoft.Exchange.Data.Internal;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    

    internal enum TokenId : byte
    {
        None = 0,                
                                 
        EndOfFile,               
        Text,
        EncodingChange ,
    }

    

    internal enum RunType : uint
    {
        Invalid = 0,             
                                 

        Special = (1u << 30),    
                                 
                                 
                                 

        Normal = (2u << 30),     
                                 
                                 

        Literal = (3u << 30),    
                                 

        Mask = 0xC0000000u,      
    }

    

    internal enum RunTextType : uint
    {
        Unknown = 0,

        Space = (1 << 27),
        NewLine = (2 << 27),
        Tabulation = (3 << 27),
        UnusualWhitespace = (4 << 27),
        LastWhitespace = UnusualWhitespace,

        Nbsp = (5 << 27),
        NonSpace = (6 << 27),
        LastText = NonSpace,

        Last = LastText,

        Mask = 0x38000000u,
    }

    

    internal enum RunKind : uint
    {
        Invalid = 0,
        Text = (1u << 26),

        StartLexicalUnitFlag = (1u << 31),

        MajorKindMask = (0x1Fu << 26),
        MajorKindMaskWithStartLexicalUnitFlag = (0x3Fu << 26),
        MinorKindMask = (3u << 24),
        KindMask = (0xFFu << 24),
    }

    

    internal struct TokenRun
    {
        private Token token;
#if DEBUG
        private int index;
#endif

        internal TokenRun(Token token)
        {
            this.token = token;
#if DEBUG
            this.index = this.token.wholePosition.run;
#endif
        }

        
#if false
        public int Index
        {
            get { this.AssertCurrent(); return this.token.wholePosition.run; }
        }

        public bool IsInvalid
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type == RunType.Invalid; }
        }

#endif
        public RunType Type
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type; }
        }

        public bool IsTextRun
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type >= RunType.Normal; }
        }

        public bool IsSpecial
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type == RunType.Special; }
        }

        public bool IsNormal
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type == RunType.Normal; }
        }

        public bool IsLiteral
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type == RunType.Literal; }
        }

        public RunTextType TextType
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].TextType; }
        }

        public char[] RawBuffer
        {
            get { this.AssertCurrent(); return this.token.buffer; }
        }

        public int RawOffset
        {
            get { this.AssertCurrent(); return this.token.wholePosition.runOffset; }
        }

        public int RawLength
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Length; }
        }

        public uint Kind
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Kind; }
        }

        public int Literal
        {
            get { this.AssertCurrent(); InternalDebug.Assert(this.IsLiteral); return this.token.runList[this.token.wholePosition.run].Value; }
        }

        public int Length
        {
            get
            {
                this.AssertCurrent();
                return this.IsNormal ? this.RawLength : this.IsLiteral ? Token.LiteralLength(this.Literal) : 0;
            }
        }

        public int Value
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Value; }
        }

        public char FirstChar
        {
            get { this.AssertCurrent(); return this.IsLiteral ? Token.LiteralFirstChar(this.Literal) : this.RawBuffer[this.RawOffset]; }
        }

        public char LastChar
        {
            get { this.AssertCurrent(); return this.IsLiteral ? Token.LiteralLastChar(this.Literal) : this.RawBuffer[this.RawOffset + this.RawLength - 1]; }
        }

        public bool IsAnyWhitespace
        {
            get
            {
                return this.TextType <= RunTextType.LastWhitespace;
            }
        }

        

        public int ReadLiteral(char[] buffer)
        {
            this.AssertCurrent();
            InternalDebug.Assert(this.IsLiteral);
            InternalDebug.Assert(buffer.Length >= 2);

            int literalValue = this.token.runList[this.token.wholePosition.run].Value;
            int literalLength = Token.LiteralLength(literalValue);

            if (literalLength == 1)
            {
                buffer[0] = (char) literalValue;
                return 1;
            }

            InternalDebug.Assert(literalLength == 2);

            buffer[0] = Token.LiteralFirstChar(literalValue);
            buffer[1] = Token.LiteralLastChar(literalValue);
            return 2;
        }

        

        public string GetString(int maxSize)
        {
            Token.Fragment fragment;
            int run = this.token.wholePosition.run;
            Token.RunEntry[] runList = this.token.runList;

            switch (runList[run].Type)
            {
                case RunType.Normal:

                        return new string(this.token.buffer, this.token.wholePosition.runOffset, Math.Min(maxSize, runList[run].Length));

                case RunType.Literal:

                        if (this.Length == 1)
                        {
                            return this.FirstChar.ToString();
                        }
                        
                        fragment = new Token.Fragment();

                        fragment.Initialize(run, this.token.wholePosition.runOffset);
                        fragment.tail = fragment.head + 1;

                        return this.token.GetString(ref fragment, maxSize);
            }

            return "";
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
#if DEBUG
            InternalDebug.Assert(this.index == this.token.wholePosition.run);
#endif
        }
    }

    internal enum CollapseWhitespaceState
    {
        NonSpace,
        Whitespace,
        NewLine
    }

    

    internal class Token
    {
        protected internal TokenId tokenId;
        protected internal int argument;                         

        protected internal char[] buffer;                        
        protected internal RunEntry[] runList;                   

        protected internal Fragment whole;                       
        protected internal FragmentPosition wholePosition;       

        
        private LowerCaseCompareSink compareSink;
        private LowerCaseSubstringSearchSink searchSink;
        private StringBuildSink stringBuildSink;

        

        public Token()
        {
            this.Reset();
        }

        

        public TokenId TokenId
        {
            get { return this.tokenId; }
            set { this.tokenId = value; }
        }

        public int Argument
        {
            get { return this.argument; }
        }

        public bool IsEmpty
        {
            get { return this.whole.tail == this.whole.head; }
        }

        public RunEnumerator Runs
        {
            get { return new RunEnumerator(this); }
        }

        public TextReader Text
        {
            get { return new TextReader(this); }
        }

        

        public bool IsWhitespaceOnly
        {
            get
            {
                return this.IsWhitespaceOnlyImp(ref this.whole);
            }
        }

        protected internal bool IsWhitespaceOnlyImp(ref Fragment fragment)
        {
            bool result = true;
            int run = fragment.head;

            while (run != fragment.tail)
            {
                if (this.runList[run].Type >= RunType.Normal)
                {
                    if (this.runList[run].TextType > RunTextType.LastWhitespace)
                    {
                        result = false;
                        break;
                    }
                }

                run ++;
            }

            return result;
        }

        internal static int LiteralLength(int literal)
        {
            return (literal > 0xFFFF) ? 2 : 1;
        }

        internal static char LiteralFirstChar(int literal)
        {
            return (literal > 0xFFFF) ? ParseSupport.HighSurrogateCharFromUcs4(literal) : (char)literal;
        }

        internal static char LiteralLastChar(int literal)
        {
            return (literal > 0xFFFF) ? ParseSupport.LowSurrogateCharFromUcs4(literal) : (char)literal;
        }

        
        
        

        protected internal int Read(ref Fragment fragment, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            int startOffset = offset;
            int run = position.run;

            if (run == fragment.head - 1)
            {
                run = position.run = fragment.head;
            }

            if (run != fragment.tail)
            {
                int runOffset = position.runOffset;
                int runDeltaOffset = position.runDeltaOffset;

                do
                {
                    RunEntry runEntry = this.runList[run];

                    InternalDebug.Assert(count != 0);

                    if (runEntry.Type == RunType.Literal)
                    {
                        int literalLength = Token.LiteralLength(runEntry.Value);

                        if (runDeltaOffset != literalLength)
                        {
                            if (literalLength == 1)
                            {
                                InternalDebug.Assert(runDeltaOffset == 0);

                                buffer[offset++] = (char) runEntry.Value;
                                count --;
                            }
                            else
                            {
                                InternalDebug.Assert(literalLength == 2);

                                if (runDeltaOffset != 0)
                                {
                                    InternalDebug.Assert(runDeltaOffset == 1);

                                    buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                                    count --;
                                }
                                else
                                {
                                    buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
                                    count --;

                                    if (count == 0)
                                    {
                                        runDeltaOffset = 1;
                                        break;
                                    }

                                    buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                                    count --;
                                }
                            }
                        }
                    }
                    else if (runEntry.Type == RunType.Normal)
                    {
                        InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                        int copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                        InternalDebug.Assert(copyCount != 0);
                        
                        {
                            Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                            offset += copyCount;
                            count -= copyCount;

                            if (runDeltaOffset + copyCount != runEntry.Length)
                            {
                                runDeltaOffset += copyCount;
                                break;
                            }
                        }
                    }

                    runOffset += runEntry.Length;
                    runDeltaOffset = 0;
                }
                while (++run != fragment.tail && count != 0);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = runDeltaOffset;
            }

            return offset - startOffset;
        }

        
        
        

        protected internal int ReadOriginal(ref Fragment fragment, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            int startOffset = offset;
            int run = position.run;

            if (run == fragment.head - 1)
            {
                run = position.run = fragment.head;
            }

            if (run != fragment.tail)
            {
                int runOffset = position.runOffset;
                int runDeltaOffset = position.runDeltaOffset;

                do
                {
                    RunEntry runEntry = this.runList[run];

                    InternalDebug.Assert(count != 0);

                    if (runEntry.Type == RunType.Literal ||runEntry.Type == RunType.Normal)
                    {
                        InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                        int copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                        InternalDebug.Assert(copyCount != 0);
                        
                        {
                            Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                            offset += copyCount;
                            count -= copyCount;

                            if (runDeltaOffset + copyCount != runEntry.Length)
                            {
                                runDeltaOffset += copyCount;
                                break;
                            }
                        }
                    }

                    runOffset += runEntry.Length;
                    runDeltaOffset = 0;
                }
                while (++run != fragment.tail && count != 0);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = runDeltaOffset;
            }

            return offset - startOffset;
        }

        protected internal int Read(LexicalUnit unit, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            int startOffset = offset;

            if (unit.head != -1)
            {
                uint kind = this.runList[unit.head].MajorKind;  

                int run = position.run;

                if (run == unit.head - 1)
                {
                    run = position.run = unit.head;
                }

                RunEntry runEntry = this.runList[run];

                if (run == unit.head || runEntry.MajorKindPlusStartFlag == kind)
                {
                    int runOffset = position.runOffset;
                    int runDeltaOffset = position.runDeltaOffset;

                    do
                    {
                        InternalDebug.Assert(count != 0);

                        if (runEntry.Type == RunType.Literal)
                        {
                            int literalLength = Token.LiteralLength(runEntry.Value);

                            if (runDeltaOffset != literalLength)
                            {
                                if (literalLength == 1)
                                {
                                    InternalDebug.Assert(runDeltaOffset == 0);

                                    buffer[offset++] = (char) runEntry.Value;
                                    count --;
                                }
                                else
                                {
                                    InternalDebug.Assert(literalLength == 2);

                                    if (runDeltaOffset != 0)
                                    {
                                        InternalDebug.Assert(runDeltaOffset == 1);

                                        buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                                        count --;
                                    }
                                    else
                                    {
                                        buffer[offset++] = Token.LiteralFirstChar(runEntry.Value);
                                        count --;

                                        if (count == 0)
                                        {
                                            runDeltaOffset = 1;
                                            break;
                                        }

                                        buffer[offset++] = Token.LiteralLastChar(runEntry.Value);
                                        count --;
                                    }
                                }
                            }
                        }
                        else if (runEntry.Type == RunType.Normal)
                        {
                            InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                            int copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                            InternalDebug.Assert(copyCount != 0);
                            
                            {
                                Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                                offset += copyCount;
                                count -= copyCount;

                                if (runDeltaOffset + copyCount != runEntry.Length)
                                {
                                    runDeltaOffset += copyCount;
                                    break;
                                }
                            }
                        }

                        runOffset += runEntry.Length;
                        runDeltaOffset = 0;

                        runEntry = this.runList[++run];

                        
                        
                        
                    }
                    while (runEntry.MajorKindPlusStartFlag == kind && count != 0);

                    position.run = run;
                    position.runOffset = runOffset;
                    position.runDeltaOffset = runDeltaOffset;
                }
            }

            return offset - startOffset;
        }

        

        protected internal virtual void Rewind()
        {
            this.wholePosition.Rewind(this.whole);
        }

        
        

        protected internal int GetLength(ref Fragment fragment)
        {
            int run = fragment.head;
            int length = 0;

            if (run != fragment.tail)
            {
                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        length += runEntry.Length;
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        length += Token.LiteralLength(runEntry.Value);
                    }
                }
                while (++run != fragment.tail);
            }

            return length;
        }

        
        

        protected internal int GetOriginalLength(ref Fragment fragment)
        {
            int run = fragment.head;
            int length = 0;

            if (run != fragment.tail)
            {
                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        length += runEntry.Length;
                    }
                }
                while (++run != fragment.tail);
            }

            return length;
        }

        protected internal int GetLength(LexicalUnit unit)
        {
            int run = unit.head;
            int length = 0;

            if (run != -1)  
            {
                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        length += runEntry.Length;
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        length += Token.LiteralLength(runEntry.Value);
                    }

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);
            }

            return length;
        }

        

        protected internal bool IsFragmentEmpty(ref Fragment fragment)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        InternalDebug.Assert(0 != runEntry.Length);
                        return false;
                    }
                }
                while (++run != fragment.tail);
            }

            return true;
        }

        protected internal bool IsFragmentEmpty(LexicalUnit unit)
        {
            int run = unit.head;

            if (run != -1)  
            {
                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        InternalDebug.Assert(0 != runEntry.Length);
                        return false;
                    }

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);
            }

            return true;
        }

        

        protected internal bool IsContiguous(ref Fragment fragment)
        {
            
            return fragment.head + 1 == fragment.tail && this.runList[fragment.head].Type == RunType.Normal;
        }

        protected internal bool IsContiguous(LexicalUnit unit)
        {
            
            InternalDebug.Assert(unit.head >= 0 && unit.head + 1 <= this.whole.tail && unit.head + 1 < this.runList.Length);

            return this.runList[unit.head].Type == RunType.Normal &&
                    this.runList[unit.head].MajorKind != this.runList[unit.head + 1].MajorKindPlusStartFlag;
        }

        protected internal int CalculateHashLowerCase(Fragment fragment)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;

                if (run + 1 == fragment.tail && this.runList[run].Type == RunType.Normal)
                {
                    
                    return HashCode.CalculateLowerCase(this.buffer, runOffset, this.runList[run].Length);
                }

                HashCode hashCode = new HashCode(true);

                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.AdvanceLowerCase(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.AdvanceLowerCase(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHashLowerCase(LexicalUnit unit)
        {
            int run = unit.head;

            if (run != -1)
            {
                int runOffset = unit.headOffset;
                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  

                if (runEntry.Type == RunType.Normal &&
                    kind != this.runList[run + 1].MajorKindPlusStartFlag)
                {
                    
                    return HashCode.CalculateLowerCase(this.buffer, runOffset, runEntry.Length);
                }

                HashCode hashCode = new HashCode(true);

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.AdvanceLowerCase(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.AdvanceLowerCase(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHash(Fragment fragment)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;

                if (run + 1 == fragment.tail && this.runList[run].Type == RunType.Normal)
                {
                    
                    return HashCode.Calculate(this.buffer, runOffset, this.runList[run].Length);
                }

                HashCode hashCode = new HashCode(true);

                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.Advance(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.Advance(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHash(LexicalUnit unit)
        {
            int run = unit.head;

            if (run != -1)
            {
                int runOffset = unit.headOffset;
                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  

                if (runEntry.Type == RunType.Normal &&
                    kind != this.runList[run + 1].MajorKindPlusStartFlag)
                {
                    
                    return HashCode.Calculate(this.buffer, runOffset, runEntry.Length);
                }

                HashCode hashCode = new HashCode(true);

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.Advance(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.Advance(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        
        

        protected internal void WriteOriginalTo(ref Fragment fragment, ITextSink sink)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;

                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        sink.Write(this.buffer, runOffset, runEntry.Length);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        
        

        protected internal void WriteTo(ref Fragment fragment, ITextSink sink)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;

                do
                {
                    RunEntry runEntry = this.runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        sink.Write(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        sink.Write(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        protected internal void WriteTo(LexicalUnit unit, ITextSink sink)
        {
            int run = unit.head;

            if (run != -1)
            {
                int runOffset = unit.headOffset;

                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        sink.Write(this.buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        sink.Write(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind && !sink.IsEnough);
            }
        }

        private static char[] staticCollapseWhitespace = { ' ', '\r', '\n' };

        protected internal void WriteToAndCollapseWhitespace(ref Fragment fragment, ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;

                
                if (this.runList[run].Type < RunType.Normal)
                {
                    this.SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                }

                while (run != fragment.tail && !sink.IsEnough)
                {
                    InternalDebug.Assert(this.runList[run].Type >= RunType.Normal);
                    

                    if (this.runList[run].TextType <= RunTextType.Nbsp)
                    {
                        if (this.runList[run].TextType == RunTextType.NewLine)
                        {
                            collapseWhitespaceState = CollapseWhitespaceState.NewLine;
                        }
                        else if (collapseWhitespaceState == CollapseWhitespaceState.NonSpace)
                        {
                            collapseWhitespaceState = CollapseWhitespaceState.Whitespace;
                        }
                    }
                    else
                    {
                        if (collapseWhitespaceState != CollapseWhitespaceState.NonSpace)
                        {
                            if (collapseWhitespaceState == CollapseWhitespaceState.NewLine)
                            {
                                
                                sink.Write(staticCollapseWhitespace, 1, 2);
                            }
                            else
                            {
                                
                                sink.Write(staticCollapseWhitespace, 0, 1);
                            }

                            collapseWhitespaceState = CollapseWhitespaceState.NonSpace;
                        }

                        if (this.runList[run].Type == RunType.Literal)
                        {
                            
                            sink.Write(this.runList[run].Value);
                        }
                        else
                        {
                            sink.Write(this.buffer, runOffset, this.runList[run].Length);
                        }
                    }

                    runOffset += this.runList[run].Length;

                    run ++;

                    
                    if (run != fragment.tail && this.runList[run].Type < RunType.Normal)
                    {
                        this.SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                    }

                }
            }
        }

        
        

        protected internal string GetString(ref Fragment fragment, int maxLength)
        {
            if (fragment.head == fragment.tail)
            {
                
                return string.Empty;
            }

            if (this.IsContiguous(ref fragment))
            {
                
                return new string(this.buffer, fragment.headOffset, this.GetLength(ref fragment));
            }

            if (this.IsFragmentEmpty(ref fragment))
            {
                
                return string.Empty;
            }

            if (this.stringBuildSink == null)
            {
                this.stringBuildSink = new StringBuildSink();
            }

            this.stringBuildSink.Reset(maxLength);

            this.WriteTo(ref fragment, this.stringBuildSink);

            return this.stringBuildSink.ToString();
        }

        protected internal string GetString(LexicalUnit unit, int maxLength)
        {
            if (this.IsFragmentEmpty(unit))
            {
                
                return string.Empty;
            }

            if (this.IsContiguous(unit))
            {
                
                return new string(this.buffer, unit.headOffset, this.GetLength(unit));
            }

            if (this.stringBuildSink == null)
            {
                this.stringBuildSink = new StringBuildSink();
            }

            this.stringBuildSink.Reset(maxLength);

            this.WriteTo(unit, this.stringBuildSink);

            return this.stringBuildSink.ToString();
        }

        
        
        
        
        
        
        

        protected internal bool CaseInsensitiveCompareEqual(ref Fragment fragment, string str)
        {
            if (this.compareSink == null)
            {
                this.compareSink = new LowerCaseCompareSink();
            }

            this.compareSink.Reset(str);

            this.WriteTo(ref fragment, this.compareSink);

            return this.compareSink.IsEqual;
        }

        protected internal bool CaseInsensitiveCompareEqual(LexicalUnit unit, string str)
        {
            if (this.compareSink == null)
            {
                this.compareSink = new LowerCaseCompareSink();
            }

            this.compareSink.Reset(str);

            this.WriteTo(unit, this.compareSink);

            return this.compareSink.IsEqual;
        }

        
        

        protected internal virtual bool CaseInsensitiveCompareRunEqual(int runOffset, string str, int strOffset)
        {
            int index = strOffset;

            while (index < str.Length)
            {
                InternalDebug.Assert(!ParseSupport.IsUpperCase(str[index]));

                if (ParseSupport.ToLowerCase(this.buffer[runOffset++]) != str[index++])
                {
                    return false;
                }
            }

            return true;
        }

        
        
        
        
        
        
        

        protected internal bool CaseInsensitiveContainsSubstring(ref Fragment fragment, string str)
        {
            if (this.searchSink == null)
            {
                this.searchSink = new LowerCaseSubstringSearchSink();
            }

            this.searchSink.Reset(str);

            this.WriteTo(ref fragment, this.searchSink);

            return this.searchSink.IsFound;
        }

        protected internal bool CaseInsensitiveContainsSubstring(LexicalUnit unit, string str)
        {
            if (this.searchSink == null)
            {
                this.searchSink = new LowerCaseSubstringSearchSink();
            }

            this.searchSink.Reset(str);

            this.WriteTo(unit, this.searchSink);

            return this.searchSink.IsFound;
        }

        

        protected internal void StripLeadingWhitespace(ref Fragment fragment)
        {
            int run = fragment.head;

            if (run != fragment.tail)
            {
                int runOffset = fragment.headOffset;
                CharClass charClass;

                
                if (this.runList[run].Type < RunType.Normal)
                {
                    this.SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                }

                if (run == fragment.tail)
                {
                    
                    return;
                }

                do
                {
                    InternalDebug.Assert(this.runList[run].Type >= RunType.Normal);

                    if (this.runList[run].Type == RunType.Literal)
                    {
                        if (this.runList[run].Value > 0xFFFF)
                        {
                            break;
                        }

                        charClass = ParseSupport.GetCharClass((char)this.runList[run].Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }

                        
                    }
                    else
                    {
                        

                        int offset = runOffset;
                        while (offset < runOffset + this.runList[run].Length)
                        {
                            charClass = ParseSupport.GetCharClass(this.buffer[offset]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }

                            offset ++;
                        }

                        if (offset < runOffset + this.runList[run].Length)
                        {
                            this.runList[run].Length -= (offset - runOffset);
                            runOffset = offset;
                            break;
                        }

                        
                    }

                    runOffset += this.runList[run].Length;

                    run ++;

                    
                    if (run != fragment.tail && this.runList[run].Type < RunType.Normal)
                    {
                        this.SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                    }
                }
                while (run != fragment.tail);

                fragment.head = run;
                fragment.headOffset = runOffset;
            }
        }

        
        protected internal bool SkipLeadingWhitespace(LexicalUnit unit, ref FragmentPosition position)
        {
            int run = unit.head;

            if (run != -1)  
            {
                int runOffset = unit.headOffset;
                CharClass charClass;

                RunEntry runEntry = this.runList[run];
                uint kind = runEntry.MajorKind;  
                int deltaOffset = 0;

                do
                {
                    if (runEntry.Type == RunType.Literal)
                    {
                        if (runEntry.Value > 0xFFFF)
                        {
                            break;
                        }

                        charClass = ParseSupport.GetCharClass((char)runEntry.Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }

                        
                    }
                    else if (runEntry.Type == RunType.Normal)
                    {
                        int offset = runOffset;
                        while (offset < runOffset + runEntry.Length)
                        {
                            charClass = ParseSupport.GetCharClass(this.buffer[offset]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }

                            offset ++;
                        }

                        if (offset < runOffset + runEntry.Length)
                        {
                            deltaOffset = offset - runOffset;
                            break;
                        }

                        
                    }

                    runOffset += runEntry.Length;

                    runEntry = this.runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = deltaOffset;

                if (run == unit.head || runEntry.MajorKindPlusStartFlag == kind)
                {
                    
                    return true;
                }
            }

            
            return false;
        }

        

        protected internal bool MoveToNextRun(ref Fragment fragment, ref FragmentPosition position, bool skipInvalid)
        {
            InternalDebug.Assert(position.run >= fragment.head - 1 && position.run <= fragment.tail);

            int run = position.run;

            if (run != fragment.tail)
            {
                InternalDebug.Assert(run >= fragment.head || (position.runDeltaOffset == 0 && position.runOffset == fragment.headOffset));

                if (run >= fragment.head)
                {
                    
                    position.runOffset += this.runList[run].Length;
                    position.runDeltaOffset = 0;
                }

                run ++;

                if (skipInvalid)
                {
                    while (run != fragment.tail && this.runList[run].Type == RunType.Invalid)
                    {
                        position.runOffset += this.runList[run].Length;
                        run ++;
                    }
                }

                position.run = run;

                return (run != fragment.tail);
            }

            return false;
        }

        

        protected internal bool IsCurrentEof(ref FragmentPosition position)
        {
            int run = position.run;

            InternalDebug.Assert(this.runList[run].Type >= RunType.Normal);

            if (this.runList[run].Type == RunType.Literal)
            {
                return (position.runDeltaOffset == Token.LiteralLength(this.runList[run].Value));
            }

            return (position.runDeltaOffset == this.runList[run].Length);
        }

        

        protected internal int ReadCurrent(ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            int run = position.run;

            InternalDebug.Assert(this.runList[run].Type >= RunType.Normal);
            InternalDebug.Assert(count > 0);

            if (this.runList[run].Type == RunType.Literal)
            {
                int literalLength = Token.LiteralLength(this.runList[run].Value);

                if (position.runDeltaOffset == literalLength)
                {
                    return 0;
                }

                if (literalLength == 1)
                {
                    InternalDebug.Assert(position.runDeltaOffset == 0);

                    buffer[offset] = (char) this.runList[run].Value;

                    position.runDeltaOffset ++;
                    return 1;
                }

                InternalDebug.Assert(literalLength == 2);

                if (position.runDeltaOffset != 0)
                {
                    InternalDebug.Assert(position.runDeltaOffset == 1);

                    buffer[offset] = Token.LiteralLastChar(this.runList[run].Value);

                    position.runDeltaOffset ++;
                    return 1;
                }

                buffer[offset++] = Token.LiteralFirstChar(this.runList[run].Value);
                count --;

                position.runDeltaOffset ++;

                if (count == 0)
                {
                    return 1;
                }

                buffer[offset] = Token.LiteralLastChar(this.runList[run].Value);

                position.runDeltaOffset ++;
                return 2;
            }

            int copyCount = Math.Min(count, this.runList[run].Length - position.runDeltaOffset);

            if (copyCount != 0)
            {
                Buffer.BlockCopy(this.buffer, (position.runOffset + position.runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                position.runDeltaOffset += copyCount;
            }

            return copyCount;
        }

         

        internal void SkipNonTextRuns(ref int run, ref int runOffset, int tail)
        {
            InternalDebug.Assert(run != tail && this.runList[run].Type < RunType.Normal);

            do
            {
                
                
                runOffset += this.runList[run].Length;
                run ++;
            }
            while (run != tail && this.runList[run].Type < RunType.Normal);
        }

        internal void Reset()
        {
            this.tokenId = TokenId.None;
            this.argument = 0;

            this.whole.Reset();
            this.wholePosition.Reset();
        }

        

        public struct RunEnumerator
        {
            private Token token;
#if DEBUG
            private int index;
#endif
            

            internal RunEnumerator(Token token)
            {
                this.token = token;
#if DEBUG
                this.index = this.token.wholePosition.run;
#endif
            }

            

            public TokenRun Current
            {
                get
                { 
                    InternalDebug.Assert(this.token.wholePosition.run >= this.token.whole.head && this.token.wholePosition.run < this.token.whole.tail);
                    this.AssertCurrent();

                    return new TokenRun(this.token);
                }
            }

            public bool IsValidPosition
            {
                get { return this.token.wholePosition.run >= this.token.whole.head && this.token.wholePosition.run < this.token.whole.tail; }
            }

            public int CurrentIndex
            {
                get { return this.token.wholePosition.run; }
            }

            public int CurrentOffset
            {
                get { return this.token.wholePosition.runOffset; }
            }

            

            public bool MoveNext()
            {
                this.AssertCurrent();

                bool ret = this.token.MoveToNextRun(ref this.token.whole, ref this.token.wholePosition, false);
#if DEBUG
                this.index = this.token.wholePosition.run;
#endif
                return ret;
            }

            public bool MoveNext(bool skipInvalid)
            {
                this.AssertCurrent();

                bool ret = this.token.MoveToNextRun(ref this.token.whole, ref this.token.wholePosition, skipInvalid);
#if DEBUG
                this.index = this.token.wholePosition.run;
#endif
                return ret;
            }

            public void Rewind()
            {
                this.AssertCurrent();

                this.token.wholePosition.Rewind(this.token.whole);
#if DEBUG
                this.index = this.token.wholePosition.run;
#endif
            }

            public RunEnumerator GetEnumerator()
            {
                return this;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.token.wholePosition.run == this.index);
#endif
            }
        }

        

        public struct TextReader
        {
            private Token token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TextReader(Token token)
            {
                this.token = token;
#if DEBUG
                this.position = this.token.wholePosition;
#endif
            }

            

            public int Length
            {
                get
                {
                    this.AssertCurrent();
                    return this.token.GetLength(ref this.token.whole);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(ref this.token.whole, ref this.token.wholePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.wholePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.AssertCurrent();

                this.token.wholePosition.Rewind(this.token.whole);
#if DEBUG
                this.position = this.token.wholePosition;
#endif
            }

            public void WriteTo(ITextSink sink)
            {
                this.AssertCurrent();

                this.token.WriteTo(ref this.token.whole, sink);
            }

            public void WriteToAndCollapseWhitespace(ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
            {
                this.AssertCurrent();

                this.token.WriteToAndCollapseWhitespace(ref this.token.whole, sink, ref collapseWhitespaceState);
            }

            public void StripLeadingWhitespace()
            {
                this.token.StripLeadingWhitespace(ref this.token.whole);
                this.Rewind();
            }

            

            public int OriginalLength
            {
                get
                {
                    this.AssertCurrent();
                    return this.token.GetOriginalLength(ref this.token.whole);
                }
            }

            
            

            public int ReadOriginal(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.ReadOriginal(ref this.token.whole, ref this.token.wholePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.wholePosition;
#endif
                return countRead;
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                this.AssertCurrent();

                this.token.WriteOriginalTo(ref this.token.whole, sink);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.wholePosition));
#endif
            }
        }

        

        internal struct RunEntry
        {
            internal const int MaxRunLength = 0x07FFFFFF;
            internal const int MaxRunValue = 0x00FFFFFF;

            private uint lengthAndType;                 
            private uint valueAndKind;                  

            public void Initialize(RunType type, RunTextType textType, uint kind, int length, int value)
            {
                InternalDebug.Assert(length <= MaxRunLength && value < MaxRunValue && (kind > MaxRunValue || kind == 0));

                this.lengthAndType = (uint)length | (uint)type | (uint)textType;
                this.valueAndKind = (uint)value | (uint)kind;
            }

            public void InitializeSentinel()
            {
                this.valueAndKind = (uint)(RunKind.Invalid | RunKind.StartLexicalUnitFlag);
                
            }

            public RunType Type { get { return (RunType)(this.lengthAndType & (uint)RunType.Mask); } }
            public RunTextType TextType { get { return (RunTextType)(this.lengthAndType & (uint)RunTextType.Mask); } }

            public int Length
            {
                get { return (int)(this.lengthAndType & 0x00FFFFFFu); }
                set { this.lengthAndType = (uint)value | (this.lengthAndType & 0xFF000000u); }
            }

            public uint Kind { get { return (this.valueAndKind & 0xFF000000u); } }
            public uint MajorKindPlusStartFlag { get { return (this.valueAndKind & 0xFC000000u); } }
            public uint MajorKind { get { return (this.valueAndKind & 0x7C000000u); } }
            public int Value { get { return (int)(this.valueAndKind & 0x00FFFFFFu); } }

            
            public override string ToString()
            {
                return this.Type.ToString() + " - " + this.TextType.ToString() + " - " + ((this.Kind & ~(uint)RunKind.StartLexicalUnitFlag) >> 26).ToString() + "/" + ((this.Kind >> 24)  & 3).ToString() + " (" + this.Length + ") = " + this.Value.ToString("X6");
            }
        }

        

        internal struct LexicalUnit
        {
            public int head;                            
            public int headOffset;                      
#if false
            public bool IsEmpty
            {
                get { return this.head == this.tail; }
            }
#endif
            public void Reset()
            {
                this.head = -1;
                this.headOffset = 0;
            }

            public void Initialize(int run, int offset)
            {
                this.head = run;
                this.headOffset = offset;
            }

            
            public override string ToString()
            {
                return this.head.ToString("X") + " / " + this.headOffset.ToString("X");
            }
        }

        

        internal struct Fragment
        {
            public int head;                            
            public int tail;                            
            public int headOffset;                      

            public bool IsEmpty
            {
                get { return this.head == this.tail; }
            }

            public void Reset()
            {
                this.head = this.tail = this.headOffset = 0;
            }

            public void Initialize(int run, int offset)
            {
                this.head = this.tail = run;
                this.headOffset = offset;
            }

            
            public override string ToString()
            {
                return this.head.ToString("X") + " - " + this.tail.ToString("X") + " / " + this.headOffset.ToString("X");
            }
        }

        

        internal struct FragmentPosition
        {
            public int run;                             
            public int runOffset;                       
            public int runDeltaOffset;                  

            public void Reset()
            {
                
                this.run = -2;      
                this.runOffset = 0;
                this.runDeltaOffset = 0;
            }

            public void Rewind(LexicalUnit unit)
            {
                this.run = unit.head - 1;
                this.runOffset = unit.headOffset;
                this.runDeltaOffset = 0;
            }

            public void Rewind(Fragment fragment)
            {
                this.run = fragment.head - 1;
                this.runOffset = fragment.headOffset;
                this.runDeltaOffset = 0;
            }

            public bool SameAs(FragmentPosition pos2)
            {
                return this.run == pos2.run && this.runOffset == pos2.runOffset && this.runDeltaOffset == pos2.runDeltaOffset;
            }

            
            public override string ToString()
            {
                return this.run.ToString("X") + " / " + this.runOffset.ToString("X") + " + " + this.runDeltaOffset.ToString("X");
            }
        }

        
        

        private class LowerCaseCompareSink : ITextSink
        {
            private bool definitelyNotEqual;
            private int strIndex;
            private string str;

            public LowerCaseCompareSink()
            {
            }

            public bool IsEqual { get { return !this.definitelyNotEqual && this.strIndex == this.str.Length; } }

            public bool IsEnough { get { return this.definitelyNotEqual; } }

            public void Reset(string str)
            {
                this.str = str;
                this.strIndex = 0;
                this.definitelyNotEqual = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                int end = offset + count;

                while (offset < end)
                {
                    if (this.strIndex == 0)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }
                    }
                    else if (this.strIndex == this.str.Length)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }

                        this.definitelyNotEqual = true;
                        break;
                    }

                    if (ParseSupport.ToLowerCase(buffer[offset]) != this.str[this.strIndex])
                    {
                        this.definitelyNotEqual = true;
                        break;
                    }

                    offset++;
                    this.strIndex++;
                }
            }

            public void Write(int ucs32Char)
            {
                if (Token.LiteralLength(ucs32Char) != 1)
                {
                    
                    
                    this.definitelyNotEqual = true;
                    return;
                }

                if (this.strIndex == 0)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        
                        return;
                    }
                }
                else if (this.strIndex == this.str.Length)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        
                        return;
                    }

                    this.definitelyNotEqual = true;
                    return;
                }

                if (this.str[this.strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    this.definitelyNotEqual = true;
                    return;
                }

                this.strIndex++;
            }
        }

        
        
        
        
        
        
        

        private class LowerCaseSubstringSearchSink : ITextSink
        {
            private bool found;
            private int strIndex;
            private string str;

            public LowerCaseSubstringSearchSink()
            {
            }

            public bool IsFound { get { return this.found; } }

            public bool IsEnough { get { return this.found; } }

            public void Reset(string str)
            {
                this.str = str;
                this.strIndex = 0;
                this.found = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(!this.found && this.strIndex < this.str.Length);

                int end = offset + count;

                while (offset < end && this.strIndex < this.str.Length)
                {
                    if (ParseSupport.ToLowerCase(buffer[offset]) == this.str[this.strIndex])
                    {
                        this.strIndex ++;
                    }
                    else
                    {
                        
                        this.strIndex = 0;
                    }

                    offset ++;
                }

                if (this.strIndex == this.str.Length)
                {
                    this.found = true;
                }
            }

            public void Write(int ucs32Char)
            {
                InternalDebug.Assert(!this.found && this.strIndex < this.str.Length);

                if (Token.LiteralLength(ucs32Char) != 1 || 
                    this.str[this.strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    
                    this.strIndex = 0;
                    return;
                }

                this.strIndex ++;

                if (this.strIndex == this.str.Length)
                {
                    this.found = true;
                }
            }
        }
    }
}

