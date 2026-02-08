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

namespace Eclipse.Extensions
{
    /// <summary>
    /// Provides extensions to call methods within try block.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Invokes given <paramref name="action"/> with default logger <see cref="EclipseLogger.LogException(Exception)"/>.
        /// </summary>
        public static void WithLog(Action? action) => Invoke(action, EclipseLogger.LogException);

        /// <summary>
        /// Invokes given <paramref name="action"/> with using <paramref name="callback"/> handler for exception handling.
        /// </summary>
        public static void Invoke(Action? action, Action<Exception>? callback)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                callback?.Invoke(ex);
            }
        }
    }
}
