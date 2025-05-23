// /_/src/Flecs.NET/Generated/Invoker/Iter/Iterator/T9.g.cs
// File was auto-generated by /_/src/Flecs.NET.Codegen/Generators/Invoker.cs
using System;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

public static unsafe partial class Invoker
{
    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, Ecs.IterFieldCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.Field<T0>(0), it.Field<T1>(1), it.Field<T2>(2), it.Field<T3>(3), it.Field<T4>(4), it.Field<T5>(5), it.Field<T6>(6), it.Field<T7>(7), it.Field<T8>(8));
        Ecs.TableUnlock(it);
    }

    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, Ecs.IterSpanCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.GetSpan<T0>(0), it.GetSpan<T1>(1), it.GetSpan<T2>(2), it.GetSpan<T3>(3), it.GetSpan<T4>(4), it.GetSpan<T5>(5), it.GetSpan<T6>(6), it.GetSpan<T7>(7), it.GetSpan<T8>(8));
        Ecs.TableUnlock(it);
    }

    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, Ecs.IterPointerCallback<T0, T1, T2, T3, T4, T5, T6, T7, T8> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.GetPointer<T0>(0), it.GetPointer<T1>(1), it.GetPointer<T2>(2), it.GetPointer<T3>(3), it.GetPointer<T4>(4), it.GetPointer<T5>(5), it.GetPointer<T6>(6), it.GetPointer<T7>(7), it.GetPointer<T8>(8));
        Ecs.TableUnlock(it);
    }

    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, delegate*<Iter, Field<T0>, Field<T1>, Field<T2>, Field<T3>, Field<T4>, Field<T5>, Field<T6>, Field<T7>, Field<T8>, void> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.Field<T0>(0), it.Field<T1>(1), it.Field<T2>(2), it.Field<T3>(3), it.Field<T4>(4), it.Field<T5>(5), it.Field<T6>(6), it.Field<T7>(7), it.Field<T8>(8));
        Ecs.TableUnlock(it);
    }

    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, delegate*<Iter, Span<T0>, Span<T1>, Span<T2>, Span<T3>, Span<T4>, Span<T5>, Span<T6>, Span<T7>, Span<T8>, void> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.GetSpan<T0>(0), it.GetSpan<T1>(1), it.GetSpan<T2>(2), it.GetSpan<T3>(3), it.GetSpan<T4>(4), it.GetSpan<T5>(5), it.GetSpan<T6>(6), it.GetSpan<T7>(7), it.GetSpan<T8>(8));
        Ecs.TableUnlock(it);
    }

    /// <summary>
    ///     Iterates over an Iter object using the provided .Iter callback.
    /// </summary>
    /// <param name="it">The iter object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T0">The T0 component type.</typeparam> <typeparam name="T1">The T1 component type.</typeparam> <typeparam name="T2">The T2 component type.</typeparam> <typeparam name="T3">The T3 component type.</typeparam> <typeparam name="T4">The T4 component type.</typeparam> <typeparam name="T5">The T5 component type.</typeparam> <typeparam name="T6">The T6 component type.</typeparam> <typeparam name="T7">The T7 component type.</typeparam> <typeparam name="T8">The T8 component type.</typeparam>
    public static void Iter<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Iter it, delegate*<Iter, T0*, T1*, T2*, T3*, T4*, T5*, T6*, T7*, T8*, void> callback)
    {
        Ecs.TableLock(it);
        callback(it, it.GetPointer<T0>(0), it.GetPointer<T1>(1), it.GetPointer<T2>(2), it.GetPointer<T3>(3), it.GetPointer<T4>(4), it.GetPointer<T5>(5), it.GetPointer<T6>(6), it.GetPointer<T7>(7), it.GetPointer<T8>(8));
        Ecs.TableUnlock(it);
    }
}