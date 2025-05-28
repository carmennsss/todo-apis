using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities;

namespace todo_apis.Controllers
{
    [Route("api/subtasks")]
    [ApiController]
    public class Tasks_SubTasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Tasks_SubTasksController(AppDbContext context)
        {
            _context = context;
        }

        // HTTP GETS -------

        /// <summary>
        /// Gets a specific subtask
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The subtask or BadRequest
        /// </returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Task_SubTask>> GetSubTask(int id)
        {
            var subTask = await _context.subtasks.FindAsync(id);

            if (subTask == null)
            {
                return BadRequest("SubTask Not Found");
            }

            return subTask;
        }

        /// <summary>
        /// Gets all the subtasks of a task
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Ok status with the subtasks or BadRequest
        /// </returns>
        [Authorize]
        [HttpGet("task/{id}")]
        public async Task<ActionResult<IEnumerable<Task_SubTask>>> GetSubTasks(int id)
        {
            var subTasks = await _context.subtasks
                .Where(subtask => subtask.task_id == id)
                .ToListAsync();

            if (subTasks == null)
            {
                return BadRequest("SubTasks Not Found");
            }

            return Ok(subTasks);
        }

        // HTTP POSTS -------

        /// <summary>
        /// Creates a subtask a adds it to the db
        /// </summary>
        /// <param name="subTask"></param>
        /// <returns>
        /// Ok status with the subtask or Conflict
        /// </returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Task_SubTask>> PostSubTask(Task_SubTask subTask)
        {
            _context.subtasks.Add(subTask);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubTask_Exists(subTask.subtask_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(subTask);
        }

        // METHODS -------

        private bool SubTask_Exists(int id)
        {
            return _context.subtasks.Any(e => e.subtask_id == id);
        }
    }
}