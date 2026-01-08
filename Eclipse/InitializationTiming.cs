namespace Eclipse
{
    /// <summary>
    /// Indicates when <see cref="EclipseInitializeAttribute"/> should run .ctor on static class.
    /// If <see cref="EclipseInitializeAttribute"/> is attached to a method - will run method instead.
    /// </summary>
    /// <remarks>
    /// Supports flags to be used with static methods.
    /// However, static .ctor can run only once, so in their case first-to-appear timing will be used.
    /// </remarks>
    /// Also, uses <see cref="ushort"/> instead of <see cref="byte"/> in case we will need 10-12 callbacks.
    public enum InitializationTiming : ushort
    {
        /// <summary>
        /// Initialization will never happen.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Runs this callback as soon as mod assembly is loaded.
        /// </summary>
        /// <remarks>
        /// For built-in assemblies, or assemblies loaded before <see cref="Engine.Initialize"/> is run
        /// - <see cref="Engine"/> will run callbacks with this attribute almost immediately on <see cref="Engine.Initialize"/> call.
        /// <para>
        /// Note: <see cref="BeforeEngineInitialization"/> will run only after ALL mods were loaded and ALL <see cref="OnAssemblyLoad"/> was run.
        /// This guarantees that <see cref="OnAssemblyLoad"/> will 100%, always, run before any <see cref="BeforeEngineInitialization"/> will run.
        /// </para>
        /// </remarks>
        /// (See me breaking the note above immediately by mistake XD)
        OnAssemblyLoad = 0b0000_0000_0000_0001,

        /// <summary>
        /// Runs before service ever starts.
        /// </summary>
        BeforeEngineInitialization = 0b0000_0000_0000_0010,

        /// <summary>
        /// Runs after all engine initialization events are completed.
        /// </summary>
        AfterEngineInitialization = 0b1000_0000_0000_0000,
    }
}
