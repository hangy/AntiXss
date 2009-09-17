// ***************************************************************
// <copyright file="FormatConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    internal enum PropertyPrecedence : byte
    {
        InlineStyle = 0,
        StyleBase = 1,
        NonStyle = 9,
        TagDefault = 10,
        Inherited = 11,
    }

    

    internal abstract class FormatConverter : IProgressMonitor
    {
        internal FormatStore store;

        
        internal BuildStackEntry[] buildStack;
        internal int buildStackTop;
        internal int lastNode;

        #if DEBUG
        internal static int buildStackNextStamp;
        #endif

        private bool mustFlush;             

        
        internal bool emptyContainer;
        internal bool containerFlushed;

        internal StyleBuildHelper containerStyleBuildHelper;

        internal StyleBuildHelper styleBuildHelper;
        internal bool styleBuildHelperLocked;
#if false
        internal StringBuildHelper stringBuildHelper;
        internal bool stringBuildHelperLocked;
#endif
        internal MultiValueBuildHelper multiValueBuildHelper;
        internal bool multiValueBuildHelperLocked;

        internal Dictionary<string, PropertyValue> fontFaceDictionary;
#if DEBUG
        internal TextWriter formatConverterTraceWriter;
#endif
        private bool endOfFile;

        protected bool madeProgress;

        bool newLine;
        bool textQuotingExpected;

        

        internal FormatConverter(Stream formatConverterTraceStream)
        {
            this.store = new FormatStore();
#if DEBUG
            if (formatConverterTraceStream != null)
            {
                this.formatConverterTraceWriter = new StreamWriter(formatConverterTraceStream);
            }
#endif
            this.buildStack = new BuildStackEntry[16];

            this.containerStyleBuildHelper = new StyleBuildHelper(this.store);

            this.styleBuildHelper = new StyleBuildHelper(this.store);
            this.multiValueBuildHelper = new MultiValueBuildHelper(this.store);
#if false
            this.stringBuildHelper = new StringBuildHelper(this.store);
#endif
            this.fontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
        }

        

        internal FormatConverter(FormatStore formatStore, Stream formatConverterTraceStream)
        {
            this.store = formatStore;
#if DEBUG
            if (formatConverterTraceStream != null)
            {
                this.formatConverterTraceWriter = new StreamWriter(formatConverterTraceStream);
            }
#endif
            this.buildStack = new BuildStackEntry[16];

            this.containerStyleBuildHelper = new StyleBuildHelper(this.store);

            this.styleBuildHelper = new StyleBuildHelper(this.store);
            this.multiValueBuildHelper = new MultiValueBuildHelper(this.store);
#if false
            this.stringBuildHelper = new StringBuildHelper(this.store);
#endif
            this.fontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
        }

        internal FormatStore Store
        {
            get { return this.store; }
        }


        

        protected bool MustFlush
        {
            get
            {
                
                
                

                return this.mustFlush;
            }

            set
            {
                this.mustFlush = value;
            }
        }

        

        private void Initialize()
        {
            

            this.buildStackTop = 0;

            this.containerStyleBuildHelper.Clean();

            this.styleBuildHelper.Clean();
            this.styleBuildHelperLocked = false;
#if false
            this.stringBuildHelper.Clean();
            this.stringBuildHelperLocked = false;
#endif
            this.multiValueBuildHelper.Cancel();
            this.multiValueBuildHelperLocked = false;

            this.fontFaceDictionary.Clear();

            this.lastNode = this.store.RootNode.Handle;

            

            this.buildStack[this.buildStackTop].type= FormatContainerType.Root;
            this.buildStack[this.buildStackTop].node = this.store.RootNode.Handle;
            this.buildStackTop ++;

            this.emptyContainer = false;
            this.containerFlushed = true;

            this.mustFlush = false;
            this.endOfFile = false;

            this.newLine = true;
            this.textQuotingExpected = true;
        }

        

        internal FormatNode InitializeDocument()
        {
            this.Initialize();

            
            FormatConverterContainer documentContainer = this.OpenContainer(FormatContainerType.Document, false);
            return documentContainer.Node;
        }

        

        internal FormatNode InitializeFragment()
        {
            this.Initialize();

            
            FormatConverterContainer fragmentContainer = this.OpenContainer(FormatContainerType.Fragment, false);

            this.OpenContainer(FormatContainerType.PropertyContainer, false);

            this.Last.SetProperty(0, PropertyId.FontFace, this.RegisterFaceName(false, "Times New Roman"));
            this.Last.SetProperty(0, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 11));

            return fragmentContainer.Node;
        }

        

        public FormatConverterContainer Root
        {
            get
            {
                InternalDebug.Assert(this.buildStackTop >= 0);
                return new FormatConverterContainer(this, 0);
            }
        }

        

        public FormatConverterContainer Last
        {
            get
            {
                InternalDebug.Assert(this.buildStackTop >= 0);
                return new FormatConverterContainer(this, this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }
        }

        

        public FormatNode LastNode
        {
            get
            {
                return new FormatNode(this.store, this.lastNode);
            }
        }

        

        public FormatConverterContainer LastNonEmpty
        {
            get
            {
                InternalDebug.Assert(this.buildStackTop > 0);
                return new FormatConverterContainer(this, this.buildStackTop - 1);
            }
        }

        public abstract void Run();

        public bool EndOfFile { get { return this.endOfFile; } }

#if true
        public virtual FormatStore ConvertToStore()
        {
            long loopsWithoutProgress = 0;

            while (!this.endOfFile)
            {
                

                this.Run();

                if (this.madeProgress)
                {
                    this.madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (200000 == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            return this.Store;
        }
#endif
        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty)
        {
            InternalDebug.Assert(this.buildStackTop > 0);

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3}", Indent(this.buildStackTop), this.buildStackTop, nodeType.ToString(), empty ? " (empty)" : "");
            }
#endif
            
            int stackPos = this.PushContainer(nodeType, empty, FormatStoreData.DefaultInheritanceMaskIndex.Any);

            return new FormatConverterContainer(this, stackPos);
        }

        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlNameIndex tagName)
        {
            InternalDebug.Assert(this.buildStackTop > 0);

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3} ({4})", Indent(this.buildStackTop), this.buildStackTop, nodeType.ToString(), empty ? " (empty)" : "", tagName.ToString());
            }
