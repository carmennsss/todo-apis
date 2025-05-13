using Microsoft.EntityFrameworkCore;
using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Models.MoTask> tasks { get; set; }
        public DbSet<Client> clients { get; set; } = default!;
        public DbSet<TagDto> tags { get; set; } = default!;

    }
}
