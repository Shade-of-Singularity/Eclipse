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
    /// Should only be applied to classes that derive from <see cref="EngineService"/>.
    /// Attribute is ignored otherwise.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Initialization execution mode of the service.
        /// </summary>
        /// <remarks>
        /// Anything else than <see cref="MainThread"/> will result in service being initialized on a background thread (not thread-safe).
        /// </remarks>
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
}
