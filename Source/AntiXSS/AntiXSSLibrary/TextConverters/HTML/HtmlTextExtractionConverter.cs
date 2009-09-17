// ***************************************************************
// <copyright file="HtmlTextExtractionConverter.cs" company="Microsoft">
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
    

    

    internal class HtmlTextExtractionConverter : IProducerConsumer, IRestartable, IDisposable
    {
        

        private IHtmlParser parser;
        private bool endOfFile;

        private ConverterOutput output;

        private bool outputAnchorLinks = true;

        private bool insideComment;
        private bool insideAnchor;

        private CollapseWhitespaceState collapseWhitespaceState;

        

        public HtmlTextExtractionConverter(
                    IHtmlParser parser, 
                    ConverterOutput output,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {

            this.output = output;

            this.parser = parser;
            this.parser.SetRestartConsumer(this);
        }

        

        public bool CanRestart()
        {
            return ((IRestartable)this.output).CanRestart();
        }

        

        public void Restart()
        {
            InternalDebug.Assert(this.CanRestart());

            ((IRestartable)this.output).Restart();

            

            this.endOfFile = false;

            this.insideComment = false;
            this.insideAnchor = false;
        }

        

        public void DisableRestart()
        {
            ((IRestartable)this.output).DisableRestart();
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

        
        

        void IDisposable.Dispose()
        {
            if (this.parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)this.parser).Dispose();
            }

            if (this.output != null && this.output is IDisposable)
            {
                ((IDisposable)this.output).Dispose();
            }

            this.parser = null;
            this.output = null;

            GC.SuppressFinalize(this);
        }

        

        private void Process(HtmlTokenId tokenId)
        {
            HtmlToken token = this.parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    
                    

                    if (token.IsTagBegin)
                    {
                        

                        switch (token.TagIndex)
                        {
                            case HtmlTagIndex.Title:

                                    
                                    break;

                            case HtmlTagIndex.Comment:
                            case HtmlTagIndex.Script:
                            case HtmlTagIndex.Style:

                                    this.insideComment = !token.IsEndTag;
                                    break;

                            case HtmlTagIndex.A:

                                    if (this.outputAnchorLinks)
                                    {
                                        this.insideAnchor = !token.IsEndTag;
                                        if (!token.IsEndTag)
                                        {

                                        }
                                        else
                                        {
                                            
                                        }
                                    }
                                    break;

                            case HtmlTagIndex.Base:
                            case HtmlTagIndex.BaseFont:
                            case HtmlTagIndex.BGSound:
                            case HtmlTagIndex.Link:
                            case HtmlTagIndex.FrameSet:
                            case HtmlTagIndex.Frame:
                            case HtmlTagIndex.Iframe:

                                    
                                    break;

                            case HtmlTagIndex.Map:
                            case HtmlTagIndex.Div:
                            case HtmlTagIndex.P:
                            case HtmlTagIndex.H1:
                            case HtmlTagIndex.H2:
                            case HtmlTagIndex.H3:
                            case HtmlTagIndex.H4:
                            case HtmlTagIndex.H5:
                            case HtmlTagIndex.H6:
                            case HtmlTagIndex.Center:
                            case HtmlTagIndex.BlockQuote:
                            case HtmlTagIndex.Address:
                            case HtmlTagIndex.Marquee:
                            case HtmlTagIndex.BR:
                            
                            case HtmlTagIndex.HR:
                            
                            case HtmlTagIndex.Form:
                            case HtmlTagIndex.FieldSet:
                            case HtmlTagIndex.OptGroup:
                            case HtmlTagIndex.Select:
                            case HtmlTagIndex.Option:
                            
                            case HtmlTagIndex.OL:
                            case HtmlTagIndex.UL:
                            case HtmlTagIndex.Dir:
                            case HtmlTagIndex.Menu:
                            case HtmlTagIndex.LI:
                            case HtmlTagIndex.DL:              
                            case HtmlTagIndex.DT:              
                            case HtmlTagIndex.DD:              
                            
                            case HtmlTagIndex.Table:
                            case HtmlTagIndex.Caption:
                            case HtmlTagIndex.ColGroup:
                            case HtmlTagIndex.Col:
                            case HtmlTagIndex.Tbody:
                            case HtmlTagIndex.Thead:
                            case HtmlTagIndex.Tfoot:
                            case HtmlTagIndex.TR:
                            case HtmlTagIndex.TC:              
                            
                            case HtmlTagIndex.Pre:
                            case HtmlTagIndex.PlainText:
                            case HtmlTagIndex.Listing:

                                    this.collapseWhitespaceState = CollapseWhitespaceState.NewLine;
                                    break;

                            case HtmlTagIndex.TH:
                            case HtmlTagIndex.TD:

                                    if (!token.IsEndTag)
                                    {
                                        this.output.Write("\t");
                                    }
                                    break;

                            
                            case HtmlTagIndex.NoEmbed:
                            case HtmlTagIndex.NoFrames:

                                    this.insideComment = !token.IsEndTag;
                                    break;
                        }
                    }

                    
                    

                    switch (token.TagIndex)
                    {
                        case HtmlTagIndex.A:

                                if (!token.IsEndTag && this.outputAnchorLinks)
                                {
                                    
                                }
                                break;

                        case HtmlTagIndex.Area:

                                if (!token.IsEndTag && this.outputAnchorLinks)
                                {
                                    
                                }
                                break;

                        case HtmlTagIndex.Img:
                        case HtmlTagIndex.Image:

                                if (!token.IsEndTag && this.outputAnchorLinks)
                                {
                                    
                                }
                                break;
                    }

                    break;

                

                case HtmlTokenId.Text:

                    if (!this.insideComment)
                    {
                        token.Text.WriteToAndCollapseWhitespace(this.output, ref collapseWhitespaceState);
                    }
                    break;

                

                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:

                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    ConverterEncodingOutput encodingOutput = this.output as ConverterEncodingOutput;
                    if (encodingOutput != null)
                    {
                        int codePage = token.Argument;

                        if (encodingOutput.CodePageSameAsInput)
                        {
                            #if DEBUG
                            Encoding newOutputEncoding;
                            
                            InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                            #endif

                            
                            encodingOutput.Encoding = Charset.GetEncoding(codePage);
                        }
                    }
                    break;

                

                case HtmlTokenId.EndOfFile:

                    this.output.Write("\r\n");
                    this.output.Flush();
                    
                    this.endOfFile = true;
                    break;
            }
        }
    }
}

