// ***************************************************************
// <copyright file="StringBuildSink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;

    

    internal class StringBuildSink : ITextSinkEx
    {
        private StringBuilder sb;
        int maxLength;

        public StringBuildSink()
        {
            
            
            
            

            this.sb = new StringBuilder();
        }

        public bool IsEnough { get { return this.sb.Length >= this.maxLength; } }

        public void Reset(int maxLength)
        {
            this.maxLength = maxLength;
            this.sb.Length = 0;
        }

        public void Write(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(!this.IsEnough);

            count = Math.Min(count, this.maxLength - this.sb.Length);
            this.sb.Append(buffer, offset, count);
        }

        public void Write(int ucs32Char)
        {
            InternalDebug.Assert(!this.IsEnough);

            if (Token.LiteralLength(ucs32Char) == 1)
            {
                this.sb.Append((char)ucs32Char);
            }
            else
            {
                this.sb.Append(Token.LiteralFirstChar(ucs32Char));
                if (!this.IsEnough)
                {
                    this.sb.Append(Token.LiteralLastChar(ucs32Char));
                }
            }
        }

        public void Write(string value)
        {
            InternalDebug.Assert(!this.IsEnough);

            
            this.sb.Append(value);
        }

        public void WriteNewLine()
        {
            InternalDebug.Assert(!this.IsEnough);

            this.sb.Append('\r');

            if (!this.IsEnough)
            {
                this.sb.Append('\n');
            }
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }
    }
}

