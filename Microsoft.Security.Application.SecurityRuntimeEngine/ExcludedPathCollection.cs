// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludedPathCollection.cs" company="Microsoft Corporation">
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
//   A collection of excluded paths sourced from configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// A collection of excluded paths sourced from configuration.
    /// </summary>
    [ConfigurationCollection(typeof(ExcludedPath), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ExcludedPathCollection : ConfigurationElementCollection, ICollection<ExcludedPath>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public new bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the <see cref="ExcludedPath"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero based index position.</param>
        public ExcludedPath this[int index]
        {
            get
            {
                return BaseGet(index) as ExcludedPath;
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }        

        /// <summary>
        /// Returns the key for the specified configuration element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the specified configuration element.</returns>
        public static string GetKey(ConfigurationElement element)
        {
            return ((ExcludedPath)element).Path.ToUpperInvariant();
        }

        /// <summary>
        /// Returns a zero based position in the collection for the specified virtual path.
        /// </summary>
        /// <param name="path">Virtual path of the page. Ex: /somedirectory/somepage.aspx</param>
        /// <returns>Zero based position for the supplied page path.</returns>
        public int IndexOf(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return -1;
            }

            return this.IndexOf((ExcludedPath)BaseGet(path.ToUpperInvariant()));
        }

        /// <summary>
        /// Returns a zero based position in the collection for the specified ExclusionElement object.
        /// </summary>
        /// <param name="item">ExclusionElement to find.</param>
        /// <returns>Zero based position of the supplied ExclusionElement.</returns>
        public int IndexOf(ExcludedPath item)
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
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(ExcludedPath item)
        {
            this.BaseAdd(item);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(string item)
        {
            this.BaseAdd(new ExcludedPath(item));
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }

        /// <summary>
        /// Determines whether the collection contains the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ExcludedPath item)
        {
            return this.IndexOf(item) >= 0;
        }

        /// <summary>
        /// Copies the contents of the <see cref="ExcludedPathCollection "/> to an array.
        /// </summary>
        /// <param name="array">Array to which to copy the contents of the <see cref="ExcludedPathCollection "/>.</param>
        /// <param name="arrayIndex">Index location at which to begin copying.</param>
        public void CopyTo(ExcludedPath[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item was removed, otherwise <c>false</c></returns>
        public bool Remove(ExcludedPath item)
        {
            if (!this.Contains(item))
            {
                return false;
            }

            this.BaseRemove(item);

            return true;
        }

        /// <summary>
        /// Gets an enumerator which is used to iterate through the <see cref="ExcludedPathCollection "/>.
        /// </summary>
        /// <returns>An enumerator which is used to iterate through the <see cref="ExcludedPathCollection "/>.</returns>
        public new IEnumerator<ExcludedPath> GetEnumerator()
        {
            foreach (ExcludedPath excludedPath in this)
            {
                yield return excludedPath;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ExcludedPath"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="ExcludedPath"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExcludedPath();
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
            return ((ExcludedPath)element).Path.ToUpperInvariant();
        }
    }
}
