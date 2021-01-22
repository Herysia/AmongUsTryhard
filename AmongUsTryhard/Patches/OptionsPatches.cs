using HarmonyLib;
using Hazel;
using Il2CppSystem.IO;
using System;
using Il2CppDumper;
using UnhollowerBaseLib;
using UnityEngine;
using Reactor;

namespace AmongUsTryhard.Patches
{
    //Code from : https://github.com/Galster-dev/CrowdedSheriff/blob/master/src/OptionsPatches.cs <3
    internal class OptionsPatches
    {
        public static byte maxPlayerAdmin = AmongUsTryhard.maxPlayerAdmin.Value;
        public static byte maxPlayerCams = AmongUsTryhard.maxPlayerAdmin.Value;
        public static byte maxPlayerVitals = AmongUsTryhard.maxPlayerAdmin.Value;

        const StringNames maxPlayerAdminTitle = (StringNames) 1337;
        const StringNames maxPlayerCamsTitle = (StringNames) 1338;
        const StringNames maxPlayerVitalsTitle = (StringNames) 1339;

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
            new Type[] {typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)})]
        static class TranslationController_GetString
        {
            public static bool Prefix(StringNames HKOIECMDOKL, ref string __result)
            {
                switch (HKOIECMDOKL)
                {
                    case maxPlayerAdminTitle:
                        __result = "Admin max players";
                        break;
                    case maxPlayerCamsTitle:
                        __result = "Cams max players";
                        break;
                    case maxPlayerVitalsTitle:
                        __result = "Vitals max players";
                        break;
                    default:
                        return true;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        static class GameOptionsMenu_Start
        {
            public static void OnValueChanged(OptionBehaviour option)
            {
                if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost) return;
                switch (option.Title)
                {
                    case maxPlayerAdminTitle:
                        maxPlayerAdmin = (byte) option.GetInt();
                        AmongUsTryhard.maxPlayerAdmin.Value = maxPlayerAdmin;
                        break;
                    case maxPlayerCamsTitle:
                        maxPlayerCams = (byte) option.GetInt();
                        AmongUsTryhard.maxPlayerCams.Value = maxPlayerCams;
                        break;
                    case maxPlayerVitalsTitle:
                        maxPlayerVitals = (byte) option.GetInt();
                        AmongUsTryhard.maxPlayerVitals.Value = maxPlayerVitals;
                        break;
                }

                if (PlayerControl.GameOptions.isDefaults)
                {
                    PlayerControl.GameOptions.isDefaults = false;
                    UnityEngine.Object.FindObjectOfType<GameOptionsMenu>().Method_16(); //RefreshChildren
                }

                var local = PlayerControl.LocalPlayer;
                if (local != null)
                {
                    local.RpcSyncSettings(PlayerControl.GameOptions);
                }
            }

            static void Postfix(ref GameOptionsMenu __instance)
            {
                var countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, -8.35f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerAdminTitle;
                countOption.Value = maxPlayerAdmin;
                var str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();

                countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, -8.85f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerCamsTitle;
                countOption.Value = maxPlayerCams;
                str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();

                countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, -9.35f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerVitalsTitle;
                countOption.Value = maxPlayerVitals;
                str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();

                __instance.GetComponentInParent<Scroller>().YBounds.max += 0.6f;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        static class GameSettingsMenu_OnEnable
        {
            static void Prefix(ref GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        static class NumberOption_OnEnable
        {
            static bool Prefix(ref NumberOption __instance)
            {
                if (__instance.Title == maxPlayerAdminTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = maxPlayerAdmin;
                    __instance.enabled = true;

                    return false;
                }

                if (__instance.Title == maxPlayerCamsTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = maxPlayerCams;
                    __instance.enabled = true;

                    return false;
                }

                if (__instance.Title == maxPlayerVitalsTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = maxPlayerVitals;
                    __instance.enabled = true;

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
        static class GameOptionsData_ToHudString
        {
            static void Postfix(ref string __result)
            {
                var builder = new System.Text.StringBuilder(__result);
                builder.AppendLine();
                builder.AppendLine($"Admin max players: {maxPlayerAdmin}");
                builder.AppendLine($"Cams max players: {maxPlayerCams}");
                builder.AppendLine($"Vitals max players: {maxPlayerVitals}");
                __result = builder.ToString();
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_65), typeof(BinaryReader))]
        static class GameOptionsData_Deserialize
        {
            static void Postfix(BinaryReader ALMCIJKELCP)
            {
                try
                {
                    maxPlayerAdmin = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerAdmin = 10;
                }

                try
                {
                    maxPlayerCams = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerCams = 10;
                }

                try
                {
                    maxPlayerVitals = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerVitals = 10;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_7), typeof(MessageReader))]
        static class GameOptionsData_DeserializeM
        {
            static void Postfix(MessageReader ALMCIJKELCP)
            {
                try
                {
                    maxPlayerAdmin = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerAdmin = 10;
                }

                try
                {
                    maxPlayerCams = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerCams = 10;
                }

                try
                {
                    maxPlayerVitals = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    maxPlayerVitals = 10;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_53),
            new Type[] {typeof(BinaryWriter), typeof(byte)})]
        static class GameOptionsData_Serialize
        {
            static void Postfix(BinaryWriter AGLJMGAODDG)
            {
                AGLJMGAODDG.Write(maxPlayerAdmin);
                AGLJMGAODDG.Write(maxPlayerCams);
                AGLJMGAODDG.Write(maxPlayerVitals);
            }
        }
    }
}