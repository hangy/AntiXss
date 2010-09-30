// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestInspector.cs" company="Microsoft Corporation">
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
//   Defines methods that must be implemented for request inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Web;

    /// <summary>
    /// Defines methods that must be implemented for request inspection.
    /// </summary>
    public interface IRequestInspector : ISecurityRuntimePlugIn
    {
        /// <summary>
        /// Inspects an HTTP request for potential problems.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestBase"/> to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        IInspectionResult Inspect(HttpRequestBase request);
    }
}
