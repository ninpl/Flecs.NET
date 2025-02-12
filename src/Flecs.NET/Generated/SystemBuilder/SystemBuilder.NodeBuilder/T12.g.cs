// /_/src/Flecs.NET/Generated/SystemBuilder/SystemBuilder.NodeBuilder/T12.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/SystemBuilder.cs
using System;
using Flecs.NET.Core.BindingContext;

using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

public unsafe partial struct SystemBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Run callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Run(Ecs.RunCallback callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetRun(callback, (delegate*<ecs_iter_t*, void>)&Functions.RunCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Run callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Run(delegate*<Iter, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetRun(callback, (delegate*<ecs_iter_t*, void>)&Functions.RunCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(Ecs.IterFieldCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterFieldCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(Ecs.IterSpanCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterSpanCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(Ecs.IterPointerCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterPointerCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(delegate*<Iter, Field<T0>, Field<T1>, Field<T2>, Field<T3>, Field<T4>, Field<T5>, Field<T6>, Field<T7>, Field<T8>, Field<T9>, Field<T10>, Field<T11>, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterFieldCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(delegate*<Iter, Span<T0>, Span<T1>, Span<T2>, Span<T3>, Span<T4>, Span<T5>, Span<T6>, Span<T7>, Span<T8>, Span<T9>, Span<T10>, Span<T11>, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterSpanCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Iter callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Iter(delegate*<Iter, T0*, T1*, T2*, T3*, T4*, T5*, T6*, T7*, T8*, T9*, T10*, T11*, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, false);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.IterPointerCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachRefCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachRefCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachEntityRefCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachEntityRefCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachIterRefCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachIterRefCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, ref T6, ref T7, ref T8, ref T9, ref T10, ref T11, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachRefCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<Entity, ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, ref T6, ref T7, ref T8, ref T9, ref T10, ref T11, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachEntityRefCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<Iter, int, ref T0, ref T1, ref T2, ref T3, ref T4, ref T5, ref T6, ref T7, ref T8, ref T9, ref T10, ref T11, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(true);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachIterRefCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachPointerCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachPointerCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachEntityPointerCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachEntityPointerCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(Ecs.EachIterPointerCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachIterPointerCallbackDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<T0*, T1*, T2*, T3*, T4*, T5*, T6*, T7*, T8*, T9*, T10*, T11*, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachPointerCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<Entity, T0*, T1*, T2*, T3*, T4*, T5*, T6*, T7*, T8*, T9*, T10*, T11*, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachEntityPointerCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }

    /// <summary>
    ///     Creates <see cref="System"/> with the provided .Each callback.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public System<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Each(delegate*<Iter, int, T0*, T1*, T2*, T3*, T4*, T5*, T6*, T7*, T8*, T9*, T10*, T11*, void> callback)
    {
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertReferenceTypes(false);
        TypeHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.AssertSparseTypes(World, true);
        return SetCallback(callback, (delegate*<ecs_iter_t*, void>)&Functions.EachIterPointerCallbackPointer<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).Build();
    }
}