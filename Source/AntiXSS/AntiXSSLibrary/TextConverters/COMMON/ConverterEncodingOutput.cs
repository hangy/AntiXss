// ***************************************************************
// <copyright file="ConverterEncodingOutput.cs" company="Microsoft">
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
    using Microsoft.Exchange.Data.Globalization;    

    
    internal class ConverterEncodingOutput : ConverterOutput, IByteSource, IRestartable, IReusable
    {
        protected IResultsFeedback resultFeedback;

        private const int LineSpaceThreshold = 256;
        private const int SpaceThreshold = 32;

        private Stream pushSink;
        private ConverterStream pullSink;
        private bool endOfFile;

        private bool restartablePushSink;
        private long restartPosition;

        private bool encodingSameAsInput;

        private bool restartable;
        private bool canRestart;
        private bool lineModeEncoding;

        private int minCharsEncode;

        private char[] lineBuffer;
        private int lineBufferCount;
        private int lineBufferLastNL;

        private ByteCache cache = new ByteCache();

        private Encoding originalEncoding;
        private Encoding encoding;
        private Encoder encoder;
        private bool encodingCompleteUnicode;

        private CodePageMap codePageMap = new CodePageMap();

        private bool isFirstChar = true;

        
        public ConverterEncodingOutput(
            Stream destination,
            bool push,
            bool restartable,
            System.Text.Encoding encoding,
            bool encodingSameAsInput,
            bool testBoundaryConditions,
            IResultsFeedback resultFeedback)
        {
            this.resultFeedback = resultFeedback;

            if (!push)
            {
                this.pullSink = destination as ConverterStream;
                InternalDebug.Assert(this.pullSink != null);

                this.pullSink.SetSource(this);
            }
            else
            {
                InternalDebug.Assert(destination.CanWrite);

                this.pushSink = destination;

                if (restartable && destination.CanSeek && destination.Position == destination.Length)
                {
                    this.restartablePushSink = true;
                    this.restartPosition = destination.Position;
                }
            }

            this.restartable = this.canRestart = restartable;

            this.lineBuffer = new char[4096];

            this.minCharsEncode = testBoundaryConditions ? 1 : 256;

            this.encodingSameAsInput = encodingSameAsInput;

            this.originalEncoding = encoding;
            this.ChangeEncoding(encoding);

            if (this.resultFeedback != null)
            {
                this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
            }
        }

        
        private void Reinitialize()
        {
            this.endOfFile = false;
            this.lineBufferCount = 0;
            this.lineBufferLastNL = 0;
            this.isFirstChar = true;

            this.cache.Reset();

            this.encoding = null;
            this.ChangeEncoding(this.originalEncoding);

            this.canRestart = this.restartable;
        }

        
        public Encoding Encoding
        {
            get { return this.encoding; }
            set
            {
                if (this.encoding != value)
                {
                    this.ChangeEncoding(value);

                    if (this.resultFeedback != null)
                    {
                        this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
                    }
                }
            }
        }

        
        public bool CodePageSameAsInput
        {
            get { return this.encodingSameAsInput; }
        }

        
        bool IRestartable.CanRestart()
        {
            return this.canRestart;
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(this.canRestart);

            if (this.pullSink == null && this.restartablePushSink)
            {
                
                this.pushSink.Position = this.restartPosition;
                this.pushSink.SetLength(this.restartPosition);
            }

            this.Reinitialize();

            this.canRestart = false;
        }

        
        void IRestartable.DisableRestart()
        {
            InternalDebug.Assert(this.canRestart);

            this.canRestart = false;

            this.FlushCached();
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            this.restartablePushSink = false;

            if (this.pushSink != null)
            {
                if (newSourceOrDestination != null)
                {
                    
                    Stream newSink = newSourceOrDestination as Stream;

                    if (newSink == null || !newSink.CanWrite)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new output should be a writable Stream object");
                    }

                    this.pushSink = newSink;

                    if (this.restartable && newSink.CanSeek && newSink.Position == newSink.Length)
                    {
                        this.restartablePushSink = true;
                        this.restartPosition = newSink.Position;
                    }
                }
            }

            this.Reinitialize();
        }

        
        public override bool CanAcceptMore
        {
            get
            {
                return this.canRestart || this.pullSink == null || this.cache.Length == 0;
            }
        }

        
        public override void Write(char[] buffer, int offset, int count, IFallback fallback)
        {
            if (fallback == null && !this.lineModeEncoding && this.lineBufferCount + count <= this.lineBuffer.Length - this.minCharsEncode)
            {
                if (count == 1)
                {
                    this.lineBuffer[this.lineBufferCount++] = buffer[offset];
                    return;
                }
                else if (count < 16)
                {
                    if (0 != (count & 8))
                    {
                        this.lineBuffer[this.lineBufferCount] = buffer[offset];
                        this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
                        this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
                        this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
                        this.lineBuffer[this.lineBufferCount + 4] = buffer[offset + 4];
                        this.lineBuffer[this.lineBufferCount + 5] = buffer[offset + 5];
                        this.lineBuffer[this.lineBufferCount + 6] = buffer[offset + 6];
                        this.lineBuffer[this.lineBufferCount + 7] = buffer[offset + 7];
                        this.lineBufferCount += 8;
                        offset += 8;
                    }

                    if (0 != (count & 4))
                    {
                        this.lineBuffer[this.lineBufferCount] = buffer[offset];
                        this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
                        this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
                        this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
                        this.lineBufferCount += 4;
                        offset += 4;
                    }

                    if (0 != (count & 2))
                    {
                        this.lineBuffer[this.lineBufferCount] = buffer[offset];
                        this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
                        this.lineBufferCount += 2;
                        offset += 2;
                    }

                    if (0 != (count & 1))
                    {
                        this.lineBuffer[this.lineBufferCount++] = buffer[offset];
                    }

                    return;
                }
            }

            WriteComplete(buffer, offset, count, fallback);
        }

        public void WriteComplete(char[] buffer, int offset, int count, IFallback fallback)
        {
            InternalDebug.Assert(!this.endOfFile);
            InternalDebug.Assert(this.encoding != null);

            if (fallback != null || this.lineModeEncoding)
            {
                byte unsafeAsciiMask = 0;
                byte[] unsafeAsciiMap = null;
                uint unsafeAsciiMapLength = 0;
                bool hasUnsafeUnicode = false;
                bool treatNonAsciiAsUnsafe = false;

                if (fallback != null)
                {
                    unsafeAsciiMap = fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
                    if (unsafeAsciiMap != null)
                    {
                        unsafeAsciiMapLength = (uint)unsafeAsciiMap.Length;
                    }

                    hasUnsafeUnicode = fallback.HasUnsafeUnicode();
                    treatNonAsciiAsUnsafe = fallback.TreatNonAsciiAsUnsafe(this.encoding.WebName);
                }

                while (0 != count)
                {
                    
                    

                    for (; 0 != count && this.lineBufferCount != this.lineBuffer.Length; count--, offset++)
                    {
                        char ch = buffer[offset];

                        if (fallback != null)
                        {
                            if (((uint)ch < unsafeAsciiMapLength && (unsafeAsciiMap[(int)ch] & unsafeAsciiMask) != 0) ||
                                (!this.encodingCompleteUnicode && (ch >= 0x7F || ch < ' ') && this.codePageMap.IsUnsafeExtendedCharacter(ch)) ||
                                (hasUnsafeUnicode && ch >= 0x7F && (treatNonAsciiAsUnsafe || fallback.IsUnsafeUnicode(ch, this.isFirstChar))))
                            {
                                if (!fallback.FallBackChar(ch, this.lineBuffer, ref this.lineBufferCount, this.lineBuffer.Length))
                                {
                                    
                                    

                                    break;
                                }
                                this.isFirstChar = false;
                                continue;
                            }
                        }

                        

                        this.lineBuffer[this.lineBufferCount++] = ch;
                        this.isFirstChar = false;

                        if (this.lineModeEncoding)
                        {
                            if (ch == '\n' || ch == '\r')
                            {
                                
                                this.lineBufferLastNL = this.lineBufferCount;
                            }
                            else if (this.lineBufferLastNL > this.lineBuffer.Length - LineSpaceThreshold)
                            {
                                count--;
                                offset++;

                                break;
                            }
                        }
                    }

                    

                    
                    

                    if (this.lineModeEncoding &&
                        (this.lineBufferLastNL > this.lineBuffer.Length - LineSpaceThreshold ||
                        (this.lineBufferCount > this.lineBuffer.Length - SpaceThreshold &&
                        this.lineBufferLastNL != 0)))
                    {
                        

                        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferLastNL, false);

                        this.lineBufferCount -= this.lineBufferLastNL;

                        if (this.lineBufferCount != 0)
                        {
                            

                            Buffer.BlockCopy(this.lineBuffer, this.lineBufferLastNL * 2, this.lineBuffer, 0, this.lineBufferCount * 2);
                        }
                    }
                    else if (this.lineBufferCount > this.lineBuffer.Length - Math.Max(this.minCharsEncode, SpaceThreshold))
                    {
                        

                        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);

                        this.lineBufferCount = 0;
                    }

                    this.lineBufferLastNL = 0;
                }
            }
            else
            {
                
                

                if (count > this.minCharsEncode)
                {
                    

                    if (this.lineBufferCount != 0)
                    {
                        
                        
                        
                        

                        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);

                        this.lineBufferCount = 0;
                        this.lineBufferLastNL = 0;
                    }

                    
                    

                    this.EncodeBuffer(buffer, offset, count, false);
                }
                else
                {
                    InternalDebug.Assert(this.lineBufferCount + count <= this.lineBuffer.Length);

                    
                    
                    
                    

                    Buffer.BlockCopy(buffer, offset * 2, this.lineBuffer, this.lineBufferCount * 2, count * 2);
                    this.lineBufferCount += count;

                    if (this.lineBufferCount > this.lineBuffer.Length - this.minCharsEncode)
                    {
                        
                        
                        
                        

                        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);

                        this.lineBufferCount = 0;
                        this.lineBufferLastNL = 0;
                    }
                }
            }
        }

        
        public override void Write(string text)
        {
            if (text.Length == 0)
            {
                return;
            }

            if (this.lineModeEncoding || this.lineBufferCount + text.Length > this.lineBuffer.Length - this.minCharsEncode)
            {
                
                this.Write(text, 0, text.Length);
                return;
            }

            

            if (text.Length <= 4)
            {
                
                

                int count = text.Length;

                this.lineBuffer[this.lineBufferCount++] = text[0];
                if (--count != 0)
                {
                    this.lineBuffer[this.lineBufferCount++] = text[1];
                    if (--count != 0)
                    {
                        this.lineBuffer[this.lineBufferCount++] = text[2];
                        if (--count != 0)
                        {
                            this.lineBuffer[this.lineBufferCount++] = text[3];
                        }
                    }
                }
            }
            else
            {
                text.CopyTo(0, this.lineBuffer, this.lineBufferCount, text.Length);
                this.lineBufferCount += text.Length;
            }
        }

        
        public override void Flush()
        {
            if (this.endOfFile)
            {
                return;
            }

            this.canRestart = false;

            this.FlushCached();

            

            this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);

            this.lineBufferCount = 0;
            this.lineBufferLastNL = 0;

            if (this.pullSink == null)
            {
                this.pushSink.Flush();
            }
            else
            {
                
                

                if (this.cache.Length == 0)
                {
                    this.pullSink.ReportEndOfFile();
                }
            }

            this.endOfFile = true;
        }

        
        bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength)
        {
            if (this.cache.Length == 0 || this.canRestart)
            {
                chunkBuffer = null;
                chunkOffset = 0;
                chunkLength = 0;
                return false;
            }

            this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
            return true;
        }

        
        void IByteSource.ReportOutput(int readCount)
        {
            InternalDebug.Assert(this.cache.Length >= readCount);

            this.cache.ReportRead(readCount);

            if (this.cache.Length == 0 && this.endOfFile)
            {
                this.pullSink.ReportEndOfFile();
            }
        }

        
        protected override void Dispose()
        {
            if (this.cache != null && this.cache is IDisposable)
            {
                ((IDisposable)this.cache).Dispose();
            }

            this.cache = null;
            this.pushSink = null;
            this.pullSink = null;
            this.lineBuffer = null;
            this.encoding = null;
            this.encoder = null;
            this.codePageMap = null;

            base.Dispose();
        }

        
        private void EncodeBuffer(char[] buffer, int offset, int count, bool flush)
        {
            int maxSpaceRequired = this.encoding.GetMaxByteCount(count);

            byte[] outputBuffer, directBuffer = null;
            int outputOffset, directOffset = 0;
            int outputCount, directSpace = 0;
            bool encodingToCache = true;

            
            

            if (this.canRestart || this.pullSink == null || this.cache.Length != 0)
            {
                

                this.cache.GetBuffer(maxSpaceRequired, out outputBuffer, out outputOffset);
            }
            else
            {
                

                this.pullSink.GetOutputBuffer(out directBuffer, out directOffset, out directSpace);

                if (directSpace >= maxSpaceRequired)
                {
                    

                    outputBuffer = directBuffer;
                    outputOffset = directOffset;

                    encodingToCache = false;
                }
                else
                {
                    this.cache.GetBuffer(maxSpaceRequired, out outputBuffer, out outputOffset);
                }
            }

            int encodedCount = this.encoder.GetBytes(buffer, offset, count, outputBuffer, outputOffset, flush);

            if (encodingToCache)
            {
                this.cache.Commit(encodedCount);

                if (this.pullSink == null)
                {
                    if (!this.canRestart || this.restartablePushSink)
                    {
                        

                        while (this.cache.Length != 0)
                        {
                            this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                            this.pushSink.Write(outputBuffer, outputOffset, outputCount);

                            this.cache.ReportRead(outputCount);

                            InternalDebug.Assert(outputCount > 0);
                        }
                    }
                }
                else
                {
                    if (!this.canRestart)
                    {
                        encodedCount = this.cache.Read(directBuffer, directOffset, directSpace);

                        this.pullSink.ReportOutput(encodedCount);
                    }
                }
            }
            else
            {
                this.pullSink.ReportOutput(encodedCount);
            }
        }

        
        internal void ChangeEncoding(Encoding newEncoding)
        {
            

            if (this.encoding != null)
            {
                

                this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);

                this.lineBufferCount = 0;
                this.lineBufferLastNL = 0;
            }

            this.encoding = newEncoding;
            this.encoder = newEncoding.GetEncoder();
            int encodingCodePage = newEncoding.CodePage;

            if (encodingCodePage == 1200 ||        
                encodingCodePage == 1201 ||        
                encodingCodePage == 12000 ||       
                encodingCodePage == 12001 ||       
                encodingCodePage == 65000 ||       
                encodingCodePage == 65001 ||       
                encodingCodePage == 65005 ||       
                encodingCodePage == 65006 ||       
                encodingCodePage == 54936)         
            {
                this.lineModeEncoding = false;
                this.encodingCompleteUnicode = true;
                this.codePageMap.ChoseCodePage(1200);
            }
            else
            {
                this.encodingCompleteUnicode = false;
                this.codePageMap.ChoseCodePage(encodingCodePage);

                if (encodingCodePage == 50220 ||       
                    encodingCodePage == 50221 ||       
                    encodingCodePage == 50222 ||       
                    encodingCodePage == 50225 ||       
                    encodingCodePage == 50227 ||       
                    encodingCodePage == 50229 ||       
                    encodingCodePage == 52936)         
                {
                    
                    
                    
                    

                    this.lineModeEncoding = true;
                }
            }
        }

        
        private bool FlushCached()
        {
            if (this.canRestart || this.cache.Length == 0)
            {
                return false;
            }

            

            byte[] outputBuffer;
            int outputOffset;
            int outputSpace;
            int outputCount;

            if (this.pullSink == null)
            {
                

                while (this.cache.Length != 0)
                {
                    this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                    this.pushSink.Write(outputBuffer, outputOffset, outputCount);

                    this.cache.ReportRead(outputCount);

                    InternalDebug.Assert(outputCount > 0);
                }
            }
            else
            {
                this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputSpace);

                outputCount = this.cache.Read(outputBuffer, outputOffset, outputSpace);

                this.pullSink.ReportOutput(outputCount);
            }

            return true;
        }
    }
}

