// ***************************************************************
// <copyright file="FormatOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    internal enum SourceFormat
    {
        Text,
        Html,
        Rtf,
        Enriched,
    }

    
    internal abstract class FormatOutput : IDisposable
    {
        private SourceFormat sourceFormat;
        private string comment;

        private FormatStore formatStore;
        private FormatNode rootNode;

        private OutputStackEntry currentOutputLevel;
        private OutputStackEntry[] outputStack;
        private int outputStackTop;

        internal ScratchBuffer scratchBuffer;
        internal ScratchBuffer scratchValueBuffer;

        internal PropertyState propertyState = new PropertyState();
#if DEBUG
        internal TextWriter formatOutputTraceWriter;
#endif
        

        private enum OutputState : byte
        {
            NotStarted,
            Started,
            EndPending,
            Ended
        }

        

        private struct OutputStackEntry
        {
            public OutputState state;
            public FormatNode node;
            public int index;
            public int childIndex;
            public int propertyUndoLevel;
        }

        public virtual bool OutputCodePageSameAsInput
        {
            get { return false; }
        }

        public virtual Encoding OutputEncoding
        {
            set
            {
                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        public virtual bool CanAcceptMoreOutput { get { return true; } }

        protected FormatStore FormatStore { get { return this.formatStore; } }
        protected SourceFormat SourceFormat { get { return this.sourceFormat; } }
        protected string Comment { get { return this.comment; } }

        protected FormatNode CurrentNode { get { return this.currentOutputLevel.node; } }
        protected int CurrentNodeIndex { get { return this.currentOutputLevel.index; } }

        protected FormatOutput(Stream formatOutputTraceStream)
        {
#if DEBUG
            if (formatOutputTraceStream != null)
            {
                this.formatOutputTraceWriter = new StreamWriter(formatOutputTraceStream);
            }
#endif
        }

        public virtual void Initialize(FormatStore store, SourceFormat sourceFormat, string comment)
        {
            this.sourceFormat = sourceFormat;
            this.comment = comment;

            this.formatStore = store;

            this.Restart(this.formatStore.RootNode);
        }

        public void Restart(FormatNode rootNode)
        {
            this.outputStackTop = 0;

            this.currentOutputLevel.node = rootNode;
            this.currentOutputLevel.state = OutputState.NotStarted;

            this.rootNode = rootNode;
        }

        protected void Restart()
        {
            this.Restart(this.rootNode);
        }

        

        public bool HaveSomethingToFlush()
        {
            return this.currentOutputLevel.node.CanFlush;
        }

        public FlagProperties GetEffectiveFlags()
        {
            return this.propertyState.GetEffectiveFlags();
        }

        public FlagProperties GetDistinctFlags()
        {
            return this.propertyState.GetDistinctFlags();
        }

        public PropertyValue GetEffectiveProperty(PropertyId id)
        {
            return this.propertyState.GetEffectiveProperty(id);
        }

        public PropertyValue GetDistinctProperty(PropertyId id)
        {
            return this.propertyState.GetDistinctProperty(id);
        }

        public void SubtractDefaultContainerPropertiesFromDistinct(FlagProperties flags, Property[] properties)
        {
            this.propertyState.SubtractDefaultFromDistinct(flags, properties);
        }

        

        public virtual bool Flush()
        {
            while (this.CanAcceptMoreOutput && this.currentOutputLevel.state != OutputState.Ended)
            {
                if (this.currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (this.StartCurrentLevel())
                    {
                        this.PushFirstChild();
                    }
                    else
                    {
                        
                        this.PopPushNextSibling();
                    }
                }
                else if (this.currentOutputLevel.state == OutputState.Started)
                {
                    if (this.ContinueCurrentLevel())
                    {
                        this.currentOutputLevel.state = OutputState.EndPending;
                    }
                }
                else 
                {
                    InternalDebug.Assert(this.currentOutputLevel.state == OutputState.EndPending);

                    this.EndCurrentLevel();

                    this.currentOutputLevel.state = OutputState.Ended;

                    if (this.outputStackTop != 0)
                    {
                        
                        this.PopPushNextSibling();
                    }
                }
            }

            InternalDebug.Assert(this.currentOutputLevel.state != OutputState.Ended || this.outputStackTop == 0);
            return this.currentOutputLevel.state == OutputState.Ended;
        }

        

        public void OutputFragment(FormatNode fragmentNode)
        {
            this.Restart(fragmentNode);
            this.FlushFragment();
        }

        

        public void OutputFragment(FormatNode beginNode, uint beginTextPosition, FormatNode endNode, uint endTextPosition)
        {
            this.Restart(this.rootNode);

            FormatNode node = beginNode;
            int countStartLevels = 0;

            

            while (node != this.rootNode)
            {
                countStartLevels ++;
                node = node.Parent;
            }

            if (this.outputStack == null)
            {
                InternalDebug.Assert(this.outputStackTop == 0 && this.formatStore != null);

                this.outputStack = new OutputStackEntry[Math.Max(32, countStartLevels)];
            }
            else if (this.outputStack.Length < countStartLevels)
            {
                if (this.outputStackTop >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                }

                this.outputStack = new OutputStackEntry[Math.Max(this.outputStack.Length * 2, countStartLevels)];
            }

            

            node = beginNode;
            int level = countStartLevels - 1;
            while (node != this.rootNode)
            {
                this.outputStack[level--].node = node;
                node = node.Parent;
            }
            InternalDebug.Assert(level == -1);

            

            for (level = 0; level < countStartLevels; level++)
            {
                if (!this.StartCurrentLevel())
                {
                    this.PopPushNextSibling();
                    break;
                }

                this.currentOutputLevel.state = OutputState.Started;

                this.Push(this.outputStack[level].node);
            }

            bool endReached = false;

            while (this.currentOutputLevel.state != OutputState.Ended)
            {
                if (this.currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (this.StartCurrentLevel())
                    {
                        this.PushFirstChild();
                    }
                    else
                    {
                        
                        this.PopPushNextSibling();
                    }
                }
                else if (this.currentOutputLevel.state == OutputState.Started)
                {
                    InternalDebug.Assert(this.currentOutputLevel.node.NodeType == FormatContainerType.Text);

                    uint nodeBeginTextPosition = this.currentOutputLevel.node == beginNode ? beginTextPosition : this.currentOutputLevel.node.BeginTextPosition;
                    uint nodeEndTextPosition = this.currentOutputLevel.node == endNode ? endTextPosition : this.currentOutputLevel.node.EndTextPosition;

                    if (nodeBeginTextPosition <= nodeEndTextPosition)
                    {
                        this.ContinueText(nodeBeginTextPosition, nodeEndTextPosition);
                    }

                    this.currentOutputLevel.state = OutputState.EndPending;
                }
                else 
                {
                    InternalDebug.Assert(this.currentOutputLevel.state == OutputState.EndPending);

                    this.EndCurrentLevel();

                    this.currentOutputLevel.state = OutputState.Ended;

                    if (this.outputStackTop != 0)
                    {
                        if (!endReached &&
                            this.currentOutputLevel.node != endNode &&
                            (this.currentOutputLevel.node.NextSibling.IsNull ||
                            this.currentOutputLevel.node.NextSibling != endNode ||
                            (this.currentOutputLevel.node.NextSibling.NodeType == FormatContainerType.Text &&
                                this.currentOutputLevel.node.NextSibling.BeginTextPosition < endTextPosition)))
                        {
                            
                            this.PopPushNextSibling();
                        }
                        else
                        {
                            this.Pop();

                            

                            this.currentOutputLevel.state = OutputState.EndPending;

                            endReached = true;
                        }
                    }
                }
            }

            InternalDebug.Assert(this.outputStackTop == 0);
            InternalDebug.Assert(this.propertyState.UndoStackTop == 0);
        }

        

        private void FlushFragment()
        {
            
            
            

            while (this.currentOutputLevel.state != OutputState.Ended)
            {
                if (this.currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (this.StartCurrentLevel())
                    {
                        this.PushFirstChild();
                    }
                    else
                    {
                        
                        this.PopPushNextSibling();
                    }
                }
                else if (this.currentOutputLevel.state == OutputState.Started)
                {
                    if (this.ContinueCurrentLevel())
                    {
                        this.currentOutputLevel.state = OutputState.EndPending;
                    }
                }
                else 
                {
                    InternalDebug.Assert(this.currentOutputLevel.state == OutputState.EndPending);

                    this.EndCurrentLevel();

                    this.currentOutputLevel.state = OutputState.Ended;

                    if (this.outputStackTop != 0)
                    {
                        
                        this.PopPushNextSibling();
                    }
                }
            }

            InternalDebug.Assert(this.outputStackTop == 0);
            InternalDebug.Assert(this.propertyState.UndoStackTop == 0);
        }

        

        private bool StartCurrentLevel()
        {
            switch (this.currentOutputLevel.node.NodeType)
            {
                case FormatContainerType.Root:

                        return this.StartRoot();

                case FormatContainerType.Document:

                        return this.StartDocument();

                case FormatContainerType.Fragment:

                        return this.StartFragment();

                case FormatContainerType.BaseFont:

                        this.StartEndBaseFont();
                        
                        return false;

                case FormatContainerType.Block:

                        return this.StartBlock();

                case FormatContainerType.BlockQuote:

                        return this.StartBlockQuote();

                case FormatContainerType.TableContainer:

                        return this.StartTableContainer();

                case FormatContainerType.TableDefinition:

                        return this.StartTableDefinition();

                case FormatContainerType.TableColumnGroup:

                        return this.StartTableColumnGroup();

                case FormatContainerType.TableColumn:

                        this.StartEndTableColumn();
                        
                        return false;

                case FormatContainerType.TableCaption:

                        return this.StartTableCaption();

                case FormatContainerType.TableExtraContent:

                        return this.StartTableExtraContent();

                case FormatContainerType.Table:

                        return this.StartTable();

                case FormatContainerType.TableRow:

                        return this.StartTableRow();

                case FormatContainerType.TableCell:

                        return this.StartTableCell();

                case FormatContainerType.List:

                        return this.StartList();

                case FormatContainerType.ListItem:

                        return this.StartListItem();

                case FormatContainerType.HyperLink:

                        return this.StartHyperLink();

                case FormatContainerType.Bookmark:

                        return this.StartBookmark();

                case FormatContainerType.Image:

                        this.StartEndImage();
                        
                        return false;

                case FormatContainerType.HorizontalLine:

                        this.StartEndHorizontalLine();
                        
                        return false;

                case FormatContainerType.Inline:

                        return this.StartInline();

                case FormatContainerType.Map:

                        return this.StartMap();

                case FormatContainerType.Area:

                        this.StartEndArea();
                        
                        return false;

                case FormatContainerType.Form:

                        return this.StartForm();

                case FormatContainerType.FieldSet:

                        return this.StartFieldSet();

                case FormatContainerType.Label:

                        return this.StartLabel();

                case FormatContainerType.Input:

                        return this.StartInput();

                case FormatContainerType.Button:

                        return this.StartButton();

                case FormatContainerType.Legend:

                        return this.StartLegend();

                case FormatContainerType.TextArea:

                        return this.StartTextArea();

                case FormatContainerType.Select:

                        return this.StartSelect();

                case FormatContainerType.OptionGroup:

                        return this.StartOptionGroup();

                case FormatContainerType.Option:

                        return this.StartOption();

                case FormatContainerType.Text:

                        
                        
                        return this.StartText();
            }

            
            InternalDebug.Assert(false);
            return true;
        }

        

        private bool ContinueCurrentLevel()
        {
            InternalDebug.Assert(this.currentOutputLevel.node.NodeType == FormatContainerType.Text);

            

            return this.ContinueText(this.currentOutputLevel.node.BeginTextPosition, this.currentOutputLevel.node.EndTextPosition);
        }

        

        private void EndCurrentLevel()
        {
            switch (this.currentOutputLevel.node.NodeType)
            {
#if DEBUG
                default:
                case FormatContainerType.BaseFont:
                case FormatContainerType.Image:
                case FormatContainerType.HorizontalLine:
                case FormatContainerType.Area:
                case FormatContainerType.TableColumn:

                        InternalDebug.Assert(false);
                        break;
#endif
                case FormatContainerType.Root:

                        this.EndRoot();
                        break;

                case FormatContainerType.Document:

                        this.EndDocument();
                        break;

                case FormatContainerType.Fragment:

                        this.EndFragment();
                        break;

                case FormatContainerType.Block:

                        this.EndBlock();
                        break;

                case FormatContainerType.BlockQuote:

                        this.EndBlockQuote();
                        break;

                case FormatContainerType.TableContainer:

                        this.EndTableContainer();
                        break;

                case FormatContainerType.TableDefinition:

                        this.EndTableDefinition();
                        break;

                case FormatContainerType.TableColumnGroup:

                        this.EndTableColumnGroup();
                        break;

                case FormatContainerType.TableCaption:

                        this.EndTableCaption();
                        break;

                case FormatContainerType.TableExtraContent:

                        this.EndTableExtraContent();
                        break;

                case FormatContainerType.Table:

                        this.EndTable();
                        break;

                case FormatContainerType.TableRow:

                        this.EndTableRow();
                        break;

                case FormatContainerType.TableCell:

                        this.EndTableCell();
                        break;

                case FormatContainerType.List:

                        this.EndList();
                        break;

                case FormatContainerType.ListItem:

                        this.EndListItem();
                        break;

                case FormatContainerType.HyperLink:

                        this.EndHyperLink();
                        break;

                case FormatContainerType.Bookmark:

                        this.EndBookmark();
                        break;

                case FormatContainerType.Inline:

                        this.EndInline();
                        break;

                case FormatContainerType.Map:

                        this.EndMap();
                        break;

                case FormatContainerType.Form:

                        this.EndForm();
                        break;

                case FormatContainerType.FieldSet:

                        this.EndFieldSet();
                        break;

                case FormatContainerType.Label:

                        this.EndLabel();
                        break;

                case FormatContainerType.Input:

                        this.EndInput();
                        break;

                case FormatContainerType.Button:

                        this.EndButton();
                        break;

                case FormatContainerType.Legend:

                        this.EndLegend();
                        break;

                case FormatContainerType.TextArea:

                        this.EndTextArea();
                        break;

                case FormatContainerType.Select:

                        this.EndSelect();
                        break;

                case FormatContainerType.OptionGroup:

                        this.EndOptionGroup();
                        break;

                case FormatContainerType.Option:

                        this.EndOption();
                        break;

                case FormatContainerType.Text:

                        this.EndText();
                        break;
            }
        }

        

        private void PushFirstChild()
        {
            FormatNode firstChild = this.currentOutputLevel.node.FirstChild;

            if (!firstChild.IsNull)
            {
                this.currentOutputLevel.state = OutputState.Started;

                this.Push(firstChild);
            }
            else if (this.currentOutputLevel.node.IsText)
            {
                
                
                this.currentOutputLevel.state = OutputState.Started;
            }
            else
            {
                this.currentOutputLevel.state = OutputState.EndPending;
            }
        }

        

        private void PopPushNextSibling()
        {
            FormatNode nextSibling = this.currentOutputLevel.node.NextSibling;

            this.Pop();

            this.currentOutputLevel.childIndex++;

            if (!nextSibling.IsNull)
            {
                InternalDebug.Assert(this.currentOutputLevel.state == OutputState.Started);

                this.Push(nextSibling);
            }
            else
            {
                this.currentOutputLevel.state = OutputState.EndPending;
            }
        }

        

        private void Push(FormatNode node)
        {
            if (this.outputStack == null)
            {
                InternalDebug.Assert(this.outputStackTop == 0 && this.formatStore != null);

                this.outputStack = new OutputStackEntry[32];
            }
            else if (this.outputStackTop == this.outputStack.Length)
            {
                if (this.outputStackTop >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                }

                OutputStackEntry[] newOutputStack = new OutputStackEntry[this.outputStack.Length * 2];
                Array.Copy(this.outputStack, 0, newOutputStack, 0, this.outputStackTop);
                this.outputStack = newOutputStack;
            }

#if DEBUG
            if (this.formatOutputTraceWriter != null)
            {
                this.formatOutputTraceWriter.WriteLine("{0}{1} Push {2}", Indent(this.outputStackTop), this.outputStackTop, node.NodeType.ToString());
                
                this.formatOutputTraceWriter.Flush();
            }
#endif
            this.outputStack[this.outputStackTop++] = this.currentOutputLevel;

            this.currentOutputLevel.node = node;
            this.currentOutputLevel.state = OutputState.NotStarted;
            this.currentOutputLevel.index = this.currentOutputLevel.childIndex;
            this.currentOutputLevel.childIndex = 0;
            this.currentOutputLevel.propertyUndoLevel = propertyState.ApplyProperties(node.FlagProperties, node.Properties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].flagProperties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].propertyMask);
#if DEBUG
            if (this.formatOutputTraceWriter != null)
            {
                this.formatOutputTraceWriter.WriteLine("{0}{1} Props After {2}", Indent(this.outputStackTop), this.outputStackTop, this.propertyState.ToString());
                this.formatOutputTraceWriter.Flush();
            }
#endif
            
            node.SetOnLeftEdge();
        }

        

        private void Pop()
        {
            InternalDebug.Assert(this.outputStackTop != 0);

            if (this.outputStackTop != 0)
            {
#if DEBUG
                if (this.formatOutputTraceWriter != null)
                {
                    this.formatOutputTraceWriter.WriteLine("{0}{1} Pop {2}", Indent(this.outputStackTop), this.outputStackTop, this.currentOutputLevel.node.NodeType.ToString());
                    
                    this.formatOutputTraceWriter.Flush();
                }
#endif
                
                this.currentOutputLevel.node.ResetOnLeftEdge();

                
                propertyState.UndoProperties(this.currentOutputLevel.propertyUndoLevel);

                

                this.currentOutputLevel = this.outputStack[--this.outputStackTop];
#if DEBUG
                if (this.formatOutputTraceWriter != null)
                {
                    
                    this.formatOutputTraceWriter.Flush();
                }
#endif
            }
        }

        

        

        protected virtual bool StartRoot()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndRoot()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartDocument()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndDocument()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartFragment()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndFragment()
        {
            this.EndBlockContainer();
        }

        protected virtual void StartEndBaseFont()
        {
        }

        protected virtual bool StartBlock()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndBlock()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartBlockQuote()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndBlockQuote()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartTableContainer()
        {
            return true;
        }

        protected virtual void EndTableContainer()
        {
        }

        protected virtual bool StartTableDefinition()
        {
            return true;
        }

        protected virtual void EndTableDefinition()
        {
        }

        protected virtual bool StartTableColumnGroup()
        {
            return true;
        }

        protected virtual void EndTableColumnGroup()
        {
        }

        protected virtual void StartEndTableColumn()
        {
        }

        protected virtual bool StartTableCaption()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndTableCaption()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartTableExtraContent()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndTableExtraContent()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartTable()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndTable()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartTableRow()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndTableRow()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartTableCell()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndTableCell()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartList()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndList()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartListItem()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndListItem()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartHyperLink()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndHyperLink()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartBookmark()
        {
            return true;
        }

        protected virtual void EndBookmark()
        {
        }

        protected virtual void StartEndImage()
        {
        }

        protected virtual void StartEndHorizontalLine()
        {
        }

        protected virtual bool StartInline()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndInline()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartMap()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndMap()
        {
            this.EndBlockContainer();
        }

        protected virtual void StartEndArea()
        {
        }

        protected virtual bool StartForm()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndForm()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartFieldSet()
        {
            return this.StartBlockContainer();
        }

        protected virtual void EndFieldSet()
        {
            this.EndBlockContainer();
        }

        protected virtual bool StartLabel()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndLabel()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartInput()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndInput()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartButton()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndButton()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartLegend()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndLegend()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartTextArea()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndTextArea()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartSelect()
        {
            return this.StartInlineContainer();
        }

        protected virtual void EndSelect()
        {
            this.EndInlineContainer();
        }

        protected virtual bool StartOptionGroup()
        {
            return true;
        }

        protected virtual void EndOptionGroup()
        {
        }

        protected virtual bool StartOption()
        {
            return true;
        }

        protected virtual void EndOption()
        {
        }


        protected virtual bool StartText()
        {
            return this.StartInlineContainer();
        }

        protected virtual bool ContinueText(uint beginTextPosition, uint endTextPosition)
        {
            return true;
        }

        protected virtual void EndText()
        {
            this.EndInlineContainer();
        }

        private static string Indent(int level)
        {
            const string spaces = "                                                  ";
            return spaces.Substring(0, Math.Min(spaces.Length, level * 2));
        }

        protected virtual bool StartBlockContainer()
        {
            return true;
        }

        protected virtual void EndBlockContainer()
        {
        }

        protected virtual bool StartInlineContainer()
        {
            return true;
        }

        protected virtual void EndInlineContainer()
        {
        }

        

        protected virtual void Dispose(bool disposing)
        {
            this.currentOutputLevel.node = FormatNode.Null;
            this.outputStack = null;
            this.formatStore = null;
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

