// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTagParts.cs" company="Microsoft Corporation">
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
    using System.IO;

    using Microsoft.Exchange.Data.TextConverters.Internal.Html;

    using Security.Application.TextConverters.HTML;

    internal struct HtmlTagParts
    {
        private HtmlToken.TagPartMajor major;
        private HtmlToken.TagPartMinor minor;

        internal HtmlTagParts(HtmlToken.TagPartMajor major, HtmlToken.TagPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        public bool Begin { get { return HtmlToken.TagPartMajor.Begin == (this.major & HtmlToken.TagPartMajor.Begin); } }

        public bool Name { get { return HtmlToken.TagPartMinor.ContinueName == (this.minor & HtmlToken.TagPartMinor.ContinueName); } }

        public override string ToString()
        {
            return this.major.ToString() + " /" + this.minor.ToString() + "/";
        }
    }

#if M5STUFF


    
    
    
    
    public interface IHtmlParsingCallback
    {
        
        
        
        
        
        
        
        

        bool EvaluateConditional(string conditional);
    }

    
    

    
    
    
    
    public enum HtmlFilterAction
    {
        
        NoAction,
        
        Drop,
        
        DropContainerOnly,
        
        DropContainerAndContent,
        
        EmptyValue,
        
        ReplaceValue,
    }

    
    
    
    
    public struct HtmlFilterContextAction
    {
        
        public HtmlFilterContextType contextType;

        
        public HtmlNameId nameId;
        
        public string name;

        
        public HtmlNameId containerNameId;
        
        public string containerName;

        
        public HtmlFilterAction action;
        
        public string replacementValue;

        
        public bool callbackOverride;
    }

    
    
    
    
    
    
    
    public class HtmlFilterTables
    {
        
        
        
        
        

        public HtmlFilterTables(HtmlFilterContextAction[] staticActions, bool mergeWithDefault)
        {
        }

        
        
    }

    
    
    
    
    public interface IImageExtractionCallback
    {
        
        
        
        
        
        

        Stream CreateImage(string imageType, out string linkUrl);
    }

#endif 
}
