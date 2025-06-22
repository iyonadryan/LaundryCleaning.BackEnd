using Microsoft.AspNetCore.Identity;

namespace LaundryCleaning.Service.Security.Permissions
{
    public class PermissionConstants
    {
        #region Business Permissions
        public const string FileUpload = "File:Upload";

        public const string UserManage = "User:Manage";
        public const string UserView = "User:View";

        public const string RoleManage = "Role:Manage";
        #endregion

        public static readonly List<string> AllPermission = new()
        {
            FileUpload,
            UserManage,
            UserView,
            RoleManage
        };
    }
}
