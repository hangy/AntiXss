// ***************************************************************
// <copyright file="FormatType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

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

        public static readonly FlagProperties AllUndefined = new FlagProperties(0);
        public static readonly FlagProperties AllOff = new FlagProperties(0);
        public static readonly FlagProperties AllOn = new FlagProperties(0xFFFFFFFFu);

        internal FlagProperties(uint bits)
        {
            this.bits = bits;
        }

        internal int IntegerBag
        {
            get { return unchecked((int)this.bits); }
            set { this.bits = unchecked((uint)value); }
        }

        public bool IsClear
        {
            get { return 0 == this.bits; }
        }

        public static bool IsFlagProperty(PropertyId id)
        {
            return id >= PropertyId.FirstFlag && id <= PropertyId.LastFlag;
        }

        public void Set(PropertyId id, bool value)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            int shift = (id - PropertyId.FirstFlag) * 2;
            uint mask = (DefinedBit | ValueBit) << shift;

            if (value)
            {
                 this.bits |= ((DefinedBit | ValueBit) << shift);               
            }
            else
            {
                this.bits &= ~(ValueBit << shift);               
                this.bits |= (DefinedBit << shift);                
            }
        }

        public void Remove(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            this.bits &= ~((DefinedBit | ValueBit) << ((id - PropertyId.FirstFlag) * 2));   
        }

        public void ClearAll()
        {
            this.bits = 0u;
        }

        public bool IsDefined(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return 0 != (this.bits & (DefinedBit << ((id - PropertyId.FirstFlag) * 2)));    
        }

        public bool IsAnyDefined()
        {
            return this.bits != 0u;
        }

        public bool IsOn(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id) && IsDefined(id));

            return 0 != (this.bits & (ValueBit << ((id - PropertyId.FirstFlag) * 2)));    
        }

        public bool IsDefinedAndOn(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return ValueAndDefinedBits == ((this.bits >> ((id - PropertyId.FirstFlag) * 2)) & ValueAndDefinedBits);    
        }

        public bool IsDefinedAndOff(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return DefinedBit == ((this.bits >> ((id - PropertyId.FirstFlag) * 2)) & ValueAndDefinedBits);    
        }

        public PropertyValue GetPropertyValue(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            int shift = (id - PropertyId.FirstFlag) * 2;
            if (0 != (this.bits & (DefinedBit << shift)))          
            {
                return new PropertyValue(0 != (this.bits & (ValueBit << shift)));
            }

            return PropertyValue.Null;
        }

        public void SetPropertyValue(PropertyId id, PropertyValue value)
        {
            if (value.IsBool)
            {
                this.Set(id, value.Bool);
            }
        }

        
        
        public bool IsSubsetOf(FlagProperties overrideFlags)
        {
            return 0 == ((this.bits & AllDefinedBits) & ~(overrideFlags.bits & AllDefinedBits));
        }

        public uint Mask { get { return (this.bits & AllDefinedBits) | ((this.bits & AllDefinedBits) >> 1); } }
#if false
        public void Mask(FlagProperties maskFlags)
        {
            
            
            this.bits &= (maskFlags.bits & AllDefinedBits) | ((maskFlags.bits & AllDefinedBits) >> 1);
        }
