# ULoader

A powerful, flexible Unity Addressables loader and management utility.

## Namespace

```
using THEBADDEST.Assets;
```

## Features

- **Easy Asset Loading**: Async and sync APIs for loading single or multiple assets by path or label.
- **Automatic Cleanup/Unload**: Prevent memory leaks with tracked handles and safe unload.
- **Preloading Support**: Preload assets for instant use at runtime.
- **Runtime Caching Policy**: Control asset memory with `None`, `AutoRelease`, or `KeepInMemory` policies.
- **Editor Integration**: Configure resource folders and grouping rules via the ULoader Config Editor.
- **Flexible Grouping**: Group assets by type, subfolder, or custom label for Addressables.

## Setup

1. Place the `ULoader` package in your Unity project's `Assets` folder.
2. Open Unity and go to `ULoader > Config Editor` to configure resource folders and grouping rules.
3. Use the provided runtime API in your scripts.

## Basic Usage

### Loading Assets
```csharp
// Async load
var myPrefab = await ULoader.Load<GameObject>("Prefabs/MyPrefab");

// Sync load (not recommended on main thread)
var myPrefab = ULoader.LoadSync<GameObject>("Prefabs/MyPrefab");
```

### Preloading
```csharp
await ULoader.Preload<GameObject>("Prefabs/MyUIPanel");
```

### Automatic Cleanup/Unload
```csharp
// Load and cache
var knight = await ULoader.LoadWithHandle<GameObject>("Characters/Knight");
// Unload when done
ULoader.Unload("Characters/Knight");
```

### Runtime Caching Policy
```csharp
ULoader.SetCachePolicy("Characters/Knight", ULoader.CachePolicy.KeepInMemory);
```

## Editor Grouping
- Open `ULoader > Config Editor` to set up resource folders and grouping rules.
- Supports grouping by file type, subfolder, or custom label.

## Example: Grouping Rule
- ByType: All `.png` files go to a `png` group.
- BySubfolder: All assets in `Characters/Heroes` go to a `Heroes` group.

## API Reference
- `ULoader.Load<T>(string path)`
- `ULoader.LoadWithHandle<T>(string path)`
- `ULoader.Unload(string path)`
- `ULoader.SetCachePolicy(string path, ULoader.CachePolicy policy)`
- `ULoader.Preload<T>(string path)`
- `ULoader.LoadAll<T>(string folderPath)`
- `ULoader.LoadByLabel<T>(string label)`

## License
MIT