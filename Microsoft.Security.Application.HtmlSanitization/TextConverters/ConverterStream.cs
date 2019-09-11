// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterStream.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   A conversion class presented as a stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using Data.Internal;
    using Strings = CtsResources.TextConvertersStrings;

    /// <summary>
    /// Indicates the type of access granted to the convertor stream.
    /// </summary>
    internal enum ConverterStreamAccess
    {
        /// <summary>
        /// Read access.
        /// </summary>        
        Read,

        /// <summary>
        /// Write access.
        /// </summary>
        Write
    }

    /// <summary>
    /// A stream class used for conversion.
    /// </summary>
    internal class ConverterStream : Stream, IProgressMonitor
    {
        /// <summary>
        /// The number of conversion loops to attempt without any progress before the conversion is cancelled.
        /// </summary>
        private readonly int maxLoopsWithoutProgress;

        /// <summary>
        /// The conversion consumer.
        /// </summary>
        private IProducerConsumer consumer;

        /// <summary>
        /// Value indicating if any progress has been made during conversion.
        /// </summary>
        private bool madeProgress;

        /// <summary>
        /// The read buffer
        /// </summary>
        private byte[] chunkToReadBuffer;

        /// <summary>
        /// The offset to start reading from
        /// </summary>
        private int chunkToReadOffset;

        /// <summary>
        /// The number of bytes read.
        /// </summary>
        private int chunkToReadCount;

        /// <summary>
        /// The data source to be converted.
        /// </summary>
        private IByteSource byteSource;

        /// <summary>
        /// The conversion producer.
        /// </summary>
        private IProducerConsumer producer;

        /// <summary>
        /// The write buffer.
        /// </summary>
        private byte[] writeBuffer;

        /// <summary>
        /// The offset to write from.
        /// </summary>
        private int writeOffset;

        /// <summary>
        /// The number of characters written.
        /// </summary>
        private int writeCount;

        /// <summary>
        /// The object source or destination.
        /// </summary>
        private object sourceOrDestination;

        /// <summary>
        /// Value indicating if the end of the file has been reached.
        /// </summary>
        private bool endOfFile;

        /// <summary>
        /// Value indicating if the conversion is in an inconsitent state.
        /// </summary>
        private bool inconsistentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterStream"/> class.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="converter">The converter to use.</param>
        public ConverterStream(
            TextReader sourceReader,
            TextConverter converter)
        {
            if (sourceReader == null)
            {
                throw new ArgumentNullException(nameof(sourceReader));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            this.producer = converter.CreatePullChain(sourceReader, this);
            this.sourceOrDestination = sourceReader;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead
        {
            get
            {
                return this.producer != null ? true : false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite
        {
            get
            {
                return this.consumer != null ? true : false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// A class derived from Stream does not support seeking.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support seeking.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
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

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
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
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountTooLarge);
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
                    throw new TextConvertersException(Strings.TooManyIterationsToProcessInput);
                }
            }

            this.inconsistentState = false;

            this.chunkToReadBuffer = null;
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Methods were called on a read-only stream.
        /// </exception>
        /// <exception cref="TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
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
                        throw new TextConvertersException(Strings.TooManyIterationsToFlushConverter);
                    }
                }

                this.inconsistentState = false;
            }

            if (this.sourceOrDestination is Stream stream)
            {
                stream.Flush();
            }
            else if (this.sourceOrDestination is TextWriter textWriter)
            {
                textWriter.Flush();
            }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
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
                    if (this.sourceOrDestination is Stream stream)
                    {
                        stream.Close();
                    }
                    else if (this.sourceOrDestination is TextReader textReader)
                    {
                        textReader.Close();
                    }
                    else if (this.sourceOrDestination is TextWriter textWriter)
                    {
                        textWriter.Close();
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

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
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
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountTooLarge);
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterStreamInInconsistentStare);
            }

            InternalDebug.Assert(this.producer != null);

            int initialCount = count;

            if (this.byteSource != null)
            {
                while (count != 0 && this.byteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount))
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
                        throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
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

        /// <summary>
        /// Report the progress of the current operation.
        /// </summary>
        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        /// <summary>
        /// Sets the source of the information to be converted.
        /// </summary>
        /// <param name="newByteSource">The byte source.</param>
        internal void SetSource(IByteSource newByteSource)
        {
            InternalDebug.Assert(this.producer == null && this.consumer == null);

            this.byteSource = newByteSource;
        }

        /// <summary>
        /// Gets the output buffer.
        /// </summary>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputCount">The output count.</param>
        internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
        {
            InternalDebug.Assert(this.producer != null);
            InternalDebug.Assert(!this.endOfFile);

            outputBuffer = this.writeBuffer;
            outputOffset = this.writeOffset;
            outputCount = this.writeCount;
        }

        /// <summary>
        /// Reports the output.
        /// </summary>
        /// <param name="outputCount">The output count.</param>
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

        /// <summary>
        /// Reports the end of file.
        /// </summary>
        internal void ReportEndOfFile()
        {
            InternalDebug.Assert(this.producer != null);

            this.endOfFile = true;
        }

        /// <summary>
        /// Gets the input chunk.
        /// </summary>
        /// <param name="chunkBuffer">The chunk buffer.</param>
        /// <param name="chunkOffset">The chunk offset.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="eof">Set to true if the EOF was reached.</param>
        /// <returns>false if there are no more bytes to read, otherwise true.</returns>
        internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
        {
            chunkBuffer = this.chunkToReadBuffer;
            chunkOffset = this.chunkToReadOffset;
            chunkCount = this.chunkToReadCount;

            eof = this.endOfFile && 0 == this.chunkToReadCount;

            return 0 != this.chunkToReadCount || this.endOfFile;
        }

        /// <summary>
        /// Reports the number of characters read.
        /// </summary>
        /// <param name="readCount">The read count.</param>
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
    }
}

