using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities;
using todo_apis.Entities.Models;

namespace todo_apis.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // HTTP GETS -------

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesClient(string username)
        {
            var client = await _context.clients.FindAsync(username);
            if (client == null) {
                return BadRequest("User Not Found");
            }

            var categories = await _context.categories.Where(category => category.client_user == client.username).ToListAsync();
            if (categories == null)
            {
                return BadRequest("Categories Not Found");
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // HTTP POST -------

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.categories.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (CategoryExists(category.category_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return  Ok(category);
        }

        // METHODS -------

        public bool CategoryExists(int id)
        {
            return _context.categories.Any(c => c.category_id == id);
        }
    }
}
