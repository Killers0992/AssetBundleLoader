using Exiled.API.Features;
using System;
using System.IO;

namespace AssetBundleLoader
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Name { get; } = "AssetBundleLoader";
        public override string Author { get; } = "Killers0992";
        public override string Prefix { get; } = "assetbundleloader";

        public override Version RequiredExiledVersion { get; } = new Version(4, 0, 0);
        public override Version Version { get; } = new Version(1, 0, 1);

        public static string PluginPath;

        public override void OnEnabled()
        {
            PluginPath = Path.Combine(Paths.Plugins, "AssetBundleLoader");

            if (!Directory.Exists(PluginPath))
                Directory.CreateDirectory(PluginPath);

            if (!Directory.Exists(Path.Combine(PluginPath, "bundles")))
                Directory.CreateDirectory(Path.Combine(PluginPath, "bundles"));

            AssetBundleManager.Init();
            base.OnEnabled();
        }
    }
}
