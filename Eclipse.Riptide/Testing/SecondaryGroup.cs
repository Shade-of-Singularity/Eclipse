namespace Eclipse.Riptide.Testing
{
    /// <summary>
    /// Group for all message for main group.
    /// </summary>
    public sealed class SecondaryGroup : GroupHolder<SecondaryGroup>
    {
        public static readonly ushort ValidateConnection = NetworkHandlers.NextID(GroupID);
        public static readonly ushort RenamePlayer = NetworkHandlers.NextID(GroupID);
    }
}
