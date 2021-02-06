using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace AmongUsTryhard.Patches
{
    internal class KillButtonBlinkPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
        static class PlayerControl_SetKillTimer
        {
            [HarmonyPriority(Priority.First)]
            static bool Prefix(PlayerControl __instance)
            {
                return __instance == PlayerControl.LocalPlayer;
            }
        }
    }
}