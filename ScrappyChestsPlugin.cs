using BepInEx;

namespace ScrappyChests
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ScrappyChestsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "Lawlzee.ScrappyChests";
        public const string PluginAuthor = "Lawlzee";
        public const string PluginName = "Scrappy Chests";
        public const string PluginVersion = "1.3.0";

        public void Awake()
        {
            Log.Init(Logger);

            Configuration.Instance = new Configuration(Config);
            Configuration.Instance.InitUI(Info);

            ChestHooks.Init();
            MobHooks.Init();
            DifficultyIconHooks.Init();
            CauldronHooks.Init();
            CostHooks.Init();
            PrinterHooks.Init();
            WaveHooks.Init();
            ArtifactHooks.Init();
        }        
    }
}
