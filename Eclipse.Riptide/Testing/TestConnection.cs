using Riptide;
using System;
using System.Threading;
using UnityEngine;

namespace Eclipse.Riptide.Testing
{
    public static class TestConnection
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static EclipseServer Server { get; } = new EclipseServer();
        public static EclipseClient Client { get; } = new EclipseClient();
        public static ushort ServerPort { get; } = 52323;
        public static ushort ClientPort { get; } = 54372;
        public static bool Enabled
        {
            get => m_Enabled;
            set
            {
                if (m_Enabled == value) return;
                if (m_Enabled = value)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static bool m_Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static void Start() => Enabled = true;
        public static void End() => Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void OnEnabled()
        {
            Debug.Log("Starting test connection.");
            Server.Start(ServerPort, 1, messageHandlerGroupId: ExampleGroup.GroupID);
            Thread.Sleep(10);
            if (!Client.Connect($"127.0.0.1:{ClientPort}", messageHandlerGroupId: ExampleGroup.GroupID))
            {
                Debug.LogWarning("Cannot connect to the server. Resetting...");
                Enabled = false;
                return;
            }

            Debug.Log("Sending example messages.");
            SendChunkContainerMessage();
            SendValidateChunkMessage();
            SendReceiveInventoryMessage();
            Thread.Sleep(10);
            Debug.Log("All messages was sent! Test concluded.");
            Enabled = false;

            // Simplifications:
            void SendChunkContainerMessage()
            {
                // Sending chunk data.
                ChunkContainer chunk = ChunkContainer.Get();
                chunk.x = 4; chunk.y = 4;
                chunk.blocks = new uint[ChunkContainer.ChunkVolume];
                Array.Fill<uint>(chunk.blocks, 1);

                Client.Send(chunk.Pack(mode: MessageSendMode.Reliable));
                Server.SendToAll(chunk.Pack(mode: MessageSendMode.Reliable));
            }

            void SendValidateChunkMessage()
            {
                // Sending validation data.
                ValidateChunk validate = ValidateChunk.Get();
                validate.x = 4; validate.y = 4;
                validate.hash = (ulong)new System.Random().Next();

                Client.Send(validate.Pack(mode: MessageSendMode.Reliable));
                Server.SendToAll(validate.Pack(mode: MessageSendMode.Reliable));
            }

            void SendReceiveInventoryMessage()
            {
                const int InventorySize = 46;

                // Sending inventory data.
                ReceiveInventory inventory = ReceiveInventory.Get();
                inventory.ids = new uint[InventorySize];
                inventory.amounts = new uint[InventorySize];
                Array.Fill<uint>(inventory.ids, 1);
                Array.Fill<uint>(inventory.amounts, 1);
                inventory.ids[0] = 12;
                inventory.ids[1] = 4;
                inventory.ids[2] = 4;
                inventory.amounts[0] = 2;
                inventory.amounts[0] = 64;
                inventory.amounts[0] = 64;

                Client.Send(inventory.Pack(mode: MessageSendMode.Reliable));
                Server.SendToAll(inventory.Pack(mode: MessageSendMode.Reliable));
            }
        }

        private static void OnDisabled()
        {
            Server.Stop();
            Client.Disconnect();
            Debug.Log("Test connection was closed.");
        }
    }
}
