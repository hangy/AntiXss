using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    /// <summary>
    /// This attribute will exclude a class or control from encoding. 
    /// If a class is decorated with this attribute, all the controls in the class 
    /// will not be encoded by the module. Similarly, if a control is decorated with
    /// the attribute, configuration specified properties will not be encoded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class SupressAntiXssEncodingAttribute : Attribute
    {
        /// <summary>
        /// Constructor for the attribute
        /// </summary>
        public SupressAntiXssEncodingAttribute()
        {

        }

    }
    
}
