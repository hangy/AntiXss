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

        // Orphaned WPL code.
#if false
        public static int Calculate(string obj)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj)
                {
                    char* s = src;
                    int len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (len < 2)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

#if !DATAGEN

        public static int Calculate(BufferString obj)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    char* s = src + obj.Offset;
                    int len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
#endif

        public static int CalculateLowerCase(string obj)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj)
                {
                    char* s = src;
                    int len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

#if !DATAGEN

        public static int CalculateLowerCase(BufferString obj)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    char* s = src + obj.Offset;
                    int len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
#endif

        public static int Calculate(char[] buffer, int offset, int length)
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
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (length == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        length -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
#endif

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

        // Orphaned WPL code.
#if false
        public void Initialize()
        {
            this.offset = 0;
            this.hash1 = this.hash2 = 5381;
        }

        public unsafe void Advance(char* s, int len)
        {
            if (0 != (this.offset & 1))
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ s[0];
                s++;
                len--;
                this.offset++;
            }

            this.offset += len;

            while (len > 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ s[0];
                if (len == 1)
                    break;
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ s[1];
                s += 2;
                len -= 2;
            }
        }
#endif

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

        // Orphaned WPL code.
#if false
        public void Advance(int ucs32)
        {
            // Unicode 32bit literal.

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
                if (0 == (this.offset++ & 1))
                {
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ (int)ucs32;
                }
                else
                {
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ (int)ucs32;
                }
            }
        }
#endif

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

        // Orphaned WPL code.
#if false
        public void Advance(char c)
        {
            if (0 == (this.offset++ & 1))
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c;
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c;
            }
        }
#endif

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

        // Orphaned WPL code.
#if false
#if !DATAGEN

        public void Advance(BufferString obj)
        {
            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    this.Advance(src + obj.Offset, obj.Length);
                }
            }
        }

        public void AdvanceLowerCase(BufferString obj)
        {
            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    this.AdvanceLowerCase(src + obj.Offset, obj.Length);
                }
            }
        }
#endif

        public void Advance(char[] buffer, int offset, int length)
        {
            HashCode.CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    this.Advance(src + offset, length);
                }
            }
        }
#endif

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

