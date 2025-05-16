using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Entities;

namespace todo_apis.Controllers
{
    [Route("api/tasks-subtasks")]
    [ApiController]
    public class Tasks_SubTasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Tasks_SubTasksController(AppDbContext context)
        {
            _context = context;
        }

        // HTTP GETS -------

        [HttpGet("subtask/{id}")]
        public async Task<ActionResult<SubTask>> GetSubTask(int id_task, int id_subtask)
        {
            var subTask_link = await _context.task_subtask
                .Where(subtask => subtask.subtask_id == id_subtask && subtask.task_id == id_task)
                .FirstOrDefaultAsync();

            if (subTask_link == null)
            {
                return BadRequest("SubTask Not Found");
            }

            var subTask = await _context.subtasks.FindAsync(subTask_link.subtask_id);
            if (subTask == null)
            {
                return BadRequest("SubTask Not Found");
            }

            return subTask;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubTask>>> GetSubTasks(int id_task)
        {
            var subTasks = await _context.task_subtask
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.subtasks,
                    ts => ts.subtask_id,
                    st => st.subtask_id,
                    (ts, st) => st
                )
                .ToListAsync();

            if (subTasks == null)
            {
                return BadRequest("SubTasks Not Found");
            }

            return Ok(subTasks);
        }

        // HTTP POSTS -------

        [HttpPost("subtask")]
        public async Task<ActionResult<SubTask>> PostSubTasking(SubTask subTask)
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


        [HttpPost]
        public async Task<ActionResult<Task_SubTask>> PostSubTask_Task(Task_SubTask subTask)
        {
            _context.task_subtask.Add(subTask);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubTask_Task_Exists(subTask.subtask_id))
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

        private bool SubTask_Task_Exists(int id)
        {
            return _context.task_subtask.Any(e => e.subtask_id == id);
        }

        private bool SubTask_Exists(int id)
        {
            return _context.subtasks.Any(e => e.subtask_id == id);
        }
    }
}