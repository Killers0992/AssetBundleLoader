using Exiled.API.Features;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleLoader
{
    public class AssetBundleInfo
    {
        public AssetBundle AssetBundle { get; set; }

        public Dictionary<string, AssetBundlePrefab> CreatedPrefabs { get; set; } = new Dictionary<string, AssetBundlePrefab>();

        public string BundleName;
        public string FileName;

        public void Init()
        {
            AssetBundleManager.AssetBundles.Add(BundleName, this);
        }

        public AssetBundlePrefab LoadPrefab(string prefabName, string name, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (CreatedPrefabs.TryGetValue(name, out AssetBundlePrefab prefab))
            {
                prefab.Unload();
                CreatedPrefabs.Remove(name);
            }

            prefab = new AssetBundlePrefab()
            {
                PrefabName = prefabName,
                Position = position,
                Rotation = rotation,
                Scale = scale,
            };

            if (!prefab.Load(AssetBundle))
                return null;

            CreatedPrefabs.Add(name, prefab);
            return prefab;
        }

        public void Reload()
        {
            foreach(var prefab in CreatedPrefabs)
            {
                prefab.Value.Cache();
                prefab.Value.Unload();
            }

            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
                AssetBundle = null;
            }

            var bundle = AssetBundle.LoadFromFile(FileName);
            if (bundle == null)
            {
                Log.Error($"Assetbundle for {BundleName} is invalid! ( most likely bundle is builded with wrong unity version )");
                return;
            }

            AssetBundle = bundle;

            foreach (var prefab in CreatedPrefabs)
            {
                prefab.Value.Load(AssetBundle);
            }
        }

        public void Unload()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
                AssetBundle = null;
            }

            foreach (var prefab in CreatedPrefabs)
            {
                prefab.Value.Unload();
            }

            CreatedPrefabs.Clear();

            if (AssetBundleManager.AssetBundles.ContainsKey(BundleName))
                AssetBundleManager.AssetBundles.Remove(BundleName);
        }
    }
}
