// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisabledPlugInCollection.cs" company="Microsoft Corporation">
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
//   A configuration collection of DisabledPlugIn entries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// A configuration collection of <see cref="DisabledPlugIn"/> entries.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1010:CollectionsShouldImplementGenericInterface",
        Justification = "As this is a ConfigurationElementCollection it is accessed through the configuration API and the ICollection<> interface would never be used.")]
    [ConfigurationCollection(typeof(DisabledPlugIn), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    internal sealed class DisabledPlugInCollection : ConfigurationElementCollection, IEnumerable<DisabledPlugIn>
    {
        /// <summary>
        /// Gets the enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator for this collection</returns>
        public new IEnumerator<DisabledPlugIn> GetEnumerator()
        {
            foreach (DisabledPlugIn item in this as IEnumerable)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Count > 0 ? base.GetHashCode() : 0;
        }

        /// <summary>
        /// Creates a new <see cref="DisabledPlugIn"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="DisabledPlugIn"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DisabledPlugIn();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DisabledPlugIn)element).Name;
        }
    }
}
