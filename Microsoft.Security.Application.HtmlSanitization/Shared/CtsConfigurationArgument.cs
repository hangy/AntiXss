// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtsConfigurationArgument.cs" company="Microsoft Corporation">
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
//   Contains a configuration argument and its value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    /// <summary>
    /// Contains a configuration argument and its value.
    /// </summary>
    internal class CtsConfigurationArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtsConfigurationArgument"/> class.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        internal CtsConfigurationArgument(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the argument name.
        /// </summary>
        /// <value>The argument name.</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <value>The argument value.</value>
        public string Value
        {
            get;
            private set;
        }
    }
}