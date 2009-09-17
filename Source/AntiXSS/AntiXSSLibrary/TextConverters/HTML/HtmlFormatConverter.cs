// ***************************************************************
// <copyright file="HtmlFormatConverters.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;
    using Microsoft.Exchange.Data.TextConverters.Internal.Css;
    
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    

    internal class HtmlFormatConverter : FormatConverter, IProducerConsumer, IRestartable, IDisposable
    {
        

        protected bool convertFragment;

        protected HtmlNormalizingParser parser;

        private FormatOutput output;

        protected HtmlToken token;

        protected bool treatNbspAsBreakable;

        protected bool insideComment;
        protected bool insideStyle;
        protected bool insidePre;

        protected int insideList;

        protected int temporarilyClosedLevels;

        protected char[] literalBuffer = new char[2];

        private CssParser cssParser;
        private ConverterBufferInput cssParserInput;

        private Dictionary<StyleSelector, int> styleDictionary;
        private int[] styleHandleIndex;
        private int styleHandleIndexCount;

        private ScratchBuffer scratch;

        private Property[] parsedProperties;    

        private PropertyValueParsingMethod attributeParsingMethod;
        private PropertyId attributePropertyId;

        private IProgressMonitor progressMonitor;

        

        public HtmlFormatConverter(
                    HtmlNormalizingParser parser, 
                    FormatOutput output,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream,
                    IProgressMonitor progressMonitor) :
            base(formatConverterTraceStream)
        {

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.progressMonitor = progressMonitor;

            this.output = output;

            if (this.output != null)
            {
                this.output.Initialize(
                        this.Store,
                        SourceFormat.Html,
                        "converted from html");
            }

            this.treatNbspAsBreakable = testTreatNbspAsBreakable;

            this.InitializeDocument();
        }

        

        public HtmlFormatConverter(
                    HtmlNormalizingParser parser, 
                    FormatStore formatStore,
                    bool fragment,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream,
                    IProgressMonitor progressMonitor) :
            base(formatStore, formatConverterTraceStream)
        {

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.progressMonitor = progressMonitor;

            this.treatNbspAsBreakable = testTreatNbspAsBreakable;

            this.convertFragment = fragment;

            if (!fragment)
            {
                this.InitializeDocument();
            }
        }

        

        public FormatNode Initialize(string fragment)
        {
            InternalDebug.Assert(this.convertFragment);

            this.parser.Initialize(fragment, false);

            
            FormatNode fragmentNode = this.InitializeFragment();

            this.Initialize();

            return fragmentNode;
        }

        

        private void Initialize()
        {
            this.insideComment = false;
            this.insidePre = false;
            this.insideStyle = false;
            this.insideList = 0;

            this.temporarilyClosedLevels = 0;

            if (this.styleDictionary != null)
            {
                this.styleDictionary.Clear();
            }

            if (this.cssParserInput != null)
            {
                this.cssParserInput.Reset();
                this.cssParser.Reset();
            }
        }

        

        bool IRestartable.CanRestart()
        {
            return this.output == null || ((IRestartable)this.output).CanRestart();
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(!this.convertFragment);
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (this.output != null)
            {
                ((IRestartable)this.output).Restart();
            }

            
            this.store.Initialize();

            
            this.InitializeDocument();

            this.Initialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (this.output != null)
            {
                ((IRestartable)this.output).DisableRestart();
            }
        }

        

        public override void Run()
        {
            if (this.output != null && this.MustFlush)
            {
                if (this.CanFlush)
                {
                    this.FlushOutput();
                }
            }
            else if (!this.EndOfFile)
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
            this.Run();
            return this.EndOfFile && !this.MustFlush;
        }

        
        

        void IDisposable.Dispose()
        {
            if (this.parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)this.parser).Dispose();
            }

            if (this.token != null && this.token is IDisposable)
            {
                ((IDisposable)this.token).Dispose();
            }

            this.parser = null;

            this.token = null;
            this.literalBuffer = null;

            GC.SuppressFinalize(this);
        }

        

        private bool CanFlush
        {
            get
            {
                

                return this.output.CanAcceptMoreOutput;
            }
        }

        

        private bool FlushOutput()
        {
            InternalDebug.Assert(this.MustFlush);

            if (this.output.Flush())
            {
                this.MustFlush = false;
                return true;
            }

            return false;
        }

        
        protected void Process(HtmlTokenId tokenId)
        {
            this.token = this.parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    

                    if (this.token.TagIndex <= HtmlTagIndex.Unknown)
                    {
                        if (this.insideStyle && this.token.TagIndex == HtmlTagIndex._COMMENT)
                        {
                            
                            this.token.Text.WriteTo(this.cssParserInput);
                        }

                        break;
                    }

                    HtmlDtd.TagDefinition tagDef = GetTagDefinition(this.token.TagIndex);

                    if (!this.token.IsEndTag)
                    {
                        

                        if (this.token.IsTagBegin)
                        {
                            

                            this.PushElement(tagDef, this.token.IsEmptyScope);
                        }

                        this.ProcessStartTagAttributes(tagDef);
                    }
                    else
                    {
                        

                        if (this.token.IsTagEnd)
                        {
                            this.PopElement(this.buildStackTop - 1 - this.temporarilyClosedLevels, this.token.Argument != 1);
                        }
                    }
                    break;

                

                case HtmlTokenId.Text:

                    if (this.insideStyle)
                    {
                        
                        this.token.Text.WriteTo(this.cssParserInput);
                    }
                    else if (this.insideComment)
                    {
                    }
                    else if (this.insidePre)
                    {
                        this.ProcessPreformatedText();
                    }
                    else
                    {
                        this.ProcessText();
                    }
                    break;

                

                case HtmlTokenId.OverlappedClose:

                    
                    this.temporarilyClosedLevels = this.token.Argument;
                    break;

                case HtmlTokenId.OverlappedReopen:

                    InternalDebug.Assert(this.temporarilyClosedLevels == this.token.Argument);

                    
                    this.temporarilyClosedLevels = 0;
                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    if (this.output != null && this.output.OutputCodePageSameAsInput)
                    {
                        int codePage = this.token.Argument;

                        
                        #if DEBUG
                        Encoding newOutputEncoding;
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                        #endif
                        this.output.OutputEncoding = Charset.GetEncoding(codePage);
                    }

                    break;

                

                case HtmlTokenId.EndOfFile:

                    
                    this.CloseAllContainersAndSetEOF();
                    break;
            }
        }

        
        private int PushElement(HtmlDtd.TagDefinition tagDef, bool emptyScope)
        {
            

            int stackPos = this.buildStackTop;

            FormatConverterContainer last = FormatConverterContainer.Null;

            if (HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].ContainerType != FormatContainerType.Null)
            {
                int defaultStyle = HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].DefaultStyle;

                last = this.OpenContainer(
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].ContainerType,
                        emptyScope,
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].InheritanceMaskIndex,
                        this.GetStyle(defaultStyle), 
                        tagDef.tagIndex);      
            }
            else if (!emptyScope)
            {
                
                

                last = this.OpenContainer(
                        FormatContainerType.PropertyContainer,
                        false,
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].InheritanceMaskIndex,
                        FormatStyle.Null,
                        tagDef.tagIndex);      
            }

            InternalDebug.Assert(last.IsNull || last == this.Last);

            if (!last.IsNull)
            {
                

                switch (tagDef.tagIndex)
                {
                    case HtmlTagIndex.Title:
                            this.insideComment = !this.token.IsEndTag;
                            break;

                    case HtmlTagIndex.Comment:
                    case HtmlTagIndex.Script:
                    case HtmlTagIndex.Xml:

                            this.insideComment = true;
                            break;

                    case HtmlTagIndex.Style:

                            if (null == this.cssParserInput)
                            {
                                this.cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, this.progressMonitor);
                                this.cssParser = new CssParser(this.cssParserInput, 1024, false);
                            }

                            this.insideStyle = true;
                            break;

                    case HtmlTagIndex.Pre:
                    case HtmlTagIndex.PlainText:
                    case HtmlTagIndex.Listing:
                    case HtmlTagIndex.Xmp:

                            this.insidePre = true;
                            
                            goto case HtmlTagIndex.P;

                    case HtmlTagIndex.OL:
                    case HtmlTagIndex.UL:
                    case HtmlTagIndex.DL:

                            int previousInsideList = this.insideList;
                            this.insideList ++;
                            if (previousInsideList == 0)
                            {
                                
                                goto case HtmlTagIndex.P;
                            }
                            break;

                    case HtmlTagIndex.P:
                    case HtmlTagIndex.BlockQuote:
                    case HtmlTagIndex.H1:
                    case HtmlTagIndex.H2:
                    case HtmlTagIndex.H3:
                    case HtmlTagIndex.H4:
                    case HtmlTagIndex.H5:
                    case HtmlTagIndex.H6:

                            
                            InternalDebug.Assert(!this.LastNode.IsNull);

                            
                            
                            

                            if (!this.LastNode.FirstChild.IsNull ||
                                (this.LastNode.NodeType != FormatContainerType.Document &&
                                this.LastNode.NodeType != FormatContainerType.Fragment &&
                                this.LastNode.NodeType != FormatContainerType.TableCell))
                            {
                                last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.TopMargin, new PropertyValue(LengthUnits.Points, 14));

                                
                                
                                
                                last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.BottomMargin, new PropertyValue(LengthUnits.Points, 14));
                            }
                            break;
                }

                
                this.FindAndApplyStyle(new StyleSelector(tagDef.nameIndex, null, null));
            }

            return stackPos;
        }

        
        private void PopElement(int stackPos, bool explicitClose/*whether the element was closed automatically*/)
        {
            
            FormatNode node = this.Last.Node;

            switch (this.buildStack[stackPos].tagIndex)
            {
                case HtmlTagIndex.Title:
                        this.insideComment = !this.token.IsEndTag;
                        break;

                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Xml:

                        this.insideComment = false;
                        break;

                case HtmlTagIndex.Style:

                        if (this.insideStyle)
                        {
                            
                            this.ProcessStylesheet();
                            this.insideStyle = false;
                        }
                        break;

                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        this.insidePre = false;
                        break;

                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.DL:

                        InternalDebug.Assert(this.insideList != 0);
                        this.insideList --;
                        break;
            }

            if (stackPos == this.buildStackTop - 1)
            {
                this.CloseContainer();
            }
            else
            {
                this.CloseOverlappingContainer(buildStackTop - 1 - stackPos);
            }

            if (!node.IsNull)
            {
                if (node.NodeType == FormatContainerType.Table)
                {
                    while (!node.LastChild.IsNull && node.LastChild.NodeType == FormatContainerType.TableRow)
                    {
                        bool emptyRow = true;

                        PropertyValue height = node.LastChild.GetProperty(PropertyId.Height);
                        if (!height.IsNull)
                        {
                            break;
                        }

                        foreach (FormatNode cellNode in node.LastChild.Children)
                        {
                            if (!cellNode.FirstChild.IsNull)
                            {
                                emptyRow = false;
                                break;
                            }
                        }

                        if (!emptyRow)
                        {
                            break;
                        }

                        
                        node.LastChild.RemoveFromParent();
                    }
                }
            }
        }

        
        protected override FormatNode GetParentForNewNode(FormatNode node, FormatNode parent, int stackPos, out int propContainerInheritanceStopLevel)
        {
            switch (node.NodeType)
            {
                case FormatContainerType.TableRow:

                        
                        
                        if (parent.NodeType != FormatContainerType.Table)
                        {
                            parent = this.FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!parent.IsNull);
                        }
                        propContainerInheritanceStopLevel = stackPos;
                        break;

                case FormatContainerType.TableCell:

                        
                        if (parent.NodeType != FormatContainerType.TableRow)
                        {
                            parent = this.FindStackAncestor(stackPos, FormatContainerType.TableRow);
                            
                            InternalDebug.Assert(!parent.IsNull);
                        }
                        propContainerInheritanceStopLevel = stackPos;
                        break;

                    case FormatContainerType.TableExtraContent:
                        {
                            
                            
                            
                            

                            propContainerInheritanceStopLevel = stackPos;

                            FormatNode newParent = parent.NodeType != FormatContainerType.Table &&
                                                    parent.NodeType != FormatContainerType.TableRow &&
                                                    parent.NodeType != FormatContainerType.TableColumnGroup ?
                                                        parent :
                                                        this.FindStackParentForExtraContent(stackPos, out propContainerInheritanceStopLevel);

                            if (newParent.IsNull)
                            {
                                

                                FormatNode table = parent.NodeType == FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, FormatContainerType.Table);
                                
                                InternalDebug.Assert(!table.IsNull);

                                FormatNode tableContainer = table.Parent;
                                FormatNode extraContentContainer = FormatNode.Null;

                                if (tableContainer.NodeType != FormatContainerType.TableContainer)
                                {
                                    
                                    

                                    tableContainer = this.store.AllocateNode(FormatContainerType.TableContainer, table.BeginTextPosition);
                                    tableContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;

                                    table.InsertSiblingAfter(tableContainer);
                                    table.RemoveFromParent();
                                    tableContainer.AppendChild(table);

                                    tableContainer.SetOnRightEdge();

                                    extraContentContainer = this.store.AllocateNode(FormatContainerType.TableExtraContent);
                                    extraContentContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                    extraContentContainer.SetOutOfOrder();

                                    if (table.OnLeftEdge)
                                    {
                                        
                                        
                                        
                                        InternalDebug.Assert(false);

                                        tableContainer.SetOnLeftEdge();

                                        tableContainer.AppendChild(extraContentContainer);
                                    }
                                    else
                                    {
                                        tableContainer.PrependChild(extraContentContainer);
                                    }
                                }
                                else
                                {
                                    foreach (FormatNode nodeT in tableContainer.Children)
                                    {
                                        if (nodeT.NodeType == FormatContainerType.TableExtraContent)
                                        {
                                            extraContentContainer = nodeT;
                                        }
                                    }
                                }

                                newParent = extraContentContainer;
                            }

                            parent = newParent;
                        }
                        break;

                    case FormatContainerType.TableCaption:
                        {
                            propContainerInheritanceStopLevel = stackPos;

                            FormatNode table = parent.NodeType == FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!table.IsNull);

                            FormatNode tableCaptionContainer = table.FirstChild;

                            if (tableCaptionContainer.IsNull ||
                                tableCaptionContainer.NodeType != FormatContainerType.TableCaption)
                            {
                                tableCaptionContainer = this.store.AllocateNode(FormatContainerType.TableCaption);
                                tableCaptionContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                tableCaptionContainer.SetOutOfOrder();
                                table.PrependChild(tableCaptionContainer);
                            }

                            parent = tableCaptionContainer;
                        }
                        break;

                    case FormatContainerType.TableColumn:
                        {
                            FormatNode newParent = parent.NodeType == FormatContainerType.TableColumnGroup ? parent : this.FindStackParentForColumn(stackPos);

                            if (!newParent.IsNull)
                            {
                                propContainerInheritanceStopLevel = stackPos;
                                parent = newParent;
                                break;
                            }
                            goto case FormatContainerType.TableColumnGroup;
                        }

                    case FormatContainerType.TableColumnGroup:
                        {
                            propContainerInheritanceStopLevel = stackPos;

                            FormatNode table = parent.NodeType == FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!table.IsNull);

                            FormatNode tableCaptionContainer = FormatNode.Null;
                            FormatNode tableDefinitionContainer = table.FirstChild;

                            if (!tableDefinitionContainer.IsNull &&
                                tableDefinitionContainer.NodeType == FormatContainerType.TableCaption)
                            {
                                tableCaptionContainer = tableDefinitionContainer;
                                tableDefinitionContainer = tableDefinitionContainer.NextSibling;
                            }

                            if (tableDefinitionContainer.IsNull ||
                                tableDefinitionContainer.NodeType != FormatContainerType.TableDefinition)
                            {
                                tableDefinitionContainer = this.store.AllocateNode(FormatContainerType.TableDefinition);
                                tableDefinitionContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                tableDefinitionContainer.SetOutOfOrder();

                                if (tableCaptionContainer.IsNull)
                                {
                                    table.PrependChild(tableDefinitionContainer);
                                }
                                else
                                {
                                    tableCaptionContainer.InsertSiblingAfter(tableDefinitionContainer);
                                }
                            }

                            parent = tableDefinitionContainer;
                        }
                        break;

                default:

                        if (parent.NodeType == FormatContainerType.Table ||
                            parent.NodeType == FormatContainerType.TableRow ||
                            parent.NodeType == FormatContainerType.TableColumnGroup)
                        {
                            goto case FormatContainerType.TableExtraContent;
                        }

                        propContainerInheritanceStopLevel = this.DefaultPropContainerInheritanceStopLevel(stackPos);
                        break;
            }

            return parent;
        }

        
        protected override FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
        {
            
            
            
            

            

            PropertyValue display = styleBuilderWithContainerProperties.GetProperty(PropertyId.Display);
            if (display.IsEnum)
            {
                switch ((Display)display.Enum)
                {
                    case Display.None:

                            
                            
                            
                            break;

                    case Display.Inline:

                            
                            if (type == FormatContainerType.Block)
                            {
                                 type = FormatContainerType.Inline;
                            }
                            break;

                    case Display.Block:

                            
                            if (type == FormatContainerType.PropertyContainer ||
                                type == FormatContainerType.Inline)
                            {
                                 type = FormatContainerType.Block;
                            }
                            break;
/*
                    
                    

                    case Display.InlineBlock:
                    case Display.ListItem:
                    case Display.TableHeaderGroup:
                    case Display.TableFooterGroup:

                    

                    case Display.RunIn:
                    case Display.Table:
                    case Display.InlineTable:
                    case Display.TableRowGroup:
                    case Display.TableRow:
                    case Display.TableColumnGroup:
                    case Display.TableColumn:
                    case Display.TableCell:
                    case Display.TableCaption:
*/
                }
            }

            if (type == FormatContainerType.PropertyContainer)
            {
                
                
                

                PropertyValue unicodeBiDi = styleBuilderWithContainerProperties.GetProperty(PropertyId.UnicodeBiDi);
                if (unicodeBiDi.IsEnum)
                {
                    if ((UnicodeBiDi)unicodeBiDi.Enum != UnicodeBiDi.Normal)
                    {
                        type = FormatContainerType.Inline;
                    }
                }
            }

            if (type == FormatContainerType.HyperLink)
            {
                PropertyValue href = styleBuilderWithContainerProperties.GetProperty(PropertyId.HyperlinkUrl);
                if (href.IsNull)
                {
                    type = FormatContainerType.Bookmark;
                }
            }

            return type;
        }

        
        private FormatNode FindStackAncestor(int stackPosOfNewContainer, FormatContainerType type)
        {
            for (int i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (this.buildStack[i].type == type)
                {
                    InternalDebug.Assert(this.buildStack[i].node != 0);
                    return this.store.GetNode(this.buildStack[i].node);
                }
            }

            return FormatNode.Null;
        }

        
        private FormatNode FindStackParentForExtraContent(int stackPosOfNewContainer, out int ancestorContainerLevel)
        {
            bool tableSeen = false;
            for (int i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (this.buildStack[i].node != 0)
                {
                    if (this.buildStack[i].type == FormatContainerType.Table)
                    {
                        tableSeen = true;
                        #if false
                        
                        ancestorContainerLevel = i + 1;
                        break;
                        #endif
                    }
                    else if (this.buildStack[i].type != FormatContainerType.TableRow &&
                        this.buildStack[i].type != FormatContainerType.TableColumnGroup)
                    {
                        ancestorContainerLevel = i + 1;
                        return tableSeen ? FormatNode.Null : this.store.GetNode(this.buildStack[i].node);
                    }
                }
            }

            ancestorContainerLevel = stackPosOfNewContainer;
            return FormatNode.Null;
        }

        
        private FormatNode FindStackParentForColumn(int stackPosOfNewContainer)
        {
            for (int i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (this.buildStack[i].node != 0)
                {
                    if (this.buildStack[i].type == FormatContainerType.Table)
                    {
                        
                        break;
                    }

                    if (this.buildStack[i].type == FormatContainerType.TableColumnGroup)
                    {
                        return this.store.GetNode(this.buildStack[i].node);
                    }
                }
            }

            return FormatNode.Null;
        }

        
        private bool StartTagHasAttribute(HtmlNameIndex attributeNameIndex)
        {
            foreach (HtmlAttribute attr in this.token.Attributes)
            {
                if (attr.NameIndex == attributeNameIndex)
                {
                    return true;
                }
            }

            return false;
        }

        
        private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
        {
            if (HtmlConverterData.tagInstructions[(int)this.token.TagIndex].ContainerType != FormatContainerType.Null)
            {
                this.token.Attributes.Rewind();

                foreach (HtmlAttribute attr in this.token.Attributes)
                {
                    if (attr.NameIndex == HtmlNameIndex.Style)
                    {
                        
                        this.ProcessStyleAttribute(tagDef, attr);
                    }
                    else if (attr.NameIndex == HtmlNameIndex.Id)
                    {
                        

                        string id = attr.Value.GetString(60);

                        this.FindAndApplyStyle(new StyleSelector(this.token.NameIndex, null, id));

                        this.FindAndApplyStyle(new StyleSelector(HtmlNameIndex.Unknown, null, id));
                    }
                    else if (attr.NameIndex == HtmlNameIndex.Class)
                    {
                        

                        string cls = attr.Value.GetString(60);

                        this.FindAndApplyStyle(new StyleSelector(this.token.NameIndex, cls, null));

                        this.FindAndApplyStyle(new StyleSelector(HtmlNameIndex.Unknown, cls, null));

                        if (!this.LastNonEmpty.Node.IsNull)
                        {
                            if (cls.Equals("EmailQuote", StringComparison.OrdinalIgnoreCase))
                            {
                                this.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.QuotingLevelDelta, new PropertyValue(PropertyType.Integer, 1/*this.quotingLevel*/));
                            }
                        }
                    }
                    else if (attr.NameIndex != HtmlNameIndex.Unknown)
                    {
                        

                        this.ProcessNonStyleAttribute(attr);
                    }
                }
            }

            if (this.token.IsTagEnd)
            {
                

                if (this.token.TagIndex == HtmlTagIndex.BR)
                {
                    this.AddLineBreak(1);
                }
            }
        }

        
        private void ProcessPreformatedText()
        {
            foreach (TokenRun run in this.token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        switch (run.TextType)
                        {
                            case RunTextType.NewLine:

                                    
                                    this.AddLineBreak(1);
                                    break;

                            case RunTextType.Space:
                            default:
#if false
                                    if (this.treatNbspAsBreakable)
                                    {
#endif
                                        this.AddSpace(run.Length);
#if false
                                    }
                                    else
                                    {
                                        this.AddNbsp(run.Length);
                                    }
#endif
                                    break;

                            case RunTextType.Tabulation:

                                    this.AddTabulation(run.Length);
                                    break;
                        }
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (this.treatNbspAsBreakable)
                        {
                            this.AddSpace(run.Length);
                        }
                        else
                        {
                            this.AddNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        this.OutputNonspace(run);
                    }
                }
            }
        }

        
        private void ProcessText()
        {
            foreach (TokenRun run in this.token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        this.AddSpace(1);
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (this.treatNbspAsBreakable)
                        {
                            this.AddSpace(run.Length);
                        }
                        else
                        {
                            this.AddNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        this.OutputNonspace(run);
                    }
                }
            }
        }

        
        private void OutputNonspace(TokenRun run)
        {
            

            if (run.IsLiteral)
            {
                this.AddNonSpaceText(this.literalBuffer, 0, run.ReadLiteral(this.literalBuffer));
            }
            else
            {
                this.AddNonSpaceText(run.RawBuffer, run.RawOffset, run.RawLength);
            }
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return tagIndex != HtmlTagIndex._NULL ? HtmlDtd.tags[(int)tagIndex] : null;
        }

        
        private void ProcessStyleAttribute(HtmlDtd.TagDefinition tagDef, HtmlAttribute attr)
        {
            if (attr.IsAttrBegin)
            {
                if (null == this.cssParserInput)
                {
                    this.cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, this.progressMonitor);
                    this.cssParser = new CssParser(this.cssParserInput, 1024, false);
                }

                this.cssParser.SetParseMode(CssParseMode.StyleAttribute);
            }

            attr.Value.Rewind();
            attr.Value.WriteTo(this.cssParserInput);

            if (attr.IsAttrEnd)
            {
                CssTokenId cssTokenId;
                do
                {
                    cssTokenId = this.cssParser.Parse();

                    if (CssTokenId.Declarations == cssTokenId && this.cssParser.Token.Properties.ValidCount != 0)
                    {
                        this.cssParser.Token.Properties.Rewind();

                        foreach (CssProperty property in this.cssParser.Token.Properties)
                        {
                            if (property.NameId != CssNameIndex.Unknown)
                            {
                                if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod)
                                {
                                    PropertyValue value;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(property.Value.ContiguousBufferString, this);
                                    }
                                    else
                                    {
                                        this.scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(this.scratch.BufferString, this);
                                        this.scratch.Reset();
                                    }

                                    if (!value.IsNull)
                                    {
                                        PropertyId propId = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId;

                                        this.Last.SetProperty(PropertyPrecedence.InlineStyle, propId, value);
                                    }
                                }
                                else if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod)
                                {
                                    

                                    if (this.parsedProperties == null)
                                    {
                                        
                                        this.parsedProperties = new Property[12];
                                    }

                                    int numParsedProperties;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(property.Value.ContiguousBufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, this.parsedProperties, out numParsedProperties);
                                    }
                                    else
                                    {
                                        this.scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(this.scratch.BufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, this.parsedProperties, out numParsedProperties);
                                        this.scratch.Reset();
                                    }

                                    if (numParsedProperties != 0)
                                    {
                                        this.Last.SetProperties(PropertyPrecedence.InlineStyle, this.parsedProperties, numParsedProperties);
                                    }
                                }
                            }
                        }
                    }
                }
                while (CssTokenId.EndOfFile != cssTokenId);

                this.cssParserInput.Reset();
                this.cssParser.Reset();
            }
        }

        
        private void ProcessNonStyleAttribute(HtmlAttribute attr)
        {
            if (attr.IsAttrBegin)
            {
                if (this.token.TagIndex == HtmlTagIndex.A && attr.NameIndex == HtmlNameIndex.Href)
                {
                    
                    this.Last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.Underline, new PropertyValue(true));
                    this.Last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.FontColor, new PropertyValue(new RGBT(0,0,255)));
                }

                this.attributeParsingMethod = null;

                if (HtmlConverterData.tagInstructions[(int)this.token.TagIndex].AttributeInstructions != null)
                {
                    for (int i = 0; i < HtmlConverterData.tagInstructions[(int)this.token.TagIndex].AttributeInstructions.Length; i++)
                    {
                        if (attr.NameIndex == HtmlConverterData.tagInstructions[(int)this.token.TagIndex].AttributeInstructions[i].AttributeNameId)
                        {
                            this.attributeParsingMethod = HtmlConverterData.tagInstructions[(int)this.token.TagIndex].AttributeInstructions[i].ParsingMethod;
                            this.attributePropertyId = HtmlConverterData.tagInstructions[(int)this.token.TagIndex].AttributeInstructions[i].PropertyId;

                            if (attr.IsAttrEnd && !attr.Value.IsEmpty && attr.Value.IsContiguous)
                            {
                                

                                PropertyValue value = this.attributeParsingMethod(attr.Value.ContiguousBufferString, this);

                                if (!value.IsNull)
                                {
                                    this.Last.SetProperty(PropertyPrecedence.NonStyle, this.attributePropertyId, value);
                                }

                                return;
                            }

                            break;
                        }
                    }
                }

                if (this.attributeParsingMethod == null)
                {
                    
                }
            }

            if (this.attributeParsingMethod == null)
            {
                return;
            }

            this.scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

            if (attr.IsAttrEnd)
            {
                PropertyValue value = this.attributeParsingMethod(this.scratch.BufferString, this);

                if (!value.IsNull)
                {
                    this.Last.SetProperty(PropertyPrecedence.NonStyle, this.attributePropertyId, value);
                }

                this.scratch.Reset();
            }
        }

        
        private void ProcessStylesheet()
        {
            this.cssParser.SetParseMode(CssParseMode.StyleTag);

            CssTokenId cssTokenId;
            do
            {
                cssTokenId = this.cssParser.Parse();

                if (CssTokenId.RuleSet == cssTokenId &&
                    this.cssParser.Token.Selectors.ValidCount != 0 &&
                    this.cssParser.Token.Properties.ValidCount != 0)
                {
                    bool slotAllocated = false;
                    bool msoNormalClassSelector = false;

                    this.cssParser.Token.Selectors.Rewind();

                    foreach (CssSelector selector in this.cssParser.Token.Selectors)
                    {
                        if (!selector.IsSimple)
                        {
                            
                            continue;
                        }

                        StyleSelector selectorDescriptor = new StyleSelector();
                        bool goodSelector = false;

                        if (selector.HasClassFragment)
                        {
                            if (selector.ClassType == CssSelectorClassType.Regular ||
                                selector.ClassType == CssSelectorClassType.Hash)
                            {
                                

                                string className = selector.ClassName.GetString(60);

                                
                                InternalDebug.Assert(className.Length > 0);

                                if (selector.ClassType == CssSelectorClassType.Regular)
                                {
                                    selectorDescriptor = new StyleSelector(selector.NameId, className, null);
                                    goodSelector = true;

                                    msoNormalClassSelector = msoNormalClassSelector || className.Equals("MsoNormal", StringComparison.OrdinalIgnoreCase);
                                }
                                else
                                {
                                    selectorDescriptor = new StyleSelector(selector.NameId, null, className);
                                    goodSelector = true;
                                }
                            }
                        }
                        else if (selector.NameId != HtmlNameIndex.Unknown)
                        {
                            
                            selectorDescriptor = new StyleSelector(selector.NameId, null, null);
                            goodSelector = true;
                        }
                        else
                        {
                            
                            
                            
                        }

                        

                        if (goodSelector && (slotAllocated || this.styleHandleIndexCount < HtmlSupport.MaxNumberOfNonInlineStyles))
                        {
                            if (!slotAllocated)
                            {
                                if (this.styleHandleIndex == null)
                                {
                                    InternalDebug.Assert(this.styleHandleIndexCount == 0);
                                    this.styleHandleIndex = new int[32];
                                }
                                else if (this.styleHandleIndexCount == this.styleHandleIndex.Length)
                                {
                                    int[] newIndex = new int[this.styleHandleIndexCount * 2];
                                    Array.Copy(this.styleHandleIndex, 0, newIndex, 0, this.styleHandleIndexCount);
                                    this.styleHandleIndex = newIndex;
                                }

                                this.styleHandleIndexCount ++;
                                slotAllocated = true;
                            }

                            if (this.styleDictionary == null)
                            {
                                this.styleDictionary = new Dictionary<StyleSelector, int>(new StyleSelectorComparer());
                            }

                            if (!this.styleDictionary.ContainsKey(selectorDescriptor))
                            {
                                this.styleDictionary.Add(selectorDescriptor, this.styleHandleIndexCount - 1);
                            }
                            else
                            {
                                this.styleDictionary[selectorDescriptor] = this.styleHandleIndexCount - 1;
                            }
                        }
                    }

                    if (slotAllocated)
                    {
                        StyleBuilder builder;

                        FormatStyle style = this.RegisterStyle(false, out builder);

                        this.cssParser.Token.Properties.Rewind();

                        foreach (CssProperty property in this.cssParser.Token.Properties)
                        {
                            if (property.NameId != CssNameIndex.Unknown)
                            {
                                if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod)
                                {
                                    

                                    PropertyValue value;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(property.Value.ContiguousBufferString, this);
                                    }
                                    else
                                    {
                                        this.scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(this.scratch.BufferString, this);
                                        this.scratch.Reset();
                                    }

                                    if (!value.IsNull)
                                    {
                                        builder.SetProperty(HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, value);
                                    }
                                }
                                else if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod)
                                {
                                    

                                    if (this.parsedProperties == null)
                                    {
                                        
                                        this.parsedProperties = new Property[12];
                                    }

                                    int numParsedProperties;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(property.Value.ContiguousBufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, this.parsedProperties, out numParsedProperties);
                                    }
                                    else
                                    {
                                        this.scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(this.scratch.BufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, this.parsedProperties, out numParsedProperties);
                                        this.scratch.Reset();
                                    }

                                    if (numParsedProperties != 0)
                                    {
                                        builder.SetProperties(this.parsedProperties, numParsedProperties);
                                    }
                                }
                            }
                        }

                        builder.Flush();

                        if (style.IsEmpty)
                        {
                            style.Release();
                            this.styleHandleIndex[this.styleHandleIndexCount - 1] = 0;
                        }
                        else
                        {
                            this.styleHandleIndex[this.styleHandleIndexCount - 1] = style.Handle;
                        }
                    }
                }
            }
            while (CssTokenId.EndOfFile != cssTokenId);

            this.cssParserInput.Reset();
            this.cssParser.Reset();
        }

        
        private void FindAndApplyStyle(StyleSelector styleSelector)
        {
            int slotIndex;

            if (this.styleDictionary != null && this.styleDictionary.TryGetValue(styleSelector, out slotIndex))
            {
                if (this.styleHandleIndex[slotIndex] != 0)
                {
                    this.Last.SetStyleReference((int)PropertyPrecedence.StyleBase + 8 - styleSelector.Specificity, this.styleHandleIndex[slotIndex]);
                }
            }
        }

        

        private struct StyleSelector
        {
            public HtmlNameIndex nameId;
            public string cls;
            public string id;

            public StyleSelector(HtmlNameIndex nameId, string cls, string id)
            {
                this.nameId = nameId;
                this.cls = (cls == null || cls.Length == 0 || cls.Equals("*")) ? null : cls;
                this.id = (string.IsNullOrEmpty(id) || id.Equals("*")) ? null : id;
            }

            public int Specificity
            {
                get
                {
                    return (this.id == null ? 0 : 4) +
                            (this.cls == null ? 0 : 2) +
                            (this.nameId == HtmlNameIndex.Unknown ? 0 : 1);
                }
            }
        }

        

        class StyleSelectorComparer : IEqualityComparer<StyleSelector>, IComparer<StyleSelector>
        {
            public int Compare(StyleSelector x, StyleSelector y)
            {
                int specificityX = x.Specificity;
                int cmp;

                if (specificityX != y.Specificity)
                {
                    return specificityX - y.Specificity;
                }

                switch (specificityX)
                {
                    case 1:     return (int)x.nameId - (int)y.nameId;
                    case 2:     return String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase);
                    case 3:     return 0 != (cmp = String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                    case 4:     return String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase);
                    case 5:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                    case 6:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase);
                    case 7:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : 0 != (cmp = String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                }

                InternalDebug.Assert(specificityX == 0);
                return 0;
            }

            public bool Equals(StyleSelector x, StyleSelector y)
            {
                return this.Compare(x, y) == 0;
            }

            public int GetHashCode(StyleSelector x)
            {
                int specificityX = x.Specificity;
                return (int)x.nameId ^ (0 == (specificityX & 4) ? 0 : x.id.GetHashCode()) ^ (0 == (specificityX & 2) ? 0 : x.cls.GetHashCode());
            }
        }
    }
}

