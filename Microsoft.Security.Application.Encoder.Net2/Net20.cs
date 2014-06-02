// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net20.cs" company="Microsoft Corporation">
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
//   Implements the linq ienumerable and func pieces for .NET 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System
{
    /// <summary>
    /// Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    internal delegate TResult Func<out TResult>();
    
    /// <summary>
    /// Encapsulates a method that has one parameter and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    internal delegate TResult Func<in T, TResult>(T arg);
}

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Indicates that a method is an extension method, or that a class or assembly contains extension methods.
    /// </summary>
    internal class ExtensionAttribute : Attribute
    {        
    }
}

namespace System.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying objects that implement IEnumerable&lt;T&gt;.
    /// </summary>
    internal static class Enumerable
    {
        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An IEnumerable&lt;Int32&gt; in C# or IEnumerable(Of Int32) in Visual Basic that contains a range of sequential integral numbers.</returns>
        internal static IEnumerable<int> Range(int start, int count)
        {
            int end = start + count;

            for (int i = start; i < end; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable&lt;T&gt; to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable&lt;T&gt; that contains elements from the input sequence that satisfy the condition.</returns>
        internal static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            List<TSource> results = new List<TSource>();
            foreach (TSource s in source)
            {
                if (predicate(s))
                {
                    results.Add(s);
                }
            }

            return results;
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <returns>An IEnumerable&lt;T&gt; that contains the concatenated elements of the two input sequences.</returns>
        internal static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            List<TSource> merged = new List<TSource>(first);
            merged.AddRange(second);
            return merged;
        }
    }
}

