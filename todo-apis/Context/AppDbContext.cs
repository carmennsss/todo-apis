using Microsoft.EntityFrameworkCore;
using todo_apis.Entities;
using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task_SubTask>().HasKey(x => new
            {
                x.subtask_id,
                x.task_id,
            });

            modelBuilder.Entity<Task_Tag>().HasKey(x => new
            {
                x.tag_id,
                x.task_id,
            });
        }
        public DbSet<CustomTask> tasks { get; set; } = default!;
        public DbSet<Client> clients { get; set; } = default!;
        public DbSet<Tag> tags { get; set; } = default!;

        public DbSet<Category> categories { get; set; } = default!;

        public DbSet<SubTask> subtasks { get; set; } = default!;
        public DbSet<Task_SubTask> task_subtask { get; set; } = default!;
        public DbSet<Task_Tag> task_tag { get; set; } = default!;

    }
}
