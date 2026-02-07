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

namespace Eclipse
{
    public static partial class Services
    {
        /// <summary>
        /// Provides methods which are usually unsafe to use unless you know the consequences.
        /// </summary>
        /// <remarks>
        /// For example - changing services here after <see cref="Engine"/> initialization will require 
        /// </remarks>
        public static partial class Unsafe
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                   Events
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Asks systems to cache services in internal fields.
            /// </summary>
            /// <remarks>
            /// Fired before <see cref="OnServicesInitializing"/>
            /// </remarks>
            public static event Action? CacheServices;
            /// <summary>
            /// Asks systems to remove any cached services from internal fields.
            /// </summary>
            /// <remarks>
            /// Fired after <see cref="OnServicesTerminated"/>
            /// </remarks>
            public static event Action? DisposeServices;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Static Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static IDictionary<Type, IService> Dictionary => m_Services;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly Lock m_InitializationLock = new Lock();
            private static readonly Lock m_TerminationLock = new Lock();




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
            /// You are meant to use returned value like that:
            /// <code><![CDATA[
            /// using (Initialize())
            /// {
            ///     // Initialize services here.
            /// }
            /// ]]></code>
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
            /// Fires <see cref="OnServicesTerminating"/> callback and returns special termination lock.
            /// </summary>
            /// <remarks>
            /// After returned value is disposed - fires <see cref="OnServicesTerminated"/> callback.
            /// And after that - automatically clears <see cref="List"/> collection.
            /// </remarks>
            /// <returns>
            /// You are meant to use returned value like that:
            /// <code><![CDATA[
            /// using (Terminate())
            /// {
            ///     // Terminate services here.
            /// }
            /// ]]></code>
            /// </returns>
            public static IDisposable Terminate()
            {
                if (m_TerminationLock.TrySet(FireTerminationCallbacks))
                {
                    OnServicesTerminating?.Invoke();
                }

                return m_TerminationLock;
            }

            /// <summary>
            /// Rebinds values for internal instance fields of <see cref="IService{T}.Instance"/>.
            /// </summary>
            public static void Rebind() => CacheServices?.Invoke();

            /// <summary>
            /// Registers <paramref name="service"/> of a type <typeparamref name="T"/>.
            /// </summary>
            //public static void Set<T>(T service) where T : class, IService
            //{
            //    // TODO: Completely remove associations with old keys.
            //    // TODO: Register the entire tree.
            //    throw new NotImplementedException();
            //}

            /// <summary>
            /// Registers <paramref name="service"/> of a given <paramref name="type"/>.
            /// </summary>
            //public static void Set(Type type, IService service)
            //{
            //    // TODO: Completely remove associations with old keys.
            //    // TODO: Replace with IEngineService.
            //    // TODO: Register the entire tree.
            //    throw new NotImplementedException();
            //}




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
                    Logger.LogException(ex);
                }
            }

            private static void FireTerminationCallbacks()
            {
                try
                {
                    OnServicesTerminated?.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }

                m_Services.Clear();
            }
        }
    }
}
