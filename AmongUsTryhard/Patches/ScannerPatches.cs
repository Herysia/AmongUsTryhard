using HarmonyLib;
using PowerTools;
using UnityEngine;

namespace AmongUsTryhard.Patches
{
    internal class ScannerPatches
    {
        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        public static class MedScanMinigame_FixedUpdate
        {
            public static void Prefix(MedScanMinigame __instance)
            {
                if (__instance.MyNormTask.IsComplete)
                {
                    return;
                }
                byte playerId = PlayerControl.LocalPlayer.PlayerId;
                if (__instance.medscan.CurrentUser != playerId)
                {
                    __instance.medscan.CurrentUser = playerId;
                }
            }
        }
    }
}