using Eclipse.Riptide.Load;
using Riptide;

namespace Eclipse.Riptide.Messages
{
    /// <inheritdoc cref="FlagMessage{TMessage, TGroup, TProfile}"/>
    /// <remarks>
    /// <para>Implements <see cref="DefaultGroup"/> as <see cref="NetworkGroup{TGroup}"/> by default.</para>
    /// <para>Implements <see cref="S1"/> as <see cref="StorageProfile{TProfile}"/> by default.</para>
    /// </remarks>
    public abstract class FlagMessage<TMessage> : NetworkMessage<TMessage>
        where TMessage : NetworkMessage<TMessage, DefaultGroup, S1>, new()
    {
        /// <inheritdoc/>
        public override Message Read(Message message) => message;

        /// <inheritdoc/>
        public override Message Write(Message message) => message;
    }

    /// <inheritdoc cref="FlagMessage{TMessage, TGroup, TProfile}"/>
    /// <remarks>
    /// Implements <see cref="S1"/> as <see cref="StorageProfile{TProfile}"/> by default.
    /// </remarks>
    public abstract class FlagMessage<TMessage, TGroup> : NetworkMessage<TMessage, TGroup>
        where TMessage : NetworkMessage<TMessage, TGroup, S1>, new()
        where TGroup : NetworkGroup<TGroup>
    {
        /// <inheritdoc/>
        public override Message Read(Message message) => message;

        /// <inheritdoc/>
        public override Message Write(Message message) => message;
    }

    /// <summary>
    /// Custom message which doesn't implement any <see cref="Read(Message)"/> and <see cref="Write(Message)"/> functionality.
    /// </summary>
    /// <typeparam name="TMessage">Class that inherited this <see cref="FlagMessage{TMessage, TGroup, TProfile}"/></typeparam>
    /// <typeparam name="TGroup">Group this message should belong to.</typeparam>
    /// <typeparam name="TProfile"><see cref="StorageProfile{TProfile}"/> of this network message. Will pool some of the message instances based on it.</typeparam>
    public abstract class FlagMessage<TMessage, TGroup, TProfile> : NetworkMessage<TMessage, TGroup, TProfile>
        where TMessage : NetworkMessage<TMessage, TGroup, TProfile>, new()
        where TGroup : NetworkGroup<TGroup>
        where TProfile : StorageProfile<TProfile>, new()
    {
        /// <inheritdoc/>
        public override Message Read(Message message) => message;

        /// <inheritdoc/>
        public override Message Write(Message message) => message;

        /// <inheritdoc/>
        protected override void Dispose() { }
    }
}
