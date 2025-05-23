// /_/src/Flecs.NET/Generated/SystemBuilder/SystemBuilder/T3.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/SystemBuilder.cs
#nullable enable

using System;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <summary>
///     A type-safe wrapper over <see cref="SystemBuilder"/> that takes 3 type arguments.
/// </summary>
/// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam>
public unsafe partial struct SystemBuilder<T0, T1, T2> : IDisposable, IEquatable<SystemBuilder<T0, T1, T2>>, IQueryBuilder<SystemBuilder<T0, T1, T2>, System<T0, T1, T2>>
{
    private SystemBuilder _systemBuilder;

    /// <inheritdoc cref="SystemBuilder.World"/>
    public ref ecs_world_t* World => ref _systemBuilder.World;

    /// <inheritdoc cref="SystemBuilder.Desc"/>
    public ref ecs_system_desc_t Desc => ref _systemBuilder.Desc;

    /// <inheritdoc cref="SystemBuilder.QueryBuilder"/>
    public ref QueryBuilder QueryBuilder => ref _systemBuilder.QueryBuilder;

    /// <summary>
    ///     Creates a system builder with the provided system builder.
    /// </summary>
    /// <param name="systemBuilder">The system builder.</param>
    public SystemBuilder(SystemBuilder systemBuilder)
    {
        TypeHelper<T0, T1, T2>.AssertNoTags();
        _systemBuilder = systemBuilder;
    }

    /// <inheritdoc cref="SystemBuilder(ecs_world_t*)"/>
    public SystemBuilder(ecs_world_t* world)
    {
        TypeHelper<T0, T1, T2>.AssertNoTags();
        _systemBuilder = new SystemBuilder(world).With<T0>().With<T1>().With<T2>();
    }

    /// <inheritdoc cref="SystemBuilder(ecs_world_t*, string)"/>
    public SystemBuilder(ecs_world_t* world, string name)
    {
        TypeHelper<T0, T1, T2>.AssertNoTags();
        _systemBuilder = new SystemBuilder(world, name).With<T0>().With<T1>().With<T2>();
    }

    /// <inheritdoc cref="SystemBuilder.Dispose()"/>
    public void Dispose()
    {
        _systemBuilder.Dispose();
    }

