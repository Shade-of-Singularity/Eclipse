namespace ServiceCore.Modding
{
    /// <summary>
    /// Describes a specific dependency.
    /// </summary>
    public readonly struct DependencyDeclaration(string target, bool isAssembly, Version version, VersionDependencyType type)
    {
        /// <summary>
        /// <para>When <see cref="isAssembly"/> is <c>true</c> - <see cref="target"/> is name of an assembly.</para>
        /// <para>Otherwise - <see cref="target"/> is <see cref="ModificationInfo.Identifier"/>.</para>
        /// </summary>
        public readonly string target = target;
        /// <summary>
        /// Whether <see cref="target"/> is assembly or <see cref="ModificationInfo"/>.
        /// </summary>
        public readonly bool isAssembly = isAssembly;
        /// <summary>
        /// <see cref="Version"/> on which this declaration depends on.
        /// </summary>
        public readonly Version version = version;
        /// <summary>
        /// <see cref="Version"/> dependency type on this declaration depends on.
        /// </summary>
        public readonly VersionDependencyType type = type;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