#endif
            
            int stackPos = this.PushContainer(nodeType, empty, inheritanceMaskIndex);

            if (!baseStyle.IsNull)
            {
                baseStyle.AddRef();     
                this.containerStyleBuildHelper.AddStyle((int)PropertyPrecedence.TagDefault, baseStyle.Handle);
            }

            this.buildStack[stackPos].tagName = tagName;

            return new FormatConverterContainer(this, stackPos);
        }

        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlTagIndex tagIndex)
        {
            InternalDebug.Assert(this.buildStackTop > 0);

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3} ({4})", Indent(this.buildStackTop), this.buildStackTop, nodeType.ToString(), empty ? " (empty)" : "", tagIndex.ToString());
            }
#endif
            
            int stackPos = this.PushContainer(nodeType, empty, inheritanceMaskIndex);

            if (!baseStyle.IsNull)
            {
                baseStyle.AddRef();     
                this.containerStyleBuildHelper.AddStyle((int)PropertyPrecedence.TagDefault, baseStyle.Handle);
            }

            this.buildStack[stackPos].tagIndex = tagIndex;

            return new FormatConverterContainer(this, stackPos);
        }

        

        public void OpenTextContainer(/*TextMapping textMapping*/)
        {
            InternalDebug.Assert(this.buildStackTop > 0);

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }

            this.PrepareToAddText(/*textMapping*/);
        }

        

        public void CloseContainer()
        {
            InternalDebug.Assert(this.buildStackTop > 1, "should never close the root container");

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer : {2}", Indent(this.buildStackTop - 1), this.buildStackTop - 1, this.buildStack[this.buildStackTop - 1].type.ToString());
            }
#endif
            this.PopContainer();
        }

        

        public void CloseOverlappingContainer(int countLevelsToKeepOpen)
        {
            InternalDebug.Assert(countLevelsToKeepOpen > 0 && countLevelsToKeepOpen < this.buildStackTop - 2);

            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} CloseOverlappedContainer({2}): {3}", Indent(this.buildStackTop - 1 - countLevelsToKeepOpen), this.buildStackTop - 1 - countLevelsToKeepOpen, countLevelsToKeepOpen, this.buildStack[this.buildStackTop - 1 - countLevelsToKeepOpen].type.ToString());
            }
#endif
            this.PopContainer(this.buildStackTop - 1 - countLevelsToKeepOpen);
        }

        

        public void CloseAllContainersAndSetEOF()
        {
            

            while (this.buildStackTop > 1)
            {
                this.CloseContainer();
            }

            
            this.store.GetNode(this.buildStack[0].node).PrepareToClose(this.store.CurrentTextPosition);

            this.mustFlush = true;
            this.endOfFile = true;
        }

        

        protected void CloseContainer(FormatContainerType containerType)
        {
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }

            for (int i = this.buildStackTop - 1; i > 0; i--)
            {
                if (this.buildStack[i].type == containerType)
                {
#if DEBUG
                    if (this.formatConverterTraceWriter != null)
                    {
                        this.formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer({2}): {3}", Indent(i), i, containerType.ToString(), this.buildStack[i].type.ToString());
                    }
#endif
                    this.PopContainer(i);
                    break;
                }
            }
        }

        

        protected void CloseContainer(HtmlNameIndex tagName)
        {
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
            }

            if (this.emptyContainer)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                }
#endif
                this.PrepareToCloseContainer(this.buildStackTop);
            }

            for (int i = this.buildStackTop - 1; i > 0; i--)
            {
                if (this.buildStack[i].tagName == tagName)
                {
#if DEBUG
                    if (this.formatConverterTraceWriter != null)
                    {
                        this.formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer({2}): {3}", Indent(i), i, tagName.ToString(), this.buildStack[i].type.ToString());
                    }
#endif
                    this.PopContainer(i);
                    break;
                }
            }
        }

        

        protected FormatNode CreateNode(FormatContainerType type)
        {
            FormatNode node = this.store.AllocateNode(type);
            node.EndTextPosition = node.BeginTextPosition;
            node.SetOutOfOrder();

            return node;
        }

        

        public void AddNonSpaceText(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count > 0);

            this.PrepareToAddText();

            InternalDebug.Assert(this.emptyContainer);

            
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}AddText: {1} \"{2}\"", Indent(this.buildStackTop), count, new string(buffer, offset, count));
            }
#endif
            this.newLine = false;

            if (this.textQuotingExpected)
            {
                
                

                if (buffer[offset] == '>')
                {
                    do
                    {
                        this.store.AddText(buffer, offset, 1);
                        offset++;
                        count--;
                    }
                    while (count != 0 && buffer[offset] == '>');

                    if (count == 0)
                    {
                        return;
                    }
                }

                this.store.SetTextBoundary();       
                this.textQuotingExpected = false;
            }

            this.store.AddText(buffer, offset, count);

        }

        

        public void AddSpace(int count)
        {
            this.PrepareToAddText();

            InternalDebug.Assert(this.emptyContainer);

            
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}AddSpace: {1}", Indent(this.buildStackTop), count);
            }
#endif
            this.store.AddSpace(count);

            this.newLine = false;
        }

        

        public void AddLineBreak(int count)
        {
            InternalDebug.Assert(count >= 1);

            this.PrepareToAddText();

            InternalDebug.Assert(this.emptyContainer);

            
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}AddSpace: {1}", Indent(this.buildStackTop), count);
            }
#endif

            if (!this.newLine)
            {
                this.store.AddLineBreak(1);
                this.store.SetTextBoundary();       

                if (count > 1)
                {
                    this.store.AddLineBreak(count - 1);
                }

                this.newLine = true;
                this.textQuotingExpected = true;
            }
            else
            {
                this.store.AddLineBreak(count);
            }
        }

        

        public void AddNbsp(int count)
        {
            this.PrepareToAddText();

            InternalDebug.Assert(this.emptyContainer);

            
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}AddNbsp: {1}", Indent(this.buildStackTop), count);
            }
#endif
            this.store.AddNbsp(count);

            this.newLine = false;
        }

        

        public void AddTabulation(int count)
        {
            this.PrepareToAddText();

            InternalDebug.Assert(this.emptyContainer);

            
            if (!this.containerFlushed)
            {
                this.FlushContainer(this.buildStackTop);
            }
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}AddTabulation: {1}", Indent(this.buildStackTop), count);
            }
