// /_/src/Flecs.NET/Generated/ObserverBuilder/ObserverBuilder/T4.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/ObserverBuilder.cs
#nullable enable

using System;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <summary>
///     A type-safe wrapper around <see cref="ObserverBuilder"/> that takes 4 type arguments.
/// </summary>
/// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam>
public unsafe partial struct ObserverBuilder<T0, T1, T2, T3> : IDisposable, IEquatable<ObserverBuilder<T0, T1, T2, T3>>, IQueryBuilder<ObserverBuilder<T0, T1, T2, T3>, Observer<T0, T1, T2, T3>>
{
    private ObserverBuilder _observerBuilder;

    /// <inheritdoc cref="ObserverBuilder.World"/>
    public ref ecs_world_t* World => ref _observerBuilder.World;

    /// <inheritdoc cref="ObserverBuilder.Desc"/>
    public ref ecs_observer_desc_t Desc => ref _observerBuilder.Desc;

    /// <inheritdoc cref="ObserverBuilder.QueryBuilder"/>
    public ref QueryBuilder QueryBuilder => ref _observerBuilder.QueryBuilder;

    /// <summary>
    ///     Creates an observer builder with the provided observer builder.
    /// </summary>
    /// <param name="observerBuilder">The observer builder.</param>
    public ObserverBuilder(ObserverBuilder observerBuilder)
    {
        TypeHelper<T0, T1, T2, T3>.AssertNoTags();
        _observerBuilder = observerBuilder;
    }

    /// <inheritdoc cref="ObserverBuilder(ecs_world_t*)"/>
    public ObserverBuilder(ecs_world_t* world)
    {
        TypeHelper<T0, T1, T2, T3>.AssertNoTags();
        _observerBuilder = new ObserverBuilder(world).With<T0>().With<T1>().With<T2>().With<T3>();
    }

    /// <inheritdoc cref="ObserverBuilder(ecs_world_t*, string)"/>
    public ObserverBuilder(ecs_world_t* world, string name)
    {
        TypeHelper<T0, T1, T2, T3>.AssertNoTags();
        _observerBuilder = new ObserverBuilder(world, name).With<T0>().With<T1>().With<T2>().With<T3>();
    }

    /// <inheritdoc cref="ObserverBuilder.Dispose()"/>
    public void Dispose()
    {
        _observerBuilder.Dispose();
    }

    /// <inheritdoc cref="ObserverBuilder.Event(ulong)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Event(ulong @event)
    {
        _observerBuilder.Event(@event);
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.Event{T}()"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Event<T>()
    {
        _observerBuilder.Event<T>();
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.YieldExisting(bool)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> YieldExisting(bool value = true)
    {
        _observerBuilder.YieldExisting(value);
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(T)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(T value)
    {
        _observerBuilder.Ctx(ref value);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(T value, Ecs.UserContextFinish<T> callback)
    {
        _observerBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(T value, delegate*<ref T, void> callback)
    {
        _observerBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(ref T)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(ref T value)
    {
        _observerBuilder.Ctx(ref value);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(ref T value, Ecs.UserContextFinish<T> callback)
    {
        _observerBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Ctx<T>(ref T value, delegate*<ref T, void> callback)
    {
        _observerBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="ObserverBuilder.Run(System.Action)"/>
    public Observer<T0, T1, T2, T3> Run(Action callback)
    {
        return new Observer<T0, T1, T2, T3>(_observerBuilder.Run(callback));
    }
    
    /// <inheritdoc cref="ObserverBuilder.Run(System.Action)"/>
    public Observer<T0, T1, T2, T3> Run(delegate*<void> callback)
    {
        return new Observer<T0, T1, T2, T3>(_observerBuilder.Run(callback));
    }

    /// <inheritdoc cref="ObserverBuilder.Run(Ecs.RunDelegateCallback)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Run(Ecs.RunDelegateCallback callback)
    {
        _observerBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.Run(Ecs.RunDelegateCallback)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Run(delegate*<Iter, Action<Iter>, void> callback)
    {
        _observerBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.Run(Ecs.RunPointerCallback)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Run(Ecs.RunPointerCallback callback)
    {
        _observerBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="ObserverBuilder.Run(Ecs.RunPointerCallback)"/>
    public ref ObserverBuilder<T0, T1, T2, T3> Run(delegate*<Iter, delegate*<Iter, void>, void> callback)
    {
        _observerBuilder.Run(callback);
        return ref this;
    }

    private ref ObserverBuilder<T0, T1, T2, T3> SetCallback<T>(T callback, nint invoker) where T : Delegate
    {
        _observerBuilder.SetCallback(callback, invoker);
        return ref this;
    }

    private ref ObserverBuilder<T0, T1, T2, T3> SetCallback(nint callback, nint invoker)
    {
        _observerBuilder.SetCallback(callback, invoker);
        return ref this;
    }

    private ref ObserverBuilder<T0, T1, T2, T3> SetRun<T>(T callback, nint invoker) where T : Delegate
    {
        _observerBuilder.SetRun(callback, invoker);
        return ref this;
    }

    private ref ObserverBuilder<T0, T1, T2, T3> SetRun(nint callback, nint invoker)
    {
        _observerBuilder.SetRun(callback, invoker);
        return ref this;
    }

    private Observer<T0, T1, T2, T3> Build()
    {
        return new Observer<T0, T1, T2, T3>(_observerBuilder.Build());
    }

    Observer<T0, T1, T2, T3> IQueryBuilder<ObserverBuilder<T0, T1, T2, T3>, Observer<T0, T1, T2, T3>>.Build()
    {
        return Build();
    }

    /// <inheritdoc cref="ObserverBuilder.Equals(ObserverBuilder)"/>
    public bool Equals(ObserverBuilder<T0, T1, T2, T3> other)
    {
        return _observerBuilder == other._observerBuilder;
    }

    /// <inheritdoc cref="ObserverBuilder.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        return obj is ObserverBuilder<T0, T1, T2, T3> other && Equals(other);
    }

    /// <inheritdoc cref="ObserverBuilder.GetHashCode()"/>
    public override int GetHashCode()
    {
        return _observerBuilder.GetHashCode();
    }

    /// <inheritdoc cref="ObserverBuilder.op_Equality"/>
    public static bool operator ==(ObserverBuilder<T0, T1, T2, T3> left, ObserverBuilder<T0, T1, T2, T3> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc cref="ObserverBuilder.op_Inequality"/>
    public static bool operator !=(ObserverBuilder<T0, T1, T2, T3> left, ObserverBuilder<T0, T1, T2, T3> right)
    {
        return !(left == right);
    }
}