// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestParameterType.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
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
//   The type of the request parameter under inspection
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// The type of the request parameter under inspection
    /// </summary>
    public enum RequestParameterType
    {
        /// <summary>
        /// The parameter comes from the request query string.
        /// </summary>
        QueryString = 0,

        /// <summary>
        /// The parameter comes from a form in the request.
        /// </summary>
        Form = 1,

        /// <summary>
        /// The parameter comes from a cookie accompanying the request.
        /// </summary>
        Cookie = 2,

        /// <summary>
        /// The parameter comes from an HTTP header accompanying the request.
        /// </summary>
        HttpHeader = 3
    }
}
