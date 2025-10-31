1. Include .asmdef files in the project to allow building it directly in UnityEditor.
2. Move all code wrapped in UNITY_EDITOR to Eclipse.Editor.
3. Remove all UI code (if present.)
4. Include multi-targeting by default:

Example (from GPT):
<PropertyGroup Condition="'$(Configuration)'=='UnityEditor'">
  <DefineConstants>UNITY_EDITOR;UNITY_STANDALONE</DefineConstants>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)'=='Runtime'">
  <DefineConstants>UNITY_STANDALONE</DefineConstants>
</PropertyGroup>

Then you can build:
```batch
dotnet build -c UnityEditor
dotnet build -c Runtime
```

5. Regenerate all .meta files for all files by compiling and testing everything via Unity directly.