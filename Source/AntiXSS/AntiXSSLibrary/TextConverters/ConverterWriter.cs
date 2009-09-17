// ***************************************************************
// <copyright file="ConverterWriter.cs" company="Microsoft">
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
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    
    
    
    internal class ConverterWriter : TextWriter, IProgressMonitor
    {
        private ConverterUnicodeInput sinkInputObject;
        private IProducerConsumer consumer;

        private bool madeProgress;
        private int maxLoopsWithoutProgress;

        private char[] chunkToReadBuffer;
        private int chunkToReadIndex;
        private int chunkToReadCount;

        private object destination;

        private bool endOfFile;

        private bool inconsistentState;

        private bool boundaryTesting;

        
        
        
        
        
        public ConverterWriter(Stream destinationStream, TextConverter converter)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (!destinationStream.CanWrite)
            {
                throw new ArgumentException(Strings.CannotWriteToDestination, "destinationStream");
            }

            this.consumer = converter.CreatePushChain(this, destinationStream);

            
            this.destination = destinationStream;

            this.boundaryTesting = converter.TestBoundaryConditions;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        

        
        
        
        
        
        public ConverterWriter(TextWriter destinationWriter, TextConverter converter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            this.consumer = converter.CreatePushChain(this, destinationWriter);

            
            this.destination = destinationWriter;

            this.boundaryTesting = converter.TestBoundaryConditions;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        public override Encoding Encoding
        {
            
            get { return null; }
        }

        
        
        
        
        public override void Flush() 
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            this.endOfFile = true;

            if (!this.inconsistentState)
            {
                long loopsWithoutProgress = 0;

                
                
                this.inconsistentState = true;

                while (!this.consumer.Flush())
                {
                    if (this.madeProgress)
                    {
                        
                        loopsWithoutProgress = 0;
                        this.madeProgress = false;
                    }
                    else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToFlushConverter);
                    }
                }

                
                this.inconsistentState = false;
            }

            if (this.destination is Stream)
            {
                ((Stream)this.destination).Flush();
            }
            else
            {
                ((TextWriter)this.destination).Flush();
            }
        }

        
        
        
        
        
        public override void Write(char value)
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            

            int parseCount = 10000;

            if (!this.boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= 1)
                {
                    inputBuffer[inputIndex] = value;
                    this.sinkInputObject.Commit(1);
                    return;
                }
            }

            

            char[] buffer = new char[]
            {
                value
            };

            this.WriteBig(buffer, 0, 1, parseCount);
        }

        
        
        
        
        
        public override void Write(char[] buffer)
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            
            
            if (buffer == null)
            {
                return;
            }

            

            int parseCount = 10000;

            if (!this.boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= buffer.Length)
                {
                    Buffer.BlockCopy(buffer, 0, inputBuffer, inputIndex * 2, buffer.Length * 2);
                    this.sinkInputObject.Commit(buffer.Length);
                    return;
                }
            }

            

            this.WriteBig(buffer, 0, buffer.Length, parseCount);
        }

        
        
        
        
        
        
        
        public override void Write(char[] buffer, int index, int count)
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
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
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            

            int parseCount = 10000;

            if (!this.boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= count)
                {
                    Buffer.BlockCopy(buffer, index * 2, inputBuffer, inputIndex * 2, count * 2);
                    this.sinkInputObject.Commit(count);
                    return;
                }
            }

            

            this.WriteBig(buffer, index, count, parseCount);
        }

        
        
        
        
        
        public override void Write(string value)
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            if (value == null)
            {
                return;
            }

            

            int parseCount = 10000;

            if (!this.boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                this.sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= value.Length)
                {
                    value.CopyTo(0, inputBuffer, inputIndex, value.Length);
                    this.sinkInputObject.Commit(value.Length);
                    return;
                }
            }

            

            char[] buffer = value.ToCharArray();

            this.WriteBig(buffer, 0, value.Length, parseCount);
        }

        
        
        
        
        
        public override void WriteLine(string value)
        {
            
            

            this.Write(value);
            this.WriteLine();
        }

        
        
        
        
        
        internal void SetSink(ConverterUnicodeInput sinkInputObject)
        {
            this.sinkInputObject = sinkInputObject;
        }

        
        
        
        
        
        
        
        
        
        internal bool GetInputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount, out bool eof)
        {
            chunkBuffer = this.chunkToReadBuffer;
            chunkIndex = this.chunkToReadIndex;
            chunkCount = this.chunkToReadCount;

            eof = this.endOfFile && 0 == this.chunkToReadCount;

            return 0 != this.chunkToReadCount || this.endOfFile;
        }

        
        
        
        
        
        internal void ReportRead(int readCount)
        {
            InternalDebug.Assert(readCount <= this.chunkToReadCount);

            if (readCount != 0)
            {
                this.chunkToReadCount -= readCount;
                this.chunkToReadIndex += readCount;

                if (this.chunkToReadCount == 0)
                {
                    this.chunkToReadBuffer = null;
                    this.chunkToReadIndex = 0;
                }

                this.madeProgress = true;
            }
        }

        
        
        
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.destination != null)
                {
                    if (!this.inconsistentState)
                    {
                        this.Flush();
                    }

                    if (this.destination is Stream)
                    {
                        ((Stream)this.destination).Close();
                    }
                    else
                    {
                        ((TextWriter)this.destination).Close();
                    }
                }
            }

            if (this.consumer != null && this.consumer is IDisposable)
            {
                ((IDisposable)this.consumer).Dispose();
            }

            this.destination = null;
            this.consumer = null;
            this.sinkInputObject = null;
            this.chunkToReadBuffer = null;

            base.Dispose(disposing);
        }

        
        
        
        
        
        
        
        
        private void WriteBig(char[] buffer, int index, int count, int parseCount)
        {
            this.chunkToReadBuffer = buffer;
            this.chunkToReadIndex = index;
            this.chunkToReadCount = count;

            
            
            
            
            
            
            
            long loopsWithoutProgress = 0;

            
            
            this.inconsistentState = true;

            while (0 != this.chunkToReadCount)
            {
                
                this.consumer.Run();

                if (this.madeProgress)
                {
                    this.madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProcessInput);
                }
            }

            
            this.inconsistentState = false;
        }

        
        
        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        
        internal void Reuse(object newSink)
        {
            if (!(this.consumer is IReusable))
            {
                throw new NotSupportedException("this converter is not reusable");
            }

            ((IReusable)this.consumer).Initialize(newSink);

            this.destination = newSink;

            this.chunkToReadBuffer = null;
            this.chunkToReadIndex = 0;
            this.chunkToReadCount = 0;

            this.endOfFile = false;
            this.inconsistentState = false;
        }
    }
}
