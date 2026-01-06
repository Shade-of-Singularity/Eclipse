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
using Eclipse.Configuration.Parameters;
using Eclipse.Configuration.Storages;
using Eclipse.Extensions;
using Eclipse.Serialization;
using Eclipse.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Service, responsible for providing ways for configuring the game either from the editor,
    /// manually during game development, or using in-game settings.
    /// </summary>
    /// <remarks>
    /// You might struggle to decide if you should manually implement your settings or use <see cref="AbstractParameter"/>s instead.
    /// For that, keep in might rule of thumb:
    /// <para>- if parameter is modifiable at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter can be modified by other mods at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter is only set at launch or engine restart - use C# Properties.</para>
    /// <para>- if parameter can be 'set' at runtime, but value will only update after reload - use <see cref="SolidParameter{TValue}"/>.</para>
    /// </remarks>
    public abstract class ConfigurationService : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <see cref="ConfigurationService"/> is initialized after <see cref="Serialization.SerializationService"/>, as it relies on it to serialize parameters.
        /// One of the first to be initialized, as all other services, like <see cref="Localization.LocalizationService"/>, relies on it.
        /// </summary>
        /// Note: This is why you cannot localize anything here btw.
        public const int InitializationOrder = -1_700_000_000;

        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[" + nameof(ConfigurationService) + "]";

        /// <summary>
        /// Path to configuration files on the disk.
        /// </summary>
        /// <remarks>
        /// Also path to save files, as <see cref="ConfigurationService"/> also responsible for general serialization.
        /// </remarks>
        public static readonly string ConfigurationPath = $"{Application.persistentDataPath}/Configuration";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:
        public static event Action? OnBeforeApplyChanges;
        public static event Action? OnBeforeRevertChanges;
        public static event Action? OnBeforeSerialization;
        public static event Action? OnBeforeServiceSerialization;
        public static event Action? OnBeforeGameStateSerialization;
        public static event Action? OnBeforeParameterSerialization;

        public static event Action? OnAfterApplyChanges;
        public static event Action? OnAfterRevertChanges;
        public static event Action? OnAfterSerialization;
        public static event Action? OnAfterServiceSerialization;
        public static event Action? OnAfterGameStateSerialization;
        public static event Action? OnAfterParameterSerialization;

        // Properties:
        /// <summary>
        /// Whether <see cref="ConfigurationService"/> saves anything to the disk at the moment.
        /// </summary>
        public static bool ExecutesSerialization
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
        /// Whether <see cref="ConfigurationService"/> is going through every <see cref="AbstractParameter"/> and saves its state to the disk.
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
        /// Directly modified by <see cref="AbstractParameter"/>s.
        /// </remarks>
        /// TODO: Reset <see cref="IsDirty"/> if all the parameters is no longer modifies
        /// after direct user inputs or direct <see cref="AbstractParameter.RevertChanges"/> usage.
        public abstract bool IsDirty { get; }

        /// <summary>
        /// Storage type to use for all parameters.
        /// </summary>
        /// <remarks>
        /// TODO: Make a way to store some of the values in different places, based on a flag (e.g. persistent, dynamic, per-profile, etc.).
        /// </remarks>
        public IDataStorage Storage
        {
            get => m_Storage;
            set
            {
                value ??= PlayerPreferenceStorage.Instance;
                if (m_Storage != value)
                {
                    m_Storage = value;
                    Engine.OnEngineInitialized += () => Serialize();
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Private Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static CoroutineHandler CoroutineRunner
        {
            get
            {
                if (m_CoroutineRunner == null)
                {
                    m_CoroutineRunner = new GameObject("Configuration Service Coroutine Runner").AddComponent<CoroutineHandler>();
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
        protected static readonly Dictionary<string, Action> m_StandaloneSetters = new Dictionary<string, Action>();
        protected static bool m_ExecutesSerialization = false;
        protected static bool m_ExecutesServiceSaving = false;
        protected static bool m_ExecutesGameStateSaving = false;
        protected static bool m_ExecutesParameterSaving = false;
        private static CoroutineHandler? m_CoroutineRunner;

        // Encapsulated Fields:
        private IDataStorage m_Storage = PlayerPreferenceStorage.Instance;

        // Local Fields:
        private readonly Dictionary<Type, EngineConfiguration> m_EngineConfigurations = new Dictionary<Type, EngineConfiguration>();
        private readonly Dictionary<string, AbstractParameter> m_Parameters = new Dictionary<string, AbstractParameter>();
        private readonly Dictionary<Type, GameState> m_GameStates = new Dictionary<Type, GameState>();
        private readonly UniTaskCompletionSource m_AwaitSource = new UniTaskCompletionSource();
        private readonly object m_AwaitLock = new object();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initializes all <see cref="AbstractParameter"/>s in a given class.
        /// </summary>
        /// <remarks>
        /// In reality, just runs a static constructor on it.
        /// </remarks>
        /// <seealso cref="RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)"/>
        /// <param name="class">Type of the class which holds <see cref="AbstractParameter"/>s.</param>
        public static void SetSettings(Type @class) => SetSettings(@class.TypeHandle);

        /// <inheritdoc cref="SetSettings(Type)"/>
        /// <param name="handle">Handle of the class which holds <see cref="AbstractParameter"/>s.</param>
        public static void SetSettings(RuntimeTypeHandle handle) => RuntimeHelpers.RunClassConstructor(handle);

        #region Callback handling

        /// <summary>
        /// Delays given action by a frame, and invokes it afterwards.
        /// </summary>
        /// <param name="wasScheduled">Indicates if this callback was already scheduled or not. Won't schedule if it is <c>true</c>.</param>
        /// <param name="action">Action to schedule and run a frame later. Not scheduled it <paramref name="wasScheduled"/> is <c>true</c>.</param>
        public static void Delay(ref bool wasScheduled, Action? action)
        {
            if (wasScheduled || action is null) return;
            wasScheduled = true;

            CoroutineRunner.StartCoroutine(DelayedInvoke());
            IEnumerator DelayedInvoke()
            {
                yield return null;
                action();
            }
        }

        /// <summary>
        /// Delays given action by a frame, and invokes it afterwards.
        /// </summary>
        /// <param name="action">Action to schedule and run a frame later.</param>
        public static void Delay(Action? action)
        {
            if (action is null) return;

            CoroutineRunner.StartCoroutine(DelayedInvoke());
            IEnumerator DelayedInvoke()
            {
                yield return null;
                action();
            }
        }

        #endregion




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .        Or in other words: you can see, I wasn't expecting anyone to inherit ConfigurationService XD
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        #region Set (custom variable)
        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, bool value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, byte value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, sbyte value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, short value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, ushort value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, char value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, int value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, uint value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, long value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, ulong value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, float value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, double value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        public abstract bool Set(string id, decimal value);

        /// <summary>
        /// (Supports <see cref="Revert"/>) Allows you to set values to raw parameters which require highest performance (i.e. <see cref="Specs.Cache.L1Cache"/>).
        /// </summary>
        /// <remarks>
        /// Might not support reverting with custom <see cref="ConfigurationService"/> implementation.
        /// </remarks>
        /// <param name="id">ID of the raw parameter to set. I recommend following "@name" pattern.</param>
        /// <param name="value">New value to set to a parameter.</param>
        /// <returns>
        /// <c>true</c> if last variable value changed. <c>false</c> otherwise.
        /// (If <see cref="IDataStorage"/> or custom <see cref="ConfigurationService"/> doesn't support value checking,
        /// will always return true instead and will set <see cref="IsDirty"/> to true as well)
        /// </returns>
        public abstract bool Set(string id, string value);
        #endregion

        /// <summary>
        /// Checks if any parameter with a given name was changed before <see cref="Apply"/> or <see cref="Revert"/> was used.
        /// </summary>
        /// <param name="id">ID of the raw parameter to set. It is recommended to follow "@name" pattern.</param>
        /// <returns><c>true</c> if parameter under given <paramref name="id"/> is waiting to be applied.<c>false</c> otherwise.</returns>
        public abstract bool IsPending(string id);

        /// <summary>
        /// Removes a pending <see cref="Set(string, string)"/> action from a raw parameter under given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of the raw parameter to set. It is recommended to follow "@name" pattern.</param>
        /// <returns><c>true</c> if parameter under given <paramref name="id"/> was removed from a pending list.<c>false</c> otherwise.</returns>
        public abstract bool RemoveSet(string id);

        #region Get (with default value)
        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out bool value, bool def = true);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out byte value, byte def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out sbyte value, sbyte def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out short value, short def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out ushort value, ushort def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out char value, char def = '\x00');

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out int value, int def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out uint value, uint def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out long value, long def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out ulong value, ulong def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out float value, float def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out double value, double def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        public abstract void Get(string id, out decimal value, decimal def = 0);

        /// <summary>
        /// Retrieves value of an parameter and strictly expects it to be an string.
        /// </summary>
        /// <param name="id">ID of the raw parameter to set. Usually in a "@name" pattern.</param>
        /// <param name="value">Value to set.</param>
        /// <param name="def">Default value to take if there is no set data in storage.</param>
        public abstract void Get(string id, out string value, string def = "");
        #endregion

        /// <summary>
        /// Registers new parameter.
        /// </summary>
        /// <remarks>
        /// Also tries to look-up data about parameter and deserialize it if possible.
        /// </remarks>
        public abstract void Register(AbstractParameter parameter);

        public abstract AbstractParameter? FindOrThrow(FullName name);
        public abstract AbstractParameter? FindOrThrow(string name);
        public abstract AbstractParameter? Find(FullName name);
        public abstract AbstractParameter? Find(string name);

        public virtual TParameter FindOrThrow<TParameter>(FullName name) where TParameter : AbstractParameter => FindOrThrow<TParameter>(name.Full);
        public virtual TParameter FindOrThrow<TParameter>(string name) where TParameter : AbstractParameter
        {
            if (m_Parameters.TryGetValue(name, out AbstractParameter? finding))
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

        public abstract TParameter? Find<TParameter>(FullName name) where TParameter : AbstractParameter;
        public abstract TParameter? Find<TParameter>(string name) where TParameter : AbstractParameter;

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
        /// (See also: <seealso cref="AbstractParameter.ApplyChangesForceFireCallbacks"/>)
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
                T result = new T();
                m_GameStates[typeof(T)] = result;
                return result;
            }
        }

        /// <summary>
        /// Starts serialization.
        /// </summary>
        /// TODO: Make it possible to use both asynchronous and synchronous saving, to save data right before game closes.
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

        /// <summary>
        /// Retrieves an <see cref="EngineConfiguration"/> file from loaded resources, or creates new instance with <see cref="ScriptableObject.CreateInstance{T}()"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="EngineConfiguration{T}.Instance"/> instead - it is WAY more performant.
        /// </remarks>
        /// <typeparam name="T"><see cref="EngineConfiguration"/> to use.</typeparam>
        /// <returns>Configuration file (existing or default) of a given type.</returns>
        public T GetOrNew<T>() where T : EngineConfiguration => m_EngineConfigurations.GetValueOrDefault(typeof(T)) as T ?? ScriptableObject.CreateInstance<T>();




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
        protected sealed class CoroutineHandler : MonoBehaviour
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
