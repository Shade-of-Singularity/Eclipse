namespace ServiceCore
{
    /// <summary>
    /// Arguments for <see cref="IService"/> termination.
    /// </summary>
    /// <seealso cref="IService.InvokeTerminate(ITerminationArgs)"/>
    public interface ITerminationArgs : ICommonStartupArgs
    {

    }
}
