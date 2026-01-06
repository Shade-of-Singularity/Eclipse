using System;
using System.Collections.Generic;
using System.Text;

namespace Eclipse.Serialization
{
    /// <summary>
    /// Serialization service which adjusts how serialization happens in the entire Engine.
    /// Methods for serialization can be retrieved via <see cref="Serializers"/> and <see cref="Serializers{TValue}"/> classes.
    /// </summary>
    /// <remarks>
    /// <see cref="SerializationService"/> adds default serialization methods to the list,
    /// so use <see cref="ServiceAfterloadMethodAttribute"/> to add your own methods after default ones are provided.
    /// </remarks>
    [Service(InitializationOrder = InitializationOrder, Dispose = true, ThreadExecutionOrder = ServiceAttribute.ThreadExecutionMode.MainThread)]
    public sealed class SerializationService : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// One of the first services in the game to be initialized, as core things like <see cref="Configuration.ConfigurationService"/> relies on it.
        /// </summary>
        public const int InitializationOrder = -1_800_000_000;

        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[" + nameof(SerializationService) + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        protected override void Initialize() { }

        /// <inheritdoc/>
        protected override void Unload() { }
    }
}
