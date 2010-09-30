// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecognizeInterestingFontNameInInlineStyle.cs" company="Microsoft Corporation">
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
    internal struct RecognizeInterestingFontNameInInlineStyle
    {
        private static byte[] CharMapToClass = new byte[]
                                                   {
          
                                                       0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   1,   0,   0,   1,   0,   0,
                                                       0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          
                                                       1,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  17,   0,   0,
          
                                                       0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  14,   2,   0,   0,   0,   0,
          
                                                       0,  18,  11,   0,   7,   0,  15,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
                                                       0,   0,   0,   8,  16,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
          
                                                       0,  18,  11,   0,   7,   0,  15,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
                                                       0,   0,   0,   8,  16,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
                                                   };

        private static sbyte[,] StateTransitionTable = new sbyte[,]
                                                           {
        
        
                                                               {  1,   0,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   2,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  

                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   3,   1,   1,   1,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   4,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   5,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   6,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   7,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   8 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   9,   1,   1,   1,   1,   1,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,  10,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,  11,   1,   1,   1,   1,   1 },  
                                                               {  1,   1,   0,   1,   1,   1,   1,   1,   1,  12,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  

                                                               {  1,  12,  -1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,  13,   1,   1,   1,   1 },  

                                                               { -1,  13,  -1,  14,  -1,  -1,  -1,  -1,  23,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

                                                               { -1,  -1,  -1,  -1,  15,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  16,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  17,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  18,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  19,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  20,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  21,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  22,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  22,  -2,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  24,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  25,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  26,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  27,  -1,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  28,  -1,  -1,  -1,  -1,  -1 },  
                                                               { -1,  28,  -3,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
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
                    case -3:
                    case 28:
                        return TextMapping.Symbol;
                    case -2:
                    case 22:
                        return TextMapping.Wingdings;
                }
                return TextMapping.Unicode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is finished.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished
        {
            get { return this.state < 0; }
        }

        public void AddCharacter(char ch)
        {
            if (this.state >= 0)
            {
                this.state = StateTransitionTable[this.state, ch > 0x7F ? 0 : (int)CharMapToClass[(int)ch]];
            }
        }
    }
}