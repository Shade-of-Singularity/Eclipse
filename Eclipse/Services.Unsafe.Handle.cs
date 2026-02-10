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

namespace Eclipse
{
    public static partial class Services
    {
        public static partial class Unsafe
        {
            /// <summary>
            /// Used for determining when to fire <see cref="OnServicesInitialized"/> and similar events.
            /// </summary>
            public sealed class Handle : IDisposable
            {
                /// <summary>
                /// Callback to fire on disposal.
                /// </summary>
                private readonly Action callback;
                /// <summary>
                /// Amount of users using this handle.
                /// </summary>
                private uint users;

                /// <summary>
                /// Default constructor.
                /// </summary>
                public Handle(Action callback) => this.callback = callback;

                /// <summary>
                /// Activates this handle and registers another user.
                /// </summary>
                public Handle Activate()
                {
                    users++;
                    return this;
                }

                /// <summary>
                /// Fires internal callback if all users disposed this handle.
                /// </summary>
                public void Dispose()
                {
                    if (users == 0)
                    {
                        callback();
                    }
                    else users--;
                }
            }
        }
    }
}
