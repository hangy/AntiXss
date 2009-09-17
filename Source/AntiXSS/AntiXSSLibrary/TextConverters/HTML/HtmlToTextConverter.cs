// ***************************************************************
// <copyright file="HtmlToTextConverter.cs" company="Microsoft">
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
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;
    
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;

    

    internal class HtmlToTextConverter : IProducerConsumer, IRestartable, IReusable, IDisposable
    {
        

        private bool convertFragment;

        private IHtmlParser parser;
        private bool endOfFile;

        private TextOutput output;

        private HtmlToken token;

        private bool treatNbspAsBreakable;
        private bool outputImageLinks = true;
        private bool outputAnchorLinks = true;

        protected bool normalizedInput;

        private NormalizerContext normalizerContext;

        private TextMapping textMapping;

        
        private struct NormalizerContext
        {
            
            public char lastCh;
            public bool oneNL;
            public bool hasSpace;
            public bool eatSpace;
        }

        private bool lineStarted;
        private bool wideGap;
        private bool nextParagraphCloseWideGap = true;
        private bool afterFirstParagraph;
        private bool ignoreNextP;

        
        private int listLevel;
        private int listIndex;
        private bool listOrdered;

        private bool insideComment;
        private bool insidePre;
        private bool insideAnchor;

        private ScratchBuffer urlScratch;
        private int imageHeightPixels;
        private int imageWidthPixels;
        private ScratchBuffer imageAltText;

        private ScratchBuffer scratch;

        private Injection injection;

        private UrlCompareSink urlCompareSink;

        

        public HtmlToTextConverter(
                    IHtmlParser parser,
                    TextOutput output,
                    Injection injection,
                    bool convertFragment,
                    bool preformattedText,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {

            this.normalizedInput = (parser is HtmlNormalizingParser);

            this.treatNbspAsBreakable = testTreatNbspAsBreakable;

            this.convertFragment = convertFragment;

            this.output = output;

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            if (!convertFragment)
            {
                this.injection = injection;

                this.output.OpenDocument();

                if (this.injection != null && this.injection.HaveHead)
                {
                    this.injection.Inject(true, this.output);
                }
            }
            else
            {
                this.insidePre = preformattedText;
            }
        }

        

        private void Reinitialize()
        {
            this.endOfFile = false;

            this.normalizerContext.hasSpace = false;
            this.normalizerContext.eatSpace = false;
            this.normalizerContext.oneNL = false;
            this.normalizerContext.lastCh = '\0';

            this.lineStarted = false;
            this.wideGap = false;
            this.nextParagraphCloseWideGap = true;
            this.afterFirstParagraph = false;
            this.ignoreNextP = false;

            this.insideComment = false;
            this.insidePre = false;
            this.insideAnchor = false;
            if (this.urlCompareSink != null)
            {
                this.urlCompareSink.Reset();
            }

            this.listLevel = 0;
            this.listIndex= 0;
            this.listOrdered = false;

            

            
            if (!this.convertFragment)
            {
                this.output.OpenDocument();

                if (this.injection != null)
                {
                    this.injection.Reset();

                    if (this.injection.HaveHead)
                    {
                        this.injection.Inject(true, this.output);
                    }
                }
            }

            this.textMapping = TextMapping.Unicode;

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

        

        private void Process(HtmlTokenId tokenId)
        {
            this.token = this.parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    if (this.token.TagIndex <= HtmlTagIndex.Unknown)
                    {
                        break;
                    }

                    HtmlDtd.TagDefinition tagDef = GetTagDefinition(this.token.TagIndex);

                    if (this.normalizedInput)
                    {
                        if (!this.token.IsEndTag)
                        {
                            if (this.token.IsTagBegin)
                            {
                                
                                this.PushElement(tagDef);
                            }

                            this.ProcessStartTagAttributes(tagDef);
                        }
                        else
                        {
                            if (this.token.IsTagBegin)
                            {
                                this.PopElement(tagDef);
                            }
                        }
                    }
                    else
                    {
                        

                        
                        
                        
                        

                        if (!this.token.IsEndTag)
                        {
                            if (this.token.IsTagBegin)
                            {
                                this.LFillTagB(tagDef);
                                this.PushElement(tagDef);
                                this.RFillTagB(tagDef);
                            }

                            this.ProcessStartTagAttributes(tagDef);
                        }
                        else
                        {
                            if (this.token.IsTagBegin)
                            {
                                this.LFillTagE(tagDef);
                                this.PopElement(tagDef);
                                this.RFillTagE(tagDef);
                            }
                        }
                    }
                    break;

                

                case HtmlTokenId.Text:

                    if (!this.insideComment)
                    {
                        if (this.insideAnchor && this.urlCompareSink.IsActive)
                        {
                            token.Text.WriteTo(this.urlCompareSink);
                        }

                        if (this.insidePre)
                        {
                            this.ProcessPreformatedText();
                        }
                        else if (this.normalizedInput)
                        {
                            this.ProcessText();
                        }
                        else
                        {
                            this.NormalizeProcessText();
                        }
                    }
                    break;


                

                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:

                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    if (this.output.OutputCodePageSameAsInput)
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

                    if (this.lineStarted)
                    {
                        this.output.OutputNewLine();
                        this.lineStarted = false;
                    }

                    if (!this.convertFragment)
                    {
                        if (this.injection != null && this.injection.HaveHead)
                        {
                            if (this.wideGap)
                            {
                                this.output.OutputNewLine();
                                this.wideGap = false;
                            }

                            this.injection.Inject(false, this.output);
                        }

                        this.output.CloseDocument();
                        this.output.Flush();
                    }
                    
                    this.endOfFile = true;
                    break;

            }
        }

        
        private void PushElement(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.Title:
                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Style:
                
                case HtmlTagIndex.NoEmbed:
                case HtmlTagIndex.NoFrames:

                        this.insideComment = true;
                        break;

                case HtmlTagIndex.A:
                

                        if (this.insideAnchor)
                        {
                            
                            
                            

                            this.EndAnchor();
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        break;

                case HtmlTagIndex.TD:
                case HtmlTagIndex.TH:

                        if (this.lineStarted)
                        {
                            this.output.OutputTabulation(1);
                        }
                        break;

                case HtmlTagIndex.P:

                        if (!this.ignoreNextP)
                        {
                            this.EndParagraph(true);
                        }
                        this.nextParagraphCloseWideGap = true;
                        break;

                
                case HtmlTagIndex.BR:
                case HtmlTagIndex.Option:

                        this.EndLine();
                        break;

                
                case HtmlTagIndex.HR:

                        this.EndParagraph(false);
                        this.OutputText("________________________________");
                        this.EndParagraph(false);
                        break;

                
                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.Dir:
                case HtmlTagIndex.Menu:

                        this.EndParagraph(this.listLevel == 0);

                        if (this.listLevel < 10)
                        {
                            this.listLevel ++;

                            if (this.listLevel == 1)
                            {
                                this.listIndex = 1;
                                this.listOrdered = (this.token.TagIndex == HtmlTagIndex.OL);
                            }
                        }
                        this.nextParagraphCloseWideGap = false;
                        break;

                case HtmlTagIndex.LI:

                        this.EndParagraph(false);

                        
                        this.OutputText("  ");

                        for (int i = 0; i < this.listLevel - 1; i++)
                        {
                            
                            this.OutputText("   ");
                        }

                        if (this.listLevel > 1 || !this.listOrdered)
                        {
                            this.OutputText("*");
                            this.output.OutputSpace(3);
                        }
                        else
                        {
                            string num = this.listIndex.ToString();

                            this.OutputText(num);

                            this.OutputText(".");
                            this.output.OutputSpace(num.Length == 1 ? 2 : 1);
                            this.listIndex ++;
                        }
                        break;

                
                case HtmlTagIndex.DL:              

                        this.EndParagraph(true);
                        break;

                case HtmlTagIndex.DT:              

                        if (this.lineStarted)
                        {
                            this.EndLine();
                        }
                        break;

                case HtmlTagIndex.DD:              

                        if (this.lineStarted)
                        {
                            this.EndLine();
                        }
                        break;

                
                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        this.EndParagraph(true);
                        this.insidePre = true;
                        break;

                case HtmlTagIndex.Font:
                case HtmlTagIndex.Span:

                        break;

                default:
                        if (tagDef.blockElement)
                        {
                            this.EndParagraph(false);
                        }
                        break;
            }

            this.ignoreNextP = false;

            if (tagDef.tagIndex == HtmlTagIndex.LI)
            {
                
                this.ignoreNextP = true;
            }
        }

        private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.A:
                

                        if (this.outputAnchorLinks)
                        {
                            foreach (HtmlAttribute attr in this.token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.Href)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        this.urlScratch.Reset();
                                    }

                                    this.urlScratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                    break;
                                }
                            }

                            if (this.token.IsTagEnd)
                            {
                                BufferString url = this.urlScratch.BufferString;

                                url.TrimWhitespace();

                                if (url.Length != 0 && url[0] != '#' && url[0] != '?' && url[0] != ';')
                                {
                                    if (!this.lineStarted)
                                    {
                                        this.StartParagraphOrLine();
                                    }

                                    
                                    

                                    string urlString = url.ToString();

                                    if (urlString.IndexOf(' ') != -1)
                                    {
                                        urlString = urlString.Replace(" ", "%20");
                                    }

                                    this.output.OpenAnchor(urlString);
                                    this.insideAnchor = true;

                                    if (this.urlCompareSink == null)
                                    {
                                        this.urlCompareSink = new UrlCompareSink();
                                    }

                                    this.urlCompareSink.Initialize(urlString);
                                }

                                this.urlScratch.Reset();
                            }
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        

                        if (this.outputImageLinks)
                        {
                            foreach (HtmlAttribute attr in this.token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.Src)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        this.urlScratch.Reset();
                                    }

                                    this.urlScratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Alt)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        this.imageAltText.Reset();
                                    }

                                    this.imageAltText.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Height)
                                {
                                    if (!attr.Value.IsEmpty)
                                    {
                                        PropertyValue value;

                                        if (attr.Value.IsContiguous)
                                        {
                                            value = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        else
                                        {
                                            this.scratch.Reset();
                                            this.scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                            value = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                        }

                                        if (value.IsAbsRelLength)
                                        {
                                            this.imageHeightPixels = value.PixelsInteger;
                                            if (this.imageHeightPixels == 0)
                                            {
                                                this.imageHeightPixels = 1;
                                            }
                                        }
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Width)
                                {
                                    if (!attr.Value.IsEmpty)
                                    {
                                        PropertyValue value;

                                        if (attr.Value.IsContiguous)
                                        {
                                            value = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        else
                                        {
                                            this.scratch.Reset();
                                            this.scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                            value = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                        }

                                        if (value.IsAbsRelLength)
                                        {
                                            this.imageWidthPixels = value.PixelsInteger;
                                            if (this.imageWidthPixels == 0)
                                            {
                                                this.imageWidthPixels = 1;
                                            }
                                        }
                                    }
                                }
                            }

                            

                            if (this.token.IsTagEnd)
                            {
                                string urlString = null;
                                string altString = null;

                                

                                BufferString alt = this.imageAltText.BufferString;

                                alt.TrimWhitespace();

                                if (alt.Length != 0)
                                {
                                    altString = alt.ToString();
                                }

                                if (altString == null || this.output.ImageRenderingCallbackDefined)
                                {
                                    BufferString url = this.urlScratch.BufferString;

                                    url.TrimWhitespace();

                                    if (url.Length != 0)
                                    {
                                        urlString = url.ToString();
                                    }
                                }

                                if (!this.lineStarted)
                                {
                                    this.StartParagraphOrLine();
                                }

                                this.output.OutputImage(urlString, altString, this.imageWidthPixels, this.imageHeightPixels);

                                this.urlScratch.Reset();
                                this.imageAltText.Reset();
                                this.imageHeightPixels = 0;
                                this.imageWidthPixels = 0;
                            }
                        }
                        break;

                case HtmlTagIndex.P:

                        
                        

                        if (this.token.Attributes.Find(HtmlNameIndex.Class) && this.token.Attributes.Current.Value.CaseInsensitiveCompareEqual("msonormal"))
                        {
                            
                            
                            

                            this.wideGap = false;
                            this.nextParagraphCloseWideGap = false;
                        }
                        break;

                case HtmlTagIndex.Font:

                        foreach (HtmlAttribute attr in this.token.Attributes)
                        {
                            if (attr.NameIndex == HtmlNameIndex.Face)
                            {
                                this.scratch.Reset();
                                this.scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

                                RecognizeInterestingFontName fontRecognizer = new RecognizeInterestingFontName();

                                for (int i = 0; i < this.scratch.Length && !fontRecognizer.IsRejected; i++)
                                {
                                    fontRecognizer.AddCharacter(this.scratch.Buffer[i]);
                                }

                                this.textMapping = fontRecognizer.TextMapping;
                                break;
                            }
                        }

                        break;


                case HtmlTagIndex.Span:

                        foreach (HtmlAttribute attr in this.token.Attributes)
                        {
                            if (attr.NameIndex == HtmlNameIndex.Style)
                            {
                                this.scratch.Reset();
                                this.scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

                                RecognizeInterestingFontNameInInlineStyle fontRecognizer = new RecognizeInterestingFontNameInInlineStyle();

                                for (int i = 0; i < this.scratch.Length && !fontRecognizer.IsFinished; i++)
                                {
                                    fontRecognizer.AddCharacter(this.scratch.Buffer[i]);
                                }

                                this.textMapping = fontRecognizer.TextMapping;
                                break;
                            }
                        }

                        break;

            }
        }

        
        private void PopElement(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.Title:
                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Style:
                
                case HtmlTagIndex.NoEmbed:
                case HtmlTagIndex.NoFrames:

                        this.insideComment = false;
                        break;

                case HtmlTagIndex.A:
                

                        if (this.insideAnchor)
                        {
                            this.EndAnchor();
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        break;

                case HtmlTagIndex.TD:
                case HtmlTagIndex.TH:

                        this.lineStarted = true;
                        break;

                case HtmlTagIndex.P:

                        this.EndParagraph(this.nextParagraphCloseWideGap);
                        this.nextParagraphCloseWideGap = true;
                        break;

                
                case HtmlTagIndex.BR:
                case HtmlTagIndex.Option:

                        this.EndLine();
                        break;

                
                case HtmlTagIndex.HR:

                        this.EndParagraph(false);
                        this.OutputText("________________________________");
                        this.EndParagraph(false);
                        break;

                
                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.Dir:
                case HtmlTagIndex.Menu:

                        if (this.listLevel != 0)
                        {
                            this.listLevel --;
                        }

                        this.EndParagraph(this.listLevel == 0);
                        break;

                case HtmlTagIndex.DT:              

                        break;

                case HtmlTagIndex.DD:              

                        break;

                
                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        this.EndParagraph(true);
                        this.insidePre = false;
                        break;

                case HtmlTagIndex.Font:
                case HtmlTagIndex.Span:

                        this.textMapping = TextMapping.Unicode;
                        break;

                default:
                        if (tagDef.blockElement)
                        {
                            this.EndParagraph(false);
                        }
                        break;
            }

            this.ignoreNextP = false;
        }

        
        private void ProcessText()
        {
            if (!this.lineStarted)
            {
                this.StartParagraphOrLine();
            }

            foreach (TokenRun run in this.token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        this.output.OutputSpace(1);
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (this.treatNbspAsBreakable)
                        {
                            this.output.OutputSpace(run.Length);
                        }
                        else
                        {
                            this.output.OutputNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        if (run.IsLiteral)
                        {
                            this.output.OutputNonspace(run.Literal, this.textMapping);
                        }
                        else
                        {
                            this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, this.textMapping);
                        }
                    }
                }
            }
        }

        
        private void ProcessPreformatedText()
        {
            if (!this.lineStarted)
            {
                this.StartParagraphOrLine();
            }

            foreach (TokenRun run in this.token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        switch (run.TextType)
                        {
                            case RunTextType.NewLine:

                                    
                                    this.output.OutputNewLine();
                                    break;

                            case RunTextType.Space:
                            default:

                                    if (this.treatNbspAsBreakable)
                                    {
                                        this.output.OutputSpace(run.Length);
                                    }
                                    else
                                    {
                                        this.output.OutputNbsp(run.Length);
                                    }
                                    break;

                            case RunTextType.Tabulation:
#if false
                                    int tabPosition = this.output.LineLength() / 8 * 8 + 8 * run.Length;
                                    int extraSpaces = tabPosition - this.output.LineLength();

                                    this.output.OutputSpace(extraSpaces);
#endif
                                    this.output.OutputTabulation(run.Length);
                                    break;
                        }
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (this.treatNbspAsBreakable)
                        {
                            this.output.OutputSpace(run.Length);
                        }
                        else
                        {
                            this.output.OutputNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        if (run.IsLiteral)
                        {
                            this.output.OutputNonspace(run.Literal, this.textMapping);
                        }
                        else
                        {
                            this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, this.textMapping);
                        }
                    }
                }
            }
        }

        
        private void NormalizeProcessText()
        {
            Token.RunEnumerator runs = this.token.Runs;

            runs.MoveNext(true);

            while (runs.IsValidPosition)
            {
                TokenRun run = runs.Current;

                if (run.IsAnyWhitespace)
                {
                    

                    int cntWhitespaces = 0;

                    do
                    {
                        
                        
                        cntWhitespaces += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                    }
                    while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace));

                    this.NormalizeAddSpace(cntWhitespaces == 1);
                }
                else if (run.TextType == RunTextType.Nbsp)
                {
                    this.NormalizeAddNbsp(run.Length);

                    runs.MoveNext(true);
                }
                else
                {
                    
                    this.NormalizeAddNonspace(run);

                    runs.MoveNext(true);
                }
            }
        }

         
        private void NormalizeAddNonspace(TokenRun run)
        {
            if (!this.lineStarted)
            {
                this.StartParagraphOrLine();
            }

            if (this.normalizerContext.hasSpace)
            {
                this.normalizerContext.hasSpace = false;

                

                if ('\0' == this.normalizerContext.lastCh || !this.normalizerContext.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.normalizerContext.lastCh, run.FirstChar))
                {
                    this.output.OutputSpace(1);
                }
            }

            if (run.IsLiteral)
            {
                this.output.OutputNonspace(run.Literal, this.textMapping);
            }
            else
            {
                this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, this.textMapping);
            }

            this.normalizerContext.eatSpace = false;
            this.normalizerContext.lastCh = run.LastChar;
            this.normalizerContext.oneNL = false;
        }

        
        private void NormalizeAddNbsp(int count)
        {
            if (!this.lineStarted)
            {
                this.StartParagraphOrLine();
            }

            if (this.normalizerContext.hasSpace)
            {
                this.normalizerContext.hasSpace = false;

                this.output.OutputSpace(1);
            }

            if (this.treatNbspAsBreakable)
            {
                this.output.OutputSpace(count);
            }
            else
            {
                this.output.OutputNbsp(count);
            }

            this.normalizerContext.eatSpace = false;
            this.normalizerContext.lastCh = '\xA0';
            this.normalizerContext.oneNL = false;
        }

        
        private void NormalizeAddSpace(bool oneNL)
        {
            InternalDebug.Assert(!this.insidePre);

            if (!this.normalizerContext.eatSpace && this.afterFirstParagraph)
            {
                this.normalizerContext.hasSpace = true;
            }

            if (this.normalizerContext.lastCh != '\0')
            {
                if (oneNL && !this.normalizerContext.oneNL)
                {
                    this.normalizerContext.oneNL = true;
                }
                else
                {
                    this.normalizerContext.lastCh = '\0';
                }
            }
        }

        
        
        
        

        
        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!this.insidePre)
            {
                this.LFill(tagDef.fill.LB);
            }
        }

        
        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!this.insidePre)
            {
                this.RFill(tagDef.fill.RB);
            }
        }

        
        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!this.insidePre)
            {
                this.LFill(tagDef.fill.LE);
            }
        }

        
        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!this.insidePre)
            {
                this.RFill(tagDef.fill.RE);
            }
        }

        
        private void LFill(HtmlDtd.FillCode codeLeft)
        {
            this.normalizerContext.lastCh = '\0';

            if (this.normalizerContext.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    if (!this.lineStarted)
                    {
                        this.StartParagraphOrLine();
                    }

                    this.output.OutputSpace(1);
                    this.normalizerContext.eatSpace = true;
                }

                this.normalizerContext.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }
        }

        
        private void RFill(HtmlDtd.FillCode code)
        {
            if (code == HtmlDtd.FillCode.EAT)
            {
                this.normalizerContext.hasSpace = false;
                this.normalizerContext.eatSpace = true;
            }
            else if (code == HtmlDtd.FillCode.PUT)
            {
                this.normalizerContext.eatSpace = false;
            }
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return tagIndex != HtmlTagIndex._NULL ? HtmlDtd.tags[(int)tagIndex] : null;
        }

        

        private void EndAnchor()
        {
            if (!this.urlCompareSink.IsMatch)
            {
                if (!this.lineStarted)
                {
                    this.StartParagraphOrLine();
                }

                this.output.CloseAnchor();
            }
            else
            {
                this.output.CancelAnchor();
            }

            this.insideAnchor = false;
            this.urlCompareSink.Reset();
        }

        

        private void OutputText(string text)
        {
            if (!this.lineStarted)
            {
                this.StartParagraphOrLine();
            }

            this.output.OutputNonspace(text, this.textMapping);
        }

        

        private void StartParagraphOrLine()
        {
            InternalDebug.Assert(!this.lineStarted);

            if (this.wideGap)
            {
                if (this.afterFirstParagraph)
                {
                    this.output.OutputNewLine();
                }

                this.wideGap = false;
            }

            this.lineStarted = true;
            this.afterFirstParagraph = true;
        }

        

        private void EndLine()
        {
            this.output.OutputNewLine();
            this.lineStarted = false;
            this.wideGap = false;
        }

        

        private void EndParagraph(bool wideGap)
        {
            if (this.insideAnchor)
            {
                
                this.EndAnchor();
            }

            if (this.lineStarted)
            {
                this.output.OutputNewLine();
                this.lineStarted = false;
            }

            this.wideGap = (this.wideGap || wideGap);
        }

        

        void IDisposable.Dispose()
        {
            if (this.parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)this.parser).Dispose();
            }

            if (!this.convertFragment && this.output != null && this.output is IDisposable)
            {
                ((IDisposable)this.output).Dispose();
            }

            if (this.token != null && this.token is IDisposable)
            {
                ((IDisposable)this.token).Dispose();
            }

            this.parser = null;
            this.output = null;

            this.token = null;

            GC.SuppressFinalize(this);
        }

        

        bool IRestartable.CanRestart()
        {
            return this.convertFragment || ((IRestartable)this.output).CanRestart();
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (!this.convertFragment)
            {
                ((IRestartable)this.output).Restart();
            }

            this.Reinitialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (!this.convertFragment)
            {
                ((IRestartable)this.output).DisableRestart();
            }
        }

        

        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(this.output is IReusable && this.parser is IReusable);

            ((IReusable)this.parser).Initialize(newSourceOrDestination);
            ((IReusable)this.output).Initialize(newSourceOrDestination);

            this.Reinitialize();

            this.parser.SetRestartConsumer(this);
        }

        

        public void Initialize(string fragment, bool preformatedText)
        {
            if (this.normalizedInput)
            {
                ((HtmlNormalizingParser)this.parser).Initialize(fragment, preformatedText);
            }
            else
            {
                ((HtmlParser)this.parser).Initialize(fragment, preformatedText);
            }

            if (!this.convertFragment)
            {
                ((IReusable)this.output).Initialize(null);
            }

            this.Reinitialize();
        }
    }
}

