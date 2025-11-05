namespace Eclipse.Structs
{
    /// <summary>
    /// Settings for unloading <see cref="Engine"/> with <see cref="Engine.Unload"/> method.
    /// </summary>
    public readonly struct UnloadSettings
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Settings to unload everything from the memory: Textures, models, any other assets, and so on.
        /// </summary>
        public static UnloadSettings UnloadEverything => new UnloadSettings(all: true);

        /// <summary>
        /// Unloading settings usually used for <see cref="Engine.Reload"/>ing.
        /// </summary>
        public static UnloadSettings ReloadSettings => new UnloadSettings(all: false);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether to unload textures from the memory during unloading.
        /// </summary>
        /// <remarks>
        /// (TODO) Used in <see cref="Engine.Reload"/> to ask <see cref="EngineService.Unload"/> to not dereference textures, so they can be reused.
        /// </remarks>
        public readonly bool UnloadTextures;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <param name="all">Whether to unload everything or keep things like Textures in the memory during reloading.</param>
        public UnloadSettings(bool all)
        {
            UnloadTextures = all;
        }
    }
}
