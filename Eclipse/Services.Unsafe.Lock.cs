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
            public sealed class Lock : IDisposable
            {
                private readonly object _lock = new object();
                private volatile Action? m_Callback;
                internal bool TrySet(Action callback)
                {
                    lock (_lock)
                    {
                        if (m_Callback is null)
                        {
                            m_Callback = callback;
                            return true;
                        }

                        return false;
                    }
                }

                /// <summary>
                /// Used to fire target event.
                /// </summary>
                public void Dispose()
                {
                    lock (_lock)
                    {
                        m_Callback?.Invoke();
                        m_Callback = null;
                    }
                }
            }
        }
    }
}
