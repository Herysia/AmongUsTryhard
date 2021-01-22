using HarmonyLib;
using PowerTools;
using Reactor;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsTryhard.Patches
{
    internal class AdminBlinkPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static class AdminBlinkKillAnimationCoPerformKill
        {
            public static bool Prefix(PlayerControl __instance, PlayerControl CAKODNGLPDF)
            {
                PlayerControl target = CAKODNGLPDF;

                if (AmongUsClient.Instance.IsGameOver)
                {
                    return false;
                }

                if (!target || __instance.Data.IsDead || !__instance.Data.IsImpostor || __instance.Data.Disconnected)
                {
                    int num = target ? ((int) target.PlayerId) : -1;
                    Debug.LogWarning(string.Format("Bad kill from {0} to {1}", __instance.PlayerId, num));
                    return false;
                }

                GameData.PlayerInfo data = target.Data;
                if (data == null || data.IsDead)
                {
                    Debug.LogWarning("Missing target data for kill");
                    return false;
                }

                if (__instance.AmOwner)
                {
                    StatsManager instance = StatsManager.Instance;
                    uint num2 = instance.ImpostorKills;
                    instance.ImpostorKills = num2 + 1u;
                    if (Constants.Method_3()) //ShouldPlaySfx
                    {
                        SoundManager.Instance.PlaySound(__instance.KillSfx, false, 0.8f);
                    }
                }

                __instance.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                DestroyableSingleton<Telemetry>.Instance.WriteMurder();
                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (target.AmOwner)
                {
                    StatsManager instance2 = StatsManager.Instance;
                    uint num2 = instance2.TimesMurdered;
                    instance2.TimesMurdered = num2 + 1u;
                    if (Minigame.Instance)
                    {
                        try
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }
                        catch
                        {
                        }
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowOne(__instance.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(__instance.transform, false);
                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        target.Method_84(); //PlayerControl.ClearTasks
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(new Object[0]));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(new Object[0]));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                Coroutines.Start(CoPerformKillFixed(__instance.KillAnimations.Random<KillAnimation>(), __instance,
                    target));
                return false;
            }

            private static System.Collections.IEnumerator CoPerformKillFixed(KillAnimation __instance,
                PlayerControl source, PlayerControl target)
            {
                FollowerCamera cam = Camera.main.GetComponent<FollowerCamera>();
                bool isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
                PlayerPhysics sourcePhys = source.MyPhysics;
                KillAnimation.SetMovement(source, false);
                KillAnimation.SetMovement(target, false);
                if (isParticipant)
                {
                    cam.Locked = true;
                }

                target.Die(DeathReason.Kill);
                //We spawn dead body before WaitForAnimationFinish, to prevent having delay before the body spawns, while the target is already dead
                DeadBody deadBody = Object.Instantiate<DeadBody>(__instance.bodyPrefab);
                Vector3 vector = target.transform.position + __instance.BodyOffset;
                vector.z = vector.y / 1000f;
                deadBody.transform.position = vector;
                deadBody.ParentId = target.PlayerId;
                target.SetPlayerMaterialColors(deadBody.GetComponent<Renderer>());
                //
                SpriteAnim sourceAnim = source.GetComponent<SpriteAnim>();
                yield return new WaitForAnimationFinish(sourceAnim, __instance.BlurAnim);
                source.NetTransform.SnapTo(target.transform.position);
                sourceAnim.Play(sourcePhys.IdleAnim, 1f);
                KillAnimation.SetMovement(source, true);
                //
                KillAnimation.SetMovement(target, true);
                if (isParticipant)
                {
                    cam.Locked = false;
                }

                yield break;
            }
        }
    }
}