#endif
            this.store.AddTabulation(count);

            this.newLine = false;
            this.textQuotingExpected = false;
        }

        

        private void PrepareToAddText()
        {
            if (!this.emptyContainer || !this.buildStack[this.buildStackTop].IsText)
            {
                if (!this.containerFlushed)
                {
                    this.FlushContainer(this.emptyContainer ? this.buildStackTop : this.buildStackTop - 1);
                }

                if (this.emptyContainer)
                {
#if DEBUG
                    if (this.formatConverterTraceWriter != null)
                    {
                        this.formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(this.buildStackTop), this.buildStackTop, this.buildStack[this.buildStackTop].type.ToString());
                    }
#endif
                    this.PrepareToCloseContainer(this.buildStackTop);
                }
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} OpenText", Indent(this.buildStackTop), this.buildStackTop);
                }
#endif
                
                this.PushContainer(FormatContainerType.Text, true, FormatStoreData.DefaultInheritanceMaskIndex.Text);
            }
        }

        

        public StringValue RegisterStringValue(bool isStatic, string value)
        {
            
            return this.store.AllocateStringValue(isStatic, value);
        }

        

        public StringValue RegisterStringValue(bool isStatic, string str, int offset, int count)
        {
            string value = str;
            if (offset != 0 || count != str.Length)
            {
                value = str.Substring(offset, count);
            }

            
            return this.store.AllocateStringValue(isStatic, value);
        }

        

        public StringValue RegisterStringValue(bool isStatic, BufferString value)
        {
            
            return this.store.AllocateStringValue(isStatic, value.ToString());
        }

        

        public PropertyValue RegisterFaceName(bool isStatic, BufferString value)
        {
            if (value.Length == 0)
            {
                return PropertyValue.Null;
            }

            
            return this.RegisterFaceName(isStatic, value.ToString());
        }

        

        public PropertyValue RegisterFaceName(bool isStatic, string faceName)
        {
            if (string.IsNullOrEmpty(faceName))
            {
                return PropertyValue.Null;
            }

            PropertyValue value;
            if (this.fontFaceDictionary.TryGetValue(faceName, out value))
            {
                if (value.IsString)
                {
                    this.store.AddRefValue(value);
                }
                return value;
            }

            StringValue registeredValue = this.RegisterStringValue(isStatic, faceName);
            
            value = registeredValue.PropertyValue;

            if (this.fontFaceDictionary.Count < 100)
            {
                registeredValue.AddRef();
                this.fontFaceDictionary.Add(faceName, value);
            }

            return value;
        }

#if false
        

        public StringValue RegisterStringValue(bool isStatic, out StringValueBuilder builder)
        {
            StringValue registeredValue = this.store.AllocateStringValue(isStatic);
            builder = new StringValueBuilder(this, registeredValue.Handle);

            
            return registeredValue;
        }
#endif
        

        public MultiValue RegisterMultiValue(bool isStatic, out MultiValueBuilder builder)
        {
            MultiValue registeredValue = this.store.AllocateMultiValue(isStatic);
            builder = new MultiValueBuilder(this, registeredValue.Handle);

            
            return registeredValue;
        }

        

        public FormatStyle RegisterStyle(bool isStatic, out StyleBuilder builder)
        {
            FormatStyle style = this.store.AllocateStyle(isStatic);
            builder = new StyleBuilder(this, style.Handle);

            
            return style;
        }

        

        public FormatStyle GetStyle(int styleHandle)
        {
            return this.store.GetStyle(styleHandle);
        }

        

        public StringValue GetStringValue(PropertyValue pv)
        {
            return this.store.GetStringValue(pv);
        }

        

        public MultiValue GetMultiValue(PropertyValue pv)
        {
            return this.store.GetMultiValue(pv);
        }

        

        public void ReleasePropertyValue(PropertyValue pv)
        {
            this.store.ReleaseValue(pv);
        }

        

        private void FlushContainer(int stackPos)
        {
            

            InternalDebug.Assert(!this.containerFlushed);
            InternalDebug.Assert((!this.emptyContainer && stackPos == this.buildStackTop - 1) || (this.emptyContainer && stackPos == this.buildStackTop));
#if DEBUG
            if (this.formatConverterTraceWriter != null)
            {
                this.formatConverterTraceWriter.WriteLine("{0}{1} Flush {2}", Indent(stackPos), stackPos, this.buildStack[stackPos].type.ToString());
            }
#endif
            

            FormatContainerType newType = this.FixContainerType(this.buildStack[stackPos].type, this.containerStyleBuildHelper);

            if (newType != this.buildStack[stackPos].type)
            {
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} Flush converting {2} to {3}", Indent(stackPos), stackPos, this.buildStack[stackPos].type.ToString(), newType.ToString());
                }
