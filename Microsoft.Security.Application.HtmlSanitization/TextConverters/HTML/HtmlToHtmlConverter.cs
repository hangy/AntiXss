// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlToHtmlConverter.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;

    using Microsoft.Exchange.Data.TextConverters.Internal.Css;

    using Security.Application.TextConverters.HTML;

    internal class HtmlToHtmlConverter : IProducerConsumer, IRestartable, IDisposable
    {
        private bool convertFragment;
        private bool outputFragment;
        private bool filterForFragment;

        private bool filterHtml;
        private bool truncateForCallback;

        private int smallCssBlockThreshold;
        private bool preserveDisplayNoneStyle;

        private bool hasTailInjection;

        private IHtmlParser parser;
        private bool endOfFile;

        private bool normalizedInput;

        internal HtmlWriter writer;

        private HtmlTagCallback callback;
        private HtmlToHtmlTagContext callbackContext;

        internal HtmlToken token;

        private bool headDivUnterminated;

        private int currentLevel;
        private int currentLevelDelta;

        private bool insideCSS;

        private int dropLevel = Int32.MaxValue;

        private EndTagActionEntry[] endTagActionStack;
        private int endTagActionStackTop;

        private bool tagDropped;
        private bool justTruncated;
        private bool tagCallbackRequested;
        private bool attributeTriggeredCallback;
        private bool endTagCallbackRequested;
        private bool ignoreAttrCallback;
        private bool styleIsCSS;

        private HtmlFilterData.FilterAction attrContinuationAction;

        private CopyPendingState copyPendingState;

        private HtmlTagIndex tagIndex;

        private int attributeCount;
        private int attributeSkipCount;

        private bool attributeIndirect;
        private AttributeIndirectEntry[] attributeIndirectIndex;

        private AttributeVirtualEntry[] attributeVirtualList;
        private int attributeVirtualCount;
        private ScratchBuffer attributeVirtualScratch;

        private ScratchBuffer attributeActionScratch;
        private bool attributeLeadingSpaces;

        private bool metaInjected;
        private bool insideHtml;
        private bool insideHead;
        private bool insideBody;

        private bool tagHasFilteredStyleAttribute;

        private CssParser cssParser;
        private ConverterBufferInput cssParserInput;

        private VirtualScratchSink virtualScratchSink;

        private IProgressMonitor progressMonitor;

        private const string NamePrefix = "x_";

        internal enum CopyPendingState : byte
        {
            NotPending,
            TagCopyPending,
            TagContentCopyPending,
            TagNameCopyPending,
            AttributeCopyPending,
            AttributeNameCopyPending,
            AttributeValueCopyPending,
        }

        [Flags]
        private enum AvailableTagParts : byte
        {
            None = 0,
            TagBegin = 0x01,
            TagEnd = 0x02,
            TagName = 0x04,
            Attributes = 0x08,
            UnstructuredContent = 0x10,
        }

        private enum AttributeIndirectKind
        {
            PassThrough,
            EmptyValue,
            FilteredStyle,
            Virtual,
            VirtualFilteredStyle,
            NameOnlyFragment,
        }

        private struct AttributeIndirectEntry
        {
            public AttributeIndirectKind kind;
            public short index;
        }

        private struct AttributeVirtualEntry
        {
            public short index;
            public int offset;
            public int length;
            public int position;
        }

        private struct EndTagActionEntry
        {
            public int tagLevel;
            public bool drop;
            public bool callback;
        }

        public HtmlToHtmlConverter(
                    IHtmlParser parser,
                    HtmlWriter writer,
                    bool convertFragment,
                    bool outputFragment,
                    bool filterHtml,
                    HtmlTagCallback callback,
                    bool truncateForCallback,
                    bool hasTailInjection,
                    Stream traceStream,
                    bool traceShowTokenNum,
                    int traceStopOnTokenNum,
                    int smallCssBlockThreshold,
                    bool preserveDisplayNoneStyle,
                    IProgressMonitor progressMonitor)
        {
            this.writer = writer;

            this.normalizedInput = parser is HtmlNormalizingParser;

            InternalDebug.Assert(progressMonitor != null);
            this.progressMonitor = progressMonitor;

            InternalDebug.Assert(!(outputFragment && !this.normalizedInput));

            this.convertFragment = convertFragment;
            this.outputFragment = outputFragment;

            this.filterForFragment = outputFragment || convertFragment;

            this.filterHtml = filterHtml || this.filterForFragment;
            this.callback = callback;

            this.parser = parser;
            if (!convertFragment)
            {
                this.parser.SetRestartConsumer(this);
            }

            this.truncateForCallback = truncateForCallback;

            this.hasTailInjection = hasTailInjection;

            this.smallCssBlockThreshold = smallCssBlockThreshold;
            this.preserveDisplayNoneStyle = preserveDisplayNoneStyle;
        }

        void IDisposable.Dispose()
        {
            if (this.parser != null && this.parser is IDisposable)
            {
                ((IDisposable)this.parser).Dispose();
            }

            if (!this.convertFragment && this.writer != null && this.writer is IDisposable)
            {
                ((IDisposable)this.writer).Dispose();
            }

            if (this.token != null && this.token is IDisposable)
            {
                ((IDisposable)this.token).Dispose();
            }

            this.parser = null;
            this.writer = null;

            this.token = null;

            GC.SuppressFinalize(this);
        }

        private CopyPendingState CopyPendingStateFlag
        {
            get
            {
                return this.copyPendingState;
            }
            set
            {
                this.writer.SetCopyPending(value != CopyPendingState.NotPending);
                this.copyPendingState = value;
            }
        }

        public void Run()
        {
            if (!this.endOfFile)
            {
                HtmlTokenId tokenId = this.parser.Parse();

                if (HtmlTokenId.None != tokenId)
                {
                    this.Process(tokenId);
                }
            }
        }

        public bool Flush()
        {
            if (!this.endOfFile)
            {
                this.Run();
            }

            return this.endOfFile;
        }

        // Orphaned WPL code.
#if false
        public void Initialize(string fragment, bool preformatedText)
        {
            InternalDebug.Assert(this.convertFragment);

            if (this.parser is HtmlNormalizingParser)
            {
                ((HtmlNormalizingParser)this.parser).Initialize(fragment, preformatedText);
            }
            else
            {
                ((HtmlParser)this.parser).Initialize(fragment, preformatedText);
            }

            ((IRestartable)this).Restart();
        }
#endif

        bool IRestartable.CanRestart()
        {
            if (this.writer is IRestartable restartable)
            {
                return restartable.CanRestart();
            }

            return false;
        }

        void IRestartable.Restart()
        {
            if (this.writer is IRestartable && !this.convertFragment)
            {
                ((IRestartable)this.writer).Restart();
            }

            this.endOfFile = false;

            this.token = null;

            this.styleIsCSS = true;
            this.insideCSS = false;

            this.headDivUnterminated = false;

            this.tagDropped = false;
            this.justTruncated = false;
            this.tagCallbackRequested = false;
            this.endTagCallbackRequested = false;
            this.ignoreAttrCallback = false;
            this.attrContinuationAction = HtmlFilterData.FilterAction.Unknown;
            this.currentLevel = 0;
            this.currentLevelDelta = 0;
            this.dropLevel = Int32.MaxValue;
            this.endTagActionStackTop = 0;

            this.copyPendingState = CopyPendingState.NotPending;

            this.metaInjected = false;
            this.insideHtml = false;
            this.insideHead = false;
            this.insideBody = false;
        }

        void IRestartable.DisableRestart()
        {
            if (this.writer is IRestartable restartable)
            {
                restartable.DisableRestart();
            }
        }

        private void Process(HtmlTokenId tokenId)
        {
            this.token = this.parser.Token;

            if (!this.metaInjected)
            {
                if (!this.InjectMetaTagIfNecessary())
                {
                    return;
                }
            }

            switch (tokenId)
            {
                case HtmlTokenId.Tag:

                    if (!this.token.IsEndTag)
                    {
                        this.ProcessStartTag();
                    }
                    else
                    {
                        this.ProcessEndTag();
                    }
                    break;

                case HtmlTokenId.OverlappedClose:

                    this.ProcessOverlappedClose();
                    break;

                case HtmlTokenId.OverlappedReopen:

                    this.ProcessOverlappedReopen();
                    break;

                case HtmlTokenId.Text:

                    this.ProcessText();
                    break;

                case HtmlTokenId.InjectionBegin:

                    this.ProcessInjectionBegin();
                    break;

                case HtmlTokenId.InjectionEnd:

                    this.ProcessInjectionEnd();
                    break;

                case HtmlTokenId.Restart:

                    break;

                case HtmlTokenId.EncodingChange:

                    if (this.writer.HasEncoding && this.writer.CodePageSameAsInput)
                    {
                        int codePage = this.token.Argument;

#if DEBUG

                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out Encoding newOutputEncoding));
#endif

                        this.writer.Encoding = Charset.GetEncoding(codePage);
                    }
                    break;

                case HtmlTokenId.EndOfFile:

                    this.ProcessEof();
                    break;
            }
        }

        private void ProcessStartTag()
        {
            AvailableTagParts availableParts = 0;

            if (this.insideCSS &&
                this.token.TagIndex == HtmlTagIndex._COMMENT &&
                this.filterHtml)
            {
                this.AppendCssFromTokenText();

                return;
            }

            if (this.token.IsTagBegin)
            {
                InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);

                this.currentLevel++;

                this.tagIndex = this.token.TagIndex;

                this.tagDropped = false;
                this.justTruncated = false;
                this.endTagCallbackRequested = false;

                this.PreProcessStartTag();

                if (this.currentLevel >= this.dropLevel)
                {
                    this.tagDropped = true;
                }
                else if (!this.tagDropped)
                {
                    this.tagCallbackRequested = false;
                    this.ignoreAttrCallback = false;

                    if (this.filterHtml || this.callback != null)
                    {
                        InternalDebug.Assert(this.normalizedInput);

                        HtmlFilterData.FilterAction tagAction = this.filterForFragment ?
                                HtmlFilterData.filterInstructions[(int)this.token.NameIndex].tagFragmentAction :
                                HtmlFilterData.filterInstructions[(int)this.token.NameIndex].tagAction;

                        if (this.callback != null && 0 != (tagAction & HtmlFilterData.FilterAction.Callback))
                        {
                            this.tagCallbackRequested = true;
                        }
                        else if (this.filterHtml)
                        {
                            this.ignoreAttrCallback = (0 != (tagAction & HtmlFilterData.FilterAction.IgnoreAttrCallbacks));

                            switch (tagAction & HtmlFilterData.FilterAction.ActionMask)
                            {
                                case HtmlFilterData.FilterAction.Drop:

                                    this.tagDropped = true;
                                    this.dropLevel = this.currentLevel;
                                    break;

                                case HtmlFilterData.FilterAction.DropKeepContent:

                                    this.tagDropped = true;
                                    break;

                                case HtmlFilterData.FilterAction.KeepDropContent:

                                    this.dropLevel = this.currentLevel + 1;
                                    break;

                                case HtmlFilterData.FilterAction.Keep:

                                    break;
                            }
                        }
                    }

                    if (!this.tagDropped)
                    {
                        this.attributeTriggeredCallback = false;
                        this.tagHasFilteredStyleAttribute = false;

                        availableParts = AvailableTagParts.TagBegin;
                    }
                }
            }

            if (!this.tagDropped)
            {
                HtmlToken.TagPartMinor tagMinorPart = this.token.MinorPart;

                if (this.token.IsTagEnd)
                {
                    availableParts |= AvailableTagParts.TagEnd;
                }

                if (this.tagIndex < HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending || this.CopyPendingStateFlag == CopyPendingState.TagCopyPending);

                    availableParts |= AvailableTagParts.UnstructuredContent;
                    this.attributeCount = 0;
                }
                else
                {
                    if (this.token.HasNameFragment || this.token.IsTagNameEnd)
                    {
                        availableParts |= AvailableTagParts.TagName;
                    }

                    this.ProcessTagAttributes();

                    if (this.attributeCount != 0)
                    {
                        availableParts |= AvailableTagParts.Attributes;
                    }
                }

                if (availableParts != 0)
                {
                    if (this.CopyPendingStateFlag != CopyPendingState.NotPending)
                    {
                        InternalDebug.Assert(!this.token.IsTagBegin);

                        switch (this.CopyPendingStateFlag)
                        {
                            case CopyPendingState.TagCopyPending:

                                this.CopyInputTag(true);

                                if (this.tagCallbackRequested && 0 != (availableParts & AvailableTagParts.TagEnd))
                                {
                                    this.attributeCount = 0;
                                    this.token.Name.MakeEmpty();

                                    availableParts &= ~AvailableTagParts.TagEnd;
                                    tagMinorPart = 0;
                                }
                                else
                                {
                                    availableParts = AvailableTagParts.None;
                                }
                                break;

                            case CopyPendingState.TagNameCopyPending:

                                InternalDebug.Assert(this.tagIndex == HtmlTagIndex.Unknown && !this.token.IsTagNameBegin);

                                this.token.Name.WriteTo(this.writer.WriteTagName());

                                if (this.token.IsTagNameEnd)
                                {
                                    this.CopyPendingStateFlag = CopyPendingState.NotPending;
                                }

                                this.token.Name.MakeEmpty();

                                availableParts &= ~AvailableTagParts.TagName;
                                tagMinorPart &= ~HtmlToken.TagPartMinor.CompleteName;
                                break;

                            case CopyPendingState.AttributeCopyPending:

                                InternalDebug.Assert(this.GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!this.token.Attributes[0].IsAttrBegin && this.attributeCount != 0);

                                this.CopyInputAttribute(0);

                                this.attributeSkipCount = 1;
                                this.attributeCount--;

                                if (0 == this.attributeCount)
                                {
                                    availableParts &= ~AvailableTagParts.Attributes;
                                }

                                tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;
                                break;

                            case CopyPendingState.AttributeNameCopyPending:

                                InternalDebug.Assert(this.GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!this.token.Attributes[0].IsAttrBegin &&
                                                    0 != (this.token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueName) &&
                                                    this.attributeCount != 0);

                                this.CopyInputAttributeName(0);

                                if (1 == this.attributeCount && 0 == (this.token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue))
                                {
                                    this.attributeSkipCount = 1;
                                    this.attributeCount--;

                                    availableParts &= ~AvailableTagParts.Attributes;
                                    tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;

                                    InternalDebug.Assert(availableParts == 0 || availableParts == AvailableTagParts.TagEnd);
                                }
                                else
                                {
                                    this.token.Attributes[0].Name.MakeEmpty();

                                    this.token.Attributes[0].SetMinorPart(this.token.Attributes[0].MinorPart & ~HtmlToken.AttrPartMinor.CompleteName);
                                }

                                break;

                            case CopyPendingState.AttributeValueCopyPending:

                                InternalDebug.Assert(this.GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!this.token.Attributes[0].IsAttrBegin &&
                                                    (0 != (this.token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue) ||
                                                     this.token.Attributes[0].MinorPart == HtmlToken.AttrPartMinor.Empty ||
                                                     this.token.Attributes[0].IsAttrEnd) &&
                                                    this.attributeCount != 0);

                                this.CopyInputAttributeValue(0);

                                this.attributeSkipCount = 1;
                                this.attributeCount--;

                                if (0 == this.attributeCount)
                                {
                                    availableParts &= ~AvailableTagParts.Attributes;
                                }

                                tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;
                                break;

                            default:

                                InternalDebug.Assert(false);
                                break;
                        }
                    }

                    if (availableParts != 0)
                    {
                        if (this.tagCallbackRequested)
                        {
                            InternalDebug.Assert(!this.truncateForCallback || this.token.IsTagBegin);

                            InternalDebug.Assert(this.callback != null);

                            if (this.callbackContext == null)
                            {
                                this.callbackContext = new HtmlToHtmlTagContext(this);
                            }

                            InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);

                            if (this.token.IsTagBegin || this.attributeTriggeredCallback)
                            {
                                this.callbackContext.InitializeTag(false/*this.token.IsEndTag*/, HtmlDtd.tags[(int)this.tagIndex].nameIndex, false);

                                this.attributeTriggeredCallback = false;
                            }

                            this.callbackContext.InitializeFragment(this.token.IsEmptyScope, this.attributeCount, new HtmlTagParts(this.token.MajorPart, this.token.MinorPart));

                            this.callback(this.callbackContext, this.writer);

                            this.callbackContext.UninitializeFragment();

                            if (this.token.IsTagEnd || this.truncateForCallback)
                            {
                                if (this.callbackContext.IsInvokeCallbackForEndTag)
                                {
                                    this.endTagCallbackRequested = true;
                                }

                                if (this.callbackContext.IsDeleteInnerContent)
                                {
                                    this.dropLevel = this.currentLevel + 1;
                                }

                                if (this.token.IsTagBegin && this.callbackContext.IsDeleteEndTag)
                                {
                                    this.tagDropped = true;
                                }

                                if (!this.tagDropped && !this.token.IsTagEnd)
                                {
                                    InternalDebug.Assert(this.truncateForCallback);

                                    this.tagDropped = true;

                                    this.justTruncated = true;

                                    this.CopyPendingStateFlag = CopyPendingState.NotPending;
                                }
                            }
                        }
                        else
                        {
                            if (this.token.IsTagBegin)
                            {
                                this.CopyInputTag(false);
                            }

                            if (this.attributeCount != 0)
                            {
                                this.CopyInputTagAttributes();
                            }

                            if (this.token.IsTagEnd)
                            {
                                if (this.tagIndex == HtmlTagIndex.Unknown)
                                {
                                    this.writer.WriteTagEnd(this.token.IsEmptyScope);
                                }
                            }
                        }
                    }
                }
            }

            if (this.token.IsTagEnd)
            {
                InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);

                if (this.writer.IsTagOpen)
                {
                    this.writer.WriteTagEnd();
                }

                if (!this.token.IsEmptyScope && this.tagIndex > HtmlTagIndex.Unknown)
                {
                    if (this.normalizedInput && this.currentLevel < this.dropLevel &&
                        ((this.tagDropped && !this.justTruncated) || this.endTagCallbackRequested))
                    {
                        if (this.endTagActionStack == null)
                        {
                            this.endTagActionStack = new EndTagActionEntry[4];
                        }
                        else if (this.endTagActionStack.Length == this.endTagActionStackTop)
                        {
                            EndTagActionEntry[] newEndTagActionStack = new EndTagActionEntry[this.endTagActionStack.Length * 2];
                            Array.Copy(this.endTagActionStack, 0, newEndTagActionStack, 0, this.endTagActionStackTop);
                            this.endTagActionStack = newEndTagActionStack;
                        }

                        this.endTagActionStack[this.endTagActionStackTop].tagLevel = this.currentLevel;
                        this.endTagActionStack[this.endTagActionStackTop].drop = this.tagDropped && !this.justTruncated;
                        this.endTagActionStack[this.endTagActionStackTop].callback = this.endTagCallbackRequested;

                        this.endTagActionStackTop++;
                    }

                    this.currentLevel++;

                    this.PostProcessStartTag();
                }
                else
                {
                    InternalDebug.Assert(!this.normalizedInput || this.currentLevel > 0);

                    this.currentLevel--;

                    if (this.dropLevel != Int32.MaxValue && this.currentLevel < this.dropLevel)
                    {
                        this.dropLevel = Int32.MaxValue;
                    }
                }
            }
        }

        private void ProcessEndTag()
        {
            AvailableTagParts availableParts = 0;

            InternalDebug.Assert(this.token.TagIndex >= HtmlTagIndex.Unknown);

            if (this.token.IsTagBegin)
            {
                InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);
                InternalDebug.Assert(!this.normalizedInput || this.currentLevel > 0 || this.token.TagIndex == HtmlTagIndex.Unknown);

                if (this.currentLevel > 0)
                {
                    this.currentLevel--;
                }

                this.tagIndex = this.token.TagIndex;

                this.tagDropped = false;
                this.tagCallbackRequested = false;
                this.tagHasFilteredStyleAttribute = false;

                availableParts = AvailableTagParts.TagBegin;

                this.PreProcessEndTag();

                InternalDebug.Assert(this.endTagActionStackTop == 0 || this.endTagActionStack[this.endTagActionStackTop - 1].tagLevel <= this.currentLevel + this.currentLevelDelta);

                if (this.currentLevel >= this.dropLevel)
                {
                    this.tagDropped = true;
                }
                else
                {
                    if (this.endTagActionStackTop != 0 && this.tagIndex > HtmlTagIndex.Unknown)
                    {
                        if (this.endTagActionStack[this.endTagActionStackTop - 1].tagLevel >= this.currentLevel)
                        {
                            if (this.endTagActionStack[this.endTagActionStackTop - 1].tagLevel == this.currentLevel)
                            {
                                this.endTagActionStackTop--;

                                this.tagDropped = this.endTagActionStack[this.endTagActionStackTop].drop;
                                this.tagCallbackRequested = this.endTagActionStack[this.endTagActionStackTop].callback;
                            }
                            else
                            {
                                InternalDebug.Assert(this.currentLevelDelta != 0);

                                int stackPos;

                                for (stackPos = this.endTagActionStackTop; stackPos > 0 && this.endTagActionStack[stackPos - 1].tagLevel > this.currentLevel; stackPos--)
                                {
                                }

                                for (int j = stackPos; j < this.endTagActionStackTop; j++)
                                {
                                    this.endTagActionStack[j].tagLevel -= 2;
                                }

                                if (stackPos > 0 && this.endTagActionStack[stackPos - 1].tagLevel == this.currentLevel)
                                {
                                    this.tagDropped = this.endTagActionStack[stackPos - 1].drop;
                                    this.tagCallbackRequested = this.endTagActionStack[stackPos - 1].callback;

                                    for (; stackPos < this.endTagActionStackTop; stackPos++)
                                    {
                                        this.endTagActionStack[stackPos - 1] = this.endTagActionStack[stackPos];
                                    }

                                    this.endTagActionStackTop--;
                                }
                            }
                        }
                    }

                    if (this.token.Argument == 1 && this.tagIndex == HtmlTagIndex.Unknown)
                    {
                        this.tagDropped = true;
                    }
                }
            }

            HtmlToken.TagPartMinor tagMinorPart;

            if (!this.tagDropped)
            {
                tagMinorPart = this.token.MinorPart & ~(HtmlToken.TagPartMinor.AttributePartMask | HtmlToken.TagPartMinor.Attributes);

                if (this.token.IsTagEnd)
                {
                    availableParts |= AvailableTagParts.TagEnd;
                }

                if (this.token.HasNameFragment)
                {
                    availableParts |= AvailableTagParts.TagName;
                }

                if (this.CopyPendingStateFlag == CopyPendingState.TagNameCopyPending)
                {
                    InternalDebug.Assert(!this.token.IsTagBegin);

                    this.token.Name.WriteTo(this.writer.WriteTagName());

                    if (this.token.IsTagNameEnd)
                    {
                        this.CopyPendingStateFlag = CopyPendingState.NotPending;
                    }

                    this.token.Name.MakeEmpty();

                    availableParts &= ~AvailableTagParts.TagName;
                    tagMinorPart &= ~HtmlToken.TagPartMinor.CompleteName;
                }

                if (availableParts != 0)
                {
                    if (this.tagCallbackRequested)
                    {
                        InternalDebug.Assert(this.callback != null && this.callbackContext != null);

                        InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);

                        if (this.token.IsTagBegin)
                        {
                            this.callbackContext.InitializeTag(true/*this.token.IsEndTag*/, HtmlDtd.tags[(int)this.tagIndex].nameIndex, false);
                        }

                        this.callbackContext.InitializeFragment(false, 0, new HtmlTagParts(this.token.MajorPart, tagMinorPart));

                        this.callback(this.callbackContext, this.writer);

                        this.callbackContext.UninitializeFragment();
                    }
                    else
                    {
                        if (this.token.IsTagBegin)
                        {
                            this.CopyInputTag(false);
                        }
                    }
                }
            }
            else if (this.tagCallbackRequested)
            {
                tagMinorPart = this.token.MinorPart & ~(HtmlToken.TagPartMinor.AttributePartMask | HtmlToken.TagPartMinor.Attributes);

                InternalDebug.Assert(this.callback != null && this.callbackContext != null);

                InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);

                if (this.token.IsTagBegin)
                {
                    this.callbackContext.InitializeTag(true/*this.token.IsEndTag*/, HtmlDtd.tags[(int)this.tagIndex].nameIndex, true/*droppedEndTag*/);
                }

                this.callbackContext.InitializeFragment(false, 0, new HtmlTagParts(this.token.MajorPart, tagMinorPart));

                this.callback(this.callbackContext, this.writer);

                this.callbackContext.UninitializeFragment();
            }

            if (this.token.IsTagEnd)
            {
                if (this.writer.IsTagOpen)
                {
                    this.writer.WriteTagEnd();
                }

                if (this.tagIndex > HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(!this.normalizedInput || this.currentLevel > 0);

                    if (this.currentLevel > 0)
                    {
                        this.currentLevel--;
                    }

                    if (this.dropLevel != Int32.MaxValue && this.currentLevel < this.dropLevel)
                    {
                        this.dropLevel = Int32.MaxValue;
                    }
                }
                else
                {
                    if (this.currentLevel > 0)
                    {
                        this.currentLevel++;
                    }
                }
            }
        }

        private void ProcessOverlappedClose()
        {
            InternalDebug.Assert(this.currentLevelDelta == 0);

            this.currentLevelDelta = this.token.Argument * 2;
            this.currentLevel -= this.currentLevelDelta;
        }

        private void ProcessOverlappedReopen()
        {
            InternalDebug.Assert(this.currentLevelDelta == this.token.Argument * 2);

            this.currentLevel += this.token.Argument * 2;
            this.currentLevelDelta = 0;
        }

        private void ProcessText()
        {
            if (this.currentLevel >= this.dropLevel)
            {
                return;
            }

            if (this.insideCSS && this.filterHtml)
            {
                this.AppendCssFromTokenText();
            }
            else if (this.token.Argument == 1)
            {
                InternalDebug.Assert(this.token.Text.Length == 1/* todo: && this.token.Text[0].IsWhitespace*/);

                this.writer.WriteCollapsedWhitespace();
            }
            else if (this.token.Runs.MoveNext(true))
            {
                this.token.Text.WriteTo(this.writer.WriteText());
            }
        }

        private void ProcessInjectionBegin()
        {
            if (this.token.Argument == 0)
            {
                if (this.headDivUnterminated)
                {
                    this.writer.WriteEndTag(HtmlNameIndex.Div);
                    this.writer.WriteAutoNewLine(true);

                    this.headDivUnterminated = false;
                }
            }
        }

        private void ProcessInjectionEnd()
        {
            if (this.token.Argument != 0)
            {
                this.writer.WriteAutoNewLine(true);
                this.writer.WriteStartTag(HtmlNameIndex.Div);

                this.headDivUnterminated = true;
            }
        }

        private void ProcessEof()
        {
            this.writer.SetCopyPending(false);

            if (this.headDivUnterminated && this.dropLevel != 0)
            {
                this.writer.WriteEndTag(HtmlNameIndex.Div);
                this.writer.WriteAutoNewLine(true);

                this.headDivUnterminated = false;
            }

            //if (this.outputFragment && !this.insideBody)
            //{
            //    this.writer.WriteStartTag(HtmlNameIndex.Div);
            //    this.writer.WriteEndTag(HtmlNameIndex.Div);
            //    this.writer.WriteAutoNewLine(true);
            //}

            if (!this.convertFragment)
            {
                this.writer.Flush();
            }

            this.endOfFile = true;
        }

        private void PreProcessStartTag()
        {
            InternalDebug.Assert(this.token.IsTagBegin && !this.token.IsEndTag);

            if (this.tagIndex > HtmlTagIndex.Unknown)
            {
                if (this.tagIndex == HtmlTagIndex.Body)
                {
                    if (this.outputFragment)
                    {
                        this.insideBody = true;

                        this.tagIndex = HtmlTagIndex.Div;
                    }
                }
                else if (this.tagIndex == HtmlTagIndex.Meta)
                {
                    if (!this.filterHtml)
                    {
                        this.token.Attributes.Rewind();

                        foreach (HtmlAttribute attribute in this.token.Attributes)
                        {
                            if (attribute.NameIndex == HtmlNameIndex.HttpEquiv)
                            {
                                if (attribute.Value.CaseInsensitiveCompareEqual("content-type") ||
                                    attribute.Value.CaseInsensitiveCompareEqual("charset"))
                                {
                                    this.tagDropped = true;
                                    break;
                                }
                            }
                            else if (attribute.NameIndex == HtmlNameIndex.Charset)
                            {
                                this.tagDropped = true;
                                break;
                            }
                        }
                    }
                }
                else if (this.tagIndex == HtmlTagIndex.Style)
                {
#if false
                    
                    

                    if (!this.outputFragment)
                    {
#endif

                    this.styleIsCSS = true;

                    if (this.token.Attributes.Find(HtmlNameIndex.Type))
                    {
                        HtmlAttribute attribute = this.token.Attributes.Current;

                        if (!attribute.Value.CaseInsensitiveCompareEqual("text/css"))
                        {
                            this.styleIsCSS = false;
                        }
                    }

#if false
                    
                    }
                    else
                    {
                        
                        
                        this.tagDropped = true;
                        this.dropLevel = this.currentLevel;
                    }
#endif
                }
                else if (this.tagIndex == HtmlTagIndex.TC)
                {
                    this.tagDropped = true;
                }
                else if (this.tagIndex == HtmlTagIndex.PlainText || this.tagIndex == HtmlTagIndex.Xmp)
                {
                    if (this.filterHtml || (this.hasTailInjection && this.tagIndex == HtmlTagIndex.PlainText))
                    {
                        this.tagDropped = true;

                        this.writer.WriteAutoNewLine(true);
                        this.writer.WriteStartTag(HtmlNameIndex.TT);
                        this.writer.WriteStartTag(HtmlNameIndex.Pre);
                        this.writer.WriteAutoNewLine();
                    }
                }
                else if (this.tagIndex == HtmlTagIndex.Image)
                {
                    if (this.filterHtml)
                    {
                        this.tagIndex = HtmlTagIndex.Img;
                    }
                }
            }
        }

        private void ProcessTagAttributes()
        {
            this.attributeSkipCount = 0;

            HtmlToken.AttributeEnumerator attributes = this.token.Attributes;
            HtmlAttribute attribute;
            HtmlFilterData.FilterAction attrAction;

            if (this.filterHtml)
            {
                this.attributeCount = 0;
                this.attributeIndirect = true;
                this.attributeVirtualCount = 0;
                this.attributeVirtualScratch.Reset();

                if (this.attributeIndirectIndex == null)
                {
                    this.attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(attributes.Count + 1, 32)];
                }
                else if (this.attributeIndirectIndex.Length <= attributes.Count)
                {
                    this.attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(this.attributeIndirectIndex.Length * 2, attributes.Count + 1)];
                }

                for (int i = 0; i < attributes.Count; i++)
                {
                    attribute = attributes[i];

                    if (attribute.IsAttrBegin)
                    {
                        attrAction = this.filterForFragment ?
                                HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrFragmentAction :
                                HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrAction;

                        if (0 != (attrAction & HtmlFilterData.FilterAction.HasExceptions) &&
                            0 != (HtmlFilterData.filterInstructions[(int)this.token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions))
                        {
                            for (int j = 0; j < HtmlFilterData.filterExceptions.Length; j++)
                            {
                                if (HtmlFilterData.filterExceptions[j].tagNameIndex == this.token.NameIndex &&
                                    HtmlFilterData.filterExceptions[j].attrNameIndex == attribute.NameIndex)
                                {
                                    attrAction = this.filterForFragment ?
                                            HtmlFilterData.filterExceptions[j].fragmentAction :
                                            HtmlFilterData.filterExceptions[j].action;
                                    break;
                                }
                            }
                        }

                        if (!this.outputFragment &&
                            (attrAction == HtmlFilterData.FilterAction.PrefixName || attrAction == HtmlFilterData.FilterAction.PrefixNameList))
                        {
                            attrAction = HtmlFilterData.FilterAction.Keep;
                        }

                        if (this.callback != null && !this.ignoreAttrCallback && 0 != (attrAction & HtmlFilterData.FilterAction.Callback))
                        {
                            if (this.token.IsTagBegin || !this.truncateForCallback)
                            {
                                this.attributeTriggeredCallback = this.attributeTriggeredCallback || !this.tagCallbackRequested;
                                this.tagCallbackRequested = true;
                            }
                            else
                            {
                                attrAction = HtmlFilterData.FilterAction.KeepDropContent;
                            }
                        }

                        attrAction &= HtmlFilterData.FilterAction.ActionMask;

                        if (!attribute.IsAttrEnd)
                        {
                            InternalDebug.Assert(attribute.Index == attributes.Count - 1 && !this.token.IsTagEnd);

                            this.attrContinuationAction = attrAction;
                        }
                    }
                    else
                    {
                        InternalDebug.Assert(attribute.Index == 0 && !this.token.IsTagBegin);
                        attrAction = this.attrContinuationAction;
                    }

                    if (attrAction != HtmlFilterData.FilterAction.Drop)
                    {
                        if (attrAction == HtmlFilterData.FilterAction.Keep)
                        {
                            this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                            this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.PassThrough;
                            this.attributeCount++;
                        }
                        else
                        {
                            if (attrAction == HtmlFilterData.FilterAction.KeepDropContent)
                            {
                                InternalDebug.Assert(attribute.IsAttrBegin);

                                this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                this.attributeCount++;
                            }
                            else if (attrAction == HtmlFilterData.FilterAction.FilterStyleAttribute)
                            {
                                if (attribute.IsAttrBegin)
                                {
                                    if (this.tagHasFilteredStyleAttribute)
                                    {
                                        this.AppendCss(";");
                                    }

                                    this.tagHasFilteredStyleAttribute = true;
                                }

                                this.AppendCssFromAttribute(attribute);

                                continue;
                            }
                            else if (attrAction == HtmlFilterData.FilterAction.ConvertBgcolorIntoStyle)
                            {
                                if (attribute.IsAttrBegin)
                                {
                                    if (this.tagHasFilteredStyleAttribute)
                                    {
                                        this.AppendCss(";");
                                    }

                                    this.tagHasFilteredStyleAttribute = true;
                                }

                                this.AppendCss("background-color:");
                                this.AppendCssFromAttribute(attribute);

                                continue;
                            }
                            else
                            {
                                int vi;
                                int offset, length;

                                InternalDebug.Assert(attrAction == HtmlFilterData.FilterAction.SanitizeUrl ||
                                                    attrAction == HtmlFilterData.FilterAction.PrefixName ||
                                                    attrAction == HtmlFilterData.FilterAction.PrefixNameList);

                                if (attribute.IsAttrBegin)
                                {
                                    this.attributeLeadingSpaces = true;
                                }

                                if (this.attributeLeadingSpaces)
                                {
                                    if (!attribute.Value.SkipLeadingWhitespace() && !attribute.IsAttrEnd)
                                    {
                                        if (!attribute.IsAttrBegin && !attribute.HasNameFragment)
                                        {
                                            continue;
                                        }

                                        this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                        this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                        this.attributeCount++;
                                        continue;
                                    }

                                    this.attributeLeadingSpaces = false;
                                    this.attributeActionScratch.Reset();
                                }

                                bool truncate = false;

                                if (!this.attributeActionScratch.AppendHtmlAttributeValue(attribute, 4 * 1024))
                                {
                                    truncate = true;
                                }

                                if (!attribute.IsAttrEnd && !truncate)
                                {
                                    if (!attribute.IsAttrBegin && !attribute.HasNameFragment)
                                    {
                                        continue;
                                    }

                                    this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                    this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                    this.attributeCount++;
                                    continue;
                                }

                                this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                if (attrAction == HtmlFilterData.FilterAction.SanitizeUrl)
                                {
                                    switch (CheckUrl(this.attributeActionScratch.Buffer, this.attributeActionScratch.Length, this.tagCallbackRequested))
                                    {
                                        case CheckUrlResult.LocalHyperlink:

                                            InternalDebug.Assert(this.attributeActionScratch[0] == '#');

                                            if (!this.outputFragment)
                                            {
                                                goto case CheckUrlResult.Safe;
                                            }

                                            int nameLength = NonWhitespaceLength(this.attributeActionScratch.Buffer, 1, this.attributeActionScratch.Length - 1);

                                            if (nameLength == 0)
                                            {
                                                goto default;
                                            }

                                            offset = this.attributeVirtualScratch.Length;
                                            length = 0;
                                            length += this.attributeVirtualScratch.Append('#', int.MaxValue);
                                            length += this.attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                            length += this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 1, nameLength, int.MaxValue);

                                            vi = this.AllocateVirtualEntry(i, offset, length);

                                            this.attributeIndirectIndex[this.attributeCount].index = (short)vi;
                                            this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.Virtual;
                                            this.attributeCount++;
                                            break;

                                        case CheckUrlResult.Safe:

                                            if (attribute.IsCompleteAttr)
                                            {
                                                this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                                this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.PassThrough;
                                                this.attributeCount++;
                                                break;
                                            }

                                            offset = this.attributeVirtualScratch.Length;
                                            length = this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 0, this.attributeActionScratch.Length, int.MaxValue);

                                            vi = this.AllocateVirtualEntry(i, offset, length);

                                            this.attributeIndirectIndex[this.attributeCount].index = (short)vi;
                                            this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.Virtual;
                                            this.attributeCount++;
                                            break;

                                        case CheckUrlResult.Inconclusive:

                                            if (this.attributeActionScratch.Length <= 256 && attribute.IsAttrEnd)
                                            {
                                                InternalDebug.Assert(attribute.IsAttrEnd);

                                                goto case CheckUrlResult.Safe;
                                            }

                                            goto default;

                                        default:

                                            this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                            this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                            this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                            this.attributeCount++;
                                            break;
                                    }
                                }
                                else if (attrAction == HtmlFilterData.FilterAction.PrefixName)
                                {
                                    InternalDebug.Assert(this.outputFragment);

                                    offset = this.attributeVirtualScratch.Length;
                                    length = 0;

                                    int nameLength = NonWhitespaceLength(this.attributeActionScratch.Buffer, 0, this.attributeActionScratch.Length);

                                    if (nameLength != 0)
                                    {
                                        length += this.attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                        length += this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, 0, nameLength, int.MaxValue);
                                    }

                                    vi = this.AllocateVirtualEntry(i, offset, length);

                                    this.attributeIndirectIndex[this.attributeCount].index = (short)vi;
                                    this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.Virtual;
                                    this.attributeCount++;
                                }
                                else if (attrAction == HtmlFilterData.FilterAction.PrefixNameList)
                                {
                                    InternalDebug.Assert(this.outputFragment);

                                    offset = this.attributeVirtualScratch.Length;
                                    length = 0;

                                    int nameOffset = 0;
                                    int nameLength = NonWhitespaceLength(this.attributeActionScratch.Buffer, nameOffset, this.attributeActionScratch.Length - nameOffset);

                                    if (nameLength != 0)
                                    {
                                        do
                                        {
                                            length += this.attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                            length += this.attributeVirtualScratch.Append(this.attributeActionScratch.Buffer, nameOffset, nameLength, int.MaxValue);

                                            nameOffset += nameLength;
                                            nameOffset += WhitespaceLength(this.attributeActionScratch.Buffer, nameOffset, this.attributeActionScratch.Length - nameOffset);

                                            nameLength = NonWhitespaceLength(this.attributeActionScratch.Buffer, nameOffset, this.attributeActionScratch.Length - nameOffset);

                                            if (nameLength != 0)
                                            {
                                                length += this.attributeVirtualScratch.Append(' ', int.MaxValue);
                                            }
                                        }
                                        while (nameLength != 0);
                                    }

                                    vi = this.AllocateVirtualEntry(i, offset, length);

                                    this.attributeIndirectIndex[this.attributeCount].index = (short)vi;
                                    this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.Virtual;
                                    this.attributeCount++;
                                }
                                else
                                {
                                    InternalDebug.Assert(false);

                                    InternalDebug.Assert((attribute.MinorPart & HtmlToken.AttrPartMinor.BeginValue) == HtmlToken.AttrPartMinor.BeginValue);

                                    this.attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                    this.attributeIndirectIndex[this.attributeCount].index = (short)i;
                                    this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                    this.attributeCount++;
                                }
                            }
                        }
                    }
                }

                if (this.tagHasFilteredStyleAttribute && (this.token.IsTagEnd || (this.tagCallbackRequested && this.truncateForCallback)))
                {
                    this.attributeIndirectIndex[this.attributeCount].index = -1;
                    this.attributeIndirectIndex[this.attributeCount].kind = AttributeIndirectKind.FilteredStyle;
                    this.attributeCount++;
                }
            }
            else
            {
                this.attributeCount = attributes.Count;
                this.attributeIndirect = false;

                if (this.callback != null && !this.tagCallbackRequested && !this.ignoreAttrCallback)
                {
                    for (int i = 0; i < attributes.Count; i++)
                    {
                        attribute = attributes[i];

                        if (attribute.IsAttrBegin)
                        {
                            attrAction = HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrAction;

                            if (0 != (attrAction & HtmlFilterData.FilterAction.HasExceptions) &&
                                0 != (HtmlFilterData.filterInstructions[(int)this.token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions))
                            {
                                for (int j = 0; j < HtmlFilterData.filterExceptions.Length; j++)
                                {
                                    if (HtmlFilterData.filterExceptions[j].tagNameIndex == this.token.NameIndex &&
                                        HtmlFilterData.filterExceptions[j].attrNameIndex == attribute.NameIndex)
                                    {
                                        attrAction = HtmlFilterData.filterExceptions[j].action;
                                        break;
                                    }
                                }
                            }

                            if (0 != (attrAction & HtmlFilterData.FilterAction.Callback))
                            {
                                if (this.token.IsTagBegin || !this.truncateForCallback)
                                {
                                    this.attributeTriggeredCallback = this.attributeTriggeredCallback || !this.tagCallbackRequested;
                                    this.tagCallbackRequested = true;

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int WhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            int length = 0;

            while (remainingLength != 0)
            {
                if (!ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
                {
                    break;
                }

                length++;
                remainingLength--;
            }

            return length;
        }

        private static int NonWhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            int length = 0;

            while (remainingLength != 0)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
                {
                    break;
                }

                length++;
                remainingLength--;
            }

            return length;
        }

        private void PostProcessStartTag()
        {
            if (this.tagIndex == HtmlTagIndex.Style && this.styleIsCSS)
            {
                this.insideCSS = true;
            }
        }

        private void PreProcessEndTag()
        {
            InternalDebug.Assert(this.token.IsTagBegin && this.token.IsEndTag);

            if (this.tagIndex > HtmlTagIndex.Unknown)
            {
                if (0 != (HtmlDtd.tags[(int)this.tagIndex].literal & HtmlDtd.Literal.Entities))
                {
                    if (this.tagIndex == HtmlTagIndex.Style)
                    {
                        if (this.insideCSS && this.filterHtml)
                        {
                            this.FlushCssInStyleTag();
                        }
                    }

                    this.insideCSS = false;
                    this.styleIsCSS = true;
                }

                if (this.tagIndex == HtmlTagIndex.PlainText || this.tagIndex == HtmlTagIndex.Xmp)
                {
                    if (this.filterHtml || (this.hasTailInjection && this.tagIndex == HtmlTagIndex.PlainText))
                    {
                        this.tagDropped = true;
                        this.writer.WriteEndTag(HtmlNameIndex.Pre);
                        this.writer.WriteEndTag(HtmlNameIndex.TT);
                    }
                    else if (this.tagIndex == HtmlTagIndex.PlainText)
                    {
                        if (this.normalizedInput)
                        {
                            this.tagDropped = true;
                            this.dropLevel = 0;
                            this.endTagActionStackTop = 0;
                        }
                    }
                }
                else if (this.tagIndex == HtmlTagIndex.Body)
                {
                    if (this.headDivUnterminated && this.dropLevel != 0)
                    {
                        this.writer.WriteEndTag(HtmlNameIndex.Div);
                        this.writer.WriteAutoNewLine(true);

                        this.headDivUnterminated = false;
                    }

                    if (this.outputFragment)
                    {
                        this.tagIndex = HtmlTagIndex.Div;
                    }
                }
                else if (this.tagIndex == HtmlTagIndex.TC)
                {
                    this.tagDropped = true;
                }
                else if (this.tagIndex == HtmlTagIndex.Image)
                {
                    if (this.filterHtml)
                    {
                        this.tagIndex = HtmlTagIndex.Img;
                    }
                }
            }
            else if (this.tagIndex == HtmlTagIndex.Unknown && this.filterHtml)
            {
                this.tagDropped = true;
            }
        }

        internal void CopyInputTag(bool copyTagAttributes)
        {
            if (this.token.IsTagBegin)
            {
                this.writer.WriteTagBegin(HtmlDtd.tags[(int)this.tagIndex].nameIndex, null, this.token.IsEndTag, this.token.IsAllowWspLeft, this.token.IsAllowWspRight);
            }

            if (this.tagIndex <= HtmlTagIndex.Unknown)
            {
                if (this.tagIndex < HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(!this.token.HasNameFragment && this.attributeCount == 0);

                    this.token.UnstructuredContent.WriteTo(this.writer.WriteUnstructuredTagContent());

                    if (this.token.IsTagEnd)
                    {
                        this.CopyPendingStateFlag = CopyPendingState.NotPending;
                    }
                    else
                    {
                        this.CopyPendingStateFlag = CopyPendingState.TagCopyPending;
                    }
                    return;
                }
                else
                {
                    if (this.token.HasNameFragment)
                    {
                        this.token.Name.WriteTo(this.writer.WriteTagName());

                        if (!this.token.IsTagNameEnd)
                        {
                            InternalDebug.Assert(this.token.Attributes.Count == 0 && !this.token.IsTagEnd);

                            if (!copyTagAttributes)
                            {
                                this.CopyPendingStateFlag = CopyPendingState.TagNameCopyPending;
                                return;
                            }
                        }
                    }
                }
            }

            if (!copyTagAttributes)
            {
                this.CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }

            if (this.attributeCount != 0)
            {
                this.CopyInputTagAttributes();
            }

            if (this.token.IsTagEnd)
            {
                this.CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                this.CopyPendingStateFlag = CopyPendingState.TagCopyPending;
            }
        }

        private void CopyInputTagAttributes()
        {
            InternalDebug.Assert(!this.token.IsEndTag);

            for (int i = 0; i < this.attributeCount; i++)
            {
                this.CopyInputAttribute(i);
            }
        }

        internal void CopyInputAttribute(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle)
            {
                if (!this.tagCallbackRequested)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);

                    InternalDebug.Assert(this.cssParserInput != null);
                    if (!this.cssParserInput.IsEmpty)
                    {
                        this.FlushCssInStyleAttribute(writer);
                    }
                    else
                    {
                        this.writer.WriteAttributeValueInternal(String.Empty);
                    }
                    return;
                }

                this.VirtualizeFilteredStyle(index);
                kind = AttributeIndirectKind.VirtualFilteredStyle;
            }

            bool endAttr = true;

            if (kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                this.writer.WriteAttributeName(HtmlNameIndex.Style);

                int vi = this.GetAttributeVirtualEntryIndex(index);

                if (this.attributeVirtualList[vi].length != 0)
                {
                    this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[vi].offset, this.attributeVirtualList[vi].length);
                }
                else
                {
                    this.writer.WriteAttributeValueInternal(String.Empty);
                }
            }
            else
            {
                HtmlAttribute attribute = this.GetAttribute(index);

                InternalDebug.Assert(!attribute.IsDeleted);

                if (attribute.IsAttrBegin)
                {
                    if (attribute.NameIndex != HtmlNameIndex.Unknown)
                    {
                        this.writer.WriteAttributeName(attribute.NameIndex);
                    }
                }

                if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
                {
                    attribute.Name.WriteTo(this.writer.WriteAttributeName());
                }

                if (kind == AttributeIndirectKind.NameOnlyFragment)
                {
                    InternalDebug.Assert(!attribute.IsAttrEnd);
                    endAttr = false;
                }
                else if (kind == AttributeIndirectKind.EmptyValue)
                {
                    this.writer.WriteAttributeValueInternal(String.Empty);
                }
                else if (kind == AttributeIndirectKind.Virtual)
                {
                    int vi = this.GetAttributeVirtualEntryIndex(index);

                    if (this.attributeVirtualList[vi].length != 0)
                    {
                        this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[vi].offset, this.attributeVirtualList[vi].length);
                    }
                    else
                    {
                        this.writer.WriteAttributeValueInternal(String.Empty);
                    }
                }
                else
                {
                    if (attribute.HasValueFragment)
                    {
                        attribute.Value.WriteTo(this.writer.WriteAttributeValue());
                    }

                    endAttr = attribute.IsAttrEnd;
                }
            }

            if (endAttr)
            {
                this.CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                this.CopyPendingStateFlag = CopyPendingState.AttributeCopyPending;
            }
        }

        internal void CopyInputAttributeName(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                this.writer.WriteAttributeName(HtmlNameIndex.Style);
                return;
            }

            HtmlAttribute attribute = this.GetAttribute(index);

            InternalDebug.Assert(!attribute.IsDeleted);

            if (attribute.IsAttrBegin)
            {
                if (attribute.NameIndex != HtmlNameIndex.Unknown)
                {
                    this.writer.WriteAttributeName(attribute.NameIndex);
                }
            }

            if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
            {
                attribute.Name.WriteTo(this.writer.WriteAttributeName());
            }

            if (attribute.IsAttrNameEnd)
            {
                this.CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                this.CopyPendingStateFlag = CopyPendingState.AttributeNameCopyPending;
            }
        }

        internal void CopyInputAttributeValue(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            bool endAttr = true;

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    if (!this.tagCallbackRequested)
                    {
                        InternalDebug.Assert(this.cssParserInput != null);
                        if (!this.cssParserInput.IsEmpty)
                        {
                            this.FlushCssInStyleAttribute(writer);
                        }
                        else
                        {
                            this.writer.WriteAttributeValueInternal(String.Empty);
                        }
                        return;
                    }

                    this.VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    int vi = this.GetAttributeVirtualEntryIndex(index);

                    if (this.attributeVirtualList[vi].length != 0)
                    {
                        this.writer.WriteAttributeValueInternal(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[vi].offset, this.attributeVirtualList[vi].length);
                    }
                    else
                    {
                        this.writer.WriteAttributeValueInternal(String.Empty);
                    }
                }
                else if (kind == AttributeIndirectKind.NameOnlyFragment)
                {
                    InternalDebug.Assert(!this.GetAttribute(index).IsAttrEnd);
                    endAttr = false;
                }
                else if (kind == AttributeIndirectKind.EmptyValue)
                {
                    this.writer.WriteAttributeValueInternal(String.Empty);
                }
            }
            else
            {
                HtmlAttribute attribute = this.GetAttribute(index);

                InternalDebug.Assert(!attribute.IsDeleted);

                if (attribute.HasValueFragment)
                {
                    attribute.Value.WriteTo(this.writer.WriteAttributeValue());
                }

                endAttr = attribute.IsAttrEnd;
            }

            if (endAttr)
            {
                this.CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                this.CopyPendingStateFlag = CopyPendingState.AttributeValueCopyPending;
            }
        }

        private static object lockObject = new object();
        private static bool textConvertersConfigured;

        private static Dictionary<string, string> safeUrlDictionary;

        // Orphaned WPL code.
