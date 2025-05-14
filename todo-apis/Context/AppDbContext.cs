using Microsoft.EntityFrameworkCore;
using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Models.CustomTask> tasks { get; set; } = default!;
        public DbSet<Client> clients { get; set; } = default!;
        public DbSet<Tag> tags { get; set; } = default!;

        public DbSet<Category> categories { get; set; } = default!;

    }
}