    /// <inheritdoc cref="SystemBuilder.Kind(ulong)"/>
    public ref SystemBuilder<T0, T1, T2> Kind(ulong phase)
    {
        _systemBuilder.Kind(phase);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Kind{T}()"/>
    public ref SystemBuilder<T0, T1, T2> Kind<T>()
    {
        _systemBuilder.Kind<T>();
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Kind{T}(T)"/>
    public ref SystemBuilder<T0, T1, T2> Kind<T>(T value) where T : Enum
    {
        _systemBuilder.Kind(value);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.MultiThreaded(bool)"/>
    public ref SystemBuilder<T0, T1, T2> MultiThreaded(bool value = true)
    {
        _systemBuilder.MultiThreaded();
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Immediate(bool)"/>
    public ref SystemBuilder<T0, T1, T2> Immediate(bool value = true)
    {
        _systemBuilder.Immediate(value);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Interval(float)"/>
    public ref SystemBuilder<T0, T1, T2> Interval(float interval)
    {
        _systemBuilder.Interval(interval);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Rate(ulong, int)"/>
    public ref SystemBuilder<T0, T1, T2> Rate(ulong tickSource, int rate)
    {
        _systemBuilder.Rate(tickSource, rate);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Rate(int)"/>
    public ref SystemBuilder<T0, T1, T2> Rate(int rate)
    {
        _systemBuilder.Rate(rate);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.TickSource(ulong)"/>
    public ref SystemBuilder<T0, T1, T2> TickSource(ulong tickSource)
    {
        _systemBuilder.TickSource(tickSource);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.TickSource{T}()"/>
    public ref SystemBuilder<T0, T1, T2> TickSource<T>()
    {
        _systemBuilder.TickSource<T>();
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Ctx{T}(T)"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(T value)
    {
        _systemBuilder.Ctx(ref value);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(T value, Ecs.UserContextFinish<T> callback)
    {
        _systemBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(T value, delegate*<ref T, void> callback)
    {
        _systemBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Ctx{T}(ref T)"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(ref T value)
    {
        _systemBuilder.Ctx(ref value);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(ref T value, Ecs.UserContextFinish<T> callback)
    {
        _systemBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public ref SystemBuilder<T0, T1, T2> Ctx<T>(ref T value, delegate*<ref T, void> callback)
    {
        _systemBuilder.Ctx(ref value, callback);
        return ref this;
    }
    
    /// <inheritdoc cref="SystemBuilder.Run(System.Action)"/>
    public System<T0, T1, T2> Run(Action callback)
    {
        return new System<T0, T1, T2>(_systemBuilder.Run(callback));
    }
    
    /// <inheritdoc cref="SystemBuilder.Run(System.Action)"/>
    public System<T0, T1, T2> Run(delegate*<void> callback)
    {
        return new System<T0, T1, T2>(_systemBuilder.Run(callback));
    }

    /// <inheritdoc cref="SystemBuilder.Run(Ecs.RunDelegateCallback)"/>
    public ref SystemBuilder<T0, T1, T2> Run(Ecs.RunDelegateCallback callback)
    {
        _systemBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Run(Ecs.RunDelegateCallback)"/>
    public ref SystemBuilder<T0, T1, T2> Run(delegate*<Iter, Action<Iter>, void> callback)
    {
        _systemBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Run(Ecs.RunPointerCallback)"/>
    public ref SystemBuilder<T0, T1, T2> Run(Ecs.RunPointerCallback callback)
    {
        _systemBuilder.Run(callback);
        return ref this;
    }

    /// <inheritdoc cref="SystemBuilder.Run(Ecs.RunPointerCallback)"/>
    public ref SystemBuilder<T0, T1, T2> Run(delegate*<Iter, delegate*<Iter, void>, void> callback)
    {
        _systemBuilder.Run(callback);
        return ref this;
    }

    internal ref SystemBuilder<T0, T1, T2> SetCallback<T>(T callback, void* invoker) where T : Delegate
    {
        _systemBuilder.SetCallback(callback, invoker);
        return ref this;
    }

    internal ref SystemBuilder<T0, T1, T2> SetCallback(void* callback, void* invoker)
    {
        _systemBuilder.SetCallback(callback, invoker);
        return ref this;
    }

    internal ref SystemBuilder<T0, T1, T2> SetRun<T>(T callback, void* invoker) where T : Delegate
    {
        _systemBuilder.SetRun(callback, invoker);
        return ref this;
    }

    internal ref SystemBuilder<T0, T1, T2> SetRun(void* callback, void* invoker)
    {
        _systemBuilder.SetRun(callback, invoker);
        return ref this;
    }

    internal System<T0, T1, T2> Build()
    {
        return new System<T0, T1, T2>(_systemBuilder.Build());
    }
    
    System<T0, T1, T2> IQueryBuilder<SystemBuilder<T0, T1, T2>, System<T0, T1, T2>>.Build()
    {
        return Build();
    }

    /// <inheritdoc cref="SystemBuilder.Equals(SystemBuilder)"/>
    public bool Equals(SystemBuilder<T0, T1, T2> other)
    {
        return _systemBuilder == other._systemBuilder;
    }

    /// <inheritdoc cref="SystemBuilder.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        return obj is SystemBuilder<T0, T1, T2> other && Equals(other);
    }

    /// <inheritdoc cref="SystemBuilder.GetHashCode()"/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Desc.GetHashCode(), QueryBuilder.GetHashCode());
    }

    /// <inheritdoc cref="SystemBuilder.op_Equality"/>
    public static bool operator ==(SystemBuilder<T0, T1, T2> left, SystemBuilder<T0, T1, T2> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc cref="SystemBuilder.op_Inequality"/>
    public static bool operator !=(SystemBuilder<T0, T1, T2> left, SystemBuilder<T0, T1, T2> right)
    {
        return !(left == right);
    }
}