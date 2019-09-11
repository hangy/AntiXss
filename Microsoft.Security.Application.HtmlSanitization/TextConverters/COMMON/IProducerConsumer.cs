// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProducerConsumer.cs" company="Microsoft Corporation">
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
//    Interface declaration for Producer Consumer.
// </summary>

namespace Microsoft.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for Producer Consumer.
    /// </summary>
    internal interface IProducerConsumer
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        void Run();

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        /// <returns>
        /// <c>true</c> if flush is successful; otherwise <c>false</c>.
        /// </returns>
        bool Flush();       
    }
}