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

using System;
using System.Collections.Generic;

namespace ServiceCore
{
    public static partial class Services
    {
        /// <summary>
        /// Provides methods which are usually unsafe to use unless you know the consequences.
        /// </summary>
        /// <remarks>
        /// For example - changing services here after <see cref="Engine"/> initialization will require 
        /// </remarks>
        /// TODO: Add partial service initialization.
        public static partial class Unsafe
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                   Events
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Asks systems to recache all services in internal fields.
            /// Called after services were added or all services were removed.
            /// </summary>
            public static event Action? RebindServices;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Static Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static IDictionary<Type, ServiceEntry> Dictionary => m_Services;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly Handle m_InitializationHandle = new(FireInitializedEvents);
            private static readonly Handle m_TerminationHandle = new(FireTerminationEvents);
            private static readonly Handle m_RebindHandle = new(FireRebindEvents);




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Fires <see cref="OnServicesInitializing"/> event and returns special unloading lock.
            /// </summary>
            /// <remarks>
            /// After returned value is disposed - fires <see cref="OnServicesInitialized"/> event.
            /// Does not fire <see cref="RebindServices"/> event.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value like that:
            /// <code><![CDATA[
            /// using (Initialize()) // Auto-fires OnServicesInitializing event.
            /// {
            ///     // Initialize *existing* services.
            ///     // ...
            /// }
            /// // Fires OnServicesInitialized event.
            /// ]]></code>
            /// </returns>
            public static IDisposable Initialize()
            {
                OnServicesInitializing?.Invoke();
                return m_InitializationHandle.Activate();
            }

            /// <summary>
            /// Fires <see cref="OnServicesTerminating"/> event and returns special termination lock.
            /// </summary>
            /// <remarks>
            /// After returned value is disposed - fires <see cref="OnServicesTerminated"/> event.
            /// Does not fire <see cref="RebindServices"/> event.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value like that:
            /// <code><![CDATA[
            /// using (Terminate()) // Auto-fires OnServicesTerminating event.
            /// {
            ///     // Terminate services.
            ///     // ...
            /// }
            /// // Auto-fires OnServicesTerminated event.
            /// ]]></code>
            /// </returns>
            public static IDisposable Terminate()
            {
                OnServicesTerminating?.Invoke();
                return m_TerminationHandle.Activate();
            }

            /// <summary>
            /// Rebinds values for internal instance fields of <see cref="IService{T}.Instance"/>.
            /// </summary>
            /// <remarks>
            /// Does so by firing <see cref="RebindServices"/> event.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value like that:
            /// <code><![CDATA[
            /// using (Rebind()) // Auto-fires nothing.
            /// {
            ///     // Add or Remove services.
            /// }
            /// // Auto-fires RebindServices event.
            /// ]]></code>
            /// </returns>
            public static IDisposable Rebind() => m_RebindHandle.Activate();

            /// <summary>
            /// Sets or Replaces existing service in internal service collection.
            /// </summary>
            /// <param Identifier="service">Service to register.</param>
            /// TODO: Add locking for internal dictionary.
            public static void Set(IService service)
            {
                if (service is null)
                {
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Attempted to register null service.");
                    return;
                }

                SetUnchecked(ServiceEntry.Construct(service));
            }

            /// <summary>
            /// Sets or Replaces existing service in internal service collection.
            /// </summary>
            /// <remarks>
            /// Internally, service is replaced on if *any* conflict between 
            /// </remarks>
            /// <param Identifier="entry">Service entry to register.</param>
            /// TODO: Add locking for internal dictionary.
            public static void Set(ServiceEntry entry)
            {
                if (entry.service is null)
                {
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Attempted to register null service.");
                    return;
                }

                SetUnchecked(entry);
            }

            /// <summary>
            /// Removes given service from a service list.
            /// </summary>
            /// <returns><inheritdoc cref="Remove(Type)"/></returns>
            public static bool Remove(IService service) => Remove(service.GetType());

            /// <summary>
            /// Removes service under given association <paramref Identifier="key"/> from a service list.
            /// </summary>
            /// <returns>
            /// <c>true</c> if service was removed.
            /// <c>false</c> if there was no service under given <paramref Identifier="key"/> to begin with.
            /// </returns>
            public static bool Remove(Type key)
            {
                if (m_Services.TryGetValue(key, out ServiceEntry entry))
                {
                    Array.ForEach(entry.associations, a => m_Services.Remove(a));
                    return true;
                }

                return false;
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static void FireInitializedEvents()
            {
                try
                {
                    OnServicesInitialized?.Invoke();
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(ex);
                }
            }

            private static void FireTerminationEvents()
            {
                try
                {
                    OnServicesTerminated?.Invoke();
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(ex);
                }
            }

            private static void FireRebindEvents()
            {
                try
                {
                    RebindServices?.Invoke();
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(ex);
                }
            }

            private static void SetUnchecked(ServiceEntry entry)
            {
                // Registers service.
                for (int i = 0; i < entry.associations.Length; i++)
                {
                    Type association = entry.associations[i];
                    if (association is null) continue;

                    if (!m_Services.TryAdd(association, entry))
                    {
                        // Overwrites previously existing service entirely.
                        ServiceEntry existing = m_Services[association];
                        Array.ForEach(existing.associations, a => m_Services.Remove(a));
                        m_Services[association] = entry;
                    }
                }
            }
        }
    }
}
