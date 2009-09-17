// ***************************************************************
// <copyright file="CodepageMap.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using Microsoft.Exchange.Data.Internal;

    
    
    internal class CodePageMap : CodePageMapData
    {
        private int codePage;
        private CodePageRange[] ranges;
        private int lastRangeIndex;
        private CodePageRange lastRange;

        
        
        
        
        public bool ChoseCodePage(int codePage)
        {
            if (codePage == this.codePage)
            {
                return true;
            }

            this.codePage = codePage;
            this.ranges = null;

            if (codePage == 1200)
            {
                return true;
            }

            for (int i = codePages.Length - 1; i >= 0; i--)
            {
                if (codePages[i].cpid == codePage)
                {
                    

                    this.ranges = codePages[i].ranges;
                    this.lastRangeIndex = this.ranges.Length / 2;
                    this.lastRange = this.ranges[this.lastRangeIndex];

                    return true;
                }
            }

            return false;
        }

        
        
        
        
        public bool IsUnsafeExtendedCharacter(char ch)
        {
            

            if (this.ranges == null)
            {
                

                
                
                
                

                InternalDebug.Assert(false);

                
                
                

                
                
                

                return false;
            }

            if (ch <= this.lastRange.last)
            {
                if (ch >= this.lastRange.first)
                {
                    
                    return this.lastRange.offset != 0xFFFFu && (bitmap[this.lastRange.offset + (ch - this.lastRange.first)] & this.lastRange.mask) == 0;
                }
                else
                {
                    

                    int i = this.lastRangeIndex;

                    while (--i >= 0)
                    {
                        if (ch >= this.ranges[i].first)
                        {
                            if (ch <= this.ranges[i].last)
                            {
                                if (ch == this.ranges[i].first)
                                {
                                    
                                    return false;
                                }

                                
                                
                                this.lastRangeIndex = i;
                                this.lastRange = this.ranges[i];

                                return this.lastRange.offset != 0xFFFFu && (bitmap[this.lastRange.offset + (ch - this.lastRange.first)] & this.lastRange.mask) == 0;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                

                int i = this.lastRangeIndex;

                while (++ i < this.ranges.Length)
                {
                    if (ch <= this.ranges[i].last)
                    {
                        if (ch >= this.ranges[i].first)
                        {
                            if (ch == this.ranges[i].first)
                            {
                                
                                return false;
                            }

                            
                            
                            this.lastRangeIndex = i;
                            this.lastRange = this.ranges[i];

                            return this.lastRange.offset != 0xFFFFu && (bitmap[this.lastRange.offset + (ch - this.lastRange.first)] & this.lastRange.mask) == 0;
                        }

                        break;
                    }
                }
            }

            
            return true;
        }
    }
}

