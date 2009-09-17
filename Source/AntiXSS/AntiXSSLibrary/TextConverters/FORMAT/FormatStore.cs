// ***************************************************************
// <copyright file="FormatStore.cs" company="Microsoft">
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
    using System.Runtime.InteropServices;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    

    internal enum FormatContainerType : byte
    {
        Null                    = 0x00,                 

        Root                    = 0x01 | BlockFlag,     

        Document                = 0x02 | BlockFlag,     
        Fragment                = 0x03 | BlockFlag,     

        Block                   = 0x04 | BlockFlag,     
        BlockQuote              = 0x05 | BlockFlag,     

        HorizontalLine          = 0x06 | BlockFlag,     

        TableContainer          = 0x07,                 
        TableDefinition         = 0x08,                 
        TableColumnGroup        = 0x09,                 
        TableColumn             = 0x0A,                 
        TableCaption            = 0x0B | BlockFlag,     
        TableExtraContent       = 0x0C | BlockFlag,     
        Table                   = 0x0D | BlockFlag,     
        TableRow                = 0x0E | BlockFlag,     
        TableCell               = 0x0F | BlockFlag,     

        List                    = 0x10 | BlockFlag,     
        ListItem                = 0x11 | BlockFlag,     
        
                                                        
                                                        

        Inline                  = 0x12,                 

        HyperLink               = 0x13,                 
                                                        

        Bookmark                = 0x14,                 

        Image                   = 0x15 | InlineObjectFlag,  

        Area                    = 0x16,
        Map                     = 0x17 | BlockFlag,

        BaseFont                = 0x18,

        Form                    = 0x19,
        FieldSet                = 0x1A,
        Label                   = 0x1B,
        Input                   = 0x1C,
        Button                  = 0x1D,
        Legend                  = 0x1E,
        TextArea                = 0x1F,
        Select                  = 0x20,
        OptionGroup             = 0x21,
        Option                  = 0x22,

        Text                    = 0x24,                 
                                                        

        PropertyContainer       = 0x25,                 
                                                        
                                                        

        InlineObjectFlag        = 0x40,                 
        BlockFlag               = 0x80,                 
    }

    

    internal class FormatStore
    {
        
        internal NodeStore nodes;   

        
        internal StyleStore styles;

        
        internal StringValueStore strings;   

        
        internal MultiValueStore multiValues;

        internal TextStore text;

        public FormatStore()
        {
            this.nodes = new NodeStore(this);

            this.styles = new StyleStore(this, FormatStoreData.GlobalStyles);

            this.strings = new StringValueStore(FormatStoreData.GlobalStringValues);

            this.multiValues = new MultiValueStore(this, FormatStoreData.GlobalMultiValues);

            this.text = new TextStore();

            this.Initialize();
        }

        public void Initialize()
        {
            this.nodes.Initialize();

            this.styles.Initialize(FormatStoreData.GlobalStyles);

            this.strings.Initialize(FormatStoreData.GlobalStringValues);

            this.multiValues.Initialize(FormatStoreData.GlobalMultiValues);

            this.text.Initialize();
        }

        public FormatNode RootNode
        {
            get { return new FormatNode(this, 1); }
        }

        public uint CurrentTextPosition
        {
            get { return this.text.CurrentPosition; }
        }

        public void ReleaseValue(PropertyValue value)
        {
            InternalDebug.Assert(value.IsRefCountedHandle);
            if (value.IsString)
            {
                this.GetStringValue(value).Release();
            }
            else if (value.IsMultiValue)
            {
                this.GetMultiValue(value).Release();
            }
        }

        public void AddRefValue(PropertyValue value)
        {
            InternalDebug.Assert(value.IsRefCountedHandle);
            if (value.IsString)
            {
                this.GetStringValue(value).AddRef();
            }
            else if (value.IsMultiValue)
            {
                this.GetMultiValue(value).AddRef();
            }
        }

        public FormatNode GetNode(int nodeHandle)
        {
            return new FormatNode(this, nodeHandle);
        }

        public FormatNode AllocateNode(FormatContainerType type)
        {
            return new FormatNode(this.nodes, this.nodes.Allocate(type, this.CurrentTextPosition));
        }

        public FormatNode AllocateNode(FormatContainerType type, uint beginTextPosition)
        {
            return new FormatNode(this.nodes, this.nodes.Allocate(type, beginTextPosition));
        }

        public void FreeNode(FormatNode node)
        {
            this.nodes.Free(node.Handle);
        }

        public FormatStyle AllocateStyle(bool isStatic)
        {
            return new FormatStyle(this, this.styles.Allocate(isStatic));
        }

        public FormatStyle GetStyle(int styleHandle)
        {
            return new FormatStyle(this, styleHandle);
        }

        public void FreeStyle(FormatStyle style)
        {
            InternalDebug.Assert(style.RefCount == 0);
            this.styles.Free(style.Handle);
        }

        public StringValue AllocateStringValue(bool isStatic)
        {
            return new StringValue(this, this.strings.Allocate(isStatic));
        }

        public StringValue AllocateStringValue(bool isStatic, string value)
        {
            StringValue sv = this.AllocateStringValue(isStatic);
            sv.SetString(value);
            return sv;
        }

        public StringValue GetStringValue(PropertyValue propertyValue)
        {
            return new StringValue(this, propertyValue.StringHandle);
        }

        public void FreeStringValue(StringValue str)
        {
            InternalDebug.Assert(str.RefCount == 0);
            this.strings.Free(str.Handle);
        }

        public MultiValue AllocateMultiValue(bool isStatic)
        {
            return new MultiValue(this, this.multiValues.Allocate(isStatic));
        }

        public MultiValue GetMultiValue(PropertyValue propertyValue)
        {
            return new MultiValue(this, propertyValue.MultiValueHandle);
        }

        public void FreeMultiValue(MultiValue multi)
        {
            InternalDebug.Assert(multi.RefCount == 0);
            this.multiValues.Free(multi.Handle);
        }

        

        public void InitializeCodepageDetector()
        {
            this.text.InitializeCodepageDetector();
        }

        public int GetBestWindowsCodePage()
        {
            return this.text.GetBestWindowsCodePage();
        }

        public int GetBestWindowsCodePage(int preferredCodePage)
        {
            return this.text.GetBestWindowsCodePage(preferredCodePage);
        }

        

        public void SetTextBoundary()
        {
            
            this.text.DoNotMergeNextRun();
        }

        

        public void AddBlockBoundary()
        {
            if (this.text.LastRunType != TextRunType.BlockBoundary)
            {
                this.text.AddSimpleRun(TextRunType.BlockBoundary, 1);
            }
        }

        

        public void AddText(char[] textBuffer, int offset, int count)
        {
            this.text.AddText(TextRunType.NonSpace, textBuffer, offset, count);
        }

        public void AddInlineObject()
        {
            this.text.AddSimpleRun(TextRunType.InlineObject, 1);
            this.text.DoNotMergeNextRun(); 
        }

        public void AddSpace(int count)
        {
            this.text.AddSimpleRun(TextRunType.Space, count);
        }

        public void AddLineBreak(int count)
        {
            this.text.AddSimpleRun(TextRunType.NewLine, count);
        }

        public void AddNbsp(int count)
        {
            this.text.AddSimpleRun(TextRunType.NbSp, count);
        }

        public void AddTabulation(int count)
        {
            this.text.AddSimpleRun(TextRunType.Tabulation, count);
        }

        internal TextRun GetTextRun(uint position)
        {
            InternalDebug.Assert(position <= this.CurrentTextPosition);

            if (position < this.CurrentTextPosition)
            {
                return this.GetTextRunReally(position);
            }

            return TextRun.Invalid;
        }

        internal TextRun GetTextRunReally(uint position)
        {
            return new TextRun(this.text, position);
        }

        
#if DEBUG
        internal void Dump(Stream stream, bool close)
        {
            StreamWriter dumpWriter = new StreamWriter(stream);
            uint runningPosition = 0;

            this.DumpNode(dumpWriter, this.RootNode, 0, false, ref runningPosition);

            this.CalculateAndDumpStatistics(dumpWriter);

            if (close)
            {
                dumpWriter.Close();
            }
            else
            {
                dumpWriter.Flush();
            }
        }

        private static void DumpIndent(TextWriter dumpWriter, int level)
        {
            if (level < 20)
            {
                for (int i = 0; i < level; i++)
                {
                    dumpWriter.Write("   ");
                }
            }
            else
            {
                for (int i = 0; i < level; i++)
                {
                    dumpWriter.Write("   ");
                }
                dumpWriter.Write("{0}: ", level);
            }
        }

        private void DumpNode(TextWriter dumpWriter, FormatNode node, int level, bool outOfOrder, ref uint runningPosition)
        {
            DumpIndent(dumpWriter, level);

            dumpWriter.WriteLine("{0} /{1:X}/ tp:/{2:X}/{3:X}/{4}", node.NodeType.ToString(), node.Handle, node.BeginTextPosition, node.EndTextPosition, node.IsInOrder ? "" : "  out-of-order!");

            NodePropertiesEnumerator enumerator = new NodePropertiesEnumerator(node);
            foreach (Property prop in enumerator)
            {
                DumpIndent(dumpWriter, level + 1);

                dumpWriter.WriteLine("| PROP " + prop.ToString());

                if (prop.Value.IsString)
                {
                    DumpIndent(dumpWriter, level + 1);

                    StringValue sv = this.GetStringValue(prop.Value);
                    dumpWriter.WriteLine("|   = \"" + sv.GetString() + "\"");
                }
                else if (prop.Value.IsMultiValue)
                {
                    MultiValue mv = this.GetMultiValue(prop.Value);

                    for (int k = 0; k < mv.Length; k++)
                    {
                        DumpIndent(dumpWriter, level + 1);

                        dumpWriter.WriteLine("|   = " + mv[k].ToString());

                        if (mv[k].IsString)
                        {
                            DumpIndent(dumpWriter, level + 1);

                            StringValue sv = this.GetStringValue(mv[k]);
                            dumpWriter.WriteLine("|      = \"" + sv.GetString() + "\"");
                        }
                    }
                }
            }
#if false
            if (!outOfOrder && !node.IsText && node.IsInOrder)
            {
                this.DumpStartMarkup(dumpWriter, node, level, ref runningPosition);
            }
#endif
            if (node.IsText)
            {
                
                this.DumpTextRuns(node.BeginTextPosition, node.EndTextPosition, dumpWriter, level);
                if (!outOfOrder)
                {
                    runningPosition = node.EndTextPosition;
                }
            }
            else
            {
                FormatNode child = node.FirstChild;
                while (!child.IsNull)
                {
                    this.DumpNode(dumpWriter, child, level + 1, outOfOrder || !node.IsInOrder, ref runningPosition);
                    child = child.NextSibling;
                }
            }
#if false
            if (!outOfOrder && node.IsInOrder && !node.Parent.IsNull)
            {
                this.DumpEndMarkup(dumpWriter, node, level, ref runningPosition);
            }
#endif
        }
#if false
        private void DumpStartMarkup(TextWriter dumpWriter, FormatNode node, int level, ref uint runningPosition)
        {
            uint start = node.BeginTextPosition;
            uint end = start;

            FormatNode firstChild = node.FirstChild;
            while (!firstChild.IsNull && !firstChild.IsInOrder)
            {
                firstChild = firstChild.NextSibling;
            }

            if (!firstChild.IsNull)
            {
                end = firstChild.BeginTextPosition;
            }
            else
            {
                end = node.EndTextPosition;
            }

            InternalDebug.Assert(start == runningPosition);

            if (start != end)
            {
                InternalDebug.Assert(start < end);
                this.DumpTextRuns(start, end, dumpWriter, level);
                runningPosition = end;
            }
        }

        private void DumpEndMarkup(TextWriter dumpWriter, FormatNode node, int level, ref uint runningPosition)
        {
            uint start = node.EndTextPosition;
            uint end = start;

            FormatNode nextSibling = node.NextSibling;
            while (!nextSibling.IsNull && !nextSibling.IsInOrder)
            {
                nextSibling = nextSibling.NextSibling;
            }

            if (!nextSibling.IsNull)
            {
                end = nextSibling.BeginTextPosition;
            }
            else
            {
                end = node.Parent.EndTextPosition;
            }

            InternalDebug.Assert(start <= runningPosition);

            if (start < end)
            {
                if (end >= runningPosition)
                {
                    if (start < runningPosition)
                    {
                        DumpIndent(dumpWriter, level + 1);
                        dumpWriter.WriteLine("| OVERLAPPED JUMP FORWARD FROM {0:X} TO {1:X}", start, runningPosition);
                        dumpWriter.Flush();

                        start = runningPosition;
                        InternalDebug.Assert(start <= end);
                    }

                    if (start != end)
                    {
                        this.DumpTextRuns(start, end, dumpWriter, level);
                        runningPosition = end;
                    }
                }
            }
            else if (start != end)
            {
                DumpIndent(dumpWriter, level + 1);
                dumpWriter.WriteLine("| OVERLAPPED STEP BACK FROM {0:X} TO {1:X}", start, end);
            }
        }
#endif
        private void DumpTextRuns(uint start, uint end, TextWriter dumpWriter, int level)
        {
            char[] buffer;
            int offset;
            int count;

            DumpIndent(dumpWriter, level + 1);
            dumpWriter.Write("| ");

            uint saveStart = start;
            bool newLine = false;

            if (start != end)
            {
                TextRun run = this.GetTextRun(start);

                do
                {
                    if (run.Position != saveStart && !newLine)
                    {
                        dumpWriter.Write(", ");
                    }
                    else if (newLine)
                    {
                        dumpWriter.WriteLine();
                        DumpIndent(dumpWriter, level + 1);
                        dumpWriter.Write("|           ");
                        newLine = false;
                    }

                    switch (run.Type)
                    {
                        case TextRunType.NewLine:

                            dumpWriter.Write("nl({0})", run.EffectiveLength);
                            newLine = true;
                            break;

                        case TextRunType.Tabulation:

                            dumpWriter.Write("tab({0})", run.EffectiveLength);
                            break;

                        case TextRunType.Space:

                            dumpWriter.Write("sp({0})", run.EffectiveLength);
                            break;

                        case TextRunType.NbSp:

                            dumpWriter.Write("nbsp({0})", run.EffectiveLength);
                            break;
                                
                        case TextRunType.NonSpace:

                            int pos = 0;

                            dumpWriter.Write("\"");

                            while (pos != run.EffectiveLength)
                            {
                                run.GetChunk(pos, out buffer, out offset, out count);
                                dumpWriter.Write(buffer, offset, count);
                                pos += count;
                            }

                            dumpWriter.Write("\"");
                            break;
                    }

                    run.MoveNext();
                }
                while (run.Position < end);
            }

            dumpWriter.WriteLine();
        }

#if PRIVATEBUILD
        internal long bytesTotal;
#endif
        public long CalculateAndDumpStatistics(TextWriter dumpWriter)
        {
#if !PRIVATEBUILD
            long bytesTotal;
#endif
            bytesTotal = 0;

            bytesTotal += this.text.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += this.nodes.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += this.styles.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += this.strings.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += this.multiValues.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
                dumpWriter.WriteLine("Total bytes for store: {0}", bytesTotal);
            }

            return bytesTotal;
        }
