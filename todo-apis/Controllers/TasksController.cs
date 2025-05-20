using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Models;
using todo_apis.Services;
using todo_apis.Services.Interfaces;
using YamlDotNet.Core.Tokens;

namespace todo_apis.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class Tasks : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITasksService tasksService;

        public Tasks(AppDbContext context, ITasksService tasksService)
        {
            _context = context;
            this.tasksService = tasksService;
        }



        // HTTP GETS -------

        [Authorize]
        [HttpGet("user/status/{status-name}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksStatus(string state_name)
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username && task.state_name == state_name).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user/category/{id-category}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksCategory(int category_id)
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var category = await _context.categories.FindAsync(category_id);
            if (category == null)
            {
                return BadRequest("Category Not Found");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username && task.list_id == category.category_id).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }

            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksClient()
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("user/date/{date}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksDate(string date)
        {
            var parsedDate = DateTime.Parse(HttpUtility.UrlDecode(date));
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var tasks = await _context.tasks.Where(task => task.client_user == username && task.task_due_date.Date == parsedDate.Date).ToListAsync();
            if (tasks == null)
            {
                return BadRequest("Tasks Not Found");
            }
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("{id-task}")]
        public async Task<ActionResult<TaskDto>> GetTask(int task_id)
        {
            var task = await _context.tasks.FindAsync(task_id);
            if (task == null)
            {
                return BadRequest("Task Not Found");
            }
            return Ok(task);
        }

        // HTTP POSTS -------

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TaskDto>> PostTasks(CustomTask task)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized("User Unauthorized");
            }
            task.client_user = username;
            _context.tasks.Add(task);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task.task_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(task);
        }

        [Authorize]
        [HttpPost("edit/{id-task}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> EditTask(int task_id, TaskDto edited_Task)
        {
            var task = await _context.tasks.FindAsync(task_id);
            if (task == null)
            {
                return BadRequest("Task Not Found");
            }
            if (edited_Task.task_name != "")
            {
                task.task_name = edited_Task.task_name;
            }
            if (edited_Task.task_desc != "")
            {
                task.task_desc = edited_Task.task_desc;
            }
            if (edited_Task.list_id != 0)
            {
                var list = _context.categories.Where(category => category.category_id == edited_Task.list_id).FirstOrDefault();
                if (list == null)
                {
                    return BadRequest("Category Not Found");
                }
                task.list_id = edited_Task.list_id;
            }
            var tasks_db = _context.tasks.Where(task => task.task_id == task.task_id).ToList();
            foreach (var task_db in tasks_db)
            {
                task_db.task_name = task.task_name;
                task_db.task_desc = task.task_desc;
                task_db.list_id = task.list_id;
            }
            _context.SaveChanges();
            return Ok();
        }

        // HTTP DELETES -------

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // METHODS -------

        private bool TaskExists(int task_id)
        {
            return _context.tasks.Any(task => task.task_id == task_id);
        }
    }
}