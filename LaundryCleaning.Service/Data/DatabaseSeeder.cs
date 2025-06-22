using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Security.Admin;
using LaundryCleaning.Service.Services.Interfaces;

namespace LaundryCleaning.Service.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService; // <-- Add This

        public DatabaseSeeder(ApplicationDbContext context,
            IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public void SeedAll()
        {
            var dataSeeder = new GenericDataSeeder(_context);
            var csvSeeder = new GenericCsvSeeder(_context);

            csvSeeder.SeedFromCsv<User>(System.IO.Path.Combine("Data", "DataSeeder", "users.csv"));
            dataSeeder.SeedFromData();

            var iTTeamDataSeeder = new ITTeamData(_context, _passwordService);
            iTTeamDataSeeder.AddData();
            
            csvSeeder.SeedFromCsv<RolePermission>(System.IO.Path.Combine("Data", "DataSeeder", "rolePermissions.csv"));
        }
    }
}
