// ***************************************************************
// <copyright file="SuppressCloseStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.IO;


    internal sealed class SuppressCloseStream : Stream, ICloneableStream
    {
        private Stream sourceStream;    

        
        public SuppressCloseStream(Stream sourceStream)
        {
            if (null == sourceStream)
            {
                throw new ArgumentNullException("sourceStream");
            }

            this.sourceStream = sourceStream;
        }

        public override bool CanRead
        {
            get 
            {
                return (null == this.sourceStream) ? false : this.sourceStream.CanRead; 
            }
        }

        public override bool CanWrite
        {
             get 
             {
                 return  (null == this.sourceStream) ? false : this.sourceStream.CanWrite; 
             }
        }

        
        public override bool CanSeek
        {
            get 
            {
                return  (null == this.sourceStream) ? false : this.sourceStream.CanSeek; 
            }
        }

        
        public override long Length
        {
            get 
            {
                this.AssertOpen();

                return this.sourceStream.Length; 
            }
        }

        
        public override long Position
        {
            get 
            {
                this.AssertOpen();

                return this.sourceStream.Position; 
            }

            set
            {
                this.AssertOpen();

                this.sourceStream.Position = value;
            }
        }

        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            this.AssertOpen();

            return this.sourceStream.Read(buffer, offset, count);
        }

        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.AssertOpen();

            this.sourceStream.Write(buffer, offset, count);
        }

        
        
        public override void Flush()
        {          
            this.AssertOpen();

            this.sourceStream.Flush();
        }

        
        public override void SetLength(long value)
        {
            this.AssertOpen();

            this.sourceStream.SetLength(value);
        }

        
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.AssertOpen();

            return this.sourceStream.Seek(offset, origin);
        }

        public override void Close()
        {
            if (null == this.sourceStream)
            {
                return;
            }

            this.sourceStream = null;
            base.Close();
        }

        
        public Stream Clone()
        {
            this.AssertOpen();

            if (this.CanWrite)
            {
                throw new NotSupportedException();
            }

            ICloneableStream cloneableStream = this.sourceStream as ICloneableStream;

            if (cloneableStream == null)
            {
                
                if (!this.sourceStream.CanSeek)
                {
                    throw new NotSupportedException();
                }

                this.sourceStream = new AutoPositionReadOnlyStream(this.sourceStream, false/*ownsStream*/);

                cloneableStream = this.sourceStream as ICloneableStream;
            }

            return new SuppressCloseStream(cloneableStream.Clone());
        }

        private void AssertOpen()
        {
            if (null == this.sourceStream)
            {
                throw new ObjectDisposedException("SuppressCloseStream");
            }
        }

        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }
}

