using Cysharp.Threading.Tasks;

namespace Eclipse.Serialization
{
    /// <inheritdoc cref="ISerializationService"/>
    [Service(ISerializationService.InitializationOrder)]
    public class DefaultSerializationService : ISerializationService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public virtual UniTask Initialize() => UniTask.CompletedTask;

        /// <inheritdoc/>
        public virtual UniTask Terminate() => UniTask.CompletedTask;
    }
}
