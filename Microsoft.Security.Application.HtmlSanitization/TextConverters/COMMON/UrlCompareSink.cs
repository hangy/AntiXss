// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlCompareSink.cs" company="Microsoft Corporation">
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
    using System.IO;
    using System.Text;

    internal class UrlCompareSink : ITextSink
    {
        private string url;
        private int urlPosition;

        public UrlCompareSink()
        {
        }

        public void Initialize(string url)
        {
            this.url = url;
            this.urlPosition = 0;
        }

        public void Reset()
        {
            this.urlPosition = -1;
        }

        public bool IsActive { get { return this.urlPosition >= 0; } }
        public bool IsMatch { get { return this.urlPosition == this.url.Length; } }

        public bool IsEnough { get { return this.urlPosition < 0; } }

        public void Write(char[] buffer, int offset, int count)
        {
            if (this.IsActive)
            {
                int end = offset + count;

                while (offset < end)
                {
                    if (this.urlPosition == 0)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }
                    }
                    else if (this.urlPosition == this.url.Length)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }

                        this.urlPosition = -1;
                        break;
                    }

                    if (buffer[offset] != this.url[this.urlPosition])
                    {
                        this.urlPosition = -1;
                        break;
                    }

                    offset++;
                    this.urlPosition ++;
                }
            }
        }

        public void Write(int ucs32Char)
        {
            if (Token.LiteralLength(ucs32Char) != 1)
            {
                
                
                this.urlPosition = -1;
                return;
            }

            if (this.urlPosition == 0)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                {
                    
                    return;
                }
            }
            else if (this.urlPosition == this.url.Length)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                {
                    
                    return;
                }

                this.urlPosition = -1;
                return;
            }

            if ((char)ucs32Char != this.url[this.urlPosition])
            {
                this.urlPosition = -1;
                return;
            }

            this.urlPosition ++;
        }
    }
}

