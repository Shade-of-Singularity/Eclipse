namespace ServiceCore
{
    /// <summary>
    /// Arguments for <see cref="IService"/> initialization.
    /// </summary>
    /// <seealso cref="IService.InvokeInitialize(IInitializationArgs)"/>
    public interface IInitializationArgs : ICommonStartupArgs
    {

    }
}
