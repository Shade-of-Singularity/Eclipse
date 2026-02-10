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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Eclipse
{
    public static partial class Engine
    {
        /// <summary>
        /// Stores assemblies while efficiently checking for duplicates.
        /// </summary>
        /// <remarks>
        /// Might be removed in future updates.
        /// </remarks>
        private sealed class AssemblyStorage(int capacity) : IEnumerable<Assembly>
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Public Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public Assembly this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => List[index];
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Public Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public readonly List<Assembly> List = new(capacity);
            public readonly HashSet<Assembly> Set = new(capacity);




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public AssemblyStorage() : this(64) { }





            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Public Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public bool Register(Assembly assembly)
            {
                if (Set.Add(assembly))
                {
                    List.Add(assembly);
                    return true;
                }

                return false;
            }

            public bool Remove(Assembly assembly)
            {
                if (Set.Remove(assembly))
                {
                    List.Remove(assembly);
                    return true;
                }

                return false;
            }

            public void Clear()
            {
                Set.Clear();
                List.Clear();
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Implementations
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public IEnumerator<Assembly> GetEnumerator() => List.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