#endif
                this.buildStack[stackPos].type = newType;
            }

            this.containerStyleBuildHelper.GetPropertyList(
                            out this.buildStack[stackPos].properties,
                            out this.buildStack[stackPos].flagProperties,
                            out this.buildStack[stackPos].propertyMask);

            if (!this.buildStack[stackPos].IsPropertyContainerOrNull)
            {
                

                if (!this.newLine && 0 != (this.buildStack[stackPos].type & FormatContainerType.BlockFlag))
                {
                    
                    this.store.AddBlockBoundary();

                    this.newLine = true;
                    this.textQuotingExpected = true;
                }

                FormatNode node = this.store.AllocateNode(this.buildStack[stackPos].type);
                node.SetOnRightEdge();
#if DEBUG
                if (this.formatConverterTraceWriter != null)
                {
                    this.formatConverterTraceWriter.WriteLine("{0}{1} Flush creating a {2} node {3} with parent {4}", Indent(stackPos), stackPos, this.buildStack[stackPos].type.ToString(), node.Handle, this.lastNode);
                }
#endif
                if (0 != (this.buildStack[stackPos].type & FormatContainerType.InlineObjectFlag))
                {
                    this.store.AddInlineObject();
                }

                FormatNode parent = this.store.GetNode(this.lastNode);

                int propContainerInheritanceStopLevel; 

                parent = this.GetParentForNewNode(node, parent, stackPos, out propContainerInheritanceStopLevel);

                parent.AppendChild(node);

                this.buildStack[stackPos].node = node.Handle;

                this.lastNode = node.Handle;

                FlagProperties flagProperties;
                PropertyBitMask propertyMask;
                Property[] properties;

                if (propContainerInheritanceStopLevel < stackPos)
                {
                    
                    

                    FlagProperties remainingFlagsMask = FlagProperties.AllOn;
                    PropertyBitMask remainingPropertiesMask = PropertyBitMask.AllOn;

                    for (int i = stackPos; i >= propContainerInheritanceStopLevel && (!remainingFlagsMask.IsClear || !remainingPropertiesMask.IsClear); i--)
                    {
                        
                        

                        if (i == stackPos || this.buildStack[i].type == FormatContainerType.PropertyContainer)
                        {
                            
                            flagProperties = this.buildStack[i].flagProperties & remainingFlagsMask;

                            
                            
                            this.containerStyleBuildHelper.AddProperties((int)PropertyPrecedence.Inherited, flagProperties, remainingPropertiesMask, this.buildStack[i].properties);

                            
                            remainingFlagsMask &= ~this.buildStack[i].flagProperties;
                            remainingPropertiesMask &= ~this.buildStack[i].propertyMask;

                            
                            remainingFlagsMask &= FormatStoreData.GlobalInheritanceMasks[this.buildStack[i].inheritanceMaskIndex].flagProperties;
                            remainingPropertiesMask &= FormatStoreData.GlobalInheritanceMasks[this.buildStack[i].inheritanceMaskIndex].propertyMask;
                        }
                    }

                    this.containerStyleBuildHelper.GetPropertyList(out properties, out flagProperties, out propertyMask);
                }
                else
                {
                    flagProperties = this.buildStack[stackPos].flagProperties;
                    propertyMask = this.buildStack[stackPos].propertyMask;

                    properties = this.buildStack[stackPos].properties;
                    if (properties != null)
                    {
                        
                        for (int j = 0; j < properties.Length; j++)
                        {
                            if (properties[j].Value.IsRefCountedHandle)
                            {
                                this.store.AddRefValue(properties[j].Value);
                            }
                        }
                    }
                }

                node.SetProps(flagProperties, propertyMask, properties, this.buildStack[stackPos].inheritanceMaskIndex);
            }

            this.containerStyleBuildHelper.Clean();
            this.containerFlushed = true;
        }

        

        protected virtual FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
        {
            return type;
        }

        

        protected virtual FormatNode GetParentForNewNode(FormatNode node, FormatNode defaultParent, int stackPos, out int propContainerInheritanceStopLevel)
        {
            propContainerInheritanceStopLevel = this.DefaultPropContainerInheritanceStopLevel(stackPos);
            return defaultParent;
        }

        

        protected int DefaultPropContainerInheritanceStopLevel(int stackPos)
        {
            int i = stackPos - 1;

            for (; i >= 0; i--)
            {
                if (this.buildStack[i].node != 0)
                {
                    break;
                }
            }

            return i + 1;
        }

        

        private int PushContainer(FormatContainerType type, bool empty, int inheritanceMaskIndex)
        {
            
            InternalDebug.Assert(!this.emptyContainer);

            int stackPos = this.buildStackTop;

            if (stackPos == this.buildStack.Length)
            {
                if (this.buildStack.Length >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                }

                int newSize = (HtmlSupport.HtmlNestingLimit / 2 > this.buildStack.Length) ? this.buildStack.Length * 2 : HtmlSupport.HtmlNestingLimit;

                BuildStackEntry[] newBuildStack = new BuildStackEntry[newSize];
                Array.Copy(this.buildStack, 0, newBuildStack, 0, this.buildStackTop);
                this.buildStack = newBuildStack;
                newBuildStack = null;
            }

            this.store.SetTextBoundary();
            
            
            this.buildStack[stackPos].type = type;
            this.buildStack[stackPos].tagName = HtmlNameIndex._NOTANAME;
            this.buildStack[stackPos].inheritanceMaskIndex = inheritanceMaskIndex;
            this.buildStack[stackPos].tagIndex = HtmlTagIndex._NULL;
            this.buildStack[stackPos].flagProperties.ClearAll();
            this.buildStack[stackPos].propertyMask.ClearAll();
            this.buildStack[stackPos].properties = null;

            #if DEBUG
            this.buildStack[stackPos].stamp = FormatConverter.buildStackNextStamp++;
            #endif

            this.buildStack[stackPos].node = 0;

            if (!empty)
            {
                this.buildStackTop ++;
            }

            this.emptyContainer = empty;
            this.containerFlushed = false;

            return stackPos;
        }

        

        private void PopContainer()
        {
            InternalDebug.Assert(!this.emptyContainer);
            InternalDebug.Assert(this.buildStackTop > 1 && this.buildStack[0].node != 0);

            this.PrepareToCloseContainer(this.buildStackTop - 1);

            this.buildStackTop --;

            #if DEBUG
            this.buildStack[this.buildStackTop].stamp = 0;
            #endif
        }

        

        private void PopContainer(int level)
        {
            
            InternalDebug.Assert(!this.emptyContainer);
            InternalDebug.Assert(level >= 0 && level < this.buildStackTop && this.buildStack[0].node != 0);

            this.PrepareToCloseContainer(level);

            
            Array.Copy(this.buildStack, level + 1, this.buildStack, level, this.buildStackTop - level - 1);

            this.buildStackTop --;

            #if DEBUG
            this.buildStack[this.buildStackTop].stamp = 0;
            #endif
        }

        

        private void PrepareToCloseContainer(int stackPosition)
        {
            InternalDebug.Assert(stackPosition > 0 && stackPosition <= this.buildStackTop);

            

            if (null != this.buildStack[stackPosition].properties)
            {
                for (int i = 0; i < this.buildStack[stackPosition].properties.Length; i++)
                {
                    if (this.buildStack[stackPosition].properties[i].Value.IsRefCountedHandle)
                    {
                        this.store.ReleaseValue(this.buildStack[stackPosition].properties[i].Value);
                    }
                }

                this.buildStack[stackPosition].properties = null;
            }

            if (this.buildStack[stackPosition].node != 0)
            {
                FormatNode node = this.store.GetNode(this.buildStack[stackPosition].node);

                if (!this.newLine && 0 != (node.NodeType & FormatContainerType.BlockFlag))
                {
                    this.store.AddBlockBoundary();

                    this.newLine = true;
                    this.textQuotingExpected = true;
                }

                node.PrepareToClose(this.store.CurrentTextPosition);

                if (!node.Parent.IsNull && node.Parent.NodeType == FormatContainerType.TableContainer)
                {
                    node.Parent.PrepareToClose(this.store.CurrentTextPosition);
                }

                
                
                

                
                

                
                
                
                
                
                
                
                

                if (this.buildStack[stackPosition].node == this.lastNode)
                {
                    for (int i = stackPosition - 1; i >= 0; i--)
                    {
                        if (this.buildStack[i].node != 0)
                        {
                            this.lastNode = this.buildStack[i].node;
                            break;
                        }
                    }
                }
            }

            this.store.SetTextBoundary();

            this.emptyContainer = false;
        }

        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        private static string Indent(int level)
        {
            const string spaces = "                                                  ";
            return spaces.Substring(0, Math.Min(spaces.Length, level * 2));
        }

        

        
        internal struct BuildStackEntry
        {
            internal FormatContainerType type;

            internal HtmlNameIndex tagName;
            internal HtmlTagIndex tagIndex;

            internal int node;                                  

            internal int inheritanceMaskIndex;                  
            internal FlagProperties flagProperties;             
            internal PropertyBitMask propertyMask;              
            internal Property[] properties;                     

            #if DEBUG
            internal int stamp;
            #endif

            public bool IsText
            {
                get { return this.type == FormatContainerType.Text; }
            }

            public bool IsPropertyContainer
            {
                get { return this.type == FormatContainerType.PropertyContainer; }
            }

            public bool IsPropertyContainerOrNull
            {
                get { return this.type == FormatContainerType.PropertyContainer || this.type == FormatContainerType.Null; }
            }

            #if DEBUG
            public int Stamp
            {
                get { return this.stamp; }
                set { this.stamp = value; }
            }
            #endif

            public FormatContainerType NodeType
            {
                get { return this.type; }
                set { this.type = value; }
            }

            public FlagProperties FlagProperties
            {
                get { return this.flagProperties; }
                set { this.flagProperties = value; }
            }

            public PropertyBitMask PropertyMask
            {
                get { return this.propertyMask; }
                set { this.propertyMask = value; }
            }

            public void Clean()
            {
                this = new BuildStackEntry();
            }
        }
    }


    

    internal struct FormatConverterContainer
    {
        private FormatConverter converter;
        private int level;
        #if DEBUG
        private int stamp;
        #endif

        public static readonly FormatConverterContainer Null = new FormatConverterContainer(null, -1);

        internal FormatConverterContainer(FormatConverter converter, int level)
        {
            InternalDebug.Assert((converter == null && level == -1) || (level >= 0 && level <= converter.buildStackTop));

            this.converter = converter;
            this.level = level;

            #if DEBUG
            this.stamp = (converter != null) ? converter.buildStack[level].stamp : -1;
            #endif
        }

        public bool IsNull { get { return this.converter == null; } }

        public FormatContainerType Type
        {
            get
            {
                this.AssertValid();
                return this.IsNull ? FormatContainerType.Null : this.converter.buildStack[this.level].type;
            }
        }

        public HtmlNameIndex TagName
        {
            get
            {
                this.AssertValid();
                return this.IsNull ? HtmlNameIndex._NOTANAME : this.converter.buildStack[this.level].tagName;
            }
        }

        public FormatConverterContainer Parent
        {
            get
            {
                this.AssertValidAndNotNull();
                return this.level <= 1 ? FormatConverterContainer.Null : new FormatConverterContainer(this.converter, this.level - 1);
            }
        }

        public FormatConverterContainer Child
        {
            get
            {
                this.AssertValidAndNotNull();
                return this.level == converter.buildStackTop - 1 ? FormatConverterContainer.Null : new FormatConverterContainer(this.converter, this.level + 1);
            }
        }

        public FlagProperties FlagProperties
        {
            get { this.AssertValidAndNotNull(); return this.converter.buildStack[this.level].flagProperties; }
            set { this.AssertValidAndNotNull(); this.converter.buildStack[this.level].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { this.AssertValidAndNotNull(); return this.converter.buildStack[this.level].propertyMask; }
            set { this.AssertValidAndNotNull(); this.converter.buildStack[this.level].propertyMask = value; }
        }

        public Property[] Properties
        {
            get { this.AssertValidAndNotNull(); return this.converter.buildStack[this.level].properties; }
        }

        public FormatNode Node
        {
            get { this.AssertValidAndNotNull(); return new FormatNode(this.converter.store, this.converter.buildStack[this.level].node); }
        }

        
        
        
        
        

        public void SetProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, PropertyValue value)
        {
            this.AssertValidNotFlushed();

            
            if (value.IsString)
            {
                this.converter.GetStringValue(value).AddRef();
            }
            else if (value.IsMultiValue)
            {
                this.converter.GetMultiValue(value).AddRef();
            }

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value);
        }

        public void SetProperties(PropertyPrecedence propertyPrecedence, Property[] properties, int propertyCount)
        {
            this.AssertValidNotFlushed();

            for (int i = 0; i < propertyCount; i++)
            {
                this.SetProperty(propertyPrecedence, properties[i].Id, properties[i].Value);
            }
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, StringValue value)
        {
            this.AssertValidNotFlushed();

            
            value.AddRef();

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, string value)
        {
            this.AssertValidNotFlushed();

            StringValue registeredValue = this.converter.RegisterStringValue(false, value);

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, BufferString value)
        {
            this.AssertValidNotFlushed();

            StringValue registeredValue = this.converter.RegisterStringValue(false, value);

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, char[] buffer, int offset, int count)
        {
            this.AssertValidNotFlushed();

            StringValue registeredValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

#if false
        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, out StringValueBuilder stringBuilder)
        {
            this.AssertValidNotFlushed();

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }
#endif
        public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, MultiValue value)
        {
            this.AssertValidNotFlushed();

            
            value.AddRef();

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value.PropertyValue);
        }

        public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
        {
            this.AssertValidNotFlushed();

            MultiValue registeredValue = this.converter.RegisterMultiValue(false, out multiValueBuilder);

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStyleReference(int stylePrecedence, int styleHandle)
        {
            this.AssertValidNotFlushed();
            InternalDebug.Assert(stylePrecedence > 0);

            this.converter.containerStyleBuildHelper.AddStyle(stylePrecedence, styleHandle);
        }

        public void SetStyleReference(int stylePrecedence, FormatStyle style)
        {
            this.AssertValidNotFlushed();
            InternalDebug.Assert(stylePrecedence > 0);

            this.converter.containerStyleBuildHelper.AddStyle(stylePrecedence, style.Handle);
        }

        
        public static bool operator ==(FormatConverterContainer x, FormatConverterContainer y) 
        {
            return x.converter == y.converter && x.level == y.level;
        }

        
        public static bool operator !=(FormatConverterContainer x, FormatConverterContainer y) 
        {
            return x.converter != y.converter || x.level != y.level;
        }

        public override bool Equals(object obj)
        {
            return (obj is FormatConverterContainer) && this.converter == ((FormatConverterContainer)obj).converter && this.level == ((FormatConverterContainer)obj).level;
        }

        public override int GetHashCode()
        {
            return this.level;
        }


        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValidAndNotNull()
        {
            this.AssertValid();
            InternalDebug.Assert(!this.IsNull);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValid()
        {
            #if DEBUG
            InternalDebug.Assert(this.IsNull || (this.level > 0 && this.level <= this.converter.buildStackTop && this.stamp == this.converter.buildStack[this.level].stamp));
            #endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValidNotFlushed()
        {
            this.AssertValidAndNotNull();
            InternalDebug.Assert(!this.converter.containerFlushed &&
                ((this.converter.emptyContainer && this.level == this.converter.buildStackTop) ||
                    (!this.converter.emptyContainer && this.level == this.converter.buildStackTop - 1)));
        }
    }

    

    internal struct StyleBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal StyleBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.styleBuildHelperLocked);
            #if DEBUG
            this.converter.styleBuildHelperLocked = true;
            #endif
        }

        public void SetProperty(PropertyId propertyId, PropertyValue value)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, value);
        }

        public void SetProperties(Property[] properties, int propertyCount)
        {
            for (int i = 0; i < propertyCount; i++)
            {
                this.SetProperty(properties[i].Id, properties[i].Value);
            }
        }

        public void SetStringProperty(PropertyId propertyId, StringValue value)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, string value)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, value);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, BufferString value)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, value);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

