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
    [Route("api/[controller]")]
    [ApiController]
    public class Tasks_SubTasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Tasks_SubTasksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("subtask/{id}")]
        public async Task<ActionResult<SubTask>> GetSubTask(int id_task, int id_subtask)
        {
            var subTask_link = await _context.tasks_subtask
                .Where(subtask => subtask.subtask_id == id_subtask && subtask.task_id == id_task)
                .FirstOrDefaultAsync();

            if (subTask_link == null)
            {
                return BadRequest("SubTask Not Found");
            }

            var subTask = await _context.subTasks.FindAsync(subTask_link.subtask_id);
            if (subTask == null)
            {
                return BadRequest("SubTask Not Found");
            }

            return subTask;
        }

        [HttpGet("{id_task}")]
        public async Task<ActionResult<IEnumerable<SubTask>>> GetSubTasks(int id_task)
        {
            var subTasks = await _context.tasks_subtask
                .Where(ts => ts.task_id == id_task)
                .Join(
                    _context.subTasks,
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


        [HttpPost]
        public async Task<ActionResult<Task_SubTask>> PostSubTasking(Task_SubTask subTask)
        {
            _context.tasks_subtask.Add(subTask);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubTaskExists(subTask.subtask_id))
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

        private bool SubTaskExists(int id)
        {
            return _context.tasks_subtask.Any(e => e.subtask_id == id);
        }
    }
}