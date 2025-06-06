// /_/src/Flecs.NET/Generated/IterIterable/IterIterable/T3.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/IterIterable.cs
#nullable enable

using System;
using System.Runtime.CompilerServices;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <inheritdoc cref="IterIterable"/>
/// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam>
public unsafe partial struct IterIterable<T0, T1, T2> : IEquatable<IterIterable<T0, T1, T2>>
{
    /// <inheritdoc cref="IIterIterable.Underlying"/>
    public IterIterable Underlying;
    
    /// <inheritdoc cref="IIterIterable.Iterator"/>
    public ref ecs_iter_t Iterator => ref Underlying.Iterator;
    
    /// <inheritdoc cref="IIterIterable.IterableType"/>
    public readonly IterableType IterableType => Underlying.IterableType;
    
    internal IterIterable(IterIterable handle)
    {
        Underlying = handle;
    }

    /// <inheritdoc cref="IterIterable(ecs_iter_t, IterableType)"/>
    public IterIterable(ecs_iter_t iter, IterableType iterableType)
    {
        Underlying = new IterIterable(iter, iterableType);
    }

    /// <inheritdoc cref="IterIterable.SetVar(int, ulong)"/>
    public ref IterIterable<T0, T1, T2> SetVar(int varId, ulong value)
    {
        Underlying.SetVar(varId, value);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.SetVar(string, ulong)"/>
    public ref IterIterable<T0, T1, T2> SetVar(string name, ulong value)
    {
        Underlying.SetVar(name, value);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.SetVar(string, ecs_table_t*)"/>
    public ref IterIterable<T0, T1, T2> SetVar(string name, ecs_table_t* value)
    {
        Underlying.SetVar(name, value);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.SetVar(string, ecs_table_range_t)"/>
    public ref IterIterable<T0, T1, T2> SetVar(string name, ecs_table_range_t value)
    {
        Underlying.SetVar(name, value);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.SetVar(string, Table)"/>
    public ref IterIterable<T0, T1, T2> SetVar(string name, Table value)
    {
        Underlying.SetVar(name, value);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.ToJson(in IterToJsonDesc)"/>
    public string ToJson(in IterToJsonDesc desc)
    {
        return Underlying.ToJson(in desc);
    }
    
    /// <inheritdoc cref="IterIterable.ToJson()"/>
    public string ToJson()
    {
        return Underlying.ToJson();
    }

    /// <inheritdoc cref="IterIterable.Count()"/>
    public int Count()
    {
        return Underlying.Count();
    }

    /// <inheritdoc cref="IterIterable.IsTrue()"/>
    public bool IsTrue()
    {
        return Underlying.IsTrue();
    }

    /// <inheritdoc cref="IterIterable.First()"/>
    public Entity First()
    {
        return Underlying.First();
    }

    /// <inheritdoc cref="IterIterable.SetGroup(ulong)"/>
    public ref IterIterable<T0, T1, T2> SetGroup(ulong groupId)
    {
        Underlying.SetGroup(groupId);
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.SetGroup{T}()"/>
    public ref IterIterable<T0, T1, T2> SetGroup<T>()
    {
        Underlying.SetGroup<T>();
        return ref this;
    }

    /// <inheritdoc cref="IterIterable.Equals(IterIterable)"/>
    public bool Equals(IterIterable<T0, T1, T2> other)
    {
        return Underlying.Equals(other.Underlying);
    }

    /// <inheritdoc cref="IterIterable.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        return obj is IterIterable<T0, T1, T2> other && Equals(other);
    }

    /// <inheritdoc cref="IterIterable.GetHashCode()"/>
    public override int GetHashCode()
    {
        return Underlying.GetHashCode();
    }

    /// <inheritdoc cref="IterIterable.op_Equality"/>
    public static bool operator ==(IterIterable<T0, T1, T2> left, IterIterable<T0, T1, T2> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc cref="IterIterable.op_Inequality"/>
    public static bool operator !=(IterIterable<T0, T1, T2> left, IterIterable<T0, T1, T2> right)
    {
        return !(left == right);
    }
}

// IIterIterable Interface
public unsafe partial struct IterIterable<T0, T1, T2> : IIterIterable
{
    ref IterIterable IIterIterable.Underlying => ref Underlying;
}

// IIterableBase Interface
public unsafe partial struct IterIterable<T0, T1, T2> : IIterableBase
{
    /// <inheritdoc cref="IIterableBase.World"/>
    ecs_world_t* IIterableBase.World => Iterator.world;
    
    /// <inheritdoc cref="IIterableBase.GetIter"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ecs_iter_t GetIter(World world = default)
    {
        return Underlying.GetIter(world);
    }
    
    /// <inheritdoc cref="IIterableBase.GetNext"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetNext(Iter it)
    {
        return Underlying.GetNext(it);
    }
}

// IIterable<T0, T1, T2> Interface
public unsafe partial struct IterIterable<T0, T1, T2> : IIterable<T0, T1, T2>
{
    /// <inheritdoc cref="IterIterable.Page(int, int)"/>
    public PageIterable<T0, T1, T2> Page(int offset, int limit)
    {
        return new PageIterable<T0, T1, T2>(Underlying.Page(offset, limit));
    }
    
    /// <inheritdoc cref="IterIterable.Worker(int, int)"/>
    public WorkerIterable<T0, T1, T2> Worker(int index, int count)
    {
        return new WorkerIterable<T0, T1, T2>(Underlying.Worker(index, count));
    }

    /// <inheritdoc cref="IterIterable.Iter(Flecs.NET.Core.World)"/>
    public IterIterable<T0, T1, T2> Iter(World world = default)
    {
        return new(Underlying.Iter(world));
    }
    
    /// <inheritdoc cref="IterIterable.Iter(Flecs.NET.Core.Iter)"/>
    public IterIterable<T0, T1, T2> Iter(Iter it)
    {
        return new(Underlying.Iter(it));
    }
    
    /// <inheritdoc cref="IterIterable.Iter(Flecs.NET.Core.Entity)"/>
    public IterIterable<T0, T1, T2> Iter(Entity entity)
    {
        return new(Underlying.Iter(entity));
    }
    
    /// <inheritdoc cref="IterIterable.SetVar(int, ulong)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetVar(int varId, ulong value)
    {
        return new(Underlying.SetVar(varId, value));
    }
    
    /// <inheritdoc cref="IterIterable.SetVar(string, ulong)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetVar(string name, ulong value)
    {
        return new(Underlying.SetVar(name, value));
    }
    
    /// <inheritdoc cref="IterIterable.SetVar(string, ecs_table_t*)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetVar(string name, ecs_table_t* value)
    {
        return new(Underlying.SetVar(name, value));
    }
    
    /// <inheritdoc cref="IterIterable.SetVar(string, ecs_table_range_t)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetVar(string name, ecs_table_range_t value)
    {
        return new(Underlying.SetVar(name, value));
    }
    
    /// <inheritdoc cref="IterIterable.SetVar(string, Table)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetVar(string name, Table value)
    {
        return new(Underlying.SetVar(name, value));
    }
    
    /// <inheritdoc cref="IterIterable.SetGroup(ulong)"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetGroup(ulong groupId)
    {
        return new(Underlying.SetGroup(groupId));
    }
    
    /// <inheritdoc cref="IterIterable.SetGroup{T}()"/>
    IterIterable<T0, T1, T2> IIterable<T0, T1, T2>.SetGroup<T>()
    {
        return new(Underlying.SetGroup<T>()); 
    }
}