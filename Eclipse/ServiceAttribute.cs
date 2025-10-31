using System;

namespace Eclipse;

/// <summary>
/// Describes when and how service should be initialized.
/// </summary>
/// <remarks>
/// Should only be applied to classes that derive from <see cref="EngineService"/>.
/// Attribute is ignored otherwise.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
    public enum ThreadExecutionMode : byte
    {
        MainThread = 0,
        ThreadSafeBeforeMain = 1,
        ThreadSafeAfterMain = 2,
    }


    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                              Public Properties
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    /// <summary>
    /// Initialization order of the service with this attribute.
    /// </summary>
    /// <remarks>
    /// Ignored whe <see cref="ThreadExecutionOrder"/> is not <see cref="ThreadExecutionMode.MainThread"/> (as it will essentially produce race conditions).
    /// </remarks>
    public int InitializationOrder { get; set; }

    /// <summary>
    /// If thread-safe - <see cref="EngineService"/> will be initialized in a background thread.
    /// </summary>
    /// <remarks>
    /// Thread-safe code is one that does not access any other service, for example.
    /// Important! This will NOT force <see cref="ServicePreloadMethodAttribute"/> and <see cref="ServiceAfterloadMethodAttribute"/> to run in background.
    /// Those will still execute on a main thread, before and after service multi-threaded initialization.
    /// <para>
    /// Only select any other than <see cref="ThreadExecutionMode.MainThread"/> if you know what you are doing.
    /// </para>
    /// </remarks>
    public ThreadExecutionMode ThreadExecutionOrder { get; set; } = ThreadExecutionMode.MainThread;

    /// <summary>
    /// Target service type to replace it during initialization.
    /// </summary>
    public Type? Replace { get; set; }




    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                                Constructors
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    public ServiceAttribute() { }





    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                               Public Methods
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                               Private Methods
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

}
