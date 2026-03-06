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
    /// Common extensions for using <see cref="ILoadingSource"/>.
    /// </summary>
    public static class LoadingSourceExtensions
    {
        /// <summary>
        /// Wraps input <see cref="IEnumerable{T}"/> of <see cref="ILoadable"/>s in <see cref="EnumerableLoadingSource"/> wrapper.
        /// </summary>
        /// <returns><paramref name="loadables"/> wrapped in <see cref="EnumerableLoadingSource"/>.</returns>
        public static EnumerableLoadingSource ToLoadingSource(this IEnumerable<ILoadable> loadables,
            string? identifier = null, Version version = default, DependencyDeclaration[]? dependencies = null)
        {
            return new(identifier ?? string.Empty, version, loadables, dependencies ?? []);
        }

        /// <summary>
        /// Wraps input <see cref="IEnumerable{T}"/> of <see cref="ILoadable"/>s in <see cref="EnumerableLoadingSource"/> wrapper.
        /// </summary>
        /// <returns><paramref name="loadables"/> wrapped in <see cref="EnumerableLoadingSource"/>.</returns>
        public static ArrayLoadingSource ToLoadingSource(this ILoadable[] loadables,
            string? identifier = null, Version version = default, DependencyDeclaration[]? dependencies = null)
        {
            return new(identifier ?? string.Empty, version, loadables, dependencies ?? []);
        }

        /// <summary>
        /// Wraps input <see cref="IEnumerable{T}"/> of <see cref="ILoadable"/>s in <see cref="EnumerableLoadingSource"/> wrapper.
        /// </summary>
        /// <returns><paramref name="loadables"/> wrapped in <see cref="EnumerableLoadingSource"/>.</returns>
        public static ListLoadingSource ToLoadingSource(this IList<ILoadable> loadables,
            string? identifier = null, Version version = default, DependencyDeclaration[]? dependencies = null)
        {
            return new(identifier ?? string.Empty, version, loadables, dependencies ?? []);
        }
    }
}
