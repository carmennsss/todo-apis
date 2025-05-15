using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities;
using todo_apis.Models;

namespace todo_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Task_TagController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Task_TagController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("task/{id_task}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTasksTags(int id_task)
        {
            var tags = await _context.Task_Tag
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.tags,
                    ts => ts.task_id,
                    tg => tg.tag_id,
                    (ts, tg) => tg
                )
                .ToListAsync();

            if (tags == null)
            {
                return BadRequest("SubTasks Not Found");
            }

            return Ok(tags);
        }

        [HttpGet("notask/{id_task}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsNotInTask(int id_task)
        {
            var tags_task = await _context.tags
                .Join(
                    _context.Task_Tag,
                    tg => tg.tag_id,
                    ts => ts.task_id,
                    (ts, tg) => tg
                )
                .ToListAsync();

            if (tags_task == null)
            {
                return BadRequest("SubTasks Not Found");
            }

            var tags = await _context.tags.Where(tag => !tags_task.Any(tag_task => tag_task.tag_id == tag.tag_id)).ToListAsync();

            if (tags == null)
            {
                return BadRequest("Tags Not Found");
            }

            return Ok(tags);
        }

        // POST: api/Task_Tag
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Task_Tag>> PostTask_Tag(Task_Tag task_Tag)
        {
            _context.Task_Tag.Add(task_Tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Task_TagExists(task_Tag.tag_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        private bool Task_TagExists(int id)
        {
            return _context.Task_Tag.Any(e => e.tag_id == id);
        }
    }
}