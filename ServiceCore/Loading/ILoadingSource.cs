using ServiceCore.Modding;
using System.Collections.Generic;

namespace ServiceCore.Loading
{
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
        public IList<DependencyDeclaration> Dependencies { get; }

        /// <summary>
        /// Loads-in the data regarding loadable things, like <see cref="System.Reflection.Assembly"/> locations.
        /// </summary>
        public IEnumerable<ILoadable> GetLoadables(); 
    }
}
