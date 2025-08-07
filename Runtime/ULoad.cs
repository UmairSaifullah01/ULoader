using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public static class ULoad
{
    public static async Task<T> Load<T>(string path) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;

        Debug.LogError($"ULoad: Failed to load {path}");
        return null;
    }

    public static async Task<T> Load<T>(string path, IProgress<float> progress, CancellationToken cancellationToken) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        while (!handle.IsDone)
        {
            progress?.Report(handle.PercentComplete);
            if (cancellationToken.IsCancellationRequested)
            {
                Addressables.Release(handle);
                return null;
            }
            await Task.Yield();
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        Debug.LogError($"ULoad: Failed to load {path}");
        return null;
    }

    public static async Task<List<T>> LoadAll<T>(string folderPath) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(
            folderPath,
            null
        );
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load folder {folderPath}");

        return results;
    }

    public static async Task<List<T>> LoadAll<T>(string folderPath, IProgress<float> progress, CancellationToken cancellationToken) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(
            folderPath,
            null
        );
        while (!handle.IsDone)
        {
            progress?.Report(handle.PercentComplete);
            if (cancellationToken.IsCancellationRequested)
            {
                Addressables.Release(handle);
                return results;
            }
            await Task.Yield();
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load folder {folderPath}");
        return results;
    }

    // Load a single asset by label (returns first found)
    public static async Task<T> LoadByLabel<T>(string label) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
            return handle.Result[0];
        Debug.LogError($"ULoad: Failed to load asset with label {label}");
        return null;
    }

    public static async Task<T> LoadByLabel<T>(string label, IProgress<float> progress, CancellationToken cancellationToken) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        while (!handle.IsDone)
        {
            progress?.Report(handle.PercentComplete);
            if (cancellationToken.IsCancellationRequested)
            {
                Addressables.Release(handle);
                return null;
            }
            await Task.Yield();
        }
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
            return handle.Result[0];
        Debug.LogError($"ULoad: Failed to load asset with label {label}");
        return null;
    }

    // Load all assets by label
    public static async Task<List<T>> LoadAllByLabel<T>(string label) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load assets with label {label}");
        return results;
    }

    public static async Task<List<T>> LoadAllByLabel<T>(string label, IProgress<float> progress, CancellationToken cancellationToken) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        while (!handle.IsDone)
        {
            progress?.Report(handle.PercentComplete);
            if (cancellationToken.IsCancellationRequested)
            {
                Addressables.Release(handle);
                return results;
            }
            await Task.Yield();
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load assets with label {label}");
        return results;
    }

    // Unload a single asset
    public static void Unload<T>(T asset) where T : Object
    {
        if (asset != null)
            Addressables.Release(asset);
    }

    // Unload a list of assets
    public static void UnloadAll<T>(IEnumerable<T> assets) where T : Object
    {
        if (assets == null) return;
        foreach (var asset in assets)
        {
            if (asset != null)
                Addressables.Release(asset);
        }
    }

    // Unload using AsyncOperationHandle (advanced)
    public static void UnloadHandle<T>(AsyncOperationHandle<T> handle)
    {
        if (handle.IsValid())
            Addressables.Release(handle);
    }

    // Example for Load<T> with error handler
    public static async Task<T> Load<T>(string path, Action<string> onError) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        onError?.Invoke($"ULoad: Failed to load {path}");
        return null;
    }

    // Example for LoadAll<T> with error handler
    public static async Task<List<T>> LoadAll<T>(string folderPath, Action<string> onError) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(folderPath, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            onError?.Invoke($"ULoad: Failed to load folder {folderPath}");
        return results;
    }

    // Example for LoadByLabel<T> with error handler
    public static async Task<T> LoadByLabel<T>(string label, Action<string> onError) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
            return handle.Result[0];
        onError?.Invoke($"ULoad: Failed to load asset with label {label}");
        return null;
    }

    // Example for LoadAllByLabel<T> with error handler
    public static async Task<List<T>> LoadAllByLabel<T>(string label, Action<string> onError) where T : Object
    {
        List<T> results = new List<T>();
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            onError?.Invoke($"ULoad: Failed to load assets with label {label}");
        return results;
    }

    /// <summary>
    /// Synchronously loads an asset by path. WARNING: Do not use on the main thread in production.
    /// </summary>
    public static T LoadSync<T>(string path) where T : Object
    {
        var handle = Addressables.LoadAssetAsync<T>(path);
        handle.WaitForCompletion();
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        Debug.LogError($"ULoad: Failed to load {path} (sync)");
        return null;
    }

    /// <summary>
    /// Synchronously loads all assets by label. WARNING: Do not use on the main thread in production.
    /// </summary>
    public static List<T> LoadAllByLabelSync<T>(string label) where T : Object
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        handle.WaitForCompletion();
        var results = new List<T>();
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load assets with label {label} (sync)");
        return results;
    }

    /// <summary>
    /// Synchronously loads all assets by path/folder. WARNING: Do not use on the main thread in production.
    /// </summary>
    public static List<T> LoadAllSync<T>(string folderPath) where T : Object
    {
        var handle = Addressables.LoadAssetsAsync<T>(folderPath, null);
        handle.WaitForCompletion();
        var results = new List<T>();
        if (handle.Status == AsyncOperationStatus.Succeeded)
            results.AddRange(handle.Result);
        else
            Debug.LogError($"ULoad: Failed to load folder {folderPath} (sync)");
        return results;
    }
}
