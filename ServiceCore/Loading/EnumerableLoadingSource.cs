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

using System.Collections.Generic;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Source of direct <see cref="System.Reflection.Assembly"/> references for <see cref="Engine"/>.
    /// </summary>
    /// <remarks>
    /// By default used for the core of the game.
    /// </remarks>
    /// <param name="identifier"><see cref="ILoadingSource.Identifier"/> to set.</param>
    /// <param name="version">Version that this <see cref="ILoadingSource"/> has.</param>
    /// <param name="loadable">Collection of loadable items to... Well... Load.</param>
    /// <param name="dependencies">Dependencies that this <see cref="ILoadingSource"/> has.</param>
    public sealed class EnumerableLoadingSource(string identifier, Version version, IEnumerable<ILoadable> loadable, DependencyDeclaration[] dependencies) : ILoadingSource
    {
        /// <inheritdoc/>
        public string Identifier => identifier;

        /// <inheritdoc/>
        public Version Version => version;

        /// <inheritdoc/>
        DependencyDeclaration[] ILoadingSource.Dependencies => dependencies;

        /// <inheritdoc/>
        public void GetLoadables(LoadableProvider provider)
        {
            foreach (var loadable in loadable)
            {
                provider(loadable);
            }
        }
    }
}
