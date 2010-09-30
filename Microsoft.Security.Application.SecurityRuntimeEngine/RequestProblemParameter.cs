// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProblemParameter.cs" company="Microsoft Corporation">
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
//   Represents a parameter in a request that is problematic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Globalization;

    /// <summary>
    /// Represents a parameter in a request that is problematic.
    /// </summary>
    public class RequestProblemParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProblemParameter"/> class.
        /// </summary>
        public RequestProblemParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProblemParameter"/> class.
        /// </summary>
        /// <param name="parameterType">The type of the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public RequestProblemParameter(RequestParameterType parameterType, string name, string value)
        {
            this.ParameterType = parameterType;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public RequestParameterType ParameterType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Parameter Type: {0}, Name: {1}, Value: {2}",
                this.ParameterType,
                this.Name,
                this.Value);
        }
    }
}
