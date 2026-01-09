namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Interface for custom network groups.
    /// </summary>
    /// <typeparam name="T">Type that implemented custom group.</typeparam>
    public interface INetworkGroup<T> where T : INetworkGroup<T>
    {
        public static readonly byte GroupID = NetworkIndex.NextGroupID();
    }

    /// <summary>
    /// Unused at the moment, but might be used to identify generic <see cref="INetworkGroup{T}"/> interfaces.
    /// </summary>
    internal interface INetworkGroup { }
}
