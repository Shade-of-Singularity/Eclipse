using Eclipse.Riptide.Messages;
using Riptide;

namespace Eclipse.Riptide.Testing
{
    public struct SendChunk : INetworkMessage<SendChunk, MainGroup>
    {
        public const int ChunkSize = 16;
        public const int ChunkHeight = 128;
        public const int ChunkArea = ChunkSize * ChunkSize;
        public const int ChunkVolume = ChunkArea * ChunkHeight;

        public int x, y;
        public uint[] blocks;

        public void Read(Message message)
        {
            x = message.GetInt();
            y = message.GetInt();
            blocks = message.GetUInts(ChunkVolume);
        }

        public readonly void Write(Message message)
        {
            message.AddInt(x);
            message.AddInt(y);
            message.AddUInts(blocks);
        }
    }

    public struct ValidateChunk : INetworkMessage<ValidateChunk, MainGroup>
    {
        public int x, y;
        public ulong ChunkHash;

        public void Read(Message message)
        {
            x = message.GetInt();
            y = message.GetInt();
            ChunkHash = message.GetULong();
        }

        public readonly void Write(Message message)
        {
            message.AddInt(x);
            message.AddInt(y);
            message.AddULong(ChunkHash);
        }
    }

    public struct ReceiveInventory : INetworkMessage<ReceiveInventory, MainGroup>
    {
        public uint[] ids;
        public uint[] amounts;

        public void Read(Message message)
        {
            ids = message.GetUInts();
            amounts = message.GetUInts();
        }

        public readonly void Write(Message message)
        {
            message.AddUInts(ids);
            message.AddUInts(amounts);
        }
    }
}
