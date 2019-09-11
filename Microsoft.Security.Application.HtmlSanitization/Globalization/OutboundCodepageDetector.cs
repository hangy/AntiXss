// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutboundCodepageDetector.cs" company="Microsoft Corporation">
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
//   Detects the code page for outbound data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.IO;
    using System.Linq;

    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    /// <summary>
    /// Value indidicating which fallback exceptions should be allowed.
    /// </summary>      
    internal enum FallbackExceptions
    {
        /// <summary>
        /// No fallback exceptions are allowed.
        /// </summary>
        None,
        
        /// <summary>
        /// Common fallback exceptions are allowed.
        /// </summary>
        Common,
        
        /// <summary>
        /// All fallback exceptions are allowed.
        /// </summary>
        All
    }

    // Orphaned WPL code.
#if false
    /// <summary>
    /// Detects the code page for outbound data.
    /// </summary>
    internal class OutboundCodePageDetector
    {
        /// <summary>
        /// The code page detection structure.
        /// </summary>
        private CodePageDetect detector;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundCodePageDetector"/> class.
        /// </summary>
        public OutboundCodePageDetector()
        {
            this.detector = new CodePageDetect();
            this.detector.Initialize();
        }

        /// <summary>
        /// Rests the detctor mask map.
        /// </summary>
        public void Reset()
        {
            this.detector.Reset();
        }

        /// <summary>
        /// Adds the specified character to the the detector maskmap.
        /// </summary>
        /// <param name="ch">The character to add.</param>
        public void AddText(char ch)
        {
            this.detector.AddData(ch);
        }

        /// <summary>
        /// Adds the specified length of characters from the offset in the passed buffer to the the detector maskmap.
        /// </summary>
        /// <param name="buffer">The buffer to source characters from.</param>
        /// <param name="index">The offset at which to start.</param>
        /// <param name="count">The number of characters to add.</param>        
        public void AddText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
            }

            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
            }

            this.detector.AddData(buffer, index, count);
        }

        /// <summary>
        /// Adds the specified length of characters from the offset in the passed buffer to the the detector maskmap.
        /// </summary>
        /// <param name="buffer">The buffer to source characters from.</param>
        public void AddText(char[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this.detector.AddData(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Adds the specified length of characters from the offset in the passed buffer to the the detector maskmap.
        /// </summary>
        /// <param name="value">The string to source characters from.</param>
        /// <param name="index">The offset at which to start.</param>
        /// <param name="count">The number of characters to add.</param>                
        public void AddText(string value, int index, int count)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (index < 0 || index > value.Length)
            {
                throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
            }

            if (count < 0 || count > value.Length)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
            }

            if (value.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
            }

            this.detector.AddData(value, index, count);
        }

        /// <summary>
        /// Adds the specified string to the the detector maskmap.
        /// </summary>
        /// <param name="value">The string to source characters from.</param>
        public void AddText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.detector.AddData(value, 0, value.Length);
        }

        /// <summary>
        /// Adds the content from the supplied TextReader to the the detector maskmap.
        /// </summary>
        /// <param name="reader">The TextReader to source characters from.</param>        
        /// <param name="maxCharacters">The maximum number of characters to add.</param>
        public void AddText(TextReader reader, int maxCharacters)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (maxCharacters < 0)
            {
                throw new ArgumentOutOfRangeException("maxCharacters", GlobalizationStrings.MaxCharactersCannotBeNegative);
            }

            this.detector.AddData(reader, maxCharacters);
        }

        /// <summary>
        /// Adds the content from the supplied TextReader to the the detector maskmap.
        /// </summary>
        /// <param name="reader">The TextReader to source characters from.</param>        
        public void AddText(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.detector.AddData(reader, Int32.MaxValue);
        }

        /// <summary>
        /// Gets the code page using the default priority order, and only from valid code pages.
        /// </summary>
        /// <returns>The code page.</returns>
        public int GetCodePage()
        {
            return this.detector.GetCodePage(Culture.Default.CodepageDetectionPriorityOrder, false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, true/*onlyValidCodePages*/);
        }

        /// <summary>
        /// Gets the code page for the specified culture, allowing common fall backs if specified.
        /// </summary>
        /// <param name="culture">The culture to retrieve the code page for.</param>
        /// <param name="allowCommonFallbackExceptions">If true allows common code page fall backs.</param>
        /// <returns>The code page for the specified culture.</returns>
        public int GetCodePage(Culture culture, bool allowCommonFallbackExceptions)
        {
            if (culture == null)
            {
                culture = Culture.Default;
            }

            return this.detector.GetCodePage(
                            culture.CodepageDetectionPriorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }

        /// <summary>
        /// Gets the code page for the specified character set, allowing common fall backs if specified.
        /// </summary>
        /// <param name="preferredCharset">The preferred character set the code page should contain.</param>
        /// <param name="allowCommonFallbackExceptions">If true allows common code page fall backs.</param>
        /// <returns>The code page for the specified character set.</returns>
        public int GetCodePage(Charset preferredCharset, bool allowCommonFallbackExceptions)
        {
            int[] priorityOrder = Culture.Default.CodepageDetectionPriorityOrder;

            if (preferredCharset != null)
            {
                priorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, priorityOrder);
            }

            return this.detector.GetCodePage(
                            priorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }
      
        /// <summary>
        /// Gets an integer array of valid code pages, with no fall backs.
        /// </summary>
        /// <returns>An integer array of valid code pages, with no fall backs.</returns>
        public int[] GetCodePages()
        {
            return this.detector.GetCodePages(Culture.Default.CodepageDetectionPriorityOrder, false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, true/*onlyValidCodePages*/);
        }

        /// <summary>
        /// Gets an integer array of valid code pages for the specified culture, with exceptions if set.
        /// </summary>
        /// <param name="culture">The culture to retrieve the code pages for.</param>
        /// <param name="allowCommonFallbackExceptions">If true allows common fall back exceptions.</param>
        /// <returns>An integer array of valid code pages for the specified culture.</returns>
        public int[] GetCodePages(Culture culture, bool allowCommonFallbackExceptions)
        {
            if (culture == null)
            {
                culture = Culture.Default;
            }

            return this.detector.GetCodePages(
                            culture.CodepageDetectionPriorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }
        
        /// <summary>
        /// Gets an integer array of valid code pages for the specified character set, with exceptions if set.
        /// </summary>
        /// <param name="preferredCharset">The character set to retrieve the code pages for.</param>
        /// <param name="allowCommonFallbackExceptions">If true allows common fall back exceptions.</param>
        /// <returns>An integer array of valid code pages for the specified culture.</returns>
        public int[] GetCodePages(Charset preferredCharset, bool allowCommonFallbackExceptions)
        {
            int[] priorityOrder = Culture.Default.CodepageDetectionPriorityOrder;

            if (preferredCharset != null)
            {
                priorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, priorityOrder);
            }

            return this.detector.GetCodePages(
                            priorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }

        /// <summary>
        /// Gets the coverage for the specified code page.
        /// </summary>
        /// <param name="codePage">The code page to return coverage for.</param>
        /// <returns>A code page coverage percentage.</returns>
        public int GetCodePageCoverage(int codePage)
        {
            Charset charset = Charset.GetCharset(codePage);
            if (charset.UnicodeCoverage == CodePageUnicodeCoverage.Complete)
            {
                return 100;
            }

            if (!charset.IsDetectable)
            {
                if (charset.DetectableCodePageWithEquivalentCoverage == 0)
                {
                    throw new ArgumentException("codePage is not detectable");
                }

                codePage = charset.DetectableCodePageWithEquivalentCoverage;
            }

            return this.detector.GetCodePageCoverage(codePage);
        }

        /// <summary>
        /// Gets the best Windows code page.
        /// </summary>
        /// <returns>A Windows code page id.</returns>
        public int GetBestWindowsCodePage()
        {
            return this.detector.GetBestWindowsCodePage(false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/);
        }

        /// <summary>
        /// Determines whether specified code page identifier is detectable
        /// </summary>
        /// <param name="cpid">The code page identifier.</param>
        /// <param name="onlyValid">If set to <c>true</c> then only valid codepages are checked.</param>
        /// <returns>
        /// <c>true</c> if is the specified code page identifier results in a detectable code page; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsCodePageDetectable(int cpid, bool onlyValid)
        {
            return CodePageDetect.IsCodePageDetectable(cpid, onlyValid);
        }

        /// <summary>
        /// Gets the default codepage priority list.
        /// </summary>
        /// <returns>
        /// The default codepage priority list.
        /// </returns>
        internal static int[] GetDefaultCodePagePriorityList()
        {
            return CodePageDetect.GetDefaultPriorityList();
        }

        /// <summary>
        /// Gets a list of common exception characters.
        /// </summary>
        /// <returns>A list of common exception characters.</returns>
        internal static char[] GetCommonExceptionCharacters()
        {
            return CodePageDetect.GetCommonExceptionCharacters();
        }

        /// <summary>
        /// Gets an integer array of code pages from the specified code page priority list and fall back exceptions only selecting 
        /// valid code pages according to the parameters
        /// </summary>
        /// <param name="codePagePriorityList">The code page priority list.</param>
        /// <param name="fallbackExceptions">The allowed fall back exceptions.</param>
        /// <param name="onlyValidCodePages">Only valid code pages will be selected if true.</param>
        /// <returns>Gets an integer array of code pages.</returns>
        internal int[] GetCodePages(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
        {
            if (codePagePriorityList != null)
            {
                if (codePagePriorityList.Any(t => !CodePageDetect.IsCodePageDetectable(t, false)))
                {
                    throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
                }
            }

            return this.detector.GetCodePages(
                            codePagePriorityList,
                            fallbackExceptions > FallbackExceptions.None/*allowCommonFallbackExceptions*/,
                            fallbackExceptions > FallbackExceptions.Common/*allowAnyFallbackExceptions*/,
                            onlyValidCodePages);
        }

        /// <summary>
        /// Gets the best Windows code page for the specified code page.
        /// </summary>
        /// <param name="preferredCodePage">The preferred code page.</param>
        /// <returns>A Windows code page id.</returns>
        internal int GetBestWindowsCodePage(int preferredCodePage)
        {
            return this.detector.GetBestWindowsCodePage(false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, preferredCodePage);
        }

        /// <summary>
        /// Gets the code page from the specified prioity list and fallback exception list.
        /// </summary>
        /// <param name="codePagePriorityList">The code page proirity list.</param>
        /// <param name="fallbackExceptions">The fall back exception list</param>
        /// <param name="onlyValidCodePages">If true only selects valid code pages.</param>
        /// <returns>The code page from the specified prioity list and fallback exception list.</returns>        
        internal int GetCodePage(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
        {
            if (codePagePriorityList != null)
            {
                if (codePagePriorityList.Any(t => !CodePageDetect.IsCodePageDetectable(t, false)))
                {
                    throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
                }
            }

            return this.detector.GetCodePage(
                            codePagePriorityList,
                            fallbackExceptions > FallbackExceptions.None/*allowCommonFallbackExceptions*/,
                            fallbackExceptions > FallbackExceptions.Common/*allowAnyFallbackExceptions*/,
                            onlyValidCodePages);
        }
    }
#endif
}

