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

namespace Eclipse
{
    /// <summary>
    /// Describes when and how service should be initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Should only be applied to classes that derive from <see cref="IService{TService}"/>.
    /// Attribute is ignored otherwise.
    /// </para>
    /// <para>
    /// To replace an existing service, simply implement <see cref="IService{TService}"/> interface it declares.
    /// It will select service based on order in which assemblies were loaded. (last are prioritized)
    /// If said service is an abstract or unsealed class - inherit class itself to overwrite it.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServiceAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initialization order of the service.
        /// </summary>
        /// <remarks>
        /// Ignored when <see cref="ExecutionMode"/> is not <see cref="IService.ThreadExecutionMode.MainThread"/>
        /// (as it will essentially produce race conditions).
        /// </remarks>
        public int ExecutionOrder { get; }

        /// <summary>
        /// Describes how service initialization/unloading will interact with threading system.
        /// </summary>
        /// <remarks>
        /// By default executes in <see cref="IService.ThreadExecutionMode.MainThread"/> mode.
        /// </remarks>
        public IService.ThreadExecutionMode ExecutionMode { get; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Defines all default values to parameters.
        /// </summary>
        public ServiceAttribute() { }

        /// <summary>
        /// Defines initialization <see cref="ExecutionOrder"/> for the underlying <see cref="IService"/>.
        /// </summary>
        /// <remarks>
        /// Will forcefully keep <see cref="ExecutionMode"/> at <see cref="IService.ThreadExecutionMode.MainThread"/>
        /// as any ordering is ignored during multi-threaded initialization.
        /// </remarks>
        public ServiceAttribute(int order)
        {
            ExecutionOrder = order;
        }

        /// <summary>
        /// Defines <see cref="IService.ThreadExecutionMode"/> for the underlying <see cref="IService{TService}"/>.
        /// </summary>
        /// <param name="mode">Mode to use. Providing <see cref="IService.ThreadExecutionMode.MainThread"/> works the same as empty .ctor</param>
        public ServiceAttribute(IService.ThreadExecutionMode mode)
        {
            ExecutionMode = mode;
        }
    }
}
