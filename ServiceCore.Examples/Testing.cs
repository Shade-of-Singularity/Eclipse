using System.Runtime.CompilerServices;

namespace ServiceCore.Examples;

internal static class Testing
{
    public static void Start()
    {
        RuntimeHelpers.RunClassConstructor(typeof(IService<AlphaService>).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(IService<BetaService>).TypeHandle);
        Console.WriteLine($"{nameof(AlphaService)} exist? ({IService<AlphaService>.Instance is not null})");
        Console.WriteLine($"{nameof(BetaService)} exist? ({IService<BetaService>.Instance is not null})");
        Console.WriteLine();

        Services.Instantiate<AlphaService>();
        Console.WriteLine($"{nameof(AlphaService)} exist? ({IService<AlphaService>.Instance is not null})");
        Console.WriteLine($"{nameof(BetaService)} exist? ({IService<BetaService>.Instance is not null})");
        Console.WriteLine();

        Services.Instantiate<BetaService>();
        Console.WriteLine($"{nameof(AlphaService)} exist? ({IService<AlphaService>.Instance is not null})");
        Console.WriteLine($"{nameof(BetaService)} exist? ({IService<BetaService>.Instance is not null})");
        Console.WriteLine();

        Services.Destroy<AlphaService>();
        Console.WriteLine($"{nameof(AlphaService)} exist? ({IService<AlphaService>.Instance is not null})");
        Console.WriteLine($"{nameof(BetaService)} exist? ({IService<BetaService>.Instance is not null})");
        Console.WriteLine();

        Services.Destroy<BetaService>();
        Console.WriteLine($"{nameof(AlphaService)} exist? ({IService<AlphaService>.Instance is not null})");
        Console.WriteLine($"{nameof(BetaService)} exist? ({IService<BetaService>.Instance is not null})");
        Console.WriteLine();
    }

    public sealed class AlphaService : IService<AlphaService> { }
    public sealed class BetaService : IService<BetaService> { }
    public interface IService { }
    public interface IService<T> : IService where T : IService<T>
    {
        public static T? Instance { get; private set; }
        static IService()
        {
            Services.Register<T>(
                getter: static () => Instance,
                setter: static (instance) => Instance = (T?)instance);
        }
    }

    public static class Services
    {
        public readonly record struct Descriptor(Getter Getter, Setter Setter) { }
        public delegate IService? Getter();
        public delegate void Setter(IService? service);
        static readonly Dictionary<Type, Descriptor> consumers = [];
        public static void Register<T>(Getter getter, Setter setter) where T : IService
        {
            consumers[typeof(T)] = new(getter, setter);
        }

        public static T? Retrieve<T>() where T : IService
        {
            return (T?)consumers[typeof(T)].Getter();
        }

        public static void Instantiate<T>() where T : IService, new()
        {
            consumers[typeof(T)].Setter(new T());
        }

        public static void Destroy<T>() where T : IService
        {
            consumers.Remove(typeof(T), out Descriptor descriptor);
            descriptor.Setter(null);
        }
    }
}
