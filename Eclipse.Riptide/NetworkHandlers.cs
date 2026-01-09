using Riptide;

namespace Eclipse.Riptide
{
    public static class NetworkHandlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// How many groups can be procedurally generated.
        /// </summary>
        public const ushort GroupAmountLimit = byte.MaxValue + 1;

        /// <summary>
        /// How many messages one group can hold.
        /// </summary>
        public const int MessageIDAmountLimit = ushort.MaxValue + 1;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether all handlers were initialized successfully.
        /// </summary>
        public static bool IsInitialized => m_IsInitialized;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly ClientHandlers[] m_ClientHandlers = new ClientHandlers[GroupAmountLimit];
        private static readonly ServerHandlers[] m_ServerHandlers = new ServerHandlers[GroupAmountLimit];
        private static readonly int[] m_NextMessageIDs = new int[GroupAmountLimit];
        private static volatile bool m_IsInitialized;
        private static readonly object _lock = new();
        private static ushort m_NextGroupID = 0;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static NetworkHandlers()
        {
            ClientHandlers[] client = m_ClientHandlers;
            for (int i = 0; i < client.Length; i++)
            {
                client[i] = new ClientHandlers();
            }

            ServerHandlers[] server = m_ServerHandlers;
            for (int i = 0; i < server.Length; i++)
            {
                server[i] = new ServerHandlers();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invalidates initialization of the network handlers, forcing game to reload all handlers before the next network call.
        /// </summary>
        /// <remarks>
        /// Using this method outside of initialization sequence is dangerous.
        /// In <see cref="Eclipse"/>, it might be used only once after <see cref="Engine"/> loads-in mod assemblies.
        /// </remarks>
        public static void Invalidate() => m_IsInitialized = false;
        public static void Initialize()
        {
            if (m_IsInitialized) return;
            lock (_lock)
            {
                // Second check after value was unlocked.
                if (m_IsInitialized) return;

                m_IsInitialized = true;
            }
        }

        /// <summary>
        /// Retrieves collection of all message handlers - specifically for client-side message handlers.
        /// </summary>
        /// <param name="group">Group ID of a collection of client-side message handlers.</param>
        /// <returns>Collection of client-side message handlers.</returns>
        public static ClientHandlers ClientHandlers(byte group = 0) => m_ClientHandlers[group];

        /// <summary>
        /// Retrieves collection of all message handlers - specifically for server-side message handlers.
        /// </summary>
        /// <param name="group">Group ID of a collection of server-side message handlers.</param>
        /// <returns>Collection of server-side message handlers.</returns>
        public static ServerHandlers ServerHandlers(byte group = 0) => m_ServerHandlers[group];

        /// <summary>
        /// Retrieves next group ID for networking with <see cref="Riptide"/>.
        /// </summary>
        public static byte NextGroupID()
        {
            // We use '>=' because ID is 0-based value, and Limit is 1-based value.
            if (m_NextGroupID >= GroupAmountLimit)
            {
                throw new Exception("Exhausted all network groups for Riptide networking.");
            }

            return (byte)(++m_NextGroupID);
        }

        public static ushort NextID(byte groupID)
        {
            // We use '>=' because ID is 0-based value, and Limit is 1-based value.
            if (m_NextMessageIDs[groupID] >= GroupAmountLimit)
            {
                throw new Exception("Exhausted all network groups for Riptide networking.");
            }

            return (byte)(++m_NextMessageIDs[groupID]);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static Client.MessageHandler[] FetchClientHandlers()
        {
            throw new NotImplementedException();
        }

        private static Server.MessageHandler[] FetchServerHandlers()
        {
            throw new NotImplementedException();
        }

        private static T[] FetchHandlers<T>() where T : Delegate
        {
            List<Delegate> handlers = new();

            // ...

            return (T[])handlers.ToArray();
        }
    }
}
