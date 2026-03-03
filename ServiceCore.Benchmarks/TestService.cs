using Cysharp.Threading.Tasks;

namespace ServiceCore.Benchmarks
{
    public sealed class TestService : Service<TestService>
    {
        public int Value = 42;
        protected override UniTask Initialize(IInitializationArgs args) => UniTask.CompletedTask;
        protected override UniTask Terminate(ITerminationArgs args) => UniTask.CompletedTask;
    }
}