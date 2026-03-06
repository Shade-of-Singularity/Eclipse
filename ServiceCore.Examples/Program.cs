using Cysharp.Threading.Tasks;
using ServiceCore.Localization;
using ServiceCore.Serialization;

namespace ServiceCore.Examples
{
    internal class Program
    {
        private sealed class SealedService : Service<SealedService>
        {
            protected override UniTask Initialize(IInitializationArgs args) => UniTask.CompletedTask;
            protected override UniTask Terminate(ITerminationArgs args) => UniTask.CompletedTask;
        }

        static async Task<int> Main(string[] args)
        {
            Testing.Start();
            await ManualInitialization();
            await AutomaticInitialization();
            CSharpExclusiveBlockingInitialization();
            return 0;
        }

        static async UniTask ManualInitialization()
        {
            ServiceCoreLogger.Log($"Starting {nameof(ManualInitialization)}.");

            // Manual initialization.
            await ISerializationService.Instantiate<DefaultSerializationService>();
            await ILocalizationService.Instantiate<DefaultLocalizationService>();
            await SealedService.Instantiate(); // Alternative with AService<T> services.

            // Manual termination.
            await ISerializationService.Destroy();
            await ILocalizationService.Destroy();
            await SealedService.Destroy();

            ServiceCoreLogger.Log($"Finished {nameof(ManualInitialization)}.");
        }

        static async UniTask AutomaticInitialization()
        {
            ServiceCoreLogger.Log($"Starting {nameof(AutomaticInitialization)}.");

            await Engine.Initialize();
            await Engine.Terminate();

            ServiceCoreLogger.Log($"Finished {nameof(AutomaticInitialization)}.");
        }

        static void CSharpExclusiveBlockingInitialization()
        {
            ServiceCoreLogger.Log($"Starting {nameof(CSharpExclusiveBlockingInitialization)}.");

            EngineHelpers.InitializeBlocking();
            EngineHelpers.TerminateBlocking();

            ServiceCoreLogger.Log($"Finished {nameof(CSharpExclusiveBlockingInitialization)}.");
        }
    }
}
