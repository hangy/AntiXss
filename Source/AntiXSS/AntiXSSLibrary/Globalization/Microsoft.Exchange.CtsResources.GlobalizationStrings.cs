// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.GlobalizationStrings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

using System;
using System.Resources;
using System.ComponentModel;

namespace Microsoft.Exchange.CtsResources
{
	
	
	
	static class GlobalizationStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"MaxCharactersCannotBeNegative", 
			"PriorityListIncludesNonDetectableCodePage", 
			"IndexOutOfRange", 
			"CountTooLarge", 
			"OffsetOutOfRange", 
			"CountOutOfRange", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			MaxCharactersCannotBeNegative, 
			
			
			
			PriorityListIncludesNonDetectableCodePage, 
			
			
			
			IndexOutOfRange, 
			
			
			
			CountTooLarge, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			CountOutOfRange, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			InvalidCharset, 
			
			
			
			InvalidLocaleId, 
			
			
			
			NotInstalledCodePage, 
			
			
			
			NotInstalledCharset, 
			
			
			
			InvalidCodePage, 
			
			
			
			NotInstalledCharsetCodePage, 
			
			
			
			InvalidCultureName, 
		}

		
		
		
		
		public static string InvalidCharset (string charsetName)
		{
			return string.Format(ResourceManager.GetString("InvalidCharset"), charsetName);
		}
		
		
		
		public static string MaxCharactersCannotBeNegative
		{
			get { return ResourceManager.GetString("MaxCharactersCannotBeNegative"); }
		}
		
		
		
		public static string PriorityListIncludesNonDetectableCodePage
		{
			get { return ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage"); }
		}
		
		
		
		public static string IndexOutOfRange
		{
			get { return ResourceManager.GetString("IndexOutOfRange"); }
		}
		
		
		
		
		public static string InvalidLocaleId (int localeId)
		{
			return string.Format(ResourceManager.GetString("InvalidLocaleId"), localeId);
		}
		
		
		
		
		public static string NotInstalledCodePage (int codePage)
		{
			return string.Format(ResourceManager.GetString("NotInstalledCodePage"), codePage);
		}
		
		
		
		
		public static string NotInstalledCharset (string charsetName)
		{
			return string.Format(ResourceManager.GetString("NotInstalledCharset"), charsetName);
		}
		
		
		
		
		public static string InvalidCodePage (int codePage)
		{
			return string.Format(ResourceManager.GetString("InvalidCodePage"), codePage);
		}
		
		
		
		
		
		public static string NotInstalledCharsetCodePage (int codePage, string charsetName)
		{
			return string.Format(ResourceManager.GetString("NotInstalledCharsetCodePage"), codePage, charsetName);
		}
		
		
		
		
		public static string InvalidCultureName (string cultureName)
		{
			return string.Format(ResourceManager.GetString("InvalidCultureName"), cultureName);
		}
		
		
		
		public static string CountTooLarge
		{
			get { return ResourceManager.GetString("CountTooLarge"); }
		}
		
		
		
		public static string OffsetOutOfRange
		{
			get { return ResourceManager.GetString("OffsetOutOfRange"); }
		}
		
		
		
		public static string CountOutOfRange
		{
			get { return ResourceManager.GetString("CountOutOfRange"); }
		}

		
		
		
		public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", typeof(Microsoft.Exchange.CtsResources.GlobalizationStrings).Assembly);
	}
}