#endif
        public void Merge(FlagProperties overrideFlags)
        {
            
            
            
            this.bits = (this.bits & ~((overrideFlags.bits & AllDefinedBits) >> 1)) | overrideFlags.bits;
        }

        public void ReverseMerge(FlagProperties baseFlags)
        {
            
            
            
            this.bits = (baseFlags.bits & ~((this.bits & AllDefinedBits) >> 1)) | this.bits;
        }

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
            return (obj is FlagProperties) && this.bits == ((FlagProperties)obj).bits;
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

        public static readonly PropertyBitMask AllOff = new PropertyBitMask(0, 0);
        public static readonly PropertyBitMask AllOn = new PropertyBitMask(0xFFFFFFFFu, 0xFFFFFFFFu);

        internal PropertyBitMask(uint bits1, uint bits2)
        {
            
            
            InternalDebug.Assert(PropertyId.MaxValue - FirstNonFlag <= 32 * 2);

            this.bits1 = bits1;
            this.bits2 = bits2;
        }

        internal void Set1(uint bits1)
        {
            this.bits1 = bits1;
        }

        internal void Set2(uint bits2)
        {
            this.bits2 = bits2;
        }

        public void Or(PropertyBitMask newBits)
        {
            this.bits1 |= newBits.bits1;
            this.bits2 |= newBits.bits2;
        }

        public bool IsClear
        {
            get { return (0 == this.bits1 && 0 == this.bits2); }
        }

        public bool IsSet(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            return 0 != (id < FirstNonFlag + 32 ? (this.bits1 & (1u << (id - FirstNonFlag))) : (this.bits2 & (1u << (id - FirstNonFlag - 32))));
        }

        public bool IsNotSet(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            return 0 == (id < FirstNonFlag + 32 ? (this.bits1 & (1u << (id - FirstNonFlag))) : (this.bits2 & (1u << (id - FirstNonFlag - 32))));
        }

        public void Set(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            if (id < FirstNonFlag + 32)
            {
                this.bits1 |= (1u << (id - FirstNonFlag));
            }
            else
            {
                this.bits2 |= (1u << (id - FirstNonFlag - 32));
            }
        }

        public void Clear(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            if (id < FirstNonFlag + 32)
            {
                this.bits1 &= ~(1u << (id - FirstNonFlag));
            }
            else
            {
                this.bits2 &= ~(1u << (id - FirstNonFlag - 32));
            }
        }

        public bool IsSubsetOf(PropertyBitMask overrideFlags)
        {
            return 0 == (this.bits1 & ~overrideFlags.bits1) && 0 == (this.bits2 & ~overrideFlags.bits2);
        }

        public void ClearAll()
        {
            this.bits1 = 0;
            this.bits2 = 0;
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
            return (obj is PropertyBitMask) && this.bits1 == ((PropertyBitMask)obj).bits1 && this.bits2 == ((PropertyBitMask)obj).bits2;
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

        public DefinedPropertyIdEnumerator GetEnumerator()
        {
            return new DefinedPropertyIdEnumerator(this);
        }

        public struct DefinedPropertyIdEnumerator
        {
            internal ulong bits;
            internal ulong currentBit;
            internal PropertyId currentId;

            internal DefinedPropertyIdEnumerator(PropertyBitMask mask)
            {
                this.bits = ((ulong)mask.bits2 << 32) | mask.bits1;
                this.currentBit = 1;
                
                this.currentId = (this.bits != 0) ? PropertyId.LastFlag : PropertyId.MaxValue;
            }

            public PropertyId Current
            {
                get
                {
                    return this.currentId;
                }
            }

            public bool MoveNext()
            {
                while (this.currentId != PropertyId.MaxValue)
                {
                    if (this.currentId != PropertyId.LastFlag)
                    {
                        this.currentBit <<= 1;
                    }

                    this.currentId ++;

                    if (this.currentId != PropertyId.MaxValue && 0 != (this.bits & this.currentBit))
                    {
                        return true;
                    }
                }

                return false;
            }
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
        private PropertyValue[] properties = new PropertyValue[(int)(PropertyId.MaxValue - PropertyBitMask.FirstNonFlag)];

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

        [StructLayout(LayoutKind.Explicit, Pack=2)]
        private struct PropertyUndoEntry
        {
            public const PropertyId FlagPropertiesFakeId = PropertyId.MaxValue + 0;
            public const PropertyId DistinctFlagPropertiesFakeId = PropertyId.MaxValue + 1;
            public const PropertyId DistinctMask1FakeId = PropertyId.MaxValue + 2;
            public const PropertyId DistinctMask2FakeId = PropertyId.MaxValue + 3;

            [FieldOffset(0)] 
            public Property property;

            [FieldOffset(0)] 
            public FlagPropertiesUndo flags;

            [FieldOffset(0)] 
            public BitsUndo bits;

            public bool IsFlags { get { return this.property.Id == FlagPropertiesFakeId; } }
            public bool IsDistinctFlags { get { return this.property.Id == DistinctFlagPropertiesFakeId; } }
            public bool IsDistinctMask1 { get { return this.property.Id == DistinctMask1FakeId; } }
            public bool IsDistinctMask2 { get { return this.property.Id == DistinctMask2FakeId; } }

            public void Set(PropertyId id, PropertyValue value)
            {
                this.property.Set(id, value);
            }

            public void Set(PropertyId fakePropId, FlagProperties flagProperties)
            {
                this.flags.fakeId = fakePropId;
                this.flags.flags = flagProperties;
            }

            public void Set(PropertyId fakePropId, uint bits)
            {
                this.bits.fakeId = fakePropId;
                this.bits.bits = bits;
            }
        }

        private PropertyUndoEntry[] propertyUndoStack = new PropertyUndoEntry[(int)PropertyId.MaxValue * 2];
        private int propertyUndoStackTop;

        public int UndoStackTop { get { return this.propertyUndoStackTop; } }

        public FlagProperties GetEffectiveFlags()
        {
            return this.flagProperties;
        }

        public FlagProperties GetDistinctFlags()
        {
            return this.distinctFlagProperties;
        }

        public PropertyValue GetEffectiveProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                return this.flagProperties.GetPropertyValue(id);
            }
            else if (this.propertyMask.IsSet(id))
            {
                return this.properties[(int)(id - PropertyBitMask.FirstNonFlag)];
            }
            return PropertyValue.Null;
        }

        public PropertyValue GetDistinctProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                return this.distinctFlagProperties.GetPropertyValue(id);
            }
            else if (this.distinctPropertyMask.IsSet(id))
            {
                return this.properties[(int)(id - PropertyBitMask.FirstNonFlag)];
            }
            return PropertyValue.Null;
        }

        public void SubtractDefaultFromDistinct(FlagProperties defaultFlags, Property[] defaultProperties)
        {
            
            FlagProperties overridenFlagsPropertiesMask = defaultFlags ^ this.distinctFlagProperties;
            FlagProperties newDistinctFlagProperties = (this.distinctFlagProperties & overridenFlagsPropertiesMask) | (distinctFlagProperties & ~defaultFlags);
            if (this.distinctFlagProperties != newDistinctFlagProperties)
            {
                this.PushUndoEntry(PropertyUndoEntry.DistinctFlagPropertiesFakeId, this.distinctFlagProperties);
                this.distinctFlagProperties = newDistinctFlagProperties;
            }

            if (defaultProperties != null)
            {
                bool savedDistinctMask = false;
                foreach (Property prop in defaultProperties)
                {
                    if (this.distinctPropertyMask.IsSet(prop.Id) && this.properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] == prop.Value)
                    {
                        if (!savedDistinctMask)
                        {
                            this.PushUndoEntry(this.distinctPropertyMask);
                            savedDistinctMask = true;
                        }

                        this.distinctPropertyMask.Clear(prop.Id);
                    }
                }
            }
        }

        public int ApplyProperties(FlagProperties flagProperties, Property[] propList, FlagProperties flagInheritanceMask, PropertyBitMask propertyInheritanceMask)
        {
            int undoStackPosition = this.propertyUndoStackTop;

            FlagProperties allInheritedFlagProperties = this.flagProperties & flagInheritanceMask;

            FlagProperties newEffectiveFlagProperties = allInheritedFlagProperties | flagProperties;

            if (newEffectiveFlagProperties != this.flagProperties)
            {
                this.PushUndoEntry(PropertyUndoEntry.FlagPropertiesFakeId, this.flagProperties);
                this.flagProperties = newEffectiveFlagProperties;
            }

            
            FlagProperties overridenFlagsPropertiesMask = allInheritedFlagProperties ^ flagProperties;

            FlagProperties newDistinctFlagProperties = (flagProperties & overridenFlagsPropertiesMask) | (flagProperties & ~allInheritedFlagProperties);

            if (newDistinctFlagProperties != this.distinctFlagProperties)
            {
                this.PushUndoEntry(PropertyUndoEntry.DistinctFlagPropertiesFakeId, this.distinctFlagProperties);
                this.distinctFlagProperties = newDistinctFlagProperties;
            }

            PropertyBitMask maskedOutProperties = this.propertyMask & ~propertyInheritanceMask;

            foreach (PropertyId propId in maskedOutProperties)
            {
                this.PushUndoEntry(propId, this.properties[(int)(propId - PropertyBitMask.FirstNonFlag)]);
            }

            PropertyBitMask newDistinctPropertyMask = PropertyBitMask.AllOff;
            this.propertyMask &= propertyInheritanceMask;

            if (propList != null)
            {
                foreach (Property prop in propList)
                {
                    if (this.propertyMask.IsSet(prop.Id))
                    {
                        if (this.properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] != prop.Value)
                        {
                            this.PushUndoEntry(prop.Id, this.properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)]);

                            if (prop.Value.IsNull)
                            {
                                this.propertyMask.Clear(prop.Id);
                            }
                            else
                            {
                                this.properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] = prop.Value;
                                newDistinctPropertyMask.Set(prop.Id);
                            }
                        }
                    }
                    else if (!prop.Value.IsNull)
                    {
                        if (!maskedOutProperties.IsSet(prop.Id))
                        {
                            this.PushUndoEntry(prop.Id, PropertyValue.Null);
                        }

                        this.properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] = prop.Value;

                        this.propertyMask.Set(prop.Id);

                        newDistinctPropertyMask.Set(prop.Id);
                    }
                }
            }

            if (newDistinctPropertyMask != this.distinctPropertyMask)
            {
                this.PushUndoEntry(this.distinctPropertyMask);
                this.distinctPropertyMask = newDistinctPropertyMask;
            }

            return undoStackPosition;
        }

        public void UndoProperties(int undoLevel)
        {
            InternalDebug.Assert(undoLevel <= this.propertyUndoStackTop);

            for (int i = this.propertyUndoStackTop - 1; i >= undoLevel; i--)
            {
                if (this.propertyUndoStack[i].IsFlags)
                {
                    this.flagProperties = this.propertyUndoStack[i].flags.flags;
                }
                else if (this.propertyUndoStack[i].IsDistinctFlags)
                {
                    this.distinctFlagProperties = this.propertyUndoStack[i].flags.flags;
                }
                else if (this.propertyUndoStack[i].IsDistinctMask1)
                {
                    this.distinctPropertyMask.Set1(this.propertyUndoStack[i].bits.bits);
                }
                else if (this.propertyUndoStack[i].IsDistinctMask2)
                {
                    this.distinctPropertyMask.Set2(this.propertyUndoStack[i].bits.bits);
                }
                else 
                {
                    if (this.propertyUndoStack[i].property.Value.IsNull)
                    {
                        this.propertyMask.Clear(this.propertyUndoStack[i].property.Id);
                    }
                    else
                    {
                        this.properties[(int)(this.propertyUndoStack[i].property.Id - PropertyBitMask.FirstNonFlag)] = this.propertyUndoStack[i].property.Value;
                        this.propertyMask.Set(this.propertyUndoStack[i].property.Id);
                    }
                }
            }

            this.propertyUndoStackTop = undoLevel;
        }

        private void PushUndoEntry(PropertyId id, PropertyValue value)
        {
            if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
            {
                if (this.propertyUndoStack.Length >= PropertyState.MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                int newStackSize = Math.Min(this.propertyUndoStack.Length * 2, PropertyState.MaxStackSize);

                PropertyUndoEntry[] newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(this.propertyUndoStack, 0, newPropertyUndoStack, 0, this.propertyUndoStackTop);
                this.propertyUndoStack = newPropertyUndoStack;
            }

            this.propertyUndoStack[this.propertyUndoStackTop++].Set(id, value);
        }

        private void PushUndoEntry(PropertyId fakePropId, FlagProperties flagProperties)
        {
            if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
            {
                if (this.propertyUndoStack.Length >= PropertyState.MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                int newStackSize = Math.Min(this.propertyUndoStack.Length * 2, PropertyState.MaxStackSize);

                PropertyUndoEntry[] newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(this.propertyUndoStack, 0, newPropertyUndoStack, 0, this.propertyUndoStackTop);
                this.propertyUndoStack = newPropertyUndoStack;
            }

            this.propertyUndoStack[this.propertyUndoStackTop++].Set(fakePropId, flagProperties);
        }

        private void PushUndoEntry(PropertyBitMask propertyMask)
        {
            if (this.propertyUndoStackTop + 1 >= this.propertyUndoStack.Length)
            {
                if (this.propertyUndoStackTop + 2 >= PropertyState.MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                int newStackSize = Math.Min(this.propertyUndoStack.Length * 2, PropertyState.MaxStackSize);

                PropertyUndoEntry[] newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(this.propertyUndoStack, 0, newPropertyUndoStack, 0, this.propertyUndoStackTop);
                this.propertyUndoStack = newPropertyUndoStack;
            }

            this.propertyUndoStack[this.propertyUndoStackTop++].Set(PropertyUndoEntry.DistinctMask1FakeId, propertyMask.bits1);
            this.propertyUndoStack[this.propertyUndoStackTop++].Set(PropertyUndoEntry.DistinctMask2FakeId, propertyMask.bits2);
        }

        public override string ToString()
        {
            return "flags: (" + this.flagProperties.ToString() + "), props: (" + this.propertyMask.ToString() + "), dflags: (" + this.distinctFlagProperties.ToString() + "), dprops: (" + this.distinctPropertyMask.ToString() + ")";
        }

    }
#endif
}

