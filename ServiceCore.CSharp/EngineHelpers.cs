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

using Cysharp.Threading.Tasks;

namespace ServiceCore
{
    /// <summary>
    /// Helper methods for pure C# implementation of <see cref="ServiceCore"/>.
    /// </summary>
    public static class EngineHelpers
    {
        /// <summary>
        /// Initializes <see cref="Engine"/> while blocking main/calling thread.
        /// Not provided in Unity because it might deadlock more easily there.
        /// </summary>
        /// <remarks>
        /// In WPF (or just plain C#) it's recommended to use this method directly in Program.Main(args) method as to avoid deadlocks.
        /// (e.g. before SynchronizationContext activates)
        /// But you are free to call it whenever if you 100% sure there will be no problems with deadlocks and synchronization.
        /// </remarks>
        /// <param name="context"><inheritdoc cref="Engine.Initialize(InitializationContext, IInitializationArgs?)"/></param>
        /// <param name="args"><inheritdoc cref="Engine.Initialize(InitializationContext, IInitializationArgs?)"/></param>
        public static void InitializeBlocking(InitializationContext context = default, IInitializationArgs? args = default)
        {
            Engine.Initialize(context, args).AsTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Terminates <see cref="Engine"/> while blocking main/calling thread.
        /// Not provided in Unity because it might deadlock more easily there.
        /// </summary>
        /// <remarks>
        /// In WPF (or just plain C#), be cautious about when you use this method, as to avoid deadlocks.
        /// No piece of advice here - just good luck in finding a good place for this method call.
        /// </remarks>
        /// <param name="args"><inheritdoc cref="Engine.Terminate(ITerminationArgs?)"/></param>
        public static void TerminateBlocking(ITerminationArgs? args = default)
        {
            Engine.Terminate(args).AsTask().GetAwaiter().GetResult();
        }
    }
}
