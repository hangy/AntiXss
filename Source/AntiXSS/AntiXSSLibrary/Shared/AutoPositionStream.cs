// ***************************************************************
// <copyright file="AutoPositionStream.cs" company="Microsoft">
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
    using Strings = Microsoft.Exchange.CtsResources.SharedStrings;

    
    
    
    
    internal sealed class AutoPositionReadOnlyStream : Stream, ICloneableStream
    {
        private ReadableDataStorage storage;
        private long position;

        

        
        
        
        
        
        public AutoPositionReadOnlyStream(Stream wrapped, bool ownsStream)
        {
            this.storage = new ReadableDataStorageOnStream(wrapped, ownsStream);
            this.position = wrapped.Position;
        }

        
        
        
        
        private AutoPositionReadOnlyStream(AutoPositionReadOnlyStream original)
        {
            original.storage.AddRef();
            this.storage = original.storage;
            this.position = original.position;
        }

        

        
        
        
        public override bool CanRead
        {
            get
            { 
                return this.storage != null;
            }
        }

        
        
        
        public override bool CanWrite
        {
            get
            { 
                return false;
            }
        }

        
        
        
        public override bool CanSeek
        {
            get
            { 
                return this.storage != null;
            }
        }

        
        
        
        public override long Length
        {
            get
            { 
                if (this.storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }

                return this.storage.Length; 
            }
        }

        
        
        
        
        
        
        
        
        
        public override long Position
        {
            get
            { 
                if (this.storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }

                return this.position; 
            }
            set 
            { 
                if (this.storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", Strings.CannotSeekBeforeBeginning);
                }

                this.position = value; 
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            int read = this.storage.Read(this.position, buffer, offset, count);
            this.position += read;
            return read;
        }

        
        
        
        
        
        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        
        
        
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        
        
        
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    break;
                }
                case SeekOrigin.Current:
                {
                    offset += this.position;
                    break;
                }
                case SeekOrigin.End:
                {
                    offset += this.Length;
                    break;
                }
                default:
                {
                    
                    throw new System.ArgumentException("origin");
                }
            }

            if (0 > offset)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.CannotSeekBeforeBeginning);
            }

            this.position = offset;

            return this.position;
        }

        
        
        
        public override void Close()
        {
            if (this.storage != null)
            {
                this.storage.Release();
                this.storage = null;
            }

            base.Close();
        }

        
        
        
        
        public Stream Clone()
        {
            if (this.storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            return new AutoPositionReadOnlyStream(this);
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }
}
