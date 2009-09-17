// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.SharedStrings.cs" company="Microsoft">
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
	
	
	
	static class SharedStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"CountTooLarge", 
			"CannotSeekBeforeBeginning", 
			"CannotSetNegativelength", 
			"InvalidFactory", 
			"OffsetOutOfRange", 
			"CountOutOfRange", 
			"StringArgumentMustBeAscii", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			CountTooLarge, 
			
			
			
			CannotSeekBeforeBeginning, 
			
			
			
			CannotSetNegativelength, 
			
			
			
			InvalidFactory, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			CountOutOfRange, 
			
			
			
			StringArgumentMustBeAscii, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			CreateFileFailed, 
		}

		
		
		
		public static string CountTooLarge
		{
			get { return ResourceManager.GetString("CountTooLarge"); }
		}
		
		
		
		public static string CannotSeekBeforeBeginning
		{
			get { return ResourceManager.GetString("CannotSeekBeforeBeginning"); }
		}
		
		
		
		public static string CannotSetNegativelength
		{
			get { return ResourceManager.GetString("CannotSetNegativelength"); }
		}
		
		
		
		
		public static string CreateFileFailed (string filePath)
		{
			return string.Format(ResourceManager.GetString("CreateFileFailed"), filePath);
		}
		
		
		
		public static string InvalidFactory
		{
			get { return ResourceManager.GetString("InvalidFactory"); }
		}
		
		
		
		public static string OffsetOutOfRange
		{
			get { return ResourceManager.GetString("OffsetOutOfRange"); }
		}
		
		
		
		public static string CountOutOfRange
		{
			get { return ResourceManager.GetString("CountOutOfRange"); }
		}
		
		
		
		public static string StringArgumentMustBeAscii
		{
			get { return ResourceManager.GetString("StringArgumentMustBeAscii"); }
		}

		
		
		
		public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.SharedStrings", typeof(Microsoft.Exchange.CtsResources.SharedStrings).Assembly);
	}
}
