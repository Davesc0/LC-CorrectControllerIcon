using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalCompanyInputUtils.Components;

namespace CorrectControllerIcon
{
    public enum ControllerIcons { Auto, Xbox, PlayStation }

    [BepInPlugin(Plugin.modGUID, Plugin.modName, Plugin.modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils")]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "dev.davesco.CorrectControllerIcon";
        public const string modName = "CorrectControllerIcon";
        public const string modVersion = "0.1.0";
        internal static readonly Harmony _harmony = new Harmony(modGUID);
        internal static ManualLogSource mlg = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        internal static ConfigEntry<ControllerIcons> Icons;

        void Awake()
        {
            Icons = Config.Bind("General", "Icons", ControllerIcons.Auto,
                "Which controller's buttons are shown. Auto leaves it to InputUtils, which sees every controller as Xbox while Steam Input is on. Also set from the top right corner of the keybind menu.");

            Config.SettingChanged += (sender, args) =>
            {
                if (args.ChangedSetting != Icons)
                    return;
                RebindButton.ReloadGlyphs();
                IconChoice.UpdateLabel();
            };

            mlg.LogInfo($"Plugin {modName} is loaded!");
            _harmony.PatchAll();
        }
    }
}
