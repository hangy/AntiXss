// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseInspectorFilter.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
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
//   A stream which passes the original response through request inspectors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using PlugIns;

    /// <summary>
    /// A stream which passes the original response through request inspectors.
    /// </summary>
    internal sealed class ResponseInspectorFilter : Stream
    {
        /// <summary>
        /// The response inspectors to pass the stream through.
        /// </summary>
        private readonly IEnumerable<IResponseInspector> inspectors;

        /// <summary>
        /// The original source stream.
        /// </summary>
        private readonly Stream source;

        /// <summary>
        /// The HTTP context for the response.
        /// </summary>
        private readonly HttpContextBase context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseInspectorFilter"/> class.
        /// </summary>
        /// <param name="inspectors">The request inspectors to pass the stream through.</param>
        /// <param name="source">The source stream.</param>
        /// <param name="context">The context for the response.</param>
        internal ResponseInspectorFilter(IEnumerable<IResponseInspector> inspectors, Stream source, HttpContextBase context)
        {
            this.inspectors = inspectors;
            this.source = source;
            this.context = context;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>Always false.</returns>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>Always false.</returns>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>Always true.</returns>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Always thrown.
        /// </exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Always thrown
        /// </exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override void Flush()
        {
            this.source.Flush();
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
        /// <exception cref="T:System.NotSupportedException">
        /// Always thrown.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Always thrown.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// Always thrown.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.context != null && SecurityRuntimeInspection.IsRequestStopped(this.context))
            {
                this.source.Write(buffer, 0, buffer.Length);
                return;
            }

            // Retrieve the number of suspect inspections that this request and response has had so far.
            int suspectRequestCount = SecurityRuntimeInspection.GetSuspectCountBeforeInspection(this.context);

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            string contentType = SecurityRuntimeInspection.GetContentType(this.context);

            foreach (IInspectionResult result in this.inspectors.Select(inspector => inspector.Inspect(this.context.Request, contentType, ref buffer)))
            {
                switch (result.Severity)
                {
                    case InspectionResultSeverity.Halt:
                        SecurityRuntimeInspection.StopRequest(result, this.context);                        
                        throw new ResponseStoppedException(result.StopReason);
                    case InspectionResultSeverity.Suspect:
                        suspectRequestCount++;
                        break;
                    default:
                        break;
                }

                this.source.Write(buffer, 0, buffer.Length);
            }

            // If we're over the maximum number of suspect results throw an exception and stop processing.
            if (SecurityRuntimeSettings.Settings.AllowedSuspectResults != -1 &&
                suspectRequestCount > SecurityRuntimeSettings.Settings.AllowedSuspectResults)
            {
                SecurityRuntimeInspection.StopRequest(new TooManySuspectInspectionsResult(Properties.Resources.ResponseInspectionStoppedMessage), this.context);
                throw new ResponseStoppedException(Properties.Resources.ResponseInspectionStoppedMessage);
            }

            // And finally if we're still good, and we're keeping the suspect inspections count between stages then save it away.
            if (!SecurityRuntimeSettings.Settings.ResetSuspectCountBetweenStages)
            {
                SecurityRuntimeInspection.SetSuspectCountAfterInspection(this.context, suspectRequestCount);
            }
        }
    }
}
