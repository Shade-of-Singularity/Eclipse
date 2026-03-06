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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Map of dependencies.
    /// </summary>
    public sealed class DependencyMap : IEnumerable<ILoadingSource>, IDictionary<string, ILoadingSource>, IEnumerable, IDisposable
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <see cref="DependencyMap"/> describing only <see cref="Engine.NativeAssemblies"/>.
        /// Returned as a default value in related methods.
        /// </summary>
        public static DependencyMap Native => Engine.NativeDependencyMap;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Direct accessor to an internal <see cref="Dictionary{TKey, TValue}"/> with all <see cref="ILoadingSource"/>s.
        /// </summary>
        public ILoadingSource this[string key]
        {
            get => m_Map[key];
            set
            {
                m_Map[key] = value;
                m_Sources = null;
            }
        }

        /// <inheritdoc/>
        public ICollection<string> Keys => m_Map.Keys;

        /// <inheritdoc/>
        public ICollection<ILoadingSource> Values => m_Map.Values;

        /// <inheritdoc/>
        public int Count => m_Map.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Map of all dependencies.
        /// </summary>
        private readonly Dictionary<string, ILoadingSource> m_Map = new(StringComparer.Ordinal);
        /// <summary>
        /// Ordered sources.
        /// </summary>
        private ILoadingSource[]? m_Sources = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private sealed class Node(ILoadingSource source) : IEquatable<Node>
        {
            public readonly ILoadingSource Source = source;
            public readonly HashSet<Node> Children = [];
            public int InDegree;

            public override string ToString() => $"{Source}";
            public override int GetHashCode() => RuntimeHelpers.GetHashCode(Source);
            public override bool Equals(object obj) => obj is Node other && Equals(other);
            public bool Equals(Node other) => ReferenceEquals(other.Source, Source);
        }

        private enum DependencyResult : byte
        {
            Ignored = 0,
            Included,
            Incompatible,
        }

        /// <summary>
        /// Attempts to resolve all dependencies. Returns cached <paramref name="sources"/> array if already resolved.
        /// </summary>
        /// <returns><c>true</c> if all dependencies were resolved successfully. <c>false</c> if dependencies are missing or incompatible.</returns>
        public bool TryResolve(out IReadOnlyList<ILoadingSource> sources)
        {
            // Cached result is returned by default.
            if (m_Sources is not null)
            {
                sources = m_Sources;
                return true;
            }

            bool ExpectFrequentReloading = false;
            if (ExpectFrequentReloading)
            {
                // TODO: Use cached Queue and Arrays.
                throw new NotSupportedException("Frequent reloading is not supported.");
            }

            // Lists all nodes.
            Node[] nodes = new Node[Count];
            Dictionary<string, Node> map = new(nodes.Length, StringComparer.Ordinal);
            int index = 0;
            foreach (var source in Values)
            {
                Node node = new(source);
                nodes[index++] = node;
                map[source.Identifier] = node;
            }

            // Creates associations.
            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                foreach (var dependency in node.Source.Dependencies)
                {
                    switch (AnalyzeDependency(map, dependency))
                    {
                        case DependencyResult.Ignored: break;
                        case DependencyResult.Included:
                            if (map.TryGetValue(dependency.target, out Node target))
                            {
                                node.InDegree++;
                                target.Children.Add(node);
                            }
                            break;

                        default:
                        case DependencyResult.Incompatible:
                            sources = [];
                            return false;
                    }
                }
            }

            // Resolves all dependencies.
            ILoadingSource[] result = new ILoadingSource[nodes.Length];
            Queue<Node> queue = new(nodes.Length);
            for (int i = 0; i < nodes.Length; i++)
            {
                // Should start from a root.
                if (nodes[i].InDegree == 0)
                    queue.Enqueue(nodes[i]);
            }

            int head = 0;
            while (queue.TryDequeue(out Node node))
            {
                foreach (var child in node.Children)
                {
                    if (--child.InDegree == 0)
                        queue.Enqueue(child);
                }


                result[head++] = node.Source;
            }

            if (head == result.Length)
            {
                sources = result;
                return true;
            }

            // Some nodes are in a loop.
            sources = [];
            return false;
        }

        private static DependencyResult AnalyzeDependency(Dictionary<string, Node> nodes, DependencyDeclaration dependency)
        {
            // TODO: Add version checking.
            Node target;
            switch (dependency.type & VersionDependencyType.BothModifiers)
            {
                // TODO: Add dependency reporting and continue looking for incompatibilities.
                // Functional note: Required to be found. Invalidates dependencies if not found.
                case VersionDependencyType.Any:
                    if (!nodes.TryGetValue(dependency.target, out target))
                    {
                        // If specific dependency is not found - invalidate dependencies.
                        return DependencyResult.Incompatible;
                    }

                    return (dependency.type & VersionDependencyType.TypeMask) switch
                    {
                        VersionDependencyType.Smaller =>
                        target.Source.Version < dependency.version ? DependencyResult.Included : DependencyResult.Incompatible,
                        VersionDependencyType.SmallerOrEqual =>
                        target.Source.Version <= dependency.version ? DependencyResult.Included : DependencyResult.Incompatible,
                        VersionDependencyType.Equal =>
                        target.Source.Version == dependency.version ? DependencyResult.Included : DependencyResult.Incompatible,
                        VersionDependencyType.LargerOrEqual =>
                        target.Source.Version >= dependency.version ? DependencyResult.Included : DependencyResult.Incompatible,
                        VersionDependencyType.Larger =>
                        target.Source.Version > dependency.version ? DependencyResult.Included : DependencyResult.Incompatible,
                        _ => DependencyResult.Incompatible,
                    };

                // Functional note: Invalidates dependencies if found.
                case VersionDependencyType.BothModifiers:
                case VersionDependencyType.Incompatible:
                    if (!nodes.TryGetValue(dependency.target, out target))
                    {
                        // If specific dependency is not found - invalidate dependencies.
                        return DependencyResult.Ignored;
                    }

                    return (dependency.type & VersionDependencyType.TypeMask) switch
                    {
                        VersionDependencyType.Smaller =>
                        target.Source.Version < dependency.version ? DependencyResult.Incompatible : DependencyResult.Included,
                        VersionDependencyType.SmallerOrEqual =>
                        target.Source.Version <= dependency.version ? DependencyResult.Incompatible : DependencyResult.Included,
                        VersionDependencyType.Equal =>
                        target.Source.Version == dependency.version ? DependencyResult.Incompatible : DependencyResult.Included,
                        VersionDependencyType.LargerOrEqual =>
                        target.Source.Version >= dependency.version ? DependencyResult.Incompatible : DependencyResult.Included,
                        VersionDependencyType.Larger =>
                        target.Source.Version > dependency.version ? DependencyResult.Incompatible : DependencyResult.Included,
                        _ => DependencyResult.Included,
                    };

                // Functional note: Included only if found.
                case VersionDependencyType.Optional:
                    if (!nodes.TryGetValue(dependency.target, out target))
                    {
                        // If specific dependency is not found - invalidate dependencies.
                        return DependencyResult.Ignored;
                    }

                    return (dependency.type & VersionDependencyType.TypeMask) switch
                    {
                        VersionDependencyType.Smaller =>
                        target.Source.Version < dependency.version ? DependencyResult.Included : DependencyResult.Ignored,
                        VersionDependencyType.SmallerOrEqual =>
                        target.Source.Version <= dependency.version ? DependencyResult.Included : DependencyResult.Ignored,
                        VersionDependencyType.Equal =>
                        target.Source.Version == dependency.version ? DependencyResult.Included : DependencyResult.Ignored,
                        VersionDependencyType.LargerOrEqual =>
                        target.Source.Version >= dependency.version ? DependencyResult.Included : DependencyResult.Ignored,
                        VersionDependencyType.Larger =>
                        target.Source.Version > dependency.version ? DependencyResult.Included : DependencyResult.Ignored,
                        _ => DependencyResult.Ignored,
                    };

                default: throw new SwitchExpressionException($"{Engine.LogPrefix} How (in DependencyMap)");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine($"Dependency map (length: {m_Map.Count}). Is resolved? ({m_Sources is not null})");
            if (m_Sources is not null)
            {
                var array = m_Sources;
                for (int i = 0; i < array.Length; i++)
                {
                    builder.AppendLine($"[{i}] {array[i]}");
                }
            }
            else
            {
                int i = 0;
                foreach (var dependency in m_Map)
                {
                    builder.AppendLine($"[{i++}] {dependency}");
                }
            }

            return builder.ToString();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        public IEnumerator<ILoadingSource> GetEnumerator() => m_Map.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_Map.Values.GetEnumerator();

        /// <inheritdoc cref="Add(string, ILoadingSource)"/>
        public void Add(ILoadingSource value) => m_Map.Add(value.Identifier, value);

        /// <inheritdoc/>
        public void Add(string key, ILoadingSource value) => m_Map.Add(key, value);

        /// <inheritdoc/>
        public void Clear()
        {
            m_Map.Clear();
            m_Sources = [];
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key) => m_Map.ContainsKey(key);

        /// <inheritdoc/>
        public bool Remove(string key) => m_Map.Remove(key);

        /// <inheritdoc/>
        public bool TryGetValue(string key, out ILoadingSource value) => m_Map.TryGetValue(key, out value);

        /// <inheritdoc/>
        public void Add(KeyValuePair<string, ILoadingSource> item) => m_Map.Add(item.Key, item.Value);

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, ILoadingSource> item)
            => ((ICollection<KeyValuePair<string, ILoadingSource>>)m_Map).Contains(item);

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, ILoadingSource>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<string, ILoadingSource>>)m_Map).CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, ILoadingSource> item)
            => ((ICollection<KeyValuePair<string, ILoadingSource>>)m_Map).Remove(item);

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<string, ILoadingSource>> IEnumerable<KeyValuePair<string, ILoadingSource>>.GetEnumerator() => m_Map.GetEnumerator();

        /// <inheritdoc/>
        public void Dispose()
        {
            m_Map.Clear();
            m_Sources = null;
        }
    }
}
