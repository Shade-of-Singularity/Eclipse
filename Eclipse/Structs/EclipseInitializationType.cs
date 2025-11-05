namespace Eclipse.Structs
{
    /// <summary>
    /// Specifies when <see cref="Eclipse"/> should be initialized.
    /// </summary>
    /// <remarks>
    /// Depends on <see cref="UnityEngine.RuntimeInitializeLoadType"/>
    /// </remarks>
    public enum EclipseInitializationType : byte
    {
        /// <summary>
        /// Callback invoked when starting up the runtime. Called before the first scene is loaded.
        /// </summary>
        /// <remarks>
        /// See also: <see cref="UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration"/>
        /// </remarks>
        SubsystemRegistration,

        /// <summary>
        /// Callback invoked when all assemblies are loaded and preloaded assets are initialized.
        /// At this time the objects of the first scene have not been loaded yet.
        /// </summary>
        /// <remarks>
        /// See also: <see cref="UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded"/>
        /// </remarks>
        AfterAssembliesLoaded,

        /// <summary>
        ///  Callback invoked before the splash screen is shown.
        ///  At this time the objects of the first scene have not been loaded yet.
        /// </summary>
        /// <remarks>
        /// See also: <see cref="UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen"/>
        /// </remarks>
        BeforeSplashScreen,

        /// <summary>
        /// Callback invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        /// <remarks>
        /// See also: <see cref="UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad"/>
        /// </remarks>
        BeforeSceneLoad,

        /// <summary>
        /// Callback invoked when the first scene's objects are loaded into memory and after Awake has been called.
        /// </summary>
        /// <remarks>
        /// See also: <see cref="UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad"/>
        /// </remarks>
        AfterSceneLoad,

        /// <summary>
        /// Indicates that engine should be manually initialized from your custom code.
        /// Useful when you want to show a warning, message, animation, cutscene, or anything else first, before engine initializes.
        /// </summary>
        /// <remarks>
        /// (TODO) It is possible to make partial initialization only first in such cases.
        /// </remarks>
        Manual,
    }
}
