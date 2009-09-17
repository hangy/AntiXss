// ***************************************************************
// <copyright file="ConverterStream.cs" company="Microsoft">
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

    

    
    
    
    internal enum ConverterStreamAccess
    {
        
        Read,
        
        Write,
    }

    
    
    
    
    internal class ConverterStream : Stream, IProgressMonitor
    {
        

        private IProducerConsumer consumer;

        private int maxLoopsWithoutProgress;
        private bool madeProgress;

        private byte[] chunkToReadBuffer;
        private int chunkToReadOffset;
        private int chunkToReadCount;

        

        private IByteSource byteSource;
        private IProducerConsumer producer;

        private byte[] writeBuffer;
        private int writeOffset;
        private int writeCount;

        

        private object sourceOrDestination;

        private bool endOfFile;
        private bool inconsistentState;

        
        
        
        
        
        
        
        
        
        
        
        
        public ConverterStream(
            Stream stream,
            TextConverter converter,
            ConverterStreamAccess access)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (access < ConverterStreamAccess.Read || ConverterStreamAccess.Write < access)
            {
                throw new ArgumentException(Strings.AccessShouldBeReadOrWrite, "access");
            }

            if (access == ConverterStreamAccess.Read)
            {
                if (!stream.CanRead)
                {
                    throw new ArgumentException(Strings.CannotReadFromSource, "stream");
                }

                this.producer = converter.CreatePullChain(stream, this);
            }
            else
            {
                if (!stream.CanWrite)
                {
                    throw new ArgumentException(Strings.CannotWriteToDestination, "stream");
                }

                this.consumer = converter.CreatePushChain(this, stream);
            }

            this.sourceOrDestination = stream;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        
        
        public ConverterStream(
            TextReader sourceReader,
            TextConverter converter)
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
            this.sourceOrDestination = sourceReader;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        
        public ConverterStream(
            TextWriter destinationWriter,
            TextConverter converter)
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
            this.sourceOrDestination = destinationWriter;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        

        
        
        
        
        public override bool CanRead
        {
            get
            {
                return this.producer != null ? true : false;
            }
        }

        
        
        
        
        public override bool CanWrite
        {
            get
            {
                return this.consumer != null ? true : false;
            }
        }

        
        
        
        
        public override bool CanSeek
        {
            get { return false; }
        }

        

        
        
        
        
        public override long Length
        {
            get
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }
        }

        
        
        
        
        public override long Position
        {
            get
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }

            set
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }
        }

        
        
        
        
        
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        
        
        
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        
        
        
        
        
        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (this.consumer == null)
            {
                throw new InvalidOperationException(Strings.WriteUnsupported);
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.endOfFile)
            {
                throw new InvalidOperationException(Strings.WriteAfterFlush);
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterStreamInInconsistentStare);
            }

            InternalDebug.Assert(this.consumer != null);

            this.chunkToReadBuffer = buffer;
            this.chunkToReadOffset = offset;
            this.chunkToReadCount = count;

            
            
            

            long loopsWithoutProgress = 0;

            
            
            this.inconsistentState = true;

            while (0 != this.chunkToReadCount)
            {
                
                this.consumer.Run();

                if (this.madeProgress)
                {
                    
                    loopsWithoutProgress = 0;
                    this.madeProgress = false;
                }
                else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProcessInput);
                }
            }

            
            this.inconsistentState = false;

            this.chunkToReadBuffer = null;       
        }

        
        
        
        
        
        
        public override void Flush()
        {
            if (this.sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (this.consumer == null)
            {
                throw new InvalidOperationException(Strings.WriteUnsupported);
            }

            InternalDebug.Assert(this.consumer != null);

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

            if (this.sourceOrDestination is Stream)
            {
                ((Stream)this.sourceOrDestination).Flush();
            }
            else if (this.sourceOrDestination is TextWriter)
            {
                ((TextWriter)this.sourceOrDestination).Flush();
            }
        }

        
        
        
        
        
        

        public override void Close()
        {
            try
            {
                if (this.sourceOrDestination != null)
                {
                    if (this.consumer != null && !this.inconsistentState)
                    {
                        this.Flush();
                    }
                }

                
                
                
                if (this.producer != null && this.producer is IDisposable)
                {
                    ((IDisposable)this.producer).Dispose();
                }

                if (this.consumer != null && this.consumer is IDisposable)
                {
                    ((IDisposable)this.consumer).Dispose();
                }
            }
            finally
            {
                if (this.sourceOrDestination != null)
                {
                    if (this.sourceOrDestination is Stream)
                    {
                        ((Stream)this.sourceOrDestination).Close();
                    }
                    else if (this.sourceOrDestination is TextReader)
                    {
                        ((TextReader)this.sourceOrDestination).Close();
                    }
                    else
                    {
                        ((TextWriter)this.sourceOrDestination).Close();
                    }
                }
                this.sourceOrDestination = null;
                this.consumer = null;
                this.producer = null;
                this.chunkToReadBuffer = null;
                this.writeBuffer = null;
                this.byteSource = null;
            }
        }

        
        
        
        
        
        
        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (this.producer == null)
            {
                throw new InvalidOperationException(Strings.ReadUnsupported);
            }

            InternalDebug.Assert(this.producer != null);

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterStreamInInconsistentStare);
            }

            InternalDebug.Assert(this.producer != null);

            int initialCount = count;

            if (this.byteSource != null)
            {
                
                

                byte[] chunkBuffer;
                int chunkOffset;
                int chunkCount;

                while (count != 0 && this.byteSource.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkCount))
                {
                    int bytesRead = Math.Min(chunkCount, count);

                    Buffer.BlockCopy(chunkBuffer, chunkOffset, buffer, offset, bytesRead);

                    offset += bytesRead;
                    count -= bytesRead;

                    this.byteSource.ReportOutput(bytesRead);
                }
            }

            
            

            if (0 != count)
            {
                

                
                
                

                long loopsWithoutProgress = 0;

                this.writeBuffer = buffer;
                this.writeOffset = offset;
                this.writeCount = count;

                
                
                this.inconsistentState = true;

                while (0 != this.writeCount && !this.endOfFile)
                {
                    

                    this.producer.Run();

                    if (this.madeProgress)
                    {
                        
                        loopsWithoutProgress = 0;
                        this.madeProgress = false;
                    }
                    else if (this.maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                    }
                }

                count = this.writeCount;     

                this.writeBuffer = null;
                this.writeOffset = 0;
                this.writeCount = 0;

                
                this.inconsistentState = false;
            }

            return initialCount - count;
        }

        
        
        
        
        
        internal void SetSource(IByteSource byteSource)
        {
            
            InternalDebug.Assert(this.producer == null && this.consumer == null);

            this.byteSource = byteSource;
        }

        
        
        
        
        
        
        
        internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
        {
            InternalDebug.Assert(this.producer != null);
            InternalDebug.Assert(!this.endOfFile);

            outputBuffer = this.writeBuffer;
            outputOffset = this.writeOffset;
            outputCount = this.writeCount;
        }

        
        
        
        
        
        internal void ReportOutput(int outputCount)
        {
            InternalDebug.Assert(this.producer != null);
            InternalDebug.Assert(!this.endOfFile && outputCount <= this.writeCount);

            if (outputCount != 0)
            {
                this.madeProgress = true;
                this.writeCount -= outputCount;
                this.writeOffset += outputCount;
            }
        }

        
        
        
        
        internal void ReportEndOfFile()
        {
            InternalDebug.Assert(this.producer != null);

            this.endOfFile = true;
        }

        
        
        
        
        
        
        
        
        
        internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
        {
            chunkBuffer = this.chunkToReadBuffer;
            chunkOffset = this.chunkToReadOffset;
            chunkCount = this.chunkToReadCount;

            eof = this.endOfFile && 0 == this.chunkToReadCount;

            return 0 != this.chunkToReadCount || this.endOfFile;
        }

        
        
        
        
        
        internal void ReportRead(int readCount)
        {
            InternalDebug.Assert(readCount >= 0 && readCount <= this.chunkToReadCount);

            if (readCount != 0)
            {
                this.madeProgress = true;

                this.chunkToReadCount -= readCount;
                this.chunkToReadOffset += readCount;

                if (this.chunkToReadCount == 0)
                {
                    this.chunkToReadBuffer = null;
                    this.chunkToReadOffset = 0;
                }
            }
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }

        
        
        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        
        internal void Reuse(object newSourceOrSink)
        {
            if (this.producer != null)
            {
                if (!(this.producer is IReusable))
                {
                    throw new NotSupportedException("this converter is not reusable");
                }

                ((IReusable)this.producer).Initialize(newSourceOrSink);
            }
            else
            {
                if (!(this.consumer is IReusable))
                {
                    throw new NotSupportedException("this converter is not reusable");
                }

                ((IReusable)this.consumer).Initialize(newSourceOrSink);
            }

            this.sourceOrDestination = newSourceOrSink;

            this.chunkToReadBuffer = null;
            this.chunkToReadOffset = 0;
            this.chunkToReadCount = 0;

            this.writeBuffer = null;
            this.writeOffset = 0;
            this.writeCount = 0;

            this.endOfFile = false;
            this.inconsistentState = false;
        }
    }
}

