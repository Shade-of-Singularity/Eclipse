using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static ServiceCore.IService;

namespace ServiceCore
{
    /// <summary>
    /// Base for services which are not meant to be changed at runtime.
    /// </summary>
    /// <remarks>
    /// When deciding whether to use <see cref="Service{T}"/> or <see cref="IService{T}"/>, ask yourself:
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
    /// <typeparam Identifier="T">Service implementing this abstract class.</typeparam>
    [IgnoreService]
    public abstract class Service<T> : IService where T : Service<T>
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
        public static bool Initialized => m_Initialized;

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
        static Service()
        {
            Services.Unsafe.RebindServices += RebindService;
            //m_Instance = Services.Get<T>(); // No need, since services instance is provided after RebindService callback.
        }

        private static void RebindService()
        {
            T? service = Services.Get<T>();
            m_Instance = service;
            if (service is null && m_Initialized)
            {
                // Note: is this even a right way to handle it?
                ServiceCoreLogger.LogError($"{Services.LogPrefix} Rebinded to null service ({typeof(T).Name}) without termination! Service state won't reset!");
                return;
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
        /// (as long as <see cref="ServiceAttribute.ExecutionMode"/> is <see cref="ThreadExecutionMode.MainThread"/>)
        /// </para>
        /// </summary>
        /// <returns>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeInitialize(InitializationArgs)"/> to change it.
        /// </returns>
        protected abstract UniTask Initialize(InitializationArgs args);

        /// <summary>
        /// Called when <see cref="Engine"/> terminates all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        /// <remarks>
        /// Doesn't change <see cref="Initialized"/>.
        /// Use <see cref="IService.InvokeTerminate(TerminationArgs)"/> to change it.
        /// </remarks>
        protected abstract UniTask Terminate(TerminationArgs args);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        UniTask IService.InternalInitialize(InitializationArgs args) => Initialize(args);

        /// <inheritdoc/>
        UniTask IService.InternalTerminate(TerminationArgs args) => Terminate(args);




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
        public static bool Exist() => m_Instance is not null;

        /// <summary>
        /// Checks if <see cref="Instance"/> exist and returns it as <paramref name="service"/>.
        /// </summary>
        /// <param name="service">Returned service or <c>null</c>.</param>
        /// <returns><c>true</c> when service <see cref="Exist"/>. <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet([NotNullWhen(true)] out T? service)
        {
            service = m_Instance;
            return service is not null;
        }
    }
}
