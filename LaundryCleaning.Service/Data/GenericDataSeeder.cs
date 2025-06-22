using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Security.Roles;

namespace LaundryCleaning.Service.Data
{
    public class GenericDataSeeder
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericDataSeeder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedFromData()
        {
            bool dataUpdated = false;

            if (!_dbContext.Roles.Any())
            {
                dataUpdated = true;
                _dbContext.Roles.AddRange(RolesData.Roles);
                _dbContext.SaveChanges();
            }

            //var userTest = _context.Users.Find(existingUserId); // misalnya dari database
            //var role = _context.Roles.Find(existingRoleId);

            if (!_dbContext.Identities.Any())
            {
                dataUpdated = true;
                _dbContext.Identities.AddRange(IdentitiesData.Identities);
                _dbContext.SaveChanges();
            }

            if (dataUpdated)
            {
                _dbContext.SaveChanges();
            }
        }
    }

    public static class IdentitiesData
    {
        public static List<Identity> Identities => new List<Identity>
        {
            new Identity
            {
                Id = Guid.Parse("9F4A1DB7-DA3B-4AF5-934A-BB367F052FE1"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("9F4A1DB7-DA3B-4AF5-934A-BB367F052FE9")
            },
            new Identity
            {
                Id = Guid.Parse("9F4A1DB7-DA3B-4AF5-934A-BB367F052FE2"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("47052A52-0AB6-4577-96F3-0C7B61ABCFD4")
            },
        };
    }
}
