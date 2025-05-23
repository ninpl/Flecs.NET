using System;
using Flecs.NET.Core.BindingContext;
using Flecs.NET.Utilities;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <summary>
///     A wrapper around ecs_app_desc_t.
/// </summary>
public unsafe struct AppBuilder : IEquatable<AppBuilder>
{
    private World _world;
    private ecs_app_desc_t _desc;

    /// <summary>
    ///     Reference to world.
    /// </summary>
    public ref World World => ref _world;

    /// <summary>
    ///     Reference to app description.
    /// </summary>
    public ref ecs_app_desc_t Desc => ref _desc;

    /// <summary>
    ///     Creates an app builder for world.
    /// </summary>
    /// <param name="world"></param>
    public AppBuilder(World world)
    {
        _world = world;
        _desc = default;

        ecs_world_info_t* stats = ecs_get_world_info(world);
        Desc.target_fps = stats->target_fps;

        if (Math.Abs(Desc.target_fps - 0) < 0.01)
            Desc.target_fps = 60;
    }

    /// <summary>
    ///     Sets the target fps.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ref AppBuilder TargetFps(float value)
    {
        Desc.target_fps = value;
        return ref this;
    }

    /// <summary>
    ///     Sets the delta time.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ref AppBuilder DeltaTime(float value)
    {
        Desc.delta_time = value;
        return ref this;
    }

    /// <summary>
    ///     Sets the number of threads to use.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ref AppBuilder Threads(int value)
    {
        Desc.threads = value;
        return ref this;
    }

    /// <summary>
    ///     Sets the number of frames to run.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ref AppBuilder Frames(int value)
    {
        Desc.frames = value;
        return ref this;
    }

    /// <summary>
    ///     Enable ecs access over http for the explorer.
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public ref AppBuilder EnableRest(ushort port = 0)
    {
        Desc.enable_rest = true;
        Desc.port = port;
        return ref this;
    }

    /// <summary>
    ///     Periodically collect statistics.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ref AppBuilder EnableStats(bool value = true)
    {
        Desc.enable_stats = value;
        return ref this;
    }

    /// <summary>
    ///     Sets a callback to be run before starting the main loop.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <returns></returns>
    public ref AppBuilder Init(Ecs.AppInitCallback callback)
    {
        World.WorldContext.AppInit.Set(callback, (delegate*<World, WorldContext*, void>)&Functions.AppInitCallbackDelegate);
        Desc.init = &Functions.AppInitCallback;
        return ref this;
    }

    /// <summary>
    ///     Sets a callback to be run before starting the main loop.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <returns></returns>
    public ref AppBuilder Init(delegate*<World, void> callback)
    {
        World.WorldContext.AppInit.Set(callback, (delegate*<World, WorldContext*, void>)&Functions.AppInitCallbackPointer);
        Desc.init = &Functions.AppInitCallback;
        return ref this;
    }

    /// <summary>
    ///     Runs the app.
    /// </summary>
    /// <returns></returns>
    public int Run()
    {
        fixed (ecs_app_desc_t* appDesc = &Desc)
            return ecs_app_run(World, appDesc);
    }

    /// <summary>
    ///     Checks if two <see cref="AppBuilder"/> instances are equal.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(AppBuilder other)
    {
        return Desc == other.Desc;
    }

    /// <summary>
    ///     Checks if two <see cref="AppBuilder"/> instances are equal.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is AppBuilder other && Equals(other);
    }

    /// <summary>
    ///     Gets the hash code of the <see cref="AppBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Desc.GetHashCode();
    }

    /// <summary>
    ///     Checks if two <see cref="AppBuilder"/> instances are equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(AppBuilder left, AppBuilder right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Checks if two <see cref="AppBuilder"/> instances are not equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(AppBuilder left, AppBuilder right)
    {
        return !(left == right);
    }
}
