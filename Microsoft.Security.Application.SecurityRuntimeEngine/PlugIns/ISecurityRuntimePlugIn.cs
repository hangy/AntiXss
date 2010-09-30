// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityRuntimePlugIn.cs" company="Microsoft Corporation">
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
//   Defines methods and properties that must be implemented for any plug-in to the Security Runtime engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    /// <summary>
    /// Defines methods and properties that must be implemented for any plug-in to the Security Runtime engine.
    /// </summary>
    public interface ISecurityRuntimePlugIn
    {
        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>
        /// The list of excluded paths for the plug-in.
        /// </value>
        ExcludedPathCollection ExcludedPaths
        {
            get;
        }
    }
}
