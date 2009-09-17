// ***************************************************************
// <copyright file="HtmlNormalizingParser.cs" company="Microsoft">
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
    using System.Collections;
    using Microsoft.Exchange.Data.Internal;
    
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    internal class HtmlNormalizingParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        

        private HtmlParser parser;
        private IRestartable restartConsumer;

        private int maxElementStack;

        private Context context;                  
        private Context[] contextStack;           
        private int contextStackTop;

        private HtmlTagIndex[] elementStack;
        private int elementStackTop;

        private QueueItem[] queue;
        private int queueHead;
        private int queueTail;
        
        
        private int queueStart;

        private bool ensureHead = true;

        
        private int[] closeList;
        private HtmlTagIndex[] openList;

        private bool validRTC;
        private HtmlTagIndex tagIdRTC;

        
        private HtmlToken token;

        
        private HtmlToken inputToken;
        private bool ignoreInputTag;     

        
        private int currentRun;
        private int currentRunOffset;
        private int numRuns;

        private bool allowWspLeft;
        private bool allowWspRight;

        private SmallTokenBuilder tokenBuilder;

        
        private HtmlInjection injection;
        private DocumentState saveState;

        
        public HtmlNormalizingParser(
                    HtmlParser parser,
                    HtmlInjection injection,
                    bool ensureHead,
                    int maxNesting,
                    bool testBoundaryConditions,
                    Stream traceStream,
                    bool traceShowTokenNum,
                    int traceStopOnTokenNum)
        {
            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.injection = injection;
            if (injection != null)
            {
                this.saveState = new DocumentState();
            }

            this.ensureHead = ensureHead;

            int initialStackSize = testBoundaryConditions ? 1 : 32;
            this.maxElementStack = testBoundaryConditions ? 30 : maxNesting;

            this.openList = new HtmlTagIndex[8];
            this.closeList = new int[8];

            this.elementStack = new HtmlTagIndex[initialStackSize];
            

            this.contextStack = new Context[testBoundaryConditions ? 1 : 4];
            

            

            this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;

            
            this.context.type = HtmlDtd.ContextType.Root;
            this.context.textType = HtmlDtd.ContextTextType.Full;
            
            this.context.reject = HtmlDtd.SetId.Empty;
            
            
            
            
            

            this.queue = new QueueItem[testBoundaryConditions ? 1 : initialStackSize / 4];

            this.tokenBuilder = new SmallTokenBuilder();
        }

        
        private void Reinitialize()
        {
            this.elementStackTop = 0;

            this.contextStackTop = 0;

            this.ignoreInputTag = false;

            this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;

            this.context.topElement = 0;
            this.context.type = HtmlDtd.ContextType.Root;
            this.context.textType = HtmlDtd.ContextTextType.Full;
            this.context.accept = HtmlDtd.SetId.Null;
            this.context.reject = HtmlDtd.SetId.Empty;
            this.context.ignoreEnd = HtmlDtd.SetId.Null;
            this.context.hasSpace = false;
            this.context.eatSpace = false;
            this.context.oneNL = false;
            this.context.lastCh = '\0';

            this.queueHead = 0;
            this.queueTail = 0;
            this.queueStart = 0;

            this.validRTC = false;
            this.tagIdRTC = HtmlTagIndex._NULL;

            this.token = null;

            if (this.injection != null)
            {
                if (this.injection.Active)
                {
                    this.parser = (HtmlParser) this.injection.Pop();
                }

                this.injection.Reset();
            }
        }

        
        private enum QueueItemKind : byte
        {
            Empty,
            None,
            Eof,
            BeginElement,
            EndElement,
            OverlappedClose,
            OverlappedReopen,
            PassThrough,
            Space,
            Text,
            Suspend,
            InjectionBegin,
            InjectionEnd,
            
            EndLastTag,
        }

        
        [Flags]
        private enum QueueItemFlags : byte
        {
            AllowWspLeft = 0x01,
            AllowWspRight = 0x02,
        }

        
        public HtmlToken Token
        {
            get
            {
                return this.token;
            }
        }

        
        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        
        public HtmlTokenId Parse()
        {
            while (true)
            {
                if (!this.QueueEmpty())
                {
                    return this.GetTokenFromQueue();
                }

                this.Process(this.parser.Parse());
            }
        }

        
        bool IRestartable.CanRestart()
        {
            return this.restartConsumer != null && this.restartConsumer.CanRestart();
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (this.restartConsumer != null)
            {
                this.restartConsumer.Restart();
            }

            this.Reinitialize();
        }

        
        void IRestartable.DisableRestart()
        {
            if (this.restartConsumer != null)
            {
                this.restartConsumer.DisableRestart();
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(this.parser is IReusable);

            ((IReusable)this.parser).Initialize(newSourceOrDestination);

            this.Reinitialize();

            this.parser.SetRestartConsumer(this);
        }

        
        public void Initialize(string fragment, bool preformatedText)
        {
            this.parser.Initialize(fragment, preformatedText);

            this.Reinitialize();
        }

        
        void IDisposable.Dispose()
        {
            if (this.parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)this.parser).Dispose();
            }

            this.parser = null;
            this.restartConsumer = null;
            this.contextStack = null;   
            this.queue = null;
            this.closeList = null;
            this.openList = null;
            this.token = null;
            this.inputToken = null;
            this.tokenBuilder = null;

            GC.SuppressFinalize(this);
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return HtmlDtd.tags[(int)tagIndex];
        }

        
        private void Process(HtmlTokenId tokenId)
        {
            if (tokenId == HtmlTokenId.None)
            {
                InternalDebug.Assert(this.QueueEmpty());

                this.EnqueueHead(QueueItemKind.None);
                return;
            }

            InternalDebug.Assert(this.queueHead == this.queueTail || this.queue[this.queueHead].kind != QueueItemKind.Suspend || tokenId == HtmlTokenId.Tag || tokenId == HtmlTokenId.EndOfFile);

            this.inputToken = this.parser.Token;

            switch (tokenId)
            {
                case HtmlTokenId.Restart:

                    this.EnqueueTail(QueueItemKind.PassThrough);
                    break;

                case HtmlTokenId.EncodingChange:

                    this.EnqueueTail(QueueItemKind.PassThrough);
                    break;

                case HtmlTokenId.EndOfFile:

                    this.HandleTokenEof();
                    break;

                case HtmlTokenId.Tag:

                    InternalDebug.Assert(this.queue[this.queueHead].kind != QueueItemKind.Suspend || !this.inputToken.IsTagBegin);

                    if (this.parser.Token.NameIndex < HtmlNameIndex.Unknown)
                    {
                        this.HandleTokenSpecialTag(this.parser.Token);
                        break;
                    }

                    this.HandleTokenTag(this.parser.Token);
                    break;

                case HtmlTokenId.Text:

                    this.HandleTokenText(this.parser.Token);
                    break;

                default:

                    
                    InternalDebug.Assert(false, "unexpected HTML token");
                    break;
            }
        }

        
        private void HandleTokenEof()
        {
            
            InternalDebug.Assert(this.QueueEmpty());

            if (this.queueHead != this.queueTail && this.queue[this.queueHead].kind == QueueItemKind.Suspend)
            {
                
                

                QueueItem qi = this.DoDequeueFirst();
                this.EnqueueHead(
                            QueueItemKind.EndLastTag, 
                            qi.tagIndex, 
                            0 != (qi.flags & QueueItemFlags.AllowWspLeft),
                            0 != (qi.flags & QueueItemFlags.AllowWspRight));
                return;
            }

            if (this.injection != null && this.injection.Active)
            {
                

                
                this.CloseAllContainers(this.saveState.SavedStackTop);

                if (this.queueHead != this.queueTail)
                {
                    
                    return;
                }

                
                this.saveState.Restore(this);
                this.EnqueueHead(QueueItemKind.InjectionEnd, this.injection.InjectingHead ? 1 : 0);
                this.parser = (HtmlParser) this.injection.Pop();
                return;
            }

            

            
            
            
            
            

            if (this.injection != null)
            {
                int bodyLevel = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                if (bodyLevel == -1)
                {
                    

                    
                    

                    
                    this.CloseAllProhibitedContainers(GetTagDefinition(HtmlTagIndex.Body));

                    this.OpenContainer(HtmlTagIndex.Body);
                    return;
                }

                
                this.CloseAllContainers(bodyLevel + 1);

                if (this.queueHead != this.queueTail)
                {
                    
                    return;
                }

                if (this.injection.HaveTail && !this.injection.TailDone)
                {
                    

                    this.parser = (HtmlParser) this.injection.Push(false, this.parser);
                    this.saveState.Save(this, this.elementStackTop);
                    this.EnqueueTail(QueueItemKind.InjectionBegin, 0);
                    if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                    {
                        this.OpenContainer(HtmlTagIndex.TT);
                        this.OpenContainer(HtmlTagIndex.Pre);
                    }
                    return;
                }
            }

            

            this.CloseAllContainers();

            

            this.EnqueueTail(QueueItemKind.Eof);
        }

        
        private void HandleTokenTag(HtmlToken tag)
        {
            HtmlTagIndex tagIndex;

            InternalDebug.Assert(HtmlNameIndex.Unknown <= tag.NameIndex && (int)tag.NameIndex < HtmlNameData.names.Length - 1);

            tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;

            if (tag.IsTagBegin)
            {
                this.StartTagProcessing(tagIndex, tag);
            }
            else
            {
                

                if (!this.ignoreInputTag)
                {
                    InternalDebug.Assert(this.QueueHeadKind() == QueueItemKind.Suspend);

                    if (tag.IsTagEnd)
                    {
                        

                        this.DoDequeueFirst();
                    }

                    if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
                    {
                        bool emptyScope = (GetTagDefinition(tagIndex).scope == HtmlDtd.TagScope.EMPTY);

                        
                        this.inputToken.Flags = emptyScope ? this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope : this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
                    }

                    this.EnqueueHead(QueueItemKind.PassThrough);
                }
                else
                {
                    if (tag.IsTagEnd)
                    {
                        this.ignoreInputTag = false;
                    }
                }
            }
        }

        
        private void HandleTokenSpecialTag(HtmlToken tag)
        {
            HtmlTagIndex tagIndex;

            InternalDebug.Assert(HtmlNameIndex._NOTANAME < tag.NameIndex && tag.NameIndex < HtmlNameIndex.Unknown);

            
            tag.Flags = tag.Flags | HtmlToken.TagFlags.EmptyScope;

            tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;

            if (tag.IsTagBegin)
            {
                this.StartSpecialTagProcessing(tagIndex, tag);
            }
            else
            {
                

                if (!this.ignoreInputTag)
                {
                    InternalDebug.Assert(this.QueueHeadKind() == QueueItemKind.Suspend);

                    if (tag.IsTagEnd)
                    {
                        

                        this.DoDequeueFirst();
                    }

                    this.EnqueueHead(QueueItemKind.PassThrough);
                }
                else
                {
                    if (tag.IsTagEnd)
                    {
                        this.ignoreInputTag = false;
                    }
                }
            }
        }

        
        private void HandleTokenText(HtmlToken token)
        {
            HtmlTagIndex tagIdRTC = this.validRTC ? this.tagIdRTC : this.RequiredTextContainer();

            int cntWhitespaces = 0;

            Token.RunEnumerator runs = this.inputToken.Runs;

            if (HtmlTagIndex._NULL != tagIdRTC)
            {
                
                while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace))
                {
                }

                if (!runs.IsValidPosition)
                {
                    
                    return;
                }

                this.CloseAllProhibitedContainers(GetTagDefinition(tagIdRTC));
                this.OpenContainer(tagIdRTC);
            }
            else if (this.context.textType != HtmlDtd.ContextTextType.Literal)
            {
                
                

                while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace))
                {
                    
                    
                    cntWhitespaces += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                }
            }

            if (this.context.textType == HtmlDtd.ContextTextType.Literal)
            {
                

                this.EnqueueTail(QueueItemKind.PassThrough);
            }
            else if (this.context.textType == HtmlDtd.ContextTextType.Full)
            {
                if (cntWhitespaces != 0)
                {
                    
                    
                    this.AddSpace(cntWhitespaces == 1);
                }

                this.currentRun = runs.CurrentIndex;
                this.currentRunOffset = runs.CurrentOffset;

                if (runs.IsValidPosition)
                {
                    char firstChar = runs.Current.FirstChar;
                    char lastChar;
                    do
                    {
                        lastChar = runs.Current.LastChar;
                    }
                    while (runs.MoveNext(true) && !(runs.Current.TextType <= RunTextType.LastWhitespace));

                    
                    this.AddNonspace(firstChar, lastChar);
                }

                this.numRuns = runs.CurrentIndex - this.currentRun;
            }
        }

        
        private void StartTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            InternalDebug.Assert(!this.ignoreInputTag);

            if ((HtmlDtd.SetId.Null != this.context.reject && !HtmlDtd.IsTagInSet(tagIndex, this.context.reject)) ||
                (HtmlDtd.SetId.Null != this.context.accept && HtmlDtd.IsTagInSet(tagIndex, this.context.accept)))
            {
                if (!tag.IsEndTag)
                {
                    if (!this.ProcessOpenTag(tagIndex, GetTagDefinition(tagIndex)))
                    {
                        this.ProcessIgnoredTag(tagIndex, tag);
                    }
                }
                else
                {
                    if (HtmlDtd.SetId.Null == this.context.ignoreEnd || !HtmlDtd.IsTagInSet(tagIndex, this.context.ignoreEnd)) 
                    {
                        if (!this.ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                        {
                            this.ProcessIgnoredTag(tagIndex, tag);
                        }
                    }
                    else
                    {
                        this.ProcessIgnoredTag(tagIndex, tag);
                    }
                }
            }
            else if (this.context.type == HtmlDtd.ContextType.Select && tagIndex == HtmlTagIndex.Select)
            {
                if (!this.ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                {
                    this.ProcessIgnoredTag(tagIndex, tag);
                }
            }
            else
            {
                this.ProcessIgnoredTag(tagIndex, tag);
            }
        }

        
        private void StartSpecialTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            InternalDebug.Assert(!this.ignoreInputTag);

            this.EnqueueTail(QueueItemKind.PassThrough);

            if (!tag.IsTagEnd)
            {
                this.EnqueueTail(QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
            }
        }

        
        private void ProcessIgnoredTag(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            

            if (!tag.IsTagEnd)
            {
                this.ignoreInputTag = true;
            }
        }

        
        private bool ProcessOpenTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            if (!this.PrepareContainer(tagIndex, tagDef))
            {
                return false;
            }

            this.PushElement(tagIndex, true, tagDef);
            return true;
        }

        
        private bool ProcessEndTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            int elementStackPos = -1;

            
            if (tagIndex == HtmlTagIndex.Unknown)
            {
                
                
                
                
                
                
                
                

                this.PushElement(tagIndex, true, tagDef);
                return true;
            }

            
            bool useInputTag = true;
            bool inputTagUsed = false;

            if (HtmlDtd.SetId.Null != tagDef.match)
            {
                elementStackPos = this.FindContainer(tagDef.match, tagDef.endContainers);

                if (elementStackPos >= 0 && this.elementStack[elementStackPos] != tagIndex)
                {
                    
                    
                    useInputTag = false;
                }
            }
            else
            {
                elementStackPos = this.FindContainer(tagIndex, tagDef.endContainers);
            }

            
            
            

            if (elementStackPos < 0)
            {
                

                HtmlTagIndex tagId2 = tagDef.unmatchedSubstitute;
                if (tagId2 == HtmlTagIndex._NULL)
                {
                    
                    return false;
                }

                if (tagId2 == HtmlTagIndex._IMPLICIT_BEGIN)
                {
                    

                    if (!this.PrepareContainer(tagIndex, tagDef))
                    {
                        return false;
                    }

                    
                    
                    
                    

                    
                    
                    this.inputToken.Flags &= ~HtmlToken.TagFlags.EndTag;

                    elementStackPos = this.PushElement(tagIndex, useInputTag, tagDef);

                    
                    inputTagUsed |= useInputTag;

                    
                    useInputTag = false;

                    
                    
                }
                else
                {
                    

                    elementStackPos = this.FindContainer(tagId2, GetTagDefinition(tagId2).endContainers);
                    if (elementStackPos < 0)
                    {
                        return false;
                    }

                    
                    
                    useInputTag = false;
                }
            }

            

            if (elementStackPos >= 0 && elementStackPos < this.elementStackTop)
            {
                
                useInputTag &= this.inputToken.IsEndTag;

                inputTagUsed |= useInputTag;

                this.CloseContainer(elementStackPos, useInputTag);
            }

            
            
            return inputTagUsed;
        }

        
        private bool PrepareContainer(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            

            if (tagIndex == HtmlTagIndex.Unknown)
            {
                
                
                
                
                
                
                return true;
            }

            if (HtmlDtd.SetId.Null != tagDef.maskingContainers)
            {
                int elementStackPos = this.FindContainer(tagDef.maskingContainers, tagDef.beginContainers);
                if (elementStackPos >= 0)
                {
                    
                    return false;
                }
            }

            

            this.CloseAllProhibitedContainers(tagDef);

            

            HtmlTagIndex tagId2 = HtmlTagIndex._NULL;

            if (tagDef.textType == HtmlDtd.TagTextType.ALWAYS ||
                (tagDef.textType == HtmlDtd.TagTextType.QUERY && this.QueryTextlike(tagIndex)))
            {
                tagId2 = this.validRTC ? this.tagIdRTC : this.RequiredTextContainer();

                if (HtmlTagIndex._NULL != tagId2)
                {
                    this.CloseAllProhibitedContainers(GetTagDefinition(tagId2));
                    if (-1 == this.OpenContainer(tagId2))
                    {
                        return false;
                    }
                }
            }

            

            if (tagId2 == HtmlTagIndex._NULL)
            {
                if (HtmlDtd.SetId.Null != tagDef.requiredContainers)
                {
                    int elementStackPos = this.FindContainer(tagDef.requiredContainers, tagDef.beginContainers);
                    if (elementStackPos < 0)
                    {
                        
                        
                        
                        

                        this.CloseAllProhibitedContainers(GetTagDefinition(tagDef.defaultContainer));
                        if (-1 == this.OpenContainer(tagDef.defaultContainer))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        
        private int OpenContainer(HtmlTagIndex tagIndex)
        {
            int numTags = 0;

            while (HtmlTagIndex._NULL != tagIndex)
            {
                InternalDebug.Assert(numTags < this.openList.Length);
                this.openList[numTags++] = tagIndex;

                HtmlDtd.TagDefinition tagDef = GetTagDefinition(tagIndex);

                if (HtmlDtd.SetId.Null == tagDef.requiredContainers)
                {
                    break;
                }

                int elementStackPos = this.FindContainer(tagDef.requiredContainers, tagDef.beginContainers);
                if (elementStackPos >= 0)
                {
                    break;
                }

                tagIndex = tagDef.defaultContainer;
            }

            
            if (HtmlTagIndex._NULL == tagIndex)
            {
                
                return -1;
            }

            int stackPos = -1;

            for (int i = numTags - 1; i >= 0; i--)
            {
                

                
                
                
                tagIndex = this.openList[i];

                stackPos = this.PushElement(tagIndex, false, GetTagDefinition(tagIndex));
            }

            return stackPos;
        }

        
        private void CloseContainer(int stackPos, bool useInputTag)
        {
            if (stackPos != this.elementStackTop - 1)
            {
                bool closeNested = false;

                int numClose = 0;

                this.closeList[numClose++] = stackPos;

                if (GetTagDefinition(this.elementStack[stackPos]).scope == HtmlDtd.TagScope.NESTED)
                {
                    closeNested = true;
                }

                for (int i = stackPos + 1; i < this.elementStackTop; i++)
                {
                    HtmlDtd.TagDefinition tagDef = GetTagDefinition(this.elementStack[i]);

                    if (numClose == this.closeList.Length)
                    {
                        
                        
                        int[] newCloseList = new int[this.closeList.Length * 2];
                        Array.Copy(this.closeList, 0, newCloseList, 0, numClose);
                        this.closeList = newCloseList;
                    }

                    if (closeNested && tagDef.scope == HtmlDtd.TagScope.NESTED)
                    {
                        this.closeList[numClose++] = i;
                    }
                    else
                    {
                        for (int j = 0; j < numClose; j++)
                        {
                            if (HtmlDtd.IsTagInSet(this.elementStack[this.closeList[j]], tagDef.endContainers))
                            {
                                this.closeList[numClose++] = i;

                                closeNested = closeNested || (tagDef.scope == HtmlDtd.TagScope.NESTED);
                                break;
                            }
                        }
                    }
                }

                

                for (int j = numClose - 1; j > 0; j--)
                {
                    

                    this.PopElement(this.closeList[j], false);
                }
            }

            

            this.PopElement(stackPos, useInputTag);
        }

        
        private void CloseAllProhibitedContainers(HtmlDtd.TagDefinition tagDef)
        {
            HtmlDtd.SetId close = tagDef.prohibitedContainers;

            if (HtmlDtd.SetId.Null != close)
            {
                while (true)
                {
                    int elementStackPos = this.FindContainer(close, tagDef.beginContainers);
                    if (elementStackPos < 0)
                    {
                        break;
                    }

                    this.CloseContainer(elementStackPos, false);
                }
            }
        }

        
        private void CloseAllContainers()
        {
            for (int i = this.elementStackTop - 1; i > 0; i--)
            {
                this.CloseContainer(i, false);
            }
        }

        
        private void CloseAllContainers(int level)
        {
            for (int i = this.elementStackTop - 1; i >= level; i--)
            {
                this.CloseContainer(i, false);
            }
        }

        
        private int FindContainer(HtmlDtd.SetId matchSet, HtmlDtd.SetId stopSet)
        {
            
            

            int i;

            for (i = this.elementStackTop - 1; i >= 0 && !HtmlDtd.IsTagInSet(this.elementStack[i], matchSet); i--)
            {
                if (HtmlDtd.IsTagInSet(this.elementStack[i], stopSet))
                {
                    return -1;
                }
            }

            return i;
        }

        
        private int FindContainer(HtmlTagIndex match, HtmlDtd.SetId stopSet)
        {
            int i;

            for (i = this.elementStackTop - 1; i >= 0 && this.elementStack[i] != match; i--)
            {
                if (HtmlDtd.IsTagInSet(this.elementStack[i], stopSet))
                {
                    return -1;
                }
            }

            return i;
        }

        
        private HtmlTagIndex RequiredTextContainer()
        {
            InternalDebug.Assert(!this.validRTC);

            this.validRTC = true;

            for (int i = this.elementStackTop - 1; i >= 0; i--)
            {
                HtmlDtd.TagDefinition tagDef = GetTagDefinition(this.elementStack[i]);

                if (tagDef.textScope == HtmlDtd.TagTextScope.INCLUDE)
                {
                    this.tagIdRTC = HtmlTagIndex._NULL;
                    return this.tagIdRTC;
                }

                if (tagDef.textScope == HtmlDtd.TagTextScope.EXCLUDE)
                {
                    this.tagIdRTC = tagDef.textSubcontainer;
                    return this.tagIdRTC;
                }
            }

            InternalDebug.Assert(false);

            this.tagIdRTC = HtmlTagIndex._NULL;
            return this.tagIdRTC;
        }

        
        private int PushElement(HtmlTagIndex tagIndex, bool useInputTag, HtmlDtd.TagDefinition tagDef)
        {
            InternalDebug.Assert(!useInputTag || this.inputToken.IsTagBegin);

            int stackPos;

            if (this.ensureHead)
            {
                if (tagIndex == HtmlTagIndex.Body)
                {
                    
                    stackPos = this.PushElement(HtmlTagIndex.Head, false, HtmlDtd.tags[(int)HtmlTagIndex.Head]);
                    this.PopElement(stackPos, false);
                }
                else if (tagIndex == HtmlTagIndex.Head)
                {
                    
                    this.ensureHead = false;
                }
            }

            if (tagDef.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                this.validRTC = false;
            }

            if (this.elementStackTop == this.elementStack.Length && !this.EnsureElementStackSpace())
            {
                
                

                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.HtmlNestingTooDeep);
            }

            bool emptyScope = (tagDef.scope == HtmlDtd.TagScope.EMPTY);

            if (useInputTag)
            {
                if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
                {
                    
                    this.inputToken.Flags = emptyScope ? this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope : this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
                }
                else
                {
                    
                    
                    
                    
                    emptyScope = true; 
                }
            }

            stackPos = this.elementStackTop ++;

            this.elementStack[stackPos] = tagIndex;

            this.LFillTagB(tagDef);

            this.EnqueueTail(
                        useInputTag ? QueueItemKind.PassThrough : QueueItemKind.BeginElement, 
                        tagIndex, 
                        this.allowWspLeft, 
                        this.allowWspRight);

            if (useInputTag && !this.inputToken.IsTagEnd)
            {
                
                this.EnqueueTail(QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
            }

            this.RFillTagB(tagDef);

            

            

            if (!emptyScope)
            {
                

                if (tagDef.contextType != HtmlDtd.ContextType.None)
                {
                    

                    if (this.contextStackTop == this.contextStack.Length)
                    {
                        

                        this.EnsureContextStackSpace();
                    }

                    this.contextStack[this.contextStackTop++] = this.context;

                    

                    this.context.topElement = stackPos;
                    this.context.type = tagDef.contextType;
                    this.context.textType = tagDef.contextTextType;
                    this.context.accept = tagDef.accept;
                    this.context.reject = tagDef.reject;
                    this.context.ignoreEnd = tagDef.ignoreEnd;
                    this.context.hasSpace = false;
                    this.context.eatSpace = false;
                    this.context.oneNL = false;
                    this.context.lastCh = '\0';

                    if (this.context.textType != HtmlDtd.ContextTextType.Full)
                    {
                        this.allowWspLeft = false;
                        this.allowWspRight = false;
                    }

                    this.RFillTagB(tagDef);
                }
            }
            else
            {
                
                
                
                

                

                

                

                this.elementStackTop --;
            }

            return stackPos;
        }

        
        private void PopElement(int stackPos, bool useInputTag)
        {
            InternalDebug.Assert(!useInputTag || this.inputToken.IsTagBegin);

            HtmlTagIndex tagIndex = this.elementStack[stackPos];
            HtmlDtd.TagDefinition tagDef = GetTagDefinition(tagIndex);

            if (tagDef.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                this.validRTC = false;
            }

            if (stackPos == this.context.topElement)
            {
                if (this.context.textType == HtmlDtd.ContextTextType.Full)
                {
                    this.LFillTagE(tagDef);
                }

                
                this.context = this.contextStack[--this.contextStackTop];
            }

            this.LFillTagE(tagDef);

            if (stackPos != this.elementStackTop - 1)
            {
                
                
                

                InternalDebug.Assert(stackPos < this.elementStackTop - 1);

                this.EnqueueTail(QueueItemKind.OverlappedClose, this.elementStackTop - stackPos - 1);
            }

            this.EnqueueTail(
                        useInputTag ? QueueItemKind.PassThrough : QueueItemKind.EndElement,
                        tagIndex, 
                        this.allowWspLeft, 
                        this.allowWspRight);

            if (useInputTag && !this.inputToken.IsTagEnd)
            {
                
                this.EnqueueTail(QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
            }

            this.RFillTagE(tagDef);

            

            if (stackPos != this.elementStackTop - 1)
            {
                
                

                InternalDebug.Assert(stackPos < this.elementStackTop - 1);

                this.EnqueueTail(QueueItemKind.OverlappedReopen, this.elementStackTop - stackPos - 1);

                
                Array.Copy(this.elementStack, stackPos + 1, this.elementStack, stackPos, this.elementStackTop - stackPos - 1);

                if (this.context.topElement > stackPos)
                {
                    this.context.topElement --;

                    for (int i = this.contextStackTop - 1; i > 0; i--)
                    {
                        InternalDebug.Assert(this.contextStack[i].topElement != stackPos);

                        if (this.contextStack[i].topElement < stackPos)
                        {
                            break;
                        }

                        this.contextStack[i].topElement --;
                    }
                }
            }

            this.elementStackTop --;
        }

        
        private void AddNonspace(char firstChar, char lastChar)
        {
            if (this.context.hasSpace)
            {
                this.context.hasSpace = false;

                

                if ('\0' == this.context.lastCh || !this.context.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.context.lastCh, firstChar))
                {
                    this.EnqueueTail(QueueItemKind.Space);
                }
            }

            this.EnqueueTail(QueueItemKind.Text);

            this.context.eatSpace = false;
            this.context.lastCh = lastChar;
            this.context.oneNL = false;
        }

        
        private void AddSpace(bool oneNL)
        {
            InternalDebug.Assert(this.context.textType == HtmlDtd.ContextTextType.Full);

            if (!this.context.eatSpace)
            {
                this.context.hasSpace = true;
            }

            if (this.context.lastCh != '\0')
            {
                if (oneNL && !this.context.oneNL)
                {
                    this.context.oneNL = true;
                }
                else
                {
                    this.context.lastCh = '\0';
                }
            }
        }

        
        private bool QueryTextlike(HtmlTagIndex tagIndex)
        {
            
            
            
            HtmlDtd.ContextType contextType = this.context.type;
            int i = this.contextStackTop;

            while (i != 0)
            {
                switch (contextType)
                {
                    case HtmlDtd.ContextType.Head:

                        if (tagIndex == HtmlTagIndex.Object)
                        {
                            return false;
                        }

                        
                        break;

                    case HtmlDtd.ContextType.Body:

                        switch (tagIndex)
                        {
                            case HtmlTagIndex.Input:
                            case HtmlTagIndex.Object:
                            case HtmlTagIndex.Applet:
                            case HtmlTagIndex.A:
                            case HtmlTagIndex.Div:
                            case HtmlTagIndex.Span:
                                    return true;
                        }

                        return false;
                }

                contextType = this.contextStack[--i].type;
            }

            

            InternalDebug.Assert(contextType == HtmlDtd.ContextType.Root);

            if (tagIndex == HtmlTagIndex.Object || tagIndex == HtmlTagIndex.Applet)
            {
                return true;
            }

            return false;
        }

        
        
        
        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (this.context.textType == HtmlDtd.ContextTextType.Full)
            {
                this.LFill(this.FillCodeFromTag(tagDef).LB, this.FillCodeFromTag(tagDef).RB);
            }
        }

        
        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (this.context.textType == HtmlDtd.ContextTextType.Full)
            {
                this.RFill(this.FillCodeFromTag(tagDef).RB);
            }
        }

        
        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (this.context.textType == HtmlDtd.ContextTextType.Full)
            {
                this.LFill(this.FillCodeFromTag(tagDef).LE, this.FillCodeFromTag(tagDef).RE);
            }
        }

        
        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (this.context.textType == HtmlDtd.ContextTextType.Full)
            {
                this.RFill(this.FillCodeFromTag(tagDef).RE);
            }
        }

        
        private void LFill(HtmlDtd.FillCode codeLeft, HtmlDtd.FillCode codeRight)
        {
            InternalDebug.Assert(this.context.textType == HtmlDtd.ContextTextType.Full);

            
            
            this.allowWspLeft = this.context.hasSpace || codeLeft == HtmlDtd.FillCode.EAT;

            this.context.lastCh = '\0';

            if (this.context.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    this.EnqueueTail(QueueItemKind.Space);
                    this.context.eatSpace = true;
                }

                this.context.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }

            this.allowWspRight = this.context.hasSpace || codeRight == HtmlDtd.FillCode.EAT;
        }

        
        private void RFill(HtmlDtd.FillCode code)
        {
            InternalDebug.Assert(this.context.textType == HtmlDtd.ContextTextType.Full);

            if (code == HtmlDtd.FillCode.EAT)
            {
                this.context.hasSpace = false;
                this.context.eatSpace = true;
            }
            else if (code == HtmlDtd.FillCode.PUT)
            {
                this.context.eatSpace = false;
            }
        }

        
        private bool QueueEmpty()
        {
            return (this.queueHead == this.queueTail || this.queue[this.queueHead].kind == QueueItemKind.Suspend);
        }

        
        private QueueItemKind QueueHeadKind()
        {
            if (this.queueHead == this.queueTail)
            {
                return QueueItemKind.Empty;
            }

            return this.queue[this.queueHead].kind;
        }

        
        private void EnqueueTail(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            if (this.queueTail == this.queue.Length)
            {
                this.ExpandQueue();
            }

            this.queue[this.queueTail].kind = kind;
            this.queue[this.queueTail].tagIndex = tagIndex;
            this.queue[this.queueTail].flags = (allowWspLeft ? QueueItemFlags.AllowWspLeft : 0) | 
                                        (allowWspRight ? QueueItemFlags.AllowWspRight : 0);
            this.queue[this.queueTail].argument = 0;

            this.queueTail ++;
        }

        
        private void EnqueueTail(QueueItemKind kind, int argument)
        {
            if (this.queueTail == this.queue.Length)
            {
                this.ExpandQueue();
            }

            this.queue[this.queueTail].kind = kind;
            this.queue[this.queueTail].tagIndex = HtmlTagIndex._NULL;
            this.queue[this.queueTail].flags = 0; 
            this.queue[this.queueTail].argument = argument;

            this.queueTail ++;
        }

        
        private void EnqueueTail(QueueItemKind kind)
        {
            if (this.queueTail == this.queue.Length)
            {
                this.ExpandQueue();
            }

            this.queue[this.queueTail].kind = kind;
            this.queue[this.queueTail].tagIndex = HtmlTagIndex._NULL;
            this.queue[this.queueTail].flags = 0; 
            this.queue[this.queueTail].argument = 0;

            this.queueTail ++;
        }

        
        private void EnqueueHead(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            

            if (this.queueHead != this.queueStart)
            {
                this.queueHead --;
            }
            else
            {
                
                InternalDebug.Assert(this.queueHead == this.queueTail);
                this.queueTail ++;
            }

            this.queue[this.queueHead].kind = kind;
            this.queue[this.queueHead].tagIndex = tagIndex;
            this.queue[this.queueHead].flags = (allowWspLeft ? QueueItemFlags.AllowWspLeft : 0) | 
                                        (allowWspRight ? QueueItemFlags.AllowWspRight : 0);
            this.queue[this.queueHead].argument = 0;
        }

        
        private void EnqueueHead(QueueItemKind kind)
        {
            this.EnqueueHead(kind, 0);
        }

        
        private void EnqueueHead(QueueItemKind kind, int argument)
        {
            

            if (this.queueHead != this.queueStart)
            {
                this.queueHead --;
            }
            else
            {
                
                InternalDebug.Assert(this.queueHead == this.queueTail);
                this.queueTail ++;
            }

            this.queue[this.queueHead].kind = kind;
            this.queue[this.queueHead].tagIndex = HtmlTagIndex._NULL;
            this.queue[this.queueHead].flags = 0; 
            this.queue[this.queueHead].argument = argument;
        }

        
        private HtmlTokenId GetTokenFromQueue()
        {
            QueueItem qi;

            switch (this.QueueHeadKind())
            {
                case QueueItemKind.None:

                        

                        this.DoDequeueFirst();

                        this.token = null;

                        return HtmlTokenId.None;

                case QueueItemKind.PassThrough:

                        

                        qi = this.DoDequeueFirst();

                        this.token = this.inputToken;

                        if (this.token.TokenId == HtmlTokenId.Tag)
                        {
                            

                            this.token.Flags |= (((qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft) ? 
                                                        HtmlToken.TagFlags.AllowWspLeft : 0) |
                                                (((qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight) ?
                                                        HtmlToken.TagFlags.AllowWspRight : 0);

                            if (this.token.OriginalTagId == HtmlTagIndex.Body &&
                                this.token.IsTagEnd &&
                                this.injection != null && 
                                this.injection.HaveHead && 
                                !this.injection.HeadDone)
                            {
                                
                                InternalDebug.Assert(this.token.IsEndTag == false);

                                
                                

                                int bodyLevel = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                                this.parser = (HtmlParser) this.injection.Push(true, this.parser);
                                this.saveState.Save(this, bodyLevel + 1);
                                this.EnqueueTail(QueueItemKind.InjectionBegin, 1);
                                if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                                {
                                    this.OpenContainer(HtmlTagIndex.TT);
                                    this.OpenContainer(HtmlTagIndex.Pre);
                                }
                            }
                        }

                        return this.token.TokenId;

                case QueueItemKind.BeginElement:
                case QueueItemKind.EndElement:

                        

                        qi = this.DoDequeueFirst();

                        this.tokenBuilder.BuildTagToken(
                                            qi.tagIndex, 
                                            qi.kind == QueueItemKind.EndElement,
                                            (qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft,
                                            (qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight,
                                            false);

                        this.token = this.tokenBuilder;

                        if (qi.kind == QueueItemKind.BeginElement &&
                            this.token.OriginalTagId == HtmlTagIndex.Body &&
                            this.injection != null && 
                            this.injection.HaveHead && 
                            !this.injection.HeadDone)
                        {
                            
                            InternalDebug.Assert(this.token.IsEndTag == false);

                            
                            

                            int bodyLevel = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                            this.parser = (HtmlParser) this.injection.Push(true, this.parser);
                            this.saveState.Save(this, bodyLevel + 1);
                            this.EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                this.OpenContainer(HtmlTagIndex.TT);
                                this.OpenContainer(HtmlTagIndex.Pre);
                            }
                        }

                        
                        return this.token.TokenId;

                case QueueItemKind.EndLastTag:

                        qi = this.DoDequeueFirst();

                        this.tokenBuilder.BuildTagToken(
                                            qi.tagIndex, 
                                            false,      
                                            (qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft,
                                            (qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight,
                                            true);

                        this.token = this.tokenBuilder;

                        if (qi.kind == QueueItemKind.BeginElement &&
                            this.token.OriginalTagId == HtmlTagIndex.Body &&
                            this.injection != null && 
                            this.injection.HaveHead && 
                            !this.injection.HeadDone)
                        {
                            
                            InternalDebug.Assert(this.token.IsEndTag == false);

                            
                            

                            int bodyLevel = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                            this.parser = (HtmlParser) this.injection.Push(true, this.parser);
                            this.saveState.Save(this, bodyLevel + 1);
                            this.EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                this.OpenContainer(HtmlTagIndex.TT);
                                this.OpenContainer(HtmlTagIndex.Pre);
                            }
                        }

                        
                        return this.token.TokenId;

                case QueueItemKind.OverlappedClose:
                case QueueItemKind.OverlappedReopen:

                        

                        qi = this.DoDequeueFirst();

                        this.tokenBuilder.BuildOverlappedToken(qi.kind == QueueItemKind.OverlappedClose, qi.argument);

                        this.token = this.tokenBuilder;

                        
                        return this.token.TokenId;

                case QueueItemKind.Space:

                        

                        qi = this.DoDequeueFirst();

                        this.tokenBuilder.BuildSpaceToken();

                        this.token = this.tokenBuilder;

                        
                        return this.token.TokenId;

                case QueueItemKind.Text:

                        
                        

                        bool requeueInjectionEnd = false;
                        int requeueInjectionArgument = 0;

                        InternalDebug.Assert(this.context.textType == HtmlDtd.ContextTextType.Full);
                        
                        
                        qi = this.DoDequeueFirst();

                        if (this.queueHead != this.queueTail)
                        {
                            InternalDebug.Assert(this.queueHead == this.queueTail - 1 && this.QueueHeadKind() == QueueItemKind.InjectionEnd);
                            requeueInjectionEnd = true;
                            requeueInjectionArgument = this.queue[this.queueHead].argument;
                            this.DoDequeueFirst();
                        }

                        
                        

                        this.tokenBuilder.BuildTextSliceToken(this.inputToken, this.currentRun, this.currentRunOffset, this.numRuns);

                        this.token = this.tokenBuilder;

                        Token.RunEnumerator runs = this.inputToken.Runs;

                        if (runs.IsValidPosition)
                        {
                            int cnt = 0;

                            InternalDebug.Assert(runs.Current.TextType <= RunTextType.LastWhitespace);

                            

                            do
                            {
                                
                                
                                cnt += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                            }
                            while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace));

                            if (cnt != 0)
                            {
                                
                                
                                this.AddSpace(cnt == 1);
                            }

                            this.currentRun = runs.CurrentIndex;
                            this.currentRunOffset = runs.CurrentOffset;

                            if (runs.IsValidPosition)
                            {
                                char firstChar = runs.Current.FirstChar;
                                char lastChar = firstChar;
                                do
                                {
                                    lastChar = runs.Current.LastChar;
                                }
                                while (runs.MoveNext(true) && !(runs.Current.TextType <= RunTextType.LastWhitespace));

                                
                                this.AddNonspace(firstChar, lastChar);
                            }

                            this.numRuns = runs.CurrentIndex - this.currentRun;
                        }
                        else
                        {
                            this.currentRun = runs.CurrentIndex;
                            this.currentRunOffset = runs.CurrentOffset;
                            this.numRuns = 0;
                        }

                        if (requeueInjectionEnd)
                        {
                            this.EnqueueTail(QueueItemKind.InjectionEnd, requeueInjectionArgument);
                        }

                        
                        return this.token.TokenId;

                case QueueItemKind.Eof:

                        
                        

                        InternalDebug.Assert(this.queueHead + 1 == this.queueTail);     

                        this.tokenBuilder.BuildEofToken();

                        this.token = this.tokenBuilder;

                        break;

                case QueueItemKind.InjectionBegin:
                case QueueItemKind.InjectionEnd:

                        

                        qi = this.DoDequeueFirst();

                        this.tokenBuilder.BuildInjectionToken(qi.kind == QueueItemKind.InjectionBegin, qi.argument != 0);

                        this.token = this.tokenBuilder;

                        break;

                default:

                        InternalDebug.Assert(false);
                        break;
            }

            
            return this.token.TokenId;
        }

        
        private void ExpandQueue()
        {
            
            

            QueueItem[] newQueue = new QueueItem[this.queue.Length * 2];
            Array.Copy(this.queue, this.queueHead, newQueue, this.queueHead, this.queueTail - this.queueHead);
            if (this.queueStart != 0)
            {
                Array.Copy(this.queue, 0, newQueue, 0, this.queueStart);
            }
            this.queue = newQueue;
            newQueue = null;
        }

        
        private QueueItem DoDequeueFirst()
        {
            int head = this.queueHead;

            this.queueHead ++;          

            if (this.queueHead == this.queueTail)
            {
                this.queueHead = this.queueTail = this.queueStart;
            }

            return this.queue[head];
        }

        
        private HtmlDtd.TagFill FillCodeFromTag(HtmlDtd.TagDefinition tagDef)
        {
            if (this.context.type == HtmlDtd.ContextType.Select && tagDef.tagIndex != HtmlTagIndex.Option)
            {
                return HtmlDtd.TagFill.PUT_PUT_PUT_PUT;
            }
            else if (this.context.type == HtmlDtd.ContextType.Title)
            {
                InternalDebug.Assert(tagDef.tagIndex == HtmlTagIndex.Title);
                return HtmlDtd.TagFill.NUL_EAT_EAT_NUL;
            }

            return tagDef.fill;
        }

        
        private bool EnsureElementStackSpace()
        {
            if (this.elementStackTop == this.elementStack.Length)
            {
                if (this.elementStack.Length >= this.maxElementStack)
                {
                    return false;
                }

                int newSize = (this.maxElementStack / 2 > this.elementStack.Length) ? this.elementStack.Length * 2 : this.maxElementStack;

                HtmlTagIndex[] newElementStack = new HtmlTagIndex[newSize];
                Array.Copy(this.elementStack, 0, newElementStack, 0, this.elementStackTop);
                this.elementStack = newElementStack;
                newElementStack = null;
            }

            return true;
        }
        
        
        private void EnsureContextStackSpace()
        {
            if (this.contextStackTop + 1 > this.contextStack.Length)
            {
                

                Context[] newContextStack = new Context[this.contextStack.Length * 2];
                Array.Copy(this.contextStack, 0, newContextStack, 0, this.contextStackTop);
                this.contextStack = newContextStack;
                newContextStack = null;
            }
        }
        
        
        private struct Context
        {
            public int topElement;                              

            
            public HtmlDtd.ContextType type;                    
            public HtmlDtd.ContextTextType textType;            
            public HtmlDtd.SetId accept;
            public HtmlDtd.SetId reject;
            public HtmlDtd.SetId ignoreEnd;

            
            public char lastCh;
            public bool oneNL;
            public bool hasSpace;
            public bool eatSpace;
        }

        
        private struct QueueItem
        {
            public QueueItemKind kind;
            public HtmlTagIndex tagIndex;
            public QueueItemFlags flags;
            public int argument;
        }

        
        
        
        private class DocumentState
        {
            private int queueHead;
            private int queueTail;
            private HtmlToken inputToken;
            private int elementStackTop;
            private int currentRun;
            private int currentRunOffset;
            private int numRuns;
            private HtmlTagIndex[] savedElementStackEntries = new HtmlTagIndex[5];
            private int savedElementStackEntriesCount;
            private bool hasSpace;
            private bool eatSpace;
            private bool validRTC;
            private HtmlTagIndex tagIdRTC;

            public DocumentState()
            {
            }

            public int SavedStackTop { get { return this.elementStackTop; } }

            public void Save(HtmlNormalizingParser document, int stackLevel)
            {
                if (stackLevel != document.elementStackTop)
                {
                    
                    InternalDebug.Assert(stackLevel < document.elementStackTop && document.elementStackTop - stackLevel < 5);

                    Array.Copy(document.elementStack, stackLevel, this.savedElementStackEntries, 0, document.elementStackTop - stackLevel);
                    this.savedElementStackEntriesCount = document.elementStackTop - stackLevel;
                    document.elementStackTop = stackLevel;
                }
                else
                {
                    this.savedElementStackEntriesCount = 0;
                }

                this.elementStackTop = document.elementStackTop;
                this.queueHead = document.queueHead;
                this.queueTail = document.queueTail;
                this.inputToken = document.inputToken;
                this.currentRun = document.currentRun;
                this.currentRunOffset = document.currentRunOffset;
                this.numRuns = document.numRuns;
                this.hasSpace = document.context.hasSpace;
                this.eatSpace = document.context.eatSpace;
                this.validRTC = document.validRTC;
                this.tagIdRTC = document.tagIdRTC;

                
                document.queueStart = document.queueTail;
                document.queueHead = document.queueTail = document.queueStart;
            }

            public void Restore(HtmlNormalizingParser document)
            {
                InternalDebug.Assert(document.elementStackTop == this.elementStackTop);

                if (this.savedElementStackEntriesCount != 0)
                {
                    Array.Copy(this.savedElementStackEntries, 0, document.elementStack, document.elementStackTop, this.savedElementStackEntriesCount);
                    document.elementStackTop += this.savedElementStackEntriesCount;
                }

                document.queueStart = 0;
                document.queueHead = this.queueHead;
                document.queueTail = this.queueTail;

                document.inputToken = this.inputToken;
                document.currentRun = this.currentRun;
                document.currentRunOffset = this.currentRunOffset;
                document.numRuns = this.numRuns;
                document.context.hasSpace = this.hasSpace;
                document.context.eatSpace = this.eatSpace;
                document.validRTC = this.validRTC;
                document.tagIdRTC = this.tagIdRTC;
            }
        }

        
        private class SmallTokenBuilder : HtmlToken
        {
            private char[] spareBuffer = new char[1];
            private RunEntry[] spareRuns = new RunEntry[1];

            public SmallTokenBuilder()
            {
            }

            public void BuildTagToken(HtmlTagIndex tagIndex, bool closingTag, bool allowWspLeft, bool allowWspRight, bool endOnly)
            {
                this.tokenId = (TokenId)HtmlTokenId.Tag;
                this.argument = 1;

                this.buffer = this.spareBuffer;
                this.runList = this.spareRuns;
                this.whole.Reset();
                this.wholePosition.Rewind(this.whole);

                this.tagIndex = this.originalTagIndex = tagIndex;
                this.nameIndex = HtmlDtd.tags[(int)tagIndex].nameIndex;

                if (!endOnly)
                {
                    this.partMajor = HtmlToken.TagPartMajor.Complete;
                    this.partMinor = HtmlToken.TagPartMinor.CompleteName;
                }
                else
                {
                    this.partMajor = HtmlToken.TagPartMajor.End;
                    this.partMinor = HtmlToken.TagPartMinor.Empty;
                }

                this.flags = (closingTag ? HtmlToken.TagFlags.EndTag : 0) |
                            (allowWspLeft ? HtmlToken.TagFlags.AllowWspLeft : 0) | 
                            (allowWspRight ? HtmlToken.TagFlags.AllowWspRight : 0);
            }

            public void BuildOverlappedToken(bool close, int argument)
            {
                this.tokenId = (TokenId)(close ? HtmlTokenId.OverlappedClose : HtmlTokenId.OverlappedReopen);
                this.argument = argument;

                this.buffer = this.spareBuffer;
                this.runList = this.spareRuns;
                this.whole.Reset();
                this.wholePosition.Rewind(this.whole);
            }

            public void BuildInjectionToken(bool begin, bool head)
            {
                this.tokenId = (TokenId)(begin ? HtmlTokenId.InjectionBegin : HtmlTokenId.InjectionEnd);
                this.argument = head ? 1 : 0;

                this.buffer = this.spareBuffer;
                this.runList = this.spareRuns;
                this.whole.Reset();
                this.wholePosition.Rewind(this.whole);
            }

            public void BuildSpaceToken()
            {
                this.tokenId = (TokenId)HtmlTokenId.Text;
                this.argument = 1;

                this.buffer = this.spareBuffer;
                this.runList = this.spareRuns;

                this.buffer[0] = ' ';
                this.runList[0].Initialize(RunType.Normal, RunTextType.Space, (uint)HtmlRunKind.Text, 1, 0);

                this.whole.Reset();
                this.whole.tail = 1;
                this.wholePosition.Rewind(this.whole);
            }

            public void BuildTextSliceToken(Token source, int startRun, int startRunOffset, int numRuns)
            {
                InternalDebug.Assert(numRuns > 0);
                InternalDebug.Assert(startRun >= source.whole.head && startRun < source.whole.tail);
                InternalDebug.Assert(startRun + numRuns <= source.whole.tail);
                InternalDebug.Assert(startRunOffset >= source.whole.headOffset && startRunOffset < source.buffer.Length);

                this.tokenId = (TokenId)HtmlTokenId.Text;
                this.argument = 0;

                this.buffer = source.buffer;
                this.runList = source.runList;

                this.whole.Initialize(startRun, startRunOffset);
                this.whole.tail = this.whole.head + numRuns;
                this.wholePosition.Rewind(this.whole);
            }

            public void BuildEofToken()
            {
                this.tokenId = (TokenId)HtmlTokenId.EndOfFile;
                this.argument = 0;

                this.buffer = this.spareBuffer;
                this.runList = this.spareRuns;
                this.whole.Reset();
                this.wholePosition.Rewind(this.whole);
            }
        }
    }
}

