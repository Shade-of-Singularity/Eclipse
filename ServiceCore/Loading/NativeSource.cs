using ServiceCore.Modding;
using System.Collections.Generic;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Source of direct <see cref="System.Reflection.Assembly"/> references for <see cref="Engine"/>.
    /// </summary>
    /// <remarks>
    /// By default used for the core of the game.
    /// </remarks>
    /// <param name="loadable">Collection of loadable items to... Well... Load.</param>
    public readonly struct NativeSource(IEnumerable<ILoadable> loadable) : ILoadingSource
    {
        /// <inheritdoc/>
        public string Identifier => Modifications.CoreModificationName;

        /// <inheritdoc/>
        public Version Version => Version.Zero; // TODO: Replace with app's/game's version.

        /// <inheritdoc/>
        public IList<DependencyDeclaration> Dependencies => [];

        /// <inheritdoc/>
        public IEnumerable<ILoadable> GetLoadables() => loadable;
    }
}
