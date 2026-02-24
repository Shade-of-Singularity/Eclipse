using ServiceCore.Loading;
using System.Collections.Generic;

namespace ServiceCore
{
    /// <summary>
    /// Describes how <see cref="Engine"/> should be initialized, and what it should initialize.
    /// </summary>
    public readonly struct InitializationContext(Engine.AssemblySorter? sorter = null, IEnumerable<ILoadingSource>? sources = null)
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Sorter for native assemblies.
        /// </summary>
        /// <seealso cref="Engine.NativeAssemblies"/>
        public readonly Engine.AssemblySorter? NativeSorter = sorter;
        /// <summary>
        /// All loading sources for <see cref="Engine"/> to initialize.
        /// </summary>
        public readonly IEnumerable<ILoadingSource>? Sources = sources;
    }
}
