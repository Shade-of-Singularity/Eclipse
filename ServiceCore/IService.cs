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
using ServiceCore.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Provides fast, type-safe null-safe access to game services registered during initialization.
    /// </summary>
    /// <remarks>
    /// <see cref="Instance"/> property should only be available from interface declaration.
    /// While it will make development a bit harder, it will automatically enforce code structure, needed for proper modding.
    /// (i.e. mod developers will be able to completely override how service behaves)
    /// (Service provider, highest in a dependency tree, will be prioritized)
    /// (Note: Modification developers should refrain from overwriting services though for compatibility)
    /// (Note: Only mod-pack developers working with older game versions should use it, to back-port stuff)
    /// <para>
    /// When deciding whether to use <see cref="Service{T}"/> or <see cref="IService{T}"/>, ask yourself:
    /// </para>
    /// <para>
    /// 1. Are you prototyping right now? If yes - use <see cref="Service{T}"/>.
    /// You don't need to care about modding at this stage. You can just turn it into a Service + <see cref="IService{T}"/> set later.
    /// </para>
    /// <para>
    /// 2. Is functionality of this service has any benefits of being changed by the modders?
    /// For example - changing how configuration manager works might introduce inconsistency in standards.
    /// It has no reason to be customized besides making modding harder and less stable.
    /// (Assuming official game's code has better quality than mods, which is reasonable for commercial projects)
    /// </para>
    /// <para>
    /// 3. Are you preparing for release? Consider both options again.
    /// Using <see cref="IService{T}"/> interfaces might be harder for you,
    /// but might help modders if service has any reason, at all, to be overwritten or expanded on.
    /// </para>
    /// </remarks>
    /// <typeparam Identifier="T">The type of the service to retrieve. Must inherit from <see cref="IService{TService}"/>.</typeparam>
    [IgnoreService] // Were added to allow child services to implement this attribute as well.
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
        /// Might be <c>null</c> when <see cref="Engine"/> is not initializing/initialized.
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
        // TODO: Remove. Descriptor will be useless if you don't use Engine.Initialize(...), so we need to initialize it only when actually needed.
        static IService() => KnownServices.Register(ServiceDescriptor.Construct<T>(ServiceGetter, ServiceSetter));
        private static IService? ServiceGetter() => m_Instance;
        private static void ServiceSetter(IService? service)
        {
            m_Instance = (T?)service;
            if (service is null && m_Initialized)
            {
                // Note: is this even a right way to handle it?
                // Maybe I should schedule termination instead? Then we need a good scheduling system.
                ServiceCoreLogger.LogError($"{Services.LogPrefix} Rebinded to null service ({typeof(T).Name}) without termination! Service state won't reset!");
            }
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
        /// (as long as <see cref="ServiceAttribute.ExecutionMode"/> is <see cref="IService.ThreadExecutionMode.MainThread"/>)
        /// </para>
        /// </summary>
        /// <returns>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeInitialize(IInitializationArgs)"/> to change it.
        /// </returns>
        UniTask Initialize(IInitializationArgs args);

        /// <summary>
        /// Called when <see cref="Engine"/> terminates all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        /// <remarks>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeTerminate(ITerminationArgs)"/> to change it.
        /// </remarks>
        UniTask Terminate(ITerminationArgs args);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        UniTask IService.InternalInitialize(IInitializationArgs args) => Initialize(args);

        /// <inheritdoc/>
        UniTask IService.InternalTerminate(ITerminationArgs args) => Terminate(args);




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
        public static bool Exist() => Instance is not null;

        /// <summary>
        /// Checks if <see cref="Instance"/> exist and returns it as <paramref name="service"/>.
        /// </summary>
        /// <param name="service">Returned service or <c>null</c>.</param>
        /// <returns><c>true</c> when service <see cref="Exist"/>. <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet([NotNullWhen(true)] out T? service) => (service = Instance) is not null;

        /// <summary>
        /// Manually instantiates and initializes this <see cref="IService{T}"/>.
        /// (Not thread-safe)
        /// </summary>
        /// <remarks>
        /// Manual initialization is incompatible with <see cref="Engine.Initialize(InitializationContext, IInitializationArgs?)"/>.
        /// If you decide to use both - make sure that manually initialized services are destroyed before using automatic initialization.
        /// </remarks>
        /// <param name="args">Arguments to provide during initialization. Defaults to terminated <see cref="EngineState"/>.</param>
        /// <returns><see cref="UniTask"/> from <see cref="IService{T}.Initialize(IInitializationArgs)"/> to await.</returns>
        public static async UniTask Instantiate<TService>(IInitializationArgs? args = default)
            where TService : class, IService<T>, new()
        {
            if (m_Instance is not null)
            {
                throw new Exception($"{Engine.LogPrefix} Service ({typeof(T).Name}) is already instantiated.");
            }

            KnownServices.Retrieve<T>()!.Persistent = true;
            // TODO: Schedule it properly.
            // TODO: Call initialization callbacks.
            await (m_Instance = (T)(IService<T>)new TService()).InvokeInitialize(args ?? Engine.State);
        }

        /// <summary>
        /// Manually terminates and destroys this <see cref="IService{T}"/>.
        /// (Not thread-safe)
        /// </summary>
        /// <remarks>
        /// Manual destruction is incompatible with <see cref="Engine.Terminate(ITerminationArgs?)"/>.
        /// Make sure all manually initialized services are destroyed while <see cref="Engine"/> is <see cref="EngineStatus.Initialized"/>.
        /// </remarks>
        /// <param name="args">Arguments to provide to the service for termination. Defaults to terminated <see cref="EngineState"/>.</param>
        /// <returns><see cref="UniTask"/> from <see cref="IService{T}.Terminate(ITerminationArgs)"/> to await.</returns>
        public static async UniTask Destroy(ITerminationArgs? args = default)
        {
            if (m_Instance is null)
            {
                throw new Exception($"{Engine.LogPrefix} Service ({typeof(T).Name}) is already destroyed.");
            }

            KnownServices.Retrieve<T>()!.Persistent = false;
            // TODO: Schedule it properly.
            // TODO: Call termination callbacks.
            await m_Instance.InvokeTerminate(args ?? Engine.State);
            m_Instance = null;
        }
    }




    /// <summary>
    /// Basic interface for a service.
    /// </summary>
    /// <remarks>
    /// For custom services, please use <see cref="IService{TService}"/> interface instead.
    /// This interface is needed only for internal usage and listing in <see cref="Services.List"/>.
    /// </remarks>
    [IgnoreService]
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
        protected UniTask InternalInitialize(IInitializationArgs args);

        /// <summary>
        /// Used to hide this method from <see cref="IService"/> users.
        /// Invokes <see cref="IService{T}.Terminate"/>.
        /// </summary>
        protected UniTask InternalTerminate(ITerminationArgs args);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Calls <see cref="IService{T}.Initialize"/> if service is not <see cref="Initialized"/>.
        /// </summary>
        public async UniTask InvokeInitialize(IInitializationArgs args)
        {
            if (!Initialized)
            {
                try
                {
                    await InternalInitialize(args);
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(new Exception($"Failed to initialize {GetType().Name} service!", ex));
                }

                Initialized = true;
            }
        }

        /// <summary>
        /// Calls <see cref="IService{T}.Terminate"/> is service is <see cref="Initialized"/>.
        /// </summary>
        public async UniTask InvokeTerminate(ITerminationArgs args)
        {
            if (Initialized)
            {
                try
                {
                    await InternalTerminate(args);
                }
                catch (Exception ex)
                {
                    ServiceCoreLogger.LogException(new Exception($"Failed to terminate {GetType().Name} service!", ex));
                }

                Initialized = false;
            }
        }
    }
}
