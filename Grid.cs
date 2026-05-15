using VectorInt;
using MemoryPack;
using System.Collections;

namespace Vertex
{
    [MemoryPackable]
    public partial class BoundedInt
    {
        private int _value;

        /// <summary>
        /// Inclusive
        /// </summary>
        public int? Min { get; init; }
        /// <summary>
        /// Exclusive
        /// </summary>
        public int? Max { get; init; }

        public int Value
        {
            get => _value;
            private set
            {
                if (value < Min || value >= Max)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"Value must be between {Min} (inclusive) and {Max} (exclusive). You tried to change it to {value}."
                    );

                _value = value;
            }
        }

        public BoundedInt(int? min, int? max, int value)
        {
            if (min >= max)
                throw new ArgumentException("Min cannot be greater than Max");

            Min = min;
            Max = max;

            Value = value;
        }

        public static implicit operator BoundedInt(int i) => new(null, null, i);

        public void AddToThis(BoundedInt bi) => Value += bi.Value;
        public void SubtractFromThis(BoundedInt bi) => Value += bi.Value;
    }

    [MemoryPackable]
    public partial class Grid<T> : IEnumerable<KeyValuePair<VectorInt2, T>>
    {
        /// <summary>
        /// Minimum is inclusive, maximum is exclusive (for both axes)
        /// </summary>
        public (VectorInt2, VectorInt2)? MinMaxBounds { get; init; }
        /// <summary>
        /// 1 means not null MinMaxBounds, 2 means it's nullable (CAN BE NULLABLE AND THIS MIGHT NOT BE >=2), 3 means each empty point should be null
        /// </summary>
        public BoundedInt NullableStages { get; init; }
        private Dictionary<VectorInt2, T> GridDictionary { get; set; } = [];
        
        public Grid((VectorInt2, VectorInt2)? minMaxBounds, bool isEmptyPointNull)
        {
            MinMaxBounds = minMaxBounds;
            NullableStages = new(0, 4, 0);
            if (MinMaxBounds != null) {
                NullableStages.AddToThis(1); 
                if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    NullableStages.AddToThis(1);
                    if (isEmptyPointNull) { NullableStages.AddToThis(1); }
                }
            }
            if (NullableStages.Value == 3)
            {
                for (int i = MinMaxBounds!.Value.Item1.X; i < MinMaxBounds.Value.Item2.X; i++)
                {
                    for (int j = MinMaxBounds.Value.Item1.Y; j < MinMaxBounds.Value.Item2.Y; j++)
                    {
                        GridDictionary[(i, j)] = default!;
                    }
                }
            }
        }

        [MemoryPackConstructor]
        public Grid((VectorInt2, VectorInt2)? minMaxBounds, int creationOfNullableStages, Dictionary<VectorInt2, T> gridDictionary)
        {
            MinMaxBounds = minMaxBounds;
            NullableStages = new(0, 4, creationOfNullableStages);
            GridDictionary = gridDictionary;5
        }

        public T this[VectorInt2 key]
        {
            get
            {
                if (InBounds(key))
                { 
                    if (GridDictionary.TryGetValue(key, out T? value)) { return value!; }
                    else { throw new($"Value from key {key} does not exist."); }
                }
                else
                { throw new($"Key exceeds inclusive lower bound or exclusive upper bound. Can't be bothered to tell you though so figure it out yourself. You're welcome :) Ok fine I'll tell you. Your key is {key} and your lower and upper bounds are {MinMaxBounds}. Now you're properly welcome :)"); }
            }
            set
            {
                if (InBounds(key))
                { GridDictionary[key] = value; }
                else
                { throw new($"Key exceeds inclusive lower bound or exclusive upper bound. Can't be bothered to tell you though so figure it out yourself. You're welcome :) Ok fine I'll tell you. Your key is {key} and your lower and upper bounds are {MinMaxBounds}. Now you're properly welcome :)"); }
            }
        }

        public void Remove(VectorInt2 key) { if (NullableStages.Value == 3) GridDictionary[key] = default!; else GridDictionary.Remove(key); }

        public static implicit operator Dictionary<VectorInt2, T>(Grid<T> grid) => grid.GridDictionary;

        public IEnumerator<KeyValuePair<VectorInt2, T>> GetEnumerator()
        { foreach (KeyValuePair<VectorInt2, T> kvp in GridDictionary) { yield return kvp; } }

        IEnumerator<KeyValuePair<VectorInt2, T>> IEnumerable<KeyValuePair<VectorInt2, T>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public static bool FirstVectorInt2IsCloserToOriginThanSecondVectorInt2(VectorInt2 v1, VectorInt2 v2) => int.Max(int.Abs(v1.X), int.Abs(v1.Y)) < int.Max(int.Abs(v2.X), int.Abs(v2.Y));
        public static bool FirstVectorInt2IsEquallyCloseToOriginThanSecondVectorInt2(VectorInt2 v1, VectorInt2 v2) => int.Max(int.Abs(v1.X), int.Abs(v1.Y)) == int.Max(int.Abs(v2.X), int.Abs(v2.Y));
        public static bool FirstVectorInt2IsFutherAwayFromOriginThanSecondVectorInt2(VectorInt2 v1, VectorInt2 v2) => int.Max(int.Abs(v1.X), int.Abs(v1.Y)) > int.Max(int.Abs(v2.X), int.Abs(v2.Y));

        public bool InBounds(VectorInt2 key)
        {
            if (MinMaxBounds != null)
            {
                return key.X >= MinMaxBounds!.Value.Item1.X &&
                    key.X < MinMaxBounds!.Value.Item2.X &&
                    key.Y >= MinMaxBounds!.Value.Item1.Y &&
                    key.Y < MinMaxBounds!.Value.Item2.Y;
            }
            else { return true; }
        }
    }
}