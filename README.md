# Eclipse
Unity Service-based moddable engine for any game or application.
Engine is being developed for a specific project. Expect this system to get even more generalized once we start preparing Eclipse for our next projects.

## Supported builds:
- Unity v6.0 (LTS)
Everything else (down to Unity v2021 LTS) will eventually get its own support as well.

## LTS Support
Eclipse was initially built for the v6.0 and is being developed as such.
However, there are plans on LTS supporting everything down to a .NET Standard 2.0, once we will start preparing Eclipse for our next projects.

Shouldn't be that hard (clueless)
(We have some experience with that already)

## Structure
Main projects (Eclipse and Eclipse.Editor) are built for the **lowest version of .NET**.
As of right now, it is **.NET 6.0**.
Projects for other versions will simply reference code from those projects to generate updated version.
(This solves problems between .NET Framework and .NET X.X, if encountered)