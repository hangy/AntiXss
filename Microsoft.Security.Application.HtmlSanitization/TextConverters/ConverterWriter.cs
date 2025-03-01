// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterWriter.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//    
// </summary>

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
        private readonly int maxLoopsWithoutProgress;

        private char[] chunkToReadBuffer;
        private int chunkToReadIndex;
        private int chunkToReadCount;

        private object destination;

        private bool endOfFile;

        private bool inconsistentState;

        private readonly bool boundaryTesting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterWriter"/> class.
        /// </summary>
        private ConverterWriter()
        {
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

            if (this.destination is Stream stream)
            {
                stream.Flush();
            }
            else if (this.destination is TextWriter textWriter)
            {
                textWriter.Flush();
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (this.destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
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
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            int parseCount = 10000;

            if (!this.boundaryTesting)
            {
                this.sinkInputObject.GetInputBuffer(out char[] inputBuffer, out int inputIndex, out int inputCount, out parseCount);

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
                this.sinkInputObject.GetInputBuffer(out char[] inputBuffer, out int inputIndex, out int inputCount, out parseCount);

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

                    if (this.destination is Stream stream)
                    {
                        stream.Close();
                    }
                    else if (this.destination is TextWriter textWriter)
                    {
                        textWriter.Close();
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
    }
}
