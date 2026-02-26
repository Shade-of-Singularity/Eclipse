using ServiceCore.Loading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServiceCore.Modding
{
    /// <summary>
    /// Map of dependencies.
    /// </summary>
    public sealed class DependencyMap : IEnumerable<ILoadingSource>, IDictionary<string, ILoadingSource>, IEnumerable
    {
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
        private ILoadingSource[]? m_Sources;

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
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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

            ILoadingSource[] array = new ILoadingSource[m_Map.Count];
            m_Map.Values.CopyTo(array, 0);
            sources = array;

            for (int i = 0; i < array.Length; i++)
            {
                ILoadingSource source = array[i];
                string identifier = source.Identifier;
                int maxIndex = i;
                foreach (var dependency in source.Dependencies)
                {
                    // TODO: Add version checking.
                    int index;
                    ILoadingSource target;
                    switch (dependency.type & VersionDependencyType.ExclusionMask)
                    {
                        case VersionDependencyType.Any:
                            index = Array.FindIndex(array, (src) => string.Equals(identifier, src.Identifier, StringComparison.Ordinal));
                            if (index == -1)
                            {
                                // If specific dependency is not found - invalidate dependencies.
                                // TODO: Add dependency reporting and continue looking for incompatibilities.
                                sources = array;
                                return false;
                            }

                            target = array[index];
                            switch (dependency.type & VersionDependencyType.TypeMask)
                            {
                                case VersionDependencyType.Smaller:
                                    if (target.Version < dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                case VersionDependencyType.SmallerOrEqual:
                                    if (target.Version <= dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                case VersionDependencyType.Equal:
                                    if (target.Version == dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                case VersionDependencyType.LargerOrEqual:
                                    if (target.Version >= dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                case VersionDependencyType.Larger:
                                    if (target.Version > dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                        break;
                                    }
                                    else
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                default: // Simply presence of a target is good enough of a reason for dependency invalidation.
                                case VersionDependencyType.TypeMask: // TODO: Add dependency reporting and continue looking for incompatibilities.
                                case VersionDependencyType.Any: maxIndex = Math.Max(maxIndex, index); break;
                            }
                            break;

                        case VersionDependencyType.ExclusionMask:
                        case VersionDependencyType.Incompatible:
                            index = Array.FindIndex(array, (src) => string.Equals(identifier, src.Identifier, StringComparison.Ordinal));
                            if (index == -1)
                            {
                                break;
                            }

                            target = array[index];
                            switch (dependency.type & VersionDependencyType.TypeMask)
                            {
                                case VersionDependencyType.Smaller:
                                    if (target.Version < dependency.version)
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                    break;

                                case VersionDependencyType.SmallerOrEqual:
                                    if (target.Version <= dependency.version)
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                    break;

                                case VersionDependencyType.Equal:
                                    if (target.Version == dependency.version)
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                    break;

                                case VersionDependencyType.LargerOrEqual:
                                    if (target.Version >= dependency.version)
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                    break;

                                case VersionDependencyType.Larger:
                                    if (target.Version > dependency.version)
                                    {
                                        // TODO: Add dependency reporting and continue looking for incompatibilities.
                                        sources = array;
                                        return false;
                                    }

                                    break;

                                default: // Simply presence of a target is good enough of a reason for dependency invalidation.
                                case VersionDependencyType.TypeMask: // TODO: Add dependency reporting and continue looking for incompatibilities.
                                case VersionDependencyType.Any: sources = array; return false;
                            }
                            break;

                        case VersionDependencyType.Optional:
                            index = Array.FindIndex(array, (src) => string.Equals(identifier, src.Identifier, StringComparison.Ordinal));
                            if (index == -1)
                            {
                                break;
                            }

                            target = array[index];
                            switch (dependency.type & VersionDependencyType.TypeMask)
                            {
                                case VersionDependencyType.Smaller:
                                    if (target.Version < dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                    }

                                    break;

                                case VersionDependencyType.SmallerOrEqual:
                                    if (target.Version <= dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                    }

                                    break;

                                case VersionDependencyType.Equal:
                                    if (target.Version == dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                    }

                                    break;

                                case VersionDependencyType.LargerOrEqual:
                                    if (target.Version >= dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                    }

                                    break;

                                case VersionDependencyType.Larger:
                                    if (target.Version > dependency.version)
                                    {
                                        maxIndex = Math.Max(maxIndex, index);
                                    }

                                    break;

                                default: // Simply presence of a target is good enough of a reason for dependency invalidation.
                                case VersionDependencyType.TypeMask: // TODO: Add dependency reporting and continue looking for incompatibilities.
                                case VersionDependencyType.Any: maxIndex = Math.Max(maxIndex, index); break;
                            }
                            break;

                        default: throw new SwitchExpressionException("How");
                    }
                }

                if (maxIndex <= i)
                {
                    // Nothing has moved.
                    continue;
                }

                Array.Copy(array, i + 1, array, i, maxIndex - i - 1);
                array[maxIndex] = source;
            }

            m_Sources = array;
            sources = array;
            return true;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public void Clear()
        {
            m_Map.Clear();
            m_Sources = null;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        public IEnumerator<ILoadingSource> GetEnumerator() => m_Map.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_Map.Values.GetEnumerator();

        /// <inheritdoc/>
        public void Add(string key, ILoadingSource value) => m_Map.Add(key, value);

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
    }
}
