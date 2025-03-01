// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatTypes.cs" company="Microsoft Corporation">
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

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;
    using Microsoft.Exchange.Data.Internal;

    internal struct FlagProperties
    {
        private const uint AllDefinedBits = 0xAAAAAAAAu;
        private const uint AllValueBits = 0x55555555u;
        private const uint ValueBit = 0x00000001u;
        private const uint DefinedBit = 0x00000002u;
        private const uint ValueAndDefinedBits = 0x00000003u;

        internal uint bits;

        public static readonly FlagProperties AllUndefined = new(0);
        public static readonly FlagProperties AllOff = new(0);
        public static readonly FlagProperties AllOn = new(0xFFFFFFFFu);

        internal FlagProperties(uint bits)
        {
            this.bits = bits;
        }

        public static bool IsFlagProperty(PropertyId id)
        {
            return id >= PropertyId.FirstFlag && id <= PropertyId.LastFlag;
        }

        public bool IsDefined(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return 0 != (this.bits & (DefinedBit << ((id - PropertyId.FirstFlag) * 2)));
        }

        public bool IsOn(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id) && IsDefined(id));

            return 0 != (this.bits & (ValueBit << ((id - PropertyId.FirstFlag) * 2)));
        }

        public uint Mask { get { return (this.bits & AllDefinedBits) | ((this.bits & AllDefinedBits) >> 1); } }

        public static FlagProperties Merge(FlagProperties baseFlags, FlagProperties overrideFlags)
        {
            return new FlagProperties((baseFlags.bits & ~((overrideFlags.bits & AllDefinedBits) >> 1)) | overrideFlags.bits);
        }

        public static FlagProperties operator &(FlagProperties x, FlagProperties y)
        {
            return new FlagProperties(x.bits & ((y.bits & AllDefinedBits) | ((y.bits & AllDefinedBits) >> 1)));
        }

        public static FlagProperties operator |(FlagProperties x, FlagProperties y)
        {
            return FlagProperties.Merge(x, y);
        }

        public static FlagProperties operator ^(FlagProperties x, FlagProperties y)
        {
            uint tmp = (x.bits ^ y.bits) & x.Mask & y.Mask;
            return new FlagProperties(tmp | (tmp << 1));
        }

        public static FlagProperties operator ~(FlagProperties x)
        {
            return new FlagProperties(~((x.bits & AllDefinedBits) | ((x.bits & AllDefinedBits) >> 1)));
        }

        public static bool operator ==(FlagProperties x, FlagProperties y)
        {
            return x.bits == y.bits;
        }

        public static bool operator !=(FlagProperties x, FlagProperties y)
        {
            return x.bits != y.bits;
        }

        public override bool Equals(object obj)
        {
            return obj is FlagProperties flagProperties && this.bits == (flagProperties).bits;
        }

        public override int GetHashCode()
        {
            return (int)this.bits;
        }

        public override string ToString()
        {
            string result = "";

            for (PropertyId pid = PropertyId.FirstFlag; pid <= PropertyId.LastFlag; pid++)
            {
                if (this.IsDefined(pid))
                {
                    if (result.Length != 0)
                    {
                        result += ", ";
                    }
                    result += pid.ToString() + (this.IsOn(pid) ? ":on" : ":off");
                }
            }

            return result;
        }
    }

    internal struct PropertyBitMask
    {
        public const PropertyId FirstNonFlag = PropertyId.LastFlag + 1;

        internal uint bits1;
        internal uint bits2;

        public static readonly PropertyBitMask AllOff = new(0, 0);
        public static readonly PropertyBitMask AllOn = new(0xFFFFFFFFu, 0xFFFFFFFFu);

        internal PropertyBitMask(uint bits1, uint bits2)
        {
            InternalDebug.Assert(PropertyId.MaxValue - FirstNonFlag <= 32 * 2);

            this.bits1 = bits1;
            this.bits2 = bits2;
        }

        public bool IsSet(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            return 0 != (id < FirstNonFlag + 32 ? (this.bits1 & (1u << (id - FirstNonFlag))) : (this.bits2 & (1u << (id - FirstNonFlag - 32))));
        }

        public static PropertyBitMask operator |(PropertyBitMask x, PropertyBitMask y)
        {
            return new PropertyBitMask(x.bits1 | y.bits1, x.bits2 | y.bits2);
        }

        public static PropertyBitMask operator &(PropertyBitMask x, PropertyBitMask y)
        {
            return new PropertyBitMask(x.bits1 & y.bits1, x.bits2 & y.bits2);
        }

        public static PropertyBitMask operator ^(PropertyBitMask x, PropertyBitMask y)
        {
            return new PropertyBitMask(x.bits1 ^ y.bits1, x.bits2 ^ y.bits2);
        }

        public static PropertyBitMask operator ~(PropertyBitMask x)
        {
            return new PropertyBitMask(~x.bits1, ~x.bits2);
        }

        public static bool operator ==(PropertyBitMask x, PropertyBitMask y)
        {
            return x.bits1 == y.bits1 && x.bits2 == y.bits2;
        }

        public static bool operator !=(PropertyBitMask x, PropertyBitMask y)
        {
            return x.bits1 != y.bits1 || x.bits2 != y.bits2;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyBitMask propertyBitMask && this.bits1 == propertyBitMask.bits1 && this.bits2 == ((PropertyBitMask)obj).bits2;
        }

        public override int GetHashCode()
        {
            return (int)(this.bits1 ^ this.bits2);
        }

        public override string ToString()
        {
            string result = "";

            for (PropertyId pid = FirstNonFlag; pid < PropertyId.MaxValue; pid++)
            {
                if (this.IsSet(pid))
                {
                    if (result.Length != 0)
                    {
                        result += ", ";
                    }
                    result += pid.ToString();
                }
            }

            return result;
        }
    }

#if !DATAGEN
    internal class PropertyState
    {
        private const int MaxStackSize = 1000;

        private FlagProperties flagProperties;
        private FlagProperties distinctFlagProperties;

        private PropertyBitMask propertyMask;
        private PropertyBitMask distinctPropertyMask;

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        private struct FlagPropertiesUndo
        {
            public PropertyId fakeId;
            public FlagProperties flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        private struct BitsUndo
        {
            public PropertyId fakeId;
            public uint bits;
        }

        public override string ToString()
        {
            return "flags: (" + this.flagProperties.ToString() + "), props: (" + this.propertyMask.ToString() + "), dflags: (" + this.distinctFlagProperties.ToString() + "), dprops: (" + this.distinctPropertyMask.ToString() + ")";
        }
    }
#endif
}

