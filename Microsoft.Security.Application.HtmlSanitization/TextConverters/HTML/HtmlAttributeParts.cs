// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlAttributeParts.cs" company="Microsoft Corporation">
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
    using Internal.Html;
    using Security.Application.TextConverters.HTML;

    internal struct HtmlAttributeParts
    {
        private HtmlToken.AttrPartMajor major;
        private HtmlToken.AttrPartMinor minor;

        internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        // Orphaned WPL code.
#if false
        public bool Begin { get { return HtmlToken.AttrPartMajor.Begin == (this.major & HtmlToken.AttrPartMajor.Begin); } }
        public bool End { get { return HtmlToken.AttrPartMajor.End == (this.major & HtmlToken.AttrPartMajor.End); } }
        public bool Complete { get { return HtmlToken.AttrPartMajor.Complete == this.major; } }

        public bool NameBegin { get { return HtmlToken.AttrPartMinor.BeginName == (this.minor & HtmlToken.AttrPartMinor.BeginName); } }
        public bool Name { get { return HtmlToken.AttrPartMinor.ContinueName == (this.minor & HtmlToken.AttrPartMinor.ContinueName); } }
        public bool NameEnd { get { return HtmlToken.AttrPartMinor.EndName == (this.minor & HtmlToken.AttrPartMinor.EndName); } }
        public bool NameComplete { get { return HtmlToken.AttrPartMinor.CompleteName == (this.minor & HtmlToken.AttrPartMinor.CompleteName); } }

        public bool ValueBegin { get { return HtmlToken.AttrPartMinor.BeginValue == (this.minor & HtmlToken.AttrPartMinor.BeginValue); } }
        public bool Value { get { return HtmlToken.AttrPartMinor.ContinueValue == (this.minor & HtmlToken.AttrPartMinor.ContinueValue); } }
        public bool ValueEnd { get { return HtmlToken.AttrPartMinor.EndValue == (this.minor & HtmlToken.AttrPartMinor.EndValue); } }
        public bool ValueComplete { get { return HtmlToken.AttrPartMinor.CompleteValue == (this.minor & HtmlToken.AttrPartMinor.CompleteValue); } }

        public override string ToString()
        {
            return this.major.ToString() + " /" + this.minor.ToString() + "/";
        }
#endif
    }
}