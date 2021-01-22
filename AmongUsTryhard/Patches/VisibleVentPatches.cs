using HarmonyLib;
using PowerTools;

namespace AmongUsTryhard.Patches
{
    internal class VisibleVentPatches
    {
        //Method_1, Method_23, Method_38, Method_50
        [HarmonyPatch(typeof(Vent), nameof(Vent.Method_38))] //EnterVent
        public static class EnterVentPatch
        {
            public static bool Prefix(Vent __instance, PlayerControl NMEAPOJFNKA)
            {
                PlayerControl pc = NMEAPOJFNKA;
                if (!__instance.EnterVentAnim)
                {
                    return false;
                }

                if (pc.AmOwner || (pc.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).magnitude <
                    PlayerControl.LocalPlayer.myLight.LightRadius)
                {
                    __instance.GetComponent<SpriteAnim>().Play(__instance.EnterVentAnim, 1f);
                }

                if (pc.AmOwner && Constants.Method_3()) //ShouldPlaySfx
                {
                    SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                    SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch =
                        UnityEngine.Random.Range(0.8f, 1.2f);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.Method_1))] //ExitVent
        public static class ExitVentPatch
        {
            public static bool Prefix(Vent __instance, PlayerControl NMEAPOJFNKA)
            {
                PlayerControl pc = NMEAPOJFNKA;
                if (!__instance.ExitVentAnim)
                {
                    return false;
                }

                if (pc.AmOwner || (pc.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).magnitude <
                    PlayerControl.LocalPlayer.myLight.LightRadius)
                {
                    __instance.GetComponent<SpriteAnim>().Play(__instance.ExitVentAnim, 1f);
                }

                if (pc.AmOwner && Constants.Method_3()) //ShouldPlaySfx
                {
                    SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                    SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch =
                        UnityEngine.Random.Range(0.8f, 1.2f);
                }

                return false;
            }
        }
    }
}