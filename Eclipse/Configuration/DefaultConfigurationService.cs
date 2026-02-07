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
using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract class DefaultConfigurationService : IConfigurationService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Path to configuration files on the disk.
        /// </summary>
        /// <remarks>
        /// Also path to save files, as <see cref="DefaultConfigurationService"/> also responsible for general serialization.
        /// </remarks>
        public static readonly string ConfigurationPath = $"{Application.persistentDataPath}/Configuration";
        // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eclipse");
        // $"{Application.persistentDataPath}/Configuration";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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
                    Engine.OnEngineInitialized += () => Serialize().Forget();
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
        private static CoroutineHandler? m_CoroutineRunner;

        // Encapsulated Fields:
        private IDataStorage m_Storage = PlayerPreferenceStorage.Instance;

        // Local Fields:
        private readonly Dictionary<Type, EngineConfiguration> m_EngineConfigurations = new Dictionary<Type, EngineConfiguration>();
        private readonly Dictionary<Type, GameState> m_GameStates = new Dictionary<Type, GameState>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public virtual UniTask Initialize() => UniTask.CompletedTask;

        /// <inheritdoc/>
        public virtual UniTask Terminate() => UniTask.CompletedTask;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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
        /// Might not support reverting with custom <see cref="DefaultConfigurationService"/> implementation.
        /// </remarks>
        /// <param name="id">ID of the raw parameter to set. I recommend following "@name" pattern.</param>
        /// <param name="value">New value to set to a parameter.</param>
        /// <returns>
        /// <c>true</c> if last variable value changed. <c>false</c> otherwise.
        /// (If <see cref="IDataStorage"/> or custom <see cref="DefaultConfigurationService"/> doesn't support value checking,
        /// will always return true instead and will set <see cref="IsDirty"/> to true as well)
        /// </returns>
        public abstract bool Set(string id, string value);
        #endregion

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
        public abstract bool RemovePending(string id);

        /// <summary>
        /// Applies all dirty properties.
        /// </summary>
        public void Apply()
        {
            if (IsDirty)
            {
                ApplyForceCallbacks();
            }
        }

        /// <summary>
        /// Applies all properties and fires related callbacks (e.g. <see cref="Parameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// </summary>
        public void ApplyForceCallbacks()
        {
            IConfigurationService.AppliesParameters = true;
            foreach (var parameter in ParameterManager.Parameters)
            {
                parameter.ApplyChangesForceFireCallbacks();
            }

            IConfigurationService.AppliesParameters = false;
        }

        /// <summary>
        /// Reverts all dirty properties.
        /// </summary>
        public void Revert()
        {
            if (IsDirty)
            {
                RevertForceCallbacks();
            }
        }

        /// <summary>
        /// Reverts all properties and fires related callbacks (e.g. <see cref="AbstractParameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// (See also: <seealso cref="AbstractParameter.ApplyChangesForceFireCallbacks"/>)
        /// </summary>
        public void RevertForceCallbacks()
        {
            IConfigurationService.RevertsParameters = true;
            foreach (var parameter in ParameterManager.Parameters)
            {
                parameter.RevertChanges();
            }

            IConfigurationService.RevertsParameters = false;
        }

        /// <summary>
        /// Saves an <see cref="GameState"/> to the configuration file.
        /// </summary>
        /// <remarks>
        /// It will be written to the disk immediately, or in a bit later, if <see cref="DefaultConfigurationService"/> is in
        /// </remarks>
        /// <typeparam name="T">Type of your data.</typeparam>
        public void Save<T>(T data) where T : GameState, new()
        {
            m_GameStates[typeof(T)] = data;
            // ...
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
            if (IConfigurationService.ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            // TODO: Fully implement serialization.
            IConfigurationService.ExecutesSerialization = true;
            await SerializeServicesInternal();
            await SerializeGameStateInternal();
            await SerializeParametersInternal();
            IConfigurationService.ExecutesSerialization = false;
        }

        public async UniTask SerializeServices()
        {
            // TODO: Add locking.
            if (IConfigurationService.ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            IConfigurationService.ExecutesSerialization = true;
            await SerializeServicesInternal();
            IConfigurationService.ExecutesSerialization = false;
        }

        public async UniTask SerializeGameStates()
        {
            // TODO: Add locking.
            if (IConfigurationService.ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            IConfigurationService.ExecutesSerialization = true;
            await SerializeGameStateInternal();
            IConfigurationService.ExecutesSerialization = false;
        }

        public async UniTask SerializeParameters()
        {
            // TODO: Add locking.
            if (IConfigurationService.ExecutesSerialization)
            {
                await WaitForCompletion();
                return;
            }

            IConfigurationService.ExecutesSerialization = true;
            await SerializeParametersInternal();
            IConfigurationService.ExecutesSerialization = false;
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
        public T GetOrNew<T>() where T : EngineConfiguration
        {
            return m_EngineConfigurations.GetValueOrDefault(typeof(T)) as T ?? ScriptableObject.CreateInstance<T>();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private async UniTask WaitForCompletion()
        {
            // TODO: Make more optimized with completion sources.
            while (IConfigurationService.ExecutesSerialization)
            {
                await UniTask.Yield();
            }
        }

        private async UniTask SerializeServicesInternal()
        {
            IConfigurationService.ExecutesServiceSerialization = true;
            uint exceptions = 0;

            foreach (IService service in Services.List)
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
                Debug.LogError($"{IConfigurationService.LogPrefix} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            IConfigurationService.ExecutesServiceSerialization = false;
        }

        private async UniTask SerializeGameStateInternal()
        {
            IConfigurationService.ExecutesGameStateSaving = true;
            uint exceptions = 0;

            foreach (IService service in Services.List)
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
                Debug.LogError($"{IConfigurationService.LogPrefix} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            IConfigurationService.ExecutesGameStateSaving = false;
        }

        private async UniTask SerializeParametersInternal()
        {
            IConfigurationService.ExecutesParameterSaving = true;
            uint exceptions = 0;

            foreach (IService service in Services.List)
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
                Debug.LogError($"{IConfigurationService.LogPrefix} ({exceptions}) {(exceptions == 1 ? "Exception" : "Exceptions")} appeared while trying to serialize services. Look above for more info.");
            }

            IConfigurationService.ExecutesParameterSaving = false;
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