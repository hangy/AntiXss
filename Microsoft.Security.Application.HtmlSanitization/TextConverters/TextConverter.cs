// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConverter.cs" company="Microsoft Corporation">
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
//   Base class for a text convertor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using Strings = CtsResources.TextConvertersStrings;

    /// <summary>
    /// Configuration parameters
    /// </summary>
    internal enum ConfigParameter
    {
        /// <summary>
        /// Input Encoding Parameter
        /// </summary>
        InputEncoding,

        /// <summary>
        /// Output Encoding parameter
        /// </summary>
        OutputEncoding,

        /// <summary>
        /// Rich Text Format compression mode
        /// </summary>
        RtfCompressionMode,

        /// <summary>
        /// Rich Text Format encapsulation mode
        /// </summary>
        RtfEncapsulation
    }

    /// <summary>
    /// An interface for setting results
    /// </summary>
    internal interface IResultsFeedback
    {
        /// <summary>
        /// Sets the configuration parameter and its associated value.
        /// </summary>
        /// <param name="parameterId">The configuration parameter to set.</param>
        /// <param name="val">The value for the configuration parameter.</param>
        void Set(ConfigParameter parameterId, object val);
    }
    
    /// <summary>
    /// Base class for a text convertor.
    /// </summary>
    internal abstract class TextConverter : IResultsFeedback
    {
        /// <summary>
        /// Value indicating if boundary conditions are to be tested.
        /// </summary>
        private bool testBoundaryConditions = false;

        /// <summary>
        /// The input buffer size.
        /// </summary>
        private int inputBufferSize = 4096;

        /// <summary>
        /// The output buffer size.
        /// </summary>
        private int outputBufferSize = 4096;

        /// <summary>
        /// Gets or sets the size of the input stream buffer.
        /// </summary>
        /// <value>The size of the input stream buffer.</value>
        public int InputStreamBufferSize
        {
            get
            {
                return this.inputBufferSize;
            }

// Orphaned WPL code.
#if false
            set
            {
                this.AssertNotLocked();

                if (value < 1024 || value > 80 * 1024)
                {
                    throw new ArgumentOutOfRangeException("value", Strings.BufferSizeValueRange);
                }

                this.inputBufferSize = value;
            }
#endif
        }

        /// <summary>
        /// Gets or sets the size of the output stream buffer.
        /// </summary>
        /// <value>The size of the output stream buffer.</value>
        public int OutputStreamBufferSize
        {
            get
            {
                return this.outputBufferSize;
            }

// Orphaned WPL code.
#if false
            set
            {
                this.AssertNotLocked();

                if (value < 1024 || value > 80 * 1024)
                {
                    throw new ArgumentOutOfRangeException("value", Strings.BufferSizeValueRange);
                }

                this.outputBufferSize = value;
            }
#endif
        }

        /// <summary>
        /// Gets or sets a value indicating whether boundary conditions should be tested.
        /// </summary>
        /// <value>
        /// <c>true</c> if boundary conditions should be tested; otherwise, <c>false</c>.
        /// </value>
        internal bool TestBoundaryConditions
        {
            get
            {
                return this.testBoundaryConditions;
            }

// Orphaned WPL code.
#if false
            set
            {
                this.AssertNotLocked();
                this.testBoundaryConditions = value;
            }
#endif
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextConverter"/> is locked.
        /// </summary>
        /// <value><c>true</c> if locked; otherwise, <c>false</c>.</value>
        protected bool Locked
        {
            get;
            set;
        }

// Orphaned WPL code.
#if false
        /// <summary>
        /// Converts the specified source stream.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="destinationStream">The destination stream.</param>
        public void Convert(Stream sourceStream, Stream destinationStream)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }
            
            Stream converter = new ConverterStream(sourceStream, this, ConverterStreamAccess.Read);

            byte[] buf = new byte[this.outputBufferSize];

            while (true)
            {
                int cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationStream.Write(buf, 0, cnt);
            }

            destinationStream.Flush();
        }

        /// <summary>
        /// Converts the specified source stream.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="destinationWriter">The destination writer.</param>
        public void Convert(Stream sourceStream, TextWriter destinationWriter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }
            
            TextReader converter = new ConverterReader(sourceStream, this);

            char[] buf = new char[4096];

            while (true)
            {
                int cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationWriter.Write(buf, 0, cnt);
            }

            destinationWriter.Flush();
        }
#endif

        /// <summary>
        /// Converts the specified source reader.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="destinationStream">The destination stream.</param>
        public void Convert(TextReader sourceReader, Stream destinationStream)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException(nameof(destinationStream));
            }
            
            Stream converter = new ConverterStream(sourceReader, this);

            byte[] buf = new byte[this.outputBufferSize];

            while (true)
            {
                int cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationStream.Write(buf, 0, cnt);
            }

            destinationStream.Flush();
        }

        /// <summary>
        /// Converts the specified source reader.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="destinationWriter">The destination writer.</param>
        public void Convert(TextReader sourceReader, TextWriter destinationWriter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException(nameof(destinationWriter));
            }
            
            TextReader converter = new ConverterReader(sourceReader, this);

            char[] buf = new char[4096];

            while (true)
            {
                int cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationWriter.Write(buf, 0, cnt);
            }

            destinationWriter.Flush();
        }

        /// <summary>
        /// Sets the configuration parameter and its associated value.
        /// </summary>
        /// <param name="parameterId">The configuration parameter to set.</param>
        /// <param name="val">The value for the configuration parameter.</param>
        void IResultsFeedback.Set(ConfigParameter parameterId, object val)
        {
            this.SetResult(parameterId, val);
        }

        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterStream">The converter stream.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output);
        
        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterStream">The converter stream.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output);

// Orphaned WPL code.
#if false
        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterWriter">The converter writer.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output);

        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterWriter">The converter writer.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output);
#endif

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input Stream.</param>
        /// <param name="converterStream">The converter stream.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input TextReader.</param>
        /// <param name="converterStream">The converter stream.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream);
        
        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="converterReader">The converter reader.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input TextReader.</param>
        /// <param name="converterReader">The converter reader.</param>
        /// <returns>An <see cref="IProducerConsumer"/> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader);

        /// <summary>
        /// Sets the result for the specified parameter.
        /// </summary>
        /// <param name="parameterId">The parameter.</param>
        /// <param name="val">The value.</param>
        internal virtual void SetResult(ConfigParameter parameterId, object val)
        {
        }

        /// <summary>
        /// Asserts that this instance is not locked.
        /// </summary>
        internal void AssertNotLocked()
        {
            if (this.Locked)
            {
                throw new InvalidOperationException(Strings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
            }
        }
    }
}

