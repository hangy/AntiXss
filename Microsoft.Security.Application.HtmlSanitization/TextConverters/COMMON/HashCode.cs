// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashCode.cs" company="Microsoft Corporation">
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
//    
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;

    internal struct HashCode
    {
        int hash1;
        int hash2;
        int offset;

        public HashCode(bool ignore)
        {
            this.offset = 0;
            this.hash1 = this.hash2 = 5381;
        }

        public static int CalculateEmptyHash()
        {
            return 5381 + unchecked(5381 * 1566083941);
        }

        public static int CalculateLowerCase(char[] buffer, int offset, int length)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            HashCode.CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    char* s = src + offset;

                    while (length > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (length == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        length -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

        public unsafe void AdvanceLowerCase(char* s, int len)
        {
            if (0 != (this.offset & 1))
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(s[0]);
                s++;
                len--;
                this.offset++;
            }

            this.offset += len;

            while (len > 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ParseSupport.ToLowerCase(s[0]);
                if (len == 1)
                    break;
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(s[1]);
                s += 2;
                len -= 2;
            }
        }

        public void AdvanceLowerCase(int ucs32)
        {
            if (ucs32 >= 0x10000)
            {
                char c1 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                char c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                if (0 == ((this.offset += 2) & 1))
                {
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c1;
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c2;
                }
                else
                {
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c1;
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c2;
                }
            }
            else
            {
                this.AdvanceLowerCase((char)ucs32);
            }
        }

        public int AdvanceAndFinalizeHash(char c)
        {
            if (0 == (this.offset++ & 1))
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c;
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c;
            }
            return this.hash1 + (this.hash2 * 1566083941);
        }

        public void AdvanceLowerCase(char c)
        {
            if (0 == (this.offset++ & 1))
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ParseSupport.ToLowerCase(c);
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(c);
            }
        }

        public void AdvanceLowerCase(char[] buffer, int offset, int length)
        {
            HashCode.CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    this.AdvanceLowerCase(src + offset, length);
                }
            }
        }

        private static void CheckArgs(char[] buffer, int offset, int length)
        {
            int bufferLength = buffer.Length;
            if (offset < 0 || offset > bufferLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            if (offset + length < offset ||
                offset + length > bufferLength)
            {
                throw new ArgumentOutOfRangeException("offset + length");
            }
        }

        public int FinalizeHash()
        {
            return this.hash1 + (this.hash2 * 1566083941);
        }
    }
}

