using Eclipse.Riptide.Load;
using Riptide;
using System.Xml.Serialization;

namespace Eclipse.Riptide.Messages
{
    /// <inheritdoc cref="NetworkMessage{TMessage, TGroup, TProfile}"/>
    /// <remarks>
    /// <para>Implements <see cref="DefaultGroup"/> as <see cref="NetworkGroup{TGroup}"/> by default.</para>
    /// <para>Implements <see cref="S1"/> as <see cref="StorageProfile{TProfile}"/> by default.</para>
    /// </remarks>
    /// TODO: Add message pooling based on load.
    public abstract class NetworkMessage<TMessage> : NetworkMessage<TMessage, DefaultGroup, S1>
        where TMessage : NetworkMessage<TMessage, DefaultGroup, S1>, new()
    { } // This instance doesn't override default behaviour.

    /// <inheritdoc cref="NetworkMessage{TMessage, TGroup, TProfile}"/>
    /// <remarks>
    /// Implements <see cref="S1"/> as <see cref="StorageProfile{TProfile}"/> by default.
    /// </remarks>
    public abstract class NetworkMessage<TMessage, TGroup> : NetworkMessage<TMessage, TGroup, S1>
        where TMessage : NetworkMessage<TMessage, TGroup, S1>, new()
        where TGroup : NetworkGroup<TGroup>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// Since this instance has <see cref="LoadProfile"/> set to <see cref="S0"/> - it guarantees that instance will always be collected by GC.
        /// Because of that, this method is empty. You can still override it, but outside of internal callback you will gain nothing from it.
        /// </remarks>
        protected override void Dispose() { }
    }

    /// <summary>
    /// Base class for custom messages.
    /// </summary>
    /// <typeparam name="TMessage">Class that inherited this <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/></typeparam>
    /// <typeparam name="TGroup">Group this message should belong to.</typeparam>
    /// <typeparam name="TProfile"><see cref="StorageProfile{TProfile}"/> of this network message. Will pool some of the message instances based on it.</typeparam>
    public abstract class NetworkMessage<TMessage, TGroup, TProfile> : NetworkMessage
        where TMessage : NetworkMessage<TMessage, TGroup, TProfile>, new()
        where TGroup : NetworkGroup<TGroup>
        where TProfile : StorageProfile<TProfile>, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <see cref="NetworkGroup{TGroup}"/> this <see cref="NetworkMessage{TMessage, TGroup, TLoad}"/> belongs to.
        /// </summary>
        public static byte GroupID => NetworkGroup<TGroup>.GroupID;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Message ID of this <see cref="NetworkMessage{TMessage, TGroup, TLoad}"/>.
        /// </summary>
        public static readonly ushort MessageID = NetworkIndex.NextMessageID(GroupID);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly TMessage[] m_Pool = new TMessage[StorageProfile<TProfile>.Instance.Storage];
        private static int m_PoolHead = -1; // Negative values indicate that there is no items in the pool. Values always change by 1.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves message from pool. If pool is empty - simply creates new instance.
        /// </summary>
        /// <returns>Empty <typeparamref name="TMessage"/> instance to be used.</returns>
        public static TMessage Get()
        {
            // Note: can this part be multithreaded in some way?
            // I doubt, but if there will be a way to do it, no matter the LoadProfile option - it **might** be work implementing.
            lock (m_Pool)
            {
                if (m_PoolHead < 0)
                {
                    return new TMessage();
                }
                else
                {
                    TMessage result = m_Pool[m_PoolHead];
                    m_Pool[m_PoolHead--] = default!;
                    return result;
                }
            }
        }

        /// <summary>
        /// Releases given <paramref name="message"/> by running <see cref="Dispose()"/> method on it and storing it in a pool, if available.
        /// </summary>
        /// <param name="message">Message data container to dispose and store.</param>
        public static void Release(NetworkMessage<TMessage, TGroup, TProfile> message)
        {
            // Always disposes.
            message.Dispose();
            lock (m_Pool)
            {
                // Checks if there is enough space in pool to store more messages.
                int index = m_PoolHead + 1;
                if (index >= m_Pool.Length)
                {
                    // If index item will occupy is outside of the array bounds - it returns.
                    return;
                }

                // If index is within array bounds - it will store message there and move head there. 
                m_Pool[index] = (TMessage)message;
                m_PoolHead = index;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Releases itself by running <see cref="Dispose()"/> method and storing itself in a pool, if available.
        /// </summary>
        public void Release() => Release(this);

        /// <summary>
        /// Packs <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> into a message, including its <see cref="MessageID"/> in the data.
        /// </summary>
        /// <param name="mode"><see cref="global::Riptide"/> Send mode of the <see cref="Message"/>.</param>
        /// <returns>Fully prepared <see cref="Message"/>, ready to be sent to another party.</returns>
        public Message Pack(MessageSendMode mode) => Write(Message.Create(mode, MessageID));

        /// <summary>
        /// Unpacks given message by overwriting values of this <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/> instance.
        /// </summary>
        /// <param name="message">Message to unpack.</param>
        /// <returns>Itself, for convenience.</returns>
        public NetworkMessage<TMessage, TGroup, TProfile> Unpack(Message message)
        {
            Read(message);
            return this;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Disposes all arrays and strings under instance control, making it possible for <see cref="System.GC"/> to collect those arrays and strings.
        /// </summary>
        protected abstract void Dispose();
    }

    /// <summary>
    /// Non-generic base type for <see cref="NetworkMessage{TMessage, TGroup, TProfile}"/>. Exist to make reflections easier.
    /// </summary>
    public abstract class NetworkMessage
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Reads data from provided <paramref name="message"/> and stores it inside itself to be reused later.
        /// </summary>
        public abstract Message Read(Message message);

        /// <summary>
        /// Writes data about this message to provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public abstract Message Write(Message message);
    }
}
