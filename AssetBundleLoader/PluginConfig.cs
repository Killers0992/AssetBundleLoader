using Exiled.API.Interfaces;

namespace AssetBundleLoader
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public int RenderDistance { get; set; } = 40;
    }
}
