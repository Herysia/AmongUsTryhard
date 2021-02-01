using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnhollowerBaseLib;

namespace AmongUsTryhard.Patches
{
    internal class HideNames
    {
        public static void hideNames()
        {
            if (!CustomGameOptionsData.customGameOptions.hideNames.value) return;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (player.nameText.Color.Equals(Palette.ImpostorRed) && PlayerControl.LocalPlayer.Data.IsImpostor)
                    continue; //TODO: color != red

                player.nameText.Text = "";
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class PlayerControl_HandleRpc_SelectInfected
        {
            [HarmonyPriority(Priority.Last)]
            public static void Postfix(byte HKHMBLJFLMC)
            {
                if (HKHMBLJFLMC == 3)
                    hideNames();
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
        public static class PlayerControl_RpcSetInfected
        {
            [HarmonyPriority(Priority.Last)]
            public static void Postfix()
            {
                hideNames();
            }
        }
    }
}