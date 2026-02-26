
using ServiceCore.Loading;
using System.Collections.Generic;

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
namespace ServiceCore.Modding
{
    /// <summary>
    /// Information about <see cref="Modification"/>, to use before loading it in.
    /// </summary>
    /// TODO: Finish.
    public sealed class ModificationInfo(string identifier, Version version) : ILoadingSource
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        string ILoadingSource.Identifier => Identifier;
        Version ILoadingSource.Version => Version;
        IList<DependencyDeclaration> ILoadingSource.Dependencies => Dependencies;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Identifier of a <see cref="Modification"/>.
        /// </summary>
        public readonly string Identifier = identifier;
        /// <summary>
        /// Identifier of a <see cref="Modification"/>.
        /// </summary>
        public readonly Version Version = version;
        /// <summary>
        /// Modifications that this mod declares.
        /// </summary>
        /// <remarks>
        /// Those are <see cref="System.Reflection.Assembly.FullName"/>s (?).
        /// </remarks>
        public DependencyDeclaration[] Dependencies = [];
        /// <summary>
        /// Assemblies that will have to be loaded (in a specified order) before mod can be initialized.
        /// </summary>
        /// <remarks>
        /// <see cref="Engine"/> will load-in all the assemblies first before analyzing them.
        /// </remarks>
        public string[] AssemblyPaths = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public IEnumerable<ILoadable> GetLoadables()
        {
            var array = AssemblyPaths;
            for (int i = 0; i < array.Length; i++)
            {
                yield return (LoadableAssembly)array[i];
            }
        }
    }
}
