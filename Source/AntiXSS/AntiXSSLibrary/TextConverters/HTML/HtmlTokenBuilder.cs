// ***************************************************************
// <copyright file="HtmlTokenBuilder.cs" company="Microsoft">
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

    

    internal class HtmlTokenBuilder : TokenBuilder
    {
        

        
        protected const byte BuildStateEndedHtml = BuildStateEnded + 1; 
        protected const byte BuildStateTagStarted = 20;
        protected const byte BuildStateTagText = 21;
        protected const byte BuildStateTagName = 22;
        protected const byte BuildStateTagBeforeAttr = 23;
        
        protected const byte BuildStateTagAttrName = 24;
        protected const byte BuildStateTagEndAttrName = 25;
        protected const byte BuildStateTagAttrValue = 26;
        protected const byte BuildStateTagEndAttrValue = 27;

        protected HtmlToken htmlToken;

        protected int maxAttrs;

        protected int numCarryOverRuns;        
        protected int carryOverRunsHeadOffset;
        protected int carryOverRunsLength;

        

        public HtmlTokenBuilder(
                    char[] buffer, 
                    int maxRuns, 
                    int maxAttrs, 
                    bool testBoundaryConditions) :
            base(new HtmlToken(), buffer, maxRuns, testBoundaryConditions)
        {
            this.htmlToken = (HtmlToken) base.Token;

            int initialAttrs = 8;

            if (maxAttrs != 0)
            {
                if (!testBoundaryConditions)
                {
                    this.maxAttrs = maxAttrs;
                }
                else
                {
                    initialAttrs = 1;
                    this.maxAttrs = 5;
                }

                

                this.htmlToken.attributeList = new HtmlToken.AttributeEntry[initialAttrs];
            }

            this.htmlToken.nameIndex = HtmlNameIndex._NOTANAME;
        }

        

        public new HtmlToken Token
        {
            get { return this.htmlToken; }
        }

        

        public bool IncompleteTag
        {
            get { return this.state >= FirstStarted && this.state != BuildStateText; }
        }

        

        public override void Reset()
        {
            if (this.state >= BuildStateEndedHtml)
            {
                this.htmlToken.Reset();

                this.numCarryOverRuns = 0;
            }

            base.Reset();
        }

        

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId);
        }

        

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId, int argument)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId, argument);
        }

        

        public void StartTag(HtmlNameIndex nameIndex, int baseOffset)
        {
            InternalDebug.Assert(this.state == BuildStateInitialized && this.htmlToken.IsEmpty);

            this.state = BuildStateTagStarted;

            this.htmlToken.tokenId = (TokenId)HtmlTokenId.Tag;
            this.htmlToken.partMajor = HtmlToken.TagPartMajor.Begin;
            this.htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
            this.htmlToken.nameIndex = nameIndex;
            this.htmlToken.tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;

            this.htmlToken.whole.headOffset = baseOffset;
            this.tailOffset = baseOffset;
        }

        

        public void AbortConditional(bool comment)
        {
            InternalDebug.Assert(this.htmlToken.nameIndex == HtmlNameIndex._CONDITIONAL);
            InternalDebug.Assert(this.state == BuildStateTagStarted || this.state == BuildStateTagText);

            
            
            
            
            
            

            this.htmlToken.nameIndex = comment ? HtmlNameIndex._COMMENT : HtmlNameIndex._BANG;
        }

        

        public void SetEndTag()
        {
            this.htmlToken.flags |= HtmlToken.TagFlags.EndTag;
        }

        

        public void SetEmptyScope()
        {
            this.htmlToken.flags |= HtmlToken.TagFlags.EmptyScope;
        }

        

        public void StartTagText()
        {
            InternalDebug.Assert(this.state == BuildStateTagStarted);

            this.state = BuildStateTagText;

            this.htmlToken.unstructured.Initialize(this.htmlToken.whole.tail, this.tailOffset);
            this.htmlToken.unstructuredPosition.Rewind(this.htmlToken.unstructured);
        }

        

        public void EndTagText()
        {
            if (this.htmlToken.unstructured.head == this.htmlToken.whole.tail)
            {
                InternalDebug.Assert(this.htmlToken.unstructured.headOffset == this.tailOffset);

                this.AddNullRun(HtmlRunKind.TagText);
            }

            this.state = BuildStateTagStarted;
        }

        

        public void StartTagName()
        {
            InternalDebug.Assert(this.state == BuildStateTagStarted);

            this.state = BuildStateTagName;

            this.htmlToken.partMinor |= HtmlToken.TagPartMinor.BeginName;

            this.htmlToken.name.Initialize(this.htmlToken.whole.tail, this.tailOffset);
            this.htmlToken.localName.Initialize(this.htmlToken.whole.tail, this.tailOffset);
            this.htmlToken.namePosition.Rewind(this.htmlToken.name);
        }

        public void EndTagNamePrefix()
        {
            InternalDebug.Assert(this.state == BuildStateTagName);

            this.htmlToken.localName.Initialize(this.htmlToken.whole.tail, this.tailOffset);
        }

        

        public void EndTagName(int nameLength)
        {
            InternalDebug.Assert(this.state == BuildStateTagName);

            InternalDebug.Assert(this.htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || 
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName);

            if (this.htmlToken.localName.head == this.htmlToken.whole.tail)
            {
                InternalDebug.Assert(this.htmlToken.localName.headOffset == this.tailOffset);

                this.AddNullRun(HtmlRunKind.Name);
                if (this.htmlToken.localName.head == this.htmlToken.name.head)
                {
                    this.htmlToken.flags |= HtmlToken.TagFlags.EmptyTagName;
                }
            }

            this.htmlToken.partMinor |= HtmlToken.TagPartMinor.EndName;

            if (this.htmlToken.IsTagBegin)
            {
                this.AddSentinelRun();
                this.htmlToken.nameIndex = this.LookupName(nameLength, this.htmlToken.name);
                this.htmlToken.tagIndex = this.htmlToken.originalTagIndex = HtmlNameData.names[(int)this.htmlToken.nameIndex].tagIndex;
            }

            this.state = BuildStateTagBeforeAttr;
        }

        

        public void EndTagName(HtmlNameIndex resolvedNameIndex)
        {
            InternalDebug.Assert(this.state == BuildStateTagName);

            InternalDebug.Assert(this.htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || 
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName);

            if (this.htmlToken.localName.head == this.htmlToken.whole.tail)
            {
                InternalDebug.Assert(this.htmlToken.localName.headOffset == this.tailOffset);

                this.AddNullRun(HtmlRunKind.Name);
                if (this.htmlToken.localName.head == this.htmlToken.name.head)
                {
                    this.htmlToken.flags |= HtmlToken.TagFlags.EmptyTagName;
                }
            }

            this.htmlToken.partMinor |= HtmlToken.TagPartMinor.EndName;

            if (this.htmlToken.IsTagBegin)
            {
                this.htmlToken.nameIndex = resolvedNameIndex;
                this.htmlToken.tagIndex = this.htmlToken.originalTagIndex = HtmlNameData.names[(int)resolvedNameIndex].tagIndex;
            }

            this.state = BuildStateTagBeforeAttr;
        }

        

        public bool CanAddAttribute()
        {
            InternalDebug.Assert(this.state == BuildStateTagBeforeAttr);

            return this.htmlToken.attributeTail < this.maxAttrs;
        }

        

        public void StartAttribute()
        {
            if (this.htmlToken.attributeTail == this.htmlToken.attributeList.Length)
            {
                

                
                InternalDebug.Assert(this.htmlToken.attributeList.Length < this.maxAttrs);

                int newSize;

                if (this.maxAttrs / 2 > this.htmlToken.attributeList.Length)
                {
                    newSize = this.htmlToken.attributeList.Length * 2;
                }
                else
                {
                    newSize = this.maxAttrs;
                }

                HtmlToken.AttributeEntry[] newAttrs = new HtmlToken.AttributeEntry[newSize];

                Array.Copy(this.htmlToken.attributeList, 0, newAttrs, 0, this.htmlToken.attributeTail);

                this.htmlToken.attributeList = newAttrs;
            }

            InternalDebug.Assert(this.htmlToken.attributeTail < this.htmlToken.attributeList.Length);
            InternalDebug.Assert(this.state == BuildStateTagBeforeAttr);

            InternalDebug.Assert(this.htmlToken.partMinor == HtmlToken.TagPartMinor.Empty ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                this.htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes);

            if (this.htmlToken.partMinor == HtmlToken.TagPartMinor.Empty)
            {
                this.htmlToken.partMinor = HtmlToken.TagPartMinor.BeginAttribute;
            }

            this.htmlToken.attributeList[this.htmlToken.attributeTail].nameIndex = HtmlNameIndex.Unknown;
            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.Begin;
            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.BeginName;
            this.htmlToken.attributeList[this.htmlToken.attributeTail].quoteChar = 0;
            this.htmlToken.attributeList[this.htmlToken.attributeTail].name.Initialize(this.htmlToken.whole.tail, this.tailOffset);
            this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.Initialize(this.htmlToken.whole.tail, this.tailOffset);
            this.htmlToken.attributeList[this.htmlToken.attributeTail].value.Reset();

            this.state = BuildStateTagAttrName;
        }

        public void EndAttributeNamePrefix()
        {
            InternalDebug.Assert(this.state == BuildStateTagAttrName);

            this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.Initialize(this.htmlToken.whole.tail, this.tailOffset);
        }

        

        public void EndAttributeName(int nameLength)
        {
            InternalDebug.Assert(this.state == BuildStateTagAttrName);

            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.EndName;

            if (this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.head == this.htmlToken.whole.tail)
            {
                InternalDebug.Assert(this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.headOffset == this.tailOffset);

                this.AddNullRun(HtmlRunKind.Name);
                if (this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.head == this.htmlToken.attributeList[this.htmlToken.attributeTail].name.head)
                {
                    this.htmlToken.attributeList[this.htmlToken.attributeTail].partMajor |= HtmlToken.AttrPartMajor.EmptyName;
                }
            }

            if (this.htmlToken.attributeList[this.htmlToken.attributeTail].IsAttrBegin)
            {
                this.AddSentinelRun();
                this.htmlToken.attributeList[this.htmlToken.attributeTail].nameIndex = this.LookupName(nameLength, this.htmlToken.attributeList[this.htmlToken.attributeTail].name);
            }

            this.state = BuildStateTagEndAttrName;
        }

        

        public void StartValue()
        {
            InternalDebug.Assert(this.state == BuildStateTagEndAttrName);

            this.htmlToken.attributeList[this.htmlToken.attributeTail].value.Initialize(this.htmlToken.whole.tail, this.tailOffset);

            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.BeginValue;

            this.state = BuildStateTagAttrValue;
        }

        

        public void SetValueQuote(char ch)
        {
            InternalDebug.Assert(this.state == BuildStateTagAttrValue);
            InternalDebug.Assert(ParseSupport.QuoteCharacter(ParseSupport.GetCharClass(ch)));

            this.htmlToken.attributeList[this.htmlToken.attributeTail].IsAttrValueQuoted = true;
            this.htmlToken.attributeList[this.htmlToken.attributeTail].quoteChar = (byte) ch;
        }

        

        public void EndValue()
        {
            InternalDebug.Assert(this.state == BuildStateTagAttrValue);

            if (this.htmlToken.attributeList[this.htmlToken.attributeTail].value.head == this.htmlToken.whole.tail)
            {
                InternalDebug.Assert(this.htmlToken.attributeList[this.htmlToken.attributeTail].value.headOffset == this.tailOffset);

                this.AddNullRun(HtmlRunKind.AttrValue);
            }

            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.EndValue;

            this.state = BuildStateTagEndAttrValue;
        }

        

        public void EndAttribute()
        {
            InternalDebug.Assert(this.state == BuildStateTagEndAttrName || this.state == BuildStateTagEndAttrValue);

            this.htmlToken.attributeList[this.htmlToken.attributeTail].partMajor |= HtmlToken.AttrPartMajor.End;

            this.htmlToken.attributeTail ++;

            if (this.htmlToken.attributeTail < this.htmlToken.attributeList.Length)
            {
                

                this.htmlToken.attributeList[this.htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.None;
                this.htmlToken.attributeList[this.htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.Empty;
            }

            if (this.htmlToken.partMinor == HtmlToken.TagPartMinor.BeginAttribute)
            {
                this.htmlToken.partMinor = HtmlToken.TagPartMinor.Attributes;
            }
            else if (this.htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueAttribute)
            {
                this.htmlToken.partMinor = HtmlToken.TagPartMinor.EndAttribute;
            }
            else
            {
                InternalDebug.Assert(this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                    this.htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes);

                this.htmlToken.partMinor |= HtmlToken.TagPartMinor.Attributes;
            }

            this.state = BuildStateTagBeforeAttr;
        }

        

        public void EndTag(bool complete)
        {
            if (complete)
            {
                

                if (this.state != BuildStateTagBeforeAttr)
                {
                    if (this.state == BuildStateTagText)
                    {
                        this.EndTagText();
                    }
                    else if (this.state == BuildStateTagName)
                    {
                        this.EndTagName(0);
                    }
                    else
                    {
                        if (this.state == BuildStateTagAttrName)
                        {
                            this.EndAttributeName(0);
                        }
                        else if (this.state == BuildStateTagAttrValue)
                        {
                            this.EndValue();
                        }

                        if (this.state == BuildStateTagEndAttrName || this.state == BuildStateTagEndAttrValue)
                        {
                            this.EndAttribute();
                        }
                    }
                }

                
                this.AddSentinelRun();

                InternalDebug.Assert(this.state == BuildStateTagBeforeAttr || this.state == BuildStateTagStarted);

                this.state = BuildStateEndedHtml;
                this.htmlToken.partMajor |= HtmlToken.TagPartMajor.End;
            }
            else
            {
                if (this.state >= BuildStateTagAttrName)
                {
                    
                    

                    InternalDebug.Assert(!complete);

                    if (0 != this.htmlToken.attributeTail ||
                        this.htmlToken.name.head != -1 ||
                        this.htmlToken.attributeList[this.htmlToken.attributeTail].name.head > 0)
                    {
                        
                        

                        InternalDebug.Assert(this.htmlToken.attributeList[this.htmlToken.attributeTail].IsAttrBegin);

                        
                        this.AddSentinelRun();

                        
                        

                        this.numCarryOverRuns = this.htmlToken.whole.tail - this.htmlToken.attributeList[this.htmlToken.attributeTail].name.head;
                        this.carryOverRunsHeadOffset = this.htmlToken.attributeList[this.htmlToken.attributeTail].name.headOffset;
                        this.carryOverRunsLength = this.tailOffset - this.carryOverRunsHeadOffset;

                        this.htmlToken.whole.tail -= this.numCarryOverRuns;

                        InternalDebug.Assert(this.numCarryOverRuns != 0);

                        
                    }
                    
                    else
                    {
                        
                        

                         if (this.state == BuildStateTagAttrName)
                        {
                            if (this.htmlToken.attributeList[this.htmlToken.attributeTail].name.head == this.htmlToken.whole.tail)
                            {
                                this.AddNullRun(HtmlRunKind.Name);
                            }
                        }
                        else if (this.state == BuildStateTagAttrValue)
                        {
                            if (this.htmlToken.attributeList[this.htmlToken.attributeTail].value.head == this.htmlToken.whole.tail)
                            {
                                this.AddNullRun(HtmlRunKind.AttrValue);
                            }
                        }

                        
                        this.AddSentinelRun();

                        this.htmlToken.attributeTail++;       
                    }
                }
                else
                {
                    if (this.state == BuildStateTagName)
                    {
                        if (this.htmlToken.name.head == this.htmlToken.whole.tail)
                        {
                            
                            this.AddNullRun(HtmlRunKind.Name);
                        }
                    }
                    else if (this.state == BuildStateTagText)
                    {
                        if (this.htmlToken.unstructured.head == this.htmlToken.whole.tail)
                        {
                            this.AddNullRun(HtmlRunKind.TagText);
                        }
                    }

                    
                    this.AddSentinelRun();
                }
            }

            this.tokenValid = true;
        }

        

        public int RewindTag()
        {
            InternalDebug.Assert(this.IncompleteTag);
            InternalDebug.Assert(this.htmlToken.whole.head == 0);
            InternalDebug.Assert(this.numCarryOverRuns == 0 || this.carryOverRunsHeadOffset + this.carryOverRunsLength == this.tailOffset);

            
            
            
            
            
            

            if (this.state >= BuildStateTagAttrName)
            {
                

                if (0 == this.htmlToken.attributeTail ||
                    this.htmlToken.attributeList[this.htmlToken.attributeTail - 1].IsAttrEnd)
                {
                    
                    
                    

                    InternalDebug.Assert(this.numCarryOverRuns != 0);

                    int deltaRuns = this.htmlToken.whole.tail;

                    Array.Copy(this.htmlToken.runList, deltaRuns, this.htmlToken.runList, 0, this.numCarryOverRuns);

                    this.htmlToken.whole.head = 0;
                    this.htmlToken.whole.headOffset = this.carryOverRunsHeadOffset;
                    this.htmlToken.whole.tail = this.numCarryOverRuns;
                    this.numCarryOverRuns = 0;

                    this.htmlToken.attributeList[0] = this.htmlToken.attributeList[this.htmlToken.attributeTail];

                    this.htmlToken.partMinor = (HtmlToken.TagPartMinor)this.htmlToken.attributeList[0].MajorPart;

                    InternalDebug.Assert(this.htmlToken.attributeList[0].IsAttrBegin);

                    

                    if (this.htmlToken.attributeList[0].name.head != -1)
                    {
                        this.htmlToken.attributeList[0].name.head -= deltaRuns;
                    }

                    if (this.htmlToken.attributeList[0].localName.head != -1)
                    {
                        this.htmlToken.attributeList[0].localName.head -= deltaRuns;
                    }

                    if (this.htmlToken.attributeList[0].value.head != -1)
                    {
                        this.htmlToken.attributeList[0].value.head -= deltaRuns;
                    }
                }
                else
                {
                    

                    InternalDebug.Assert(this.numCarryOverRuns == 0);

                    this.htmlToken.whole.Initialize(0, this.tailOffset);

                    this.htmlToken.attributeList[0].nameIndex = this.htmlToken.attributeList[this.htmlToken.attributeTail - 1].nameIndex;

                    this.htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.Continue;

                    HtmlToken.AttrPartMinor oldMinor = this.htmlToken.attributeList[this.htmlToken.attributeTail - 1].partMinor;

                    if (oldMinor == HtmlToken.AttrPartMinor.BeginName || oldMinor == HtmlToken.AttrPartMinor.ContinueName)
                    {
                        this.htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueName;
                    }
                    else if (oldMinor == HtmlToken.AttrPartMinor.EndNameWithBeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.CompleteNameWithBeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.BeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.ContinueValue)
                    {
                        this.htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueValue;
                    }
                    else
                    {
                        InternalDebug.Assert(oldMinor == HtmlToken.AttrPartMinor.EndName || oldMinor == HtmlToken.AttrPartMinor.CompleteName || oldMinor == HtmlToken.AttrPartMinor.Empty);

                        this.htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                    }

                    this.htmlToken.attributeList[0].IsAttrDeleted = false;
                    this.htmlToken.attributeList[0].IsAttrValueQuoted = this.htmlToken.attributeList[this.htmlToken.attributeTail - 1].IsAttrValueQuoted;
                    this.htmlToken.attributeList[0].quoteChar = this.htmlToken.attributeList[this.htmlToken.attributeTail - 1].quoteChar;

                    if (this.state == BuildStateTagAttrName)
                    {
                        this.htmlToken.attributeList[0].name.Initialize(0, this.tailOffset);
                        this.htmlToken.attributeList[0].localName.Initialize(0, this.tailOffset);
                    }
                    else
                    {
                        this.htmlToken.attributeList[0].name.Reset();
                        this.htmlToken.attributeList[0].localName.Reset();
                    }

                    if (this.state == BuildStateTagAttrValue)
                    {
                        this.htmlToken.attributeList[0].value.Initialize(0, this.tailOffset);
                    }
                    else
                    {
                        this.htmlToken.attributeList[0].value.Reset();
                    }

                    this.htmlToken.partMinor = (HtmlToken.TagPartMinor)this.htmlToken.attributeList[0].MajorPart;
                }

                InternalDebug.Assert(!this.htmlToken.attributeList[0].IsAttrEnd);
            }
            else
            {
                InternalDebug.Assert(this.numCarryOverRuns == 0);

                this.htmlToken.whole.Initialize(0, this.tailOffset);

                if (this.htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || this.htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName)
                {
                    this.htmlToken.partMinor = HtmlToken.TagPartMinor.ContinueName;
                }
                else
                {
                    InternalDebug.Assert(this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes ||
                                        this.htmlToken.partMinor == HtmlToken.TagPartMinor.Empty);

                    this.htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
                }

                if (this.htmlToken.attributeList != null)
                {
                    this.htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.None;
                    this.htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                }
            }

            if (this.state == BuildStateTagText)
            {
                this.htmlToken.unstructured.Initialize(0, this.tailOffset);
            }
            else
            {
                this.htmlToken.unstructured.Reset();
            }

            if (this.state == BuildStateTagName)
            {
                this.htmlToken.name.Initialize(0, this.tailOffset);
                this.htmlToken.localName.Initialize(0, this.tailOffset);
            }
            else
            {
                this.htmlToken.name.Reset();
                this.htmlToken.localName.Reset();
            }

            this.htmlToken.attributeTail = 0;
            this.htmlToken.currentAttribute = -1;

            this.htmlToken.partMajor = HtmlToken.TagPartMajor.Continue;

            this.tokenValid = false;

            return this.htmlToken.whole.headOffset;
        }

        
#if false
        public HtmlNameIndex LookupName(int nameLength, ref HtmlToken.Fragment fragment)
        {
            if (nameLength > HtmlData.MAX_NAME || nameHashValue < 0)
            {
                
                return HtmlNameIndex.Unknown;
            }

            

            int nameIndex = (int) HtmlData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    string name = HtmlData.names[nameIndex].name;

                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(this.token.buffer[fragment.headOffset]))
                            {
                                if (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1))
                                {
                                    return (HtmlNameIndex)nameIndex;
                                }
                            }
                        }
                        else if (this.token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            return (HtmlNameIndex)nameIndex;
                        }
                    }

                    nameIndex ++;

                    
                    
                }
                while (HtmlData.names[nameIndex].hash == nameHashValue);
            }

            return HtmlNameIndex.Unknown;
        }
