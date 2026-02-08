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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Main class for <see cref="Eclipse"/> Foundation Library
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
        public const string LogPrefix = "[" + nameof(Eclipse) + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Event that is fired when <see cref="Status"/> is set <see cref="EngineStatus.Initialized"/>
        /// </summary>
        /// <remarks>
        /// Callback list is cleared after initialization.
        /// This implies that you should only use this callback before calling <see cref="Initialize"/>, or inside <see cref="Service.Initialize"/>.
        /// <para>
        /// To get consistent callback, you can use <see cref="EclipseInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action OnEngineInitialized
        {
            remove => m_OnEngineInitialized -= value;
            add
            {
                if (value == null) return;
                if (m_Status == EngineStatus.Initialized)
                {
                    value.Invoke();
                    return;
                }

                m_OnEngineInitialized += value;
            }
        }

        /// <summary>
        /// Called when every existing instance of <see cref="IService"/> and similar is fully unloaded. (e.g. on <see cref="Terminate()"/>)
        /// <para>
        /// Used to reset static references to the old services and configuration classes, as to prevent memory leaks on mod reloading.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Callback list is cleared after engine reset.
        /// This implies that you should only use this callback before calling <see cref="Terminate"/>, or inside <see cref="IService.Terminate"/>.
        /// <para>
        /// To get consistent callback, you can use <see cref="EclipseInitializeAttribute"/> on custom static methods.
        /// </para>
        /// </remarks>
        public static event Action? OnEngineTerminated
        {
            remove => m_OnEngineTerminated -= value;
            add
            {
                if (value == null) return;
                if (m_Status == EngineStatus.Terminated)
                {
                    value.Invoke();
                    return;
                }

                m_OnEngineTerminated += value;
            }
        }

        // Properties
        /// <summary>
        /// Status of the engine.
        /// </summary>
        /// <remarks>
        /// <para>Set to <see cref="EngineStatus.Terminated"/> - by default.</para>
        /// <para>Set to <see cref="EngineStatus.Initializing"/> - during initialization (after calling <see cref="Initialize"/>, potentially automatically).</para>
        /// <para>Set to <see cref="EngineStatus.Initialized"/> - when <see cref="Engine"/> and <see cref="Modding.Mod"/>s are fully initialized!</para>
        /// <para>Set to <see cref="EngineStatus.Terminating"/> - during unloading (after calling <see cref="Terminate"/>, maybe by <see cref="QuitHandler"/>)</para>
        /// <para>Set to <see cref="EngineStatus.InitializationBroken"/> - if engine got irreversibly broken during initialization.</para>
        /// <para>Set to <see cref="EngineStatus.TerminationBroken"/> - if engine got irreversibly broken during unloading.</para>
        /// </remarks>
        public static EngineStatus Status
        {
            get => m_Status;
            private set
            {
                bool exceptions;
                Delegate[] delegates;
                switch (m_Status = value)
                {
                    //
                    // Temporary states are ignored.
                    //
                    case EngineStatus.Initializing: break;
                    case EngineStatus.Terminating: break;

                    //
                    // Unknown states are immediately reported.
                    //
                    default: throw new SwitchExpressionException(value);

                    //
                    // Final states are processed.
                    //
                    case EngineStatus.Initialized:
                        if (m_OnEngineInitialized == null) break;
                        delegates = m_OnEngineInitialized.GetInvocationList();
                        m_OnEngineInitialized = null;

                        // Callback list should not be modifiable at this point, since after IsInitialized is set to true - callbacks are auto fired immediately.
                        // Because of that, we don't need any locks, AFAIK.
                        exceptions = false;
                        foreach (var callback in delegates)
                        {
                            try
                            {
                                callback?.DynamicInvoke();
                            }
                            catch (Exception ex)
                            {
                                EclipseLogger.LogException(ex);
                                exceptions |= true;
                            }
                        }

                        if (exceptions)
                        {
                            EclipseLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineInitialized)}' thrown exceptions! Look above for errors.");
                        }

                        break;

                    case EngineStatus.Terminated:
                        if (m_OnEngineTerminated == null) break;
                        delegates = m_OnEngineTerminated.GetInvocationList();
                        m_OnEngineTerminated = null;

                        // Callback list should not be modifiable at this point, since after IsInitialized is set to true - callbacks are auto fired immediately.
                        // Because of that, we don't need any locks, AFAIK.
                        exceptions = false;
                        foreach (var callback in delegates)
                        {
                            try
                            {
                                callback?.DynamicInvoke();
                            }
                            catch (Exception ex)
                            {
                                EclipseLogger.LogException(ex);
                                exceptions |= true;
                            }
                        }

                        if (exceptions)
                        {
                            EclipseLogger.LogError($"{LogPrefix} Some callbacks in '{nameof(OnEngineTerminated)}' thrown exceptions! Look above for errors.");
                        }

                        break;

                    //
                    // Broken states are reported:
                    //
                    case EngineStatus.InitializationBroken:
                        // TODO: Replace with EngineLogger implementation.
                        EclipseLogger.LogError($"{LogPrefix} {nameof(Engine)} was irreversibly broken during initialization. You will need to restart your app to fix this.");
                        break;

                    case EngineStatus.TerminationBroken:
                        // TODO: Replace with EngineLogger implementation.
                        EclipseLogger.LogError($"{LogPrefix} {nameof(Engine)} was irreversibly broken during unloading. You will need to restart your app to fix this.");
                        break;
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Encapsulated Fields:
        private static volatile EngineStatus m_Status = EngineStatus.Terminated;
        private static volatile Action? m_OnEngineInitialized;
        private static volatile Action? m_OnEngineTerminated;

        // Local Fields:
        private static readonly AssemblyStorage m_Assemblies = new AssemblyStorage(64);
        private static volatile bool m_AcceptsAssemblies = true;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                       Unity Initialization Callbacks
        /// .                                TODO: Add Editor-time initialization methods.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnSubsystemRegistration()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.SubsystemRegistration)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnAfterAssembliesLoaded()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.AfterAssembliesLoaded)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.BeforeSplashScreen)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.BeforeSceneLoad)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.AfterSceneLoad)
            {
                Initialize().Forget();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Fast-Access API
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Throws "Not modifiable" exception if called when <see cref="Status"/> is anything but <see cref="EngineStatus.Initializing"/>.
        /// This is usually when core systems are still modifiable.
        /// Assets and other resources, however, usually still modifiable at runtime to some degree (depends on type).
        /// </summary>
        /// <remarks>
        /// (Hmm... Is it a good idea to lock systems behind such limitation though?)
        /// <para>
        /// (Use it in a non-performance critical code, like class setters that usually never called, or initialization-only setters, etc.)
        /// </para>
        /// </remarks>
        public static void AssertModifiable([CallerFilePath] string caller = "")
        {
            if (m_Status != EngineStatus.Initializing)
            {
                throw new Exception($"Cannot modify ('{Path.GetFileNameWithoutExtension(caller)}') outside of the engine initialization stage.");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Unloading
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Unloads entire engine, all initialized services.
        /// </summary>
        /// <remarks>
        /// Will not unload mod Assemblies from the memory, as it is impossible.
        /// </remarks>
        public static async UniTask Terminate()
        {
            // Only already initialized engine can be unloaded.
            // (TODO) Note: should we introduce unloading of a partially loaded engine? Something to think about later.
            if (Status != EngineStatus.Initialized)
            {
                return;
            }

            // TODO: Hold callers in await block until engine is fully unloaded.
            using (Services.Unsafe.Terminate())
            {
                Status = EngineStatus.Terminating;
                foreach (var service in Services.List)
                {
                    // TODO: Terminate asynchronously if possible.
                    await service.InvokeTerminate();
                }
            }

            m_Assemblies.Clear();
            m_AcceptsAssemblies = true;
            await UniTask.CompletedTask;
            Status = EngineStatus.Terminated;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Initialization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initializes the entire engine: <see cref="IService"/>s, <see cref="Modding.Mod"/>s, and so on.
        /// </summary>
        public static async UniTask Initialize()
        {
            if (Status != EngineStatus.Terminated)
            {
                return;
            }

            Status = EngineStatus.Initializing;
            // TODO: Decide what to do with service unloading when in the Editor.
            //  Maybe provide special UNITY_EDITOR-only methods?
            //  We can keep them in the code so people can restore Editor's tools more easily.
            //  Although, a lot of it will be gate-kept behind Application.isEditor anyway.
            //Application.quitting += ResetState;

            if (Application.isEditor)
            {
                Debug.LogWarning($"Engine initializes in the Editor. Application.isPlaying: {Application.isPlaying}");
            }
            else
            {
                Debug.LogWarning($"Engine initializes at Runtime. Application.isPlaying: {Application.isPlaying}");
            }

            await LoadModsAndTheirAssemblies();
            await InitializeEngine();
            Status = EngineStatus.Initialized;
        }

        private static async UniTask LoadModsAndTheirAssemblies()
        {
            Debug.Log($"{LogPrefix} Executing '{nameof(LoadModsAndTheirAssemblies)}'");

            try
            {
                // Adds core assembly to the initialization root.
                EnqueueAssemblies(Assembly.GetAssembly(typeof(Engine)));

                // Tries to load editor-defined assemblies.
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var name = assembly.GetName().Name;
                    foreach (var expectation in EclipseConfiguration.Instance.TargetAssemblyNames)
                    {
                        if (string.Equals(name, expectation, StringComparison.Ordinal))
                        {
                            EnqueueAssemblies(assembly);
                            goto NextItem;
                        }
                    }

                    NextItem:
                    continue;
                }

                // (TODO) Analyzes and (TODO) loads-in mod's assemblies.
                // (Note: With BepInEx, here game will stop initialization, wait for the UI to load-in, and will warn player about the danger of BepInEx modding (?))
                // Hope people wont get too scared, but modding support was all made for the security purposes.
                if (Application.isMobilePlatform || Application.isConsolePlatform)
                {
                    Debug.LogWarning($"{LogPrefix} Just a note to you - modding is not supported on Mobile platforms and Console platforms yet.");
                }
                else
                {
                    Harmony_BeforeLoadingMods();

                    // TODO: Load-in all C# mods.
                    // TODO: Register their assemblies.
                    // TODO: Load-in all textures and other resources.
                    //
                    // TODO: Instead of service initialization order-based systems, additionally order method callbacks using mod dependency trees.
                    LoadModsAndAssemblies_LoadMods();
                    LoadModsAndAssemblies_RegisterModAssemblies();
                    LoadModsAndAssemblies_IndexAndLoadTexturesAndAtlases();
                    LoadModsAndAssemblies_IndexAndLoadResources();
                    // TODO: Do the same for the core of the game.
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LogPrefix} Mod registration on '{nameof(LoadModsAndTheirAssemblies)}' failed!");
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log($"{LogPrefix} Mod registration (on '{nameof(LoadModsAndTheirAssemblies)}') successful!");
            }

            Harmony_AfterLoadingMods();
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Can be used by mods to enqueue assemblies directly, if implementing them using provided tools is not for you.
        /// </summary>
        private static void Harmony_BeforeLoadingMods() { }
        private static void Harmony_AfterLoadingMods() { }
        private static void LoadModsAndAssemblies_LoadMods()
        {
            // Loads general information about the mods here.
        }

        private static void LoadModsAndAssemblies_RegisterModAssemblies()
        {
            // Add loading of the assemblies here (using safe compiler - all networking should use our methods).
        }

        private static void LoadModsAndAssemblies_IndexAndLoadTexturesAndAtlases()
        {
            // Loads texture atlases and makes them indexable for further use.
        }

        private static void LoadModsAndAssemblies_IndexAndLoadResources()
        {
            // Loads texture atlases and makes them indexable for further use.
        }

        private static async UniTask InitializeEngine()
        {
            // Note: async execution messes-up execution order. Account for that further on.
            Debug.Log($"{LogPrefix} Executing '{nameof(InitializeEngine)}'");
            m_AcceptsAssemblies = false;

            // Loads EngineService attributes.
            try
            {
#if UNITY_WEBGL
                Debug.LogWarning($"{LogName} Threaded initialization runs synchronously on WebGL. Long initialization time is to be expected.");
#endif
                // TODO: Both CPU and memory optimize the initialization.
                List<ServiceSummary>? services = new List<ServiceSummary>();

                // Note: 'preload'/'afterload' here refer to method attribute categories,
                // not the threaded execution mode ('BeforeMain' / 'AfterMain').   - Dark & GPT
                List<MethodSummary<BeforeServiceInitializedAttribute>> preload = new List<MethodSummary<BeforeServiceInitializedAttribute>>();
                List<MethodSummary<AfterServiceInitializedAttribute>> afterload = new List<MethodSummary<AfterServiceInitializedAttribute>>();

                // TODO: resolve initialization order from a mod dependency order.
                Assembly[] assemblies = m_Assemblies.ToArray();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    // Loads-in all the services. Also automatically removes replaced services.
                    LoadServices(assemblies[i], services, preload, afterload);
                }

                // Turn services into array, just so we can later sort the list and sort the method callbacks in parallel.
                // Gives a small performance benefit if done earlier. (Not benchmarked - intuition)   - Dark
                ServiceSummary[] summaries = services.ToArray();

                // Maps service types to the summaries.
                // Note: maybe 'LoadServices' can be optimized (specifically duplicate fetching) if we provide dictionary instead?
                //Dictionary<Type, ServiceSummary> mapping = summaries.ToDictionary(s => s.service);
                const float ResizeSafetyMargin = 1.75f; // How much more space to reserve in dictionary for associations with the same services.
                Dictionary<Type, ServiceSummary> mapping = new Dictionary<Type, ServiceSummary>(
                    capacity: Mathf.NextPowerOfTwo((int)(summaries.Length * ResizeSafetyMargin)));

                // Creates association between all parent classes with ServiceAttribute of replaced services, so for any of them child will be returned.
                using var initialization = Services.Unsafe.Initialize();
                for (int i = 0; i < summaries.Length; i++)
                {
                    // TODO: Test if service types are still valid.
                    ServiceSummary summary = summaries[i];
                    Type target = summary.service!;

                    // Note: would of been nice to make mapping of m_Services and class activation execute in parallel with passes below, in a background thread.
                    // Maybe by adding some kind of internal temporary reference table?
                    // 
                    // Right now activation is synced with a main thread, but it doesn't have to. This code will be moved to background thread later.
                    // You should use EngineService Initialize for executing code on a main thread instead.
                    IService service = (IService)Activator.CreateInstance(summary.service)!;
                    foreach (var declaration in summary.declarations)
                    {
                        mapping[declaration] = summary;
                        Services.Unsafe.Dictionary[declaration] = service;
                        RuntimeHelpers.RunClassConstructor(declaration.TypeHandle);
                    }

                    do
                    {
                        mapping[target] = summary;
                        Services.Unsafe.Dictionary[target] = service;
                        target = target.BaseType;
                    }
                    while (target is { } && target.GetInterface(nameof(IService)) != null);
                }

                // Binds before initializing.
                Services.Unsafe.Rebind();

                // No reason to parallelize this one - it will just create unnecessary overhead.
                // We will have at max 50-100 services with mods, I assume   - Dark
                // (Note: I wonder if it will even work in WebGL XD   - Dark)
                int before = summaries.Count(s => s.attribute.ExecutionMode == IService.ThreadExecutionMode.ThreadedBeforeMain);
                int after = summaries.Count(s => s.attribute.ExecutionMode == IService.ThreadExecutionMode.ThreadedAfterMain);
                int normal = summaries.Length - before - after;

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

                //, Desired result, but it was moved to a synchronous context at the moment. This code should be used instead at some point.
                //UniTask.Run(() =>
                //{
                //    /// Note: because of this pass .ctor initialization is NOT thread-safe! Only <see cref="EngineService.Initialize"/> is!
                //    for (int i = 0; i < summaries.Length; i++)
                //    {
                //        var set = summaries[i];
                //        m_Services[set.service] = (EngineService)Activator.CreateInstance(set.service);
                //    }
                //})
                );

                // Note: 'preload' and 'afterload' lists should NOT be used with m_Services after this section without TryGetValue checks.
                // Some of the MethodSummaries might reference a non-existing service.
                // Use 'ServiceSummary.preload' and 'ServiceSummary.afterload' from 'summaries' or 'mapping' instead.
                preload.Clear();
                afterload.Clear();

                // Sorts everything by the execution/initialization order.
                await UniTask.WhenAll(
                    UniTask.Run(() => Array.ForEach(summaries, s => s.preload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder)))),
                    UniTask.Run(() => Array.ForEach(summaries, s => s.afterload.Sort((a, b) => a.attribute.InvokeOrder.CompareTo(b.attribute.InvokeOrder)))),
                    UniTask.Run(() => services.Sort((a, b) => a.attribute.ExecutionOrder.CompareTo(b.attribute.ExecutionOrder)))
                );

                // Updates summaries with sorted data.
                services.CopyTo(summaries);
                services = null; // List itself should not be used after this point, as it is inefficient.

                // Executed thread-safe initializations and callbacks before main thread.
                await RunThreadedInitialization(before, IService.ThreadExecutionMode.ThreadedBeforeMain);

                // Initialization part on a Main Unity thread.
                if (normal > 0)
                {
                    foreach (ServiceSummary summary in summaries)
                    {
                        if (summary.attribute.ExecutionMode != IService.ThreadExecutionMode.MainThread) continue;
                        summary.preload.ForEach(m => m.method.Invoke(null, null));
                        await Services.Unsafe.Dictionary[summary.service].InvokeInitialize();
                        summary.afterload.ForEach(m => m.method.Invoke(null, null));
                    }
                }

                // Executed thread-safe initializations and callbacks after main thread.
                await RunThreadedInitialization(after, IService.ThreadExecutionMode.ThreadedAfterMain);

                // Simplifications:
                async UniTask RunThreadedInitialization(int expected, IService.ThreadExecutionMode mode)
                {
                    // Runs services that are thread-safe and should be executed before main thread in parallel.
                    // Note: using m_Services[ServiceSummary.service] here should never produce an exception.
                    //  I believe this is ensured by filtering in 'LoadServices' method.   - Dark
                    // Note #2: Down the line, we can group executions by the order:
                    // - Services with the same execution order will execute in parallel.
                    // - And services in different groups will be executed sequentially.
                    // Because as of right now, execution order on threaded services is ignored.
                    if (expected > 0)
                    {
                        ServiceSummary[] temp = new ServiceSummary[expected];
                        int stored = 0;
                        for (int i = 0; i < summaries.Length; i++)
                        {
                            var set = summaries[i];
                            if (set.attribute.ExecutionMode == mode)
                            {
                                temp[stored++] = set;
                                if (stored >= expected) break;
                            }
                        }

                        // Runs non-thread-safe 'preload' method callbacks.
                        for (int i = 0; i < stored; i++)
                        {
                            temp[i].preload.ForEach(c =>
                            {
                                if (!c.attribute.ThreadSafe) c.method.Invoke(null, null);
                            });
                        }

                        // Executes all thread-safe methods and handlers in a right order.
                        UniTask[] tasks = new UniTask[stored];
                        for (int i = 0; i < stored; i++)
                        {
                            tasks[i] = UniTask.Run(async () =>
                            {
                                var set = temp[i];
                                set.preload.ForEach(c =>
                                {
                                    if (c.attribute.ThreadSafe) c.method.Invoke(null, null);
                                });

                                await Services.Unsafe.Dictionary[set.service].InvokeInitialize();
                                set.afterload.ForEach(c =>
                                {
                                    if (c.attribute.ThreadSafe) c.method.Invoke(null, null);
                                });
                            });
                        }

                        await UniTask.WhenAll(tasks);

                        // Runs non-thread-safe 'afterload' method callbacks.
                        for (int i = 0; i < stored; i++)
                        {
                            temp[i].afterload.ForEach(c =>
                            {
                                if (!c.attribute.ThreadSafe) c.method.Invoke(null, null);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LogPrefix} Game and mod initialization on '{nameof(InitializeEngine)}' failed!");
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log($"{LogPrefix} Game and mod initialization (on '{nameof(InitializeEngine)}') successful!");
            }

            m_Assemblies.Clear();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void EnqueueAssemblies(Assembly? assembly)
        {
            if (assembly == null)
            {
                return;
            }

            if (!m_AcceptsAssemblies)
            {
                Debug.LogError($"{LogPrefix} Assembly was not loaded because it was queued after Engine read the queue. ('{assembly.FullName}')");
                return;
            }

            m_Assemblies.Register(assembly);
        }

        private static void LoadServices(
            Assembly assembly, List<ServiceSummary> services,
            List<MethodSummary<BeforeServiceInitializedAttribute>> preload,
            List<MethodSummary<AfterServiceInitializedAttribute>> afterload)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // TODO: Benchmark if storing typeof result in stack is more optimized.
                if (typeof(IService).IsAssignableFrom(type))
                {
                    if (!type.IsDefined(typeof(ServiceAttribute)))
                    {
                        continue;
                    }

                    ServiceAttribute attribute = type.GetCustomAttribute<ServiceAttribute>();
                    Type[] interfaces = type.FindInterfaces(Filter, null);
                    static bool Filter(Type type, object _)
                    {
                        // Allows interface declaration + generic IService<T> type declaration for better mapping.
                        return typeof(IService).IsAssignableFrom(type) && type != typeof(IService);
                    }

                    ServiceSummary summary = new ServiceSummary(attribute, type);
                    foreach (Type declaration in interfaces)
                    {
                        summary.declarations.Add(declaration);
                    }

                    Type target = summary.declarations.Find(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IService<>));
                    int index = services.FindIndex(s => s.declarations.Contains(target));
                    if (index == -1)
                    {
                        services.Add(summary);
                    }
                    else
                    {
                        EclipseLogger.LogWarning($"Replacing {services[index].service}.");
                        services[index] = summary;
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
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Structs
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly struct ServiceSummary
        {
            public readonly ServiceAttribute attribute;
            public readonly Type service;
            public readonly List<MethodSummary<BeforeServiceInitializedAttribute>> preload;
            public readonly List<MethodSummary<AfterServiceInitializedAttribute>> afterload;
            public ServiceSummary(ServiceAttribute attribute, Type service)
            {
                this.attribute = attribute;
                this.service = service;
                preload = new List<MethodSummary<BeforeServiceInitializedAttribute>>(0);
                afterload = new List<MethodSummary<AfterServiceInitializedAttribute>>(0);
            }
        }

        private readonly struct MethodSummary<T> where T : Attribute
        {
            public readonly T attribute;
            public readonly MethodInfo method;
            public MethodSummary(T attribute, MethodInfo method)
            {
                this.attribute = attribute;
                this.method = method;
            }
        }
    }
}
