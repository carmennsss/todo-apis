using Microsoft.AspNetCore.Authorization;
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

        //HTTP GETS -------

        /// <summary>
        /// Gets the categories from a client
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <returns>
        /// Ok status and the categories or BadRequest or Unauthorized
        /// </returns>
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesClient()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var categories = await _context.categories.Where(category => category.client_user == username).ToListAsync();
            if (categories == null)
            {
                return BadRequest("Categories Not Found");
            }

            return Ok(categories);
        }

        /// <summary>
        /// Gets a specific category
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The category or NotFound status
        /// </returns>
        [Authorize]
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

        /// <summary>
        /// Adds a category to the db
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <param name="category"></param>
        /// <returns>
        /// Ok status with the category or Conflict or Unauthorized
        /// </returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryDto category)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }
            var category_db = new Category(category.category_name, username);
            _context.categories.Add(category_db);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (CategoryExists(category_db.category_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(category_db);
        }

        // METHODS -------

        public bool CategoryExists(int id)
        {
            return _context.categories.Any(c => c.category_id == id);
        }
    }
}
