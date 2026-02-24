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
using ServiceCore.Configuration.Storages;
using ServiceCore.Parameters;
using System;

namespace ServiceCore.Configuration
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
    public interface IConfigurationService : IService<IConfigurationService>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <see cref="IConfigurationService"/> is initialized after <see cref="Serialization.ISerializationService"/>,
        /// as it relies on it to serialize parameters.
        /// One of the first to be initialized, as all other services, like <see cref="Localization.DefaultLocalizationService"/>, relies on it.
        /// </summary>
        /// Note: This is why you cannot localize anything here btw.
        public const int InitializationOrder = -1_700_000_000;
        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[ConfigurationService]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether <see cref="IConfigurationService"/> saves anything to the disk at the moment.
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
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="IConfigurationService"/> is going through every <see cref="Service"/> and saves its state to the disk.
        /// </summary>
        /// <remarks>
        /// At the moment engine services simply use <see cref="GameState"/>s for serialization, so this method is added just for the future.
        /// </remarks>
        public static bool ExecutesServiceSerialization
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
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="IConfigurationService"/> is going through every <see cref="GameState"/> and saves it to the disk.
        /// </summary>
        public static bool ExecutesGameStateSaving
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
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="IConfigurationService"/> is going through every <see cref="AbstractParameter"/> and saves its state to the disk.
        /// </summary>
        public static bool ExecutesParameterSaving
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
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="IConfigurationService"/> is applies <see cref="Parameter{TValue}"/> values.
        /// </summary>
        public static bool AppliesParameters
        {
            get => m_AppliesParameters;
            protected set
            {
                if (m_AppliesParameters != value)
                {
                    try
                    {
                        if (m_AppliesParameters = value)
                        {
                            OnBeforeApplyChanges?.Invoke();
                        }
                        else
                        {
                            OnAfterApplyChanges?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Whether <see cref="IConfigurationService"/> is reverts <see cref="Parameter{TValue}"/> values.
        /// </summary>
        public static bool RevertsParameters
        {
            get => m_RevertsParameters;
            protected set
            {
                if (m_RevertsParameters != value)
                {
                    try
                    {
                        if (m_RevertsParameters = value)
                        {
                            OnBeforeRevertChanges?.Invoke();
                        }
                        else
                        {
                            OnAfterRevertChanges?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        ServiceCoreLogger.LogException(ex);
                    }
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static bool m_ExecutesSerialization = false;
        private static bool m_ExecutesServiceSaving = false;
        private static bool m_ExecutesGameStateSaving = false;
        private static bool m_ExecutesParameterSaving = false;
        private static bool m_AppliesParameters = false;
        private static bool m_RevertsParameters = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Custom Get/Set
        /// .        Or in other words: you can see, I wasn't expecting anyone to inherit ConfigurationService XD
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        #region Set (custom variable)
        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, bool value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, byte value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, sbyte value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, short value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, ushort value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, char value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, int value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, uint value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, long value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, ulong value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, float value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, double value);

        /// <summary><inheritdoc cref="Set(string, string)"/></summary>
        bool Set(string id, decimal value);

        /// <summary>
        /// (Supports <see cref="Revert"/>) Allows you to set values to raw parameters which require highest performance (i.e. <see cref="Specs.Cache.L1Cache"/>).
        /// </summary>
        /// <remarks>
        /// Might not support reverting with custom <see cref="IConfigurationService"/> implementation.
        /// </remarks>
        /// <param Identifier="id">ID of the raw parameter to set. I recommend following "@Identifier" pattern.</param>
        /// <param Identifier="value">New value to set to a parameter.</param>
        /// <returns>
        /// <c>true</c> if last variable value changed. <c>false</c> otherwise.
        /// (If <see cref="IDataStorage"/> or custom <see cref="IConfigurationService"/> doesn't support value checking,
        /// will always return true instead and will set <see cref="IsDirty"/> to true as well)
        /// </returns>
        bool Set(string id, string value);
        #endregion

        #region Get (with default value)
        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out bool value, bool def = true);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out byte value, byte def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out sbyte value, sbyte def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out short value, short def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out ushort value, ushort def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out char value, char def = '\x00');

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out int value, int def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out uint value, uint def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out long value, long def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out ulong value, ulong def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out float value, float def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out double value, double def = 0);

        /// <summary><inheritdoc cref="Get(string, out string, string)"/></summary>
        void Get(string id, out decimal value, decimal def = 0);

        /// <summary>
        /// Retrieves value of an parameter and strictly expects it to be an string.
        /// </summary>
        /// <param Identifier="id">ID of the raw parameter to set. Usually in a "@Identifier" pattern.</param>
        /// <param Identifier="value">Value to set.</param>
        /// <param Identifier="def">Default value to take if there is no set data in storage.</param>
        void Get(string id, out string value, string def = "");
        #endregion




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if any parameter with a given Identifier was changed before <see cref="Apply"/> or <see cref="Revert"/> was used.
        /// </summary>
        /// <param Identifier="id">ID of the raw parameter to set. It is recommended to follow "@Identifier" pattern.</param>
        /// <returns><c>true</c> if parameter under given <paramref Identifier="id"/> is waiting to be applied.<c>false</c> otherwise.</returns>
        abstract bool IsPending(string id);

        /// <summary>
        /// Removes a pending <see cref="Set(string, string)"/> action from a raw parameter under given <paramref Identifier="id"/>.
        /// </summary>
        /// <param Identifier="id">ID of the raw parameter to set. It is recommended to follow "@Identifier" pattern.</param>
        /// <returns><c>true</c> if parameter under given <paramref Identifier="id"/> was removed from a pending list.<c>false</c> otherwise.</returns>
        abstract bool RemovePending(string id);

        /// <summary>
        /// Applies all dirty properties.
        /// </summary>
        void Apply();

        /// <summary>
        /// Applies all properties and fires related callbacks (e.g. <see cref="Parameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// </summary>
        void ApplyForceCallbacks();

        /// <summary>
        /// Reverts all dirty properties.
        /// </summary>
        void Revert();

        /// <summary>
        /// Reverts all properties and fires related callbacks (e.g. <see cref="AbstractParameter{TValue}.OnValueApplied"/>)
        /// regardless of whether parameter actually changed.
        /// (See also: <seealso cref="AbstractParameter.ApplyChangesForceFireCallbacks"/>)
        /// </summary>
        void RevertForceCallbacks();

        /// <summary>
        /// Saves an <see cref="GameState"/> to the configuration file.
        /// </summary>
        /// <remarks>
        /// It will be written to the disk immediately, or in a bit later, if <see cref="DefaultConfigurationService"/> is in
        /// </remarks>
        /// <typeparam Identifier="T">Type of your data.</typeparam>
        void Save<T>(T data) where T : GameState, new();

        /// <summary>
        /// Loads <see cref="GameState"/> from a configuration file.
        /// </summary>
        /// <remarks>
        /// You can use it to load game data, such as player positions, inventory data, etc.
        /// </remarks>
        /// <returns>
        /// New <typeparamref Identifier="T"/> parameter if no configuration file found.
        /// Otherwise - loaded data.
        /// </returns>
        /// <typeparam Identifier="T"></typeparam>
        T Load<T>() where T : GameState, new();

        /// <summary>
        /// Starts full serialization.
        /// </summary>
        /// TODO: Make it possible to use both asynchronous and synchronous saving, to save data right before game closes.
        UniTask Serialize();

        /// <summary>
        /// Serializes service states.
        /// </summary>
        UniTask SerializeServices();

        /// <summary>
        /// Serializes game states.
        /// </summary>
        UniTask SerializeGameStates();

        /// <summary>
        /// Serializes all parameters in <see cref="ParameterManager"/>.
        /// </summary>
        /// <returns></returns>
        UniTask SerializeParameters();

        /// <summary>
        /// Allows you to overwrite engine configurations at runtime.
        /// </summary>
        /// <remarks>
        /// Do it only if you want to completely overhaul how game works. Otherwise - do NOT touch it.
        /// <para>
        /// Make sure that <see cref="EngineConfiguration{T}.Instance"/> is NEVER called before you call this one.
        /// </para>
        /// </remarks>
        /// <typeparam Identifier="T"></typeparam>
        /// <param Identifier="value"></param>
        void Set<T>(T value) where T : EngineConfiguration;

        /// <summary>
        /// Retrieves an <see cref="EngineConfiguration"/> file from loaded resources, or creates new m_Instance with <see cref="ScriptableObject.CreateInstance{T}()"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="EngineConfiguration{T}.Instance"/> instead - it is WAY more performant.
        /// </remarks>
        /// <typeparam Identifier="T"><see cref="EngineConfiguration"/> to use.</typeparam>
        /// <returns>Configuration file (existing or default) of a given type.</returns>
        T GetOrNew<T>() where T : EngineConfiguration;
    }
}