#if false
        internal static void RefreshConfiguration()
        {
            textConvertersConfigured = false;
        }

        internal static bool TestSafeUrlSchema(string schema)
        {
            if (schema.Length < 2 || schema.Length > 20)
            {
                return false;
            }

            if (!textConvertersConfigured)
            {
                ConfigureTextConverters();
            }

            if (safeUrlDictionary.ContainsKey(schema))
            {
                return true;
            }

            return false;
        }
#endif

        private static void ConfigureTextConverters()
        {
            lock (lockObject)
            {
                if (!textConvertersConfigured)
                {
                    /* example of the configuration:

                    <TextConverters>

                        <SafeUrlScheme Add="foo bar buz"/>
                        -- or --
                        <SafeUrlScheme Override="foo bar buz"/>

                    </TextConverters>
                    */

                    safeUrlDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    bool safeUrlSchemeListSpecified = false;
                    bool safeUrlSchemeListAdd = true;

                    CtsConfigurationSetting safeUrlSchemeSetting = ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "SafeUrlScheme");

                    if (safeUrlSchemeSetting != null)
                    {
                        if (safeUrlSchemeSetting.Arguments.Count != 1 || (!safeUrlSchemeSetting.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase) &&
                                !safeUrlSchemeSetting.Arguments[0].Name.Equals("Override", StringComparison.OrdinalIgnoreCase)))
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                        }
                        else
                        {
                            safeUrlSchemeListAdd = safeUrlSchemeSetting.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase);

                            string[] entries = safeUrlSchemeSetting.Arguments[0].Value.Split(new char[] { ',', ' ', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
                            string badEntries = "";

                            foreach (string entry in entries)
                            {
                                string lowerEntry = entry.Trim().ToLower();
                                bool badEntry = false;

                                foreach (char ch in lowerEntry)
                                {
                                    if (ch > 0x7F || (!Char.IsLetterOrDigit(ch) && ch != '_' && ch != '-' && ch != '+'))
                                    {
                                        if (badEntries.Length != 0)
                                        {
                                            badEntries += " ";
                                        }
                                        badEntries += entry;
                                        badEntry = true;
                                        break;
                                    }
                                }

                                if (!badEntry && !safeUrlDictionary.ContainsKey(lowerEntry))
                                {
                                    safeUrlDictionary.Add(lowerEntry, null);
                                }
                            }

                            if (badEntries.Length != 0)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                            }

                            safeUrlSchemeListSpecified = true;
                        }
                    }

                    if (!safeUrlSchemeListSpecified || safeUrlSchemeListAdd)
                    {
                        safeUrlDictionary["http"] = null;
                        safeUrlDictionary["https"] = null;
                        safeUrlDictionary["ftp"] = null;
                        safeUrlDictionary["file"] = null;
                        safeUrlDictionary["mailto"] = null;
                        safeUrlDictionary["news"] = null;
                        safeUrlDictionary["gopher"] = null;
                        safeUrlDictionary["about"] = null;
                        safeUrlDictionary["wais"] = null;
                        safeUrlDictionary["cid"] = null;
                        safeUrlDictionary["mhtml"] = null;
                        safeUrlDictionary["ipp"] = null;
                        safeUrlDictionary["msdaipp"] = null;
                        safeUrlDictionary["meet"] = null;
                        safeUrlDictionary["tel"] = null;
                        safeUrlDictionary["sip"] = null;
                        safeUrlDictionary["conf"] = null;
                        safeUrlDictionary["im"] = null;
                        safeUrlDictionary["callto"] = null;
                        safeUrlDictionary["notes"] = null;
                        safeUrlDictionary["onenote"] = null;
                        safeUrlDictionary["groove"] = null;
                        safeUrlDictionary["mms"] = null;
                    }

                    textConvertersConfigured = true;
                }
            }
        }

        private static bool SafeUrlSchema(char[] urlBuffer, int schemaLength)
        {
            if (schemaLength < 2 || schemaLength > 20)
            {
                return false;
            }

            if (!textConvertersConfigured)
            {
                ConfigureTextConverters();
            }

            if (safeUrlDictionary.ContainsKey(new string(urlBuffer, 0, schemaLength)))
            {
                return true;
            }

            return false;
        }

        private enum CheckUrlResult
        {
            Inconclusive,
            Unsafe,
            Safe,
            LocalHyperlink,
        }

        private static CheckUrlResult CheckUrl(char[] urlBuffer, int urlLength, bool callbackRequested)
        {
            int i;

            if (urlLength > 0 && urlBuffer[0] == '#')
            {
                return CheckUrlResult.LocalHyperlink;
            }

            for (i = 0; i < urlLength; i++)
            {
                if (urlBuffer[i] == '/' || urlBuffer[i] == '\\')
                {
                    if (i == 0 && urlLength > 1 && (urlBuffer[1] == '/' || urlBuffer[1] == '\\'))
                    {
                        return callbackRequested ? CheckUrlResult.Safe : CheckUrlResult.Unsafe;
                    }

                    return CheckUrlResult.Safe;
                }
                if (urlBuffer[i] == '?' || urlBuffer[i] == '#' || urlBuffer[i] == ';')
                {
                    return CheckUrlResult.Safe;
                }
                if (urlBuffer[i] == ':')
                {
                    if (SafeUrlSchema(urlBuffer, i))
                    {
                        return CheckUrlResult.Safe;
                    }

                    if (callbackRequested)
                    {
                        if (i == 1 &&
                            urlLength > 2 &&
                            ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(urlBuffer[0])) &&
                            (urlBuffer[2] == '/' || urlBuffer[2] == '\\'))
                        {
                            return CheckUrlResult.Safe;
                        }
                        else
                        {
                            BufferString url = new BufferString(urlBuffer, 0, urlLength);

                            if (url.EqualsToLowerCaseStringIgnoreCase("objattph://") ||
                                url.EqualsToLowerCaseStringIgnoreCase("rtfimage://"))
                            {
                                return CheckUrlResult.Safe;
                            }
                        }
                    }

                    return CheckUrlResult.Unsafe;
                }
            }

            InternalDebug.Assert(i == urlLength);

            return CheckUrlResult.Inconclusive;
        }

        // Orphaned WPL code.
