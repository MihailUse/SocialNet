using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Follower> Followers => Set<Follower>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostFile> PostFiles => Set<PostFile>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<Attach> Attaches => Set<Attach>();

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        #region override SaveChanges
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("API"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureSoftDeleteFilter(modelBuilder);

            modelBuilder.Entity<Follower>()
                .HasKey(x => new { x.FollewerId, x.FollowingId });
            modelBuilder.Entity<PostLike>()
                .HasKey(x => new { x.PostId, x.UserId });

            modelBuilder.Entity<Avatar>().ToTable(nameof(Avatars));
            modelBuilder.Entity<PostFile>().ToTable(nameof(PostFiles));
        }

        private void ConfigureSoftDeleteFilter(ModelBuilder builder)
        {
            var softDeletableTypes = builder.Model.FindLeastDerivedEntityTypes(typeof(Timestamp));

            foreach (var softDeletableType in softDeletableTypes)
            {
                var parameter = Expression.Parameter(softDeletableType.ClrType, "parameter");
                var deletableProperty = Expression.Property(parameter, nameof(Timestamp.DeletedAt));
                var expression = Expression.Equal(deletableProperty, Expression.Constant(null));

                var queryFilter = Expression.Lambda(expression, parameter);
                softDeletableType.SetQueryFilter(queryFilter);
            }
        }

        private void AddTimestamps()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            foreach (var entity in ChangeTracker.Entries<Timestamp>())
            {
                switch (entity.State)
                {
                    case EntityState.Modified:
                        entity.Property(nameof(Timestamp.UpdatedAt)).CurrentValue = now;
                        break;

                    case EntityState.Added:
                        entity.Property(nameof(Timestamp.CreatedAt)).CurrentValue = now;
                        break;

                    case EntityState.Deleted:
                        entity.State = EntityState.Unchanged;
                        entity.Property(nameof(Timestamp.DeletedAt)).CurrentValue = now;
                        break;
                }
            }
        }
    }
}