// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodepageDetectData.cs" company="Microsoft Corporation">
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
//   Encapsulates code page detection data
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    /// <summary>
    /// Encapsulates code page detection data.
    /// </summary>
    internal class CodePageDetectData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodePageDetectData"/> class.
        /// </summary>
        private CodePageDetectData()
        {
        }

        /// <summary>
        /// The list of code pages and their masks.
        /// </summary>
        internal static readonly CodePage[] CodePages =
        {
            new CodePage(20127, 0x00000001, false),
            new CodePage(28591, 0x00000002, false),
            new CodePage(28592, 0x00000004, false),
            new CodePage(20866, 0x00000008, false),
            new CodePage(21866, 0x00000010, false),
            new CodePage(28595, 0x00000020, false),
            new CodePage(28597, 0x00000040, false),
            new CodePage(28593, 0x00000080, false),
            new CodePage(28594, 0x00000100, false),
            new CodePage(28596, 0x00000200, false),
            new CodePage(38598, 0x00000400, false),
            new CodePage(28605, 0x00000800, false),
            new CodePage(28599, 0x00001000, false),
            new CodePage(1252, 0x00002000, true),
            new CodePage(1250, 0x00004000, true),
            new CodePage(1251, 0x00008000, true),
            new CodePage(1253, 0x00010000, true),
            new CodePage(1254, 0x00020000, true),
            new CodePage(1257, 0x00040000, true),
            new CodePage(1258, 0x00080000, true),
            new CodePage(1256, 0x00100000, true),
            new CodePage(1255, 0x00200000, true),
            new CodePage(874, 0x00400000, true),
            new CodePage(50220, 0x00800000, false),
            new CodePage(932, 0x01000000, true),
            new CodePage(949, 0x02000000, true),
            new CodePage(950, 0x04000000, true),
            new CodePage(936, 0x08000000, true),
            new CodePage(51932, 0x10000000, false),
            new CodePage(51949, 0x20000000, false),
            new CodePage(50225, 0x40000000, false),
            new CodePage(52936, 0x80000000, false),
        };

        /// <summary>
        /// Represents a code page.
        /// </summary>
        internal struct CodePage
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            public ushort Id;

            /// <summary>
            /// The Mask for this codepage.
            /// </summary>
            public uint Mask;

            /// <summary>
            /// True if the codepage is a windows codepage, otherwise false.
            /// </summary>
            public bool IsWindowsCodePage;

            /// <summary>
            /// Initializes a new instance of the <see cref="CodePage"/> struct.
            /// </summary>
            /// <param name="id">The code page identifier.</param>
            /// <param name="mask">The code page Mask.</param>
            /// <param name="isWindowsCodePage">if set to <c>true</c> the code page is a Windows codepage..</param>
            public CodePage(ushort id, uint mask, bool isWindowsCodePage)
            {
                this.Id = id;
                this.Mask = mask;
                this.IsWindowsCodePage = isWindowsCodePage;
            }
        }
    }
}
