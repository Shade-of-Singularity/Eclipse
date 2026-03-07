# About ServiceCore
*Note: It's mandatory to attribute Shade of Singularity (or simply SoG) if you use ServiceCore.*

**ServiceCore** - is a high-performance library for managing (initializing, terminating) services at runtime, based on [CRTP](https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern) pattern, with complete (optional) community modding support.

It can be used either as an utility library, or as a core for your application or game
(Especially if you want it to support modding from a get-go)

And it utilizes [UniTask](https://github.com/Cysharp/UniTask) for async initialization and termination.

Services can be initialized in a multi-threaded context as well, even when used in Unity.

## Benchmarks
Here is a benchmark for aquiring a service:
```C#
Benchmark                       | Best (Avr.)(μs) | Worst (Avr.)(μs) | Complexity
------------------------------- | --------------- | ---------------- | ----------
Idle (Control)                  | 0.0003          | 0.0003           | O(1)      
Native GetField                 | 0.0101          | 0.0104           | O(1)      
ServiceCore IService.Instance   | 0.0103          | 0.0117           | O(1)      
Naninovel Engine.GetService<>() | 0.0345          | 0.0345           | O(1)      
RimWorld Game.GetComponent<>()  | 0.0167          | 0.4001           | O(n) n:7  
```
Benchmark project: https://github.com/Shade-of-Singularity/EclipseBenchmark

## Supported Unity versions:
- C# (.NET Standard 2.1)
- Unity v6.0 (LTS)
- Unity v2022.X (LTS)
- (Should be compatible with Godot, but it is not directly supported)

Everything else (down to Unity v2021 LTS) might be supported as well, but if not - will get supported later (let us know if you need it).

# Defining and using services
You can define new service simply like this:
(From the experience - this is very useful on prototyping stage)
```C#
// Attribute asks engine to initialize the service.
// It will not be initialize *automatically* if you don't provide it.
[Service]
public sealed class ScriptService : Service<AssetService>
{
    public async UniTask Initialize(IInitializationArgs args) {}
    public async UniTask Terminate(ITerminationArgs args) {}
    public Script LoadAt(string path) => ...
}

// Usage:
Script script = ScriptService.Instance.LoadAt("Scripts/HelloWorld.sc");
```

Completely moddable/overwritable services can use interfaces.
(Interfaces require more maintenance, so you might choose to make them later - near the app/game release)
```C#
public interface IScriptService : IService<IScriptService>
{
    Script LoadAt(string path);
}

[Service]
public sealed class ScriptService : IScriptService
{
    public Script LoadAt(string path) => ...
}

// Usage
// Notice that we use interface instead of a class.
Script script = IScriptService.Instance.LoadAt(path) => ...
``` 

You can also define moddable service as abstract class (but using `IService<T>` is recommended instead)
```C#
[Service]
public class BasicWorldService : Service<BasicWorldService>
{
    public virtual void SpawnEnemy(EnemyType type, float x, float y) => ...
}

// In modification:
[Service] // Automatically overwrites the parent. Only this service will be instantiated.
public sealed class CustomWorldService : BasicWorldService
{
    public override void SpawnEnemy(EnemyType type, float x, float y) => ...
}
```

For Unity, you can use special kind of service. It doesn't support multi-threading, but you can edit its fields from the editor.
(Interfaces require more maintenance, so you might choose to make them later - near the app/game release)
```C#
[Service]
// (Optional) Specifies what to do with extra service instances.
// If not specified - uses 'KeepInstance.Older' by default.
[KeepService(KeepInstance.Newer | KeepInstance.Older)]
// inherits MonoBehaviour.
public sealed class ScriptService : MonoService<ScriptService>
{
    public Script LoadAt(string path) => ...
}

// Usage
// Notice that we use interface instead of a class.
Script script = ScriptService.Instance.LoadAt(path) => ...
``` 

If you already have a base class you need to inherit - you can define your service like this:
```C#
[Service]
public sealed class LocalizationService : CoreAPI.LicalizationService, IService<LocalizationService>
{
    ...
}

// But usage becomes a bit inconvenient:
string result = IService<LocalizationService>.Instance.Localize("...");
```

To deal with inconvenience, you can choose to define a custom parameter, but there is no optimal solution at the moment.
```C#
[Service]
public sealed class LocalizationService : CoreAPI.LicalizationService, IService<LocalizationService>
{
    public static LocalizationService Instance => IService<LocalizationService>.Instance;
}

// More convenient, but requires more maintenance:
string result = LocalizationService.Instance.Localize("...");
```

# Initialization
Engine supports 3 different initialization methods:
```C#
public interface IActorService : IService<IActorService> { }
[Service] public sealed ActorService : IActorService { }
[Service] public sealed ScriptService : Service<ScriptService> { }

// Manual initialization:
// Instantiates and initializes services at once.
await IActorService.Instantiate<ActorService>();
await ScriptService.Instantiate();
// Manual termination:
await IActorService.Destroy();
await ScriptService.Destroy();


// Automatic initialization/termination:
await Engine.Initialize();
await Engine.Terminate();


// (C# Exclusive) Blocking initialization/termination:
EngineHelpers.InitializeBlocking();
EngineHelpers.TerminateBlocking();
```

# Important!
Reference under`CustomService.Instance` and `ICustomService.Instance` property can change.
To allow community modding, you should **never-ever** cache service instances.
Do **NOT** do the following:
```C#
private readonly ScriptService m_ScriptService;
public MyClass()
{
    m_ScriptService = ScriptService.Instance;
}

public void MyMethod()
{
    m_ScriptService.LoadAt(...);
}
```
(But feel free to use injection pattern)
Recommended approaches are shown above.
**Cost of not caching**: Difference between using cached `m_Service` field and `Service.Instance` is measured to be just `0.0001μs - 0.001μs`.
To achieve even 0.1 second of lag, you will need to reference it ~100,000,000 times a second. You will never reference it this many times outside of benchmarks.
You will save a lot more mental power to not bother about it at all.

# Additional information
In some cases - especially during service initialization, you might want to check if some of them exist.
You can do it like so:
```C#
if (ICustomService.Exist())
{
    ICustomService.Instance.MyMethod(...);
}

// Or simply:
ICustomService.Instance?.MyMethod(...);
```
In Unity, ServiceCore can initialize automatically.
ServiceCore will create a configuration file in `Assets/Resources/Configuration/Imbedded`, where you can control when this happens.

`IService<T>.Instance` and all similar properties are marked as non-nullable, despite being nullable before automatic or manual initialization.
If you prefer to check to null-safely though, feel free to use `TryGet`. It will also store service reference on stack as well:
```C#
if (ICustomService.TryGet(out var instance))
{
    instance.MyMethod(...);
}
```

# Comparison
## Naninovel
In `Naninovel`, you would define and use services like that:
```C#
using Naninovel;

public interface ILocalizationService : IEngineService<ILocalizationService>
{
	/// <summary> Retrieves text for given key under current locale. </summary>
	string Localize(string key);
}

[Service]
public sealed class LocalizationService : ILocalizationService
{
	public override string Localize(string key) => $"Key ({key}) not found.";
}

// Usage:
// Once (at Startup)
await Engine.Initialize();

// In initializer/.ctor:
// Naninovel  requires caching, so it allocates 8 bytes in a containing class.
// Having services cached doesn't allow services to be overwritten at runtime.
private ILocalizationservice m_LocalizationService;

public CustomClass()
{
	// Uses ~0.45μs or more, and uses C# Dictionary underneath.
	// Requres CPU to cache entire C# Dictionary for faster access.
	m_LocalizationService = Engine.Get<ILocalizationService>();
	
	// You can check if service exist using:
	if (Engine.TryGet(out ILocalizationService service))
	{
		m_LocalizationService = service;
	}
}

// Later:
string value = m_LocalizationService.Localize("test");
Console.WriteLine($"Result: {value}");
```

In `ServiceCore`, you define and use services like this:
```C#
using ServiceCore;

public interface ILocalizationService : IService<ILocalizationService>
{
	/// <summary> Retrieves text for given key under current locale. </summary>
	string Localize(string key);
}

[Service]
public sealed class LocalizationService : ILocalizationService
{
	public UniTask Initialize() => UniTask.CompletedTask;
	public UniTask Terminate() => UniTask.CompletedTask;
	
	public override string Localize(string key) => $"Key ({key}) not found.";
}

// Usage:
// Once (at Startup)
await Engine.Initialize();

// No need to cache the reference.

// Later:
// Access Service via parameter, accessing a field directly, without null checks.
// No caching the reference allows overwriting the service at runtime.
string value = ILocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");

// You can check for existance using:
if (ILocalizationService.Exist())
{
	// It's a simplest field null check begist the scenes.
	ILocalizationService.Instance.Localize("test");
}

// Or even simpler:
string value = ILocalizationService.Instance?.Localize("test") ?? string.Empty;

// If you forgot to initialize the service or engine:
// It simply throws a NullReferenceException.
ILocalizationService.Instance.Localize("test"); // Assuming it's not initialized.
```
