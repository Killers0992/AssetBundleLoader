using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace AssetBundleLoader
{
    public class MainClass
    {
        public static MainClass Plugin { get; private set; }
        public static PluginHandler Handler { get; private set; }
        
        public static AssetBundleManager Manager { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("AssetBundleLoader", "1.0.0", "Plugin for loading asset bundles.", "Killers0992")]
        public void Entry()
        {
            Plugin = this;
            Handler = PluginHandler.Get(Plugin);

            Manager = new AssetBundleManager();
            Manager.Init();
        }
    }
}
