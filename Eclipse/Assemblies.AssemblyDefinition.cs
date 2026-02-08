using System.Reflection;

namespace Eclipse
{
public static partial class Assemblies
    {
        /// <summary>
        /// Definition on an assembly.
        /// </summary>
        public readonly struct AssemblyDefinition
        {
            /// <summary>
            /// An analyzed assembly.
            /// </summary>
            public readonly Assembly assembly;
            /// <summary>
            /// Whether assembly was internally verified successfully.
            /// </summary>
            public readonly bool verified;
            /// <summary>
            /// Full constructor.
            /// </summary>
            public AssemblyDefinition(Assembly assembly, bool verified)
            {
                this.assembly = assembly;
                this.verified = verified;
            }
        }
    }
}
