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
