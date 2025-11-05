# Eclipse
Is a Service-based moddable Unity Foundation Library for any game or application.
It should be used as a core for your application to support proper runtime modding.

It provides a way for you or modders to modify initialization orders of different systems to made modding easier.
Or depend on a specific order to initialize its own systems in the right moment.
Provides optional multi-threaded initialization system for thread-safe functions.

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
