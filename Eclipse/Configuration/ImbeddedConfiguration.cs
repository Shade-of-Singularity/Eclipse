using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Base class for all configuration files that are imbedded into the game.
    /// Used by <see cref="Eclipse"/> to retrieve settings that are used before <see cref="Engine.Initialize"/> happens.
    /// </summary>
    /// <typeparam name="T">Type of your configuration file.</typeparam>
    public abstract class ImbeddedConfiguration<T> : ImbeddedConfiguration where T : ImbeddedConfiguration<T>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Instance of the <see cref="ImbeddedConfiguration"/> file, loaded from <see cref="Resources"/>.
        /// </summary>
        public static T Instance => m_Instance ??= GetOrNew<T>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static T? m_Instance;
    }

    /// <summary>
    /// Base class for generic <see cref="ImbeddedConfiguration{T}"/> Unity <see cref="Resources"/>-based configuration class.
    /// </summary>
    public abstract class ImbeddedConfiguration : ScriptableObject
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, ImbeddedConfiguration> m_Configurations = new Dictionary<Type, ImbeddedConfiguration>();
        private static bool m_IsInitialized;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves instance of <see cref="ImbeddedConfiguration"/> of a given type (<typeparamref name="T"/>).
        /// If instance do not exist in <see cref="Resources"/> - creates new instance with <see cref="ScriptableObject.CreateInstance{T}()"/>.
        /// </summary>
        /// <remarks>
        /// Newly created value will also be automatically associated with given <typeparamref name="T"/>, as to not create new instance on each call.
        /// </remarks>
        /// <returns>
        /// Existing instance of <see cref="ImbeddedConfiguration"/> from <see cref="Resources"/>
        /// or new instance created with <see cref="ScriptableObject.CreateInstance{T}()"/> method.
        /// </returns>
        public static T GetOrNew<T>() where T : ImbeddedConfiguration
        {
            if (!m_IsInitialized) Initialize();
            if (m_Configurations.TryGetValue(typeof(T), out ImbeddedConfiguration config) && config is T result)
            {
                return result;
            }
            else
            {
                result = CreateInstance<T>();
                m_Configurations[typeof(T)] = result;
                return result;
            }
        }

        /// <summary>
        /// Retrieves instance of <see cref="ImbeddedConfiguration"/> of a given type (<typeparamref name="T"/>).
        /// </summary>
        /// <returns>
        /// Existing instance of <see cref="ImbeddedConfiguration"/> from <see cref="Resources"/>
        /// or provided default value (<paramref name="def"/>).
        /// </returns>
        public static T GetOrDefault<T>(T def) where T : ImbeddedConfiguration
        {
            if (!m_IsInitialized) Initialize();
            if (m_Configurations.TryGetValue(typeof(T), out ImbeddedConfiguration config) && config is T result)
            {
                return result;
            }
            else
            {
                return def;
            }
        }

        /// <summary>
        /// Retrieves instance of <see cref="ImbeddedConfiguration"/> of a given type (<typeparamref name="T"/>).
        /// </summary>
        /// <returns>
        /// Existing instance of <see cref="ImbeddedConfiguration"/> from <see cref="Resources"/>.
        /// If instance fo not exist - calls <paramref name="def"/> instance provider function.
        /// </returns>
        public static T GetOrDefault<T>(Func<T> def) where T : ImbeddedConfiguration
        {
            if (!m_IsInitialized) Initialize();
            if (def is null) throw new NullReferenceException("Default value provider is null.");
            if (m_Configurations.TryGetValue(typeof(T), out ImbeddedConfiguration config) && config is T result)
            {
                return result;
            }
            else
            {
                return def.Invoke();
            }
        }

        /// <summary>
        /// Tries to retrieve <see cref="ImbeddedConfiguration"/> file from <see cref="Resources"/>.
        /// </summary>
        /// <typeparam name="T">Type of your configuration class.</typeparam>
        /// <param name="configuration">Configuration file retrieved from <see cref="Resources"/> or <c>null</c>.</param>
        /// <returns>'<c>true</c>' when file was found and <paramref name="configuration"/> variable was set. '<c>false</c>' if otherwise.</returns>
        public static bool TryGet<T>([NotNullWhen(true)] out T? configuration) where T : ImbeddedConfiguration
        {
            if (!m_IsInitialized) Initialize();
            if (m_Configurations.TryGetValue(typeof(T), out ImbeddedConfiguration config) && config is T result)
            {
                configuration = result;
                return true;
            }
            else
            {
                configuration = null;
                return false;
            }
        }

        /// <summary>
        /// Finds all <see cref="ImbeddedConfiguration"/> files in <see cref="Resources"/> and associates them with their types in cache.
        /// </summary>
        private static void Initialize()
        {
            m_IsInitialized = false;
            m_Configurations.Clear();

            // Adds direct associations.
            ImbeddedConfiguration[] configurations = Resources.LoadAll<ImbeddedConfiguration>("");
            for (int i = 0; i < configurations.Length; i++)
            {
                var configuration = configurations[i];
                m_Configurations[configuration.GetType()] = configuration;
            }

            // Adds "derived" associations, so if you derive and replace base configuration class in Resources, new class will still be used.
            for (int i = 0; i < configurations.Length; i++)
            {
                // Note: if Engine will start parallel association - apply it here as well.
                var configuration = configurations[i];
                Type? type = configuration.GetType()!.BaseType;

                while (!(type is null) && type != typeof(ImbeddedConfiguration))
                {
                    m_Configurations.TryAdd(type, configuration);
                    type = type.BaseType;
                }
            }

            m_IsInitialized = true;
        }
    }
}
