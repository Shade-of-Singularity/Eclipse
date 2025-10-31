using System;

namespace Eclipse;

/// <summary>
/// Attribute to flag methods that should be executed after an service was initialized.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class ServicePreloadMethodAttribute : Attribute
{
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                              Public Properties
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    /// <summary>
    /// Order in which preload methods should be invoked.
    /// </summary>
    /// <remarks>
    /// <see cref="InvokeOrder"/> still applies even when <see cref="Service"/> runs on a background thread
    /// - it will run within synchronization context of <see cref="Service"/>.
    /// </remarks>
    public int InvokeOrder { get; set; } = 0;

    /// <summary>
    /// Whether method can be executed in a background thread.
    /// </summary>
    /// <remarks>
    /// <remarks>
    /// Flag is ignored if target <see cref="Service"/> has <see cref="ServiceAttribute.ThreadExecutionMode.MainThread"/> flag set on it.
    /// In which case method will be executed on a main thread instead.
    /// <para>
    /// Regardless of being executed in a background or on a main thread,
    /// will still only run before <see cref="Service"/>'s <see cref="EngineService.Initialize"/> method.
    /// </para>
    /// </remarks>
    public bool ThreadSafe { get; set; } = false;

    /// <summary>
    /// Reference service to use. Method with this attribute will be executed after this service is initialized.
    /// </summary>
    public Type Service { get; }




    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                                Constructors
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    public ServicePreloadMethodAttribute(Type service)
    {
        Service = service;
    }





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
