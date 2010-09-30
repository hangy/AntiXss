// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameConfigurationElementCollection.cs" company="Microsoft Corporation">
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
//   A configuration collection of Name configuration elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// A configuration collection of Name configuration elements.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1010:CollectionsShouldImplementGenericInterface",
        Justification = "As this is a ConfigurationElementCollection it is accessed through the configuration API and the ICollection<> interface would never be used.")]
    [ConfigurationCollection(typeof(NameConfigurationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public sealed class NameConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the <see cref="NameConfigurationElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero based index position.</param>
        public NameConfigurationElement this[int index]
        {
            get
            {
                return BaseGet(index) as NameConfigurationElement;
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Returns the key for the specified configuration element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the specified configuration element.</returns>
        public static string GetKey(ConfigurationElement element)
        {
            return ((NameConfigurationElement)element).Name.ToUpperInvariant();
        }

        /// <summary>
        /// Returns a zero based position in the collection for the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>Zero based position for the specified name.</returns>
        public int IndexOf(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            return this.IndexOf((NameConfigurationElement)BaseGet(name.ToUpperInvariant()));
        }

        /// <summary>
        /// Returns a value indicating if the specified name is contained in the collection
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns><c>true</c> if the name is in the collection, otherwise <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return this.IndexOf(name) >= 0;
        }

        /// <summary>
        /// Returns a zero based position in the collection for the specified <see cref="NameConfigurationElement"/>
        /// </summary>
        /// <param name="item">The <see cref="NameConfigurationElement"/>to find.</param>
        /// <returns>Zero based position of the supplied <see cref="NameConfigurationElement"/>.</returns>
        public int IndexOf(NameConfigurationElement item)
        {
            if (item != null)
            {
                return BaseIndexOf(item);
            }

            return -1;
        }

        /// <summary>
        /// Removes an element from the collection based on the specified key.
        /// </summary>
        /// <param name="key">Key of the element</param>
        public void Remove(object key)
        {
            this.BaseRemove(key);
        }

        /// <summary>
        /// Removes all configuration elements from the collection.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
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
        /// Creates and returns a new child element for the collection.
        /// </summary>
        /// <returns>A new instance of <see cref="ConfigurationElement"/>.</returns>
        public ConfigurationElement CreateChild()
        {
            return this.CreateNewElement();
        }

        /// <summary>
        /// Adds the specified <see cref="NameConfigurationElement"/>.
        /// </summary>
        /// <param name="nameConfigurationElement">The <see cref="NameConfigurationElement"/> to add.</param>
        public void Add(NameConfigurationElement nameConfigurationElement)
        {
            this.BaseAdd(nameConfigurationElement);
        }

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to add.</param>
        protected override void
            BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
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
            return ((NameConfigurationElement)element).Name.ToUpperInvariant();
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new NameConfigurationElement();
        }
    }
}

