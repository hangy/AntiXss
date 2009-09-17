// ***************************************************************
// <copyright file="HtmlCallback.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;


    internal struct HtmlTagParts
    {
        private HtmlToken.TagPartMajor major;
        private HtmlToken.TagPartMinor minor;

        internal HtmlTagParts(HtmlToken.TagPartMajor major, HtmlToken.TagPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        internal void Reset()
        {
            this.minor = 0;
            this.major = 0;
        }

        public bool Begin { get { return HtmlToken.TagPartMajor.Begin == (this.major & HtmlToken.TagPartMajor.Begin); } }
        public bool End { get { return HtmlToken.TagPartMajor.End == (this.major & HtmlToken.TagPartMajor.End); } }
        public bool Complete { get { return HtmlToken.TagPartMajor.Complete == this.major; } }

        public bool NameBegin { get { return HtmlToken.TagPartMinor.BeginName == (this.minor & HtmlToken.TagPartMinor.BeginName); } }
        public bool Name { get { return HtmlToken.TagPartMinor.ContinueName == (this.minor & HtmlToken.TagPartMinor.ContinueName); } }
        public bool NameEnd { get { return HtmlToken.TagPartMinor.EndName == (this.minor & HtmlToken.TagPartMinor.EndName); } }
        public bool NameComplete { get { return HtmlToken.TagPartMinor.CompleteName == (this.minor & HtmlToken.TagPartMinor.CompleteName); } }

        public bool Attributes { get { return 0 != (this.minor & (HtmlToken.TagPartMinor.Attributes | HtmlToken.TagPartMinor.ContinueAttribute)); } }

        public override string ToString()
        {
            return this.major.ToString() + " /" + this.minor.ToString() + "/";
        }
    }

    

    internal struct HtmlAttributeParts
    {
        private HtmlToken.AttrPartMajor major;
        private HtmlToken.AttrPartMinor minor;

        internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        public bool Begin { get { return HtmlToken.AttrPartMajor.Begin == (this.major & HtmlToken.AttrPartMajor.Begin); } }
        public bool End { get { return HtmlToken.AttrPartMajor.End == (this.major & HtmlToken.AttrPartMajor.End); } }
        public bool Complete { get { return HtmlToken.AttrPartMajor.Complete == this.major; } }

        public bool NameBegin { get { return HtmlToken.AttrPartMinor.BeginName == (this.minor & HtmlToken.AttrPartMinor.BeginName); } }
        public bool Name { get { return HtmlToken.AttrPartMinor.ContinueName == (this.minor & HtmlToken.AttrPartMinor.ContinueName); } }
        public bool NameEnd { get { return HtmlToken.AttrPartMinor.EndName == (this.minor & HtmlToken.AttrPartMinor.EndName); } }
        public bool NameComplete { get { return HtmlToken.AttrPartMinor.CompleteName == (this.minor & HtmlToken.AttrPartMinor.CompleteName); } }

        public bool ValueBegin { get { return HtmlToken.AttrPartMinor.BeginValue == (this.minor & HtmlToken.AttrPartMinor.BeginValue); } }
        public bool Value { get { return HtmlToken.AttrPartMinor.ContinueValue == (this.minor & HtmlToken.AttrPartMinor.ContinueValue); } }
        public bool ValueEnd { get { return HtmlToken.AttrPartMinor.EndValue == (this.minor & HtmlToken.AttrPartMinor.EndValue); } }
        public bool ValueComplete { get { return HtmlToken.AttrPartMinor.CompleteValue == (this.minor & HtmlToken.AttrPartMinor.CompleteValue); } }

        public override string ToString()
        {
            return this.major.ToString() + " /" + this.minor.ToString() + "/";
        }
    }

    

    
    
    
    
    
    internal delegate void HtmlTagCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter);

    

    
    
    
    internal abstract class HtmlTagContext
    {
        internal enum TagWriteState
        {
            Undefined,
            Written,
            Deleted,
        }

        private TagWriteState writeState;

        private byte cookie;
        private bool valid;

        private bool invokeCallbackForEndTag;
        private bool deleteInnerContent;
        private bool deleteEndTag;

        private bool isEndTag;
        private bool isEmptyElementTag;

        private HtmlNameIndex tagNameIndex;

        private HtmlTagParts tagParts;

        private int attributeCount;

        internal HtmlTagContext()
        {
        }

        
        
        
        public bool IsEndTag       
        {
            get
            {
                this.AssertContextValid();
                return this.isEndTag;
            }
        }

        
        
        
        public bool IsEmptyElementTag   
        {
            get
            {
                this.AssertContextValid();
                return this.isEmptyElementTag;
            }
        }

        
        
        
        public HtmlTagId TagId
        {
            get
            {
                this.AssertContextValid();
                return HtmlNameData.names[(int)this.tagNameIndex].publicTagId;
            }
        }

        
        
        
        public string TagName
        {
            get
            {
                this.AssertContextValid();
                return this.GetTagNameImpl();
            }
        }

        internal HtmlNameIndex TagNameIndex
        {
            get
            {
                this.AssertContextValid();
                return this.tagNameIndex;
            }
        }

        internal HtmlTagParts TagParts
        {
            get
            {
                this.AssertContextValid();
                return this.tagParts;
            }
        }

        
        
        
        public AttributeCollection Attributes
        {
            get
            {
                this.AssertContextValid();
                return new AttributeCollection(this);
            }
        }

        
        

        
        
        
        

        internal bool IsInvokeCallbackForEndTag
        {
            get
            {
                return this.invokeCallbackForEndTag;
            }
        }

        internal bool IsDeleteInnerContent
        {
            get
            {
                return this.deleteInnerContent;
            }
        }

        internal bool IsDeleteEndTag
        {
            get
            {
                return this.deleteEndTag;
            }
        }

        internal bool CopyPending
        {
            get
            {
                this.AssertContextValid();
                return this.GetCopyPendingStateImpl();
            }
        }

        
        
        
        public void WriteTag()
        {
            this.WriteTag(false);
        }

        
        
        
        
        public void WriteTag(bool copyInputAttributes)
        {
            this.AssertContextValid();

            if (this.writeState != TagWriteState.Undefined)
            {
                throw new InvalidOperationException(this.writeState == TagWriteState.Written ? Strings.CallbackTagAlreadyWritten : Strings.CallbackTagAlreadyDeleted);
            }

            this.deleteEndTag = false;

            

            this.WriteTagImpl(!this.isEndTag && copyInputAttributes);

            this.writeState = TagWriteState.Written;
        }

        
        
        
        public void DeleteTag()
        {
            this.DeleteTag(false);
        }

        
        
        
        
        public void DeleteTag(bool keepEndTag)
        {
            this.AssertContextValid();

            if (this.writeState != TagWriteState.Undefined)
            {
                throw new InvalidOperationException(this.writeState == TagWriteState.Written ? Strings.CallbackTagAlreadyWritten : Strings.CallbackTagAlreadyDeleted);
            }

            if (!this.isEndTag && !this.isEmptyElementTag)
            {
                this.deleteEndTag = !keepEndTag;
            }
            else
            {
                this.deleteEndTag = false;
            }

            this.DeleteTagImpl();

            this.writeState = TagWriteState.Deleted;
        }

        
        
        
        public void DeleteInnerContent()
        {
            this.AssertContextValid();

            if (!this.isEndTag && !this.isEmptyElementTag)
            {
                this.deleteInnerContent = true;
            }
        }

        
        
        
        public void InvokeCallbackForEndTag()
        {
            this.AssertContextValid();

            if (!this.isEndTag && !this.isEmptyElementTag)
            {
                this.invokeCallbackForEndTag = true;
            }
        }

        
        

        internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
        {
            this.isEndTag = isEndTag;
            this.isEmptyElementTag = false;
            this.tagNameIndex = tagNameIndex;
            this.writeState = droppedEndTag ? TagWriteState.Deleted : TagWriteState.Undefined;

            
            this.invokeCallbackForEndTag = false;
            this.deleteInnerContent = false;
            this.deleteEndTag = !this.isEndTag;

            this.cookie = unchecked((byte)(this.cookie + 1));       
        }

        internal void InitializeFragment(bool isEmptyElementTag, int attributeCount, HtmlTagParts tagParts)
        {
            if (attributeCount >= 0x00FFFFFF)
            {
                
                throw new TextConvertersException();
            }

            this.isEmptyElementTag = isEmptyElementTag;
            this.tagParts = tagParts;
            this.attributeCount = attributeCount;

            this.cookie = unchecked((byte)(this.cookie + 1));       
            this.valid = true;
        }

        internal void UninitializeFragment()
        {
            this.valid = false;
        }

        
        

        internal virtual bool GetCopyPendingStateImpl()
        {
            return false;
        }

        internal abstract string GetTagNameImpl();

        internal abstract HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex);

        internal abstract HtmlAttributeParts GetAttributePartsImpl(int attributeIndex);

        internal abstract string GetAttributeNameImpl(int attributeIndex);

        

        internal abstract string GetAttributeValueImpl(int attributeIndex);

        internal abstract int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count);

        internal abstract void WriteTagImpl(bool writeAttributes);

        internal virtual void DeleteTagImpl()
        {
        }

        internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

        
        

        internal void AssertAttributeValid(int attributeIndexAndCookie)
        {
            
            if (!this.valid)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidInThisState);
            }

            

            if (ExtractCookie(attributeIndexAndCookie) != this.cookie)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidForThisContext);
            }

            

            int index = ExtractIndex(attributeIndexAndCookie);

            if (index < 0 || index >= this.attributeCount)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidForThisContext);
            }
        }

        internal void AssertContextValid()
        {
            
            if (!this.valid)
            {
                throw new InvalidOperationException(Strings.ContextNotValidInThisState);
            }
        }

        
        

        internal static byte ExtractCookie(int attributeIndexAndCookie)
        {
            return (byte)((uint)attributeIndexAndCookie >> 24);
        }

        internal static int ExtractIndex(int attributeIndexAndCookie)
        {
            InternalDebug.Assert((attributeIndexAndCookie & 0x00FFFFFF) - 1 >= -1);
            return (attributeIndexAndCookie & 0x00FFFFFF) - 1;
        }

        internal static int ComposeIndexAndCookie(byte cookie, int attributeIndex)
        {
            InternalDebug.Assert(attributeIndex >= -1);
            return ((int)cookie << 24) + (attributeIndex + 1);
        }

        

        
        
        
        public struct AttributeCollection : IEnumerable<HtmlTagContextAttribute>, System.Collections.IEnumerable
        {
            private HtmlTagContext tagContext;

            internal AttributeCollection(HtmlTagContext tagContext)
            {
                this.tagContext = tagContext;
            }

            
            
            
            public int Count
            {
                get
                {
                    this.AssertValid();
                    return this.tagContext.attributeCount;
                }
            }

            
            
            
            
            
            public HtmlTagContextAttribute this[int index]
            {
                get
                {
                    this.AssertValid();
                    
                    if (index < 0 || index >= this.tagContext.attributeCount)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    return new HtmlTagContextAttribute(this.tagContext, ComposeIndexAndCookie(this.tagContext.cookie, index));
                }
            }

            
            
            
            
            public Enumerator GetEnumerator()
            {
                this.AssertValid();
                return new Enumerator(this.tagContext);
            }

            IEnumerator<HtmlTagContextAttribute> IEnumerable<HtmlTagContextAttribute>.GetEnumerator()
            {
                this.AssertValid();
                return new Enumerator(this.tagContext);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.AssertValid();
                return new Enumerator(this.tagContext);
            }

            private void AssertValid()
            {
                if (this.tagContext == null)
                {
                    throw new InvalidOperationException(Strings.AttributeCollectionNotInitialized);
                }
            }

            

            
            
            
            public struct Enumerator : IEnumerator<HtmlTagContextAttribute>, System.Collections.IEnumerator
            {
                private HtmlTagContext tagContext;
                private int attributeIndexAndCookie;

                internal Enumerator(HtmlTagContext tagContext)
                {
                    this.tagContext = tagContext;
                    
                    this.attributeIndexAndCookie = ComposeIndexAndCookie(this.tagContext.cookie, -1);
                }

                
                
                
                public void Dispose()
                {
                }

                
                
                
                public HtmlTagContextAttribute Current
                {
                    get
                    {
                        return new HtmlTagContextAttribute(this.tagContext, this.attributeIndexAndCookie);
                    }
                }

                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return new HtmlTagContextAttribute(this.tagContext, this.attributeIndexAndCookie);
                    }
                }

                
                
                
                
                public bool MoveNext()
                {
                    if (ExtractIndex(this.attributeIndexAndCookie) < this.tagContext.attributeCount)
                    {
                        this.attributeIndexAndCookie ++;

                        return ExtractIndex(this.attributeIndexAndCookie) < this.tagContext.attributeCount;
                    }

                    return false;
                }

                void System.Collections.IEnumerator.Reset()
                {
                    
                    this.attributeIndexAndCookie = ComposeIndexAndCookie(this.tagContext.cookie, -1);
                }
            }
        }
    }

    

    
    
    
    internal struct HtmlTagContextAttribute
    {
        private HtmlTagContext tagContext;
        private int attributeIndexAndCookie;

        internal HtmlTagContextAttribute(HtmlTagContext tagContext, int attributeIndexAndCookie)
        {
            this.tagContext = tagContext;
            this.attributeIndexAndCookie = attributeIndexAndCookie;
#if DEBUG
            this.AssertValid();
#endif
        }

        
        
        
        public static readonly HtmlTagContextAttribute Null = new HtmlTagContextAttribute();

        
        
        
        public bool IsNull { get { return this.tagContext == null; } }

        
        
        
        public HtmlAttributeId Id { get { this.AssertValid(); return this.tagContext.GetAttributeNameIdImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie)); } }

        
        
        
        public string Name { get { this.AssertValid(); return this.tagContext.GetAttributeNameImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie)); } }
        
        
        
        
        public string Value { get { this.AssertValid(); return this.tagContext.GetAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie)); } }    

        internal HtmlAttributeParts Parts { get { this.AssertValid(); return this.tagContext.GetAttributePartsImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie)); } }

        
        
        
        
        
        
        
        public int ReadValue(char[] buffer, int offset, int count)
        {
            this.AssertValid();
            return this.tagContext.ReadAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), buffer, offset, count);
        }

        
        
        
        public void Write()
        {
            this.AssertValid();
            this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, true);
        }

        
        
        
        public void WriteName()
        {
            this.AssertValid();
            this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, false);
        }

        
        
        
        public void WriteValue()
        {
            this.AssertValid();
            this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), false, true);
        }

        private void AssertValid()
        {
            if (this.tagContext == null)
            {
                throw new InvalidOperationException(Strings.AttributeNotInitialized);
            }

            this.tagContext.AssertAttributeValid(this.attributeIndexAndCookie);
        }

        
        
        public override string ToString()
        {
            
            return this.tagContext == null ? "null" : HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie).ToString();
        }
    }

#if M5STUFF


    
    
    
    
    public interface IHtmlParsingCallback
    {
        
        
        
        
        
        
        
        

        bool EvaluateConditional(string conditional);
    }

    
    

    
    
    
    
    public enum HtmlFilterAction
    {
        
        NoAction,
        
        Drop,
        
        DropContainerOnly,
        
        DropContainerAndContent,
        
        EmptyValue,
        
        ReplaceValue,
    }

    
    
    
    
    public struct HtmlFilterContextAction
    {
        
        public HtmlFilterContextType contextType;

        
        public HtmlNameId nameId;
        
        public string name;

        
        public HtmlNameId containerNameId;
        
        public string containerName;

        
        public HtmlFilterAction action;
        
        public string replacementValue;

        
        public bool callbackOverride;
    }

    
    
    
    
    
    
    
    public class HtmlFilterTables
    {
        
        
        
        
        

        public HtmlFilterTables(HtmlFilterContextAction[] staticActions, bool mergeWithDefault)
        {
        }

        
        
    }

    
    
    
    
    public interface IImageExtractionCallback
    {
        
        
        
        
        
        

        Stream CreateImage(string imageType, out string linkUrl);
    }

#endif 
}
