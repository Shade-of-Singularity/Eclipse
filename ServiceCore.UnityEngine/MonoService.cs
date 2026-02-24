using Cysharp.Threading.Tasks;
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
        private static bool m_Initialized;
        private static T? m_Instance;




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
            m_Instance ??= (T)this;
            ((IService)this).InvokeInitialize().Forget();
        }

        /// <summary>
        /// Called right before <see cref="MonoBehaviour"/> is destroyed.
        /// </summary>
        protected virtual void Destroy()
        {
            ((IService)this).InvokeTerminate().Forget();
            m_Instance = default;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        UniTask IService.InternalInitialize() => Initialize();

        /// <inheritdoc/>
        UniTask IService.InternalTerminate() => Terminate();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="IService{T}.Initialize"/>
        protected abstract UniTask Initialize();

        /// <inheritdoc cref="IService{T}.Terminate"/>
        protected abstract UniTask Terminate();
    }
}
