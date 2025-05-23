// /_/src/Flecs.NET/Generated/TypeHelper/T7.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/TypeHelper.cs
#nullable enable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
internal static unsafe partial class TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7>
{
    private static string[]? _typeNames;
    public static string[] TypeNames => _typeNames ??= [ Type<T0>.FullName, Type<T1>.FullName, Type<T2>.FullName, Type<T3>.FullName, Type<T4>.FullName, Type<T5>.FullName, Type<T6>.FullName, Type<T7>.FullName ];
    
    public static readonly int Tags = (Type<T0>.IsTag ? 1 << 0 : 0) | (Type<T1>.IsTag ? 1 << 1 : 0) | (Type<T2>.IsTag ? 1 << 2 : 0) | (Type<T3>.IsTag ? 1 << 3 : 0) | (Type<T4>.IsTag ? 1 << 4 : 0) | (Type<T5>.IsTag ? 1 << 5 : 0) | (Type<T6>.IsTag ? 1 << 6 : 0) | (Type<T7>.IsTag ? 1 << 7 : 0);
    public static readonly int ReferenceTypes = (RuntimeHelpers.IsReferenceOrContainsReferences<T0>() ? 1 << 0 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T1>() ? 1 << 1 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T2>() ? 1 << 2 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T3>() ? 1 << 3 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T4>() ? 1 << 4 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T5>() ? 1 << 5 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T6>() ? 1 << 6 : 0) | (RuntimeHelpers.IsReferenceOrContainsReferences<T7>() ? 1 << 7 : 0);
    
    private static string GetTypeListString(int fields)
    {
        return string.Join(", ", Enumerable.Range(0, 8)
            .Where(i => (fields & (1 << i)) != 0)
            .Select(i => TypeNames[i]));
    }
    
    [Conditional("DEBUG")]
    public static void AssertNoTags()
    {
        if (Tags == 0)
            return;
    
        Ecs.Error($"Cannot use zero-sized structs as generic type arguments for this struct. Remove the following type arguments: {GetTypeListString(Tags)}");
    }
    
    [Conditional("DEBUG")]
    public static void AssertReferenceTypes(bool allowReferenceTypes)
    {
        if (allowReferenceTypes || ReferenceTypes == 0)
            return;
    
        Ecs.Error($"Cannot use managed types as generic type arguments for callback signatures that retrieve pointers or spans. Remove the following type arguments: {GetTypeListString(ReferenceTypes)}");
    }
    
    [Conditional("DEBUG")]
    public static void AssertSparseTypes(ecs_world_t* world, bool allowSparseTypes)
    {
        if (allowSparseTypes)
            return;
 
        int sparseTypes = (ecs_has_id(world, Type<T0>.Id(world), Ecs.Sparse) ? 1 << 0 : 0) | (ecs_has_id(world, Type<T1>.Id(world), Ecs.Sparse) ? 1 << 1 : 0) | (ecs_has_id(world, Type<T2>.Id(world), Ecs.Sparse) ? 1 << 2 : 0) | (ecs_has_id(world, Type<T3>.Id(world), Ecs.Sparse) ? 1 << 3 : 0) | (ecs_has_id(world, Type<T4>.Id(world), Ecs.Sparse) ? 1 << 4 : 0) | (ecs_has_id(world, Type<T5>.Id(world), Ecs.Sparse) ? 1 << 5 : 0) | (ecs_has_id(world, Type<T6>.Id(world), Ecs.Sparse) ? 1 << 6 : 0) | (ecs_has_id(world, Type<T7>.Id(world), Ecs.Sparse) ? 1 << 7 : 0);
 
        if (sparseTypes == 0)
            return;
 
        Ecs.Error($"Cannot use sparse components as generic type arguments for this struct when using .Iter() to iterate because sparse fields must be obtained with Iter.FieldAt(). Use .Each()/.Run() to iterate or remove the following types from the list: {GetTypeListString(sparseTypes)}");
    }
}