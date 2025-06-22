using LaundryCleaning.Service.Common.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaundryCleaning.Service.Security.Roles
{
    public static class RolesData
    {
        public static List<Role> Roles => new List<Role>
        {
            new Role
            {
                Id = RoleIdConstants.Admin,
                Name = RoleConstants.Admin,
                Code = RoleConstants.Admin.ToUpper()
            },
            new Role
            {
                Id = RoleIdConstants.Guest,
                Name = RoleConstants.Guest,
                Code = RoleConstants.Guest.ToUpper()
            },
            new Role
            {
                Id = RoleIdConstants.ITTeam,
                Name = RoleConstants.ITTeam,
                Code = RoleConstants.ITTeam.ToUpper()
            },
            new Role
            {
                Id = RoleIdConstants.Manager,
                Name = RoleConstants.Manager,
                Code = RoleConstants.Manager.ToUpper()
            },
            new Role
            {
                Id = RoleIdConstants.User,
                Name = RoleConstants.User,
                Code = RoleConstants.User.ToUpper()
            }
        };
    }
}
