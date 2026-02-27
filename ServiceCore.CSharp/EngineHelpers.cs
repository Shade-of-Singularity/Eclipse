using Cysharp.Threading.Tasks;

namespace ServiceCore.CSharp
{
    /// <summary>
    /// Helper methods for pure <see cref="CSharp"/> implementation of <see cref="ServiceCore"/>.
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
        /// <param name="context"><inheritdoc cref="Engine.Initialize(InitializationContext?, IInitializationArgs?)"/></param>
        /// <param name="args"><inheritdoc cref="Engine.Initialize(InitializationContext?, IInitializationArgs?)"/></param>
        public static void InitializeBlocking(InitializationContext? context = default, IInitializationArgs? args = default)
        {
            Engine.Initialize(context, args).AsValueTask().GetAwaiter().GetResult();
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
            Engine.Terminate(args).AsValueTask().GetAwaiter().GetResult();
        }
    }
}
