using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Security.Application.SecurityRuntimeEngine.Configuration
{
    /// <summary>
    /// Class that identifies a list of erncoding configurations.
    /// </summary>
    internal sealed class ControlEncodingContexts : List<ControlEncodingContext>
    {
        /// <summary>
        /// Returns true if the specified type of control and property 
        /// name exists in the configuration.
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <param name="propertyName">Property name of the control.</param>
        /// <returns>Returns true if the configuration exists.</returns>
        public bool Contains(string fullName, string propertyName)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException();

            bool blnReturn=false;
            int listCount = this.Count;
            for(int index=0;index<listCount;index++)
            {
                ControlEncodingContext et = this[index];
                if (et.FullClassName == fullName && et.PropertyName == propertyName)
                    blnReturn=true;
            }
            return blnReturn;
        }

        /// <summary>
        /// Returns true if the specified type of control exists in 
        /// the configuration. 
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <returns>Returns true if the configuration exists.</returns>
        public bool Contains(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentNullException("fullName");

            bool blnReturn = false;
            int listCount = this.Count;
            for(int index=0;index<listCount;index++)
            {
                ControlEncodingContext et = this[index];
                if (et.FullClassName == fullName)
                    blnReturn = true;
            }
            return blnReturn;
        }

        /// <summary>
        /// Returns all the ControlEncodingContext objects for a specified control type.
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <returns>Returns a list of ControlEncodingContext</returns>
        public List<ControlEncodingContext> GetEncodingTypes(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentNullException("fullName");
            List<ControlEncodingContext> lstProperties = new List<ControlEncodingContext>();

            int listCount = this.Count;
            for(int index=0;index<listCount;index++)
            {
                ControlEncodingContext et = this[index];
                if (et.FullClassName == fullName)
                    lstProperties.Add(et);
            }
            return lstProperties;
        }
    }

}
