﻿using CommandSystem;
using PluginAPI.Core;
using System;
using System.Linq;
using UnityEngine;

namespace AssetBundleLoader.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class LoadPrefabCommand : ICommand
    {
        public string Command => "loadprefab";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Load prefab.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            /*if (!player.CheckPermission("loadbundles"))
            {
                response = "Missing permission";
                return false;
            }*/

            if (arguments.Count < 2)
            {
                response = "Syntax: loadprefab <uniqueBundleName> <prefabName> <uniquePrefabName>";
                return false;
            }

            var bundleName = arguments.At(0);
            var prefabName = arguments.At(1);
            var uniqueName = arguments.At(2);

            if (AssetBundleManager.AssetBundles.TryGetValue(bundleName, out AssetBundleInfo bundle))
            {
                var prefab = bundle.LoadPrefab(prefabName, uniqueName, player.Position, Vector3.zero, Vector3.one);
                if (prefab == null)
                {
                    response = $"Prefab \"{prefabName}\" not exists in bundle. \n Assets: \n" + string.Join("\n - ", bundle.AssetBundle.GetAllAssetNames());
                    return false;
                }
                response = $"Loaded prefab \"{prefabName}\" with unique name \"{uniqueName}\".";
                return true;
            }

            response = $"Assetbundle with unique name \"{uniqueName}\" is not loaded! \n Possible bundles: \n" + String.Join("\n - ", AssetBundleManager.AssetBundles.Keys.ToArray());
            return false;
        }
    }
}
