// ***************************************************************
// <copyright file="ConverterUnicodeOutput.cs" company="Microsoft">
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

    
    internal class ConverterUnicodeOutput : ConverterOutput, IRestartable, IReusable, IDisposable
    {
        private const int FallbackExpansionMax = 16;

        private TextWriter pushSink;
        private ConverterReader pullSink;

        private bool endOfFile;

        private bool restartable;
        private bool canRestart;
        private bool isFirstChar = true;

        private TextCache cache = new TextCache();

        
        public ConverterUnicodeOutput(object destination, bool push, bool restartable)
        {
            if (push)
            {
                this.pushSink = destination as TextWriter;
                InternalDebug.Assert(this.pushSink != null);
            }
            else
            {
                this.pullSink = destination as ConverterReader;
                InternalDebug.Assert(this.pullSink != null);

                this.pullSink.SetSource(this);
            }

            this.restartable = this.canRestart = restartable;
        }

        
        private void Reinitialize()
        {
            this.endOfFile = false;
            this.cache.Reset();
            this.canRestart = this.restartable;
            this.isFirstChar = true;
        }

        
        bool IRestartable.CanRestart()
        {
            return this.canRestart;
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(this.canRestart);

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
            if (this.pushSink != null)
            {
                if (newSourceOrDestination != null)
                {
                    
                    TextWriter newSink = newSourceOrDestination as TextWriter;

                    if (newSink == null)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new output should be a TextWriter object");
                    }

                    this.pushSink = newSink;
                }
            }

            this.Reinitialize();
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

            base.Dispose();
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
            InternalDebug.Assert(!this.endOfFile);

            byte unsafeAsciiMask = 0;
            byte[] unsafeAsciiMap = fallback == null ? null : fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
            bool hasUnsafeUnicode = fallback == null ? false : fallback.HasUnsafeUnicode();

            if (this.cache.Length != 0 || this.canRestart)
            {
                

                while (count != 0)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    if (fallback != null)
                    {
                        this.cache.GetBuffer(FallbackExpansionMax, out cacheBuffer, out cacheOffset, out cacheSpace);

                        int cacheOffsetStart = cacheOffset;

                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            char ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
                            {
                                int cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }
                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            this.isFirstChar = false;
                        }

                        
                        
                        

                        this.cache.Commit(cacheOffset - cacheOffsetStart);
                    }
                    else
                    {
                        int minSpace = Math.Min(count, 256);

                        this.cache.GetBuffer(minSpace, out cacheBuffer, out cacheOffset, out cacheSpace);

                        int countToCopy = Math.Min(cacheSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, cacheBuffer, cacheOffset * 2, countToCopy * 2);

                        this.isFirstChar = false;
                        this.cache.Commit(countToCopy);

                        offset += countToCopy;
                        count -= countToCopy;
                    }
                }
            }
            else if (this.pullSink != null)
            {
                

                char[] pullBuffer;
                int pullOffset;
                int pullSpace;

                this.pullSink.GetOutputBuffer(out pullBuffer, out pullOffset, out pullSpace);

                if (pullSpace != 0)
                {
                    if (fallback != null)
                    {
                        int pullStartOffset = pullOffset;

                        for (; 0 != count && 0 != pullSpace; count--, offset++)
                        {
                            char ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
                            {
                                int pullOffsetSave = pullOffset;

                                if (!fallback.FallBackChar(ch, pullBuffer, ref pullOffset, pullOffset + pullSpace))
                                {
                                    
                                    

                                    break;
                                }

                                pullSpace -= (pullOffset - pullOffsetSave);
                            }
                            else
                            {
                                

                                pullBuffer[pullOffset++] = ch;
                                pullSpace--;
                            }
                            this.isFirstChar = false;
                        }

                        this.pullSink.ReportOutput(pullOffset - pullStartOffset);
                    }
                    else
                    {
                        int countToCopy = Math.Min(pullSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, pullBuffer, pullOffset * 2, countToCopy * 2);

                        this.isFirstChar = false;
                        count -= countToCopy;
                        offset += countToCopy;

                        this.pullSink.ReportOutput(countToCopy);

                        pullOffset += countToCopy;
                        pullSpace -= countToCopy;
                    }
                }

                

                while (count != 0)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    if (fallback != null)
                    {
                        this.cache.GetBuffer(FallbackExpansionMax, out cacheBuffer, out cacheOffset, out cacheSpace);

                        int cacheOffsetStart = cacheOffset;

                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            char ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
                            {
                                int cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }

                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            this.isFirstChar = false;
                        }

                        
                        
                        

                        this.cache.Commit(cacheOffset - cacheOffsetStart);
                    }
                    else
                    {
                        int minSpace = Math.Min(count, 256);

                        this.cache.GetBuffer(minSpace, out cacheBuffer, out cacheOffset, out cacheSpace);

                        int countToCopy = Math.Min(cacheSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, cacheBuffer, cacheOffset * 2, countToCopy * 2);
                        this.isFirstChar = false;

                        this.cache.Commit(countToCopy);

                        offset += countToCopy;
                        count -= countToCopy;
                    }
                }

                

                while (pullSpace != 0 && this.cache.Length != 0)
                {
                    
                    
                    
                    

                    char[] outputBuffer;
                    int outputOffset;
                    int outputCount;

                    this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                    int countToCopy = Math.Min(outputCount, pullSpace);

                    Buffer.BlockCopy(outputBuffer, outputOffset * 2, pullBuffer, pullOffset * 2, countToCopy * 2);

                    this.cache.ReportRead(countToCopy);

                    this.pullSink.ReportOutput(countToCopy);

                    pullOffset += countToCopy;
                    pullSpace -= countToCopy;
                }
            }
            else
            {
                
                

                if (fallback != null)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    
                    
                    this.cache.GetBuffer(1024, out cacheBuffer, out cacheOffset, out cacheSpace);

                    int cacheOffsetStart = cacheOffset;
                    int cacheSpaceStart = cacheSpace;

                    while (count != 0)
                    {
                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            char ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
                            {
                                int cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }

                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            this.isFirstChar = false;
                        }

                        if (cacheOffset - cacheOffsetStart != 0)
                        {
                            this.pushSink.Write(cacheBuffer, cacheOffsetStart, cacheOffset - cacheOffsetStart);

                            cacheOffset = cacheOffsetStart;
                            cacheSpace = cacheSpaceStart;
                        }
                    }
                }
                else
                {
                    if (count != 0)
                    {
                        this.pushSink.Write(buffer, offset, count);
                        this.isFirstChar = false;
                    }
                }
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

        
        public bool GetOutputChunk(out char[] chunkBuffer, out int chunkOffset, out int chunkLength)
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

        
        public void ReportOutput(int readCount)
        {
            InternalDebug.Assert(this.cache.Length >= readCount);

            this.cache.ReportRead(readCount);

            if (this.cache.Length == 0 && this.endOfFile)
            {
                this.pullSink.ReportEndOfFile();
            }
        }

        
        private bool FlushCached()
        {
            if (this.canRestart || this.cache.Length == 0)
            {
                return false;
            }

            

            char[] outputBuffer;
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

                    InternalDebug.Assert(outputCount != 0);
                }
            }
            else
            {
                this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputSpace);

                outputCount = this.cache.Read(outputBuffer, outputOffset, outputSpace);

                this.pullSink.ReportOutput(outputCount);

                if (this.cache.Length == 0 && this.endOfFile)
                {
                    this.pullSink.ReportEndOfFile();
                }
            }

            return true;
        }

        
        private static bool IsUnsafeCharacter(
            char ch, 
            byte[] unsafeAsciiMap, 
            byte unsafeAsciiMask,
            bool hasUnsafeUnicode,
            bool isFirstChar,
            IFallback fallback)
        {
            if (unsafeAsciiMap == null)
            {
                return false;
            }

            bool result = ((ch >= unsafeAsciiMap.Length) ? false : (unsafeAsciiMap[(int)ch] & unsafeAsciiMask) != 0);
            result = result || (hasUnsafeUnicode && ch >= 0x7F && fallback.IsUnsafeUnicode(ch, isFirstChar));
            return result;
        }
    }
}

