// ***************************************************************
// <copyright file="HtmlToken.cs" company="Microsoft">
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

    

    internal enum HtmlTokenId : byte
    {
        None = TokenId.None,
        EndOfFile = TokenId.EndOfFile,
        Text = TokenId.Text,
        EncodingChange = TokenId.EncodingChange,

        Tag = TokenId.EncodingChange + 1,

        Restart,
        OverlappedClose,                
        OverlappedReopen,               

        InjectionBegin,
        InjectionEnd,
    }

    

    internal enum HtmlLexicalUnit : uint
    {
        Invalid = RunKind.Invalid,
        Text = RunKind.Text,

        TagPrefix = (2 << 26),                      
        TagSuffix = (3 << 26),                      
        Name = (4 << 26),
        TagWhitespace = (5 << 26),                  
        AttrEqual = (6 << 26),
        AttrQuote = (7 << 26),
        AttrValue = (8 << 26),
        TagText = (9 << 26),                        
    }

    

    internal enum HtmlRunKind : uint
    {
        Invalid = RunKind.Invalid,
        Text = RunKind.Text,

        TagPrefix = HtmlLexicalUnit.TagPrefix,
        TagSuffix = HtmlLexicalUnit.TagSuffix,
        Name = HtmlLexicalUnit.Name,
        NamePrefixDelimiter = HtmlLexicalUnit.Name + (1 << 24),     
        TagWhitespace = HtmlLexicalUnit.TagWhitespace,
        AttrEqual = HtmlLexicalUnit.AttrEqual,
        AttrQuote = HtmlLexicalUnit.AttrQuote,
        AttrValue = HtmlLexicalUnit.AttrValue,
        TagText = HtmlLexicalUnit.TagText,                          
    }

    

    internal class HtmlToken : Token
    {
        protected internal HtmlTagIndex tagIndex;
        protected internal HtmlTagIndex originalTagIndex;
        protected internal HtmlNameIndex nameIndex;                  

        protected internal TagFlags flags;                           
        protected internal TagPartMajor partMajor;                   
        protected internal TagPartMinor partMinor;                   

        protected internal LexicalUnit unstructured;                 
        protected internal FragmentPosition unstructuredPosition;    

        protected internal LexicalUnit name;
        protected internal LexicalUnit localName;
        protected internal FragmentPosition namePosition;            

        protected internal AttributeEntry[] attributeList;           
        protected internal int attributeTail;                        

        protected internal int currentAttribute;                     
        protected internal FragmentPosition attrNamePosition;        
        protected internal FragmentPosition attrValuePosition;       

        

        public HtmlToken()
        {
            this.Reset();
        }

        

        [Flags]
        public enum TagFlags : byte
        {
            None = 0,

            
            EmptyTagName = 0x08,

            EndTag = 0x10,
            EmptyScope = 0x20,

            AllowWspLeft = 0x40,
            AllowWspRight = 0x80,
        }

        

        public enum TagPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,
        }

        

        public enum TagPartMinor : byte
        {
            Empty = 0,

            
            BeginName = 0x01 | ContinueName,
            ContinueName = 0x02,
            EndName = ContinueName | 0x04,
            EndNameWithAttributes = EndName | Attributes,
            CompleteName = BeginName | EndName,
            CompleteNameWithAttributes = CompleteName | Attributes,

            
            BeginAttribute = 0x08 | ContinueAttribute,
            ContinueAttribute = 0x10,
            EndAttribute = ContinueAttribute | 0x20,
            EndAttributeWithOtherAttributes = EndAttribute | Attributes,
            AttributePartMask = BeginAttribute | EndAttribute,

            
            Attributes = 0x80,
        }

        

        public enum AttrPartMajor : byte
        {
            None = 0,
            Begin = TagPartMinor.BeginAttribute,        
            Continue = TagPartMinor.ContinueAttribute,  
            End = TagPartMinor.EndAttribute,            
            Complete = Begin | End,

            
            EmptyName = 0x01,
            ValueQuoted = 0x40,
            Deleted = 0x80,
            MaskOffFlags = Complete,
        }

        

        public enum AttrPartMinor : byte
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

        

        public new HtmlTokenId TokenId
        {
            get { return (HtmlTokenId)base.TokenId; }
            set { base.TokenId = (TokenId)value; }
        }

        public TagFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        public bool IsEndTag
        {
            get { return 0 != (this.flags & TagFlags.EndTag); }
        }

        public bool IsEmptyScope
        {
            get { return 0 != (this.flags & TagFlags.EmptyScope); }
        }

        public TagPartMajor MajorPart
        {
            get { return this.partMajor; }
        }

        public TagPartMinor MinorPart
        {
            get { return this.partMinor; }
        }

        public bool IsTagComplete
        {
            get { return this.partMajor == TagPartMajor.Complete; }
        }

        public bool IsTagBegin 
        { 
            get { return (this.partMajor & TagPartMajor.Begin) == TagPartMajor.Begin; }
        }

        public bool IsTagEnd
        {
            get { return (this.partMajor & TagPartMajor.End) == TagPartMajor.End; }
        }

        public bool IsTagNameEmpty
        {
            get { return 0 != (this.flags & TagFlags.EmptyTagName); }
        }

        public bool IsTagNameBegin
        {
            get { return (this.partMinor & TagPartMinor.BeginName) == TagPartMinor.BeginName; }
        }

        public bool IsTagNameEnd
        {
            get { return (this.partMinor & TagPartMinor.EndName) == TagPartMinor.EndName; }
        }

        public bool HasNameFragment
        {
            get { return !this.IsFragmentEmpty(this.name); }
        }
