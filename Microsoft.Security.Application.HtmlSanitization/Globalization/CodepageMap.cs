// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodepageMap.cs" company="Microsoft Corporation">
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
//   Represents the logic to choose a codepage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using Internal;
    
    /// <summary>
    /// Represents the logic to choose a codepage.
    /// </summary>
    internal class CodePageMap : CodePageMapData
    {
        /// <summary>
        /// The current codepage.
        /// </summary>
        private int codePage;

        /// <summary>
        /// The ranges for the current codepage.
        /// </summary>
        private CodePageRange[] ranges;
        
        /// <summary>
        /// The previous index used for range operations.
        /// </summary>
        private int lastRangeIndex;

        /// <summary>
        /// The last codepage range used.
        /// </summary>
        private CodePageRange lastRange;
        
        /// <summary>
        /// Chooses the current code page.
        /// </summary>
        /// <param name="newCodePage">The code page to choose.</param>
        /// <returns>True if the selection is succesful, otherwise false.</returns>
        public bool ChoseCodePage(int newCodePage)
        {
            if (newCodePage == this.codePage)
            {
                return true;
            }

            this.codePage = newCodePage;
            this.ranges = null;

            if (newCodePage == 1200)
            {
                return true;
            }

            for (int i = CodePages.Length - 1; i >= 0; i--)
            {
                if (CodePages[i].Id != newCodePage)
                {
                    continue;
                }

                this.ranges = CodePages[i].Ranges;
                this.lastRangeIndex = this.ranges.Length / 2;
                this.lastRange = this.ranges[this.lastRangeIndex];

                return true;
            }

            return false;
        }
       
        /// <summary>
        /// Decides if an extended chracter is unsafe for the current codepage.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>True if the character is unsafe, otherwise false.</returns>
        public bool IsUnsafeExtendedCharacter(char ch)
        {
            if (this.ranges == null)
            {
                InternalDebug.Assert(false);
                return false;
            }

            if (ch <= this.lastRange.Last)
            {
                if (ch >= this.lastRange.First)
                {                    
                    return this.lastRange.Offset != 0xFFFFu && (Bitmap[this.lastRange.Offset + (ch - this.lastRange.First)] & this.lastRange.Mask) == 0;
                }
                
                int i = this.lastRangeIndex;

                while (--i >= 0)
                {
                    if (ch < this.ranges[i].First)
                    {
                        continue;
                    }

                    if (ch <= this.ranges[i].Last)
                    {
                        if (ch == this.ranges[i].First)
                        {                                    
                            return false;
                        }

                        this.lastRangeIndex = i;
                        this.lastRange = this.ranges[i];

                        return this.lastRange.Offset != 0xFFFFu && (Bitmap[this.lastRange.Offset + (ch - this.lastRange.First)] & this.lastRange.Mask) == 0;
                    }

                    break;
                }
            }
            else
            {
                int i = this.lastRangeIndex;

                while (++ i < this.ranges.Length)
                {
                    if (ch > this.ranges[i].Last)
                    {
                        continue;
                    }

                    if (ch >= this.ranges[i].First)
                    {
                        if (ch == this.ranges[i].First)
                        {                                
                            return false;
                        }

                        this.lastRangeIndex = i;
                        this.lastRange = this.ranges[i];

                        return this.lastRange.Offset != 0xFFFFu && (Bitmap[this.lastRange.Offset + (ch - this.lastRange.First)] & this.lastRange.Mask) == 0;
                    }

                    break;
                }
            }

            return true;
        }
    }
}

