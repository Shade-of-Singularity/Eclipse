namespace Eclipse.Riptide.Testing
{
    /// <summary>
    /// Group for all message for main group.
    /// </summary>
    public sealed class MainGroup : GroupHolder<MainGroup>
    {
        public static readonly ushort SendWorldData = NetworkHandlers.NextID(GroupID);
        public static readonly ushort ValidateWorldData = NetworkHandlers.NextID(GroupID);
    }
}
