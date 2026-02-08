using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eclipse
{
    /// <summary>
    /// Manages info about each loaded-in assembly.
    /// </summary>
    /// <remarks>
    /// Mods can define multiple assemblies. Each of them will be verified separately.
    /// </remarks>
    public static partial class Assemblies
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<string, AssemblyDefinition> m_Assemblies = new Dictionary<string, AssemblyDefinition>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Loads all data from an assembly. It includes: <see cref="Modding.Mod"/>s, <see cref="IService"/>s, etc.
        /// </summary>
        /// <remarks>
        /// Will run (TODO: add entry point method) on assembly load, so it can run some initialization methods.
        /// (Note: might not be implemented in favor of using mod initialization order instead and <see cref="Modding.Mod.Initializing"/>)
        /// </remarks>
        public static void Load(Assembly assembly)
        {
            if (!IsExistInAppDomain(assembly))
            {
                throw new NotSupportedException("You can't load assemblies not from current AppDomain yet.");
            }

            AssemblyDefinition definition = new AssemblyDefinition(assembly, verified: true); // TODO: Add verification.
            m_Assemblies[assembly.FullName] = definition;

            // TODO: Add custom assembly analyzers.
        }

        /// <summary>
        /// Not supported. To support it, we will need to add AppDomain reloading.
        /// There was no experiments with it yet.
        /// </summary>
        /// <exception cref="NotSupportedException">Assembly unloading via AppDomain change is yet to be supported.</exception>
        public static void Unload(Assembly assembly)
        {
            throw new NotSupportedException("Assembly unloading is not supported.");
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if this <paramref name="assembly"/> exist in current <see cref="AppDomain"/> already.
        /// </summary>
        /// <remarks>
        /// Method introduced until very basic ways of providing assembly verification are not provided.
        /// </remarks>
        private static bool IsExistInAppDomain(Assembly assembly)
        {
            foreach (var current in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(current.FullName, assembly.FullName)) return true;
            }

            return false;
        }
    }
}
