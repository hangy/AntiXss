// ***************************************************************
// <copyright file="ConverterOutput.cs" company="Microsoft">
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

    
    
    internal interface IFallback
    {
        byte[] GetUnsafeAsciiMap(out byte unsafeAsciiMask);

        bool HasUnsafeUnicode();
        bool TreatNonAsciiAsUnsafe(string charset);
        bool IsUnsafeUnicode(char ch, bool isFirstChar);

        bool FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int lineBufferEnd);
    }

    
    
    internal abstract class ConverterOutput : ITextSink, IDisposable
    {
        
        
        protected char[] stringBuffer;

        protected const int stringBufferMax = 128; 
        protected const int stringBufferReserve = 20;
        protected const int stringBufferThreshold = stringBufferMax - stringBufferReserve;

        
        private IFallback fallback;

        
        public ConverterOutput()
        {
            this.stringBuffer = new char[stringBufferMax];
        }

        public abstract bool CanAcceptMore { get; }

        
        
        
        
        
        
        
        public abstract void Write(char[] buffer, int offset, int count, IFallback fallback);

        
        
        public abstract void Flush();

        
        
        
        
        
        
        public void Write(char[] buffer, int offset, int count)
        {
            this.Write(buffer, offset, count, null);
        }

        
        
        
        
        public virtual void Write(string text)
        {
            
            this.Write(text, 0, text.Length, null);
        }

        
        
        
        
        
        public void Write(string text, IFallback fallback)
        {
            this.Write(text, 0, text.Length, fallback);
        }

        
        
        
        
        
        
        public void Write(string text, int offset, int count)
        {
            this.Write(text, offset, count, null);
        }

        
        
        
        
        
        
        
        public void Write(string text, int offset, int count, IFallback fallback)
        {
            

            
            
            
            
            
            
            
            
            
            

            if (this.stringBuffer.Length < count)
            {
                
                this.stringBuffer = new char[count * 2];
            }

            text.CopyTo(offset, this.stringBuffer, 0, count);

            this.Write(this.stringBuffer, 0, count, fallback);
        }

        
        
        
        
        public void Write(char ch)
        {
            this.Write(ch, null);
        }

        
        
        
        
        
        public void Write(char ch, IFallback fallback)
        {
            this.stringBuffer[0] = ch;
            this.Write(this.stringBuffer, 0, 1, fallback);
        }

        
        
        
        
        public void Write(int ucs32Literal)
        {
            this.Write(ucs32Literal, null);
        }

        
        
        
        
        
        public void Write(int ucs32Literal, IFallback fallback)
        {
            if (ucs32Literal > 0xFFFF)
            {
                this.stringBuffer[0] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Literal);
                this.stringBuffer[1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Literal);
            }
            else
            {
                this.stringBuffer[0] = (char)ucs32Literal;
            }

            this.Write(this.stringBuffer, 0, ucs32Literal > 0xFFFF ? 2 : 1, fallback);
        }

        
        public ITextSink PrepareSink(IFallback fallback)
        {
            this.fallback = fallback;
            return this as ITextSink;
        }

        
        bool ITextSink.IsEnough { get { return false; } }

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            this.Write(buffer, offset, count, this.fallback);
        }

        void ITextSink.Write(int ucs32Literal)
        {
            this.Write(ucs32Literal, this.fallback);
        }

        
        
        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose()
        {
        }
    }
}
