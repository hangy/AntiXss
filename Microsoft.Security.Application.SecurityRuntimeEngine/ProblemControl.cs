// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProblemControl.cs" company="Microsoft Corporation">
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
//   Details of a page control which has caused a problem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Collections.Specialized;
    using System.Text;

    /// <summary>
    /// Details of a page control which has caused a problem.
    /// </summary>
    public class ProblemControl
    {
        /// <summary>
        /// The property name and value that caused the problem
        /// </summary>
        private readonly NameValueCollection properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemControl"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="id">The control id.</param>
        /// <param name="controlType">The control type.</param>
        public ProblemControl(string name, string id, string controlType)
            : this(name, id, controlType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemControl"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="id">The control id.</param>
        /// <param name="controlType">The control type.</param>
        /// <param name="problemProperties">The properties which are problematic.</param>
        public ProblemControl(string name, string id, string controlType, NameValueCollection problemProperties)
        {
            this.Name = name;
            this.Id = id;
            this.ControlType = controlType;

            if (problemProperties != null && problemProperties.HasKeys())
            {
                this.properties = new NameValueCollection(problemProperties);
            }
        }

        /// <summary>
        /// Gets or sets the name of the problem control.
        /// </summary>
        /// <value>The name of the problem control.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the problem control.
        /// </summary>
        /// <value>The id of the problem control.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the problem control.
        /// </summary>
        /// <value>The type of the problem control.</value>
        public string ControlType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the problem properties.
        /// </summary>
        /// <value>The problem properties.</value>
        public NameValueCollection ProblemProperties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Control Name: {0}\n", this.Name);
            stringBuilder.AppendFormat("Control Id: {0}\n", this.Id);
            stringBuilder.AppendFormat("Control Type: {0}\n", this.ControlType);
            if (this.ProblemProperties.HasKeys())
            {
                stringBuilder.Append("Problem Properties:\n");
                foreach (string propertyName in this.ProblemProperties)
                {
                    stringBuilder.AppendFormat(
                        "Name : {0} Value : {1}\n",
                        propertyName,
                        this.ProblemProperties[propertyName]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