#if false
        public void SetStringProperty(PropertyId propertyId, out StringValueBuilder stringBuilder)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }
#endif
        public void SetMultiValueProperty(PropertyId propertyId, MultiValue value)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            
            value.AddRef();

            this.converter.styleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
        }

        public void SetMultiValueProperty(PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            MultiValue registeredValue = this.converter.RegisterMultiValue(false, out multiValueBuilder);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void Flush()
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);
            this.converter.styleBuildHelper.GetPropertyList(
                            out this.converter.store.styles.Plane(this.handle)[this.converter.store.styles.Index(this.handle)].propertyList,
                            out this.converter.store.styles.Plane(this.handle)[this.converter.store.styles.Index(this.handle)].flagProperties,
                            out this.converter.store.styles.Plane(this.handle)[this.converter.store.styles.Index(this.handle)].propertyMask);
            #if DEBUG
            this.converter.styleBuildHelperLocked = false;
            #endif
        }
    }

#if false

    

    internal struct StringValueBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal StringValueBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.stringBuildHelperLocked);
            #if DEBUG
            this.converter.stringBuildHelperLocked = true;
            #endif
        }

        public void AddText(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.stringBuildHelper.AddText(buffer, offset, count);
        }

        public void AddText(string value)
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.stringBuildHelper.AddText(value);
        }

        public void Flush()
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.store.strings[this.handle].str = this.converter.stringBuildHelper.GetString();
            #if DEBUG
            this.converter.stringBuildHelperLocked = false;
            #endif
        }
    }

