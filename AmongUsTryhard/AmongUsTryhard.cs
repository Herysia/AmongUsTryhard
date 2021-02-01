using AmongUsTryhard.Patches;
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
    [BepInDependency("com.herysia.LobbyOptionsAPI")]
    public class AmongUsTryhard : BasePlugin
    {
        public const string Id = "com.herysia.amongustryhard";
        public static byte rpcSettingsId = 60;
        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            CustomGameOptionsData.customGameOptions = new CustomGameOptionsData();
            Harmony.PatchAll();
        }
    }
}