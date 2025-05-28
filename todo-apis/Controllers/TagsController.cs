using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Models;

namespace todo_apis.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        // HTTP GETS -------

        /// <summary>
        /// Gets a specific tag with the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Ok status with the tag or NotFound
        /// </returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _context.tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        /// <summary>
        /// Gets all the tags from a user
        /// The client's user is obtained from the http request (name)
        /// </summary>
        /// <returns>
        /// Ok status with the tags or BadRequest or Unauthorized
        /// </returns>
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsClient()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tags = await _context.tags
                .Where(tag => tag.client_user == username)
                .ToListAsync();

            if (tags == null) { 
                return BadRequest("Tags Not Found"); 
            }

            return Ok(tags);
        }

        // HTTP POST -------

        /// <summary>
        /// Adds a tag to the db
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>
        /// Ok status with the tag or Conflict or Unauthorized
        /// </returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(TagDto tag)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }
            var tag_db = new Tag(tag.tag_name, username);
            _context.tags.Add(tag_db);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TagExists(tag_db.tag_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(tag_db);
        }

        // METHODS -------

        private bool TagExists(int tag_id)
        {
            return _context.tags.Any(tag => tag.tag_id == tag_id);
        }
    }
}