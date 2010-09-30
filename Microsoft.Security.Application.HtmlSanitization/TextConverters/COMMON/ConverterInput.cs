// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterInput.cs" company="Microsoft Corporation">
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
    
    internal abstract class ConverterInput : IDisposable
    {
        protected bool endOfFile;
        protected int maxTokenSize;

        protected IProgressMonitor progressMonitor;

        /// <summary>
        /// Gets a value indicating whether reached end of file.
        /// </summary>
        /// <value><c>true</c> if reached end of file; otherwise, <c>false</c>.</value>
        public bool EndOfFile
        {
            get { return this.endOfFile; }
        }

        /// <summary>
        /// Gets the max size of the token.
        /// </summary>
        /// <value>The max size of the token.</value>
        public int MaxTokenSize
        {
            get { return this.maxTokenSize; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterInput"/> class.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor.</param>
        protected ConverterInput(IProgressMonitor progressMonitor)
        {
            this.progressMonitor = progressMonitor;
        }

        /// <summary>
        /// Sets the restart consumer.
        /// </summary>
        /// <param name="restartConsumer">The restart consumer.</param>
        public virtual void SetRestartConsumer(IRestartable restartConsumer)
        {
        }
        
        public abstract bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end);
        
        public abstract void ReportProcessed(int processedSize);
        
        public abstract int RemoveGap(int gapBegin, int gapEnd);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose()
        {
        }
    }
}
