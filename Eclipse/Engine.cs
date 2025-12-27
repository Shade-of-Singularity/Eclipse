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
using Eclipse.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public static class Engine
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Log name of this class for when it logs anything. Used for identifying what exactly warns you about an exception.
        /// </summary>
        public const string LogName = nameof(Eclipse);

        /// <summary>
        /// Same as <see cref="LogName"/> but this square braces.
        /// </summary>
        public const string LogNameBraced = "[" + LogName + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:
        /// <summary>
        /// Event that is fired when <see cref="IsInitialized"/> is set to '<c>true</c>'
        /// </summary>
        public static event Action OnEngineInitialized
        {
            remove
            {
                lock (m_OnEngineInitializedCallbacks)
                {
                    m_OnEngineInitializedCallbacks.Remove(value);
                }
            }

            add
            {
                if (value == null) return;
                lock (m_IsInitializedStateLock)
                {
                    if (m_IsInitialized)
                    {
                        value.Invoke();
                        return;
                    }
                }

                lock (m_OnEngineInitializedCallbacks)
                {
                    m_OnEngineInitializedCallbacks.Add(value);
                }
            }
        }

        /// <summary>
        /// Called when every existing instance of <see cref="EngineService"/> and similar is fully unloaded. (e.g. on <see cref="Unload(UnloadSettings)"/>)
        /// </summary>
        /// <remarks>
        /// Used to reset static references to the old services and configuration classes, as to prevent memory leaks on mod reloading.
        /// </remarks>
        public static event Action? OnEngineResetting;

        // Properties:
        /// <summary>
        /// Collection of all services.
        /// </summary>
        /// <remarks>
        /// All non-overwritten services are constructed (from .ctor) before <see cref="EngineService.Initialize"/> is run.
        /// So it is safe to reference this collection from <see cref="EngineService.Initialize"/>.
        /// (However, in this case, some services might not be initialized yet.)
        /// </remarks>
        public static IReadOnlyCollection<EngineService> Services => m_Services.Values;

        /// <summary>
        /// Whether engine initialized: all mods are checked and loaded, assemblies as well, services are initialized and so on.
        /// </summary>
        /// <remarks>
        /// Will be also set to 'true' when initialization failed, to prevent a lot of reloads. (Note: TODO)
        /// </remarks>
        public static bool IsInitialized => m_IsInitialized;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:
        private static readonly Dictionary<Type, EngineService> m_Services = new Dictionary<Type, EngineService>();
        private static readonly List<Assembly> m_Assemblies = new List<Assembly>();

        // Encapsulated Fields:
        private static readonly HashSet<Action> m_OnEngineInitializedCallbacks = new HashSet<Action>();
        private static readonly object m_IsInitializedStateLock = new object();
        private static volatile bool m_IsInitialized;

        // Local Fields:
        private static volatile bool m_AcceptsAssemblies = true;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Method is extremely optimized for a repeated usage. Feel free to use it very frequently.
        /// <para>
        /// However, using generic <see cref="EngineService{T}"/> is even better (by like x20 times).
        /// </para>
        /// <para>
        /// Can be used in <see cref="EngineService.Initialize"/> method,
        /// but service might not be initialized depending on <see cref="ServiceAttribute.InitializationOrder"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Will throw if requested <see cref="EngineService"/> was not created on initialization.
        /// Use '<see cref="TryGet{T}(out T)"/>' or '<see cref="GetOrDefault{T}(T)"/>' if you need to handle missing services more gracefully.
        /// </remarks>
        public static T GetOrThrow<T>() where T : EngineService
        {
            if (m_Services.TryGetValue(typeof(T), out EngineService? service))
            {
                if (service is T result)
                {
                    return result;
                }
                else
                {
                    throw new Exception($"{LogNameBraced} Service type miss-match: Found service '{service.GetType().Name}' but requested '{typeof(T).Name}'");
                }
            }
            else
            {
                throw new Exception($"{LogNameBraced} Service of type '{typeof(T).Name}' was not provided during initialization and cannot be found.");
            }
        }

        /// <summary>
        /// Returns a service, associated with a given type (<typeparamref name="T"/>), or default value (<paramref name="def"/>).
        /// </summary>
        /// <typeparam name="T">Type of the service to retrieve.</typeparam>
        /// <param name="def">Default value to return.</param>
        /// <returns>Either a requested service, or its version, overwritten by another mod.</returns>
        public static T GetOrDefault<T>(T def = default!) where T : EngineService
        {
            if (m_Services.TryGetValue(typeof(T), out EngineService? service))
            {
                if (service is T result)
                {
                    return result;
                }
                else
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }

        /// <summary>
        /// Returns a service, associated with a given type (<typeparamref name="T"/>), or default value provided by (<paramref name="def"/>).
        /// </summary>
        /// <typeparam name="T">Type of the service to retrieve.</typeparam>
        /// <param name="def">Default value provider to use.</param>
        /// <returns>Either a requested service, or its version, overwritten by another mod.</returns>
        public static T GetOrDefault<T>(Func<T> def) where T : EngineService
        {
            if (m_Services.TryGetValue(typeof(T), out EngineService? service))
            {
                if (service is T result)
                {
                    return result;
                }
                else
                {
                    return def();
                }
            }
            else
            {
                return def();
            }
        }

        /// <summary>
        /// Tries to retrieve <see cref="EngineService"/> associated with given type (<typeparamref name="T"/>).
        /// </summary>
        /// <typeparam name="T">Target type of <see cref="EngineService"/> to retrieve.</typeparam>
        /// <param name="service">Retrieved <see cref="EngineService"/> or null.</param>
        /// <returns>'<c>true</c>' if service was found. Otherwise '<c>false</c>'.</returns>
        public static bool TryGet<T>([NotNullWhen(true)] out T service) where T : EngineService
        {
            if (m_Services.TryGetValue(typeof(T), out EngineService? finding) && finding is T result)
            {
                service = result;
                return true;
            }
            else
            {
                service = default!;
                return false;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Finalization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void SetInitialized()
        {
            if (m_IsInitialized) return;

            // Locks exclusively briefly to avoid deadlocks, if invoked callback, for any reason, tries to add another callback to the list.
            lock (m_IsInitializedStateLock)
            {
                m_IsInitialized = true;
            }

            bool exceptions = false;
            foreach (var callback in m_OnEngineInitializedCallbacks)
            {
                try
                {
                    callback.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    exceptions |= true;
                }
            }

            if (exceptions)
            {
                Debug.LogError($"{LogNameBraced} Some callbacks during '{nameof(OnEngineInitialized)}' event has encountered exceptions! Look above for errors.");
            }

            // Only locks on clear, since after 'm_IsInitialized' was set to true - nothing can be modified.
            // Also should avoid deadlocks if invoked callback, for any reason, tries to add another callback to the list.
            lock (m_OnEngineInitializedCallbacks)
            {
                m_OnEngineInitializedCallbacks.Clear();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Initialization API
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Throws "Not modifiable" exception if called when <see cref="IsInitialized"/> is false.
        /// This is usually when assets are still modifiable.
        /// </summary>
        /// <remarks>
        /// Is it a good idea to lock systems behind such limitation though?
        /// <para>
        /// Use it only in a non-performance critical code, like class setters that usually never called, or initialization-only setters.
        /// </para>
        /// </remarks>
        public static void AssertModifiable([CallerFilePath] string caller = "")
        {
            if (IsInitialized) throw new Exception($"Cannot modify ('{Path.GetFileNameWithoutExtension(caller)}') outside of the engine initialization stage.");
        }
        
        /// <inheritdoc cref="Reload(UnloadSettings)"/>
        public static async UniTask Reload() => await Reload(UnloadSettings.ReloadSettings);

        /// <summary>
        /// Reloads entire engine from the ground-up.
        /// </summary>
        /// <param name="unloading"><see cref="UnloadSettings"/> to use with <see cref="Unload(UnloadSettings)"/></param>
        public static async UniTask Reload(UnloadSettings unloading)
        {
            await Unload(unloading);
            await Initialize();
        }

        /// <summary>
        /// Initializes the entire engine: <see cref="EngineService"/>s, <see cref="Modding.Mod"/>s, and so on.
        /// </summary>
        /// <remarks>
        /// (TODO) Use <see cref="Reload()"/> instead to make a "shallow reload" - with all the same configurations,
        /// but without Textures and other assets being fully unloaded from the memory.
        /// </remarks>
        public static async UniTask Initialize()
        {
            // TODO: Decide what to do with service unloading when in the Editor.
            //  Maybe provide special UNITY_EDITOR-only methods?
            //  We can keep them in the code so people can restore Editor's tools more easily.
            //  Although, a lot of it will be gate-kept behind Application.isEditor anyway.
            Application.quitting += ResetState;

            if (Application.isEditor)
            {
                Debug.LogWarning($"Engine initializes in the Editor. Application.isPlaying: {Application.isPlaying}");
            }
            else
            {
                Debug.LogWarning($"Engine initializes at Runtime. Application.isPlaying: {Application.isPlaying}");
            }

            await LoadModsAndAssemblies();
            await InitializeEngine();
        }

        /// <inheritdoc cref="Unload(UnloadSettings)"/>
        public static async UniTask Unload() => await Unload(UnloadSettings.UnloadEverything);

        /// <summary>
        /// Unloads entire engine, all initialized services and mods from the memory.
        /// </summary>
        /// <remarks>
        /// (TODO) Use <see cref="Reload()"/> instead to make a "shallow reload" - with all the same configurations,
        /// but without Textures and other assets being fully unloaded from the memory.
        /// </remarks>
        /// <param name="settings">Settings to use for unloading.</param>
        public static async UniTask Unload(UnloadSettings settings)
        {
            ResetState();
            GC.Collect();

            // TODO: Still decide what to do with service unloading in the editor.
            Application.quitting -= ResetState;
            await UniTask.CompletedTask;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                       Unity Initialization Callbacks
        /// .                                TODO: Add Editor-time initialization methods.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnSubsystemRegistration()
        {
            if (EclipseConfiguration.Instance.InitializationType == EclipseInitializationType.SubsystemRegistration)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnAfterAssembliesLoaded()
        {
            if (EclipseConfiguration.Instance.InitializationType == EclipseInitializationType.AfterAssembliesLoaded)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            if (EclipseConfiguration.Instance.InitializationType == EclipseInitializationType.BeforeSplashScreen)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == EclipseInitializationType.BeforeSceneLoad)
            {
                Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == EclipseInitializationType.AfterSceneLoad)
            {
                Initialize().Forget();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Unloading
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Unloads assemblies and classes from the memory.
        /// </summary>
        /// <remarks>
        /// Will cache resources like textures, atlases, music and etc. in case it will be referenced again.
        /// Will use update date and time to decide whether to update a resource or not.
        /// </remarks>
        private static void ResetState()
        {
            foreach (var service in m_Services.Values)
            {
                ((IEngineServiceDirectAccess)service).EngineInvokeUnloading();
            }

            m_Services.Clear();
            m_Assemblies.Clear();
            m_AcceptsAssemblies = true;

            try
            {
                OnEngineResetting?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LogNameBraced} Failed to dispose references on engine reload. Expect small/large memory leaks.");
                Debug.LogException(ex);
            }

            OnEngineResetting = null;
            m_IsInitialized = false;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                            Fetching and Loading
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static async UniTask LoadModsAndAssemblies()
        {
            Debug.Log($"{LogNameBraced} Executing '{nameof(LoadModsAndAssemblies)}'");

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
                    Debug.LogWarning($"{LogNameBraced} Just a note to you - modding is not supported on Mobile platforms and Console platforms yet.");
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
                Debug.LogError($"{LogNameBraced} Mod registration on '{nameof(LoadModsAndAssemblies)}' failed!");
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log($"{LogNameBraced} Mod registration (on '{nameof(LoadModsAndAssemblies)}') successful!");
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




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Initialization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static async UniTask InitializeEngine()
        {
            // Note: async execution messes-up execution order. Account for that further on.
            Debug.Log($"{LogNameBraced} Executing '{nameof(InitializeEngine)}'");
            m_AcceptsAssemblies = false;

            // Loads EngineService attributes.
            try
            {
#if UNITY_WEBGL
                Debug.LogWarning($"{LogName} Threaded initialization runs synchronously on WebGL. Long initialization time is to be expected.");
#endif
                m_Services.Clear();

                // TODO: Both CPU and memory optimize the initialization.
                List<ServiceSummary>? services = new List<ServiceSummary>();

                // Note: 'preload'/'afterload' here refer to method attribute categories,
                // not the threaded execution mode ('BeforeMain' / 'AfterMain').   - Dark & GPT
                List<MethodSummary<ServicePreloadMethodAttribute>> preload = new List<MethodSummary<ServicePreloadMethodAttribute>>();
                List<MethodSummary<ServiceAfterloadMethodAttribute>> afterload = new List<MethodSummary<ServiceAfterloadMethodAttribute>>();

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
                Dictionary<Type, ServiceSummary> mapping = new Dictionary<Type, ServiceSummary>(Mathf.NextPowerOfTwo((int)(summaries.Length * ResizeSafetyMargin)));

                // Creates association between all parent classes with ServiceAttribute of replaced services, so for any of them child will be returned.
                for (int i = 0; i < summaries.Length; i++)
                {
                    ServiceSummary summary = summaries[i];
                    Type target = summary.service!;

                    // Note: would of been nice to make mapping of m_Services and class activation execute in parallel with passes below, in a background thread.
                    // Maybe by adding some kind of internal temporary reference table?
                    // 
                    // Right now activation is synced with a main thread, but it doesn't have to. This code will be moved to background thread later.
                    // You should use EngineService Initialize for executing code on a main thread instead.
                    EngineService service = (EngineService)Activator.CreateInstance(summary.service)!;

                    do
                    {
                        mapping[target] = summary;
                        m_Services[target] = service;
                        target = target.BaseType!;
                    }
                    while (!(target is null) && target != typeof(EngineService) && target != typeof(EngineService));
                }

                // No reason to parallelize this one - it will just create unnecessary overhead.
                // We will have at max 50-100 services with mods, I assume   - Dark
                // (Note: I wonder if it will even work in WebGL XD   - Dark)
                int before = summaries.Count(s => s.attribute.ThreadExecutionOrder == ServiceAttribute.ThreadExecutionMode.ThreadSafeBeforeMain);
                int after = summaries.Count(s => s.attribute.ThreadExecutionOrder == ServiceAttribute.ThreadExecutionMode.ThreadSafeAfterMain);
                int normal = summaries.Length - before - after;

                // Adds all methods to a referenced services.
                // Also creates instances of the services (Note: because of that .ctor of services are not thread-safe)
                await UniTask.WhenAll(
                    UniTask.Run(() =>
                    {
                        foreach (MethodSummary<ServicePreloadMethodAttribute> callback in preload)
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
                        foreach (MethodSummary<ServiceAfterloadMethodAttribute> callback in afterload)
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
                    UniTask.Run(() => services.Sort((a, b) => a.attribute.InitializationOrder.CompareTo(b.attribute.InitializationOrder)))
                );

                // Updates summaries with sorted data.
                services.CopyTo(summaries);
                services = null; // List itself should not be used after this point, as it is inefficient.

                // Executed thread-safe initializations and callbacks before main thread.
                await RunThreadedInitialization(before, ServiceAttribute.ThreadExecutionMode.ThreadSafeBeforeMain);

                // Initialization part on a Main Unity thread.
                if (normal > 0)
                {
                    foreach (ServiceSummary summary in summaries)
                    {
                        if (summary.attribute.ThreadExecutionOrder != ServiceAttribute.ThreadExecutionMode.MainThread) continue;
                        summary.preload.ForEach(m => m.method.Invoke(null, null));
                        ((IEngineServiceDirectAccess)m_Services[summary.service]).EngineInvokeInitialization();
                        summary.afterload.ForEach(m => m.method.Invoke(null, null));
                    }
                }

                // Executed thread-safe initializations and callbacks after main thread.
                await RunThreadedInitialization(after, ServiceAttribute.ThreadExecutionMode.ThreadSafeAfterMain);

                // Simplifications:
                async UniTask RunThreadedInitialization(int expected, ServiceAttribute.ThreadExecutionMode mode)
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
                            if (set.attribute.ThreadExecutionOrder == mode)
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
                            tasks[i] = UniTask.Run(() =>
                            {
                                var set = temp[i];
                                set.preload.ForEach(c =>
                                {
                                    if (c.attribute.ThreadSafe) c.method.Invoke(null, null);
                                });

                                ((IEngineServiceDirectAccess)m_Services[set.service]).EngineInvokeInitialization();
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
                Debug.LogError($"{LogNameBraced} Game and mod initialization on '{nameof(InitializeEngine)}' failed!");
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log($"{LogNameBraced} Game and mod initialization (on '{nameof(InitializeEngine)}') successful!");
            }

            m_Assemblies.Clear();
            SetInitialized();
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSplashScreen()
        {
            TestEngine();
        }
#endif




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
                Debug.LogError($"{LogNameBraced} Assembly was not loaded because it was queued after Engine read the queue. ('{assembly.FullName}')");
                return;
            }

            m_Assemblies.Add(assembly);
        }

        private static void LoadServices(
            Assembly assembly, List<ServiceSummary> services,
            List<MethodSummary<ServicePreloadMethodAttribute>> preload,
            List<MethodSummary<ServiceAfterloadMethodAttribute>> afterload)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(EngineService).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var attribute = type.GetCustomAttribute<ServiceAttribute>();
                    if (attribute == null)
                    {
                        services.Add(new ServiceSummary(new ServiceAttribute(), type));
                    }
                    else
                    {
                        ServiceSummary summary = new ServiceSummary(attribute, type);
                        if (attribute.Replace != null)
                        {
                            int length = services.Count;
                            for (int i = 0; i < length; i++)
                            {
                                var entry = services[i];
                                if (entry.service == attribute.Replace)
                                {
                                    services.RemoveAt(i);
                                    i--; length--;

#if UNITY_EDITOR
                                    for (; i < length; i++)
                                    {
                                        if (services[i].service == attribute.Replace)
                                        {
                                            Debug.LogWarning($"Found a duplicate of the service '{attribute.Replace.Name}'! This should not happen!");
                                        }
                                    }

                                    break; // Removes only one reference, as normally, it should be impossible for you to get two of the same services.
#else
                                    break; // Removes only one reference, as normally, it should be impossible for you to get two of the same services.
#endif
                                }
                            }
                        }

                        services.Add(summary);
                    }
                }

                foreach (var method in type.GetMethods())
                {
                    if (!method.IsStatic) continue;
                    foreach (var attribute in method.GetCustomAttributes<ServicePreloadMethodAttribute>(inherit: false))
                    {
                        preload.Add(new MethodSummary<ServicePreloadMethodAttribute>(attribute, method));
                    }

                    foreach (var attribute in method.GetCustomAttributes<ServiceAfterloadMethodAttribute>(inherit: false))
                    {
                        afterload.Add(new MethodSummary<ServiceAfterloadMethodAttribute>(attribute, method));
                    }
                }
            }
        }




#if UNITY_EDITOR
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Testing
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void TestEngine()
        {
            try
            {
                TestServiceRetrieval();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LogNameBraced} Engine testing failed! See below for the exceptions.");
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log($"{LogNameBraced} All tests succeeded!");
            }
        }
        private static void TestServiceRetrieval()
        {
            Assert.IsNotNull(Get<Localization.LocalizationService>());
        }
#endif



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Structs
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly struct ServiceSummary
        {
            public readonly ServiceAttribute attribute;
            public readonly Type service;
            public readonly List<MethodSummary<ServicePreloadMethodAttribute>> preload;
            public readonly List<MethodSummary<ServiceAfterloadMethodAttribute>> afterload;
            public ServiceSummary(ServiceAttribute attribute, Type service)
            {
                this.attribute = attribute;
                this.service = service;
                preload = new List<MethodSummary<ServicePreloadMethodAttribute>>(0);
                afterload = new List<MethodSummary<ServiceAfterloadMethodAttribute>>(0);
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
