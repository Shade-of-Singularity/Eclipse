namespace Eclipse.Riptide
{
    public abstract class GroupHolder<T> where T : GroupHolder<T>
    {
        public static readonly byte GroupID = NetworkHandlers.NextGroupID();
        public static byte Group() => GroupID;
    }
}
