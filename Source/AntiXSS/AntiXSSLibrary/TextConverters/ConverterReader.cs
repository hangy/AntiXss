// ***************************************************************
// <copyright file="ConverterReader.cs" company="Microsoft">
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
    using Microsoft.Exchange.Data.Internal;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    
    
    internal class ConverterReader : TextReader, IProgressMonitor
    {
        private ConverterUnicodeOutput sourceOutputObject;
        private IProducerConsumer producer;

        private bool madeProgress;
        private int maxLoopsWithoutProgress;

        private char[] writeBuffer;
        private int writeIndex;
        private int writeCount;

        private object source;

        private bool endOfFile;

        private bool inconsistentState;

        

        
        
        
        
        
        public ConverterReader(Stream sourceStream, TextConverter converter)
        {
            if (sourceStream == null)
            {
                throw new ArgumentNullException("sourceStream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (!sourceStream.CanRead)
            {
                throw new ArgumentException(Strings.CannotReadFromSource, "sourceStream");
            }

            this.producer = converter.CreatePullChain(sourceStream, this);

            
            this.source = sourceStream;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        public ConverterReader(TextReader sourceReader, TextConverter converter)
        {
            if (sourceReader == null)
            {
                throw new ArgumentNullException("sourceReader");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            this.producer = converter.CreatePullChain(sourceReader, this);

            
            this.source = sourceReader;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        

        
        
        
        
        public override int Peek()
        {
            if (this.source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            long loopsWithoutProgress = 0;

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            
            
            this.inconsistentState = true;

            while (!this.endOfFile)
            {
                

                if (this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    
                    this.inconsistentState = false;

                    return (int)(ushort)chunkBuffer[chunkIndex];
                }

                this.producer.Run();

                if (this.madeProgress)
                {
                    this.madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            
            this.inconsistentState = false;

            return -1;
        }
        
        
        
        
        
        public override int Read()
        {
            if (this.source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            long loopsWithoutProgress = 0;

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            
            
            this.inconsistentState = true;

            while (!this.endOfFile)
            {
                

                if (this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    this.sourceOutputObject.ReportOutput(1);

                    
                    this.inconsistentState = false;

                    return (int)(ushort)chunkBuffer[chunkIndex];
                }

                this.producer.Run();

                if (this.madeProgress)
                {
                    this.madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            
            this.inconsistentState = false;

            return -1;
        }

        
        
        
        
        
        
        
        public override int Read(char[] buffer, int index, int count)
        {
            if (this.source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index", Strings.IndexOutOfRange);
            }

            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            int initialCount = count;

            
            

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            while (count != 0 && this.sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
            {
                int charsRead = Math.Min(chunkCount, count);

                Buffer.BlockCopy(chunkBuffer, chunkIndex * 2, buffer, index * 2, charsRead * 2);

                index += charsRead;
                count -= charsRead;

                this.sourceOutputObject.ReportOutput(charsRead);
            }

            
            

            if (0 != count)
            {
                

                
                

                long loopsWithoutProgress = 0;

                this.writeBuffer = buffer;
                this.writeIndex = index;
                this.writeCount = count;

                
                
                this.inconsistentState = true;

                while (0 != this.writeCount && !this.endOfFile)
                {
                    

                    this.producer.Run();

                    if (this.madeProgress)
                    {
                        this.madeProgress = false;
                        loopsWithoutProgress = 0;
                    }
                    else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                    }
                }

                count = this.writeCount;

                this.writeBuffer = null;
                this.writeIndex = 0;
                this.writeCount = 0;

                
                this.inconsistentState = false;
            }

            return initialCount - count;
        }

        internal void SetSource(ConverterUnicodeOutput sourceOutputObject)
        {
            this.sourceOutputObject = sourceOutputObject;
        }

        internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
        {
            InternalDebug.Assert(!this.endOfFile);

            outputBuffer = this.writeBuffer;
            outputIndex = this.writeIndex;
            outputCount = this.writeCount;
        }

        internal void ReportOutput(int outputCount)
        {
            InternalDebug.Assert(!this.endOfFile && outputCount <= this.writeCount);

            if (outputCount != 0)
            {
                this.writeCount -= outputCount;
                this.writeIndex += outputCount;

                this.madeProgress = true;
            }
        }

        internal void ReportEndOfFile()
        {
            this.endOfFile = true;
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.source != null)
                {
                    if (this.source is Stream)
                    {
                        ((Stream)this.source).Close();
                    }
                    else
                    {
                        ((TextReader)this.source).Close();
                    }
                }
            }

            if (this.producer != null && this.producer is IDisposable)
            {
                ((IDisposable)this.producer).Dispose();
            }

            this.source = null;
            this.producer = null;
            this.sourceOutputObject = null;
            this.writeBuffer = null;

            base.Dispose(disposing);
        }

        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        
        internal void Reuse(object newSource)
        {
            if (!(this.producer is IReusable))
            {
                throw new NotSupportedException("this converter is not reusable");
            }

            ((IReusable)this.producer).Initialize(newSource);

            this.source = newSource;

            this.writeBuffer = null;
            this.writeIndex = 0;
            this.writeCount = 0;

            this.endOfFile = false;
            this.inconsistentState = false;
        }
    }
}
