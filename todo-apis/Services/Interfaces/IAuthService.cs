using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<Client?> RegisterAsync(ClientDto request);
        public Task<string?> LoginAsync(ClientDto request);
    }
}
