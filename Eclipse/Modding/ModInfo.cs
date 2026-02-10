using System;

namespace Eclipse.Modding
{
    /// <summary>
    /// Information about <see cref="Mod"/>, to use before loading it in.
    /// </summary>
    /// TODO: Finish.
    public sealed class ModInfo
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Assemblies that this mod will try to load-in.
        /// </summary>
        /// <remarks>
        /// Those are <see cref="System.Reflection.Assembly.FullName"/>s (?).
        /// </remarks>
        public string[] Assemblies { get; set; } = Array.Empty<string>();
    }
}
