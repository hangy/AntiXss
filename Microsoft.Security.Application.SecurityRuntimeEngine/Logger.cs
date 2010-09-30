// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   Provides logging functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;

    using PlugIns;

    /// <summary>
    /// Provides logging functionality.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// A list of loggers to use.
        /// </summary>
        [ImportMany(typeof(ILogger))]
        private static IEnumerable<ILogger> loggers;

        /// <summary>
        /// Adds the specified message to the log if the message level is above the configured logging threshold.
        /// </summary>
        /// <param name="level">The message level.</param>
        /// <param name="message">The level to log.</param>
        public static void Log(LogLevel level, string message)
        {
            if (level < SecurityRuntimeSettings.Settings.LogLevel)
            {
                return;
            }

            foreach (ILogger logger in loggers)
            {
                logger.Log(message);
            }
        }

        /// <summary>
        /// Adds the specified exception to the log if the message level is above the configured logging threshold.
        /// </summary>
        /// <param name="level">The message level.</param>
        /// <param name="exception">The exception to log.</param>
        public static void Log(LogLevel level, Exception exception)
        {
            if (level < SecurityRuntimeSettings.Settings.LogLevel)
            {
                return;
            }

            foreach (ILogger logger in loggers)
            {
                logger.Log(exception);
            }
        }

        /// <summary>
        /// Adds the specified formatted string and arguments to the log if the message level is above the configured logging threshold.
        /// </summary>
        /// <param name="level">The message level.</param>
        /// <param name="format">The format string to use.</param>
        /// <param name="args">The arguments for the format string.</param>
        public static void Log(LogLevel level, string format, params object[] args)
        {
            Log(level, CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Adds the specified formatted string, using the supplied format provider and arguments to the log if the message level is above the configured logging threshold.
        /// </summary>
        /// <param name="level">The message level.</param>
        /// <param name="provider">The <see cref="IFormatProvider"/> to use when creating the string.</param>
        /// <param name="format">The format string to use.</param>
        /// <param name="args">The arguments for the format string.</param>
        public static void Log(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            string message = string.Format(provider, format, args);
            Log(level, message);
        }

        /// <summary>
        /// Configures the available loggers using the supplied list of loggers.
        /// </summary>
        /// <param name="loadedLoggers">The loggers to use.</param>
        internal static void ConfigureLoggers(IEnumerable<ILogger> loadedLoggers)
        {
            loggers = new List<ILogger>(loadedLoggers);
        }
    }
}
