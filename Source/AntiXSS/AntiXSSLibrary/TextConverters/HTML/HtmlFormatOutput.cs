// ***************************************************************
// <copyright file="HtmlFormatOutput.cs" company="Microsoft">
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
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;

    

    internal class HtmlFormatOutput : FormatOutput, IRestartable
    {
        private const int MaxRecognizedHyperlinkLength = 4096;

        

        internal HtmlWriter writer;

        private HtmlInjection injection;

        private bool filterHtml;
        private HtmlTagCallback callback;
        private HtmlFormatOutputCallbackContext callbackContext;

        private bool outputFragment;

        private bool recognizeHyperlinks;

        
#if DEBUG
        private Stream formatTraceStream;
#endif
        private int hyperlinkLevel;

        private struct EndTagActionEntry
        {
            public int tagLevel;
            public bool drop;
            public bool callback;
        }

        private EndTagActionEntry[] endTagActionStack;
        private int endTagActionStackTop;

        

        public HtmlFormatOutput(
                HtmlWriter writer,
                HtmlInjection injection,
                bool outputFragment,
                Stream formatTraceStream,
                Stream formatOutputTraceStream,
                bool filterHtml,
                HtmlTagCallback callback,
                bool recognizeHyperlinks) :
                base(formatOutputTraceStream)
        {
            this.writer = writer;
            this.injection = injection;
            this.outputFragment = outputFragment;
#if DEBUG
            this.formatTraceStream = formatTraceStream;
#endif
            this.filterHtml = filterHtml;
            this.callback = callback;

            this.recognizeHyperlinks = recognizeHyperlinks;
        }

        internal void SetWriter(HtmlWriter writer)
        {
            this.writer = writer;
        }

        

        bool IRestartable.CanRestart()
        {
            if (this.writer is IRestartable)
            {
                return ((IRestartable)this.writer).CanRestart();
            }

            return false;
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            ((IRestartable)this.writer).Restart();

            

            this.Restart();

            if (this.injection != null)
            {
                this.injection.Reset();
            }

            this.hyperlinkLevel = 0;
        }

        

        void IRestartable.DisableRestart()
        {
            if (this.writer is IRestartable)
            {
                ((IRestartable)this.writer).DisableRestart();
            }
        }

        

        public override bool Flush()
        {
            if (!base.Flush())
            {
                return false;
            }

            this.writer.Flush();
            return true;
        }

        

        public override bool OutputCodePageSameAsInput
        {
            get
            {
                
                return false;
            }

        }

        

        public override Encoding OutputEncoding
        {
            set
            {
                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        

        public override bool CanAcceptMoreOutput
        {
            get
            {
                return this.writer.CanAcceptMore;
            }
        }

        

        protected override bool StartRoot()
        {
            return true;
        }

        

        protected override void EndRoot()
        {
        }

        

        protected override bool StartDocument()
        {
#if DEBUG
            if (this.formatTraceStream != null)
            {
                this.FormatStore.Dump(this.formatTraceStream, true);
            }
#endif
            if (!this.outputFragment)
            {
                bool endTagCallbackRequested = false;
                bool dropInnerContent = false;
                bool dropEndTag = false;

                this.writer.WriteStartTag(HtmlNameIndex.Html);

                if (this.callback != null)
                {
                    if (this.callbackContext == null)
                    {
                        this.callbackContext = new HtmlFormatOutputCallbackContext(this);
                    }

                    
                    this.callbackContext.InitializeTag(false, HtmlNameIndex.Head, false);
                }
                else
                {
                    this.writer.WriteStartTag(HtmlNameIndex.Head);
                }

                if (this.callback != null)
                {
                    this.callbackContext.InitializeFragment(false);

                    this.callback(this.callbackContext, this.writer);

                    this.callbackContext.UninitializeFragment();

                    if (this.callbackContext.IsInvokeCallbackForEndTag)
                    {
                        
                        

                        endTagCallbackRequested = true;
                    }

                    if (this.callbackContext.IsDeleteInnerContent)
                    {
                        dropInnerContent = true;
                    }

                    if (this.callbackContext.IsDeleteEndTag)
                    {
                        dropEndTag = true;
                    }
                }

                if (!dropInnerContent)
                {
                    if (this.writer.HasEncoding)
                    {
                        this.writer.WriteStartTag(HtmlNameIndex.Meta);
                        this.writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
                        this.writer.WriteAttributeName(HtmlNameIndex.Content);
                        this.writer.WriteAttributeValueInternal("text/html; charset=");
                        this.writer.WriteAttributeValue(Charset.GetCharset(this.writer.Encoding.CodePage).Name);
                        this.writer.WriteNewLine(true);
                    }

                    this.writer.WriteStartTag(HtmlNameIndex.Meta);
                    this.writer.WriteAttribute(HtmlNameIndex.Name, "Generator");
                    this.writer.WriteAttribute(HtmlNameIndex.Content, "Microsoft Exchange Server");     
                    this.writer.WriteNewLine(true);

                    if (this.Comment != null)
                    {
                        this.writer.WriteMarkupText("<!-- " + this.Comment + " -->");
                        this.writer.WriteNewLine(true);
                    }

                    this.writer.WriteStartTag(HtmlNameIndex.Style);
                    
                    
                    this.writer.WriteMarkupText("<!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } -->");
                    this.writer.WriteEndTag(HtmlNameIndex.Style);
                }

                if (endTagCallbackRequested)
                {
                    this.callbackContext.InitializeTag(true, HtmlNameIndex.Head, dropEndTag);

                    this.callbackContext.InitializeFragment(false);

                    this.callback(this.callbackContext, this.writer);

                    this.callbackContext.UninitializeFragment();
                }
                else if (!dropEndTag)
                {
                    this.writer.WriteEndTag(HtmlNameIndex.Head);
                    this.writer.WriteNewLine(true);
                }

                this.writer.WriteStartTag(HtmlNameIndex.Body);
                this.writer.WriteNewLine(true);
            }
            else
            {
                this.writer.WriteStartTag(HtmlNameIndex.Div);
                this.writer.WriteAttribute(HtmlNameIndex.Class, "BodyFragment");
                this.writer.WriteNewLine(true);
            }

            if (this.injection != null && this.injection.HaveHead)
            {
                InternalDebug.Assert(!this.injection.HeadDone);
                this.injection.Inject(true, this.writer);
            }

            this.ApplyCharFormat();
            return true;
        }

        

        protected override void EndDocument()
        {
            this.RevertCharFormat();

            if (this.injection != null && this.injection.HaveTail)
            {
                InternalDebug.Assert(!this.injection.TailDone);
                this.injection.Inject(false, this.writer);
            }

            if (!this.outputFragment)
            {
                this.writer.WriteNewLine(true);
                this.writer.WriteEndTag(HtmlNameIndex.Body);
                this.writer.WriteNewLine(true);
                this.writer.WriteEndTag(HtmlNameIndex.Html);
            }
            else
            {
                this.writer.WriteNewLine(true);
                this.writer.WriteEndTag(HtmlNameIndex.Div);
            }

            this.writer.WriteNewLine(true);
        }

        

        protected override void StartEndBaseFont()
        {
        }

        

        protected override bool StartTable()
        {
            PropertyValue fontFaceValue = this.GetDistinctProperty(PropertyId.FontFace);
            if (!fontFaceValue.IsNull)
            {
                this.writer.WriteStartTag(HtmlNameIndex.Font);

                this.writer.WriteAttributeName(HtmlNameIndex.Face);

                string name;

                StringValue sv;
                MultiValue mv;

                if (fontFaceValue.IsMultiValue)
                {
                    mv = this.FormatStore.GetMultiValue(fontFaceValue);

                    for (int i = 0; i < mv.Length; i++)
                    {
                        sv = mv.GetStringValue(i);
                        name = sv.GetString();
                        if (i != 0)
                        {
                            this.writer.WriteAttributeValue(",");
                        }
                        this.writer.WriteAttributeValue(name);
                    }
                }
                else
                {
                    sv = this.FormatStore.GetStringValue(fontFaceValue);
                    name = sv.GetString();
                    this.writer.WriteAttributeValue(name);
                }
            }

            this.writer.WriteNewLine(true);
            this.writer.WriteStartTag(HtmlNameIndex.Table);

            this.OutputTableTagAttributes();

            bool styleAttributeOpen = false;

            this.OutputTableCssProperties(ref styleAttributeOpen);

            this.OutputBlockCssProperties(ref styleAttributeOpen);

            this.writer.WriteNewLine(true);

            return true;
        }

        

        protected override void EndTable()
        {
            this.writer.WriteNewLine(true);
            this.writer.WriteEndTag(HtmlNameIndex.Table);
            this.writer.WriteNewLine(true);

            PropertyValue fontFaceValue = this.GetDistinctProperty(PropertyId.FontFace);
            if (!fontFaceValue.IsNull)
            {
                this.writer.WriteEndTag(HtmlNameIndex.Font);
            }
        }

        

        protected override bool StartTableColumnGroup()
        {
            this.writer.WriteNewLine(true);
            this.writer.WriteStartTag(HtmlNameIndex.ColGroup);

            
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            PropertyValue span = this.GetDistinctProperty(PropertyId.NumColumns);
            if (!span.IsNull && span.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Span, span.Integer.ToString());
            }

            bool styleAttributeOpen = false;

            this.OutputTableColumnCssProperties(ref styleAttributeOpen);

            return true;
        }

        

        protected override void EndTableColumnGroup()
        {
            this.writer.WriteEndTag(HtmlNameIndex.ColGroup);
            this.writer.WriteNewLine(true);
        }

        

        protected override void StartEndTableColumn()
        {
            this.writer.WriteStartTag(HtmlNameIndex.Col);

            
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            PropertyValue span = this.GetDistinctProperty(PropertyId.NumColumns);
            if (!span.IsNull && span.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Span, span.Integer.ToString());
            }

            bool styleAttributeOpen = false;

            this.OutputTableColumnCssProperties(ref styleAttributeOpen);

            this.writer.WriteNewLine(true);
        }

        

        protected override bool StartTableCaption()
        {
            this.writer.WriteNewLine(true);

            if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == FormatContainerType.Table)
            {
                this.writer.WriteStartTag(HtmlNameIndex.Caption);

                FormatStyle defaultStyle = this.FormatStore.GetStyle(HtmlConverterData.DefaultStyle.Caption);
                this.SubtractDefaultContainerPropertiesFromDistinct(defaultStyle.FlagProperties, defaultStyle.PropertyList);

                
                PropertyValue pv = this.GetDistinctProperty(PropertyId.BlockAlignment);
                if (!pv.IsNull)
                {
                    string val = HtmlSupport.GetBlockAlignmentString(pv);
                    if (val != null)
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                    }
                }

                this.writer.WriteNewLine(true);
            }

            this.ApplyCharFormat();

            return true;
        }

        

        protected override void EndTableCaption()
        {
            this.RevertCharFormat();

            if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == FormatContainerType.Table)
            {
                this.writer.WriteNewLine(true);
                this.writer.WriteEndTag(HtmlNameIndex.Caption);
            }

            this.writer.WriteNewLine(true);
        }

        protected override bool StartTableExtraContent()
        {
            return this.StartBlockContainer();
        }

        protected override void EndTableExtraContent()
        {
            this.EndBlockContainer();
        }

        

        protected override bool StartTableRow()
        {
            this.writer.WriteNewLine(true);
            this.writer.WriteStartTag(HtmlNameIndex.TR);

            
            PropertyValue height = this.GetDistinctProperty(PropertyId.Height);
            if (!height.IsNull && height.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Height, height.PixelsInteger.ToString());
            }

            bool styleAttributeOpen = false;

            this.OutputBlockCssProperties(ref styleAttributeOpen);

            this.writer.WriteNewLine(true);

            return true;
        }

        

        protected override void EndTableRow()
        {
            this.writer.WriteNewLine(true);
            this.writer.WriteEndTag(HtmlNameIndex.TR);
            this.writer.WriteNewLine(true);
        }

        

        protected override bool StartTableCell()
        {
            PropertyValue mergeCell = this.GetDistinctProperty(PropertyId.MergedCell);

            
            if (mergeCell.IsNull || !mergeCell.Bool)
            {
                this.writer.WriteNewLine(true);
                this.writer.WriteStartTag(HtmlNameIndex.TD);

                this.OutputTableCellTagAttributes();

                bool styleAttributeOpen = false;

                this.OutputBlockCssProperties(ref styleAttributeOpen);

                this.ApplyCharFormat();
            }

            return true;
        }

        

        protected override void EndTableCell()
        {
            PropertyValue mergeCell = this.GetDistinctProperty(PropertyId.MergedCell);

            if (mergeCell.IsNull || !mergeCell.Bool)
            {
                this.RevertCharFormat();

                this.writer.WriteEndTag(HtmlNameIndex.TD);
                this.writer.WriteNewLine(true);
            }
        }

        

        private static string[] listType =
        {
            null,   
            null,   
            "1",
            "a",
            "A",
            "i",
            "I",
        };

        protected override bool StartList()
        {
            this.writer.WriteNewLine(true);

            PropertyValue listStyle = this.GetEffectiveProperty(PropertyId.ListStyle);

            bool bulletedList = true;

            if (listStyle.IsNull || (ListStyle)listStyle.Enum == ListStyle.Bullet)
            {
                this.writer.WriteStartTag(HtmlNameIndex.UL);
            }
            else
            {
                this.writer.WriteStartTag(HtmlNameIndex.OL);
                bulletedList = false;
            }

            
            PropertyValue rtl = this.GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            
            if (!bulletedList && (ListStyle)listStyle.Enum != ListStyle.Decimal)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Type, listType[listStyle.Enum]);
            }

            
            PropertyValue listStart = this.GetDistinctProperty(PropertyId.ListStart);
            if (!bulletedList && listStart.IsInteger && listStart.Integer != 1)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Start, listStart.Integer.ToString());
            }

            bool styleAttributeOpen = false;

            this.OutputBlockCssProperties(ref styleAttributeOpen);

            this.writer.WriteNewLine(true);

            this.ApplyCharFormat();

            return true;
        }

        

        protected override void EndList()
        {
            this.RevertCharFormat();

            PropertyValue listStyle = this.GetEffectiveProperty(PropertyId.ListStyle);

            this.writer.WriteNewLine(true);

            if (listStyle.IsNull || (ListStyle)listStyle.Enum == ListStyle.Bullet)
            {
                this.writer.WriteEndTag(HtmlNameIndex.UL);
            }
            else
            {
                this.writer.WriteEndTag(HtmlNameIndex.OL);
            }

            this.writer.WriteNewLine(true);
        }

        

        protected override bool StartListItem()
        {
            this.writer.WriteNewLine(true);
            this.writer.WriteStartTag(HtmlNameIndex.LI);

            bool styleAttributeOpen = false;

            this.OutputBlockCssProperties(ref styleAttributeOpen);

            this.ApplyCharFormat();

            return true;
        }

        

        protected override void EndListItem()
        {
            this.RevertCharFormat();

            this.writer.WriteEndTag(HtmlNameIndex.LI);
            this.writer.WriteNewLine(true);
        }

        

        private static Property[] DefaultHyperlinkProperties = new Property[]
        {
            new Property(PropertyId.FontColor, new PropertyValue(new RGBT(0,0,255))),
        };

        protected override bool StartHyperLink()
        {
            bool dropEndTag = false;
            bool dropInnerContent = false;
            bool endTagCallbackRequested = false;

            FlagProperties defaultFlags = new FlagProperties();
            defaultFlags.Set(PropertyId.Underline, true);
            this.SubtractDefaultContainerPropertiesFromDistinct(defaultFlags, DefaultHyperlinkProperties);

            if (this.callback != null)
            {
                if (this.callbackContext == null)
                {
                    this.callbackContext = new HtmlFormatOutputCallbackContext(this);
                }

                
                this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
            }
            else
            {
                this.writer.WriteStartTag(HtmlNameIndex.A);
            }

            PropertyValue pv = this.GetDistinctProperty(PropertyId.HyperlinkUrl);
            if (!pv.IsNull)
            {
                StringValue sv = this.FormatStore.GetStringValue(pv);
                string url = sv.GetString();

                if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, this.callback != null))
                {
                    url = string.Empty;
                }

                if (this.callback != null)
                {
                    this.callbackContext.AddAttribute(HtmlNameIndex.Href, url);
                }
                else
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Href);
                    this.writer.WriteAttributeValue(url);
                }

                pv = this.GetDistinctProperty(PropertyId.HyperlinkTarget);
                if (!pv.IsNull)
                {
                    string target = HtmlSupport.GetTargetString(pv);

                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Target, target);
                    }
                    else
                    {
                        this.writer.WriteAttributeName(HtmlNameIndex.Target);
                        this.writer.WriteAttributeValue(target);
                    }
                }
            }

            

            if (this.callback != null)
            {
                this.callbackContext.InitializeFragment(false);

                this.callback(this.callbackContext, this.writer);

                this.callbackContext.UninitializeFragment();

                if (this.callbackContext.IsInvokeCallbackForEndTag)
                {
                    
                    

                    endTagCallbackRequested = true;
                }

                if (this.callbackContext.IsDeleteInnerContent)
                {
                    dropInnerContent = true;
                }

                if (this.callbackContext.IsDeleteEndTag)
                {
                    dropEndTag = true;
                }

                if (dropEndTag || endTagCallbackRequested)
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

                    this.endTagActionStack[this.endTagActionStackTop].tagLevel = this.hyperlinkLevel;
                    this.endTagActionStack[this.endTagActionStackTop].drop = dropEndTag;
                    this.endTagActionStack[this.endTagActionStackTop].callback = endTagCallbackRequested;

                    this.endTagActionStackTop++;
                }
            }

            this.hyperlinkLevel ++;

            if (!dropInnerContent)
            {
                this.ApplyCharFormat();
            }
            else
            {
                
                this.CloseHyperLink();
            }

            if (this.writer.IsTagOpen)
            {
                this.writer.WriteTagEnd();
            }

            return !dropInnerContent;
        }

        

        protected override void EndHyperLink()
        {
            this.hyperlinkLevel --;

            RevertCharFormat();

            this.CloseHyperLink();

            if (this.writer.IsTagOpen)
            {
                this.writer.WriteTagEnd();
            }
        }

        

        private void CloseHyperLink()
        {
            bool dropTag = false;
            bool endTagCallbackRequested = false;

            if (this.endTagActionStackTop != 0)
            {
                InternalDebug.Assert(this.callback != null && this.callbackContext != null);
                InternalDebug.Assert(this.endTagActionStack[this.endTagActionStackTop - 1].tagLevel <= this.hyperlinkLevel);

                
                
                

                if (this.endTagActionStack[this.endTagActionStackTop - 1].tagLevel == this.hyperlinkLevel)
                {
                    this.endTagActionStackTop --;

                    dropTag = this.endTagActionStack[this.endTagActionStackTop].drop;
                    endTagCallbackRequested = this.endTagActionStack[this.endTagActionStackTop].callback;
                }
            }

            if (endTagCallbackRequested)
            {
                InternalDebug.Assert(this.callback != null && this.callbackContext != null);

                this.callbackContext.InitializeTag(true, HtmlNameIndex.A, dropTag);
                this.callbackContext.InitializeFragment(false);

                this.callback(this.callbackContext, this.writer);

                this.callbackContext.UninitializeFragment();
            }
            else if (!dropTag)
            {
                this.writer.WriteEndTag(HtmlNameIndex.A);
            }
        }

        

        protected override bool StartBookmark()
        {
            PropertyValue pv = this.GetDistinctProperty(PropertyId.BookmarkName);
            if (!pv.IsNull)
            {
                

                this.writer.WriteStartTag(HtmlNameIndex.A);

                StringValue sv = this.FormatStore.GetStringValue(pv);
                string name = sv.GetString();

                this.writer.WriteAttributeName(HtmlNameIndex.Name);
                this.writer.WriteAttributeValue(name);
            }

            this.ApplyCharFormat();

            if (this.writer.IsTagOpen)
            {
                this.writer.WriteTagEnd();
            }

            return true;
        }

        

        protected override void EndBookmark()
        {
            RevertCharFormat();

            PropertyValue pv = this.GetDistinctProperty(PropertyId.BookmarkName);
            if (!pv.IsNull)
            {
                this.writer.WriteEndTag(HtmlNameIndex.A);
            }

            if (this.writer.IsTagOpen)
            {
                this.writer.WriteTagEnd();
            }
        }

        

        protected override void StartEndImage()
        {
            if (this.callback != null)
            {
                if (this.callbackContext == null)
                {
                    this.callbackContext = new HtmlFormatOutputCallbackContext(this);
                }

                
                this.callbackContext.InitializeTag(false, HtmlNameIndex.Img, false);
            }
            else
            {
                this.writer.WriteStartTag(HtmlNameIndex.Img);
            }

            PropertyValue pv = this.GetDistinctProperty(PropertyId.Width);
            if (!pv.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Width, val.ToString());
                    }
                    else
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Width, val);
                    }
                }
            }

            pv = this.GetDistinctProperty(PropertyId.Height);
            if (!pv.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Height, val.ToString());
                    }
                    else
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Height, val);
                    }
                }
            }

            pv = this.GetDistinctProperty(PropertyId.BlockAlignment);
            if (!pv.IsNull)
            {
                string val = HtmlSupport.GetBlockAlignmentString(pv);
                if (val != null)
                {
                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Align, val);
                    }
                    else
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                    }
                }
            }

            pv = this.GetDistinctProperty(PropertyId.ImageBorder);
            if (!pv.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Border, val.ToString());
                    }
                    else
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Border, val);
                    }
                }
            }

            pv = this.GetDistinctProperty(PropertyId.ImageUrl);
            if (!pv.IsNull)
            {
                

                StringValue sv = this.FormatStore.GetStringValue(pv);
                string url = sv.GetString();

                if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, this.callback != null))
                {
                    url = string.Empty;
                }

                if (this.callback != null)
                {
                    this.callbackContext.AddAttribute(HtmlNameIndex.Src, url);
                }
                else
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Src);
                    this.writer.WriteAttributeValue(url);
                }
            }

            pv = this.GetDistinctProperty(PropertyId.RightToLeft);
            if (pv.IsBool)
            {
                if (this.callback != null)
                {
                    this.callbackContext.AddAttribute(HtmlNameIndex.Dir, pv.Bool ? "rtl" : "ltr");
                }
                else
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Dir);
                    this.writer.WriteAttributeValue(pv.Bool ? "rtl" : "ltr");
                }
            }

            pv = this.GetDistinctProperty(PropertyId.Language);
            if (pv.IsInteger)
            {
                Microsoft.Exchange.Data.Globalization.Culture culture;

                if (Microsoft.Exchange.Data.Globalization.Culture.TryGetCulture(pv.Integer, out culture) || String.IsNullOrEmpty(culture.Name))
                {
                    if (this.callback != null)
                    {
                        this.callbackContext.AddAttribute(HtmlNameIndex.Lang, culture.Name);
                    }
                    else
                    {
                        this.writer.WriteAttributeName(HtmlNameIndex.Lang);
                        this.writer.WriteAttributeValue(culture.Name);
                    }
                }
            }

            pv = this.GetDistinctProperty(PropertyId.ImageAltText);
            if (!pv.IsNull)
            {
                

                StringValue sv = this.FormatStore.GetStringValue(pv);
                string altText = sv.GetString();

                if (this.callback != null)
                {
                    this.callbackContext.AddAttribute(HtmlNameIndex.Alt, altText);
                }
                else
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Alt);
                    this.writer.WriteAttributeValue(altText);
                }
            }

            

            if (this.callback != null)
            {
                this.callbackContext.InitializeFragment(true);

                this.callback(this.callbackContext, this.writer);

                this.callbackContext.UninitializeFragment();

                
            }

            if (this.writer.IsTagOpen)
            {
                this.writer.WriteTagEnd();
            }
        }

        

        protected override void StartEndHorizontalLine()
        {
            this.writer.WriteNewLine(true);

            this.writer.WriteStartTag(HtmlNameIndex.HR);

            
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, width);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Width, val);
                }
            }

            
            PropertyValue size = this.GetDistinctProperty(PropertyId.Height);
            if (!size.IsNull && size.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Size, size.PixelsInteger.ToString());
            }

            
            PropertyValue align = this.GetDistinctProperty(PropertyId.HorizontalAlignment);
            if (!align.IsNull)
            {
                string val = HtmlSupport.GetHorizontalAlignmentString(align);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            
            PropertyValue color = this.GetDistinctProperty(PropertyId.FontColor);
            if (!color.IsNull)
            {
                BufferString val = HtmlSupport.FormatColor(ref this.scratchBuffer, color);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Color, val);
                }
            }

            
            if (!width.IsNull)
            {
                this.writer.WriteAttributeName(HtmlNameIndex.Style);

                if (!width.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("width:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }
            }

            if (this.writer.LiteralWhitespaceNesting == 0)
            {
                this.writer.WriteNewLine(true);
            }
        }

        

        protected override bool StartInline()
        {
            this.ApplyCharFormat();
            return true;
        }

        

        protected override void EndInline()
        {
            this.RevertCharFormat();
        }

        

        protected override bool StartMap()
        {
            return this.StartBlockContainer();
        }

        

        protected override void EndMap()
        {
            this.EndBlockContainer();
        }

        

        protected override void StartEndArea()
        {
        }

        

        protected override bool StartForm()
        {
            return this.StartInlineContainer();
        }

        

        protected override void EndForm()
        {
            this.EndInlineContainer();
        }

        

        protected override bool StartFieldSet()
        {
            return this.StartBlockContainer();
        }

        

        protected override void EndFieldSet()
        {
            this.EndBlockContainer();
        }

        

        protected override bool StartSelect()
        {
            return true;
        }

        

        protected override void EndSelect()
        {
        }

        

        protected override bool StartOptionGroup()
        {
            return true;
        }

        

        protected override void EndOptionGroup()
        {
        }

        

        protected override bool StartOption()
        {
            return true;
        }

        

        protected override void EndOption()
        {
        }

        

        protected override bool StartText()
        {
            this.ApplyCharFormat();
            this.writer.StartTextChunk();
            return true;
        }

        

        protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
        {
            InternalDebug.Assert(this.CurrentNode.IsText);
            InternalDebug.Assert(this.CurrentNode.BeginTextPosition <= beginTextPosition &&
                                beginTextPosition <= endTextPosition &&
                                endTextPosition <= this.CurrentNode.EndTextPosition);

            if (beginTextPosition != endTextPosition)
            {
                TextRun run = this.FormatStore.GetTextRun(beginTextPosition);

                do
                {
                    int effectiveLength = run.EffectiveLength;
                    InternalDebug.Assert(effectiveLength > 0 || run.Type == TextRunType.Invalid);

                    switch (run.Type)
                    {
                        case TextRunType.NewLine:

                            while (0 != effectiveLength--)
                            {
                                if (this.writer.LiteralWhitespaceNesting == 0)
                                {
                                    this.writer.WriteStartTag(HtmlNameIndex.BR);
                                }
                                this.writer.WriteNewLine(false);
                            }
                            break;

                        case TextRunType.Tabulation:

                            this.writer.WriteTabulation(effectiveLength);
                            break;

                        case TextRunType.Space:

                            this.writer.WriteSpace(effectiveLength);
                            break;

                        case TextRunType.NbSp:

                            this.writer.WriteNbsp(effectiveLength);
                            break;

                        case TextRunType.NonSpace:

                            int start = 0;

                            if (this.recognizeHyperlinks && this.hyperlinkLevel == 0 && effectiveLength > 10 && effectiveLength < MaxRecognizedHyperlinkLength)
                            {
                                

                                bool addHttpPrefix;
                                bool addFilePrefix;
                                int offset;             
                                int length;             

                                bool link = RecognizeHyperLink(run, out offset, out length, out addFilePrefix, out addHttpPrefix);

                                if (link)
                                {
                                    if (offset != 0)
                                    {
                                        
                                        this.writer.WriteTextInternal(this.scratchBuffer.Buffer, 0, offset);
                                    }

                                    if (this.callback != null)
                                    {
                                        if (this.callbackContext == null)
                                        {
                                            this.callbackContext = new HtmlFormatOutputCallbackContext(this);
                                        }

                                        
                                        this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);

                                        
                                        string href = new string(this.scratchBuffer.Buffer, offset, length);
                                        if (addHttpPrefix)
                                        {
                                            href = "http://" + href;
                                        }
                                        else if (addFilePrefix)
                                        {
                                            href = "file://" + href;
                                        }

                                        this.callbackContext.AddAttribute(HtmlNameIndex.Href, href);

                                        this.callbackContext.InitializeFragment(false);

                                        this.callback(this.callbackContext, this.writer);

                                        this.callbackContext.UninitializeFragment();

                                        if (this.writer.IsTagOpen)
                                        {
                                            this.writer.WriteTagEnd();
                                        }

                                        if (!this.callbackContext.IsDeleteInnerContent)
                                        {
                                            this.writer.WriteTextInternal(this.scratchBuffer.Buffer, offset, length);
                                        }

                                        if (this.callbackContext.IsInvokeCallbackForEndTag)
                                        {
                                            
                                            

                                            
                                            this.callbackContext.InitializeTag(true, HtmlNameIndex.A, this.callbackContext.IsDeleteEndTag);

                                            this.callbackContext.InitializeFragment(false);

                                            this.callback(this.callbackContext, this.writer);

                                            this.callbackContext.UninitializeFragment();
                                        }
                                        else if (!this.callbackContext.IsDeleteEndTag)
                                        {
                                            this.writer.WriteEndTag(HtmlNameIndex.A);
                                        }

                                        if (this.writer.IsTagOpen)
                                        {
                                            this.writer.WriteTagEnd();
                                        }
                                    }
                                    else
                                    {
                                        this.writer.WriteStartTag(HtmlNameIndex.A);

                                        
                                        this.writer.WriteAttributeName(HtmlNameIndex.Href);
                                        if (addHttpPrefix)
                                        {
                                            this.writer.WriteAttributeValue("http://");
                                        }
                                        else if (addFilePrefix)
                                        {
                                            this.writer.WriteAttributeValue("file://");
                                        }

                                        this.writer.WriteAttributeValue(this.scratchBuffer.Buffer, offset, length);
                                        this.writer.WriteTagEnd();

                                        this.writer.WriteTextInternal(this.scratchBuffer.Buffer, offset, length);

                                        this.writer.WriteEndTag(HtmlNameIndex.A);
                                    }

                                    start += offset + length;

                                    if (start == effectiveLength)
                                    {
                                        
                                        run.MoveNext();
                                        continue;
                                    }

                                    
                                }

                                
                            }

                            

                            do
                            {
                                char[] buffer;
                                int offset;
                                int count;

                                run.GetChunk(start, out buffer, out offset, out count);
                                this.writer.WriteTextInternal(buffer, offset, count);
                                start += count;
                            }
                            while (start != effectiveLength);

                            break;
                    }

                    run.MoveNext();
                }
                while (run.Position < endTextPosition);
            }

            return true;
        }

        

        protected override void EndText()
        {
            this.writer.EndTextChunk();

            this.RevertCharFormat();
        }

        

        protected override bool StartBlockContainer()
        {
            this.writer.WriteNewLine(true);

            PropertyValue preformatted = this.GetDistinctProperty(PropertyId.Preformatted);
            PropertyValue quotingLevel = this.GetDistinctProperty(PropertyId.QuotingLevelDelta);

            if (!preformatted.IsNull && preformatted.Bool)
            {
                FormatStyle defaultStyle = this.FormatStore.GetStyle(HtmlConverterData.DefaultStyle.Pre);
                
                this.SubtractDefaultContainerPropertiesFromDistinct(FlagProperties.AllOff, defaultStyle.PropertyList);

                this.writer.WriteStartTag(HtmlNameIndex.Pre);
            }
            else if (!quotingLevel.IsNull && quotingLevel.Integer != 0)
            {
                for (int i = 0; i < quotingLevel.Integer; i++)
                {
                    this.writer.WriteStartTag(HtmlNameIndex.Div);

                    
                    this.writer.WriteAttribute(HtmlNameIndex.Class, "EmailQuote");
                }
            }
            else
            {
                
                
                if (this.SourceFormat == SourceFormat.Text)
                {
                    this.ApplyCharFormat();
                }

                this.writer.WriteStartTag(HtmlNameIndex.Div);

                if (this.SourceFormat == SourceFormat.Text)
                {
                    
                    this.writer.WriteAttribute(HtmlNameIndex.Class, "PlainText");
                }
            }

            this.OutputBlockTagAttributes();

            bool styleAttributeOpen = false;

            this.OutputBlockCssProperties(ref styleAttributeOpen);

            if (this.SourceFormat != SourceFormat.Text)
            {
                this.ApplyCharFormat();
            }

            if (this.CurrentNode.FirstChild.IsNull)
            {
                
                this.writer.WriteText('\xA0');
            }
            else if (this.CurrentNode.FirstChild == this.CurrentNode.LastChild && this.CurrentNode.FirstChild.NodeType == FormatContainerType.Text)
            {
                FormatNode child = this.CurrentNode.FirstChild;
                if (child.BeginTextPosition + 1 == child.EndTextPosition)
                {
                    TextRun run = this.FormatStore.GetTextRun(child.BeginTextPosition);

                    InternalDebug.Assert(run.Length == 1);

                    if (run.Type == TextRunType.Space)
                    {
                        
                        this.writer.WriteText('\xA0');
                        EndBlockContainer();
                        return false;
                    }
                }
            }

            return true;
        }

        

        protected override void EndBlockContainer()
        {
            PropertyValue preformatted = this.GetDistinctProperty(PropertyId.Preformatted);
            PropertyValue quotingLevel = this.GetDistinctProperty(PropertyId.QuotingLevelDelta);

            if (this.SourceFormat != SourceFormat.Text)
            {
                this.RevertCharFormat();
            }

            if (!preformatted.IsNull && preformatted.Bool)
            {
                this.writer.WriteEndTag(HtmlNameIndex.Pre);
            }
            else if (!quotingLevel.IsNull && quotingLevel.Integer != 0)
            {
                for (int i = 0; i < quotingLevel.Integer; i++)
                {
                    this.writer.WriteEndTag(HtmlNameIndex.Div);
                }
            }
            else
            {
                this.writer.WriteEndTag(HtmlNameIndex.Div);

                if (this.SourceFormat == SourceFormat.Text)
                {
                    this.RevertCharFormat();
                }
            }

            this.writer.WriteNewLine(true);
        }

        

        protected override bool StartInlineContainer()
        {
            return true;
        }

        

        protected override void EndInlineContainer()
        {
        }

        

        private void ApplyCharFormat()
        {
            BufferString val;

            this.scratchBuffer.Reset();

            FlagProperties flags = this.GetDistinctFlags();

            PropertyValue fontSizeValue = this.GetDistinctProperty(PropertyId.FontSize);
            if (!fontSizeValue.IsNull && !fontSizeValue.IsHtmlFontUnits && !fontSizeValue.IsRelativeHtmlFontUnits)
            {
                this.scratchBuffer.Append("font-size:");
                HtmlSupport.AppendCssFontSize(ref this.scratchBuffer, fontSizeValue);
                this.scratchBuffer.Append(';');
            }

            PropertyValue backColorValue = this.GetDistinctProperty(PropertyId.BackColor);
            if (backColorValue.IsColor)
            {
                this.scratchBuffer.Append("background-color:");
                HtmlSupport.AppendColor(ref this.scratchBuffer, backColorValue);
                this.scratchBuffer.Append(';');
            }

            Microsoft.Exchange.Data.Globalization.Culture culture = null;

            PropertyValue languageValue = this.GetDistinctProperty(PropertyId.Language);
            if (languageValue.IsInteger)
            {
                if (!Microsoft.Exchange.Data.Globalization.Culture.TryGetCulture(languageValue.Integer, out culture) || String.IsNullOrEmpty(culture.Name))
                {
                    culture = null;
                }
            }

            if (0 == (this.CurrentNode.NodeType & FormatContainerType.BlockFlag))
            {
                PropertyValue displayValue = this.GetDistinctProperty(PropertyId.Display);
                PropertyValue unicodeBiDiValue = this.GetDistinctProperty(PropertyId.UnicodeBiDi);

                if (!displayValue.IsNull)
                {
                    string str = HtmlSupport.GetDisplayString(displayValue);
                    if (str != null)
                    {
                        this.scratchBuffer.Append("display:");
                        this.scratchBuffer.Append(str);
                        this.scratchBuffer.Append(";");
                    }
                }

                if (flags.IsDefined(PropertyId.Visible))
                {
                    this.scratchBuffer.Append(flags.IsOn(PropertyId.Visible) ? "visibility:visible;" : "visibility:hidden;");
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    string str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        this.scratchBuffer.Append("unicode-bidi:");
                        this.scratchBuffer.Append(str);
                        this.scratchBuffer.Append(";");
                    }
                }
            }

            if (flags.IsDefinedAndOff(PropertyId.Bold))
            {
                
                this.scratchBuffer.Append("font-weight:normal;");
            }

            if (flags.IsDefined(PropertyId.SmallCaps))
            {
                this.scratchBuffer.Append(flags.IsOn(PropertyId.SmallCaps) ? "font-variant:small-caps;" : "font-variant:normal;");
            }

            if (flags.IsDefined(PropertyId.Capitalize))
            {
                this.scratchBuffer.Append(flags.IsOn(PropertyId.Capitalize) ? "text-transform:uppercase;" : "text-transform:none;");
            }

            
            

            PropertyValue fontFaceValue = this.GetDistinctProperty(PropertyId.FontFace);
            PropertyValue fontColorValue = this.GetDistinctProperty(PropertyId.FontColor);

            if (!fontFaceValue.IsNull || !fontSizeValue.IsNull || !fontColorValue.IsNull)
            {
                this.writer.WriteStartTag(HtmlNameIndex.Font);

                if (!fontFaceValue.IsNull)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Face);

                    string name;

                    StringValue sv;
                    MultiValue mv;

                    if (fontFaceValue.IsMultiValue)
                    {
                        mv = this.FormatStore.GetMultiValue(fontFaceValue);

                        for (int i = 0; i < mv.Length; i++)
                        {
                            sv = mv.GetStringValue(i);
                            name = sv.GetString();
                            if (i != 0)
                            {
                                this.writer.WriteAttributeValue(",");
                            }
                            this.writer.WriteAttributeValue(name);
                        }
                    }
                    else
                    {
                        sv = this.FormatStore.GetStringValue(fontFaceValue);
                        name = sv.GetString();
                        this.writer.WriteAttributeValue(name);
                    }
                }

                if (!fontSizeValue.IsNull)
                {
                    val = HtmlSupport.FormatFontSize(ref this.scratchValueBuffer, fontSizeValue);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Size, val);
                    }
                }

                if (!fontColorValue.IsNull)
                {
                    val = HtmlSupport.FormatColor(ref this.scratchValueBuffer, fontColorValue);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttribute(HtmlNameIndex.Color, val);
                    }
                }
            }

            if (this.scratchBuffer.Length != 0 || flags.IsDefined(PropertyId.RightToLeft) || culture != null)
            {
                this.writer.WriteStartTag(HtmlNameIndex.Span);

                if (this.scratchBuffer.Length != 0)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    this.writer.WriteAttributeValue(this.scratchBuffer.BufferString);
                }

                if (flags.IsDefined(PropertyId.RightToLeft))
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Dir);
                    this.writer.WriteAttributeValue(flags.IsOn(PropertyId.RightToLeft) ? "rtl" : "ltr");
                }

                if (culture != null)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Lang);
                    this.writer.WriteAttributeValue(culture.Name);
                }
            }

            if (flags.IsDefinedAndOn(PropertyId.Bold))
            {
                this.writer.WriteStartTag(HtmlNameIndex.B);
            }

            if (flags.IsDefinedAndOn(PropertyId.Italic))
            {
                this.writer.WriteStartTag(HtmlNameIndex.I);
            }

            if (flags.IsDefinedAndOn(PropertyId.Underline))
            {
                this.writer.WriteStartTag(HtmlNameIndex.U);
            }

            if (flags.IsDefinedAndOn(PropertyId.Subscript))
            {
                this.writer.WriteStartTag(HtmlNameIndex.Sub);
            }

            if (flags.IsDefinedAndOn(PropertyId.Superscript))
            {
                this.writer.WriteStartTag(HtmlNameIndex.Sup);
            }

            if (flags.IsDefinedAndOn(PropertyId.Strikethrough))
            {
                this.writer.WriteStartTag(HtmlNameIndex.Strike);
            }
        }

        

        private void RevertCharFormat()
        {
            FlagProperties flags = this.GetDistinctFlags();

            bool closeFontTag = false;
            bool closeSpanTag = false;

            PropertyValue fontSizeValue = this.GetDistinctProperty(PropertyId.FontSize);
            if (!fontSizeValue.IsNull && !fontSizeValue.IsHtmlFontUnits && !fontSizeValue.IsRelativeHtmlFontUnits)
            {
                closeSpanTag = true;
            }

            PropertyValue backColorValue = this.GetDistinctProperty(PropertyId.BackColor);
            if (backColorValue.IsColor)
            {
                closeSpanTag = true;
            }

            Microsoft.Exchange.Data.Globalization.Culture culture = null;

            PropertyValue languageValue = this.GetDistinctProperty(PropertyId.Language);
            if (languageValue.IsInteger)
            {
                if (Microsoft.Exchange.Data.Globalization.Culture.TryGetCulture(languageValue.Integer, out culture) && !String.IsNullOrEmpty(culture.Name))
                {
                    closeSpanTag = true;
                }
            }

            if (0 == (this.CurrentNode.NodeType & FormatContainerType.BlockFlag))
            {
                PropertyValue displayValue = this.GetDistinctProperty(PropertyId.Display);
                PropertyValue unicodeBiDiValue = this.GetDistinctProperty(PropertyId.UnicodeBiDi);

                if (!displayValue.IsNull)
                {
                    string str = HtmlSupport.GetDisplayString(displayValue);
                    if (str != null)
                    {
                        closeSpanTag = true;
                    }
                }

                if (flags.IsDefined(PropertyId.Visible))
                {
                    closeSpanTag = true;
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    string str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        closeSpanTag = true;
                    }
                }
            }

            if (flags.IsDefinedAndOff(PropertyId.Bold))
            {
                
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.SmallCaps))
            {
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.Capitalize))
            {
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.RightToLeft))
            {
                closeSpanTag = true;
            }

            PropertyValue fontFaceValue = this.GetDistinctProperty(PropertyId.FontFace);
            PropertyValue fontColorValue = this.GetDistinctProperty(PropertyId.FontColor);

            if (!fontFaceValue.IsNull || !fontSizeValue.IsNull || !fontColorValue.IsNull)
            {
                closeFontTag = true;
            }

            
            

            if (flags.IsDefinedAndOn(PropertyId.Strikethrough))
            {
                this.writer.WriteEndTag(HtmlNameIndex.Strike);
            }

            if (flags.IsDefinedAndOn(PropertyId.Superscript))
            {
                this.writer.WriteEndTag(HtmlNameIndex.Sup);
            }

            if (flags.IsDefinedAndOn(PropertyId.Subscript))
            {
                this.writer.WriteEndTag(HtmlNameIndex.Sub);
            }

            if (flags.IsDefinedAndOn(PropertyId.Underline))
            {
                this.writer.WriteEndTag(HtmlNameIndex.U);
            }

            if (flags.IsDefinedAndOn(PropertyId.Italic))
            {
                this.writer.WriteEndTag(HtmlNameIndex.I);
            }

            if (flags.IsDefinedAndOn(PropertyId.Bold))
            {
                this.writer.WriteEndTag(HtmlNameIndex.B);
            }

            if (closeSpanTag)
            {
                this.writer.WriteEndTag(HtmlNameIndex.Span);
            }

            if (closeFontTag)
            {
                this.writer.WriteEndTag(HtmlNameIndex.Font);
            }
        }

        

        private void OutputBlockCssProperties(ref bool styleAttributeOpen)
        {
            PropertyValue display = this.GetDistinctProperty(PropertyId.Display);
            PropertyValue visible = this.GetDistinctProperty(PropertyId.Visible);

            PropertyValue height = this.GetDistinctProperty(PropertyId.Height);
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);

            PropertyValue unicodeBiDiValue = this.GetDistinctProperty(PropertyId.UnicodeBiDi);

            PropertyValue firstLineIndent = this.GetDistinctProperty(PropertyId.FirstLineIndent);
            PropertyValue textAlignment = this.GetDistinctProperty(PropertyId.TextAlignment);
            PropertyValue backColor = this.GetDistinctProperty(PropertyId.BackColor);
 
            PropertyValue topMargin = this.GetDistinctProperty(PropertyId.TopMargin);
            PropertyValue rightMargin = this.GetDistinctProperty(PropertyId.RightMargin);
            PropertyValue bottomMargin = this.GetDistinctProperty(PropertyId.BottomMargin);
            PropertyValue leftMargin = this.GetDistinctProperty(PropertyId.LeftMargin);
            PropertyValue topPadding = this.GetDistinctProperty(PropertyId.TopPadding);
            PropertyValue rightPadding = this.GetDistinctProperty(PropertyId.RightPadding);
            PropertyValue bottomPadding = this.GetDistinctProperty(PropertyId.BottomPadding);
            PropertyValue leftPadding = this.GetDistinctProperty(PropertyId.LeftPadding);

            PropertyValue topBorderWidth = this.GetDistinctProperty(PropertyId.TopBorderWidth);
            PropertyValue rightBorderWidth = this.GetDistinctProperty(PropertyId.RightBorderWidth);
            PropertyValue bottomBorderWidth = this.GetDistinctProperty(PropertyId.BottomBorderWidth);
            PropertyValue leftBorderWidth = this.GetDistinctProperty(PropertyId.LeftBorderWidth);
            PropertyValue topBorderStyle = this.GetDistinctProperty(PropertyId.TopBorderStyle);
            PropertyValue rightBorderStyle = this.GetDistinctProperty(PropertyId.RightBorderStyle);
            PropertyValue bottomBorderStyle = this.GetDistinctProperty(PropertyId.BottomBorderStyle);
            PropertyValue leftBorderStyle = this.GetDistinctProperty(PropertyId.LeftBorderStyle);
            PropertyValue topBorderColor = this.GetDistinctProperty(PropertyId.TopBorderColor);
            PropertyValue rightBorderColor = this.GetDistinctProperty(PropertyId.RightBorderColor);
            PropertyValue bottomBorderColor = this.GetDistinctProperty(PropertyId.BottomBorderColor);
            PropertyValue leftBorderColor = this.GetDistinctProperty(PropertyId.LeftBorderColor);

            if (!visible.IsNull ||
                !display.IsNull ||
                !unicodeBiDiValue.IsNull ||
                !width.IsNull ||
                !height.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!display.IsNull)
                {
                    string val = HtmlSupport.GetDisplayString(display);
                    if (val != null)
                    {
                        this.scratchBuffer.Append("display:");
                        this.scratchBuffer.Append(val);
                        this.scratchBuffer.Append(";");
                    }
                }

                if (!visible.IsNull)
                {
                    this.scratchBuffer.Append(visible.Bool ? "visibility:visible;" : "visibility:hidden;");
                }

                if (!width.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("width:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }

                if (!height.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, height);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("height:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    string str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        this.writer.WriteAttributeValue("unicode-bidi:");
                        this.writer.WriteAttributeValue(str);
                        this.writer.WriteAttributeValue(";");
                    }
                }
            }

            if (!firstLineIndent.IsNull ||
                !textAlignment.IsNull ||
                !backColor.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!firstLineIndent.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, firstLineIndent);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("text-indent:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }

                if (!textAlignment.IsNull)
                {
                    if (textAlignment.IsEnum && textAlignment.Enum < HtmlSupport.TextAlignmentEnumeration.Length)
                    {
                        this.writer.WriteAttributeValue("text-align:");
                        this.writer.WriteAttributeValue(HtmlSupport.TextAlignmentEnumeration[textAlignment.Enum].name);
                        this.writer.WriteAttributeValue(";");
                    }
                }

                if (!backColor.IsNull)
                {
                    BufferString val = HtmlSupport.FormatColor(ref this.scratchBuffer, backColor);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("background-color:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }
            }

            if (!topMargin.IsNull ||
                !rightMargin.IsNull ||
                !bottomMargin.IsNull ||
                !leftMargin.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                this.OutputMarginAndPaddingProperties(
                        "margin",
                        topMargin, rightMargin, bottomMargin, leftMargin);
            }

            if (!topPadding.IsNull ||
                !rightPadding.IsNull ||
                !bottomPadding.IsNull ||
                !leftPadding.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                this.OutputMarginAndPaddingProperties(
                        "padding",
                        topPadding, rightPadding, bottomPadding, leftPadding);
            }

            if (!topBorderWidth.IsNull ||
                !rightBorderWidth.IsNull ||
                !bottomBorderWidth.IsNull ||
                !leftBorderWidth.IsNull ||
                !topBorderStyle.IsNull ||
                !rightBorderStyle.IsNull ||
                !bottomBorderStyle.IsNull ||
                !leftBorderStyle.IsNull ||
                !topBorderColor.IsNull ||
                !rightBorderColor.IsNull ||
                !bottomBorderColor.IsNull ||
                !leftBorderColor.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                this.OutputBorderProperties(
                        topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth,
                        topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle,
                        topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
            }
        }

        

        private void OutputMarginAndPaddingProperties(
                            string name,    
                            PropertyValue topValue, PropertyValue rightValue, PropertyValue bottomValue, PropertyValue leftValue)
        {
            int numSidesDefined = 0;

            if (!topValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!rightValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!bottomValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!leftValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (numSidesDefined == 4)
            {
                
                this.writer.WriteAttributeValue(name);
                this.writer.WriteAttributeValue(":");

                if (topValue == rightValue && topValue == bottomValue && topValue == leftValue)
                {
                    
                    this.OutputLengthPropertyValue(topValue);
                }
                else
                {
                    if (topValue == bottomValue && rightValue == leftValue)
                    {
                        
                        this.OutputCompositeLengthPropertyValue(topValue, rightValue);
                    }
                    else
                    {
                        
                        this.OutputCompositeLengthPropertyValue(topValue, rightValue, bottomValue, leftValue);
                    }
                }

                this.writer.WriteAttributeValue(";");
            }
            else
            {
                

                if (!topValue.IsNull)
                {
                    
                    this.writer.WriteAttributeValue(name);
                    this.writer.WriteAttributeValue("-top:");
                    this.OutputLengthPropertyValue(topValue);
                    this.writer.WriteAttributeValue(";");
                }

                if (!rightValue.IsNull)
                {
                    
                    this.writer.WriteAttributeValue(name);
                    this.writer.WriteAttributeValue("-right:");
                    this.OutputLengthPropertyValue(rightValue);
                    this.writer.WriteAttributeValue(";");
                }

                if (!bottomValue.IsNull)
                {
                    
                    this.writer.WriteAttributeValue(name);
                    this.writer.WriteAttributeValue("-bottom:");
                    this.OutputLengthPropertyValue(bottomValue);
                    this.writer.WriteAttributeValue(";");
                }

                if (!leftValue.IsNull)
                {
                    
                    this.writer.WriteAttributeValue(name);
                    this.writer.WriteAttributeValue("-left:");
                    this.OutputLengthPropertyValue(leftValue);
                    this.writer.WriteAttributeValue(";");
                }
            }
        }

        

        private void OutputBorderProperties(
                            PropertyValue topBorderWidth, PropertyValue rightBorderWidth, PropertyValue bottomBorderWidth, PropertyValue leftBorderWidth,
                            PropertyValue topBorderStyle, PropertyValue rightBorderStyle, PropertyValue bottomBorderStyle, PropertyValue leftBorderStyle,
                            PropertyValue topBorderColor, PropertyValue rightBorderColor, PropertyValue bottomBorderColor, PropertyValue leftBorderColor)
        {
            int numSidesDefinedWidth = 0;
            int numSidesDefinedStyle = 0;
            int numSidesDefinedColor = 0;

            int numPropsDefinedTop = 0;
            int numPropsDefinedRight = 0;
            int numPropsDefinedBottom = 0;
            int numPropsDefinedLeft = 0;

            if (!topBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedLeft ++;
            }

            if (!topBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedLeft ++;
            }

            if (!topBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedLeft ++;
            }

            bool allSidesEqualWidth = false;
            bool hvSidesEqualWidth = false;
            bool allSidesEqualStyle = false;
            bool hvSidesEqualStyle = false;
            bool allSidesEqualColor = false;
            bool hvSidesEqualColor = false;

            if (numSidesDefinedWidth == 4)
            {
                if (topBorderWidth == bottomBorderWidth && rightBorderWidth == leftBorderWidth)
                {
                    hvSidesEqualWidth = true;
                    allSidesEqualWidth = (topBorderWidth == rightBorderWidth);
                }
            }

            if (numSidesDefinedStyle == 4)
            {
                if (topBorderStyle == bottomBorderStyle && rightBorderStyle == leftBorderStyle)
                {
                    hvSidesEqualStyle = true;
                    allSidesEqualStyle = (topBorderStyle == rightBorderStyle);
                }
            }

            if (numSidesDefinedColor == 4)
            {
                if (topBorderColor == bottomBorderColor && rightBorderColor == leftBorderColor)
                {
                    hvSidesEqualColor = true;
                    allSidesEqualColor = (topBorderColor == rightBorderColor);
                }
            }

            if (numSidesDefinedWidth == 4 && numSidesDefinedStyle == 4 && numSidesDefinedColor == 4)
            {
                
                if (allSidesEqualWidth && allSidesEqualStyle && allSidesEqualColor)
                {
                    

                    this.writer.WriteAttributeValue("border:");
                    this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
                    this.writer.WriteAttributeValue(";");
                }
                else
                {
                    
                    

                    this.writer.WriteAttributeValue("border-width:");
                    if (allSidesEqualWidth)
                    {
                        this.OutputBorderWidthPropertyValue(topBorderWidth);
                    }
                    else if (hvSidesEqualWidth)
                    {
                        this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
                    }
                    else
                    {
                        this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
                    }

                    this.writer.WriteAttributeValue(";");

                    this.writer.WriteAttributeValue("border-style:");
                    if (allSidesEqualStyle)
                    {
                        this.OutputBorderStylePropertyValue(topBorderStyle);
                    }
                    else if (hvSidesEqualStyle)
                    {
                        this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
                    }
                    else
                    {
                        this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
                    }
                    this.writer.WriteAttributeValue(";");

                    this.writer.WriteAttributeValue("border-color:");
                    if (allSidesEqualColor)
                    {
                        this.OutputBorderColorPropertyValue(topBorderColor);
                    }
                    else if (hvSidesEqualColor)
                    {
                        this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
                    }
                    else
                    {
                        this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
                    }
                    this.writer.WriteAttributeValue(";");
                }
            }
            else
            {
                

                bool topWidthOutput = false, rightWidthOutput = false, bottomWidthOutput = false, leftWidthOutput = false;
                bool topStyleOutput = false, rightStyleOutput = false, bottomStyleOutput = false, leftStyleOutput = false;
                bool topColorOutput = false, rightColorOutput = false, bottomColorOutput = false, leftColorOutput = false;

                if (numSidesDefinedWidth == 4 || numSidesDefinedStyle == 4 || numSidesDefinedColor == 4)
                {
                    

                    if (numSidesDefinedWidth == 4)
                    {
                        this.writer.WriteAttributeValue("border-width:");
                        if (allSidesEqualWidth)
                        {
                            this.OutputBorderWidthPropertyValue(topBorderWidth);
                        }
                        else if (hvSidesEqualWidth)
                        {
                            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
                        }
                        else
                        {
                            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
                        }
                        this.writer.WriteAttributeValue(";");

                        topWidthOutput = true;
                        rightWidthOutput = true;
                        bottomWidthOutput = true;
                        leftWidthOutput = true;
                    }

                    if (numSidesDefinedStyle == 4)
                    {
                        this.writer.WriteAttributeValue("border-style:");
                        if (allSidesEqualStyle)
                        {
                            this.OutputBorderStylePropertyValue(topBorderStyle);
                        }
                        else if (hvSidesEqualStyle)
                        {
                            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
                        }
                        else
                        {
                            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
                        }
                        this.writer.WriteAttributeValue(";");

                        topStyleOutput = true;
                        rightStyleOutput = true;
                        bottomStyleOutput = true;
                        leftStyleOutput = true;
                    }

                    if (numSidesDefinedColor == 4)
                    {
                        this.writer.WriteAttributeValue("border-color:");
                        if (allSidesEqualColor)
                        {
                            this.OutputBorderColorPropertyValue(topBorderColor);
                        }
                        else if (hvSidesEqualColor)
                        {
                            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
                        }
                        else
                        {
                            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
                        }
                        this.writer.WriteAttributeValue(";");

                        topColorOutput = true;
                        rightColorOutput = true;
                        bottomColorOutput = true;
                        leftColorOutput = true;
                    }
                }
                else if (numPropsDefinedTop == 3 || numPropsDefinedRight == 3 || numPropsDefinedBottom == 3 || numPropsDefinedLeft == 3)
                {
                    

                    if (numPropsDefinedTop == 3)
                    {
                        this.writer.WriteAttributeValue("border-top:");
                        this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
                        this.writer.WriteAttributeValue(";");

                        topWidthOutput = true;
                        topStyleOutput = true;
                        topColorOutput = true;
                    }

                    if (numPropsDefinedRight == 3)
                    {
                        this.writer.WriteAttributeValue("border-right:");
                        this.OutputCompositeBorderSidePropertyValue(rightBorderWidth, rightBorderStyle, rightBorderColor);
                        this.writer.WriteAttributeValue(";");

                        rightWidthOutput = true;
                        rightStyleOutput = true;
                        rightColorOutput = true;
                    }

                    if (numPropsDefinedBottom == 3)
                    {
                        this.writer.WriteAttributeValue("border-bottom:");
                        this.OutputCompositeBorderSidePropertyValue(bottomBorderWidth, bottomBorderStyle, bottomBorderColor);
                        this.writer.WriteAttributeValue(";");

                        bottomWidthOutput = true;
                        bottomStyleOutput = true;
                        bottomColorOutput = true;
                    }

                    if (numPropsDefinedLeft == 3)
                    {
                        this.writer.WriteAttributeValue("border-left:");
                        this.OutputCompositeBorderSidePropertyValue(leftBorderWidth, leftBorderStyle, leftBorderColor);
                        this.writer.WriteAttributeValue(";");

                        leftWidthOutput = true;
                        leftStyleOutput = true;
                        leftColorOutput = true;
                    }
                }

                

                if (!topWidthOutput && !topBorderWidth.IsNull)
                {
                    this.writer.WriteAttributeValue("border-top-width:");
                    this.OutputBorderWidthPropertyValue(topBorderWidth);
                    this.writer.WriteAttributeValue(";");
                }

                if (!rightWidthOutput && !rightBorderWidth.IsNull)
                {
                    this.writer.WriteAttributeValue("border-right-width:");
                    this.OutputBorderWidthPropertyValue(rightBorderWidth);
                    this.writer.WriteAttributeValue(";");
                }

                if (!bottomWidthOutput && !bottomBorderWidth.IsNull)
                {
                    this.writer.WriteAttributeValue("border-bottom-width:");
                    this.OutputBorderWidthPropertyValue(bottomBorderWidth);
                    this.writer.WriteAttributeValue(";");
                }

                if (!leftWidthOutput && !leftBorderWidth.IsNull)
                {
                    this.writer.WriteAttributeValue("border-left-width:");
                    this.OutputBorderWidthPropertyValue(leftBorderWidth);
                    this.writer.WriteAttributeValue(";");
                }

                if (!topStyleOutput && !topBorderStyle.IsNull)
                {
                    this.writer.WriteAttributeValue("border-top-style:");
                    this.OutputBorderStylePropertyValue(topBorderStyle);
                    this.writer.WriteAttributeValue(";");
                }

                if (!rightStyleOutput && !rightBorderStyle.IsNull)
                {
                    this.writer.WriteAttributeValue("border-right-style:");
                    this.OutputBorderStylePropertyValue(rightBorderStyle);
                    this.writer.WriteAttributeValue(";");
                }

                if (!bottomStyleOutput && !bottomBorderStyle.IsNull)
                {
                    this.writer.WriteAttributeValue("border-bottom-style:");
                    this.OutputBorderStylePropertyValue(bottomBorderStyle);
                    this.writer.WriteAttributeValue(";");
                }

                if (!leftStyleOutput && !leftBorderStyle.IsNull)
                {
                    this.writer.WriteAttributeValue("border-left-style:");
                    this.OutputBorderStylePropertyValue(leftBorderStyle);
                    this.writer.WriteAttributeValue(";");
                }

                if (!topColorOutput && !topBorderColor.IsNull)
                {
                    this.writer.WriteAttributeValue("border-top-color:");
                    this.OutputBorderColorPropertyValue(topBorderColor);
                    this.writer.WriteAttributeValue(";");
                }

                if (!rightColorOutput && !rightBorderColor.IsNull)
                {
                    this.writer.WriteAttributeValue("border-right-color:");
                    this.OutputBorderColorPropertyValue(rightBorderColor);
                    this.writer.WriteAttributeValue(";");
                }

                if (!bottomColorOutput && !bottomBorderColor.IsNull)
                {
                    this.writer.WriteAttributeValue("border-bottom-color:");
                    this.OutputBorderColorPropertyValue(bottomBorderColor);
                    this.writer.WriteAttributeValue(";");
                }

                if (!leftColorOutput && !leftBorderColor.IsNull)
                {
                    this.writer.WriteAttributeValue("border-left-color:");
                    this.OutputBorderColorPropertyValue(leftBorderColor);
                    this.writer.WriteAttributeValue(";");
                }
            }
        }

        private void OutputCompositeBorderSidePropertyValue(PropertyValue width, PropertyValue style, PropertyValue color)
        {
            this.OutputBorderWidthPropertyValue(width);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderStylePropertyValue(style);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderColorPropertyValue(color);
        }

        private void OutputCompositeLengthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            this.OutputLengthPropertyValue(topBottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputLengthPropertyValue(rightLeft);
        }

        private void OutputCompositeLengthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            this.OutputLengthPropertyValue(top);
            this.writer.WriteAttributeValue(" ");
            this.OutputLengthPropertyValue(right);
            this.writer.WriteAttributeValue(" ");
            this.OutputLengthPropertyValue(bottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputLengthPropertyValue(left);
        }

        private void OutputCompositeBorderWidthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            this.OutputBorderWidthPropertyValue(topBottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderWidthPropertyValue(rightLeft);
        }

        private void OutputCompositeBorderWidthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            this.OutputBorderWidthPropertyValue(top);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderWidthPropertyValue(right);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderWidthPropertyValue(bottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderWidthPropertyValue(left);
        }

        private void OutputCompositeBorderStylePropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            this.OutputBorderStylePropertyValue(topBottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderStylePropertyValue(rightLeft);
        }

        private void OutputCompositeBorderStylePropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            this.OutputBorderStylePropertyValue(top);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderStylePropertyValue(right);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderStylePropertyValue(bottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderStylePropertyValue(left);
        }

        private void OutputCompositeBorderColorPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            this.OutputBorderColorPropertyValue(topBottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderColorPropertyValue(rightLeft);
        }

        private void OutputCompositeBorderColorPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            this.OutputBorderColorPropertyValue(top);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderColorPropertyValue(right);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderColorPropertyValue(bottom);
            this.writer.WriteAttributeValue(" ");
            this.OutputBorderColorPropertyValue(left);
        }

        private void OutputLengthPropertyValue(PropertyValue width)
        {
            BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                this.writer.WriteAttributeValue(val);
            }
            else
            {
                this.writer.WriteAttributeValue("0");
            }
        }

        private void OutputBorderWidthPropertyValue(PropertyValue width)
        {
            BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                this.writer.WriteAttributeValue(val);
            }
            else
            {
                this.writer.WriteAttributeValue("medium");
            }
        }

        private void OutputBorderStylePropertyValue(PropertyValue style)
        {
            InternalDebug.Assert(!style.IsNull);

            string str = HtmlSupport.GetBorderStyleString(style);
            InternalDebug.Assert(str != null);
            if (str != null)
            {
                this.writer.WriteAttributeValue(str);
            }
            else
            {
                this.writer.WriteAttributeValue("solid");
            }
        }

        private void OutputBorderColorPropertyValue(PropertyValue color)
        {
            InternalDebug.Assert(!color.IsNull);

            BufferString val = HtmlSupport.FormatColor(ref this.scratchBuffer, color);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                this.writer.WriteAttributeValue(val);
            }
            else
            {
                this.writer.WriteAttributeValue("black");
            }
        }

        

        private void OutputTableCssProperties(ref bool styleAttributeOpen)
        {
            PropertyValue layoutFixed = this.GetDistinctProperty(PropertyId.TableLayoutFixed);
            PropertyValue borderCollapse = this.GetDistinctProperty(PropertyId.TableBorderCollapse);
            PropertyValue showEmptyCells = this.GetDistinctProperty(PropertyId.TableShowEmptyCells);
            PropertyValue captionSideTop = this.GetDistinctProperty(PropertyId.TableCaptionSideTop);
            PropertyValue spacingVertical = this.GetDistinctProperty(PropertyId.TableBorderSpacingVertical);
            PropertyValue spacingHorizontal = this.GetDistinctProperty(PropertyId.TableBorderSpacingHorizontal);

            if (!layoutFixed.IsNull ||
                !borderCollapse.IsNull ||
                !showEmptyCells.IsNull ||
                !captionSideTop.IsNull ||
                !spacingVertical.IsNull ||
                !spacingHorizontal.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!layoutFixed.IsNull)
                {
                    this.writer.WriteAttributeValue("table-layout:");
                    this.writer.WriteAttributeValue(layoutFixed.Bool ? "fixed" : "auto");
                    this.writer.WriteAttributeValue(";");
                }

                if (!borderCollapse.IsNull)
                {
                    this.writer.WriteAttributeValue("border-collapse:");
                    this.writer.WriteAttributeValue(borderCollapse.Bool ? "collapse" : "separate");
                    this.writer.WriteAttributeValue(";");
                }

                if (!showEmptyCells.IsNull)
                {
                    this.writer.WriteAttributeValue("empty-cells:");
                    this.writer.WriteAttributeValue(showEmptyCells.Bool ? "show" : "hide");
                    this.writer.WriteAttributeValue(";");
                }

                if (!captionSideTop.IsNull)
                {
                    this.writer.WriteAttributeValue("caption-side:");
                    this.writer.WriteAttributeValue(captionSideTop.Bool ? "top" : "bottom");
                    this.writer.WriteAttributeValue(";");
                }

                if (!spacingVertical.IsNull && !spacingVertical.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, spacingVertical);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("border-spacing:");
                        this.writer.WriteAttributeValue(val);

                        if (spacingVertical != spacingHorizontal)
                        {
                            val = HtmlSupport.FormatLength(ref this.scratchBuffer, spacingHorizontal);
                            if (val.Length != 0)
                            {
                                this.writer.WriteAttributeValue(" ");
                                this.writer.WriteAttributeValue(val);
                            }
                        }
                        this.writer.WriteAttributeValue(";");
                    }
                }
            }
        }

        

        private void OutputTableColumnCssProperties(ref bool styleAttributeOpen)
        {
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            PropertyValue backColor = this.GetDistinctProperty(PropertyId.BackColor);

            if (!backColor.IsNull ||
                !width.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    this.writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!width.IsNull)
                {
                    BufferString val = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("width:");
                        this.writer.WriteAttributeValue(val);
                        this.writer.WriteAttributeValue(";");
                    }
                }

                if (!backColor.IsNull)
                {
                    BufferString val = HtmlSupport.FormatColor(ref this.scratchBuffer, backColor);
                    if (val.Length != 0)
                    {
                        this.writer.WriteAttributeValue("background-color:");
                        this.writer.WriteAttributeValue(val);
                    }
                }
            }
        }

        

        private void OutputBlockTagAttributes()
        {
            PropertyValue rtl = this.GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            PropertyValue textAlignment = this.GetDistinctProperty(PropertyId.TextAlignment);
            if (!textAlignment.IsNull)
            {
                string val = HtmlSupport.GetTextAlignmentString(textAlignment);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }
        }

        

        private void OutputTableTagAttributes()
        {
            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, width);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Width, val);
                }
            }

            PropertyValue align = this.GetDistinctProperty(PropertyId.HorizontalAlignment);
            if (!align.IsNull)
            {
                string val = HtmlSupport.GetHorizontalAlignmentString(align);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            PropertyValue rtl = this.GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            PropertyValue border = this.GetDistinctProperty(PropertyId.TableBorder);
            if (!border.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, border);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Border, val);
                }
            }

            PropertyValue frame = this.GetDistinctProperty(PropertyId.TableFrame);
            if (!frame.IsNull)
            {
                string val = HtmlSupport.GetTableFrameString(frame);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Frame, val);
                }
            }

            PropertyValue rules = this.GetDistinctProperty(PropertyId.TableRules);
            if (!rules.IsNull)
            {
                string val = HtmlSupport.GetTableRulesString(rules);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Rules, val);
                }
            }

            PropertyValue cellSpacing = this.GetDistinctProperty(PropertyId.TableCellSpacing);
            if (!cellSpacing.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, cellSpacing);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.CellSpacing, val);
                }
            }

            PropertyValue cellPadding = this.GetDistinctProperty(PropertyId.TableCellPadding);
            if (!cellPadding.IsNull)
            {
                BufferString val = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, cellPadding);
                if (val.Length != 0)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.CellPadding, val);
                }
            }

        }

        

        private void OutputTableCellTagAttributes()
        {
            PropertyValue colSpan = this.GetDistinctProperty(PropertyId.NumColumns);
            if (colSpan.IsInteger && colSpan.Integer != 1)
            {
                this.writer.WriteAttribute(HtmlNameIndex.ColSpan, colSpan.Integer.ToString());
            }

            PropertyValue rowSpan = this.GetDistinctProperty(PropertyId.NumRows);
            if (rowSpan.IsInteger && rowSpan.Integer != 1)
            {
                this.writer.WriteAttribute(HtmlNameIndex.RowSpan, rowSpan.Integer.ToString());
            }

            PropertyValue width = this.GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                this.writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            PropertyValue textAlignment = this.GetDistinctProperty(PropertyId.TextAlignment);
            if (!textAlignment.IsNull)
            {
                string val = HtmlSupport.GetTextAlignmentString(textAlignment);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            PropertyValue verticalAlignment = this.GetDistinctProperty(PropertyId.VerticalAlignment);
            if (!verticalAlignment.IsNull)
            {
                string val = HtmlSupport.GetVerticalAlignmentString(verticalAlignment);
                if (val != null)
                {
                    this.writer.WriteAttribute(HtmlNameIndex.Valign, val);
                }
            }

            PropertyValue noWrap = this.GetDistinctProperty(PropertyId.TableCellNoWrap);
            if (!noWrap.IsNull && noWrap.Bool)
            {
                this.writer.WriteAttribute(HtmlNameIndex.NoWrap, "");
            }
        }

        

        private bool RecognizeHyperLink(TextRun run, out int offset, out int length, out bool addFilePrefix, out bool addHttpPrefix)
        {
            
            
            

            
            
            

            this.scratchBuffer.Reset();

            run.AppendFragment(0, ref this.scratchBuffer, 10);

            offset = 0;
            length = 0;

            
            while (offset < 3 && (this.scratchBuffer[offset] == '<' || this.scratchBuffer[offset] == '\"' || this.scratchBuffer[offset] == '\'' ||
                this.scratchBuffer[offset] == '(' || this.scratchBuffer[offset] == '['))
            {
                offset ++;
            }

            
            
            
            
            
            

            bool link = false;
            addHttpPrefix = false;
            addFilePrefix = false;

            if (this.scratchBuffer[offset] == '\\')
            {
                if (this.scratchBuffer[offset + 1] == '\\' &&
                    Char.IsLetterOrDigit(this.scratchBuffer[offset + 2]))
                {
                    
                    link = true;
                    addFilePrefix = true;
                }
            }
            else if (this.scratchBuffer[offset] == 'h')
            {
                if (this.scratchBuffer[offset + 1] == 't' &&
                    this.scratchBuffer[offset + 2] == 't' &&
                    this.scratchBuffer[offset + 3] == 'p' &&
                        (this.scratchBuffer[offset + 4] == ':' ||
                        (this.scratchBuffer[offset + 4] == 's' && this.scratchBuffer[offset + 5] == ':')))
                {
                    
                    link = true;
                }
            }
            else if (this.scratchBuffer[offset] == 'f')
            {
                if (this.scratchBuffer[offset + 1] == 't' &&
                    this.scratchBuffer[offset + 2] == 'p' &&
                    this.scratchBuffer[offset + 3] == ':')
                {
                    
                    link = true;
                }
                else if (this.scratchBuffer[offset + 1] == 'i' &&
                    this.scratchBuffer[offset + 2] == 'l' &&
                    this.scratchBuffer[offset + 3] == 'e' &&
                    this.scratchBuffer[offset + 4] == ':' &&
                    this.scratchBuffer[offset + 5] == '/' &&
                    this.scratchBuffer[offset + 6] == '/')
                {
                    
                    
                    
                    link = true;
                }
            }
            else if (this.scratchBuffer[offset] == 'm')
            {
                if (this.scratchBuffer[offset + 1] == 'a' &&
                    this.scratchBuffer[offset + 2] == 'i' &&
                    this.scratchBuffer[offset + 3] == 'l' &&
                    this.scratchBuffer[offset + 4] == 't' &&
                    this.scratchBuffer[offset + 5] == 'o' &&
                    this.scratchBuffer[offset + 6] == ':')
                {
                    
                    link = true;
                }
            }
            else if (this.scratchBuffer[offset] == 'w')
            {
                if (this.scratchBuffer[offset + 1] == 'w' &&
                    this.scratchBuffer[offset + 2] == 'w' &&
                    this.scratchBuffer[offset + 3] == '.')
                {
                    
                    link = true;
                    addHttpPrefix = true;
                }
            }

            if (link)
            {
                int end = 10 + run.AppendFragment(10, ref this.scratchBuffer, MaxRecognizedHyperlinkLength - 10);

                
                while (this.scratchBuffer[end - 1] == '>' || this.scratchBuffer[end - 1] == '\"' || this.scratchBuffer[end - 1] == '\'' || 
                    this.scratchBuffer[end - 1] == ')' || this.scratchBuffer[end - 1] == ']' || 
                    this.scratchBuffer[end - 1] == '.' || this.scratchBuffer[end - 1] == ',' || this.scratchBuffer[end - 1] == ';')
                {
                    end --;
                }

                length = end - offset;
            }

            return link;
        }

        

        protected override void Dispose(bool disposing)
        {
            if (this.writer != null && this.writer is IDisposable)
            {
                ((IDisposable)this.writer).Dispose();
            }

            this.writer = null;

            base.Dispose(disposing);
        }
    }

    

    internal class HtmlFormatOutputCallbackContext : HtmlTagContext
    {
        private const int MaxCallbackAttributes = 10;

        private HtmlFormatOutput formatOutput;
        private int countAttributes;
        private AttributeDescriptor[] attributes = new AttributeDescriptor[MaxCallbackAttributes];

        private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteName | HtmlToken.AttrPartMinor.CompleteValue);
        private static readonly HtmlTagParts CompleteTagWithAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName | HtmlToken.TagPartMinor.Attributes);
        private static readonly HtmlTagParts CompleteTagWithoutAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName);

        private struct AttributeDescriptor
        {
            public HtmlNameIndex nameIndex;
            public string value;
            public int readIndex;
        }

        public HtmlFormatOutputCallbackContext(HtmlFormatOutput formatOutput)
        {
            this.formatOutput = formatOutput;
        }

        public new void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool tagDropped)
        {
            base.InitializeTag(isEndTag, tagNameIndex, tagDropped);
            this.countAttributes = 0;
        }

        internal void Reset()
        {
            this.countAttributes = 0;
        }

        internal void AddAttribute(HtmlNameIndex nameIndex, string value)
        {
            InternalDebug.Assert(this.countAttributes < MaxCallbackAttributes);

            this.attributes[this.countAttributes].nameIndex = nameIndex;
            this.attributes[this.countAttributes].value = value;
            this.attributes[this.countAttributes].readIndex = 0;
            this.countAttributes ++;
        }

        public void InitializeFragment(bool isEmptyElementTag)
        {
            base.InitializeFragment(isEmptyElementTag, this.countAttributes, this.countAttributes == 0 ? CompleteTagWithoutAttributesParts : CompleteTagWithAttributesParts);
        }

        
        

        internal override string GetTagNameImpl()
        {
            InternalDebug.Assert(this.TagNameIndex > HtmlNameIndex.Unknown);
            return HtmlNameData.names[(int)this.TagNameIndex].name;
        }

        internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
        {
            return HtmlNameData.names[(int)this.attributes[attributeIndex].nameIndex].publicAttributeId;
        }

        internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
        {
            return CompleteAttributeParts;
        }

        internal override string GetAttributeNameImpl(int attributeIndex)
        {
            InternalDebug.Assert(this.attributes[attributeIndex].nameIndex > HtmlNameIndex.Unknown);
            return HtmlNameData.names[(int)this.attributes[attributeIndex].nameIndex].name;
        }

        

        internal override string GetAttributeValueImpl(int attributeIndex)
        {
            return this.attributes[attributeIndex].value;
        }

        internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
        {
            int countToCopy = Math.Min(count, this.attributes[attributeIndex].value.Length - this.attributes[attributeIndex].readIndex);

            if (countToCopy != 0)
            {
                this.attributes[attributeIndex].value.CopyTo(this.attributes[attributeIndex].readIndex, buffer, offset, countToCopy);
                this.attributes[attributeIndex].readIndex += countToCopy;
            }

            return countToCopy;
        }

        internal override void WriteTagImpl(bool copyTagAttributes)
        {
            this.formatOutput.writer.WriteTagBegin(this.TagNameIndex, null, this.IsEndTag, false, false);

            if (copyTagAttributes)
            {
                for (int i = 0; i < this.countAttributes; i++)
                {
                    this.WriteAttributeImpl(i, true, true);
                }
            }
        }

        internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
        {
            if (writeName)
            {
                this.formatOutput.writer.WriteAttributeName(this.attributes[attributeIndex].nameIndex);
            }

            if (writeValue)
            {
                this.formatOutput.writer.WriteAttributeValue(this.attributes[attributeIndex].value);
            }
        }
    }
}

