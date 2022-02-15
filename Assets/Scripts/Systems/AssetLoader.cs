using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Management
{
    public static class AssetLoader
    {
        public enum LoadStatus
        { 
            Unloaded,
            Loading,
            Loaded
        }

        private static readonly Dictionary<Type, Dictionary<string, object>> _assets = new Dictionary<Type, Dictionary<string, object>>();

        public static event Action OnAssetsLoaded;
        public static LoadStatus AssetsStatus { private get; set; } = LoadStatus.Unloaded;

        private static void CheckLoadStatus()
        {
            switch (AssetsStatus)
            {
                case LoadStatus.Unloaded:
                    throw new InvalidOperationException("Unable to access assets - AssetLoader has not been loaded.");
                case LoadStatus.Loading:
                    throw new InvalidOperationException("Unable to access assets - AssetLoader is currently loading.");
            }
        }

        /// <summary>
        /// Retrieve a single asset from the collection of loaded assets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAsset<T>(string key) where T : class
        {
            CheckLoadStatus();
            if (_assets.TryGetValue(typeof(T), out var assetGroup))
            {
                if (assetGroup.TryGetValue(key, out var asset))
                {
                    return asset as T;
                }
                throw new ArgumentException($"Asset type '{typeof(T)}' was found but key '{key}' was invalid.");
            }
            throw new System.ArgumentException($"Asset type '{typeof(T)}' was not found in loaded assets.");
        }

        /// <summary>
        /// Retrieve the raw dictionary for a single type of asset.
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetAssetGroup(Type assetType)
        {
            CheckLoadStatus();
            if (_assets.TryGetValue(assetType, out var result))
            {
                return result;
            }

            throw new System.ArgumentException($"Asset type '{assetType}' was not found in loaded assets.");
        }

        /// <summary>
        /// Begin loading assets. Invokes OnAssetsLoaded when finished.
        /// </summary>
        /// <returns></returns>
        public static async Task LoadAssets()
        {
            AssetsStatus = LoadStatus.Loading;
            var assetLocations = Addressables.LoadResourceLocationsAsync("default");

            await assetLocations.Task;

            List<Task> handles = new List<Task>();

            LoadAssetsFromLocations(ref handles, assetLocations.Result);

            await Task.WhenAll(handles);

            AssetsStatus = LoadStatus.Loaded;
            OnAssetsLoaded?.Invoke();
        }
        
        /// <summary>
        /// Load assets into the broadly typed Assets collection.
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="locations"></param>
        private static void LoadAssetsFromLocations(ref List<Task> handles, IList<IResourceLocation> locations)
        {
            if (handles == null)
            {
                handles = new List<Task>();
            }

            foreach (IResourceLocation location in locations)
            {
                bool typeExists = _assets.TryGetValue(location.ResourceType, out Dictionary<string, object> typeDict);
                if (!typeExists)
                {
                    typeDict = new Dictionary<string, object>();
                    _assets.Add(location.ResourceType, typeDict);
                }

                var handle = Addressables.LoadAssetAsync<object>(location);
                handle.Completed += objHandle => typeDict.Add(location.PrimaryKey, objHandle.Result);
                handles.Add(handle.Task);
            }
        }
    } 
}
