using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        private readonly  IHttpContextAccessor _contextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userIdClaim = _contextAccessor.HttpContext?
       .User?
       .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                var userId = int.Parse(userIdClaim.Value);

                foreach (var entry in ChangeTracker.Entries<TaskItem>())
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.UserId = userId;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
