using ServiceCore.Loading;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceCore.Modding
{
    /// <summary>
    /// Map of dependencies.
    /// </summary>
    public readonly struct DependencyMap(List<ILoadingSource> list) : IEnumerable<ILoadingSource>
    {
        /// <summary>
        /// List of all dependencies.
        /// </summary>
        public readonly List<ILoadingSource> List = list;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to resolve all dependencies in <see cref="ILoadingSource"/> <see cref="List"/>.
        /// </summary>
        /// <returns><c>true</c> if all dependencies were resolved successfully. <c>false</c> if dependencies are missing or incompatible.</returns>
        public bool TryResolve()
        {
            // At the moment dependencies are not supported yet.
            // Returning false will cause Engine to load only Native assemblies - exactly what we need.
            return false;
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
