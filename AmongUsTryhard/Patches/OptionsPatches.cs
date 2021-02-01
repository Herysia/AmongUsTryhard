using Hazel;
using LobbyOptionsAPI;
using System.IO;

namespace AmongUsTryhard.Patches
{
    public class CustomGameOptionsData : LobbyOptions
    {
        private byte settingsVersion = 1;
        public static CustomGameOptionsData customGameOptions;

        public CustomGameOptionsData() : base(AmongUsTryhard.Id, AmongUsTryhard.rpcSettingsId)
        {
            maxPlayerAdmin = AddOption(10, "Admin max players", 0, 10);
            maxPlayerCams = AddOption(10, "Cams max players", 0, 10);
            maxPlayerVitals = AddOption(10, "Vitals max players", 0, 10);
            hideNames = AddOption(false, "Hide names");
            meetingKillCD = AddOption(10, "Kill cooldown after meeting", 10, 200, 10, "%");
        }

        public CustomNumberOption maxPlayerAdmin;
        public CustomNumberOption maxPlayerCams;
        public CustomNumberOption maxPlayerVitals;
        public CustomToggleOption hideNames;
        public CustomNumberOption meetingKillCD;

        public override void SetRecommendations()
        {
            maxPlayerAdmin.value = 10;
            maxPlayerCams.value = 10;
            maxPlayerVitals.value = 10;
            hideNames.value = false;
            meetingKillCD.value = 100;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(this.settingsVersion);
            writer.Write((byte)maxPlayerAdmin.value);
            writer.Write((byte)maxPlayerCams.value);
            writer.Write((byte)maxPlayerVitals.value);
            writer.Write(hideNames.value);
            writer.Write((byte)meetingKillCD.value);
        }

        public override void Deserialize(BinaryReader reader)
        {
            try
            {
                SetRecommendations();
                byte b = reader.ReadByte();
                maxPlayerAdmin.value = reader.ReadByte();
                maxPlayerCams.value = reader.ReadByte();
                maxPlayerVitals.value = reader.ReadByte();
                hideNames.value = reader.ReadBoolean();
                meetingKillCD.value = reader.ReadByte();
            }
            catch
            {
            }
        }

        public override void Deserialize(MessageReader reader)
        {
            try
            {
                SetRecommendations();
                byte b = reader.ReadByte();
                maxPlayerAdmin.value = reader.ReadByte();
                maxPlayerCams.value = reader.ReadByte();
                maxPlayerVitals.value = reader.ReadByte();
                hideNames.value = reader.ReadBoolean();
                meetingKillCD.value = reader.ReadByte();
            }
            catch
            {
            }
        }

        public override string ToHudString()
        {
            settings.Length = 0;

            try
            {
                settings.AppendLine();
                settings.AppendLine(
                    $"Max players for: Admin={maxPlayerAdmin.value}, Cams={maxPlayerCams.value}, Vitals={maxPlayerVitals.value}");
                settings.AppendLine($"Hide names: {hideNames.value}");
                settings.Append("Kill cooldown after meeting: ");
                settings.Append(meetingKillCD.value);
                settings.Append("%");
                settings.AppendLine();
            }
            catch
            {
            }

            return settings.ToString();
        }
    }


}