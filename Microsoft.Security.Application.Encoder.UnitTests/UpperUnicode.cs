
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpperUnicode.cs" company="Microsoft Corporation">
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
//   Performs upper UpperUnicode unit tests for each of the encoder methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Application.Security
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// UpperUnicode test cases for AntiXss library. 
    /// Following Test cases shall provide testing coverage of each UpperUnicode
    /// code page version 5.2 as defined in http://www.unicode.org.
    /// </summary>
    [TestClass()]
    public class UpperUnicode
    {
        /// <summary>
        /// All UpperUnicode characters from Ancient Greek Musical Notation 0x1D200-0x1D24F
        /// </summary>
        /// <remarks>not supported</remarks>
        [TestMethod]
        public void UpperUnicodeAncientGreekMusicalNotation()
        {
            long codePageStart = 0x1D200;
            long codePageEnd = 0x1D24F;
            string codePageTitle = "Ancient Greek Musical Notation 0x1D200-0x1D24F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Ancient Greek Musical Notation http://www.unicode.org/charts/PDF/U1D200.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1D246, 0x1D247, 0x1D248, 0x1D249, 0x1D24A, 0x1D24B, 0x1D24C, 0x1D24D, 0x1D24E, 0x1D24F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Ancient Greek Numbers 0x10140-0x1018F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeAncientGreekNumbers()
        {
            long codePageStart = 0x10140;
            long codePageEnd = 0x1018F;
            string codePageTitle = "Ancient Greek Numbers 0x10140-0x1018F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Ancient Greek Numbers http://www.unicode.org/charts/PDF/U10140.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1018B, 0x1018C, 0x1018D, 0x1018E, 0x1018F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Ancient Symbols 0x10190-0x101CF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeAncientSymbols()
        {
            long codePageStart = 0x10190;
            long codePageEnd = 0x101CF;
            string codePageTitle = "Ancient Symbols 0x10190-0x101CF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Ancient Symbols http://www.unicode.org/charts/PDF/U10190.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1019C, 0x1019D, 0x1019E, 0x1019F, 
                0x101A0, 0x101A1, 0x101A2, 0x101A3, 0x101A4, 0x101A5, 0x101A6, 0x101A7, 0x101A8, 0x101A9, 0x101AA, 0x101AB, 0x101AC, 0x101AD, 0x101AE, 0x101AF,
                0x101B0, 0x101B1, 0x101B2, 0x101B3, 0x101B4, 0x101B5, 0x101B6, 0x101B7, 0x101B8, 0x101B9, 0x101BA, 0x101BB, 0x101BC, 0x101BD, 0x101BE, 0x101BF,
                0x101C0, 0x101C1, 0x101C2, 0x101C3, 0x101C4, 0x101C5, 0x101C6, 0x101C7, 0x101C8, 0x101C9, 0x101CA, 0x101CB, 0x101CC, 0x101CD, 0x101CE, 0x101CF
           };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Aegean Numbers 0x10100-0x1013F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeAegeanNumbers()
        {
            long codePageStart = 0x10100;
            long codePageEnd = 0x1013F;
            string codePageTitle = "Aegean Numbers 0x10100-0x1013F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Aegean Numbers http://www.unicode.org/charts/PDF/U10100.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x10103, 0x10104, 0x10105, 0x10106, 0x10134, 0x10135, 0x10136
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Avestan 0x10B00-0x10B3F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeAvestan()
        {
            long codePageStart = 0x10B00;
            long codePageEnd = 0x10B3F;
            string codePageTitle = "Avestan 0x10B00-0x10B3F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Avestan http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10B00.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x10B36, 0x10B37, 0x10B38
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Byzantine Musical Symbols 0x1D000-0x1D0FF
        /// </summary>
        /// <remarks> Does not have a MarkAsSafe enum</remarks>
        [TestMethod]
        public void UpperUnicodeByzantineMusicalSymbols()
        {
            long codePageStart = 0x1D000;
            long codePageEnd = 0x1D0FF;
            string codePageTitle = "Byzantine Musical Symbols 0x1D000-0x1D0FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Byzantine Musical Symbols http://www.unicode.org/charts/PDF/U1D000.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1D0F6,  0x1D0F7,  0x1D0F8,  0x1D0F9,  0x1D0FA,  0x1D0FB,  0x1D0FC,  0x1D0FD,  0x1D0FE,  0x1D0FF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Carian 0x102A0-0x102DF
        /// </summary>
        /// <remarks> Does not have a MarkAsSafe enum</remarks>
        [TestMethod]
        public void UpperUnicodeCarian()
        {
            long codePageStart = 0x102A0;
            long codePageEnd = 0x102DF;
            string codePageTitle = "Carian 0x102A0-0x102DF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Carian http://www.unicode.org/charts/PDF/U102A0.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x102D1, 0x102D2, 0x102D3, 0x102D4, 0x102D5, 0x102D6, 0x102D7, 0x102D8, 0x102D9, 0x102DA, 0x102DB, 0x102DC, 0x102DD, 0x102DE, 0x102DF
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from CJK Compatibility Ideographs Supplement 0x2F800-0x2FA1F
        /// </summary>
        /// <remarks> Does not have a MarkAsSafe enum</remarks>
        [TestMethod]
        public void UpperUnicodeCJKCompatibilityIdeographsSupplement()
        {
            long codePageStart = 0x2F800;
            long codePageEnd = 0x2FA1F;
            string codePageTitle = "CJK Compatibility Ideographs Supplement 0x2F800-0x2FA1F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 CJK Compatibility Ideographs Supplement http://www.unicode.org/charts/PDF/U2F800.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x2FA1E, 0x2FA1F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from CJK Unified Ideographs Extension B 0x20000-0x2A6DF
        /// </summary>
        /// <remarks> Does not have a MarkAsSafe enum</remarks>
        [TestMethod]
        public void UpperUnicodeCJKUnifiedIdeographsExtensionB()
        {
            long codePageStart = 0x20000;
            long codePageEnd = 0x2A6DF;
            string codePageTitle = "CJK Unified Ideographs Extension B 0x20000-0x2A6DF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 CJK Unified Ideographs Extension B http://www.unicode.org/charts/PDF/U20000.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from CJK Unified Ideographs Extension – C 0x2A700-0x2B73F
        /// </summary>
        /// <remarks> Does not have a MarkAsSafe enum</remarks>
        [TestMethod]
        public void UpperUnicodeCJKUnifiedIdeographsExtensionC()
        {
            long codePageStart = 0x2A700;
            long codePageEnd = 0x2B73F;
            string codePageTitle = "CJK Unified Ideographs Extension – C 0x2A700-0x2B73F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 CJK Unified Ideographs Extension – C http://www.unicode.org/charts/PDF/Unicode-5.2/U52-2A700.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }
        
        /// <summary>
        /// All UpperUnicode characters from Counting Rod Numerals 0x1D360-0x1D37FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeCountingRodNumerals()
        {
            long codePageStart = 0x1D360;
            long codePageEnd = 0x1D37F;
            string codePageTitle = "Counting Rod Numerals 0x1D360-0x1D37F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Counting Rod Numerals http://www.unicode.org/charts/PDF/U1D360.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1D372, 0x1D373, 0x1D374, 0x1D375, 0x1D376, 0x1D377, 0x1D378, 0x1D379, 0x1D37A, 0x1D37B, 0x1D37C, 0x1D37D, 0x1D37E, 0x1D37F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Cuneiform Numbers and Punctuation 0x12400-0x1247F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeCuneiformNumbersandPunctuation()
        {
            long codePageStart = 0x12400;
            long codePageEnd = 0x1247F;
            string codePageTitle = "Cuneiform Numbers and Punctuation 0x12400-0x1247F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Cuneiform Numbers and Punctuation http://www.unicode.org/charts/PDF/U12400.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x12463, 0x12464, 0x12465, 0x12466, 0x12467, 0x12468, 0x12469, 0x1246A, 0x1246B, 0x1246C, 0x1246D, 0x1246E, 0x1246F,
                0x12474, 0x12475, 0x12476, 0x12477, 0x12478, 0x12479, 0x1247A, 0x1247B, 0x1247C, 0x1247D, 0x1247E, 0x1247F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Cypriot Syllabary 0x10800-0x1083F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeCypriotSyllabary()
        {
            long codePageStart = 0x10800;
            long codePageEnd = 0x1083F;
            string codePageTitle = "Cypriot Syllabary 0x10800-0x1083F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Cypriot Syllabary http://www.unicode.org/charts/PDF/U10800.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x10806, 0x10807, 0x10809, 0x10836, 0x10839, 0x1083A, 0x1083
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All unicode characters from Deseret 0x10400-0x1044F
        /// </summary>
        [TestMethod]
        public void UnicodeDeseret()
        {
            long codePageStart = 0x10400;
            long codePageEnd = 0x1044F;
            string codePageTitle = "Deseret 0x10400-0x1044F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the Unicode Standard, Version 5.2 Deseret
            List<long> upperUnicodeGaps = new List<long>()
            {
                // no gaps
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Domino Tiles 0x1F030-0x1F09F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeDominoTiles()
        {
            long codePageStart = 0x1F030;
            long codePageEnd = 0x1F09F;
            string codePageTitle = "Domino Tiles 0x1F030-0x1F09F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Domino Tiles
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Enclosed Alphanumeric Supplement 0x1F100-0x1F1FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeEnclosedAlphanumericSupplement()
        {
            long codePageStart = 0x1F100;
            long codePageEnd = 0x1F1FF;
            string codePageTitle = "Enclosed Alphanumeric Supplement 0x1F100-0x1F1FF";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts.Default, Microsoft.Security.Application.LowerMidCodeCharts.None, Microsoft.Security.Application.MidCodeCharts.None, Microsoft.Security.Application.UpperMidCodeCharts.None, Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Enclosed Alphanumeric Supplement http://www.unicode.org/charts/PDF/Unicode-5.2/U52-1F100.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0x1F10B, 0x1F10C, 0x1F10D, 0x1F10E, 0x1F10F, 0x1F12F, 0x1F130, 0x1F132, 0x1F133, 0x1F134, 0x1F135, 0x1F136, 0x1F137, 0x1F138, 0x1F139, 0x1F13A, 0x1F13B, 0x1F13C, 0x1F13E,
                0x1F140, 0x1F141, 0x1F143, 0x1F144, 0x1F145, 0x1F147, 0x1F148, 0x1F149, 0x1F14F // to do: need to complete
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Enclosed Ideographic Supplement 0x1F200-0x1F2FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeEnclosedIdeographicSupplement()
        {
            long codePageStart = 0x1F200;
            long codePageEnd = 0x1F2FF;
            string codePageTitle = "Enclosed Ideographic Supplement 0x1F200-0x1F2FF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Enclosed Ideographic Supplement http://www.unicode.org/charts/PDF/Unicode-5.2/U52-1F200.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Egyptian Hieroglyphs 0x13000-0x1342F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeEgyptianHieroglyphs()
        {
            long codePageStart = 0x13000;
            long codePageEnd = 0x1342F;
            string codePageTitle = "Egyptian Hieroglyphs 0x13000-0x1342F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Egyptian Hieroglyphs http://www.unicode.org/charts/PDF/Unicode-5.2/U52-13000.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Gothic 0x10330-0x1034F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeGothic()
        {
            long codePageStart = 0x10330;
            long codePageEnd = 0x1034F;
            string codePageTitle = "Gothic 0x10330-0x1034F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Gothic 
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Imperial Aramaic – A 0x10840-0x1085F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeImperialAramaic()
        {
            long codePageStart = 0x10840;
            long codePageEnd = 0x1085F;
            string codePageTitle = "Imperial Aramaic – A 0x10840-0x1085F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Imperial Aramaic http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10840.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Inscriptional Pahlavi 0x10B60-0x10B7F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeInscriptionalPahlavi()
        {
            long codePageStart = 0x10B60;
            long codePageEnd = 0x10B7F;
            string codePageTitle = "Inscriptional Pahlavi 0x10B60-0x10B7F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Inscriptional Pahlavi http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10B60.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Inscriptional Parthian 0x10B40-0x10B5F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeInscriptionalParthian()
        {
            long codePageStart = 0x10B40;
            long codePageEnd = 0x10B5F;
            string codePageTitle = "Inscriptional Parthian 0x10B40-0x10B5F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Inscriptional Parthian http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10B40.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Kaithi 0x11080-0x110CF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeKaithi()
        {
            long codePageStart = 0x11080;
            long codePageEnd = 0x110CF;
            string codePageTitle = "Kaithi 0x11080-0x110CF";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Kaithi http://www.unicode.org/charts/PDF/Unicode-5.2/U52-11080.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Kharoshthi 0x10A00-0x10A5F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeKharoshthi()
        {
            long codePageStart = 0x10A00;
            long codePageEnd = 0x10A5F;
            string codePageTitle = "Kharoshthi 0x10A00-0x10A5F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Kharoshthi
            List<long> upperUnicodeGaps = new List<long>()
            {
                // note: there are no gaps
            };
            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Linear B Ideograms 0x10080-0x100FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeLinearBIdeograms()
        {
            long codePageStart = 0x10080;
            long codePageEnd = 0x100FF;
            string codePageTitle = "Linear B Ideograms 0x10080-0x100FF";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Linear B Ideograms 
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Linear B Syllabary 0x10000-0x1007F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeLinearBSyllabary()
        {
            long codePageStart = 0x10000;
            long codePageEnd = 0x1007F;
            string codePageTitle = "Linear B Syllabary 0x10000-0x1007F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Linear B Syllabary 
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }
        
        /// <summary>
        /// All UpperUnicode characters from Lydian 0x10920-0x1093F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeLydian()
        {
            long codePageStart = 0x10920;
            long codePageEnd = 0x1093F;
            string codePageTitle = "Lydian 0x10920-0x1093F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Lydian
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Mahjong Tiles 0x1F000-0x1F02F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeMahjongTiles()
        {
            long codePageStart = 0x1F000;
            long codePageEnd = 0x1F02F;
            string codePageTitle = "Mahjong Tiles 0x1F000-0x1F02F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Mahjong Tiles
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Mathematical Alphanumeric Symbols 0x1D400-0x1D7FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeMathematicalAlphanumericSymbols()
        {
            long codePageStart = 0x1D400;
            long codePageEnd = 0x1D7FF;
            string codePageTitle = "Mathematical Alphanumeric Symbols 0x1D400-0x1D7FF";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Mathematical Alphanumeric Symbols
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Musical Symbols 0x1D100-0x1D1FF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeMusicalSymbols()
        {
            long codePageStart = 0x1D100;
            long codePageEnd = 0x1D1FF;
            string codePageTitle = "Musical Symbols 0x1D100-0x1D1FF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Musical Symbols
            List<long> upperUnicodeGaps = new List<long>()
            {
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Old Italic 0x10300-0x1032F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeOldItalic()
        {
            long codePageStart = 0x10300;
            long codePageEnd = 0x1032F;
            string codePageTitle = "Old Italic 0x10300-0x1032F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Old Italic
            List<long> upperUnicodeGaps = new List<long>()
        {
        0x1031F, 0x10324, 0x10325, 0x10326, 0x10327, 0x10328, 0x10329, 0x1032A, 0x1032B, 0x1032C, 0x1032D, 0x1032E, 0x1032F
        };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Old Persian 0x103A0-0x103DF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeOldPersian()
        {
            long codePageStart = 0x103A0;
            long codePageEnd = 0x103DF;
            string codePageTitle = "Old Persian 0x103A0-0x103DF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Old Persian
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Old Turkic 0x10C00-0x10C4F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeOldTurkic()
        {
            long codePageStart = 0x10C00;
            long codePageEnd = 0x10C4F;
            string codePageTitle = "Old Turkic 0x10C00-0x10C4F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Old Turkic http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10C00.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Old South Arabian 0x10A60-0x10A7F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeOldSouthArabian()
        {
            long codePageStart = 0x10A60;
            long codePageEnd = 0x10A7F;
            string codePageTitle = "Old South Arabian 0x10A60-0x10A7F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Old South Arabian http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10A60.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Phoenician 0x10900-0x1091F
        /// </summary>
        [TestMethod]
        public void UpperUnicodePhoenician()
        {
            long codePageStart = 0x10900;
            long codePageEnd = 0x1091F;
            string codePageTitle = "Phoenician 0x10900-0x1091F";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Phoenician http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10900.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Osmanya 0x10480-0x104AF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeOsmanya()
        {
            long codePageStart = 0x10480;
            long codePageEnd = 0x104AF;
            string codePageTitle = "Osmanya 0x10480-0x104AF";

            // compiled list of not assigned from the UpperUnicode Standard, Version 5.2 Osmanya 
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Rumi Numeral Symbols 0x10E60-0x10E7F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeRumiNumeralSymbols()
        {
            long codePageStart = 0x10E60;
            long codePageEnd = 0x10E7F;
            string codePageTitle = "Rumi Numeral Symbols 0x10E60-0x10E7F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Rumi Numeral Symbols http://www.unicode.org/charts/PDF/Unicode-5.2/U52-10E60.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }
        
        /// <summary>
        /// All UpperUnicode characters from Shavian 0x10450-0x1047F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeShavian()
        {
            long codePageStart = 0x10450;
            long codePageEnd = 0x1047F;
            string codePageTitle = "Shavian 0x10450-0x1047F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Shavian 
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Supplementary Private Use Area-A 0xF0000-0xFFFFF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeSupplementaryPrivateUseAreaA()
        {
            long codePageStart = 0xF0000;
            long codePageEnd = 0xFFFFF;
            string codePageTitle = "Supplementary Private Use Area-A 0xF0000-0xFFFFF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Supplementary Private Use Area-A http://www.unicode.org/charts/PDF/UF0000.pdf
            List<long> upperUnicodeGaps = new List<long>();
            for (long i = codePageStart; i <= codePageEnd; i++)
            {
                upperUnicodeGaps.Add(i);
            }

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Supplementary Private Use Area-B 0x100000-0x10FFFF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeSupplementaryPrivateUseAreaB()
        {
            long codePageStart = 0x100000;
            long codePageEnd = 0x10FFFF;
            string codePageTitle = "Supplementary Private Use Area-B 0x100000-0x10FFFF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Supplementary Private Use Area-A
            List<long> upperUnicodeGaps = new List<long>()
            {
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Tags 0xE0000-0xE007F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeTags()
        {
            long codePageStart = 0xE0000;
            long codePageEnd = 0xE007F;
            string codePageTitle = "Tags 0xE0000-0xE007F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Tags http://www.unicode.org/charts/PDF/UE0000.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                0xE0000, 0xE0002, 0xE0003, 0xE0004, 0xE0005, 0xE0006, 0xE0007, 0xE0008, 0xE0009, 0xE000A, 0xE000B, 0xE000C, 0xE000D, 0xE000E, 0xE000F,
                0xE0010, 0xE0011, 0xE0012, 0xE0013, 0xE0014, 0xE0015, 0xE0016, 0xE0017, 0xE0018, 0xE0019, 0xE001A, 0xE001B, 0xE001C, 0xE001D, 0xE001E, 0xE001F
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Tai Xuan Jing Symbols 0x1D300-0x1D35F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeTaiXuanJingSymbols()
        {
            long codePageStart = 0x1D300;
            long codePageEnd = 0x1D35F;
            string codePageTitle = "Tai Xuan Jing Symbols 0x1D300-0x1D35F";

            Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(
            Microsoft.Security.Application.LowerCodeCharts.Default,
            Microsoft.Security.Application.LowerMidCodeCharts.None,
            Microsoft.Security.Application.MidCodeCharts.None,
            Microsoft.Security.Application.UpperMidCodeCharts.None,
            Microsoft.Security.Application.UpperCodeCharts.None);

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Tai Xuan Jing Symbols http://www.unicode.org/charts/PDF/U1D300.pdf
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Ugaritic 0x10380-0x1039F
        /// </summary>
        [TestMethod]
        public void UpperUnicodeUgaritic()
        {
            long codePageStart = 0x10380;
            long codePageEnd = 0x1039F;
            string codePageTitle = "Ugaritic 0x10380-0x1039F";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Ugaritic
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// All UpperUnicode characters from Variation Selectors Supplement 0xE0100-0xE01EF
        /// </summary>
        [TestMethod]
        public void UpperUnicodeVariationSelectorsSupplement()
        {
            long codePageStart = 0xE0100;
            long codePageEnd = 0xE01EF;
            string codePageTitle = "Variation Selectors Supplement 0xE0100-0xE01EF";

            // compiled list of "not assigned" from the UpperUnicode Standard, Version 5.2 Variation Selectors Supplement
            List<long> upperUnicodeGaps = new List<long>()
            {
                // todo: need to transcribe this
            };

            this.CallUnitTests(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// Method that is called by all the codepage unit tests and executes the each encoding test
        /// for that codepage.
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="upperUnicodeGaps">List of the codepage UpperUnicode gaps</param>
        public void CallUnitTests(long codePageStart, long codePageEnd, string codePageTitle, List<long> upperUnicodeGaps)
        {
            this.Encoder_Unicode_Test(codePageStart, codePageEnd, codePageTitle, upperUnicodeGaps);
        }

        /// <summary>
        /// A test for Encoder
        /// </summary>
        /// <param name="codePageStart">String of the start of the codepage</param>
        /// <param name="codePageEnd">String of the end of the codepage</param>
        /// <param name="codePageTitle">String of the title of the codepage</param>
        /// <param name="upperUnicodeGaps">List of the codepage UpperUnicode gaps</param>
        private void Encoder_Unicode_Test(long codePageStart, long codePageEnd, string codePageTitle, List<long> upperUnicodeGaps)
        {
            string expected;
            string actual;
            string testmessage;

            for (long i = codePageStart; i < codePageEnd; i++)
            {
                long h = ((i - 0x10000) / 0x400) + 0xD800;
                long l = ((i - 0x10000) % 0x400) + 0xDC00;
                char x = (char)(((i - 0x10000) / 0x400) + 0xd800);
                char y = (char)((i - ((x - 0xd800) * 0x400)) - 0x10000 + 0xdc00);

                string high = h.ToString("x").PadLeft(4, '0');
                string low = l.ToString("x").PadLeft(4, '0');
                string target = "\\u" + Convert.ToString((char)h) + "\\u" + Convert.ToString((char)l);

                target = Convert.ToString((char)h) + Convert.ToString((char)l);
                expected = "&#" + int.Parse(Convert.ToString(i, 16), System.Globalization.NumberStyles.HexNumber) + ";";
                actual = Microsoft.Security.Application.Encoder.HtmlEncode(target);

                testmessage = "0x" + i.ToString("x").PadLeft(5, '0') + " (gap value) ";

                Assert.AreEqual(expected, actual, "Encoder.HtmlEncode " + testmessage + codePageTitle);
            }
        }
    }
}
