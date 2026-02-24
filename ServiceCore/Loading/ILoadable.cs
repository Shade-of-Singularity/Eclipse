namespace ServiceCore.Loading
{
    /// <summary>
    /// Thing that can be loaded-in during <see cref="Engine"/> initialization.
    /// </summary>
    /// <remarks>
    /// Resolved by <see cref="Engine"/> via pattern matching.
    /// </remarks>
    public interface ILoadable { }
}
