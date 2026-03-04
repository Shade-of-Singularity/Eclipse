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
using System.Threading;

namespace ServiceCore
{
    /// <summary>
    /// Main class for <see cref="ServiceCore"/> Library.
    /// </summary>
    /// TODO: Make the entire engine transactional (i.e. revert back if failure happens anywhere),
    /// and remove <see cref="EngineStatus.InitializationBroken"/> and <see cref="EngineStatus.TerminationBroken"/>.
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
        /// TODO: Should we remove all callbacks on engine termination? Callback lifespan is something to consider in the future.
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
        public static IReadOnlyCollection<Assembly> NativeAssemblies => m_NativeAssemblies;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly EngineState m_State = new(EngineStatus.Terminated); // Starts as terminated.
        private static readonly HashSet<Assembly> m_NativeAssemblies = new(AssemblyOrdinalComparer.Default);
        private static readonly AssemblyStorage m_Assemblies = new(64);




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
            // TODO: Decide what to do with summary unloading when in the Editor.
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
                // TODO: Remove allocation if needed.
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
            var assemblies = m_NativeAssemblies;
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
        private sealed class AssemblyOrdinalComparer : IEqualityComparer<Assembly>
        {
            public static readonly AssemblyOrdinalComparer Default = new();

            /// <inheritdoc/>
            public bool Equals(Assembly x, Assembly y) => StringComparer.Ordinal.Equals(x.FullName, y.FullName);

