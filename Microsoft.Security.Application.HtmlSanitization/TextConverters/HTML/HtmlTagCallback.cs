// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTagCallback.cs" company="Microsoft Corporation">
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
//    Delegate callback definition for the HTML tag.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    /// <summary>
    /// Delegate callback definition for the HTML tag.
    /// </summary>
    /// <param name="tagContext">An instance fo the HtmlTagContext object.</param>
    /// <param name="htmlWriter">An instance fo the HtmlWriter object.</param>
    internal delegate void HtmlTagCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter);
}