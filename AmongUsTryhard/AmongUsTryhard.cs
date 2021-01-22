using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace AmongUsTryhard
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class AmongUsTryhard : BasePlugin
    {
        public const string Id = "com.herysia.amongustryhard";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static ConfigEntry<byte> maxPlayerAdmin { get; private set; }
        public static ConfigEntry<byte> maxPlayerCams { get; private set; }
        public static ConfigEntry<byte> maxPlayerVitals { get; private set; }
        public override void Load()
        {
            maxPlayerAdmin = Config.Bind("AmongUsTryhard", "maxPlayerAdmin", (byte)10);
            maxPlayerCams = Config.Bind("AmongUsTryhard", "maxPlayerCams", (byte)10);
            maxPlayerVitals = Config.Bind("AmongUsTryhard", "maxPlayerVitals", (byte)10);

            Harmony.PatchAll();
        }
    }
}