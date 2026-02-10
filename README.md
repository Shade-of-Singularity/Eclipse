# Eclipse
Is a Service-based Unity Foundation Library with community-modding support. Uses UniTask for async initialization. Release for non-Unity environments is planned for later.

Eclipse enforces service access via static declarations in `ICustomService : IService<ICustomService>`, but in return provides exceptionally high performance.
It should be used as a core for your application to support proper runtime modding.

## Benchmarks
```C#
Benchmark                       | Best (Avr.)(μs) | Worst (Avr.)(μs) | Complexity
------------------------------- | --------------- | ---------------- | ----------
Eclipse IService.Instance       | 0.0103          | 0.0117           | O(1)      
Naninovel Engine.GetService<>() | 0.0345          | 0.0345           | O(1)      
RimWorld Game.GetComponent<>()  | 0.0167          | 0.4001           | O(n) n:7  
Native GetField                 | 0.0101          | 0.0104           | O(1)      
Idle (Control)                  | 0.0003          | 0.0003           | O(1)      
```

Provides optional multi-threaded initialization system for thread-safe functions.
Also provides a way for you or modders to modify initialization orders of different systems to made modding easier.

Eclipse is still WIP - expect this system to get even more generalized once we start preparing Eclipse for our next projects.
We are open to suggestions and lib trials!

## Usage Example
```C#
using Eclipse;

public static void Main(string[] args)
{
    // Initializer called automatically unless you change settings:
    // Engine.Initialize();
    IGameService.Instance.LoadFirstLevel();
}

// Service implementation.
[Service]
public sealed class GameService : IGameService
{
    /// <inheritdoc/>
    public void LoadFirstLevel() => SceneManagement.LoadLevel(1);

    /// <inheritdoc/>
    public virtual UniTask Initialize() => UniTask.CompletedTask;

    /// <inheritdoc/>
    public virtual UniTask Terminate() => UniTask.CompletedTask;
}

// Recommended way to declare services:
public interface IGameService : IService<IGameService>
{
    /// <summary>Loads first level of the game.</summary>
    void LoadFirstLevel();
}

```
*Note: If other mod/assembly defines their own service implementing `IGameService` - it will replace previous service.*

## Supported Unity versions:
- Unity v6.0 (LTS)
- Unity v2022.X (LTS)
Everything else (down to Unity v2021 LTS) might be supported as well, but if not - will get supported later (hit me up if you need me to speed-up).
