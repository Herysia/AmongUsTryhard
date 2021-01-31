using HarmonyLib;
using Hazel;
using Il2CppSystem.IO;
using System;
using System.Linq;
using Il2CppDumper;
using UnhollowerBaseLib;
using UnityEngine;
using Reactor;

namespace AmongUsTryhard.Patches
{
    //Code from : https://github.com/Galster-dev/CrowdedSheriff/blob/master/src/OptionsPatches.cs <3
    internal class OptionsPatches
    {
        const StringNames maxPlayerAdminTitle = (StringNames) 1337;
        const StringNames maxPlayerCamsTitle = (StringNames) 1338;
        const StringNames maxPlayerVitalsTitle = (StringNames) 1339;
        const StringNames hideNamesTitle = (StringNames) 1340;
        const StringNames meetingKillCDTitle = (StringNames) 1341;

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
                    case hideNamesTitle:
                        __result = "Hide names";
                        break;
                    case meetingKillCDTitle:
                        __result = "Kill cooldown % after meeting";
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
                        CustomGameOptionsData.customGameOptions.maxPlayerAdmin = (byte) option.GetInt();
                        break;
                    case maxPlayerCamsTitle:
                        CustomGameOptionsData.customGameOptions.maxPlayerCams = (byte) option.GetInt();
                        break;
                    case maxPlayerVitalsTitle:
                        CustomGameOptionsData.customGameOptions.maxPlayerVitals = (byte) option.GetInt();
                        break;
                    case hideNamesTitle:
                        CustomGameOptionsData.customGameOptions.hideNames = option.GetBool();
                        break;
                    case meetingKillCDTitle:
                        CustomGameOptionsData.customGameOptions.meetingKillCD = (byte)option.GetInt();
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

            static float GetLowestConfigY(GameOptionsMenu __instance)
            {
                return __instance.GetComponentsInChildren<OptionBehaviour>()
                    .Min(option => option.transform.localPosition.y);
            }

            [HarmonyPriority(Priority.Normal - 1)]
            static void Postfix(ref GameOptionsMenu __instance)
            {
                var lowestY = GetLowestConfigY(__instance);
                //__instance.GetComponentsInChildren<NumberOption>().First(opt => opt.name == "Crewmate Vision").Increment = 0.05f;

                var countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, lowestY - 0.5f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerAdminTitle;
                countOption.Value = CustomGameOptionsData.customGameOptions.maxPlayerAdmin;
                var str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();
                countOption.ValidRange.max = 10;
                countOption.ValidRange.min = 3;

                countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, lowestY - 1.0f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerCamsTitle;
                countOption.Value = CustomGameOptionsData.customGameOptions.maxPlayerCams;
                str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();
                countOption.ValidRange.max = 10;
                countOption.ValidRange.min = 3;

                countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, lowestY - 1.5f,
                    countOption.transform.localPosition.z);
                countOption.Title = maxPlayerVitalsTitle;
                countOption.Value = CustomGameOptionsData.customGameOptions.maxPlayerVitals;
                str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();
                countOption.ValidRange.max = 10;
                countOption.ValidRange.min = 3;

                var toggleOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<ToggleOption>()[1],
                    __instance.transform);
                toggleOption.transform.localPosition = new Vector3(toggleOption.transform.localPosition.x,
                    lowestY - 2.0f, toggleOption.transform.localPosition.z);
                toggleOption.Title = hideNamesTitle;
                toggleOption.CheckMark.enabled = CustomGameOptionsData.customGameOptions.hideNames;
                str = "";
                TranslationController_GetString.Prefix(toggleOption.Title, ref str);
                toggleOption.TitleText.Text = str;
                toggleOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                toggleOption.gameObject.AddComponent<OptionBehaviour>();

                countOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[1],
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x, lowestY - 2.5f,
                    countOption.transform.localPosition.z);
                countOption.Title = meetingKillCDTitle;
                countOption.Value = CustomGameOptionsData.customGameOptions.meetingKillCD;
                str = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str);
                countOption.TitleText.Text = str;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();
                countOption.ValidRange.max = 200;
                countOption.ValidRange.min = 10;
                countOption.Increment = 10;
                countOption.FormatString = "{0:0}%";

                __instance.GetComponentInParent<Scroller>().YBounds.max += 2.5f;
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

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        static class ToggleOption_OnEnable
        {
            static bool Prefix(ref ToggleOption __instance)
            {
                if (__instance.Title == hideNamesTitle)
                {
                    string str = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref str);
                    __instance.TitleText.Text = str;
                    __instance.CheckMark.enabled = CustomGameOptionsData.customGameOptions.hideNames;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.enabled = true;

                    return false;
                }

                return true;
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
                    __instance.Value = CustomGameOptionsData.customGameOptions.maxPlayerAdmin;
                    __instance.enabled = true;

                    return false;
                }

                if (__instance.Title == maxPlayerCamsTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = CustomGameOptionsData.customGameOptions.maxPlayerCams;
                    __instance.enabled = true;

                    return false;
                }

                if (__instance.Title == maxPlayerVitalsTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = CustomGameOptionsData.customGameOptions.maxPlayerVitals;
                    __instance.enabled = true;

                    return false;
                }

                if (__instance.Title == meetingKillCDTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = CustomGameOptionsData.customGameOptions.meetingKillCD;
                    __instance.enabled = true;

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
        static class GameOptionsData_ToHudString
        {
            [HarmonyPriority(Priority.Normal - 1)]
            static void Postfix(ref string __result)
            {
                var builder = new System.Text.StringBuilder(__result);
                builder.Append(CustomGameOptionsData.customGameOptions.ToHudString());
                __result = builder.ToString();

                DestroyableSingleton<HudManager>.Instance.GameSettings.scale = 0.6f;
            }
        }
    }
}