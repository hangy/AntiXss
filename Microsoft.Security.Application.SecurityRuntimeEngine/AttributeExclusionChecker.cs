// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeExclusionChecker.cs" company="Microsoft Corporation">
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
//   A utility class for attribute based exclusion checks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.UI;

    /// <summary>
    /// Provides attribute exclusion checks
    /// </summary>
    public static class AttributeExclusionChecker
    {
        /// <summary>
        /// The lock to use when reading or writing from the type exclusion cache.
        /// </summary>
        private static readonly ReaderWriterLockSlim TypeLevelSyncLock = new ReaderWriterLockSlim();

        /// <summary>
        /// A list of types where type checking has already occurred.
        /// </summary>
        private static readonly List<Type> TypeExclusionCheckPerformedCache = new List<Type>();

        /// <summary>
        /// A list of type level exclusions.
        /// </summary>
        private static readonly List<string> TypeExclusionsCache = new List<string>();

        /// <summary>
        /// Gets a list of unique identifiers of Web Forms controls contained within a page which are
        /// excluded from processing by the specified plug-in.
        /// </summary>
        /// <param name="container">The instance of a <see cref="System.Web.UI.Page"/> to examine.</param>
        /// <param name="plugIn">The type of the plug-in to exclude.</param>
        /// <returns>A list of <see cref="System.Web.UI.Control.UniqueID"/>s of any excluded controls.</returns>
        public static IEnumerable<string> GetExcludedControlUniqueIdsForContainer(Control container, Type plugIn)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (plugIn == null)
            {
                throw new ArgumentNullException("plugIn");
            }

            List<string> excludedUniqueControlIds = new List<string>();

            if (!container.HasControls())
            {
                return excludedUniqueControlIds;
            }            

            List<FieldInfo> pageFieldInformationForWebControls = new List<FieldInfo>();

            // Extract the field information for the provided page and filter it to only those fields which inherit from System.Web.UI.Control.
            FieldInfo[] publicFieldInfo = container.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            pageFieldInformationForWebControls.AddRange(GetWebControlsWhichAreNotTypeExcluded(publicFieldInfo, plugIn));
            FieldInfo[] nonPublicFieldInfo = container.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            pageFieldInformationForWebControls.AddRange(GetWebControlsWhichAreNotTypeExcluded(nonPublicFieldInfo, plugIn));

            // Loop through each of the controls looking for any with the SupressProtectionAttribute applied to them.
            foreach (FieldInfo fieldInfo in pageFieldInformationForWebControls)
            {
                object[] attributes = Attribute.GetCustomAttributes(fieldInfo, typeof(SuppressProtectionAttribute), true);
                if (attributes.Length == 0)
                {
                    continue;
                }

                // Get the actual field instance.
                object fieldObject = fieldInfo.GetValue(container);

                // The Page class uses the composition pattern and contains a private field, Page which is itself.
                // We don't want that, otherwise we're going to get into all sorts of fun loops later on.
                // This should never happen - page level exclusion checks happen in the SRE code however better to be safe
                // than sorry.
                if (fieldObject == container)
                {
                    continue;
                }

                Control control = fieldObject as Control;

                if (control != null)
                {
                    excludedUniqueControlIds.AddRange(
                        from exclusionAttribute in attributes.OfType<SuppressProtectionAttribute>()
                        where exclusionAttribute.PlugInType == null || exclusionAttribute.PlugInType == plugIn
                        select control.UniqueID);
                }
            }

            return excludedUniqueControlIds;
        }

        /// <summary>
        /// Returns a value indicating whether the specified processor has been excluded from the specified page type.
        /// </summary>
        /// <param name="type">The type of the page to check.</param>
        /// <param name="plugIn">The processor type to check.</param>
        /// <returns><c>true</c> if the processor has been excluded, otherwise <c>false</c>.</returns>
        internal static bool IsPlugInExcludedForType(Type type, Type plugIn)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (plugIn == null)
            {
                throw new ArgumentNullException("plugIn");
            }

            if (!HasTypeLevelCheckingTakenPlace(type))
            {
                CacheAttributeBasedExclusions(type);
            }

            TypeLevelSyncLock.EnterReadLock();
            try
            {                
                return TypeExclusionsCache.Contains(type.AssemblyQualifiedName) ||                   // All plug-ins
                       TypeExclusionsCache.Contains(GetTypePlugInCombinationCacheKey(type, plugIn)); // Specified plug-in
            }
            finally
            {
                TypeLevelSyncLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns a subset of the specified <see cref="FieldInfo"/>s containing
        /// the fields which are web controls and are not excluded from processing by the plug-in, based on the field type.
        /// </summary>
        /// <param name="fields">The <see cref="FieldInfo"/>s to examine.</param>
        /// <param name="plugIn">The excluded plug-in.</param>
        /// <returns>A subset of the specified <see cref="FieldInfo"/>s which are web controls and are not excluded from plug-in processing based on their type.</returns>
        private static IEnumerable<FieldInfo> GetWebControlsWhichAreNotTypeExcluded(IEnumerable<FieldInfo> fields, Type plugIn)
        {
            return fields.Where(fieldInfo => fieldInfo.FieldType.IsSubclassOf(typeof(Control)) && !IsPlugInExcludedForType(fieldInfo.FieldType, plugIn)).ToList();
        }

        /// <summary>
        /// Returns a value indicating whether attribute checking has already taken place for the specified type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns><c>true</c> if page level checking has already taken place, otherwise <c>false</c>.</returns>
        private static bool HasTypeLevelCheckingTakenPlace(Type type)
        {
            TypeLevelSyncLock.EnterReadLock();
            try
            {
                return TypeExclusionCheckPerformedCache.Contains(type);
            }
            finally
            {
                TypeLevelSyncLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Extracts the attribute based exclusions for the specified type and caches them.
        /// </summary>
        /// <param name="type">The type to extract the exclusions from.</param>
        private static void CacheAttributeBasedExclusions(Type type)
        {
            TypeLevelSyncLock.EnterWriteLock();
            try
            {
                TypeExclusionCheckPerformedCache.Add(type);

                object[] attributeArray = type.GetCustomAttributes(typeof(SuppressProtectionAttribute), true);
                if (attributeArray.Length == 0)
                {
                    return;
                }
                
                foreach (SuppressProtectionAttribute exclusionAttribute in
                    attributeArray.OfType<SuppressProtectionAttribute>())
                {
                    TypeExclusionsCache.Add(GetTypePlugInCombinationCacheKey(type, exclusionAttribute.PlugInType));
                }
            }
            finally
            {
                TypeLevelSyncLock.ExitWriteLock();
            }            
        }

        /// <summary>
        /// Generates a cache key from the type and plug-in type provided.
        /// </summary>
        /// <param name="type">The type whose exclusions should be cached.</param>
        /// <param name="plugIn">The plug-in type to cache.</param>
        /// <returns>A cache key from the type and plug-in types provided.</returns>
        private static string GetTypePlugInCombinationCacheKey(Type type, Type plugIn)
        {
            return plugIn == null ? type.AssemblyQualifiedName : string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", type.AssemblyQualifiedName, plugIn.AssemblyQualifiedName);
        }
    }
}
