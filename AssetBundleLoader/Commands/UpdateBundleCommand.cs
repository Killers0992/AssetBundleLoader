using CommandSystem;
using PluginAPI.Core;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace AssetBundleLoader.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpdateBundleCommand  : ICommand
    {
        public string Command => "updatebundle";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Update bundle.";

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
                response = "Syntax: updatebundle <uniqueBundleName> <url>";
                return false;
            }

            var bundleName = arguments.At(0);
            var url = arguments.At(1);

            if (AssetBundleManager.AssetBundles.TryGetValue(bundleName, out AssetBundleInfo bundle))
            {
                using(var web = new WebClient())
                {
                    //web.DownloadFile(url, Path.Combine(Paths.Exiled, "tempbundle"));
                    //File.Copy(Path.Combine(Paths.Exiled, "tempbundle"), bundle.FileName, true);
                }
                response = $"Updated bundle \"{bundleName}\" with \"{url}\".";
                return true;
            }
            response = $"Assetbundle with unique name \"{bundleName}\" is not loaded! \n Possible bundles: \n" + String.Join("\n - ", AssetBundleManager.AssetBundles.Keys.ToArray());
            return false;
        }
    }
}
