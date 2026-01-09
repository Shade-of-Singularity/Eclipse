namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Interface for custom messages.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message with implemented this interface.</typeparam>
    /// <typeparam name="TGroup">Type of networking group this message belongs to.</typeparam>
    public interface INetworkMessage<TMessage, TGroup> : INetworkMessage, INetworkGroup<TGroup>
        where TMessage : INetworkMessage<TMessage, TGroup>
        where TGroup : INetworkGroup<TGroup>
    {
        public static readonly ushort MessageID = NetworkIndex.NextMessageID(GroupID);
        public void Read(global::Riptide.Message message);
        public void Write(global::Riptide.Message message);
    }

    /// <summary>
    /// Used purely to identify <see cref="INetworkMessage{TMessage, TGroup}"/> without using generics and complicated reflections.
    /// </summary>
    public interface INetworkMessage { }
}
