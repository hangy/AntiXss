// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResponseInspector.cs" company="Microsoft Corporation">
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
//   Defines methods that must be implemented for response inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Web;

    /// <summary>
    /// Defines methods that must be implemented for response inspection.
    /// </summary>
    public interface IResponseInspector : ISecurityRuntimePlugIn
    {
        /// <summary>
        /// Inspects an HTTP response for potential problems.
        /// </summary>
        /// <param name="request">The request which triggered this response.</param>
        /// <param name="contentType">The content type of the response.</param>
        /// <param name="response">A byte array containing the response to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        /// <remarks>
        /// The response is writable - any changes made to it will be sent to the client.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1045:DoNotPassTypesByReference", 
            MessageId = "2#",
            Justification = "This enables changes to the response. WCF does this with its inspectors.")]
        IInspectionResult Inspect(HttpRequestBase request, string contentType, ref byte[] response);
    }
}
