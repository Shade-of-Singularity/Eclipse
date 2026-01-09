using System;

namespace Eclipse.Riptide.Handlers
{
    /// <summary>
    /// Collection of all network message handlers.
    /// </summary>
    /// <remarks>
    /// DO NOT inherit this class!
    /// Use <see cref="ServerHandlers"/> and <see cref="ClientHandlers"/> directly instead.
    /// This is because custom message handler collections are not supported at the moment.
    /// </remarks>
    public abstract class MessageHandlers<THandler>
    {
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
        public THandler Get(ushort id)
        {
            NetworkIndex.Initialize();
            // We don't need checks here - it will throw anyway.
            // if (id > m_Handlers.Length) throw new ArgumentOutOfRangeException(nameof(id));
            return m_Handlers[id];
        }
        
        public bool Has(ushort id)
        {
            NetworkIndex.Initialize();
            return m_Handlers[id] != null;
        }

        public bool TryGet(ushort id, out THandler hander)
        {
            NetworkIndex.Initialize();
            hander = m_Handlers[id];
            return hander != null;
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
            public static void Clear(MessageHandlers<THandler> handlers) => Array.Fill(handlers.m_Handlers, default);
            public static void Reset(MessageHandlers<THandler> handlers, ushort size)
            {
                if (handlers.m_Handlers.Length == size)
                {
                    Array.Fill(handlers.m_Handlers, default);
                }
                else
                {
                    handlers.m_Handlers = new THandler[size];
                }
            }

            public static void Put(MessageHandlers<THandler> handlers, ushort messageID, THandler handler)
            {
                if (messageID >= handlers.m_Handlers.Length)
                {
                    // Resizes array to the next power of two (or the same amount if ID is already PoT).
                    Array.Resize(ref handlers.m_Handlers, GetPoTArraySize(messageID));
                }

                handlers.m_Handlers[messageID] = handler;
            }

            public static void Remove(MessageHandlers<THandler> handlers, ushort messageID)
            {
                if (messageID < handlers.m_Handlers.Length)
                {
                    // Resets only if target id is even present.
                    handlers.m_Handlers[messageID] = default!;
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static int GetPoTArraySize(ushort x)
            {
                if (x < (ushort)SystemMessageID.Amount)
                {
                    // Clamps to the minimal allowed amount.
                    return (int)SystemMessageID.Amount;
                }

                int v = x - 1;
                v |= v >> 1;
                v |= v >> 2;
                v |= v >> 4;
                v |= v >> 8;
                return v + 1;
            }
        }
    }
}
