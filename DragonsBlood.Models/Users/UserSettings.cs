namespace DragonsBlood.Models.Users
{
    public class UserSettings
    {
        public int InitialChatMessagesToDisplay { get; set; }
        public bool ShowMiniChat { get; set; }
        public string ChatNameColor { get; set; }
        public bool PingForMessages { get; set; }
        public bool PulseForAlerts { get; set; }

    }
}