#if false
        public bool HasUnstructuredContentFragment
        {
            get { return !this.IsFragmentEmpty(this.unstructured); }
        }
#endif
        public HtmlNameIndex NameIndex
        {
            get { return this.nameIndex; }
        }

        public TagNameTextReader Name
        {
            get { return new TagNameTextReader(this); }
        }

        public TagUnstructuredContentTextReader UnstructuredContent
        {
            get { return new TagUnstructuredContentTextReader(this); }
        }

        public HtmlTagIndex TagIndex
        {
            get { return this.tagIndex; }
        }

        public HtmlTagIndex OriginalTagId
        {
            get { return this.originalTagIndex; }
        }

        public bool IsAllowWspLeft
        {
            get { return (this.flags & TagFlags.AllowWspLeft) == TagFlags.AllowWspLeft; }
        }

        public bool IsAllowWspRight
        {
            get { return (this.flags & TagFlags.AllowWspRight) == TagFlags.AllowWspRight; }
        }

        public AttributeEnumerator Attributes
        {
            get { return new AttributeEnumerator(this); }
        }

        internal new void Reset()
        {
            this.tagIndex = this.originalTagIndex = HtmlTagIndex._NULL;
            this.nameIndex = HtmlNameIndex._NOTANAME;
            this.flags = HtmlToken.TagFlags.None;
            this.partMajor = HtmlToken.TagPartMajor.None;
            this.partMinor = HtmlToken.TagPartMinor.Empty;

            this.name.Reset();
            this.unstructured.Reset();

            
            this.namePosition.Reset();
            this.unstructuredPosition.Reset();

            this.attributeTail = 0;
            this.currentAttribute = -1;

            
            this.attrNamePosition.Reset();
            this.attrValuePosition.Reset();
        }

        

        public struct AttributeEnumerator
        {
            private HtmlToken token;
#if DEBUG
            private int index;
#endif
            

            internal AttributeEnumerator(HtmlToken token)
            {
                this.token = token;
#if DEBUG
                this.index = this.token.currentAttribute;
#endif
            }

            

            public int Count
            {
                get
                {
                    return this.token.attributeTail;
                }
            }

            public HtmlAttribute Current
            {
                get
                { 
                    InternalDebug.Assert(this.token.currentAttribute >= 0 && this.token.currentAttribute < this.token.attributeTail);
                    this.AssertCurrent();

                    return new HtmlAttribute(this.token);
                }
            }

            public int CurrentIndex
            {
                get { return this.token.currentAttribute; }
            }

            public HtmlAttribute this[int i]
            {
                get
                { 
                    InternalDebug.Assert(i >= 0 && i < this.token.attributeTail);

                    if (i != this.token.currentAttribute)
                    {
                        this.token.attrNamePosition.Rewind(this.token.attributeList[i].name);
                        this.token.attrValuePosition.Rewind(this.token.attributeList[i].value);
                    }

                    this.token.currentAttribute = i;

                    return new HtmlAttribute(this.token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(this.token.currentAttribute >= -1 && this.token.currentAttribute <= this.token.attributeTail);
                this.AssertCurrent();

                if (this.token.currentAttribute != this.token.attributeTail)
                {
                    this.token.currentAttribute ++;

                    if (this.token.currentAttribute != this.token.attributeTail)
                    {
                        this.token.attrNamePosition.Rewind(this.token.attributeList[this.token.currentAttribute].name);
                        this.token.attrValuePosition.Rewind(this.token.attributeList[this.token.currentAttribute].value);
                    }
#if DEBUG
                    this.index = this.token.currentAttribute;
#endif
                }

                return (this.token.currentAttribute != this.token.attributeTail);
            }

            public void Rewind()
            {
                this.AssertCurrent();

                this.token.currentAttribute = -1;
#if DEBUG
                this.index = this.token.currentAttribute;
#endif
            }

            public AttributeEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(HtmlNameIndex nameIndex)
            {
                for (int i = 0; i < this.token.attributeTail; i++)
                {
                    if (this.token.attributeList[i].nameIndex == nameIndex)
                    {
                        this.token.currentAttribute = i;

                        this.token.attrNamePosition.Rewind(this.token.attributeList[i].name);
                        this.token.attrValuePosition.Rewind(this.token.attributeList[i].value);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.token.currentAttribute == this.index);
#endif
            }
        }

        

        public struct TagUnstructuredContentTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TagUnstructuredContentTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                this.position = this.token.unstructuredPosition;
#endif
            }
#if false
            public int Length
            {
                get { return this.token.GetLength(this.token.unstructured); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(this.token.unstructured, ref this.token.unstructuredPosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.unstructuredPosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.unstructuredPosition.Rewind(this.token.unstructured);
            }
#endif
            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(this.token.unstructured, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(this.token.unstructured, maxSize);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.unstructuredPosition));
#endif
            }
        }

        

        public struct TagNameTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TagNameTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                this.position = this.token.namePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(this.token.name); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(this.token.name, ref this.token.namePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.namePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.namePosition.Rewind(this.token.name);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(this.token.name, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(this.token.name, maxSize);
            }

            public void MakeEmpty()
            {
                this.token.name.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.namePosition));
#endif
            }
        }

        

        public struct AttributeNameTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal AttributeNameTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                this.position = this.token.attrNamePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(this.token.attributeList[this.token.currentAttribute].name); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(this.token.attributeList[this.token.currentAttribute].name, ref this.token.attrNamePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.attrNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.attrNamePosition.Rewind(this.token.attributeList[this.token.currentAttribute].name);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(this.token.attributeList[this.token.currentAttribute].name, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(this.token.attributeList[this.token.currentAttribute].name, maxSize);
            }

            public void MakeEmpty()
            {
                this.token.attributeList[this.token.currentAttribute].name.Reset();
                this.token.attrNamePosition.Rewind(this.token.attributeList[this.token.currentAttribute].name);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.attrNamePosition));