#endif
        public HtmlNameIndex LookupName(int nameLength, HtmlToken.LexicalUnit unit)
        {
            InternalDebug.Assert(nameLength >= 0);

            if (nameLength != 0 && nameLength <= HtmlNameData.MAX_NAME)
            {
                short nameHashValue = (short)(((uint)this.token.CalculateHashLowerCase(unit) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                

                int nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        string name = HtmlNameData.names[nameIndex].name;

                        if (name.Length == nameLength)
                        {
                            if (this.token.IsContiguous(unit))
                            {
                                if (name[0] == ParseSupport.ToLowerCase(this.token.buffer[unit.headOffset]))
                                {
                                    if (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(unit.headOffset + 1, name, 1))
                                    {
                                        return (HtmlNameIndex)nameIndex;
                                    }
                                }
                            }
                            else if (this.token.CaseInsensitiveCompareEqual(unit, name))
                            {
                                return (HtmlNameIndex)nameIndex;
                            }
                        }

                        nameIndex ++;

                        
                        
                    }
                    while (HtmlNameData.names[nameIndex].hash == nameHashValue);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        

        public static HtmlNameIndex LookupName(char[] nameBuffer, int nameOffset, int nameLength)
        {
            InternalDebug.Assert(nameLength >= 0);

            if (nameLength != 0 && nameLength <= HtmlNameData.MAX_NAME)
            {
                

                short nameHashValue = (short)(((uint)HashCode.CalculateLowerCase(nameBuffer, nameOffset, nameLength) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                int nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        string name = HtmlNameData.names[nameIndex].name;

                        if (name.Length == nameLength &&
                            name[0] == ParseSupport.ToLowerCase(nameBuffer[nameOffset]))
                        {
                            int i = 0;

                            while (++i < name.Length)
                            {
                                InternalDebug.Assert(!ParseSupport.IsUpperCase(name[i]));

                                if (ParseSupport.ToLowerCase(nameBuffer[nameOffset + i]) != name[i])
                                {
                                    break;
                                }
                            }

                            if (i == name.Length)
                            {
                                return (HtmlNameIndex)nameIndex;
                            }
                        }

                        nameIndex ++;

                        
                        
                    }
                    while (HtmlNameData.names[nameIndex].hash == nameHashValue);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        

        public bool PrepareToAddMoreRuns(int numRuns, int start, HtmlRunKind skippedRunKind)
        {
            return base.PrepareToAddMoreRuns(numRuns, start, (uint)skippedRunKind);
        }

        

        public void AddInvalidRun(int end, HtmlRunKind kind)
        {
            base.AddInvalidRun(end, (uint)kind);
        }

        public void AddNullRun(HtmlRunKind kind)
        {
            base.AddNullRun((uint)kind);
        }

        

        public void AddRun(RunTextType textType, HtmlRunKind kind, int start, int end)
        {
            base.AddRun(RunType.Normal, textType, (uint)kind, start, end, 0);
        }

        

        public void AddLiteralRun(RunTextType textType, HtmlRunKind kind, int start, int end, int literal)
        {
            base.AddRun(RunType.Literal, textType, (uint)kind, start, end, literal);
        }

        

        protected override void Rebase(int deltaOffset)
        {
            
            
            

            this.htmlToken.unstructured.headOffset += deltaOffset;
            this.htmlToken.unstructuredPosition.runOffset += deltaOffset;

            this.htmlToken.name.headOffset += deltaOffset;
            this.htmlToken.localName.headOffset += deltaOffset;
            this.htmlToken.namePosition.runOffset += deltaOffset;

            for (int i = 0; i < this.htmlToken.attributeTail; i++)
            {
                this.htmlToken.attributeList[i].name.headOffset += deltaOffset;
                this.htmlToken.attributeList[i].localName.headOffset += deltaOffset;
                this.htmlToken.attributeList[i].value.headOffset += deltaOffset;
            }

            if (this.state >= BuildStateTagAttrName)
            {
                this.htmlToken.attributeList[this.htmlToken.attributeTail].name.headOffset += deltaOffset;
                this.htmlToken.attributeList[this.htmlToken.attributeTail].localName.headOffset += deltaOffset;
                this.htmlToken.attributeList[this.htmlToken.attributeTail].value.headOffset += deltaOffset;
            }

            this.htmlToken.attrNamePosition.runOffset += deltaOffset;
            this.htmlToken.attrValuePosition.runOffset += deltaOffset;

            this.carryOverRunsHeadOffset += deltaOffset;

            base.Rebase(deltaOffset);
        }
    }
}

