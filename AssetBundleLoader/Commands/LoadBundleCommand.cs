using CommandSystem;
using PluginAPI.Core;
using System;
using System.IO;

namespace AssetBundleLoader.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class LoadBundleCommand : ICommand
    {
        public string Command => "loadbundle";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Load bundle.";

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
                response = "Syntax: loadbundle <bundleName> <uniqueName>";
                return false;
            }

            var bundleName = arguments.At(0);
            var uniqueName = arguments.At(1);

            var file = "";// Path.Combine(MainClass.PluginPath, "bundles", bundleName);

            var bundle = AssetBundleManager.LoadBundle(file, uniqueName);
            if (bundle != null)
            {
                response = $"Loaded asset bundle \"{bundleName}\" with unique name \"{uniqueName}\". \n Assets: \n" + string.Join("\n - ", bundle.AssetBundle.GetAllAssetNames());
                return true;
            }

            response = $"Failed loading assetbundle \"{bundleName}\" ( check console ).";
            return false;
        }
    }
}
