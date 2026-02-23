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

using ServiceCore.Parameters;

namespace ServiceCore
{
    public static partial class Engine
    {
        /// <summary>
        /// General settings of the engine.
        /// </summary>
        /// TODO: Specify which settings are meant to be saved in cloud or locally.
        public static class Settings
        {
            /// <summary>
            /// Streamer mode hides sensitive info on the screen.
            /// </summary>
            /// <remarks>
            /// (TODO) Forcefully set to 'true' if <see cref="Flags"/> contains <see cref="Flags.StreamerModeFlag"/>.
            /// </remarks>
            public static readonly Parameter<bool> StreamerMode = Parameter<bool>.Get(nameof(StreamerMode), false);



            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            //[AfterServiceInitialized(typeof(IConfigurationService))]
            //internal static void Initialize()
            //{
            //    if (Flags.StreamerMode)
            //    {
            //        StreamerMode.DefaultValue = StreamerMode.Value = true;
            //    }
            //}
        }
    }
}
