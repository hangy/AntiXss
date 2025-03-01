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

namespace Microsoft.Exchange.CtsResources
{
    using System;
    using System.ComponentModel;
    using System.Resources;

    static class TextConvertersStrings
	{
        private static ResourceManager ResourceManager = new("Microsoft.Exchange.CtsResources.TextConvertersStrings", typeof(Microsoft.Exchange.CtsResources.TextConvertersStrings).Assembly);

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

		public static string InputEncodingRequired
		{
			get { return ResourceManager.GetString("InputEncodingRequired"); }
		}

		public static string ConverterWriterInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterWriterInInconsistentStare"); }
		}

		public static string TagTooLong
		{
			get { return ResourceManager.GetString("TagTooLong"); }
		}

		public static string WriteAfterFlush
		{
			get { return ResourceManager.GetString("WriteAfterFlush"); }
		}

		public static string TooManyIterationsToFlushConverter
		{
			get { return ResourceManager.GetString("TooManyIterationsToFlushConverter"); }
		}

		public static string IndexOutOfRange
		{
			get { return ResourceManager.GetString("IndexOutOfRange"); }
		}

		public static string CountTooLarge
		{
			get { return ResourceManager.GetString("CountTooLarge"); }
		}

		public static string TooManyIterationsToProduceOutput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProduceOutput"); }
		}

		public static string CannotWriteOtherTagsInsideElement (string elementName)
		{
			return string.Format(ResourceManager.GetString("CannotWriteOtherTagsInsideElement"), elementName);
		}

		public static string TooManyIterationsToProcessInput
		{
			get { return ResourceManager.GetString("TooManyIterationsToProcessInput"); }
		}

		public static string CannotWriteWhileCopyPending
		{
			get { return ResourceManager.GetString("CannotWriteWhileCopyPending"); }
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

		public static string ContextNotValidInThisState
		{
			get { return ResourceManager.GetString("ContextNotValidInThisState"); }
		}

		public static string OffsetOutOfRange
		{
			get { return ResourceManager.GetString("OffsetOutOfRange"); }
		}

		public static string ReadUnsupported
		{
			get { return ResourceManager.GetString("ReadUnsupported"); }
		}

		public static string SeekUnsupported
		{
			get { return ResourceManager.GetString("SeekUnsupported"); }
		}

		public static string ConverterStreamInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterStreamInInconsistentStare"); }
		}

		public static string HtmlNestingTooDeep
		{
			get { return ResourceManager.GetString("HtmlNestingTooDeep"); }
		}

		public static string ConverterReaderInInconsistentStare
		{
			get { return ResourceManager.GetString("ConverterReaderInInconsistentStare"); }
		}
	}
}
