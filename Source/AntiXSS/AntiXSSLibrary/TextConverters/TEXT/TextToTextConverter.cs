// ***************************************************************
// <copyright file="TextToTextConverter.cs" company="Microsoft">
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
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.Globalization;
    using Microsoft.Exchange.Data.TextConverters.Internal.Test;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;


    internal class TextToTextConverter : IProducerConsumer, IDisposable
    {
        protected TextParser parser;
        protected bool endOfFile;

        protected TextOutput output;
#if DEBUG
        protected TestHtmlTrace trace;
#endif
        protected bool convertFragment;

        protected int lineLength;
        protected int newLines;
        protected int spaces;
        protected int nbsps;
        protected bool paragraphStarted;

        protected bool treatNbspAsBreakable;

        private bool started;

        protected Injection injection;


        public TextToTextConverter(
                    TextParser parser, 
                    TextOutput output,
                    Injection injection,
                    bool convertFragment,
                    bool treatNbspAsBreakable,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {
#if DEBUG
            if (traceStream != null)
            {
                this.trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.treatNbspAsBreakable = treatNbspAsBreakable;

            this.convertFragment = convertFragment;

            this.output = output;
            this.parser = parser;

            if (!this.convertFragment)
            {
                this.injection = injection;
            }
        }

        public void Initialize(string fragment)
        {
            this.parser.Initialize(fragment);

            this.endOfFile = false;

            this.lineLength = 0;
            this.newLines = 0;
            this.spaces = 0;
            this.nbsps = 0;
            this.paragraphStarted = false;
            this.started = false;
        }


        public void Run()
        {
            if (this.endOfFile)
            {
                // nothing to do after the end of file.
                return;
            }

            TextTokenId tokenId = this.parser.Parse();

            if (TextTokenId.None == tokenId)
            {
                return;
            }

            this.Process(tokenId);
        }

        public bool Flush()
        {
            if (!this.endOfFile)
            {
                this.Run();
            }

            return this.endOfFile;
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
            if (!started)
            {
                if (!this.convertFragment)
                {
                    this.output.OpenDocument();

                    if (this.injection != null && this.injection.HaveHead)
                    {
                        this.injection.Inject(true, this.output);
                    }
                }

                this.output.SetQuotingLevel(0);
                started = true;
            }

            switch (tokenId)
            {
                case TextTokenId.Text:

                    this.OutputFragmentSimple(this.parser.Token);
                    break;

                case TextTokenId.EncodingChange:

                    if (!this.convertFragment)
                    {
                        if (this.output.OutputCodePageSameAsInput)
                        {
                            int codePage = this.parser.Token.Argument;

                            #if DEBUG
                            Encoding newOutputEncoding;
                            // we should have already checked this in parser before reporting change
                            InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                            #endif

                            this.output.OutputEncoding = Charset.GetEncoding(codePage);
                        }
                    }
                    break;

                case TextTokenId.EndOfFile:

                    if (!this.convertFragment)
                    {
                        if (this.injection != null && this.injection.HaveTail)
                        {
                            if (!this.output.LineEmpty)
                            {
                                this.output.OutputNewLine();
                            }

                            this.injection.Inject(false, this.output);
                        }

                        this.output.Flush();
                    }
                    else
                    {
                        this.output.CloseParagraph();
                    }

                    this.endOfFile = true;

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

                                this.output.OutputNewLine();
                                break;

                        case RunTextType.Space:
                        case RunTextType.UnusualWhitespace:

                                this.output.OutputSpace(run.Length);
                                break;

                        case RunTextType.Tabulation:

                                this.output.OutputTabulation(run.Length);
                                break;

                        case RunTextType.Nbsp:

                                if (this.treatNbspAsBreakable)
                                {
                                    this.output.OutputSpace(run.Length);
                                }
                                else
                                {
                                    this.output.OutputNbsp(run.Length);
                                }
                                break;

                        case RunTextType.NonSpace:

                                // InternalDebug.Assert(run.IsNormal);
                                this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, TextMapping.Unicode);
                                break;

                        default:

                                InternalDebug.Assert(false, "unexpected run text type");
                                break;
                    }
                }
                else if (run.IsSpecial && run.Kind == (uint)TextRunKind.QuotingLevel)
                {
                    this.output.SetQuotingLevel((ushort)run.Value);
                }
            }
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
#if DEBUG
            if (this.trace != null /*&& this.trace is IDisposable*/)
            {
                ((IDisposable)this.trace).Dispose();
            }
#endif
            this.parser = null;
            this.output = null;
#if DEBUG
            this.trace = null;
#endif
            GC.SuppressFinalize(this);
        }
    }
}
