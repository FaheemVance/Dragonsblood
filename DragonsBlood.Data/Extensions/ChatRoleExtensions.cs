using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Data.Extensions
{
    public static class ChatRoleExtensions
    {
        public static bool IsAdmin(this ChatUserRole role)
        {
            return role.Role.Name == "Admin";
        }

        public static bool IsMod(this ChatUserRole role)
        {
            return role.Role.Name == "Admin" || role.Role.Name == "Moderator";
        }
    }
}