            /// <inheritdoc/>
            public int GetHashCode(Assembly assembly) => StringComparer.Ordinal.GetHashCode(assembly.FullName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNative(Assembly assembly) => m_NativeAssemblies.Contains(assembly);
        private static void SetStatus(EngineStatus status)
        {
            EngineStatus diff = (Status ^ status) & status; // Checks which bits have changed.
            m_State.Status = status;

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
            var local = Interlocked.Exchange(ref callbacks, null);
            if (local is null) return true;

            // Callback list should not be modifiable at this point, since after IsInitialized is set to true - callbacks are auto fired immediately.
            // Because of that, we don't need any locks, AFAIK.
            bool exceptions = false;
            foreach (var callback in local.GetInvocationList())
            {
                // Note: Consider moving away from per-source invocation.
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
        private struct LoadingContext()
        {
            public readonly List<MethodSummary<BeforeServiceInitializedAttribute>> Preload = [];
            public readonly List<MethodSummary<AfterServiceInitializedAttribute>> Afterload = [];
            /// <summary>
            /// Stores <see cref="ServiceDescriptor"/>s that will be actively used during initialization.
            /// </summary>
            public readonly Dictionary<ServiceDescriptor, ServiceSummary> ActiveRange = [];
            /// <summary>
            /// Instantiated services.
            /// </summary>
            public ServiceSummary[] Services = [];
        }

        /// <summary>
        /// .ctor for a first initialization.
        /// </summary>
        private sealed class ServiceSummary(Type service, ServiceRange range) : IEquatable<ServiceSummary>
        {
            public Type Type = service;
            public ServiceRange Range = range;
            public ServiceAttribute Attribute = null!;

            // Fields below are supplied at full initialization.
            public IService Instance = null!;
            public List<MethodSummary<BeforeServiceInitializedAttribute>> Preload = null!;
            public List<MethodSummary<AfterServiceInitializedAttribute>> Afterload = null!;

            public void InitializeAttribute() => Attribute = Type.GetCustomAttribute<ServiceAttribute>(inherit: false);
            public void InitializeMapping(IService instance)
            {
                Instance = instance;
                Preload = [];
                Afterload = [];
            }

            /// <inheritdoc/>
            public override string ToString() => $"{Type.FullName} (Preload: {Preload?.Count}) (Afterload: {Afterload?.Count})";

            /// <inheritdoc/>
            public override bool Equals(object obj) => obj is ServiceSummary summary && Equals(summary);

            /// <inheritdoc/>
            public bool Equals(ServiceSummary other) => other.Type == Type;

            /// <inheritdoc/>
            public override int GetHashCode() => Type.GetHashCode();
        }

        private readonly struct MethodSummary<T>(T attribute, MethodInfo method) where T : Attribute
        {
            public readonly T attribute = attribute;
            public readonly MethodInfo method = method;
        }

        private static async UniTask UnloadInternal(IEnumerable<ILoadingSource> sources, ITerminationArgs args)
        {
            await UniTask.CompletedTask;
            throw new NotSupportedException("Partial termination is not supported yet.");
        }

        private static async UniTask LoadInternal(IEnumerable<ILoadingSource> sources, IInitializationArgs args)
        {
            // TODO: Avoid context allocation if all loaded input assemblies are the same.
            LoadingContext context = new();

            // Extracts all important information in all assemblies.
            foreach (ILoadable? source in sources.SelectMany(s => s.GetLoadables()))
            {
                if (source is not LoadableAssemblyReference reference)
                {
                    // Here we should load-in assemblies from the disk, for example, and stuff like that.
                    // We might improve on the pattern, because now we will need to change Engine.cs with this one, and devs should have power to change it as well.
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Loadable Type of ({source.GetType().Name}) is not supported.");
                    continue;
                }

                if (!m_Assemblies.Register(reference.assembly))
                {
                    ServiceCoreLogger.LogWarning($"Skipping already initialized assemblies.");
                    continue;
                }

                Extract(reference.assembly, context);
            }

            context.Services = ConstructServices(context);
            await InitializeServices(context, args);
        }

        /// <summary>
        /// Extracts all important information from an <paramref Identifier="source"/> to the <paramref Identifier="context"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Throws when <paramref Identifier="source"/> is not amongst <see cref="NativeAssemblies"/>.</exception>
        private static void Extract(Assembly assembly, in LoadingContext context)
        {
            if (!IsNative(assembly))
            {
                throw new NotSupportedException("You can't load assemblies not from current AppDomain yet.");
            }

            // TODO: Support services defined inside other classes (?)
            Dictionary<ServiceDescriptor, ServiceSummary> active = context.ActiveRange;
            List<MethodSummary<BeforeServiceInitializedAttribute>> preload = context.Preload;
            List<MethodSummary<AfterServiceInitializedAttribute>> afterload = context.Afterload;
            foreach (Type? type in assembly.GetTypes())
            {
                // TODO: Benchmark if storing typeof result in stack is more optimized.
                // TODO: Also benchmark which order of checks is more performant.
                if (!type.IsAbstract && type.IsDefined(typeof(ServiceAttribute)))
                {
                    ServiceRange range = ServiceRanges.Retrieve(type);
                    if (ServiceRange.Invalid.Equals(range))
                    {
                        ServiceCoreLogger.LogError($"{LogPrefix} Service ({type.Name}) declares {nameof(ServiceAttribute)}, but doesn't implement any valid {nameof(IService)} base definition.");
                        continue;
                    }

                    var summary = new ServiceSummary(type, range);
                    var array = range.Descriptors;
                    for (int i = 0; i < array.Length; i++)
                    {
                        // Note: ServiceAttribute will be retrieved later.
                        // This way we can completely override a summary first.
                        active[array[i]] = summary;
                    }
                }

                // Note: maybe introduce anonymous reports to see how system behaves and where we can optimize it?
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (!method.IsDefined(typeof(ServiceInitializationAttribute), inherit: false)) continue;
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
        }

        private static ServiceSummary[] ConstructServices(in LoadingContext context)
        {
            HashSet<ServiceSummary> unique = new(context.ActiveRange.Count);
            foreach (var service in context.ActiveRange.Values)
            {
                unique.Add(service);
            }

            ServiceSummary[] services = new ServiceSummary[unique.Count];
            unique.CopyTo(services);
            for (int i = 0; i < services.Length; i++)
            {
                services[i].InitializeAttribute();
            }

            // TODO: Sort based on ILoadingSources first, and then by internal ordering itself.
            Array.Sort(services, (a, b) => a.Attribute.ExecutionOrder.CompareTo(b.Attribute.ExecutionOrder));
            for (int i = 0; i < services.Length; i++)
            {
                ServiceSummary summary = services[i];
                IService service = (IService)Activator.CreateInstance(summary.Type);
                Services.Unsafe.Set(service);

                // Fully initializes a summary.
                summary.InitializeMapping(service);

                // Sets the instance value.
                // TODO: Make sure that only overwritten descriptors will be set, probably by reversing ActiveRange dictionary.
                ServiceDescriptor[] range = summary.Range.Descriptors;
                for (int j = 0; j < range.Length; j++)
                {
                    range[j].Setter(service);
                }
            }

            return services;

            //foreach (ServiceSummary summary in services)
            //{
            //    // Note: would of been nice to make mapping of m_RuntimeServices and class activation execute it
            //    // in parallel with passes below, in a background thread.
            //    // Maybe by adding some kind of internal temporary reference table?
            //    // 
            //    // Right now activation is synced with a main thread, but it doesn't have to.
            //    // This code will be moved to background thread later.
            //    // You should use EngineService Initialize for executing code on a main thread instead.

            //    // TODO: Instead of using mappings:
            //    // 1. Enlist all services.
            //    // 1.1. Services which override other services have to be removed.
            //    // 2. Enlist all their ServiceDescriptors.
            //    // 3. With HashMap, make sure that only the newest services remains.
            //    // 4. Make sure to remove services, descriptors of which were completely removed from the list.
            //    // 5. Initialize services using their descriptors, based on initialization order associated with a class defining them.
            //    IService summary = (IService)Activator.CreateInstance(summary.summary);
            //    Services.Unsafe.Set(summary); // TODO: Terminate summary on overwriting.

            //    // Registers all associations with current summary.
            //    Type[] associations = summary.Descriptor;
            //    for (int j = 0; j < associations.Length; j++)
            //    {
            //        context.Mapping[associations[j]] = summary;
            //    }
            //}
        }

        private static async UniTask InitializeServices(LoadingContext context, IInitializationArgs args)
        {
            // TODO: Make it independent enough so we can use this method for custom summary initialization.
            //  and make it schedulable.
            ServiceSummary[] services = context.Services;
            var preload = context.Preload;
            var afterload = context.Afterload;
            var mapping = context.ActiveRange;
            using (Services.Unsafe.Initialize())
            {
                // No reason to parallelize this one - it will just create unnecessary overhead.
                // We will have at max 50-100 services with mods, I assume   - Dark
                // (Note: I wonder if it will even work in WebGL XD   - Dark)
                // TODO: Sort the array based on execution mode and iterate through it in 3 branchless passes.
                int before = 0, normal = 0, after = 0;
                for (int i = 0; i < services.Length; i++)
                {
                    switch (services[i].Attribute.ExecutionMode)
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
                            // TODO: Remove array look-up by caching a first element, if needed.
                            var descriptor = ServiceRanges.Retrieve(callback.attribute.Service).First;
                            if (descriptor is null) continue;
                            if (mapping.TryGetValue(descriptor, out ServiceSummary summary))
                            {
                                summary.Preload.Add(callback);
                            }
                        }
                    }),

                    UniTask.Run(() =>
                    {
                        foreach (MethodSummary<AfterServiceInitializedAttribute> callback in afterload)
                        {
                            // TODO: Remove array look-up by caching a first element, if needed.
                            var descriptor = ServiceRanges.Retrieve(callback.attribute.Service).First;
                            if (descriptor is null) continue;
                            if (mapping.TryGetValue(descriptor, out ServiceSummary summary))
                            {
                                summary.Afterload.Add(callback);
                            }
                        }
                    })
                );

                // Note: 'preload' and 'afterload' lists should NOT be used with m_RuntimeServices after this section without TryGetValue checks.
                // Some of the MethodSummaries might reference a non-existing summary.
                // Use 'ServiceSummary.preload' and 'ServiceSummary.afterload' from 'summaries' or 'mapping' instead.
                preload.Clear();
                afterload.Clear();

                // Sorts everything by the execution/initialization order.
                // TODO: Also order it in 3 sections: [preload][main][afterload].
                await UniTask.WhenAll(
                    UniTask.Run(() => Array.ForEach(services, static s => s.Preload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder)))),
                    UniTask.Run(() => Array.ForEach(services, static s => s.Afterload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder))))
                );

