namespace Eclipse
{
    /// <summary>
    /// Current status of the engine.
    /// </summary>
    public enum EngineStatus : byte
    {
        /// <summary>
        /// (Default) Engine is unloaded and waits for <see cref="Engine.Initialize"/> to be used.
        /// </summary>
        /// <remarks>
        /// If automatic initialization enabled in <see cref="EclipseConfiguration"/> by setting <see cref="EclipseConfiguration.InitializationType"/>
        /// to anything but <see cref="AutomaticStartupType.Manual"/> - will be in unloaded state very briefly.
        /// </remarks>
        Unloaded = 0b0000_0000,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is in active initialization right now.
        /// Will be set to <see cref="Initialized"/> once initialization is finished.
        /// </summary>
        Initializing = 0b0000_0001,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is fully initialized.
        /// </summary>
        Initialized = 0b0000_0010,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is being unloaded.
        /// Will be set to <see cref="Unloaded"/> once unloading is finished.
        /// </summary>
        Unloading = 0b0000_0100,

        /// <summary>
        /// Represents that <see cref="Engine"/> got irreversibly broken during <see cref="Initializing"/> and cannot be restored without full application reloading.
        /// </summary>
        /// <remarks>
        /// After thorough engine testing, should only happen if mods or your custom code mess-up the system soo much, that we just can't do anything about it.
        /// </remarks>
        InitializationBroken = 0b0100_0000,

        /// <summary>
        /// Represents that <see cref="Engine"/> got irreversibly broken during <see cref="Unloading"/> and cannot be restored without full application reloading.
        /// </summary>
        /// <remarks>
        /// After thorough engine testing, should only happen if mods or your custom code mess-up the system soo much, that we just can't do anything about it.
        /// </remarks>
        UnloadingBroken = 0b1000_0000,
    }
}
