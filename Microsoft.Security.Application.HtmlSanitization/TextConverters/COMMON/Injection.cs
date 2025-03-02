// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Injection.cs" company="Microsoft Corporation">
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
    using Microsoft.Exchange.Data.TextConverters.Internal.Format;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;

    internal abstract class Injection : IDisposable
    {
        protected HeaderFooterFormat injectionFormat;

        protected string injectHead;
        protected string injectTail;

        protected bool headInjected;
        protected bool tailInjected;

        protected bool testBoundaryConditions;
        protected Stream traceStream;

        public HeaderFooterFormat HeaderFooterFormat { get { return this.injectionFormat; } }

        public bool HaveHead { get { return this.injectHead != null; } }
        public bool HaveTail { get { return this.injectTail != null; } }

        public bool HeadDone { get { return this.headInjected; } }
        public bool TailDone { get { return this.tailInjected; } }

        public abstract void Inject(bool head, TextOutput output);

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void Reset()
        {
            this.headInjected = false;
            this.tailInjected = false;
        }


        public abstract void InjectRtfFonts(int firstAvailableFontHandle);
        public abstract void InjectRtfColors(int nextColorIndex);
    }
}

