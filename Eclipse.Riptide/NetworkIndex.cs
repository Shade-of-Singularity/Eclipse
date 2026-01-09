using Eclipse.Riptide.Messages;
using System;
using System.Reflection;
using UnityEngine;

namespace Eclipse.Riptide
{
    /// <summary>
    /// Helps with message indexing.
    /// </summary>
    /// TODO: Add ways to register new handlers for new loaded-in assemblies, automatically or manually, without full networking reload.
    public static class NetworkIndex
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// How many groups can be generated.
        /// </summary>
        public const ushort MaxGroupIDAmount = byte.MaxValue + 1;

        /// <summary>
        /// How many messages one group can hold.
        /// </summary>
        public const uint MaxMessageIDAmount = ushort.MaxValue + 1;




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
        private static readonly ClientHandlers[] m_ClientHandlers = new ClientHandlers[MaxGroupIDAmount];
        private static readonly ServerHandlers[] m_ServerHandlers = new ServerHandlers[MaxGroupIDAmount];
        private static readonly uint[] m_NextMessageIDs = new uint[MaxGroupIDAmount];
        private static volatile bool m_IsInitialized = false;
        private static readonly object _lock = new object();
        private static ushort m_NextGroupID; // We use ushort instead of byte, to gracefully handle ID exhaustion.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static NetworkIndex()
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

            // Makes sure that system messages are left untouched.
            const uint StartingID = (uint)SystemMessageID.Amount;
            Array.Fill(m_NextMessageIDs, StartingID);
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
                UpdateHandlers();
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
            // If group ID returns back to 0
            if (m_NextGroupID >= MaxGroupIDAmount)
            {
                throw new Exception("Exhausted all network Group IDs for Riptide networking.");
            }

            return (byte)m_NextGroupID++;
        }

        public static ushort NextMessageID(byte groupID)
        {
            // We use '>=' because ID is 0-based value, and Limit is 1-based value.
            if (m_NextMessageIDs[groupID] >= MaxMessageIDAmount)
            {
                throw new Exception("Exhausted all network Message IDs for Riptide networking.");
            }

            return (ushort)m_NextMessageIDs[groupID]++;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void UpdateHandlers()
        {
            try
            {
                foreach (var classes in typeof(NetworkIndex).Assembly.GetTypes())
                {
                    foreach (var method in classes.GetMethods(BindingFlags.Static | BindingFlags.InvokeMethod))
                    {
                        RegisterHandlers(method);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Could not update message handlers gracefully! Networking is likely broken at this point.");
            }
        }

        private static void RegisterHandlers(MethodInfo method)
        {
            EclipseMessageAttribute attribute = method.GetCustomAttribute<EclipseMessageAttribute>();
            if (attribute is null) return;

            // Differentiates client-side and server-side message handlers by the parameter types they specify.
            ParameterInfo[] parameters = method.GetParameters();
            switch (parameters.Length)
            {
                // Likely client-side method - they only have the INetworkMessage in parameters.
                case 1:
                    if (typeof(INetworkMessage).IsAssignableFrom(parameters[0].ParameterType))
                    {
                        Debug.Log($"Client message handler ({method.Name}) with message type {parameters[0].ParameterType.Name} was found!");
                    }
                    else
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely client-side, but doesn't have {nameof(INetworkMessage)} in args.");
                    }
                    break;

                // Likely server-side method - they have UserID and INetworkMessage in parameters.
                case 2:
                    if (parameters[0].ParameterType != typeof(ushort))
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely client-side, but doesn't have ClientID as first parameter.");
                    }

                    if (typeof(INetworkMessage).IsAssignableFrom(parameters[1].ParameterType))
                    {
                        Debug.Log($"Server message handler ({method.Name}) with message type {parameters[1].ParameterType.Name} was found!");
                    }
                    else
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely client-side, but doesn't have {nameof(INetworkMessage)} in args.");
                    }
                    break;

                // Any other kind of signature is not supported at the moment.
                default:
                case 0: throw new Exception($"Message handler ({method.Name}) doesn't have client-side nor server-side Riptide message signature.");
            }
        }
    }
}
