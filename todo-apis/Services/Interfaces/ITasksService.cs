using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Services.Interfaces
{
    public interface ITasksService
    {
        public Task<Client?> FindClient(string username);
        public Task<CustomTask> EditTask(CustomTask edited_Task);
    }
}
