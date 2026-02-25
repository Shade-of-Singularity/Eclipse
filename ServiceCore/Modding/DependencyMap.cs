using ServiceCore.Loading;
using System.Collections;
using System.Collections.Generic;

namespace ServiceCore.Modding
{
    /// <summary>
    /// Map of dependencies.
    /// </summary>
    public readonly struct DependencyMap : IEnumerable<ILoadingSource>
    {
        /// <summary>
        /// List of all dependencies.
        /// </summary>
        public readonly List<ILoadingSource> List;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DependencyMap() : this(0, null) { }

        /// <summary>
        /// Provides initial capacity to the internal <see cref="List"/>.
        /// </summary>
        /// <param name="capacity">Initial List capacity.</param>
        public DependencyMap(int capacity) : this(capacity, null) { }

        /// <summary>
        /// Provides items for the internal <see cref="List"/>.
        /// </summary>
        /// <param name="sources">Initial items to add to the <see cref="List"/>.</param>
        public DependencyMap(IEnumerable<ILoadingSource>? sources) : this(0, sources) { }

        /// <summary>
        /// Provides both initial capacity and items to the internal <see cref="List"/>
        /// </summary>
        /// <param name="capacity">Initial List capacity.</param>
        /// <param name="sources">Initial items to add to the <see cref="List"/>.</param>
        public DependencyMap(int capacity, IEnumerable<ILoadingSource>? sources = null)
        {
            List = new(capacity);

            if (sources is not null) List.AddRange(sources);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        public IEnumerator<ILoadingSource> GetEnumerator() => List.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    }
}
