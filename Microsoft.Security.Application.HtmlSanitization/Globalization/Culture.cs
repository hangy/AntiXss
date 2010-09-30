// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Culture.cs" company="Microsoft Corporation">
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
//   Represents a culture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a culture 
    /// </summary>
    [Serializable]
    internal class Culture
    {
        /// <summary>
        /// The locale id for this culture.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in CultureCharsetDatabase.cs.")]
        private readonly int lcid;

        /// <summary>
        /// The culture name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in CultureCharsetDatabase.cs.")]
        private readonly string name;

        /// <summary>
        /// The codepage detection order, by priority.
        /// </summary>
        private int[] codepageDetectionPriorityOrder;

        /// <summary>
        /// The culture info.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="Set in CultureCharsetDatabase.cs.")]
        private CultureInfo cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Culture"/> class.
        /// </summary>
        /// <param name="lcid">
        /// The locale identifier.
        /// </param>
        /// <param name="name">
        /// The name of the culture.
        /// </param>
        internal Culture(int lcid, string name)
        {
            this.lcid = lcid;
            this.name = name;
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Gets the default culture.
        /// </summary>
        public static Culture Default
        {
            get
            {
                return CultureCharsetDatabase.InternalGlobalizationData.DefaultCulture;
            }
        }

        /// <summary>
        /// Gets the invariant culture.
        /// </summary>
        public static Culture Invariant
        {
            get
            {
                return CultureCharsetDatabase.InternalGlobalizationData.InvariantCulture;
            }
        }

        /// <summary>
        /// Gets the local identifier.
        /// </summary>
        public int LocaleId
        {
            get
            {
                return this.lcid;
            }
        }

        /// <summary>
        /// Gets the name of the culture.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the Windows character set for the culture.
        /// </summary>
        public Charset WindowsCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME character set for the culture.
        /// </summary>
        public Charset MimeCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web character set for the culture.
        /// </summary>
        public Charset WebCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description for the culture.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Using default get/set syntax.")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the native description.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Using default get/set syntax.")]
        public string NativeDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent culture.
        /// </summary>
        public Culture ParentCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the culture information for this culture.
        /// </summary>
        /// <returns>
        /// The <see cref="CultureInfo"/> for this culture.
        /// </returns>
        public CultureInfo CultureInfo
        {

        // Orphaned WPL code.
#if false
            get
            {
                return this.cultureInfo ?? CultureInfo.InvariantCulture;
            }
#endif

            set
            {
                this.cultureInfo = value;
            }
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Gets the code page detection order, sorted by priority.
        /// </summary>
        internal int[] CodepageDetectionPriorityOrder
        {
            get
            {
                return this.GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.InternalGlobalizationData);
            }
        }

        /// <summary>
        /// Gets a <see cref="Culture"/> object for the specified culture name.
        /// </summary>
        /// <param name="name">
        /// The culture name to retrive.
        /// </param>
        /// <returns>
        /// The <see cref="Culture"/> object for the specified index.
        /// </returns>
        /// <exception cref="UnknownCultureException">
        /// Thrown if the culture name is not found.
        /// </exception>
        public static Culture GetCulture(string name)
        {
            Culture culture;
            if (!TryGetCulture(name, out culture))
            {
                throw new UnknownCultureException(name);
            }

            return culture;
        }

        /// <summary>
        /// Gets the culture for the specified locale identifier
        /// </summary>
        /// <param name="lcid">
        /// The locale identifier.
        /// </param>
        /// <returns>
        /// The culture class for the specified locale.
        /// </returns>
        /// <exception cref="UnknownCultureException">
        /// Throw if the locale identifier is unknown.
        /// </exception>
        public static Culture GetCulture(int lcid)
        {
            Culture culture;
            if (!TryGetCulture(lcid, out culture))
            {
                throw new UnknownCultureException(lcid);
            }

            return culture;
        }


        /// <summary>
        /// Looks up the passed in culture name and returns the corresponding culture.
        /// </summary>
        /// <param name="name">
        /// The name of the culture.
        /// </param>
        /// <param name="culture">
        /// The culture associated with the name.
        /// </param>
        /// <returns>
        /// True if a culture for the name is found, otherwise false.
        /// </returns>
        public static bool TryGetCulture(string name, out Culture culture)
        {
            if (name == null)
            {
                culture = null;
                return false;
            }

            return CultureCharsetDatabase.InternalGlobalizationData.NameToCulture.TryGetValue(name, out culture);
        }

        /// <summary>
        /// Looks up the passed in locale identifier and returns the corresponding culture.
        /// </summary>
        /// <param name="lcid">
        /// The locale identifier.
        /// </param>
        /// <param name="culture">
        /// The culture associated with the locale.
        /// </param>
        /// <returns>
        /// True if a culture for the locale identifier is found, otherwise false.
        /// </returns>
        public static bool TryGetCulture(int lcid, out Culture culture)
        {
            return CultureCharsetDatabase.InternalGlobalizationData.LocaleIdToCulture.TryGetValue(lcid, out culture);
        }

        /// <summary>
        /// Gets the culture information for this culture, based upon the <see cref="Culture.Name"/> property.
        /// </summary>
        /// <returns>
        /// The culture information for this culture, based upon the <see cref="Culture.Name"/> property.
        /// </returns>
        internal CultureInfo GetSpecificCultureInfo()
        {
            if (this.cultureInfo == null)
            {
                return CultureInfo.InvariantCulture;
            }

            try
            {
                return CultureInfo.CreateSpecificCulture(this.cultureInfo.Name);
            }
            catch (ArgumentException)
            {
                return CultureInfo.InvariantCulture;
            }
        }
#endif

        /// <summary>
        /// Gets code page detection priority order for the specified globalization data.
        /// </summary>
        /// <param name="data">
        /// The globalization data.
        /// </param>
        /// <returns>
        /// The code page detection priority.
        /// </returns>
        internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data)
        {
            return this.codepageDetectionPriorityOrder ??
                   (this.codepageDetectionPriorityOrder =
                    CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(
                        this,
                        this.ParentCulture == null || this.ParentCulture == this ? data.DefaultDetectionPriorityOrder : this.ParentCulture.GetCodepageDetectionPriorityOrder(data)));
        }

        /// <summary>
        /// Sets the code page detection prioity order.
        /// </summary>
        /// <param name="newCodepageDetectionPriorityOrder">
        /// The new code page detection priority order.
        /// </param>
        internal void SetCodepageDetectionPriorityOrder(int[] newCodepageDetectionPriorityOrder)
        {
            this.codepageDetectionPriorityOrder = newCodepageDetectionPriorityOrder;
        }
    }
}