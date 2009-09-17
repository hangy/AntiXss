// ***************************************************************
// <copyright file="TextCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Collections;
    using Microsoft.Exchange.Data.Internal;


    internal class TextCache
    {
        
        private int cachedLength;

        private CacheEntry headEntry;
        private CacheEntry tailEntry;
        private CacheEntry freeList;

        
        
        public TextCache()
        {
        }

        
        
        public int Length
        {
            get
            {
                return this.cachedLength;
            }
        }
        
        
        
        public void Reset()
        {
            while (this.headEntry != null)
            {
                this.headEntry.Reset();

                

                CacheEntry newFree = this.headEntry;

                this.headEntry = this.headEntry.Next;

                if (this.headEntry == null)
                {
                    this.tailEntry = null;
                }

                

                newFree.Next = this.freeList;
                this.freeList = newFree;
            }

            this.cachedLength = 0;
        }

        
        
        
        
        
        
        public void GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
        {
            if (this.tailEntry != null)
            {
                if (this.tailEntry.GetBuffer(size, out buffer, out offset, out realSize))
                {
                    return;
                }

                
            }

            

            this.AllocateTail(size);

            InternalDebug.Assert(this.tailEntry != null);

            bool success = this.tailEntry.GetBuffer(size, out buffer, out offset, out realSize);

            InternalDebug.Assert(success);
        }

        
        
        
        public void Commit(int count)
        {
            InternalDebug.Assert(this.tailEntry != null);

            this.tailEntry.Commit(count);

            this.cachedLength += count;
        }

        
        
        
        
        
        public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
        {
            InternalDebug.Assert(this.headEntry != null);

            this.headEntry.GetData(out outputBuffer, out outputOffset, out outputCount);
        }

        
        
        
        public void ReportRead(int count)
        {
            InternalDebug.Assert(this.headEntry != null);

            this.headEntry.ReportRead(count);

            this.cachedLength -= count;

            if (0 == this.headEntry.Length)
            {
                

                CacheEntry newFree = this.headEntry;

                this.headEntry = this.headEntry.Next;

                if (this.headEntry == null)
                {
                    InternalDebug.Assert(this.cachedLength == 0);

                    this.tailEntry = null;
                }

                newFree.Next = this.freeList;
                this.freeList = newFree;
            }
        }

        
        
        
        
        
        
        public int Read(char[] buffer, int offset, int count)
        {
            int countCopiedTotal = 0;

            while (0 != count)
            {
                int countCopied = this.headEntry.Read(buffer, offset, count);

                offset += countCopied;
                count -= countCopied;

                countCopiedTotal += countCopied;

                this.cachedLength -= countCopied;

                if (0 == this.headEntry.Length)
                {
                    

                    CacheEntry newFree = this.headEntry;

                    this.headEntry = this.headEntry.Next;

                    if (this.headEntry == null)
                    {
                        InternalDebug.Assert(this.cachedLength == 0);

                        this.tailEntry = null;
                    }

                    newFree.Next = this.freeList;
                    this.freeList = newFree;
                }

                if (0 == count || this.headEntry == null)
                {
                    break;
                }

                

                InternalDebug.Assert(0 != this.cachedLength && this.tailEntry != null);
            }

            return countCopiedTotal;
        }

        
        
        
        private void AllocateTail(int size)
        {
            CacheEntry newEntry = this.freeList;
            if (newEntry != null)
            {
                this.freeList = newEntry.Next;
                newEntry.Next = null;
            }
            else
            {
                newEntry = new CacheEntry(size);
            }

            InternalDebug.Assert(newEntry.Length == 0);

            if (this.tailEntry != null)
            {
                this.tailEntry.Next = newEntry;
            }
            else
            {
                InternalDebug.Assert(this.headEntry == null);
                this.headEntry = newEntry;
            }

            this.tailEntry = newEntry;
        }

        
        
        internal class CacheEntry
        {
            private const int DefaultMaxLength = 4096;

            private char[] buffer;
            private int count;
            private int offset;

            private CacheEntry next;

            
            
            
            public CacheEntry(int size)
            {
                this.AllocateBuffer(size);
            }

            
            
            public int Length
            {
                get { return this.count; }
            }
        
            
            
            public CacheEntry Next
            {
                get { return this.next; }
                set { this.next = value; }
            }
        
            
            
            public void Reset()
            {
                this.count = 0;
            }

            
            
            
            
            
            
            
            public bool GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
            {
                if (this.count == 0)
                {
                    this.offset = 0;

                    if (this.buffer.Length < size)
                    {
                        
                        

                        this.AllocateBuffer(size);
                    }
                }

                if (this.buffer.Length - (this.offset + this.count) >= size)
                {
                    buffer = this.buffer;
                    offset = this.offset + this.count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }

                InternalDebug.Assert(this.count != 0);

                if (this.count < 64 && this.buffer.Length - this.count >= size)
                {
                    
                    
                    
                    Buffer.BlockCopy(this.buffer, this.offset * 2, this.buffer, 0, this.count * 2);
                    this.offset = 0;

                    buffer = this.buffer;
                    offset = this.offset + this.count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }

                

                buffer = null;
                offset = 0;
                realSize = 0;
                return false;
            }

            
            
            
            public void Commit(int count)
            {
                InternalDebug.Assert(this.buffer.Length - (this.offset + this.count) >= count);

                this.count += count;
            }

            
            
            
            
            
            public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
            {
                InternalDebug.Assert(this.count > 0);

                outputBuffer = this.buffer;
                outputOffset = this.offset;
                outputCount = this.count;
            }

            
            
            
            public void ReportRead(int count)
            {
                InternalDebug.Assert(this.count >= count);

                this.offset += count;
                this.count -= count;
            }

            
            
            
            
            
            
            public int Read(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(this.count > 0);

                int countToCopy = Math.Min(count, this.count);

                Buffer.BlockCopy(this.buffer, this.offset * 2, buffer, offset * 2, countToCopy * 2);

                this.count -= countToCopy;
                this.offset += countToCopy;
            
                count -= countToCopy;
                offset += countToCopy;

                return countToCopy;
            }

            
            
            
            private void AllocateBuffer(int size)
            {
                if (size < DefaultMaxLength / 2)
                {
                    size = DefaultMaxLength / 2;
                }

                size = (size * 2 + 1023) / 1024 * 1024;

                this.buffer = new char[size];
            }
        }
    }
}

