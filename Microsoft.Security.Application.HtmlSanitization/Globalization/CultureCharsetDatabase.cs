// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureCharsetDatabase.cs" company="Microsoft Corporation">
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
//   The culture character set database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Internal;

    /// <summary>
    /// The culture character set database
    /// </summary>
    internal static class CultureCharsetDatabase
    {
        /// <summary>
        /// An instance of the integer comparer.
        /// </summary>
        private static readonly IntComparer IntComparerInstance = new IntComparer();

        /// <summary>
        /// The internal globalization data.
        /// </summary>
        private static GlobalizationData internalGlobalizationDataStore = LoadGlobalizationData(null);

        /// <summary>
        /// Gets or sets the internal globalization data.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses dictionary syntax to set value.  FXCop misses this.")]
        internal static GlobalizationData InternalGlobalizationData
        {
            get
            {
                return internalGlobalizationDataStore;
            }

            set
            {
                internalGlobalizationDataStore = value;
            }
        }

                    // Orphaned WPL code.
#if false

        /// <summary>
        /// Refreshes the globalization data with the specified default culture name.
        /// </summary>
        /// <param name="defaultCultureName">
        /// The default culture name.
        /// </param>
        internal static void RefreshConfiguration(string defaultCultureName)
        {
            internalGlobalizationDataStore = LoadGlobalizationData(defaultCultureName);
        }

        /// <summary>
        /// Gets the code page detection list, ordered by priority for the specified Culture.
        /// </summary>
        /// <param name="charset">
        /// The character set  to get the ordered detection list for.
        /// </param>
        /// <param name="originalPriorityList">
        /// The original priority list.
        /// </param>
        /// <returns>
        /// An ordered list of the code page ids.
        /// </returns>
        internal static int[] GetAdjustedCodepageDetectionPriorityOrder(Charset charset, int[] originalPriorityList)
        {
            if (!charset.IsDetectable &&
                originalPriorityList != null)
            {
                return originalPriorityList;
            }

            var newPriorityList = new int[CodePageDetectData.CodePages.Length];
            int newPriorityListIndex = 0;

            newPriorityList[newPriorityListIndex++] = 20127;

            if (charset.IsDetectable && !IsDbcs(charset.CodePage) &&
                !InList(charset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = charset.CodePage;
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(originalPriorityList[i], charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(CodePageDetectData.CodePages[i].Id, charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (charset.IsDetectable && IsDbcs(charset.CodePage) &&
                !InList(charset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = charset.CodePage;
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(originalPriorityList[i], charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(CodePageDetectData.CodePages[i].Id, charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            InternalDebug.Assert(newPriorityListIndex == CodePageDetectData.CodePages.Length);

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (newPriorityList[i] !=
                        originalPriorityList[i])
                    {
                        return newPriorityList;
                    }
                }

                return originalPriorityList;
            }

            return newPriorityList;
        }
#endif

        /// <summary>
        /// Gets the code page detection list, ordered by priority for the specified Culture.
        /// </summary>
        /// <param name="culture">
        /// The culture to get the ordered detection list for.
        /// </param>
        /// <param name="originalPriorityList">
        /// The original priority list.
        /// </param>
        /// <returns>
        /// An ordered list of the code page ids.
        /// </returns>
        internal static int[] GetCultureSpecificCodepageDetectionPriorityOrder(
            Culture culture,
            int[] originalPriorityList)
        {
            var newPriorityList = new int[CodePageDetectData.CodePages.Length];
            int newPriorityListIndex = 0;

            newPriorityList[newPriorityListIndex++] = 20127;

            if (culture.MimeCharset.IsDetectable && !IsDbcs(culture.MimeCharset.CodePage) &&
                !InList(culture.MimeCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.MimeCharset.CodePage;
            }

            if (culture.WebCharset.IsDetectable && !IsDbcs(culture.WebCharset.CodePage) &&
                !InList(culture.WebCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WebCharset.CodePage;
            }

            if (culture.WindowsCharset.IsDetectable && !IsDbcs(culture.WindowsCharset.CodePage) &&
                !InList(culture.WindowsCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WindowsCharset.CodePage;
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(originalPriorityList[i], culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(CodePageDetectData.CodePages[i].Id, culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (culture.MimeCharset.IsDetectable && IsDbcs(culture.MimeCharset.CodePage) &&
                !InList(culture.MimeCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.MimeCharset.CodePage;
            }

            if (culture.WebCharset.IsDetectable && IsDbcs(culture.WebCharset.CodePage) &&
                !InList(culture.WebCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WebCharset.CodePage;
            }

            if (culture.WindowsCharset.IsDetectable && IsDbcs(culture.WindowsCharset.CodePage) &&
                !InList(culture.WindowsCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WindowsCharset.CodePage;
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (IsDbcs(originalPriorityList[i]) &&
                        !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(originalPriorityList[i], culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (IsDbcs(CodePageDetectData.CodePages[i].Id) &&
                        !InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex) &&
                        IsSameLanguage(CodePageDetectData.CodePages[i].Id, culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CodePageDetectData.CodePages.Length; i++)
                {
                    if (!InList(CodePageDetectData.CodePages[i].Id, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.CodePages[i].Id;
                    }
                }
            }

            InternalDebug.Assert(newPriorityListIndex == CodePageDetectData.CodePages.Length);

            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (newPriorityList[i] !=
                        originalPriorityList[i])
                    {
                        return newPriorityList;
                    }
                }

                return originalPriorityList;
            }

            return newPriorityList;
        }

        /// <summary>
        /// Loads the globalization data with the specified default culture name.
        /// </summary>
        /// <param name="defaultCultureName">
        /// The default culture name.
        /// </param>
        /// <returns>
        /// A <see cref="GlobalizationData"/> class using the default culture nane specified.
        /// </returns>
        private static GlobalizationData LoadGlobalizationData(string defaultCultureName)
        {
            InternalWindowsCodePage[] windowsCodePages =
                {
                    new InternalWindowsCodePage(1200, "unicode", 0, null, 65001, 65001, "Unicode generic culture"), 
                    new InternalWindowsCodePage(1250, "windows-1250", 0, null, 28592, 1250, "Central European generic culture"), 
                    new InternalWindowsCodePage(1251, "windows-1251", 0, null, 20866, 1251, "Cyrillic generic culture"), 
                    new InternalWindowsCodePage(1252, "windows-1252", 0, null, 28591, 1252, "Western European generic culture"), 
                    new InternalWindowsCodePage(1253, "windows-1253", 0x0008, "el", 28597, 1253, null), 
                    new InternalWindowsCodePage(1254, "windows-1254", 0, null, 28599, 1254, "Turkish / Azeri generic culture"), 
                    new InternalWindowsCodePage(1255, "windows-1255", 0x000D, "he", 1255, 1255, null), 
                    new InternalWindowsCodePage(1256, "windows-1256", 0, null, 1256, 1256, "Arabic generic culture"), 
                    new InternalWindowsCodePage(1257, "windows-1257", 0, null, 1257, 1257, "Baltic generic culture"), 
                    new InternalWindowsCodePage(1258, "windows-1258", 0x002A, "vi", 1258, 1258, null), 
                    new InternalWindowsCodePage(874, "windows-874", 0x001E, "th", 874, 874, null), 
                    new InternalWindowsCodePage(932, "windows-932", 0x0011, "ja", 50220, 932, null), 
                    new InternalWindowsCodePage(936, "windows-936", 0x0004, "zh-CHS", 936, 936, null), 
                    new InternalWindowsCodePage(949, "windows-949", 0x0012, "ko", 949, 949, null), 
                    new InternalWindowsCodePage(950, "windows-950", 0x7C04, "zh-CHT", 950, 950, null), 
                };

            CodePageCultureOverride[] codePageCultureOverrides =
                {
                    new CodePageCultureOverride(37, "en"), 
                    new CodePageCultureOverride(437, "en"), 
                    new CodePageCultureOverride(860, "pt"), 
                    new CodePageCultureOverride(861, "is"), 
                    new CodePageCultureOverride(863, "fr-CA"), 
                    new CodePageCultureOverride(1141, "de"), 
                    new CodePageCultureOverride(1144, "it"), 
                    new CodePageCultureOverride(1145, "es"), 
                    new CodePageCultureOverride(1146, "en-GB"), 
                    new CodePageCultureOverride(1147, "fr"), 
                    new CodePageCultureOverride(1149, "is"), 
                    new CodePageCultureOverride(10010, "ro"), 
                    new CodePageCultureOverride(10017, "uk"), 
                    new CodePageCultureOverride(10079, "is"), 
                    new CodePageCultureOverride(10082, "hr"), 
                    new CodePageCultureOverride(20106, "de"), 
                    new CodePageCultureOverride(20107, "sv"), 
                    new CodePageCultureOverride(20108, "no"), 
                    new CodePageCultureOverride(20127, "en"), 
                    new CodePageCultureOverride(20273, "de"), 
                    new CodePageCultureOverride(20280, "it"), 
                    new CodePageCultureOverride(20284, "es"), 
                    new CodePageCultureOverride(20285, "en-GB"), 
                    new CodePageCultureOverride(20297, "fr"), 
                    new CodePageCultureOverride(20866, "ru"), 
                    new CodePageCultureOverride(20871, "is"), 
                    new CodePageCultureOverride(20880, "ru"), 
                    new CodePageCultureOverride(21866, "uk"), 
                    new CodePageCultureOverride(57003, "bn-IN"), 
                    new CodePageCultureOverride(57004, "ta"), 
                    new CodePageCultureOverride(57005, "te"), 
                    new CodePageCultureOverride(57008, "kn"), 
                    new CodePageCultureOverride(57009, "ml-IN"), 
                    new CodePageCultureOverride(57010, "gu"), 
                    new CodePageCultureOverride(57011, "pa"), 
                };

            CultureCodePageOverride[] cultureCodePageOverrides =
                {
                    new CultureCodePageOverride("et", 28605, 28605), 
                    new CultureCodePageOverride("lt", 28603, 28603), 
                    new CultureCodePageOverride("lv", 28603, 28603), 
                    new CultureCodePageOverride("uk", 21866, 1251), 
                    new CultureCodePageOverride("az-AZ-Cyrl", 1251, 1251), 
                    new CultureCodePageOverride("be", 1251, 1251), 
                    new CultureCodePageOverride("bg", 1251, 1251), 
                    new CultureCodePageOverride("mk", 1251, 1251), 
                    new CultureCodePageOverride("sr", 1251, 1251), 
                    new CultureCodePageOverride("sr-BA-Cyrl", 1251, 1251), 
                    new CultureCodePageOverride("sr-Cyrl-CS", 1251, 1251), 
                    new CultureCodePageOverride("ky", 1251, 1251), 
                    new CultureCodePageOverride("kk", 1251, 1251), 
                    new CultureCodePageOverride("tt", 1251, 1251), 
                    new CultureCodePageOverride("uz-UZ-Cyrl", 1251, 1251), 
                    new CultureCodePageOverride("mn", 65001, 65001), 
                };

            CharsetName[] charsetNames =
                {
                    new CharsetName("_autodetect", 50932), 
                    new CharsetName("_autodetect_all", 50001), 
                    new CharsetName("_autodetect_kr", 50949), 
                    new CharsetName("_iso-2022-jp$ESC", 50221), 
                    new CharsetName("_iso-2022-jp$SIO", 50222), 
                    new CharsetName("437", 437), 
                    new CharsetName("ANSI_X3.4-1968", 20127), 
                    new CharsetName("ANSI_X3.4-1986", 20127), 
                    new CharsetName("arabic", 28596), 
                    new CharsetName("ascii", 20127), 
                    new CharsetName("ASMO-708", 708), 
                    new CharsetName("Big5-HKSCS", 950), 
                    new CharsetName("Big5", 950), 
                    new CharsetName("CCSID00858", 858), 
                    new CharsetName("CCSID00924", 20924), 
                    new CharsetName("CCSID01140", 1140), 
                    new CharsetName("CCSID01141", 1141), 
                    new CharsetName("CCSID01142", 1142), 
                    new CharsetName("CCSID01143", 1143), 
                    new CharsetName("CCSID01144", 1144), 
                    new CharsetName("CCSID01145", 1145), 
                    new CharsetName("CCSID01146", 1146), 
                    new CharsetName("CCSID01147", 1147), 
                    new CharsetName("CCSID01148", 1148), 
                    new CharsetName("CCSID01149", 1149), 
                    new CharsetName("chinese", 936), 
                    new CharsetName("cn-big5", 950), 
                    new CharsetName("CN-GB", 936), 
                    new CharsetName("CP00858", 858), 
                    new CharsetName("CP00924", 20924), 
                    new CharsetName("CP01140", 1140), 
                    new CharsetName("CP01141", 1141), 
                    new CharsetName("CP01142", 1142), 
                    new CharsetName("CP01143", 1143), 
                    new CharsetName("CP01144", 1144), 
                    new CharsetName("CP01145", 1145), 
                    new CharsetName("CP01146", 1146), 
                    new CharsetName("CP01147", 1147), 
                    new CharsetName("CP01148", 1148), 
                    new CharsetName("CP01149", 1149), 
                    new CharsetName("cp037", 37), 
                    new CharsetName("cp1025", 21025), 
                    new CharsetName("CP1026", 1026), 
                    new CharsetName("cp1256", 1256), 
                    new CharsetName("CP273", 20273), 
                    new CharsetName("CP278", 20278), 
                    new CharsetName("CP280", 20280), 
                    new CharsetName("CP284", 20284), 
                    new CharsetName("CP285", 20285), 
                    new CharsetName("cp290", 20290), 
                    new CharsetName("cp297", 20297), 
                    new CharsetName("cp367", 20127), 
                    new CharsetName("cp420", 20420), 
                    new CharsetName("cp423", 20423), 
                    new CharsetName("cp424", 20424), 
                    new CharsetName("cp437", 437), 
                    new CharsetName("CP500", 500), 
                    new CharsetName("cp50227", 50227), 
                    new CharsetName("cp50229", 50229), 
                    new CharsetName("cp819", 28591), 
                    new CharsetName("cp850", 850), 
                    new CharsetName("cp852", 852), 
                    new CharsetName("cp855", 855), 
                    new CharsetName("cp857", 857), 
                    new CharsetName("cp858", 858), 
                    new CharsetName("cp860", 860), 
                    new CharsetName("cp861", 861), 
                    new CharsetName("cp862", 862), 
                    new CharsetName("cp863", 863), 
                    new CharsetName("cp864", 864), 
                    new CharsetName("cp865", 865), 
                    new CharsetName("cp866", 866), 
                    new CharsetName("cp869", 869), 
                    new CharsetName("CP870", 870), 
                    new CharsetName("CP871", 20871), 
                    new CharsetName("cp875", 875), 
                    new CharsetName("cp880", 20880), 
                    new CharsetName("CP905", 20905), 
                    new CharsetName("cp930", 50930), 
                    new CharsetName("cp933", 50933), 
                    new CharsetName("cp935", 50935), 
                    new CharsetName("cp937", 50937), 
                    new CharsetName("cp939", 50939), 
                    new CharsetName("csASCII", 20127), 
                    new CharsetName("csbig5", 950), 
                    new CharsetName("csEUCKR", 51949), 
                    new CharsetName("csEUCPkdFmtJapanese", 51932), 
                    new CharsetName("csGB2312", 936), 
                    new CharsetName("csGB231280", 936), 
                    new CharsetName("csIBM037", 37), 
                    new CharsetName("csIBM1026", 1026), 
                    new CharsetName("csIBM273", 20273), 
                    new CharsetName("csIBM277", 20277), 
                    new CharsetName("csIBM278", 20278), 
                    new CharsetName("csIBM280", 20280), 
                    new CharsetName("csIBM284", 20284), 
                    new CharsetName("csIBM285", 20285), 
                    new CharsetName("csIBM290", 20290), 
                    new CharsetName("csIBM297", 20297), 
                    new CharsetName("csIBM420", 20420), 
                    new CharsetName("csIBM423", 20423), 
                    new CharsetName("csIBM424", 20424), 
                    new CharsetName("csIBM500", 500), 
                    new CharsetName("csIBM870", 870), 
                    new CharsetName("csIBM871", 20871), 
                    new CharsetName("csIBM880", 20880), 
                    new CharsetName("csIBM905", 20905), 
                    new CharsetName("csIBMThai", 20838), 
                    new CharsetName("csISO2022JP", 50221), 
                    new CharsetName("csISO2022KR", 50225), 
                    new CharsetName("csISO58GB231280", 936), 
                    new CharsetName("csISOLatin1", 28591), 
                    new CharsetName("csISOLatin2", 28592), 
                    new CharsetName("csISOLatin3", 28593), 
                    new CharsetName("csISOLatin4", 28594), 
                    new CharsetName("csISOLatin5", 28599), 
                    new CharsetName("csISOLatin9", 28605), 
                    new CharsetName("csISOLatinArabic", 28596), 
                    new CharsetName("csISOLatinCyrillic", 28595), 
                    new CharsetName("csISOLatinGreek", 28597), 
                    new CharsetName("csISOLatinHebrew", 38598), 
                    new CharsetName("csKOI8R", 20866), 
                    new CharsetName("csKSC56011987", 949), 
                    new CharsetName("csPC8CodePage437", 437), 
                    new CharsetName("csShiftJIS", 932), 
                    new CharsetName("csUnicode11UTF7", 65000), 
                    new CharsetName("csWindows31J", 932), 
                    new CharsetName("Windows-31J", 932), 
                    new CharsetName("cyrillic", 28595), 
                    new CharsetName("DIN_66003", 20106), 
                    new CharsetName("DOS-720", 720), 
                    new CharsetName("DOS-862", 862), 
                    new CharsetName("DOS-874", 874), 
                    new CharsetName("ebcdic-cp-ar1", 20420), 
                    new CharsetName("ebcdic-cp-be", 500), 
                    new CharsetName("ebcdic-cp-ca", 37), 
                    new CharsetName("ebcdic-cp-ch", 500), 
                    new CharsetName("EBCDIC-CP-DK", 20277), 
                    new CharsetName("ebcdic-cp-es", 20284), 
                    new CharsetName("ebcdic-cp-fi", 20278), 
                    new CharsetName("ebcdic-cp-fr", 20297), 
                    new CharsetName("ebcdic-cp-gb", 20285), 
                    new CharsetName("ebcdic-cp-gr", 20423), 
                    new CharsetName("ebcdic-cp-he", 20424), 
                    new CharsetName("ebcdic-cp-is", 20871), 
                    new CharsetName("ebcdic-cp-it", 20280), 
                    new CharsetName("ebcdic-cp-nl", 37), 
                    new CharsetName("EBCDIC-CP-NO", 20277), 
                    new CharsetName("ebcdic-cp-roece", 870), 
                    new CharsetName("ebcdic-cp-se", 20278), 
                    new CharsetName("ebcdic-cp-tr", 20905), 
                    new CharsetName("ebcdic-cp-us", 37), 
                    new CharsetName("ebcdic-cp-wt", 37), 
                    new CharsetName("ebcdic-cp-yu", 870), 
                    new CharsetName("EBCDIC-Cyrillic", 20880), 
                    new CharsetName("ebcdic-de-273+euro", 1141), 
                    new CharsetName("ebcdic-dk-277+euro", 1142), 
                    new CharsetName("ebcdic-es-284+euro", 1145), 
                    new CharsetName("ebcdic-fi-278+euro", 1143), 
                    new CharsetName("ebcdic-fr-297+euro", 1147), 
                    new CharsetName("ebcdic-gb-285+euro", 1146), 
                    new CharsetName("ebcdic-international-500+euro", 1148), 
                    new CharsetName("ebcdic-is-871+euro", 1149), 
                    new CharsetName("ebcdic-it-280+euro", 1144), 
                    new CharsetName("EBCDIC-JP-kana", 20290), 
                    new CharsetName("ebcdic-Latin9--euro", 20924), 
                    new CharsetName("ebcdic-no-277+euro", 1142), 
                    new CharsetName("ebcdic-se-278+euro", 1143), 
                    new CharsetName("ebcdic-us-37+euro", 1140), 
                    new CharsetName("ECMA-114", 28596), 
                    new CharsetName("ECMA-118", 28597), 
                    new CharsetName("ELOT_928", 28597), 
                    new CharsetName("euc-cn", 51936), 
                    new CharsetName("euc-jp", 51932), 
                    new CharsetName("euc-kr", 51949), 
                    new CharsetName("Extended_UNIX_Code_Packed_Format_for_Japanese", 51932), 
                    new CharsetName("GB_2312-80", 936), 
                    new CharsetName("GB18030", 54936), 
                    new CharsetName("GB2312-80", 936), 
                    new CharsetName("GB2312", 936), 
                    new CharsetName("GB231280", 936), 
                    new CharsetName("GBK", 936), 
                    new CharsetName("German", 20106), 
                    new CharsetName("greek", 28597), 
                    new CharsetName("greek8", 28597), 
                    new CharsetName("hebrew", 38598), 
                    new CharsetName("hz-gb-2312", 52936), 
                    new CharsetName("IBM-Thai", 20838), 
                    new CharsetName("IBM00858", 858), 
                    new CharsetName("IBM00924", 20924), 
                    new CharsetName("IBM01047", 1047), 
                    new CharsetName("IBM01140", 1140), 
                    new CharsetName("IBM01141", 1141), 
                    new CharsetName("IBM01142", 1142), 
                    new CharsetName("IBM01143", 1143), 
                    new CharsetName("IBM01144", 1144), 
                    new CharsetName("IBM01145", 1145), 
                    new CharsetName("IBM01146", 1146), 
                    new CharsetName("IBM01147", 1147), 
                    new CharsetName("IBM01148", 1148), 
                    new CharsetName("IBM01149", 1149), 
                    new CharsetName("IBM037", 37), 
                    new CharsetName("IBM1026", 1026), 
                    new CharsetName("IBM273", 20273), 
                    new CharsetName("IBM277", 20277), 
                    new CharsetName("IBM278", 20278), 
                    new CharsetName("IBM280", 20280), 
                    new CharsetName("IBM284", 20284), 
                    new CharsetName("IBM285", 20285), 
                    new CharsetName("IBM290", 20290), 
                    new CharsetName("IBM297", 20297), 
                    new CharsetName("IBM367", 20127), 
                    new CharsetName("IBM420", 20420), 
                    new CharsetName("IBM423", 20423), 
                    new CharsetName("IBM424", 20424), 
                    new CharsetName("IBM437", 437), 
                    new CharsetName("IBM500", 500), 
                    new CharsetName("ibm737", 737), 
                    new CharsetName("ibm775", 775), 
                    new CharsetName("ibm819", 28591), 
                    new CharsetName("IBM850", 850), 
                    new CharsetName("IBM852", 852), 
                    new CharsetName("IBM855", 855), 
                    new CharsetName("IBM857", 857), 
                    new CharsetName("IBM860", 860), 
                    new CharsetName("IBM861", 861), 
                    new CharsetName("IBM862", 862), 
                    new CharsetName("IBM863", 863), 
                    new CharsetName("IBM864", 864), 
                    new CharsetName("IBM865", 865), 
                    new CharsetName("IBM866", 866), 
                    new CharsetName("IBM869", 869), 
                    new CharsetName("IBM870", 870), 
                    new CharsetName("IBM871", 20871), 
                    new CharsetName("IBM880", 20880), 
                    new CharsetName("IBM905", 20905), 
                    new CharsetName("irv", 20105), 
                    new CharsetName("ISO-10646-UCS-2", 1200), 
                    new CharsetName("iso-2022-jp", 50220), 
                    new CharsetName("iso-2022-jpdbcs", 50220), 
                    new CharsetName("iso-2022-jpesc", 50221), 
                    new CharsetName("iso-2022-jpsio", 50222), 
                    new CharsetName("iso-2022-jpeuc", 51932), 
                    new CharsetName("iso-2022-kr-7", 50225), 
                    new CharsetName("iso-2022-kr-7bit", 50225), 
                    new CharsetName("iso-2022-kr-8", 51949), 
                    new CharsetName("iso-2022-kr-8bit", 51949), 
                    new CharsetName("iso-2022-kr", 50225), 
                    new CharsetName("iso-8859-1", 28591), 
                    new CharsetName("iso-8859-11", 874), 
                    new CharsetName("iso-8859-13", 28603), 
                    new CharsetName("iso-8859-15", 28605), 
                    new CharsetName("iso-8859-2", 28592), 
                    new CharsetName("iso-8859-3", 28593), 
                    new CharsetName("iso-8859-4", 28594), 
                    new CharsetName("iso-8859-5", 28595), 
                    new CharsetName("iso-8859-6", 28596), 
                    new CharsetName("iso-8859-7", 28597), 
                    new CharsetName("iso-8859-8-i", 38598), 
                    new CharsetName("ISO-8859-8 Visual", 28598), 
                    new CharsetName("iso-8859-8", 28598), 
                    new CharsetName("iso-8859-9", 28599), 
                    new CharsetName("iso-ir-100", 28591), 
                    new CharsetName("iso-ir-101", 28592), 
                    new CharsetName("iso-ir-109", 28593), 
                    new CharsetName("iso-ir-110", 28594), 
                    new CharsetName("iso-ir-126", 28597), 
                    new CharsetName("iso-ir-127", 28596), 
                    new CharsetName("iso-ir-138", 38598), 
                    new CharsetName("iso-ir-144", 28595), 
                    new CharsetName("iso-ir-148", 28599), 
                    new CharsetName("iso-ir-149", 949), 
                    new CharsetName("iso-ir-58", 936), 
                    new CharsetName("iso-ir-6", 20127), 
                    new CharsetName("ISO_646.irv:1991", 20127), 
                    new CharsetName("iso_8859-1", 28591), 
                    new CharsetName("iso_8859-1:1987", 28591), 
                    new CharsetName("ISO_8859-15", 28605), 
                    new CharsetName("iso_8859-2", 28592), 
                    new CharsetName("iso_8859-2:1987", 28592), 
                    new CharsetName("ISO_8859-3", 28593), 
                    new CharsetName("ISO_8859-3:1988", 28593), 
                    new CharsetName("ISO_8859-4", 28594), 
                    new CharsetName("ISO_8859-4:1988", 28594), 
                    new CharsetName("ISO_8859-5", 28595), 
                    new CharsetName("ISO_8859-5:1988", 28595), 
                    new CharsetName("ISO_8859-6", 28596), 
                    new CharsetName("ISO_8859-6:1987", 28596), 
                    new CharsetName("ISO_8859-7", 28597), 
                    new CharsetName("ISO_8859-7:1987", 28597), 
                    new CharsetName("ISO_8859-8", 28598), 
                    new CharsetName("ISO_8859-8:1988", 28598), 
                    new CharsetName("ISO_8859-9", 28599), 
                    new CharsetName("ISO_8859-9:1989", 28599), 
                    new CharsetName("ISO646-US", 20127), 
                    new CharsetName("646", 20127), 
                    new CharsetName("iso8859-1", 28591), 
                    new CharsetName("iso8859-2", 28592), 
                    new CharsetName("Johab", 1361), 
                    new CharsetName("koi", 20866), 
                    new CharsetName("koi8-r", 20866), 
                    new CharsetName("koi8-ru", 21866), 
                    new CharsetName("koi8-u", 21866), 
                    new CharsetName("koi8", 20866), 
                    new CharsetName("koi8r", 20866), 
                    new CharsetName("korean", 949), 
                    new CharsetName("ks-c-5601", 949), 
                    new CharsetName("ks-c5601", 949), 
                    new CharsetName("ks_c_5601-1987", 949), 
                    new CharsetName("ks_c_5601-1989", 949), 
                    new CharsetName("ks_c_5601", 949), 
                    new CharsetName("ks_c_5601_1987", 949), 
                    new CharsetName("KSC_5601", 949), 
                    new CharsetName("KSC5601", 949), 
                    new CharsetName("l1", 28591), 
                    new CharsetName("l2", 28592), 
                    new CharsetName("l3", 28593), 
                    new CharsetName("l4", 28594), 
                    new CharsetName("l5", 28599), 
                    new CharsetName("l9", 28605), 
                    new CharsetName("latin1", 28591), 
                    new CharsetName("latin2", 28592), 
                    new CharsetName("latin3", 28593), 
                    new CharsetName("latin4", 28594), 
                    new CharsetName("latin5", 28599), 
                    new CharsetName("latin9", 28605), 
                    new CharsetName("logical", 38598), 
                    new CharsetName("macintosh", 10000), 
                    new CharsetName("MacRoman", 10000), 
                    new CharsetName("ms_Kanji", 932), 
                    new CharsetName("Norwegian", 20108), 
                    new CharsetName("NS_4551-1", 20108), 
                    new CharsetName("PC-Multilingual-850+euro", 858), 
                    new CharsetName("SEN_850200_B", 20107), 
                    new CharsetName("shift-jis", 932), 
                    new CharsetName("shift_jis", 932), 
                    new CharsetName("sjis", 932), 
                    new CharsetName("Swedish", 20107), 
                    new CharsetName("TIS-620", 874), 
                    new CharsetName("ucs-2", 1200), 
                    new CharsetName("unicode-1-1-utf-7", 65000), 
                    new CharsetName("unicode-1-1-utf-8", 65001), 
                    new CharsetName("unicode-2-0-utf-7", 65000), 
                    new CharsetName("unicode-2-0-utf-8", 65001), 
                    new CharsetName("unicode", 1200), 
                    new CharsetName("unicodeFFFE", 1201), 
                    new CharsetName("us-ascii", 20127), 
                    new CharsetName("us", 20127), 
                    new CharsetName("utf-16", 1200), 
                    new CharsetName("UTF-16BE", 1201), 
                    new CharsetName("UTF-16LE", 1200), 
                    new CharsetName("utf-32", 12000), 
                    new CharsetName("UTF-32BE", 12001), 
                    new CharsetName("UTF-32LE", 12000), 
                    new CharsetName("utf-7", 65000), 
                    new CharsetName("utf-8", 65001), 
                    new CharsetName("utf7", 65000), 
                    new CharsetName("utf8", 65001), 
                    new CharsetName("visual", 28598), 
                    new CharsetName("windows-1250", 1250), 
                    new CharsetName("windows-1251", 1251), 
                    new CharsetName("windows-1252", 1252), 
                    new CharsetName("windows-1253", 1253), 
                    new CharsetName("Windows-1254", 1254), 
                    new CharsetName("windows-1255", 1255), 
                    new CharsetName("windows-1256", 1256), 
                    new CharsetName("windows-1257", 1257), 
                    new CharsetName("windows-1258", 1258), 
                    new CharsetName("windows-874", 874), 
                    new CharsetName("x-ansi", 1252), 
                    new CharsetName("x-Chinese-CNS", 20000), 
                    new CharsetName("x-Chinese-Eten", 20002), 
                    new CharsetName("x-cp1250", 1250), 
                    new CharsetName("x-cp1251", 1251), 
                    new CharsetName("x-cp20001", 20001), 
                    new CharsetName("x-cp20003", 20003), 
                    new CharsetName("x-cp20004", 20004), 
                    new CharsetName("x-cp20005", 20005), 
                    new CharsetName("x-cp20261", 20261), 
                    new CharsetName("x-cp20269", 20269), 
                    new CharsetName("x-cp20936", 20936), 
                    new CharsetName("x-cp20949", 20949), 
                    new CharsetName("x-cp21027", 21027), 
                    new CharsetName("x-cp50227", 50227), 
                    new CharsetName("x-cp50229", 50229), 
                    new CharsetName("X-EBCDIC-JapaneseAndUSCanada", 50931), 
                    new CharsetName("X-EBCDIC-KoreanExtended", 20833), 
                    new CharsetName("x-euc-cn", 51936), 
                    new CharsetName("x-euc-jp", 51932), 
                    new CharsetName("x-euc", 51932), 
                    new CharsetName("x-Europa", 29001), 
                    new CharsetName("x-IA5-German", 20106), 
                    new CharsetName("x-IA5-Norwegian", 20108), 
                    new CharsetName("x-IA5-Swedish", 20107), 
                    new CharsetName("x-IA5", 20105), 
                    new CharsetName("x-iscii-as", 57006), 
                    new CharsetName("x-iscii-be", 57003), 
                    new CharsetName("x-iscii-de", 57002), 
                    new CharsetName("x-iscii-gu", 57010), 
                    new CharsetName("x-iscii-ka", 57008), 
                    new CharsetName("x-iscii-ma", 57009), 
                    new CharsetName("x-iscii-or", 57007), 
                    new CharsetName("x-iscii-pa", 57011), 
                    new CharsetName("x-iscii-ta", 57004), 
                    new CharsetName("x-iscii-te", 57005), 
                    new CharsetName("x-mac-arabic", 10004), 
                    new CharsetName("x-mac-ce", 10029), 
                    new CharsetName("x-mac-chinesesimp", 10008), 
                    new CharsetName("x-mac-chinesetrad", 10002), 
                    new CharsetName("x-mac-croatian", 10082), 
                    new CharsetName("x-mac-cyrillic", 10007), 
                    new CharsetName("x-mac-greek", 10006), 
                    new CharsetName("x-mac-hebrew", 10005), 
                    new CharsetName("x-mac-icelandic", 10079), 
                    new CharsetName("x-mac-japanese", 10001), 
                    new CharsetName("x-mac-korean", 10003), 
                    new CharsetName("x-mac-romanian", 10010), 
                    new CharsetName("x-mac-thai", 10021), 
                    new CharsetName("x-mac-turkish", 10081), 
                    new CharsetName("x-mac-ukrainian", 10017), 
                    new CharsetName("x-ms-cp932", 932), 
                    new CharsetName("x-sjis", 932), 
                    new CharsetName("x-unicode-1-1-utf-7", 65000), 
                    new CharsetName("x-unicode-1-1-utf-8", 65001), 
                    new CharsetName("x-unicode-2-0-utf-7", 65000), 
                    new CharsetName("x-unicode-2-0-utf-8", 65001), 
                    new CharsetName("x-user-defined", 1252), 
                    new CharsetName("x-x-big5", 950), 
                };

            CultureData[] hardcodedCultures =
                {
                    new CultureData(0x401, "ar-SA", 1256, 1256, 1256, "ar", "Arabic (Saudi Arabia)"), 
                    new CultureData(0x402, "bg-BG", 1251, 1251, 1251, "bg", "Bulgarian (Bulgaria)"), 
                    new CultureData(0x403, "ca-ES", 1252, 28591, 1252, "ca", "Catalan (Catalan)"), 
                    new CultureData(0x404, "zh-TW", 950, 950, 950, "zh-CHT", "Chinese (Taiwan)"), 
                    new CultureData(0x405, "cs-CZ", 1250, 28592, 1250, "cs", "Czech (Czech Republic)"), 
                    new CultureData(0x406, "da-DK", 1252, 28591, 1252, "da", "Danish (Denmark)"), 
                    new CultureData(0x407, "de-DE", 1252, 28591, 1252, "de", "German (Germany)"), 
                    new CultureData(0x408, "el-GR", 1253, 28597, 1253, "el", "Greek (Greece)"), 
                    new CultureData(0x409, "en-US", 1252, 28591, 1252, "en", "English (United States)"), 
                    new CultureData(0x40B, "fi-FI", 1252, 28591, 1252, "fi", "Finnish (Finland)"), 
                    new CultureData(0x40C, "fr-FR", 1252, 28591, 1252, "fr", "French (France)"), 
                    new CultureData(0x40D, "he-IL", 1255, 1255, 1255, "he", "Hebrew (Israel)"), 
                    new CultureData(0x40E, "hu-HU", 1250, 28592, 1250, "hu", "Hungarian (Hungary)"), 
                    new CultureData(0x40F, "is-IS", 1252, 28591, 1252, "is", "Icelandic (Iceland)"), 
                    new CultureData(0x410, "it-IT", 1252, 28591, 1252, "it", "Italian (Italy)"), 
                    new CultureData(0x411, "ja-JP", 932, 50220, 932, "ja", "Japanese (Japan)"), 
                    new CultureData(0x412, "ko-KR", 949, 949, 949, "ko", "Korean (Korea)"), 
                    new CultureData(0x413, "nl-NL", 1252, 28591, 1252, "nl", "Dutch (Netherlands)"), 
                    new CultureData(0x414, "nb-NO", 1252, 28591, 1252, "no", "Norwegian - Bokm†l (Norway)"), 
                    new CultureData(0x415, "pl-PL", 1250, 28592, 1250, "pl", "Polish (Poland)"), 
                    new CultureData(0x416, "pt-BR", 1252, 28591, 1252, "pt", "Portuguese (Brazil)"), 
                    new CultureData(0x418, "ro-RO", 1250, 28592, 1250, "ro", "Romanian (Romania)"), 
                    new CultureData(0x419, "ru-RU", 1251, 20866, 1251, "ru", "Russian (Russia)"), 
                    new CultureData(0x41A, "hr-HR", 1250, 28592, 1250, "hr", "Croatian (Croatia)"), 
                    new CultureData(0x41B, "sk-SK", 1250, 28592, 1250, "sk", "Slovak (Slovakia)"), 
                    new CultureData(0x41D, "sv-SE", 1252, 28591, 1252, "sv", "Swedish (Sweden)"), 
                    new CultureData(0x41E, "th-TH", 874, 874, 874, "th", "Thai (Thailand)"), 
                    new CultureData(0x41F, "tr-TR", 1254, 28599, 1254, "tr", "Turkish (Turkey)"), 
                    new CultureData(0x420, "ur-PK", 1256, 1256, 1256, "ur", "Urdu (Islamic Republic of Pakistan)"), 
                    new CultureData(0x421, "id-ID", 1252, 28591, 1252, "id", "Indonesian (Indonesia)"), 
                    new CultureData(0x422, "uk-UA", 1251, 21866, 1251, "uk", "Ukrainian (Ukraine)"), 
                    new CultureData(0x424, "sl-SI", 1250, 28592, 1250, "sl", "Slovenian (Slovenia)"), 
                    new CultureData(0x425, "et-EE", 1257, 28605, 28605, "et", "Estonian (Estonia)"), 
                    new CultureData(0x426, "lv-LV", 1257, 28603, 28603, "lv", "Latvian (Latvia)"), 
                    new CultureData(0x427, "lt-LT", 1257, 28603, 28603, "lt", "Lithuanian (Lithuania)"), 
                    new CultureData(0x429, "fa-IR", 1256, 1256, 1256, "fa", "Persian (Iran)"), 
                    new CultureData(0x42A, "vi-VN", 1258, 1258, 1258, "vi", "Vietnamese (Vietnam)"), 
                    new CultureData(0x42D, "eu-ES", 1252, 28591, 1252, "eu", "Basque (Basque)"), 
                    new CultureData(0x439, "hi-IN", 1200, 65001, 65001, "hi", "Hindi (India)"), 
                    new CultureData(0x43E, "ms-MY", 1252, 28591, 1252, "ms", "Malay (Malaysia)"), 
                    new CultureData(0x43F, "kk-KZ", 1251, 1251, 1251, "kk", "Kazakh (Kazakhstan)"), 
                    new CultureData(0x456, "gl-ES", 1252, 28591, 1252, "gl", "Galician (Galician)"), 
                    new CultureData(0x464, "fil-PH", 1252, 28591, 1252, "fil", "Filipino (Philippines)"), 
                    new CultureData(0x804, "zh-CN", 936, 936, 936, "zh-CHS", "Chinese (People's Republic of China)"), 
                    new CultureData(0x816, "pt-PT", 1252, 28591, 1252, "pt", "Portuguese (Portugal)"), 
                    new CultureData(0x81A, "sr-Latn-CS", 1250, 28592, 1250, "sr", "Serbian (Latin - Serbia and Montenegro)"), 
                    new CultureData(0xC04, "zh-HK", 950, 950, 950, "zh-CHT", "Chinese (Hong Kong S.A.R.)"), 
                    new CultureData(0xC0A, "es-ES", 1252, 28591, 1252, "es", "Spanish (Spain)"), 
                    new CultureData(0xC1A, "sr-Cyrl-CS", 1251, 1251, 1251, "sr", "Serbian (Cyrillic)"), 
                    new CultureData(0x8411, "ja-JP-Yomi", 932, 50220, 932, "ja", "Japanese (Phonetic)"), 
                    new CultureData(0x1, "ar", 1256, 1256, 1256, null, "Arabic"), 
                    new CultureData(0x2, "bg", 1251, 1251, 1251, null, "Bulgarian"), 
                    new CultureData(0x3, "ca", 1252, 28591, 1252, null, "Catalan"), 
                    new CultureData(0x4, "zh-CHS", 936, 936, 936, null, "Chinese (Simplified)"), 
                    new CultureData(0x5, "cs", 1250, 28592, 1250, null, "Czech"), 
                    new CultureData(0x6, "da", 1252, 28591, 1252, null, "Danish"), 
                    new CultureData(0x7, "de", 1252, 28591, 1252, null, "German"), 
                    new CultureData(0x8, "el", 1253, 28597, 1253, null, "Greek"), 
                    new CultureData(0x9, "en", 1252, 28591, 1252, null, "English"), 
                    new CultureData(0xA, "es", 1252, 28591, 1252, null, "Spanish"), 
                    new CultureData(0xB, "fi", 1252, 28591, 1252, null, "Finnish"), 
                    new CultureData(0xC, "fr", 1252, 28591, 1252, null, "French"), 
                    new CultureData(0xD, "he", 1255, 1255, 1255, null, "Hebrew"), 
                    new CultureData(0xE, "hu", 1250, 28592, 1250, null, "Hungarian"), 
                    new CultureData(0xF, "is", 1252, 28591, 1252, null, "Icelandic"), 
                    new CultureData(0x10, "it", 1252, 28591, 1252, null, "Italian"), 
                    new CultureData(0x11, "ja", 932, 50220, 932, null, "Japanese"), 
                    new CultureData(0x12, "ko", 949, 949, 949, null, "Korean"), 
                    new CultureData(0x13, "nl", 1252, 28591, 1252, null, "Dutch"), 
                    new CultureData(0x14, "no", 1252, 28591, 1252, null, "Norwegian"), 
                    new CultureData(0x15, "pl", 1250, 28592, 1250, null, "Polish"), 
                    new CultureData(0x16, "pt", 1252, 28591, 1252, null, "Portuguese"), 
                    new CultureData(0x18, "ro", 1250, 28592, 1250, null, "Romanian"), 
                    new CultureData(0x19, "ru", 1251, 20866, 1251, null, "Russian"), 
                    new CultureData(0x1A, "hr", 1250, 28592, 1250, null, "Croatian"), 
                    new CultureData(0x1B, "sk", 1250, 28592, 1250, null, "Slovak"), 
                    new CultureData(0x1D, "sv", 1252, 28591, 1252, null, "Swedish"), 
                    new CultureData(0x1E, "th", 874, 874, 874, null, "Thai"), 
                    new CultureData(0x1F, "tr", 1254, 28599, 1254, null, "Turkish"), 
                    new CultureData(0x20, "ur", 1256, 1256, 1256, null, "Urdu"), 
                    new CultureData(0x21, "id", 1252, 28591, 1252, null, "Indonesian"), 
                    new CultureData(0x22, "uk", 1251, 21866, 1251, null, "Ukrainian"), 
                    new CultureData(0x24, "sl", 1250, 28592, 1250, null, "Slovenian"), 
                    new CultureData(0x25, "et", 1257, 28605, 28605, null, "Estonian"), 
                    new CultureData(0x26, "lv", 1257, 28603, 28603, null, "Latvian"), 
                    new CultureData(0x27, "lt", 1257, 28603, 28603, null, "Lithuanian"), 
                    new CultureData(0x29, "fa", 1256, 1256, 1256, null, "Persian"), 
                    new CultureData(0x2A, "vi", 1258, 1258, 1258, null, "Vietnamese"), 
                    new CultureData(0x2D, "eu", 1252, 28591, 1252, null, "Basque"), 
                    new CultureData(0x39, "hi", 1200, 65001, 65001, null, "Hindi"), 
                    new CultureData(0x3E, "ms", 1252, 28591, 1252, null, "Malay"), 
                    new CultureData(0x3F, "kk", 1251, 1251, 1251, null, "Kazakh"), 
                    new CultureData(0x56, "gl", 1252, 28591, 1252, null, "Galician"), 
                    new CultureData(0x64, "fil", 1252, 28591, 1252, null, "Filipino"), 
                    new CultureData(0x7C04, "zh-CHT", 950, 950, 950, null, "Chinese (Traditional)"), 
                    new CultureData(0x7C1A, "sr", 1251, 1251, 1251, null, "Serbian"), 
                };

            var newData = new GlobalizationData();

            Culture invariantCulture = null;
            Culture culture;
            Charset charset;

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            foreach (CultureInfo ci in cultures)
            {
                InternalDebug.Assert(
                    !newData.LocaleIdToCulture.ContainsKey(ci.LCID),
                    String.Format("Culture LCID {0:X} has more than one entry in CLR (\"{1}\")", ci.LCID, ci.Name));

                if (!newData.LocaleIdToCulture.TryGetValue(ci.LCID, out culture))
                {
                    culture = new Culture(ci.LCID, ci.Name);

                    newData.LocaleIdToCulture.Add(ci.LCID, culture);
                    culture.Description = ci.EnglishName;
                    culture.NativeDescription = ci.NativeName;
                    culture.CultureInfo = ci;

                    if (ci.LCID == 127)
                    {
                        invariantCulture = culture;
                    }
                }

                InternalDebug.Assert(
                    !newData.NameToCulture.ContainsKey(ci.Name),
                    String.Format("Culture name \"{0}\" is already reserved in CLR for another LCID ({1:X})", ci.Name, ci.LCID));

                if (!newData.NameToCulture.ContainsKey(ci.Name))
                {
                    newData.NameToCulture.Add(ci.Name, culture);
                }
            }

            foreach (CultureData cd in hardcodedCultures)
            {
                if (!newData.LocaleIdToCulture.TryGetValue(cd.LocaleId, out culture))
                {
                    culture = new Culture(cd.LocaleId, cd.Name);

                    newData.LocaleIdToCulture.Add(cd.LocaleId, culture);
                    culture.Description = cd.Description;
                    culture.NativeDescription = cd.Description;
                }

                if (!newData.NameToCulture.ContainsKey(cd.Name))
                {
                    newData.NameToCulture.Add(cd.Name, culture);
                }
            }

            InternalDebug.Assert(invariantCulture != null);

            // Orphaned WPL code.
#if false
            newData.InvariantCulture = invariantCulture;
#endif

            foreach (CultureInfo ci in cultures)
            {
                culture = newData.LocaleIdToCulture[ci.LCID];

                if (ci.Parent != null)
                {
                    if (newData.LocaleIdToCulture.TryGetValue(ci.Parent.LCID, out Culture parentCulture))
                    {
                        culture.ParentCulture = parentCulture;
                    }
                    else
                    {
                        InternalDebug.Assert(false);
                        culture.ParentCulture = culture;
                    }
                }
                else
                {
                    culture.ParentCulture = culture;
                }
            }

            foreach (CultureData cd in hardcodedCultures)
            {
                culture = newData.LocaleIdToCulture[cd.LocaleId];

                if (culture.ParentCulture == null)
                {
                    if (cd.ParentCultureName != null)
                    {
                        if (newData.NameToCulture.TryGetValue(cd.ParentCultureName, out Culture parentCulture))
                        {
                            culture.ParentCulture = parentCulture;
                        }
                        else
                        {
                            InternalDebug.Assert(false);
                            culture.ParentCulture = culture;
                        }
                    }
                    else
                    {
                        culture.ParentCulture = culture;
                    }
                }
            }

            EncodingInfo[] encodings = Encoding.GetEncodings();

            foreach (EncodingInfo ei in encodings)
            {
                InternalDebug.Assert(
                    !newData.CodePageToCharset.ContainsKey(ei.CodePage),
                    String.Format("CodePage {0} has more than one entry in CLR (\"{1}\")", ei.CodePage, ei.Name));

                if (!newData.CodePageToCharset.TryGetValue(ei.CodePage, out charset))
                {
                    charset = new Charset(ei.CodePage, ei.Name)
                    {
                        Description = ei.DisplayName
                    };

                    newData.CodePageToCharset.Add(ei.CodePage, charset);
                }

                if (!newData.NameToCharset.ContainsKey(ei.Name))
                {
                    newData.NameToCharset.Add(ei.Name, charset);
                    if (ei.Name.Length >
                        newData.MaxCharsetNameLength)
                    {
                        newData.MaxCharsetNameLength = ei.Name.Length;
                    }
                }
            }

            foreach (InternalWindowsCodePage wcp in windowsCodePages)
            {
                if (wcp.LocaleId == 0)
                {
                    InternalDebug.Assert(wcp.CultureName == null);

                    if (wcp.CodePage == 1200 &&
                        invariantCulture != null)
                    {
                        culture = invariantCulture;
                    }
                    else
                    {
                        culture = new Culture(0, null)
                        {
                            ParentCulture = invariantCulture
                        };
                    }

                    culture.Description = wcp.GenericCultureDescription;
                }
                else
                {
                    InternalDebug.Assert(wcp.CultureName != null);

                    if (!newData.LocaleIdToCulture.TryGetValue(wcp.LocaleId, out culture))
                    {
                        InternalDebug.Assert(false);

                        if (!newData.NameToCulture.TryGetValue(wcp.CultureName, out culture))
                        {
                            culture = new Culture(wcp.LocaleId, wcp.CultureName);

                            newData.LocaleIdToCulture.Add(wcp.LocaleId, culture);
                            newData.NameToCulture.Add(wcp.CultureName, culture);
                        }
                    }
                }

                if (!newData.CodePageToCharset.TryGetValue(wcp.CodePage, out charset))
                {
                    InternalDebug.Assert(false);

                    charset = new Charset(wcp.CodePage, wcp.Name);

                    newData.NameToCharset.Add(charset.Name, charset);
                    newData.CodePageToCharset.Add(charset.CodePage, charset);

                    if (charset.Name.Length >
                        newData.MaxCharsetNameLength)
                    {
                        newData.MaxCharsetNameLength = charset.Name.Length;
                    }
                }

                charset.IsWindowsCharset = true;

                culture.WindowsCharset = charset;
                charset.Culture = culture;

                if (!newData.CodePageToCharset.TryGetValue(wcp.MimeCodePage, out charset))
                {
                    charset = newData.CodePageToCharset[wcp.CodePage];
                }

                culture.MimeCharset = charset;

                if (!newData.CodePageToCharset.TryGetValue(wcp.WebCodePage, out charset))
                {
                    charset = newData.CodePageToCharset[wcp.CodePage];
                }

                culture.WebCharset = charset;
            }

            foreach (CharsetName csn in charsetNames)
            {
                if (!newData.NameToCharset.TryGetValue(csn.Name, out charset))
                {
                    if (newData.CodePageToCharset.TryGetValue(csn.CodePage, out charset))
                    {
                        newData.NameToCharset.Add(csn.Name, charset);
                        if (csn.Name.Length >
                            newData.MaxCharsetNameLength)
                        {
                            newData.MaxCharsetNameLength = csn.Name.Length;
                        }
                    }
                }
                else
                {
                    if (charset.CodePage !=
                        csn.CodePage)
                    {
                        if (newData.CodePageToCharset.TryGetValue(csn.CodePage, out charset))
                        {
                            newData.NameToCharset[csn.Name] = charset;
                        }
                    }
                }

                InternalDebug.Assert(charset == null || charset.CodePage == csn.CodePage);
            }

            for (int i = 0; i < CodePageMapData.CodePages.Length; i++)
            {
                if (newData.CodePageToCharset.TryGetValue(CodePageMapData.CodePages[i].Id, out charset))
                {
                    charset.MapIndex = i;
                }

                if (charset.Culture == null)
                {
                    Charset windowsCharset = newData.CodePageToCharset[CodePageMapData.CodePages[i].WindowsId];

                    charset.Culture = windowsCharset.Culture;
                }
            }

            foreach (CultureCodePageOverride over in cultureCodePageOverrides)
            {
                if (newData.NameToCulture.TryGetValue(over.CultureName, out culture))
                {
                    if (newData.CodePageToCharset.TryGetValue(over.MimeCodePage, out charset))
                    {
                        culture.MimeCharset = charset;
                    }

                    if (newData.CodePageToCharset.TryGetValue(over.WebCodePage, out charset))
                    {
                        culture.MimeCharset = charset;
                    }
                }
            }

            foreach (CultureInfo ci in cultures)
            {
                InternalDebug.Assert(newData.LocaleIdToCulture.ContainsKey(ci.LCID));

                culture = newData.LocaleIdToCulture[ci.LCID];

                if (culture.WindowsCharset == null)
                {
                    int windowsCodePage = ci.TextInfo.ANSICodePage;

                    if (windowsCodePage <= 500)
                    {
                        windowsCodePage = 1200;
                    }
                    else
                    {
                        int page = windowsCodePage;
                        bool validWindowsCodepage = windowsCodePages.Any(wcp => page == wcp.CodePage);

                        if (!validWindowsCodepage)
                        {
                            windowsCodePage = 1200;
                        }
                    }

                    charset = newData.CodePageToCharset[windowsCodePage];

                    culture.WindowsCharset = charset;

                    if (culture != culture.ParentCulture)
                    {
                        if (culture.WindowsCharset == culture.ParentCulture.WindowsCharset)
                        {
                            culture.MimeCharset = culture.ParentCulture.MimeCharset;
                            culture.WebCharset = culture.ParentCulture.WebCharset;
                        }
                    }

                    if (culture.MimeCharset == null)
                    {
                        culture.MimeCharset = charset.Culture.MimeCharset;
                    }

                    if (culture.WebCharset == null)
                    {
                        culture.WebCharset = charset.Culture.WebCharset;
                    }
                }
            }

            foreach (CultureData cd in hardcodedCultures)
            {
                InternalDebug.Assert(newData.LocaleIdToCulture.ContainsKey(cd.LocaleId));

                culture = newData.LocaleIdToCulture[cd.LocaleId];

                if (culture.WindowsCharset != null)
                {
                    continue;
                }

                int windowsCodePage = cd.WindowsCodePage;

                InternalDebug.Assert(windowsCodePage > 500);

                charset = newData.CodePageToCharset[windowsCodePage];

                culture.WindowsCharset = charset;


                if (newData.CodePageToCharset.TryGetValue(cd.MimeCodePage, out Charset otherCharset))
                {
                    culture.MimeCharset = otherCharset;
                }

                if (newData.CodePageToCharset.TryGetValue(cd.WebCodePage, out otherCharset))
                {
                    culture.WebCharset = otherCharset;
                }

                if (culture != culture.ParentCulture)
                {
                    if (culture.WindowsCharset ==
                        culture.ParentCulture.WindowsCharset)
                    {
                        if (culture.MimeCharset == null)
                        {
                            culture.MimeCharset = culture.ParentCulture.MimeCharset;
                        }

                        if (culture.WebCharset == null)
                        {
                            culture.WebCharset = culture.ParentCulture.WebCharset;
                        }
                    }
                }

                if (culture.MimeCharset == null)
                {
                    culture.MimeCharset = charset.Culture.MimeCharset;
                }

                if (culture.WebCharset == null)
                {
                    culture.WebCharset = charset.Culture.WebCharset;
                }
            }

            foreach (EncodingInfo ei in encodings)
            {
                charset = newData.CodePageToCharset[ei.CodePage];

                if (charset.Culture == null)
                {
                    int windowsCodePage = 1200;

                    if (charset.TryGetEncoding(out Encoding encoding))
                    {
                        windowsCodePage = encoding.WindowsCodePage;
                        bool validWindowsCodepage = windowsCodePages.Any(wcp => windowsCodePage == wcp.CodePage);

                        if (!validWindowsCodepage)
                        {
                            windowsCodePage = 1200;
                        }
                    }

                    Charset windowsCharset = newData.CodePageToCharset[windowsCodePage];

                    charset.Culture = windowsCharset.Culture;
                }
            }

            foreach (CodePageCultureOverride over in codePageCultureOverrides)
            {
                if (newData.CodePageToCharset.TryGetValue(over.CodePage, out charset))
                {
                    if (newData.NameToCulture.TryGetValue(over.CultureName, out culture))
                    {
                        charset.Culture = culture;
                    }
                }
            }

            if (!newData.LocaleIdToCulture.TryGetValue(CultureInfo.CurrentUICulture.LCID, out Culture newCulture))
            {
                newData.DefaultCulture = newCulture;
                if (!newData.LocaleIdToCulture.TryGetValue(CultureInfo.CurrentCulture.LCID, out newCulture))
                {
                    newData.DefaultCulture = newData.LocaleIdToCulture[0x409];
                }
            }

            /* example of the configuration:

            <Exchange.Data.Globalization>

                <AddCharsetAliasName Charset="us-ascii" AliasName="foo"/>
                <OverrideCharsetDefaultName Charset="us-ascii" DefaultName="foo"/>
                <OverrideCharsetCulture Charset="us-ascii" Culture="jp-JP"/>

                <AddCulture LocaleId="1234" Name="xx-YY" WindowsCharset="1255" MimeCharset="gb-18030" WebCharset="utf-8"/>
                <AddCultureAliasName Culture="xx-YY" AliasName="xx-FOO"/>
                <OverrideCultureCharset Culture="en-US" WindowsCharset="1257" MimeCharset="65001" WebCharset="ascii"/>

                <DefaultCulture Culture="MMM"/>

            </Exchange.Data.Globalization>

            */
            IList<CtsConfigurationSetting> settings = ApplicationServices.Provider.GetConfiguration("Globalization");

            string charSetArg = null;
            string aliasNameArg = null;
            string defaultNameArg = null;
            string cultureArg = null;
            string windowsCharsetArg = null;
            string mimeCharsetArg = null;
            string webCharsetArg = null;
            int codePage;
            int lcid;
            Charset charset1;

            foreach (CtsConfigurationSetting setting in settings)
            {
                string command = setting.Name;
                IList<CtsConfigurationArgument> arguments = setting.Arguments;

                switch (command.ToLower())
                {
                    case "overridecharsetdefaultname":

                        if (arguments.Count != 2)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                            arguments[1].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[0].Value.Trim();
                            defaultNameArg = arguments[1].Value.Trim();
                        }
                        else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                                 arguments[0].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[1].Value.Trim();
                            defaultNameArg = arguments[0].Value.Trim();
                        }
                        else
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (int.TryParse(charSetArg, out codePage))
                        {
                            if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCharset.TryGetValue(charSetArg, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (newData.NameToCharset.TryGetValue(defaultNameArg, out charset1))
                        {
                            if (charset != charset1)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        charset.Name = defaultNameArg;
                        break;

                    case "addcharsetaliasname":

                        if (arguments.Count != 2)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                            arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[0].Value.Trim();
                            aliasNameArg = arguments[1].Value.Trim();
                        }
                        else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                                 arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[1].Value.Trim();
                            aliasNameArg = arguments[0].Value.Trim();
                        }
                        else
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (int.TryParse(charSetArg, out codePage))
                        {
                            if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCharset.TryGetValue(charSetArg, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (newData.NameToCharset.TryGetValue(aliasNameArg, out charset1))
                        {
                            if (charset != charset1)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                            }

                            break;
                        }

                        newData.NameToCharset.Add(aliasNameArg, charset);
                        if (aliasNameArg.Length >
                            newData.MaxCharsetNameLength)
                        {
                            newData.MaxCharsetNameLength = aliasNameArg.Length;
                        }

                        break;

                    case "overridecharsetculture":

                        if (arguments.Count != 2)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                            arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[0].Value.Trim();
                            cultureArg = arguments[1].Value.Trim();
                        }
                        else if (arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) &&
                                 arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                        {
                            charSetArg = arguments[1].Value.Trim();
                            cultureArg = arguments[0].Value.Trim();
                        }
                        else
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (int.TryParse(charSetArg, out codePage))
                        {
                            if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCharset.TryGetValue(charSetArg, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (int.TryParse(cultureArg, out lcid))
                        {
                            if (!newData.LocaleIdToCulture.TryGetValue(lcid, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCulture.TryGetValue(cultureArg, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        charset.Culture = culture;
                        break;

                    case "addculturealiasname":

                        if (arguments.Count != 2)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) &&
                            arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                        {
                            cultureArg = arguments[0].Value.Trim();
                            aliasNameArg = arguments[1].Value.Trim();
                        }
                        else if (arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) &&
                                 arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                        {
                            cultureArg = arguments[1].Value.Trim();
                            aliasNameArg = arguments[0].Value.Trim();
                        }
                        else
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (int.TryParse(cultureArg, out lcid))
                        {
                            if (!newData.LocaleIdToCulture.TryGetValue(lcid, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCulture.TryGetValue(cultureArg, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (newData.NameToCulture.TryGetValue(aliasNameArg, out Culture culture1))
                        {
                            if (culture != culture1)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                            }

                            break;
                        }

                        newData.NameToCulture.Add(aliasNameArg, culture);
                        break;

                    case "overrideculturecharset":

                        cultureArg = null;
                        windowsCharsetArg = null;
                        mimeCharsetArg = null;
                        webCharsetArg = null;

                        foreach (CtsConfigurationArgument arg in arguments)
                        {
                            if (arg.Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                            {
                                cultureArg = arg.Value.Trim();
                            }
                            else if (arg.Name.Equals("WindowsCharset", StringComparison.OrdinalIgnoreCase))
                            {
                                windowsCharsetArg = arg.Value.Trim();
                            }
                            else if (arg.Name.Equals("MimeCharset", StringComparison.OrdinalIgnoreCase))
                            {
                                mimeCharsetArg = arg.Value.Trim();
                            }
                            else if (arg.Name.Equals("WebCharset", StringComparison.OrdinalIgnoreCase))
                            {
                                webCharsetArg = arg.Value.Trim();
                            }
                            else
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(cultureArg) ||
                            (string.IsNullOrEmpty(windowsCharsetArg) && string.IsNullOrEmpty(mimeCharsetArg) &&
                             string.IsNullOrEmpty(webCharsetArg)))
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        if (int.TryParse(cultureArg, out lcid))
                        {
                            if (!newData.LocaleIdToCulture.TryGetValue(lcid, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCulture.TryGetValue(cultureArg, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(windowsCharsetArg))
                        {
                            if (int.TryParse(windowsCharsetArg, out codePage))
                            {
                                if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }
                            else
                            {
                                if (!newData.NameToCharset.TryGetValue(windowsCharsetArg, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }

                            if (!charset.IsWindowsCharset)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }

                            culture.WindowsCharset = charset;
                        }

                        if (!string.IsNullOrEmpty(mimeCharsetArg))
                        {
                            if (int.TryParse(mimeCharsetArg, out codePage))
                            {
                                if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }
                            else
                            {
                                if (!newData.NameToCharset.TryGetValue(mimeCharsetArg, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }

                            culture.MimeCharset = charset;
                        }

                        if (!string.IsNullOrEmpty(webCharsetArg))
                        {
                            if (int.TryParse(webCharsetArg, out codePage))
                            {
                                if (!newData.CodePageToCharset.TryGetValue(codePage, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }
                            else
                            {
                                if (!newData.NameToCharset.TryGetValue(webCharsetArg, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    break;
                                }
                            }

                            culture.WebCharset = charset;
                        }

                        break;

                    case "defaultculture":

                        if (arguments.Count != 1 ||
                            !arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            break;
                        }

                        cultureArg = arguments[0].Value.Trim();

                        if (int.TryParse(cultureArg, out lcid))
                        {
                            if (!newData.LocaleIdToCulture.TryGetValue(lcid, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }
                        else
                        {
                            if (!newData.NameToCulture.TryGetValue(cultureArg, out culture))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                break;
                            }
                        }

                        newData.DefaultCulture = culture;
                        break;

                    default:

                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                        break;
                }
            }

            if (defaultCultureName != null)
            {
                newData.DefaultCulture = newData.NameToCulture[defaultCultureName];
            }

            newData.DefaultDetectionPriorityOrder =
                GetCultureSpecificCodepageDetectionPriorityOrder(newData.DefaultCulture, null);
            invariantCulture.SetCodepageDetectionPriorityOrder(newData.DefaultDetectionPriorityOrder);
            newData.DefaultCulture.GetCodepageDetectionPriorityOrder(newData);

            newData.AsciiCharset = newData.CodePageToCharset[20127];
            newData.Utf8Charset = newData.CodePageToCharset[65001];
            newData.UnicodeCharset = newData.CodePageToCharset[1200];

            return newData;
        }

        /// <summary>
        /// Returns a value indicating if the code page is a double byte code page
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <returns>
        /// True if the code page is a double byte code page otherwise false.
        /// </returns>
        private static bool IsDbcs(int codePage)
        {
            switch (codePage)
            {
                case 932:
                case 949:
                case 950:
                case 936:
                case 50220:
                case 51932:
                case 51949:
                case 50225:
                case 52936:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a boolean indicating if the specified code page is in the specified list.
        /// </summary>
        /// <param name="codePage">
        /// The code page to detect.
        /// </param>
        /// <param name="list">
        /// The code page list.
        /// </param>
        /// <param name="listCount">
        /// The size of the list.
        /// </param>
        /// <returns>
        /// True if the code page is in the list, otherwise false.
        /// </returns>
        private static bool InList(int codePage, int[] list, int listCount)
        {
            for (int i = 0; i < listCount; i++)
            {
                if (list[i] == codePage)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Dectects if the specified code page and the windows code page are for the same language.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <param name="windowsCodePage">
        /// The windows code page.
        /// </param>
        /// <returns>
        /// True if the code page and the windows code page are equivilant.
        /// </returns>
        private static bool IsSameLanguage(int codePage, int windowsCodePage)
        {
            return windowsCodePage == codePage ||
                   (windowsCodePage == 1250 && codePage == 28592) ||
                   (windowsCodePage == 1251 && (codePage == 28595 || codePage == 20866 || codePage == 21866)) ||
                   (windowsCodePage == 1252 && (codePage == 28591 || codePage == 28605)) ||
                   (windowsCodePage == 1253 && codePage == 28597) ||
                   (windowsCodePage == 1254 && codePage == 28599) ||
                   (windowsCodePage == 1255 && codePage == 38598) ||
                   (windowsCodePage == 1256 && codePage == 28596) ||
                   (windowsCodePage == 1257 && codePage == 28594) ||
                   (windowsCodePage == 932 && (codePage == 50220 || codePage == 51932)) ||
                   (windowsCodePage == 949 && (codePage == 50225 || codePage == 51949)) ||
                   (windowsCodePage == 936 & codePage == 52936);
        }

        /// <summary>
        /// The character set name.
        /// </summary>
        private struct CharsetName
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The code page name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// Initializes a new instance of the <see cref="CharsetName"/> struct.
            /// </summary>
            /// <param name="name">
            /// The code page name.
            /// </param>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            public CharsetName(string name, int codePage)
            {
                this.name = name;
                this.codePage = codePage;
            }

            /// <summary>
            /// Gets the name of the character set name.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            /// <summary>
            /// Gets the code page identifier.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return this.codePage;
                }
            }
        }

        /// <summary>
        /// The code page culture override.
        /// </summary>
        private struct CodePageCultureOverride
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// Initializes a new instance of the <see cref="CodePageCultureOverride"/> struct.
            /// </summary>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            public CodePageCultureOverride(int codePage, string cultureName)
            {
                this.codePage = codePage;
                this.cultureName = cultureName;
            }

            /// <summary>
            /// Gets the code page identifier.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return this.codePage;
                }
            }

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return this.cultureName;
                }
            }
        }

        /// <summary>
        /// The culture and code page override structure.
        /// </summary>
        private struct CultureCodePageOverride
        {
            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// The mime code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureCodePageOverride"/> struct.
            /// </summary>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            /// <param name="mimeCodePage">
            /// The mime code page.
            /// </param>
            /// <param name="webCodePage">
            /// The web code page.
            /// </param>
            public CultureCodePageOverride(string cultureName, int mimeCodePage, int webCodePage)
            {
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
            }

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return this.cultureName;
                }
            }

            /// <summary>
            /// Gets the MIME code page identifier.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return this.mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page identifier.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return this.webCodePage;
                }
            }
        }

        /// <summary>
        /// A structure representing the data for culture.
        /// </summary>
        private struct CultureData
        {
            /// <summary>
            /// The culture description.
            /// </summary>
            private readonly string description;

            /// <summary>
            /// The locale identifier.
            /// </summary>
            private readonly int localeId;

            /// <summary>
            /// The mime code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The parent culture name.
            /// </summary>
            private readonly string parentCultureName;

            /// <summary>
            /// The web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// The Windows code page.
            /// </summary>
            private readonly int windowsCodePage;

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureData"/> struct.
            /// </summary>
            /// <param name="localeId">
            /// The local identifier.
            /// </param>
            /// <param name="name">
            /// The culture name.
            /// </param>
            /// <param name="windowsCodePage">
            /// The windows code page.
            /// </param>
            /// <param name="mimeCodePage">
            /// The mime code page.
            /// </param>
            /// <param name="webCodePage">
            /// The web code page.
            /// </param>
            /// <param name="parentCultureName">
            /// The parent culture name.
            /// </param>
            /// <param name="description">
            /// The culture description.
            /// </param>
            public CultureData(
                int localeId,
                string name,
                int windowsCodePage,
                int mimeCodePage,
                int webCodePage,
                string parentCultureName,
                string description)
            {
                this.localeId = localeId;
                this.name = name;
                this.windowsCodePage = windowsCodePage;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.parentCultureName = parentCultureName;
                this.description = description;
            }

            /// <summary>
            /// Gets the culture locale identifier.
            /// </summary>
            public int LocaleId
            {
                get
                {
                    return this.localeId;
                }
            }

            /// <summary>
            /// Gets the culture name.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            /// <summary>
            /// Gets the Windows code page for this culture.
            /// </summary>
            public int WindowsCodePage
            {
                get
                {
                    return this.windowsCodePage;
                }
            }

            /// <summary>
            /// Gets the MIME code page.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return this.mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return this.webCodePage;
                }
            }

            /// <summary>
            /// Gets the name of the parent culture.
            /// </summary>
            public string ParentCultureName
            {
                get
                {
                    return this.parentCultureName;
                }
            }

            /// <summary>
            /// Gets the description of the culture.
            /// </summary>
            public string Description
            {
                get
                {
                    return this.description;
                }
            }
        }

        /// <summary>
        /// A structure encapsulting Windows code page.
        /// </summary>
        private struct InternalWindowsCodePage
        {
            /// <summary>
            /// The code page.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// The generic culture description.
            /// </summary>
            private readonly string genericCultureDescription;

            /// <summary>
            /// The locale identifier.
            /// </summary>
            private readonly int localeId;

            /// <summary>
            /// The MIME code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The code page name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The equivilant web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalWindowsCodePage"/> struct.
            /// </summary>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            /// <param name="name">
            /// The name of the code page.
            /// </param>
            /// <param name="localeId">
            /// The locale id.
            /// </param>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            /// <param name="mimeCodePage">
            /// The MIME code page.
            /// </param>
            /// <param name="webCodePage">
            /// The equivilant web code page.
            /// </param>
            /// <param name="genericCultureDescription">
            /// The generic culture description.
            /// </param>
            public InternalWindowsCodePage(
                int codePage,
                string name,
                int localeId,
                string cultureName,
                int mimeCodePage,
                int webCodePage,
                string genericCultureDescription)
            {
                this.codePage = codePage;
                this.name = name;
                this.localeId = localeId;
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.genericCultureDescription = genericCultureDescription;
            }

            /// <summary>
            /// Gets the code page number.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return this.codePage;
                }
            }

            /// <summary>
            /// Gets the name of the code page.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            /// <summary>
            /// Gets the locale identifier.
            /// </summary>
            public int LocaleId
            {
                get
                {
                    return this.localeId;
                }
            }

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return this.cultureName;
                }
            }

            /// <summary>
            /// Gets the MIME code page.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return this.mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return this.webCodePage;
                }
            }

            /// <summary>
            /// Gets the generic culture description.
            /// </summary>
            public string GenericCultureDescription
            {
                get
                {
                    return this.genericCultureDescription;
                }
            }
        }

        /// <summary>
        /// Encapsultates globalization data
        /// </summary>
        internal class GlobalizationData
        {
            /// <summary>
            /// The local identifier to culture map.
            /// </summary>
            private Dictionary<int, Culture> localeIdToCulture = new Dictionary<int, Culture>(IntComparerInstance);

            /// <summary>
            /// The name to character set map.
            /// </summary>
            private Dictionary<string, Charset> nameToCharset =
                new Dictionary<string, Charset>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// The name to culture map.
            /// </summary>
            private Dictionary<string, Culture> nameToCulture =
                new Dictionary<string, Culture>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// The code page to charset map.
            /// </summary>
            private Dictionary<int, Charset> codePageToCharset = new Dictionary<int, Charset>(IntComparerInstance);

            // Orphaned WPL code.
#if false
            /// <summary>
            /// The invariant culture.
            /// </summary>
            private Culture invariantCulture;
#endif

            /// <summary>
            /// Gets or sets the default culture.
            /// </summary>
            public Culture DefaultCulture
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the unicode charset.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses default get/set syntax.")]
            public Charset UnicodeCharset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the utf 8 charset.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses default get/set syntax.")]
            public Charset Utf8Charset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the default detection priority order.
            /// </summary>
            public int[] DefaultDetectionPriorityOrder
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the code page to charset map.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses dictionary syntax to set value.  FXCop misses this.")]
            public Dictionary<int, Charset> CodePageToCharset
            {
                get
                {
                    return this.codePageToCharset;
                }

                set
                {
                    this.codePageToCharset = value;
                }
            }

            // Orphaned WPL code.
#if false
            /// <summary>
            /// Gets or sets the invariant culture.
            /// </summary>
            public Culture InvariantCulture
            {
                // Orphaned WPL code.
#if false
                get
                {
                    return this.invariantCulture;
                }
#endif

                set
                {
                    this.invariantCulture = value;
                }
            }
#endif

            /// <summary>
            /// Gets or sets the name to character set map.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses dictionary syntax to set value.  FXCop misses this.")]
            public Dictionary<string, Charset> NameToCharset
            {
                get
                {
                    return this.nameToCharset;
                }

                set
                {
                    this.nameToCharset = value;
                }
            }

            /// <summary>
            /// Gets or sets the local identifier to culture map.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Uses dictionary syntax to set value.  FXCop misses this.")]
            public Dictionary<int, Culture> LocaleIdToCulture
            {
                get
                {
                    return this.localeIdToCulture;
                }

                set
                {
                    this.localeIdToCulture = value;
                }
            }

            /// <summary>
            /// Gets or sets the maximum length for a character set name.
            /// </summary>
            internal int MaxCharsetNameLength
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name to culture map.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Uses dictionary syntax to set value.  FXCop misses this.")]
            internal Dictionary<string, Culture> NameToCulture
            {
                get
                {
                    return this.nameToCulture;
                }

                set
                {
                    this.nameToCulture = value;
                }
            }

            /// <summary>
            /// Gets or sets the ascii charset.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Uses default get/set syntax.")]
            internal Charset AsciiCharset
            {
                get;
                set;
            }
        }

        /// <summary>
        /// An integer comparator.
        /// </summary>
        private sealed class IntComparer : IEqualityComparer<int>
        {
            /// <summary>
            /// Determines whether the specified integers are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>True if the integers are equal, otherwise false.</returns>
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            /// <summary>
            /// Returns a hash code for the specified integer.
            /// </summary>
            /// <param name="obj">
            /// The integer for which a hash code is to be returned.
            /// </param>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            public int GetHashCode(int obj)
            {
                return obj;
            }
        }
    }
}