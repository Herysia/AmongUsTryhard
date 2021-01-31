using HarmonyLib;

namespace AmongUsTryhard.Patches
{
    internal class InvisibleVentPatches
    {
        [HarmonyPatch(typeof(PlayerPhysics.CoExitVent__d), nameof(PlayerPhysics.CoExitVent__d.MoveNext))]
        public static class PlayerPhysics_CoExitVent
        {
            public static void Postfix(PlayerPhysics.CoExitVent__d __instance)
            {
                __instance.__this.myPlayer.Visible = true;
            }
        }

    }
}
