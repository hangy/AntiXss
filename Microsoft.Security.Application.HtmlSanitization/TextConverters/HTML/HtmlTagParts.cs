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

        // Orphaned WPL code.
#if false
        internal void Reset()
        {
            this.minor = 0;
            this.major = 0;
        }
#endif

        public bool Begin { get { return HtmlToken.TagPartMajor.Begin == (this.major & HtmlToken.TagPartMajor.Begin); } }

        // Orphaned WPL code.
#if false
        public bool End { get { return HtmlToken.TagPartMajor.End == (this.major & HtmlToken.TagPartMajor.End); } }
        public bool Complete { get { return HtmlToken.TagPartMajor.Complete == this.major; } }

        public bool NameBegin { get { return HtmlToken.TagPartMinor.BeginName == (this.minor & HtmlToken.TagPartMinor.BeginName); } }
#endif

        public bool Name { get { return HtmlToken.TagPartMinor.ContinueName == (this.minor & HtmlToken.TagPartMinor.ContinueName); } }

        // Orphaned WPL code.
#if false
        public bool NameEnd { get { return HtmlToken.TagPartMinor.EndName == (this.minor & HtmlToken.TagPartMinor.EndName); } }
        public bool NameComplete { get { return HtmlToken.TagPartMinor.CompleteName == (this.minor & HtmlToken.TagPartMinor.CompleteName); } }

        public bool Attributes { get { return 0 != (this.minor & (HtmlToken.TagPartMinor.Attributes | HtmlToken.TagPartMinor.ContinueAttribute)); } }
#endif

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
