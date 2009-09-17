// ***************************************************************
// <copyright file="ScratchPad.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Threading;
    using System.Text;
    using System.Diagnostics;

    internal static class ScratchPad
    {
        private static LocalDataStoreSlot scratchPadTlsSlot = Thread.AllocateDataSlot();

        public static void Begin()
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);
            if (pad == null)
            {
                pad = new ScratchPadContainer();

                Thread.SetData(scratchPadTlsSlot, pad);
            }
            else
            {
                pad.AddRef();
            }
        }

        public static void End()
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            InternalDebug.Assert(pad != null);

            if (pad != null)
            {
                if (pad.Release())
                {
                    
                    Thread.SetData(scratchPadTlsSlot, null);
                }
            }
        }

        public static byte[] GetByteBuffer(int size)
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new byte[size];
            }
               
            return pad.GetByteBuffer(size);
        }

        [Conditional("DEBUG")] 
        public static void ReleaseByteBuffer()
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseByteBuffer();
            }
        }

        public static char[] GetCharBuffer(int size)
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new char[size];
            }
               
            return pad.GetCharBuffer(size);
        }

        [Conditional("DEBUG")] 
        public static void ReleaseCharBuffer()
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseCharBuffer();
            }
        }

        public static StringBuilder GetStringBuilder()
        {
            
            return GetStringBuilder(16);
        }

        public static StringBuilder GetStringBuilder(int initialCapacity)
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new StringBuilder(initialCapacity);
            }
               
            return pad.GetStringBuilder(initialCapacity);
        }

        
        public static void ReleaseStringBuilder()
        {
            ScratchPadContainer pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseStringBuilder();
            }
        }

        private class ScratchPadContainer
        {
            public const int ScratchStringBuilderCapacity = 512;

            private int refCount;

            private byte[] byteBuffer;
            private char[] charBuffer;
            private StringBuilder stringBuilder;
#if DEBUG
            private bool byteBufferUsed;
            private bool charBufferUsed;
            private bool stringBuilderUsed;
#endif
            public ScratchPadContainer()
            {
                this.refCount = 1;
            }

            public void AddRef()
            {
                InternalDebug.Assert(this.refCount > 0);
                this.refCount ++;
            }

            public bool Release()
            {
                InternalDebug.Assert(this.refCount > 0);

                this.refCount --;
                return this.refCount == 0;
            }

            public byte[] GetByteBuffer(int size)
            {
#if DEBUG
                InternalDebug.Assert(!this.byteBufferUsed);
                this.byteBufferUsed = true;
#endif
                if (this.byteBuffer == null || this.byteBuffer.Length < size)
                {
                    this.byteBuffer = new byte[size];
                }

                return this.byteBuffer;
            }

            public void ReleaseByteBuffer()
            {
#if DEBUG
                InternalDebug.Assert(this.byteBufferUsed);
                this.byteBufferUsed = false;
#endif
            }

            public char[] GetCharBuffer(int size)
            {
#if DEBUG
                InternalDebug.Assert(!this.charBufferUsed);
                this.charBufferUsed = true;
#endif
                if (this.charBuffer == null || this.charBuffer.Length < size)
                {
                    this.charBuffer = new char[size];
                }

                return this.charBuffer;
            }

            public void ReleaseCharBuffer()
            {
#if DEBUG
                InternalDebug.Assert(this.charBufferUsed);
                this.charBufferUsed = false;
#endif
            }

            public StringBuilder GetStringBuilder(int initialCapacity)
            {
#if DEBUG
                InternalDebug.Assert(!this.stringBuilderUsed);
                this.stringBuilderUsed = true;
#endif
                if (initialCapacity <= ScratchStringBuilderCapacity)
                {
                    if (this.stringBuilder == null)
                    {
                        this.stringBuilder = new StringBuilder(ScratchStringBuilderCapacity);
                    }
                    else
                    {
                        InternalDebug.Assert(this.stringBuilder.Capacity == ScratchStringBuilderCapacity);
                        this.stringBuilder.Length = 0; 
                    }

                    return this.stringBuilder;
                }

                return new StringBuilder(initialCapacity);
            }

            public void ReleaseStringBuilder()
            {
#if DEBUG
                InternalDebug.Assert(this.stringBuilderUsed);
                this.stringBuilderUsed = false;
#endif
                if (this.stringBuilder != null &&
                    (this.stringBuilder.Capacity > ScratchStringBuilderCapacity ||
                    this.stringBuilder.Length * 2 >= this.stringBuilder.Capacity + 1))
                {
                    
                    
                    
                    this.stringBuilder = null;
                }
            }
        }
    }
}