#endif
            }
        }

        

        public struct AttributeValueTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal AttributeValueTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                this.position = this.token.attrValuePosition;
#endif
            }

            public int Length
            {
                get { return this.token.GetLength(this.token.attributeList[this.token.currentAttribute].value); }
            }

            public bool IsEmpty
            {
                get { return this.token.IsFragmentEmpty(this.token.attributeList[this.token.currentAttribute].value); }
            }

            public bool IsContiguous
            {
                get { return this.token.IsContiguous(this.token.attributeList[this.token.currentAttribute].value); }
            }

            public BufferString ContiguousBufferString
            {
                get
                {
                    InternalDebug.Assert(!this.IsEmpty && this.IsContiguous);
                    return new BufferString(
                                this.token.buffer,
                                this.token.attributeList[this.token.currentAttribute].value.headOffset,
                                this.token.runList[this.token.attributeList[this.token.currentAttribute].value.head].Length);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(this.token.attributeList[this.token.currentAttribute].value, ref this.token.attrValuePosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.attrValuePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.attrValuePosition.Rewind(this.token.attributeList[this.token.currentAttribute].value);
            }

            public void WriteTo(ITextSink sink)
            {
                this.token.WriteTo(this.token.attributeList[this.token.currentAttribute].value, sink);
            }

            public string GetString(int maxSize)
            {
                return this.token.GetString(this.token.attributeList[this.token.currentAttribute].value, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return this.token.CaseInsensitiveCompareEqual(this.token.attributeList[this.token.currentAttribute].value, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return this.token.CaseInsensitiveContainsSubstring(this.token.attributeList[this.token.currentAttribute].value, str);
            }

            
            public bool SkipLeadingWhitespace()
            {
                return this.token.SkipLeadingWhitespace(this.token.attributeList[this.token.currentAttribute].value, ref this.token.attrValuePosition);
            }

            public void MakeEmpty()
            {
                this.token.attributeList[this.token.currentAttribute].value.Reset();
                this.Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(this.position.SameAs(this.token.attrValuePosition));
#endif
            }
        }

        

        protected internal struct AttributeEntry
        {
            public HtmlNameIndex nameIndex;
            public byte quoteChar;              
            public AttrPartMajor partMajor;
            public AttrPartMinor partMinor;

            public LexicalUnit name;
            public LexicalUnit localName;
            public LexicalUnit value;

            

            public bool IsCompleteAttr
            {
                get { return this.MajorPart == AttrPartMajor.Complete; }
            }

            public bool IsAttrBegin
            {
                get { return (this.partMajor & AttrPartMajor.Begin) == AttrPartMajor.Begin; }
            }

            public bool IsAttrEnd
            {
                get { return (this.partMajor & AttrPartMajor.End) == AttrPartMajor.End; }
            }

            public bool IsAttrEmptyName
            {
                get { return (this.partMajor & AttrPartMajor.EmptyName) == AttrPartMajor.EmptyName; }
            }

            public bool IsAttrNameEnd
            {
                get { return (this.partMinor & AttrPartMinor.EndName) == AttrPartMinor.EndName; }
            }

            public bool IsAttrValueBegin
            {
                get { return (this.partMinor & AttrPartMinor.BeginValue) == AttrPartMinor.BeginValue; }
            }

            public AttrPartMajor MajorPart
            {
                get { return this.partMajor & AttrPartMajor.MaskOffFlags; }
            }

            public AttrPartMinor MinorPart
            {
                get { return this.partMinor; }
                set { this.partMinor = value; }
            }

            public bool IsAttrValueQuoted
            {
                get { return (this.partMajor & AttrPartMajor.ValueQuoted) == AttrPartMajor.ValueQuoted; }
                set { this.partMajor = value ? (this.partMajor | AttrPartMajor.ValueQuoted) : (this.partMajor & ~AttrPartMajor.ValueQuoted); }
            }

            public bool IsAttrDeleted
            {
                get { return (this.partMajor & AttrPartMajor.Deleted) == AttrPartMajor.Deleted; }
                set { this.partMajor = value ? (this.partMajor | AttrPartMajor.Deleted) : (this.partMajor & ~AttrPartMajor.Deleted); }
            }
        }
    }

    

    internal struct HtmlAttribute
    {
        private HtmlToken token;
#if DEBUG
        private int index;
#endif
        

        internal HtmlAttribute(HtmlToken token)
        {
            this.token = token;
#if DEBUG
            this.index = this.token.currentAttribute;
#endif
        }

        public bool IsNull
        {
            get { return this.token == null; }
        }

        

        public int Index
        {
            get { this.AssertCurrent(); return this.token.currentAttribute; }
        }

        public HtmlToken.AttrPartMajor MajorPart
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].MajorPart; }
        }

        public HtmlToken.AttrPartMinor MinorPart
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].MinorPart; }
        }

        public bool IsCompleteAttr
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsCompleteAttr; }
        }

        public bool IsAttrBegin
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrBegin; }
        }

        public bool IsAttrEmptyName
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrEmptyName; }
        }

        public bool IsAttrEnd
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrEnd; }
        }

        public bool IsAttrNameEnd
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrNameEnd; }
        }

        public bool IsDeleted
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrDeleted; }
        }

        public bool IsAttrValueBegin
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrValueBegin; }
        }

        public bool IsAttrValueQuoted
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].IsAttrValueQuoted; }
        }

        public HtmlNameIndex NameIndex
        {
            get { this.AssertCurrent(); return this.token.attributeList[this.token.currentAttribute].nameIndex; }
        }

        public char QuoteChar
        {
            get { this.AssertCurrent(); return (char) this.token.attributeList[this.token.currentAttribute].quoteChar; }
        }

        public bool HasNameFragment
        {
            get { this.AssertCurrent(); return !this.token.IsFragmentEmpty(this.token.attributeList[this.token.currentAttribute].name); }
        }

        public HtmlToken.AttributeNameTextReader Name 
        {
            get { this.AssertCurrent(); return new HtmlToken.AttributeNameTextReader(this.token); }
        }

        public bool HasValueFragment
        {
            get { this.AssertCurrent(); return !this.token.IsFragmentEmpty(this.token.attributeList[this.token.currentAttribute].value); }
        }

        public HtmlToken.AttributeValueTextReader Value
        {
            get { this.AssertCurrent(); return new HtmlToken.AttributeValueTextReader(this.token); }
        }

        public void SetMinorPart(HtmlToken.AttrPartMinor newMinorPart)
        {
            this.AssertCurrent(); 
            this.token.attributeList[this.token.currentAttribute].MinorPart = newMinorPart;
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(this.token.currentAttribute == this.index);
#endif
        }
    }
}

