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

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag_found = await _context.tags.FindAsync(id);

            if (tag_found == null)
            {
                return NotFound();
            }
            return tag_found;
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsFromClient(string username)
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

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnly()
        {
            return Ok("You are Authenticated");
        }

        // HTTP POSTS -------

        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag([FromBody] Models.Tag tag, string username)
        {
            var client = await _context.clients.FindAsync(username);
            if (client == null)
            {
                return BadRequest("User Not Found");
            }
            _context.tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.tag_id }, tag);
        }
    }
}
