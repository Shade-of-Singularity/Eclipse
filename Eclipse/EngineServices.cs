using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Eclipse
{
    /// <summary>
    /// Stores references to all the services, to not overload <see cref="Engine"/>.
    /// </summary>
    public static class EngineServices
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invoked when services was instantiated, but haven't initialized yet.
        /// </summary>
        public static event Action? OnServicesInitializing;
        /// <summary>
        /// Invoked when services was fully initialized.
        /// </summary>
        public static event Action? OnServicesInitialized;
        /// <summary>
        /// Invoked when services are about to be unloaded. Fires after 'Engine.OnEngineUnloading' (TODO: Add reference).
        /// </summary>
        public static event Action? OnServicesUnloading;
        /// <summary>
        /// Invoked when services was fully unloaded.
        /// </summary>
        public static event Action? OnServicesUnloaded;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static IReadOnlyCollection<EngineService> Services => m_Services.Values;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, EngineService> m_Services = new Dictionary<Type, EngineService>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if there is service with requested type.
        /// </summary>
        public static bool Has<T>() where T : EngineService
        {
            return m_Services.TryGetValue(typeof(T), out EngineService result) && result is T;
        }

        /// <summary>
        /// Checks if there is a service with requested <paramref name="type"/>.
        /// </summary>
        public static bool Has(Type type)
        {
            return m_Services.TryGetValue(type, out EngineService result) && result.GetType() == type;
        }


        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="EngineService{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        public static T? Get<T>() where T : EngineService
        {
            return m_Services.GetValueOrDefault(typeof(T)) as T;
        }

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="EngineService{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        public static EngineService? Get(Type type)
        {
            if (m_Services.TryGetValue(type, out EngineService service) && service.GetType() == type)
            {
                return service;
            }

            return default;
        }

        


        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="EngineService{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        public static bool TryGet<T>([NotNullWhen(true)] out T? service) where T : EngineService
        {
            if (m_Services.TryGetValue(typeof(T), out EngineService result) && result is T t)
            {
                service = t;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="EngineService{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        public static bool TryGet(Type type, [NotNullWhen(true)] out EngineService? service)
        {
            return m_Services.TryGetValue(type, out service) && service.GetType() == type;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Initialization
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static class Unsafe
        {
            /// <summary>
            /// Used for determining when to fire <see cref="OnServicesInitialized"/> and similar events.
            /// </summary>
            public sealed class Lock : IDisposable
            {
                private readonly object _lock = new object();
                private volatile Action? m_Callback;
                internal bool TrySet(Action callback)
                {
                    lock (_lock)
                    {
                        if (m_Callback is null)
                        {
                            m_Callback = callback;
                            return true;
                        }

                        return false;
                    }
                }

                /// <summary>
                /// Used to fire target event.
                /// </summary>
                public void Dispose()
                {
                    lock (_lock)
                    {
                        m_Callback?.Invoke();
                        m_Callback = null;
                    }
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Static Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static IReadOnlyDictionary<Type, EngineService> Dictionary => m_Services;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly Lock m_InitializationLock = new Lock();
            private static readonly Lock m_UnloadingLock = new Lock();




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Fires <see cref="OnServicesInitializing"/> callback and returns special unloading lock.
            /// </summary>
            /// <remarks>
            /// After returned value is disposed - fires <see cref="OnServicesInitialized"/> callback.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value in a 'using(Initialize()) { ... }' statement.
            /// </returns>
            public static IDisposable Initialize()
            {
                if (m_InitializationLock.TrySet(FireInitializedCallbacks))
                {
                    m_Services.Clear();
                    OnServicesInitializing?.Invoke();
                }

                return m_InitializationLock;
            }

            /// <summary>
            /// Fires <see cref="OnServicesUnloading"/> callback and returns special unloading lock.
            /// </summary>
            /// <remarks>
            /// After returned value is disposed - fires <see cref="OnServicesUnloaded"/> callback.
            /// And after that - automatically clears <see cref="Services"/> collection.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value in a 'using(Unload()) { ... }' statement.
            /// </returns>
            public static IDisposable Unload()
            {
                if (m_UnloadingLock.TrySet(FireUnloadedCallbacks))
                {
                    OnServicesUnloading?.Invoke();
                }

                return m_UnloadingLock;
            }

            /// <summary>
            /// Registers <paramref name="service"/> of a type <typeparamref name="T"/>.
            /// </summary>
            public static void Set<T>(T service)
            {
                // TODO: Completely remove associations with old keys.
                // TODO: Register the entire tree.
                throw new NotImplementedException();
            }

            /// <summary>
            /// Registers <paramref name="service"/> of a given <paramref name="type"/>.
            /// </summary>
            public static void Set(Type type, EngineService service)
            {
                // TODO: Completely remove associations with old keys.
                // TODO: Replace with IEngineService.
                // TODO: Register the entire tree.
                throw new NotImplementedException();
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static void FireInitializedCallbacks()
            {
                try
                {
                    OnServicesInitialized?.Invoke();
                }
                catch (Exception ex)
                {
                    EngineLogger.LogException(ex);
                }
            }

            private static void FireUnloadedCallbacks()
            {
                try
                {
                    OnServicesUnloaded?.Invoke();
                }
                catch (Exception ex)
                {
                    EngineLogger.LogException(ex);
                }

                m_Services.Clear();
            }
        }
    }
}
