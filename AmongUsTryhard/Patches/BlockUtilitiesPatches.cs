using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;

namespace AmongUsTryhard.Patches
{
    internal class BlockUtilitiesPatches
    {
        private static bool vitalsBool = true;
        private static bool adminBool = true;
        private static bool camsBool = true;

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        public static class VitalsMinigameUpdate
        {
            public static bool Prefix(VitalsMinigame __instance)
            {
                if (!__instance.SabText.isActiveAndEnabled &&
                    vitalsBool)
                {
                    __instance.SabText.gameObject.SetActive(true);
                    for (int j = 0; j < __instance.vitals.Length; j++)
                    {
                        __instance.vitals[j].gameObject.SetActive(false);
                    }
                }

                return !vitalsBool;
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnEnable))]
        public static class MapCountOverlayOnEnable
        {
            public static void Postfix(MapCountOverlay __instance)
            {
                if (adminBool)
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        public static class MapCountOverlayUpdate
        {
            public static bool Prefix(MapCountOverlay __instance)
            {
                //Delay before display (Among Us code)
                __instance.timer += Time.deltaTime;
                if (__instance.timer < 0.1f)
                {
                    return false;
                }

                //Toggle ON/OFF depending on minPlayerVitals parameter
                if (!__instance.isSab &&
                    adminBool)
                {
                    __instance.isSab = true;
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                    __instance.SabotageText.gameObject.SetActive(true);
                }

                return !adminBool;
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
        public static class PlanetSurveillanceMinigameUpdate
        {
            public static bool Prefix(PlanetSurveillanceMinigame __instance)
            {
                //Toggle ON/OFF depending on minPlayerCams parameter
                if (!__instance.isStatic && camsBool)
                {
                    __instance.isStatic = true;
                    __instance.ViewPort.sharedMaterial = __instance.StaticMaterial;
                    __instance.SabText.gameObject.SetActive(true);
                }

                return !camsBool;
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.NextCamera))]
        public static class PlanetSurveillanceMinigameNextCamera
        {
            public static bool Prefix(PlanetSurveillanceMinigame __instance, int BPDCKHHJEHC)
            {
                if (camsBool)
                {
                    int direction = BPDCKHHJEHC;
                    if (direction != 0 && Constants.Method_3()) //ShouldPlaySfx
                    {
                        SoundManager.Instance.PlaySound(__instance.ChangeSound, false, 1f);
                    }

                    __instance.Dots[__instance.currentCamera].sprite = __instance.DotDisabled;

                    __instance.currentCamera = Extensions.Wrap(__instance.currentCamera + direction,
                        __instance.survCameras.Length);
                    __instance.Dots[__instance.currentCamera].sprite = __instance.DotEnabled;
                    SurvCamera survCamera = __instance.survCameras[__instance.currentCamera];
                    __instance.Camera.transform.position = survCamera.transform.position +
                                                           __instance.survCameras[__instance.currentCamera].Offset;
                    __instance.LocationName.Text = survCamera.CamName;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        public static class SurveillanceMinigameUpdate
        {
            public static bool Prefix(SurveillanceMinigame __instance)
            {
                //Toggle ON/OFF depending on minPlayerCams parameter
                if (!__instance.isStatic && camsBool)
                {
                    __instance.isStatic = true;
                    for (int j = 0; j < __instance.ViewPorts.Length; j++)
                    {
                        __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                        __instance.SabText[j].gameObject.SetActive(true);
                    }
                }

                return !camsBool;
            }
        }

        static void udpateBools(int playersLeft)
        {
            vitalsBool = playersLeft > CustomGameOptionsData.customGameOptions.maxPlayerVitals;
            adminBool = playersLeft > CustomGameOptionsData.customGameOptions.maxPlayerAdmin;
            camsBool = playersLeft > CustomGameOptionsData.customGameOptions.maxPlayerCams;
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        public static class ExileControllerBegin
        {
            public static void Postfix(ExileController __instance, GameData.PlayerInfo LNMDIKCFBAK)
            {
                int playersLeft = PlayerControl.AllPlayerControls.ToArray().Count(pc =>
                    pc.Data.IsDead || pc.Data.Disconnected) + (LNMDIKCFBAK != null ? 1 : 0);
                udpateBools(playersLeft);
            }
        }

        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleMessage))]
        public static class InnerNetClientHandleMessage
        {
            public static void Postfix(InnerNetClient __instance, MessageReader ALMCIJKELCP, SendOption GLLMNHCBIOC)
            {
                MessageReader reader = ALMCIJKELCP;
                if (reader.Tag != 2) return;


                int playersLeft = PlayerControl.AllPlayerControls.ToArray().Count(pc =>
                    !pc.Data.IsDead && !pc.Data.Disconnected);
                udpateBools(playersLeft);

            }
        }
    }
}