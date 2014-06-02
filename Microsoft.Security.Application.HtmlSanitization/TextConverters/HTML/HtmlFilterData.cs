// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlFilterData.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;

    internal static class HtmlFilterData
    {
        public enum FilterAction : byte
        {
            Unknown = 0,                
            Drop = 1,                   
            DropKeepContent = 2,        
            KeepDropContent = 3,        
            Keep = 4,                   
            SanitizeUrl = 5,            
            FilterStyleAttribute = 6,   
            PrefixName = 7,             
            PrefixNameList = 8,         
            ConvertBgcolorIntoStyle = 9,  
            
            Callback = 0x40,            
            IgnoreAttrCallbacks = 0x20,  
            HasExceptions = 0x80,       
            ActionMask = 0x0F,          
        }

        public struct FilterActionEntry
        {
            public FilterAction tagAction;
            public FilterAction tagFragmentAction;
            public FilterAction attrAction;
            public FilterAction attrFragmentAction;

            public FilterActionEntry(FilterAction tagAction, FilterAction tagFragmentAction, FilterAction attrAction, FilterAction attrFragmentAction)
            {
                this.tagAction = tagAction;
                this.tagFragmentAction = tagFragmentAction;
                this.attrAction = attrAction;
                this.attrFragmentAction = attrFragmentAction;
            }
        }

        public struct FilterAttributeExceptionEntry
        {
            public HtmlNameIndex tagNameIndex;
            public HtmlNameIndex attrNameIndex;
            public FilterAction action;
            public FilterAction fragmentAction;

            public FilterAttributeExceptionEntry(HtmlNameIndex tagNameIndex, HtmlNameIndex attrNameIndex, FilterAction action, FilterAction fragmentAction)
            {
                this.tagNameIndex = tagNameIndex;
                this.attrNameIndex = attrNameIndex;
                this.action = action;
                this.fragmentAction = fragmentAction;
            }
        }

        public static FilterActionEntry[] filterInstructions =
        {
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.PrefixNameList),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.DropKeepContent, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.PrefixName),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.DropKeepContent, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.PrefixName),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback, FilterAction.Keep | FilterAction.Callback, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.DropKeepContent | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Keep | FilterAction.Callback | FilterAction.HasExceptions, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.DropKeepContent, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop | FilterAction.HasExceptions, FilterAction.Drop | FilterAction.HasExceptions),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Keep, FilterAction.Keep),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.Keep, FilterAction.Keep, FilterAction.Drop, FilterAction.Drop),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.SanitizeUrl | FilterAction.Callback, FilterAction.SanitizeUrl | FilterAction.Callback),    
            new FilterActionEntry(FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.DropKeepContent | FilterAction.IgnoreAttrCallbacks, FilterAction.Drop, FilterAction.Drop),    
        };

        public static FilterAttributeExceptionEntry[] filterExceptions =
        {
            new FilterAttributeExceptionEntry(HtmlNameIndex.A, HtmlNameIndex.Href, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.A, HtmlNameIndex.Target, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Area, HtmlNameIndex.Href, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Area, HtmlNameIndex.Target, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Base, HtmlNameIndex.Href, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Body, HtmlNameIndex.Link, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Body, HtmlNameIndex.Vlink, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Body, HtmlNameIndex.BGColor, FilterAction.Keep, FilterAction.ConvertBgcolorIntoStyle),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Form, HtmlNameIndex.Action, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Form, HtmlNameIndex.Method, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Form, HtmlNameIndex.Target, FilterAction.Keep, FilterAction.Keep),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Img, HtmlNameIndex.Src, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Img, HtmlNameIndex.LowSrc, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Img, HtmlNameIndex.DynSrc, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Img, HtmlNameIndex.UseMap, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Img, HtmlNameIndex.IsMap, FilterAction.KeepDropContent, FilterAction.KeepDropContent),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Image, HtmlNameIndex.Src, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Image, HtmlNameIndex.LowSrc, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Image, HtmlNameIndex.DynSrc, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Image, HtmlNameIndex.UseMap, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Image, HtmlNameIndex.IsMap, FilterAction.KeepDropContent, FilterAction.KeepDropContent),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Input, HtmlNameIndex.Src, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Input, HtmlNameIndex.UseMap, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
            new FilterAttributeExceptionEntry(HtmlNameIndex.Link, HtmlNameIndex.Href, FilterAction.SanitizeUrl, FilterAction.SanitizeUrl),    
        };

    }

}
