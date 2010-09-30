// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdapterFactory.cs" company="Microsoft Corporation">
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
//   Converts an inspector plugin to a wrapped adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    
    using PlugIns;

    using Properties;

    /// <summary>
    /// The type of adapter that <see cref="AdapterFactory"/> should created.
    /// </summary>
    internal enum InspectorConversionTarget
    {
        /// <summary>
        /// Selects a <see cref="RequestInspectorAdapter"/>
        /// </summary>
        RequestInspector = 0,

        /// <summary>
        /// Selects a <see cref="ResponseHeaderInspectorAdapter"/>
        /// </summary>
        ResponseHeaderInspector = 1,

        /// <summary>
        /// Selects a <see cref="PageInspectorAdapter"/>
        /// </summary>        
        PageInspector = 2
    }

    /// <summary>
    /// Converts an inspector plug-in to a wrapped adapter class.
    /// </summary>
    internal static class AdapterFactory
    {
        /// <summary>
        /// Converts an <see cref="ISecurityRuntimePlugIn"/> to its adapted types to enable inspection.
        /// </summary>
        /// <param name="plugIn">The plug-in to convert.</param>
        /// <param name="conversionTarget">The target for the conversion.</param>
        /// <returns>An <see cref="IInspector"/> wrapping the specified plug-in.</returns>
        public static IInspector Convert(ISecurityRuntimePlugIn plugIn, InspectorConversionTarget conversionTarget)
        {
            IRequestInspector requestInspector = plugIn as IRequestInspector;
            IResponseHeaderInspector responseHeaderInspector = plugIn as IResponseHeaderInspector;
            IPageInspector pageInspector = plugIn as IPageInspector;

            if (conversionTarget == InspectorConversionTarget.RequestInspector && requestInspector != null)
            {
                return new RequestInspectorAdapter(requestInspector);
            }

            if (conversionTarget == InspectorConversionTarget.ResponseHeaderInspector && responseHeaderInspector != null)
            {
                return new ResponseHeaderInspectorAdapter(responseHeaderInspector);
            }

            if (conversionTarget == InspectorConversionTarget.PageInspector && pageInspector != null)
            {
                return new PageInspectorAdapter(pageInspector);
            }

            throw new ArgumentOutOfRangeException("plugIn", Resources.CannotConvertPluginExceptionMessage);
        }
    }
 }
