/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using System;

namespace Eclipse.Modding
{
    public static class Mod<T> where T : Mod
    {
        public static T Instance
        {
            get
            {
                if (m_Instance is null)
                {
                    m_Instance = ModManager.GetOrThrow<T>();
                    Engine.OnEngineResetting += () => m_Instance = null;
                }

                return m_Instance;
            }
        }

        private static T? m_Instance;
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
    public abstract class Mod : IEngineModDirectAccess
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default mod name to be used.
        /// </summary>
        public const string EmptyModName = "";




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
        public virtual string Name => EmptyModName;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:

        // Local Fields:
        private IEngineModDirectAccess.Callback m_SkippedCallbacks = IEngineModDirectAccess.Callback.Unloading | IEngineModDirectAccess.Callback.Unloaded;




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
        void IEngineModDirectAccess.EngineInvokeInitializing()
        {
            if ((m_SkippedCallbacks & IEngineModDirectAccess.Callback.Initializing) == IEngineModDirectAccess.Callback.None)
            {
                m_SkippedCallbacks |= IEngineModDirectAccess.Callback.Initializing;
                Initializing();
            }
        }

        void IEngineModDirectAccess.EngineInvokeInitialized()
        {
            if ((m_SkippedCallbacks & IEngineModDirectAccess.Callback.Initialized) == IEngineModDirectAccess.Callback.None)
            {
                m_SkippedCallbacks |= IEngineModDirectAccess.Callback.Initialized;
                Initialized();
            }
        }

        void IEngineModDirectAccess.EngineInvokeGameLoaded()
        {
            if ((m_SkippedCallbacks & IEngineModDirectAccess.Callback.GameLoaded) == IEngineModDirectAccess.Callback.None)
            {
                m_SkippedCallbacks |= IEngineModDirectAccess.Callback.GameLoaded;
                GameLoaded();
            }
        }

        void IEngineModDirectAccess.EngineInvokeUnloading()
        {
            if ((m_SkippedCallbacks & IEngineModDirectAccess.Callback.Unloading) == IEngineModDirectAccess.Callback.None)
            {
                m_SkippedCallbacks |= IEngineModDirectAccess.Callback.Unloading;
                Unloading();
            }
        }

        void IEngineModDirectAccess.EngineInvokeUnloaded()
        {
            if ((m_SkippedCallbacks & IEngineModDirectAccess.Callback.Unloaded) == IEngineModDirectAccess.Callback.None)
            {
                m_SkippedCallbacks = IEngineModDirectAccess.Callback.Unloading | IEngineModDirectAccess.Callback.Unloaded;
                Unloaded();
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Called synchronously when mod is first loaded, before any services are initialized. 
        /// </summary>
        /// <remarks>
        /// Other services might not be available at this point.
        /// </remarks>
        protected virtual void Initializing() { }

        /// <summary>
        /// Called synchronously when all mod data and services were loaded.
        /// </summary>
        /// <remarks>
        /// Services from next mods in a queue will not be accessible at this point.
        /// </remarks>
        protected virtual void Initialized() { }

        /// <summary>
        /// Called synchronously when all mods in the entire game was successfully initialized.
        /// </summary>
        protected virtual void GameLoaded() { }

        /// <summary>
        /// Called synchronously when mod and services is right about to be unloaded. 
        /// </summary>
        /// <remarks>
        /// Unloading order is reversed to the loading order. Do not expect any service to be available at this point.
        /// </remarks>
        protected virtual void Unloading() { }

        /// <summary>
        /// Called synchronously when all mod data and services were unloaded.
        /// </summary>
        /// <remarks>
        /// Unloading order is reversed to the loading order. Do not expect any service to be available at this point.
        /// </remarks>
        protected virtual void Unloaded() { }
    }

    // TODO: Review important callbacks and when they are called.
    public interface IEngineModDirectAccess
    {
        [Flags]
        public enum Callback : byte
        {
            None = 0b000_0000,
            Initializing = 0b0000_0001,
            Initialized = 0b0000_0010,
            GameLoaded = 0b0000_0100,
            Unloading = 0b0000_1000,
            Unloaded = 0b0001_0000,
        }

        void EngineInvokeInitializing();
        void EngineInvokeInitialized();
        void EngineInvokeGameLoaded();
        void EngineInvokeUnloading();
        void EngineInvokeUnloaded();
    }
}