#if false
        internal static bool IsUrlSafe(string url, bool callbackRequested)
        {
            
            char[] urlBuffer = url.ToCharArray();

            switch (CheckUrl(urlBuffer, urlBuffer.Length, callbackRequested))
            {
                case CheckUrlResult.Safe:
                case CheckUrlResult.Inconclusive:
                case CheckUrlResult.LocalHyperlink:

                    return true;
            }

            return false;
        }
#endif

        private int AllocateVirtualEntry(int index, int offset, int length)
        {
            if (this.attributeVirtualList == null)
            {
                this.attributeVirtualList = new AttributeVirtualEntry[4];
            }
            else if (this.attributeVirtualList.Length == this.attributeVirtualCount)
            {
                AttributeVirtualEntry[] newVirtualList = new AttributeVirtualEntry[this.attributeVirtualList.Length * 2];
                Array.Copy(this.attributeVirtualList, 0, newVirtualList, 0, this.attributeVirtualCount);
                this.attributeVirtualList = newVirtualList;
            }

            int vi = this.attributeVirtualCount++;

            this.attributeVirtualList[vi].index = (short)index;
            this.attributeVirtualList[vi].offset = offset;
            this.attributeVirtualList[vi].length = length;
            this.attributeVirtualList[vi].position = 0;

            return vi;
        }

        private void VirtualizeFilteredStyle(int index)
        {
            int offset = this.attributeVirtualScratch.Length;
            this.FlushCssInStyleAttributeToVirtualScratch();
            int length = this.attributeVirtualScratch.Length - offset;

            int vi = this.AllocateVirtualEntry(this.attributeIndirectIndex[index + this.attributeSkipCount].index, offset, length);

            this.attributeIndirectIndex[index + this.attributeSkipCount].index = (short)vi;
            this.attributeIndirectIndex[index + this.attributeSkipCount].kind = AttributeIndirectKind.VirtualFilteredStyle;
        }

        private bool InjectMetaTagIfNecessary()
        {
            if (this.filterForFragment || !this.writer.HasEncoding)
            {
                this.metaInjected = true;
            }
            else
            {
                if (this.token.TokenId != HtmlTokenId.Restart && this.token.TokenId != HtmlTokenId.EncodingChange)
                {
                    if (this.writer.Encoding.CodePage == 65000)
                    {
                        this.OutputMetaTag();
                        this.metaInjected = true;
                    }
                    else if (this.token.TokenId == HtmlTokenId.Tag)
                    {
                        if (!this.insideHtml && this.token.TagIndex == HtmlTagIndex.Html)
                        {
                            if (this.token.IsTagEnd)
                            {
                                this.insideHtml = true;
                            }
                        }
                        else if (!this.insideHead && this.token.TagIndex == HtmlTagIndex.Head)
                        {
                            if (this.token.IsTagEnd)
                            {
                                this.insideHead = true;
                            }
                        }
                        else if (this.token.TagIndex <= HtmlTagIndex._ASP)
                        {
                        }
                        else
                        {
                            InternalDebug.Assert(this.token.IsTagBegin);

                            if (this.insideHtml && !this.insideHead)
                            {
                                this.writer.WriteNewLine(true);
                                this.writer.WriteStartTag(HtmlNameIndex.Head);
                                this.writer.WriteNewLine(true);

                                this.OutputMetaTag();

                                this.writer.WriteEndTag(HtmlNameIndex.Head);
                                this.writer.WriteNewLine(true);
                            }
                            else
                            {
                                if (this.insideHead)
                                {
                                    this.writer.WriteNewLine(true);
                                }
                                this.OutputMetaTag();
                            }

                            this.metaInjected = true;
                        }
                    }
                    else if (this.token.TokenId == HtmlTokenId.Text)
                    {
                        if (this.token.IsWhitespaceOnly)
                        {
                            return false;
                        }

                        this.token.Text.StripLeadingWhitespace();

                        if (this.insideHtml && !this.insideHead)
                        {
                            this.writer.WriteNewLine(true);
                            this.writer.WriteStartTag(HtmlNameIndex.Head);
                            this.writer.WriteNewLine(true);

                            this.OutputMetaTag();

                            this.writer.WriteEndTag(HtmlNameIndex.Head);
                            this.writer.WriteNewLine(true);
                        }
                        else
                        {
                            if (this.insideHead)
                            {
                                this.writer.WriteNewLine(true);
                            }

                            this.OutputMetaTag();
                        }

                        this.metaInjected = true;
                    }
                }
            }

            return true;
        }

        private void OutputMetaTag()
        {
            InternalDebug.Assert(this.CopyPendingStateFlag == CopyPendingState.NotPending);
            InternalDebug.Assert(!this.filterForFragment);

            InternalDebug.Assert(this.writer.HasEncoding);

            Encoding encoding = this.writer.Encoding;

            if (encoding.CodePage == 65000)
            {
                this.writer.Encoding = Encoding.ASCII;
            }

            this.writer.WriteStartTag(HtmlNameIndex.Meta);
            this.writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
            this.writer.WriteAttributeName(HtmlNameIndex.Content);
            this.writer.WriteAttributeValueInternal("text/html; charset=");
            this.writer.WriteAttributeValueInternal(Charset.GetCharset(encoding.CodePage).Name);

            if (encoding.CodePage == 65000)
            {
                this.writer.WriteTagEnd();
                this.writer.Encoding = encoding;
            }
        }

        private AttributeIndirectKind GetAttributeIndirectKind(int index)
        {
            InternalDebug.Assert(index >= 0 && index < this.attributeCount);

            return this.attributeIndirect ? this.attributeIndirectIndex[index + this.attributeSkipCount].kind : AttributeIndirectKind.PassThrough;
        }

        private int GetAttributeVirtualEntryIndex(int index)
        {
            InternalDebug.Assert(index >= 0 && index < this.attributeCount);
            InternalDebug.Assert(this.attributeIndirect);
            InternalDebug.Assert(this.GetAttributeIndirectKind(index) == AttributeIndirectKind.Virtual ||
                                this.GetAttributeIndirectKind(index) == AttributeIndirectKind.VirtualFilteredStyle);

            return this.attributeIndirectIndex[index + this.attributeSkipCount].index;
        }

        private HtmlAttribute GetAttribute(int index)
        {
            InternalDebug.Assert(index >= 0 && index < this.attributeCount);

            InternalDebug.Assert(this.GetAttributeIndirectKind(index) != AttributeIndirectKind.FilteredStyle &&
                                this.GetAttributeIndirectKind(index) != AttributeIndirectKind.VirtualFilteredStyle);

            if (!this.attributeIndirect)
            {
                return this.token.Attributes[index + this.attributeSkipCount];
            }
            else if (this.attributeIndirectIndex[index + this.attributeSkipCount].kind != AttributeIndirectKind.Virtual)
            {
                return this.token.Attributes[this.attributeIndirectIndex[index + this.attributeSkipCount].index];
            }
            else
            {
                return this.token.Attributes[this.attributeVirtualList[this.attributeIndirectIndex[index + this.attributeSkipCount].index].index];
            }
        }

        internal HtmlAttributeId GetAttributeNameId(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                return HtmlAttributeId.Style;
            }

            HtmlAttribute attribute = this.GetAttribute(index);
            return HtmlNameData.names[(int)attribute.NameIndex].publicAttributeId;
        }

        private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);

        internal HtmlAttributeParts GetAttributeParts(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                return CompleteAttributeParts;
            }

            HtmlAttribute attribute = this.GetAttribute(index);

            if (kind == AttributeIndirectKind.NameOnlyFragment)
            {
                InternalDebug.Assert((attribute.MajorPart & HtmlToken.AttrPartMajor.End) != HtmlToken.AttrPartMajor.End);

                InternalDebug.Assert((attribute.MinorPart & HtmlToken.AttrPartMinor.ContinueName) != 0);

                return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart & ~HtmlToken.AttrPartMinor.CompleteValue);
            }
            else if (kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.Virtual)
            {
                return new HtmlAttributeParts(attribute.MajorPart | HtmlToken.AttrPartMajor.End, attribute.MinorPart | HtmlToken.AttrPartMinor.CompleteValue);
            }

            return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart);
        }

        internal string GetAttributeName(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                InternalDebug.Assert(this.tagHasFilteredStyleAttribute);

                return HtmlNameData.names[(int)HtmlNameIndex.Style].name;
            }

            HtmlAttribute attribute = this.GetAttribute(index);

            if (attribute.NameIndex > HtmlNameIndex.Unknown)
            {
                return attribute.IsAttrBegin ? HtmlNameData.names[(int)attribute.NameIndex].name : String.Empty;
            }
            else if (attribute.HasNameFragment)
            {
                return attribute.Name.GetString(int.MaxValue);
            }
            else
            {
                return attribute.IsAttrBegin ? "?" : String.Empty;
            }
        }

        internal string GetAttributeValue(int index)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    this.VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    int vi = this.GetAttributeVirtualEntryIndex(index);
                    if (this.attributeVirtualList[vi].length != 0)
                    {
                        return new string(this.attributeVirtualScratch.Buffer, this.attributeVirtualList[vi].offset, this.attributeVirtualList[vi].length);
                    }

                    return String.Empty;
                }

                InternalDebug.Assert(kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.NameOnlyFragment);
                return String.Empty;
            }

            HtmlAttribute attribute = this.GetAttribute(index);

            if (!attribute.HasValueFragment)
            {
                return String.Empty;
            }

            return attribute.Value.GetString(int.MaxValue);
        }

        // Orphaned WPL code.
