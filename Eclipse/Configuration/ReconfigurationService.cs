namespace Eclipse.Configuration
{
    /// <summary>
    /// Reconfiguration service for you to target with <see cref="ServicePreloadMethodAttribute"/>s or <see cref="ServiceAfterloadMethodAttribute"/>s
    /// in order to reconfigure <see cref="EngineConfiguration{T}"/>s.
    /// </summary>
    /// <remarks>
    /// Cannot be inherited as it should not be replaced by any other service.
    /// </remarks>
    [Service(InitializationOrder = InitializationOrder, ThreadExecutionOrder = ServiceAttribute.ThreadExecutionMode.MainThread)]
    public sealed class ReconfigurationService : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const int InitializationOrder = ConfigurationService.InitializationOrder + 1000;
        /// <see cref="ReconfigurationService"/> itself doesn't reconfigure anything.
        /// Code example: <![CDATA[
        /// [ServicePreloadMethod(typeof(ReconfigurationService), InvokeOrder = 0, ThreadSafe = false)]
        /// public static void AllowAutoSave()
        /// {
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSave = true;
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSaveDelay = 10f;
        /// }
        /// ]]>
    }
}
