using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entities.Models.TagDto>>> GetTag()
        {
            return await _context.tags.ToListAsync();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entities.Models.TagDto>> GetTag(long id)
        {
            var tag = await _context.tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // POST: api/Tags
        [HttpPost]
        public async Task<ActionResult<Entities.Models.TagDto>> PostTag([FromBody] Models.Tag newTag)
        {
            Entities.Models.TagDto tag = new Entities.Models.TagDto(newTag.tag_name);
            _context.tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.tag_id }, tag);
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var tag = await _context.tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagExists(long id)
        {
            return _context.tags.Any(e => e.tag_id == id);
        }
    }
}
