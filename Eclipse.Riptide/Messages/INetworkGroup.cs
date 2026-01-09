using UnityEngine.UIElements;

namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Interface for custom network groups.
    /// </summary>
    /// <typeparam name="T">Type that implemented custom group.</typeparam>
    public interface INetworkGroup<T> : INetworkGroup where T : INetworkGroup<T>
    {
        public static readonly byte GroupID = NetworkIndex.NextGroupID();
    }

    /// <summary>
    /// Unused at the moment, but might be used to identify generic <see cref="INetworkGroup{T}"/> interfaces.
    /// </summary>
    public interface INetworkGroup
    {
        /// <summary>
        /// Name of the readonly GroupID field in a generic interface above.
        /// Used in reflections by <see cref="NetworkIndex"/> class.
        /// </summary>
        public const string GroupIDFieldName = "GroupID";
    }
}
