// ***************************************************************
// <copyright file="CssToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;

    

    internal enum CssTokenId : byte
    {
        None = TokenId.None,
        EndOfFile = TokenId.EndOfFile,
        AtRule = TokenId.EncodingChange + 1,
        Declarations,
        RuleSet,
    }

    

    internal enum CssRunKind : uint
    {
        Invalid = RunKind.Invalid,
        Comment = RunKind.Invalid,

        Text = RunKind.Text,

        Space = (11u << 24),

        SimpleSelector = (12u << 24),
        Identifier = (13u << 24),
        Delimiter = (14u << 24),
        AtRuleName = (15u << 24),

        SelectorName = (16u << 24),
        SelectorCombinatorOrComma = (17u << 24),

        
        SelectorPseudoStart = (18u << 24),
        SelectorPseudo = (19u << 24),
        SelectorPseudoArg = (20u << 24),
        SelectorClassStart = (21u << 24),
        SelectorClass = (22u << 24),
        SelectorHashStart = (23u << 24),
        SelectorHash = (24u << 24),
        SelectorAttribStart = (25u << 24),
        SelectorAttribName = (26u << 24),
        SelectorAttribEquals = (27u << 24),
        SelectorAttribIncludes = (28u << 24),
        SelectorAttribDashmatch = (29u << 24),
        SelectorAttribIdentifier = (30u << 24),
        SelectorAttribString = (31u << 24),
        SelectorAttribEnd = (32u << 24),

        PropertyName = (40u << 24),
        PropertyColon = (41u << 24),

        
        ImportantStart = (42u << 24),
        Important = (43u << 24),
        Operator = (44u << 24),
        UnaryOperator = (45u << 24),
        Dot = (46u << 24),
        Percent = (47u << 24),
        Metrics = (48u << 24),
        TermIdentifier = (49u << 24),
        UnicodeRange = (50u << 24),
        FunctionStart = (51u << 24),
        FunctionEnd = (52u << 24),
        HexColorStart = (53u << 24),
        HexColor = (54u << 24),
        String = (55u << 24),
        Numeric = (56u << 24),
        Url = (58u << 24),

        PropertySemicolon = (57u << 24),

        
        PageIdent = (70u << 24),
        PagePseudoStart = (71u << 24),
        PagePseudo = (72u << 24),

    }

    

    internal enum CssSelectorClassType : byte
    {
        Regular,
        Pseudo,
        Hash,
        Attrib,
    }

    

    internal enum CssSelectorCombinator : byte
    {
        None, 
        Descendant, 
        Adjacent, 
        Child, 
    }

    

    internal class CssToken : Token
    {
        protected internal PropertyListPartMajor partMajor;         
        protected internal PropertyListPartMinor partMinor;         

        protected internal PropertyEntry[] propertyList;            
        protected internal int propertyHead;                        
        protected internal int propertyTail;                        
        protected internal int currentProperty;                     

        protected internal FragmentPosition propertyNamePosition;   
        protected internal FragmentPosition propertyValuePosition;  

        protected internal SelectorEntry[] selectorList;            
        protected internal int selectorHead;                        
        protected internal int selectorTail;                        
        protected internal int currentSelector;                     

        protected internal FragmentPosition selectorNamePosition;   
        protected internal FragmentPosition selectorClassPosition;  

        

        public enum PropertyListPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,
        }

        

        public enum PropertyListPartMinor : byte
        {
            Empty = 0,

            
            BeginProperty = 0x08 | ContinueProperty,
            ContinueProperty = 0x10,
            EndProperty = ContinueProperty | 0x20,
            EndPropertyWithOtherProperties = EndProperty | Properties,
            PropertyPartMask = BeginProperty | EndProperty,

            
            Properties = 0x80,
        }

        

        public enum PropertyPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,

            
            ValueQuoted = 0x40,
            Deleted = 0x80,
            MaskOffFlags = Complete,
        }

        

        public enum PropertyPartMinor : byte
        {
            Empty = 0,

            BeginName = 0x01 | ContinueName,
            ContinueName = 0x02,
            EndName = ContinueName | 0x04,
            EndNameWithBeginValue = EndName | BeginValue,
            EndNameWithCompleteValue = EndName | CompleteValue,
            CompleteName = BeginName | EndName,
            CompleteNameWithBeginValue = CompleteName | BeginValue,
            CompleteNameWithCompleteValue = CompleteName | CompleteValue,

            BeginValue = 0x08 | ContinueValue,
            ContinueValue = 0x10,
            EndValue = ContinueValue | 0x20,
            CompleteValue = BeginValue | EndValue,
        }

        public CssToken()
        {
            this.Reset();
        }

        public new CssTokenId TokenId
        {
            get { return (CssTokenId)base.TokenId; }
        }

        public PropertyListPartMajor MajorPart
        {
            get { return this.partMajor; }
        }

        public PropertyListPartMinor MinorPart
        {
            get { return this.partMinor; }
        }

        public bool IsPropertyListBegin
        {
            get { return (this.partMajor & PropertyListPartMajor.Begin) == PropertyListPartMajor.Begin; }
        }

        public bool IsPropertyListEnd
        {
            get { return (this.partMajor & PropertyListPartMajor.End) == PropertyListPartMajor.End; }
        }

        public PropertyEnumerator Properties
        {
            get { return new PropertyEnumerator(this); }
        }

        public SelectorEnumerator Selectors
        {
            get { return new SelectorEnumerator(this); }
        }

        internal new void Reset()
        {
            this.partMajor = CssToken.PropertyListPartMajor.None;
            this.partMinor = CssToken.PropertyListPartMinor.Empty;

            this.propertyHead = this.propertyTail = 0;
            this.currentProperty = -1;

            this.selectorHead = this.selectorTail = 0;
            this.currentSelector = -1;
        }

        
        
        
        
        
        protected internal void WriteEscapedOriginalTo(ref Fragment fragment, ITextSink sink)
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
                        this.EscapeAndWriteBuffer(
                            this.buffer,
                            runOffset,
                            runEntry.Length,
                            sink);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        private void EscapeAndWriteBuffer(
            char[] buffer,
            int offset,
            int length,
            ITextSink sink)
        {
            int lastIdx = offset;
            for (int idx = offset; idx < offset + length; )
            {
                char ch = buffer[idx];

                
                
                if (ch == '>' || ch == '<')
                {
                    
                    
                    if (idx - lastIdx > 0)
                    {
                        sink.Write(buffer, lastIdx, idx - lastIdx);
                    }

                    
                    
                    uint value = (uint)ch;
                    char[] escapeSequenceBuffer = new char[4];
                    escapeSequenceBuffer[0] = '\\';
                    escapeSequenceBuffer[3] = ' ';
                    for (int i = 2; i > 0; --i)
                    {
                        uint digit = value & 0xF;
                        escapeSequenceBuffer[i] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    sink.Write(escapeSequenceBuffer, 0, 4);

                    lastIdx = ++idx;
                }
                else
                {
                    
                    
                    CssToken.AttemptUnescape(buffer, offset + length, ref ch, ref idx);
                    ++idx;
                }
            }
            
            
            sink.Write(buffer, lastIdx, length - (lastIdx - offset));
        }

        
        
        
        
        
        
        
        internal static bool AttemptUnescape(char[] parseBuffer, int parseEnd, ref char ch, ref int parseCurrent)
        {
            if (ch != '\\' || parseCurrent == parseEnd)
            {
                return false;
            }

            ch = parseBuffer[++parseCurrent];
            CharClass charClass = ParseSupport.GetCharClass(ch);

            int end = parseCurrent + 6;
            end = end < parseEnd ? end : parseEnd;

            if (ParseSupport.HexCharacter(charClass))
            {
                
                
                int result = 0;
                while (true)
                {
                    result <<= 4;
                    result |= ParseSupport.CharToHex(ch);

                    if (parseCurrent == end)
                    {
                        break;
                    }
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (!ParseSupport.HexCharacter(charClass))
                    {
                        if (ch == '\r' && parseCurrent != parseEnd)
                        {
                            ch = parseBuffer[++parseCurrent];
                            if (ch == '\n')
                            {
                                charClass = ParseSupport.GetCharClass(ch);
                            }
                            else
                            {
                                --parseCurrent;
                            }
                        }
                        if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n' && ch != '\f')
                        {
                            --parseCurrent;
                        }
                        break;
                    }
                }

                ch = (char)result;
                return true;
            }

            if (ch >= ' ' && ch != (char)0x7F)
            {
                
                
                return true;
            }

            
            
            --parseCurrent;
            ch = '\\';

            return false;
        }

        

        public struct PropertyEnumerator
        {
            private CssToken token;
#if DEBUG
            private int index;
#endif
            

            internal PropertyEnumerator(CssToken token)
            {
                this.token = token;
#if DEBUG
                this.index = this.token.currentProperty;
#endif
            }

            

            public int Count
            {
                get
                {
                    return this.token.propertyTail - this.token.propertyHead;
                }
            }

            public int ValidCount
            {
                get
                {
                    int count = 0;
                    for (int i = this.token.propertyHead; i < this.token.propertyTail; i++)
                    {
                        if (!this.token.propertyList[i].IsPropertyDeleted)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            public CssProperty Current
            {
                get
                {
                    InternalDebug.Assert(this.token.currentProperty >= this.token.propertyHead && this.token.currentProperty < this.token.propertyTail);
                    this.AssertCurrent();

                    return new CssProperty(this.token);
                }
            }

            public int CurrentIndex
            {
                get { return this.token.currentProperty; }
            }

            public CssProperty this[int i]
            {
                get
                {
                    InternalDebug.Assert(i >= this.token.propertyHead && i < this.token.propertyTail);

                    this.token.currentProperty = i;

                    this.token.propertyNamePosition.Rewind(this.token.propertyList[i].name);
                    this.token.propertyValuePosition.Rewind(this.token.propertyList[i].value);

                    return new CssProperty(this.token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(this.token.currentProperty >= this.token.propertyHead - 1 && this.token.currentProperty <= this.token.propertyTail);
                this.AssertCurrent();

                if (this.token.currentProperty != this.token.propertyTail)
                {
                    this.token.currentProperty++;

                    if (this.token.currentProperty != this.token.propertyTail)
                    {
                        this.token.propertyNamePosition.Rewind(this.token.propertyList[this.token.currentProperty].name);
                        this.token.propertyValuePosition.Rewind(this.token.propertyList[this.token.currentProperty].value);
                    }
#if DEBUG
                    this.index = this.token.currentProperty;
#endif
                }

                return (this.token.currentProperty != this.token.propertyTail);
            }

            public void Rewind()
            {
                this.AssertCurrent();

                this.token.currentProperty = this.token.propertyHead - 1;
#if DEBUG
                this.index = this.token.currentProperty;
#endif
            }

            public PropertyEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(CssNameIndex nameId)
            {
                for (int i = this.token.propertyHead; i < this.token.propertyTail; i++)
                {
                    if (this.token.propertyList[i].nameId == nameId)
                    {
                        this.token.currentProperty = i;

                        this.token.propertyNamePosition.Rewind(this.token.propertyList[i].name);
                        this.token.propertyValuePosition.Rewind(this.token.propertyList[i].value);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.token.currentProperty == this.index);
#endif
            }
        }

        

        public struct PropertyNameTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal PropertyNameTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                this.position = this.token.propertyNamePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(ref this.token.propertyList[this.token.currentProperty].name); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(ref this.token.propertyList[this.token.currentProperty].name, ref this.token.propertyNamePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.propertyNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.propertyNamePosition.Rewind(this.token.propertyList[this.token.currentProperty].name);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(ref this.token.propertyList[this.token.currentProperty].name, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                this.token.WriteOriginalTo(ref this.token.propertyList[this.token.currentProperty].name, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(ref this.token.propertyList[this.token.currentProperty].name, maxSize);
            }

            public void MakeEmpty()
            {
                this.token.propertyList[this.token.currentProperty].name.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.propertyNamePosition));
#endif
            }
        }

        

        public struct PropertyValueTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal PropertyValueTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                this.position = this.token.propertyValuePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(ref this.token.propertyList[this.token.currentProperty].value); }
            }

            public bool IsEmpty
            {
                get { return this.token.IsFragmentEmpty(ref this.token.propertyList[this.token.currentProperty].value); }
            }

            public bool IsContiguous
            {
                get { return this.token.IsContiguous(ref this.token.propertyList[this.token.currentProperty].value); }
            }

            public BufferString ContiguousBufferString
            {
                get
                {
                    InternalDebug.Assert(this.IsContiguous);
                    return new BufferString(
                                this.token.buffer,
                                this.token.propertyList[this.token.currentProperty].value.headOffset,
                                this.token.runList[this.token.propertyList[this.token.currentProperty].value.head].Length);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(ref this.token.propertyList[this.token.currentProperty].value, ref this.token.propertyValuePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.propertyValuePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.propertyValuePosition.Rewind(this.token.propertyList[this.token.currentProperty].value);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(ref this.token.propertyList[this.token.currentProperty].value, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                this.token.WriteOriginalTo(ref this.token.propertyList[this.token.currentProperty].value, sink);
            }

            public void WriteEscapedOriginalTo(ITextSink sink)
            {
                this.token.WriteEscapedOriginalTo(ref this.token.propertyList[this.token.currentProperty].value, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(ref this.token.propertyList[this.token.currentProperty].value, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return this.token.CaseInsensitiveCompareEqual(ref this.token.propertyList[this.token.currentProperty].value, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return this.token.CaseInsensitiveContainsSubstring(ref this.token.propertyList[this.token.currentProperty].value, str);
            }

            public void MakeEmpty()
            {
                this.token.propertyList[this.token.currentProperty].value.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.propertyValuePosition));
#endif
            }
        }

        

        protected internal struct PropertyEntry
        {
            public CssNameIndex nameId;
            public byte quoteChar;              
            public PropertyPartMajor partMajor;
            public PropertyPartMinor partMinor;

            public Fragment name;
            public Fragment value;

            

            public bool IsCompleteProperty
            {
                get { return this.MajorPart == PropertyPartMajor.Complete; }
            }

            public bool IsPropertyBegin
            {
                get { return (this.partMajor & PropertyPartMajor.Begin) == PropertyPartMajor.Begin; }
            }

            public bool IsPropertyEnd
            {
                get { return (this.partMajor & PropertyPartMajor.End) == PropertyPartMajor.End; }
            }

            public bool IsPropertyNameEnd
            {
                get { return (this.partMinor & PropertyPartMinor.EndName) == PropertyPartMinor.EndName; }
            }

            public bool IsPropertyValueBegin
            {
                get { return (this.partMinor & PropertyPartMinor.BeginValue) == PropertyPartMinor.BeginValue; }
            }

            public PropertyPartMajor MajorPart
            {
                get { return this.partMajor & PropertyPartMajor.MaskOffFlags; }
            }

            public PropertyPartMinor MinorPart
            {
                get { return this.partMinor; }
                set { this.partMinor = value; }
            }

            public bool IsPropertyValueQuoted
            {
                get { return (this.partMajor & PropertyPartMajor.ValueQuoted) == PropertyPartMajor.ValueQuoted; }
                set { this.partMajor = value ? (this.partMajor | PropertyPartMajor.ValueQuoted) : (this.partMajor & ~PropertyPartMajor.ValueQuoted); }
            }

            public bool IsPropertyDeleted
            {
                get { return (this.partMajor & PropertyPartMajor.Deleted) == PropertyPartMajor.Deleted; }
                set { this.partMajor = value ? (this.partMajor | PropertyPartMajor.Deleted) : (this.partMajor & ~PropertyPartMajor.Deleted); }
            }
        }

        

        public struct SelectorEnumerator
        {
            private CssToken token;
#if DEBUG
            private int index;
#endif
            

            internal SelectorEnumerator(CssToken token)
            {
                this.token = token;
#if DEBUG
                this.index = this.token.currentSelector;
#endif
            }

            

            public int Count
            {
                get
                {
                    return this.token.selectorTail - this.token.selectorHead;
                }
            }

            public int ValidCount
            {
                get
                {
                    int count = 0;
                    for (int i = this.token.selectorHead; i < this.token.selectorTail; i++)
                    {
                        if (!this.token.selectorList[i].IsSelectorDeleted)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            public CssSelector Current
            {
                get
                {
                    InternalDebug.Assert(this.token.currentSelector >= this.token.selectorHead && this.token.currentSelector < this.token.selectorTail);
                    this.AssertCurrent();

                    return new CssSelector(this.token);
                }
            }

            public int CurrentIndex
            {
                get { return this.token.currentSelector; }
            }

            public CssSelector this[int i]
            {
                get
                {
                    InternalDebug.Assert(i >= this.token.selectorHead && i < this.token.selectorTail);

                    this.token.currentSelector = i;

                    this.token.selectorNamePosition.Rewind(this.token.selectorList[i].name);
                    this.token.selectorClassPosition.Rewind(this.token.selectorList[i].className);

                    return new CssSelector(this.token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(this.token.currentSelector >= this.token.selectorHead - 1 && this.token.currentSelector <= this.token.selectorTail);
                this.AssertCurrent();

                if (this.token.currentSelector != this.token.selectorTail)
                {
                    this.token.currentSelector++;

                    if (this.token.currentSelector != this.token.selectorTail)
                    {
                        this.token.selectorNamePosition.Rewind(this.token.selectorList[this.token.currentSelector].name);
                        this.token.selectorClassPosition.Rewind(this.token.selectorList[this.token.currentSelector].className);
                    }
#if DEBUG
                    this.index = this.token.currentSelector;
#endif
                }

                return (this.token.currentSelector != this.token.selectorTail);
            }

            public void Rewind()
            {
                this.AssertCurrent();

                this.token.currentSelector = this.token.selectorHead - 1;
#if DEBUG
                this.index = this.token.currentSelector;
#endif
            }

            public SelectorEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(HtmlNameIndex nameId)
            {
                for (int i = this.token.selectorHead; i < this.token.selectorTail; i++)
                {
                    if (this.token.selectorList[i].nameId == nameId)
                    {
                        this.token.currentSelector = i;

                        this.token.selectorNamePosition.Rewind(this.token.selectorList[i].name);
                        this.token.selectorClassPosition.Rewind(this.token.selectorList[i].className);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.token.currentSelector == this.index);
#endif
            }
        }

        

        public struct SelectorNameTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal SelectorNameTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                this.position = this.token.selectorNamePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(ref this.token.selectorList[this.token.currentSelector].name); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(ref this.token.selectorList[this.token.currentSelector].name, ref this.token.selectorNamePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.selectorNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.selectorNamePosition.Rewind(this.token.selectorList[this.token.currentSelector].name);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(ref this.token.selectorList[this.token.currentSelector].name, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                this.token.WriteOriginalTo(ref this.token.selectorList[this.token.currentSelector].name, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(ref this.token.selectorList[this.token.currentSelector].name, maxSize);
            }

            public void MakeEmpty()
            {
                this.token.selectorList[this.token.currentSelector].name.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.selectorNamePosition));
#endif
            }
        }

        

        public struct SelectorClassTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal SelectorClassTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                this.position = this.token.selectorClassPosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(ref this.token.selectorList[this.token.currentSelector].className); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(ref this.token.selectorList[this.token.currentSelector].className, ref this.token.selectorClassPosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.selectorClassPosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.selectorClassPosition.Rewind(this.token.selectorList[this.token.currentSelector].className);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(ref this.token.selectorList[this.token.currentSelector].className, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                this.token.WriteEscapedOriginalTo(ref this.token.selectorList[this.token.currentSelector].className, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(ref this.token.selectorList[this.token.currentSelector].className, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return this.token.CaseInsensitiveCompareEqual(ref this.token.selectorList[this.token.currentSelector].className, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return this.token.CaseInsensitiveContainsSubstring(ref this.token.selectorList[this.token.currentSelector].className, str);
            }

            public void MakeEmpty()
            {
                this.token.selectorList[this.token.currentSelector].className.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.selectorClassPosition));
#endif
            }
        }

        

        protected internal struct SelectorEntry
        {
            public HtmlNameIndex nameId;

            public bool deleted;

            public Fragment name;
            public Fragment className;
            public CssSelectorClassType classType;
            public CssSelectorCombinator combinator;

            

            public bool IsSelectorDeleted
            {
                get { return this.deleted; }
                set { this.deleted = value; }
            }
        }
    }

    

    internal struct CssSelector
    {
        private CssToken token;
#if DEBUG
        private int index;
#endif
        

        internal CssSelector(CssToken token)
        {
            this.token = token;
#if DEBUG
            this.index = this.token.currentSelector;
#endif
        }

        

        public int Index
        {
            get { this.AssertCurrent(); return this.token.currentSelector; }
        }

        public bool IsDeleted
        {
            get { this.AssertCurrent(); return this.token.selectorList[this.token.currentSelector].IsSelectorDeleted; }
        }

        public HtmlNameIndex NameId
        {
            get { this.AssertCurrent(); return this.token.selectorList[this.token.currentSelector].nameId; }
        }

        public bool HasNameFragment
        {
            get { this.AssertCurrent(); return !this.token.selectorList[this.token.currentSelector].name.IsEmpty; }
        }

        public CssToken.SelectorNameTextReader Name
        {
            get { this.AssertCurrent(); return new CssToken.SelectorNameTextReader(this.token); }
        }

        public bool HasClassFragment
        {
            get { this.AssertCurrent(); return !this.token.selectorList[this.token.currentSelector].className.IsEmpty; }
        }

        public CssToken.SelectorClassTextReader ClassName
        {
            get { this.AssertCurrent(); return new CssToken.SelectorClassTextReader(this.token); }
        }

        public CssSelectorClassType ClassType
        {
            get { this.AssertCurrent(); return this.token.selectorList[this.token.currentSelector].classType; }
        }

        public bool IsSimple
        {
            get
            {
                this.AssertCurrent();

                
                
                return (this.token.selectorList[this.token.currentSelector].combinator == CssSelectorCombinator.None &&
                    ((this.token.selectorTail == this.token.currentSelector + 1) ||
                    (this.token.selectorList[this.token.currentSelector + 1].combinator == CssSelectorCombinator.None)));
            }
        }

        public CssSelectorCombinator Combinator
        {
            get { this.AssertCurrent(); return this.token.selectorList[this.token.currentSelector].combinator; }
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(this.token.currentSelector == this.index);
#endif
        }
    }

    

    internal struct CssProperty
    {
        private CssToken token;
#if DEBUG
        private int index;
#endif
        

        internal CssProperty(CssToken token)
        {
            this.token = token;
#if DEBUG
            this.index = this.token.currentProperty;
#endif
        }

        

        public int Index
        {
            get { this.AssertCurrent(); return this.token.currentProperty; }
        }

        public bool IsCompleteProperty
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsCompleteProperty; }
        }

        public bool IsPropertyBegin
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsPropertyBegin; }
        }

        public bool IsPropertyEnd
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsPropertyEnd; }
        }

        public bool IsPropertyNameEnd
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsPropertyNameEnd; }
        }

        public bool IsDeleted
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsPropertyDeleted; }
        }

        public bool IsPropertyValueQuoted
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].IsPropertyValueQuoted; }
        }

        public CssNameIndex NameId
        {
            get { this.AssertCurrent(); return this.token.propertyList[this.token.currentProperty].nameId; }
        }

        public char QuoteChar
        {
            get { this.AssertCurrent(); return (char)this.token.propertyList[this.token.currentProperty].quoteChar; }
        }

        public bool HasNameFragment
        {
            get { this.AssertCurrent(); return !this.token.propertyList[this.token.currentProperty].name.IsEmpty; }
        }

        public CssToken.PropertyNameTextReader Name
        {
            get { this.AssertCurrent(); return new CssToken.PropertyNameTextReader(this.token); }
        }

        public bool HasValueFragment
        {
            get { this.AssertCurrent(); return !this.token.propertyList[this.token.currentProperty].value.IsEmpty; }
        }

        public CssToken.PropertyValueTextReader Value
        {
            get { this.AssertCurrent(); return new CssToken.PropertyValueTextReader(this.token); }
        }

        public void SetMinorPart(CssToken.PropertyPartMinor newMinorPart)
        {
            this.AssertCurrent();
            this.token.propertyList[this.token.currentProperty].MinorPart = newMinorPart;
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(this.token.currentProperty == this.index);
#endif
        }
    }
}

