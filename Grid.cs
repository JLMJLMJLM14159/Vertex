using VectorInt;
using MemoryPack;
using System.Collections;

namespace Vertex
{
    [MemoryPackable]
    public partial class Grid<T>((VectorInt2, VectorInt2)? minMaxBounds) : IEnumerable<KeyValuePair<VectorInt2, T>>
    {
        public (VectorInt2, VectorInt2)? MinMaxBounds { get; set; } = minMaxBounds; //MIN IS INCLUSIVE, MAX IS EXCLUSIVE
        private Dictionary<VectorInt2, T> GridDictionary { get; set; } = [];

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

        public void Remove(VectorInt2 vectorInt2) => GridDictionary.Remove(vectorInt2);

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