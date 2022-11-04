﻿using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Follower> Followers => Set<Follower>();

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
        }

        private void ConfigureSoftDeleteFilter(ModelBuilder builder)
        {
            var softDeletableTypes = builder.Model.FindEntityTypes(typeof(Timestamp));

            foreach (var softDeletableType in softDeletableTypes)
            {
                var parameter = Expression.Parameter(softDeletableType.ClrType);
                var deletableProperty = Expression.Property(parameter, nameof(Timestamp.DeletedAt));
                var nullableProperty = Expression.Constant(null);

                var queryFilter = Expression.Lambda(Expression.Equal(deletableProperty, nullableProperty));
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
                        entity.Property(nameof(Timestamp.UpdatedAt)).CurrentValue = now;
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