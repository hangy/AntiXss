// ***************************************************************
// <copyright file="ConverterInput.cs" company="Microsoft">
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

    
    
    internal abstract class ConverterInput : IDisposable
    {
        protected bool endOfFile;
        protected int maxTokenSize;

        protected IProgressMonitor progressMonitor;

        
        
        public bool EndOfFile
        {
            get { return this.endOfFile; }
        }

        
        
        public int MaxTokenSize
        {
            get { return this.maxTokenSize; }
        }

        protected ConverterInput(IProgressMonitor progressMonitor)
        {
            this.progressMonitor = progressMonitor;
        }

        
        
        public virtual void SetRestartConsumer(IRestartable restartConsumer)
        {
        }

        
        
        
        
        
        
        public abstract bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end);

        
        
        public abstract void ReportProcessed(int processedSize);

        
        
        
        
        public abstract int RemoveGap(int gapBegin, int gapEnd);

        
        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        
        protected virtual void Dispose()
        {
        }
    }
}