#endif
        

        [Flags]
        internal enum NodeFlags : byte
        {
            OnRightEdge = 0x01,         
            OnLeftEdge = 0x02,          
            CanFlush = 0x04,            
            OutOfOrder = 0x08,          
            Visited = 0x10,             
                                        
        }

        [StructLayout(LayoutKind.Sequential, Pack=4)]
        internal struct NodeEntry
        {
            
            

            internal FormatContainerType type;
            internal NodeFlags nodeFlags;
            internal TextMapping textMapping;

            internal int parent;
            internal int lastChild;              
            internal int nextSibling;            

            internal uint beginTextPosition;
            internal uint endTextPosition;

            internal int inheritanceMaskIndex;
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           
            internal Property[] properties;                  

            
            internal int NextFree
            {
                get { return this.nextSibling; }
                set { this.nextSibling = value; }
            }

            public void Clean()
            {
                this = new NodeEntry();
            }

            public override string ToString()
            {
                return this.type.ToString() + " (" + this.parent.ToString("X") + ", " + this.lastChild.ToString("X") + ", " + this.nextSibling.ToString("X") + ") " + this.beginTextPosition.ToString("X") + " - " + this.endTextPosition.ToString("X");
            }
        }

        

        internal struct StyleEntry
        {
            internal int refCount;
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           
            internal Property[] propertyList;                

            public StyleEntry(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propertyList)
            {
                this.refCount = Int32.MaxValue;     
                this.flagProperties = flagProperties;
                this.propertyMask = propertyMask;
                this.propertyList = propertyList;
            }

            
            internal int NextFree
            {
                get { return this.flagProperties.IntegerBag; }
                set { this.flagProperties.IntegerBag = value; }
            }

            public void Clean()
            {
                this = new StyleEntry();
            }
        }

        

        internal struct MultiValueEntry
        {
            internal int refCount;
            internal int nextFree;                           
            internal PropertyValue[] values;                

            public MultiValueEntry(PropertyValue[] values)
            {
                this.refCount = Int32.MaxValue;     
                this.values = values;
                this.nextFree = 0;
            }

            
            
            internal int NextFree
            {
                get { return this.nextFree; }
                set { this.nextFree = value; }
            }

            public void Clean()
            {
                this = new MultiValueEntry();
            }
        }

        

        internal struct StringValueEntry
        {
            internal int refCount;
            internal int nextFree;                           
            internal string str;                            

            public StringValueEntry(string str)
            {
                this.refCount = Int32.MaxValue;     
                this.str = str;
                this.nextFree = 0;
            }
     
            
            
            internal int NextFree
            {
                get { return this.nextFree; }
                set { this.nextFree = value; }
            }

            public void Clean()
            {
                this = new StringValueEntry();
            }
        }

        internal struct InheritaceMask
        {
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           

            public InheritaceMask(FlagProperties flagProperties, PropertyBitMask propertyMask)
            {
                this.flagProperties = flagProperties;
                this.propertyMask = propertyMask;
            }
        }

        

        internal class NodeStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 1024;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 32;

            
            internal const int InitialElements = 16;

            private FormatStore store;
            private NodeEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countPropLists;
            internal long totalPropListsLength;
            internal long averagePropListLength;
            internal long maxPropListLength;
            internal long bytesTotal;
#endif
            public NodeStore(FormatStore store)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(NodeEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");

                this.store = store;
                this.planes = new NodeEntry[InitialPlanes][];       
                this.planes[0] = new NodeEntry[InitialElements];
                this.freeListHead = 0;
                this.top = 0;
            }

            public NodeEntry[] Plane(int handle) { return this.planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize()
            {
                

                this.freeListHead = -1;

                this.top = 1;          

                
                this.planes[0][1].type = FormatContainerType.Root;
                this.planes[0][1].nodeFlags = NodeFlags.OnRightEdge | NodeFlags.CanFlush;
                this.planes[0][1].textMapping = TextMapping.Unicode;
                this.planes[0][1].parent = 0;
                this.planes[0][1].nextSibling = 1;
                this.planes[0][1].lastChild = 0;
                this.planes[0][1].beginTextPosition = 0;
                this.planes[0][1].endTextPosition = uint.MaxValue;
                this.planes[0][1].flagProperties = new FlagProperties();
                this.planes[0][1].propertyMask = new PropertyBitMask();
                this.planes[0][1].properties = null;

                this.top ++;
            }

            public int Allocate(FormatContainerType type, uint currentTextPosition)
            {
                InternalDebug.Assert(this.planes != null);

                NodeEntry[] plane;
                int index;

                int handle = this.freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = this.planes[handle / MaxElementsPerPlane];

                    this.freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(this.top <= this.planes.Length * MaxElementsPerPlane);

                    handle = this.top++;

                    index = handle % MaxElementsPerPlane;
                    int planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == this.planes.Length)
                        {
                            

                            int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                            NodeEntry[][] newPlanes = new NodeEntry[newLength][];
                            Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                            this.planes = newPlanes;
                        }

                        if (this.planes[planeIndex] == null)
                        {
                            

                            this.planes[planeIndex] = new NodeEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == this.planes[planeIndex].Length)
                    {
                        

                        int newLength = Math.Min(this.planes[0].Length * 2, MaxElementsPerPlane);
                        NodeEntry[] newPlane = new NodeEntry[newLength];
                        Array.Copy(this.planes[0], 0, newPlane, 0, this.planes[0].Length);
                        this.planes[0] = newPlane;
                    }

                    plane = this.planes[planeIndex];
                }

                
                plane[index].type = type;
                plane[index].nodeFlags = 0;
                plane[index].textMapping = TextMapping.Unicode;
                plane[index].parent = 0;
                plane[index].lastChild = 0;
                plane[index].nextSibling = handle;
                plane[index].beginTextPosition = currentTextPosition;
                plane[index].endTextPosition = uint.MaxValue;
                plane[index].flagProperties.ClearAll();
                plane[index].propertyMask.ClearAll();
                plane[index].properties = null;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(this.planes != null &&
                    handle / MaxElementsPerPlane < this.planes.Length &&
                    this.planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < this.planes[handle / MaxElementsPerPlane].Length);

                int index = handle % MaxElementsPerPlane;
                NodeEntry[] plane = this.planes[handle / MaxElementsPerPlane];

                

                if (null != plane[index].properties)
                {
                    for (int i = 0; i < plane[index].properties.Length; i++)
                    {
                        if (plane[index].properties[i].Value.IsRefCountedHandle)
                        {
                            this.store.ReleaseValue(plane[index].properties[i].Value);
                        }
                    }
                }

                plane[index].NextFree = this.freeListHead;
                this.freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countPropLists;
                long totalPropListsLength;
                long averagePropListLength;
                long maxPropListLength;
                long bytesTotal;
