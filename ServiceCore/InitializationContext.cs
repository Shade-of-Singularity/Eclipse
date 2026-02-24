using ServiceCore.Modding;

namespace ServiceCore
{
    /// <summary>
    /// Describes how <see cref="Engine"/> should be initialized, and what it should initialize.
    /// </summary>
    public readonly struct InitializationContext(Engine.AssemblySorter? sorter = default, DependencyMap? dependencies = default)
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
        /// Dependency map of all <see cref="ModificationInfo"/>s.
        /// </summary>
        public readonly DependencyMap? Dependencies = dependencies;
    }
}
