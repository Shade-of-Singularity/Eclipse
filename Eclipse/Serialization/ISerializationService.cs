namespace Eclipse.Serialization
{
    /// <summary>
    /// Serialization service which adjusts how serialization happens in the entire Engine.
    /// Methods for serialization can be retrieved via <see cref="Serializers"/> and <see cref="Serializers{TValue}"/> classes.
    /// </summary>
    /// <remarks>
    /// <see cref="ISerializationService"/> adds default serialization methods to the list,
    /// so use <see cref="AfterServiceInitializedAttribute"/> to add your own methods after default ones are provided.
    /// </remarks>
    public interface ISerializationService : IService<ISerializationService>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// One of the first services in the game to be initialized,
        /// as core things like <see cref="Configuration.DefaultConfigurationService"/> relies on it.
        /// </summary>
        public const int InitializationOrder = -1_800_000_000;
        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[SerializationService]";
    }
}
