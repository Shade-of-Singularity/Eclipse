using Riptide;
using System;

namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Exist with a sole purpose of simplifying reflections.
    /// Never used in networking.
    /// </summary>
    public sealed class ReflectionMessage : NetworkMessage<ReflectionMessage>
    {
        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        public override Message Read(Message message) => throw new InvalidOperationException($"Cannot read message with {nameof(ReflectionMessage)}, by design.");

        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        public override Message Write(Message message) => throw new InvalidOperationException($"Cannot write message with {nameof(ReflectionMessage)}, by design.");

        /// <inheritdoc/>
        /// <remarks>
        /// With <see cref="ReflectionMessage"/> - throws immediately when this method is called.
        /// </remarks>
        protected override void Dispose() => throw new InvalidOperationException($"Cannot dispose {nameof(ReflectionMessage)}, by design.");
    }
}
