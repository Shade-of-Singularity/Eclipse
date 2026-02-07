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
    /// Forces static class to run its static .ctor with <see cref="Eclipse"/>.<see cref="Engine"/>.
    /// Or runs a method to which it attribute is attached.
    /// </summary>
    /// <remarks>
    /// Static .ctor will run after <see cref="IService"/>s were instantiated, but BEFORE they were initialized.
    /// Read this as - it will be dangerous to interact with <see cref="IService{T}.Instance"/>s at this point.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EclipseInitializeAttribute : Attribute
    {
        /// <summary>
        /// When attribute should be employed.
        /// </summary>
        public readonly InitializationTiming Timing;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor. Runs method in the latest possible point during engine initialization.
        /// <para>See also: <see cref="InitializationTiming.AfterEngineInitialization"/>.</para>
        /// </summary>
        public EclipseInitializeAttribute() : this(InitializationTiming.AfterEngineInitialization) { }

        /// <summary>
        /// Full constructor. Allows you to specify when exactly attribute will used.
        /// </summary>
        /// <param name="timing"></param>
        public EclipseInitializeAttribute(InitializationTiming timing)
        {
            Timing = timing;
        }
    }
}
