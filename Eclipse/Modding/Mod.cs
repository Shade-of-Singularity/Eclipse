using System;

namespace Eclipse.Modding
{
    public static class Mod<T> where T : Mod
    {
        public static readonly T Instance = Mod.GetOrThrow<T>();
    }

    // TODO: Add thrust checker, which stores "Do you thrust the mod?" flag somewhere.
    //  Later - store this data both locally and on the server for each user (if authenticated), so mods cannot tamper with data.
    //  Any assemblies should not be loaded (as well as textures and AssetBundles) if they were found and mod wasn't approved/trusted.
    //  However, we might introduce a public mod listing with trusted mods.
    //  This is a security question after all. We need to protect users here if we can.
    /// <summary>
    /// Class describes all the mod data it can have.
    /// AssetBundles, assemblies, and other resources are described here with wrappers around dictionaries and resource addresses.
    /// </summary>
    /// <remarks>
    /// You are free to develop mods for the game, but they won't be used in public events before approval.
    /// Approval requires you to expose mod code to the public, and make the last update 2-3 days before the event (in most cases).
    /// Our team will go through the code and verify its contents.
    /// <para>
    /// If mod was already verified and you have a hotfix - we can make a quick verification, but you need to contact out team for that.
    /// This section applies only to those, who's mods were selected for usage in any of the upcoming events.
    /// </para>
    /// Keep in mind that ALL the mods that use BepInEx and Harmony will be disqualified outside of the special cases.
    /// If your mod is large enough, we will consider verifying it, but otherwise - they won't be accepted.
    /// And it doesn't matter that we have a support for BepInEx and Harmony in code - it was made for ease in modding, not for our public events.
    /// Refer to the modding guide to grasp all the tools available for you as a mod-maker by default (that's a lot btw, with native C# support, dw)
    /// <para>
    /// TL;DR:
    /// <para>- Try to use native modding tools for creating verifiable code instead of BepInEx and Harmony.</para>
    /// <para>- Contact us to request new features.</para>
    /// <para>- BepInEx and Harmony mods wont be accepted (for events) outside of special circumstances.</para>
    /// (This message is from Dark)
    /// </para>
    /// </remarks>
    public abstract class Mod
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string EmptyModName = "";
        public const string CoreModName = "Core";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:

        // Properties:
        public bool IsEnabled { get; set; } = true;
        public bool IsLoaded { get; set; } = false;
        public string Name { get; set; } = EmptyModName;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static T GetOrThrow<T>() where T : Mod
        {
            throw new NotImplementedException();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }
}
