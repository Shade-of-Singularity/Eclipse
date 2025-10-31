/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using System;

namespace Eclipse;

/// <summary>
/// Attribute to flag methods that should be executed after an service was initialized.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class ServiceAfterloadMethodAttribute : Attribute
{
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
    /// .
    /// .                                              Public Properties
    /// .
    /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    /// <summary>
    /// Order in which afterload methods should be invoked.
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
    /// Flag is ignored if target <see cref="Service"/> has <see cref="ServiceAttribute.ThreadExecutionMode.MainThread"/> flag set on it.
    /// In which case method will be executed on a main thread instead.
    /// <para>
    /// Regardless of being executed in a background or on a main thread,
    /// will still only run after <see cref="Service"/>'s <see cref="EngineService.Initialize"/> method.
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
    public ServiceAfterloadMethodAttribute(Type service)
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
