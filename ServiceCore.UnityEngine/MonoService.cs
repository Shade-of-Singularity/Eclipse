using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ServiceCore
{
    /// <summary>
    /// Service based on <see cref="MonoBehaviour"/>
    /// </summary>
    /// <remarks>
    /// Essentially just your regular singleton, but initializes in async mode.
    /// </remarks>
    /// <typeparam Identifier="T"></typeparam>
    [ServiceIdentifier]
    public abstract class MonoService<T> : MonoBehaviour, IService where T : MonoService<T>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
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
        public static bool Initialized => m_Initialized;

        /// <summary>
        /// <see cref="ServiceDescriptor"/> for this service.
        /// </summary>
        public static ServiceDescriptor Descriptor => m_Descriptor;

        /// <summary>
        /// Implementation to access static <see cref="Descriptor"/> field from <see cref="IService"/> instance.
        /// </summary>
        ServiceDescriptor IService.Descriptor => m_Descriptor;

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
        /// <summary>
        /// <see cref="ServiceDescriptor"/> for this service.
        /// </summary>
        private static readonly ServiceDescriptor m_Descriptor = ServiceDescriptor.Construct<MonoService<T>>(ServiceGetter, ServiceSetter);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                           Initialization / Reset
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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
        /// .                                               Unity Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Called right after <see cref="MonoBehaviour"/> and its fields were initialized.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_Instance)
            {
                // TODO: Introduce "Keep(mode: KeepInstance.Newer / KeepInstance.Older)" attribute, and load it in a static .ctor.
                ServiceCoreLogger.LogWarning($"{Engine.LogPrefix} New service of a type ({GetType().Name}) was instantiated, but resolution is not supported yet. New service will be destroyed by default.");
                Destroy(this);
                return;
            }

            // TODO: Multi-thread properly.
            ServiceSetter(this);
            DontDestroyOnLoad(this);
            // TODO: Schedule via Engine or postpone until previous service is initialized. Use custom initialization args.
            ((IService)this).InvokeInitialize(Engine.State).Forget();
        }

        /// <summary>
        /// Called right before <see cref="MonoBehaviour"/> is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (m_Instance == this)
            {
                // TODO: Schedule via Engine or postpone until previous service is terminated. Use custom termination args.
                ((IService)this).InvokeTerminate(Engine.State).Forget();
                ServiceSetter(null);
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        UniTask IService.InternalInitialize(IInitializationArgs args) => Initialize(args);

        /// <inheritdoc/>
        UniTask IService.InternalTerminate(ITerminationArgs args) => Terminate(args);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="IService{T}.Initialize"/>
        protected abstract UniTask Initialize(IInitializationArgs args);

        /// <inheritdoc cref="IService{T}.Terminate"/>
        protected abstract UniTask Terminate(ITerminationArgs args);




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
        public static UniTask Instantiate(IInitializationArgs? args = default)
        {
            if (m_Instance)
            {
                throw new Exception($"{Engine.LogPrefix} MonoService ({typeof(T).Name}) is already instantiated.");
            }

            var descriptors = ServiceRanges<T>.Range.Descriptors;
            if (descriptors.Length == 0)
            {
                throw new Exception($"{Engine.LogPrefix} MonoService {typeof(T).Name} doesn't have any {nameof(ServiceIdentifierAttribute)}s defined in the inheritance tree.");
            }

            T instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(instance);
            for (int i = 0; i < descriptors.Length; i++)
            {
                ServiceDescriptor descriptor = descriptors[i];
                descriptor.Setter(instance); // Intentionally overwrites service reference.
                descriptor.Persistent = true;
            }

            // TODO: Schedule it properly.
            // TODO: Call initialization callbacks.
            return ((IService)instance).InvokeInitialize(args ?? Engine.State);
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
            if (!m_Instance)
            {
                throw new Exception($"{Engine.LogPrefix} MonoService ({typeof(T).Name}) is already destroyed or was never initialized.");
            }

            if (!Descriptor.Persistent)
            {
                throw new Exception($"{Engine.LogPrefix} Cannot manually destroy automatically initialized MonoService ({typeof(T).Name}).");
            }

            var descriptors = ServiceRanges<T>.Range.Descriptors;
            if (descriptors.Length == 0)
            {
                throw new Exception($"{Engine.LogPrefix} MonoService {typeof(T).Name} doesn't have any {nameof(ServiceIdentifierAttribute)}s defined in the inheritance tree.");
            }

            // TODO: Schedule it properly.
            // TODO: Call termination callbacks.
            await ((IService)m_Instance).InvokeTerminate(args ?? Engine.State);

            for (int i = 0; i < descriptors.Length; i++)
            {
                ServiceDescriptor descriptor = descriptors[i];
                if (descriptor.Getter() == m_Instance)
                {
                    descriptor.Setter(null);
                    descriptor.Persistent = false;
                }
            }

            Destroy(m_Instance.gameObject);
        }
    }
}
