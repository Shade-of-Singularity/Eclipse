namespace ServiceCore.Modding
{
    /// <summary>
    /// How exactly a thing depends on an <see cref="Version"/>.
    /// </summary>
    public enum VersionDependencyType : byte
    {
        /// <summary>
        /// Any <see cref="Version"/> - just a dependency should be present.
        /// </summary>
        Any = 0b000,
        /// <summary>
        /// <see cref="Version"/> smaller than declared one.
        /// </summary>
        Smaller = 0b001,
        /// <summary>
        /// <see cref="Version"/> smaller or equal than declared one.
        /// </summary>
        SmallerOrEqual = 0b011,
        /// <summary>
        /// <see cref="Version"/> equal to declared one.
        /// </summary>
        Equal = 0b010,
        /// <summary>
        /// <see cref="Version"/> larger or equal than declared one.
        /// </summary>
        LargerOrEqual = 0b110,
        /// <summary>
        /// <see cref="Version"/> larger than declared one.
        /// </summary>
        Larger = 0b100,
        /// <summary>
        /// Mask covering <see cref="Any"/>, <see cref="Smaller"/>, <see cref="SmallerOrEqual"/>,
        /// <see cref="Equal"/>, <see cref="LargerOrEqual"/>, and <see cref="Larger"/> types.
        /// </summary>
        TypeMask = 0b111,

        /// <remarks>
        /// Dependency on an <see cref="Version"/> is optional.
        /// <para>
        /// Can be used to purposefully alter initialization order.
        /// You can declare some library as having optional dependency, but never actually use anything from said library.
        /// It will result in said library initializing first.
        /// </para>
        /// </remarks>
        Optional = 0b01_000_000,
        /// <remarks>
        /// Incompatible with an <see cref="Version"/> or a declaration generally.
        /// </remarks>
        Incompatible = 0b10_000_000,
        /// <remarks>
        /// Mask covering <see cref="Optional"/> and <see cref="Incompatible"/> states.
        /// </remarks>
        ExclusionMask = 0b11_000_000,
    }
}
