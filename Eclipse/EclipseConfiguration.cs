using System;
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Special configuration file that will be manually loaded-in from in-game resources.
    /// Used to, for example, specify specific assemblies that has to be analyzed when engine starts.
    /// </summary>
    /// <remarks>
    /// (TODO) Main Unity assembly is automatically added to the list.
    /// </remarks>
    public sealed class EclipseConfiguration : ScriptableObject
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // TODO: Finish.
        // TODO: Automatically create one if there is no configuration file in any resource folder, on InitializeOnLoadMethod.
        //  This should be made in Eclipse.Editor.
        public string[] AssemblyNames = Array.Empty<string>();
    }
}
