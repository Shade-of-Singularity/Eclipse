using Riptide;

namespace Eclipse.Riptide
{
    /// <summary>
    /// Collection of all server-side message handlers for specific handler group ID.
    /// </summary>
    public sealed class ServerHandlers : MessageHandlers<Server.MessageHandler> { }

    /// <summary>
    /// Collection of all server-side message handlers for specific handler group ID.
    /// </summary>
    public sealed class ClientHandlers : MessageHandlers<Client.MessageHandler> { }

    /// <summary>
    /// Collection of all network message handlers.
    /// </summary>
    /// <remarks>
    /// DO NOT inherit this class!
    /// Use <see cref="ServerHandlers"/> and <see cref="ClientHandlers"/> directly instead.
    /// This is because custom message handler collections are not supported at the moment.
    /// </remarks>
    /// <typeparam name="THandler">Type of the network message handler.</typeparam>
    public abstract class MessageHandlers<THandler> where THandler : Delegate
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public THandler this[ushort id]
        { 
            get
            {
                NetworkHandlers.Initialize();
                // This will throw anyway.
                // if (id > m_Handlers.Length) throw new ArgumentOutOfRangeException(nameof(id));
                return m_Handlers[id];
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private THandler[] m_Handlers = new THandler[(int)SystemMessageID.Amount];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public bool Has(ushort id)
        {
            NetworkHandlers.Initialize();
            return m_Handlers[id] is not null;
        }

        public bool TryGet(ushort id, out THandler hander)
        {
            NetworkHandlers.Initialize();
            hander = m_Handlers[id];
            return hander is not null;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static class Unsafe
        {
            public static THandler[] GetHandlers(MessageHandlers<THandler> handlers) => handlers.m_Handlers;
            public static void SetHandlers(MessageHandlers<THandler> handlers, THandler[] value) => handlers.m_Handlers = value;
            public static ushort GetCapacity(MessageHandlers<THandler> handlers) => (ushort)handlers.m_Handlers.Length;
            public static void Resize(MessageHandlers<THandler> handlers, ushort size) => Array.Resize(ref handlers.m_Handlers, size);
            public static void Clear(MessageHandlers<THandler> handlers) => Array.Fill(handlers.m_Handlers, null);
            public static void Reset(MessageHandlers<THandler> handlers, ushort size)
            {
                if (handlers.m_Handlers.Length == size)
                {
                    Array.Fill(handlers.m_Handlers, null);
                }
                else
                {
                    handlers.m_Handlers = new THandler[size];
                }
            }
        }
    }
}
