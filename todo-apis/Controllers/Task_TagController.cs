using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities;
using todo_apis.Models;
using todo_apis.Utils.Comparers;

namespace todo_apis.Controllers
{
    [Route("api/tasks-tag")]
    [ApiController]
    public class Task_TagController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Task_TagController(AppDbContext context)
        {
            _context = context;
        }

        // HTTP GETS -------

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTasksTags(int id_task)
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

        [Authorize]
        [HttpGet("task/excluded-tags")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsNotInTask(int id_task)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tags = await _context.task_tag
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.tags.Where(tag => tag.client_user == username),
                    ts => ts.tag_id,
                    tg => tg.tag_id,
                    (ts, tg) => tg
                )
                .ToListAsync();

            if (tags == null)
            {
                return BadRequest("Tags Not Found");
            }

            var allTags = await _context.tags
                .Where(tag => tag.client_user == username)
                .ToListAsync();

            var tags_not_in = allTags.Except(tags, new TagComparer()).ToList();

            if (tags_not_in == null)
            {
                return BadRequest("Tags Not Found");
            }

            return Ok(tags_not_in);
        }

        [Authorize]
        [HttpGet("task/included-tags")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsInTask(int id_task)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tags = await _context.task_tag
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.tags.Where(tag => tag.client_user == username),
                    ts => ts.tag_id,
                    tg => tg.tag_id,
                    (ts, tg) => tg
                )
                .ToListAsync();

            if (tags == null)
            {
                return BadRequest("Tags Not Found");
            }

            return Ok(tags);
        }

        // HTTP POSTS -------

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Task_Tag>> PostTask_Tag(Task_Tag task_Tag)
        {
            if (_context.task_tag.Any(tt => tt.task_id == task_Tag.task_id && tt.tag_id == task_Tag.tag_id))
            {
                return Conflict(new { message = "It already exists" });
            }

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