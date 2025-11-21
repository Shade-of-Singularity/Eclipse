using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Eclipse.Editor
{
    public static class EclipseProcessor
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:

        // Properties:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Serialized Fields:

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Unity Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [InitializeOnLoadMethod]
        public static void UpdateResourcesReferences()
        {
            List<GUID> guids = new List<GUID>(AssetDatabase.FindAssetGUIDs($"t:{nameof(EclipseConfiguration)}"));
            for (int i = 0; i < guids.Count; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!path.Contains("Resources"))
                {
                    guids.RemoveAt(i); i--;
                }
            }

            EclipseConfiguration configuration;
            if (guids.Count == 0)
            {
                const string DefaultFolder = "Assets/Resources/Configurations/Imbedded";
                AssetDatabaseExtensions.EnsurePathExists(DefaultFolder);

                configuration = ScriptableObject.CreateInstance<EclipseConfiguration>();
                AssetDatabase.CreateAsset(configuration, Path.Join(DefaultFolder, "Eclipse Configuration.asset"));
                EditorUtility.SetDirty(configuration);

                EditorApplication.delayCall += () =>
                {
                    StringBuilder builder = new StringBuilder(512);
                    Debug.LogWarning($"No {nameof(EclipseConfiguration)} file was found in the entire project. New file was created at: \"{DefaultFolder}\"");
                };

                return;
            }

            configuration = AssetDatabase.LoadAssetByGUID<EclipseConfiguration>(guids[0]);
            if (guids.Count >= 2)
            {

                EditorApplication.delayCall += () =>
                {
                    StringBuilder builder = new StringBuilder(512);
                    builder.Append($"A total of ({guids.Count}) different {nameof(EclipseConfiguration)} files we found.");
                    builder.Append($" This is not allowed, and only the first one will be used. Keep only one configuration file at all times. Paths:\n");
                    foreach (var guid in guids)
                    {
                        builder.Append("- ");
                        builder.AppendLine(AssetDatabase.GUIDToAssetPath(guid));
                    }
                };
            }

            // We update the list of assemblies to analyze.
            // TODO: Only analyze the assembly if it actually present in the game. Use name only as a look-up reference.
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
            var names = new List<string>(assemblies.Length);
            for (int i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                if (string.IsNullOrWhiteSpace(assembly.rootNamespace))
                {
                    // Root is empty in Unity's DLLs for some reason.
                    // I believe that's because you don't have direct access to their assemblies at compile time?
                    // Whatever the reason - we can use it to filter-out assemblies that we don't need to process.
                    continue;
                }

                names.Add(assembly.name);
            }

            if (configuration.FoundAssemblyNames.Length != names.Count)
            {
                configuration.FoundAssemblyNames = new string[names.Count];
            }

            names.CopyTo(configuration.FoundAssemblyNames);
            EditorUtility.SetDirty(configuration);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    }
}
