// ***************************************************************
// <copyright file="TextCodePageConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;


    internal class TextCodePageConverter : IProducerConsumer, IDisposable
    {
        ////////////////////////////////////////////////////////////

        protected ConverterInput input;
        protected bool endOfFile;
        protected bool gotAnyText;

        protected ConverterOutput output;


        public TextCodePageConverter(ConverterInput input, ConverterOutput output)
        {
            this.input = input;
            this.output = output;
        }



        public void Run()
        {
            if (this.endOfFile)
            {
                return;
            }

            char[] buffer = null;
            int start = 0;
            int current = 0;
            int end = 0;

            if (!this.input.ReadMore(ref buffer, ref start, ref current, ref end))
            {
                // cannot decode more data until next input chunk is available

                return;
            }

            if (this.input.EndOfFile)
            {
                this.endOfFile = true;
            }

            if (end - start != 0)
            {
                if (!this.gotAnyText)
                {
                    if (this.output is ConverterEncodingOutput)
                    {
                        ConverterEncodingOutput encodingOutput = this.output as ConverterEncodingOutput;

                        if (encodingOutput.CodePageSameAsInput)
                        {
 
                            if (this.input is ConverterDecodingInput)
                            {
                                encodingOutput.Encoding = (this.input as ConverterDecodingInput).Encoding;
                            }
                            else
                            {
                                encodingOutput.Encoding = Encoding.UTF8;
                            }

                        }

                    }

                    this.gotAnyText = true;

                }

                this.output.Write(buffer, start, end - start);

                this.input.ReportProcessed(end - start);

            }

            if (this.endOfFile)
            {
                this.output.Flush();
            }

        }



        public bool Flush()
        {
            if (!this.endOfFile)
            {
                this.Run();
            }

            return this.endOfFile;
        }


        void IDisposable.Dispose()
        {
            if (this.input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)this.input).Dispose();
            }

            if (this.output != null /*&& this.output is IDisposable*/)
            {
                ((IDisposable)this.output).Dispose();
            }

            this.input = null;
            this.output = null;

            GC.SuppressFinalize(this);
        }

    }

}
