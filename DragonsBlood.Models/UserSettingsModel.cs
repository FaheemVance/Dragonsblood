using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace DragonsBlood.Models
{
    public class UserSettingsModel
    {
        [DisplayName("Initial Chat Messages To Display")]
        [Range(10, 100)]
        public int InitialChatMessagesToDisplay { get; set; }

        [DisplayName("Show Mini Chat")]
        public bool ShowMiniChat { get; set; }

        [DisplayName("Colour for name in chat")]
        public Color ChatNameColor { get; set; }
    }
}