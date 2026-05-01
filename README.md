# Vertex

Just a simple grid system in C#, used for a few of my projects. Uses VectorInt so no floats. The reference to MemoryPack is just to make it `[MemoryPackable]`, and it does nothing on its own, so don't worry about it.

Uses an MIT liscence, so feel free to use and edit! ([Github project is here](https://github.com/JLMJLMJLM14159/Vertex))

## Documentation

### Creation

To create a `Grid`, simply use

```csharp
Grid<int> grid = new(null);
```

(Replace `int` with whatever type you want to store in the grid, and `null` with a `(VectorInt2, VectorInt2)` for the inclusive minimum bounds and exclusive maximum bounds of the grid if you want, but you can keep it borderless.)

### Usage

To add, change and remove stuff from a `Grid`, do:

```csharp
grid[new(4, 4)] = 3;
grid.Remove(new(4, 4));
```

You can check the distance of different points from the origin (what "ring" they are in around the origin):

```csharp
bool b1 = Grid.FirstVectorInt2IsCloserToOriginThanSecondVectorInt2(v1, v2);
bool b2 = Grid.FirstVectorInt2IsEquallyCloseToOriginThanSecondVectorInt2(v1, v2);
bool b3 = Grid.FirstVectorInt2IsFutherAwayFromOriginThanSecondVectorInt2(v1, v2);
```

(I know, catchy.)

You can check if a coordinate is within the bounds of your `Grid` by doing:

```csharp
bool b = grid.InBounds(v);
```

(Will return true if your `Grid` has no bounds.)

You can cycle through KVPs with foreach:

```csharp
foreach (KeyValuePair<VectorInt2, int> kvp in grid) ...
```

You can implicitly convert the `Grid` to a `Dictionary`:

```csharp
(Dictionary<VectorInt2, int>)grid ...
```
