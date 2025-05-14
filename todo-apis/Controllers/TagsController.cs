using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities.Models;
using todo_apis.Models;

namespace todo_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tags/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Entities.Models.TagDto>> GetTag(long id)
        {
            var tag = new TagDto();
            var tag_found = await _context.tags.FindAsync(id);

            if (tag_found == null)
            {
                return NotFound();
            }

            tag.tag_name = tag_found.tag_name;
            tag.tag_id = tag_found.tag_id;
            tag.client_user = tag_found.client_user;

            return tag;
        }

        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsFromClient(string username)
        {
            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }
            var tags = await _context.tags.Where(tag => tag.client_user == client.username).ToListAsync();
            if (tags == null)
            {
                return BadRequest("Tags Not Found");
            }
            return Ok(tags);
        }

        // POST: api/Tags
        [HttpPost]
        public async Task<ActionResult<Entities.Models.TagDto>> PostTag([FromBody] Models.Tag newTag, string username)
        {
            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }
            Tag tag = new Tag(newTag.tag_name, client.username);
            _context.tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.tag_id }, tag);
        }

        private bool TagExists(long id)
        {
            return _context.tags.Any(e => e.tag_id == id);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnly()
        {
            return Ok("You are Authenticated");
        }
    }
}
