using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor;
using UnityEngine;

//TODO: create class in lib, that is extended by custom settings this way, and which contains the patches and common code for every modules
// + handle 1 line setting display also
namespace AmongUsTryhard.Patches
{
    public class CustomGameOptionsData
    {
        //PlayerControl.RpcSyncSettings
        //PlayerControl.HandleRpc
        //SaveManager.LoadGameOptions
        //SaveManager.SaveGameOptions
        public static CustomGameOptionsData customGameOptions = new CustomGameOptionsData();

        public byte maxPlayerAdmin = 10;
        public byte maxPlayerCams = 10;
        public byte maxPlayerVitals = 10;
        public bool hideNames = false;
        public byte meetingKillCD = 100;
        private byte settingsVersion = 1;
        private StringBuilder settings = new StringBuilder(2048);

        public void SetRecommendations()
        {
            maxPlayerAdmin = 10;
            maxPlayerCams = 10;
            maxPlayerVitals = 10;
            hideNames = false;
            meetingKillCD = 100;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.settingsVersion);
            writer.Write(maxPlayerAdmin);
            writer.Write(maxPlayerCams);
            writer.Write(maxPlayerVitals);
            writer.Write(hideNames);
            writer.Write(meetingKillCD);
        }

        public static CustomGameOptionsData Deserialize(BinaryReader reader)
        {
            try
            {
                byte b = reader.ReadByte();
                CustomGameOptionsData gameOptionsData = new CustomGameOptionsData();
                gameOptionsData.maxPlayerAdmin = reader.ReadByte();
                gameOptionsData.maxPlayerCams = reader.ReadByte();
                gameOptionsData.maxPlayerVitals = reader.ReadByte();
                gameOptionsData.hideNames = reader.ReadBoolean();
                gameOptionsData.meetingKillCD = reader.ReadByte();
                /*
                try
                {
                    if (b > 1)
                    {
                        gameOptionsData.EmergencyCooldown = (int)reader.ReadByte();
                    }
                    if (b > 2)
                    {
                        gameOptionsData.ConfirmImpostor = reader.ReadBoolean();
                        gameOptionsData.VisualTasks = reader.ReadBoolean();
                    }
                    if (b > 3)
                    {
                        gameOptionsData.AnonymousVotes = reader.ReadBoolean();
                        gameOptionsData.TaskBarMode = (TaskBarMode)reader.ReadByte();
                    }
                }
                catch
                {
                }
                */
                return gameOptionsData;
            }
            catch
            {
            }

            return null;
        }

        public static CustomGameOptionsData Deserialize(MessageReader reader)
        {
            try
            {
                byte b = reader.ReadByte();
                CustomGameOptionsData gameOptionsData = new CustomGameOptionsData();
                gameOptionsData.maxPlayerAdmin = reader.ReadByte();
                gameOptionsData.maxPlayerCams = reader.ReadByte();
                gameOptionsData.maxPlayerVitals = reader.ReadByte();
                gameOptionsData.hideNames = reader.ReadBoolean();
                gameOptionsData.meetingKillCD = reader.ReadByte();
                /*
                try
                {
                    if (b > 1)
                    {
                        gameOptionsData.EmergencyCooldown = (int)reader.ReadByte();
                    }
                    if (b > 2)
                    {
                        gameOptionsData.ConfirmImpostor = reader.ReadBoolean();
                        gameOptionsData.VisualTasks = reader.ReadBoolean();
                    }
                    if (b > 3)
                    {
                        gameOptionsData.AnonymousVotes = reader.ReadBoolean();
                        gameOptionsData.TaskBarMode = (TaskBarMode)reader.ReadByte();
                    }
                }
                catch
                {
                }
                */
                return gameOptionsData;
            }
            catch
            {
            }

            return null;
        }

        public byte[] ToBytes()
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    this.Serialize(binaryWriter);
                    binaryWriter.Flush();
                    memoryStream.Position = 0L;
                    result = memoryStream.ToArray();
                }
            }

            return result;
        }

        public static CustomGameOptionsData FromBytes(byte[] bytes)
        {
            CustomGameOptionsData result;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    result = (CustomGameOptionsData.Deserialize(binaryReader) ?? new CustomGameOptionsData());
                }
            }

            return result;
        }

        public override string ToString()
        {
            return this.ToHudString();
        }

        // Token: 0x06000735 RID: 1845 RVA: 0x0002A0DC File Offset: 0x000282DC
        public string ToHudString()
        {
            settings.Length = 0;

            try
            {
                settings.AppendLine();
                settings.AppendLine(
                    $"Max players for: Admin={maxPlayerAdmin}, Cams={maxPlayerCams}, Vitals={maxPlayerVitals}");
                settings.AppendLine($"Hide names: {hideNames}");
                settings.Append("Kill cooldown after meeting: ");
                settings.Append(meetingKillCD);
                settings.Append("%");
                settings.AppendLine();
            }
            catch
            {
            }

            return settings.ToString();
        }

        public static CustomGameOptionsData LoadGameOptions(string filename)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        return CustomGameOptionsData.Deserialize(binaryReader) ?? new CustomGameOptionsData();
                    }
                }
            }

            return new CustomGameOptionsData();
        }
        private static void SaveGameOptions(CustomGameOptionsData data, string filename)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    data.Serialize(binaryWriter);
                }
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        public static class PlayerControl_RpcSyncSettings
        {
            public static void Postfix(GameOptionsData IOFBPLNIJIC)
            {
                if (PlayerControl.AllPlayerControls.Count > 1)
                {
                    MessageWriter messageWriter =
                        AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, AmongUsTryhard.rpcSettingsId,
                            SendOption.Reliable);
                    messageWriter.WriteBytesAndSize(customGameOptions.ToBytes());
                    messageWriter.EndMessage();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class PlayerControl_HandleRpc
        {
            public static void Postfix(byte HKHMBLJFLMC, MessageReader ALMCIJKELCP)
            {
                if (HKHMBLJFLMC == AmongUsTryhard.rpcSettingsId)
                {
                    CustomGameOptionsData.customGameOptions =
                        CustomGameOptionsData.FromBytes(ALMCIJKELCP.ReadBytesAndSize());
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_68))] //SetRecommendations
        public static class GameOptionsData_SetRecommendations
        {
            public static void Postfix()
            {
                CustomGameOptionsData.customGameOptions.SetRecommendations();
            }
        }

        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.Method_47))] //LoadGameOptions
        public static class SaveManager_LoadGameOptions
        {
            public static void Postfix(string HAGKNHFOMIJ)
            {
                if (HAGKNHFOMIJ == "gameHostOptions")
                {
                    CustomGameOptionsData.customGameOptions = LoadGameOptions(AmongUsTryhard.Id);
                }
            }
        }
        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveGameOptions))] //SaveGameOptions
        public static class SaveManager_SaveGameOptions
        {
            public static void Postfix(string HAGKNHFOMIJ)
            {
                if (HAGKNHFOMIJ == "gameHostOptions")
                {
                    SaveGameOptions(CustomGameOptionsData.customGameOptions, AmongUsTryhard.Id);
                }
            }
        }
    }
}