/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

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
