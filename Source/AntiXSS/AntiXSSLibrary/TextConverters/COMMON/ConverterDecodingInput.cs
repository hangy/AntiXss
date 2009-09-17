// ***************************************************************
// <copyright file="ConverterDecodingInput.cs" company="Microsoft">
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
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    internal class ConverterDecodingInput : ConverterInput, IReusable
    {
        private IResultsFeedback resultFeedback;

        private Stream pullSource;
        private ConverterStream pushSource;
        private bool rawEndOfFile;

        private Encoding originalEncoding;
        private Encoding encoding;
        private Decoder decoder;
        private bool encodingChanged;

        private int minDecodeBytes;
        private int minDecodeChars;

        private char[] parseBuffer;
        private int parseStart;
        private int parseEnd;

        private int readFileOffset;
        private byte[] readBuffer;
        private int readCurrent;
        private int readEnd;

        private byte[] pushChunkBuffer;
        private int pushChunkStart;
        private int pushChunkCount;
        private int pushChunkUsed;

        private bool detectEncodingFromByteOrderMark;
        private byte[] preamble;

        private IRestartable restartConsumer;
        private int restartMax;
        private ByteCache restartCache;
        private bool restarting;

        
        public ConverterDecodingInput(
                    Stream source,
                    bool push,
                    System.Text.Encoding encoding,
                    bool detectEncodingFromByteOrderMark,
                    int maxParseToken,
                    int restartMax,
                    int inputBufferSize,
                    bool testBoundaryConditions,
                    IResultsFeedback resultFeedback,
                    IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.resultFeedback = resultFeedback;

            this.restartMax = restartMax;

            if (push)
            {
                InternalDebug.Assert(source is ConverterStream);

                this.pushSource = source as ConverterStream;
            }
            else
            {
                InternalDebug.Assert(source.CanRead);

                this.pullSource = source;
            }

            this.detectEncodingFromByteOrderMark = detectEncodingFromByteOrderMark;

            this.minDecodeBytes = testBoundaryConditions ? 1 : 64;

            this.originalEncoding = encoding;
            this.SetNewEncoding(encoding);

            
            
            
            InternalDebug.Assert(this.minDecodeBytes == 1 || this.minDecodeBytes >= Math.Max(4, this.preamble.Length));

            this.maxTokenSize = (maxParseToken == Int32.MaxValue) ? 
                        maxParseToken : 
                        testBoundaryConditions ? 
                            maxParseToken :
                            (maxParseToken + 1023) / 1024 * 1024;

            
            this.parseBuffer = new char[testBoundaryConditions ? 55 : Math.Min(4096, (long)this.maxTokenSize + (this.minDecodeChars + 1))];

            if (this.pushSource != null)
            {
                this.readBuffer = new byte[Math.Max(this.minDecodeBytes * 2, 8)];
            }
            else
            {
                int size = Math.Max(this.CalculateMaxBytes(this.parseBuffer.Length), inputBufferSize);

                this.readBuffer = new byte[size];
            }
        }

        
        private void Reinitialize()
        {
            this.parseStart = 0;
            this.parseEnd = 0;

            this.rawEndOfFile = false;

            this.SetNewEncoding(this.originalEncoding);

            this.encodingChanged = false;

            this.readFileOffset = 0;
            this.readCurrent = 0;
            this.readEnd = 0;

            this.pushChunkBuffer = null;
            this.pushChunkStart = 0;
            this.pushChunkCount = 0;
            this.pushChunkUsed = 0;

            if (this.restartCache != null)
            {
                this.restartCache.Reset();
            }
            this.restarting = false;

            this.endOfFile = false;
        }

        
        public Encoding Encoding
        {
            get { return this.encoding; }
        }

        
        public bool EncodingChanged
        {
            
            get { return this.encodingChanged; }
            set { InternalDebug.Assert(value == false); this.encodingChanged = false; }
        }

        
        public override void SetRestartConsumer(IRestartable restartConsumer)
        {
            if (this.restartMax != 0 || restartConsumer == null)
            {
                this.restartConsumer = restartConsumer;
            }
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == this.parseBuffer &&
                                start == this.parseStart &&
                                end == this.parseEnd &&
                                start <= current && current <= end));

            if (this.parseBuffer.Length - this.parseEnd <= this.minDecodeChars && !this.EnsureFreeSpace())
            {
                
                
                
                
                
                
                
                

                return true;
            }

            int charactersProduced = 0;

            

            while (!this.rawEndOfFile || this.readEnd - this.readCurrent != 0 || this.restarting)
            {
                

                if (this.parseBuffer.Length - this.parseEnd <= this.minDecodeChars)
                {
                    
                    
                    
                    

                    InternalDebug.Assert(charactersProduced != 0);

                    break;
                }

                
                

                if (this.readEnd - this.readCurrent >= 
                        (this.readFileOffset == 0 ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes) || 
                    (this.rawEndOfFile && !this.restarting))
                {
                    
                    
                    

                    
                    
                    InternalDebug.Assert(this.readEnd - this.readCurrent != 0);

                    charactersProduced += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
                }
                else
                {
                    
                    

                    if (this.restarting)
                    {
                        InternalDebug.Assert(this.readEnd - this.readCurrent == 0);

                        byte[] restartChunk;
                        int restartStart, restartStartSave;
                        int restartEnd;

                        if (!this.GetRestartChunk(out restartChunk, out restartStart, out restartEnd))
                        {
                            this.restarting = false;
                            continue;
                        }

                        restartStartSave = restartStart;

                        charactersProduced += this.DecodeFromBuffer(restartChunk, ref restartStart, restartEnd, this.readFileOffset, false);

                        this.readFileOffset += (restartStart - restartStartSave);

                        this.ReportRestartChunkUsed(restartStart - restartStartSave);
                    }
                    else if (this.pushSource != null)
                    {
                        

                        if (this.pushChunkCount == 0)
                        {
                            InternalDebug.Assert(this.pushChunkUsed == 0);

                            if (!this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.rawEndOfFile))
                            {
                                
                                

                                InternalDebug.Assert(0 == this.pushChunkCount);
                                break;
                            }

                            
                            InternalDebug.Assert((this.pushChunkCount != 0) != this.rawEndOfFile);
                        }
                        else if (this.pushChunkCount - this.pushChunkUsed == 0)
                        {
                            
                            

                            if (this.restartConsumer != null)
                            {
                                
                                this.BackupForRestart(this.pushChunkBuffer, this.pushChunkStart, this.pushChunkCount, this.readFileOffset, false);
                            }

                            this.pushSource.ReportRead(this.pushChunkCount);

                            this.readFileOffset += this.pushChunkCount;

                            this.pushChunkCount = 0;
                            this.pushChunkUsed = 0;

                            
                            
                            

                            
                            
                            InternalDebug.Assert(!this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.rawEndOfFile) && this.pushChunkCount == 0 && !this.rawEndOfFile);

                            break;
                        }

                        
                        

                        if (this.pushChunkCount - this.pushChunkUsed < (this.readFileOffset == 0 ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes))
                        {
                            

                            if (this.pushChunkCount - this.pushChunkUsed != 0)
                            {
                                
                                
                                InternalDebug.Assert(this.readEnd - this.readCurrent + (this.pushChunkCount - this.pushChunkUsed) <= this.readBuffer.Length);

                                

                                if (this.readBuffer.Length - this.readEnd < (this.pushChunkCount - this.pushChunkUsed))
                                {
                                    

                                    if (this.restartConsumer != null)
                                    {
                                        
                                        this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
                                    }

                                    Buffer.BlockCopy(this.readBuffer, this.readCurrent, this.readBuffer, 0, this.readEnd - this.readCurrent);

                                    this.readFileOffset += this.readCurrent;

                                    this.readEnd = this.readEnd - this.readCurrent;
                                    this.readCurrent = 0;
                                }

                                if (this.pushChunkUsed != 0)
                                {
                                    InternalDebug.Assert(this.readEnd == 0);

                                    if (this.restartConsumer != null)
                                    {
                                        

                                        this.BackupForRestart(this.pushChunkBuffer, this.pushChunkStart, this.pushChunkUsed, this.readFileOffset + this.readEnd, false);
                                    }

                                    this.readFileOffset += this.pushChunkUsed;
                                }

                                Buffer.BlockCopy(this.pushChunkBuffer, this.pushChunkStart + this.pushChunkUsed, this.readBuffer, this.readEnd, this.pushChunkCount - this.pushChunkUsed);
                                this.readEnd += this.pushChunkCount - this.pushChunkUsed;

                                

                                this.pushSource.ReportRead(this.pushChunkCount);

                                this.pushChunkCount = 0;
                                this.pushChunkUsed = 0;

                                if (this.readEnd - this.readCurrent < (this.readFileOffset == 0 ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes))
                                {
                                    
                                    break;
                                }
                            }

                            charactersProduced += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
                        }
                        else if (this.readEnd - this.readCurrent != 0)
                        {
                            

                            if (this.readFileOffset == 0 && this.readCurrent == 0)
                            {
                                
                                

                                InternalDebug.Assert(this.pushChunkUsed == 0);
                                InternalDebug.Assert(this.readEnd - this.readCurrent < Math.Max(4, this.minDecodeBytes));
                                InternalDebug.Assert(this.pushChunkCount - this.pushChunkUsed >= Math.Max(4, this.minDecodeBytes));

                                int bytesToAppend = Math.Max(4, this.minDecodeBytes) - (this.readEnd - this.readCurrent);

                                Buffer.BlockCopy(this.pushChunkBuffer, this.pushChunkStart + this.pushChunkUsed, this.readBuffer, this.readEnd, bytesToAppend);
                                this.readEnd += bytesToAppend;

                                
                                
                                

                                this.pushSource.ReportRead(bytesToAppend);

                                this.pushChunkCount -= bytesToAppend;
                                this.pushChunkStart += bytesToAppend;
                            }

                            

                            charactersProduced += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, false);
                        }

                        if (this.parseBuffer.Length - this.parseEnd > this.minDecodeChars && this.pushChunkCount - this.pushChunkUsed != 0 && this.readEnd - this.readCurrent == 0)
                        {
                            InternalDebug.Assert(!this.rawEndOfFile);

                            
                            
                            

                            if (this.readEnd != 0)
                            {
                                
                                

                                if (this.restartConsumer != null)
                                {
                                    this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
                                }

                                this.readFileOffset += this.readCurrent;

                                this.readEnd = 0;
                                this.readCurrent = 0;
                            }

                            int chunkUnusedStart = this.pushChunkStart + this.pushChunkUsed;

                            charactersProduced += this.DecodeFromBuffer(this.pushChunkBuffer, ref chunkUnusedStart, this.pushChunkStart + this.pushChunkCount, this.readFileOffset + this.pushChunkUsed, false);

                            

                            this.pushChunkUsed = chunkUnusedStart - this.pushChunkStart;
                        }
                    }
                    else
                    {
                        

                        
                        

                        if (this.readBuffer.Length - this.readEnd < this.minDecodeBytes)
                        {
                            

                            InternalDebug.Assert(this.readEnd - this.readCurrent < (this.readFileOffset == 0 ? Math.Max(4, this.minDecodeBytes) : this.minDecodeBytes));

                            if (this.restartConsumer != null)
                            {
                                
                                this.BackupForRestart(this.readBuffer, 0, this.readCurrent, this.readFileOffset, false);
                            }

                            Buffer.BlockCopy(this.readBuffer, this.readCurrent, this.readBuffer, 0, this.readEnd - this.readCurrent);

                            this.readFileOffset += this.readCurrent;

                            this.readEnd = this.readEnd - this.readCurrent;
                            this.readCurrent = 0;
                        }

                        int readCount = this.pullSource.Read(this.readBuffer, this.readEnd, this.readBuffer.Length - this.readEnd);

                        if (readCount == 0)
                        {
                            this.rawEndOfFile = true;
                        }
                        else
                        {
                            this.readEnd += readCount;
                            if (this.progressMonitor != null)
                            {
                                this.progressMonitor.ReportProgress();
                            }
                        }

                        charactersProduced += this.DecodeFromBuffer(this.readBuffer, ref this.readCurrent, this.readEnd, this.readFileOffset + this.readCurrent, this.rawEndOfFile);
                    }
                }
            }

            if (this.rawEndOfFile && this.readEnd - this.readCurrent == 0)
            {
                this.endOfFile = true;
            }

            if (buffer != this.parseBuffer)
            {
                buffer = this.parseBuffer;
            }

            if (start != this.parseStart)
            {
                current = this.parseStart + (current - start);
                start = this.parseStart;
            }

            end = this.parseEnd;

            return charactersProduced != 0 || this.endOfFile || this.encodingChanged;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            InternalDebug.Assert(this.parseStart + processedSize <= this.parseEnd);

            this.parseStart += processedSize;
        }

        
        
        
        
        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            
            
            InternalDebug.Assert(gapEnd == this.parseEnd);

            this.parseEnd = gapBegin;
            this.parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }

        
        public bool RestartWithNewEncoding(Encoding newEncoding)
        {
            if (this.encoding.CodePage == newEncoding.CodePage)
            {
                

                if (this.restartConsumer != null)
                {
                    this.restartConsumer.DisableRestart();
                    this.restartConsumer = null;

                    if (this.restartCache != null)
                    {
                        this.restartCache.Reset();
                        this.restartCache = null;
                    }
                }

                return false;
            }

            if (this.restartConsumer == null || !this.restartConsumer.CanRestart())
            {
                return false;
            }

            this.restartConsumer.Restart();

            
            

            this.SetNewEncoding(newEncoding);

            this.encodingChanged = true;

            
            
            
            

            if (this.readEnd != 0 && this.readFileOffset != 0)
            {
                

                this.BackupForRestart(this.readBuffer, 0, this.readEnd, this.readFileOffset, true);

                this.readEnd = 0;

                this.readFileOffset = 0;
            }
            else
            {
                
            }

            this.readCurrent = 0;
            this.pushChunkUsed = 0;

            
            
            this.restartConsumer = null;

            
            this.parseStart = this.parseEnd = 0;

            
            
            
            this.restarting = this.restartCache != null && this.restartCache.Length != 0;

            return true;
        }

        
        private void SetNewEncoding(Encoding newEncoding)
        {
            this.encoding = newEncoding;
            this.decoder = this.encoding.GetDecoder();

            
            this.preamble = this.encoding.GetPreamble();
            InternalDebug.Assert(this.preamble != null);

            this.minDecodeChars = this.GetMaxCharCount(this.minDecodeBytes);

            if (this.resultFeedback != null)
            {
                this.resultFeedback.Set(ConfigParameter.InputEncoding, newEncoding);
            }
        }


        
        protected override void Dispose()
        {
            if (this.restartCache != null && this.restartCache is IDisposable)
            {
                ((IDisposable)this.restartCache).Dispose();
            }

            this.restartCache = null;
            this.pullSource = null;
            this.pushSource = null;
            this.parseBuffer = null;
            this.readBuffer = null;
            this.pushChunkBuffer = null;
            this.preamble = null;
            this.restartConsumer = null;

            base.Dispose();
        }

        
        private int DecodeFromBuffer(byte[] buffer, ref int start, int end, int fileOffset, bool flush)
        {
            
            
            

            int preambleLength = 0;

            if (fileOffset == 0)
            {
                
                
                

                if (this.detectEncodingFromByteOrderMark)
                {
                    
                    this.DetectEncoding(buffer, start, end);
                }

                
                InternalDebug.Assert(this.preamble != null);

                

                if (this.preamble.Length != 0 && end - start >= this.preamble.Length)
                {
                    int i;

                    for (i = 0; i < this.preamble.Length; i++)
                    {
                        if (this.preamble[i] != buffer[start + i])
                        {
                            break;
                        }
                    }

                    if (i == this.preamble.Length)
                    {
                        
                        start += this.preamble.Length;
                        preambleLength = this.preamble.Length;

                        if (this.restartConsumer != null)
                        {
                            
                            

                            this.restartConsumer.DisableRestart();
                            this.restartConsumer = null;
                        }
                    }
                }

                
                this.encodingChanged = true;

                
                this.preamble = null;
            }

            int bytesToDecode = end - start;

            if (this.GetMaxCharCount(bytesToDecode) >= this.parseBuffer.Length - this.parseEnd)
            {
                bytesToDecode = this.CalculateMaxBytes(this.parseBuffer.Length - this.parseEnd - 1);

                InternalDebug.Assert(bytesToDecode < end - start);
            }

            int charsDecoded = this.decoder.GetChars(buffer, start, bytesToDecode, this.parseBuffer, this.parseEnd);

            InternalDebug.Assert(charsDecoded <= this.parseBuffer.Length - this.parseEnd - 1);

            this.parseEnd += charsDecoded;

            this.parseBuffer[this.parseEnd] = '\0';     
                                                        
                                                        
            start += bytesToDecode;

            return bytesToDecode + preambleLength;
        }

        
        private bool EnsureFreeSpace()
        {
            
            InternalDebug.Assert(this.parseBuffer.Length - this.parseEnd <= this.minDecodeChars);

            

            if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) < (this.minDecodeChars + 1) ||
                (this.parseStart < this.minDecodeChars &&
                (long)this.parseBuffer.Length < (long)this.maxTokenSize + (this.minDecodeChars + 1)))
            {
                
                
                
                
                

                

                if ((long)this.parseBuffer.Length >= (long)this.maxTokenSize + (this.minDecodeChars + 1))
                {
                    
                    return false;
                }

                

                long newSize = this.parseBuffer.Length * 2;

                if (newSize > (long)this.maxTokenSize + (this.minDecodeChars + 1))
                {
                    newSize = (long)this.maxTokenSize + (this.minDecodeChars + 1);
                }

                if (newSize > (long)Int32.MaxValue)
                {
                    
                    
                    
                    
                    
                    
                    
                    

                    newSize = (long)Int32.MaxValue;
                }

                if (newSize - (this.parseEnd - this.parseStart) < (this.minDecodeChars + 1))
                {
                    
                    
                    

                    return false;
                }

                char[] newBuffer;

                try
                {
                    newBuffer = new char[(int)newSize];
                }
                catch (OutOfMemoryException e)
                {
                    throw new TextConvertersException(Strings.TagTooLong, e);
                }

                
                Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, newBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);

                
                this.parseBuffer = newBuffer;

                this.parseEnd = (this.parseEnd - this.parseStart);
                this.parseStart = 0;
            }
            else
            {
                

                
                Buffer.BlockCopy(this.parseBuffer, this.parseStart * 2, this.parseBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);

                this.parseEnd = (this.parseEnd - this.parseStart);
                this.parseStart = 0;
            }

            return true;
        }        

        
        
        private int GetMaxCharCount(int byteCount)
        {
            if (this.encoding.CodePage == 65001)
            {
                InternalDebug.Assert(this.encoding.GetMaxCharCount(byteCount) == byteCount + 1, "when this assert fires, it means that we are back on Everett?");
                return byteCount + 1;
            }
            else if (this.encoding.CodePage == 54936)
            {
                InternalDebug.Assert(this.encoding.GetMaxCharCount(byteCount) == byteCount + 3, "when this assert fires, it means that we are back on Everett?");
                return byteCount + 3;
            }

            return this.encoding.GetMaxCharCount(byteCount);
        }

        
        private int CalculateMaxBytes(int charCount)
        {
            
            
            
            

            
            
            

            if (charCount == this.GetMaxCharCount(charCount))
            {
                
                return charCount;
            }

            if (charCount == this.GetMaxCharCount(charCount - 1))
            {
                
                return charCount - 1;
            }

            if (charCount == this.GetMaxCharCount(charCount - 3))
            {
                
                return charCount - 3;
            }

            

            int byteCountN = charCount - 4;
            int charCountN = this.GetMaxCharCount(byteCountN);

            
            
            

            int byteCount = (int)((float)byteCountN * (float)charCount / (float)charCountN);

            
            

            while (this.GetMaxCharCount(byteCount) < charCount)
            {
                byteCount ++;
            }

            do
            {
                byteCount --;
            }
            while (this.GetMaxCharCount(byteCount) > charCount);

            return byteCount;
        }

        
        private void DetectEncoding(byte[] buffer, int start, int end)
        {
            

            if (end - start < 2)
            {
                return;
            }

            Encoding newEncoding = null;

            if (buffer[start] == 0xFE && buffer[start + 1] == 0xFF)
            {
                
                newEncoding = Encoding.BigEndianUnicode;
            }
            else if (buffer[start] == 0xFF && buffer[start + 1] == 0xFE)
            {
                

                if (end - start >= 4 && 
                    buffer[start + 2] == 0 && 
                    buffer[start + 3] == 0) 
                {
                    newEncoding = Encoding.UTF32;
                }
                else 
                {
                    newEncoding = Encoding.Unicode;
                }
            }
            else if (end - start >= 3 && 
                    buffer[start] == 0xEF && 
                    buffer[start + 1] == 0xBB && 
                    buffer[start + 2] == 0xBF) 
            {
                
                newEncoding = Encoding.UTF8;
            }
            else if (end - start >= 4 && 
                    buffer[start] == 0 && 
                    buffer[start + 1] == 0 &&
                    buffer[start + 2] == 0xFE && 
                    buffer[start + 3] == 0xFF) 
            {
                
                newEncoding = new UTF32Encoding(true, true);
            }

            
            
            

            if (newEncoding != null)
            {
                this.encoding = newEncoding;
                this.decoder = this.encoding.GetDecoder();

                
                this.preamble = this.encoding.GetPreamble();

                this.minDecodeChars = this.GetMaxCharCount(this.minDecodeBytes);

                
                
                

                if (this.restartConsumer != null)
                {
                    
                    

                    this.restartConsumer.DisableRestart();
                    this.restartConsumer = null;
                }
            }
        }

        
        private void BackupForRestart(byte[] buffer, int offset, int count, int fileOffset, bool force)
        {
            InternalDebug.Assert(this.restartConsumer != null);

            if (!force && fileOffset > this.restartMax)
            {
                
                

                this.restartConsumer.DisableRestart();
                this.restartConsumer = null;

                this.preamble = null;
                return;
            }

            if (this.restartCache == null)
            {
                this.restartCache = new ByteCache();
            }

            byte[] cacheBuffer;
            int cacheOffset;

            this.restartCache.GetBuffer(count, out cacheBuffer, out cacheOffset);

            Buffer.BlockCopy(buffer, offset, cacheBuffer, cacheOffset, count);

            this.restartCache.Commit(count);
        }

        
        private bool GetRestartChunk(out byte[] restartChunk, out int restartStart, out int restartEnd)
        {
            InternalDebug.Assert(this.restartConsumer == null && this.restarting);

            if (this.restartCache.Length == 0)
            {
                restartChunk = null;
                restartStart = 0;
                restartEnd = 0;

                return false;
            }

            int outputCount;

            this.restartCache.GetData(out restartChunk, out restartStart, out outputCount);

            restartEnd = restartStart + outputCount;

            return true;
        }

        
        private void ReportRestartChunkUsed(int count)
        {
            InternalDebug.Assert(this.restartConsumer == null && this.restarting);
            InternalDebug.Assert(this.restartCache.Length >= count);

            this.restartCache.ReportRead(count);
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (this.pullSource != null)
            {
                

                if (newSourceOrDestination != null)
                {
                    Stream newSource = newSourceOrDestination as Stream;

                    if (newSource == null || !newSource.CanRead)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new input should be a readable Stream object");
                    }

                    this.pullSource = newSource;
                }
            }

            this.Reinitialize();
        }
    }
}

