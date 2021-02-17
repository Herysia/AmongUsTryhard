using System.Globalization;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsTryhard.Patches
{
    // Token: 0x0200000A RID: 10
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch
    {
        // Token: 0x06000031 RID: 49 RVA: 0x00002D14 File Offset: 0x00000F14
        public static void Postfix(VersionShower __instance)
        {
            TextRenderer herysiaVersionShower =
                Object.FindObjectsOfType<TextRenderer>().FirstOrDefault(obj => obj.name == "HerysiaVersion");
            if (!herysiaVersionShower)
            {
                TextRenderer original = __instance.text;
                herysiaVersionShower = Object.Instantiate(original, original.transform.parent);
                if (herysiaVersionShower.name == "Text(Clone)")
                    herysiaVersionShower.transform.localPosition = new Vector3(10.43f,
                        herysiaVersionShower.transform.localPosition.y, herysiaVersionShower.transform.localPosition.z);
                else
                    herysiaVersionShower.transform.localPosition = new Vector3(5.2f,
                        herysiaVersionShower.transform.localPosition.y, herysiaVersionShower.transform.localPosition.z);
                herysiaVersionShower.name = "HerysiaVersion";
                herysiaVersionShower.RightAligned = true;
            }
            herysiaVersionShower.Text = string.Concat(new string[]
            {
                herysiaVersionShower.Text,
                "\n",
                "AmongUsTryhard",
                " v1.7.2",
                " by Herysia#4293"
            });
        }

        // Token: 0x04000010 RID: 16
        public static string name;

        // Token: 0x04000011 RID: 17
        public static string version;
    }
}