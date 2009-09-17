// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.TextConvertersStrings.cs" company="Microsoft">
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
	
	
	
	static class TextConvertersStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"AttributeNotStarted", 
			"InputEncodingRequired", 
			"AttributeCollectionNotInitialized", 
			"ConverterWriterInInconsistentStare", 
			"CannotUseConverterReader", 
			"CannotReadFromSource", 
			"AttributeIdIsUnknown", 
			"TagTooLong", 
			"EndTagCannotHaveAttributes", 
			"CannotUseConverterWriter", 
			"WriteAfterFlush", 
			"PriorityListIncludesNonDetectableCodePage", 
			"TagNotStarted", 
			"CannotSetNegativelength", 
			"TooManyIterationsToFlushConverter", 
			"IndexOutOfRange", 
			"TagNameIsEmpty", 
			"MaxCharactersCannotBeNegative", 
			"AttributeNotValidForThisContext", 
			"InputDocumentTooComplex", 
			"TextWriterUnsupported", 
			"PropertyNotValidForCodepageConversionMode", 
			"CountTooLarge", 
			"TooManyIterationsToProduceOutput", 
			"AttributeNotInitialized", 
			"AttributeIdInvalid", 
			"TooManyIterationsToProcessInput", 
			"BufferSizeValueRange", 
			"CannotWriteWhileCopyPending", 
			"AttributeNotValidInThisState", 
			"ParametersCannotBeChangedAfterConverterObjectIsUsed", 
			"CountOutOfRange", 
			"WriteUnsupported", 
			"AccessShouldBeReadOrWrite", 
			"ContextNotValidInThisState", 
			"CannotSeekBeforeBeginning", 
			"OffsetOutOfRange", 
			"ReadUnsupported", 
			"SeekUnsupported", 
			"CallbackTagAlreadyWritten", 
			"ConverterStreamInInconsistentStare", 
			"AttributeNameIsEmpty", 
			"HtmlNestingTooDeep", 
			"CannotWriteToDestination", 
			"ConverterReaderInInconsistentStare", 
			"TextReaderUnsupported", 
			"PropertyNotValidForTextExtractionMode", 
			"TagIdIsUnknown", 
			"TagIdInvalid", 
			"CallbackTagAlreadyDeleted", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			AttributeNotStarted, 
			
			
			
			InputEncodingRequired, 
			
			
			
			AttributeCollectionNotInitialized, 
			
			
			
			ConverterWriterInInconsistentStare, 
			
			
			
			CannotUseConverterReader, 
			
			
			
			CannotReadFromSource, 
			
			
			
			AttributeIdIsUnknown, 
			
			
			
			TagTooLong, 
			
			
			
			EndTagCannotHaveAttributes, 
			
			
			
			CannotUseConverterWriter, 
			
			
			
			WriteAfterFlush, 
			
			
			
			PriorityListIncludesNonDetectableCodePage, 
			
			
			
			TagNotStarted, 
			
			
			
			CannotSetNegativelength, 
			
			
			
			TooManyIterationsToFlushConverter, 
			
			
			
			IndexOutOfRange, 
			
			
			
			TagNameIsEmpty, 
			
			
			
			MaxCharactersCannotBeNegative, 
			
			
			
			AttributeNotValidForThisContext, 
			
			
			
			InputDocumentTooComplex, 
			
			
			
			TextWriterUnsupported, 
			
			
			
			PropertyNotValidForCodepageConversionMode, 
			
			
			
			CountTooLarge, 
			
			
			
			TooManyIterationsToProduceOutput, 
			
			
			
			AttributeNotInitialized, 
			
			
			
			AttributeIdInvalid, 
			
			
			
			TooManyIterationsToProcessInput, 
			
			
			
			BufferSizeValueRange, 
			
			
			
			CannotWriteWhileCopyPending, 
			
			
			
			AttributeNotValidInThisState, 
			
			
			
			ParametersCannotBeChangedAfterConverterObjectIsUsed, 
			
			
			
			CountOutOfRange, 
			
			
			
			WriteUnsupported, 
			
			
			
			AccessShouldBeReadOrWrite, 
			
			
			
			ContextNotValidInThisState, 
			
			
			
			CannotSeekBeforeBeginning, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			ReadUnsupported, 
			
			
			
			SeekUnsupported, 
			
			
			
			CallbackTagAlreadyWritten, 
			
			
			
			ConverterStreamInInconsistentStare, 
			
			
			
			AttributeNameIsEmpty, 
			
			
			
			HtmlNestingTooDeep, 
			
			
			
			CannotWriteToDestination, 
			
			
			
			ConverterReaderInInconsistentStare, 
			
			
			
			TextReaderUnsupported, 
			
			
			
			PropertyNotValidForTextExtractionMode, 
			
			
			
			TagIdIsUnknown, 
			
			
			
			TagIdInvalid, 
			
			
			
			CallbackTagAlreadyDeleted, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			CreateFileFailed, 
			
			
			
			InvalidConfigurationBoolean, 
			
			
			
			InvalidConfigurationStream, 
			
			
			
			CannotWriteOtherTagsInsideElement, 
			
			
			
			InvalidConfigurationInteger, 
			
			
			
			LengthExceeded, 
			
			
			
			InvalidCodePage, 
		}

		
		
		
		public static string AttributeNotStarted
		{
			get { return ResourceManager.GetString("AttributeNotStarted"); }
		}
		
		
		
		public static string InputEncodingRequired
		{
			get { return ResourceManager.GetString("InputEncodingRequired"); }
		}
		
		
		
		
		public static string CreateFileFailed (string filePath)
		{
			return string.Format(ResourceManager.GetString("CreateFileFailed"), filePath);
		}
		
		
		
		public static string AttributeCollectionNotInitialized
		{
			get { return ResourceManager.GetString("AttributeCollectionNotInitialized"); }
		}
		
		
		
		public static string ConverterWriterInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterWriterInInconsistentStare"); }
		}
		
		
		
		public static string CannotUseConverterReader
		{
			get { return ResourceManager.GetString("CannotUseConverterReader"); }
		}
		
		
		
		public static string CannotReadFromSource
		{
			get { return ResourceManager.GetString("CannotReadFromSource"); }
		}
		
		
		
		public static string AttributeIdIsUnknown
		{
			get { return ResourceManager.GetString("AttributeIdIsUnknown"); }
		}
		
		
		
		public static string TagTooLong
		{
			get { return ResourceManager.GetString("TagTooLong"); }
		}
		
		
		
		public static string EndTagCannotHaveAttributes
		{
			get { return ResourceManager.GetString("EndTagCannotHaveAttributes"); }
		}
		
		
		
		public static string CannotUseConverterWriter
		{
			get { return ResourceManager.GetString("CannotUseConverterWriter"); }
		}
		
		
		
		public static string WriteAfterFlush
		{
			get { return ResourceManager.GetString("WriteAfterFlush"); }
		}
		
		
		
		public static string PriorityListIncludesNonDetectableCodePage
		{
			get { return ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage"); }
		}
		
		
		
		public static string TagNotStarted
		{
			get { return ResourceManager.GetString("TagNotStarted"); }
		}
		
		
		
		public static string CannotSetNegativelength
		{
			get { return ResourceManager.GetString("CannotSetNegativelength"); }
		}
		
		
		
		public static string TooManyIterationsToFlushConverter
		{
			get { return ResourceManager.GetString("TooManyIterationsToFlushConverter"); }
		}
		
		
		
		public static string IndexOutOfRange
		{
			get { return ResourceManager.GetString("IndexOutOfRange"); }
		}
		
		
		
		public static string TagNameIsEmpty
		{
			get { return ResourceManager.GetString("TagNameIsEmpty"); }
		}
		
		
		
		public static string MaxCharactersCannotBeNegative
		{
			get { return ResourceManager.GetString("MaxCharactersCannotBeNegative"); }
		}
		
		
		
		public static string AttributeNotValidForThisContext
		{
			get { return ResourceManager.GetString("AttributeNotValidForThisContext"); }
		}
		
		
		
		
		public static string InvalidConfigurationBoolean (int propertyId)
		{
			return string.Format(ResourceManager.GetString("InvalidConfigurationBoolean"), propertyId);
		}
		
		
		
		public static string InputDocumentTooComplex
		{
			get { return ResourceManager.GetString("InputDocumentTooComplex"); }
		}
		
		
		
		public static string TextWriterUnsupported
		{
			get { return ResourceManager.GetString("TextWriterUnsupported"); }
		}
		
		
		
		public static string PropertyNotValidForCodepageConversionMode
		{
			get { return ResourceManager.GetString("PropertyNotValidForCodepageConversionMode"); }
		}
		
		
		
		public static string CountTooLarge
		{
			get { return ResourceManager.GetString("CountTooLarge"); }
		}
		
		
		
		
		public static string InvalidConfigurationStream (int propertyId)
		{
			return string.Format(ResourceManager.GetString("InvalidConfigurationStream"), propertyId);
		}
		
		
		
		public static string TooManyIterationsToProduceOutput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProduceOutput"); }
		}
		
		
		
		public static string AttributeNotInitialized
		{
			get { return ResourceManager.GetString("AttributeNotInitialized"); }
		}
		
		
		
		
		public static string CannotWriteOtherTagsInsideElement (string elementName)
		{
			return string.Format(ResourceManager.GetString("CannotWriteOtherTagsInsideElement"), elementName);
		}
		
		
		
		public static string AttributeIdInvalid
		{
			get { return ResourceManager.GetString("AttributeIdInvalid"); }
		}
		
		
		
		
		public static string InvalidConfigurationInteger (int propertyId)
		{
			return string.Format(ResourceManager.GetString("InvalidConfigurationInteger"), propertyId);
		}
		
		
		
		public static string TooManyIterationsToProcessInput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProcessInput"); }
		}
		
		
		
		public static string BufferSizeValueRange
		{
			get { return ResourceManager.GetString("BufferSizeValueRange"); }
		}
		
		
		
		public static string CannotWriteWhileCopyPending
		{
			get { return ResourceManager.GetString("CannotWriteWhileCopyPending"); }
		}
		
		
		
		public static string AttributeNotValidInThisState
		{
			get { return ResourceManager.GetString("AttributeNotValidInThisState"); }
		}
		
		
		
		public static string ParametersCannotBeChangedAfterConverterObjectIsUsed
		{
			get { return ResourceManager.GetString("ParametersCannotBeChangedAfterConverterObjectIsUsed"); }
		}
		
		
		
		public static string CountOutOfRange
		{
			get { return ResourceManager.GetString("CountOutOfRange"); }
		}
		
		
		
		public static string WriteUnsupported
		{
			get { return ResourceManager.GetString("WriteUnsupported"); }
		}
		
		
		
		public static string AccessShouldBeReadOrWrite
		{
			get { return ResourceManager.GetString("AccessShouldBeReadOrWrite"); }
		}
		
		
		
		public static string ContextNotValidInThisState
		{
			get { return ResourceManager.GetString("ContextNotValidInThisState"); }
		}
		
		
		
		public static string CannotSeekBeforeBeginning
		{
			get { return ResourceManager.GetString("CannotSeekBeforeBeginning"); }
		}
		
		
		
		public static string OffsetOutOfRange
		{
			get { return ResourceManager.GetString("OffsetOutOfRange"); }
		}
		
		
		
		public static string ReadUnsupported
		{
			get { return ResourceManager.GetString("ReadUnsupported"); }
		}
		
		
		
		
		
		public static string LengthExceeded (int sum, int length)
		{
			return string.Format(ResourceManager.GetString("LengthExceeded"), sum, length);
		}
		
		
		
		public static string SeekUnsupported
		{
			get { return ResourceManager.GetString("SeekUnsupported"); }
		}
		
		
		
		
		public static string InvalidCodePage (int codePage)
		{
			return string.Format(ResourceManager.GetString("InvalidCodePage"), codePage);
		}
		
		
		
		public static string CallbackTagAlreadyWritten
		{
			get { return ResourceManager.GetString("CallbackTagAlreadyWritten"); }
		}
		
		
		
		public static string ConverterStreamInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterStreamInInconsistentStare"); }
		}
		
		
		
		public static string AttributeNameIsEmpty
		{
			get { return ResourceManager.GetString("AttributeNameIsEmpty"); }
		}
		
		
		
		public static string HtmlNestingTooDeep
		{
			get { return ResourceManager.GetString("HtmlNestingTooDeep"); }
		}
		
		
		
		public static string CannotWriteToDestination
		{
			get { return ResourceManager.GetString("CannotWriteToDestination"); }
		}
		
		
		
		public static string ConverterReaderInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterReaderInInconsistentStare"); }
		}
		
		
		
		public static string TextReaderUnsupported
		{
			get { return ResourceManager.GetString("TextReaderUnsupported"); }
		}
		
		
		
		public static string PropertyNotValidForTextExtractionMode
		{
			get { return ResourceManager.GetString("PropertyNotValidForTextExtractionMode"); }
		}
		
		
		
		public static string TagIdIsUnknown
		{
			get { return ResourceManager.GetString("TagIdIsUnknown"); }
		}
		
		
		
		public static string TagIdInvalid
		{
			get { return ResourceManager.GetString("TagIdInvalid"); }
		}
		
		
		
		public static string CallbackTagAlreadyDeleted
		{
			get { return ResourceManager.GetString("CallbackTagAlreadyDeleted"); }
		}

		
		
		
		public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.TextConvertersStrings", typeof(Microsoft.Exchange.CtsResources.TextConvertersStrings).Assembly);
	}
}
