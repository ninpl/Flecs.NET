// /_/src/Flecs.NET/Generated/System_/System/T14.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/System_.cs
#nullable enable

using System;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <summary>
///     A type-safe wrapper around <see cref="System"/> that takes 14 type arguments.
/// </summary>
/// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam> <typeparam name="T9">The T9 component type.</typeparam> <typeparam name="T10">The T10 component type.</typeparam> <typeparam name="T11">The T11 component type.</typeparam> <typeparam name="T12">The T12 component type.</typeparam> <typeparam name="T13">The T13 component type.</typeparam>
public unsafe partial struct System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IDisposable, IEquatable<System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, IEntity<System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>
{
    private System_ _system;

    /// <inheritdoc cref="System_.Entity"/>
    public ref Entity Entity => ref _system.Entity;

    /// <inheritdoc cref="System_.Id"/>
    public ref Id Id => ref _system.Id;

    /// <inheritdoc cref="System_.World"/>
    public ref ecs_world_t* World => ref _system.World;

    /// <summary>
    ///     Creates a new system with the provided system.
    /// </summary>
    /// <param name="system">The system.</param>
    public System(System_ system)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.AssertNoTags();
        _system = system;
    }

    /// <inheritdoc cref="System_(ecs_world_t*, ulong)"/>
    public System(ecs_world_t* world, ulong entity)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.AssertNoTags();
        _system = new System_(world, entity);
    }

    /// <inheritdoc cref="System_(Core.Entity)"/>
    public System(Entity entity)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.AssertNoTags();
        _system = new System_(entity);
    }
    
    /// <inheritdoc cref="System_.Dispose"/>
    public void Dispose()
    {
        _system.Dispose();
    }

    /// <inheritdoc cref="System_.Ctx{T}(T)"/>
    public void Ctx<T>(T value)
    {
        _system.Ctx(ref value);
    }
    
    /// <inheritdoc cref="System_.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public void Ctx<T>(T value, Ecs.UserContextFinish<T> callback)
    {
        _system.Ctx(ref value, callback);
    }
    
    /// <inheritdoc cref="System_.Ctx{T}(T, Ecs.UserContextFinish{T})"/>
    public void Ctx<T>(T value, delegate*<ref T, void> callback)
    {
        _system.Ctx(ref value, callback);
    }
    
    /// <inheritdoc cref="System_.Ctx{T}(ref T)"/>
    public void Ctx<T>(ref T value)
    {
        _system.Ctx(ref value);
    }
    
    /// <inheritdoc cref="System_.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public void Ctx<T>(ref T value, Ecs.UserContextFinish<T> callback)
    {
        _system.Ctx(ref value, callback);
    }
    
    /// <inheritdoc cref="System_.Ctx{T}(ref T, Ecs.UserContextFinish{T})"/>
    public void Ctx<T>(ref T value, delegate*<ref T, void> callback)
    {
        _system.Ctx(ref value, callback);
    }

    /// <inheritdoc cref="System_.Ctx{T}()"/>
    public ref T Ctx<T>()
    {
        return ref _system.Ctx<T>();
    }

    /// <inheritdoc cref="System_.Query()"/>
    public Query<T0> Query()
    {
        return new Query<T0>(ecs_system_get(World, Entity)->query);
    }

    /// <inheritdoc cref="System_.Run(float)"/>
    public void Run(float deltaTime = 0)
    {
        _system.Run(deltaTime);
    }

    /// <inheritdoc cref="System_.RunWithParam"/>
    public void RunWithParam(float deltaTime = 0, void* param = null)
    {
        _system.RunWithParam(deltaTime, param);
    }

    /// <inheritdoc cref="System_.RunWorker"/>
    public void RunWorker(int stageCurrent, int stageCount, float deltaTime = 0)
    {
        _system.RunWorker(stageCurrent, stageCount, deltaTime);
    }

    /// <inheritdoc cref="System_.RunWorkerWithParam"/>
    public void RunWorkerWithParam(int stageCurrent, int stageCount, float deltaTime = 0, void* param = null)
    {
        _system.RunWorkerWithParam(stageCurrent, stageCount, deltaTime, param);
    }

    /// <inheritdoc cref="System_.Interval(float)"/>
    public void Interval(float interval)
    {
        _system.Interval(interval);
    }

    /// <inheritdoc cref="System_.Interval()"/>
    public float Interval()
    {
        return _system.Interval();
    }

    /// <inheritdoc cref="System_.Timeout(float)"/>
    public void Timeout(float timeout)
    {
        _system.Timeout(timeout);
    }

    /// <inheritdoc cref="System_.Timeout()"/>
    public float Timeout()
    {
        return _system.Timeout();
    }

    /// <inheritdoc cref="System_.Rate(int)"/>
    public void Rate(int rate)
    {
        _system.Rate(rate);
    }

    /// <inheritdoc cref="System_.Start()"/>
    public void Start()
    {
        _system.Start();
    }

    /// <inheritdoc cref="System_.StopTimer()"/>
    public void StopTimer()
    {
        _system.StopTimer();
    }

    /// <inheritdoc cref="System_.SetTickSource(ulong)"/>
    public void SetTickSource(ulong entity)
    {
        _system.SetTickSource(entity);
    }

    /// <inheritdoc cref="System_.SetTickSource(TimerEntity)"/>
    public void SetTickSource(TimerEntity timerEntity)
    {
        _system.SetTickSource(timerEntity);
    }

    /// <inheritdoc cref="System_.SetTickSource{T}()"/>
    public void SetTickSource<T>()
    {
        _system.SetTickSource<T>();
    }

    /// <inheritdoc cref="System_.ToUInt64"/>
    public static implicit operator ulong(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return ToUInt64(system);
    }

    /// <inheritdoc cref="System_.ToId"/>
    public static implicit operator Id(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return ToId(system);
    }

    /// <inheritdoc cref="System_.ToEntity(System_)"/>
    public static implicit operator Entity(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return ToEntity(system);
    }

    /// <inheritdoc cref="System_.ToUInt64"/>
    public static ulong ToUInt64(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return system.Entity;
    }

    /// <inheritdoc cref="System_.ToId"/>
    public static Id ToId(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return system.Id;
    }

    /// <inheritdoc cref="System_.ToEntity(System_)"/>
    public static Entity ToEntity(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system)
    {
        return system.Entity;
    }

    /// <inheritdoc cref="System_.ToString()"/>
    public override string ToString()
    {
        return _system.ToString();
    }

    /// <inheritdoc cref="System_.Equals(System_)"/>
    public bool Equals(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> other)
    {
        return _system == other._system;
    }

    /// <inheritdoc cref="System_.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        return obj is System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> system && Equals(system);
    }

    /// <inheritdoc cref="System_.GetHashCode()"/>
    public override int GetHashCode()
    {
        return _system.GetHashCode();
    }

    /// <inheritdoc cref="System_.op_Equality"/>
    public static bool operator ==(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> left, System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc cref="System_.op_Inequality"/>
    public static bool operator !=(System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> left, System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> right)
    {
        return !(left == right);
    }
}