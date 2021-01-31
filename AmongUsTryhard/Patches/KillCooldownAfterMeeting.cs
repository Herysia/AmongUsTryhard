using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace AmongUsTryhard.Patches
{
    class KillCooldownAfterMeeting
    {
        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Method_37))] //WrapUp
        static class ExileController_WrapUp
        {
            [HarmonyPriority(Priority.Last)]
            static void Postfix()
            {
                if (DestroyableSingleton<TutorialManager>.InstanceExists || !ShipStatus.Instance.IsGameOverDueToDeath())
                {
                    PlayerControl.LocalPlayer.killTimer *= OptionsPatches.meetingKillCD / 100f;
                }
            }
        }
    }
}