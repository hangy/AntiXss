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
            private bool detectEncodingFromByteOrderMark = true;

            /// <summary>
            /// Value indicating whether encoding should be detected from the charset meta tag/
            /// </summary>
            private bool detectEncodingFromMetaTag = true;

            /// <summary>
            /// The output encoding.
            /// </summary>
            private Encoding outputEncoding;

            /// <summary>
            /// Value indicating if the output encoding should be the same as the input encoding.
            /// </summary>
            private bool outputEncodingSameAsInput = true;

            /// <summary>
            /// Value indicating if the HTML input should be normalized.
            /// </summary>
            private bool normalizeInputHtml;

            /// <summary>
            /// The format to use for header and footer injection.
            /// </summary>
            private HeaderFooterFormat injectionFormat = HeaderFooterFormat.Text;

            /// <summary>
            /// The header to inject.
            /// </summary>
            private string injectHead;

            /// <summary>
            /// The tail to inject.
            /// </summary>
            private string injectTail;

            /// <summary>
            /// Value indicating if HTML should be filtered.
            /// </summary>
            private bool filterHtml;

            /// <summary>
            /// The call back to use when parsing HTML
            /// </summary>
            private HtmlTagCallback htmlCallback;

            /// <summary>
            /// Value indicating if truncation should be tested for when a callback is performed.
            /// </summary>
            private bool testTruncateForCallback = true;

            // Orphaned WPL code.
#if false
            /// <summary>
            /// Value indicating if tests for fragmentation during conversion are performed.
            /// </summary>
            private bool testConvertFragment;
#endif

            /// <summary>
            /// Value indicating fragmented output can be generated.
            /// </summary>        
            private bool outputFragment;

            /// <summary>
            /// The maximum number of tokenisation runs to perform.
            /// </summary>
            private int testMaxTokenRuns = 512;

            /// <summary>
            /// The trace stream for tokenisation
            /// </summary>
            private Stream testTraceStream;

            /// <summary>
            /// Value indicating if the test traces should show the token number.
            /// </summary>
            private bool testTraceShowTokenNum = true;

            /// <summary>
            /// The token number at which test tracing should stop.
            /// </summary>
            private int testTraceStopOnTokenNum;

            private Stream testNormalizerTraceStream;
            private bool testNormalizerTraceShowTokenNum = true;
            private int testNormalizerTraceStopOnTokenNum;

            /// <summary>
            /// The maximum size of an HTML tag.
            /// </summary>
            private int maxHtmlTagSize = 32768;

            /// <summary>
            /// The maximum number of attributes for an HTML tag
            /// </summary>
            private int testMaxHtmlTagAttributes = 64;

            /// <summary>
            /// The maximum offset for parsing restarting.
            /// </summary>
            private int testMaxHtmlRestartOffset = 4096;

            /// <summary>
            /// The limit for nested tags.
            /// </summary>
            private int testMaxHtmlNormalizerNesting = HtmlSupport.HtmlNestingLimit;

            /// <summary>
            /// The threshold for small CSS blocks.
            /// </summary>
            private int smallCssBlockThreshold = -1;

            /// <summary>
            /// Value indicating whether display styles should be reserved.
            /// </summary>
            private bool preserveDisplayNoneStyle;


            // Orphaned WPL code.
#if false
        private bool testConvertFragment;

            // <summary>
            // Value indicating whether no new lines should be included.
            // </summary>
            private bool testNoNewLines;
#endif

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
            // Orphaned WPL code.
