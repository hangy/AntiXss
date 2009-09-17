// ***************************************************************
// <copyright file="TokenBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Microsoft.Exchange.Data.Internal;

    

    internal abstract class TokenBuilder
    {
        protected const byte BuildStateInitialized = 0;
        protected const byte BuildStateEnded = 5;
        protected const byte FirstStarted = 10;
        protected const byte BuildStateText = FirstStarted;

        protected byte state;

        protected Token token;

        protected int maxRuns;

        protected int tailOffset;

        protected bool tokenValid;              

#if DEBUG
        private int preparedForNumRuns;
#endif

        

        public TokenBuilder(Token token, char[] buffer, int maxRuns, bool testBoundaryConditions)
        {
            int initialRuns = 64;

            if (!testBoundaryConditions)
            {
                this.maxRuns = maxRuns;
            }
            else
            {
                this.maxRuns = 55;
                initialRuns = 6 + 1;
            }

            

            this.token = token;

            this.token.buffer = buffer;
            this.token.runList = new Token.RunEntry[initialRuns];

            
        }

        

        public Token Token
        {
            get { return this.token; }
        }

        

        public bool IsStarted
        {
            get { return this.state != BuildStateInitialized; }
        }

        

        public bool Valid
        {
            get { return this.tokenValid; }
        }

#if PRIVATEBUILD_UNUSED
        

        public char[] Buffer
        {
            get { return this.token.buffer; }
            set { this.token.buffer = value; }
        }
#endif

#if PRIVATEBUILD_UNUSED
        

        public int BaseOffset
        {
            get { return this.token.whole.headOffset; }
            set { this.token.whole.headOffset = value; }
        }
#endif

        

        public int TotalLength
        {
            get { return this.tailOffset - this.token.whole.headOffset; }
        }

        

        public void BufferChanged(char[] newBuffer, int newBase)
        {
            if (newBuffer != this.token.buffer || newBase != this.token.whole.headOffset)
            {
                

                
                this.token.buffer = newBuffer;

                if (newBase != this.token.whole.headOffset)
                {
                    

                    int delta = newBase - this.token.whole.headOffset;

                    this.Rebase(delta);
                }
            }
        }

        

        public virtual void Reset()
        {
            InternalDebug.Assert(this.state != BuildStateInitialized || (this.token.whole.headOffset == 0 && this.tailOffset == 0));

            if (this.state > BuildStateInitialized)
            {
                this.token.Reset();

                this.tailOffset = 0;

                this.tokenValid = false;

                this.state = BuildStateInitialized;
            }
        }

        

        public TokenId MakeEmptyToken(TokenId tokenId)
        {
            InternalDebug.Assert(this.state == BuildStateInitialized && this.token.IsEmpty && !this.tokenValid);

            this.token.tokenId = tokenId;

            this.state = BuildStateEnded;
            this.tokenValid = true;

            return tokenId;
        }

        

        public TokenId MakeEmptyToken(TokenId tokenId, int argument)
        {
            InternalDebug.Assert(this.state == BuildStateInitialized && this.token.IsEmpty && !this.tokenValid);

            this.token.tokenId = tokenId;
            this.token.argument = argument;

            this.state = BuildStateEnded;
            this.tokenValid = true;

            return tokenId;
        }

        

        public void StartText(int baseOffset)
        {
            InternalDebug.Assert(this.state == BuildStateInitialized && this.token.IsEmpty && !this.tokenValid);

            this.token.tokenId = TokenId.Text;

            this.state = BuildStateText;

            this.token.whole.headOffset = baseOffset;
            this.tailOffset = baseOffset;
        }

        

        public void EndText()
        {
            InternalDebug.Assert(this.state == BuildStateText);

            this.state = BuildStateEnded;

            this.tokenValid = true;

            this.token.wholePosition.Rewind(this.token.whole);

            
            this.AddSentinelRun();
        }

        

        public void SkipRunIfNecessary(int start, uint skippedRunKind)
        {
#if DEBUG
            InternalDebug.Assert(this.preparedForNumRuns > 0);
#endif
            
            if (start != this.tailOffset)
            {
                this.AddInvalidRun(start, skippedRunKind);
            }
        }

        

        public bool PrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
#if DEBUG
            this.preparedForNumRuns = numRuns;
#endif
            
            return (start == this.tailOffset && this.token.whole.tail + numRuns < this.token.runList.Length) || 
                            this.SlowPrepareToAddMoreRuns(numRuns, start, skippedRunKind);
        }

        public bool SlowPrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
            InternalDebug.Assert(start >= this.tailOffset);

            if (start != this.tailOffset)
            {
                numRuns ++;
            }

            if (this.token.whole.tail + numRuns < this.token.runList.Length || this.ExpandRunsArray(numRuns))
            {
                if (start != this.tailOffset)
                {
#if DEBUG
                    this.preparedForNumRuns ++;
#endif
                    this.AddInvalidRun(start, skippedRunKind);
                }

                return true;
            }

            return false;
        }

        

        public bool PrepareToAddMoreRuns(int numRuns)
        {
#if DEBUG
            this.preparedForNumRuns = numRuns;
#endif
            return this.token.whole.tail + numRuns < this.token.runList.Length || this.ExpandRunsArray(numRuns);
        }

        

        [Conditional("DEBUG")]
        public void AssertPreparedToAddMoreRuns(int numRuns)
        {
#if DEBUG
            InternalDebug.Assert(numRuns <= this.preparedForNumRuns);
#endif
        }

        [Conditional("DEBUG")]
        public void AssertCanAddMoreRuns(int numRuns)
        {
            InternalDebug.Assert(this.token.whole.tail + numRuns <= this.token.runList.Length);
        }

        [Conditional("DEBUG")]
        public void AssertCurrentRunPosition(int position)
        {
            InternalDebug.Assert(position == this.tailOffset);
        }

        [Conditional("DEBUG")]
        public void DebugPrepareToAddMoreRuns(int numRuns)
        {
            InternalDebug.Assert(this.token.whole.head == 0 && this.token.whole.tail == 0);
            InternalDebug.Assert(this.token.whole.tail + numRuns + 1 <= this.token.runList.Length);
#if DEBUG
            this.preparedForNumRuns = numRuns;
#endif
        }

        

        public void AddTextRun(RunTextType textType, int start, int end)
        {
            InternalDebug.Assert(start == this.tailOffset);
            this.AddRun(RunType.Normal, textType, (uint)RunKind.Text, start, end, 0);
        }

        

        public void AddLiteralTextRun(RunTextType textType, int start, int end, int literal)
        {
            InternalDebug.Assert(start == this.tailOffset);
            this.AddRun(RunType.Literal, textType, (uint)RunKind.Text, start, end, literal);
        }

        

        public void AddSpecialRun(RunKind kind, int startEnd, int value)
        {
            InternalDebug.Assert(startEnd == this.tailOffset);
            this.AddRun(RunType.Special, RunTextType.Unknown, (uint)kind, this.tailOffset, startEnd, value);
        }

        

        internal void AddRun(RunType type, RunTextType textType, uint kind, int start, int end, int value)
        {
#if DEBUG
            InternalDebug.Assert(this.preparedForNumRuns > 0);
            this.preparedForNumRuns --;
#endif
            InternalDebug.Assert(this.state >= FirstStarted);
            InternalDebug.Assert(end >= start);
            InternalDebug.Assert(this.token.whole.head != this.token.whole.tail || this.tailOffset == this.token.whole.headOffset);
            InternalDebug.Assert(this.token.whole.tail + 1 < this.token.runList.Length);
            InternalDebug.Assert(start == this.tailOffset);

            this.token.runList[this.token.whole.tail++].Initialize(type, textType, kind, end - start, value);
            this.tailOffset = end;
        }

        

        internal void AddInvalidRun(int offset, uint kind)
        {
#if DEBUG
            InternalDebug.Assert(this.preparedForNumRuns > 0);
            this.preparedForNumRuns --;
#endif
            InternalDebug.Assert(offset > this.tailOffset);
            InternalDebug.Assert(this.token.whole.tail + 1 < this.token.runList.Length);

            this.token.runList[this.token.whole.tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, offset - this.tailOffset, 0);
            this.tailOffset = offset;
        }

        

        internal void AddNullRun(uint kind)
        {
#if DEBUG
            InternalDebug.Assert(this.preparedForNumRuns > 0);
            this.preparedForNumRuns --;
#endif
            InternalDebug.Assert(this.token.whole.tail + 1 < this.token.runList.Length);

            this.token.runList[this.token.whole.tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, 0, 0);
        }

        

        internal void AddSentinelRun()
        {
            
            InternalDebug.Assert(this.token.whole.tail + 1 <= this.token.runList.Length);

            this.token.runList[this.token.whole.tail].InitializeSentinel();
        }

        

        protected virtual void Rebase(int deltaOffset)
        {
            this.token.whole.headOffset += deltaOffset;
            this.token.wholePosition.runOffset += deltaOffset;

            this.tailOffset += deltaOffset;
        }

        

        private bool ExpandRunsArray(int numRuns)
        {
            int newSize;

            newSize = Math.Min(this.maxRuns, Math.Max(this.token.runList.Length * 2, this.token.whole.tail + numRuns + 1));

            if (newSize - this.token.whole.tail < numRuns + 1)
            {
                return false;
            }

            Token.RunEntry[] newRuns = new Token.RunEntry[newSize];
            Array.Copy(this.token.runList, 0, newRuns, 0, this.token.whole.tail);
            this.token.runList = newRuns;

            return true;
        }
    }
}

