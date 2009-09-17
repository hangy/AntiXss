// ***************************************************************
// <copyright file="ConverterUnicodeInput.cs" company="Microsoft">
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

    
    internal class ConverterUnicodeInput : ConverterInput, IReusable, IDisposable
    {
        private TextReader pullSource;
        private ConverterWriter pushSource;

        private char[] parseBuffer;
        private int parseStart;
        private int parseEnd;

        private char[] pushChunkBuffer;
        private int pushChunkStart;
        private int pushChunkCount;
        private int pushChunkUsed;

        
        public ConverterUnicodeInput(
            object source,
            bool push,
            int maxParseToken,
            bool testBoundaryConditions,
            IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            if (push)
            {
                InternalDebug.Assert(source is ConverterWriter);

                this.pushSource = source as ConverterWriter;
            }
            else
            {
                InternalDebug.Assert(source is TextReader);

                this.pullSource = source as TextReader;
            }

            this.maxTokenSize = maxParseToken;

            this.parseBuffer = new char[testBoundaryConditions ? 123 : 4096];

            if (this.pushSource != null)
            {
                this.pushSource.SetSink(this);
            }
        }

        
        private void Reinitialize()
        {
            this.parseStart = this.parseEnd = 0;
            this.pushChunkStart = 0;
            this.pushChunkCount = 0;
            this.pushChunkUsed = 0;
            this.pushChunkBuffer = null;
            this.endOfFile = false;
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (this.pullSource != null)
            {
                

                if (newSourceOrDestination != null)
                {
                    TextReader newSource = newSourceOrDestination as TextReader;

                    if (newSource == null)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new input should be a TextReader object");
                    }

                    this.pullSource = newSource;
                }
            }

            this.Reinitialize();
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == this.parseBuffer &&
                                start == this.parseStart &&
                                end <= this.parseEnd &&
                                start <= current));

            int charactersProduced = this.parseEnd - end;   
                                                            
                                                            

            if (this.parseBuffer.Length - this.parseEnd <= 1 && !this.EnsureFreeSpace() && charactersProduced == 0)
            {
                
                
                
                
                
                
                
                

                return true;
            }

            

            while (!this.endOfFile)
            {
                

                if (this.parseBuffer.Length - this.parseEnd <= 1)
                {
                    
                    
                    
                    

                    InternalDebug.Assert(charactersProduced != 0);

                    break;
                }

                
                

                if (this.pushSource != null)
                {
                    

                    if (this.pushChunkCount == 0)
                    {
                        InternalDebug.Assert(this.pushChunkUsed == 0);

                        if (!this.pushSource.GetInputChunk(out this.pushChunkBuffer, out this.pushChunkStart, out this.pushChunkCount, out this.endOfFile))
                        {
                            
                            

                            InternalDebug.Assert(0 == this.pushChunkCount);
                            break;
                        }

                        
                        InternalDebug.Assert((this.pushChunkCount != 0) != this.endOfFile);
                    }

                    InternalDebug.Assert(this.pushChunkCount - this.pushChunkUsed != 0 || this.endOfFile);

                    if (this.pushChunkCount - this.pushChunkUsed != 0)
                    {
                        int charactersToAppend = Math.Min(this.pushChunkCount - this.pushChunkUsed, this.parseBuffer.Length - this.parseEnd - 1);

                        Buffer.BlockCopy(this.pushChunkBuffer, (this.pushChunkStart + this.pushChunkUsed) * 2, this.parseBuffer, this.parseEnd * 2, charactersToAppend * 2);

                        this.pushChunkUsed += charactersToAppend;

                        this.parseEnd += charactersToAppend;

                        this.parseBuffer[this.parseEnd] = '\0';     

                        charactersProduced += charactersToAppend;

                        if (this.pushChunkCount - this.pushChunkUsed == 0)
                        {
                            
                            
                            

                            this.pushSource.ReportRead(this.pushChunkCount);

                            this.pushChunkStart = 0;
                            this.pushChunkCount = 0;
                            this.pushChunkUsed = 0;
                            this.pushChunkBuffer = null;
                        }
                    }
                }
                else
                {
                    

                    int readCharactersCount = this.pullSource.Read(this.parseBuffer, this.parseEnd, this.parseBuffer.Length - this.parseEnd - 1);

                    if (readCharactersCount == 0)
                    {
                        this.endOfFile = true;
                    }
                    else
                    {
                        this.parseEnd += readCharactersCount;

                        this.parseBuffer[this.parseEnd] = '\0';     

                        charactersProduced += readCharactersCount;
                    }

                    if (this.progressMonitor != null)
                    {
                        this.progressMonitor.ReportProgress();
                    }
                }
            }

            buffer = this.parseBuffer;

            if (start != this.parseStart)
            {
                current = this.parseStart + (current - start);
                start = this.parseStart;
            }

            end = this.parseEnd;

            return charactersProduced != 0 || this.endOfFile;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            InternalDebug.Assert(this.parseStart + processedSize <= this.parseEnd);

            this.parseStart += processedSize;
        }

        
        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            
            
            InternalDebug.Assert(gapEnd <= this.parseEnd);

            if (gapEnd == this.parseEnd)
            {
                
                this.parseEnd = gapBegin;
                this.parseBuffer[gapBegin] = '\0';
                return gapBegin;
            }

            
            
            

            Buffer.BlockCopy(this.parseBuffer, gapEnd, this.parseBuffer, gapBegin, this.parseEnd - gapEnd);
            this.parseEnd = gapBegin + (this.parseEnd - gapEnd);
            this.parseBuffer[this.parseEnd] = '\0';
            return this.parseEnd;
        }


        
        public void GetInputBuffer(out char[] inputBuffer, out int inputOffset, out int inputCount, out int parseCount)
        {
            InternalDebug.Assert(this.parseBuffer.Length - this.parseEnd >= 1);

            inputBuffer = this.parseBuffer;
            inputOffset = this.parseEnd;
            inputCount = this.parseBuffer.Length - this.parseEnd - 1;
            parseCount = this.parseEnd - this.parseStart;
        }

        
        public void Commit(int inputCount)
        {
            this.parseEnd += inputCount;
            this.parseBuffer[this.parseEnd] = '\0';
        }

        
        protected override void Dispose()
        {
            this.pullSource = null;
            this.pushSource = null;
            this.parseBuffer = null;
            this.pushChunkBuffer = null;

            base.Dispose();
        }

        
        private bool EnsureFreeSpace()
        {
            
            InternalDebug.Assert(this.parseBuffer.Length - this.parseEnd <= 1);

            

            if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) <= 1 ||
                (this.parseStart < 1 && 
                (long)this.parseBuffer.Length < (long)this.maxTokenSize + 1))
            {
                
                
                
                
                

                

                if ((long)this.parseBuffer.Length >= (long)this.maxTokenSize + 1)
                {
                    
                    return false;
                }

                

                long newSize = this.parseBuffer.Length * 2;

                if (newSize > (long)this.maxTokenSize + 1)
                {
                    newSize = (long)this.maxTokenSize + 1;
                }

                if (newSize > (long)Int32.MaxValue)
                {
                    
                    
                    
                    
                    
                    
                    
                    

                    newSize = (long)Int32.MaxValue;
                }

                char[] newBuffer = new char[(int)newSize];

                
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
    }
}

