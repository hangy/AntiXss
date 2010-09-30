// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Microsoft.Exchange.CtsResources.GlobalizationStrings.cs" company="Microsoft Corporation">
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
//   Strings used for globalization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.CtsResources
{
    using System.Resources;

    /// <summary>
    /// Strings used for globalization.
    /// </summary>
    internal static class GlobalizationStrings
    {
        /// <summary>
        /// The resource manager for the globalization strings resources.
        /// </summary>
        private static readonly ResourceManager ResourceManager =
            new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", typeof(GlobalizationStrings).Assembly);

        /// <summary>
        /// Resource identifiers
        /// </summary>
        public enum ResourceIdentifier
        {
            /// <summary>
            /// The maximum number of characters cannot be negative.
            /// </summary>
            MaxCharactersCannotBeNegative,

            /// <summary>
            /// The code page priority list includes a code page which cannot be detected.
            /// </summary>
            PriorityListIncludesNonDetectableCodePage,

            /// <summary>
            /// Index out of range.
            /// </summary>
            IndexOutOfRange,

            /// <summary>
            /// The count is too large.
            /// </summary>
            CountTooLarge,

            /// <summary>
            /// The offset is out of range.
            /// </summary>
            OffsetOutOfRange,

            /// <summary>
            /// The count is out of range.
            /// </summary>
            CountOutOfRange
        }

        /// <summary>
        /// Parameter identifiers
        /// </summary>
        public enum ParameterIdentifier
        {
            /// <summary>
            /// Invalid character set.
            /// </summary>
            InvalidCharset,

            /// <summary>
            /// Invalid locale identifier.
            /// </summary>
            InvalidLocaleId,

            /// <summary>
            /// The code page is not installed.
            /// </summary>
            NotInstalledCodePage,

            /// <summary>
            /// The character set is not installed.
            /// </summary>
            NotInstalledCharset,

            /// <summary>
            /// The code page is invalid.
            /// </summary>
            InvalidCodePage,

            /// <summary>
            /// The code page and the character set are not installed.
            /// </summary>
            NotInstalledCharsetCodePage,

            /// <summary>
            /// The culture name is invalid.
            /// </summary>
            InvalidCultureName
        }

// Orphaned WPL code.
#if false
        /// <summary>
        /// Gets the string for the max characters cannot be negative error.
        /// </summary>
        /// <value>A string for the max characters cannot be negative error.</value>
        public static string MaxCharactersCannotBeNegative
        {
            get
            {
                return ResourceManager.GetString("MaxCharactersCannotBeNegative");
            }
        }

        /// <summary>
        /// Gets the string for the priortity list includes non-detectable code page error.
        /// </summary>
        /// <value>
        /// A string for the priortity list includes non-detectable code page error.
        /// </value>
        public static string PriorityListIncludesNonDetectableCodePage
        {
            get
            {
                return ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");
            }
        }

        /// <summary>
        /// Gets the string for the Offset Out Of Range error.
        /// </summary>
        /// <returns>The Offset Out Of Range error string.</returns>
        public static string OffsetOutOfRange
        {
            get
            {
                return ResourceManager.GetString("OffsetOutOfRange");
            }
        }

        /// <summary>
        /// Gets the string for the index out of range error.
        /// </summary>
        /// <value>The index out of range error string.</value>
        public static string IndexOutOfRange
        {
            get
            {
                return ResourceManager.GetString("IndexOutOfRange");
            }
        }

        /// <summary>
        /// Gets the string for the Count Too Large error.
        /// </summary>
        /// <returns>The Count Too Large error string.</returns>
        public static string CountTooLarge
        {
            get
            {
                return ResourceManager.GetString("CountTooLarge");
            }
        }

        /// <summary>
        /// Gets the string for the Count Out Of Range error.
        /// </summary>
        /// <returns>The Count Out of Range error string.</returns>
        public static string CountOutOfRange
        {
            get
            {
                return ResourceManager.GetString("CountOutOfRange");
            }
        }
#endif

        /// <summary>
        /// Gets the string for the Invalid Code Page error.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <returns>The Invalid Code Page error string.</returns>
        internal static string InvalidCodePage(int codePage)
        {
            return string.Format(ResourceManager.GetString("InvalidCodePage"), codePage);
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// String identifiers for resources
        /// </summary>
        private static readonly string[] StringIdentifier = 
        { 
            "MaxCharactersCannotBeNegative", 
            "PriorityListIncludesNonDetectableCodePage", 
            "IndexOutOfRange", 
            "CountTooLarge", 
            "OffsetOutOfRange",  
            "CountOutOfRange"
        };

        /// <summary>
        /// Gets a string for the invalid character set error.
        /// </summary>
        /// <param name="charsetName">Name of the invalid character set.</param>
        /// <returns>A string for the invalid character set error.</returns>
        public static string InvalidCharset(string charsetName)
        {
            return string.Format(ResourceManager.GetString("InvalidCharset"), charsetName);
        }

        /// <summary>
        /// Gets the string for the Invalid Locale Id error.
        /// </summary>
        /// <param name="localeId">The locale id.</param>
        /// <returns>The for the Invalid Locale Id error string.</returns>
        public static string InvalidLocaleId(int localeId)
        {
            return string.Format(ResourceManager.GetString("InvalidLocaleId"), localeId);
        }

        /// <summary>
        /// Gets the string for the Character Set Not Installed error.
        /// </summary>
        /// <param name="charsetName">The character set name.</param>
        /// <returns>The Character Set Not Installed error string.</returns>
        public static string NotInstalledCharset(string charsetName)
        {
            return string.Format(ResourceManager.GetString("NotInstalledCharset"), charsetName);
        }

        /// <summary>
        /// Gets the string for the Code Page Not Installed error.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <returns>The Code Page not Installed error string.</returns>
        public static string NotInstalledCodePage(int codePage)
        {
            return string.Format(ResourceManager.GetString("NotInstalledCodePage"), codePage);
        }

        /// <summary>
        /// Gets the localized string for the provided key.
        /// </summary>
        /// <param name="key">The resource key as a string.</param>
        /// <returns>The localized string for the provided key.</returns>
        public static string GetLocalizedString(ResourceIdentifier key)
        {
            return ResourceManager.GetString(StringIdentifier[(int)key]);
        }

        /// <summary>
        /// Gets the string for the Character Set / Code Page Not Installed error.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="charsetName">The character set name.</param>
        /// <returns>The Character Set / Code Page Not Installed error string.</returns>
        public static string NotInstalledCharsetCodePage(int codePage, string charsetName)
        {
            return string.Format(ResourceManager.GetString("NotInstalledCharsetCodePage"), codePage, charsetName);
        }

       /// <summary>
        /// Gets the string for the Invalid Culture Name error.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <returns>The Invalid Culture Name error string.</returns>
        internal static string InvalidCultureName(string cultureName)
        {
            return string.Format(ResourceManager.GetString("InvalidCultureName"), cultureName);
        }
#endif

    }
}
