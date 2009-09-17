// ***************************************************************
// <copyright file="ConverterBufferInput.cs" company="Microsoft">
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

    
    internal class ConverterBufferInput : ConverterInput, ITextSink, IDisposable
    {
        private const int DefaultMaxLength = 32 * 1024;
        private int maxLength;
        private string originalFragment;
        private char[] parseBuffer;

        
        public ConverterBufferInput(IProgressMonitor progressMonitor) :
            this(DefaultMaxLength, progressMonitor)
        {
        }

        
        public ConverterBufferInput(int maxLength, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            
            

            this.maxLength = maxLength;
        }

        
        public ConverterBufferInput(string fragment, IProgressMonitor progressMonitor) :
            this(DefaultMaxLength, fragment, progressMonitor)
        {
        }

        
        public ConverterBufferInput(int maxLength, string fragment, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.maxLength = maxLength;

            this.originalFragment = fragment;

            this.parseBuffer = new char[fragment.Length + 1];
            fragment.CopyTo(0, this.parseBuffer, 0, fragment.Length);
            this.parseBuffer[fragment.Length] = '\0';
            this.maxTokenSize = fragment.Length;
        }

#if PRIVATEBUILD
        
        public ConverterBufferInput(char[] fragment, int offset, int count, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.parseBuffer = new char[count + 1];
            Buffer.BlockCopy(fragment, offset * 2, this.parseBuffer, 0, count * 2);
            this.parseBuffer[count] = '\0';
            this.maxTokenSize = count;
        }
#endif

        
        public bool IsEnough
        {
            get { return (this.maxTokenSize >= this.maxLength); }
        }

        
        public bool IsEmpty
        {
            get { return (this.maxTokenSize == 0); }
        }

        
        public void Write(string str)
        {
            int count = this.PrepareToBuffer(str.Length);

            if (count > 0)
            {
                str.CopyTo(0, this.parseBuffer, this.maxTokenSize, count);

                this.maxTokenSize += count;
                this.parseBuffer[this.maxTokenSize] = '\0';
            }
        }

        
        public void Write(char[] buffer, int offset, int count)
        {
            count = this.PrepareToBuffer(count);

            if (count > 0)
            {
                Buffer.BlockCopy(buffer, offset * 2, this.parseBuffer, this.maxTokenSize * 2, count * 2);

                this.maxTokenSize += count;
                this.parseBuffer[this.maxTokenSize] = '\0';
            }
        }

        
        public void Write(int ucs32Char)
        {
            int count;

            if (ucs32Char > 0xFFFF)
            {
                count = this.PrepareToBuffer(2);
                if (count > 0)
                {
                    
                    
                    this.parseBuffer[this.maxTokenSize] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Char);
                    this.parseBuffer[this.maxTokenSize + 1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Char);
                    this.maxTokenSize += count;
                    this.parseBuffer[this.maxTokenSize] = '\0';
                }
            }
            else
            {
                count = this.PrepareToBuffer(1);
                if (count > 0)
                {
                    
                    this.parseBuffer[this.maxTokenSize++] = (char)ucs32Char;
                    this.parseBuffer[this.maxTokenSize] = '\0';
                }
            }
        }

        
        public void Reset()
        {
            this.maxTokenSize = 0;
            this.endOfFile = false;
        }

        
        public void Initialize(string fragment)
        {
            if (this.originalFragment != fragment)
            {
                this.originalFragment = fragment;

                this.parseBuffer = new char[fragment.Length + 1];
                fragment.CopyTo(0, this.parseBuffer, 0, fragment.Length);
                this.parseBuffer[fragment.Length] = '\0';
                this.maxTokenSize = fragment.Length;
            }

            this.endOfFile = false;
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == this.parseBuffer &&
                                
                                end <= this.maxTokenSize &&
                                start <= current &&
                                current <= end));

            if (buffer == null)
            {
                buffer = this.parseBuffer;
                start = 0;
                end = this.maxTokenSize;
                current = 0;

                if (end != 0)
                {
                    return true;
                }
            }

            this.endOfFile = true;
            return true;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            this.progressMonitor.ReportProgress();
        }

        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            

            
            
            

            this.parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }


        
        protected override void Dispose()
        {
            this.parseBuffer = null;
            base.Dispose();
        }

        
        private int PrepareToBuffer(int count)
        {
            if (this.maxTokenSize + count > this.maxLength)
            {
                count = this.maxLength - this.maxTokenSize;
            }

            if (count > 0)
            {
                if (null == this.parseBuffer)
                {
                    InternalDebug.Assert(this.maxTokenSize == 0);
                    this.parseBuffer = new char[count + 1];
                }
                else if (this.parseBuffer.Length <= this.maxTokenSize + count)
                {
                    char[] oldBuffer = this.parseBuffer;

                    

                    int newLength = (this.maxTokenSize + count) * 2;
                    if (newLength > this.maxLength)
                    {
                        newLength = this.maxLength;
                    }
                    
                    this.parseBuffer = new char[newLength + 1];

                    if (this.maxTokenSize > 0)
                    {
                        Buffer.BlockCopy(oldBuffer, 0, this.parseBuffer, 0, this.maxTokenSize * 2);
                    }
                }
            }

            return count;
        }
    }
}

