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
        public static byte maxPlayerAdmin = 10;
        public static byte maxPlayerCams = 10;
        public static byte maxPlayerVitals = 10;
        public static bool hideNames = false;
        public static byte meetingKillCD = 100;

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
                        maxPlayerAdmin = (byte) option.GetInt();
                        break;
                    case maxPlayerCamsTitle:
                        maxPlayerCams = (byte) option.GetInt();
                        break;
                    case maxPlayerVitalsTitle:
                        maxPlayerVitals = (byte) option.GetInt();
                        break;
                    case hideNamesTitle:
                        hideNames = option.GetBool();
                        break;
                    case meetingKillCDTitle:
                        meetingKillCD = (byte)option.GetInt();
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
                countOption.Value = maxPlayerAdmin;
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
                countOption.Value = maxPlayerCams;
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
                countOption.Value = maxPlayerVitals;
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
                toggleOption.CheckMark.enabled = hideNames;
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
                countOption.Value = meetingKillCD;
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
                    __instance.CheckMark.enabled = hideNames;
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

                if (__instance.Title == meetingKillCDTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = meetingKillCD;
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
                builder.AppendLine();
                builder.AppendLine($"Max players for: Admin={maxPlayerAdmin}, Cams={maxPlayerCams}, Vitals={maxPlayerVitals}");
                builder.AppendLine($"Hide names: {hideNames}");
                builder.Append("Kill cooldown after meeting: ");
                builder.Append(meetingKillCD);
                builder.Append("%");
                builder.AppendLine();
                __result = builder.ToString();

                DestroyableSingleton<HudManager>.Instance.GameSettings.scale = 0.6f;
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
                try
                {
                    hideNames = ALMCIJKELCP.ReadBoolean();
                }
                catch
                {
                    hideNames = false;
                }

                try
                {
                    meetingKillCD = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    meetingKillCD = 100;
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
                try
                {
                    hideNames = ALMCIJKELCP.ReadBoolean();
                }
                catch
                {
                    hideNames = false;
                }
                try
                {
                    meetingKillCD = ALMCIJKELCP.ReadByte();
                }
                catch
                {
                    meetingKillCD = 100;
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
                AGLJMGAODDG.Write(hideNames);
                AGLJMGAODDG.Write(meetingKillCD);
            }
        }
    }
}