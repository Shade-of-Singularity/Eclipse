using Riptide;
using System;

namespace Eclipse.Riptide
{
    /// <summary>
    /// Replacement for <see cref="MessageHandlerAttribute"/>s with non-strict Message ID.
    /// To use it, provide message type. It is mandatory that specified message inherits <see cref="Messages.INetworkMessage{TMessage, TGroup}"/>.
    /// </summary>
    /// <remarks>
    /// Non-strict implementation makes it impossible to play games with different networking mods installed.
    /// Mods also has to be initialized in one set order, but it should be handled by <see cref="Engine"/> automatically anyway.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EclipseMessageAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Type of the message. Must inherit <see cref="Messages.INetworkMessage{T}"/>.
        /// </summary>
        public readonly Type MessageType;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor for the attribute.
        /// </summary>
        /// <param name="messageType">Type of the message this attribute employs. Target type must inherit <see cref="Messages.INetworkMessage{T}"/>.</param>
        public EclipseMessageAttribute(Type messageType)
        {
            MessageType = messageType;
        }
    }
}
