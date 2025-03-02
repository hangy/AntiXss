// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlInjection.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//    
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;

    internal class HtmlInjection : Injection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in method call.  Not changing API at this time.")]
        protected bool filterHtml;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="Used in method call.  Not changing API at this time.")]
        protected HtmlTagCallback callback;

        protected bool injectingHead;

        protected IProgressMonitor progressMonitor;

        protected IHtmlParser documentParser;
        protected HtmlParser fragmentParser;

        protected HtmlToHtmlConverter fragmentToHtmlConverter;
        protected HtmlToTextConverter fragmentToTextConverter;

        public HtmlInjection(
                    string injectHead,
                    string injectTail,
                    HeaderFooterFormat injectionFormat,
                    bool filterHtml,
                    HtmlTagCallback callback,
                    bool testBoundaryConditions,
                    Stream traceStream,
                    IProgressMonitor progressMonitor)
        {
            InternalDebug.Assert(progressMonitor != null);
            this.injectHead = injectHead;
            this.injectTail = injectTail;
            this.injectionFormat = injectionFormat;
            this.filterHtml = filterHtml;
            this.callback = callback;
            this.testBoundaryConditions = testBoundaryConditions;
#if DEBUG
            this.traceStream = traceStream;
#endif
            this.progressMonitor = progressMonitor;
        }

        public bool Active { get { return this.documentParser != null; } }
        public bool InjectingHead { get { InternalDebug.Assert(this.Active); return this.injectingHead; } }

        public IHtmlParser Push(bool head, IHtmlParser documentParser)
        {
            if (head)
            {
                if (this.injectHead != null && !this.headInjected)
                {
                    this.documentParser = documentParser;

                    if (this.fragmentParser == null)
                    {
                        this.fragmentParser = new HtmlParser(
                                        new ConverterBufferInput(this.injectHead, this.progressMonitor),
                                        false,
                                        (this.injectionFormat == HeaderFooterFormat.Text),
                                        64,
                                        8,
                                        this.testBoundaryConditions);
                    }
                    else
                    {
                        this.fragmentParser.Initialize(
                                    this.injectHead,
                                    (this.injectionFormat == HeaderFooterFormat.Text));
                    }

                    this.injectingHead = true;

                    return this.fragmentParser;
                }
            }
            else
            {
                if (this.injectHead != null && !this.headInjected)
                {
                    InternalDebug.Assert(false);

                    this.headInjected = true;
                }

                if (this.injectTail != null && !this.tailInjected)
                {
                    this.documentParser = documentParser;

                    if (this.fragmentParser == null)
                    {
                        this.fragmentParser = new HtmlParser(
                                    new ConverterBufferInput(this.injectTail, this.progressMonitor),
                                    false,
                                    (this.injectionFormat == HeaderFooterFormat.Text),
                                    64,
                                    8,
                                    this.testBoundaryConditions);
                    }
                    else
                    {
                        this.fragmentParser.Initialize(
                                    this.injectTail,
                                    (this.injectionFormat == HeaderFooterFormat.Text));
                    }

                    this.injectingHead = false;

                    return this.fragmentParser;
                }
            }

            return documentParser;
        }

        public IHtmlParser Pop()
        {
            InternalDebug.Assert(this.Active);

            if (this.injectingHead)
            {
                this.headInjected = true;

                if (this.injectTail == null)
                {
                    ((IDisposable)this.fragmentParser).Dispose();
                    this.fragmentParser = null;
                }
            }
            else
            {
                this.tailInjected = true;

                ((IDisposable)this.fragmentParser).Dispose();
                this.fragmentParser = null;
            }

            IHtmlParser parser = this.documentParser;
            this.documentParser = null;

            return parser;
        }

        public override void Inject(bool head, TextOutput output)
        {
            HtmlParser parser;

            if (head)
            {
                if (this.injectHead != null && !this.headInjected)
                {
                    parser = new HtmlParser(
                                        new ConverterBufferInput(this.injectHead, this.progressMonitor),
                                        false,
                                        (this.injectionFormat == HeaderFooterFormat.Text),
                                        64,
                                        8,
                                        this.testBoundaryConditions);

                    this.fragmentToTextConverter = new HtmlToTextConverter(
                                        parser,
                                        output,
                                        null,
                                        true,
                                        this.injectionFormat == HeaderFooterFormat.Text,
                                        false,
                                        this.traceStream,
                                        true,
                                        0);

                    while (!this.fragmentToTextConverter.Flush())
                    {
                    }

                    this.headInjected = true;

                    if (this.injectTail == null)
                    {
                        ((IDisposable)this.fragmentToTextConverter).Dispose();
                        this.fragmentToTextConverter = null;
                    }
                }
            }
            else
            {
                if (this.injectHead != null && !this.headInjected)
                {
                    InternalDebug.Assert(false);

                    this.headInjected = true;
                }

                if (this.injectTail != null && !this.tailInjected)
                {
                    if (this.fragmentToTextConverter == null)
                    {
                        parser = new HtmlParser(
                                        new ConverterBufferInput(this.injectTail, this.progressMonitor),
                                        false,
                                        (this.injectionFormat == HeaderFooterFormat.Text),
                                        64,
                                        8,
                                        this.testBoundaryConditions);

                        this.fragmentToTextConverter = new HtmlToTextConverter(
                                        parser,
                                        output,
                                        null,
                                        true,
                                        this.injectionFormat == HeaderFooterFormat.Text,
                                        false,
                                        this.traceStream,
                                        true,
                                        0);
                    }
                    else
                    {
                        this.fragmentToTextConverter.Initialize(
                                        this.injectTail,
                                        (this.injectionFormat == HeaderFooterFormat.Text));
                    }

                    while (!this.fragmentToTextConverter.Flush())
                    {
                    }

                    ((IDisposable)this.fragmentToTextConverter).Dispose();
                    this.fragmentToTextConverter = null;

                    this.tailInjected = true;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.fragmentToHtmlConverter != null)
            {
                ((IDisposable)this.fragmentToHtmlConverter).Dispose();
                this.fragmentToHtmlConverter = null;
            }

            if (this.fragmentToTextConverter != null)
            {
                ((IDisposable)this.fragmentToTextConverter).Dispose();
                this.fragmentToTextConverter = null;
            }
#if false
            if (this.fragmentToRtfConverter != null)
            {
                ((IDisposable)this.fragmentToRtfConverter).Dispose();
                this.fragmentToRtfConverter = null;
            }
#endif
            if (this.fragmentParser != null)
            {
                ((IDisposable)this.fragmentParser).Dispose();
                this.fragmentParser = null;
            }

            base.Reset();
            base.Dispose(disposing);
        }



        public override void InjectRtfFonts(int firstAvailableFontHandle)
        {
        }

        public override void InjectRtfColors(int nextColorIndex)
        {
        }



    }






















































































}

