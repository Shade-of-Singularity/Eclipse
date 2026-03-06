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

using System.Text;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Zero-allocation loadable provider, injected by <see cref="Engine"/> to all affected <see cref="ILoadingSource"/>s
    /// during <see cref="Engine.Initialize(InitializationContext, IInitializationArgs?)"/>.
    /// </summary>
    /// <param name="loadable"></param>
    public delegate void LoadableProvider(ILoadable loadable);

    /// <summary>
    /// Source of data that can be loaded by the engine.
    /// </summary>
    public interface ILoadingSource
    {
        /// <summary>
        /// Identifier of a source.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Version of the loadable source.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// All dependencies for this source.
        /// </summary>
        public DependencyDeclaration[] Dependencies { get; }

        /// <summary>
        /// Loads-in the data regarding loadable things, like <see cref="System.Reflection.Assembly"/> locations.
        /// </summary>
        public void GetLoadables(LoadableProvider provider);

        /// <summary>
        /// Default <see cref="object.ToString"/> implementation.
        /// </summary>
        /// <param name="source">Source to stringify.</param>
        /// <param name="singleLine">Whether to stringify it as a single line or not. Used for compatibility with <see cref="DependencyMap.ToString"/>.</param>
        public static string ToString(ILoadingSource source, bool singleLine = true)
        {
            StringBuilder builder = new();
            builder.Append(source.Identifier);
            builder.Append(' ');
            builder.Append('(');
            builder.Append(source.Version);
            builder.Append(')');
            builder.Append(' ');

            var array = source.Dependencies;
            if (singleLine || array.Length == 0) // Length == 0 - simply inline squared braces.
            {
                builder.Append('[');
                for (int i = 0; i < array.Length; i++)
                {
                    builder.Append($"[{i}] {array[i]}");
                }
                builder.Append(']');
            }
            else
            {
                builder.AppendLine("[");
                for (int i = 0; i < array.Length; i++)
                {
                    builder.AppendLine($"[{i}] {array[i]}");
                }
                builder.AppendLine("]");
            }

            return builder.ToString();
        }
    }
}
