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
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Default Engine <see cref="QuitHandler"/> implementation.
    /// </summary>
    /// <remarks>
    /// You can inherit <see cref="QuitHandler{T}"/> yourself and modify it
    /// to make an animated exit screen or something like that ^-^
    /// </remarks>
    public sealed class DefaultQuitHandler : QuitHandler<DefaultQuitHandler>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private bool isUnloading;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override bool Interrupt()
        {
            // If already unloads - no reason to start another unloading session.
            if (isUnloading) return false;
            if (Enabled && !isUnloading)
            {
                // Allows starting Engine unloading only 
                AsyncQuit();
            }

            return false;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Actually runs <see cref="Engine.Terminate"/>.
        /// </summary>
        /// <remarks>
        /// See <see cref="Interrupt"/> to understand how it is used.
        /// </remarks>
        private async void AsyncQuit()
        {
            isUnloading = true;
            try
            {
                await Engine.Terminate();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            isUnloading = false;
            Application.Quit(0);
        }
    }
}
