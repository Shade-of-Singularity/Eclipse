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
using System;
using System.Runtime.CompilerServices;
using static Eclipse.IService;

namespace Eclipse
{
    /// <summary>
    /// Provides fast, type-safe null-safe access to game services registered during initialization.
    /// </summary>
    /// <remarks>
    /// <see cref="Instance"/> property should only be available from interface declaration.
    /// While it will make development a bit harder, it will automatically enforce code structure, needed for proper modding.
    /// (i.e. mod developers will be able to completely override how service behaves)
    /// (Service provider, highest in a dependency tree, will be prioritized)
    /// (Note: Mod developers should refrain from overwriting services though for compatibility)
    /// (Note: Only mod-pack developers working with older game versions should use it, to back-port stuff)
    /// </remarks>
    /// <typeparam name="T">The type of the service to retrieve. Must inherit from <see cref="IService{TService}"/>.</typeparam>
    public interface IService<T> : IService where T : class, IService<T>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Cached type-safe null-safe instance of the service. ~x40 times faster than <see cref="Services.Get{T}()"/>!
        /// </summary>
        /// <remarks>
        /// Might be <c>null</c> when <see cref="Engine"/> is not initialized.
        /// </remarks>
        public static T Instance => m_Instance!; // Marks as non-null as it will be non-null after Engine initialization.

        /// <summary>
        /// Whether service was initialized or not.
        /// Safe to access with <see cref="Instance"/> set to <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Always <c>true</c> after <see cref="Engine.Status"/> is set to <see cref="EngineStatus.Initialized"/>.
        /// Might be <c>false</c> during initialization.
        /// Note: Services are initialized base on <see cref="ServiceAttribute.ExecutionOrder"/>.
        /// </remarks>
        public static new bool Initialized => m_Initialized;

        /// <summary>
        /// Flag implementation to access static <see cref="Initialized"/> field.
        /// </summary>
        bool IService.Initialized
        {
            get => m_Initialized;
            set => m_Initialized = value;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Internal instance of a service.
        /// </summary>
        private static T? m_Instance;
        /// <summary>
        /// Internal null-safe flag for checking for initialization.
        /// </summary>
        private static bool m_Initialized;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                           Initialization / Reset
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static IService()
        {
            Services.Unsafe.DisposeServices += DisposeService;
            Services.Unsafe.CacheServices += CacheService;
            m_Instance = Services.Get<T>();
        }

        private static void DisposeService()
        {
            m_Initialized = false;
            m_Instance = null;
        }

        private static void CacheService()
        {
            m_Instance = Services.Get<T>();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Called when <see cref="Engine"/> initializes all the code and resources from the memory.
        /// <para>
        /// Unlike any <see cref="IService"/> .ctor (constructor), this method is thread-safe.
        /// (as long as <see cref="ServiceAttribute.ExecutionMode"/> is <see cref="ThreadExecutionMode.MainThread"/>)
        /// </para>
        /// </summary>
        /// <returns>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeInitialize()"/> to change it.
        /// </returns>
        UniTask Initialize();

        /// <summary>
        /// Called when <see cref="Engine"/> terminates all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        /// <remarks>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeTerminate()"/> to change it.
        /// </remarks>
        UniTask Terminate();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        UniTask IService.InternalInitialize() => Initialize();
        UniTask IService.InternalTerminate() => Terminate();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if <see cref="Instance"/> exist.
        /// </summary>
        /// <remarks>
        /// During <see cref="Engine"/> initialization, even if service exist, <see cref="Initialized"/> might be <c>false</c>.
        /// Use <see cref="Initialized"/> itself instead.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exist() => !(m_Instance is null);
    }




    /// <summary>
    /// Basic interface for a service.
    /// </summary>
    /// <remarks>
    /// For custom services, please use <see cref="IService{TService}"/> interface instead.
    /// This interface is needed only for internal usage and listing in <see cref="Services.List"/>.
    /// </remarks>
    public partial interface IService
    {
        /// <summary>
        /// Flags whether <see cref="InvokeInitialize"/> was called on a service or not.
        /// </summary>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Used to hide this method from <see cref="IService"/> users.
        /// Invokes <see cref="IService{T}.Initialize"/>.
        /// </summary>
        protected UniTask InternalInitialize();

        /// <summary>
        /// Used to hide this method from <see cref="IService"/> users.
        /// Invokes <see cref="IService{T}.Terminate"/>.
        /// </summary>
        protected UniTask InternalTerminate();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Calls <see cref="IService{T}.Initialize"/> if service is not <see cref="Initialized"/>.
        /// </summary>
        public async UniTask InvokeInitialize()
        {
            if (!Initialized)
            {
                try
                {
                    await InternalInitialize();
                }
                catch (Exception ex)
                {
                    Logger.LogException(new Exception($"Failed to initialize {GetType().Name} service!", ex));
                }

                Initialized = true;
            }
        }

        /// <summary>
        /// Calls <see cref="IService{T}.Terminate"/> is service is <see cref="Initialized"/>.
        /// </summary>
        public async UniTask InvokeTerminate()
        {
            if (Initialized)
            {
                try
                {
                    await InternalTerminate();
                }
                catch (Exception ex)
                {
                    Logger.LogException(new Exception($"Failed to terminate {GetType().Name} service!", ex));
                }

                Initialized = false;
            }
        }
    }
}
