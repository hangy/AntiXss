// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Microsoft.Exchange.CtsResources.TextConvertersStrings.cs" company="Microsoft Corporation">
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

using System;
using System.ComponentModel;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	static class TextConvertersStrings
	{
        private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.TextConvertersStrings", typeof(Microsoft.Exchange.CtsResources.TextConvertersStrings).Assembly);

        // Orphaned WPL code.
#if false
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
#endif
		
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

        // Orphaned WPL code.
#if false
		public static string AttributeNotStarted
		{
			get { return ResourceManager.GetString("AttributeNotStarted"); }
		}
#endif
		
		public static string InputEncodingRequired
		{
			get { return ResourceManager.GetString("InputEncodingRequired"); }
		}

        // Orphaned WPL code.
#if false
		public static string CreateFileFailed (string filePath)
		{
			return string.Format(ResourceManager.GetString("CreateFileFailed"), filePath);
		}
		
		public static string AttributeCollectionNotInitialized
		{
			get { return ResourceManager.GetString("AttributeCollectionNotInitialized"); }
		}
#endif
		
		public static string ConverterWriterInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterWriterInInconsistentStare"); }
		}

        // Orphaned WPL code.
#if false
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
#endif
		
		public static string TagTooLong
		{
			get { return ResourceManager.GetString("TagTooLong"); }
		}

        // Orphaned WPL code.
#if false
		public static string EndTagCannotHaveAttributes
		{
			get { return ResourceManager.GetString("EndTagCannotHaveAttributes"); }
		}

		public static string CannotUseConverterWriter
		{
			get { return ResourceManager.GetString("CannotUseConverterWriter"); }
		}
#endif
		
		public static string WriteAfterFlush
		{
			get { return ResourceManager.GetString("WriteAfterFlush"); }
		}

        // Orphaned WPL code.
#if false
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
#endif
		
		public static string TooManyIterationsToFlushConverter
		{
			get { return ResourceManager.GetString("TooManyIterationsToFlushConverter"); }
		}
		
		public static string IndexOutOfRange
		{
			get { return ResourceManager.GetString("IndexOutOfRange"); }
		}

        // Orphaned WPL code.
#if false
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
#endif
		
		public static string CountTooLarge
		{
			get { return ResourceManager.GetString("CountTooLarge"); }
		}

        // Orphaned WPL code.
#if false
		public static string InvalidConfigurationStream (int propertyId)
		{
			return string.Format(ResourceManager.GetString("InvalidConfigurationStream"), propertyId);
		}
#endif
		
		public static string TooManyIterationsToProduceOutput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProduceOutput"); }
		}

        // Orphaned WPL code.
#if false
		public static string AttributeNotInitialized
		{
			get { return ResourceManager.GetString("AttributeNotInitialized"); }
		}
#endif
		
		public static string CannotWriteOtherTagsInsideElement (string elementName)
		{
			return string.Format(ResourceManager.GetString("CannotWriteOtherTagsInsideElement"), elementName);
		}

        // Orphaned WPL code.
#if false
		public static string AttributeIdInvalid
		{
			get { return ResourceManager.GetString("AttributeIdInvalid"); }
		}
		
		public static string InvalidConfigurationInteger (int propertyId)
		{
			return string.Format(ResourceManager.GetString("InvalidConfigurationInteger"), propertyId);
		}
#endif		
		public static string TooManyIterationsToProcessInput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProcessInput"); }
		}

        // Orphaned WPL code.
#if false
		public static string BufferSizeValueRange
		{
			get { return ResourceManager.GetString("BufferSizeValueRange"); }
		}
#endif		
		
		public static string CannotWriteWhileCopyPending
		{
			get { return ResourceManager.GetString("CannotWriteWhileCopyPending"); }
		}

        // Orphaned WPL code.
#if false
		public static string AttributeNotValidInThisState
		{
			get { return ResourceManager.GetString("AttributeNotValidInThisState"); }
		}
#endif		
		
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

// Orphaned WPL code.
#if false
		public static string AccessShouldBeReadOrWrite
		{
			get { return ResourceManager.GetString("AccessShouldBeReadOrWrite"); }
		}
#endif
		
		public static string ContextNotValidInThisState
		{
			get { return ResourceManager.GetString("ContextNotValidInThisState"); }
		}

        // Orphaned WPL code.
#if false
		public static string CannotSeekBeforeBeginning
		{
			get { return ResourceManager.GetString("CannotSeekBeforeBeginning"); }
		}
#endif		
		
		public static string OffsetOutOfRange
		{
			get { return ResourceManager.GetString("OffsetOutOfRange"); }
		}
		
		public static string ReadUnsupported
		{
			get { return ResourceManager.GetString("ReadUnsupported"); }
		}

        // Orphaned WPL code.
#if false
		public static string LengthExceeded (int sum, int length)
		{
			return string.Format(ResourceManager.GetString("LengthExceeded"), sum, length);
		}
#endif
		
		public static string SeekUnsupported
		{
			get { return ResourceManager.GetString("SeekUnsupported"); }
		}

        // Orphaned WPL code.
#if false
		public static string InvalidCodePage (int codePage)
		{
			return string.Format(ResourceManager.GetString("InvalidCodePage"), codePage);
		}

		public static string CallbackTagAlreadyWritten
		{
			get { return ResourceManager.GetString("CallbackTagAlreadyWritten"); }
		}
#endif		
		
		public static string ConverterStreamInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterStreamInInconsistentStare"); }
		}

        // Orphaned WPL code.
#if false
		public static string AttributeNameIsEmpty
		{
			get { return ResourceManager.GetString("AttributeNameIsEmpty"); }
		}
#endif
		
		public static string HtmlNestingTooDeep
		{
			get { return ResourceManager.GetString("HtmlNestingTooDeep"); }
		}

        // Orphaned WPL code.
#if false
		public static string CannotWriteToDestination
		{
			get { return ResourceManager.GetString("CannotWriteToDestination"); }
		}
#endif
		
		public static string ConverterReaderInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterReaderInInconsistentStare"); }
		}

        // Orphaned WPL code.
#if false
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
#endif
	}
}
