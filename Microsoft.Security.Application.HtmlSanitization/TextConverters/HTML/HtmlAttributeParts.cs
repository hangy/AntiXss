﻿// --------------------------------------------------------------------------------------------------------------------
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
        private readonly HtmlToken.AttrPartMajor major;
        private readonly HtmlToken.AttrPartMinor minor;

        internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }
    }
}