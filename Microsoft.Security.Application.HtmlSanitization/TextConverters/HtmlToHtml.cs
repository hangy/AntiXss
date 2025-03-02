// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlToHtml.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   A HTML to HTML converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using System.Text;

    using Internal.Html;

    using Strings = CtsResources.TextConvertersStrings;

        /// <summary>
        /// A HTML to HTML converter.
        /// </summary>
        internal class HtmlToHtml : TextConverter
        {
            /// <summary>
            /// The input encoding.
            /// </summary>
            private Encoding inputEncoding;

            /// <summary>
            /// Value indicating whether encoding should be detected from the BOM
            /// </summary>
            private readonly bool detectEncodingFromByteOrderMark = true;

            /// <summary>
            /// Value indicating whether encoding should be detected from the charset meta tag/
            /// </summary>
            private readonly bool detectEncodingFromMetaTag = true;

            /// <summary>
            /// The output encoding.
            /// </summary>
            private Encoding outputEncoding;

            /// <summary>
            /// Value indicating if the output encoding should be the same as the input encoding.
            /// </summary>
            private readonly bool outputEncodingSameAsInput = true;

            /// <summary>
            /// Value indicating if the HTML input should be normalized.
            /// </summary>
            private bool normalizeInputHtml;

            /// <summary>
            /// The format to use for header and footer injection.
            /// </summary>
            private readonly HeaderFooterFormat injectionFormat = HeaderFooterFormat.Text;

            /// <summary>
            /// The header to inject.
            /// </summary>
            private readonly string injectHead;

            /// <summary>
            /// The tail to inject.
            /// </summary>
            private readonly string injectTail;

            /// <summary>
            /// Value indicating if HTML should be filtered.
            /// </summary>
            private bool filterHtml;

            /// <summary>
            /// The call back to use when parsing HTML
            /// </summary>
            private readonly HtmlTagCallback htmlCallback;

            /// <summary>
            /// Value indicating if truncation should be tested for when a callback is performed.
            /// </summary>
            private readonly bool testTruncateForCallback = true;

            /// <summary>
            /// Value indicating fragmented output can be generated.
            /// </summary>
            private bool outputFragment;

            /// <summary>
            /// The maximum number of tokenisation runs to perform.
            /// </summary>
            private readonly int testMaxTokenRuns = 512;

            /// <summary>
            /// The trace stream for tokenisation
            /// </summary>
            private readonly Stream testTraceStream;

            /// <summary>
            /// Value indicating if the test traces should show the token number.
            /// </summary>
            private readonly bool testTraceShowTokenNum = true;

            /// <summary>
            /// The token number at which test tracing should stop.
            /// </summary>
            private readonly int testTraceStopOnTokenNum;

            private readonly Stream testNormalizerTraceStream;
            private readonly bool testNormalizerTraceShowTokenNum = true;
            private readonly int testNormalizerTraceStopOnTokenNum;

            /// <summary>
            /// The maximum size of an HTML tag.
            /// </summary>
            private readonly int maxHtmlTagSize = 32768;

            /// <summary>
            /// The maximum number of attributes for an HTML tag
            /// </summary>
            private readonly int testMaxHtmlTagAttributes = 64;

            /// <summary>
            /// The maximum offset for parsing restarting.
            /// </summary>
            private readonly int testMaxHtmlRestartOffset = 4096;

            /// <summary>
            /// The limit for nested tags.
            /// </summary>
            private readonly int testMaxHtmlNormalizerNesting = HtmlSupport.HtmlNestingLimit;

            /// <summary>
            /// The threshold for small CSS blocks.
            /// </summary>
            private readonly int smallCssBlockThreshold = -1;

            /// <summary>
            /// Value indicating whether display styles should be reserved.
            /// </summary>
            private readonly bool preserveDisplayNoneStyle;

            internal bool NormalizeHtml
            {
                set { this.AssertNotLocked(); this.normalizeInputHtml = value; }
            }

            internal bool OutputHtmlFragment
            {
                set { this.AssertNotLocked(); this.outputFragment = value; }
            }

            internal bool FilterHtml
            {
                set { this.AssertNotLocked(); this.filterHtml = value; }
            }

#if M3STUFF
        public IHtmlParsingCallback HtmlParsingCallback
        {
            get { return this.parsingCallback; }
            set { this.AssertNotLocked(); this.parsingCallback = value; }
        }

        public HtmlFilterTables HtmlFilterTables
        {
            get { return this.filterTables; }
            set { this.AssertNotLocked(); this.filterTables = value; }
        }
#endif

            internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
            {
                if (this.inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                ConverterInput converterIn = new ConverterDecodingInput(
                                        converterStream,
                                        true,
                                        this.inputEncoding,
                                        this.detectEncodingFromByteOrderMark,
                                        this.maxHtmlTagSize,
                                        this.testMaxHtmlRestartOffset,
                                        this.InputStreamBufferSize,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback,
                                        null);

                ConverterOutput converterOut = new ConverterEncodingOutput(
                                        output,
                                        true,
                                        true,
                                        this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding,
                                        this.outputEncodingSameAsInput,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback);

                return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
            }

            internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
            {
                if (this.inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                this.outputEncoding = Encoding.Unicode;

                ConverterInput converterIn = new ConverterDecodingInput(
                                        converterStream,
                                        true,
                                        this.inputEncoding,
                                        this.detectEncodingFromByteOrderMark,
                                        this.maxHtmlTagSize,
                                        this.testMaxHtmlRestartOffset,
                                        this.InputStreamBufferSize,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback,
                                        null);

                ConverterOutput converterOut = new ConverterUnicodeOutput(
                                        output,
                                        true,
                                        true);

                return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
            }
            internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
            {
                if (this.inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                ConverterInput converterIn = new ConverterDecodingInput(
                                        input,
                                        false,
                                        this.inputEncoding,
                                        this.detectEncodingFromByteOrderMark,
                                        this.maxHtmlTagSize,
                                        this.testMaxHtmlRestartOffset,
                                        this.InputStreamBufferSize,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback,
                                        converterStream as IProgressMonitor);

                ConverterOutput converterOut = new ConverterEncodingOutput(
                                        converterStream,
                                        false,
                                        true,
                                        this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding,
                                        this.outputEncodingSameAsInput,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback);

                return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
            }

            internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
            {
                this.inputEncoding = Encoding.Unicode;

                ConverterInput converterIn = new ConverterUnicodeInput(
                                        input,
                                        false,
                                        this.maxHtmlTagSize,
                                        this.TestBoundaryConditions,
                                        converterStream as IProgressMonitor);

                ConverterOutput converterOut = new ConverterEncodingOutput(
                                        converterStream,
                                        false,
                                        false,
                                        this.outputEncodingSameAsInput ? System.Text.Encoding.UTF8 : this.outputEncoding,
                                        this.outputEncodingSameAsInput,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback);

                return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
            }

            internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
            {
                if (this.inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                this.outputEncoding = Encoding.Unicode;

                ConverterInput converterIn = new ConverterDecodingInput(
                                        input,
                                        false,
                                        this.inputEncoding,
                                        this.detectEncodingFromByteOrderMark,
                                        this.maxHtmlTagSize,
                                        this.testMaxHtmlRestartOffset,
                                        this.InputStreamBufferSize,
                                        this.TestBoundaryConditions,
                                        this as IResultsFeedback,
                                        converterReader as IProgressMonitor);

                ConverterOutput converterOut = new ConverterUnicodeOutput(
                                        converterReader,
                                        false,
                                        true);

                return CreateChain(converterIn, converterOut, converterReader as IProgressMonitor);
            }

            internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
            {
                this.inputEncoding = Encoding.Unicode;
                this.outputEncoding = Encoding.Unicode;

                ConverterInput converterIn = new ConverterUnicodeInput(
                                        input,
                                        false,
                                        this.maxHtmlTagSize,
                                        this.TestBoundaryConditions,
                                        converterReader as IProgressMonitor);

                ConverterOutput converterOut = new ConverterUnicodeOutput(
                                        converterReader,
                                        false,
                                        false);

                return CreateChain(converterIn, converterOut, converterReader as IProgressMonitor);
            }

            internal override void SetResult(ConfigParameter parameterId, object val)
            {
                switch (parameterId)
                {
                    case ConfigParameter.InputEncoding:
                        this.inputEncoding = (System.Text.Encoding)val;
                        break;

                    case ConfigParameter.OutputEncoding:
                        this.outputEncoding = (System.Text.Encoding)val;
                        break;
                }

                base.SetResult(parameterId, val);
            }

            private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
            {
                this.Locked = true;

                HtmlInjection injection = null;

                if (this.injectHead != null || this.injectTail != null)
                {
                    injection = new HtmlInjection(
                                this.injectHead,
                                this.injectTail,
                                this.injectionFormat,
                                this.filterHtml,
                                this.htmlCallback,
                                this.TestBoundaryConditions,
                                null,
                                progressMonitor);

                    this.normalizeInputHtml = true;
                }

                if (this.filterHtml || this.outputFragment || this.htmlCallback != null)
                {
                    this.normalizeInputHtml = true;
                }

                IHtmlParser parser;

                if (this.normalizeInputHtml)
                {
                    HtmlParser preParser = new(
                                            input,
                                            this.detectEncodingFromMetaTag,
                                            false,
                                            this.testMaxTokenRuns,
                                            this.testMaxHtmlTagAttributes,
                                            this.TestBoundaryConditions);

                    parser = new HtmlNormalizingParser(
                                            preParser,
                                            injection,
                                            this.htmlCallback != null,
                                            this.testMaxHtmlNormalizerNesting,
                                            this.TestBoundaryConditions,
                                            this.testNormalizerTraceStream,
                                            this.testNormalizerTraceShowTokenNum,
                                            this.testNormalizerTraceStopOnTokenNum);
                }
                else
                {
                    parser = new HtmlParser(
                                            input,
                                            this.detectEncodingFromMetaTag,
                                            false,
                                            this.testMaxTokenRuns,
                                            this.testMaxHtmlTagAttributes,
                                            this.TestBoundaryConditions);
                }

                HtmlWriter writer = new(
                                        output,
                                        this.filterHtml,
                                        this.normalizeInputHtml);
                return new HtmlToHtmlConverter(
                                        parser,
                                        writer,
                                        false,
                                        this.outputFragment,
                                        this.filterHtml,
                                        this.htmlCallback,
                                        this.testTruncateForCallback,
                                        injection != null && injection.HaveTail,
                                        this.testTraceStream,
                                        this.testTraceShowTokenNum,
                                        this.testTraceStopOnTokenNum,
                                        this.smallCssBlockThreshold,
                                        this.preserveDisplayNoneStyle,
                                        progressMonitor);
            }
        }
    }
