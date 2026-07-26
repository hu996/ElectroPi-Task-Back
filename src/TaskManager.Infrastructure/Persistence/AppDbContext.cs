using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(project => project.Name).HasMaxLength(100).IsRequired();
            entity.Property(project => project.Description).HasMaxLength(500);
            entity.Property(project => project.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(project => project.Name).IsUnique();

            entity.HasMany(project => project.Tasks)
                .WithOne(task => task.Project)
                .HasForeignKey(task => task.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(task => task.Title).HasMaxLength(150).IsRequired();
            entity.Property(task => task.Description).HasMaxLength(1000);
            entity.Property(task => task.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(task => new { task.ProjectId, task.Title })
                .IsUnique()
                .HasFilter("[Status] <> 'Done'");
        });
    }
}



