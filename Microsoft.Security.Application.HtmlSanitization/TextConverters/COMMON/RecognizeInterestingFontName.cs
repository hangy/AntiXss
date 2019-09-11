// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecognizeInterestingFontName.cs" company="Microsoft Corporation">
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
    using System.Runtime.Serialization;

    internal struct RecognizeInterestingFontName
    {
        private static byte[] CharMapToClass = new byte[]
        {
          0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   1,   0,   0,   1,   0,   0,
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          1,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   2,   0,   0,   0,   0,
          
          0,   0,  11,   0,   7,   0,   0,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,   0,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
          
          0,   0,  11,   0,   7,   0,   0,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,   0,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
        };

        private static sbyte[,] StateTransitionTable = new sbyte[,]
        {
          { -1,   0,  -1,   3,  -1,  -1,  -1,  -1,  11,  -1,  -1,  -1,  -1,  -1 },  

          { -1,   1,   1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,   2,   2,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,   4,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,   5,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,   6,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,   7,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,   8,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,   9,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  10,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,   2,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  12,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  13,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  14,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  15,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,   1 },  
        };

        private sbyte state;

        /// <summary>
        /// Gets the text mapping.
        /// </summary>
        /// <value>The text mapping.</value>
        public TextMapping TextMapping
        {
            get
            {
                switch (this.state)
                {
                    case 1: return TextMapping.Symbol;
                    case 2: return TextMapping.Wingdings;
                }
                return TextMapping.Unicode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is rejected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is rejected; otherwise, <c>false</c>.
        /// </value>
        public bool IsRejected
        {
            get { return this.state < 0; }
        }

        // Orphaned WPL code.
#if false
        public void AddCharacter(byte ch)
        {
            if (this.state >= 0)
            {
                this.state = StateTransitionTable[this.state, ch > 0x7F ? 0 : (int)CharMapToClass[ch]];
            }
        }
#endif

        public void AddCharacter(char ch)
        {
            if (this.state >= 0)
            {
                this.state = StateTransitionTable[this.state, ch > 0x7F ? 0 : (int)CharMapToClass[(int)ch]];
            }
        }
    }
}

