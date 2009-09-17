// ***************************************************************
// <copyright file="CssTokenBuilder.cs" company="Microsoft">
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

    
    

    internal class CssTokenBuilder : TokenBuilder
    {
        
        protected const byte BuildStateEndedCss = BuildStateEnded + 1; 
        protected const byte BuildStatePropertyListStarted = 20;

        protected const byte BuildStateBeforeSelector = 23;
        protected const byte BuildStateSelectorName = 24;
        protected const byte BuildStateEndSelectorName = 25;
        protected const byte BuildStateSelectorClass = 26;
        protected const byte BuildStateEndSelectorClass = 27;

        protected const byte BuildStateBeforeProperty = 43;
        
        protected const byte BuildStatePropertyName = 44;
        protected const byte BuildStateEndPropertyName = 45;
        protected const byte BuildStatePropertyValue = 46;
        protected const byte BuildStateEndPropertyValue = 47;

        protected CssToken cssToken;
        protected int maxProperties;
        protected int maxSelectors;

        
        
        
        
        
        
        

        public CssTokenBuilder(char[] buffer, int maxProperties, int maxSelectors, int maxRuns, bool testBoundaryConditions) :
            base(new CssToken(), buffer, maxRuns, testBoundaryConditions)
        {
            this.cssToken = (CssToken) base.Token;

            int initialProperties = 16;
            int initialSelectors = 16;

            if (!testBoundaryConditions)
            {
                this.maxProperties = maxProperties;
                this.maxSelectors = maxSelectors;
            }
            else
            {
                initialProperties = 1;
                initialSelectors = 1;
                this.maxProperties = 5;
                this.maxSelectors = 5;
            }

            

            this.cssToken.propertyList = new CssToken.PropertyEntry[initialProperties];
            this.cssToken.selectorList = new CssToken.SelectorEntry[initialSelectors];
        }

        
        

        public new CssToken Token
        {
            get { return this.cssToken; }
        }

        

        public bool Incomplete
        {
            get { return this.state >= FirstStarted && this.state != BuildStateText; }
        }

        

        public override void Reset()
        {
            if (this.state >= BuildStateEndedCss)
            {
                this.cssToken.Reset();
            }

            base.Reset();
        }

        

        public void StartRuleSet(int baseOffset, CssTokenId id)
        {
            this.state = BuildStateBeforeSelector;

            this.cssToken.tokenId = (TokenId)id;
            this.cssToken.whole.headOffset = baseOffset;
            this.tailOffset = baseOffset;
        }

        

        public void EndRuleSet()
        {
            if (this.state >= BuildStateBeforeProperty)
            {
                this.EndDeclarations();
            }

            this.tokenValid = true;
            this.state = BuildStateEndedCss;
            this.token.wholePosition.Rewind(this.token.whole);
        }

        

        public void BuildUniversalSelector()
        {
            this.StartSelectorName();
            this.EndSelectorName(0);
        }

        

        public bool CanAddSelector()
        {
            InternalDebug.Assert(this.state == BuildStateBeforeSelector ||
                this.state == BuildStateEndSelectorName ||
                this.state == BuildStateEndSelectorClass );

            return this.cssToken.selectorTail - this.cssToken.selectorHead < this.maxSelectors;
        }

        

        public void StartSelectorName()
        {
            if (this.cssToken.selectorTail == this.cssToken.selectorList.Length)
            {
                

                
                InternalDebug.Assert(this.cssToken.selectorList.Length < this.maxSelectors);

                int newSize;

                if (this.maxSelectors / 2 > this.cssToken.selectorList.Length)
                {
                    newSize = this.cssToken.selectorList.Length * 2;
                }
                else
                {
                    newSize = this.maxSelectors;
                }

                CssToken.SelectorEntry[] newSelectors = new CssToken.SelectorEntry[newSize];

                Array.Copy(this.cssToken.selectorList, 0, newSelectors, 0, this.cssToken.selectorTail);

                this.cssToken.selectorList = newSelectors;
            }

            InternalDebug.Assert(this.state == BuildStateBeforeSelector ||
                this.state == BuildStateEndSelectorName ||
                this.state == BuildStateEndSelectorClass );

            this.cssToken.selectorList[this.cssToken.selectorTail].nameId = HtmlNameIndex.Unknown;
            this.cssToken.selectorList[this.cssToken.selectorTail].name.Initialize(this.cssToken.whole.tail, this.tailOffset);
            this.cssToken.selectorList[this.cssToken.selectorTail].className.Reset();

            this.state = BuildStateSelectorName;
        }

        

        public void EndSelectorName(int nameLength)
        {
            InternalDebug.Assert(this.state == BuildStateSelectorName);

            this.cssToken.selectorList[this.cssToken.selectorTail].name.tail = this.cssToken.whole.tail;

            
            this.cssToken.selectorList[this.cssToken.selectorTail].nameId = this.LookupTagName(nameLength, this.cssToken.selectorList[this.cssToken.selectorTail].name);

            this.state = BuildStateEndSelectorName;
        }

        

        public void StartSelectorClass(CssSelectorClassType classType)
        {
            InternalDebug.Assert(this.state == BuildStateEndSelectorName);

            this.cssToken.selectorList[this.cssToken.selectorTail].className.Initialize(this.cssToken.whole.tail, this.tailOffset);
            this.cssToken.selectorList[this.cssToken.selectorTail].classType = classType;

            this.state = BuildStateSelectorClass;
        }

        

        public void EndSelectorClass()
        {
            InternalDebug.Assert(this.state == BuildStateSelectorClass);

            this.cssToken.selectorList[this.cssToken.selectorTail].className.tail = this.cssToken.whole.tail;

            this.state = BuildStateEndSelectorClass;
        }

        

        public void SetSelectorCombinator(CssSelectorCombinator combinator, bool previous)
        {
            InternalDebug.Assert(this.state == BuildStateEndSelectorName || this.state == BuildStateEndSelectorClass);

            int index = this.cssToken.selectorTail;
            if (previous)
            {
                
                index--;
            }

            InternalDebug.Assert(index >= 0);

            this.cssToken.selectorList[index].combinator = combinator;
        }

        

        public void EndSimpleSelector()
        {
            InternalDebug.Assert(
                this.state == BuildStateEndSelectorName ||
                this.state == BuildStateSelectorClass ||
                this.state == BuildStateEndSelectorClass);

            this.cssToken.selectorTail ++;
        }

        

        public void StartDeclarations(int baseOffset)
        {
            InternalDebug.Assert((this.state == BuildStateInitialized && this.cssToken.IsEmpty) ||
                this.state == BuildStateEndSelectorName ||
                this.state == BuildStateSelectorClass ||
                this.state == BuildStateEndSelectorClass);

            this.state = BuildStateBeforeProperty;

            if (this.cssToken.tokenId == (TokenId)CssTokenId.None)
            {
                this.cssToken.tokenId = (TokenId)CssTokenId.Declarations;
            }
            this.cssToken.partMajor = CssToken.PropertyListPartMajor.Begin;
            this.cssToken.partMinor = CssToken.PropertyListPartMinor.Empty;

            this.cssToken.whole.headOffset = baseOffset;
            this.tailOffset = baseOffset;
        }

        

        public bool CanAddProperty()
        {
            InternalDebug.Assert(this.state == BuildStateBeforeProperty);

            return this.cssToken.propertyTail - this.cssToken.propertyHead < this.maxProperties;
        }

        

        public void StartPropertyName()
        {
            if (this.cssToken.propertyTail == this.cssToken.propertyList.Length)
            {
                

                
                InternalDebug.Assert(this.cssToken.propertyList.Length < this.maxProperties);

                int newSize;

                if (this.maxProperties / 2 > this.cssToken.propertyList.Length)
                {
                    newSize = this.cssToken.propertyList.Length * 2;
                }
                else
                {
                    newSize = this.maxProperties;
                }

                CssToken.PropertyEntry[] newProperties = new CssToken.PropertyEntry[newSize];

                Array.Copy(this.cssToken.propertyList, 0, newProperties, 0, this.cssToken.propertyTail);

                this.cssToken.propertyList = newProperties;
            }

            InternalDebug.Assert(this.cssToken.propertyTail < this.cssToken.propertyList.Length);
            InternalDebug.Assert(this.state == BuildStateBeforeProperty);

            InternalDebug.Assert(this.cssToken.partMinor == CssToken.PropertyListPartMinor.Empty ||
                                this.cssToken.partMinor == CssToken.PropertyListPartMinor.EndProperty ||
                                this.cssToken.partMinor == CssToken.PropertyListPartMinor.EndPropertyWithOtherProperties ||
                                this.cssToken.partMinor == CssToken.PropertyListPartMinor.Properties);

            if (this.cssToken.partMinor == CssToken.PropertyListPartMinor.Empty)
            {
                this.cssToken.partMinor = CssToken.PropertyListPartMinor.BeginProperty;
            }

            this.cssToken.propertyList[this.cssToken.propertyTail].nameId = CssNameIndex.Unknown;
            this.cssToken.propertyList[this.cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.Begin;
            this.cssToken.propertyList[this.cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.BeginName;
            this.cssToken.propertyList[this.cssToken.propertyTail].quoteChar = 0;
            this.cssToken.propertyList[this.cssToken.propertyTail].name.Initialize(this.cssToken.whole.tail, this.tailOffset);
            this.cssToken.propertyList[this.cssToken.propertyTail].value.Reset();

            this.state = BuildStatePropertyName;
        }

        

        public void EndPropertyName(int nameLength)
        {
            InternalDebug.Assert(this.state == BuildStatePropertyName);

            this.cssToken.propertyList[this.cssToken.propertyTail].name.tail = this.cssToken.whole.tail;

            this.cssToken.propertyList[this.cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.EndName;

            if (this.cssToken.propertyList[this.cssToken.propertyTail].IsPropertyBegin)
            {
                this.cssToken.propertyList[this.cssToken.propertyTail].nameId = this.LookupName(nameLength, this.cssToken.propertyList[this.cssToken.propertyTail].name);
            }

            this.state = BuildStateEndPropertyName;
        }

        

        public void StartPropertyValue()
        {
            InternalDebug.Assert(this.state == BuildStateEndPropertyName);

            this.cssToken.propertyList[this.cssToken.propertyTail].value.Initialize(this.cssToken.whole.tail, this.tailOffset);

            this.cssToken.propertyList[this.cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.BeginValue;

            this.state = BuildStatePropertyValue;
        }

        

        public void SetPropertyValueQuote(char ch)
        {
            InternalDebug.Assert(this.state == BuildStatePropertyValue);
            InternalDebug.Assert(ParseSupport.QuoteCharacter(ParseSupport.GetCharClass(ch)));

            this.cssToken.propertyList[this.cssToken.propertyTail].IsPropertyValueQuoted = true;
            this.cssToken.propertyList[this.cssToken.propertyTail].quoteChar = (byte) ch;
        }

        

        public void EndPropertyValue()
        {
            InternalDebug.Assert(this.state == BuildStatePropertyValue);

            this.cssToken.propertyList[this.cssToken.propertyTail].value.tail = this.cssToken.whole.tail;

            this.cssToken.propertyList[this.cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.EndValue;

            this.state = BuildStateEndPropertyValue;
        }

        

        public void EndProperty()
        {
            InternalDebug.Assert(this.state == BuildStateEndPropertyName || this.state == BuildStateEndPropertyValue);

            this.cssToken.propertyList[this.cssToken.propertyTail].partMajor |= CssToken.PropertyPartMajor.End;

            this.cssToken.propertyTail ++;

            if (this.cssToken.propertyTail < this.cssToken.propertyList.Length)
            {
                

                this.cssToken.propertyList[this.cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.None;
                this.cssToken.propertyList[this.cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.Empty;
            }

            if (this.cssToken.partMinor == CssToken.PropertyListPartMinor.BeginProperty)
            {
                this.cssToken.partMinor = CssToken.PropertyListPartMinor.Properties;
            }
            else if (this.cssToken.partMinor == CssToken.PropertyListPartMinor.ContinueProperty)
            {
                this.cssToken.partMinor = CssToken.PropertyListPartMinor.EndProperty;
            }
            else
            {
                InternalDebug.Assert(this.cssToken.partMinor == CssToken.PropertyListPartMinor.EndProperty ||
                                    this.cssToken.partMinor == CssToken.PropertyListPartMinor.EndPropertyWithOtherProperties ||
                                    this.cssToken.partMinor == CssToken.PropertyListPartMinor.Properties);

                this.cssToken.partMinor |= CssToken.PropertyListPartMinor.Properties;
            }

            this.state = BuildStateBeforeProperty;
        }

        

        public void EndDeclarations()
        {
            if (this.state != BuildStatePropertyListStarted)
            {
                

                if (this.state == BuildStatePropertyName)
                {
                    this.cssToken.propertyList[this.cssToken.propertyTail].name.tail = this.cssToken.whole.tail;
                }
                else if (this.state == BuildStatePropertyValue)
                {
                    this.cssToken.propertyList[this.cssToken.propertyTail].value.tail = this.cssToken.whole.tail;
                }

            }

            if (this.state == BuildStatePropertyName)
            {
                this.EndPropertyName(0);
            }
            else if (this.state == BuildStatePropertyValue)
            {
                this.EndPropertyValue();
            }

            if (this.state == BuildStateEndPropertyName || this.state == BuildStateEndPropertyValue)
            {
                this.EndProperty();
            }

            InternalDebug.Assert(this.state == BuildStateBeforeProperty || this.state == BuildStatePropertyListStarted);

            this.state = BuildStateBeforeProperty;
            this.cssToken.partMajor |= CssToken.PropertyListPartMajor.End;

            this.tokenValid = true;
        }

        

        public bool PrepareAndAddRun(CssRunKind cssRunKind, int start, int end)
        {
            if (end != start)
            {
                if (!this.PrepareToAddMoreRuns(1))
                {
                    return false;
                }

                base.AddRun(
                    cssRunKind == CssRunKind.Invalid ? RunType.Invalid : RunType.Normal,
                    cssRunKind == CssRunKind.Space ? RunTextType.Space : RunTextType.NonSpace,
                    (uint)cssRunKind,
                    start,
                    end,
                    0);
            }

            return true;
        }

        

        public bool PrepareAndAddInvalidRun(CssRunKind cssRunKind, int end)
        {
            if (!this.PrepareToAddMoreRuns(1))
            {
                return false;
            }

            base.AddInvalidRun(end, (uint)cssRunKind);

            return true;
        }

        

        public bool PrepareAndAddLiteralRun(CssRunKind cssRunKind, int start, int end, int value)
        {
            if (end != start)
            {
                if (!this.PrepareToAddMoreRuns(1))
                {
                    return false;
                }

                base.AddRun(
                    RunType.Literal,
                    RunTextType.NonSpace,
                    (uint)cssRunKind,
                    start,
                    end,
                    value);
            }

            return true;
        }

        

        public void InvalidateLastValidRun(CssRunKind kind)
        {
            int tail = this.token.whole.tail;
            InternalDebug.Assert(tail > 0);

            do
            {
                tail--;

                CssToken.RunEntry tailEntry = this.token.runList[tail];
                if (tailEntry.Type != RunType.Invalid)
                {
                    if ((uint)kind == (uint)tailEntry.Kind)
                    {
                        this.token.runList[tail].Initialize(RunType.Invalid, tailEntry.TextType, tailEntry.Kind, tailEntry.Length, tailEntry.Value);
                    }
                    break;
                }
            }
            while (tail > 0);
        }

        

        public void MarkPropertyAsDeleted()
        {
            this.cssToken.propertyList[this.cssToken.propertyTail].IsPropertyDeleted = true;
        }

        

        public CssTokenId MakeEmptyToken(CssTokenId tokenId)
        {
            return (CssTokenId)base.MakeEmptyToken((TokenId)tokenId);
        }

        

        public CssNameIndex LookupName(int nameLength, CssToken.Fragment fragment)
        {
            if (nameLength > CssData.MAX_NAME)
            {
                
                return CssNameIndex.Unknown;
            }

            short nameHashValue = (short)(((uint)this.token.CalculateHashLowerCase(fragment) ^ CssData.NAME_HASH_MODIFIER) % CssData.NAME_HASH_SIZE);

            

            int nameIndex = (int) CssData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    string name = CssData.names[nameIndex].name;

                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(this.token.buffer[fragment.headOffset]))
                            {
                                if (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1))
                                {
                                    return (CssNameIndex)nameIndex;
                                }
                            }
                        }
                        else if (this.token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            return (CssNameIndex)nameIndex;
                        }
                    }

                    nameIndex ++;

                    
                    
                }
                while (CssData.names[nameIndex].hash == nameHashValue);
            }

            return CssNameIndex.Unknown;
        }

        

        public HtmlNameIndex LookupTagName(int nameLength, CssToken.Fragment fragment)
        {
            if (nameLength > HtmlNameData.MAX_NAME)
            {
                
                return HtmlNameIndex.Unknown;
            }

            short nameHashValue = (short)(((uint)this.token.CalculateHashLowerCase(fragment) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

            

            int nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    string name = HtmlNameData.names[nameIndex].name;

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
                while (HtmlNameData.names[nameIndex].hash == nameHashValue);
            }

            return HtmlNameIndex.Unknown;
        }
    }
}

