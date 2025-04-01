// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Charset.cs" company="Microsoft Corporation">
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
// </copyright>
// <summary>
//   Represents a character set.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a character set
    /// </summary>
    [Serializable]
    internal class Charset
    {
        /// <summary>
        /// Flag indicating if the character set is available.
        /// </summary>
        private bool available;

        /// <summary>
        /// The character set encoding.
        /// </summary>
        private Encoding? encoding;

        /// <summary>
        /// The character set map index.
        /// </summary>
        private short mapIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Charset"/> class.
        /// </summary>
        /// <param name="codePage">
        /// The code page number.
        /// </param>
        /// <param name="name">
        /// The name of the character set.
        /// </param>
        internal Charset(int codePage, string name)
        {
            this.CodePage = codePage;
            this.Name = name;
            this.Culture = null;

            this.available = true;
            this.mapIndex = -1;
        }

        /// <summary>
        /// Gets or sets the character set number
        /// </summary>
        public int CodePage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the character set name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets  the character set culture.
        /// </summary>
        public Culture? Culture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the character set is detectable.
        /// </summary>
        public bool IsDetectable
        {
            get
            {
                return this.mapIndex >= 0 &&
                       0 != (CodePageMapData.CodePages[this.mapIndex].Flags & CodePageFlags.Detectable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the character set is a Windows character set.
        /// </summary>
        public bool IsWindowsCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description for the character set.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Using default get/set syntax.")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map index for this character set.
        /// </summary>
        internal int MapIndex
        {
            set
            {
                this.mapIndex = (short)value;
            }
        }

        /// <summary>
        /// Looks up the passed-in character set name and sets the corresponding character set.
        /// </summary>
        /// <param name="name">
        /// The name of the character set to find.
        /// </param>
        /// <param name="charset">
        /// The character set associated with the key.
        /// </param>
        /// <returns>
        /// True if the character set is found, otherwise false.
        /// </returns>
        public static bool TryGetCharset(string? name, out Charset? charset)
        {
            if (name == null)
            {
                charset = null;
                return false;
            }

            if (CultureCharsetDatabase.InternalGlobalizationData.NameToCharset.TryGetValue(name, out charset))
            {
                return true;
            }

            if (name.StartsWith("cp", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("ms", StringComparison.OrdinalIgnoreCase))
            {
                int cpid = 0;

                for (int i = 2; i < name.Length; i++)
                {
                    if (name[i] < '0' ||
                        name[i] > '9')
                    {
                        return false;
                    }

                    cpid = (cpid * 10) + (name[i] - '0');

                    if (cpid >= 65536)
                    {
                        return false;
                    }
                }

                return cpid != 0 && TryGetCharset(cpid, out charset);
            }

            return false;
        }

        /// <summary>
        /// Looks up the passed-in code page and sets the corresponding character set.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <param name="charset">
        /// The character set associated with the key.
        /// </param>
        /// <returns>
        /// True if a character set for the key is found, otherwise false.
        /// </returns>
        public static bool TryGetCharset(int codePage, out Charset? charset)
        {
            return CultureCharsetDatabase.InternalGlobalizationData.CodePageToCharset.TryGetValue(codePage, out charset);
        }

        /// <summary>
        /// Looks up the passed-in code page and sets the corresponding encoding.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <param name="encoding">
        /// The encoding asscoiated with the key.
        /// </param>
        /// <returns>
        /// True if a character set for the key is found, otherwise false.
        /// </returns>
        public static bool TryGetEncoding(int codePage, out Encoding? encoding)
        {
            if (!TryGetCharset(codePage, out Charset? charset))
            {
                encoding = null;
                return false;
            }

            return charset!.TryGetEncoding(out encoding);
        }

        /// <summary>
        /// Looks up the passed in character set name and sets the corresponding encoding.
        /// </summary>
        /// <param name="name">
        /// The character set name.
        /// </param>
        /// <param name="encoding">
        /// The encoding associated with the key.
        /// </param>
        /// <returns>
        /// True if a character set for the key is found, otherwise false.
        /// </returns>
        public static bool TryGetEncoding(string? name, out Encoding? encoding)
        {
            if (!TryGetCharset(name, out Charset? charset))
            {
                encoding = null;
                return false;
            }

            return charset!.TryGetEncoding(out encoding);
        }

        /// <summary>
        /// Gets the encoding for the specified code page.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <returns>
        /// The <see cref="Encoding"/> for the specified code page.
        /// </returns>
        public static Encoding? GetEncoding(int codePage)
        {
            Charset? charset = GetCharset(codePage);
            return charset?.GetEncoding();
        }

        /// <summary>
        /// Gets the character set for the given code page.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <returns>
        /// The character set for the given code page.
        /// </returns>
        /// <exception cref="InvalidCharsetException">
        /// Thrown if the character set is not found.
        /// </exception>
        public static Charset? GetCharset(int codePage)
        {
            if (!TryGetCharset(codePage, out Charset? charset))
            {
                throw new InvalidCharsetException(codePage);
            }

            return charset;
        }

        /// <summary>
        /// Looks up the encoding class for this character set.
        /// </summary>
        /// <param name="attemptedEncoding">
        /// The encoding class created if one is available.
        /// </param>
        /// <returns>
        /// True if encoding is available, otherwise false.
        /// </returns>
        public bool TryGetEncoding(out Encoding? attemptedEncoding)
        {
            if (this.encoding == null &&
                this.available)
            {
                try
                {
                    switch (this.CodePage)
                    {
                        case 20127:
                            this.encoding = Encoding.GetEncoding(
                                this.CodePage,
                                new AsciiEncoderFallback(),
                                DecoderFallback.ReplacementFallback);
                            break;
                        case 28599:
                        case 28591:
                            this.encoding = new RemapEncoding(this.CodePage);
                            break;
                        default:
                            this.encoding = Encoding.GetEncoding(this.CodePage);
                            break;
                    }
                }
                catch (ArgumentException)
                {
                    this.encoding = null;
                }
                catch (NotSupportedException)
                {
                    this.encoding = null;
                }

                if (this.encoding == null)
                {
                    this.available = false;
                }
            }

            attemptedEncoding = this.encoding;
            return attemptedEncoding != null;
        }

        /// <summary>
        /// Gets the <see cref="Encoding"/> for this character set.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/> class for this character set.
        /// </returns>
        /// <exception cref="CharsetNotInstalledException">
        /// Thrown if an encoding class cannot be found for this character set.
        /// </exception>
        public Encoding? GetEncoding()
        {
            if (!this.TryGetEncoding(out Encoding? discoveredEncoding))
            {
                throw new CharsetNotInstalledException(this.CodePage, this.Name);
            }

            return discoveredEncoding;
        }
    }
}
