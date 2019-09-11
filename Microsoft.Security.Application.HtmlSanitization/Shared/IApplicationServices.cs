// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationServices.cs" company="Microsoft Corporation">
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
// </copyright>
// <summary>
//   An interface for application configuration services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Internal
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// An interface for application configuration services.
    /// </summary>
    internal interface IApplicationServices
    {
        // Orphaned WPL code.
#if false
        /// <summary>
        /// Creates the temporary storage stream.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for temporary storage.</returns>
        Stream CreateTemporaryStorage();
#endif
        /// <summary>
        /// Gets the configuration subsection specified.
        /// </summary>
        /// <param name="subSectionName">Name of the subsection.</param>
        /// <returns>A list of <see cref="CtsConfigurationSetting"/>s for the specified section.</returns>
        IList<CtsConfigurationSetting> GetConfiguration(string subSectionName);
        
        /// <summary>
        /// Refreshes the configuration from the application configuration file.
        /// </summary>
        void RefreshConfiguration();

        /// <summary>
        /// Logs an error during configuration processing.
        /// </summary>
        void LogConfigurationErrorEvent();
    }
}
