namespace ServiceCore
{
    /// <summary>
    /// Arguments used for initializing individual services.
    /// </summary>
    /// TODO: Replace with non-record struct. Defined via record for faster iteration.
    public readonly record struct InitializationArgs(bool IsDependenciesBroken) { }
}
