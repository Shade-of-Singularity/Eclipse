using System.Reflection;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Loadable from a direct <see cref="Assembly"/> reference.
    /// </summary>
    /// <param name="path">Path of an <see cref="Assembly"/> to load-in.</param>
    public readonly struct LoadableAssembly(string path) : ILoadable
    {
        /// <summary>
        /// Assembly to analyze.
        /// </summary>
        public readonly string Path = path;

        /// <summary>
        /// Creates an <see cref="ILoadable"/> struct from provided <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Assembly to analyze.</param>
        public static implicit operator LoadableAssembly(string path) => new(path);
    }
}
