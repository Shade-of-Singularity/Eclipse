namespace Eclipse.Riptide
{
    /// <summary>
    /// All message IDs used with <see cref="global::Riptide"/> networking to determine of game versions are compatible or not.
    /// </summary>
    public enum SystemMessageID : ushort
    {
        /// <summary>
        /// Range of all values system messages will take.
        /// This range will be reserved in handler lists and never employed by regular networking messages.
        /// </summary>
        Range = 0b1111, // First 8 IDs are reserved.
        
        /// <summary>
        /// Amount of Message IDs reserved for system messages.
        /// </summary>
        Amount = Range + 1,

        /// <summary>
        /// Message handler used to compare whether all message handlers have the same IDs or not.
        /// Used by <see cref="NetworkIndex"/>.
        /// </summary>
        /// <remarks>
        /// Amount of handlers usually different only if two clients, or client and server has different networking mods installed.
        /// Client-side mods never modify handler collection (only by a developer's mistake), so they can be used safely.
        /// </remarks>
        NetworkingValidationCheck = 0,
    }
}
