// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutboundCodepageDetector.cs" company="Microsoft Corporation">
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
//   Detects the code page for outbound data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.IO;
    using System.Linq;

    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    /// <summary>
    /// Value indidicating which fallback exceptions should be allowed.
    /// </summary>      
    internal enum FallbackExceptions
    {
        /// <summary>
        /// No fallback exceptions are allowed.
        /// </summary>
        None,
        
        /// <summary>
        /// Common fallback exceptions are allowed.
        /// </summary>
        Common,
        
        /// <summary>
        /// All fallback exceptions are allowed.
        /// </summary>
        All
    }
}