#endif
                int numPlanes = (this.top < MaxElementsPerPlane) ? 1 : ((this.top % MaxElementsPerPlane == 0) ? (this.top / MaxElementsPerPlane) : (this.top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? this.planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + this.planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(NodeEntry));
                countUsed = 0;
                countPropLists = 0;
                totalPropListsLength = 0;
                maxPropListLength = 0;

                NodeEntry[] plane;
                int index;

                for (int i = 0; i < this.top; i++)
                {
                    InternalDebug.Assert(this.planes != null &&
                        i / MaxElementsPerPlane < this.planes.Length &&
                        this.planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < this.planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = this.planes[i / MaxElementsPerPlane];

                    if (plane[index].type != FormatContainerType.Null)
                    {
                        countUsed ++;

                        if (plane[index].properties != null)
                        {
                            bytesTotal += 12 + plane[index].properties.Length * Marshal.SizeOf(typeof(Property));

                            countPropLists ++;
                            totalPropListsLength += plane[index].properties.Length;
                            if (plane[index].properties.Length > maxPropListLength)
                            {
                                maxPropListLength = plane[index].properties.Length;
                            }
                        }
                    }
                }

                averagePropListLength = countPropLists == 0 ? 0 : (totalPropListsLength + countPropLists - 1) / countPropLists;

                #if DEBUG
                int countFree = 0;
                int next = this.freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = this.planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == this.top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Nodes alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("Nodes used: {0}", countUsed);
                    dumpWriter.WriteLine("Nodes proplists: {0}", countPropLists);
                    if (countPropLists != 0)
                    {
                        dumpWriter.WriteLine("Nodes props: {0}", totalPropListsLength);
                        dumpWriter.WriteLine("Nodes average proplist: {0}", averagePropListLength);
                        dumpWriter.WriteLine("Nodes max proplist: {0}", maxPropListLength);
                    }
                    dumpWriter.WriteLine("Nodes bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class StyleStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 2048;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 32;

            private FormatStore store;
            private StyleEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countPropLists;
            internal long totalPropListsLength;
            internal long averagePropListLength;
            internal long maxPropListLength;
            internal long bytesTotal;
#endif
            public StyleStore(FormatStore store, StyleEntry[] globalStyles)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(StyleEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globalStyles.Length < MaxElementsPerPlane);

                this.store = store;
                this.planes = new StyleEntry[InitialPlanes][];       
                this.planes[0] = new StyleEntry[Math.Max(InitialElements, globalStyles.Length + 1)];
                this.freeListHead = 0;
                this.top = 0;

                if (globalStyles != null && globalStyles.Length != 0)
                {
                    Array.Copy(globalStyles, 0, this.planes[0], 0, globalStyles.Length);
                }
            }

            public StyleEntry[] Plane(int handle) { return this.planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(StyleEntry[] globalStyles)
            {
                

                this.freeListHead = -1;

                if (globalStyles != null && globalStyles.Length != 0)
                {
                    this.top = globalStyles.Length;
                }
                else
                {
                    this.top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(this.planes != null);

                StyleEntry[] plane;
                int index;

                int handle = this.freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = this.planes[handle / MaxElementsPerPlane];

                    this.freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(this.top <= this.planes.Length * MaxElementsPerPlane);

                    handle = this.top++;

                    index = handle % MaxElementsPerPlane;
                    int planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == this.planes.Length)
                        {
                            

                            int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                            StyleEntry[][] newPlanes = new StyleEntry[newLength][];
                            Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                            this.planes = newPlanes;
                        }

                        if (this.planes[planeIndex] == null)
                        {
                            

                            this.planes[planeIndex] = new StyleEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == this.planes[planeIndex].Length)
                    {
                        

                        int newLength = Math.Min(this.planes[0].Length * 2, MaxElementsPerPlane);
                        StyleEntry[] newPlane = new StyleEntry[newLength];
                        Array.Copy(this.planes[0], 0, newPlane, 0, this.planes[0].Length);
                        this.planes[0] = newPlane;
                    }

                    plane = this.planes[planeIndex];
                }

                
                plane[index].propertyList = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].flagProperties.ClearAll();
                plane[index].propertyMask.ClearAll();

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(this.planes != null &&
                    handle / MaxElementsPerPlane < this.planes.Length &&
                    this.planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < this.planes[handle / MaxElementsPerPlane].Length);

                int index = handle % MaxElementsPerPlane;
                StyleEntry[] plane = this.planes[handle / MaxElementsPerPlane];

                if (null != plane[index].propertyList)
                {
                    for (int i = 0; i < plane[index].propertyList.Length; i++)
                    {
                        if (plane[index].propertyList[i].Value.IsRefCountedHandle)
                        {
                            this.store.ReleaseValue(plane[index].propertyList[i].Value);
                        }
                    }
                }

                plane[index].NextFree = this.freeListHead;
                this.freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countPropLists;
                long totalPropListsLength;
                long averagePropListLength;
                long maxPropListLength;
                long bytesTotal;
#endif
                int numPlanes = (this.top < MaxElementsPerPlane) ? 1 : ((this.top % MaxElementsPerPlane == 0) ? (this.top / MaxElementsPerPlane) : (this.top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? this.planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + this.planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(StyleEntry));
                countUsed = 0;
                countPropLists = 0;
                totalPropListsLength = 0;
                maxPropListLength = 0;

                StyleEntry[] plane;
                int index;

                for (int i = 0; i < this.top; i++)
                {
                    InternalDebug.Assert(this.planes != null &&
                        i / MaxElementsPerPlane < this.planes.Length &&
                        this.planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < this.planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = this.planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].propertyList != null)
                        {
                            bytesTotal += 12 + plane[index].propertyList.Length * Marshal.SizeOf(typeof(Property));

                            countPropLists ++;
                            totalPropListsLength += plane[index].propertyList.Length;
                            if (plane[index].propertyList.Length > maxPropListLength)
                            {
                                maxPropListLength = plane[index].propertyList.Length;
                            }
                        }
                    }
                }

                averagePropListLength = countPropLists == 0 ? 0 : (totalPropListsLength + countPropLists - 1) / countPropLists;

                #if DEBUG
                int countFree = 0;
                int next = this.freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = this.planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == this.top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Styles alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("Styles used: {0}", countUsed);
                    dumpWriter.WriteLine("Styles non-null prop lists: {0}", countPropLists);
                    if (countPropLists != 0)
                    {
                        dumpWriter.WriteLine("Styles total prop lists length: {0}", totalPropListsLength);
                        dumpWriter.WriteLine("Styles average prop list length: {0}", averagePropListLength);
                        dumpWriter.WriteLine("Styles max prop list length: {0}", maxPropListLength);
                    }
                    dumpWriter.WriteLine("Styles bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class StringValueStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 4096;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 16;

            private StringValueEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countNonNullStrings;
            internal long totalStringsLength;
            internal long averageStringLength;
            internal long maxStringLength;
            internal long bytesTotal;
#endif
            public StringValueStore(StringValueEntry[] globalStrings)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(StringValueEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globalStrings.Length < MaxElementsPerPlane);

                this.planes = new StringValueEntry[InitialPlanes][];       
                this.planes[0] = new StringValueEntry[Math.Max(InitialElements, globalStrings.Length + 1)];
                this.freeListHead = 0;
                this.top = 0;

                if (globalStrings != null && globalStrings.Length != 0)
                {
                    Array.Copy(globalStrings, 0, this.planes[0], 0, globalStrings.Length);
                }
            }

            public StringValueEntry[] Plane(int handle) { return this.planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(StringValueEntry[] globalStrings)
            {
                

                this.freeListHead = -1;

                if (globalStrings != null && globalStrings.Length != 0)
                {
                    this.top = globalStrings.Length;
                }
                else
                {
                    this.top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(this.planes != null);

                StringValueEntry[] plane;
                int index;

                int handle = this.freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = this.planes[handle / MaxElementsPerPlane];

                    this.freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(this.top <= this.planes.Length * MaxElementsPerPlane);

                    handle = this.top++;

                    index = handle % MaxElementsPerPlane;
                    int planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == this.planes.Length)
                        {
                            

                            int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                            StringValueEntry[][] newPlanes = new StringValueEntry[newLength][];
                            Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                            this.planes = newPlanes;
                        }

                        if (this.planes[planeIndex] == null)
                        {
                            

                            this.planes[planeIndex] = new StringValueEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == this.planes[planeIndex].Length)
                    {
                        

                        int newLength = Math.Min(this.planes[0].Length * 2, MaxElementsPerPlane);
                        StringValueEntry[] newPlane = new StringValueEntry[newLength];
                        Array.Copy(this.planes[0], 0, newPlane, 0, this.planes[0].Length);
                        this.planes[0] = newPlane;
                    }

                    plane = this.planes[planeIndex];
                }

                
                plane[index].str = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].nextFree = -1;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(this.planes != null &&
                    handle / MaxElementsPerPlane < this.planes.Length &&
                    this.planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < this.planes[handle / MaxElementsPerPlane].Length);

                int index = handle % MaxElementsPerPlane;
                StringValueEntry[] plane = this.planes[handle / MaxElementsPerPlane];

                plane[index].NextFree = this.freeListHead;
                this.freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countNonNullStrings;
                long totalStringsLength;
                long averageStringLength;
                long maxStringLength;
                long bytesTotal;
#endif
                int numPlanes = (this.top < MaxElementsPerPlane) ? 1 : ((this.top % MaxElementsPerPlane == 0) ? (this.top / MaxElementsPerPlane) : (this.top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? this.planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + this.planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(StringValueEntry));
                countUsed = 0;
                countNonNullStrings = 0;
                totalStringsLength = 0;
                maxStringLength = 0;

                StringValueEntry[] plane;
                int index;

                for (int i = 0; i < this.top; i++)
                {
                    InternalDebug.Assert(this.planes != null &&
                        i / MaxElementsPerPlane < this.planes.Length &&
                        this.planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < this.planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = this.planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].str != null)
                        {
                            bytesTotal += 12 + plane[index].str.Length * 2;    

                            countNonNullStrings ++;
                            totalStringsLength += plane[index].str.Length;
                            if (plane[index].str.Length > maxStringLength)
                            {
                                maxStringLength = plane[index].str.Length;
                            }
                        }
                    }
                }

                averageStringLength = countNonNullStrings == 0 ? 0 : (totalStringsLength + countNonNullStrings - 1) / countNonNullStrings;

                #if DEBUG
                int countFree = 0;
                int next = this.freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = this.planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == this.top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("StringValues alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("StringValues used: {0}", countUsed);
                    dumpWriter.WriteLine("StringValues non-null strings: {0}", countNonNullStrings);
                    if (countNonNullStrings != 0)
                    {
                        dumpWriter.WriteLine("StringValues total string length: {0}", totalStringsLength);
                        dumpWriter.WriteLine("StringValues average string length: {0}", averageStringLength);
                        dumpWriter.WriteLine("StringValues max string length: {0}", maxStringLength);
                    }
                    dumpWriter.WriteLine("StringValues bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class MultiValueStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 4096;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 16;

            private FormatStore store;
            private MultiValueEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countNonNullValueLists;
            internal long totalValueListsLength;
            internal long averageValueListLength;
            internal long maxValueListLength;
            internal long bytesTotal;
#endif
            public MultiValueStore(FormatStore store, MultiValueEntry[] globaMultiValues)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(MultiValueEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globaMultiValues.Length < MaxElementsPerPlane);

                this.store = store;
                this.planes = new MultiValueEntry[InitialPlanes][];       
                this.planes[0] = new MultiValueEntry[Math.Max(InitialElements, globaMultiValues.Length + 1)];
                this.freeListHead = 0;
                this.top = 0;

                if (globaMultiValues != null && globaMultiValues.Length != 0)
                {
                    Array.Copy(globaMultiValues, 0, this.planes[0], 0, globaMultiValues.Length);
                }
            }

            public FormatStore Store { get { return this.store; } }

            public MultiValueEntry[] Plane(int handle) { return this.planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(MultiValueEntry[] globaMultiValues)
            {
                

                this.freeListHead = -1;

                if (globaMultiValues != null && globaMultiValues.Length != 0)
                {
                    this.top = globaMultiValues.Length;
                }
                else
                {
                    this.top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(this.planes != null);

                MultiValueEntry[] plane;
                int index;

                int handle = this.freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = this.planes[handle / MaxElementsPerPlane];

                    this.freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(this.top <= this.planes.Length * MaxElementsPerPlane);

                    handle = this.top++;

                    index = handle % MaxElementsPerPlane;
                    int planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == this.planes.Length)
                        {
                            

                            int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                            MultiValueEntry[][] newPlanes = new MultiValueEntry[newLength][];
                            Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                            this.planes = newPlanes;
                        }

                        if (this.planes[planeIndex] == null)
                        {
                            

                            this.planes[planeIndex] = new MultiValueEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == this.planes[planeIndex].Length)
                    {
                        

                        int newLength = Math.Min(this.planes[0].Length * 2, MaxElementsPerPlane);
                        MultiValueEntry[] newPlane = new MultiValueEntry[newLength];
                        Array.Copy(this.planes[0], 0, newPlane, 0, this.planes[0].Length);
                        this.planes[0] = newPlane;
                    }

                    plane = this.planes[planeIndex];
                }

                
                plane[index].values = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].nextFree = -1;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(this.planes != null &&
                    handle / MaxElementsPerPlane < this.planes.Length &&
                    this.planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < this.planes[handle / MaxElementsPerPlane].Length);

                int index = handle % MaxElementsPerPlane;
                MultiValueEntry[] plane = this.planes[handle / MaxElementsPerPlane];

                if (null != plane[index].values)
                {
                    for (int i = 0; i < plane[index].values.Length; i++)
                    {
                        if (plane[index].values[i].IsRefCountedHandle)
                        {
                            this.store.ReleaseValue(plane[index].values[i]);
                        }
                    }
                }

                plane[index].NextFree = this.freeListHead;
                this.freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countNonNullValueLists;
                long totalValueListsLength;
                long averageValueListLength;
                long maxValueListLength;
                long bytesTotal;
#endif
                int numPlanes = (this.top < MaxElementsPerPlane) ? 1 : ((this.top % MaxElementsPerPlane == 0) ? (this.top / MaxElementsPerPlane) : (this.top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? this.planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + this.planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(MultiValueEntry));
                countUsed = 0;
                countNonNullValueLists = 0;
                totalValueListsLength = 0;
                maxValueListLength = 0;

                MultiValueEntry[] plane;
                int index;

                for (int i = 0; i < this.top; i++)
                {
                    InternalDebug.Assert(this.planes != null &&
                        i / MaxElementsPerPlane < this.planes.Length &&
                        this.planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < this.planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = this.planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].values != null)
                        {
                            bytesTotal += 12 + plane[index].values.Length * Marshal.SizeOf(typeof(PropertyValue));

                            countNonNullValueLists ++;
                            totalValueListsLength += plane[index].values.Length;
                            if (plane[index].values.Length > maxValueListLength)
                            {
                                maxValueListLength = plane[index].values.Length;
                            }
                        }
                    }
                }

                averageValueListLength = countNonNullValueLists == 0 ? 0 : (totalValueListsLength + countNonNullValueLists - 1) / countNonNullValueLists;

                #if DEBUG
                int countFree = 0;
                int next = this.freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = this.planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == this.top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("MultiValues alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("MultiValues used: {0}", countUsed);
                    dumpWriter.WriteLine("MultiValues non-null value lists: {0}", countNonNullValueLists);
                    if (countNonNullValueLists != 0)
                    {
                        dumpWriter.WriteLine("MultiValues total value lists length: {0}", totalValueListsLength);
                        dumpWriter.WriteLine("MultiValues average value list length: {0}", averageValueListLength);
                        dumpWriter.WriteLine("MultiValues max value list length: {0}", maxValueListLength);
                    }
                    dumpWriter.WriteLine("MultiValues bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class TextStore
        {
            
            
            internal const int LogMaxCharactersPerPlane = 15;

            
            
            

            internal const int MaxCharactersPerPlane = 1 << LogMaxCharactersPerPlane;

            
            
            internal const int MaxPlanes = (20 * 1024 * 1024) / MaxCharactersPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialCharacters = 2048;

            internal const int MaxRunEffectivelength = 0x1FFF;

            private OutboundCodePageDetector detector;

            private char[][] planes;
            private uint position;

            private TextRunType lastRunType;
            private uint lastRunPosition;

#if PRIVATEBUILD
            internal long charactersAllocated;
            internal long charactersUsed;
            internal long bytesTotal;
#endif
            public TextStore()
            {
                InternalDebug.Assert(InitialCharacters <= MaxCharactersPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(char)) * MaxCharactersPerPlane < 85000, "exceeding LOH threshold!");

                this.planes = new char[InitialPlanes][];       
                this.planes[0] = new char[InitialCharacters];
            }

            public char[] Plane(uint position) { return this.planes[position >> LogMaxCharactersPerPlane]; }
            public int Index(uint position) { return (int)(position & (MaxCharactersPerPlane - 1)); }
            public char Pick(uint position) { return this.planes[position >> LogMaxCharactersPerPlane][position & (MaxCharactersPerPlane - 1)]; }

            public uint CurrentPosition { get { return this.position; } }

            public TextRunType LastRunType { get { return this.lastRunType; } }

            public void Initialize()
            {
                this.position = 0;
                this.lastRunType = TextRunType.Invalid;
                this.lastRunPosition = 0;

                if (this.detector != null)
                {
                    this.detector.Reset();
                }
            }

            public void InitializeCodepageDetector()
            {
                if (this.detector == null)
                {
                    this.detector = new OutboundCodePageDetector();
                }
            }

            public int GetBestWindowsCodePage()
            {
                InternalDebug.Assert(this.detector != null);
                return this.detector.GetBestWindowsCodePage();
            }

            public int GetBestWindowsCodePage(int preferredCodePage)
            {
                InternalDebug.Assert(this.detector != null);
                return this.detector.GetBestWindowsCodePage(preferredCodePage);
            }

            

            public void AddText(TextRunType runType, char[] textBuffer, int offset, int count)
            {
                InternalDebug.Assert(count > 0);
                InternalDebug.Assert(runType == TextRunType.NonSpace);

                if (this.detector != null)
                {
                    this.detector.AddText(textBuffer, offset, count);
                }

                int index = (int)(this.position & (MaxCharactersPerPlane - 1));
                int planeIndex = (int)(this.position >> LogMaxCharactersPerPlane);

                if (this.lastRunType == runType && index != 0)
                {
                    
                    

                    InternalDebug.Assert((this.lastRunPosition >> LogMaxCharactersPerPlane) == planeIndex);

                    char[] lastRunPlane = this.planes[planeIndex];
                    int lastRunIndex = (int)(this.lastRunPosition & (MaxCharactersPerPlane - 1));

                    InternalDebug.Assert(TypeFromRunHeader(lastRunPlane[lastRunIndex]) == runType);
                    InternalDebug.Assert(lastRunIndex + 1 + LengthFromRunHeader(lastRunPlane[lastRunIndex]) == index);

                    int appendLength = Math.Min(Math.Min(count, MaxRunEffectivelength - LengthFromRunHeader(lastRunPlane[lastRunIndex])), MaxCharactersPerPlane - index);

                    InternalDebug.Assert(appendLength >= 0);

                    if (appendLength != 0)
                    {
                        if (planeIndex == 0 && index + appendLength > lastRunPlane.Length)
                        {
                            

                            int newLength = Math.Min(Math.Max(this.planes[0].Length * 2, index + appendLength), MaxCharactersPerPlane);
                            char[] newPlane = new char[newLength];
                            Buffer.BlockCopy(this.planes[0], 0, newPlane, 0, (int)this.position * 2);
                            this.planes[0] = lastRunPlane = newPlane;
                        }

                        InternalDebug.Assert(index + appendLength <= lastRunPlane.Length);

                        lastRunPlane[lastRunIndex] = MakeTextRunHeader(runType, (int)(appendLength + LengthFromRunHeader(lastRunPlane[lastRunIndex])));
                        Buffer.BlockCopy(textBuffer, offset * 2, lastRunPlane, (int)index * 2, (int)appendLength * 2);

                        offset += appendLength;
                        count -= appendLength;

                        this.position += (uint)(appendLength);
                    }
                }

                InternalDebug.Assert(index <= MaxCharactersPerPlane);

                while (count != 0)
                {
                    index = (int)(this.position & (MaxCharactersPerPlane - 1));
                    planeIndex = (int)(this.position >> LogMaxCharactersPerPlane);

                    if (MaxCharactersPerPlane - index < 20 + 1)
                    {
                        

                        InternalDebug.Assert(this.planes[planeIndex].Length == MaxCharactersPerPlane);

                        
                        
                        

                        

                        this.planes[planeIndex][index] = MakeTextRunHeader(TextRunType.Invalid, (int)(MaxCharactersPerPlane - index - 1));
                        this.position += (uint)(MaxCharactersPerPlane - index);
                    }
                    else
                    {
                        int newRunLength = Math.Min(Math.Min(count, MaxRunEffectivelength), MaxCharactersPerPlane - index - 1);

                        if (planeIndex == 0 && index + newRunLength + 1 > this.planes[0].Length)
                        {
                            

                            InternalDebug.Assert(this.planes[planeIndex].Length < MaxCharactersPerPlane);

                            int newLength = Math.Min(Math.Max(this.planes[0].Length * 2, index + newRunLength + 1), MaxCharactersPerPlane);
                            char[] newPlane = new char[newLength];
                            Buffer.BlockCopy(this.planes[0], 0, newPlane, 0, (int)(this.position * 2));
                            this.planes[0] = newPlane;

                            InternalDebug.Assert(this.position + 1 + newRunLength <= this.planes[0].Length);
                        }
                        else if (index == 0)
                        {
                            if (planeIndex == this.planes.Length)
                            {
                                if (planeIndex == MaxPlanes)
                                {
                                    
                                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                                }

                                

                                int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                                char[][] newPlanes = new char[newLength][];
                                Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                                this.planes = newPlanes;
                            }

                            if (this.planes[planeIndex] == null)
                            {
                                
                                this.planes[planeIndex] = new char[MaxCharactersPerPlane];
                            }
                        }

                        this.lastRunType = runType;
                        this.lastRunPosition = this.position;

                        this.planes[planeIndex][index] = MakeTextRunHeader(runType, newRunLength);
                        Buffer.BlockCopy(textBuffer, offset * 2, this.planes[planeIndex], (index + 1) * 2, newRunLength * 2);

                        offset += newRunLength;
                        count -= newRunLength;

                        this.position += (uint)(newRunLength + 1);
                    }
                }
            }

            

            public void AddSimpleRun(TextRunType runType, int count)
            {
                InternalDebug.Assert(count <= MaxRunEffectivelength);

                if (this.lastRunType == runType)
                {
                    
                    

                    InternalDebug.Assert(this.lastRunPosition + 1 == this.position);

                    char[] lastRunPlane = this.planes[this.lastRunPosition >> LogMaxCharactersPerPlane];
                    int lastRunIndex = (int)(this.lastRunPosition & (MaxCharactersPerPlane - 1));

                    InternalDebug.Assert(TypeFromRunHeader(lastRunPlane[lastRunIndex]) == runType);
                    InternalDebug.Assert(((lastRunIndex + 1 - this.position) & (MaxCharactersPerPlane - 1)) == 0);

                    int appendLength = Math.Min(count, MaxRunEffectivelength - LengthFromRunHeader(lastRunPlane[lastRunIndex]));

                    InternalDebug.Assert(appendLength >= 0);

                    if (appendLength != 0)
                    {
                        lastRunPlane[lastRunIndex] = MakeTextRunHeader(runType, appendLength + LengthFromRunHeader(lastRunPlane[lastRunIndex]));

                        count -= appendLength;
                    }
                }

                if (count != 0)
                {
                    

                    int index = (int)(this.position & (MaxCharactersPerPlane - 1));
                    int planeIndex = (int)(this.position >> LogMaxCharactersPerPlane);

                    if (index == 0)
                    {
                        

                        if (planeIndex == this.planes.Length)
                        {
                            if (planeIndex == MaxPlanes)
                            {
                                
                                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.InputDocumentTooComplex);
                            }

                            

                            int newLength = Math.Min(this.planes.Length * 2, MaxPlanes);
                            char[][] newPlanes = new char[newLength][];
                            Array.Copy(this.planes, 0, newPlanes, 0, this.planes.Length);
                            this.planes = newPlanes;
                        }

                        if (this.planes[planeIndex] == null)
                        {
                            
                            this.planes[planeIndex] = new char[MaxCharactersPerPlane];
                        }
                    }
                    else
                    {
                        if (planeIndex == 0 && this.position + 1 > this.planes[0].Length)
                        {
                            

                            int newLength = Math.Min(this.planes[0].Length * 2, MaxCharactersPerPlane);
                            char[] newPlane = new char[newLength];
                            Buffer.BlockCopy(this.planes[0], 0, newPlane, 0, (int)(this.position * 2));
                            this.planes[0] = newPlane;

                            InternalDebug.Assert(this.position + 1 <= this.planes[0].Length);
                        }
                    }

                    this.lastRunType = runType;
                    this.lastRunPosition = this.position;

                    this.planes[planeIndex][index] = MakeTextRunHeader(runType, count);

                    this.position ++;
                }
            }

            public void ConvertToInvalid(uint startPosition)
            {
                InternalDebug.Assert(startPosition < this.position);

                char[] buffer = this.Plane(startPosition);
                int index = this.Index(startPosition);

                int runLength = buffer[index] >= (char)TextRunType.FirstShort ? 1 : FormatStore.TextStore.LengthFromRunHeader(buffer[index]) + 1;

                buffer[index] = MakeTextRunHeader(TextRunType.Invalid, runLength - 1);
            }

            public void ConvertToInvalid(uint startPosition, int countToConvert)
            {
                InternalDebug.Assert(startPosition < this.position && countToConvert > 0);

                char[] buffer = this.Plane(startPosition);
                int index = this.Index(startPosition);

                InternalDebug.Assert(FormatStore.TextStore.TypeFromRunHeader(buffer[index]) == TextRunType.NonSpace);

                int runEffectiveLength = FormatStore.TextStore.LengthFromRunHeader(buffer[index]);

                InternalDebug.Assert(countToConvert < runEffectiveLength);

                int remainingLength = runEffectiveLength - countToConvert;
                int headLength = runEffectiveLength + 1 - (remainingLength + 1);

                InternalDebug.Assert(headLength > 0);

                buffer[index] = MakeTextRunHeader(TextRunType.Invalid, headLength - 1);
                buffer[index + headLength] = MakeTextRunHeader(TextRunType.NonSpace, remainingLength);
            }

            public void ConvertShortRun(uint startPosition, TextRunType type, int newEffectiveLength)
            {
                InternalDebug.Assert(startPosition < this.position && newEffectiveLength > 0);

                char[] buffer = this.Plane(startPosition);
                int index = this.Index(startPosition);

                InternalDebug.Assert(FormatStore.TextStore.TypeFromRunHeader(buffer[index]) >= TextRunType.FirstShort);

                buffer[index] = MakeTextRunHeader(type, newEffectiveLength);
            }

            

            public void DoNotMergeNextRun()
            {
                
                

                if (this.lastRunType != TextRunType.BlockBoundary)
                {
                    
                    
                    this.lastRunType = TextRunType.Invalid;
                }
            }

            

            internal char MakeTextRunHeader(TextRunType runType, int length)
            {
                InternalDebug.Assert(length <= MaxRunEffectivelength);
                return (char)((int)runType | length);
            }

            internal static TextRunType TypeFromRunHeader(char runHeader)
            {
                return (TextRunType)(runHeader & 0xE000);
            }

            internal static int LengthFromRunHeader(char runHeader)
            {
                return (int)runHeader & MaxRunEffectivelength;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long charactersAllocated;
                long charactersUsed;
                long bytesTotal;
#endif
                int numPlanes = (int)((this.position + MaxCharactersPerPlane - 1) >> LogMaxCharactersPerPlane);
                if (numPlanes == 0)
                {
                    numPlanes = 1;
                }

                charactersAllocated = (numPlanes == 1) ? this.planes[0].Length : (numPlanes * MaxCharactersPerPlane);
                charactersUsed = this.position;
                bytesTotal = 12 + this.planes.Length * 4 + numPlanes * 12 + charactersAllocated * 2;

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Text alloc: {0}", charactersAllocated);
                    dumpWriter.WriteLine("Text used: {0}", charactersUsed);
                    dumpWriter.WriteLine("Text bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
          }
    }

    

    internal struct FormatNode
    {
        internal FormatStore.NodeStore nodes;
        private int nodeHandle;

        internal FormatNode(FormatStore.NodeStore nodes, int nodeHandle)
        {
            this.nodes = nodes;
            this.nodeHandle = nodeHandle;
        }

        internal FormatNode(FormatStore store, int nodeHandle)
        {
            this.nodes = store.nodes;
            this.nodeHandle = nodeHandle;
        }

        public static readonly FormatNode Null = new FormatNode();

        public int Handle { get { return this.nodeHandle; } }

        public bool IsNull
        {
            get { return this.nodeHandle == 0; }
        }

        public bool IsInOrder
        {
            get { return 0 == (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags & FormatStore.NodeFlags.OutOfOrder); }
        }

        public bool OnRightEdge
        {
            get { return 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags & FormatStore.NodeFlags.OnRightEdge); }
        }

        public bool OnLeftEdge
        {
            get { return 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags & FormatStore.NodeFlags.OnLeftEdge); }
        }

        public bool IsVisited
        {
            get { return 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags & FormatStore.NodeFlags.Visited); }
        }

        public bool IsEmptyBlockNode
        {
            get { return 0 != (this.NodeType & FormatContainerType.BlockFlag) && this.BeginTextPosition + 1 == this.EndTextPosition; }
        }

        public bool CanFlush
        {
            get { return 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags & FormatStore.NodeFlags.CanFlush); }
        }

        public FormatContainerType NodeType
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].type; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].type = value; }
        }

        public bool IsText
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].type == FormatContainerType.Text; }
        }

        public FormatNode Parent
        {
            get
            {
                int parentHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].parent;
                return parentHandle == 0 ?
                            FormatNode.Null :
                            new FormatNode(this.nodes, parentHandle);
            }
        }

        public bool IsOnlySibling
        {
            get
            {
                return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nextSibling == this.nodeHandle;
            }
        }

        public FormatNode FirstChild
        {
            get
            {
                int lastChildHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].lastChild;
                return lastChildHandle == 0 ?
                            FormatNode.Null :
                            new FormatNode(this.nodes, this.nodes.Plane(lastChildHandle)[this.nodes.Index(lastChildHandle)].nextSibling);
            }
        }

        public FormatNode LastChild
        {
            get
            {
                int lastChildHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].lastChild;
                return lastChildHandle == 0 ?
                            FormatNode.Null :
                            new FormatNode(this.nodes, lastChildHandle);
            }
        }

        public FormatNode NextSibling
        {
            get
            {
                int parentHandle = this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].parent;
                return (parentHandle == 0 ||
                        this.nodeHandle == this.nodes.Plane(parentHandle)[this.nodes.Index(parentHandle)].lastChild) ?
                            FormatNode.Null :
                            new FormatNode(this.nodes, this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nextSibling);
            }
        }

        public FormatNode PreviousSibling
        {
            get
            {
                FormatNode leftSibling = this.Parent.FirstChild;
                if (this == leftSibling)
                {
                    
                    return FormatNode.Null;
                }

                while (leftSibling.NextSibling != this)
                {
                    leftSibling = leftSibling.NextSibling;
                }

                return leftSibling;
            }
        }

        public uint BeginTextPosition
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].beginTextPosition; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].beginTextPosition = value; }
        }

        public uint EndTextPosition
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].endTextPosition; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].endTextPosition = value; }
        }

        public int InheritanceMaskIndex
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].inheritanceMaskIndex; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].inheritanceMaskIndex = value; }
        }

        public FlagProperties FlagProperties
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].flagProperties; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].propertyMask; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].propertyMask = value; }
        }

        public Property[] Properties
        {
            get { return this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].properties; }
            set { this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].properties = value; }
        }

        public NodePropertiesEnumerator PropertiesEnumerator
        {
            get { return new NodePropertiesEnumerator(this); }
        }

        public bool IsBlockNode
        {
            get
            {
                return 0 != (this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].type & FormatContainerType.BlockFlag);
            }
        }

        public void SetOutOfOrder()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OutOfOrder;
        }

        public void SetOnLeftEdge()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OnLeftEdge;
        }

        public void ResetOnLeftEdge()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags &= ~FormatStore.NodeFlags.OnLeftEdge;
        }

        public void SetOnRightEdge()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OnRightEdge;
        }

        public void SetVisited()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags |= FormatStore.NodeFlags.Visited;
        }

        public void ResetVisited()
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].nodeFlags &= ~FormatStore.NodeFlags.Visited;
        }

        public PropertyValue GetProperty(PropertyId id)
        {
            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            if (FlagProperties.IsFlagProperty(id))
            {
                return thisPlane[thisIndex].flagProperties.GetPropertyValue(id);
            }
            else if (thisPlane[thisIndex].propertyMask.IsSet(id))
            {
                InternalDebug.Assert(thisPlane[thisIndex].properties != null);

                for (int i = 0; i < thisPlane[thisIndex].properties.Length; i++)
                {
                    Property prop = thisPlane[thisIndex].properties[i];
                    if (prop.Id == id)
                    {
                        return prop.Value;
                    }
                    else if (prop.Id > id)
                    {
                        break;
                    }
                }

                InternalDebug.Assert(false);
            }

            return PropertyValue.Null;
        }

        public void SetProperty(PropertyId id, PropertyValue value)
        {
            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            if (FlagProperties.IsFlagProperty(id))
            {
                thisPlane[thisIndex].flagProperties.SetPropertyValue(id, value);
                return;
            }

            int i = 0;

            if (thisPlane[thisIndex].properties != null)
            {
                
                for (; i < thisPlane[thisIndex].properties.Length; i++)
                {
                    Property prop = thisPlane[thisIndex].properties[i];
                    if (prop.Id == id)
                    {
                        InternalDebug.Assert(thisPlane[thisIndex].propertyMask.IsSet(id));

                        
                        thisPlane[thisIndex].properties[i].Set(id, value);
                        return;
                    }
                    else if (prop.Id > id)
                    {
                        break;
                    }
                }
            }

            if (thisPlane[thisIndex].properties == null)
            {
                InternalDebug.Assert(i == 0);

                thisPlane[thisIndex].properties = new Property[1];

                thisPlane[thisIndex].properties[0].Set(id, value);

                thisPlane[thisIndex].propertyMask.Set(id);
            }
            else
            {
                
                

                Property[] newProperties = new Property[thisPlane[thisIndex].properties.Length + 1];

                if (i != 0)
                {
                    Array.Copy(thisPlane[thisIndex].properties, 0, newProperties, 0, i);
                }

                if (i != thisPlane[thisIndex].properties.Length)
                {
                    Array.Copy(thisPlane[thisIndex].properties, i, newProperties, i + 1, thisPlane[thisIndex].properties.Length - i);
                }

                newProperties[i].Set(id, value);

                thisPlane[thisIndex].properties = newProperties;

                thisPlane[thisIndex].propertyMask.Set(id);
            }
        }

        internal static void InternalAppendChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
        {
            
            InternalPrependChild(nodes, thisNode, newChildNode);
            nodes.Plane(thisNode)[nodes.Index(thisNode)].lastChild = newChildNode;
        }

        internal static void InternalPrependChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
        {
            FormatStore.NodeEntry[] thisPlane = nodes.Plane(thisNode);
            int thisIndex = nodes.Index(thisNode);

            FormatStore.NodeEntry[] childPlane = nodes.Plane(newChildNode);
            int childIndex = nodes.Index(newChildNode);

            if (thisPlane[thisIndex].lastChild != 0)  
            {
                int oldLastChild = thisPlane[thisIndex].lastChild;

                FormatStore.NodeEntry[] oldLastChildPlane = nodes.Plane(oldLastChild);
                int oldLastChildIndex = nodes.Index(oldLastChild);

                childPlane[childIndex].nextSibling = oldLastChildPlane[oldLastChildIndex].nextSibling;
                oldLastChildPlane[oldLastChildIndex].nextSibling = newChildNode;
                childPlane[childIndex].parent = thisNode;
            }
            else
            {
                
                childPlane[childIndex].nextSibling = newChildNode;
                childPlane[childIndex].parent = thisNode;
                thisPlane[thisIndex].lastChild = newChildNode;
            }
        }


        public void AppendChild(FormatNode newChildNode)
        {
            InternalAppendChild(this.nodes, this.nodeHandle, newChildNode.Handle);
        }

        public void PrependChild(FormatNode newChildNode)
        {
            InternalPrependChild(this.nodes, this.nodeHandle, newChildNode.Handle);
        }

        public void InsertSiblingAfter(FormatNode newSiblingNode)
        {
            int newSibling = newSiblingNode.nodeHandle;

            FormatStore.NodeEntry[] newSiblingPlane = this.nodes.Plane(newSibling);
            int newSiblingIndex = this.nodes.Index(newSibling);

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            int parent = thisPlane[thisIndex].parent;

            FormatStore.NodeEntry[] parentPlane = this.nodes.Plane(parent);
            int parentIndex = this.nodes.Index(parent);

            newSiblingPlane[newSiblingIndex].parent = parent;
            newSiblingPlane[newSiblingIndex].nextSibling = thisPlane[thisIndex].nextSibling;
            thisPlane[thisIndex].nextSibling = newSibling;

            if (this.nodeHandle == parentPlane[parentIndex].lastChild)
            {
                parentPlane[parentIndex].lastChild = newSibling;
            }
        }

        public void InsertSiblingBefore(FormatNode newSiblingNode)
        {
            int newSibling = newSiblingNode.nodeHandle;

            FormatStore.NodeEntry[] newSiblingPlane = this.nodes.Plane(newSibling);
            int newSiblingIndex = this.nodes.Index(newSibling);

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            int parent = thisPlane[thisIndex].parent;

            FormatStore.NodeEntry[] parentPlane = this.nodes.Plane(parent);
            int parentIndex = this.nodes.Index(parent);

            int previousHandle = parentPlane[parentIndex].lastChild;

            FormatStore.NodeEntry[] previousPlane = this.nodes.Plane(previousHandle);
            int previousIndex = this.nodes.Index(previousHandle);

            while (previousPlane[previousIndex].nextSibling != this.nodeHandle)
            {
                previousHandle = previousPlane[previousIndex].nextSibling;
                previousPlane = this.nodes.Plane(previousHandle);
                previousIndex = this.nodes.Index(previousHandle);
            }

            newSiblingPlane[newSiblingIndex].parent = parent;
            newSiblingPlane[newSiblingIndex].nextSibling = this.nodeHandle;

            previousPlane[previousIndex].nextSibling = newSibling;
        }

        public void RemoveFromParent()
        {
            

            

            InternalDebug.Assert(this.nodeHandle != 0);

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            InternalDebug.Assert(thisPlane[thisIndex].parent != 0);

            int parent = thisPlane[thisIndex].parent;

            FormatStore.NodeEntry[] parentPlane = this.nodes.Plane(parent);
            int parentIndex = this.nodes.Index(parent);

            int nextSibling = thisPlane[thisIndex].nextSibling;

            if (this.nodeHandle == nextSibling)
            {
                
                InternalDebug.Assert(parentPlane[parentIndex].lastChild == this.nodeHandle);

                parentPlane[parentIndex].lastChild = 0;
            }
            else
            {
                int previousSibling = parentPlane[parentIndex].lastChild;

                FormatStore.NodeEntry[] previousSiblingPlane = this.nodes.Plane(previousSibling);
                int previousSiblingIndex = this.nodes.Index(previousSibling);

                while (previousSiblingPlane[previousSiblingIndex].nextSibling != this.nodeHandle)
                {
                    previousSibling = previousSiblingPlane[previousSiblingIndex].nextSibling;

                    previousSiblingPlane = this.nodes.Plane(previousSibling);
                    previousSiblingIndex = this.nodes.Index(previousSibling);
                }

                previousSiblingPlane[previousSiblingIndex].nextSibling = thisPlane[thisIndex].nextSibling;

                if (parentPlane[parentIndex].lastChild == this.nodeHandle)
                {
                    
                    parentPlane[parentIndex].lastChild = previousSibling;
                }
            }

            thisPlane[thisIndex].parent = 0;
        }

        public void MoveAllChildrenToNewParent(FormatNode newParent)
        {
            while (!this.FirstChild.IsNull)
            {
                FormatNode child = this.FirstChild;
                child.RemoveFromParent();
                newParent.AppendChild(child);
            }
        }

        public void ChangeNodeType(FormatContainerType newType)
        {
            this.nodes.Plane(this.nodeHandle)[this.nodes.Index(this.nodeHandle)].type = newType;
        }

        public void PrepareToClose(uint endTextPosition)
        {
            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            thisPlane[thisIndex].endTextPosition = endTextPosition;
            thisPlane[thisIndex].nodeFlags &= ~FormatStore.NodeFlags.OnRightEdge;
            thisPlane[thisIndex].nodeFlags |= FormatStore.NodeFlags.CanFlush;
        }

        public void SetProps(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] properties, int inheritanceMaskIndex)
        {
            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            thisPlane[thisIndex].flagProperties = flagProperties;
            thisPlane[thisIndex].propertyMask = propertyMask;
            thisPlane[thisIndex].properties = properties;
            thisPlane[thisIndex].inheritanceMaskIndex = inheritanceMaskIndex;
        }

        public FormatNode SplitTextNode(uint splitPosition)
        {
            InternalDebug.Assert(splitPosition >= this.BeginTextPosition && splitPosition <= this.EndTextPosition);

            InternalDebug.Assert(this.NodeType == FormatContainerType.Text &&
                                this.FirstChild == FormatNode.Null &&
                                this.Parent != FormatNode.Null);

            

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            int newNodeHandle = this.nodes.Allocate(FormatContainerType.Text, thisPlane[thisIndex].beginTextPosition);

            FormatStore.NodeEntry[] newPlane = this.nodes.Plane(newNodeHandle);
            int newIndex = this.nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = splitPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            
            thisPlane[thisIndex].beginTextPosition = splitPosition;

            FormatNode newNode = new FormatNode(this.nodes, newNodeHandle);

            
            this.InsertSiblingBefore(newNode);

            
            return newNode;
        }

        public FormatNode SplitNodeBeforeChild(FormatNode child)
        {
            

            InternalDebug.Assert(child.Parent == this &&
                                this.Parent != FormatNode.Null);

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            int newNodeHandle = this.nodes.Allocate(this.NodeType, thisPlane[thisIndex].beginTextPosition);

            FormatStore.NodeEntry[] newPlane = this.nodes.Plane(newNodeHandle);
            int newIndex = this.nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = child.BeginTextPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            
            thisPlane[thisIndex].beginTextPosition = child.BeginTextPosition;

            FormatNode newNode = new FormatNode(this.nodes, newNodeHandle);

            

            FormatNode next;

            do
            {
                next = this.FirstChild;
                next.RemoveFromParent();
                newNode.AppendChild(next);
            }
            while (this.FirstChild != child);

            
            this.InsertSiblingBefore(newNode);

            return newNode;
        }

        public FormatNode DuplicateInsertAsChild()
        {
            

            int newNodeHandle = this.nodes.Allocate(this.NodeType, this.BeginTextPosition);

            FormatStore.NodeEntry[] thisPlane = this.nodes.Plane(this.nodeHandle);
            int thisIndex = this.nodes.Index(this.nodeHandle);

            FormatStore.NodeEntry[] newPlane = this.nodes.Plane(newNodeHandle);
            int newIndex = this.nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = thisPlane[thisIndex].endTextPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            FormatNode newNode = new FormatNode(this.nodes, newNodeHandle);

            
            this.MoveAllChildrenToNewParent(newNode);

            
            this.AppendChild(newNode);

            
            return newNode;
        }

        
        public static bool operator ==(FormatNode x, FormatNode y) 
        {
            return x.nodes == y.nodes && x.nodeHandle == y.nodeHandle;
        }

        
        public static bool operator !=(FormatNode x, FormatNode y) 
        {
            return x.nodes != y.nodes || x.nodeHandle != y.nodeHandle;
        }

        public override bool Equals(object obj)
        {
            return (obj is FormatNode) && this.nodes == ((FormatNode)obj).nodes && this.nodeHandle == ((FormatNode)obj).nodeHandle;
        }

        public override int GetHashCode()
        {
            return this.nodeHandle;
        }

        public NodeSubtree Subtree
        {
            get { return new NodeSubtree(this); }
        }

        public NodeChildren Children
        {
            get { return new NodeChildren(this); }
        }

        internal struct NodeSubtree : IEnumerable<FormatNode>
        {
            private FormatNode node;

            internal NodeSubtree(FormatNode node)
            {
                this.node = node;
            }

            public SubtreeEnumerator GetEnumerator()
            {
                return new SubtreeEnumerator(this.node, false/*revisitParent*/);
            }

            public SubtreeEnumerator GetEnumerator(bool revisitParent)
            {
                return new SubtreeEnumerator(this.node, revisitParent);
            }

            IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
            {
                return new SubtreeEnumerator(this.node, false/*revisitParent*/);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SubtreeEnumerator(this.node, false/*revisitParent*/);
            }
        }

        internal struct NodeChildren : IEnumerable<FormatNode>
        {
            private FormatNode node;

            internal NodeChildren(FormatNode node)
            {
                this.node = node;
            }

            public ChildrenEnumerator GetEnumerator()
            {
                return new ChildrenEnumerator(this.node);
            }

            IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
            {
                return new ChildrenEnumerator(this.node);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ChildrenEnumerator(this.node);
            }
        }

        internal struct ChildrenEnumerator : IEnumerator<FormatNode>
        {
            private FormatNode node;
            private FormatNode current;
            private FormatNode next;

            internal ChildrenEnumerator(FormatNode node)
            {
                this.node = node;
                this.current = FormatNode.Null;
                this.next = this.node.FirstChild;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.current.IsNull)
                    {
                        throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");

                    }

                    return this.current;
                }
            }

            public FormatNode Current
            {
                get
                {
                    if (this.current.IsNull)
                    {
                        throw new InvalidOperationException(this.next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
                    }

                    return this.current;
                }
            }

            public bool MoveNext()
            {
                this.current = this.next;

                if (this.current.IsNull)
                {
                    return false;
                }

                this.next = this.current.NextSibling;
                return true;
            }

            public void Reset()
            {
                this.current = FormatNode.Null;
                this.next = this.node.FirstChild;
            }

            public void Dispose()
            {
                Reset();
            }
        }

        internal struct SubtreeEnumerator : IEnumerator<FormatNode>
        {
            [Flags]
            private enum EnumeratorDisposition : byte
            {
                Begin = 0x01,
                End = 0x02,
            }

            private bool revisitParent;
            private EnumeratorDisposition currentDisposition;
            private FormatNode root;
            private FormatNode current;
            private FormatNode nextChild;
            private int depth;

            internal SubtreeEnumerator(FormatNode node, bool revisitParent)
            {
                this.revisitParent = revisitParent;
                this.root = node;
                this.current = FormatNode.Null;
                this.currentDisposition = 0;
                this.nextChild = node;
                this.depth = -1;
            }

            public FormatNode Current
            {
                get
                {
                    InternalDebug.Assert(this.current != FormatNode.Null);

                    return this.current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    InternalDebug.Assert(this.current != FormatNode.Null);

                    return this.current;
                }
            }

            
            
            
            
            
            
            
            
            public bool FirstVisit
            {
                get
                {
                    InternalDebug.Assert(this.current != FormatNode.Null);

                    return 0 != (this.currentDisposition & EnumeratorDisposition.Begin);
                }
            }

            
            
            
            
            
            
            
            
            public bool LastVisit
            {
                get
                {
                    InternalDebug.Assert(this.current != FormatNode.Null);

                    return 0 != (this.currentDisposition & EnumeratorDisposition.End);
                }
            }

            
            
            
            
            public int Depth
            {
                get
                {
                    InternalDebug.Assert(this.current != FormatNode.Null);

                    return this.depth;
                }
            }

            public bool MoveNext()
            {
                InternalDebug.Assert(this.depth >= -1);

                if (this.nextChild != FormatNode.Null)
                {
                    this.depth ++;

                    this.current = this.nextChild;

                    this.nextChild = this.current.FirstChild;

                    this.currentDisposition = EnumeratorDisposition.Begin | (this.nextChild == FormatNode.Null ? EnumeratorDisposition.End : 0);
                    return true;
                }

                if (this.depth < 0)
                {
                    InternalDebug.Assert(this.current == FormatNode.Null);

                    
                    return false;
                }

                do
                {
                    this.depth --;

                    if (this.depth < 0)
                    {
                        this.current = FormatNode.Null;
                        this.nextChild = FormatNode.Null;
                        this.currentDisposition = 0;
                        return false;
                    }

                    this.nextChild = this.current.NextSibling;

                    this.current = this.current.Parent;
                    this.currentDisposition = (this.nextChild == FormatNode.Null ? EnumeratorDisposition.End : 0);
                }
                while (!this.revisitParent && this.nextChild == FormatNode.Null);

                
                
                

                InternalDebug.Assert(this.nextChild != FormatNode.Null || this.revisitParent);
                return this.revisitParent || this.MoveNext();
            }

            public FormatNode PreviewNextNode()
            {
                InternalDebug.Assert(this.depth >= -1);

                if (this.nextChild != FormatNode.Null)
                {
                    
                    return this.nextChild;
                }

                if (this.depth < 0)
                {
                    InternalDebug.Assert(this.current == FormatNode.Null);

                    
                    return FormatNode.Null;
                }

                int depth = this.depth;
                FormatNode current = this.current;
                FormatNode nextChild;

                do
                {
                    depth --;

                    if (depth < 0)
                    {
                        return FormatNode.Null;
                    }

                    nextChild = current.NextSibling;
                    current = current.Parent;
                }
                while (!this.revisitParent && nextChild == FormatNode.Null);

                InternalDebug.Assert(nextChild != FormatNode.Null || this.revisitParent);
                return this.revisitParent ? current : nextChild;
            }

            
            
            
            
            public void SkipChildren()
            {
                InternalDebug.Assert(this.current != FormatNode.Null);

                if (this.nextChild != FormatNode.Null)
                {
                    this.nextChild = FormatNode.Null;
                    this.currentDisposition |= EnumeratorDisposition.End;
                }
            }

            void IEnumerator.Reset()
            {
                this.current = FormatNode.Null;
                this.currentDisposition = 0;
                this.nextChild = this.root;
                this.depth = -1;
            }

            void IDisposable.Dispose()
            {
                ((IEnumerator)this).Reset();
                GC.SuppressFinalize(this);
            }
        }

    }


    

    internal struct FormatStyle
    {
        internal FormatStore.StyleStore styles;
        internal int styleHandle;

        public static readonly FormatStyle Null = new FormatStyle();

        internal FormatStyle(FormatStore store, int styleHandle)
        {
            this.styles = store.styles;
            this.styleHandle = styleHandle;
        }

        internal FormatStyle(FormatStore.StyleStore styles, int styleHandle)
        {
            this.styles = styles;
            this.styleHandle = styleHandle;
        }

        public int Handle { get { return this.styleHandle; } }

        public bool IsNull
        {
            get { return this.styleHandle == 0; }
        }

        internal int RefCount
        {
            get { return this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount; }
        }

        public bool IsEmpty
        {
            get { return this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyMask.IsClear &&
                (this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyList == null ||
                    this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyList.Length == 0); }
        }

        public FlagProperties FlagProperties
        {
            get { return this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].flagProperties; }
            set { this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { return this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyMask; }
            set { this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyMask = value; }
        }

        public Property[] PropertyList
        {
            get { return this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyList; }
            set { this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].propertyList = value; }
        }

        public void AddRef()
        {
            InternalDebug.Assert(this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount > 0);

            if (this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount != Int32.MaxValue)
            {
                this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount > 0);

            if (this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount != Int32.MaxValue)
            {
                if (--this.styles.Plane(this.styleHandle)[this.styles.Index(this.styleHandle)].refCount == 0)
                {
                    this.styles.Free(this.styleHandle);
                }
            }

            this.styleHandle = -1;
        }

    }

    

    internal struct StringValue
    {
        internal FormatStore.StringValueStore strings;
        internal int stringHandle;

        internal StringValue(FormatStore store, int stringHandle)
        {
            this.strings = store.strings;
            this.stringHandle = stringHandle;
        }

        internal StringValue(FormatStore.StringValueStore strings, int stringHandle)
        {
            this.strings = strings;
            this.stringHandle = stringHandle;
        }

        public PropertyValue PropertyValue { get { return new PropertyValue(PropertyType.String, this.stringHandle); } }

        internal int Handle { get { return this.stringHandle; } }

        public int Length
        {
            get { return this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].str.Length; }
        }

        public int RefCount
        {
            get { return this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount; }
        }

        public string GetString()
        {
            return this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].str;
        }

        internal void SetString(string str)
        {
            this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].str = str;
        }

        public void CopyTo(int sourceOffset, char[] buffer, int offset, int count)
        {
            this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].str.CopyTo(sourceOffset, buffer, offset, count);
        }

        public void AddRef()
        {
            InternalDebug.Assert(this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount > 0);

            if (this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount != Int32.MaxValue)
            {
                

                this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount > 0);

            if (this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount != Int32.MaxValue)
            {
                

                if (--this.strings.Plane(this.stringHandle)[this.strings.Index(this.stringHandle)].refCount == 0)
                {
                    this.strings.Free(this.stringHandle);
                }
            }

            this.stringHandle = -1;
        }
    }

    

    internal struct MultiValue
    {
        internal FormatStore.MultiValueStore multiValues;
        internal int multiValueHandle;

        internal MultiValue(FormatStore store, int multiValueHandle)
        {
            this.multiValues = store.multiValues;
            this.multiValueHandle = multiValueHandle;
        }

        internal MultiValue(FormatStore.MultiValueStore multiValues, int multiValueHandle)
        {
            this.multiValues = multiValues;
            this.multiValueHandle = multiValueHandle;
        }

        public PropertyValue PropertyValue { get { return new PropertyValue(PropertyType.MultiValue, this.multiValueHandle); } }

        internal int Handle { get { return this.multiValueHandle; } }

        public int Length
        {
            get { return this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].values.Length; }
        }

        internal int RefCount
        {
            get { return this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount; }
        }

        public PropertyValue this[int index]
        {
            get { return this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].values[index]; }
            
        }

        public StringValue GetStringValue(int index)
        {
            return this.multiValues.Store.GetStringValue(this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].values[index]);
        }

        public void AddRef()
        {
            InternalDebug.Assert(this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount > 0);

            if (this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount != Int32.MaxValue)
            {
                
                this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount > 0);

            if (this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount != Int32.MaxValue)
            {
                

                if (--this.multiValues.Plane(this.multiValueHandle)[this.multiValues.Index(this.multiValueHandle)].refCount == 0)
                {
                    this.multiValues.Free(this.multiValueHandle);
                }
            }

            this.multiValueHandle = -1;
        }
    }

    

    internal enum TextRunType : ushort
    {
        

        Invalid = (0 << 13),
        Skip = Invalid,

        NonSpace = (1 << 13),

        FirstShort = InlineObject,  

        InlineObject = (2 << 13),

        

        NbSp = (3 << 13),
        Space = (4 << 13),
        Tabulation = (5 << 13),

        NewLine = (6 << 13),
        BlockBoundary = (7 << 13),
    }

    internal struct TextRun
    {
        private FormatStore.TextStore text;
        private uint position;
        private bool isImmutable;

        public static readonly TextRun Invalid = new TextRun();

        internal TextRun(FormatStore.TextStore text, uint position)
        {
            this.isImmutable = false;
            this.text = text;
            this.position = position;
        }

        public const int MaxEffectiveLength = FormatStore.TextStore.MaxRunEffectivelength;

        

        public uint Position { get { return this.position; } }
        public TextRunType Type { get { return FormatStore.TextStore.TypeFromRunHeader(this.text.Pick(this.position)); } }
        public int EffectiveLength { get { return FormatStore.TextStore.LengthFromRunHeader(this.text.Pick(this.position)); } }
        public int Length { get { char ch = this.text.Pick(this.position); return ch >= (char)TextRunType.FirstShort ? 1 : FormatStore.TextStore.LengthFromRunHeader(ch) + 1; } }

        
        
        
        public int WordLength
        {
            get
            {
                int returnValue = 0;

                
                
                
                
                TextRun nextRun = this;

                for(;
                    !nextRun.IsEnd() && nextRun.Type == TextRunType.NonSpace && returnValue < 1024;
                    nextRun = nextRun.GetNext())
                {
                    returnValue += nextRun.EffectiveLength;
                };

                return returnValue;
            }
        }

        private bool IsLong { get { return this.Type < TextRunType.FirstShort; } }

        public char this[int index]
        {
            get
            {
                InternalDebug.Assert(this.Type == TextRunType.NonSpace && index < this.EffectiveLength);
                return this.text.Plane(this.position)[this.text.Index(this.position) + 1 + index];
            }
        }

        
        
        
        public char GetWordChar(int index)
        {
            if (index < this.EffectiveLength)
            {
                return this[index];
            }
            else
            {
                TextRun nextRun = this.GetNext();
                index -= this.EffectiveLength;

                while(!nextRun.IsEnd())
                {
                    if (index < nextRun.EffectiveLength)
                    {
                        return nextRun[index];
                    }
                    else if (nextRun.Type != TextRunType.NonSpace)
                    {
                        break;
                    }

                    index -= nextRun.EffectiveLength;
                    nextRun = nextRun.GetNext();
                };

                throw new ArgumentOutOfRangeException("index");
            }
        }

        public void MoveNext()
        {
            if (this.isImmutable)
            {
                throw new InvalidOperationException("This run is immutable");
            }
            this.position += (uint)this.Length;
        }

        
        
        
        internal void MakeImmutable()
        {
            this.isImmutable = true;
        }

        public void SkipInvalid()
        {
            if (this.isImmutable)
            {
                throw new InvalidOperationException("This run is immutable");
            }
            while (!this.IsEnd() && this.Type == TextRunType.Invalid)
            {
                this.MoveNext();
            }
        }

        public bool IsEnd()
        {
            InternalDebug.Assert(this.position <= this.text.CurrentPosition);
            return this.position >= this.text.CurrentPosition;
        }

        public TextRun GetNext()
        {
            return new TextRun(this.text, this.position + (uint)this.Length);
        }

        public void GetChunk(int start, out char[] buffer, out int offset, out int count)
        {
            InternalDebug.Assert(this.IsLong);
            InternalDebug.Assert(start <= this.EffectiveLength);

            buffer = this.text.Plane(this.position);
            offset = this.text.Index(this.position) + 1 + start;
            count = this.EffectiveLength - start;
        }

        public int AppendFragment(int start, ref ScratchBuffer scratchBuffer, int maxLength)
        {
            InternalDebug.Assert(this.IsLong);
            InternalDebug.Assert(start <= this.EffectiveLength);

            int copyOffset = this.text.Index(this.position) + 1 + start;
            int copyLength = Math.Min(this.EffectiveLength - start, maxLength);

            if (copyLength != 0)
            {
                scratchBuffer.Append(this.text.Plane(this.position), copyOffset, copyLength);
            }

            return copyLength;
        }

        public void ConvertToInvalid()
        {
            this.text.ConvertToInvalid(this.position);
        }

        public void ConvertToInvalid(int count)
        {
            InternalDebug.Assert(this.Type == TextRunType.NonSpace);
            this.text.ConvertToInvalid(this.position, count);
        }

        public void ConvertShort(TextRunType type, int newEffectiveLength)
        {
            InternalDebug.Assert(!this.IsLong);
            this.text.ConvertShortRun(this.position, type, newEffectiveLength);
        }

        public override string ToString()
        {
            int wordLength = this.WordLength;
            StringBuilder stringBuilder = new StringBuilder(wordLength);
            for (int iWord = 0; iWord < wordLength; ++iWord)
            {
                stringBuilder.Append(this.GetWordChar(iWord));
            }

            return stringBuilder.ToString();
        }
    }

    

    internal struct NodePropertiesEnumerator : IEnumerable<Property>, IEnumerator<Property>
    {
        internal FlagProperties flagProperties;          
        internal Property[] properties;                  
        internal int currentPropertyIndex;
        internal Property currentProperty;

        public NodePropertiesEnumerator(FormatNode node)
        {
            this.flagProperties = node.FlagProperties;
            this.properties = node.Properties;
            this.currentPropertyIndex = 0;
            this.currentProperty = Property.Null;
        }

        public IEnumerator<Property> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Property Current
        {
            get
            {
                return this.currentProperty;
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                return this.currentProperty;
            }
        }

        public bool MoveNext()
        {
            while (this.currentPropertyIndex < (int)PropertyId.LastFlag)
            {
                this.currentPropertyIndex ++;

                InternalDebug.Assert(FlagProperties.IsFlagProperty((PropertyId)this.currentPropertyIndex));

                if (this.flagProperties.IsDefined((PropertyId)this.currentPropertyIndex))
                {
                    this.currentProperty.Set((PropertyId)this.currentPropertyIndex, new PropertyValue(this.flagProperties.IsOn((PropertyId)this.currentPropertyIndex)));
                    return true;
                }
            }

            if (this.properties != null)
            {
                if (this.currentPropertyIndex < this.properties.Length + ((int)PropertyId.LastFlag + 1))
                {
                    this.currentPropertyIndex ++;

                    if (this.currentPropertyIndex < this.properties.Length + ((int)PropertyId.LastFlag + 1))
                    {
                        this.currentProperty = this.properties[this.currentPropertyIndex - ((int)PropertyId.LastFlag + 1)];
                        return true;
                    }
                }
            }

            this.currentProperty = Property.Null;
            return false;
        }

        public void Reset()
        {
            this.currentPropertyIndex = 0;
            this.currentProperty = Property.Null;
        }

        public void Dispose()
        {
        }
    }
}

