// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScratchBuffer.cs" company="Microsoft Corporation">
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
    using System.Diagnostics;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Css;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Security.Application.TextConverters.HTML;

    internal struct BufferString
    {
        private readonly char[] buffer;
        private int offset;
        private int count;

        public static readonly BufferString Null = new();

        public BufferString(char[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public char this[int index]
        {
            get
            {
                InternalDebug.Assert(index < this.count);
                return this.buffer[this.offset + index];
            }
        }

        public int Length
        {
            get { return this.count; }
        }

        public void TrimWhitespace()
        {
            while (this.count != 0 && ParseSupport.WhitespaceCharacter(this.buffer[this.offset]))
            {
                this.offset++;
                this.count --;
            }

            if (this.count != 0)
            {
                int end = this.offset + this.count - 1;
                while (ParseSupport.WhitespaceCharacter(this.buffer[end--]))
                {
                    this.count --;
                }
            }
        }

        public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            if (this.count != rightPart.Length)
            {
                return false;
            }

            for (int i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(this.buffer[this.offset + i]) != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return this.buffer == null ? null : this.count == 0 ? String.Empty : new String(this.buffer, this.offset, this.count);
        }

        [Conditional("DEBUG")]
        private static void AssertStringIsLowerCase(string rightPart)
        {
#if DEBUG
            foreach (char ch in rightPart)
            {
                if (ch > (char)0x7F || char.ToLowerInvariant(ch) != ch)
                {
                    InternalDebug.Assert(false, "right part string is supposed to be in lower case for this method.");
                    break;
                }
            }
#endif
        }
    }

    internal struct ScratchBuffer
    {
        private char[] buffer;
        private int count;

        public char[] Buffer
        {
            get { return this.buffer; }
        }

        public int Length
        {
            get { return this.count; }
        }

        public BufferString BufferString
        {
            get { return new BufferString(this.buffer, 0, this.count); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Erroneous FXCop warning.")]
        public char this[int offset]
        {
            get
            {
                InternalDebug.Assert(offset < this.buffer.Length);

                return this.buffer[offset];
            }
            set
            {
                InternalDebug.Assert(offset < this.buffer.Length);

                this.buffer[offset] = value;
            }
        }

        public void Reset()
        {
            this.count = 0;
        }

        public bool AppendHtmlAttributeValue(HtmlAttribute attr, int maxSize)
        {
            int countRead;
            int countTotal = 0;

            while (0 != (countRead = this.GetSpace(maxSize)) &&
                0 != (countRead = attr.Value.Read(this.buffer, this.count, countRead)))
            {
                this.count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
        }

        public int Append(char ch, int maxSize)
        {
            if (0 == this.GetSpace(maxSize))
            {
                return 0;
            }

            this.buffer[this.count++] = ch;
            return 1;
        }

        public int Append(string str, int maxSize)
        {
            int countRead;
            int countTotal = 0;

            while (0 != (countRead = Math.Min(this.GetSpace(maxSize), str.Length - countTotal)))
            {
                str.CopyTo(countTotal, this.buffer, this.count, countRead);
                this.count += countRead;
                countTotal += countRead;
            }

            return countTotal;
        }

        public int Append(char[] buffer, int offset, int length, int maxSize)
        {
            int countRead;
            int countTotal = 0;

            while (0 != (countRead = Math.Min(this.GetSpace(maxSize), length)))
            {
                System.Buffer.BlockCopy(buffer, offset * 2, this.buffer, this.count * 2, countRead * 2);
                this.count += countRead;
                offset += countRead;
                length -= countRead;
                countTotal += countRead;
            }

            return countTotal;
        }

        private int GetSpace(int maxSize)
        {
            InternalDebug.Assert((this.buffer == null && this.count == 0) || this.count <= this.buffer.Length);

            if (this.count >= maxSize)
            {
                return 0;
            }

            if (this.buffer == null)
            {
                this.buffer = new char[64];
            }
            else if (this.buffer.Length == this.count)
            {
                char[] newBuffer = new char[this.buffer.Length * 2];
                System.Buffer.BlockCopy(this.buffer, 0, newBuffer, 0, this.count * 2);
                this.buffer = newBuffer;
            }

            return this.buffer.Length - this.count;
        }
    }
}

