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

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTagContext"/> class.
        /// </summary>
        internal HtmlTagContext()
        {
        }

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

        internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
        {
            this.isEndTag = isEndTag;

            this.isEmptyElementTag = false;
            this.tagNameIndex = tagNameIndex;

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

        internal abstract void WriteTagImpl(bool writeAttributes);

        internal virtual void DeleteTagImpl()
        {
        }

        internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

        internal void AssertContextValid()
        {
            if (!this.valid)
            {
                throw new InvalidOperationException(TextConvertersStrings.ContextNotValidInThisState);
            }
        }
    }
}