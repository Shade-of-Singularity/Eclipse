using Eclipse.Riptide.Handlers;
using Riptide;
using Riptide.Transports;
using Riptide.Transports.Udp;
using Riptide.Utils;
using UnityEngine;

namespace Eclipse.Riptide
{
    /// <summary>
    /// Custom <see cref="global::Riptide"/> <see cref="Client"/> which supports runtime Message ID identification.
    /// </summary>
    public sealed class EclipseClient : Client
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ClientHandlers? m_MessageHandlers;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Handles initial setup using the built-in UDP transport.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public EclipseClient(string logName = "CLIENT") : this(new UdpClient(), logName) { }

        /// <summary>
        /// Handles initial setup.
        /// </summary>
        /// <param name="transport">The transport to use for sending and receiving data.</param>
        /// <param name="logName">The name to use when logging messages via <see cref="RiptideLogger"/>.</param>
        public EclipseClient(IClient transport, string logName = "CLIENT") : base(transport, logName)
        {
            // We use custom handling method, so built-in one should be disabled.
            useMessageHandlers = false;

            // We also cannot override built-in handling method without breaking the message, so we just route message manually via callback.
            MessageReceived += ClientBroadcastMessage;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override void CreateMessageHandlersDictionary(byte groupID) => m_MessageHandlers = NetworkIndex.ClientHandlers(groupID);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void ClientBroadcastMessage(object sender, MessageReceivedEventArgs args)
        {
            if (m_MessageHandlers?.TryFire(args.MessageId, args.Message) != true)
            {
                Debug.LogWarning($"No message handler method found for message ID ({args.MessageId})!");
            }
        }
    }
}
