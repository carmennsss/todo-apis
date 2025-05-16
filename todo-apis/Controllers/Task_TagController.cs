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

        // HTTP GETS -------

        [HttpGet("task/{id_task}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTasksTags(int id_task)
        {
            var tags = await _context.task_tag
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.tags,
                    ts => ts.tag_id,
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

        [HttpGet("tasks_not_in/{id_task}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsNotInTask(int id_task)
        {
            var tags = await _context.task_tag
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.tags,
                    ts => ts.tag_id,
                    tg => tg.tag_id,
                    (ts, tg) => tg
                )
                .ToListAsync();

            if (tags == null)
            {
                return BadRequest("Tags Not Found");
            }
            var allTags = await _context.tags.ToListAsync();
            var tags_not_in = allTags.Except(tags, new TagComparer()).ToList();

            if (tags_not_in == null)
            {
                return BadRequest("Tags Not Found");
            }

            return Ok(tags_not_in);
        }

        // HTTP POSTS -------

        [HttpPost]
        public async Task<ActionResult<Task_Tag>> PostTask_Tag(Task_Tag task_Tag)
        {
            _context.task_tag.Add(task_Tag);
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

            return Ok(task_Tag);
        }

        // METHODS -------

        private bool Task_TagExists(int id)
        {
            return _context.task_tag.Any(e => e.tag_id == id);
        }
    }
}