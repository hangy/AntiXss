// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterReader.cs" company="Microsoft Corporation">
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
//   A conversion class presented as a text reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using Data.Internal;
    using Strings = CtsResources.TextConvertersStrings;

    /// <summary>
    /// A conversion class presented as a text reader.
    /// </summary>
    internal class ConverterReader : TextReader, IProgressMonitor
    {
        /// <summary>
        /// The number of conversion loops to attempt without any progress before the conversion is cancelled.
        /// </summary>
        private readonly int maxLoopsWithoutProgress;

        /// <summary>
        /// The output of the convertor.
        /// </summary>
        private ConverterUnicodeOutput sourceOutputObject;

        /// <summary>
        /// The conversion producer and consume.
        /// </summary>
        private IProducerConsumer producer;

        /// <summary>
        /// Value indicating if any progress has been made during conversion.
        /// </summary>
        private bool madeProgress;

        /// <summary>
        /// The internal write buffer.
        /// </summary>
        private char[] writeBuffer;

        /// <summary>
        /// The position within the internal write buffer.
        /// </summary>
        private int writeIndex;

        /// <summary>
        /// A running total of the number of characters written.
        /// </summary>
        private int writeCount;

        /// <summary>
        /// The conversion source.
        /// </summary>
        private object source;

        /// <summary>
        /// Value indicating if the end of the file has been reached.
        /// </summary>
        private bool endOfFile;

        /// <summary>
        /// Value indicating if the conversion is in an inconsitent state.
        /// </summary>
        private bool inconsistentState;

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterReader"/> class.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="converter">The converter to use.</param>
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
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterReader"/> class.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="converter">The converter to use.</param>
        public ConverterReader(TextReader sourceReader, TextConverter converter)
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

            this.source = sourceReader;

            this.maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        /// <summary>
        /// Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the input stream.
        /// </summary>
        /// <returns>
        /// An integer representing the next character to be read, or -1 if no more characters are available or the stream does not support seeking.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="System.IO.TextReader"/> is closed.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
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

            this.inconsistentState = true;

            while (!this.endOfFile)
            {
                if (this.sourceOutputObject.GetOutputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    this.inconsistentState = false;

                    return chunkBuffer[chunkIndex];
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
                    throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            this.inconsistentState = false;

            return -1;
        }

        /// <summary>
        /// Reads the next character from the input stream and advances the character position by one character.
        /// </summary>
        /// <returns>
        /// The next character from the input stream, or -1 if no more characters are available. The default implementation returns -1.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="System.IO.TextReader"/> is closed.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
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

            this.inconsistentState = true;

            while (!this.endOfFile)
            {
                if (this.sourceOutputObject.GetOutputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    this.sourceOutputObject.ReportOutput(1);

                    this.inconsistentState = false;

                    return chunkBuffer[chunkIndex];
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
                    throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            this.inconsistentState = false;

            return -1;
        }

        /// <summary>
        /// Reads a maximum of <paramref name="count"/> characters from the current stream and writes the data to <paramref name="buffer"/>, beginning at <paramref name="index"/>.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The place in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before <paramref name="count"/> of characters is read into <paramref name="buffer"/>, the current method returns.</param>
        /// <returns>
        /// The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the stream. This method returns zero if called when no more characters are left to read.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="System.IO.TextReader"/> is closed.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override int Read(char[] buffer, int index, int count)
        {
            if (this.source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), Strings.IndexOutOfRange);
            }

            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountOutOfRange);
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), Strings.CountTooLarge);
            }

            if (this.inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            int initialCount = count;

            while (count != 0 && this.sourceOutputObject.GetOutputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount))
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
                        throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
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

        /// <summary>
        /// Report the progress of the current operation.
        /// </summary>
        void IProgressMonitor.ReportProgress()
        {
            this.madeProgress = true;
        }

        /// <summary>
        /// Sets the data source for conversion.
        /// </summary>
        /// <param name="converterUnicodeOutputSource">The data source for conversion.</param>
        internal void SetSource(ConverterUnicodeOutput converterUnicodeOutputSource)
        {
            this.sourceOutputObject = converterUnicodeOutputSource;
        }

        /// <summary>
        /// Gets the output buffer.
        /// </summary>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputIndex">Current index position in the output buffer.</param>
        /// <param name="outputCount">The number of characters in the output buffer.</param>
        internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
        {
            InternalDebug.Assert(!this.endOfFile);

            outputBuffer = this.writeBuffer;
            outputIndex = this.writeIndex;
            outputCount = this.writeCount;
        }

        /// <summary>
        /// Notes that output has been written.
        /// </summary>
        /// <param name="outputCount">The number of characters written.</param>
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

        /// <summary>
        /// Reports that the end of the file has been reached.
        /// </summary>
        internal void ReportEndOfFile()
        {
            this.endOfFile = true;
        }

        // Orphaned WPL code.
#if false
        /// <summary>
        /// Reuses the specified new source.
        /// </summary>
        /// <param name="newSource">The new source.</param>
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
#endif

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="System.IO.TextReader"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.source != null)
                {
                    if (this.source is Stream stream)
                    {
                        stream.Close();
                    }
                    else if  (this.source is TextReader textReader)
                    {
                        textReader.Close();
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
    }
}
