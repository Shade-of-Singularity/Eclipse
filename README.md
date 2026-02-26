# About ServiceCore
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
Eclipse IService.Instance       | 0.0103          | 0.0117           | O(1)      
Naninovel Engine.GetService<>() | 0.0345          | 0.0345           | O(1)      
RimWorld Game.GetComponent<>()  | 0.0167          | 0.4001           | O(n) n:7  
Native GetField                 | 0.0101          | 0.0104           | O(1)      
Idle (Control)                  | 0.0003          | 0.0003           | O(1)      
```
Benchmark project: https://github.com/Shade-of-Singularity/EclipseBenchmark

## Usage Notes
You don't need to cache services with ServiceCore.
As you can see from a benchmark above - accessing services directly via `IService<T>.Instance` is already as fast as it can get.
It completely avoid null checks for re-initialization, allowing for the highest performance.

Additionally, it allows us to better GC manage the services,
and allows swaping a service reference at runtime at your will, so we actually discourage you from caching anything.
(Engine itself doesn't ever swap the instances though - it's in case users do it themselves)

E.g. **Don't ever** do the following:
```C#
private IMyService myService;

public void Initialize()
{
    myService = IMyService.Instance;
}

public void Update()
{
    myService.MyMethod();
{
```
**Do this instead**:
```C#
public void Update()
{
    IMyService.Instance.MyMethod();
{
```


## Supported Unity versions:
- Unity v6.0 (LTS)
- Unity v2022.X (LTS)
Everything else (down to Unity v2021 LTS) might be supported as well, but if not - will get supported later (hit me up if you need me to speed-up).

# Usage Examples
## Common
Provided by `ServiceCore.dll`
### (moddable) Service declaration:
```C#
using ServiceCore;

public interface ILocalizationService : IService<ILocalizationService>
{
	/// <summary> Retrieves text for given key under current locale. </summary>
	string Localize(string key);
}

[Service] // Won't initialize without attribute.
public sealed class LocalizationService : ILocalizationService
{
	public UniTask Initialize() => UniTask.CompletedTask;
	public UniTask Terminate() => UniTask.CompletedTask;
	
	public string Localize(string key) => $"Key ({key}) not found.";	
}

// Usage:
// Once (at Startup):
await Engine.Initialize();

// Later:
string value = ILocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");
```
*Note: If other mod/assembly defines their own service implementing `ILocalizationService` - it will replace previous service.*
### (non-moddable) Service declaration:
```C#
using ServiceCore;

[Service] // Won't initialize without attribute.
public sealed class LocalizationService : Service<LocalizationService>
{
	public override UniTask Initialize() => UniTask.CompletedTask;
	public override UniTask Terminate() => UniTask.CompletedTask;
	
	public string Localize(string key) => $"Key ({key}) not found.";
}

// Usage:
// Once (at Startup)
await Engine.Initialize();

// Later:
string value = LocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");
```
### (moddable) Service declaration with custom base class:
```C#
using ServiceCore;

public interface ILocalizationService : IService<ILocalizationService>
{
	/// <summary> Retrieves text for given key under current locale. </summary>
	string Localize(string key);
}

[Service]
public partial sealed class LocalizationService : CustomClass, ILocalizationService
{
	public UniTask Initialize() => UniTask.CompletedTask;
	public UniTask Terminate() => UniTask.CompletedTask;
	
	public string Localize(string key) => $"Key ({key}) not found.";
}

// Usage:
// Once (at Startup)
await Engine.Initialize();

// Later:
string value = ILocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");
```
### (non-moddable) Service declaration with custom base class:
```C#
using ServiceCore;

[Service]
public partial sealed class LocalizationService : CustomClass, IService
{
	// Instance field is created in partial class via CodeGen.
	public UniTask Initialize() => UniTask.CompletedTask;
	public UniTask Terminate() => UniTask.CompletedTask;
	
	public string Localize(string key) => $"Key ({key}) not found.";
}

// Usage:
// Once (at Startup)
await Engine.Initialize();

// Later:
string value = LocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");
```
## Unity Exclusive
Provided by `ServerCore.UnityEngine.dll`
### (non-moddable) Service declaration:
```C#
using ServiceCore;

// Uses CodeGen to avoid actually checking for the attribute.
[MonoService] // Defaults to 'MonoServiceMode.KeepOlder'
[MonoService(keep: MonoServiceMode.KeepNewer)]
public partial sealed class LocalizationService : MonoService<LocalizationService>
{
	// Instance is initialized at Awake().
	[SerializeField] string Format = "Key ({key}) not found.";
	
	public override UniTask Initialize() => UniTask.CompletedTask;
	public override UniTask Terminate() => UniTask.CompletedTask;
	
	public string Localize(string key) => Format.Replace("{key}", key);
}

// Usage:
// Attach service to a GameObject.

// Later:
string value = LocalizationService.Instance.Localize("test");
Console.WriteLine($"Result: {value}");
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
