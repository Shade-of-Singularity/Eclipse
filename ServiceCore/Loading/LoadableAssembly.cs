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

using System.Reflection;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Loadable from a direct <see cref="Assembly"/> reference.
    /// </summary>
    /// <param name="path">Path of an <see cref="Assembly"/> to load-in.</param>
    public readonly struct LoadableAssembly(string path) : ILoadable
    {
        /// <summary>
        /// Assembly to analyze.
        /// </summary>
        public readonly string Path = path;

        /// <summary>
        /// Creates an <see cref="ILoadable"/> struct from provided <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Assembly to analyze.</param>
        public static implicit operator LoadableAssembly(string path) => new(path);
    }
}
