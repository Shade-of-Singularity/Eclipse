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
using System.Runtime.CompilerServices;

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
            /// .                                               Static Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly Handle m_InitializationHandle = new(FireInitializedEvents);
            private static readonly Handle m_TerminationHandle = new(FireTerminationEvents);




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
            /// Sets or Replaces existing service in internal service collection.
            /// </summary>
            /// <param Identifier="service">Service to register.</param>
            public static void Set(IService service)
            {
                if (service is null)
                {
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Attempted to register null service.");
                    return;
                }

                SetUnchecked(service);
            }

            /// <summary>
            /// Removes service under given association <paramref Identifier="key"/> from a service list.
            /// </summary>
            /// <returns>
            /// <c>true</c> if service was removed.
            /// <c>false</c> if there was no service under given <paramref Identifier="key"/> to begin with.
            /// </returns>
            public static bool Remove(IService service)
            {
                if (service is null)
                {
                    ServiceCoreLogger.LogWarning($"{LogPrefix} Attempted to remove null service.");
                    return false;
                }

                return RemoveUnchecked(service.GetType());
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

            private static void SetUnchecked(IService service)
            {
                lock (m_Services)
                {
                    ServiceDescriptor descriptor = service.GetDescriptor();
                    ActiveService entry = new(service, descriptor);

                    // Registers service.
                    Type[] associations = descriptor.Associations;
                    for (int i = 0; i < associations.Length; i++)
                    {
                        Type association = associations[i];
                        if (association is null) continue;

                        if (!m_Services.TryAdd(association, entry))
                        {
                            // Removes previously existing service entirely.
                            ActiveService existing = m_Services[association];
                            Array.ForEach(descriptor.Associations, static a => m_Services.Remove(a));
                            descriptor.Setter(null);

                            m_Services[association] = entry;
                        }
                    }

                    // Updates underlying Instance field.
                    descriptor.Setter(service);
                }
            }

            private static bool RemoveUnchecked(Type key)
            {
                lock (m_Services)
                {
                    if (m_Services.TryGetValue(key, out ActiveService entry))
                    {
                        Array.ForEach(entry.Descriptor.Associations, static a => m_Services.Remove(a));
                        entry.Descriptor.Setter(null);
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}
