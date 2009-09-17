// ***************************************************************
// <copyright file="TextFormatConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Test;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;



    internal class TextFormatConverter : FormatConverter, IProducerConsumer, IDisposable
    {
        protected TextParser parser;

        private FormatOutput output;
#if DEBUG
        protected TestHtmlTrace trace;
#endif
        protected int lineLength;
        protected int newLines;
        protected int spaces;
        protected int nbsps;
        protected bool paragraphStarted;

        protected Injection injection;

 

        public TextFormatConverter(
                    TextParser parser, 
                    FormatOutput output,
                    Injection injection,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream) :
            base(formatConverterTraceStream)
        {
#if DEBUG
            if (traceStream != null)
            {
                this.trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.parser = parser;

            this.output = output;

            if (this.output != null)
            {
                this.output.Initialize(
                        this.Store,
                        SourceFormat.Text,
                        "converted from text");
            }

            this.injection = injection;

            // open the document container
            this.InitializeDocument();

            if (this.injection != null && this.injection.HaveHead)
            {
                InternalDebug.Assert(this.output != null);
                // this.injection.Inject(true, this.output);
            }


            this.OpenContainer(FormatContainerType.Block, false);


            this.Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
        }

        public TextFormatConverter(
                    TextParser parser, 
                    FormatStore store,
                    Injection injection,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream) :
            base(store, formatConverterTraceStream)
        {
#if DEBUG
            if (traceStream != null)
            {
                this.trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.parser = parser;

            this.injection = injection;

            // open the document container
            this.InitializeDocument();

            // open the first paragraph container
            this.OpenContainer(FormatContainerType.Block, false);


            this.Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
        }



        public void Initialize(string fragment)
        {
            this.parser.Initialize(fragment);

            this.lineLength = 0;
            this.newLines = 0;
            this.spaces = 0;
            this.nbsps = 0;
            this.paragraphStarted = false;
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
                TextTokenId tokenId = this.parser.Parse();

                if (TextTokenId.None != tokenId)
                {
                    this.Process(tokenId);
                }
            }
        }

        /////////////////////////////////////////////////////////////

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
#if DEBUG
            if (this.trace != null /*&& this.trace is IDisposable*/)
            {
                ((IDisposable)this.trace).Dispose();
            }
#endif
            this.parser = null;
#if DEBUG
            this.trace = null;
#endif
            GC.SuppressFinalize(this);
        }


        private bool CanFlush
        {
            get
            {
                // output can accept more data

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


        protected void Process(TextTokenId tokenId)
        {
#if DEBUG
            if (this.trace != null)
            {
                this.trace.TraceToken(this.parser.Token, 0);

                if (tokenId == TextTokenId.EndOfFile)
                {
                    this.trace.Flush();
                }
            }
#endif
            switch (tokenId)
            {
                case TextTokenId.Text:

                    this.OutputFragmentSimple(this.parser.Token);
                    break;

                case TextTokenId.EncodingChange:

                    if (this.output != null && this.output.OutputCodePageSameAsInput)
                    {
                        int codePage = this.parser.Token.Argument;

                        #if DEBUG
                        Encoding newOutputEncoding;
                        
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                        #endif

                        
                        this.output.OutputEncoding = Charset.GetEncoding(codePage);
                    }
                    break;

                case TextTokenId.EndOfFile:

                    if (this.injection != null && this.injection.HaveTail)
                    {
                        this.AddLineBreak(1);
                        InternalDebug.Assert(this.output != null);
                        
                    }

                    // close the paragraph container
                    this.CloseContainer();

                    // close the document container
                    this.CloseAllContainersAndSetEOF();
                    break;
            }
        }

        
        private void OutputFragmentSimple(TextToken token)
        {
            foreach (TokenRun run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    switch (run.TextType)
                    {
                        case RunTextType.NewLine:

                                this.AddLineBreak(1);
                                break;

                        case RunTextType.Space:
                        case RunTextType.UnusualWhitespace:

                                this.AddSpace(run.Length);
                                break;

                        case RunTextType.Tabulation:

                                this.AddTabulation(run.Length);
                                break;

                        case RunTextType.Nbsp:

                                this.AddNbsp(run.Length);
                                break;

                        case RunTextType.NonSpace:
                        case RunTextType.Unknown:

                                // InternalDebug.Assert(run.IsNormal);
                                this.AddNonSpaceText(run.RawBuffer, run.RawOffset, run.RawLength/*, TextMapping.Unicode*/);
                                break;

                        default:

                                InternalDebug.Assert(false, "unexpected run text type");
                                break;
                    }
                }
                else if (run.IsSpecial && run.Kind == (uint)TextRunKind.QuotingLevel)
                {
                    // consider: set QuotingLevelDelta property
                }
            }
        }
    }
}

