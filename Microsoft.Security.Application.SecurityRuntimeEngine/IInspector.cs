// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInspector.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Adapter Interface for all inspectors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// Provides the adapter Interface for all inspectors.
    /// </summary>
    internal interface IInspector
    {
        /// <summary>
        /// Inspects the specified request, response or page.
        /// </summary>
        /// <param name="request">The request to inspect.</param>
        /// <param name="response">The response to inspect.</param>
        /// <param name="page">The page to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        IInspectionResult Inspect(HttpRequestBase request, HttpResponseBase response, Page page);
    }
}
