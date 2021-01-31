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

        public override void Load()
        {

            Harmony.PatchAll();
        }
    }
}