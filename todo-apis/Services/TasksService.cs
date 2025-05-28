/* NOT USED */

using todo_apis.Context;
using todo_apis.Models;
using todo_apis.Services.Interfaces;

namespace todo_apis.Services
{
    public class TasksService(AppDbContext context, IConfiguration configuration) : ITasksService
    {
        public Task<CustomTask> EditTask(CustomTask edited_Task)
        {
            throw new NotImplementedException();
        }

        public async Task<Client?> FindClient(string username)
        {
            var client = await context.clients.FindAsync(username);
            if (client == null)
            {
                return null;
            }
            return client;
        }
    }
}
