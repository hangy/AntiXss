// ***************************************************************
// <copyright file="HtmlToHtml.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    
    
    
    internal class HtmlToHtml : TextConverter
    {
        private System.Text.Encoding inputEncoding = null;
        private bool detectEncodingFromByteOrderMark = true;
        private bool detectEncodingFromMetaTag = true;

        private System.Text.Encoding outputEncoding = null;
        private bool outputEncodingSameAsInput = true;

        private bool normalizeInputHtml = false;

        private HeaderFooterFormat injectionFormat = HeaderFooterFormat.Text;
        private string injectHead = null;
        private string injectTail = null;

        

        private bool filterHtml = false;
        private HtmlTagCallback htmlCallback = null;
        
        private bool testTruncateForCallback = true;

        private bool testConvertFragment;
        private bool outputFragment;

        private int testMaxTokenRuns = 512;

        private Stream testTraceStream = null;
        private bool testTraceShowTokenNum = true;
        private int testTraceStopOnTokenNum = 0;

        private Stream testNormalizerTraceStream = null;
        private bool testNormalizerTraceShowTokenNum = true;
        private int testNormalizerTraceStopOnTokenNum = 0;

        private int maxHtmlTagSize = 32768;

        private int testMaxHtmlTagAttributes = 64;
        private int testMaxHtmlRestartOffset = 4096;

        
        private int testMaxHtmlNormalizerNesting = HtmlSupport.HtmlNestingLimit;

        private int smallCssBlockThreshold = -1;        
        private bool preserveDisplayNoneStyle = false;  

        private bool testNoNewLines;

        

        
        
        
        
        
        
        
        public HtmlToHtml()
        {
        }

        
        

        
        
        
        
        
        
        
        
        
        
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

        
        
        
        
        
        
        
        public bool NormalizeHtml
        {
            get { return this.normalizeInputHtml; }
            set { this.AssertNotLocked(); this.normalizeInputHtml = value; }
        }

#if M3STUFF
        
        
        
        
        
        public IHtmlParsingCallback HtmlParsingCallback
        {
            get { return this.parsingCallback; }
            set { this.AssertNotLocked(); this.parsingCallback = value; }
        }
#endif

        
        

        
        
        
        
        
        
        
        
        
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

        
        
        
        
        
        
        public bool OutputHtmlFragment
        {
            get { return this.outputFragment; }
            set { this.AssertNotLocked(); this.outputFragment = value; }
        }

        
        
        
        
        
        
        public bool FilterHtml
        {
            get { return this.filterHtml; }
            set { this.AssertNotLocked(); this.filterHtml = value; }
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

#if M3STUFF
        
        
        
        
        
        
        public HtmlFilterTables HtmlFilterTables
        {
            get { return this.filterTables; }
            set { this.AssertNotLocked(); this.filterTables = value; }
        }
#endif

        
        

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
            this.testBoundaryConditions = value;
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
                                    this.testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    true,       
                                    this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, 
                                    this.outputEncodingSameAsInput,
                                    this.testBoundaryConditions,
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
                                    this.testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    true);      

            return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
        {
            this.inputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    converterWriter,
                                    true,
                                    this.maxHtmlTagSize,
                                    this.testBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    false,      
                                    this.outputEncodingSameAsInput ? System.Text.Encoding.UTF8 : this.outputEncoding, 
                                    this.outputEncodingSameAsInput,
                                    this.testBoundaryConditions,
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
                                    this.testBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    false);     

            return CreateChain(converterIn, converterOut, converterWriter as IProgressMonitor);
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
                                    this.testBoundaryConditions,
                                    this as IResultsFeedback,
                                    converterStream as IProgressMonitor);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    converterStream,
                                    false,      
                                    true,       
                                    this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, 
                                    this.outputEncodingSameAsInput,
                                    this.testBoundaryConditions,
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
                                    this.testBoundaryConditions,
                                    converterStream as IProgressMonitor);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    converterStream,
                                    false,      
                                    false,      
                                    this.outputEncodingSameAsInput ? System.Text.Encoding.UTF8 : this.outputEncoding, 
                                    this.outputEncodingSameAsInput,
                                    this.testBoundaryConditions,
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
                                    this.testBoundaryConditions,
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
                                    this.testBoundaryConditions,
                                    converterReader as IProgressMonitor);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    converterReader,
                                    false,      
                                    false);     

            return CreateChain(converterIn, converterOut, converterReader as IProgressMonitor);
        }

        

        private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
        {
            this.locked = true;

            HtmlInjection injection = null;

            if (this.injectHead != null || this.injectTail != null)
            {
                injection = new HtmlInjection(
                            this.injectHead,
                            this.injectTail,
                            this.injectionFormat,
                            this.filterHtml,
                            this.htmlCallback,
                            this.testBoundaryConditions,
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
                                        this.testBoundaryConditions);

                parser = new HtmlNormalizingParser(
                                        preParser,
                                        injection,
                                        this.htmlCallback != null,      
                                        this.testMaxHtmlNormalizerNesting,
                                        this.testBoundaryConditions,
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
                                        this.testBoundaryConditions);
            }

            HtmlWriter writer = new HtmlWriter(
                                    output,
                                    this.filterHtml,
                                    this.normalizeInputHtml && !this.testNoNewLines);

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
        }
        
        

        internal override void SetResult(ConfigParameter parameterId, object val)
        {
            switch (parameterId)
            {
                case ConfigParameter.InputEncoding:
                        this.inputEncoding = (System.Text.Encoding) val;
                        break;

                case ConfigParameter.OutputEncoding:
                        this.outputEncoding = (System.Text.Encoding) val;
                        break;
            }

            base.SetResult(parameterId, val);
        }
    }
}

