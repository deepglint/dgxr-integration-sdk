using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
     /// <summary>
    /// Helper to avoid array allocations if there's only a single value in the array.
    /// </summary>
    /// <remarks>
    /// Also, once more than one entry is necessary, allows treating the extra array as having capacity.
    /// This means that, for example, 5 or 10 entries can be allocated in batch rather than growing an
    /// array one by one.
    /// </remarks>
    /// <typeparam name="TValue">Element type for the array.</typeparam>
    internal struct InlinedArray<TValue> : IEnumerable<TValue>
    {
        // We inline the first value so if there's only one, there's
        // no additional allocation. If more are added, we allocate an array.
        public int Length;
        public TValue FirstValue;
        public TValue[] AdditionalValues;

        public int Capacity => AdditionalValues?.Length + 1 ?? 1;

        public InlinedArray(TValue value)
        {
            Length = 1;
            FirstValue = value;
            AdditionalValues = null;
        }

        public InlinedArray(TValue firstValue, params TValue[] additionalValues)
        {
            Length = 1 + additionalValues.Length;
            this.FirstValue = firstValue;
            this.AdditionalValues = additionalValues;
        }

        public InlinedArray(IEnumerable<TValue> values)
            : this()
        {
            Length = values.Count();
            if (Length > 1)
                AdditionalValues = new TValue[Length - 1];
            else
                AdditionalValues = null;

            var index = 0;
            foreach (var value in values)
            {
                if (index == 0)
                    FirstValue = value;
                else
                    AdditionalValues[index - 1] = value;
                ++index;
            }
        }

        public TValue this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (index == 0)
                    return FirstValue;

                return AdditionalValues[index - 1];
            }
            set
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (index == 0)
                    FirstValue = value;
                else
                    AdditionalValues[index - 1] = value;
            }
        }

        public void Clear()
        {
            Length = 0;
            FirstValue = default;
            AdditionalValues = null;
        }

        public void ClearWithCapacity()
        {
            FirstValue = default;
            for (var i = 0; i < Length - 1; ++i)
                AdditionalValues[i] = default;
            Length = 0;
        }

        ////REVIEW: This is inconsistent with ArrayHelpers.Clone() which also clones elements
        public InlinedArray<TValue> Clone()
        {
            return new InlinedArray<TValue>
            {
                Length = Length,
                FirstValue = FirstValue,
                AdditionalValues = AdditionalValues != null ? ArrayHelper.Copy(AdditionalValues) : null
            };
        }

        public void SetLength(int size)
        {
            // Null out everything we're cutting off.
            if (size < Length)
            {
                for (var i = size; i < Length; ++i)
                    this[i] = default;
            }

            Length = size;

            if (size > 1 && (AdditionalValues == null || AdditionalValues.Length < size - 1))
                Array.Resize(ref AdditionalValues, size - 1);
        }

        public TValue[] ToArray()
        {
            return ArrayHelper.Join(FirstValue, AdditionalValues);
        }

        public TOther[] ToArray<TOther>(Func<TValue, TOther> mapFunction)
        {
            if (Length == 0)
                return null;

            var result = new TOther[Length];
            for (var i = 0; i < Length; ++i)
                result[i] = mapFunction(this[i]);

            return result;
        }

        public int IndexOf(TValue value)
        {
            var comparer = EqualityComparer<TValue>.Default;
            if (Length > 0)
            {
                if (comparer.Equals(FirstValue, value))
                    return 0;
                if (AdditionalValues != null)
                {
                    for (var i = 0; i < Length - 1; ++i)
                        if (comparer.Equals(AdditionalValues[i], value))
                            return i + 1;
                }
            }

            return -1;
        }

        public int Append(TValue value)
        {
            if (Length == 0)
            {
                FirstValue = value;
            }
            else if (AdditionalValues == null)
            {
                AdditionalValues = new TValue[1];
                AdditionalValues[0] = value;
            }
            else
            {
                Array.Resize(ref AdditionalValues, Length);
                AdditionalValues[Length - 1] = value;
            }

            var index = Length;
            ++Length;
            return index;
        }

        public int AppendWithCapacity(TValue value, int capacityIncrement = 10)
        {
            if (Length == 0)
            {
                FirstValue = value;
            }
            else
            {
                var numAdditionalValues = Length - 1;
                ArrayHelper.AppendWithCapacity(ref AdditionalValues, ref numAdditionalValues, value, capacityIncrement: capacityIncrement);
            }

            var index = Length;
            ++Length;
            return index;
        }

        public void AssignWithCapacity(InlinedArray<TValue> values)
        {
            if (Capacity < values.Length && values.Length > 1)
                AdditionalValues = new TValue[values.Length - 1];

            Length = values.Length;
            if (Length > 0)
                FirstValue = values.FirstValue;
            if (Length > 1)
                Array.Copy(values.AdditionalValues, AdditionalValues, Length - 1);
        }

        public void Append(IEnumerable<TValue> values)
        {
            foreach (var value in values)
                Append(value);
        }

        public void Remove(TValue value)
        {
            if (Length < 1)
                return;

            if (EqualityComparer<TValue>.Default.Equals(FirstValue, value))
            {
                RemoveAt(0);
            }
            else if (AdditionalValues != null)
            {
                for (var i = 0; i < Length - 1; ++i)
                {
                    if (EqualityComparer<TValue>.Default.Equals(AdditionalValues[i], value))
                    {
                        RemoveAt(i + 1);
                        break;
                    }
                }
            }
        }

        public void RemoveAtWithCapacity(int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == 0)
            {
                if (Length == 1)
                {
                    FirstValue = default;
                }
                else if (Length == 2)
                {
                    FirstValue = AdditionalValues[0];
                    AdditionalValues[0] = default;
                }
                else
                {
                    Debug.Assert(Length > 2);
                    FirstValue = AdditionalValues[0];
                    var numAdditional = Length - 1;
                    ArrayHelper.EraseAtWithCapacity(AdditionalValues, ref numAdditional, 0);
                }
            }
            else
            {
                var numAdditional = Length - 1;
                ArrayHelper.EraseAtWithCapacity(AdditionalValues, ref numAdditional, index - 1);
            }

            --Length;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == 0)
            {
                if (AdditionalValues != null)
                {
                    FirstValue = AdditionalValues[0];
                    if (AdditionalValues.Length == 1)
                        AdditionalValues = null;
                    else
                    {
                        Array.Copy(AdditionalValues, 1, AdditionalValues, 0, AdditionalValues.Length - 1);
                        Array.Resize(ref AdditionalValues, AdditionalValues.Length - 1);
                    }
                }
                else
                {
                    FirstValue = default;
                }
            }
            else
            {
                Debug.Assert(AdditionalValues != null);

                var numAdditionalValues = Length - 1;
                if (numAdditionalValues == 1)
                {
                    // Remove only entry in array.
                    AdditionalValues = null;
                }
                else if (index == Length - 1)
                {
                    // Remove entry at end.
                    Array.Resize(ref AdditionalValues, numAdditionalValues - 1);
                }
                else
                {
                    // Remove entry at beginning or in middle by pasting together
                    // into a new array.
                    var newAdditionalValues = new TValue[numAdditionalValues - 1];
                    if (index >= 2)
                    {
                        // Copy elements before entry.
                        Array.Copy(AdditionalValues, 0, newAdditionalValues, 0, index - 1);
                    }

                    // Copy elements after entry. We already know that we're not removing
                    // the last entry so there have to be entries.
                    Array.Copy(AdditionalValues, index + 1 - 1, newAdditionalValues, index - 1,
                        Length - index - 1);

                    AdditionalValues = newAdditionalValues;
                }
            }

            --Length;
        }

        public void RemoveAtByMovingTailWithCapacity(int index)
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            var numAdditionalValues = Length - 1;
            if (index == 0)
            {
                if (Length > 1)
                {
                    FirstValue = AdditionalValues[numAdditionalValues - 1];
                    AdditionalValues[numAdditionalValues - 1] = default;
                }
                else
                {
                    FirstValue = default;
                }
            }
            else
            {
                Debug.Assert(AdditionalValues != null);

                ArrayHelper.EraseAtByMovingTail(AdditionalValues, ref numAdditionalValues, index - 1);
            }

            --Length;
        }

        public bool RemoveByMovingTailWithCapacity(TValue value)
        {
            var index = IndexOf(value);
            if (index == -1)
                return false;

            RemoveAtByMovingTailWithCapacity(index);
            return true;
        }

        public bool Contains(TValue value, IEqualityComparer<TValue> comparer)
        {
            for (var n = 0; n < Length; ++n)
                if (comparer.Equals(this[n], value))
                    return true;
            return false;
        }

        public void Merge(InlinedArray<TValue> other)
        {
            var comparer = EqualityComparer<TValue>.Default;
            for (var i = 0; i < other.Length; ++i)
            {
                var value = other[i];
                if (Contains(value, comparer))
                    continue;

                ////FIXME: this is ugly as it repeatedly copies
                Append(value);
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator { Array = this, Index = -1 };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct Enumerator : IEnumerator<TValue>
        {
            public InlinedArray<TValue> Array;
            public int Index;

            public bool MoveNext()
            {
                if (Index >= Array.Length)
                    return false;
                ++Index;
                return Index < Array.Length;
            }

            public void Reset()
            {
                Index = -1;
            }

            public TValue Current => Array[Index];
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    internal static class InputArrayExtensions
    {
        public static int IndexOfReference<TValue>(this InlinedArray<TValue> array, TValue value)
            where TValue : class
        {
            for (var i = 0; i < array.Length; ++i)
                if (ReferenceEquals(array[i], value))
                    return i;

            return -1;
        }

        public static bool Contains<TValue>(this InlinedArray<TValue> array, TValue value)
        {
            for (var i = 0; i < array.Length; ++i)
                if (array[i].Equals(value))
                    return true;
            return false;
        }

        public static bool ContainsReference<TValue>(this InlinedArray<TValue> array, TValue value)
            where TValue : class
        {
            return IndexOfReference(array, value) != -1;
        }
    }
}