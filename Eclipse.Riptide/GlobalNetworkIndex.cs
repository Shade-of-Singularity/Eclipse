namespace Eclipse.Riptide
{
    public static class Testing
    {

    }

    public sealed class TestingMod : Mod<TestingMod>
    {

    }

    public abstract class Mod<T> where T : Mod<T>, new()
    {
        public static readonly T Instance = new();
        public static readonly NetworkIndex Net = new();
    }

    public static class ServerMessage<T> where T : Enum
    {
        public static readonly ushort ID = GlobalNetworkIndex.NextServerMessageID();
    }

    public static class ClientMessage<T> where T : Enum
    {
        public static readonly ushort ID = GlobalNetworkIndex.NextClientMessageID();
    }

    public sealed class NetworkIndex
    {

    }

    public static class GlobalNetworkIndex
    {
        private static ushort ClientMessageID;
        private static ushort ServerMessageID;

        public static ushort NextClientMessageID()
        {
            checked { return ClientMessageID++; }
        }

        public static ushort NextServerMessageID()
        {
            checked { return ServerMessageID++; }
        }
    }
}
