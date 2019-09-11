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
            set
            {
                this.cultureInfo = value;
            }
        }

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