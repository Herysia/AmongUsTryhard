using Hazel;
using LobbyOptionsAPI;
using System.IO;

namespace AmongUsTryhard.Patches
{
    public class CustomGameOptionsData : LobbyOptions
    {
        private byte settingsVersion = 2;
        public static CustomGameOptionsData customGameOptions;

        public CustomGameOptionsData() : base(AmongUsTryhard.Id, AmongUsTryhard.rpcSettingsId)
        {
            maxPlayerAdmin = AddOption(10, "Admin max players", 0, 10);
            maxPlayerCams = AddOption(10, "Cams max players", 0, 10);
            maxPlayerVitals = AddOption(10, "Vitals max players", 0, 10);
            hideNames = AddOption(false, "Hide names");
            meetingKillCD = AddOption(10, "Kill cooldown after meeting", 10, 200, 10, "%");
            wiresAlwaysON = AddOption(false, "Force wire task");
        }

        public CustomNumberOption maxPlayerAdmin;
        public CustomNumberOption maxPlayerCams;
        public CustomNumberOption maxPlayerVitals;
        public CustomToggleOption hideNames;
        public CustomNumberOption meetingKillCD;
        public CustomToggleOption wiresAlwaysON;

        public override void SetRecommendations()
        {
            maxPlayerAdmin.value = 10;
            maxPlayerCams.value = 10;
            maxPlayerVitals.value = 10;
            hideNames.value = false;
            meetingKillCD.value = 100;
            wiresAlwaysON.value = false;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(this.settingsVersion);
            writer.Write((byte)maxPlayerAdmin.value);
            writer.Write((byte)maxPlayerCams.value);
            writer.Write((byte)maxPlayerVitals.value);
            writer.Write(hideNames.value);
            writer.Write((byte)meetingKillCD.value);
            writer.Write(wiresAlwaysON.value);
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
                if (b >= 2)
                {
                    wiresAlwaysON.value = reader.ReadBoolean();
                }
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
                if (b >= 2)
                {
                    wiresAlwaysON.value = reader.ReadBoolean();
                }
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
                settings.AppendLine($"Force wire task: {wiresAlwaysON.value}");
            }
            catch
            {
            }

            return settings.ToString();
        }
    }


}