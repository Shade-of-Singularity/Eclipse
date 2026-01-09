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

using Eclipse.Riptide.Handlers;
using Eclipse.Riptide.Messages;
using Riptide;
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
                // Resets all handlers first.
                // Note: All managed handlers will be locked at this point, so mutation is safe, even in multi-threaded context (untested though)
                Array.ForEach(m_ClientHandlers, Handlers.ClientHandlers.Unsafe.Clear);
                Array.ForEach(m_ServerHandlers, Handlers.ServerHandlers.Unsafe.Clear);

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
            Type messageType, dataType;
            byte groupID; ushort messageID;
            switch (parameters.Length)
            {
                // Likely client-side method - they only have the INetworkMessage in parameters.
                case 1:
                    dataType = parameters[0].ParameterType;
                    if (attribute.MessageType != null)
                    {
                        messageType = attribute.MessageType;
                    }
                    else if (typeof(INetworkMessage).IsAssignableFrom(dataType))
                    {
                        messageType = dataType;
                    }
                    else
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely client-side, but has unsupported parameter ({parameters[0].ParameterType}). Make sure that parameter is either {nameof(Riptide)}.{nameof(Message)} or {nameof(INetworkMessage)}.");
                    }

                    groupID = (byte)messageType.GetField(INetworkGroup.GroupIDFieldName, BindingFlags.Static).GetValue(null);
                    messageID = (ushort)messageType.GetField(INetworkMessage.MessageIDFieldName, BindingFlags.Static).GetValue(null);
                    ClientHandlers.HandlerInfo clientHandler = new ClientHandlers.HandlerInfo(method, dataType);
                    Handlers.ClientHandlers.Unsafe.Put(m_ClientHandlers[groupID], messageID, clientHandler);
                    Debug.Log($"Client message handler ({method.Name}) with message type {messageType.Name} was found and it is valid!");
                    break;

                // Likely server-side method - they have UserID and INetworkMessage in parameters.
                case 2:
                    if (parameters[0].ParameterType != typeof(ushort))
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely server-side, but doesn't have ClientID (ushort) as first parameter.");
                    }

                    dataType = parameters[0].ParameterType;
                    if (attribute.MessageType != null)
                    {
                        messageType = attribute.MessageType;
                    }
                    else if (typeof(INetworkMessage).IsAssignableFrom(dataType))
                    {
                        messageType = dataType;
                    }
                    else
                    {
                        throw new Exception($"Message handler ({method.Name}) is likely client-side, but has unsupported parameter ({parameters[0].ParameterType}). Make sure that parameter is either {nameof(Riptide)}.{nameof(Message)} or {nameof(INetworkMessage)}.");
                    }

                    groupID = (byte)messageType.GetField(INetworkGroup.GroupIDFieldName, BindingFlags.Static).GetValue(null);
                    messageID = (ushort)messageType.GetField(INetworkMessage.MessageIDFieldName, BindingFlags.Static).GetValue(null);
                    ServerHandlers.HandlerInfo serverHandler = new ServerHandlers.HandlerInfo(method, dataType);
                    Handlers.ServerHandlers.Unsafe.Put(m_ServerHandlers[groupID], messageID, serverHandler);
                    Debug.Log($"Client message handler ({method.Name}) with message type {messageType.Name} was found and it is valid!");
                    break;

                // Any other kind of signature is not supported at the moment.
                default:
                case 0: throw new Exception($"Message handler ({method.Name}) doesn't have client-side nor server-side Riptide message signature.");
            }
        }
    }
}
