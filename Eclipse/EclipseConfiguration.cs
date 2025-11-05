using Eclipse.Configuration;
using Eclipse.Structs;
using System;
using System.Collections.Generic;

namespace Eclipse
{
    /// <summary>
    /// Special configuration file that will be manually loaded-in from in-game resources.
    /// Used to, for example, specify specific assemblies that has to be analyzed when engine starts.
    /// </summary>
    /// <remarks>
    /// (TODO) Main Unity assembly is automatically added to the list.
    /// </remarks>
    public sealed class EclipseConfiguration : ImbeddedConfiguration<EclipseConfiguration>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// [Cached] List of all assembly names (see also: <see cref="System.Reflection.AssemblyName.Name"/>),
        /// with <see cref="ExcludedAssemblyNames"/> excluded from the list.
        /// </summary>
        public string[] TargetAssemblyNames
        {
            get
            {
                if (ProcessedAssemblies is null)
                {
                    List<string> names = new List<string>(FoundAssemblyNames.Length);
                    for (int i = 0; i < names.Count; i++)
                    {
                        var name = names[i];
                        for (int j = 0; j < ExcludedAssemblyNames.Length; j++)
                        {
                            var excluded = ExcludedAssemblyNames[j];
                            if (name.Equals(excluded, StringComparison.Ordinal))
                            {
                                // Skips assembly name if it was explicitly excluded.
                                goto NextItem;
                            }
                        }

                        // Only if never skipped, the name will be added.
                        names.Add(name);

                        // Allows to skip adding currently analyzed assembly name to the general list.
                        NextItem:;
                    }

                    return ProcessedAssemblies = names.ToArray();
                }
                else
                {
                    return ProcessedAssemblies;
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// When <see cref="Engine"/> should be automatically initialized.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="EclipseInitializationType.Manual"/> to prevent any automatic initialization and to handle it manually from your code.
        /// (TODO) Alternatively, you can use special MonoBehaviours for initializing.
        /// </remarks>
        public EclipseInitializationType InitializationType = EclipseInitializationType.AfterAssembliesLoaded;

        // TODO: Finish.
        // TODO: Automatically create one if there is no configuration file in any resource folder, on InitializeOnLoadMethod.
        //  This should be made in Eclipse.Editor.
        /// <summary>
        /// List of names to all <see cref="System.Reflection.Assembly"/> files to analyze.
        /// </summary>
        [UnityEngine.HideInInspector, UnityEngine.SerializeField] public string[] FoundAssemblyNames = Array.Empty<string>();

        /// <summary>
        /// List of all assemblies to exclude from analysis on initialization.
        /// </summary>
        public string[] ExcludedAssemblyNames = Array.Empty<string>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private string[]? ProcessedAssemblies = null;
    }
}