#endif

    

    internal struct MultiValueBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal MultiValueBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.multiValueBuildHelperLocked);
            #if DEBUG
            this.converter.multiValueBuildHelperLocked = true;
            #endif
        }

        public int Count { get { return this.converter.multiValueBuildHelper.Count; } }

        public PropertyValue this[int i] { get { return this.converter.multiValueBuildHelper[i]; } }

        public void AddValue(PropertyValue value)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);
            
            InternalDebug.Assert(!value.IsMultiValue);

            
            this.converter.multiValueBuildHelper.AddValue(value);
        }

        public void AddStringValue(StringValue value)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);
            
            this.converter.multiValueBuildHelper.AddValue(value.PropertyValue);
        }

        public void AddStringValue(string value)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, value);

            this.converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }

        public void AddStringValue(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            this.converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }
#if false
        public void AddStringValue(out StringValueBuilder stringBuilder)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            this.converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }
#endif
        public void Flush()
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);
            this.converter.store.multiValues.Plane(this.handle)[this.converter.store.multiValues.Index(this.handle)].values = this.converter.multiValueBuildHelper.GetValues();
            #if DEBUG
            this.converter.multiValueBuildHelperLocked = false;
            #endif
        }

        public void Cancel()
        {
            this.converter.multiValueBuildHelper.Cancel();
            #if DEBUG
            this.converter.multiValueBuildHelperLocked = false;
            #endif
        }
    }

    

    internal struct StyleBuildHelper
    {
        internal FormatStore store;

        internal PropertyBitMask propertyMask;
        private PrecedenceEntry[] entries;
        private int topEntry;
        private int currentEntry;
        private int nonFlagPropertiesCount;

        private class PrecedenceEntry
        {
            public int precedence;
            public FlagProperties flagProperties;
            public PropertyBitMask propertyMask;
            public Property[] properties;
            public int count;
            public int nextSearchIndex;

            public PrecedenceEntry(int precedence)
            {
                this.precedence = precedence;
                this.properties = new Property[(int)PropertyId.MaxValue];
            }

            public void ReInitialize(int precedence)
            {
                this.precedence = precedence;
                this.flagProperties.ClearAll();
                this.propertyMask.ClearAll();
                InternalDebug.Assert(this.properties != null);
                this.count = 0;
                this.nextSearchIndex = 0;
            }
        }

        internal StyleBuildHelper(FormatStore store)
        {
            this.store = store;
            this.propertyMask = new PropertyBitMask();
            this.entries = null; 
            this.topEntry = 0;
            this.currentEntry = -1;
            this.nonFlagPropertiesCount = 0;
        }

        public void Clean()
        {
            this.propertyMask.ClearAll();
            this.topEntry = 0;
            this.currentEntry = -1;
            this.nonFlagPropertiesCount = 0;
        }

        private void InitializeEntry(int entry, int precedence)
        {
            if (this.entries == null)
            {
                InternalDebug.Assert(entry == 0);
                this.entries = new PrecedenceEntry[4];
            }
            else if (entry == this.entries.Length)
            {
                if (entry == 16)
                {
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                }

                PrecedenceEntry[] newEntries = new PrecedenceEntry[this.entries.Length * 2];
                Array.Copy(this.entries, 0, newEntries, 0, this.entries.Length);
                this.entries = newEntries;
            }

            if (this.entries[entry] == null)
            {
                this.entries[entry] = new PrecedenceEntry(precedence);
            }
            else
            {
                this.entries[entry].ReInitialize(precedence);
            }
        }

        private int GetEntry(int precedence)
        {
            int entry = this.currentEntry;

            if (entry != -1)
            {
                if (this.entries[entry].precedence != precedence)
                {
                    

                    for (entry = this.topEntry - 1; entry >= 0; entry--)
                    {
                        if (this.entries[entry].precedence < precedence)
                        {
                            break;
                        }
                    }

                    entry++;

                    if (entry == this.topEntry || this.entries[entry].precedence != precedence)
                    {
                        InternalDebug.Assert(entry == this.topEntry || this.entries[entry].precedence > precedence);

                        this.InitializeEntry(this.topEntry, precedence);

                        if (entry < this.topEntry)
                        {
                            PrecedenceEntry saveEntry = this.entries[this.topEntry];

                            for (int i = this.topEntry - 1; i >= entry; i--)
                            {
                                this.entries[i + 1] = this.entries[i];
                            }

                            this.entries[entry] = saveEntry;
                        }

                        this.topEntry ++;
                    }
                }
            }
            else
            {
                
                InternalDebug.Assert(this.topEntry == 0);

                entry = this.topEntry++;
                this.InitializeEntry(entry, precedence);
            }

            this.currentEntry = entry;
            return entry;
        }

        private void AddProperty(int entry, PropertyId id, PropertyValue value)
        {
            int index;

            for (index = this.entries[entry].count - 1; index >= 0; index--)
            {
                if (this.entries[entry].properties[index].Id <= id)
                {
                    break;
                }

                this.entries[entry].properties[index + 1] = this.entries[entry].properties[index];
            }

            this.entries[entry].count ++;
            this.entries[entry].properties[index + 1].Set(id, value);
            this.entries[entry].propertyMask.Set(id);
            this.propertyMask.Set(id);
            this.nonFlagPropertiesCount ++;
        }

        private void RemoveProperty(int entry, int index)
        {
            
            this.entries[entry].propertyMask.Clear(this.entries[entry].properties[index].Id);

            if (this.entries[entry].properties[index].Value.IsRefCountedHandle)
            {
                this.store.ReleaseValue(this.entries[entry].properties[index].Value);
            }

            this.entries[entry].count--;
            for (int i = index; i < this.entries[entry].count; i++)
            {
                this.entries[entry].properties[i] = this.entries[entry].properties[i + 1];
            }
            this.nonFlagPropertiesCount --;
        }

        private void FindProperty(PropertyId id, out int entryFound, out int indexFound)
        {
            InternalDebug.Assert(this.propertyMask.IsSet(id));

            for (entryFound = 0; entryFound < this.topEntry; entryFound++)
            {
                if (this.entries[entryFound].propertyMask.IsSet(id))
                {
                    

                    int start = 0;
                    int end = this.entries[entryFound].count;
                    Property[] properties = this.entries[entryFound].properties;

                    indexFound = this.entries[entryFound].nextSearchIndex;

                    
                    

                    if (indexFound < end)
                    {
                        if (properties[indexFound].Id == id)
                        {
                            this.entries[entryFound].nextSearchIndex++;
                            return;
                        }
                        else if (properties[indexFound].Id < id)
                        {
                            start = indexFound + 1;
                        }
                    }

                    for (indexFound = start; indexFound < end; indexFound++)
                    {
                        if (properties[indexFound].Id == id)
                        {
                            this.entries[entryFound].nextSearchIndex = indexFound + 1;
                            return;
                        }
                    }

                    
                }
            }

            
            
            InternalDebug.Assert(false);
            indexFound = -1;
        }

        private void SetPropertyImpl(int entry, PropertyId id, PropertyValue value)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                if (!value.IsNull)
                {
                    InternalDebug.Assert(value.IsBool);
                    this.entries[entry].flagProperties.Set(id, value.Bool);
                }
                else
                {
                    this.entries[entry].flagProperties.Remove(id);
                }
            }
            else
            {
                if (this.propertyMask.IsSet(id))
                {
                    

                    int entryFound, indexFound;

                    this.FindProperty(id, out entryFound, out indexFound);

                    if (entryFound < entry)
                    {
                        
                        return;
                    }

                    if (entryFound == entry)
                    {
                        if (this.entries[entry].properties[indexFound].Value.IsRefCountedHandle)
                        {
                            this.store.ReleaseValue(this.entries[entry].properties[indexFound].Value);
                        }
                        else if (this.entries[entry].properties[indexFound].Value.IsRelativeHtmlFontUnits && value.IsRelativeHtmlFontUnits)
                        {
                            
                            value = new PropertyValue(PropertyType.RelHtmlFontUnits, this.entries[entry].properties[indexFound].Value.RelativeHtmlFontUnits + value.RelativeHtmlFontUnits);
                        }

                        this.entries[entry].properties[indexFound].Value = value;
                        return;
                    }

                    this.RemoveProperty(entryFound, indexFound);
                }

                if (!value.IsNull)
                {
                    this.AddProperty(entry, id, value);
                }
            }
        }

        public void SetProperty(int precedence, PropertyId id, PropertyValue value)
        {
            

            InternalDebug.Assert(id > PropertyId.Null);

            int entry = this.GetEntry(precedence);

            this.SetPropertyImpl(entry, id, value);
        }

        public void SetProperty(int precedence, Property prop)
        {
            

            InternalDebug.Assert(prop.Id > PropertyId.Null);

            int entry = this.GetEntry(precedence);

            this.SetPropertyImpl(entry, prop.Id, prop.Value);
        }

        public void AddStyle(int precedence, int handle)
        {
            InternalDebug.Assert(handle != 0);

            int entry = this.GetEntry(precedence);

            FormatStyle style = store.GetStyle(handle);

            this.entries[entry].flagProperties.Merge(style.FlagProperties);

            Property[] propList = style.PropertyList;
            if (propList != null)
            {
                for (int i = 0; i < propList.Length; i++)
                {
                    Property prop = propList[i];

                    if (prop.Value.IsRefCountedHandle)
                    {
                        this.store.AddRefValue(prop.Value);
                    }

                    this.SetPropertyImpl(entry, prop.Id, prop.Value);
                }
            }
        }

        public void AddProperties(int precedence, FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propList)
        {
            int entry = this.GetEntry(precedence);

            this.entries[entry].flagProperties.Merge(flagProperties);

            if (propList != null)
            {
                for (int i = 0; i < propList.Length; i++)
                {
                    Property prop = propList[i];
                    if (propertyMask.IsSet(prop.Id))
                    {
                        if (prop.Value.IsRefCountedHandle)
                        {
                            this.store.AddRefValue(prop.Value);
                        }

                        this.SetPropertyImpl(entry, prop.Id, prop.Value);
                    }
                }
            }
        }

        public PropertyValue GetProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                for (int i = 0; i < this.topEntry; i++)
                {
                    if (this.entries[i].flagProperties.IsDefined(id))
                    {
                        return this.entries[i].flagProperties.GetPropertyValue(id);
                    }
                }
            }
            else if (this.propertyMask.IsSet(id))
            {
                int entryFound, indexFound;

                this.FindProperty(id, out entryFound, out indexFound);

                return this.entries[entryFound].properties[indexFound].Value;
            }

            return PropertyValue.Null;
        }

        public void GetPropertyList(
                    out Property[] propertyList,
                    out FlagProperties effectiveFlagProperties,
                    out PropertyBitMask effectivePropertyMask)
        {
            effectiveFlagProperties = new FlagProperties();
            effectivePropertyMask = this.propertyMask;

            #if DEBUG
            PropertyBitMask debugMask = new PropertyBitMask();
            #endif

            for (int i = 0; i < this.topEntry; i++)
            {
                effectiveFlagProperties.Merge(this.entries[i].flagProperties);

                #if DEBUG
                debugMask.Or(this.entries[i].propertyMask);
                #endif
            }

            #if DEBUG
            InternalDebug.Assert(debugMask == this.propertyMask);
            #endif

            if (this.nonFlagPropertiesCount != 0)
            {
                InternalDebug.Assert(!this.propertyMask.IsClear);

                propertyList = new Property[this.nonFlagPropertiesCount];

                if (this.topEntry == 1)
                {
                    

                    InternalDebug.Assert(this.entries[0].count == this.nonFlagPropertiesCount);
                    Array.Copy(this.entries[0].properties, 0, propertyList, 0, this.nonFlagPropertiesCount);
                }
                else if (this.topEntry == 2)
                {
                    

                    Property[] p0 = this.entries[0].properties;
                    Property[] p1 = this.entries[1].properties;
                    int i0 = 0, i1 = 0;
                    int max0 = this.entries[0].count;
                    int max1 = this.entries[1].count;
                    int resultIndex = 0;

                    while (true)
                    {
                        if (i0 < max0)
                        {
                            if (i1 == max1 || p0[i0].Id <= p1[i1].Id)
                            {
                                propertyList[resultIndex++] = p0[i0++];
                            }
                            else
                            {
                                propertyList[resultIndex++] = p1[i1++];
                            }
                        }
                        else if (i1 < max1)
                        {
                            propertyList[resultIndex++] = p1[i1++];
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                }
                else
                {
                    

                    PropertyId id = PropertyId.LastFlag;
                    int resultIndex = 0;

                    while (++id < PropertyId.MaxValue)
                    {
                        if (this.propertyMask.IsSet(id))
                        {
                            int entryFound, indexFound;

                            FindProperty(id, out entryFound, out indexFound);

                            propertyList[resultIndex++] = this.entries[entryFound].properties[indexFound];
                        }
                    }

                    InternalDebug.Assert(resultIndex == propertyList.Length);
                }
            }
            else
            {
                propertyList = null;
            }

            this.Clean();
        }
    }

#if false
    

    internal struct StringBuildHelper
    {
        internal FormatStore store;
        internal char[] buffer;
        internal int count;
        
        internal StringBuildHelper(FormatStore store)
        {
            this.store = store;
            this.buffer = null; 
            this.count = 0;
        }

        public void AddText(char[] buffer, int offset, int count)
        {
            
            if (this.count + count > this.buffer.Length)
            {
                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException();
            }

            Buffer.BlockCopy(buffer, offset * 2, this.buffer, this.count * 2, count * 2);
            this.count += count;
        }

        public void AddText(string value)
        {
            
            if (this.count + value.Length > this.buffer.Length)
            {
                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException();
            }

            value.CopyTo(0, this.buffer, this.count, value.Length);
            this.count += value.Length;
        }

        public string GetString()
        {
            if (this.count == 0)
            {
                return null;
            }

            string result = new string(this.buffer, 0, this.count);
            this.count = 0;
            return result;
        }
    }
#endif

    

    internal struct MultiValueBuildHelper
    {
        internal FormatStore store;
        internal PropertyValue[] values;
        internal int valuesCount;

        internal MultiValueBuildHelper(FormatStore store)
        {
            this.store = store;
            this.values = null;
            this.valuesCount = 0;
        }

        public int Count { get { return this.valuesCount; } }
        public PropertyValue this[int i] { get { return this.values[i]; } }

        public void AddValue(PropertyValue value)
        {
            
            InternalDebug.Assert(!value.IsMultiValue);

            if (this.values == null)
            {
                this.values = new PropertyValue[4];
            }
            else if (this.valuesCount == this.values.Length)
            {
                if (this.valuesCount == 32)
                {
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                }

                PropertyValue[] newValues = new PropertyValue[this.valuesCount * 2];
                Array.Copy(this.values, 0, newValues, 0, this.valuesCount);
                this.values = newValues;
            }

            this.values[this.valuesCount++] = value;
        }

        public PropertyValue[] GetValues()
        {
            if (this.valuesCount == 0)
            {
                return null;
            }

            PropertyValue[] result = new PropertyValue[this.valuesCount];
            Array.Copy(this.values, 0, result, 0, this.valuesCount);
            this.valuesCount = 0;
            return result;
        }

        public void Cancel()
        {
            for (int i = 0; i < this.valuesCount; i++)
            {
                if (this.values[i].IsRefCountedHandle)
                {
                    this.store.ReleaseValue(this.values[i]);
                }
            }
            this.valuesCount = 0;
        }
    }
}

