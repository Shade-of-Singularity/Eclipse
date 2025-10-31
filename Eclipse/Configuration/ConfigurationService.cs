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
using Eclipse.Configuration.Categorization;
using Eclipse.Configuration.Parameters;
using Eclipse.Configuration.Storages;
using Eclipse.Extensions;
using Eclipse.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Service, responsible for providing ways for configuring the game either from the editor,
    /// manually during game development, or using in-game settings.
    /// </summary>
    /// Note: if service won't be overwritable - we can just directly imbed it into Engine initialization process.
    [Service(InitializationOrder = InitializationOrder)]
    public class ConfigurationService : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <see cref="ConfigurationService"/> is initialized very first in the entire game,
        /// as even <see cref="Localization.LocalizationService"/> relies on it.
        /// </summary>
        /// Note: This is why you cannot localize anything here btw.
        public const int InitializationOrder = -2_000_000_000;
        public const string LogName = nameof(ConfigurationService);
        public const string LogNameBraced = "[" + LogName + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:
        /// <param name="configuration">Configuration class what have been changed.</param>
        //public delegate void ConfigurationChangedHandler(EngineConfiguration configuration);

        // Events:
        //public event ConfigurationChangedHandler OnConfigurationChanged;
        public event Action? OnBeforeApplyChanges;
        public event Action? OnBeforeRevertChanges;
        public event Action? OnBeforeSerialization;
        public event Action? OnBeforeServiceSerialization;
        public event Action? OnBeforeGameStateSerialization;
        public event Action? OnBeforeParameterSerialization;

        public event Action? OnAfterApplyChanges;
        public event Action? OnAfterRevertChanges;
        public event Action? OnAfterSerialization;
        public event Action? OnAfterServiceSerialization;
        public event Action? OnAfterGameStateSerialization;
        public event Action? OnAfterParameterSerialization;

        /// <summary>
        /// Called when any of the <see cref="Parameter"/>s have their name/category changed. (See also: <seealso cref="Parameter.Name"/>)
        /// </summary>
        public static event Action? OnCategorizationChanged;

        // Properties:
        public static readonly string ConfigurationPath = Application.persistentDataPath;

        /// <summary>
        /// Whether <see cref="ConfigurationService"/> saves anything to the disk at the moment.
        /// </summary>
        public bool ExecutesSerialization
        {
            get => m_ExecutesSerialization;
            protected set
            {
                if (m_ExecutesSerialization != value)
                {
                    try
                    {
                        if (m_ExecutesSerialization = value)
                        {
                            OnBeforeSerialization?.Invoke();
                        }
                        else
                        {
                            OnAfterSerialization?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="ConfigurationService"/> is going through every <see cref="EngineService"/> and saves its state to the disk.
        /// </summary>
        /// <remarks>
        /// At the moment engine services simply use <see cref="GameState"/>s for serialization, so this method is added just for the future.
        /// </remarks>
        public bool ExecutesServiceSerialization
        {
            get => m_ExecutesServiceSaving;
            protected set
            {
                if (m_ExecutesServiceSaving != value)
                {
                    try
                    {
                        if (m_ExecutesServiceSaving = value)
                        {
                            OnBeforeServiceSerialization?.Invoke();
                        }
                        else
                        {
                            OnAfterServiceSerialization?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="ConfigurationService"/> is going through every <see cref="GameState"/> and saves it to the disk.
        /// </summary>
        public bool ExecutesGameStateSaving
        {
            get => m_ExecutesGameStateSaving;
            protected set
            {
                if (m_ExecutesGameStateSaving != value)
                {
                    try
                    {
                        if (m_ExecutesGameStateSaving = value)
                        {
                            OnBeforeGameStateSerialization?.Invoke();
                        }
                        else
                        {
                            OnAfterGameStateSerialization?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="ConfigurationService"/> is going through every <see cref="Parameter"/> and saves its state to the disk.
        /// </summary>
        public bool ExecutesParameterSaving
        {
            get => m_ExecutesParameterSaving;
            protected set
            {
                if (m_ExecutesParameterSaving != value)
                {
                    try
                    {
                        if (m_ExecutesParameterSaving = value)
                        {
                            OnBeforeParameterSerialization?.Invoke();
                        }
                        else
                        {
                            OnAfterParameterSerialization?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether any of the parameters have changed and can be applied.
        /// </summary>
        /// <remarks>
        /// Directly modified by <see cref="Parameter"/>s.
        /// </remarks>
        /// TODO: Reset <see cref="IsDirty"/> if all the parameters is no longer modifies
        /// after direct user inputs or direct <see cref="Parameter.RevertChanges"/> usage.
        public bool IsDirty { get; set; }

        /// <summary>
        /// Whether to do a frame delay when something tried to invoke <see cref="OnCategorizationChanged"/> callback.
        /// </summary>
        public bool DoCategorizationCallbackDelays => true;

        /// <summary>
        /// Storage type to use for all parameters.
        /// </summary>
        /// <remarks>
        /// TODO: Make a way to store some of the values in different places, based on a flag (e.g. persistent, dynamic, per-profile, etc.).
        /// </remarks>
        public IParameterStorage Storage
        {
            get => m_Storage;
            set
            {
                value ??= PlayerPreferenceStorage.Instance;
                if (m_Storage != value)
                {
                    m_Storage = value;
                }
            }
        }



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Private Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ConfigurationServiceCoroutineHandler CoroutineRunner
        {
            get
            {
                if (m_CoroutineRunner == null)
                {
                    m_CoroutineRunner = new GameObject("Configuration Service Coroutine Runner").AddComponent<ConfigurationServiceCoroutineHandler>();
                }

                return m_CoroutineRunner;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:
        private static ConfigurationServiceCoroutineHandler m_CoroutineRunner;

        // Encapsulated Fields:
        private IParameterStorage m_Storage = PlayerPreferenceStorage.Instance;
        private bool m_ExecutesSerialization = false;
        private bool m_ExecutesServiceSaving = false;
        private bool m_ExecutesGameStateSaving = false;
        private bool m_ExecutesParameterSaving = false;

        // Local Fields:
        private readonly Dictionary<Type, EngineConfiguration> m_EngineConfigurations = new();
        private readonly Dictionary<string, Parameter> m_Parameters = new();
        private readonly Dictionary<string, Category> m_Categories = new();
        private readonly Dictionary<Type, GameState> m_GameStates = new();
        private UniTaskCompletionSource m_AwaitSource = new();
        private readonly object m_AwaitLock = new();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void InitializeParameters() => ApplyForceCallbacks();
        protected override void Initialize()
        {
            Engine.OnEngineInitialized += InitializeParameters;
            LoadResources();
            LoadInternal();
        }

        protected override void Unload()
        {
            Engine.OnEngineInitialized -= InitializeParameters;
            SaveInternal();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initializes all <see cref="Parameter"/>s in a given class.
        /// </summary>
        /// <remarks>
        /// In reality, just runs a static constructor on it.
        /// </remarks>
        /// <param name="class">Type of the class which holds <see cref="Parameter"/>s.</param>
        public void SetSettings(Type @class) => SetSettings(@class.TypeHandle);

        /// <inheritdoc cref="SetSettings(Type)"/>
        /// <param name="handle">Handle of the class which holds <see cref="Parameter"/>s.</param>
        public void SetSettings(RuntimeTypeHandle handle) => RuntimeHelpers.RunClassConstructor(handle);

        /// <summary>
        /// Registers new parameter.
        /// </summary>
        public void Register(Parameter parameter)
        {
            Assert.IsNotNull(parameter);
            if (string.IsNullOrEmpty(parameter.Name))
            {
#if UNITY_EDITOR && DEBUG
                Debug.LogWarning($"Attempted to register parameter with an empty name (\"{parameter.Name}\")");
#endif
                return; // Empty properties are simply ignored.
            }

            if (!m_Parameters.TryAdd(parameter.Name, parameter))
            {
                Debug.LogError($"Attempted to add a parameter duplicate of the name: \"{parameter.Name}\". This is not allowed. Older parameter will be serialized. Consider following a parameter name convention for mods: \"<mod-name>-<parameter>\", e.g. \"MyAwesomeMod-CharacterSpeed\"");
            }
        }

        // TODO: Add more comments. Those methods just look for a specified parameter in the database.
        public Parameter? FindOrThrow(FullName name) => FindOrThrow(name.Full);
        public Parameter? FindOrThrow(string name)
        {
            if (m_Parameters.TryGetValue(name, out Parameter? finding))
            {
                return finding;
            }

            throw new Exception($"{LogNameBraced} Cannot find property with name: '{name}' (in typeless search scope).");
        }

        public Parameter? Find(FullName name) => Find(name.Full);
        public Parameter? Find(string name)
        {
            if (m_Parameters.TryGetValue(name, out Parameter? finding))
            {
                return finding;
            }

            return null;
        }

        public TParameter FindOrThrow<TParameter>(FullName name) where TParameter : Parameter => FindOrThrow<TParameter>(name.Full);
        public TParameter FindOrThrow<TParameter>(string name) where TParameter : Parameter
        {
            if (m_Parameters.TryGetValue(name, out Parameter? finding))
            {
                if (finding is TParameter result)
                {
                    return result;
                }
                else
                {
                    throw new Exception($"{LogNameBraced} Property with name '{name}' doesn't have required type." +
                        $"Found: {finding.GetType().Name}  Requested: {typeof(TParameter).Name}.");
                }
            }

            throw new Exception($"{LogNameBraced} Cannot find property with name: '{name}'. Type: {typeof(TParameter).Name}");
        }

        public TParameter? Find<TParameter>(FullName name) where TParameter : Parameter => Find<TParameter>(name.Full);
        public TParameter? Find<TParameter>(string name) where TParameter : Parameter
        {
            if (m_Parameters.TryGetValue(name, out Parameter? finding) && finding is TParameter result)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Applies all dirty properties.
        /// </summary>
        public void Apply()
        {
            if (IsDirty)
            {
                Try.WithLog(() => OnBeforeApplyChanges?.Invoke());
                foreach (var parameter in m_Parameters.Values)
                {
                    parameter.ApplyChanges();
                }

                Try.WithLog(() => OnAfterApplyChanges?.Invoke());
            }
        }

        /// <summary>
        /// Applies all properties and fires related callbacks (e.g. <see cref="AbstractParameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// </summary>
        public void ApplyForceCallbacks()
        {
            Try.WithLog(() => OnBeforeApplyChanges?.Invoke());
            foreach (var parameter in m_Parameters.Values)
            {
                parameter.ApplyChangesForceFireCallbacks();
            }

            Try.WithLog(() => OnAfterApplyChanges?.Invoke());
        }

        /// <summary>
        /// Reverts all dirty properties.
        /// </summary>
        public void Revert()
        {
            if (IsDirty)
            {
                Try.WithLog(() => OnBeforeRevertChanges?.Invoke());
                foreach (var parameter in m_Parameters.Values)
                {
                    parameter.RevertChanges();
                }

                Try.WithLog(() => OnAfterRevertChanges?.Invoke());
            }
        }

        /// <summary>
        /// Reverts all properties and fires related callbacks (e.g. <see cref="AbstractParameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// (See also: <seealso cref="Parameter.ApplyChangesForceFireCallbacks"/>)
        /// </summary>
        public void RevertForceCallbacks()
        {
            Try.WithLog(() => OnBeforeRevertChanges?.Invoke());
            foreach (var parameter in m_Parameters.Values)
            {
                parameter.RevertChanges();
            }

            Try.WithLog(() => OnAfterRevertChanges?.Invoke());
        }

        /// <summary>
        /// Saves an <see cref="GameState"/> to the configuration file.
        /// </summary>
        /// <remarks>
        /// It will be written to the disk immediately, or in a bit later, if <see cref="ConfigurationService"/> is in
        /// </remarks>
        /// <typeparam name="T">Type of your data.</typeparam>
        public void Save<T>(T data) where T : GameState, new()
        {
            m_GameStates[typeof(T)] = data;

        }

        /// <summary>
        /// Loads <see cref="GameState"/> from a configuration file.
        /// </summary>
        /// <remarks>
        /// You can use it to load game data, such as player positions, inventory data, etc.
        /// </remarks>
        /// <returns>
        /// New <typeparamref name="T"/> parameter if no configuration file found.
        /// Otherwise - loaded data.
        /// </returns>
        /// <typeparam name="T"></typeparam>
        public T Load<T>() where T : GameState, new()
        {
            // TODO: Load-in GameStates.
            if (m_GameStates.TryGetValue(typeof(T), out GameState state))
            {
                // Guaranteed to be of a right type.
                // If not - I will be retiring before even finding a job dude XD
                return (T)state;
            }
            else
            {
                T result = new();
                m_GameStates[typeof(T)] = result;
                return result;
            }
        }

        /// <summary>
        /// Starts serialization
        /// </summary>
        public async UniTask Serialize()
        {
            // TODO: Add locking.
            if (ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            // TODO: Fully implement serialization.
            ExecutesSerialization = true;
            await SerializeServicesInternal();
            await SerializeGameStateInternal();
            await SerializeParametersInternal();
            ExecutesSerialization = false;
        }

        public async UniTask SerializeServices()
        {
            // TODO: Add locking.
            if (ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            ExecutesSerialization = true;
            await SerializeServicesInternal();
            ExecutesSerialization = false;
        }

        public async UniTask SerializeGameStates()
        {
            // TODO: Add locking.
            if (ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            ExecutesSerialization = true;
            await SerializeGameStateInternal();
            ExecutesSerialization = false;
        }

        public async UniTask SerializeParameters()
        {
            // TODO: Add locking.
            if (ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            ExecutesSerialization = true;
            await SerializeParametersInternal();
            ExecutesSerialization = false;
        }

        /// <summary>
        /// Allows you to overwrite engine configurations at runtime.
        /// </summary>
        /// <remarks>
        /// Do it only if you want to completely overhaul how game works. Otherwise - do NOT touch it.
        /// <para>
        /// Make sure that <see cref="EngineConfiguration{T}.Instance"/> is NEVER called before you call this one.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Set<T>(T value) where T : EngineConfiguration
        {
            m_EngineConfigurations[typeof(T)] = value;
        }

        /// TODO: Optimize engine configuration access the name way in which <see cref="Engine.Get{T}"/> was optimized, if needed.
        /// Note: Done! You can now use <see cref="EngineConfiguration{T}.Instance"/> or <see cref="EngineConfiguration.Get{T}"/> for that!
        public T GetOrNew<T>() where T : EngineConfiguration => m_EngineConfigurations.GetValueOrDefault(typeof(T)) as T ?? ScriptableObject.CreateInstance<T>();
        public void NotifyCategorizationChanged()
        {
            if (DoCategorizationCallbackDelays && !CoroutineRunner.IsRunning)
            {
                CoroutineRunner.StartCoroutine(() => OnCategorizationChanged?.Invoke());
            }
            else
            {
                // I wasn't feeling like dealing with callback resolving for when you switch that value on and off frequently.
                throw new NotSupportedException("Non-delayed categorization is not supported at the moment.");
            }
        }

        public bool TryGetCategory(FullCategory category, [NotNullWhen(true)] out Category? result) => TryGetCategory(category.Name, out result);
        public bool TryGetCategory(string category, [NotNullWhen(true)] out Category? result)
        {
            result = GetCategory(category);
            return result != null;
        }

        public Category? GetCategory(FullCategory category) => GetCategory(category.Name);
        public Category? GetCategory(string category)
        {
            m_Categories.TryGetValue(category, out Category? result);
            return result;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void LoadResources()
        {
            // TODO: Allow overwriting engine configurations at runtime, if needed.
            m_EngineConfigurations.Clear();
            EngineConfiguration[] configurations = Resources.LoadAll<EngineConfiguration>(string.Empty);
            foreach (var configuration in configurations)
            {
                Type key = configuration.GetType();
#if DEBUG
                if (m_EngineConfigurations.ContainsKey(key))
                {
                    Debug.LogWarning($"Found additional instance of {key.Name}. Using new one.");
                }
#endif

                m_EngineConfigurations[key] = configuration;
            }
        }

        /// <summary>
        /// Forcefully loads-in all data about registered parameters.
        /// </summary>
        private void LoadInternal()
        {
            foreach (var parameters in m_Parameters.Values)
            {
                Storage.Load(parameters);
            }
        }

        /// <summary>
        /// Forcefully saves a save file data about all registered parameters.
        /// </summary>
        /// <remarks>
        /// Will not check for <see cref="IsDirty"/>.
        /// </remarks>
        private void SaveInternal()
        {
            foreach (var parameter in m_Parameters.Values)
            {
                Storage.Save(parameter);
            }
        }

        private async UniTask WaitForCompletion()
        {
            // TODO: Make more optimized with completion sources.
            while (ExecutesSerialization)
            {
                await UniTask.Yield();
            }
        }

        private async UniTask SerializeServicesInternal()
        {
            ExecutesServiceSerialization = true;
            uint exceptions = 0;

            foreach (var service in Engine.Services)
            {
                try
                {
                    // Serialize services here.
                    await UniTask.CompletedTask;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    exceptions++;
                }
            }

            if (exceptions > 0)
            {
                Debug.LogError(
                    $"{LogNameBraced} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            ExecutesServiceSerialization = false;
        }

        private async UniTask SerializeGameStateInternal()
        {
            ExecutesGameStateSaving = true;
            uint exceptions = 0;

            foreach (var service in Engine.Services)
            {
                try
                {
                    // Serialize game states here.
                    await UniTask.CompletedTask;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    exceptions++;
                }
            }

            if (exceptions > 0)
            {
                Debug.LogError(
                    $"{LogNameBraced} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            ExecutesGameStateSaving = false;
        }

        private async UniTask SerializeParametersInternal()
        {
            ExecutesParameterSaving = true;
            uint exceptions = 0;

            foreach (var service in Engine.Services)
            {
                try
                {
                    // Serialize parameters here.
                    await UniTask.CompletedTask;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    exceptions++;
                }
            }

            if (exceptions > 0)
            {
                Debug.LogError(
                    $"{LogNameBraced} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            ExecutesParameterSaving = false;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Coroutine Handler
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public sealed class ConfigurationServiceCoroutineHandler : MonoBehaviour
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Public Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            // Delegates:

            // Events:

            // Properties:
            public bool IsRunning => m_IsRunning;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            // Static Fields:

            // Encapsulated Fields:
            private bool m_IsRunning;

            // Local Fields:




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Unity Callbacks
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private void Awake() => DontDestroyOnLoad(gameObject);




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Public Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public void StartCoroutine(Action? endAction)
            {
                if (endAction == null) return;
                if (!m_IsRunning)
                {
                    StartCoroutine(Delay(endAction));
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private IEnumerator Delay(Action callback)
            {
                m_IsRunning = true;
                yield return null;
                m_IsRunning = false;
                callback();
            }
        }
    }
}
