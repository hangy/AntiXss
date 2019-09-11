// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTagContext.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//    
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Collections.Generic;
    using CtsResources;
    using Data.Internal;
    using Internal.Html;

    internal abstract class HtmlTagContext
    {
        internal enum TagWriteState
        {
            Undefined,
            Written,
            Deleted,
        }

        // Orphaned WPL code.
#if false
        private TagWriteState writeState;
#endif

        private byte cookie;
        private bool valid;

        private bool invokeCallbackForEndTag;
        private bool deleteInnerContent;
        private bool deleteEndTag;

        private bool isEndTag;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="Value set by internal method call. Can't change API at this time.")]
        private bool isEmptyElementTag = false;

        private HtmlNameIndex tagNameIndex;

        private HtmlTagParts tagParts;

        // Orphaned WPL code.
#if false
        private int attributeCount;
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTagContext"/> class.
        /// </summary>
        internal HtmlTagContext()
        {
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Gets a value indicating whether this instance is an end tag.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is end tag; otherwise, <c>false</c>.
        /// </value>
        public bool IsEndTag       
        {
            get
            {
                this.AssertContextValid();
                return this.isEndTag;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an empty element tag.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is empty element tag; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmptyElementTag   
        {
            get
            {
                this.AssertContextValid();
                return this.isEmptyElementTag;
            }
        }

        /// <summary>
        /// Gets the tag id.
        /// </summary>
        /// <value>The tag id.</value>
        public HtmlTagId TagId
        {
            get
            {
                this.AssertContextValid();
                return HtmlNameData.names[(int)this.tagNameIndex].publicTagId;
            }
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        public string TagName
        {
            get
            {
                this.AssertContextValid();
                return this.GetTagNameImpl();
            }
        }
#endif

        /// <summary>
        /// Gets the index of the tag name.
        /// </summary>
        /// <value>The index of the tag name.</value>
        internal HtmlNameIndex TagNameIndex
        {
            get
            {
                this.AssertContextValid();
                return this.tagNameIndex;
            }
        }

        /// <summary>
        /// Gets the tag parts.
        /// </summary>
        /// <value>The tag parts.</value>
        internal HtmlTagParts TagParts
        {
            get
            {
                this.AssertContextValid();
                return this.tagParts;
            }
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public AttributeCollection Attributes
        {
            get
            {
                this.AssertContextValid();
                return new AttributeCollection(this);
            }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether this instance can invoke callback for end tag.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can invoke callback for end tag; otherwise, <c>false</c>.
        /// </value>
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

        // Orphaned WPL code.
#if false
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
                throw new InvalidOperationException(this.writeState == TagWriteState.Written ? TextConvertersStrings.CallbackTagAlreadyWritten : TextConvertersStrings.CallbackTagAlreadyDeleted);
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
                throw new InvalidOperationException(this.writeState == TagWriteState.Written ? TextConvertersStrings.CallbackTagAlreadyWritten : TextConvertersStrings.CallbackTagAlreadyDeleted);
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
#endif

        internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
        {
            this.isEndTag = isEndTag;

            this.isEmptyElementTag = false;
            this.tagNameIndex = tagNameIndex;

            // Orphaned WPL code.
#if false
            this.writeState = droppedEndTag ? TagWriteState.Deleted : TagWriteState.Undefined;
#endif

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

            // Orphaned WPL code.
#if false
            this.attributeCount = attributeCount;
#endif

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

        // Orphaned WPL code.
#if false
        internal abstract int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count);
#endif

        internal abstract void WriteTagImpl(bool writeAttributes);

        internal virtual void DeleteTagImpl()
        {
        }

        internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

        // Orphaned WPL code.
#if false
        internal void AssertAttributeValid(int attributeIndexAndCookie)
        {
            if (!this.valid)
            {
                throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidInThisState);
            }

            if (ExtractCookie(attributeIndexAndCookie) != this.cookie)
            {
                throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidForThisContext);
            }

            int index = ExtractIndex(attributeIndexAndCookie);

            if (index < 0 || index >= this.attributeCount)
            {
                throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidForThisContext);
            }
        }
#endif

        internal void AssertContextValid()
        {
            if (!this.valid)
            {
                throw new InvalidOperationException(TextConvertersStrings.ContextNotValidInThisState);
            }
        }

        // Orphaned WPL code.
#if false
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
                    throw new InvalidOperationException(TextConvertersStrings.AttributeCollectionNotInitialized);
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
#endif
    }
}