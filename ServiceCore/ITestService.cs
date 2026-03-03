using Cysharp.Threading.Tasks;
using ServiceCore.Localization;
using ServiceCore.Serialization;

namespace ServiceCore
{
    internal interface IAdditionalService : IService<IAdditionalService> { }
    internal interface ITestService : ILocalizationService, ISerializationService { }
    internal sealed class TestMultiService : Service<TestMultiService>, ITestService, IAdditionalService
    {
        protected override UniTask Initialize(IInitializationArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override UniTask Terminate(ITerminationArgs args)
        {
            throw new System.NotImplementedException();
        }

        UniTask IService<ILocalizationService>.Initialize(IInitializationArgs args) => Initialize(args);
        UniTask IService<ISerializationService>.Initialize(IInitializationArgs args) => Initialize(args);
        UniTask IService<IAdditionalService>.Initialize(IInitializationArgs args) => Initialize(args);
        UniTask IService<ILocalizationService>.Terminate(ITerminationArgs args) => Terminate(args);
        UniTask IService<ISerializationService>.Terminate(ITerminationArgs args) => Terminate(args);
        UniTask IService<IAdditionalService>.Terminate(ITerminationArgs args) => Terminate(args);
    }
}
