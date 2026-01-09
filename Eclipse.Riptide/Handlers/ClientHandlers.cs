using Eclipse.Riptide.Messages;
using Riptide;
using System;
using System.Reflection;

namespace Eclipse.Riptide.Handlers
{
    /// <summary>
    /// Collection of all server-side message handlers for specific handler group ID.
    /// </summary>
    public sealed class ClientHandlers : MessageHandlers<ClientHandlers.HandlerInfo>
    {
        /// <summary>
        /// Client-side handler info.
        /// </summary>
        public readonly struct HandlerInfo
        {
            /// <summary>
            /// Method which have to be invoked.
            /// </summary>
            /// <remarks>
            /// Using it over <see cref="Delegate.DynamicInvoke(object[])"/> should be better for performance. -Gemini (TODO: actually benchmark it)
            /// </remarks>
            public readonly MethodInfo Method;

            /// <summary>
            /// <see cref="INetworkMessage"/> type.
            /// </summary>
            public readonly Type MessageType;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="method">Method info to use for invocation.</param>
            /// <param name="dataType">Type in the message data holder, and first parameter of the <paramref name="method"/>.</param>
            public HandlerInfo(MethodInfo method, Type dataType)
            {
                Method = method;
                MessageType = dataType;
            }
        }



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly object[] args = new object[1]; // TODO: Parallelize, be it with garbage generation, if needed.
        private static readonly object _lock = new object();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to fire message handler under given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of a message handler.</param>
        /// <param name="message">Message to read.</param>
        /// <returns><c>false</c> if there is no handler under given <paramref name="id"/> registered. <c>true</c> otherwise.</returns>
        public bool TryFire(ushort id, Message message)
        {
            if (Has(id))
            {
                Fire(id, message);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Fires handler with specified ID client-side.
        /// </summary>
        /// <remarks>
        /// Throws if handler wasn't found.
        /// </remarks>
        /// <param name="id">ID of a message handler.</param>
        /// <param name="message">Message to read.</param>
        public void Fire(ushort id, Message message)
        {
            HandlerInfo info = Get(id);
            if (info.MessageType == typeof(Message))
            {
                lock (_lock)
                {
                    args[0] = message;
                    info.Method.Invoke(null, args);
                }
            }
            else
            {
                INetworkMessage container = (INetworkMessage)Activator.CreateInstance(info.MessageType);
                container.Read(message);
                lock (_lock)
                {
                    args[0] = container;
                    info.Method.Invoke(null, args);
                }
            }
        }
    }
}
