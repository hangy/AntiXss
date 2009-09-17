// ***************************************************************
// <copyright file="ScratchBuffer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    
    
    using System.Diagnostics;
    using Microsoft.Exchange.Data.Internal;
    
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    using Microsoft.Exchange.Data.TextConverters.Internal.Css;
    

    

    internal struct BufferString
    {
        private char[] buffer;
        private int offset;
        private int count;

        public static readonly BufferString Null = new BufferString();

        public BufferString(char[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public void Set(char[] buffer, int offset, int count)
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

        public char[] Buffer
        {
            get { return this.buffer; }
        }

        public int Offset
        {
            get { return this.offset; }
        }

        public int Length
        {
            get { return this.count; }
        }

        public bool IsEmpty
        {
            get { return this.count == 0; }
        }

        public BufferString SubString(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= this.count && count >= 0 && offset + count <= this.count);
            return new BufferString(this.buffer, this.offset + offset, count);
        }

        public void Trim(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= this.count && count >= 0 && offset + count <= this.count);
            this.offset += offset;
            this.count = count;
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

        public bool EqualsToString(string rightPart)
        {
            
            if (this.count != rightPart.Length)
            {
                return false;
            }


            for (int i = 0; i < rightPart.Length; i++)
            {
                if (this.buffer[this.offset + i] != rightPart[i])
                {
                    return false;
                }
            }

            return true;
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

        public static int CompareLowerCaseStringToBufferStringIgnoreCase(string left, BufferString right)
        {
            int len = Math.Min(left.Length, right.Length);

            for (int i = 0; i < len; i++)
            {
                int cmp = (int)left[i] - (int)ParseSupport.ToLowerCase(right[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            
            
            return left.Length - right.Length;                            
        }

        public bool StartsWithLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            

            if (this.count < rightPart.Length)
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

        public bool StartsWithString(string rightPart)
        {
            

            if (this.count < rightPart.Length)
            {
                return false;
            }

            for (int i = 0; i < rightPart.Length; i++)
            {
                if (this.buffer[this.offset + i] != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWithLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            

            if (this.count < rightPart.Length)
            {
                return false;
            }

            int offset = this.offset + this.count - rightPart.Length;

            for (int i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(this.buffer[offset + i]) != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWithString(string rightPart)
        {
            

            if (this.count < rightPart.Length)
            {
                return false;
            }

            int offset = this.offset + this.count - rightPart.Length;

            for (int i = 0; i < rightPart.Length; i++)
            {
                if (this.buffer[offset + i] != rightPart[i])
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

        public int Offset
        {
            get { return 0; }
        }

        public int Length
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public int Capacity
        {
            get { return this.buffer == null ? 64 : this.buffer.Length; }
        }

        public BufferString BufferString
        {
            get { return new BufferString(this.buffer, 0, this.count); }
        }

        public BufferString SubString(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= this.buffer.Length && count >= 0 && offset + count <= this.buffer.Length);
            return new BufferString(this.buffer, offset, count);
        }

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

        public void Reset(int space)
        {
            this.count = 0;

            if (this.buffer == null || this.buffer.Length < space)
            {
                this.buffer = new char[space];
            }
        }

        
        
        
        

        
        
        
        
        
        

        
        

        public bool AppendTokenText(Token token, int maxSize)
        {
            int countRead;
            int countTotal = 0;

            while (0 != (countRead = this.GetSpace(maxSize)) && 
                0 != (countRead = token.Text.Read(this.buffer, this.count, countRead)))
            {
                this.count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
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

        public bool AppendCssPropertyValue(CssProperty prop, int maxSize)
        {
            int countRead;
            int countTotal = 0;

            while (0 != (countRead = this.GetSpace(maxSize)) && 
                0 != (countRead = prop.Value.Read(this.buffer, this.count, countRead)))
            {
                this.count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
        }

        public int AppendInt(int value)
        {
            int len = 1;
            bool negative = false;

            if (value < 0)
            {
                negative = true;
                value = -value;
                len ++;

                if (value < 0)
                {
                    value = int.MaxValue;
                }
            }

            int t = value;
            while (t >= 10)
            {
                t /= 10;
                len ++;
            }

            this.EnsureSpace(len);

            int offset = this.count + len;
            while (value >= 10)
            {
                this.buffer[--offset] = (char)(value % 10 + '0');
                value /= 10;
            }

            this.buffer[--offset] = (char)(value + '0');
            if (negative)
            {
                this.buffer[--offset] = '-';
            }

            this.count += len;

            return len;
        }

        public int AppendFractional(int value, int decimalPoint)
        {
            int len = this.AppendInt(value / decimalPoint);

            if (value % decimalPoint != 0)
            {
                if (value < 0)
                {
                    value = -value;
                }

                int fraction = (int)(((long)value * 100 + decimalPoint / 2) / decimalPoint) % 100;

                if (fraction != 0)
                {
                    len += this.Append('.');

                    if (fraction % 10 == 0)
                    {
                        fraction /= 10;
                    }

                    len += this.AppendInt(fraction);
                }
            }

            return len;
        }

        public int AppendHex2(uint value)
        {
            this.EnsureSpace(2);

            uint h = (value >> 4) & 0xF;
            if (h < 10)
            {
                this.buffer[this.count++] = (char)(h + '0');
            }
            else
            {
                this.buffer[this.count++] = (char)(h - 10 + 'A');
            }

            h = value & 0xF;
            if (h < 10)
            {
                this.buffer[this.count++] = (char)(h + '0');
            }
            else
            {
                this.buffer[this.count++] = (char)(h - 10 + 'A');
            }

            return 2;
        }

        public int Append(char ch)
        {
            return this.Append(ch, int.MaxValue);
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

        public int Append(string str)
        {
            return this.Append(str, int.MaxValue);
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

        public int Append(char[] buffer, int offset, int length)
        {
            return this.Append(buffer, offset, length, int.MaxValue);
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

        public string ToString(int offset, int count)
        {
            
            return new String(this.buffer, offset, count);
        }

        public void DisposeBuffer()
        {
            this.buffer = null;
            this.count = 0;
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

        private void EnsureSpace(int space)
        {
            InternalDebug.Assert(this.buffer != null || this.count == 0);
            InternalDebug.Assert(this.buffer == null || this.count <= this.buffer.Length);

            if (this.buffer == null)
            {
                this.buffer = new char[Math.Max(space, 64)];
            }
            else if (this.buffer.Length - this.count < space)
            {
                char[] newBuffer = new char[Math.Max(this.buffer.Length * 2, this.count + space)];
                System.Buffer.BlockCopy(this.buffer, 0, newBuffer, 0, this.count * 2);
                this.buffer = newBuffer;
            }
        }
    }
}

