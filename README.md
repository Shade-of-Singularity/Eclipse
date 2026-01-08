# Eclipse
Is a Service-based moddable Unity Foundation Library for any game or application.
It should be used as a core for your application to support proper runtime modding.

It provides a way for you or modders to modify initialization orders of different systems to made modding easier.
Or depend on a specific order to initialize its own systems in the right moment.
Provides optional multi-threaded initialization system for thread-safe functions.
(Note #1: Since January 6th, 2026, Runtime reloading was removed. Read below for the reasons)

Engine is being developed for a specific project.
Expect this system to get even more generalized once we start preparing Eclipse for our next projects.

## Supported builds:
- Unity v6.0 (LTS)
- Unity v2023.X (Not tested, but should work)
Everything else (down to Unity v2021 LTS) might be supported as well, but if not - will get supported later (hit me up if you need me to speed-up).

## LTS Support
Eclipse was initially built for the v6.0 and is being developed as such.
However, there are plans on LTS supporting everything down to a .NET Standard 2.1, once we will start preparing Eclipse for our next projects.

Shouldn't be that hard (clueless)
(We have some experience with that already)

## Structure
Main projects (Eclipse and Eclipse.Editor) are built for the **lowest version of .NET**.
As of right now, it is **.NET Standard 2.1**.
Projects for other versions will simply reference code from those projects to generate updated version.
(This solves problems between .NET Framework and any other .NET version, if encountered)

# Breaking changes
Lists all major changes which happened during Eclipse development.
Will re restructurized after full engine release.

## Runtime reloading
Until January 6th, 2026, you were able to unload the entire engine from the memory.
After this date, however, this functionality only partially supported.
C# doesn't allow unloading Assemblies after they were loaded in.
This means that we cannot reset the state of mods reliably, or unload unused Assemblies from memory.
This might lead to unexpected behaviour, as we cannot control how mods are written (nor should we).
As such, runtime reloading is deemed unreliable. You can only postpone engine initialization, for example: to show warnings about mods being present, etc.
As such, `Mod.Unloading()` and `Mod.Unloaded()` methods should only be used to store data before Application quits.

From other reasons - C# aren't built with manual memory management in mind.
Runtime reloads will 100% require it though, as we need to control when memory gets freed.
`GC.Collect()` is not a reliable tool for that, since some developers (myself included), for maximum productivity, use static constructors, and as such Engine should provide callback systems, which should keep all references to all callbacks even after engine reload. This means that some methods might reference old services, preventing them from benig unloaded. At the same time, having such events is mandatory, to allow sugar-coding and pleasant mod development experience, with little to no guidance.

Very sad to see this feature go, but it will help mod maintainers in a long run.
Our next engine will have runtime reloading. It is needed for games, where you can have multiple different sub-games with entirely different functionality within one game.
Providing modding support to such games will require us to create instancable Engine (meaning: multiple engines can be instanced at once), and will also demand having a way to reload mods entirely.
There are multiple ways how it can be achieved, but since we only have theoretical solutions at the moment - Eclipse will use none of them.