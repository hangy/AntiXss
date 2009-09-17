// ***************************************************************
// <copyright file="TextOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;
    
    using Microsoft.Exchange.Data.Globalization;

    internal delegate bool ImageRenderingCallbackInternal(string attachmentUrl, int approximateRenderingPosition);

    

    internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
    {
        

        protected ConverterOutput output;

        
        protected bool lineWrapping;
        protected bool rfc2646;                                
        protected int longestNonWrappedParagraph;
        protected int wrapBeforePosition;
        protected bool preserveTrailingSpace;
        protected bool preserveTabulation;
        protected bool preserveNbsp;

        
        protected int lineLength;
        protected int lineLengthBeforeSoftWrap;
        protected int flushedLength;
        protected int tailSpace;
        protected int breakOpportunity;
        protected int nextBreakOpportunity;
        protected int quotingLevel;                           
        protected bool seenSpace;
        protected bool wrapped;
        protected char[] wrapBuffer;
        protected bool signaturePossible = true;
        protected bool anyNewlines;

        private bool fallbacks;
        private bool htmlEscape;

        protected bool endParagraph;

        private string anchorUrl;
        private int linePosition;
        private ImageRenderingCallbackInternal imageRenderingCallback;

        static readonly char[] Whitespaces = { ' ', '\t', '\r', '\n', '\f' };

        

        public TextOutput(
                ConverterOutput output,
                bool lineWrapping,
                bool flowed,
                int wrapBeforePosition,
                int longestNonWrappedParagraph,
                ImageRenderingCallbackInternal imageRenderingCallback,
                bool fallbacks,
                bool htmlEscape,
                bool preserveSpace,
                Stream testTraceStream)
        {

            this.rfc2646 = flowed;
            this.lineWrapping = lineWrapping;
            this.wrapBeforePosition = wrapBeforePosition;
            this.longestNonWrappedParagraph = longestNonWrappedParagraph;

            if (!this.lineWrapping)
            {
                this.preserveTrailingSpace = preserveSpace;
                this.preserveTabulation = preserveSpace;
                this.preserveNbsp = preserveSpace;
            }

            this.output = output;

            this.fallbacks = fallbacks;
            this.htmlEscape = htmlEscape;

            this.imageRenderingCallback = imageRenderingCallback;

            
            
            
            
            
            
            
            
            
            
            this.wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
        }

        
        private void Reinitialize()
        {
            this.anchorUrl = null;
            this.linePosition = 0;
            this.lineLength = 0;
            this.lineLengthBeforeSoftWrap = 0;
            this.flushedLength = 0;
            this.tailSpace = 0;
            this.breakOpportunity = 0;
            this.nextBreakOpportunity = 0;
            this.quotingLevel = 0;
            this.seenSpace = false;
            this.wrapped = false;
            this.signaturePossible = true;
            this.anyNewlines = false;
            this.endParagraph = false;

        }

        

        public bool OutputCodePageSameAsInput
        {
            get
            {
                if (this.output is ConverterEncodingOutput)
                {
                    return (this.output as ConverterEncodingOutput).CodePageSameAsInput;
                }

                return false;
            }
        }

        

        public Encoding OutputEncoding
        {
            set
            {
                if (this.output is ConverterEncodingOutput)
                {
                    (this.output as ConverterEncodingOutput).Encoding = value;
                    return;
                }

                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        

        public bool LineEmpty
        {
            get
            {
                return this.lineLength == 0 && this.tailSpace == 0;
            }
        }

        

        public bool ImageRenderingCallbackDefined
        {
            get
            {
                return this.imageRenderingCallback != null;
            }
        }

        

        public void OpenDocument()
        {

        }

        

        public void CloseDocument()
        {

            if (!this.anyNewlines)
            {
                
                this.output.Write("\r\n");
            }

            this.endParagraph = false;
        }

        

        public void SetQuotingLevel(int quotingLevel)
        {

            
            
            this.quotingLevel = Math.Min(quotingLevel, this.wrapBeforePosition / 2);
        }

        

        public void CloseParagraph()
        {

            if (this.lineLength != 0 || this.tailSpace != 0)
            {
                this.OutputNewLine();
            }

            this.endParagraph = true;
        }

        

        public void OutputNewLine()
        {

            

            

            if (this.lineWrapping)
            {
                this.FlushLine('\n');

                if (this.signaturePossible && this.lineLength == 2 && this.tailSpace == 1)
                {
                    
                    this.output.Write(' ');
                    this.lineLength++;
                }
            }
            else if (this.preserveTrailingSpace && this.tailSpace != 0)
            {
                this.FlushTailSpace();
            }

            if (!this.endParagraph)
            {
                this.output.Write("\r\n");
                this.anyNewlines = true;
                this.linePosition += 2;
            }

            this.linePosition += this.lineLength;

            this.lineLength = 0;
            this.lineLengthBeforeSoftWrap = 0;
            this.flushedLength = 0;
            this.tailSpace = 0;
            this.breakOpportunity = 0;
            this.nextBreakOpportunity = 0;
            this.wrapped = false;
            
            this.seenSpace = false;
            this.signaturePossible = true;
        }

        

        public void OutputTabulation(int count)
        {

            if (this.preserveTabulation)
            {
                while (count != 0)
                {
                    this.OutputNonspace("\t", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                int tabPosition = (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace) / 8 * 8 + 8 * count;
                count = tabPosition - (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace);

                this.OutputSpace(count);
            }
        }

        

        public void OutputSpace(int count)
        {

            

            InternalDebug.Assert(count != 0);

            if (this.lineWrapping)
            {
                if (this.breakOpportunity == 0 || this.lineLength + this.tailSpace <= this.WrapBeforePosition())
                {
                    this.breakOpportunity = this.lineLength + this.tailSpace;

                    InternalDebug.Assert(this.breakOpportunity >= 0);

                    if (this.lineLength + this.tailSpace < this.WrapBeforePosition() && count > 1)
                    {
                        this.breakOpportunity += Math.Min(this.WrapBeforePosition() - (this.lineLength + this.tailSpace), count - 1);
                    }

                    if (this.breakOpportunity < this.lineLength + this.tailSpace + count - 1)
                    {
                        
                        
                        this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
                    }

                    if (this.lineLength > this.flushedLength)
                    {
                        this.FlushLine(' ');
                    }
                }
                else
                {
                    
                    
                    
                    

                    this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
                }
            }

            this.tailSpace += count;

            InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
        }

        

        public void OutputNbsp(int count)
        {

            

            if (this.preserveNbsp)
            {
                while (count != 0)
                {
                    this.OutputNonspace("\xA0", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                this.tailSpace += count;
            }
        }

        

        public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
        {

            if (!this.lineWrapping && !this.endParagraph && textMapping == TextMapping.Unicode)
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                this.lineLength += count;
            }
            else
            {
                OutputNonspaceImpl(buffer, offset, count, textMapping);
            }
        }

        

        private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (count != 0)
            {
                if (textMapping != TextMapping.Unicode)
                {
                    
                    
                    
                    
                    for (int i = 0; i < count; i++)
                    {
                        this.MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
                    }
                    return;
                }

                if (this.endParagraph)
                {
                    InternalDebug.Assert(this.lineLength == 0);

                    this.output.Write("\r\n");
                    this.linePosition += 2;
                    this.anyNewlines = true;
                    this.endParagraph = false;
                }

                if (this.lineWrapping)
                {
                    
                    
                    
                    
                    
                    

                    this.WrapPrepareToAppendNonspace(count);

                    if (this.breakOpportunity == 0)
                    {
                        this.FlushLine(buffer[offset]);

                        this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                        this.flushedLength += count;
                    }
                    else
                    {
                        
                        

                        Buffer.BlockCopy(buffer, offset * 2, this.wrapBuffer, (this.lineLength - this.flushedLength) * 2, count * 2);
                    }

                    this.lineLength += count;

                    if (this.lineLength > 2 || buffer[offset] != '-' || (count == 2 && buffer[offset + 1] != '-'))
                    {
                        this.signaturePossible = false;
                    }
                }
                else
                {
                    if (this.tailSpace != 0)
                    {
                        this.FlushTailSpace();
                    }

                    this.output.Write(buffer, offset, count, this.fallbacks ? this : null);

                    this.lineLength += count;
                }
            }
        }

        

        public void OutputNonspace(string text, TextMapping textMapping)
        {
            this.OutputNonspace(text, 0, text.Length, textMapping);
        }

        

        public void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
        {

            if (textMapping != TextMapping.Unicode)
            {
                for (int i = offset; i < length; i++)
                {
                    this.MapAndOutputSymbolCharacter(text[i], textMapping);
                }
                return;
            }

            if (this.endParagraph)
            {
                InternalDebug.Assert(this.lineLength == 0);

                this.output.Write("\r\n");
                this.linePosition += 2;
                this.anyNewlines = true;
                this.endParagraph = false;
            }

            if (this.lineWrapping)
            {
                if (length != 0)
                {
                    
                    
                    
                    
                    
                    

                    this.WrapPrepareToAppendNonspace(length);

                    if (this.breakOpportunity == 0)
                    {
                        this.FlushLine(text[offset]);

                        this.output.Write(text, offset, length, this.fallbacks ? this : null);

                        this.flushedLength += length;
                    }
                    else
                    {
                        text.CopyTo(offset, this.wrapBuffer, this.lineLength - this.flushedLength, length);
                    }

                    this.lineLength += length;

                    if (this.lineLength > 2 || text[offset] != '-' || (length == 2 && text[offset + 1] != '-'))
                    {
                        this.signaturePossible = false;
                    }
                }
            }
            else
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(text, offset, length, this.fallbacks ? this : null);

                this.lineLength += length;
            }
        }

        

        public void OutputNonspace(int ucs32Literal, TextMapping textMapping)
        {

            if (textMapping != TextMapping.Unicode)
            {
                this.MapAndOutputSymbolCharacter((char)ucs32Literal, textMapping);
                return;
            }

            if (this.endParagraph)
            {
                InternalDebug.Assert(this.lineLength == 0);

                this.output.Write("\r\n");
                this.linePosition += 2;
                this.anyNewlines = true;
                this.endParagraph = false;
            }

            if (this.lineWrapping)
            {
                int count = Token.LiteralLength(ucs32Literal);

                
                
                
                
                
                

                this.WrapPrepareToAppendNonspace(count);

                if (this.breakOpportunity == 0)
                {
                    this.FlushLine(Token.LiteralFirstChar(ucs32Literal));

                    this.output.Write(ucs32Literal, this.fallbacks ? this : null);

                    this.flushedLength += count;
                }
                else
                {
                    this.wrapBuffer[this.lineLength - this.flushedLength] = Token.LiteralFirstChar(ucs32Literal);
                    if (count != 1)
                    {
                        this.wrapBuffer[this.lineLength - this.flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
                    }
                }

                this.lineLength += count;

                if (this.lineLength > 2 || count != 1 || (char)ucs32Literal != '-')
                {
                    this.signaturePossible = false;
                }
            }
            else
            {
                if (this.tailSpace != 0)
                {
                    this.FlushTailSpace();
                }

                this.output.Write(ucs32Literal, this.fallbacks ? this : null);

                this.lineLength += Token.LiteralLength(ucs32Literal);
            }
        }

        

        public void OpenAnchor(string anchorUrl)
        {
            

            this.anchorUrl = anchorUrl;
        }

        

        public void CloseAnchor()
        {
            
            

            if (this.anchorUrl != null)
            {
                bool addSpace = (this.tailSpace != 0);

                string urlString = this.anchorUrl;

                if (urlString.IndexOf(' ') != -1)
                {
                    urlString = urlString.Replace(" ", "%20");
                }

                this.OutputNonspace("<", TextMapping.Unicode);
                this.OutputNonspace(urlString, TextMapping.Unicode);
                this.OutputNonspace(">", TextMapping.Unicode);

                if (addSpace)
                {
                    this.OutputSpace(1);
                }

                this.anchorUrl = null;
            }
        }

        

        public void CancelAnchor()
        {

            this.anchorUrl = null;
        }

        

        public void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
        {
            
            

            if (this.imageRenderingCallback != null && this.imageRenderingCallback(imageUrl, this.RenderingPosition()))
            {
                
                this.OutputSpace(1);
            }
            else
            {
                if ((wdthPixels == 0 || wdthPixels >= 8) && (heightPixels == 0 || heightPixels >= 8))
                {
                    bool addSpace = (this.tailSpace != 0);

                    
                    this.OutputNonspace("[", TextMapping.Unicode);

                    if (!string.IsNullOrEmpty(imageAltText))
                    {
                        

                        int offset = 0;
                        while (offset != imageAltText.Length)
                        {
                            int nextOffset = imageAltText.IndexOfAny(Whitespaces, offset);

                            if (nextOffset == -1)
                            {
                                InternalDebug.Assert(imageAltText.Length - offset > 0);
                                this.OutputNonspace(imageAltText, offset, imageAltText.Length - offset, TextMapping.Unicode);
                                break;
                            }

                            if (nextOffset != offset)
                            {
                                this.OutputNonspace(imageAltText, offset, nextOffset - offset, TextMapping.Unicode);
                            }

                            if (imageAltText[offset] == '\t')
                            {
                                this.OutputTabulation(1);
                            }
                            else
                            {
                                
                                this.OutputSpace(1);
                            }

                            offset = nextOffset + 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(imageUrl))
                    {
                        
                        
                        
                        
                        if (imageUrl.Contains("/") &&
                            !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) )
                        {
                            imageUrl = "X";
                        }
                        else if (imageUrl.IndexOf(' ') != -1)
                        {
                            imageUrl = imageUrl.Replace(" ", "%20");
                        }

                        this.OutputNonspace(imageUrl, TextMapping.Unicode);
                    }
                    else
                    {
                        
                        this.OutputNonspace("X", TextMapping.Unicode);
                    }
                    this.OutputNonspace("]", TextMapping.Unicode);

                    if (addSpace)
                    {
                        this.OutputSpace(1);
                    }
                }
            }
        }

        

        public int RenderingPosition()
        {
            return this.linePosition + this.lineLength + this.tailSpace;
        }

        

        public void Flush()
        {
    if (this.lineWrapping)
            {
                if (this.lineLength != 0)
                {
                    this.FlushLine('\r');

                    this.OutputNewLine();
                }
            }
            else if (this.lineLength != 0)
            {
                this.OutputNewLine();
            }

            this.output.Flush();

        }

        

        private int WrapBeforePosition()
        {
            return this.wrapBeforePosition - (this.rfc2646 ? this.quotingLevel + 1 : 0);
        }

        

        private int LongestNonWrappedParagraph()
        {
            return this.longestNonWrappedParagraph - (this.rfc2646 ? this.quotingLevel + 1 : 0);
        }

        

        private void WrapPrepareToAppendNonspace(int count)
        {
            InternalDebug.Assert(this.lineWrapping);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity > this.breakOpportunity);

            while (this.breakOpportunity != 0 &&
                this.lineLength + this.tailSpace + count
                    > (this.wrapped ? this.WrapBeforePosition() : this.LongestNonWrappedParagraph()))
            {
                InternalDebug.Assert(this.breakOpportunity >= this.flushedLength);

                if (this.flushedLength == 0 && this.rfc2646)
                {
                    for (int i = 0; i < this.quotingLevel; i++)
                    {
                        this.output.Write('>');
                    }

                    if (this.quotingLevel != 0 || this.wrapBuffer[0] == '>' || this.wrapBuffer[0] == ' ')
                    {
                        this.output.Write(' ');
                    }
                }

                if (this.breakOpportunity >= this.lineLength)
                {
                    InternalDebug.Assert(this.tailSpace >= this.breakOpportunity + 1 - this.lineLength);
                    InternalDebug.Assert(this.flushedLength == this.lineLength);

                    do
                    {
                        
                        
                        
                        if (this.lineLength - this.flushedLength == this.wrapBuffer.Length)
                        {
                            this.output.Write(this.wrapBuffer, 0, this.wrapBuffer.Length, this.fallbacks ? this : null);
                            this.flushedLength += this.wrapBuffer.Length;
                        }

                        this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';

                        this.lineLength++;
                        this.tailSpace--;
                    }
                    while (this.lineLength != this.breakOpportunity + 1);
                }

                this.output.Write(this.wrapBuffer, 0, this.breakOpportunity + 1 - this.flushedLength, this.fallbacks ? this : null);

                this.anyNewlines = true;
                this.output.Write("\r\n");

                this.wrapped = true;
                this.lineLengthBeforeSoftWrap += this.breakOpportunity + 1;

                this.linePosition += this.breakOpportunity + 1 + 2;
                this.lineLength -= this.breakOpportunity + 1;

                InternalDebug.Assert(this.lineLength >= 0);

                int oldFlushedLength = this.flushedLength;
                this.flushedLength = 0;

                if (this.lineLength != 0)
                {
                    

                    if (this.nextBreakOpportunity == 0 ||
                        this.nextBreakOpportunity - (this.breakOpportunity + 1) >= this.lineLength ||
                        this.nextBreakOpportunity - (this.breakOpportunity + 1) == 0)
                    {
                        

                        if (this.rfc2646)
                        {
                            for (int i = 0; i < this.quotingLevel; i++)
                            {
                                this.output.Write('>');
                            }

                            if (this.quotingLevel != 0 || this.wrapBuffer[this.breakOpportunity + 1 - oldFlushedLength] == '>' || this.wrapBuffer[this.breakOpportunity + 1 - oldFlushedLength] == ' ')
                            {
                                this.output.Write(' ');
                            }
                        }

                        this.output.Write(this.wrapBuffer, this.breakOpportunity + 1 - oldFlushedLength, this.lineLength, this.fallbacks ? this : null);
                        this.flushedLength = this.lineLength;
                    }
                    else
                    {
                        

                        Buffer.BlockCopy(this.wrapBuffer, (this.breakOpportunity + 1 - oldFlushedLength) * 2, this.wrapBuffer, 0, this.lineLength * 2);
                    }
                }

                if (this.nextBreakOpportunity != 0)
                {
                    InternalDebug.Assert(this.nextBreakOpportunity > this.breakOpportunity);

                    this.breakOpportunity = this.nextBreakOpportunity - (this.breakOpportunity + 1);

                    InternalDebug.Assert(this.breakOpportunity >= 0);
                    InternalDebug.Assert(this.breakOpportunity < this.lineLength || this.tailSpace >= this.breakOpportunity + 1 - this.lineLength);

                    if (this.breakOpportunity > this.WrapBeforePosition())
                    {
                        
                        
                        
                        
                        
                        

                        if (this.lineLength < this.WrapBeforePosition())
                        {
                            InternalDebug.Assert(this.tailSpace != 0);

                            this.nextBreakOpportunity = this.breakOpportunity;
                            this.breakOpportunity = this.WrapBeforePosition();
                        }
                        else if (this.breakOpportunity > this.lineLength)
                        {
                            InternalDebug.Assert(this.tailSpace != 0);

                            this.nextBreakOpportunity = this.breakOpportunity;
                            this.breakOpportunity = this.lineLength;
                        }
                        else
                        {
                            this.nextBreakOpportunity = 0;
                        }

                        InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
                        InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
                    }
                    else
                    {
                        this.nextBreakOpportunity = 0;
                    }
                }
                else
                {
                    this.breakOpportunity = 0;
                }

                InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
                InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
            }

            if (this.tailSpace != 0)
            {
                if (this.breakOpportunity == 0)
                {
                    InternalDebug.Assert(this.lineLength == this.flushedLength);

                    if (this.flushedLength == 0 && this.rfc2646)
                    {
                        for (int i = 0; i < this.quotingLevel; i++)
                        {
                            this.output.Write('>');
                        }

                        
                        this.output.Write(' ');
                    }

                    this.flushedLength += this.tailSpace;
                    this.FlushTailSpace();
                }
                else
                {
                    InternalDebug.Assert(this.lineLength + this.tailSpace + count - this.flushedLength
                                                <= this.LongestNonWrappedParagraph());

                    do
                    {
                        this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';

                        this.lineLength++;
                        this.tailSpace--;
                    }
                    while (this.tailSpace != 0);
                }
            }

            InternalDebug.Assert(this.breakOpportunity == 0 || this.breakOpportunity < this.lineLength + this.tailSpace);
            InternalDebug.Assert(this.nextBreakOpportunity == 0 || this.nextBreakOpportunity < this.lineLength + this.tailSpace);
        }

        private void FlushLine(char nextChar)
        {
            if (this.flushedLength == 0 && this.rfc2646)
            {
                for (int i = 0; i < this.quotingLevel; i++)
                {
                    this.output.Write('>');
                }

                char firstChar = this.lineLength != 0 ? this.wrapBuffer[0] : nextChar;
                if (this.quotingLevel != 0 || firstChar == '>' || firstChar == ' ')
                {
                    this.output.Write(' ');
                }
            }

            if (this.lineLength != this.flushedLength)
            {
                this.output.Write(this.wrapBuffer, 0, this.lineLength - this.flushedLength, this.fallbacks ? this : null);
                this.flushedLength = this.lineLength;
            }
        }

        private void FlushTailSpace()
        {
            InternalDebug.Assert(this.tailSpace != 0);

            this.lineLength += this.tailSpace;
            do
            {
                this.output.Write(' ');
                this.tailSpace--;
            }
            while (this.tailSpace != 0);
        }

        

        private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
        {
            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                this.OutputNonspace((int)ch, TextMapping.Unicode);
                return;
            }

            string substitute = null;

            if (textMapping == TextMapping.Wingdings)
            {
                switch ((int)ch)
                {
                    case 74:	
                        substitute = "\x263A";
                        break;
                    case 75:	
                        substitute = ":|";
                        break;
                    case 76:	
                        substitute = "\x2639";
                        break;
                    case 216:	
                        substitute = ">";
                        break;
                    case 223:	
                        substitute = "<--";
                        break;
                    case 224:	
                        substitute = "-->";
                        break;
                    case 231:	
                        substitute = "<==";
                        break;
                    case 232:	
                        substitute = "==>";
                        break;
                    case 239:	
                        substitute = "<=";
                        break;
                    case 240:	
                        substitute = "=>";
                        break;
                    case 243:	
                        substitute = "<=>";
                        break;
                }
            }

            if (substitute == null)
            {
                substitute = "\x2022";  
            }

            this.OutputNonspace(substitute, TextMapping.Unicode);
        }

        

        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (this.htmlEscape)
            {
                
                unsafeAsciiMask = 0x01;
                return HtmlSupport.UnsafeAsciiMap;
            }

            unsafeAsciiMask = 0;
            return null;
        }

        

        bool IFallback.HasUnsafeUnicode()
        {
            return this.htmlEscape;
        }

        

        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return false;
        }

        
        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return this.htmlEscape &&
                ((byte)(ch & 0xFF) == (byte)'<' ||
                (byte)((ch >> 8) & 0xFF) == (byte)'<');
        }

        

        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (this.htmlEscape)
            {
                

                
                
                
                
                
                

                HtmlEntityIndex namedEntityId = 0;

                if (ch <= '>')
                {
                    if (ch == '>')
                    {
                        namedEntityId = HtmlEntityIndex.gt;
                    }
                    else if (ch == '<')
                    {
                        namedEntityId = HtmlEntityIndex.lt;
                    }
                    else if (ch == '&')
                    {
                        namedEntityId = HtmlEntityIndex.amp;
                    }
                    else if (ch == '\"')
                    {
                        namedEntityId = HtmlEntityIndex.quot;
                    }
                }
                else if ((char)0xA0 <= ch && ch <= (char)0xFF)
                {
                    namedEntityId = HtmlSupport.EntityMap[(int)ch - 0xA0];
                }

                if ((int)namedEntityId != 0)
                {
                    

                    string strQuote = HtmlNameData.entities[(int)namedEntityId].name;

                    if (outputEnd - outputBufferCount < strQuote.Length + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    strQuote.CopyTo(0, outputBuffer, outputBufferCount, strQuote.Length);
                    outputBufferCount += strQuote.Length;
                    outputBuffer[outputBufferCount++] = ';';
                }
                else
                {
                    

                    uint value = (uint)ch;
                    int len = (value < 0x10) ? 1 : (value < 0x100) ? 2 : (value < 0x1000) ? 3 : 4;
                    if (outputEnd - outputBufferCount < len + 4)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';
                    outputBuffer[outputBufferCount++] = 'x';

                    int offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        uint digit = value & 0xF;
                        outputBuffer[--offset] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ';';
                }
            }
            else
            {
                string substitute = AsciiEncoderFallback.GetCharacterFallback(ch);

                if (substitute != null)
                {
                    if (outputEnd - outputBufferCount < substitute.Length)
                    {
                        return false;
                    }

                    substitute.CopyTo(0, outputBuffer, outputBufferCount, substitute.Length);
                    outputBufferCount += substitute.Length;
                }
                else
                {
                    InternalDebug.Assert(outputEnd - outputBufferCount > 0);

                    
                    
                    outputBuffer[outputBufferCount++] = ch;
                }
            }

            return true;
        }

        

        void IDisposable.Dispose()
        {
            if (this.output != null /*&& this.output is IDisposable*/)
            {
                ((IDisposable)this.output).Dispose();
            }

            this.output = null;
            this.wrapBuffer = null;

            GC.SuppressFinalize(this);
        }

        

        bool IRestartable.CanRestart()
        {
            if (this.output is IRestartable)
            {
                return ((IRestartable)this.output).CanRestart();
            }

            return false;
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            ((IRestartable)this.output).Restart();

            this.Reinitialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (this.output is IRestartable)
            {
                ((IRestartable)this.output).DisableRestart();
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(this.output is IReusable);

            ((IReusable)this.output).Initialize(newSourceOrDestination);

            this.Reinitialize();
        }
    }
}

