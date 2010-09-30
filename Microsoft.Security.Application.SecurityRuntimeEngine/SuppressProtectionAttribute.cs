// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuppressProtectionAttribute.cs" company="Microsoft Corporation">
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
//   Suppresses a page or control from processing by an SRE plug-in.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;

    /// <summary>
    /// Suppresses a page or control from processing by an SRE plug-in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SuppressProtectionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressProtectionAttribute"/> class.
        /// </summary>
        public SuppressProtectionAttribute()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressProtectionAttribute"/> class.
        /// </summary>
        /// <param name="plugInType">Type of plug-in to suppress</param>
        public SuppressProtectionAttribute(Type plugInType) 
        {
            this.PlugInType = plugInType;
        }

        /// <summary>
        /// Gets the processor type to exclude.
        /// </summary>
        public Type PlugInType
        {
            get;
            private set;
        }
    }
}