#if false
        public System.Text.Encoding InputEncoding
        {
            get { return this.inputEncoding; }
            set { this.AssertNotLocked(); this.inputEncoding = value; }
        }
        
        public bool DetectEncodingFromByteOrderMark
        {
            get { return this.detectEncodingFromByteOrderMark; }
            set { this.AssertNotLocked(); this.detectEncodingFromByteOrderMark = value; }
        }
        
        public bool DetectEncodingFromMetaTag
        {
            get { return this.detectEncodingFromMetaTag; }
            set { this.AssertNotLocked(); this.detectEncodingFromMetaTag = value; }
        }

        public System.Text.Encoding OutputEncoding
        {
            get { return this.outputEncoding; }
            set
            {
                this.AssertNotLocked(); 
                this.outputEncoding = value;
                this.outputEncodingSameAsInput = (value == null);
            }
        }

        public HeaderFooterFormat HeaderFooterFormat
        {
            get { return this.injectionFormat; }
            set { this.AssertNotLocked(); this.injectionFormat = value; }
        }
        
        public string Header
        {
            get { return this.injectHead; }
            set { this.AssertNotLocked(); this.injectHead = value; }
        }
        
        public string Footer
        {
            get { return this.injectTail; }
            set { this.AssertNotLocked(); this.injectTail = value; }
        }
        
        public HtmlTagCallback HtmlTagCallback
        {
            get { return this.htmlCallback; }
            set { this.AssertNotLocked(); this.htmlCallback = value; }
        }
        
        public int MaxCallbackTagLength
        {
            get { return this.maxHtmlTagSize; }
            set { this.AssertNotLocked(); this.maxHtmlTagSize = value; }
        }

        internal HtmlToHtml SetInputEncoding(System.Text.Encoding value)
        {
            this.InputEncoding = value;
            return this;
        }

        internal HtmlToHtml SetDetectEncodingFromByteOrderMark(bool value)
        {
            this.DetectEncodingFromByteOrderMark = value;
            return this;
        }

        internal HtmlToHtml SetDetectEncodingFromMetaTag(bool value)
        {
            this.DetectEncodingFromMetaTag = value;
            return this;
        }

        internal HtmlToHtml SetOutputEncoding(System.Text.Encoding value)
        {
            this.OutputEncoding = value;
            return this;
        }

        internal HtmlToHtml SetNormalizeHtml(bool value)
        {
            this.NormalizeHtml = value;
            return this;
        }

        internal HtmlToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
        {
            this.HeaderFooterFormat = value;
            return this;
        }

        internal HtmlToHtml SetHeader(string value)
        {
            this.Header = value;
            return this;
        }

        internal HtmlToHtml SetFooter(string value)
        {
            this.Footer = value;
            return this;
        }

        internal HtmlToHtml SetFilterHtml(bool value)
        {
            this.FilterHtml = value;
            return this;
        }

        internal HtmlToHtml SetHtmlTagCallback(HtmlTagCallback value)
        {
            this.HtmlTagCallback = value;
            return this;
        }

        internal HtmlToHtml SetTestTruncateForCallback(bool value)
        {
            this.testTruncateForCallback = value;
            return this;
        }

        internal HtmlToHtml SetMaxCallbackTagLength(int value)
        {
            this.maxHtmlTagSize = value;
            return this;
        }

        internal HtmlToHtml SetInputStreamBufferSize(int value)
        {
            this.InputStreamBufferSize = value;
            return this;
        }

        internal HtmlToHtml SetOutputHtmlFragment(bool value)
        {
            this.OutputHtmlFragment = value;
            return this;
        }

        internal HtmlToHtml SetTestConvertHtmlFragment(bool value)
        {
            this.testConvertFragment = value;
            return this;
        }

        internal HtmlToHtml SetTestBoundaryConditions(bool value)
        {
            this.TestBoundaryConditions = value;
            if (value)
            {
                this.maxHtmlTagSize = 123;
                this.testMaxHtmlTagAttributes = 5;
                this.testMaxHtmlNormalizerNesting = 10;
            }
            return this;
        }

        internal HtmlToHtml SetTestMaxTokenRuns(int value)
        {
            this.testMaxTokenRuns = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceStream(Stream value)
        {
            this.testTraceStream = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceShowTokenNum(bool value)
        {
            this.testTraceShowTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceStopOnTokenNum(int value)
        {
            this.testTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceStream(Stream value)
        {
            this.testNormalizerTraceStream = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceShowTokenNum(bool value)
        {
            this.testNormalizerTraceShowTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceStopOnTokenNum(int value)
        {
            this.testNormalizerTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlTagAttributes(int value)
        {
            this.testMaxHtmlTagAttributes = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlRestartOffset(int value)
        {
            this.testMaxHtmlRestartOffset = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlNormalizerNesting(int value)
        {
            this.testMaxHtmlNormalizerNesting = value;
            return this;
        }

        internal HtmlToHtml SetTestNoNewLines(bool value)
        {
            this.testNoNewLines = value;
            return this;
        }

        internal HtmlToHtml SetSmallCssBlockThreshold(int value)
        {
            this.smallCssBlockThreshold = value;
            return this;
        }

        internal HtmlToHtml SetPreserveDisplayNoneStyle(bool value)
        {
            this.preserveDisplayNoneStyle = value;
            return this;
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

            // Orphaned WPL code.
#if false
        internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
        {
            this.inputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    converterWriter,
                                    true,
                                    this.maxHtmlTagSize,
                                    this.TestBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    false,      
                                    this.outputEncodingSameAsInput ? System.Text.Encoding.UTF8 : this.outputEncoding, 
                                    this.outputEncodingSameAsInput,
                                    this.TestBoundaryConditions,
                                    this as IResultsFeedback);

            return CreateChain(converterIn, converterOut, converterWriter as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
        {
            this.inputEncoding = Encoding.Unicode;
            this.outputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    converterWriter,
                                    true,
                                    this.maxHtmlTagSize,
                                    this.TestBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    false);     

            return CreateChain(converterIn, converterOut, converterWriter as IProgressMonitor);
        }
#endif
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
                    HtmlParser preParser = new HtmlParser(
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

                HtmlWriter writer = new HtmlWriter(
                                        output,
                                        this.filterHtml,
                                        this.normalizeInputHtml);

                // Orphaned WPL code.
#if false
            HtmlWriter writer = new HtmlWriter(
                                    output,
                                    this.filterHtml,
                                    this.normalizeInputHtml && !this.testNoNewLines);
#endif
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

                // Orphaned WPL code.
#if false
            return new HtmlToHtmlConverter(
                                    parser,
                                    writer,
                                    this.testConvertFragment,
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
#endif
            }
        }
    }
