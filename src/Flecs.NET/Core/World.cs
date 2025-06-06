using System;
using System.Diagnostics.CodeAnalysis;
using Flecs.NET.Core.BindingContext;
using Flecs.NET.Utilities;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core;

/// <summary>
///     The world is the container of all ECS data and systems. If the world is deleted, all data in the world will be
///     deleted as well.
/// </summary>
public readonly unsafe partial struct World : IDisposable, IEquatable<World>
{
    private readonly ecs_world_t* _handle;

    internal ref WorldContext WorldContext => ref *EnsureBindingContext();

    /// <summary>
    ///     The handle to the C world.
    /// </summary>
    public ref readonly ecs_world_t* Handle => ref _handle;

    /// <summary>
    ///     Constructs a world from an <see cref="ecs_world_t"/> pointer.
    /// </summary>
    /// <param name="handle">The world handle.</param>
    public World(ecs_world_t* handle)
    {
        _handle = handle;
    }

    /// <summary>
    ///     Creates a flecs world that is owned.
    /// </summary>
    /// <returns></returns>
    public static World Create()
    {
        Ecs.Os.OverrideOsApi();

        World w = new World(ecs_init());
        w.EnsureBindingContext();
        w.InitBuiltinComponents();

        return w;
    }

    /// <summary>
    ///     Creates a flecs world from an <see cref="ecs_world_t"/> pointer that is not owned.
    /// </summary>
    /// <param name="world">A C world.</param>
    /// <returns>A newly created world.</returns>
    public static World Create(ecs_world_t* world)
    {
        Ecs.Os.OverrideOsApi();

        World w = new World(world);
        w.EnsureBindingContext();
        w.InitBuiltinComponents();

        return w;
    }

    /// <summary>
    ///     Creates world from command line arguments.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns></returns>
    public static World Create(params Span<string> args)
    {
        NativeString* nativeStrings = Memory.AllocZeroed<NativeString>(args.Length);

        for (int i = 0; i < args.Length; i++)
            nativeStrings[i] = (NativeString)args[i];

        Ecs.Os.OverrideOsApi();

        World w = new World(ecs_init_w_args(args.Length, (byte**)nativeStrings));
        w.EnsureBindingContext();
        w.InitBuiltinComponents();

        for (int i = 0; i < args.Length; i++)
            nativeStrings[i].Dispose();

        Memory.Free(nativeStrings);

        return w;
    }

    /// <summary>
    ///     Calls <see cref="ecs_fini"/> and cleans up resources.
    /// </summary>
    public void Dispose()
    {
        if (Handle == null || !Ecs.IsStageOrWorld(Handle))
            return;

        _ = ecs_fini(Handle);
    }

    /// <summary>
    ///     Signals that the application should quit. The next call to <see cref="Progress"/> returns false.
    /// </summary>
    public void Quit()
    {
        ecs_quit(Handle);
    }

    /// <summary>
    ///     Register action to be executed when world is destroyed.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void AtFini(Ecs.WorldFinishCallback callback)
    {
        WorldFinishContext* context = AllocateWorldFinishContext();
        context->Callback.Set(callback, (delegate*<ecs_world_t*, void*, void>)&Functions.WorldFinishCallbackDelegate);
        ecs_atfini(Handle, &Functions.WorldFinishCallback, context);
    }

    /// <summary>
    ///     Register action to be executed when world is destroyed.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void AtFini(delegate*<World, void> callback)
    {
        WorldFinishContext* context = AllocateWorldFinishContext();
        context->Callback.Set(callback, (delegate*<ecs_world_t*, void*, void>)&Functions.WorldFinishCallbackPointer);
        ecs_atfini(Handle, &Functions.WorldFinishCallback, context);
    }

    /// <summary>
    ///     Test if Quit() has been called.
    /// </summary>
    /// <returns></returns>
    public bool ShouldQuit()
    {
        return ecs_should_quit(Handle);
    }

    /// <summary>
    ///     Begin frame.
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public float FrameBegin(float deltaTime = 0)
    {
        return ecs_frame_begin(Handle, deltaTime);
    }

    /// <summary>
    ///     End frame.
    /// </summary>
    public void FrameEnd()
    {
        ecs_frame_end(Handle);
    }

    /// <summary>
    ///     Begin staging.
    /// </summary>
    /// <returns></returns>
    public bool ReadonlyBegin(bool multiThreaded = false)
    {
        return ecs_readonly_begin(Handle, multiThreaded);
    }

    /// <summary>
    ///     End staging.
    /// </summary>
    public void ReadonlyEnd()
    {
        ecs_readonly_end(Handle);
    }

    /// <summary>
    ///     Defer operations until end end of frame, or until DeferEnd() is called.
    /// </summary>
    /// <returns></returns>
    public bool DeferBegin()
    {
        return ecs_defer_begin(Handle);
    }

    /// <summary>
    ///     End block of operations to defer.
    /// </summary>
    /// <returns></returns>
    public bool DeferEnd()
    {
        return ecs_defer_end(Handle);
    }

    /// <summary>
    ///     Test whether deferring is enabled.
    /// </summary>
    /// <returns></returns>
    public bool IsDeferred()
    {
        return ecs_is_deferred(Handle);
    }

    /// <summary>
    ///     Configure world to have N stages.
    /// </summary>
    /// <param name="stages"></param>
    public void SetStageCount(int stages)
    {
        ecs_set_stage_count(Handle, stages);
    }

    /// <summary>
    ///     Get number of configured stages.
    /// </summary>
    /// <returns></returns>
    public int GetStageCount()
    {
        return ecs_get_stage_count(Handle);
    }

    /// <summary>
    ///     Get current stage id.
    /// </summary>
    /// <returns></returns>
    public int GetStageId()
    {
        return ecs_stage_get_id(Handle);
    }

    /// <summary>
    ///     Test if is a stage.
    /// </summary>
    /// <returns></returns>
    public bool IsStage()
    {
        Ecs.Assert(
            flecs_poly_is_(Handle, ecs_world_t_magic) ||
            flecs_poly_is_(Handle, ecs_stage_t_magic),
            nameof(ECS_INVALID_PARAMETER)
        );
        return flecs_poly_is_(Handle, ecs_stage_t_magic);
    }

    /// <summary>
    ///     Merge world or stage.
    /// </summary>
    public void Merge()
    {
        ecs_merge(Handle);
    }

    /// <summary>
    ///     Get stage-specific world pointer.
    /// </summary>
    /// <param name="stageId"></param>
    /// <returns></returns>
    public World GetStage(int stageId)
    {
        return new World(ecs_get_stage(Handle, stageId));
    }

    /// <summary>
    ///     Creates asynchronous stage.
    /// </summary>
    /// <returns></returns>
    public World AsyncStage()
    {
        return new World(ecs_stage_new(Handle));
    }

    /// <summary>
    ///     Get actual world.
    /// </summary>
    /// <returns></returns>
    public World GetWorld()
    {
        return new World(Handle == null ? null : ecs_get_world(Handle));
    }

    /// <summary>
    ///     Test whether the current world object is readonly.
    /// </summary>
    /// <returns></returns>
    public bool IsReadOnly()
    {
        return ecs_stage_is_readonly(Handle);
    }

    /// <summary>
    ///     Sets the user context object for the world.
    /// </summary>
    /// <param name="value">The user context object.</param>
    /// <typeparam name="T">The user context type.</typeparam>
    public void SetCtx<T>(T value)
    {
        WorldContext.UserContext.Set(ref value);
    }

    /// <summary>
    ///     Sets the user context object for the world. The provided callback will be run before the
    ///     user context object is released by flecs.
    /// </summary>
    /// <param name="value">The user context object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The user context type.</typeparam>
    public void SetCtx<T>(T value, Ecs.UserContextFinish<T> callback)
    {
        WorldContext.UserContext.Set(ref value, callback);
    }

    /// <summary>
    ///     Sets the user context object for the world. The provided callback will be run before the
    ///     user context object is released by flecs.
    /// </summary>
    /// <param name="value">The user context object.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The user context type.</typeparam>
    public void SetCtx<T>(T value, delegate*<ref T, void> callback)
    {
        WorldContext.UserContext.Set(ref value, callback);
    }

    /// <summary>
    ///     Gets the user context object for the world.
    /// </summary>
    /// <typeparam name="T">The user context type.</typeparam>
    /// <returns></returns>
    public ref T GetCtx<T>()
    {
        return ref WorldContext.UserContext.Get<T>();
    }

    /// <summary>
    ///     Preallocate memory for number of entities.
    /// </summary>
    /// <param name="entityCount"></param>
    public void Dim(int entityCount)
    {
        ecs_dim(Handle, entityCount);
    }

    /// <summary>
    ///     Set entity range.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void SetEntityRange(ulong min, ulong max)
    {
        ecs_set_entity_range(Handle, min, max);
    }

    /// <summary>
    ///     Enforce that operations cannot modify entities outside of range.
    /// </summary>
    /// <param name="enabled"></param>
    public void EnableRangeCheck(bool enabled = true)
    {
        ecs_enable_range_check(Handle, enabled);
    }

    /// <summary>
    ///     Set the current scope.
    /// </summary>
    /// <param name="scope">The entity to use as scope.</param>
    /// <returns>The previous scope.</returns>
    public Entity SetScope(ulong scope)
    {
        return new Entity(Handle, ecs_set_scope(Handle, scope));
    }

    /// <summary>
    ///     Set the current scope.
    /// </summary>
    /// <typeparam name="T">The entity to use as scope.</typeparam>
    /// <returns>The previous scope.</returns>
    public Entity SetScope<T>()
    {
        return SetScope(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Set the current scope.
    /// </summary>
    /// <param name="value">The entity to use as scope.</param>
    /// <typeparam name="T">The enum.</typeparam>
    /// <returns>The previous scope.</returns>
    public Entity SetScope<T>(T value) where T : Enum
    {
        return SetScope(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Get the current scope.
    /// </summary>
    /// <returns>The current scope.</returns>
    public Entity GetScope()
    {
        return new Entity(Handle, ecs_get_scope(Handle));
    }

    /// <summary>
    ///     Set search path.
    /// </summary>
    /// <param name="searchPath"></param>
    /// <returns></returns>
    public ulong* SetLookupPath(ulong* searchPath)
    {
        return ecs_set_lookup_path(Handle, searchPath);
    }

    /// <summary>
    ///     Lookup entity by name.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="rootSeparator">The root separator.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <returns>The entity if found, else 0.</returns>
    public Entity Lookup(
        string path,
        string separator = Ecs.DefaultSeparator,
        string rootSeparator = Ecs.DefaultSeparator,
        bool recursive = true)
    {
        if (string.IsNullOrEmpty(path))
            return new Entity(Handle, 0);

        using NativeString nativePath = (NativeString)path;
        using NativeString nativeSeparator = (NativeString)separator;
        using NativeString nativeRootSeparator = (NativeString)rootSeparator;

        return new Entity(
            Handle,
            ecs_lookup_path_w_sep(Handle, 0, nativePath, nativeSeparator, nativeRootSeparator, recursive)
        );
    }

    /// <summary>
    ///     Sets the data of a component.
    /// </summary>
    /// <param name="id">The id of the component to set.</param>
    /// <param name="size">The size of the pointed-to data.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <returns>Reference to self.</returns>
    public World SetUntyped(ulong id, int size, void* data)
    {
        Entity(id).SetUntyped(id, size, data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a component.
    /// </summary>
    /// <param name="id">The id of the component to set.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <returns>Reference to self.</returns>
    public World SetUntyped(ulong id, void* data)
    {
        Entity(id).SetUntyped(id, data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a pair component.
    /// </summary>
    /// <param name="first">The first id of the pair.</param>
    /// <param name="second">The second id of the pair.</param>
    /// <param name="size">The size of the pointed-to data.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <returns>Reference to self.</returns>
    public World SetUntyped(ulong first, ulong second, int size, void* data)
    {
        Entity(first).SetUntyped(first, second, size, data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton component.
    /// </summary>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<T>(T* data)
    {
        Entity<T>().SetPtr(data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="second">The second id of the pair.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<TFirst>(ulong second, TFirst* data)
    {
        Entity<TFirst>().SetPtr(second, data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<TFirst, TSecond>(TFirst* data)
    {
        Entity<TFirst>().SetPtr<TFirst, TSecond>(data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<TFirst, TSecond>(TSecond* data)
    {
        Entity<TFirst>().SetPtr<TFirst, TSecond>(data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="second">The second id (enum member) of the pair.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<TFirst, TSecond>(TSecond second, TFirst* data) where TSecond : Enum
    {
        return SetPtr(Type<TSecond>.Id(Handle, second), data);
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="first">The first id (enum member) of the pair.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtr<TFirst, TSecond>(TFirst first, TSecond* data) where TFirst : Enum
    {
        return SetPtrSecond(Type<TFirst>.Id(Handle, first), data);
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="first">The first id of the pair.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetPtrSecond<TSecond>(ulong first, TSecond* data)
    {
        Entity(first).SetPtrSecond(first, data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton component.
    /// </summary>
    /// <param name="data">The component data.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<T>(in T data)
    {
        Entity<T>().Set(in data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="second">The second id of the pair.</param>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<TFirst>(ulong second, in TFirst data)
    {
        Entity<TFirst>().Set(second, in data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<TFirst, TSecond>(in TFirst data)
    {
        Entity<TFirst>().Set<TFirst, TSecond>(in data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<TFirst, TSecond>(in TSecond data)
    {
        Entity<TFirst>().Set<TFirst, TSecond>(in data);
        return this;
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="second">The second id (enum member) of the pair.</param>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<TFirst, TSecond>(TSecond second, in TFirst data) where TSecond : Enum
    {
        return Set(Type<TSecond>.Id(Handle, second), in data);
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="first">The first id (enum member) of the pair.</param>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TFirst">The first type of the pair.</typeparam>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World Set<TFirst, TSecond>(TFirst first, in TSecond data) where TFirst : Enum
    {
        return SetSecond(Type<TFirst>.Id(Handle, first), in data);
    }

    /// <summary>
    ///     Sets the data of a singleton pair component.
    /// </summary>
    /// <param name="first">The first id of the pair.</param>
    /// <param name="data">The component data.</param>
    /// <typeparam name="TSecond">The second type of the pair.</typeparam>
    /// <returns>Reference to self.</returns>
    public World SetSecond<TSecond>(ulong first, in TSecond data)
    {
        Entity(first).SetSecond(first, in data);
        return this;
    }

    /// <summary>
    ///     Ensure singleton component inside a callback.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void Insert<T>(Ecs.InsertRefCallback<T> callback)
    {
        Invoker.Insert(Handle, Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     Get mutable pointer to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T* EnsurePtr<T>() where T : unmanaged
    {
        return Entity<T>().EnsurePtr<T>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public TFirst* EnsurePtr<TFirst>(ulong second) where TFirst : unmanaged
    {
        return Entity<TFirst>().EnsurePtr<TFirst>(second);
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* EnsurePtr<TFirst, TSecond>(TSecond second)
        where TFirst : unmanaged
        where TSecond : Enum
    {
        return EnsurePtr<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* EnsurePtr<TFirst, TSecond>(TFirst first)
        where TFirst : Enum
        where TSecond : unmanaged
    {
        return EnsureSecondPtr<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* EnsureFirstPtr<TFirst, TSecond>() where TFirst : unmanaged
    {
        return Entity<TFirst>().EnsureFirstPtr<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* EnsureSecondPtr<TFirst, TSecond>() where TSecond : unmanaged
    {
        return Entity<TFirst>().EnsureSecondPtr<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* EnsureSecondPtr<TSecond>(ulong first) where TSecond : unmanaged
    {
        return Entity(first).EnsureSecondPtr<TSecond>(first);
    }

    /// <summary>
    ///     Get managed mutable reference to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ref T Ensure<T>()
    {
        return ref Entity<T>().Ensure<T>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public ref TFirst Ensure<TFirst>(ulong second)
    {
        return ref Entity<TFirst>().Ensure<TFirst>(second);
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TFirst Ensure<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return ref Ensure<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond Ensure<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return ref EnsureSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TFirst EnsureFirst<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().EnsureFirst<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond EnsureSecond<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().EnsureSecond<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond EnsureSecond<TSecond>(ulong first)
    {
        return ref Entity(first).EnsureSecond<TSecond>(first);
    }

    /// <summary>
    ///     Mark singleton component as modified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Modified<T>()
    {
        Entity<T>().Modified<T>();
    }

    /// <summary>
    ///     Mark singleton pair as modified.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void Modified<TFirst>(ulong second)
    {
        Entity<TFirst>().Modified<TFirst>(second);
    }

    /// <summary>
    ///     Mark singleton pair as modified.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Modified<TFirst, TSecond>()
    {
        Entity<TFirst>().Modified<TFirst, TSecond>();
    }

    /// <summary>
    ///     Mark singleton pair as modified.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Modified<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        Modified<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Mark singleton pair as modified.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Modified<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        ModifiedSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Mark singleton pair as modified.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void ModifiedSecond<TSecond>(ulong first)
    {
        Entity(first).ModifiedSecond<TSecond>(first);
    }

    /// <summary>
    ///     Gets ref to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Ref<T> GetRef<T>()
    {
        return Entity<T>().GetRef<T>();
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public Ref<TFirst> GetRef<TFirst>(ulong second)
    {
        return Entity<TFirst>().GetRef<TFirst>(second);
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Ref<TFirst> GetRef<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return GetRef<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Ref<TSecond> GetRef<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return GetRefSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Ref<TFirst> GetRefFirst<TFirst, TSecond>()
    {
        return Entity<TFirst>().GetRefFirst<TFirst, TSecond>();
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Ref<TSecond> GetRefSecond<TFirst, TSecond>()
    {
        return Entity<TFirst>().GetRefSecond<TFirst, TSecond>();
    }

    /// <summary>
    ///     Gets ref to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Ref<TSecond> GetRefSecond<TSecond>(ulong first)
    {
        return Entity(first).GetRefSecond<TSecond>(first);
    }

    /// <summary>
    ///     Gets pointer to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T* GetPtr<T>() where T : unmanaged
    {
        return Entity<T>().GetPtr<T>();
    }

    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public TFirst* GetPtr<TFirst>(ulong second) where TFirst : unmanaged
    {
        return Entity<TFirst>().GetPtr<TFirst>(second);
    }

    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* GetPtr<TFirst, TSecond>(TSecond second)
        where TFirst : unmanaged
        where TSecond : Enum
    {
        return GetPtr<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetPtr<TFirst, TSecond>(TFirst first)
        where TFirst : Enum
        where TSecond : unmanaged
    {
        return GetSecondPtr<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* GetFirstPtr<TFirst, TSecond>() where TFirst : unmanaged
    {
        return Entity<TFirst>().GetFirstPtr<TFirst, TSecond>();
    }


    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetSecondPtr<TFirst, TSecond>() where TSecond : unmanaged
    {
        return Entity<TFirst>().GetSecondPtr<TFirst, TSecond>();
    }

    /// <summary>
    ///     Gets pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetSecondPtr<TSecond>(ulong first) where TSecond : unmanaged
    {
        return Entity(first).GetSecondPtr<TSecond>(first);
    }

    /// <summary>
    ///     Gets managed reference to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ref readonly T Get<T>()
    {
        return ref Entity<T>().Get<T>();
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public ref readonly TFirst Get<TFirst>(ulong second)
    {
        return ref Entity<TFirst>().Get<TFirst>(second);
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref readonly TFirst Get<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return ref Get<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref readonly TSecond Get<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return ref GetSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref readonly TFirst GetFirst<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().GetFirst<TFirst, TSecond>();
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref readonly TSecond GetSecond<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().GetSecond<TFirst, TSecond>();
    }

    /// <summary>
    ///     Gets managed reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref readonly TSecond GetSecond<TSecond>(ulong first)
    {
        return ref Entity(first).GetSecond<TSecond>(first);
    }

    /// <summary>
    ///     Get mutable pointer to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T* GetMutPtr<T>() where T : unmanaged
    {
        return Entity<T>().GetMutPtr<T>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public TFirst* GetMutPtr<TFirst>(ulong second) where TFirst : unmanaged
    {
        return Entity<TFirst>().GetMutPtr<TFirst>(second);
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* GetMutPtr<TFirst, TSecond>(TSecond second)
        where TFirst : unmanaged
        where TSecond : Enum
    {
        return GetMutPtr<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetMutPtr<TFirst, TSecond>(TFirst first)
        where TFirst : Enum
        where TSecond : unmanaged
    {
        return GetMutSecondPtr<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TFirst* GetMutFirstPtr<TFirst, TSecond>() where TFirst : unmanaged
    {
        return Entity<TFirst>().GetMutFirstPtr<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetMutSecondPtr<TFirst, TSecond>() where TSecond : unmanaged
    {
        return Entity<TFirst>().GetMutSecondPtr<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get mutable pointer to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public TSecond* GetMutSecondPtr<TSecond>(ulong first) where TSecond : unmanaged
    {
        return Entity(first).GetMutSecondPtr<TSecond>(first);
    }

    /// <summary>
    ///     Get managed mutable reference to singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ref T GetMut<T>()
    {
        return ref Entity<T>().GetMut<T>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public ref TFirst GetMut<TFirst>(ulong second)
    {
        return ref Entity<TFirst>().GetMut<TFirst>(second);
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TFirst GetMut<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return ref GetMut<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond GetMut<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return ref GetMutSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TFirst GetMutFirst<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().GetMutFirst<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond GetMutSecond<TFirst, TSecond>()
    {
        return ref Entity<TFirst>().GetMutSecond<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get managed mutable reference to singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public ref TSecond GetMutSecond<TSecond>(ulong first)
    {
        return ref Entity(first).GetMutSecond<TSecond>(first);
    }

    /// <summary>
    ///     Get a readonly singleton component inside a callback.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public void Read<T>(Ecs.ReadRefCallback<T> callback)
    {
        Invoker.Read(Handle, Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     Get a mutable singleton component inside a callback.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public void Write<T>(Ecs.WriteRefCallback<T> callback)
    {
        Invoker.Write(Handle, Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///      Test if world has singleton component.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Has(ulong id)
    {
        return Entity(id).Has(id);
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public bool Has(ulong first, ulong second)
    {
        return Entity(first).Has(first, second);
    }

    /// <summary>
    ///     Test if world has singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Has<T>()
    {
        return Entity<T>().Has<T>();
    }

    /// <summary>
    ///     Test if world has singleton component.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Has<T>(T value) where T : Enum
    {
        return Entity<T>().Has(value);
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public bool Has<TFirst>(ulong second)
    {
        return Entity<TFirst>().Has<TFirst>(second);
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public bool Has<TFirst, TSecond>()
    {
        return Entity<TFirst>().Has<TFirst, TSecond>();
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public bool Has<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return Has<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public bool Has<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return HasSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Test if world has singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public bool HasSecond<TSecond>(ulong first)
    {
        return Entity(first).Has(first, Type<TSecond>.Id(Handle));
    }

    /// <summary>
    ///     Add singleton component.
    /// </summary>
    /// <param name="id"></param>
    public void Add(ulong id)
    {
        Entity(id).Add(id);
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Add(ulong first, ulong second)
    {
        Entity(first).Add(first, second);
    }

    /// <summary>
    ///     Add singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Add<T>()
    {
        Entity<T>().Add<T>();
    }

    /// <summary>
    ///     Add singleton component.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void Add<T>(T value) where T : Enum
    {
        Entity<T>().Add(value);
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void Add<TFirst>(ulong second)
    {
        Entity<TFirst>().Add<TFirst>(second);
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Add<TFirst, TSecond>()
    {
        Entity<TFirst>().Add<TFirst, TSecond>();
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Add<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        Add<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Add<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        AddSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Add singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void AddSecond<TSecond>(ulong first)
    {
        Entity(first).AddSecond<TSecond>(first);
    }

    /// <summary>
    ///     Remove singleton id.
    /// </summary>
    /// <param name="id"></param>
    public void Remove(ulong id)
    {
        Entity(id).Remove(id);
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Remove(ulong first, ulong second)
    {
        Entity(first).Remove(first, second);
    }

    /// <summary>
    ///     Remove singleton component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Remove<T>()
    {
        Entity<T>().Remove<T>();
    }

    /// <summary>
    ///     Remove singleton id.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void Remove<T>(T value) where T : Enum
    {
        Entity<T>().Remove(value);
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void Remove<TFirst>(ulong second)
    {
        Entity<TFirst>().Remove<TFirst>(second);
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Remove<TFirst, TSecond>()
    {
        Entity<TFirst>().Remove<TFirst, TSecond>();
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Remove<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        Remove<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Remove<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        RemoveSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Remove singleton pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void RemoveSecond<TSecond>(ulong first)
    {
        Entity(first).Remove<TSecond>(first);
    }

    /// <summary>
    ///     Iterate entities in root of world
    /// </summary>
    /// <param name="func"></param>
    public void Children(Ecs.EachEntityCallback func)
    {
        Entity(0).Children(func);
    }

    /// <summary>
    ///     Get singleton entity for type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Singleton<T>()
    {
        return Entity<T>();
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Entity Target(ulong id, int index = 0)
    {
        return Entity(ecs_get_target(Handle, id, id, index));
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="rel"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Entity Target(ulong id, ulong rel, int index = 0)
    {
        return Entity(ecs_get_target(Handle, id, rel, index));
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Target<T>(int index = 0)
    {
        return Entity(ecs_get_target(Handle, Type<T>.Id(Handle), Type<T>.Id(Handle), index));
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="rel"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Target<T>(ulong rel, int index = 0)
    {
        return Entity(ecs_get_target(Handle, Type<T>.Id(Handle), rel, index));
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Target<T>(T value, int index = 0) where T : Enum
    {
        return Entity(ecs_get_target(Handle, Entity(value), Entity(value), index));
    }

    /// <summary>
    ///     Get target for a given pair from a singleton entity.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rel"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Target<T>(T value, ulong rel, int index = 0) where T : Enum
    {
        return Entity(ecs_get_target(Handle, Entity(value), rel, index));
    }

    /// <summary>
    ///     Creates alias for component.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="alias"></param>
    public Entity Use(ulong entity, string alias = "")
    {
        if (string.IsNullOrEmpty(alias))
            alias = NativeString.GetString(ecs_get_name(Handle, entity));

        using NativeString nativeAlias = (NativeString)alias;
        ecs_set_alias(Handle, entity, nativeAlias);
        return Entity(entity);
    }

    /// <summary>
    ///     Creates alias for component.
    /// </summary>
    /// <param name="alias"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Use<T>(string alias = "")
    {
        ulong entity = Type<T>.Id(Handle);

        if (string.IsNullOrEmpty(alias))
            alias = NativeString.GetString(ecs_get_name(Handle, entity));

        using NativeString nativeAlias = (NativeString)alias;

        ecs_set_alias(Handle, entity, nativeAlias);
        return Entity(entity);
    }

    /// <summary>
    ///     Creates alias for component.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="alias"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Use<T>(T value, string alias = "") where T : Enum
    {
        return Use(Type<T>.Id(Handle, value), alias);
    }

    /// <summary>
    ///     Creates alias for component.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public Entity Use(string name, string alias = "")
    {
        using NativeString nativeName = (NativeString)name;

        ulong entity = ecs_lookup_path_w_sep(Handle, 0, nativeName,
            Pointers.DefaultSeparator, Pointers.DefaultSeparator, true);

        Ecs.Assert(entity != 0, nameof(ECS_INVALID_PARAMETER));

        using NativeString nativeAlias = (NativeString)alias;

        ecs_set_alias(Handle, entity, nativeAlias);
        return Entity(entity);
    }

    /// <summary>
    ///     Count entities matching a component.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int Count(ulong id)
    {
        return ecs_count_id(Handle, id);
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public int Count(ulong first, ulong second)
    {
        return Count(Ecs.Pair(first, second));
    }

    /// <summary>
    ///     Count entities matching a component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public int Count<T>()
    {
        return Count(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Count entities matching a component.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public int Count<T>(T value) where T : Enum
    {
        return Count<T>(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public int Count<TFirst>(ulong second)
    {
        return Count(Ecs.Pair<TFirst>(second, Handle));
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public int Count<TFirst, TSecond>()
    {
        return Count(Ecs.Pair<TFirst, TSecond>(Handle));
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public int Count<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return Count<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public int Count<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return CountSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Count entities matching a pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public int CountSecond<TSecond>(ulong first)
    {
        return Count(first, Type<TSecond>.Id(Handle));
    }

    /// <summary>
    ///     All entities created in function are created with id.
    /// </summary>
    /// <param name="id">Id to be added to the created entities.</param>
    /// <param name="callback">The callback.</param>
    public void With(ulong id, Action callback)
    {
        ulong prev = ecs_set_with(Handle, id);
        callback();
        ecs_set_with(Handle, prev);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first id.</param>
    /// <param name="second">The second id.</param>
    /// <param name="callback">The callback.</param>
    public void With(ulong first, ulong second, Action callback)
    {
        With(Ecs.Pair(first, second), callback);
    }

    /// <summary>
    ///     All entities created in function are created with type.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The component.</typeparam>
    public void With<T>(Action callback)
    {
        With(Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with enum.
    /// </summary>
    /// <param name="value">The enum constant.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The enum.</typeparam>
    public void With<T>(T value, Action callback) where T : Enum
    {
        With<T>(Type<T>.Id(Handle, value), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="second">The second id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    public void With<TFirst>(ulong second, Action callback)
    {
        With(Ecs.Pair<TFirst>(second, Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void With<TFirst, TSecond>(Action callback)
    {
        With(Ecs.Pair<TFirst, TSecond>(Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="second">The second enum constant id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    /// <typeparam name="TSecond">The second enum.</typeparam>
    public void With<TFirst, TSecond>(TSecond second, Action callback) where TSecond : Enum
    {
        With<TFirst>(Type<TSecond>.Id(Handle, second), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first enum constant.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first enum.</typeparam>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void With<TFirst, TSecond>(TFirst first, Action callback) where TFirst : Enum
    {
        WithSecond<TSecond>(Type<TFirst>.Id(Handle, first), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void WithSecond<TSecond>(ulong first, Action callback)
    {
        With(first, Type<TSecond>.Id(Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created in scope. All operations
    ///     called in function (such as lookup) are relative to scope.
    /// </summary>
    /// <param name="parent">The entity to use as scope.</param>
    /// <param name="callback">The callback.</param>
    public void Scope(ulong parent, Action callback)
    {
        ulong prev = ecs_set_scope(Handle, parent);
        callback();
        ecs_set_scope(Handle, prev);
    }

    /// <summary>
    ///     Same as Scope(parent, callback), but with T as parent.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The entity to use as scope.</typeparam>
    public void Scope<T>(Action callback)
    {
        Scope(Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     Same as Scope(parent, callback), but with enum as parent.
    /// </summary>
    /// <param name="value">The entity to use as scope.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The enum.</typeparam>
    public void Scope<T>(T value, Action callback) where T : Enum
    {
        Scope(Type<T>.Id(Handle, value), callback);
    }

    /// <summary>
    ///     Use provided scope for operations ran on returned world.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public ScopedWorld Scope(ulong parent)
    {
        return new ScopedWorld(Handle, parent);
    }

    /// <summary>
    ///     Use provided scope for operations ran on returned world.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ScopedWorld Scope<T>()
    {
        return Scope(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Use provided scope for operations ran on returned world.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ScopedWorld Scope<T>(T value) where T : Enum
    {
        return Scope(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Use provided scope for operations ran on returned world.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ScopedWorld Scope(string name)
    {
        return Scope(Entity(name));
    }

    /// <summary>
    ///     Delete all entities with specified id.
    /// </summary>
    /// <param name="id"></param>
    public void DeleteWith(ulong id)
    {
        ecs_delete_with(Handle, id);
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void DeleteWith(ulong first, ulong second)
    {
        DeleteWith(Ecs.Pair(first, second));
    }

    /// <summary>
    ///     Delete all entities with specified component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void DeleteWith<T>()
    {
        DeleteWith(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Delete all entities with specified enum.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void DeleteWith<T>(T value) where T : Enum
    {
        DeleteWith<T>(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void DeleteWith<TFirst>(ulong second)
    {
        DeleteWith(Ecs.Pair<TFirst>(second, Handle));
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void DeleteWith<TFirst, TSecond>()
    {
        DeleteWith(Ecs.Pair<TFirst, TSecond>(Handle));
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void DeleteWith<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        DeleteWith<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void DeleteWith<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        DeleteWithSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Delete all entities with specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void DeleteWithSecond<TSecond>(ulong first)
    {
        DeleteWith(Ecs.PairSecond<TSecond>(first, Handle));
    }

    /// <summary>
    ///     Remove all instances of specified id.
    /// </summary>
    /// <param name="id"></param>
    public void RemoveAll(ulong id)
    {
        ecs_remove_all(Handle, id);
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void RemoveAll(ulong first, ulong second)
    {
        RemoveAll(Ecs.Pair(first, second));
    }

    /// <summary>
    ///     Remove all instances of specified component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemoveAll<T>()
    {
        RemoveAll(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Remove all instances of specified enum.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void RemoveAll<T>(T value) where T : Enum
    {
        RemoveAll<T>(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void RemoveAll<TFirst>(ulong second)
    {
        RemoveAll(Ecs.Pair<TFirst>(second, Handle));
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void RemoveAll<TFirst, TSecond>()
    {
        RemoveAll(Ecs.Pair<TFirst, TSecond>(Handle));
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void RemoveAll<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        RemoveAll<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void RemoveAll<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        RemoveAllSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Remove all instances of specified pair.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void RemoveAllSecond<TSecond>(ulong first)
    {
        RemoveAll(Ecs.PairSecond<TSecond>(first, Handle));
    }

    /// <summary>
    ///     Defer all operations called in function. If the world is already in
    ///     deferred mode, do nothing.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void Defer(Action callback)
    {
        ecs_defer_begin(Handle);
        callback();
        ecs_defer_end(Handle);
    }

    /// <summary>
    ///     Suspend deferring operations.
    /// </summary>
    public void DeferSuspend()
    {
        ecs_defer_suspend(Handle);
    }

    /// <summary>
    ///     Resume deferring operations.
    /// </summary>
    public void DeferResume()
    {
        ecs_defer_resume(Handle);
    }

    /// <summary>
    ///     Check if entity id exists in the world.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool Exists(ulong entity)
    {
        return ecs_exists(Handle, entity);
    }

    /// <summary>
    ///     Check if entity id exists in the world.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsAlive(ulong entity)
    {
        return ecs_is_alive(Handle, entity);
    }

    /// <summary>
    ///     Check if entity id is valid.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsValid(ulong entity)
    {
        return ecs_is_valid(Handle, entity);
    }

    /// <summary>
    ///     Get alive entity for id.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Entity GetAlive(ulong entity)
    {
        return Entity(ecs_get_alive(Handle, entity));
    }

    /// <summary>
    ///     Ensures that entity with provided generation is alive.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Entity MakeAlive(ulong entity)
    {
        ecs_make_alive(Handle, entity);
        return Entity(entity);
    }

    /// <summary>
    ///     Override the generation of an entity.
    /// </summary>
    /// <param name="entity">The entity for which to set the generation with the new generation.</param>
    public void SetVersion(ulong entity)
    {
        ecs_set_version(Handle, entity);
    }

    /// <summary>
    ///     Run callback after completing frame.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void RunPostFrame(Ecs.PostFrameCallback callback)
    {
        PostFrameContext* postFrameContext = AllocatePostFrameContext();
        postFrameContext->Callback.Set(callback, (delegate*<ecs_world_t*, void*, void>)&Functions.PostFrameCallbackDelegate);
        ecs_run_post_frame(Handle, &Functions.PostFrameCallback, postFrameContext);
    }

    /// <summary>
    ///     Run callback after completing frame.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void RunPostFrame(delegate*<World, void> callback)
    {
        PostFrameContext* postFrameContext = AllocatePostFrameContext();
        postFrameContext->Callback.Set(callback, (delegate*<ecs_world_t*, void*, void>)&Functions.PostFrameCallbackPointer);
        ecs_run_post_frame(Handle, &Functions.PostFrameCallback, postFrameContext);
    }

    /// <summary>
    ///     Get the world info.
    /// </summary>
    /// <returns></returns>
    public WorldInfo GetInfo()
    {
        return new WorldInfo(ecs_get_world_info(Handle));
    }

    /// <summary>
    ///     Get delta time.
    /// </summary>
    /// <returns></returns>
    public float DeltaTime()
    {
        return ecs_get_world_info(Handle)->delta_time;
    }

    /// <summary>
    ///     Get default id.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Usage", "CA1822")]
    public Id Id()
    {
        return default;
    }

    /// <summary>
    ///     Get id from string expression.
    /// </summary>
    /// <param name="expr">The string expression.</param>
    /// <returns></returns>
    public Id Id(string expr)
    {
        return new Id(Handle, expr);
    }

    /// <summary>
    ///     Get id from id value.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Id Id(ulong id)
    {
        return new Id(Handle, id);
    }

    /// <summary>
    ///     Get pair from id values.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public Id Id(ulong first, ulong second)
    {
        return new Id(Handle, first, second);
    }

    /// <summary>
    ///     Get id from a type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Id Id<T>()
    {
        return new Id(Handle, Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Get id from an enum.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Id Id<T>(T value) where T : Enum
    {
        return new Id(Handle, Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public Id Id<TFirst>(ulong second)
    {
        return Id(Type<TFirst>.Id(Handle), second);
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Id<TFirst, TSecond>()
    {
        return Id(Type<TFirst>.Id(Handle), Type<TSecond>.Id(Handle));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Id<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return Id<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Id<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return IdSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id IdSecond<TSecond>(ulong first)
    {
        return Id(first, Type<TSecond>.Id(Handle));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public Id Pair(ulong first, ulong second)
    {
        return Id(first, second);
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Id Pair<T>(T value) where T : Enum
    {
        return Id<T>(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public Id Pair<TFirst>(ulong second)
    {
        return Id<TFirst>(second);
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Pair<TFirst, TSecond>()
    {
        return Id<TFirst, TSecond>();
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Pair<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return Pair<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id Pair<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return PairSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Get pair id.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Id PairSecond<TSecond>(ulong first)
    {
        return IdSecond<TSecond>(first);
    }

    /// <summary>
    ///     Creates an <see cref="UntypedComponent"/>.
    /// </summary>
    /// <returns></returns>
    public UntypedComponent Component()
    {
        return new UntypedComponent(Handle);
    }

    /// <summary>
    ///     Creates an <see cref="UntypedComponent"/> from id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UntypedComponent Component(ulong id)
    {
        return new UntypedComponent(Handle, id);
    }

    /// <summary>
    ///     Creates an <see cref="UntypedComponent"/> from name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UntypedComponent Component(string name)
    {
        return new UntypedComponent(Handle, name);
    }

    /// <summary>
    ///     Find or register component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Component<T> Component<T>()
    {
        return new Component<T>(Handle);
    }

    /// <summary>
    ///     Get component associated with name.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Component<T> Component<T>(string name)
    {
        return new Component<T>(Handle, name);
    }

    /// <summary>
    ///     Get component with associated with id.
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Component<T> Component<T>(ulong id)
    {
        return new Component<T>(Handle, id);
    }

    /// <summary>
    ///     Creates an entity.
    /// </summary>
    /// <returns></returns>
    public Entity Entity()
    {
        return new Entity(Handle);
    }

    /// <summary>
    ///     Creates an entity from id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Entity Entity(ulong id)
    {
        return new Entity(Handle, id);
    }

    /// <summary>
    ///     Creates an entity from name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Entity Entity(string name)
    {
        return new Entity(Handle, name);
    }

    /// <summary>
    ///     Creates an entity from type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Entity<T>()
    {
        return new Entity(Handle, Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Creates an entity that's associated with a type.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Entity<T>(string name)
    {
        return new Entity(Handle, Type<T>.Id(Handle, false, false, 0, name));
    }

    /// <summary>
    ///     Convert enum constant to entity.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Entity<T>(T value) where T : Enum
    {
        return new Entity(Handle, Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Creates a prefab.
    /// </summary>
    /// <returns></returns>
    public Entity Prefab()
    {
        return new Entity(Handle).Add(EcsPrefab);
    }

    /// <summary>
    ///     Creates a prefab with the provided name.
    /// </summary>
    /// <returns></returns>
    public Entity Prefab(string name)
    {
        return new Entity(Handle, name).Add(EcsPrefab);
    }

    /// <summary>
    ///     Creates a prefab that's associated with a type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Prefab<T>()
    {
        return new Component<T>(Handle).Add(EcsPrefab);
    }

    /// <summary>
    ///     Creates a prefab that's associated with a type.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Prefab<T>(string name)
    {
        return new Component<T>(Handle, name).Add(EcsPrefab);
    }

    /// <summary>
    ///     Creates a new event.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public EventBuilder Event(ulong eventId)
    {
        return new EventBuilder(Handle, eventId);
    }

    /// <summary>
    ///     Creates a new event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public EventBuilder Event<T>()
    {
        return new EventBuilder(Handle, Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Creates a new event.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public EventBuilder Event<T>(T value) where T : Enum
    {
        return new EventBuilder(Handle, Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <returns></returns>
    public Term Term()
    {
        return new Term(Handle);
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Term Term(ecs_term_t value)
    {
        return new Term(Handle, value);
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Term Term(ulong id)
    {
        return new Term(Handle, id);
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public Term Term(ulong first, ulong second)
    {
        return new Term(Handle, first, second);
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Term Term<T>()
    {
        return new Term(Handle, Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Term Term<T>(T value) where T : Enum
    {
        return Term<T>(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <returns></returns>
    public Term Term<TFirst>(ulong second)
    {
        return new Term(Handle, Ecs.Pair<TFirst>(second, Handle));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Term Term<TFirst, TSecond>()
    {
        return new Term(Handle, Ecs.Pair<TFirst, TSecond>(Handle));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="second"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Term Term<TFirst, TSecond>(TSecond second) where TSecond : Enum
    {
        return Term<TFirst>(Type<TSecond>.Id(Handle, second));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Term Term<TFirst, TSecond>(TFirst first) where TFirst : Enum
    {
        return TermSecond<TSecond>(Type<TFirst>.Id(Handle, first));
    }

    /// <summary>
    ///     Creates a new term.
    /// </summary>
    /// <param name="first"></param>
    /// <typeparam name="TSecond"></typeparam>
    /// <returns></returns>
    public Term TermSecond<TSecond>(ulong first)
    {
        return new Term(Handle, Ecs.PairSecond<TSecond>(first, Handle));
    }

    /// <summary>
    ///     Iterate over all entities with the provided (component) id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    public void Each(ulong id, Ecs.EachEntityCallback callback)
    {
        ecs_iter_t it = ecs_each_id(Handle, id);
        while (ecs_each_next(&it))
            Invoker.Each(&it, callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="callback"></param>
    public void Each(ulong first, ulong second, Ecs.EachEntityCallback callback)
    {
        Each(Ecs.Pair(first, second), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    public void Each<T>(T value, Ecs.EachEntityRefCallback<T> callback) where T : Enum
    {
        Each(Type<T>.Id(Handle, value), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="second"></param>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    public void Each<TFirst>(ulong second, Ecs.EachEntityRefCallback<TFirst> callback)
    {
        ecs_iter_t it = ecs_each_id(Handle, Ecs.Pair<TFirst>(second, Handle));
        while (ecs_each_next(&it))
            Invoker.Each(&it, callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Each<TFirst, TSecond>(Ecs.EachEntityCallback callback)
    {
        Each(Pair<TFirst, TSecond>(), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Each<TFirst, TSecond>(Ecs.EachEntityRefCallback<TFirst> callback)
    {
        Each(Type<TSecond>.Id(Handle), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Each<TFirst, TSecond>(Ecs.EachEntityRefCallback<TSecond> callback)
    {
        EachSecond(Type<TFirst>.Id(Handle), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="second"></param>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Each<TFirst, TSecond>(TSecond second, Ecs.EachEntityRefCallback<TFirst> callback) where TSecond : Enum
    {
        Each(Type<TSecond>.Id(Handle, second), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="callback"></param>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public void Each<TFirst, TSecond>(TFirst first, Ecs.EachEntityRefCallback<TSecond> callback) where TFirst : Enum
    {
        EachSecond(Type<TFirst>.Id(Handle, first), callback);
    }

    /// <summary>
    ///     Iterate over all entities with the provided pair.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="callback"></param>
    /// <typeparam name="TSecond"></typeparam>
    public void EachSecond<TSecond>(ulong first, Ecs.EachEntityRefCallback<TSecond> callback)
    {
        ecs_iter_t it = ecs_each_id(Handle, Ecs.PairSecond<TSecond>(first, Handle));
        while (ecs_each_next(&it))
            Invoker.Each(&it, callback);
    }

    /// <summary>
    ///     Convert enum constant to entity.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity ToEntity<T>(T value) where T : Enum
    {
        return Entity(value);
    }

    /// <summary>
    ///     Define a module.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TModule"></typeparam>
    /// <returns></returns>
    public Entity Module<TModule>(string name = "") where TModule : IFlecsModule, new()
    {
        Entity result = Entity(Type<TModule>.Id(Handle));

        if (string.IsNullOrEmpty(name))
            return Entity(result);

        using NativeString nativeName = (NativeString)name;

        Entity prevParent = result.Parent();
        ecs_add_path_w_sep(Handle, result, 0, nativeName, Pointers.DefaultSeparator, Pointers.DefaultSeparator);
        Entity parent = result.Parent();

        if (prevParent == parent)
            return Entity(result);

        if (parent != 0)
            parent.Add(Ecs.Module);

        // Module was reparented, cleanup old parent(s)
        Entity current = prevParent;

        while (current != 0)
        {
            Entity next = current.Parent();

            ecs_iter_t it = ecs_each_id(Handle, Pair(Ecs.ChildOf, current));

            if (!ecs_iter_is_true(&it))
            {
                current.Destruct();
                SetVersion(current);
            }

            current = next;
        }

        return Entity(result);
    }

    /// <summary>
    ///     Imports a module.
    /// </summary>
    /// <typeparam name="T">The module type.</typeparam>
    /// <returns>The module entity.</returns>
    public Entity Import<T>() where T : IFlecsModule, new()
    {
        return Ecs.Import<T>(this);
    }

    /// <summary>
    ///     Set pipeline.
    /// </summary>
    /// <param name="pipeline"></param>
    public void SetPipeline(ulong pipeline)
    {
        ecs_set_pipeline(Handle, pipeline);
    }

    /// <summary>
    ///     Set pipeline.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void SetPipeline<T>()
    {
        SetPipeline(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Set pipeline.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void SetPipeline<T>(T value) where T : Enum
    {
        SetPipeline(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Get pipeline.
    /// </summary>
    /// <returns></returns>
    public Entity GetPipeline()
    {
        return Entity(ecs_get_pipeline(Handle));
    }

    /// <summary>
    ///     Progress world one tick.
    /// </summary>
    /// <returns></returns>
    public bool Progress(float deltaTime = 0)
    {
        return ecs_progress(Handle, deltaTime);
    }

    /// <summary>
    ///     Run pipeline.
    /// </summary>
    /// <param name="pipeline"></param>
    /// <param name="deltaTime"></param>
    public void RunPipeline(ulong pipeline, float deltaTime = 0)
    {
        ecs_run_pipeline(Handle, pipeline, deltaTime);
    }

    /// <summary>
    ///     Run pipeline.
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <typeparam name="TPipeline"></typeparam>
    public void RunPipeline<TPipeline>(float deltaTime = 0)
    {
        ecs_run_pipeline(Handle, Type<TPipeline>.Id(Handle), deltaTime);
    }

    /// <summary>
    ///     Set timescale.
    /// </summary>
    /// <param name="scale"></param>
    public void SetTimeScale(float scale)
    {
        ecs_set_time_scale(Handle, scale);
    }

    /// <summary>
    ///     Set target FPS.
    /// </summary>
    /// <param name="targetFps"></param>
    public void SetTargetFps(float targetFps)
    {
        ecs_set_target_fps(Handle, targetFps);
    }

    /// <summary>
    ///     Reset simulation clock.
    /// </summary>
    public void ResetClock()
    {
        ecs_reset_clock(Handle);
    }

    /// <summary>
    ///     Set number of threads.
    /// </summary>
    /// <param name="threads"></param>
    public void SetThreads(int threads)
    {
        ecs_set_threads(Handle, threads);
    }

    /// <summary>
    ///     Set number of threads.
    /// </summary>
    /// <returns></returns>
    public int GetThreads()
    {
        return ecs_get_stage_count(Handle);
    }

    /// <summary>
    ///     Set number of task threads.
    /// </summary>
    /// <param name="taskThreads"></param>
    public void SetTaskThreads(int taskThreads)
    {
        ecs_set_task_threads(Handle, taskThreads);
    }

    /// <summary>
    ///     Returns true if task thread use has been requested.
    /// </summary>
    /// <returns></returns>
    public bool UsingTaskThreads()
    {
        return ecs_using_task_threads(Handle);
    }

    /// <summary>
    ///     Creates a timer.
    /// </summary>
    /// <returns></returns>
    public TimerEntity Timer()
    {
        return new TimerEntity(Handle);
    }

    /// <summary>
    ///     Creates a timer.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TimerEntity Timer(ulong id)
    {
        return new TimerEntity(Handle, id);
    }

    /// <summary>
    ///     Creates a timer.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public TimerEntity Timer(string name)
    {
        return new TimerEntity(Handle, name);
    }

    /// <summary>
    ///     Creates a timer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public TimerEntity Timer<T>()
    {
        return Timer(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Creates a timer.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void Timer<T>(T value) where T : Enum
    {
        Timer(Type<T>.Id(Handle, value));
    }

    /// <summary>
    ///     Enable randomization of initial time values for timers.
    /// </summary>
    public void RandomizeTimers()
    {
        ecs_randomize_timers(Handle);
    }

    /// <summary>
    ///     Run script.
    /// </summary>
    /// <param name="name">The script name (typically the file).</param>
    /// <param name="str">The script.</param>
    /// <returns>Zero if success, non-zero otherwise.</returns>
    public int ScriptRun(string name, string str)
    {
        using NativeString nativeName = (NativeString)name;
        using NativeString nativeStr = (NativeString)str;

        return ecs_script_run(Handle, nativeName, nativeStr);
    }

    /// <summary>
    ///     Run script from file.
    /// </summary>
    /// <param name="fileName">The script file name.</param>
    /// <returns>Zero if success, non-zero if failed.</returns>
    public int ScriptRunFile(string fileName)
    {
        using NativeString nativeFileName = (NativeString)fileName;
        return ecs_script_run_file(Handle, nativeFileName);
    }

    /// <summary>
    ///     Creates a script builder.
    /// </summary>
    /// <returns>The created script builder.</returns>
    public ScriptBuilder Script()
    {
        return new ScriptBuilder(Handle);
    }

    /// <summary>
    ///     Creates a script builder with the provided entity name.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <returns>The created script builder.</returns>
    public ScriptBuilder Script(string name)
    {
        return new ScriptBuilder(Handle, name);
    }

    /// <summary>
    ///     Convert value to string.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string ToExpr(ulong id, void* value)
    {
        return NativeString.GetStringAndFree(ecs_ptr_to_expr(Handle, id, value));
    }

    /// <summary>
    ///     Convert value to string.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <returns>String with expression, or empty string if failed.</returns>
    public string ToExpr<T>(T* value) where T : unmanaged
    {
        return ToExpr(Type<T>.Id(Handle), value);
    }

    /// <summary>
    ///     Covert value to string.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <returns>String with expression, or empty string if failed.</returns>
    public string ToExpr<T>(ref T value) where T : unmanaged
    {
        fixed (T* ptr = &value)
        {
            return ToExpr(ptr);
        }
    }

    /// <summary>
    ///     Return meta cursor to value.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public Cursor Cursor(ulong id, void* data)
    {
        return new Cursor(Handle, id, data);
    }

    /// <summary>
    ///     Return meta cursor to value.
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Cursor Cursor<T>(T* data)
    {
        return Cursor(Type<T>.Id(Handle), data);
    }

    /// <summary>
    ///     Return meta cursor to value.
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // TODO: Figure out better api that doesn't cause GC hole
    public Cursor Cursor<T>(ref T data) where T : unmanaged
    {
        fixed (T* ptr = &data)
        {
            return Cursor(ptr);
        }
    }

    /// <summary>
    ///     Create primitive type.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public Entity Primitive(ecs_primitive_kind_t kind)
    {
        ecs_primitive_desc_t desc = default;
        desc.kind = kind;

        ulong id = ecs_primitive_init(Handle, &desc);
        Ecs.Assert(id != 0, nameof(ECS_INVALID_OPERATION));

        return Entity(id);
    }

    /// <summary>
    ///     Creates array type.
    /// </summary>
    /// <param name="elemId"></param>
    /// <param name="arrayCount"></param>
    /// <returns></returns>
    public Entity Array(ulong elemId, int arrayCount)
    {
        ecs_array_desc_t desc = default;
        desc.type = elemId;
        desc.count = arrayCount;

        ulong id = ecs_array_init(Handle, &desc);
        Ecs.Assert(id != 0, nameof(ECS_INVALID_OPERATION));

        return Entity(id);
    }

    /// <summary>
    ///     Creates array type.
    /// </summary>
    /// <param name="arrayCount"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Array<T>(int arrayCount)
    {
        return Array(Type<T>.Id(Handle), arrayCount);
    }

    /// <summary>
    ///     Create vector type.
    /// </summary>
    /// <param name="elemId"></param>
    /// <returns></returns>
    public Entity Vector(ulong elemId)
    {
        ecs_vector_desc_t desc = default;
        desc.type = elemId;
        ulong id = ecs_vector_init(Handle, &desc);
        Ecs.Assert(id != 0, nameof(ECS_INVALID_OPERATION));
        return Entity(id);
    }

    /// <summary>
    ///     Create vector type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity Vector<T>()
    {
        return Vector(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Serialize untyped value to JSON.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string ToJson(ulong id, void* value)
    {
        return NativeString.GetStringAndFree(ecs_ptr_to_json(Handle, id, value));
    }

    /// <summary>
    ///     Serialize value to JSON.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // TODO: Support managed types with ToJson and FromJson.
    public string ToJson<T>(T* value) where T : unmanaged
    {
        return ToJson(Type<T>.Id(Handle), value);
    }

    /// <summary>
    ///     Serialize value to JSON.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string ToJson<T>(ref T value) where T : unmanaged
    {
        fixed (T* ptr = &value)
        {
            return ToJson(ptr);
        }
    }

    /// <summary>
    ///     Serializes the world to a JSON string using the provided description.
    /// </summary>
    /// <param name="desc">The description settings for JSON serialization.</param>
    /// <returns>A JSON string with the serialized world data, or an empty string if failed.</returns>
    public string ToJson(in WorldToJsonDesc desc)
    {
        fixed (ecs_world_to_json_desc_t* ptr = &desc.Desc)
            return NativeString.GetStringAndFree(ecs_world_to_json(Handle, ptr));
    }

    /// <summary>
    ///     Serializes the world to a JSON string.
    /// </summary>
    /// <returns>A JSON string with the serialized world data, or an empty string if failed.</returns>
    public string ToJson()
    {
        return NativeString.GetStringAndFree(ecs_world_to_json(Handle, null));
    }

    /// <summary>
    ///     Deserialize value from JSON.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <param name="json"></param>
    /// <param name="desc"></param>
    /// <returns></returns>
    public bool FromJson(ulong id, void* value, string json, ecs_from_json_desc_t* desc = null)
    {
        using NativeString nativeJson = (NativeString)json;
        return ecs_ptr_from_json(Handle, id, value, nativeJson, desc) != null;
    }

    /// <summary>
    ///     Deserialize value from JSON.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="json"></param>
    /// <param name="desc"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool FromJson<T>(T* value, string json, ecs_from_json_desc_t* desc = null) where T : unmanaged
    {
        return FromJson(Type<T>.Id(Handle), value, json, desc);
    }

    /// <summary>
    ///     Deserialize value from JSON.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="desc"></param>
    /// <returns></returns>
    public bool FromJson(string json, ecs_from_json_desc_t* desc)
    {
        using NativeString nativeJson = (NativeString)json;
        return ecs_world_from_json(Handle, nativeJson, desc) != null;
    }

    /// <summary>
    ///     Deserialize value from JSON.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public bool FromJson(string json)
    {
        using NativeString nativeJson = (NativeString)json;
        return FromJson(json, null);
    }

    /// <summary>
    ///     Deserialize JSON file into world.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="desc"></param>
    /// <returns></returns>
    public bool FromJsonFile(string file, ecs_from_json_desc_t *desc)
    {
        using NativeString nativeFile = (NativeString)file;
        return ecs_world_from_json_file(Handle, nativeFile, desc) != null;
    }

    /// <summary>
    ///     Deserialize JSON file into world.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool FromJsonFile(string file)
    {
        return FromJsonFile(file, null);
    }

    /// <summary>
    ///     Creates an app builder.
    /// </summary>
    /// <returns></returns>
    public AppBuilder App()
    {
        return new AppBuilder(Handle);
    }

    /// <summary>
    ///     Create metric builder.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public MetricBuilder Metric(ulong entity)
    {
        return new MetricBuilder(Handle, entity);
    }

    /// <summary>
    ///     Creates an alert builder.
    /// </summary>
    /// <returns>An alert builder.</returns>
    public AlertBuilder AlertBuilder()
    {
        return new AlertBuilder(Handle);
    }

    /// <summary>
    ///     Creates an alert builder.
    /// </summary>
    /// <param name="name">The alert name.</param>
    /// <returns>An alert builder.</returns>
    public AlertBuilder AlertBuilder(string name)
    {
        return new AlertBuilder(Handle, name);
    }

    /// <summary>
    ///     Creates an alert builder.
    /// </summary>
    /// <param name="entity">The alert entity.</param>
    /// <returns>An alert builder.</returns>
    public AlertBuilder AlertBuilder(ulong entity)
    {
        return new AlertBuilder(Handle, entity);
    }

    /// <summary>
    ///     Creates an alert.
    /// </summary>
    /// <returns>An alert.</returns>
    public Alert Alert()
    {
        return AlertBuilder().Build();
    }

    /// <summary>
    ///     Creates an alert.
    /// </summary>
    /// <param name="name">The alert name.</param>
    /// <returns>An alert.</returns>
    public Alert Alert(string name)
    {
        return AlertBuilder(name).Build();
    }

    /// <summary>
    ///     Creates an alert.
    /// </summary>
    /// <param name="entity">The alert entity.</param>
    /// <returns>An alert.</returns>
    public Alert Alert(ulong entity)
    {
        return AlertBuilder(entity).Build();
    }

    /// <summary>
    ///     Creates a query builder.
    /// </summary>
    /// <returns>A query builder.</returns>
    public QueryBuilder QueryBuilder()
    {
        return new QueryBuilder(Handle);
    }

    /// <summary>
    ///     Creates a query builder.
    /// </summary>
    /// <param name="name">The query name.</param>
    /// <returns>A query builder.</returns>
    public QueryBuilder QueryBuilder(string name)
    {
        return new QueryBuilder(Handle, name);
    }

    /// <summary>
    ///     Creates a query builder.
    /// </summary>
    /// <param name="entity">The query entity.</param>
    /// <returns>A query builder.</returns>
    public QueryBuilder QueryBuilder(ulong entity)
    {
        return new QueryBuilder(Handle, entity);
    }

    /// <summary>
    ///     Creates a query.
    /// </summary>
    /// <returns>A query.</returns>
    public Query Query()
    {
        return QueryBuilder().Build();
    }

    /// <summary>
    ///     Creates a query.
    /// </summary>
    /// <param name="name">The query name.</param>
    /// <returns>A query.</returns>
    public Query Query(string name)
    {
        return new Query(Handle, Lookup(name));
    }

    /// <summary>
    ///     Creates a query.
    /// </summary>
    /// <param name="entity">The query entity.</param>
    /// <returns>A query.</returns>
    public Query Query(ulong entity)
    {
        return new Query(Handle, entity);
    }

    /// <summary>
    ///     Creates a pipeline builder.
    /// </summary>
    /// <returns>A pipeline builder.</returns>
    public PipelineBuilder Pipeline()
    {
        return new PipelineBuilder(Handle);
    }

    /// <summary>
    ///     Creates a pipeline builder.
    /// </summary>
    /// <param name="name">The pipeline name.</param>
    /// <returns>A pipeline builder.</returns>
    public PipelineBuilder Pipeline(string name)
    {
        return new PipelineBuilder(Handle, name);
    }

    /// <summary>
    ///     Creates a pipeline builder.
    /// </summary>
    /// <param name="entity">The pipeline entity.</param>
    /// <returns>A pipeline builder.</returns>
    public PipelineBuilder Pipeline(ulong entity)
    {
        return new PipelineBuilder(Handle, entity);
    }

    /// <summary>
    ///     Creates a pipeline builder associated with the provided type.
    /// </summary>
    /// <returns>A pipeline builder.</returns>
    public PipelineBuilder Pipeline<T>()
    {
        return Pipeline(Type<T>.Id(Handle));
    }

    /// <summary>
    ///     Creates an observer builder.
    /// </summary>
    /// <returns>An observer builder.</returns>
    public ObserverBuilder Observer()
    {
        return new ObserverBuilder(Handle);
    }

    /// <summary>
    ///     Creates an observer builder.
    /// </summary>
    /// <param name="name">The observer name.</param>
    /// <returns>An observer builder.</returns>
    public ObserverBuilder Observer(string name)
    {
        return new ObserverBuilder(Handle, name);
    }

    /// <summary>
    ///     Creates an observer.
    /// </summary>
    /// <param name="entity">The observer entity.</param>
    /// <returns>An observer.</returns>
    public Observer Observer(ulong entity)
    {
        return new Observer(Handle, entity);
    }

    /// <summary>
    ///     Creates a system builder.
    /// </summary>
    /// <returns>A system builder.</returns>
    public SystemBuilder System()
    {
        return new SystemBuilder(Handle);
    }

    /// <summary>
    ///     Creates a system builder.
    /// </summary>
    /// <param name="name">The system name.</param>
    /// <returns>A system builder.</returns>
    public SystemBuilder System(string name)
    {
        return new SystemBuilder(Handle, name);
    }

    /// <summary>
    ///     Create a system.
    /// </summary>
    /// <param name="entity">The system entity.</param>
    /// <returns>A system.</returns>
    public System_ System(ulong entity)
    {
        return new System_(Handle, entity);
    }

    /// <summary>
    ///     Initializes built-in components.
    /// </summary>
    public void InitBuiltinComponents()
    {
        Import<Ecs.Meta>();
    }

    internal WorldContext* GetBindingContext()
    {
        return (WorldContext*)ecs_get_binding_ctx_fast(Handle);
    }

    private WorldContext* EnsureBindingContext()
    {
        WorldContext* ptr = (WorldContext*)ecs_get_binding_ctx_fast(Handle);

        if (ptr != null)
            return ptr;

        ptr = Memory.AllocZeroed<WorldContext>(1);
        ecs_set_binding_ctx(Handle, ptr, &Functions.WorldContextFree);
        return ptr;
    }

    private PostFrameContext* AllocatePostFrameContext()
    {
        PostFrameContext* context = Memory.Alloc<PostFrameContext>(1);
        WorldContext.RunPostFrameContexts.Add((nint)context);
        return context;
    }

    private WorldFinishContext* AllocateWorldFinishContext()
    {
        WorldFinishContext* context = Memory.Alloc<WorldFinishContext>(1);
        WorldContext.WorldFinishContexts.Add((nint)context);
        return context;
    }

    /// <summary>
    ///     Returns native pointer to world.
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static ecs_world_t* To(World world)
    {
        return world.Handle;
    }

    /// <summary>
    ///     Returns world wrapper for native world pointer.
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static World From(ecs_world_t* world)
    {
        return new World(world);
    }

    /// <summary>
    ///     Returns native pointer to world.
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static implicit operator ecs_world_t*(World world)
    {
        return To(world);
    }

    /// <summary>
    ///     Returns world wrapper for native world pointer.
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static implicit operator World(ecs_world_t* world)
    {
        return From(world);
    }

    /// <summary>
    ///     Checks if two <see cref="World"/> instances are equal.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(World other)
    {
        return Handle == other.Handle;
    }

    /// <summary>
    ///     Checks if two <see cref="World"/> instances are equal.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is World world && Equals(world);
    }

    /// <summary>
    ///     Returns the hash code of the <see cref="World"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Handle->GetHashCode();
    }

    /// <summary>
    ///     Checks if two <see cref="World"/> instances are equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(World left, World right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Checks if two <see cref="World"/> instances are not equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(World left, World right)
    {
        return !(left == right);
    }
}

// Flecs.NET Extensions
public unsafe partial struct World
{
    /// <summary>
    ///     Gets an empty table.
    /// </summary>
    /// <returns></returns>
    public Table Table()
    {
        return new Table(Handle);
    }

    /// <summary>
    ///     Create new entity in table.
    /// </summary>
    /// <param name="table">The table to which to add the new entity.</param>
    /// <returns>The new entity.</returns>
    public Entity Entity(Table table)
    {
        return Entity(ecs_new_w_table(Handle, table));
    }

    /// <summary>
    ///     Set current with id.
    /// </summary>
    /// <param name="with">The id.</param>
    /// <returns>The previous id.</returns>
    public Entity SetWith(ulong with)
    {
        return new Entity(Handle, ecs_set_with(Handle, with));
    }

    /// <summary>
    ///     Get current with id.
    /// </summary>
    /// <returns>The last id provided to ecs_set_with().</returns>
    public Entity GetWith()
    {
        return new Entity(Handle, ecs_get_with(Handle));
    }

    /// <summary>
    ///     All entities created in function are created in scope. All operations
    ///     called in function (such as lookup) are relative to scope.
    /// </summary>
    /// <param name="parent">The entity to use as scope.</param>
    /// <param name="callback">The callback.</param>
    public void Scope(ulong parent, Ecs.WorldCallback callback)
    {
        ulong prev = ecs_set_scope(Handle, parent);
        callback(Handle);
        ecs_set_scope(Handle, prev);
    }

    /// <summary>
    ///     Same as Scope(parent, func), but with T as parent.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The entity to use as scope.</typeparam>
    public void Scope<T>(Ecs.WorldCallback callback)
    {
        Scope(Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     Same as Scope(parent, func), but with enum as parent.
    /// </summary>
    /// <param name="value">The entity to use as scope.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The enum.</typeparam>
    public void Scope<T>(T value, Ecs.WorldCallback callback) where T : Enum
    {
        Scope(Type<T>.Id(Handle, value), callback);
    }

    /// <summary>
    ///     Defer all operations called in function. If the world is already in
    ///     deferred mode, do nothing.
    /// </summary>
    /// <param name="callback">The callback.</param>
    public void Defer(Ecs.WorldCallback callback)
    {
        ecs_defer_begin(Handle);
        callback(Handle);
        ecs_defer_end(Handle);
    }

    /// <summary>
    ///     All entities created in function are created with id.
    /// </summary>
    /// <param name="id">Id to be added to the created entities.</param>
    /// <param name="callback">The callback.</param>
    public void With(ulong id, Ecs.WorldCallback callback)
    {
        ulong prev = ecs_set_with(Handle, id);
        callback(Handle);
        ecs_set_with(Handle, prev);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first id.</param>
    /// <param name="second">The second id.</param>
    /// <param name="callback">The callback.</param>
    public void With(ulong first, ulong second, Ecs.WorldCallback callback)
    {
        With(Ecs.Pair(first, second), callback);
    }

    /// <summary>
    ///     All entities created in function are created with type.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The component.</typeparam>
    public void With<T>(Ecs.WorldCallback callback)
    {
        With(Type<T>.Id(Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with enum.
    /// </summary>
    /// <param name="value">The enum constant.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="T">The enum.</typeparam>
    public void With<T>(T value, Ecs.WorldCallback callback) where T : Enum
    {
        With<T>(Type<T>.Id(Handle, value), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="second">The second id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    public void With<TFirst>(ulong second, Ecs.WorldCallback callback)
    {
        With(Ecs.Pair<TFirst>(second, Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void With<TFirst, TSecond>(Ecs.WorldCallback callback)
    {
        With(Ecs.Pair<TFirst, TSecond>(Handle), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="second">The second enum constant id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first component.</typeparam>
    /// <typeparam name="TSecond">The second enum.</typeparam>
    public void With<TFirst, TSecond>(TSecond second, Ecs.WorldCallback callback) where TSecond : Enum
    {
        With<TFirst>(Type<TSecond>.Id(Handle, second), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first enum constant.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TFirst">The first enum.</typeparam>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void With<TFirst, TSecond>(TFirst first, Ecs.WorldCallback callback) where TFirst : Enum
    {
        WithSecond<TSecond>(Type<TFirst>.Id(Handle, first), callback);
    }

    /// <summary>
    ///     All entities created in function are created with pair.
    /// </summary>
    /// <param name="first">The first id.</param>
    /// <param name="callback">The callback.</param>
    /// <typeparam name="TSecond">The second component.</typeparam>
    public void WithSecond<TSecond>(ulong first, Ecs.WorldCallback callback)
    {
        With(first, Type<TSecond>.Id(Handle), callback);
    }

    /// <summary>
    ///     Lookup entity by name.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <returns>The entity if found, else 0.</returns>
    public Entity Lookup(string path)
    {
        return Lookup(path, true);
    }

    /// <summary>
    ///     Lookup entity by name.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <returns>The entity if found, else 0.</returns>
    public Entity Lookup(string path, bool recursive)
    {
        if (string.IsNullOrEmpty(path))
            return new Entity(Handle, 0);

        using NativeString nativePath = (NativeString)path;

        return new Entity(
            Handle,
            ecs_lookup_path_w_sep(Handle, 0, nativePath,
                Pointers.DefaultSeparator, Pointers.DefaultSeparator, recursive)
        );
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, out Entity entity)
    {
        return TryLookup(path, Ecs.DefaultSeparator, Ecs.DefaultSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, bool recursive, out Entity entity)
    {
        return TryLookup(path, Ecs.DefaultSeparator, Ecs.DefaultSeparator, recursive, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, out Entity entity)
    {
        return TryLookup(path, separator, Ecs.DefaultSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="rootSeparator">The root separator.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, string rootSeparator, out Entity entity)
    {
        return TryLookup(path, separator, rootSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="rootSeparator">The root separator.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, string rootSeparator, bool recursive, out Entity entity)
    {
        return (entity = Lookup(path, separator, rootSeparator, recursive)) != 0;
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, out ulong entity)
    {
        return TryLookup(path, Ecs.DefaultSeparator, Ecs.DefaultSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, bool recursive, out ulong entity)
    {
        return TryLookup(path, Ecs.DefaultSeparator, Ecs.DefaultSeparator, recursive, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, out ulong entity)
    {
        return TryLookup(path, separator, Ecs.DefaultSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="rootSeparator">The root separator.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, string rootSeparator, out ulong entity)
    {
        return TryLookup(path, separator, rootSeparator, true, out entity);
    }

    /// <summary>
    ///     Lookup an entity from a path.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="rootSeparator">The root separator.</param>
    /// <param name="recursive">Recursively traverse up the tree until entity is found.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookup(string path, string separator, string rootSeparator, bool recursive, out ulong entity)
    {
        return (entity = Lookup(path, separator, rootSeparator, recursive)) != 0;
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="lookupAsPath">If not found as a symbol, lookup as path.</param>
    /// <param name="recursive">If looking up as path, recursively traverse up the tree.</param>
    /// <returns>The entity if found, else 0.</returns>
    public Entity LookupSymbol(string symbol, bool lookupAsPath = false, bool recursive = false)
    {
        if (string.IsNullOrEmpty(symbol))
            return new Entity(Handle, 0);

        using NativeString nativeSymbol = (NativeString)symbol;

        return new Entity(
            Handle,
            ecs_lookup_symbol(Handle, nativeSymbol, lookupAsPath, recursive)
        );
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, out Entity entity)
    {
        return TryLookupSymbol(symbol, false, false, out entity);
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="lookupAsPath">If not found as a symbol, lookup as path.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, bool lookupAsPath, out Entity entity)
    {
        return TryLookupSymbol(symbol, lookupAsPath, false, out entity);
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="lookupAsPath">If not found as a symbol, lookup as path.</param>
    /// <param name="recursive">If looking up as path, recursively traverse up the tree.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, bool lookupAsPath, bool recursive, out Entity entity)
    {
        return (entity = LookupSymbol(symbol, lookupAsPath, recursive)) != 0;
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, out ulong entity)
    {
        return TryLookupSymbol(symbol, false, false, out entity);
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="lookupAsPath">If not found as a symbol, lookup as path.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, bool lookupAsPath, out ulong entity)
    {
        return TryLookupSymbol(symbol, lookupAsPath, false, out entity);
    }

    /// <summary>
    ///     Lookup an entity by its symbol name.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="lookupAsPath">If not found as a symbol, lookup as path.</param>
    /// <param name="recursive">If looking up as path, recursively traverse up the tree.</param>
    /// <param name="entity">The entity if found, else 0.</param>
    /// <returns>True if the entity was found, else false.</returns>
    public bool TryLookupSymbol(string symbol, bool lookupAsPath, bool recursive, out ulong entity)
    {
        return (entity = LookupSymbol(symbol, lookupAsPath, recursive)) != 0;
    }

    /// <summary>
    ///     Cleanup empty tables.
    /// </summary>
    /// <param name="desc">The description object.</param>
    /// <returns>The number of deleted tables.</returns>
    public int DeleteEmptyTables(in DeleteEmptyTablesDesc desc)
    {
        fixed (ecs_delete_empty_tables_desc_t* ptr = &desc.Desc)
            return ecs_delete_empty_tables(Handle, ptr);
    }
}
