using LaundryCleaning.Service.Common.Domain;
using LaundryCleaning.Service.Common.Models.Builder;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Security.Roles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace LaundryCleaning.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Entity
        public DbSet<SystemPublisher> _publisher => Set<SystemPublisher>();
        public DbSet<SystemReceived> _received => Set<SystemReceived>();

        public DbSet<Identity> Identities => Set<Identity>();
        public DbSet<InvoiceNumberTracker> InvoiceNumberTrackers => Set<InvoiceNumberTracker>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<User> Users => Set<User>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            #region Entity Builder
            new SystemPublisherBuilder(this).Configure(modelBuilder.Entity<SystemPublisher>());
            new SystemReceivedBuilder(this).Configure(modelBuilder.Entity<SystemReceived>());

            new IdentityBuilder(this).Configure(modelBuilder.Entity<Identity>());
            new InvoiceNumberTrackerBuilder(this).Configure(modelBuilder.Entity<InvoiceNumberTracker>());
            new RoleBuilder(this).Configure(modelBuilder.Entity<Role>());
            new RolePermissionBuilder(this).Configure(modelBuilder.Entity<RolePermission>());
            new UserBuilder(this).Configure(modelBuilder.Entity<User>());
            #endregion

            //GlobalQueryFilters.Apply(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<EntityBase>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTime.Now; 
            var userId = GetCurrentUserId();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = now;
                    entry.Entity.CreatedBy = userId;
                }

                entry.Entity.Modified = now;
                entry.Entity.ModifiedBy = userId;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private Guid GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return Guid.Empty;

            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");

            return Guid.TryParse(userIdClaim?.Value, out var userId) ? userId : Guid.Empty;
        }
    }
}
