using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Migrations;
using LaundryCleaning.Service.Security.Permissions;
using LaundryCleaning.Service.Services.Interfaces;
using System.Threading;

namespace LaundryCleaning.Service.Security.Admin
{
    public class ITTeamData
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public ITTeamData(ApplicationDbContext dbContext,
            IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        public void AddData()
        {
            bool dataUpdated = false;

            // User
            if (!_dbContext.Users.Any(x => x.Id == (Guid.Parse("C0B3553A-5EE3-43C6-AD65-FF9FA34B3F66")))) 
            {
                dataUpdated = true;

                var newUser = new User()
                {
                    Id = Guid.Parse("C0B3553A-5EE3-43C6-AD65-FF9FA34B3F66"),
                    Password = _passwordService.GeneratePassword("itteam@admin1234"),
                    Email = "itteam@itteam.com",
                    Username = "itteam",
                    FirstName = "itteam",
                    LastName = string.Empty
                };

                _dbContext.Users.Add(newUser);
            }

            // Identity
            if (!_dbContext.Identities.Any(x => x.Id == (Guid.Parse("13613846-4F25-42C1-9F37-DBC87C50C816"))))
            {
                dataUpdated = true;

                var newIdentity = new Identity()
                {
                    Id = Guid.Parse("13613846-4F25-42C1-9F37-DBC87C50C816"),
                    UserId = Guid.Parse("C0B3553A-5EE3-43C6-AD65-FF9FA34B3F66"),
                    RoleId = Guid.Parse("382254A2-2FEE-4F45-8A9F-68FBDBC07966"), // ITTeam
                };

                _dbContext.Identities.Add(newIdentity);
            }

            // Role Permission
            #region Role Permission
            if (!_dbContext.RolePermissions.Any(x => x.Id == (Guid.Parse("5CEDE765-262F-4447-88F6-D8EB4BF85000"))))
            {
                dataUpdated = true;

                var newRolePermission = new RolePermission()
                {
                    Id = Guid.Parse("5CEDE765-262F-4447-88F6-D8EB4BF85000"),
                    RoleId = Guid.Parse("382254A2-2FEE-4F45-8A9F-68FBDBC07966"), // ITTeam
                    Permission = PermissionConstants.RoleManage
                };

                _dbContext.RolePermissions.Add(newRolePermission);
            }
            if (!_dbContext.RolePermissions.Any(x => x.Id == (Guid.Parse("5CEDE765-262F-4447-88F6-D8EB4BF85001"))))
            {
                dataUpdated = true;

                var newRolePermission = new RolePermission()
                {
                    Id = Guid.Parse("5CEDE765-262F-4447-88F6-D8EB4BF85001"),
                    RoleId = Guid.Parse("382254A2-2FEE-4F45-8A9F-68FBDBC07966"), // ITTeam
                    Permission = PermissionConstants.UserManage
                };

                _dbContext.RolePermissions.Add(newRolePermission);
            }

            #endregion

            if (dataUpdated)
            {
                _dbContext.SaveChanges();
            }
        }
    }
}
