using System.Reflection;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Loadable from a direct <see cref="Assembly"/> reference.
    /// </summary>
    /// <param name="assembly">Assembly to reflection-analyze.</param>
    public readonly struct LoadableAssemblyReference(Assembly assembly) : ILoadable
    {
        /// <summary>
        /// Assembly to analyze.
        /// </summary>
        public readonly Assembly assembly = assembly;

        /// <summary>
        /// Creates an <see cref="ILoadable"/> struct from provided <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">Assembly to analyze.</param>
        public static implicit operator LoadableAssemblyReference(Assembly assembly) => new(assembly);
    }
}