#if false
        internal int ReadAttributeValue(int index, char[] buffer, int offset, int count)
        {
            AttributeIndirectKind kind = this.GetAttributeIndirectKind(index);

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    
                    this.VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    
                    int vi = this.GetAttributeVirtualEntryIndex(index);

                    InternalDebug.Assert(this.attributeVirtualList[vi].position < this.attributeVirtualList[vi].length);

                    int countRead = Math.Min(this.attributeVirtualList[vi].length - this.attributeVirtualList[vi].position, count);
                    if (countRead != 0)
                    {
                        Buffer.BlockCopy(this.attributeVirtualScratch.Buffer, 2 * (this.attributeVirtualList[vi].offset + this.attributeVirtualList[vi].position), buffer, offset, 2 * countRead);
                        this.attributeVirtualList[vi].position += countRead;
                    }

                    return countRead;
                }

                InternalDebug.Assert(kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.NameOnlyFragment);
                return 0;
            }

            HtmlAttribute attribute = this.GetAttribute(index);

            if (!attribute.HasValueFragment)
            {
                return 0;
            }

            return attribute.Value.Read(buffer, offset, count);
        }
#endif

        internal void WriteTag(bool copyTagAttributes)
        {
            this.CopyInputTag(copyTagAttributes);
        }

        internal void WriteAttribute(int index, bool writeName, bool writeValue)
        {
            if (writeName)
            {
                if (writeValue)
                {
                    this.CopyInputAttribute(index);
                }
                else
                {
                    this.CopyInputAttributeName(index);
                }
            }
            else if (writeValue)
            {
                this.CopyInputAttributeValue(index);
            }
        }

        private void AppendCssFromTokenText()
        {
            if (null == this.cssParserInput)
            {
                this.cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, this.progressMonitor);
                this.cssParser = new CssParser(this.cssParserInput, 4 * 1024, false);
            }

            this.token.Text.WriteTo(this.cssParserInput);
        }

        private void AppendCss(string css)
        {
            if (null == this.cssParserInput)
            {
                this.cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, this.progressMonitor);
                this.cssParser = new CssParser(this.cssParserInput, 4 * 1024, false);
            }

            this.cssParserInput.Write(css);
        }

        private void AppendCssFromAttribute(HtmlAttribute attribute)
        {
            if (null == this.cssParserInput)
            {
                this.cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, this.progressMonitor);
                this.cssParser = new CssParser(this.cssParserInput, 4 * 1024, false);
            }

            attribute.Value.Rewind();
            attribute.Value.WriteTo(this.cssParserInput);
        }

        private void FlushCssInStyleTag()
        {
            if (null != this.cssParserInput)
            {
                this.writer.WriteNewLine();
                this.writer.WriteMarkupText("<!--");
                this.writer.WriteNewLine();

                bool agressiveFiltering = false;

                if (this.smallCssBlockThreshold != -1 && this.cssParserInput.MaxTokenSize > this.smallCssBlockThreshold)
                {
                    agressiveFiltering = true;
                }

                this.cssParser.SetParseMode(CssParseMode.StyleTag);

                CssTokenId tokenId;
                bool firstProperty = true;

                ITextSinkEx sink = this.writer.WriteText();

                do
                {
                    tokenId = this.cssParser.Parse();

                    if ((CssTokenId.RuleSet == tokenId || CssTokenId.AtRule == tokenId) &&
                        this.cssParser.Token.Selectors.ValidCount != 0 &&
                        this.cssParser.Token.Properties.ValidCount != 0)
                    {
                        bool selectorOk = this.CopyInputCssSelectors(this.cssParser.Token.Selectors, sink, agressiveFiltering);

                        if (selectorOk)
                        {
                            if (this.cssParser.Token.IsPropertyListBegin)
                            {
                                sink.Write("\r\n\t{");
                            }

                            this.CopyInputCssProperties(true, this.cssParser.Token.Properties, sink, ref firstProperty);

                            if (this.cssParser.Token.IsPropertyListEnd)
                            {
                                sink.Write("}\r\n");
                                firstProperty = true;
                            }
                        }
                    }
                }
                while (CssTokenId.EndOfFile != tokenId);

                this.cssParserInput.Reset();
                this.cssParser.Reset();

                this.writer.WriteMarkupText("-->");
                this.writer.WriteNewLine();
            }
        }

        private void FlushCssInStyleAttributeToVirtualScratch()
        {
            this.cssParser.SetParseMode(CssParseMode.StyleAttribute);

            if (this.virtualScratchSink == null)
            {
                this.virtualScratchSink = new VirtualScratchSink(this, int.MaxValue);
            }

            CssTokenId tokenId;
            bool firstProperty = true;
            do
            {
                tokenId = this.cssParser.Parse();

                if (CssTokenId.Declarations == tokenId && this.cssParser.Token.Properties.ValidCount != 0)
                {
                    this.CopyInputCssProperties(false, this.cssParser.Token.Properties, this.virtualScratchSink, ref firstProperty);
                }
            }
            while (CssTokenId.EndOfFile != tokenId);

            this.cssParserInput.Reset();
            this.cssParser.Reset();
        }

        private void FlushCssInStyleAttribute(HtmlWriter writer)
        {
            this.cssParser.SetParseMode(CssParseMode.StyleAttribute);

            ITextSinkEx sink = writer.WriteAttributeValue();

            CssTokenId tokenId;
            bool firstProperty = true;
            do
            {
                tokenId = this.cssParser.Parse();

                if (CssTokenId.Declarations == tokenId && this.cssParser.Token.Properties.ValidCount != 0)
                {
                    this.CopyInputCssProperties(false, this.cssParser.Token.Properties, sink, ref firstProperty);
                }
            }
            while (CssTokenId.EndOfFile != tokenId);

            this.cssParserInput.Reset();
            this.cssParser.Reset();
        }

        private bool CopyInputCssSelectors(CssToken.SelectorEnumerator selectors, ITextSinkEx sink, bool agressiveFiltering)
        {
            bool atLeastOneOk = false;
            bool lastOk = false;

            selectors.Rewind();

            foreach (CssSelector selector in selectors)
            {
                if (!selector.IsDeleted)
                {
                    if (lastOk)
                    {
                        if (selector.Combinator == CssSelectorCombinator.None)
                        {
                            sink.Write(", ");
                        }
                        else if (selector.Combinator == CssSelectorCombinator.Descendant)
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            sink.Write(' ');
                        }
                        else if (selector.Combinator == CssSelectorCombinator.Adjacent)
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            sink.Write(" + ");
                        }
                        else
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            InternalDebug.Assert(selector.Combinator == CssSelectorCombinator.Child);
                            sink.Write(" > ");
                        }
                    }

                    lastOk = this.CopyInputCssSelector(selector, sink, agressiveFiltering);
                    atLeastOneOk = atLeastOneOk || lastOk;
                }
            }

            return atLeastOneOk;
        }

        private bool CopyInputCssSelector(CssSelector selector, ITextSinkEx sink, bool agressiveFiltering)
        {
            InternalDebug.Assert(!selector.IsDeleted);

            if (this.filterForFragment)
            {
                if (!selector.HasClassFragment ||
                    (selector.ClassType != CssSelectorClassType.Regular &&
                     selector.ClassType != CssSelectorClassType.Hash))
                {
                    return false;
                }
#if false
                string className = selector.ClassName.GetString(256);

                if (!className.StartsWith("Mso", StringComparison.Ordinal) &&
                    !className.StartsWith("NL", StringComparison.Ordinal) &&
                    !className.StartsWith("email", StringComparison.OrdinalIgnoreCase) &&
                    !className.StartsWith("outlook", StringComparison.OrdinalIgnoreCase))
                {
                    
                    return false;
                }
#endif
            }

            if (agressiveFiltering)
            {
                if (!selector.HasClassFragment || selector.ClassType != CssSelectorClassType.Regular)
                {
                    return false;
                }

                string className = selector.ClassName.GetString(256);

                InternalDebug.Assert(className.Length > 0);

                if (!className.Equals("MsoNormal", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            if (selector.NameId != HtmlNameIndex.Unknown && selector.NameId != HtmlNameIndex._NOTANAME)
            {
                sink.Write(HtmlNameData.names[(int)selector.NameId].name);
            }
            else if (selector.HasNameFragment)
            {
                selector.Name.WriteOriginalTo(sink);
            }

            if (selector.HasClassFragment)
            {
                if (selector.ClassType == CssSelectorClassType.Regular)
                {
                    sink.Write(".");
                }
                else if (selector.ClassType == CssSelectorClassType.Hash)
                {
                    sink.Write("#");
                }
                else if (selector.ClassType == CssSelectorClassType.Pseudo)
                {
                    sink.Write(":");
                }
                else
                {
                    InternalDebug.Assert(selector.ClassType == CssSelectorClassType.Attrib);
                }

                if (this.outputFragment)
                {
                    InternalDebug.Assert(selector.ClassType == CssSelectorClassType.Hash || selector.ClassType == CssSelectorClassType.Regular);

                    sink.Write(NamePrefix);
                }

                selector.ClassName.WriteOriginalTo(sink);
            }

            return true;
        }

        private void CopyInputCssProperties(bool inTag, CssToken.PropertyEnumerator properties, ITextSinkEx sink, ref bool firstProperty)
        {
            properties.Rewind();

            foreach (CssProperty property in properties)
            {
                if (property.IsPropertyBegin && !property.IsDeleted)
                {
                    CssData.FilterAction action = CssData.filterInstructions[(int)property.NameId].propertyAction;

                    if (CssData.FilterAction.CheckContent == action)
                    {
                        if (property.NameId == CssNameIndex.Display &&
                            property.HasValueFragment &&
                            property.Value.CaseInsensitiveContainsSubstring("none") &&
                            !this.preserveDisplayNoneStyle)
                        {
                            action = CssData.FilterAction.Drop;
                        }
                        else if (property.NameId == CssNameIndex.Position &&
                            property.HasValueFragment &&
                            (property.Value.CaseInsensitiveContainsSubstring("absolute") ||
                              property.Value.CaseInsensitiveContainsSubstring("relative")) &&
                            this.outputFragment)
                        {
                            action = CssData.FilterAction.Drop;
                        }
                        else
                        {
                            action = CssData.FilterAction.Keep;
                        }
                    }

                    if (CssData.FilterAction.Keep == action)
                    {
                        if (firstProperty)
                        {
                            firstProperty = false;
                        }
                        else
                        {
                            sink.Write(inTag ? ";\r\n\t" : "; ");
                        }

                        CopyInputCssProperty(property, sink);
                    }
                }
            }
        }

        private static void CopyInputCssProperty(CssProperty property, ITextSinkEx sink)
        {
            InternalDebug.Assert(!property.IsDeleted);

            if (property.IsPropertyBegin)
            {
                if (property.NameId != CssNameIndex.Unknown)
                {
                    sink.Write(CssData.names[(int)property.NameId].name);
                }
            }

            if (property.NameId == CssNameIndex.Unknown && property.HasNameFragment)
            {
                property.Name.WriteOriginalTo(sink);
            }

            if (property.IsPropertyNameEnd)
            {
                sink.Write(":");
            }

            if (property.HasValueFragment)
            {
                property.Value.WriteEscapedOriginalTo(sink);
            }
        }

        internal class VirtualScratchSink : ITextSinkEx
        {
            private HtmlToHtmlConverter converter;
            private int maxLength;

            public VirtualScratchSink(HtmlToHtmlConverter converter, int maxLength)
            {
                this.converter = converter;
                this.maxLength = maxLength;
            }

            public bool IsEnough { get { return this.converter.attributeVirtualScratch.Length >= this.maxLength; } }

            public void Write(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(!this.IsEnough);
                this.converter.attributeVirtualScratch.Append(buffer, offset, count, this.maxLength);
            }

            public void Write(int ucs32Char)
            {
                InternalDebug.Assert(!this.IsEnough);

                if (Token.LiteralLength(ucs32Char) == 1)
                {
                    this.converter.attributeVirtualScratch.Append((char)ucs32Char, this.maxLength);
                }
                else
                {
                    this.converter.attributeVirtualScratch.Append(Token.LiteralFirstChar(ucs32Char), this.maxLength);
                    if (!this.IsEnough)
                    {
                        this.converter.attributeVirtualScratch.Append(Token.LiteralLastChar(ucs32Char), this.maxLength);
                    }
                }
            }

            public void Write(string value)
            {
                InternalDebug.Assert(!this.IsEnough);

                this.converter.attributeVirtualScratch.Append(value, this.maxLength);
            }

            public void WriteNewLine()
            {
                InternalDebug.Assert(!this.IsEnough);

                this.converter.attributeVirtualScratch.Append('\r', this.maxLength);

                if (!this.IsEnough)
                {
                    this.converter.attributeVirtualScratch.Append('\n', this.maxLength);
                }
            }
        }
    }

    internal class HtmlToHtmlTagContext : HtmlTagContext
    {
        private HtmlToHtmlConverter converter;

        public HtmlToHtmlTagContext(HtmlToHtmlConverter converter)
        {
            this.converter = converter;
        }

        internal override string GetTagNameImpl()
        {
            if (this.TagNameIndex > HtmlNameIndex.Unknown)
            {
                return this.TagParts.Begin ? HtmlNameData.names[(int)this.TagNameIndex].name : String.Empty;
            }
            else if (this.TagParts.Name)
            {
                return this.converter.token.Name.GetString(int.MaxValue);
            }
            else
            {
                return String.Empty;
            }
        }

        internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
        {
            return this.converter.GetAttributeNameId(attributeIndex);
        }

        internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
        {
            return this.converter.GetAttributeParts(attributeIndex);
        }

        internal override string GetAttributeNameImpl(int attributeIndex)
        {
            return this.converter.GetAttributeName(attributeIndex);
        }

        internal override string GetAttributeValueImpl(int attributeIndex)
        {
            return this.converter.GetAttributeValue(attributeIndex);
        }

        // Orphaned WPL code.
#if false
        internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
        {
            return this.converter.ReadAttributeValue(attributeIndex, buffer, offset, count);
        }
#endif

        internal override void WriteTagImpl(bool copyTagAttributes)
        {
            this.converter.WriteTag(copyTagAttributes);
        }

        internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
        {
            this.converter.WriteAttribute(attributeIndex, writeName, writeValue);
        }
    }
}
