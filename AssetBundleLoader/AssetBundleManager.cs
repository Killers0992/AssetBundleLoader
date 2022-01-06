using AssetBundleLoader.Patches;
using Exiled.API.Features;
using MapGeneration;
using MEC;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleLoader
{
    public class AssetBundleManager
    {
        public static Dictionary<string, AssetBundleInfo> AssetBundles = new Dictionary<string, AssetBundleInfo>();

        public static FileSystemWatcher Watcher;

        public static ConcurrentQueue<string> BundlesToReload = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> BundlesToUnload = new ConcurrentQueue<string>();

        public static CoroutineHandle handler;

        public static GameObject LCZDoor, HCZDoor, EZDoor, SportShootingTarget, DboyShootingTarget, BinaryShootingTarget;

        public static void Init()
        {
            handler = Timing.RunCoroutine(WatchChanges());
            Watcher = new FileSystemWatcher(Path.Combine(MainClass.PluginPath, "bundles"));
            Watcher.EnableRaisingEvents = true;
            Watcher.Deleted += Watcher_Deleted;
            Watcher.Changed += Watcher_Changed;
            Exiled.Events.Handlers.Map.Generated += Map_Generated;
        }

        private static void Map_Generated()
        {
            SyncMovementPatch.Cache.Clear();

            DoorSpawnpoint[] doorList = UnityEngine.Object.FindObjectsOfType<DoorSpawnpoint>();
            LCZDoor = doorList.First(x => x.TargetPrefab.name.ToUpper().Contains("LCZ BREAKABLEDOOR")).TargetPrefab.gameObject;
            HCZDoor = doorList.First(x => x.TargetPrefab.name.ToUpper().Contains("HCZ BREAKABLEDOOR")).TargetPrefab.gameObject;
            EZDoor = doorList.First(x => x.TargetPrefab.name.ToUpper().Contains("EZ BREAKABLEDOOR")).TargetPrefab.gameObject;
            SportShootingTarget = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "sportTargetPrefab");
            DboyShootingTarget = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "dboyTargetPrefab");
            BinaryShootingTarget = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "binaryTargetPrefab");
        }

        public static IEnumerator<float> WatchChanges()
        {
            while (true)
            {
                try
                {
                    if (BundlesToReload.TryDequeue(out string path))
                        foreach (var assetbundle in AssetBundles.Where(p => p.Value.FileName == path))
                            assetbundle.Value.Reload();

                    if (BundlesToUnload.TryDequeue(out string path2))
                        foreach (var assetbundle in AssetBundles.Where(p => p.Value.FileName == path2))
                            assetbundle.Value.Unload();
                }
                catch (Exception) { }

                yield return Timing.WaitForSeconds(5f);
            }
        }

        public static AssetBundleInfo LoadBundle(string file, string name)
        {
            if (!File.Exists(file))
            {
                Log.Info($"Assetbundle \"{name}\" not exists...");
                return null;
            }

            if (AssetBundles.TryGetValue(name, out AssetBundleInfo assetBundleInfo))
            {
                assetBundleInfo.Unload();
            }

            var bundle = AssetBundle.LoadFromFile(file);
            if (bundle == null)
            {
                Log.Info($"Assetbundle for \"{name}\" is invalid! ( most likely bundle is builded with wrong unity version, use 2019.4 )");
                return null;
            }

            var bundleInfo = new AssetBundleInfo()
            {
                AssetBundle = bundle,
                BundleName = name,
                FileName = file,
            };

            bundleInfo.Init();
            return bundleInfo;
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!BundlesToReload.Contains(e.FullPath))
                BundlesToReload.Enqueue(e.FullPath);
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!BundlesToUnload.Contains(e.FullPath))
                BundlesToUnload.Enqueue(e.FullPath);
        }
    }
}
