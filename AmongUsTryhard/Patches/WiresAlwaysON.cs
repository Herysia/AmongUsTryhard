using System;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace AmongUsTryhard.Patches
{
    internal class WiresAlwaysON
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static class ShipStatus_Awake
        {
            [HarmonyPriority(Priority.Last)]
            public static bool Prefix(ShipStatus __instance)
            {
                __instance.Method_32(); //AssignTaskIndexes
                GameOptionsData gameOptions = PlayerControl.GameOptions;
                List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
                HashSet<TaskTypes> hashSet = new HashSet<TaskTypes>();
                List<byte> list = new List<byte>(20);
                List<NormalPlayerTask> list2 =
                    ConvertToListAndShuffle(
                        new System.Collections.Generic.List<NormalPlayerTask>(__instance.CommonTasks));
                if (CustomGameOptionsData.customGameOptions.wiresAlwaysON.value)
                {
                    hashSet.Add(TaskTypes.FixWiring);
                    list.Add((byte)__instance.CommonTasks.First(tsk => tsk.TaskType == TaskTypes.FixWiring).Index);
                }

                int num = 0;
                __instance.Method_69(ref num, gameOptions.NumCommonTasks, list, hashSet, list2); //AddTasksFromList
                for (int i = 0; i < gameOptions.NumCommonTasks; i++)
                {
                    if (list2.Count == 0)
                    {
                        Debug.LogWarning("Not enough common tasks");
                        break;
                    }

                    int index = new System.Random().Next(0, list2.Count);
                    list.Add((byte) list2[index].Index);
                    list2.RemoveAt(index);
                }

                List<NormalPlayerTask> list3 =
                    ConvertToListAndShuffle(
                        new System.Collections.Generic.List<NormalPlayerTask>(__instance.LongTasks));
                List<NormalPlayerTask> list4 =
                    ConvertToListAndShuffle(
                        new System.Collections.Generic.List<NormalPlayerTask>(__instance.NormalTasks));

                int num2 = 0;
                int num3 = 0;
                int count = gameOptions.NumShortTasks;
                if (gameOptions.NumCommonTasks + gameOptions.NumLongTasks + gameOptions.NumShortTasks == 0)
                {
                    count = 1;
                }

                byte b = 0;
                while ((int) b < allPlayers.Count)
                {
                    hashSet.Clear();
                    list.RemoveRange(gameOptions.NumCommonTasks, list.Count - gameOptions.NumCommonTasks);
                    __instance.Method_69(ref num2, gameOptions.NumLongTasks, list, hashSet, list3); //AddTasksFromList
                    __instance.Method_69(ref num3, count, list, hashSet, list4); //AddTasksFromList
                    GameData.PlayerInfo playerInfo = allPlayers[(int) b];
                    if (playerInfo.Object && !playerInfo.Object.GetComponent<DummyBehaviour>().enabled)
                    {
                        byte[] taskTypeIds = list.ToArray();
                        GameData.Instance.RpcSetTasks(playerInfo.PlayerId, taskTypeIds);
                    }

                    b += 1;
                }

                __instance.enabled = true;
                return false;
            }
        }

        public static List<TSource> ConvertToListAndShuffle<TSource>(System.Collections.Generic.List<TSource> list)
        {
            list.Shuffle();
            var ret = new List<TSource>();
            foreach (var v in list)
            {
                ret.Add(v);
            }

            return ret;
        }
    }

    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this System.Collections.Generic.IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}