// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unicode.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Performs Unicode unit tests for each of the encoder methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unicode test cases for AntiXss library. 
    /// Following Test cases shall provide testing coverage of each Unicode
    /// code page version 5.2 as defined in http://www.Unicode.org.
    /// </summary>
    [TestClass]
    public class Unicode
    {
        /// <summary>
        /// Tries combining Arabic character sets.
        /// </summary>
        [TestMethod]
        public void UnicodeCombineMarkSafe()
        {
            // 1st: set Arabic code page as safe and run unit tests
            const long codePageStart = 0x0600;
            const long codePageEnd = 0x06FF;
            const string codePageTitle = "Arabic 0x0600-0x06FF";

            UnicodeCharacterEncoder.MarkAsSafe(LowerCodeCharts.Arabic, LowerMidCodeCharts.None, MidCodeCharts.None, UpperMidCodeCharts.None, UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arabic http://www.Unicode.org/charts/PDF/U0600.pdf
            List<long> unicodeGaps = new List<long>()
            {
                 0x0604, 0x0605, 0x061C, 0x061D, 0x0620, 0x065F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);

            // 2nd: Set ArabicSupplement as safe and run unit tests for both Arabic and ArabicSupplement code pages
            UnicodeCharacterEncoder.MarkAsSafe(LowerCodeCharts.Arabic | LowerCodeCharts.ArabicSupplement, LowerMidCodeCharts.None, MidCodeCharts.None, UpperMidCodeCharts.None, UpperCodeCharts.None);

            const long codePageStart2 = 0x0750;
            const long codePageEnd2 = 0x077F;
            const string codePageTitle2 = "Arabic 0x0600-0x06FF and Arabic Supplement 0x0750-0x077F";

            this.CallUnitTests(codePageStart2, codePageEnd2, codePageTitle2, unicodeGaps);
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle + " - round2", unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Arabic 0x0600-0x06FF
        /// </summary>
        [TestMethod]
        public void UnicodeArabic()
        {
            const long codePageStart = 0x0600;
            const long codePageEnd = 0x06FF;
            const string codePageTitle = "Arabic 0x0600-0x06FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Arabic, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arabic http://www.Unicode.org/charts/PDF/U0600.pdf
            List<long> unicodeGaps = new List<long>()
            {
                 0x0604, 0x0605, 0x061C, 0x061D, 0x0620, 0x065F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Arabic Supplement 0x0750-0x077F
        /// </summary>
        [TestMethod]
        public void UnicodeArabicSupplement()
        {
            const long codePageStart = 0x0750;
            const long codePageEnd = 0x077F;
            const string codePageTitle = "Arabic Supplement 0x0750-0x077F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.ArabicSupplement, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arabic Supplement http://www.Unicode.org/charts/PDF/U0750.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Arabic Presentation Forms-A 0xFB50-0xFDFF
        /// </summary>
        [TestMethod]
        public void UnicodeArabicPresentationFormsA()
        {
            const long codePageStart = 0xFB50;
            const long codePageEnd = 0xFDFF;
            const string codePageTitle = "Arabic Presentation Forms-A 0xFB50-0xFDFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.ArabicPresentationFormsA);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arabic Presentation Forms-A http://www.Unicode.org/charts/PDF/UFB50.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFBB2, 0xFBB3, 0xFBB4, 0xFBB5, 0xFBB6, 0xFBB7, 0xFBB8, 0xFBB9, 0xFBBA, 0xFBBB, 0xFBBC, 0xFBBD, 0xFBBE, 0xFBBF,
                0xFBC0, 0xFBC1, 0xFBC2, 0xFBC3, 0xFBC4, 0xFBC5, 0xFBC6, 0xFBC7, 0xFBC8, 0xFBC9, 0xFBCA, 0xFBCB, 0xFBCC, 0xFBCD, 0xFBCE, 0xFBCF,
                0xFBD0, 0xFBD1, 0xFBD2,
                0xFD40, 0xFD41, 0xFD42, 0xFD43, 0xFD44, 0xFD45, 0xFD46, 0xFD47, 0xFD48, 0xFD49, 0xFD4A, 0xFD4B, 0xFD4C, 0xFD4D, 0xFD4E, 0xFD4F,
                0xFD90, 0xFD91,
                0xFDC8, 0xFDC9, 0xFDCA, 0xFDCB, 0xFDCC, 0xFDCD, 0xFDCE, 0xFDCF,
                0xFDD0, 0xFDD1, 0xFDD2, 0xFDD3, 0xFDD4, 0xFDD5, 0xFDD6, 0xFDD7, 0xFDD8, 0xFDD9, 0xFDDA, 0xFDDB, 0xFDDC, 0xFDDD, 0xFDDE, 0xFDDF,
                0xFDE0, 0xFDE1, 0xFDE2, 0xFDE3, 0xFDE4, 0xFDE5, 0xFDE6, 0xFDE7, 0xFDE8, 0xFDE9, 0xFDEA, 0xFDEB, 0xFDEC, 0xFDED, 0xFDEE, 0xFDEF,
                0xFDFE, 0xFDFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Arabic Presentation Forms-B 0xFB50-0xFDFF
        /// </summary>
        [TestMethod]
        public void UnicodeArabicPresentationFormsB()
        {
            const long codePageStart = 0xFE70;
            const long codePageEnd = 0xFEFF;
            const string codePageTitle = "Arabic Presentation Forms-B 0xFB50-0xFDFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.ArabicPresentationFormsB);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arabic Presentation Forms-B http://www.Unicode.org/charts/PDF/UFE70.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFE75, 0xFEFD, 0xFEFE
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Arrows 0x2190-0x21FF
        /// </summary>
        [TestMethod]
        public void UnicodeArrows()
        {
            const long codePageStart = 0x2190;
            const long codePageEnd = 0x21FF;
            const string codePageTitle = "Arrows 0x2190-0x21FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.Arrows, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Arrows http://www.Unicode.org/charts/PDF/U2190.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // there are no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Armenian 0x0530-0x058F
        /// </summary>
        [TestMethod]
        public void UnicodeArmenian()
        {
            const long codePageStart = 0x0530;
            const long codePageEnd = 0x058F;
            const string codePageTitle = "Armenian 0x0530-0x058F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Armenian, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Armenian http://www.Unicode.org/charts/PDF/U0530.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0530, 0x0557, 0x0558, 0x0560, 0x0588, 0x058B, 0x058C, 0x058D, 0x058E, 0x058F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Armenian Ligatures/Alphabetic Presentation Forms 0xFB00-0xFB4F
        /// </summary>
        [TestMethod]
        public void UnicodeArmenianLigatures_AlphabeticPresentationForms()
        {
            const long codePageStart = 0xFB00;
            const long codePageEnd = 0xFB4F;
            const string codePageTitle = "Armenian Ligatures/Alphabetic Presentation Forms 0xFB00-0xFB4F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Armenian, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.AlphabeticPresentationForms);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Alphabetic Presentation Forms http://www.Unicode.org/charts/PDF/UFB00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFB07, 0xFB08, 0xFB09, 0xFB0A, 0xFB0B, 0xFB0C, 0xFB0D, 0xFB0E, 0xFB0F, 0xFB10, 0xFB11, 0xFB12, 0xFB18, 0xFB19, 0xFB1A, 0xFB1B, 0xFB1C, 0xFB37, 0xFB3D, 0xFB3F, 0xFB42, 0xFB4, 0xFB45
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Balinese 0x1B00-0x1B7F
        /// </summary>
        [TestMethod]
        public void UnicodeBalinese()
        {
            const long codePageStart = 0x1B00;
            const long codePageEnd = 0x1B7F;
            const string codePageTitle = "Balinese 0x1B00-0x1B7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Balinese, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Balinese http://www.Unicode.org/charts/PDF/U1B00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1B4C, 0x1B4D, 0x1B4E, 0x1B4F, 0x1B7D, 0x1B7E, 0x1B7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Bamum 0xA6A0-0xA6FF
        /// </summary>
        [TestMethod]
        public void UnicodeBamum()
        {
            const long codePageStart = 0xA6A0;
            const long codePageEnd = 0xA6FF;
            const string codePageTitle = "Bamum 0xA6A0-0xA6FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.Bamum, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Bamum http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-A6A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA6F8, 0xA6F9, 0xA6FA, 0xA6FB, 0xA6FC, 0xA6FD, 0xA6FE, 0xA6FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from C0 Controls and Basic Latin 0x0000-0x007A
        /// </summary>
        [TestMethod]
        public void UnicodeBasicLatin()
        {
            const long codePageStart = 0x0000;
            const long codePageEnd = 0x007F;

            const string codePageTitle = "C0 Controls and Basic Latin 0x0000-0x007A";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.BasicLatin, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 C0 Controls Basic Latin http://www.Unicode.org/charts/PDF/U0000.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E, 0x000F,
                0x0010, 0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 0x0018, 0x0019, 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Bengali 0x0980-0x09FF
        /// </summary>
        [TestMethod]
        public void UnicodeBengali()
        {
            const long codePageStart = 0x0980;
            const long codePageEnd = 0x09FF;
            const string codePageTitle = "Bengali 0x0980-0x09FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Bengali, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Bengali http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-0980.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0980, 0x0984, 0x098D, 0x098E, 0x0991, 0x0992, 0x09A9, 0x09B1, 0x09B3, 0x09B4, 0x09B5, 0x09BA, 0x09BB, 0x09C5, 0x09C6, 0x09C9, 0x09CA, 0x09CF,
                0x09D0, 0x09D1, 0x09D2, 0x09D3, 0x09D4, 0x09D5, 0x09D6, 0x09D8, 0x09D9, 0x09DA, 0x09DB, 0x09DE, 0x09E4, 0x09E5, 0x09FC, 0x09FD, 0x09FE, 0x09FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Block Elements 0x2580-0x259F
        /// </summary>
        [TestMethod]
        public void UnicodeBlockElements()
        {
            const long codePageStart = 0x2580;
            const long codePageEnd = 0x259F;
            const string codePageTitle = "Block Elements 0x2580-0x259F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.BlockElements, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Block Elements http://www.Unicode.org/charts/PDF/U2580.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Bopomofo 0x3100-0x312F
        /// </summary>
        [TestMethod]
        public void UnicodeBopomofo()
        {
            const long codePageStart = 0x3100;
            const long codePageEnd = 0x312F;
            const string codePageTitle = "Bopomofo 0x3100-0x312F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.Bopomofo, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Bopomofo http://www.Unicode.org/charts/PDF/U3100.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x3100, 0x3101, 0x3102, 0x3103, 0x3104, 0x312E, 0x312F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Bopomofo Extended 0x31A0-0x31BF
        /// </summary>
        [TestMethod]
        public void UnicodeBopomofoExtended()
        {
            const long codePageStart = 0x31A0;
            const long codePageEnd = 0x31BF;
            const string codePageTitle = "Bopomofo Extended 0x31A0-0x31BF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.BopomofoExtended, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Bopomofo Extended http://www.Unicode.org/charts/PDF/U31A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x31B8,  0x31B9,  0x31BA,  0x31BB,  0x31BC,  0x31BD,  0x31BE,  0x31BF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Box Drawing 0x2500-0x257F
        /// </summary>
        [TestMethod]
        public void UnicodeBoxDrawing()
        {
            const long codePageStart = 0x2500;
            const long codePageEnd = 0x257F;
            const string codePageTitle = "Box Drawing 0x2500-0x257F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.BoxDrawing, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Box Drawing http://www.Unicode.org/charts/PDF/U2500.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Braille Patterns 0x2800-0x28FF
        /// </summary>
        [TestMethod]
        public void UnicodeBraillePatterns()
        {
            const long codePageStart = 0x2800;
            const long codePageEnd = 0x28FF;
            const string codePageTitle = "Braille Patterns 0x2800-0x28FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.BraillePatterns, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Braille Patterns http://www.Unicode.org/charts/PDF/U2800.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Buginese 0x1A00-0x1A1F
        /// </summary>
        [TestMethod]
        public void UnicodeBuginese()
        {
            const long codePageStart = 0x1A00;
            const long codePageEnd = 0x1A1F;
            const string codePageTitle = "Buginese 0x1A00-0x1A1F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Buginese, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Buginese http://www.Unicode.org/charts/PDF/U1A00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1A1C, 0x1A1D
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Buhid 0x1740-0x175F
        /// </summary>
        [TestMethod]
        public void UnicodeBuhid()
        {
            const long codePageStart = 0x1740;
            const long codePageEnd = 0x175F;
            const string codePageTitle = "Buhid 0x1740-0x175F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Buhid, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Buhid http://www.Unicode.org/charts/PDF/U1740.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1754, 0x1755, 0x1756, 0x1757, 0x1758, 0x1759, 0x175A, 0x175B, 0x175C, 0x175D, 0x175E, 0x175F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cham 0xAA00-0xAA5F
        /// </summary>
        [TestMethod]
        public void UnicodeCham()
        {
            const long codePageStart = 0xAA00;
            const long codePageEnd = 0xAA5F;
            const string codePageTitle = "Cham 0xAA00-0xAA5F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.Cham);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cham http://www.Unicode.org/charts/PDF/UAA00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xAA37, 0xAA38, 0xAA39, 0xAA3A, 0xAA3B, 0xAA3C, 0xAA3D, 0xAA3E, 0xAA3F, 0xAA4E, 0xAA4F, 0xAA5A, 0xAA5B
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cherokee 0x13A0-0x13FF
        /// </summary>
        [TestMethod]
        public void UnicodeCherokee()
        {
            const long codePageStart = 0x13A0;
            const long codePageEnd = 0x13FF;
            const string codePageTitle = "Cherokee 0x13A0-0x13FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Cherokee, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cherokee http://www.Unicode.org/charts/PDF/U13A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x13F5, 0x13F6, 0x13F7, 0x13F8, 0x13F9, 0x13FA, 0x13FB, 0x13FC, 0x13FD, 0x13FE, 0x13FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Compatibility 0x3300-0x33FF
        /// </summary>
        [TestMethod]
        public void UnicodeCjkCompatibility()
        {
            const long codePageStart = 0x3300;
            const long codePageEnd = 0x33FF;
            const string codePageTitle = "CJK Compatibility 0x3300-0x33FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkCompatibility, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK CJK Compatibility http://www.Unicode.org/charts/PDF/U3300.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Compatibility Forms 0xFE30-0xFE4F
        /// </summary>
        [TestMethod]
        public void UnicodeCjkCompatibilityForms()
        {
            const long codePageStart = 0xFE30;
            const long codePageEnd = 0xFE4F;
            const string codePageTitle = "CJK Compatibility Forms 0xFE30-0xFE4F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.CjkCompatibilityForms);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Compatibility Forms http://www.Unicode.org/charts/PDF/UFE30.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Compatibility Ideographs 0xF900-0xFAFF
        /// </summary>
        [TestMethod]
        public void UnicodeCJKCompatibilityIdeographs()
        {
            const long codePageStart = 0xF900;
            const long codePageEnd = 0xFAFF;
            const string codePageTitle = "CJK Compatibility Ideographs 0xF900-0xFAFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.CjkCompatibilityIdeographs);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Compatibility Ideographs http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-F900.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFA2E, 0xFA2F, 0xFA6E, 0xFA6F, 0xFADA, 0xFADB, 0xFADC, 0xFADD, 0xFADE, 0xFADF,
                0xFAE0, 0xFAE1, 0xFAE2, 0xFAE3, 0xFAE4, 0xFAE5, 0xFAE6, 0xFAE7, 0xFAE8, 0xFAE9, 0xFAEA, 0xFAEB, 0xFAEC, 0xFAED, 0xFAEE, 0xFAEF,
                0xFAF0, 0xFAF1, 0xFAF2, 0xFAF3, 0xFAF4, 0xFAF5, 0xFAF6, 0xFAF7, 0xFAF8, 0xFAF9, 0xFAFA, 0xFAFB, 0xFAFC, 0xFAFD, 0xFAFE, 0xFAFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Radicals Supplement 0x2E80-0x2EFF
        /// </summary>
        [TestMethod]
        public void UnicodeCJKRadicalsSupplement()
        {
            const long codePageStart = 0x2E80;
            const long codePageEnd = 0x2EFF;
            const string codePageTitle = "CJK Radicals Supplement 0x2E80-0x2EFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkRadicalsSupplement, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Radicals Supplement http://www.Unicode.org/charts/PDF/U2E80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2E9A, 0x2EF4, 0x2EF5, 0x2EF6, 0x2EF7, 0x2EF8, 0x2EF9, 0x2EFA, 0x2EFB, 0x2EFC, 0x2EFD, 0x2EFE, 0x2EFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Strokes 0x31C0-0x31EF
        /// </summary>
        [TestMethod]
        public void UnicodeCJKStrokes()
        {
            const long codePageStart = 0x31C0;
            const long codePageEnd = 0x31EF;
            const string codePageTitle = "CJK Strokes 0x31C0-0x31EF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkStrokes, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Strokes http://www.Unicode.org/charts/PDF/U31C0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x31E4, 0x31E5, 0x31E6, 0x31E7, 0x31E8, 0x31E9, 0x31EA, 0x31EB, 0x31EC, 0x31ED, 0x31EE, 0x31EF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Symbols and Punctuation 0x3000-0x303F
        /// </summary>
        [TestMethod]
        public void UnicodeCJKSymbolsandPunctuation()
        {
            const long codePageStart = 0x3000;
            const long codePageEnd = 0x303F;
            const string codePageTitle = "CJK Symbols and Punctuation 0x3000-0x303F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkSymbolsAndPunctuation, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Symbols and Punctuation http://www.Unicode.org/charts/PDF/U3000.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Unified Ideographs 0x4300-0x9FCF
        /// </summary>
        [TestMethod]
        public void UnicodeCJKUnifiedIdeographs()
        {
            const long codePageStart = 0x4E00;
            const long codePageEnd = 0x9FCF;
            const string codePageTitle = "CJK Unified Ideographs 0x4E00-0x9FCF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkUnifiedIdeographs, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Unified Ideographs http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-4E00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x9FCC, 0x9FCD, 0x9FCE, 0x9FCF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from CJK Unified Ideographs Extension A 0x3400-0x4DBF
        /// </summary>
        [TestMethod]
        public void UnicodeCJKUnifiedIdeographsExtensionA()
        {
            const long codePageStart = 0x3400;
            const long codePageEnd = 0x4DBF;
            const string codePageTitle = "CJK Unified Ideographs Extension A 0x3400-0x4DBF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CjkUnifiedIdeographsExtensionA, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 CJK Unified Ideographs Extension A http://www.Unicode.org/charts/PDF/U3400.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x4DB6, 0x4DB7, 0x4DB8, 0x4DB9, 0x4DBA, 0x4DBB, 0x4DBC, 0x4DBD, 0x4DBE, 0x4DBF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Combining Diacritical Marks 0x0300-0x036F
        /// </summary>
        [TestMethod]
        public void UnicodeCombiningDiacriticalMarks()
        {
            const long codePageStart = 0x0300;
            const long codePageEnd = 0x036F;
            const string codePageTitle = "Combining Diacritical Marks 0x0300-0x036F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.CombiningDiacriticalMarks, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Combining Diacritical Marks http://www.Unicode.org/charts/PDF/U0300.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Combining Diacritical Marks for Symbols 0x20D0-0x20FF
        /// </summary>
        [TestMethod]
        public void UnicodeCombiningDiacriticalMarksforSymbols()
        {
            const long codePageStart = 0x20D0;
            const long codePageEnd = 0x20FF;
            const string codePageTitle = "Combining Diacritical Marks for Symbols 0x20D0-0x20FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.CombiningDiacriticalMarksForSymbols, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Combining Diacritical Marks for Symbols http://www.Unicode.org/charts/PDF/U20D0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x20F1, 0x20F2, 0x20F3, 0x20F4, 0x20F5, 0x20F6, 0x20F7, 0x20F8, 0x20F9, 0x20FA, 0x20FB, 0x20FC, 0x20FD, 0x20FE, 0x20FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Combining Diacritical Marks Supplement 0x1DC0-0x1DFF
        /// </summary>
        [TestMethod]
        public void UnicodeCombiningDiacriticalMarksSupplement()
        {
            const long codePageStart = 0x1DC0;
            const long codePageEnd = 0x1DFF;
            const string codePageTitle = "Combining Diacritical Marks Supplement 0x1DC0-0x1DFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.CombiningDiacriticalMarksSupplement, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Combining Diacritical Marks Supplement http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-1DC0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1DE7, 0x1DE8, 0x1DE9, 0x1DEA, 0x1DEB, 0x1DEC, 0x1DED, 0x1DEE, 0x1DEF, 0x1DF0, 0x1DF1, 0x1DF2, 0x1DF3, 0x1DF4, 0x1DF5, 0x1DF6, 0x1DF7, 0x1DF8, 0x1DF9, 0x1DFA, 0x1DFB, 0x1DFC
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Combining Half Marks 0xFE20-0xFE2F
        /// </summary>
        [TestMethod]
        public void UnicodeCombiningHalfMarks()
        {
            const long codePageStart = 0xFE20;
            const long codePageEnd = 0xFE2F;
            const string codePageTitle = "Combining Half Marks 0xFE20-0xFE2F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.CombiningHalfMarks);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Combining Half Marks http://www.Unicode.org/charts/PDF/UFE20.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFE27, 0xFE28, 0xFE29, 0xFE2A, 0xFE2B, 0xFE2C, 0xFE2D, 0xFE2E, 0xFE2F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Common Indic Number Forms 0xA830-0xA83F
        /// </summary>
        [TestMethod]
        public void UnicodeCommonIndicNumberForms()
        {
            const long codePageStart = 0xA830;
            const long codePageEnd = 0xA83F;
            const string codePageTitle = "Common Indic Number Forms 0xA830-0xA83F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CommonIndicNumberForms, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Common Indic Number Forms http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-A830.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA83A, 0xA83B, 0xA83C, 0xA83D, 0xA83E, 0xA83F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Control Pictures 0x2400-0x243F
        /// </summary>
        [TestMethod]
        public void UnicodeControlPictures()
        {
            const long codePageStart = 0x2400;
            const long codePageEnd = 0x243F;
            const string codePageTitle = "Control Pictures 0x2400-0x243F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.ControlPictures, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Control Pictures http://www.Unicode.org/charts/PDF/U2400.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2427, 0x2428, 0x2429, 0x242A, 0x242B, 0x242C, 0x242D, 0x242E, 0x242F, 0x2430, 0x2431, 0x2432, 0x2433, 0x2434, 0x2435, 0x2436, 0x2437, 0x2438, 0x2439, 0x243A, 0x243B, 0x243C, 0x243D, 0x243E, 0x243F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Coptic 0x2C80-0x2CFF
        /// </summary>
        [TestMethod]
        public void UnicodeCoptic()
        {
            const long codePageStart = 0x2C80;
            const long codePageEnd = 0x2CFF;
            const string codePageTitle = "Coptic 0x2C80-0x2CFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.Coptic, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Coptic http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-2C80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2CF2, 0x2CF3, 0x2CF4, 0x2CF5, 0x2CF6, 0x2CF7, 0x2CF8
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Coptic in Greek block 0x0370-0x03FF
        /// </summary>
        [TestMethod]
        public void UnicodeCopticinGreekblock()
        {
            const long codePageStart = 0x0370;
            const long codePageEnd = 0x03FF;
            const string codePageTitle = "Coptic in Greek block 0x0370-0x03FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.GreekAndCoptic, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Coptic in Greek block http://www.Unicode.org/charts/PDF/U0370.pdf
            List<long> unicodeGaps = new List<long>()
            {
               0x0378, 0x0379, 0x037F, 0x0380, 0x0381, 0x0382, 0x0383, 0x038B, 0x038D, 0x03A2
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Currency Symbols 0x20A0-0x20CF
        /// </summary>
        [TestMethod]
        public void UnicodeCurrencySymbols()
        {
            const long codePageStart = 0x20A0;
            const long codePageEnd = 0x20CF;
            const string codePageTitle = "Currency Symbols 0x20A0-0x20CF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.CurrencySymbols, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Currency Symbols http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-20A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x20B9, 0x20BA, 0x20BB, 0x20BC, 0x20BD, 0x20BE, 0x20BF, 0x20C0, 0x20C1, 0x20C2, 0x20C3, 0x20C4, 0x20C5, 0x20C6, 0x20C7, 0x20C8, 0x20C9, 0x20CA, 0x20CB, 0x20CC, 0x20CD, 0x20CE, 0x20CF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cyrillic 0x0400-0x04FF
        /// </summary>
        [TestMethod]
        public void UnicodeCyrillic()
        {
            const long codePageStart = 0x0400;
            const long codePageEnd = 0x04FF;
            const string codePageTitle = "Cyrillic 0x0400-0x04FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Cyrillic, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cyrillic http://www.Unicode.org/charts/PDF/U0400.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cyrillic Extended-A 0x2DE0-0x2DFF
        /// </summary>
        [TestMethod]
        public void UnicodeCyrillicExtendedA()
        {
            const long codePageStart = 0x2DE0;
            const long codePageEnd = 0x2DFF;
            const string codePageTitle = "Cyrillic Extended-A 0x2DE0-0x2DFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CyrillicExtendedA, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cyrillic Extended-A http://www.Unicode.org/charts/PDF/U2DE0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cyrillic Extended-B 0xA640-0xA69F
        /// </summary>
        [TestMethod]
        public void UnicodeCyrillicExtendedB()
        {
            const long codePageStart = 0xA640;
            const long codePageEnd = 0xA69F;
            const string codePageTitle = "Cyrillic Extended-B 0xA640-0xA69F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.CyrillicExtendedB, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cyrillic Extended-B http://www.Unicode.org/charts/PDF/UA640.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA660, 0xA661, 0xA674, 0xA675, 0xA676, 0xA677, 0xA678, 0xA679, 0xA67A, 0xA67B, 0xA698, 0xA699, 0xA69A, 0xA69B, 0xA69C, 0xA69D, 0xA69E, 0xA69F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Cyrillic Supplement 0x0500-0x052F
        /// </summary>
        [TestMethod]
        public void UnicodeCyrillicSupplement()
        {
            const long codePageStart = 0x0500;
            const long codePageEnd = 0x052F;
            const string codePageTitle = "Cyrillic Supplement 0x0500-0x052F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.CyrillicSupplement, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Cyrillic Supplement http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-0500.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0526, 0x0527, 0x0528, 0x0529, 0x052A, 0x052B, 0x052C, 0x052D, 0x052E, 0x052F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Devanagari 0x0900-0x097F
        /// </summary>
        [TestMethod]
        public void UnicodeDevanagari()
        {
            const long codePageStart = 0x0900;
            const long codePageEnd = 0x097F;
            const string codePageTitle = "Devanagari 0x0900-0x097F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Devanagari, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Devanagari http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-0900.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x093A, 0x093B, 0x094F, 0x0956, 0x0957, 0x0973, 0x0974, 0x0975, 0x0976, 0x0977, 0x0978
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Devanagari Extended 0xA8E0-0xA8FF
        /// </summary>
        [TestMethod]
        public void UnicodeDevanagariExtended()
        {
            const long codePageStart = 0xA8E0;
            const long codePageEnd = 0xA8FF;
            const string codePageTitle = "Devanagari Extended 0xA8E0-0xA8FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.DevanagariExtended);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Devanagari Extended http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-A8E0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA8FC, 0xA8FD, 0xA8FE, 0xA8FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Dingbats – A 0x2700-0x27BF
        /// </summary>
        [TestMethod]
        public void UnicodeDingbats()
        {
            const long codePageStart = 0x2700;
            const long codePageEnd = 0x27BF;
            const string codePageTitle = "Dingbats – A 0x2700-0x27BF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.Dingbats, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Dingbats http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-2700.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2700, 0x2705, 0x270A, 0x270B, 0x2728, 0x274C, 0x274E, 0x2753, 0x2754, 0x2755, 0x275F, 0x2760, 0x2795, 0x2796, 0x2797, 0x27B0, 0x27BF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Enclosed Alphanumerics 0x2460-0x24FF
        /// </summary>
        [TestMethod]
        public void UnicodeEnclosedAlphanumerics()
        {
            const long codePageStart = 0x2460;
            const long codePageEnd = 0x24FF;
            const string codePageTitle = "Enclosed Alphanumerics 0x2460-0x24FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.EnclosedAlphanumerics, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Enclosed Alphanumerics http://www.Unicode.org/charts/PDF/U2460.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Enclosed CJK Letters and Months 0x3200-0x32FF
        /// </summary>
        [TestMethod]
        public void UnicodeEnclosedCJKLettersMonths()
        {
            const long codePageStart = 0x3200;
            const long codePageEnd = 0x32FF;
            const string codePageTitle = "Enclosed CJK Letters and Months 0x3200-0x32FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.EnclosedCjkLettersAndMonths, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Enclosed CJK Letters and Months http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-3200.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x321F, 0x32FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ethiopic 0x1200-0x137F
        /// </summary>
        [TestMethod]
        public void UnicodeEthiopic()
        {
            const long codePageStart = 0x1200;
            const long codePageEnd = 0x137F;
            const string codePageTitle = "Ethiopic 0x1200-0x137F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Ethiopic, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Ethiopic http://www.Unicode.org/charts/PDF/U1200.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1249, 0x124E, 0x124F, 0x1257, 0x1259, 0x125E, 0x125F, 0x1289, 0x128E, 0x128F, 0x12B1, 0x12B6, 0x12B7, 0x12BF,
                0x12c1, 0x12C6, 0x12C7, 0x12D7, 0x1311, 0x1316, 0x1317, 0x135B, 0x135C, 0x135D, 0x135E, 0x137D, 0x137E, 0x137F
           };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ethiopic Extended 0x2D80-0x2DDF
        /// </summary>
        [TestMethod]
        public void UnicodeEthiopicExtended()
        {
            const long codePageStart = 0x2D80;
            const long codePageEnd = 0x2DDF;
            const string codePageTitle = "Ethiopic Extended 0x2D80-0x2DDF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.EthiopicExtended, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Ethiopic Extended http://www.Unicode.org/charts/PDF/U2D80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2D97, 0x2D98, 0x2D99, 0x2D9A, 0x2D9B, 0x2D9C, 0x2D9D, 0x2D9E, 0x2D9F, 0x2DA7, 0x2DAF, 0x2DB7, 0x2DBF, 0x2DC7, 0x2DCF, 0x2DD7, 0x2DDF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ethiopic Supplement 0x1380-0x139F
        /// </summary>
        [TestMethod]
        public void UnicodeEthiopicSupplement()
        {
            const long codePageStart = 0x1380;
            const long codePageEnd = 0x139F;
            const string codePageTitle = "Ethiopic Supplement 0x1380-0x139F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.EthiopicSupplement, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Ethiopic Supplement http://www.Unicode.org/charts/PDF/U1380.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x139A, 0x139B, 0x139C, 0x139D, 0x139E, 0x139F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from General Punctuation 0x2000-0x206F
        /// </summary>
        [TestMethod]
        public void UnicodeGeneralPunctuation()
        {
            const long codePageStart = 0x2000;
            const long codePageEnd = 0x206F;
            const string codePageTitle = "General Punctuation 0x2000-0x206F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.GeneralPunctuation, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 General Punctuation http://www.Unicode.org/charts/PDF/U2000.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2065, 0x2066, 0x2067, 0x2068, 0x2069
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Geometric Shapes 0x25A0-0x25FF
        /// </summary>
        [TestMethod]
        public void UnicodeGeometricShapes()
        {
            const long codePageStart = 0x25A0;
            const long codePageEnd = 0x25FF;
            const string codePageTitle = "Geometric Shapes 0x25A0-0x25FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.GeometricShapes, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Geometric Shapes  http://www.Unicode.org/charts/PDF/U25A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Georgian 0x10A0-0x10FF
        /// </summary>
        [TestMethod]
        public void UnicodeGeorgian()
        {
            const long codePageStart = 0x10A0;
            const long codePageEnd = 0x10FF;
            const string codePageTitle = "Georgian 0x10A0-0x10FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Georgian, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Georgian http://www.Unicode.org/charts/PDF/U10A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x10C6, 0x10C7, 0x10C8, 0x10C9, 0x10CA, 0x10CB, 0x10CC, 0x10CD, 0x10CE, 0x10CF, 0x10FD, 0x10FE, 0x10FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Georgian Supplement 0x2D00-0x2D2F
        /// </summary>
        [TestMethod]
        public void UnicodeGeorgianSupplement()
        {
            const long codePageStart = 0x2D00;
            const long codePageEnd = 0x2D2F;
            const string codePageTitle = "Georgian Supplement 0x2D00-0x2D2F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.GeorgianSupplement, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Georgian Supplement http://www.Unicode.org/charts/PDF/U2D00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2D26, 0x2D27, 0x2D28, 0x2D29, 0x2D2A, 0x2D2B, 0x2D2C, 0x2D2D, 0x2D2E, 0x2D2F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Glagolitic 0x2C00-0x2C5F
        /// </summary>
        [TestMethod]
        public void UnicodeGlagolitic()
        {
            const long codePageStart = 0x2C00;
            const long codePageEnd = 0x2C5F;
            const string codePageTitle = "Glagolitic 0x2C00-0x2C5F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.Glagolitic, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Glagolitic
            List<long> unicodeGaps = new List<long>()
            {
                0x2C2F, 0x2C5F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Greek Extended 0x1F00-0x1FFF
        /// </summary>
        [TestMethod]
        public void UnicodeGreekExtended()
        {
            const long codePageStart = 0x1F00;
            const long codePageEnd = 0x1FFF;
            const string codePageTitle = "Greek Extended 0x1F00-0x1FFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.GreekExtended, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Greek Extended http://www.Unicode.org/charts/PDF/U1F00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1F16, 0x1F17, 0x1F1E, 0x1F1F, 0x1F46, 0x1F47, 0x1F4E, 0x1F4F, 0x1F58, 0x1F5A, 0x1F5C, 0x1F5E,
                0x1F7E, 0x1F7F, 0x1FB5, 0x1FC5, 0x1F, 0x1FD4, 0x1FD5, 0x1FDC, 0x1FF0, 0x1FF1, 0x1FF5, 0x1FFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Gujarati 0x0A80-0x0AFF
        /// </summary>
        [TestMethod]
        public void UnicodeGujarati()
        {
            const long codePageStart = 0x0A80;
            const long codePageEnd = 0x0AFF;
            const string codePageTitle = "Gujarati 0x0A80-0x0AFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Gujarati, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Gujarati http://www.Unicode.org/charts/PDF/U0A80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0A80, 0x0A84, 0x0A8E,
                0x0A92, 0x0AA9, 0x0AB1, 0x0AB4, 0x0ABA, 0x0ABB,
                0x0AC6, 0x0ACA, 0x0ACE, 0x0ACF,
                0x0AD1, 0x0AD2, 0x0AD3, 0x0AD4, 0x0AD5, 0x0AD6, 0x0AD7, 0x0AD8, 0x0AD9, 0x0ADA, 0x0ADB, 0x0ADC, 0x0ADD, 0x0ADE, 0x0ADF,
                0x0AE4, 0x0AE5,
                0x0AF0, 0x0AF2, 0x0AF3, 0x0AF4, 0x0AF5, 0x0AF6, 0x0AF7, 0x0AF8, 0x0AF9, 0x0AFA, 0x0AFB, 0x0AFC, 0x0AFD, 0x0AFE, 0x0AFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Gurmukhi 0x0A00-0x0A7F
        /// </summary>
        [TestMethod]
        public void UnicodeGurmukhi()
        {
            const long codePageStart = 0x0A00;
            const long codePageEnd = 0x0A7F;
            const string codePageTitle = "Gurmukhi 0x0A00-0x0A7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Gurmukhi, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Gurmukhi
            List<long> unicodeGaps = new List<long>()
            {
                0x0A00, 0x0A04, 0x0A0B, 0x0A0C, 0x0A0D, 0x0A0E,
                0x0A11, 0x0A12, 0x0A29, 0x0A31, 0x0A34, 0x0A37, 0x0A3A, 0x0A3B, 0x0A3D,
                0x0A43, 0x0A44, 0x0A45, 0x0A46, 0x0A49, 0x0A4A, 0x0A4E, 0x0A4F,
                0x0A50, 0x0A52, 0x0A53, 0x0A54, 0x0A55, 0x0A56, 0x0A57, 0x0A58, 0x0A5D, 0x0A5F,
                0x0A60, 0x0A61, 0x0A62, 0x0A63, 0x0A64, 0x0A65,
                0x0A76, 0x0A77, 0x0A78, 0x0A79, 0x0A7A, 0x0A7B, 0x0A7C, 0x0A7D, 0x0A7E, 0x0A7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Halfwidth and Fullwidth Forms 0xFF00-0xFFEF
        /// </summary>
        [TestMethod]
        public void UnicodeHalfwidthandFullwidthForms()
        {
            const long codePageStart = 0xFF00;
            const long codePageEnd = 0xFFEF;
            const string codePageTitle = "Halfwidth and Fullwidth Forms 0xFF00-0xFFEF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.HalfWidthAndFullWidthForms);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Halfwidth and Fullwidth Forms
            List<long> unicodeGaps = new List<long>()
            {
                0xFF00, 0xFFBF, 0xFFC0, 0xFFC1, 0xFFC8, 0xFFC9, 0xFFD0, 0xFFD1, 0xFFD8, 0xFFD9, 0xFFDD, 0xFFDE, 0xFFDF, 0xFFE7, 0xFFEF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hangul Compatibility Jamo 0x3130-0x318F
        /// </summary>
        [TestMethod]
        public void UnicodeHangulCompatibilityJamo()
        {
            const long codePageStart = 0x3130;
            const long codePageEnd = 0x318F;
            const string codePageTitle = "Hangul Compatibility Jamo 0x3130-0x318F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.HangulCompatibilityJamo, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hangul Compatibility Jamo http://www.Unicode.org/charts/PDF/U3130.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x3130, 0x318F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hangul Jamo 0x1100-0x11FF
        /// </summary>
        [TestMethod]
        public void UnicodeHangulJamo()
        {
            const long codePageStart = 0x1100;
            const long codePageEnd = 0x11FF;
            const string codePageTitle = "Hangul Jamo 0x1100-0x11FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.HangulJamo, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hangul Jamo http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-1100.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hangul Jamo Extended – A 0x1100-0x11FF
        /// </summary>
        [TestMethod]
        public void UnicodeHangulJamoExtendedA()
        {
            const long codePageStart = 0xA960;
            const long codePageEnd = 0xA97F;
            const string codePageTitle = "Hangul Jamo Extended – A 0xA960-0xA97F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.HangulJamoExtendedA);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hangul Jamo Extended – A http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-A960.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA97D, 0xA97E, 0xA97F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hangul Jamo Extended – B 0xD7B0-0xD7FF
        /// </summary>
        [TestMethod]
        public void UnicodeHangulJamoExtendedB()
        {
            const long codePageStart = 0xD7B0;
            const long codePageEnd = 0xD7FF;
            const string codePageTitle = "Hangul Jamo Extended – B 0xD7B0-0xD7FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.HangulJamoExtendedB);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hangul Jamo Extended – B http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-D7B0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xD7C7, 0xD7C8, 0xD7C9, 0xD7CA, 0xD7FC, 0xD7FD, 0xD7FE, 0xD7FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hangul Syllables 0xAC00-0xD7AF
        /// </summary>
        [TestMethod]
        public void UnicodeHangulSyllables()
        {
            const long codePageStart = 0xAC00;
            const long codePageEnd = 0xD7AF;
            const string codePageTitle = "Hangul Syllables 0xAC00-0xD7AF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.HangulSyllables);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hangul Syllables
            List<long> unicodeGaps = new List<long>()
            {
                0xD7A4, 0xD7A5, 0xD7A6, 0xD7A7, 0xD7A8, 0xD7A9, 0xD7AA, 0xD7AB, 0xD7AC, 0xD7AD, 0xD7AE, 0xD7AF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hanunoo 0x1720-0x173F
        /// </summary>
        [TestMethod]
        public void UnicodeHanunoo()
        {
            const long codePageStart = 0x1720;
            const long codePageEnd = 0x173F;
            const string codePageTitle = "Hanunoo 0x1720-0x173F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.Hanunoo, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hanunoo
            List<long> unicodeGaps = new List<long>()
            {
                0x1737, 0x1738, 0x1739, 0x173A, 0x173B, 0x173C, 0x173D, 0x173E, 0x173F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hebrew 0x0590-0x05FF
        /// </summary>
        [TestMethod]
        public void UnicodeHebrew()
        {
            const long codePageStart = 0x0590;
            const long codePageEnd = 0x05FF;
            const string codePageTitle = "Hebrew 0x0590-0x05FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Hebrew, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hebrew 
            List<long> unicodeGaps = new List<long>()
            {
                0x0590, 0x05C8, 0x05C9, 0x05CA, 0x05CB, 0x05CC, 0x05CD, 0x05CE, 0x05CF, 0x05EB, 0x05EC, 0x05ED, 0x05EE, 0x05EF, 0x05F5, 0x05F6, 0x05F7, 0x05F8, 0x05F9, 0x05FA, 0x05FB, 0x05FC, 0x05FD, 0x05FE, 0x05FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Hiragana 0x3040-0x309F
        /// </summary>
        [TestMethod]
        public void UnicodeHiragana()
        {
            const long codePageStart = 0x3040;
            const long codePageEnd = 0x309F;
            const string codePageTitle = "Hiragana 0x3040-0x309F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.Hiragana, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Hiragana 
            List<long> unicodeGaps = new List<long>()
            {
                0x3040, 0x3097, 0x3098
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ideographic Description Characters 0x2FF0-0x2FFF
        /// </summary>
        [TestMethod]
        public void UnicodeIdeographicDescriptionCharacters()
        {
            const long codePageStart = 0x2FF0;
            const long codePageEnd = 0x2FFF;
            const string codePageTitle = "Ideographic Description Characters 0x2FF0-0x2FFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.IdeographicDescriptionCharacters, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Ideographic Description Characters http://www.Unicode.org/charts/PDF/U2FF0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2FFC, 0x2FFD, 0x2FFE, 0x2FFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from IPA Extensions 0x0250-0x02AF
        /// </summary>
        [TestMethod]
        public void UnicodeIpaExtensions()
        {
            const long codePageStart = 0x0250;
            const long codePageEnd = 0x02AF;
            const string codePageTitle = "IPA Extensions 0x0250-0x02AF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.IpaExtensions, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 IPA Extensions http://www.Unicode.org/charts/PDF/U0250.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Javanese 0xA980-0xA9DF
        /// </summary>
        [TestMethod]
        public void UnicodeJavanese()
        {
            const long codePageStart = 0xA980;
            const long codePageEnd = 0xA9DF;
            const string codePageTitle = "Javanese 0xA980-0xA9DF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.Javanese);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Javanese http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-A980.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA9CE, 0xA9DA, 0xA9DB, 0xA9DC, 0xA9DD
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Kanbun 0x3190-0x319F
        /// </summary>
        [TestMethod]
        public void UnicodeKanbun()
        {
            const long codePageStart = 0x3190;
            const long codePageEnd = 0x319F;
            const string codePageTitle = "Kanbun 0x3190-0x319F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.Kanbun, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Kanbun http://www.Unicode.org/charts/PDF/U3190.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Kangxi Radicals 0x2F00-0x2FDF
        /// </summary>
        [TestMethod]
        public void UnicodeKangxiRadicals()
        {
            const long codePageStart = 0x2F00;
            const long codePageEnd = 0x2FDF;
            const string codePageTitle = "Kangxi Radicals 0x2F00-0x2FDF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.KangxiRadicals, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Kangxi Radicals
            List<long> unicodeGaps = new List<long>()
            {
                0x2FD6, 0x2FD7, 0x2FD8, 0x2FD9, 0x2FDA, 0x2FDB, 0x2FDC, 0x2FDD, 0x2FDE, 0x2FDF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Kannada 0x0C80-0x0CFF
        /// </summary>
        [TestMethod]
        public void UnicodeKannada()
        {
            const long codePageStart = 0x0C80;
            const long codePageEnd = 0x0CFF;
            const string codePageTitle = "Kannada 0x0C80-0x0CFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Kannada, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Kannada http://www.Unicode.org/charts/PDF/U0C80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0C80, 0x0C81, 0x0C84, 0x0C8D, 0x0C91, 0x0CA9, 0x0CB4, 0x0CBA, 0x0CBB, 0x0CC5, 0x0CC9, 0x0CCE, 0x0CCF,
                0x0CD0, 0x0CD1, 0x0CD2, 0x0CD3, 0x0CD4, 0x0CD7, 0x0CD8, 0x0CD9, 0x0CDA, 0x0CDB, 0x0CDC, 0x0CDD, 0x0CDF,
                0x0CE4, 0x0CE5, 0x0CF0, 0x0CF3, 0x0CF4, 0x0CF5, 0x0CF6, 0x0CF7, 0x0CF8, 0x0CF9, 0x0CFA, 0x0CFB, 0x0CFC, 0x0CFD, 0x0CFE, 0x0CFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Katakana 0x30A0-0x30FF
        /// </summary>
        [TestMethod]
        public void UnicodeKatakana()
        {
            const long codePageStart = 0x30A0;
            const long codePageEnd = 0x30FF;
            const string codePageTitle = "Katakana 0x30A0-0x30FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.Katakana, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Katakana http://www.Unicode.org/charts/PDF/U30A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Katakana Phonetic Extensions 0x31F0-0x31FF
        /// </summary>
        [TestMethod]
        public void UnicodeKatakanaPhoneticExtensions()
        {
            const long codePageStart = 0x31F0;
            const long codePageEnd = 0x31FF;
            const string codePageTitle = "Katakana Phonetic Extensions 0x31F0-0x31FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.KatakanaPhoneticExtensions, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Katakana Phonetic Extensions http://www.Unicode.org/charts/PDF/U31F0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Kayah Li 0xA900-0xA92F
        /// </summary>
        [TestMethod]
        public void UnicodeKayahLi()
        {
            const long codePageStart = 0xA900;
            const long codePageEnd = 0xA92F;
            const string codePageTitle = "Kayah Li 0xA900-0xA92F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
                Microsoft.Security.Application.LowerCodeCharts.Default,
                Microsoft.Security.Application.LowerMidCodeCharts.None,
                Microsoft.Security.Application.MidCodeCharts.None,
                Microsoft.Security.Application.UpperMidCodeCharts.None,
                Microsoft.Security.Application.UpperCodeCharts.KayahLi);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Kayah Li http://www.Unicode.org/charts/PDF/UA900.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Khmer 0x1780-0x17FF
        /// </summary>
        [TestMethod]
        public void UnicodeKhmer()
        {
            const long codePageStart = 0x1780;
            const long codePageEnd = 0x17FF;
            const string codePageTitle = "Khmer 0x1780-0x17FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Khmer,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Khmer http://www.Unicode.org/charts/PDF/U1780.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x17DE, 0x17DF, 0x17EA, 0x17EB, 0x17EC, 0x17ED, 0x17EE, 0x17EF, 0x17FA, 0x17FB, 0x17FC, 0x17FD, 0x17FE, 0x17FF
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Khmer Symbols 0x19E0-0x19FF
        /// </summary>
        [TestMethod]
        public void UnicodeKhmerSymbols()
        {
            const long codePageStart = 0x19E0;
            const long codePageEnd = 0x19FF;
            const string codePageTitle = "Khmer Symbols 0x19E0-0x19FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.KhmerSymbols,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Khmer Symbols http://www.Unicode.org/charts/PDF/U19E0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Lao 0x0E80-0x0EFF
        /// </summary>
        [TestMethod]
        public void UnicodeLao()
        {
            const long codePageStart = 0x0E80;
            const long codePageEnd = 0x0EFF;
            const string codePageTitle = "Lao 0x0E80-0x0EFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Lao,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Lao http://www.Unicode.org/charts/PDF/U0E80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0E80,
                0x0E83, 0x0E85, 0x0E86, 0x0E89, 0x0E8B, 0x0E8C, 0x0E8E, 0x0E8F,
                0x0E90, 0x0E91, 0x0E92, 0x0E93, 0x0E98, 0x0EA0, 0x0EA4, 0x0EA6, 0x0EA8, 0x0EA9, 0x0EAC,
                0x0EBA, 0x0EBE, 0x0EBF, 0x0EC5, 0x0EC7, 0x0ECE, 0x0ECF,
                0x0EDA, 0x0EDA, 0x0EDB, 0x0EDE, 0x0EDF,
                0x0EE0, 0x0EE1, 0x0EE2, 0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6, 0x0EE7, 0x0EE8, 0x0EE9, 0x0EEA, 0x0EEB, 0x0EEC, 0x0EED, 0x0EEE, 0x0EEF,
                0x0EF0, 0x0EF1, 0x0EF2, 0x0EF3, 0x0EF4, 0x0EF5, 0x0EF6, 0x0EF7, 0x0EF8, 0x0EF9, 0x0EFA, 0x0EFB, 0x0EFC, 0x0EFD, 0x0EFE, 0x0EFF
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin-1 Supplement 0x2C60-0x2C7F
        /// </summary>
        [TestMethod]
        public void UnicodeLatin1Supplement()
        {
            const long codePageStart = 0x0080;
            const long codePageEnd = 0x00FF;
            const string codePageTitle = "Latin-1 Supplement 0x0080-0x00FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.C1ControlsAndLatin1Supplement, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin-1 Supplement http://www.Unicode.org/charts/PDF/U0080.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
                0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087, 0x0088, 0x0089, 0x008A, 0x008B, 0x008C, 0x008D, 0x008E, 0x008F,
                0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097, 0x0098, 0x0099, 0x009A, 0x009B, 0x009C, 0x009D, 0x009E, 0x009F,
                0x00A0, 0x00AD
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin Extended-A 0x0100-0x017F
        /// </summary>
        [TestMethod]
        public void UnicodeLatinExtendedA()
        {
            const long codePageStart = 0x0100;
            const long codePageEnd = 0x017F;
            const string codePageTitle = "Latin Extended-A 0x0100-0x017F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.LatinExtendedA, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin Extended-A http://www.Unicode.org/charts/PDF/U0100.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin Extended-A 0x0180-0x024F
        /// </summary>
        [TestMethod]
        public void UnicodeLatinExtendedB()
        {
            const long codePageStart = 0x0180;
            const long codePageEnd = 0x024F;
            const string codePageTitle = "Latin Extended-A 0x0180-0x024F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.LatinExtendedB, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin Extended-B http://www.Unicode.org/charts/PDF/U0100.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin Extended-C 0x2C60-0x2C7F
        /// </summary>
        [TestMethod]
        public void UnicodeLatinExtendedC()
        {
            const long codePageStart = 0x2C60;
            const long codePageEnd = 0x2C7F;
            const string codePageTitle = "Latin Extended-C 0x2C60-0x2C7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.LatinExtendedC, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin Extended-C http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-2C60.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin Extended-D 0xA720-0xA7FF
        /// </summary>
        [TestMethod]
        public void UnicodeLatinExtendedD()
        {
            const long codePageStart = 0xA720;
            const long codePageEnd = 0xA7FF;
            const string codePageTitle = "Latin Extended-D 0xA720-0xA7FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.LatinExtendedD, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin Extended-D http://www.Unicode.org/charts/PDF/UA720.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA78D, 0xA78E, 0xA78F, 0xA790, 0xA791, 0xA792, 0xA793, 0xA794, 0xA795, 0xA796, 0xA797, 0xA798, 0xA799, 0xA79A, 0xA79B, 0xA79C, 0xA79D, 0xA79E, 0xA79F,
                0xA7A0, 0xA7A1, 0xA7A2, 0xA7A3, 0xA7A4, 0xA7A5, 0xA7A6, 0xA7A7, 0xA7A8, 0xA7A9, 0xA7AA, 0xA7AB, 0xA7AC, 0xA7AD, 0xA7AE, 0xA7AF,
                0xA7B0, 0xA7B1, 0xA7B2, 0xA7B3, 0xA7B4, 0xA7B5, 0xA7B6, 0xA7B7, 0xA7B8, 0xA7B9, 0xA7BA, 0xA7BB, 0xA7BC, 0xA7BD, 0xA7BE, 0xA7BF,
                0xA7C0, 0xA7C1, 0xA7C2, 0xA7C3, 0xA7C4, 0xA7C5, 0xA7C6, 0xA7C7, 0xA7C8, 0xA7C9, 0xA7CA, 0xA7CB, 0xA7CC, 0xA7CD, 0xA7CE, 0xA7CF,
                0xA7D0, 0xA7D1, 0xA7D2, 0xA7D3, 0xA7D4, 0xA7D5, 0xA7D6, 0xA7D7, 0xA7D8, 0xA7D9, 0xA7DA, 0xA7DB, 0xA7DC, 0xA7DD, 0xA7DE, 0xA7DF,
                0xA7E0, 0xA7E1, 0xA7E2, 0xA7E3, 0xA7E4, 0xA7E5, 0xA7E6, 0xA7E7, 0xA7E8, 0xA7E9, 0xA7EA, 0xA7EB, 0xA7EC, 0xA7ED, 0xA7EE, 0xA7EF,
                0xA7F0, 0xA7F1, 0xA7F2, 0xA7F3, 0xA7F4, 0xA7F5, 0xA7F6, 0xA7F7, 0xA7F8, 0xA7F9, 0xA7FA
       };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Latin Extended Additional 0x1E00-0x1EFF
        /// </summary>
        [TestMethod]
        public void UnicodeLatinExtendedAdditional()
        {
            const long codePageStart = 0x1E00;
            const long codePageEnd = 0x1EFF;
            const string codePageTitle = "Latin Extended Additional 0x1E00-0x1EFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.LatinExtendedAdditional, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Latin Extended Additional  http://www.Unicode.org/charts/PDF/U1E00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Lepcha 0x1C00-0x1C4F
        /// </summary>
        [TestMethod]
        public void UnicodeLepcha()
        {
            const long codePageStart = 0x1C00;
            const long codePageEnd = 0x1C4F;
            const string codePageTitle = "Lepcha 0x1C00-0x1C4F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Lepcha,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Lepcha 
            List<long> unicodeGaps = new List<long>()
            {
                0x1C38, 0x1C39, 0x1C3A, 0x1C4A, 0x1C4B, 0x1C4C
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Letterlike Symbols 0x2100-0x214F
        /// </summary>
        [TestMethod]
        public void UnicodeLetterlikeSymbols()
        {
            const long codePageStart = 0x2100;
            const long codePageEnd = 0x214F;
            const string codePageTitle = "Letterlike Symbols 0x2100-0x214F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.LetterlikeSymbols,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Letterlike Symbols http://www.Unicode.org/charts/PDF/U2100.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Limbu 0x1900-0x194F
        /// </summary>
        [TestMethod]
        public void UnicodeLimbu()
        {
            const long codePageStart = 0x1900;
            const long codePageEnd = 0x194F;
            const string codePageTitle = "Limbu 0x1900-0x194F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Limbu,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Limbu http://www.Unicode.org/charts/PDF/U1900.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x191D, 0x191E, 0x191F, 0x192C, 0x192D, 0x192E, 0x192F, 0x193C, 0x193D, 0x193E, 0x193F, 0x1941, 0x1942, 0x1943
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Lisu 0xA4D0-0xA4FF
        /// </summary>
        [TestMethod]
        public void UnicodeLisu()
        {
            const long codePageStart = 0xA4D0;
            const long codePageEnd = 0xA4FF;
            const string codePageTitle = "Lisu 0xA4D0-0xA4FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.Lisu,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Lisu http://www.Unicode.org/charts/PDF/UA4D0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        ////Not supported by .Net 4.1, out of boundry for testing
        /////// <summary>
        /////// All Unicode characters from Lycian 0x10280-0x1029F
        /////// </summary>
        ////[TestMethod]
        ////public void UnicodeLycian()
        ////{
        ////    long codePageStart = 0x10280;
        ////    long codePageEnd = 0x1029F;
        ////    string codePageTitle = "Lycian 0x10280-0x1029F";

        ////    Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
        ////    Microsoft.Security.Application.LowerCodeCharts.Default,
        ////    Microsoft.Security.Application.LowerMidCodeCharts.None,
        ////    Microsoft.Security.Application.MidCodeCharts.None,
        ////    Microsoft.Security.Application.UpperMidCodeCharts.None,
        ////    Microsoft.Security.Application.UpperCodeCharts.None);

        ////    // compiled list of not assigned from the Unicode Standard, Version 5.2 Lycian
        ////    List<long> unicodeGaps = new List<long>()
        ////    {
        ////        // not supported in .Net 4.0
        ////    };

        ////    this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        ////}

        /// <summary>
        /// All Unicode characters from Malayalam 0x0D00-0x0D7F
        /// </summary>
        [TestMethod]
        public void UnicodeMalayalam()
        {
            const long codePageStart = 0x0D00;
            const long codePageEnd = 0x0D7F;
            const string codePageTitle = "Malayalam 0x0D00-0x0D7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Malayalam,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Malayalam http://www.Unicode.org/charts/PDF/U0D00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0D00, 0x0D01, 0x0D04, 0x0D0D, 0x0D11, 0x0D29, 0x0D3A, 0x0D3B, 0x0D3C,
                0x0D45, 0x0D49, 0x0D4E, 0x0D4F,
                0x0D50, 0x0D51, 0x0D52, 0x0D53, 0x0D54, 0x0D55, 0x0D56, 0x0D58, 0x0D59, 0x0D5A, 0x0D5B, 0x0D5C, 0x0D5D, 0x0D5E, 0x0D5F,
                0x0D64, 0x0D65, 0x0D76, 0x0D77, 0x0D78
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Mathematical Operators 0x2200-0x22FF
        /// </summary>
        [TestMethod]
        public void UnicodeMathematicalOperators()
        {
            const long codePageStart = 0x2200;
            const long codePageEnd = 0x22FF;
            const string codePageTitle = "Mathematical Operators 0x2200-0x22FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.MathematicalOperators,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Mathematical Operators http://www.Unicode.org/charts/PDF/U2200.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Meeteri Mayek 0xABC0-0xABFF
        /// </summary>
        [TestMethod]
        public void UnicodeMeeteriMayek()
        {
            const long codePageStart = 0xABC0;
            const long codePageEnd = 0xABFF;
            const string codePageTitle = "Meeteri Mayek 0xABC0-0xABFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.MeeteiMayek);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Meeteri Mayek http://www.Unicode.org/charts/PDF/UABC0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xABEE, 0xABEF, 0xABFA, 0xABFA, 0xABFB, 0xABFC, 0xABFD, 0xABFE, 0xABFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Miscellaneous Mathematical Symbols-A 0x27C0-0x27EF
        /// </summary>
        [TestMethod]
        public void UnicodeMiscellaneousMathematicalSymbolsA()
        {
            const long codePageStart = 0x27C0;
            const long codePageEnd = 0x27EF;
            const string codePageTitle = "Miscellaneous Mathematical Symbols-A 0x27C0-0x27EF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.MiscellaneousMathematicalSymbolsA,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Miscellaneous Mathematical Symbols-A http://www.Unicode.org/charts/PDF/U27C0.pdf
            List<long> unicodeGaps = new List<long>()
             {
                0x27CB, 0x27CD, 0x27CE, 0x27CF
             };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Miscellaneous Mathematical Symbols-B 0x2980-0x29FF
        /// </summary>
        [TestMethod]
        public void UnicodeMiscellaneousMathematicalSymbolsB()
        {
            const long codePageStart = 0x2980;
            const long codePageEnd = 0x29FF;
            const string codePageTitle = "Miscellaneous Mathematical Symbols-B 0x2980-0x29FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.MiscellaneousMathematicalSymbolsB,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Miscellaneous Mathematical Symbols-B http://www.Unicode.org/charts/PDF/U2980.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Miscellaneous Symbols 0x2600-0x26FF
        /// </summary>
        [TestMethod]
        public void UnicodeMiscellaneousSymbols()
        {
            const long codePageStart = 0x2600;
            const long codePageEnd = 0x26FF;
            const string codePageTitle = "Miscellaneous Symbols 0x2600-0x26FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.MiscellaneousSymbols, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Miscellaneous Symbols http://www.Unicode.org/charts/PDF/U2600.pdf
            List<long> unicodeGaps = new List<long>()
             {
                0x26CE, 0x26E2, 0x26E4, 0x26E5, 0x26E6, 0x26E7
             };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Miscellaneous Symbols and Arrows 0x2B00-0x2BFF
        /// </summary>
        [TestMethod]
        public void UnicodeMiscellaneousSymbolsArrows()
        {
            const long codePageStart = 0x2B00;
            const long codePageEnd = 0x2BFF;
            const string codePageTitle = "Miscellaneous Symbols and Arrows 0x2B00-0x2BFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.MiscellaneousSymbolsAndArrows, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Miscellaneous Symbols and Arrows http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-2B00.pdf
            List<long> unicodeGaps = new List<long>()
             {
                0x2B4D, 0x2B4E, 0x2B4F,
                0x2B5A, 0x2B5B, 0x2B5C, 0x2B5D, 0x2B5E, 0x2B5F,
                0x2B60, 0x2B61, 0x2B62, 0x2B63, 0x2B64, 0x2B65, 0x2B66, 0x2B67, 0x2B68, 0x2B69, 0x2B6A, 0x2B6B, 0x2B6C, 0x2B6D, 0x2B6E, 0x2B6F,
                0x2B70, 0x2B71, 0x2B72, 0x2B73, 0x2B74, 0x2B75, 0x2B76, 0x2B77, 0x2B78, 0x2B79, 0x2B7A, 0x2B7B, 0x2B7C, 0x2B7D, 0x2B7E, 0x2B7F,
                0x2B80, 0x2B81, 0x2B82, 0x2B83, 0x2B84, 0x2B85, 0x2B86, 0x2B87, 0x2B88, 0x2B89, 0x2B8A, 0x2B8B, 0x2B8C, 0x2B8D, 0x2B8E, 0x2B8F,
                0x2B90, 0x2B91, 0x2B92, 0x2B93, 0x2B94, 0x2B95, 0x2B96, 0x2B97, 0x2B98, 0x2B99, 0x2B9A, 0x2B9B, 0x2B9C, 0x2B9D, 0x2B9E, 0x2B9F,
                0x2BA0, 0x2BA1, 0x2BA2, 0x2BA3, 0x2BA4, 0x2BA5, 0x2BA6, 0x2BA7, 0x2BA8, 0x2BA9, 0x2BAA, 0x2BAB, 0x2BAC, 0x2BAD, 0x2BAE, 0x2BAF,
                0x2BB0, 0x2BB1, 0x2BB2, 0x2BB3, 0x2BB4, 0x2BB5, 0x2BB6, 0x2BB7, 0x2BB8, 0x2BB9, 0x2BBA, 0x2BBB, 0x2BBC, 0x2BBD, 0x2BBE, 0x2BBF,
                0x2BC0, 0x2BC1, 0x2BC2, 0x2BC3, 0x2BC4, 0x2BC5, 0x2BC6, 0x2BC7, 0x2BC8, 0x2BC9, 0x2BCA, 0x2BCB, 0x2BCC, 0x2BCD, 0x2BCE, 0x2BCF,
                0x2BD0, 0x2BD1, 0x2BD2, 0x2BD3, 0x2BD4, 0x2BD5, 0x2BD6, 0x2BD7, 0x2BD8, 0x2BD9, 0x2BDA, 0x2BDB, 0x2BDC, 0x2BDD, 0x2BDE, 0x2BDF,
                0x2BE0, 0x2BE1, 0x2BE2, 0x2BE3, 0x2BE4, 0x2BE5, 0x2BE6, 0x2BE7, 0x2BE8, 0x2BE9, 0x2BEA, 0x2BEB, 0x2BEC, 0x2BED, 0x2BEE, 0x2BEF,
                0x2BF0, 0x2BF1, 0x2BF2, 0x2BF3, 0x2BF4, 0x2BF5, 0x2BF6, 0x2BF7, 0x2BF8, 0x2BF9,
                0x2BFA, 0x2BFB, 0x2BFC, 0x2BFD, 0x2BFE, 0x2BFF
             };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Miscellaneous Technical 0x2300-0x23FF
        /// </summary>
        [TestMethod]
        public void UnicodeMiscellaneousTechnical()
        {
            const long codePageStart = 0x2300;
            const long codePageEnd = 0x23FF;
            const string codePageTitle = "Miscellaneous Technical 0x2300-0x23FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.MiscellaneousTechnical, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Miscellaneous Technical http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-2300.pdf
            List<long> unicodeGaps = new List<long>()
             {
                0x23E9, 0x23EA, 0x23EB, 0x23EC, 0x23ED, 0x23EE, 0x23EF,
                0x23F0, 0x23F1, 0x23F2, 0x23F3, 0x23F4, 0x23F5, 0x23F6, 0x23F7, 0x23F8, 0x23F9, 0x23FA, 0x23FB, 0x23FC, 0x23FD, 0x23FE, 0x23FF
             };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Modifier Tone Letters 0xA700-0xA71F
        /// </summary>
        [TestMethod]
        public void UnicodeModifierToneLetters()
        {
            const long codePageStart = 0xA700;
            const long codePageEnd = 0xA71F;
            const string codePageTitle = "Modifier Tone Letters 0xA700-0xA71F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.ModifierToneLetters,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Modifier Tone Letters http://www.Unicode.org/charts/PDF/UA700.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Mongolian 0x1800-0x18AF
        /// </summary>
        [TestMethod]
        public void UnicodeMongolian()
        {
            const long codePageStart = 0x1800;
            const long codePageEnd = 0x18AF;
            const string codePageTitle = "Mongolian 0x1800-0x18AF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Mongolian,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Mongolian http://www.Unicode.org/charts/PDF/U1800.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x180F, 0x181A, 0x181B, 0x181C, 0x181D, 0x181E, 0x181F, 0x1878, 0x1879, 0x187A, 0x187B, 0x187C, 0x187D, 0x187E, 0x187F, 0x18AB, 0x18AC, 0x18AD, 0x18AE, 0x18AF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Myanmar 0x1000-0x109F
        /// </summary>
        [TestMethod]
        public void UnicodeMyanmar()
        {
            const long codePageStart = 0x1000;
            const long codePageEnd = 0x109F;
            const string codePageTitle = "Myanmar 0x1000-0x109F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Myanmar,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Myanmar http://www.Unicode.org/charts/PDF/U1000.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Myanmar Extended – A 0xAA60-0xAA7F
        /// </summary>
        [TestMethod]
        public void UnicodeMyanmarExtendedA()
        {
            const long codePageStart = 0xAA60;
            const long codePageEnd = 0xAA7F;
            const string codePageTitle = "Myanmar Extended – A 0xAA60-0xAA7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.MyanmarExtendedA);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Myanmar Extended – A http://www.Unicode.org/charts/PDF/UAA60.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xAA7C, 0xAA7D, 0xAA7E, 0xAA7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from New Tai Lue 0x1980-0x19DF
        /// </summary>
        [TestMethod]
        public void UnicodeNewTaiLue()
        {
            const long codePageStart = 0x1980;
            const long codePageEnd = 0x19DF;
            const string codePageTitle = "New Tai Lue 0x1980-0x19DF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.NewTaiLue,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 New Tai Lue http://www.Unicode.org/charts/PDF/U1980.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x19AC, 0x19AD, 0x19AE, 0x19AF, 0x19CA, 0x19CB, 0x19CC, 0x19CD, 0x19CE, 0x19CF, 0x19DB, 0x19DC, 0x19DD
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from NKo 0x07C0-0x07FF
        /// </summary>
        [TestMethod]
        public void UnicodeNKo()
        {
            const long codePageStart = 0x07C0;
            const long codePageEnd = 0x07FF;
            const string codePageTitle = "NKo 0x07C0-0x07FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Nko,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 NKo http://www.Unicode.org/charts/PDF/U07C0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x07FB, 0x07FC, 0x07FD, 0x07FE, 0x07FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Number Forms – A 0x2150-0x218F
        /// </summary>
        [TestMethod]
        public void UnicodeNumberForms()
        {
            const long codePageStart = 0x2150;
            const long codePageEnd = 0x218F;
            const string codePageTitle = "Number Forms – A 0x2150-0x218F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.NumberForms,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Number Forms http://www.Unicode.org/charts/PDF/U2150.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x218A, 0x218B, 0x218C, 0x218D, 0x218E, 0x218F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ogham 0x1680-0x169F
        /// </summary>
        [TestMethod]
        public void UnicodeOgham()
        {
            const long codePageStart = 0x1680;
            const long codePageEnd = 0x169F;
            const string codePageTitle = "Ogham 0x1680-0x169F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Ogham,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Ogham http://www.Unicode.org/charts/PDF/U1680.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x169D, 0x169E, 0x169F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Ol Chiki 0x1C50-0x1C7F
        /// </summary>
        [TestMethod]
        public void UnicodeOlChiki()
        {
            const long codePageStart = 0x1C50;
            const long codePageEnd = 0x1C7F;
            const string codePageTitle = "Ol Chiki 0x1C50-0x1C7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.OlChiki,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Ol Chiki http://www.Unicode.org/charts/PDF/U1C50.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Optical Character Recognition 0x2440-0x245F
        /// </summary>
        [TestMethod]
        public void UnicodeOpticalCharacterRecognition()
        {
            const long codePageStart = 0x2440;
            const long codePageEnd = 0x245F;
            const string codePageTitle = "Optical Character Recognition 0x2440-0x245F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.OpticalCharacterRecognition,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Optical Character Recognition  http://www.Unicode.org/charts/PDF/U2440.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x244B, 0x244C, 0x244D, 0x244E, 0x244F, 0x2450, 0x2451, 0x2452, 0x2453, 0x2454, 0x2455, 0x2456, 0x2457, 0x2458, 0x2459, 0x245A, 0x245B, 0x245C, 0x245D, 0x245E, 0x245F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Oriya 0x0B00-0x0B7F
        /// </summary>
        [TestMethod]
        public void UnicodeOriya()
        {
            const long codePageStart = 0x0B00;
            const long codePageEnd = 0x0B7F;
            const string codePageTitle = "Oriya 0x0B00-0x0B7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Oriya,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Oriya http://www.Unicode.org/charts/PDF/U0B00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0B00, 0x0B04, 0x0B0D, 0x0B0E, 0x0B11, 0x0B12, 0x0B29, 0x0B31, 0x0B34, 0x0B3A, 0x0B3B, 0x0B45, 0x0B46, 0x0B49, 0x0B4A, 0x0B4E, 0x0B4F,
                0x0B50, 0x0B51, 0x0B52, 0x0B53, 0x0B54, 0x0B55, 0x0B58, 0x0B59, 0x0B5A, 0x0B5B, 0x0B5E,
                0x0B64, 0x0B65, 0x0B72, 0x0B73, 0x0B74, 0x0B75, 0x0B76, 0x0B77, 0x0B78, 0x0B79, 0x0B7A, 0x0B7B, 0x0B7C, 0x0B7D, 0x0B7E, 0x0B7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Phags-pa 0xA840-0xA87F
        /// </summary>
        [TestMethod]
        public void UnicodePhagspa()
        {
            const long codePageStart = 0xA840;
            const long codePageEnd = 0xA87F;
            const string codePageTitle = "Phags-pa 0xA840-0xA87F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.Phagspa,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Phags-pa http://www.Unicode.org/charts/PDF/UA840.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA878, 0xA879, 0xA87A, 0xA87B, 0xA87C, 0xA87D, 0xA87E, 0xA87F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        ////Not supported by .Net 4.1, out of the boundry for testing
        /////// <summary>
        /////// All Unicode characters from Phaistos Disc 0x101D0-0x101FF
        /////// </summary>
        ////[TestMethod]
        ////public void UnicodePhaistosDisc()
        ////{
        ////    long codePageStart = 0x101D0;
        ////    long codePageEnd = 0x101FF;
        ////    string codePageTitle = "Phaistos Disc 0x101D0-0x101FF";

        ////    // compiled list of not assigned from the Unicode Standard, Version 5.2 Phaistos Disc http://www.Unicode.org/charts/PDF/U101D0.pdf
        ////    List<long> unicodeGaps = new List<long>()
        ////    {
        ////        // not supported by .Net 4.1
        ////    };

        ////    this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        ////}

        /// <summary>
        /// All Unicode characters from Phonetic Extensions 0x1D00-0x1D7F
        /// </summary>
        [TestMethod]
        public void UnicodePhoneticExtensions()
        {
            const long codePageStart = 0x1D00;
            const long codePageEnd = 0x1D7F;
            const string codePageTitle = "Phonetic Extensions 0x1D00-0x1D7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.PhoneticExtensions,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Phonetic Extensions http://www.Unicode.org/charts/PDF/U1D00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Phonetic Extensions Supplement 0x1D80-0x1DBF
        /// </summary>
        [TestMethod]
        public void UnicodePhoneticExtensionsSupplement()
        {
            const long codePageStart = 0x1D80;
            const long codePageEnd = 0x1DBF;
            const string codePageTitle = "Phonetic Extensions Supplement 0x1D80-0x1DBF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.PhoneticExtensionsSupplement,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Phonetic Extensions Supplement http://www.Unicode.org/charts/PDF/U1D80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Private Use Area 0xE000-0xF8FF
        /// </summary>
        [TestMethod]
        public void UnicodePrivateUseArea()
        {
            const long codePageStart = 0xE000;
            const long codePageEnd = 0xF8FF;
            const string codePageTitle = "Private Use Area 0xE000-0xF8FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Private Use Area http://www.Unicode.org/charts/PDF/UE000.pdf
            List<long> unicodeGaps = new List<long>();
            for (long i = codePageStart; i <= codePageEnd; i++)
            {
                unicodeGaps.Add(i);
            }

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Rejang 0xA930-0xA95F
        /// </summary>
        [TestMethod]
        public void UnicodeRejang()
        {
            const long codePageStart = 0xA930;
            const long codePageEnd = 0xA95F;
            const string codePageTitle = "Rejang 0xA930-0xA95F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.Rejang);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Rejang http://www.Unicode.org/charts/PDF/UA930.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA954, 0xA955, 0xA956, 0xA957, 0xA958, 0xA959, 0xA95A, 0xA95B, 0xA95C, 0xA95D, 0xA95E, 0xA95F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Runic 0x16A0-0x16FF
        /// </summary>
        [TestMethod]
        public void UnicodeRunic()
        {
            const long codePageStart = 0x16A0;
            const long codePageEnd = 0x16FF;
            const string codePageTitle = "Runic 0x16A0-0x16FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Runic,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the Unicode Standard, Version 5.2 Runic http://www.Unicode.org/charts/PDF/U16A0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x16F1, 0x16F2, 0x16F3, 0x16F4, 0x16F5, 0x16F6, 0x16F7, 0x16F8, 0x16F9, 0x16FA, 0x16FB, 0x16FC, 0x16FD, 0x16FE, 0x16FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Samaritan 0x0800-0x083F
        /// </summary>
        [TestMethod]
        public void UnicodeSamaritan()
        {
            const long codePageStart = 0x0800;
            const long codePageEnd = 0x083F;
            const string codePageTitle = "Samaritan 0x0800-0x083F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Samaritan,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Samaritan http://www.Unicode.org/charts/PDF/U0800.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x082E, 0x082F, 0x083F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Saurashtra 0xA880-0xA8DF
        /// </summary>
        [TestMethod]
        public void UnicodeSaurashtra()
        {
            const long codePageStart = 0xA880;
            const long codePageEnd = 0xA8DF;
            const string codePageTitle = "Saurashtra 0xA880-0xA8DF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.Saurashtra,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Saurashtra http://www.Unicode.org/charts/PDF/UA880.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA8C5, 0xA8C6, 0xA8C7, 0xA8C8, 0xA8C9, 0xA8CA, 0xA8CB, 0xA8CC, 0xA8CD, 0xA8DA, 0xA8DB, 0xA8DC, 0xA8DD, 0xA8DE, 0xA8DF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Sinhala 0x0D80-0x0DFF
        /// </summary>
        [TestMethod]
        public void UnicodeSinhala()
        {
            const long codePageStart = 0x0D80;
            const long codePageEnd = 0x0DFF;
            const string codePageTitle = "Sinhala 0x0D80-0x0DFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Sinhala,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Sinhala http://www.Unicode.org/charts/PDF/U0D80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0D80, 0x0D81, 0x0D84, 0x0D97, 0x0D98, 0x0D99, 0x0DB2, 0x0DBC, 0x0DBE, 0x0DBF, 0x0DC7, 0x0DC8, 0x0DC9, 0x0DCB, 0x0DCC, 0x0DCD, 0x0DCE,
                0x0DD5, 0x0DD7,
                0x0DE0, 0x0DE1, 0x0DE2, 0x0DE3, 0x0DE4, 0x0DE5, 0x0DE6, 0x0DE7, 0x0DE8, 0x0DE9, 0x0DEA, 0x0DEB, 0x0DEC, 0x0DED, 0x0DEE, 0x0DEF,
                0x0DF0, 0x0DF1, 0x0DF5, 0x0DF6, 0x0DF7, 0x0DF8, 0x0DF9, 0x0DFA, 0x0DFB, 0x0DFC, 0x0DFD, 0x0DFE, 0x0DFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Small Form Variants 0xFE50-0xFE6F
        /// </summary>
        [TestMethod]
        public void UnicodeSmallFormVariants()
        {
            const long codePageStart = 0xFE50;
            const long codePageEnd = 0xFE6F;
            const string codePageTitle = "Small Form Variants 0xFE50-0xFE6F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.SmallFormVariants);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Small Form Variants http://www.Unicode.org/charts/PDF/UFE50.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFE53, 0xFE67, 0xFE6C, 0xFE6D, 0xFE6E, 0xFE6F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Spacing Modifier Letters 0x02B0-0x02FF
        /// </summary>
        [TestMethod]
        public void UnicodeSpacingModifierLetters()
        {
            const long codePageStart = 0x02B0;
            const long codePageEnd = 0x02FF;
            const string codePageTitle = "Spacing Modifier Letters 0x02B0-0x02FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.SpacingModifierLetters,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Spacing Modifier Letters http://www.Unicode.org/charts/PDF/U02B0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Specials 0xFFF0-0xFFFF
        /// </summary>
        [TestMethod]
        public void UnicodeSpecials()
        {
            const long codePageStart = 0xFFF0;
            const long codePageEnd = 0xFFFD;
            const string codePageTitle = "Specials 0xFFF0-0xFFFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.Specials);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Specials http://www.Unicode.org/charts/PDF/UFFF0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFFF0, 0xFFF1, 0xFFF2, 0xFFF3, 0xFFF4, 0xFFF5, 0xFFF6, 0xFFF7, 0xFFF8
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Sundanese 0x1B80-0x1BBF
        /// </summary>
        [TestMethod]
        public void UnicodeSundanese()
        {
            const long codePageStart = 0x1B80;
            const long codePageEnd = 0x1BBF;
            const string codePageTitle = "Sundanese 0x1B80-0x1BBF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Sudanese,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Sundanese http://www.Unicode.org/charts/PDF/U1B80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1BAB, 0x1BAC, 0x1BAD, 0x1BBA, 0x1BBB, 0x1BBC, 0x1BBD, 0x1BBE, 0x1BBF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Superscripts and Subscripts 0x2070-0x209F
        /// </summary>
        [TestMethod]
        public void UnicodeSuperscriptsandSubscripts()
        {
            const long codePageStart = 0x2070;
            const long codePageEnd = 0x209F;
            const string codePageTitle = "Superscripts and Subscripts 0x2070-0x209F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.SuperscriptsAndSubscripts,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Superscripts and Subscripts http://www.Unicode.org/charts/PDF/U2070.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2072,
                0x2073,
                0x208F,
                0x2095, 0x2096, 0x2097, 0x2098, 0x2099, 0x209A, 0x209B, 0x209C, 0x209D, 0x209E, 0x209F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Supplemental Arrows-A 0x27F0-0x27FF
        /// </summary>
        [TestMethod]
        public void UnicodeSupplementalArrowsA()
        {
            const long codePageStart = 0x27F0;
            const long codePageEnd = 0x27FF;
            const string codePageTitle = "Supplemental Arrows-A 0x27F0-0x27FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.SupplementalArrowsA,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Supplemental Arrows-A http://www.Unicode.org/charts/PDF/U27F0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Supplemental Arrows-B 0x2900-0x297F
        /// </summary>
        [TestMethod]
        public void UnicodeSupplementalArrowsB()
        {
            const long codePageStart = 0x2900;
            const long codePageEnd = 0x297F;
            const string codePageTitle = "Supplemental Arrows-B 0x2900-0x297F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.SupplementalArrowsB,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Supplemental Arrows-B http://www.Unicode.org/charts/PDF/U2900.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Supplemental Mathematical Operators 0x2A00-0x2AFF
        /// </summary>
        [TestMethod]
        public void UnicodeSupplementalMathematicalOperators()
        {
            const long codePageStart = 0x2A00;
            const long codePageEnd = 0x2AFF;
            const string codePageTitle = "Supplemental Mathematical Operators 0x2A00-0x2AFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.SupplementalMathematicalOperators,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Supplemental Mathematical Operators http://www.Unicode.org/charts/PDF/U2A00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Supplemental Punctuation 0x2E00-0x2E7F
        /// </summary>
        [TestMethod]
        public void UnicodeSupplementalPunctuation()
        {
            const long codePageStart = 0x2E00;
            const long codePageEnd = 0x2E7F;
            const string codePageTitle = "Supplemental Punctuation 0x2E00-0x2E7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.SupplementalPunctuation,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Supplemental Punctuation http://www.Unicode.org/charts/PDF/U2E00.pdf
            List<long> unicodeGaps = new List<long>()
             {
                0x2E32, 0x2E33, 0x2E34, 0x2E35, 0x2E36, 0x2E37, 0x2E38, 0x2E39, 0x2E3A, 0x2E3B, 0x2E3C, 0x2E3D, 0x2E3E, 0x2E3F,
                0x2E40, 0x2E41, 0x2E42, 0x2E43, 0x2E44, 0x2E45, 0x2E46, 0x2E47, 0x2E48, 0x2E49, 0x2E4A, 0x2E4B, 0x2E4C, 0x2E4D, 0x2E4E, 0x2E4F,
                0x2E50, 0x2E51, 0x2E52, 0x2E53, 0x2E54, 0x2E55, 0x2E56, 0x2E57, 0x2E58, 0x2E59, 0x2E5A, 0x2E5B, 0x2E5C, 0x2E5D, 0x2E5E, 0x2E5F,
                0x2E60, 0x2E61, 0x2E62, 0x2E63, 0x2E64, 0x2E65, 0x2E66, 0x2E67, 0x2E68, 0x2E69, 0x2E6A, 0x2E6B, 0x2E6C, 0x2E6D, 0x2E6E, 0x2E6F,
                0x2E70, 0x2E71, 0x2E72, 0x2E73, 0x2E74, 0x2E75, 0x2E76, 0x2E77, 0x2E78, 0x2E79, 0x2E7A, 0x2E7B, 0x2E7C, 0x2E7D, 0x2E7E, 0x2E7F
             };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Syloti Nagri 0xA800-0xA82F
        /// </summary>
        [TestMethod]
        public void UnicodeSylotiNagri()
        {
            const long codePageStart = 0xA800;
            const long codePageEnd = 0xA82F;
            const string codePageTitle = "Syloti Nagri 0xA800-0xA82F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.SylotiNagri,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Syloti Nagri http://www.Unicode.org/charts/PDF/UA800.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA82C, 0xA82D, 0xA82E, 0xA82F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Syriac 0x0700-0x074F
        /// </summary>
        [TestMethod]
        public void UnicodeSyriac()
        {
            const long codePageStart = 0x0700;
            const long codePageEnd = 0x074F;
            const string codePageTitle = "Syriac 0x0700-0x074F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Syriac,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Syriac http://www.Unicode.org/charts/PDF/U0700.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x070E, 0x074B, 0x074C
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tagalog 0x1700-0x171F
        /// </summary>
        [TestMethod]
        public void UnicodeTagalog()
        {
            const long codePageStart = 0x1700;
            const long codePageEnd = 0x171F;
            const string codePageTitle = "Tagalog 0x1700-0x171F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Tagalog,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tagalog http://www.Unicode.org/charts/PDF/U1700.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x170D, 0x1715, 0x1716, 0x1717, 0x1718, 0x1719, 0x171A, 0x171B, 0x171C, 0x171D, 0x171E, 0x171F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tagbanwa 0x1760-0x177F
        /// </summary>
        [TestMethod]
        public void UnicodeTagbanwa()
        {
            const long codePageStart = 0x1760;
            const long codePageEnd = 0x177F;
            const string codePageTitle = "Tagbanwa 0x1760-0x177F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.Tagbanwa,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tagbanwa http://www.Unicode.org/charts/PDF/U1760.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x176D, 0x1771, 0x1774, 0x1775, 0x1776, 0x1777, 0x1778, 0x1779, 0x177A, 0x177B, 0x177C, 0x177D, 0x177E, 0x177F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tai Le 0x1950-0x197F
        /// </summary>
        [TestMethod]
        public void UnicodeTaiLe()
        {
            const long codePageStart = 0x1950;
            const long codePageEnd = 0x197F;
            const string codePageTitle = "Tai Le 0x1950-0x197F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.TaiLe,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tai Le http://www.Unicode.org/charts/PDF/U1950.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x196E, 0x196F, 0x1975, 0x1976, 0x1977, 0x1978, 0x1979, 0x197A, 0x197B, 0x197C, 0x197D, 0x197E, 0x197F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tai Tham 0x1A20-0x1AAF
        /// </summary>
        [TestMethod]
        public void UnicodeTaiTham()
        {
            const long codePageStart = 0x1A20;
            const long codePageEnd = 0x1AAF;
            const string codePageTitle = "Tai Tham 0x1A20-0x1AAF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.TaiTham,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tai Tham http://www.Unicode.org/charts/PDF/U1A20.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1A5F, 0x1A7D, 0x1A7E, 0x1A8A, 0x1A8B, 0x1A8C, 0x1A8D, 0x1A8E, 0x1A8F, 0x1A9A, 0x1A9B, 0x1A9C, 0x1A9D, 0x1A9E, 0x1A9F, 0x1AAE, 0x1AAF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tai Viet 0xAA80-0xAADF
        /// </summary>
        [TestMethod]
        public void UnicodeTaiViet()
        {
            const long codePageStart = 0xAA80;
            const long codePageEnd = 0xAADF;
            const string codePageTitle = "Tai Viet 0xAA80-0xAADF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.TaiViet);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tai Viet http://www.Unicode.org/charts/PDF/UAA80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xAAC3, 0xAAC4, 0xAAC5, 0xAAC6, 0xAAC7, 0xAAC8, 0xAAC9, 0xAACA, 0xAACB, 0xAACC, 0xAACD, 0xAACE, 0xAACF, 0xAAD0, 0xAAD1, 0xAAD2, 0xAAD3, 0xAAD4, 0xAAD5, 0xAAD6, 0xAAD7, 0xAAD8, 0xAAD9, 0xAADA
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tamil 0x0B80-0x0BFF
        /// </summary>
        [TestMethod]
        public void UnicodeTamil()
        {
            const long codePageStart = 0x0B80;
            const long codePageEnd = 0x0BFF;
            const string codePageTitle = "Tamil 0x0B80-0x0BFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Tamil,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tamil http://www.Unicode.org/charts/PDF/U0B80.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0B98,
                0x0BFB, 0x0BFC, 0x0BFD, 0x0BFE,
                0x0B80, 0x0B81, 0x0B84, 0x0B8B, 0x0B8C, 0x0B8D, 0x0B91, 0x0B96, 0x0B97, 0x0B9B, 0x0B9D,
                0x0BA0, 0x0BA1, 0x0BA2, 0x0BA5, 0x0BA6, 0x0BA7, 0x0BAB, 0x0BAC, 0x0BAD,
                0x0BBA, 0x0BBB, 0x0BBC, 0x0BBD,
                0x0BC3, 0x0BC4, 0x0BC5, 0x0BC9, 0x0BCE, 0x0BCF,
                0x0BD1, 0x0BD2, 0x0BD3, 0x0BD4, 0x0BD5, 0x0BD6, 0x0BD8, 0x0BD9, 0x0BDA, 0x0BDB, 0x0BDC, 0x0BDD, 0x0BDE, 0x0BDF,
                0x0BE0, 0x0BE1, 0x0BE2, 0x0BE3, 0x0BE4, 0x0BE5,  0x0BFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Telugu 0x0C00-0x0C7F
        /// </summary>
        [TestMethod]
        public void UnicodeTelugu()
        {
            const long codePageStart = 0x0C00;
            const long codePageEnd = 0x0C7F;
            const string codePageTitle = "Telugu 0x0C00-0x0C7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Telugu,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Telugu http://www.Unicode.org/charts/PDF/U0C00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0C00, 0x0C04, 0x0C0D, 0x0C11, 0x0C29, 0x0C34, 0x0C3A, 0x0C3B, 0x0C3C, 0x0C45, 0x0C49, 0x0C4E, 0x0C4F,
                0x0C50, 0x0C51, 0x0C52, 0x0C53, 0x0C54, 0x0C57, 0x0C5A, 0x0C5B, 0x0C5C, 0x0C5D, 0x0C5E, 0x0C5F,
                0x0C64, 0x0C65, 0x0C70, 0x0C71, 0x0C72, 0x0C73, 0x0C74, 0x0C75, 0x0C76, 0x0C77
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Thaana 0x0780-0x07BF
        /// </summary>
        [TestMethod]
        public void UnicodeThaana()
        {
            const long codePageStart = 0x0780;
            const long codePageEnd = 0x07BF;
            const string codePageTitle = "Thaana 0x0780-0x07BF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Thaana,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Thaana  http://www.Unicode.org/charts/PDF/U0780.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x07B2, 0x07B3, 0x07B4, 0x07B5, 0x07B6, 0x07B7, 0x07B8, 0x07B9, 0x07BA, 0x07BB, 0x07BC, 0x07BD, 0x07BE, 0x07BF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Thai 0x0E00-0x0E7F
        /// </summary>
        [TestMethod]
        public void UnicodeThai()
        {
            const long codePageStart = 0x0E00;
            const long codePageEnd = 0x0E7F;
            const string codePageTitle = "Thai 0x0E00-0x0E7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Thai,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Thai http://www.Unicode.org/charts/PDF/U0E00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0E00, 0x0E3B, 0x0E3C, 0x0E3D, 0x0E3E, 0x0E5C, 0x0E5D, 0x0E5E, 0x0E5F,
                0x0E60, 0x0E61, 0x0E62, 0x0E63, 0x0E64, 0x0E65, 0x0E66, 0x0E67, 0x0E68, 0x0E69, 0x0E6A, 0x0E6B, 0x0E6C, 0x0E6D, 0x0E6E, 0x0E6F,
                0x0E70, 0x0E71, 0x0E72, 0x0E73, 0x0E74, 0x0E75, 0x0E76, 0x0E77, 0x0E78, 0x0E79, 0x0E7A, 0x0E7B, 0x0E7C, 0x0E7D, 0x0E7E, 0x0E7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tibetan 0x0F00-0x0FFF
        /// </summary>
        [TestMethod]
        public void UnicodeTibetan()
        {
            const long codePageStart = 0x0F00;
            const long codePageEnd = 0x0FFF;
            const string codePageTitle = "Tibetan 0x0F00-0x0FFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Tibetan,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tibetan http://www.Unicode.org/charts/PDF/U0F00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x0F48, 0x0F6D, 0x0F6E, 0x0F6F, 0x0F70, 0x0F8C, 0x0F8D, 0x0F8E, 0x0F8F, 0x0F98, 0x0FBD, 0x0FCD, 0x0FD9, 0x0FDA, 0x0FDB, 0x0FDC, 0x0FDD, 0x0FDE, 0x0FDF,
                0x0FE0, 0x0FE1, 0x0FE2, 0x0FE3, 0x0FE4, 0x0FE5, 0x0FE6, 0x0FE7, 0x0FE8, 0x0FE9, 0x0FEA, 0x0FEB, 0x0FEC, 0x0FED, 0x0FEE, 0x0FEF,
                0x0ff0, 0x0ff1, 0x0ff2, 0x0ff3, 0x0ff4, 0x0ff5, 0x0ff6, 0x0ff7, 0x0ff8, 0x0ff9, 0x0ffA, 0x0ffB, 0x0ffC, 0x0ffD, 0x0ffE, 0x0ffF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Tifinagh 0x2D30-0x2D7F
        /// </summary>
        [TestMethod]
        public void UnicodeTifinagh()
        {
            const long codePageStart = 0x2D30;
            const long codePageEnd = 0x2D7F;
            const string codePageTitle = "Tifinagh 0x2D30-0x2D7F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.Tifinagh,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Tifinagh http://www.Unicode.org/charts/PDF/U2D30.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x2D66, 0x2D67, 0x2D68, 0x2D69, 0x2D6A, 0x2D6B, 0x2D6C, 0x2D6D, 0x2D6E, 0x2D70, 0x2D71, 0x2D72, 0x2D73, 0x2D74, 0x2D75, 0x2D76, 0x2D77, 0x2D78, 0x2D79, 0x2D7A, 0x2D7B, 0x2D7C, 0x2D7D, 0x2D7E, 0x2D7F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Unified Canadian Aboriginal Syllabics 0x1400-0x167F
        /// </summary>
        [TestMethod]
        public void UnicodeUnifiedCanadianAboriginalSyllabics()
        {
            const long codePageStart = 0x1400;
            const long codePageEnd = 0x167F;
            const string codePageTitle = "Unified Canadian Aboriginal Syllabics 0x1400-0x167F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.UnifiedCanadianAboriginalSyllabics,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Unified Canadian Aboriginal Syllabics http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-1400.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Unified Canadian Aboriginal Syllabics Extended 0x18B0-0x18FF
        /// </summary>
        [TestMethod]
        public void UnicodeUnifiedCanadianAboriginalSyllabicsExtended()
        {
            const long codePageStart = 0x18B0;
            const long codePageEnd = 0x18FF;
            const string codePageTitle = "Unified Canadian Aboriginal Syllabics Extended 0x18B0-0x18FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.UnifiedCanadianAboriginalSyllabicsExtended,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Unified Canadian Aboriginal Syllabics Extended http://www.Unicode.org/charts/PDF/Unicode-5.2/U52-18B0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x18F6, 0x18F7, 0x18F8, 0x18F9, 0x18FA, 0x18FB, 0x18FC, 0x18FD, 0x18FE, 0x18FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Vai 0xA500-0xA63F
        /// </summary>
        [TestMethod]
        public void UnicodeVai()
        {
            const long codePageStart = 0xA500;
            const long codePageEnd = 0xA63F;
            const string codePageTitle = "Vai 0xA500-0xA63F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.Vai,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Vai http://www.Unicode.org/charts/PDF/UA500.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA62C, 0xA62D, 0xA62E, 0xA62F, 0xA630, 0xA631, 0xA632, 0xA633, 0xA634, 0xA635, 0xA636, 0xA637, 0xA638, 0xA639, 0xA63A, 0xA63B, 0xA63C, 0xA63D, 0xA63E, 0xA63F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Variation Selectors 0xFE00-0xFE0F
        /// </summary>
        [TestMethod]
        public void UnicodeVariationSelectors()
        {
            const long codePageStart = 0xFE00;
            const long codePageEnd = 0xFE0F;
            const string codePageTitle = "Variation Selectors 0xFE00-0xFE0F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.VariationSelectors);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Variation Selectors http://www.Unicode.org/charts/PDF/UFE00.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Vedic Extensions 0x1CD0-0x1CFF
        /// </summary>
        [TestMethod]
        public void UnicodeVedicExtensions()
        {
            const long codePageStart = 0x1CD0;
            const long codePageEnd = 0x1CFF;
            const string codePageTitle = "Vedic Extensions 0x1CD0-0x1CFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.VedicExtensions,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Vedic Extensions http://www.Unicode.org/charts/PDF/U1CD0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0x1CF3, 0x1CF4, 0x1CF5, 0x1CF6, 0x1CF7, 0x1CF8, 0x1CF9, 0x1CFA, 0x1CFB, 0x1CFC, 0x1CFD, 0x1CFE, 0x1CFF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Vertical Forms 0xFE10-0xFE1F
        /// </summary>
        [TestMethod]
        public void UnicodeVerticalForms()
        {
            const long codePageStart = 0xFE10;
            const long codePageEnd = 0xFE1F;
            const string codePageTitle = "Vertical Forms 0xFE10-0xFE1F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.VerticalForms);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Vertical Forms http://www.Unicode.org/charts/PDF/UFE10.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xFE1A, 0xFE1B, 0xFE1C, 0xFE1D, 0xFE1E, 0xFE1F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Yi Radicals 0xA490-0xA4CF
        /// </summary>
        [TestMethod]
        public void UnicodeYiRadicals()
        {
            const long codePageStart = 0xA490;
            const long codePageEnd = 0xA4CF;
            const string codePageTitle = "Yi Radicals 0xA490-0xA4CF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.YiRadicals, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Yi Radicals http://www.Unicode.org/charts/PDF/UA490.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA4C7, 0xA4C8, 0xA4C9, 0xA4CA, 0xA4CB, 0xA4CC, 0xA4CD, 0xA4CE, 0xA4CF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Yi Syllables 0xA000-0xA48F
        /// </summary>
        [TestMethod]
        public void UnicodeYiSyllables()
        {
            const long codePageStart = 0xA000;
            const long codePageEnd = 0xA48F;
            const string codePageTitle = "Yi Syllables 0xA000-0xA48F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.YiSyllables, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Yi Syllables http://www.Unicode.org/charts/PDF/UA000.pdf
            List<long> unicodeGaps = new List<long>()
            {
                0xA48D, 0xA48E, 0xA48F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode characters from Yijing Hexagram Symbols 0x4DC0-0x4DFF
        /// </summary>
        [TestMethod]
        public void UnicodeYijingHexagramSymbols()
        {
            const long codePageStart = 0x4DC0;
            const long codePageEnd = 0x4DFF;
            const string codePageTitle = "Yijing Hexagram Symbols 0x4DC0-0x4DFF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.YijingHexagramSymbols,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Yijing Hexagram Symbols http://www.Unicode.org/charts/PDF/U4DC0.pdf
            List<long> unicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
        }

        /// <summary>
        /// All Unicode named entities.
        /// </summary>
        [TestMethod]
        public void UnicodeNamedEntities()
        {
            string expected;
            string actual;
            const string codePageTitle = " Named Entities";

            // compiled list of characters that translate to named entities http://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references
            using DataSet ds = NamedEntitiesSet();

            foreach (DataRow dr in ds.Tables["NamedEntity"].Rows)
            {
                int value = Convert.ToInt32(dr["Code"].ToString(), 16);
                actual = Encoder.HtmlEncode(Convert.ToString((char)value), true);
                expected = "&" + dr["Name"].ToString() + ";";
                Assert.AreEqual(expected, actual, "HtmlEncoder.HtmlEncode - use namedentities " + dr["Code"].ToString() + codePageTitle);
            }
        }

        /// <summary>
        /// Tests that MarkAsSafe with none works as expected.
        /// </summary>
        [TestMethod]
        public void WhenNothingIsMarkedAsSafeEverythingShouldBeEncoded()
        {
            UnicodeCharacterEncoder.MarkAsSafe(
                LowerCodeCharts.None,
                LowerMidCodeCharts.None,
                MidCodeCharts.None,
                UpperMidCodeCharts.None,
                UpperCodeCharts.None);

            for (int i = 0; i < 0xFFFF; i++)
            {
                char character = (char)i;

                if (char.IsSurrogate(character))
                {
                    continue;
                }

                string actual = Encoder.HtmlEncode(new string(character, 1));

                var expected = i switch
                {
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '&' => "&amp;",
                    '"' => "&quot;",
                    _ => string.Format(CultureInfo.InvariantCulture, "&#{0};", i),
                };
                Assert.AreEqual(expected, actual, string.Format(CultureInfo.InvariantCulture, "All unsafe failed on 0x{0:X}", i));
            }
        }

        /// <summary>
        /// Method that is called by all the codepage unit tests and executes the each encoding test
        /// for that codepage.
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="unicodeGaps">List of the codepage Unicode gaps</param>
        public void CallUnitTests(long codePageStart, long codePageEnd, string codePageTitle, List<long> unicodeGaps)
        {
            UrlEncodeUnicodeTest(codePageStart, codePageEnd, codePageTitle);
            HtmlFormUrlEncodeUnicodeTest(codePageStart, codePageEnd, codePageTitle);
            HtmlEncoderUnicodeTest(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
            XmlEncodeUnicodeTest(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
            XmlAttributeEncodeUnicodeTest(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
            HtmlAttributeEncodeUnicodeTest(codePageStart, codePageEnd, codePageTitle, unicodeGaps);
            LdapEncodeDistinguishedNameUnicodeTest(codePageStart, codePageEnd, codePageTitle);
            LdapEncodeFilterUnicodeTest(codePageStart, codePageEnd, codePageTitle);
            TestCssEncoderForSpecifiedCodePage(codePageStart, codePageEnd, codePageTitle);
        }

        /// <summary>
        /// Named Entities
        /// </summary>
        /// <returns>Dataset for named entities</returns>
        private static DataSet NamedEntitiesSet()
        {
            DataSet ds = new DataSet();

            const string XmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <Root> <NamedEntity><Name>quot</Name><Code>0x0022</Code></NamedEntity><NamedEntity><Name>amp</Name><Code>0x0026</Code></NamedEntity><NamedEntity><Name>lt</Name><Code>0x003C</Code></NamedEntity><NamedEntity><Name>gt</Name><Code>0x003E</Code></NamedEntity><NamedEntity><Name>nbsp</Name><Code>0x00A0</Code></NamedEntity><NamedEntity><Name>iexcl</Name><Code>0x00A1</Code></NamedEntity><NamedEntity><Name>cent</Name><Code>0x00A2</Code></NamedEntity><NamedEntity><Name>pound</Name><Code>0x00A3</Code></NamedEntity><NamedEntity><Name>curren</Name><Code>0x00A4</Code></NamedEntity><NamedEntity><Name>yen</Name><Code>0x00A5</Code></NamedEntity><NamedEntity><Name>brvbar</Name><Code>0x00A6</Code></NamedEntity><NamedEntity><Name>sect</Name><Code>0x00A7</Code></NamedEntity><NamedEntity><Name>uml</Name><Code>0x00A8</Code></NamedEntity><NamedEntity><Name>copy</Name><Code>0x00A9</Code></NamedEntity><NamedEntity><Name>ordf</Name><Code>0x00AA</Code></NamedEntity><NamedEntity><Name>laquo</Name><Code>0x00AB</Code></NamedEntity><NamedEntity><Name>not</Name><Code>0x00AC</Code></NamedEntity><NamedEntity><Name>shy</Name><Code>0x00AD</Code></NamedEntity><NamedEntity><Name>reg</Name><Code>0x00AE</Code></NamedEntity><NamedEntity><Name>macr</Name><Code>0x00AF</Code></NamedEntity><NamedEntity><Name>deg</Name><Code>0x00B0</Code></NamedEntity><NamedEntity><Name>plusmn</Name><Code>0x00B1</Code></NamedEntity><NamedEntity><Name>sup2</Name><Code>0x00B2</Code></NamedEntity><NamedEntity><Name>sup3</Name><Code>0x00B3</Code></NamedEntity><NamedEntity><Name>acute</Name><Code>0x00B4</Code></NamedEntity><NamedEntity><Name>micro</Name><Code>0x00B5</Code></NamedEntity><NamedEntity><Name>para</Name><Code>0x00B6</Code></NamedEntity><NamedEntity><Name>middot</Name><Code>0x00B7</Code></NamedEntity><NamedEntity><Name>cedil</Name><Code>0x00B8</Code></NamedEntity><NamedEntity><Name>sup1</Name><Code>0x00B9</Code></NamedEntity><NamedEntity><Name>ordm</Name><Code>0x00BA</Code></NamedEntity><NamedEntity><Name>raquo</Name><Code>0x00BB</Code></NamedEntity><NamedEntity><Name>frac14</Name><Code>0x00BC</Code></NamedEntity><NamedEntity><Name>frac12</Name><Code>0x00BD</Code></NamedEntity><NamedEntity><Name>frac34</Name><Code>0x00BE</Code></NamedEntity><NamedEntity><Name>iquest</Name><Code>0x00BF</Code></NamedEntity><NamedEntity><Name>Agrave</Name><Code>0x00C0</Code></NamedEntity><NamedEntity><Name>Aacute</Name><Code>0x00C1</Code></NamedEntity><NamedEntity><Name>Acirc</Name><Code>0x00C2</Code></NamedEntity><NamedEntity><Name>Atilde</Name><Code>0x00C3</Code></NamedEntity><NamedEntity><Name>Auml</Name><Code>0x00C4</Code></NamedEntity><NamedEntity><Name>Aring</Name><Code>0x00C5</Code></NamedEntity><NamedEntity><Name>AElig</Name><Code>0x00C6</Code></NamedEntity><NamedEntity><Name>Ccedil</Name><Code>0x00C7</Code></NamedEntity><NamedEntity><Name>Egrave</Name><Code>0x00C8</Code></NamedEntity><NamedEntity><Name>Eacute</Name><Code>0x00C9</Code></NamedEntity><NamedEntity><Name>Ecirc</Name><Code>0x00CA</Code></NamedEntity><NamedEntity><Name>Euml</Name><Code>0x00CB</Code></NamedEntity><NamedEntity><Name>Igrave</Name><Code>0x00CC</Code></NamedEntity><NamedEntity><Name>Iacute</Name><Code>0x00CD</Code></NamedEntity><NamedEntity><Name>Icirc</Name><Code>0x00CE</Code></NamedEntity><NamedEntity><Name>Iuml</Name><Code>0x00CF</Code></NamedEntity><NamedEntity><Name>ETH</Name><Code>0x00D0</Code></NamedEntity><NamedEntity><Name>Ntilde</Name><Code>0x00D1</Code></NamedEntity><NamedEntity><Name>Ograve</Name><Code>0x00D2</Code></NamedEntity><NamedEntity><Name>Oacute</Name><Code>0x00D3</Code></NamedEntity><NamedEntity><Name>Ocirc</Name><Code>0x00D4</Code></NamedEntity><NamedEntity><Name>Otilde</Name><Code>0x00D5</Code></NamedEntity><NamedEntity><Name>Ouml</Name><Code>0x00D6</Code></NamedEntity><NamedEntity><Name>times</Name><Code>0x00D7</Code></NamedEntity><NamedEntity><Name>Oslash</Name><Code>0x00D8</Code></NamedEntity><NamedEntity><Name>Ugrave</Name><Code>0x00D9</Code></NamedEntity><NamedEntity><Name>Uacute</Name><Code>0x00DA</Code></NamedEntity><NamedEntity><Name>Ucirc</Name><Code>0x00DB</Code></NamedEntity><NamedEntity><Name>Uuml</Name><Code>0x00DC</Code></NamedEntity><NamedEntity><Name>Yacute</Name><Code>0x00DD</Code></NamedEntity><NamedEntity><Name>THORN</Name><Code>0x00DE</Code></NamedEntity><NamedEntity><Name>szlig</Name><Code>0x00DF</Code></NamedEntity><NamedEntity><Name>agrave</Name><Code>0x00E0</Code></NamedEntity><NamedEntity><Name>aacute</Name><Code>0x00E1</Code></NamedEntity><NamedEntity><Name>acirc</Name><Code>0x00E2</Code></NamedEntity><NamedEntity><Name>atilde</Name><Code>0x00E3</Code></NamedEntity><NamedEntity><Name>auml</Name><Code>0x00E4</Code></NamedEntity><NamedEntity><Name>aring</Name><Code>0x00E5</Code></NamedEntity><NamedEntity><Name>aelig</Name><Code>0x00E6</Code></NamedEntity><NamedEntity><Name>ccedil</Name><Code>0x00E7</Code></NamedEntity><NamedEntity><Name>egrave</Name><Code>0x00E8</Code></NamedEntity><NamedEntity><Name>eacute</Name><Code>0x00E9</Code></NamedEntity><NamedEntity><Name>ecirc</Name><Code>0x00EA</Code></NamedEntity><NamedEntity><Name>euml</Name><Code>0x00EB</Code></NamedEntity><NamedEntity><Name>igrave</Name><Code>0x00EC</Code></NamedEntity><NamedEntity><Name>iacute</Name><Code>0x00ED</Code></NamedEntity><NamedEntity><Name>icirc</Name><Code>0x00EE</Code></NamedEntity><NamedEntity><Name>iuml</Name><Code>0x00EF</Code></NamedEntity><NamedEntity><Name>eth</Name><Code>0x00F0</Code></NamedEntity><NamedEntity><Name>ntilde</Name><Code>0x00F1</Code></NamedEntity><NamedEntity><Name>ograve</Name><Code>0x00F2</Code></NamedEntity><NamedEntity><Name>oacute</Name><Code>0x00F3</Code></NamedEntity><NamedEntity><Name>ocirc</Name><Code>0x00F4</Code></NamedEntity><NamedEntity><Name>otilde</Name><Code>0x00F5</Code></NamedEntity><NamedEntity><Name>ouml</Name><Code>0x00F6</Code></NamedEntity><NamedEntity><Name>divide</Name><Code>0x00F7</Code></NamedEntity><NamedEntity><Name>oslash</Name><Code>0x00F8</Code></NamedEntity><NamedEntity><Name>ugrave</Name><Code>0x00F9</Code></NamedEntity><NamedEntity><Name>uacute</Name><Code>0x00FA</Code></NamedEntity><NamedEntity><Name>ucirc</Name><Code>0x00FB</Code></NamedEntity><NamedEntity><Name>uuml</Name><Code>0x00FC</Code></NamedEntity><NamedEntity><Name>yacute</Name><Code>0x00FD</Code></NamedEntity><NamedEntity><Name>thorn</Name><Code>0x00FE</Code></NamedEntity><NamedEntity><Name>yuml</Name><Code>0x00FF</Code></NamedEntity><NamedEntity><Name>OElig</Name><Code>0x0152</Code></NamedEntity><NamedEntity><Name>oelig</Name><Code>0x0153</Code></NamedEntity><NamedEntity><Name>Scaron</Name><Code>0x0160</Code></NamedEntity><NamedEntity><Name>scaron</Name><Code>0x0161</Code></NamedEntity><NamedEntity><Name>Yuml</Name><Code>0x0178</Code></NamedEntity><NamedEntity><Name>fnof</Name><Code>0x0192</Code></NamedEntity><NamedEntity><Name>circ</Name><Code>0x02C6</Code></NamedEntity><NamedEntity><Name>tilde</Name><Code>0x02DC</Code></NamedEntity><NamedEntity><Name>Alpha</Name><Code>0x0391</Code></NamedEntity><NamedEntity><Name>Beta</Name><Code>0x0392</Code></NamedEntity><NamedEntity><Name>Gamma</Name><Code>0x0393</Code></NamedEntity><NamedEntity><Name>Delta</Name><Code>0x0394</Code></NamedEntity><NamedEntity><Name>Epsilon</Name><Code>0x0395</Code></NamedEntity><NamedEntity><Name>Zeta</Name><Code>0x0396</Code></NamedEntity><NamedEntity><Name>Eta</Name><Code>0x0397</Code></NamedEntity><NamedEntity><Name>Theta</Name><Code>0x0398</Code></NamedEntity><NamedEntity><Name>Iota</Name><Code>0x0399</Code></NamedEntity><NamedEntity><Name>Kappa</Name><Code>0x039A</Code></NamedEntity><NamedEntity><Name>Lambda</Name><Code>0x039B</Code></NamedEntity><NamedEntity><Name>Mu</Name><Code>0x039C</Code></NamedEntity><NamedEntity><Name>Nu</Name><Code>0x039D</Code></NamedEntity><NamedEntity><Name>Xi</Name><Code>0x039E</Code></NamedEntity><NamedEntity><Name>Omicron</Name><Code>0x039F</Code></NamedEntity><NamedEntity><Name>Pi</Name><Code>0x03A0</Code></NamedEntity><NamedEntity><Name>Rho</Name><Code>0x03A1</Code></NamedEntity><NamedEntity><Name>Sigma</Name><Code>0x03A3</Code></NamedEntity><NamedEntity><Name>Tau</Name><Code>0x03A4</Code></NamedEntity><NamedEntity><Name>Upsilon</Name><Code>0x03A5</Code></NamedEntity><NamedEntity><Name>Phi</Name><Code>0x03A6</Code></NamedEntity><NamedEntity><Name>Chi</Name><Code>0x03A7</Code></NamedEntity><NamedEntity><Name>Psi</Name><Code>0x03A8</Code></NamedEntity><NamedEntity><Name>Omega</Name><Code>0x03A9</Code></NamedEntity><NamedEntity><Name>alpha</Name><Code>0x03B1</Code></NamedEntity><NamedEntity><Name>beta</Name><Code>0x03B2</Code></NamedEntity><NamedEntity><Name>gamma</Name><Code>0x03B3</Code></NamedEntity><NamedEntity><Name>delta</Name><Code>0x03B4</Code></NamedEntity><NamedEntity><Name>epsilon</Name><Code>0x03B5</Code></NamedEntity><NamedEntity><Name>zeta</Name><Code>0x03B6</Code></NamedEntity><NamedEntity><Name>eta</Name><Code>0x03B7</Code></NamedEntity><NamedEntity><Name>theta</Name><Code>0x03B8</Code></NamedEntity><NamedEntity><Name>iota</Name><Code>0x03B9</Code></NamedEntity><NamedEntity><Name>kappa</Name><Code>0x03BA</Code></NamedEntity><NamedEntity><Name>lambda</Name><Code>0x03BB</Code></NamedEntity><NamedEntity><Name>mu</Name><Code>0x03BC</Code></NamedEntity><NamedEntity><Name>nu</Name><Code>0x03BD</Code></NamedEntity><NamedEntity><Name>xi</Name><Code>0x03BE</Code></NamedEntity><NamedEntity><Name>omicron</Name><Code>0x03BF</Code></NamedEntity><NamedEntity><Name>pi</Name><Code>0x03C0</Code></NamedEntity><NamedEntity><Name>rho</Name><Code>0x03C1</Code></NamedEntity><NamedEntity><Name>sigmaf</Name><Code>0x03C2</Code></NamedEntity><NamedEntity><Name>sigma</Name><Code>0x03C3</Code></NamedEntity><NamedEntity><Name>tau</Name><Code>0x03C4</Code></NamedEntity><NamedEntity><Name>upsilon</Name><Code>0x03C5</Code></NamedEntity><NamedEntity><Name>phi</Name><Code>0x03C6</Code></NamedEntity><NamedEntity><Name>chi</Name><Code>0x03C7</Code></NamedEntity><NamedEntity><Name>psi</Name><Code>0x03C8</Code></NamedEntity><NamedEntity><Name>omega</Name><Code>0x03C9</Code></NamedEntity><NamedEntity><Name>thetasym</Name><Code>0x03D1</Code></NamedEntity><NamedEntity><Name>upsih</Name><Code>0x03D2</Code></NamedEntity><NamedEntity><Name>piv</Name><Code>0x03D6</Code></NamedEntity><NamedEntity><Name>ensp</Name><Code>0x2002</Code></NamedEntity><NamedEntity><Name>emsp</Name><Code>0x2003</Code></NamedEntity><NamedEntity><Name>thinsp</Name><Code>0x2009</Code></NamedEntity><NamedEntity><Name>zwnj</Name><Code>0x200C</Code></NamedEntity><NamedEntity><Name>zwj</Name><Code>0x200D</Code></NamedEntity><NamedEntity><Name>lrm</Name><Code>0x200E</Code></NamedEntity><NamedEntity><Name>rlm</Name><Code>0x200F</Code></NamedEntity><NamedEntity><Name>ndash</Name><Code>0x2013</Code></NamedEntity><NamedEntity><Name>mdash</Name><Code>0x2014</Code></NamedEntity><NamedEntity><Name>lsquo</Name><Code>0x2018</Code></NamedEntity><NamedEntity><Name>rsquo</Name><Code>0x2019</Code></NamedEntity><NamedEntity><Name>sbquo</Name><Code>0x201A</Code></NamedEntity><NamedEntity><Name>ldquo</Name><Code>0x201C</Code></NamedEntity><NamedEntity><Name>rdquo</Name><Code>0x201D</Code></NamedEntity><NamedEntity><Name>bdquo</Name><Code>0x201E</Code></NamedEntity><NamedEntity><Name>dagger</Name><Code>0x2020</Code></NamedEntity><NamedEntity><Name>Dagger</Name><Code>0x2021</Code></NamedEntity><NamedEntity><Name>bull</Name><Code>0x2022</Code></NamedEntity><NamedEntity><Name>hellip</Name><Code>0x2026</Code></NamedEntity><NamedEntity><Name>permil</Name><Code>0x2030</Code></NamedEntity><NamedEntity><Name>prime</Name><Code>0x2032</Code></NamedEntity><NamedEntity><Name>Prime</Name><Code>0x2033</Code></NamedEntity><NamedEntity><Name>lsaquo</Name><Code>0x2039</Code></NamedEntity><NamedEntity><Name>rsaquo</Name><Code>0x203A</Code></NamedEntity><NamedEntity><Name>oline</Name><Code>0x203E</Code></NamedEntity><NamedEntity><Name>frasl</Name><Code>0x2044</Code></NamedEntity><NamedEntity><Name>euro</Name><Code>0x20AC</Code></NamedEntity><NamedEntity><Name>image</Name><Code>0x2111</Code></NamedEntity><NamedEntity><Name>weierp</Name><Code>0x2118</Code></NamedEntity><NamedEntity><Name>real</Name><Code>0x211C</Code></NamedEntity><NamedEntity><Name>trade</Name><Code>0x2122</Code></NamedEntity><NamedEntity><Name>alefsym</Name><Code>0x2135</Code></NamedEntity><NamedEntity><Name>larr</Name><Code>0x2190</Code></NamedEntity><NamedEntity><Name>uarr</Name><Code>0x2191</Code></NamedEntity><NamedEntity><Name>rarr</Name><Code>0x2192</Code></NamedEntity><NamedEntity><Name>darr</Name><Code>0x2193</Code></NamedEntity><NamedEntity><Name>harr</Name><Code>0x2194</Code></NamedEntity><NamedEntity><Name>crarr</Name><Code>0x21B5</Code></NamedEntity><NamedEntity><Name>lArr</Name><Code>0x21D0</Code></NamedEntity><NamedEntity><Name>uArr</Name><Code>0x21D1</Code></NamedEntity><NamedEntity><Name>rArr</Name><Code>0x21D2</Code></NamedEntity><NamedEntity><Name>dArr</Name><Code>0x21D3</Code></NamedEntity><NamedEntity><Name>hArr</Name><Code>0x21D4</Code></NamedEntity><NamedEntity><Name>forall</Name><Code>0x2200</Code></NamedEntity><NamedEntity><Name>part</Name><Code>0x2202</Code></NamedEntity><NamedEntity><Name>exist</Name><Code>0x2203</Code></NamedEntity><NamedEntity><Name>empty</Name><Code>0x2205</Code></NamedEntity><NamedEntity><Name>nabla</Name><Code>0x2207</Code></NamedEntity><NamedEntity><Name>isin</Name><Code>0x2208</Code></NamedEntity><NamedEntity><Name>notin</Name><Code>0x2209</Code></NamedEntity><NamedEntity><Name>ni</Name><Code>0x220B</Code></NamedEntity><NamedEntity><Name>prod</Name><Code>0x220F</Code></NamedEntity><NamedEntity><Name>sum</Name><Code>0x2211</Code></NamedEntity><NamedEntity><Name>minus</Name><Code>0x2212</Code></NamedEntity><NamedEntity><Name>lowast</Name><Code>0x2217</Code></NamedEntity><NamedEntity><Name>radic</Name><Code>0x221A</Code></NamedEntity><NamedEntity><Name>prop</Name><Code>0x221D</Code></NamedEntity><NamedEntity><Name>infin</Name><Code>0x221E</Code></NamedEntity><NamedEntity><Name>ang</Name><Code>0x2220</Code></NamedEntity><NamedEntity><Name>and</Name><Code>0x2227</Code></NamedEntity><NamedEntity><Name>or</Name><Code>0x2228</Code></NamedEntity><NamedEntity><Name>cap</Name><Code>0x2229</Code></NamedEntity><NamedEntity><Name>cup</Name><Code>0x222A</Code></NamedEntity><NamedEntity><Name>int</Name><Code>0x222B</Code></NamedEntity><NamedEntity><Name>there4</Name><Code>0x2234</Code></NamedEntity><NamedEntity><Name>sim</Name><Code>0x223C</Code></NamedEntity><NamedEntity><Name>cong</Name><Code>0x2245</Code></NamedEntity><NamedEntity><Name>asymp</Name><Code>0x2248</Code></NamedEntity><NamedEntity><Name>ne</Name><Code>0x2260</Code></NamedEntity><NamedEntity><Name>equiv</Name><Code>0x2261</Code></NamedEntity><NamedEntity><Name>le</Name><Code>0x2264</Code></NamedEntity><NamedEntity><Name>ge</Name><Code>0x2265</Code></NamedEntity><NamedEntity><Name>sub</Name><Code>0x2282</Code></NamedEntity><NamedEntity><Name>sup</Name><Code>0x2283</Code></NamedEntity><NamedEntity><Name>nsub</Name><Code>0x2284</Code></NamedEntity><NamedEntity><Name>sube</Name><Code>0x2286</Code></NamedEntity><NamedEntity><Name>supe</Name><Code>0x2287</Code></NamedEntity><NamedEntity><Name>oplus</Name><Code>0x2295</Code></NamedEntity><NamedEntity><Name>otimes</Name><Code>0x2297</Code></NamedEntity><NamedEntity><Name>perp</Name><Code>0x22A5</Code></NamedEntity><NamedEntity><Name>sdot</Name><Code>0x22C5</Code></NamedEntity><NamedEntity><Name>lceil</Name><Code>0x2308</Code></NamedEntity><NamedEntity><Name>rceil</Name><Code>0x2309</Code></NamedEntity><NamedEntity><Name>lfloor</Name><Code>0x230A</Code></NamedEntity><NamedEntity><Name>rfloor</Name><Code>0x230B</Code></NamedEntity><NamedEntity><Name>lang</Name><Code>0x2329</Code></NamedEntity><NamedEntity><Name>rang</Name><Code>0x232A</Code></NamedEntity><NamedEntity><Name>loz</Name><Code>0x25CA</Code></NamedEntity><NamedEntity><Name>spades</Name><Code>0x2660</Code></NamedEntity><NamedEntity><Name>clubs</Name><Code>0x2663</Code></NamedEntity><NamedEntity><Name>hearts</Name><Code>0x2665</Code></NamedEntity><NamedEntity><Name>diams</Name><Code>0x2666</Code></NamedEntity></Root>";

            using (StringReader stringReader = new StringReader(XmlData))
            {
                ds.ReadXml(stringReader, XmlReadMode.InferSchema);
            }

            return ds;
        }

        /// <summary>
        /// A test for HtmlAttributeEncode
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="unicodeGaps">List of the codepage Unicode gaps</param>
        private static void HtmlAttributeEncodeUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle, List<long> unicodeGaps)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string expected;
                string actual;
                string testmessage;

                if (!unicodeGaps.Contains(i))
                {
                    expected = Convert.ToString((char)i);
                    actual = Encoder.HtmlAttributeEncode(Convert.ToString((char)i));
                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (non-gap value) ";
                }
                else
                {
                    expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                    actual = Encoder.HtmlAttributeEncode(Convert.ToString((char)i));

                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                }

                if (i.Equals(0x0020))
                {
                    expected = "&#32;";
                }

                if (i.Equals(0x0022))
                {
                    expected = "&quot;";
                }

                if (i.Equals(0x0026))
                {
                    expected = "&amp;";
                }

                if (i.Equals(0x0027))
                {
                    expected = "&#39;";
                }

                if (i.Equals(0x003c))
                {
                    expected = "&lt;";
                }

                if (i.Equals(0x003e))
                {
                    expected = "&gt;";
                }

                Assert.AreEqual(expected, actual, "HtmlAttributeEncode.HtmlAttributeEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for HtmlFormUrlEncode
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        private static void HtmlFormUrlEncodeUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                System.Text.Encoding encoders = System.Text.Encoding.UTF8;
                string expected = string.Empty;

                byte[] bytes = encoders.GetBytes(Convert.ToString((char)i));
                for (int x = 0; x < bytes.Length; x++)
                {
                    if (bytes[x] < 16)
                    {
                        expected = expected + "%0" + bytes[x].ToString("X");
                        expected = expected.ToLower();
                    }
                    else
                    {
                        expected = expected + "%" + bytes[x].ToString("X");
                        expected = expected.ToLower();
                    }
                }

                if (i.Equals(0x0020))
                {
                    expected = "+";
                }

                if (i.Equals(0x002d))
                {
                    expected = "-";
                }

                if (i.Equals(0x002e))
                {
                    expected = ".";
                }

                if (i.Equals(0x005f))
                {
                    expected = "_";
                }

                if (i.Equals(0x007e))
                {
                    expected = "~";
                }
                ////Exception for the Digits
                if (i > 0x002F && i < 0x003a)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }
                ////Exception for the Uppercase Alphas
                if (i > 0x0040 && i < 0x005B)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }
                ////Exception for the Lowercase Alphas
                if (i > 0x0060 && i < 0x007B)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }

                string actual = Encoder.HtmlFormUrlEncode(Convert.ToString((char)i));
                string testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                Assert.AreEqual(expected, actual, "HtmlFormUrlEncode.HtmlFormUrlEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for UrlEncoder
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        private static void UrlEncodeUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle)
        {
            string expected;
            System.Text.Encoding encoders = System.Text.Encoding.UTF8;
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                expected = string.Empty;
                byte[] bytes = encoders.GetBytes(Convert.ToString((char)i));
                for (int x = 0; x < bytes.Length; x++)
                {
                    if (bytes[x] < 16)
                    {
                        expected = expected + "%0" + bytes[x].ToString("X");
                        expected = expected.ToLower();
                    }
                    else
                    {
                        expected = expected + "%" + bytes[x].ToString("X");
                        expected = expected.ToLower();
                    }
                }

                if (i.Equals(0x002d))
                {
                    expected = "-";
                }

                if (i.Equals(0x002e))
                {
                    expected = ".";
                }

                if (i.Equals(0x005f))
                {
                    expected = "_";
                }

                if (i.Equals(0x007e))
                {
                    expected = "~";
                }
                ////Exception for the Digits
                if (i > 0x002F && i < 0x003a)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }
                ////Exception for the Uppercase Alphas
                if (i > 0x0040 && i < 0x005B)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }
                ////Exception for the Lowercase Alphas
                if (i > 0x0060 && i < 0x007B)
                {
                    expected = System.Text.Encoding.ASCII.GetString(bytes);
                }

                string actual = Encoder.UrlEncode(Convert.ToString((char)i));
                string testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                Assert.AreEqual(expected, actual, "UrlEncode.UrlEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for HtmlEncoder
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="unicodeGaps">List of the codepage Unicode gaps</param>
        private static void HtmlEncoderUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle, List<long> unicodeGaps)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string testmessage;
                string expected;
                string actual;
                if (!unicodeGaps.Contains(i))
                {
                    expected = Convert.ToString((char)i);
                    actual = Encoder.HtmlEncode(Convert.ToString((char)i));
                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (non-gap value) ";
                }
                else
                {
                    expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                    actual = Encoder.HtmlEncode(Convert.ToString((char)i));

                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                }

                if (i.Equals(0x0022))
                {
                    expected = "&quot;";
                }

                if (i.Equals(0x0026))
                {
                    expected = "&amp;";
                }

                if (i.Equals(0x003c))
                {
                    expected = "&lt;";
                }

                if (i.Equals(0x003e))
                {
                    expected = "&gt;";
                }

                if (i.Equals(0x003e))
                {
                    expected = "&gt;";
                }

                if (i.Equals(0x0027))
                {
                    expected = "&#39;";
                }

                Assert.AreEqual(expected, actual, "HtmlEncoder.HtmlEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for XmlAttributeEncode
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="unicodeGaps">List of the codepage Unicode gaps</param>
        private static void XmlAttributeEncodeUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle, List<long> unicodeGaps)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string expected;
                string actual;
                string testmessage;
                if (!unicodeGaps.Contains(i))
                {
                    expected = Convert.ToString((char)i);
                    actual = Encoder.XmlAttributeEncode(Convert.ToString((char)i));
                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (non-gap value) ";
                }
                else
                {
                    expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                    actual = Encoder.XmlAttributeEncode(Convert.ToString((char)i));

                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                }

                if (i.Equals(0x0020))
                {
                    expected = "&#32;";
                }

                if (i.Equals(0x0022))
                {
                    expected = "&quot;";
                }

                if (i.Equals(0x0027))
                {
                    expected = "&apos;";
                }

                if (i.Equals(0x0026))
                {
                    expected = "&amp;";
                }

                if (i.Equals(0x003c))
                {
                    expected = "&lt;";
                }

                if (i.Equals(0x003e))
                {
                    expected = "&gt;";
                }

                Assert.AreEqual(expected, actual, "XmlAttributeEncode.XmlAttributeEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for XmlEncode
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="unicodeGaps">List of the codepage Unicode gaps</param>
        private static void XmlEncodeUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle, List<long> unicodeGaps)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string expected;
                string actual;
                string testmessage;

                if (!unicodeGaps.Contains(i))
                {
                    expected = Convert.ToString((char)i);
                    actual = Encoder.XmlEncode(Convert.ToString((char)i));
                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (non-gap value) ";
                }
                else
                {
                    expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                    actual = Encoder.XmlEncode(Convert.ToString((char)i));

                    testmessage = "0x" + i.ToString("X").PadLeft(4, '0') + " (gap value) ";
                }

                if (i.Equals(0x0022))
                {
                    expected = "&quot;";
                }

                if (i.Equals(0x0026))
                {
                    expected = "&amp;";
                }

                if (i.Equals(0x0027))
                {
                    expected = "&apos;";
                }

                if (i.Equals(0x003c))
                {
                    expected = "&lt;";
                }

                if (i.Equals(0x003e))
                {
                    expected = "&gt;";
                }

                Assert.AreEqual(expected, actual, "XmlEncode.XmlEncode " + testmessage + codePageTitle);
            }
        }

        /// <summary>
        /// A test for CSS Encode
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        private static void TestCssEncoderForSpecifiedCodePage(long codePageStart, long codePageEnd, string codePageTitle)
        {
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string expected;
                if ((i > 0x002F && i < 0x003A) || (i > 0x0040 && i < 0x005B) || (i > 0x0060 && i < 0x007B))
                {
                    expected = Convert.ToString((char)i);
                }
                else
                {
                    expected = "\\" + i.ToString("X");
                    while (expected.Length < 7)
                    {
                        expected = expected.Insert(1, "0");
                    }

                    expected = expected.ToUpper();
                }

                string actual = Encoder.CssEncode(Convert.ToString((char)i));
                Assert.AreEqual(expected, actual, "CssEncode.CssEncode " + codePageTitle + " In Unicode: " + i);
            }
        }

        /// <summary>
        /// A test for LDAPEncode DN
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        private static void LdapEncodeDistinguishedNameUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle)
        {
            string expected;
            System.Text.Encoding encoders = System.Text.Encoding.UTF8;
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                expected = string.Empty;
                byte[] bytes = encoders.GetBytes(Convert.ToString((char)i));
                if (i > 0x007F || i < 0x0021)
                {
                    for (int x = 0; x < bytes.Length; x++)
                    {
                        if (bytes[x] < 16)
                        {
                            expected = expected + "#0" + bytes[x].ToString("X");
                        }
                        else
                        {
                            expected = expected + "#" + bytes[x].ToString("X");
                        }
                    }
                }
                else
                {
                    expected = Convert.ToString((char)i);
                }

                if (i.Equals(0x0023))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x005c))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x003C))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x003E))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x002C))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x002B))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x0022))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x003B))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x0020))
                {
                    expected = "\\" + Convert.ToString((char)i);
                }

                if (i.Equals(0x0021))
                {
                    expected = "#21";
                }

                if (i.Equals(0x0026))
                {
                    expected = "#26";
                }

                if (i.Equals(0x0027))
                {
                    expected = "#27";
                }

                if (i.Equals(0x002d))
                {
                    expected = "#2D";
                }

                if (i.Equals(0x003d))
                {
                    expected = "#3D";
                }

                if (i.Equals(0x007c))
                {
                    expected = "#7C";
                }

                string actual = Encoder.LdapDistinguishedNameEncode(Convert.ToString((char)i));
                Assert.AreEqual(expected, actual, "LDAPEncode.LDAPEncode - DN - -" + codePageTitle + " Code - " + i.ToString("X"));
            }
        }

        /// <summary>
        /// A test for LDAPEncode Filter
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        private static void LdapEncodeFilterUnicodeTest(long codePageStart, long codePageEnd, string codePageTitle)
        {
            System.Text.Encoding encoders = System.Text.Encoding.UTF8;
            for (long i = codePageStart; i < codePageEnd; i++)
            {
                string expected = string.Empty;
                byte[] bytes = encoders.GetBytes(Convert.ToString((char)i));
                if (i > 0x007F || i < 0x0020)
                {
                    for (int x = 0; x < bytes.Length; x++)
                    {
                        if (bytes[x] < 16)
                        {
                            expected = expected + "\\0" + bytes[x].ToString("X");
                            expected = expected.ToLower();
                        }
                        else
                        {
                            expected = expected + "\\" + bytes[x].ToString("X");
                            expected = expected.ToLower();
                        }
                    }
                }
                else
                {
                    expected = Convert.ToString((char)i);
                }

                if (i.Equals(0x0028))
                {
                    expected = "\\28";
                }

                if (i.Equals(0x0029))
                {
                    expected = "\\29";
                }

                if (i.Equals(0x005C))
                {
                    expected = "\\5c";
                }

                if (i.Equals(0x002A))
                {
                    expected = "\\2a";
                }

                if (i.Equals(0x002F))
                {
                    expected = "\\2f";
                }

                string actual = Encoder.LdapFilterEncode(Convert.ToString((char)i));
                Assert.AreEqual(expected, actual, "LDAPEncode.LDAPEncode - Filter -  -" + codePageTitle + " Code - " + i.ToString("X"));
            }
        }
    }
}
