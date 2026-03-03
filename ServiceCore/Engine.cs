/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using Cysharp.Threading.Tasks;
using ServiceCore.Loading;
using ServiceCore.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Main class for <see cref="ServiceCore"/> Library.
    /// </summary>
    public static partial class Engine
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Prefix for console messages sent from this class.
        /// </summary>
        public const string LogPrefix = "[" + nameof(ServiceCore) + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Sorting function for <see cref="NativeAssemblies"/>.
        /// </summary>
        public delegate IEnumerable<Assembly> AssemblySorter(Assembly[] assemblies);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invoked right after <see cref="Initialize"/> is called.
        /// </summary>
        public static event Action? OnEngineInitializing;
        /// <summary>
        /// Invoked when all engine systems, including <see cref="Services"/>, were fully initialized.
        /// </summary>
        public static event Action? OnEngineInitialized;
        /// <summary>
        /// Invoked right after <see cref="Terminate"/> is called.
        /// </summary>
        public static event Action? OnEngineTerminating;
        /// <summary>
        /// Invoked when entire <see cref="Engine"/> were terminated.
        /// </summary>
        public static event Action? OnEngineTerminated;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Auto-fire Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <inheritdoc cref="OnEngineInitializing"/>
        /// </summary>
        /// <remarks>
        /// If <see cref="OnEngineInitializing"/> event was already fired - immediately fires attaching callback.
        /// <para>
        /// To get consistent callback, you can use <see cref="ServiceCoreInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action? FireWithEngineInitializing
        {
            remove => OnEngineInitializing -= value;
            add
            {
                if (value is null) return;
                const EngineStatus Combined = EngineStatus.Initializing | EngineStatus.Initialized;
                if ((Status & Combined) != EngineStatus.Invalid)
                {
                    value();
                }

                OnEngineInitializing += value;
            }
        }

        /// <summary>
        /// <inheritdoc cref="OnEngineInitializing"/>
        /// </summary>
        /// <remarks>
        /// If <see cref="OnEngineInitialized"/> event was already fired - immediately fires attaching callback.
        /// <para>
        /// To get consistent callback, you can use <see cref="ServiceCoreInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action? FireWithEngineInitialized
        {
            remove => OnEngineInitialized -= value;
            add
            {
                if (value is null) return;
                if ((Status & EngineStatus.Initialized) == EngineStatus.Initialized)
                {
                    value();
                }

                OnEngineInitialized += value;
            }
        }

        /// <summary>
        /// <inheritdoc cref="OnEngineTerminating"/>
        /// </summary>
        /// <remarks>
        /// If <see cref="OnEngineTerminating"/> event was already fired - immediately fires attaching callback.
        /// <para>
        /// To get consistent callback, you can use <see cref="ServiceCoreInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action? FireWithEngineTerminating
        {
            remove => OnEngineTerminating -= value;
            add
            {
                if (value is null) return;
                const EngineStatus Combined = EngineStatus.Terminating | EngineStatus.Terminated;
                if ((Status & Combined) != EngineStatus.Invalid)
                {
                    value();
                }

                OnEngineTerminating += value;
            }
        }

        /// <summary>
        /// <inheritdoc cref="OnEngineTerminated"/>
        /// </summary>
        /// <remarks>
        /// If <see cref="OnEngineTerminated"/> event was already fired - immediately fires attaching callback.
        /// <para>
        /// To get consistent callback, you can use <see cref="ServiceCoreInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action? FireWithEngineTerminated
        {
            remove => OnEngineTerminated -= value;
            add
            {
                if (value is null) return;
                if ((Status & EngineStatus.Terminated) == EngineStatus.Terminated)
                {
                    value();
                }

                OnEngineTerminated += value;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Status of the engine.
        /// </summary>
        /// <remarks>
        /// <para>Set to <see cref="EngineStatus.Terminated"/> - by default.</para>
        /// <para>Set to <see cref="EngineStatus.Initializing"/> - during initialization (after calling <see cref="Initialize"/>).</para>
        /// <para>Set to <see cref="EngineStatus.Initialized"/> - when <see cref="Engine"/> and <see cref="Modification"/>s are fully initialized!</para>
        /// <para>Set to <see cref="EngineStatus.Terminating"/> - during unloading (after <see cref="Terminate"/>/automatically by <see cref="QuitHandler"/>)</para>
        /// <para>Set to <see cref="EngineStatus.InitializationBroken"/> - if engine got irreversibly broken during initialization.</para>
        /// <para>Set to <see cref="EngineStatus.TerminationBroken"/> - if engine got irreversibly broken during unloading.</para>
        /// </remarks>
        public static EngineStatus Status => m_State.Status;

        /// <summary>
        /// Current args of the engine.
        /// </summary>
        public static EngineState State => m_State;

        /// <summary>
        /// Lists all Modifications referencing <see cref="Engine"/>.
        /// Such Modifications are considered "Native" and will be automatically loaded first on <see cref="Initialize"/> call.
        /// </summary>
        public static IReadOnlyList<Assembly> NativeAssemblies => m_NativeAssemblies;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly EngineState m_State = new(EngineStatus.Terminated); // Starts as terminated.
        private static readonly List<Assembly> m_NativeAssemblies = []; // 
        private static readonly AssemblyStorage m_Assemblies = new(64); // NOOOO! My square field declaration! T^T




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Initialization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initializes the entire engine: <see cref="IService"/>s, <see cref="Modification"/>s, and so on.
        /// You can specify <see cref="InitializationContext"/> to make <see cref="Engine"/> load-in community modifications.
        /// </summary>
        /// <param name="context">Specifies how <see cref="NativeAssemblies"/> should be ordered. and provides<see cref="ILoadingSource"/>s to load. </param>
        /// <param name="args">Args to use for <see cref="IService"/> termination. Replaced with <see cref="DefaultInitializationArgs"/> if not provided.</param>
        public static async UniTask Initialize(InitializationContext? context = default, IInitializationArgs? args = default)
        {
            if (Status != EngineStatus.Terminated)
            {
                ServiceCoreLogger.LogWarning($"{LogPrefix} Cannot initialize non-idle engine.");
                return;
            }

            SetStatus(EngineStatus.Initializing);
            // TODO: Decide what to do with service unloading when in the Editor.
            //  Maybe provide special UNITY_EDITOR-only methods?
            //  We can keep them in the code so people can restore Editor's tools more easily.
            //  Although, a lot of it will be gate-kept behind Application.isEditor anyway.
            //Application.quitting += ResetState;

            const bool ExpectFrequentReloads = false;
            try
            {
                if (ExpectFrequentReloads)
                {
                    // Should use caches lists instead of creating them on each call.
                    throw new NotSupportedException();
                }

                context ??= InitializationContext.Default;
                // Listing built-in assemblies with built-in services.
                // TODO: Remove allocations if needed.
                List<ILoadable> loadables = new(m_NativeAssemblies.Count);
                // TODO: Since assemblies added as loading source at the beginning of the list - they should stay here unless reordering is absolutely needed.
                // Note: Add tool to see order of initialization of all sources based on "layer orders" - value starting from 0,
                //  and multiple sources can take the same order, showing you that they are not guaranteed to be executed in a specific order.
                foreach (var assembly in context.NativeSorter is null ? m_NativeAssemblies : context.NativeSorter([.. m_NativeAssemblies]))
                {
                    loadables.Add((LoadableAssemblyReference)assembly);
                }

                NativeSource natives = new(loadables);

                // Note: Looks like it's mandatory for us to have a "core" mod after all in the code.
                //  It seems to be easier this way. Next time I will implement this, so modding can be supported properly.
                DependencyMap dependencies = m_State.Modifications;
                dependencies.Clear();
                dependencies[natives.Identifier] = natives;

                // Reserves space for sources, if there is any.
                // Note: m_Sources list will also be filled with sorted native assemblies.
                if (context.Sources is not null)
                {
                    foreach (var source in context.Sources)
                    {
                        dependencies[source.Identifier] = source;
                    }
                }

                // Resolve dependencies here.
                if (dependencies.TryResolve(out IReadOnlyList<ILoadingSource> sources))
                {
                    // Loads engine and all dependencies.
                    args ??= new DefaultInitializationArgs();
                    args.Setup(m_State);

                    await LoadInternal(sources, args);
                }
                else
                {
                    // Loads only native libraries if dependencies cannot be resolved.
                    m_State.IsDependenciesBroken = true;
                    args ??= new DefaultInitializationArgs();
                    args.Setup(m_State);

                    await LoadInternal(Provider(natives), args);
                    static IEnumerable<ILoadingSource> Provider(ILoadingSource source)
                    {
                        yield return source;
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceCoreLogger.LogException(ex);
                SetStatus(EngineStatus.InitializationBroken);
                return;
            }

            SetStatus(EngineStatus.Initialized);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Termination
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Unloads entire engine, all initialized services.
        /// </summary>
        /// <remarks>
        /// Will not unload mod Modifications from the memory, as it is impossible.
        /// </remarks>
        /// <param name="args">Args to use for <see cref="IService"/> termination. Replaced with <see cref="DefaultTerminationArgs"/> if not provided.</param>
        public static async UniTask Terminate(ITerminationArgs? args = default)
        {
            // Only already initialized engine can be unloaded.
            // (TODO) Note: should we introduce unloading of a partially loaded engine? Something to think about later.
            if (Status != EngineStatus.Initialized)
            {
                return;
            }

            SetStatus(EngineStatus.Terminating);
            // TODO: Hold callers in await block until engine is fully unloaded.
            try
            {
                // EngineState.IsDependenciesBroken args here is retained from the last initialization sequence.
                if (args is null) args = new DefaultTerminationArgs(m_State);
                else args.Setup(m_State);

                using (Services.Unsafe.Terminate())
                {
                    foreach (var service in Services.RuntimeServices)
                    {
                        // TODO: Terminate asynchronously if possible.
                        await service.Getter()!.InvokeTerminate(args);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Assemblies.Clear();
                ServiceCoreLogger.LogException(ex);
                SetStatus(EngineStatus.TerminationBroken);
                return;
            }

            m_Assemblies.Clear();
            SetStatus(EngineStatus.Terminated);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static Engine()
        {
            List<Assembly> assemblies = m_NativeAssemblies;
            Assembly engine = typeof(Engine).Assembly;

            assemblies.Add(engine);
            string current = engine.FullName;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
                {
                    // Filters assemblies who use Engine directly.
                    // Should reduce memory usage by a lot, since GC won't collect assemblies defined here.
                    if (string.Equals(reference.FullName, current, StringComparison.Ordinal))
                    {
                        assemblies.Add(assembly);
                        break;
                    }
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNative(Assembly assembly)
        {
            return m_NativeAssemblies.Any(c => c.FullName.Equals(assembly.FullName, StringComparison.Ordinal));
        }

        private static void SetStatus(EngineStatus status)
        {
            EngineStatus diff = (Status ^ status) & status; // Checks which bits have changed.

            // Order is: (initializing) -> (initialized) -> (terminating) -> terminated.
            if ((diff & EngineStatus.Initializing) != EngineStatus.Invalid && !TryFireCallback(ref OnEngineInitializing))
            {
                ServiceCoreLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineInitializing)}' event thrown exceptions! Look above for errors.");
            }

            if ((diff & EngineStatus.Initialized) != EngineStatus.Invalid && !TryFireCallback(ref OnEngineInitialized))
            {
                ServiceCoreLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineInitialized)}' event thrown exceptions! Look above for errors.");
            }

            if ((diff & EngineStatus.Terminating) != EngineStatus.Invalid && !TryFireCallback(ref OnEngineTerminating))
            {
                ServiceCoreLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineTerminating)}' event thrown exceptions! Look above for errors.");
            }

            if ((diff & EngineStatus.Terminated) != EngineStatus.Invalid && !TryFireCallback(ref OnEngineTerminated))
            {
                ServiceCoreLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineTerminated)}' event thrown exceptions! Look above for errors.");
            }

            m_State.Status = status;

            // Handles explicit status errors just in case.
            if ((diff & EngineStatus.InitializationBroken) != EngineStatus.Invalid)
            {
                ServiceCoreLogger.LogError($"{LogPrefix} {nameof(Engine)} was irreversibly broken during initialization. You will need to restart your app to fix this.");
            }

            if ((diff & EngineStatus.TerminationBroken) != EngineStatus.Invalid)
            {
                ServiceCoreLogger.LogError($"{LogPrefix} {nameof(Engine)} was irreversibly broken during unloading. You will need to restart your app to fix this.");
            }
        }

        /// <remarks>Whether callbacks was fired without any exceptions.</remarks>
        static bool TryFireCallback(ref Action? callbacks)
        {
            if (callbacks is null)
            {
                return true;
            }

            Delegate[] delegates = callbacks.GetInvocationList();
            callbacks = null;

            // Callback list should not be modifiable at this point, since after IsInitialized is set to true - callbacks are auto fired immediately.
            // Because of that, we don't need any locks, AFAIK.
            bool exceptions = false;
            foreach (var callback in delegates)
            {
                try
                {
                    callback?.DynamicInvoke();
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(ex);
                    exceptions |= true;
                }
            }

            return !exceptions;
        }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Initialization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly struct ServiceSummary(ServiceAttribute attribute, Type service)
        {
            public readonly ServiceAttribute attribute = attribute;
            public readonly Type service = service;
            public readonly List<MethodSummary<BeforeServiceInitializedAttribute>> preload = [];
            public readonly List<MethodSummary<AfterServiceInitializedAttribute>> afterload = [];
        }

        private readonly struct MethodSummary<T>(T attribute, MethodInfo method) where T : Attribute
        {
            public readonly T attribute = attribute;
            public readonly MethodInfo method = method;
        }

        private readonly struct LoadingContext()
        {
            public readonly List<ServiceSummary> Services = [];
            public readonly List<MethodSummary<BeforeServiceInitializedAttribute>> Preload = [];
            public readonly List<MethodSummary<AfterServiceInitializedAttribute>> Afterload = [];
            public readonly Dictionary<Type, ServiceSummary> Mapping = [];
        }

        private static async UniTask UnloadInternal(IEnumerable<ILoadingSource> sources, ITerminationArgs args)
        {
            await UniTask.CompletedTask;
            throw new NotSupportedException("Partial termination is not supported yet.");
        }

        private static async UniTask LoadInternal(IEnumerable<ILoadingSource> sources, IInitializationArgs args)
        {
            // TODO: Avoid context allocation if all input assemblies are loaded.
            LoadingContext context = new();

            // Extracts all important information in all assemblies.
            foreach (ILoadable? source in sources.SelectMany(s => s.GetLoadables()))
            {
                if (source is not LoadableAssemblyReference reference)
                {
                    // Here we should load-in assemblies from the disk, for example, and stuff like that.
                    // We might improve on the pattern, because now we will need to change Engine.cs with this one, and devs should have power to change it as well.
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Loadable type of ({source.GetType().Name}) is not supported.");
                    continue;
                }

                if (!m_Assemblies.Register(reference.assembly))
                {
                    ServiceCoreLogger.LogWarning($"Skipping already initialized assemblies.");
                    continue;
                }

                await Extract(reference.assembly, context);
            }

            await ConstructServices(context);
            await InitializeServices(context, args);
        }

        /// <summary>
        /// Extracts all important information from an <paramref Identifier="source"/> to the <paramref Identifier="context"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Throws when <paramref Identifier="source"/> is not amongst <see cref="NativeAssemblies"/>.</exception>
        private static async UniTask Extract(Assembly assembly, LoadingContext context)
        {
            if (!IsNative(assembly))
            {
                throw new NotSupportedException("You can't load assemblies not from current AppDomain yet.");
            }

            // TODO: Support services defined inside other classes (?)
            List<ServiceSummary> services = context.Services;
            List<MethodSummary<BeforeServiceInitializedAttribute>> preload = context.Preload;
            List<MethodSummary<AfterServiceInitializedAttribute>> afterload = context.Afterload;
            foreach (Type? type in assembly.GetTypes())
            {
                // TODO: Benchmark if storing typeof result in stack is more optimized.
                // TODO: Also benchmark which order of checks is more performant.
                if (!type.IsAbstract && type.IsDefined(typeof(ServiceAttribute)))
                {
                    if (typeof(IService).IsAssignableFrom(type))
                    {
                        ServiceAttribute attribute = type.GetCustomAttribute<ServiceAttribute>(inherit: false);
                        // TODO: Add prioritizing, based on length of the inheritance tree, maybe?
                        //  Or maybe throw if there are two service declarations for the same type within one source?
                        //  So source loading order can be enforced.
                        //  What about "Service selection", when you can select a service to use from a menu and such?
                        //  So many things to think about...
                        services.Add(new(attribute, service: type));
                    }
                    else
                    {
                        ServiceCoreLogger.LogError($"Class ({type.Name}) defines {nameof(ServiceAttribute)} but does not implement {nameof(IService)}<>!");
                    }
                }

                foreach (var method in type.GetMethods())
                {
                    if (!method.IsStatic) continue;
                    foreach (var attribute in method.GetCustomAttributes<BeforeServiceInitializedAttribute>(inherit: false))
                    {
                        preload.Add(new MethodSummary<BeforeServiceInitializedAttribute>(attribute, method));
                    }

                    foreach (var attribute in method.GetCustomAttributes<AfterServiceInitializedAttribute>(inherit: false))
                    {
                        afterload.Add(new MethodSummary<AfterServiceInitializedAttribute>(attribute, method));
                    }
                }
            }

            await UniTask.CompletedTask;
        }

        private static async UniTask ConstructServices(LoadingContext context)
        {
            List<ServiceSummary> services = context.Services;

            // How much more space to reserve in dictionary for associations with the same services.
            const int ResizeSafetyMargin = 2;
            context.Mapping.EnsureCapacity(services.Count * ResizeSafetyMargin);

            foreach (ServiceSummary summary in services)
            {
                // Note: would of been nice to make mapping of m_RuntimeServices and class activation execute it
                // in parallel with passes below, in a background thread.
                // Maybe by adding some kind of internal temporary reference table?
                // 
                // Right now activation is synced with a main thread, but it doesn't have to.
                // This code will be moved to background thread later.
                // You should use EngineService Initialize for executing code on a main thread instead.

                // TODO: Instead of using mappings:
                // 1. Enlist all services.
                // 1.1. Services which override other services have to be removed.
                // 2. Enlist all their ServiceDescriptors.
                // 3. With HashMap, make sure that only the newest services remains.
                // 4. Make sure to remove services, descriptors of which were completely removed from the list.
                // 5. Initialize services using their descriptors, based on initialization order associated with a class defining them.
                IService service = (IService)Activator.CreateInstance(summary.service);
                Services.Unsafe.Set(service); // TODO: Terminate service on overwriting.

                // Registers all associations with current service.
                Type[] associations = service.Descriptor;
                for (int j = 0; j < associations.Length; j++)
                {
                    context.Mapping[associations[j]] = summary;
                }
            }

            await UniTask.CompletedTask;
        }

        private static async UniTask InitializeServices(LoadingContext context, IInitializationArgs args)
        {
            // TODO: Make it independent enough so we can use this method for custom service initialization.
            //  and make it schedulable.
            List<ServiceSummary> services = context.Services;
            var preload = context.Preload;
            var afterload = context.Afterload;
            var mapping = context.Mapping;
            using (Services.Unsafe.Initialize())
            {
                // No reason to parallelize this one - it will just create unnecessary overhead.
                // We will have at max 50-100 services with mods, I assume   - Dark
                // (Note: I wonder if it will even work in WebGL XD   - Dark)
                // TODO: Sort the array based on execution mode and iterate through it in 3 branchless passes.
                int before = 0, normal = 0, after = 0;
                ServiceSummary[] buffer = [.. services]; // We need an array later, so why not form and use it earlier?
                for (int i = 0; i < buffer.Length; i++)
                {
                    switch (buffer[i].attribute.ExecutionMode)
                    {
                        case ThreadExecutionMode.MainThread: normal++; break;
                        case ThreadExecutionMode.ThreadedBeforeMain: before++; break;
                        case ThreadExecutionMode.ThreadedAfterMain: after++; break;
                    }
                }

                // Adds all methods to a referenced services.
                // Also creates instances of the services (Note: because of that .ctor of services are not thread-safe)
                await UniTask.WhenAll(
                    UniTask.Run(() =>
                    {
                        foreach (MethodSummary<BeforeServiceInitializedAttribute> callback in preload)
                        {
                            // Note: 'TryGetValue' checks are mandatory, as some of the MethodSummaries might reference removed service.
                            // TODO: Move all references attached to an removed service to its replacement, somehow.
                            // (Maybe provide service map to the 'LoadServices' after all, and link multiple types to the same service? Account for multiple replacing)
                            if (mapping.TryGetValue(callback.attribute.Service, out var summary))
                            {
                                summary.preload.Add(callback);
                            }
                        }
                    }),

                    UniTask.Run(() =>
                    {
                        foreach (MethodSummary<AfterServiceInitializedAttribute> callback in afterload)
                        {
                            // Note: 'TryGetValue' checks are mandatory, as some of the MethodSummaries might reference removed service.
                            // TODO: Move all references attached to an removed service to its replacement, somehow.
                            // (Maybe provide service map to the 'LoadServices' after all, and link multiple types to the same service? Account for multiple replacing)
                            if (mapping.TryGetValue(callback.attribute.Service, out var summary))
                            {
                                summary.afterload.Add(callback);
                            }
                        }
                    })
                );

                // Note: 'preload' and 'afterload' lists should NOT be used with m_RuntimeServices after this section without TryGetValue checks.
                // Some of the MethodSummaries might reference a non-existing service.
                // Use 'ServiceSummary.preload' and 'ServiceSummary.afterload' from 'summaries' or 'mapping' instead.
                preload.Clear();
                afterload.Clear();

                // Sorts everything by the execution/initialization order.
                await UniTask.WhenAll(
                    UniTask.Run(() => Array.ForEach(buffer, s => s.preload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder)))),
                    UniTask.Run(() => Array.ForEach(buffer, s => s.afterload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder)))),
                    UniTask.Run(() => services.Sort((a, b) => a.attribute.ExecutionOrder.CompareTo(b.attribute.ExecutionOrder)))
                );

                // Updates summaries with sorted data.
                services.CopyTo(buffer);

                // Executed thread-safe initializations and callbacks before main thread.
                await RunThreadedInitialization(services, buffer, before, ThreadExecutionMode.ThreadedBeforeMain, args);

                // Initialization part on a Main Unity thread.
                if (normal > 0)
                {
                    foreach (ServiceSummary summary in services)
                    {
                        if (summary.attribute.ExecutionMode != ThreadExecutionMode.MainThread) continue;
                        summary.preload.ForEach(m => m.method.Invoke(null, null));
                        await Services.Map[summary.service].Service.InvokeInitialize(args);
                        summary.afterload.ForEach(m => m.method.Invoke(null, null));
                    }
                }

                // Executed thread-safe initializations and callbacks after main thread.
                await RunThreadedInitialization(services, buffer, after, ThreadExecutionMode.ThreadedAfterMain, args);

                // Simplifications:
                static async UniTask RunThreadedInitialization(List<ServiceSummary> services, ServiceSummary[] buffer, int allocation, ThreadExecutionMode mode, IInitializationArgs args)
                {
                    // Runs services that are thread-safe and should be executed before main thread in parallel.
                    // Note: using m_RuntimeServices[ServiceSummary.service] here should never produce an exception.
                    //  I believe this is ensured by filtering in 'LoadServices' method.   - Dark
                    // Note #2: Down the line, we can group executions by the order:
                    // - Services with the same execution order will execute in parallel.
                    // - And services in different groups will be executed sequentially.
                    // Because as of right now, execution order on threaded services is ignored.
                    if (allocation <= 0)
                    {
                        return;
                    }

                    int buffered = 0;
                    foreach (var set in services)
                    {
                        if (set.attribute.ExecutionMode == mode)
                        {
                            buffer[buffered++] = set;
                            if (buffered >= allocation) break;
                        }
                    }

                    // Runs non-thread-safe 'preload' method callbacks.
                    for (int i = 0; i < buffered; i++)
                    {
                        buffer[i].preload.ForEach(static pre =>
                        {
                            if (!pre.attribute.ThreadSafe) pre.method.Invoke(null, null);
                        });
                    }

                    // Executes all thread-safe methods and handlers in a right order.
                    UniTask[] tasks = new UniTask[buffered];
                    for (int i = 0; i < buffered; i++)
                    {
                        tasks[i] = UniTask.Run(async () =>
                        {
                            var set = buffer[i];
                            set.preload.ForEach(static pre =>
                            {
                                if (pre.attribute.ThreadSafe) pre.method.Invoke(null, null);
                            });

                            await Services.Map[set.service].Service.InvokeInitialize(args);
                            set.afterload.ForEach(static after =>
                            {
                                if (after.attribute.ThreadSafe) after.method.Invoke(null, null);
                            });
                        });
                    }

                    await UniTask.WhenAll(tasks);

                    // Runs non-thread-safe 'afterload' method callbacks.
                    for (int i = 0; i < buffered; i++)
                    {
                        buffer[i].afterload.ForEach(static after =>
                        {
                            if (!after.attribute.ThreadSafe) after.method.Invoke(null, null);
                        });
                    }
                }
            }
        }
    }
}