                // Executed thread-safe initializations and callbacks before main thread.
                if (before > 0)
                {
                    await RunThreadedInitialization(services, before, ThreadExecutionMode.ThreadedBeforeMain, args);
                }

                // Initialization part on a Main Unity thread.
                if (normal > 0)
                {
                    for (int i = 0; i < services.Length; i++)
                    {
                        var summary = services[i];
                        if (summary.Attribute.ExecutionMode == ThreadExecutionMode.MainThread)
                        {
                            summary.Preload.ForEach(m => m.method.Invoke(null, null));
                            await summary.Instance.InvokeInitialize(args);
                            summary.Afterload.ForEach(m => m.method.Invoke(null, null));
                        }
                    }
                }

                // Executed thread-safe initializations and callbacks after main thread.
                if (after > 0)
                {
                    await RunThreadedInitialization(services, after, ThreadExecutionMode.ThreadedAfterMain, args);
                }

                // Simplifications:
                static async UniTask RunThreadedInitialization(ServiceSummary[] services, int allocation, ThreadExecutionMode mode, IInitializationArgs args)
                {
                    // Checks for the amount of services with current thread execution mode.
                    if (allocation <= 0)
                    {
                        return;
                    }

                    const int StackAllocationThreshold = 32;
                    Span<int> targets = allocation < StackAllocationThreshold ? stackalloc int[allocation] : new int[allocation];
                    int head = 0;
                    for (int i = 0; i < services.Length; i++)
                    {
                        if (services[i].Attribute.ExecutionMode == mode)
                            targets[head++] = i;
                    }

                    // Runs non-thread-safe 'preload' method callbacks.
                    foreach (var target in targets)
                    {
                        services[target].Preload.ForEach(static pre =>
                        {
                            if (!pre.attribute.ThreadSafe) pre.method.Invoke(null, null);
                        });
                    }

                    // Executes all thread-safe methods and handlers in a right order.
                    UniTask[] tasks = new UniTask[allocation];
                    for (int i = 0; i < allocation; i++)
                    {
                        var target = targets[i];
                        tasks[i] = UniTask.Run(async () =>
                        {
                            ServiceSummary summary = services[target];
                            summary.Preload.ForEach(static pre =>
                            {
                                if (pre.attribute.ThreadSafe) pre.method.Invoke(null, null);
                            });

                            await summary.Instance.InvokeInitialize(args);
                            summary.Afterload.ForEach(static after =>
                            {
                                if (after.attribute.ThreadSafe) after.method.Invoke(null, null);
                            });
                        });
                    }

                    await UniTask.WhenAll(tasks);

                    // Runs non-thread-safe 'afterload' method callbacks.
                    foreach (var target in targets)
                    {
                        services[target].Afterload.ForEach(static after =>
                        {
                            if (!after.attribute.ThreadSafe) after.method.Invoke(null, null);
                        });
                    }
                }
            }
        }
    }
}
