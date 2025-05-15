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

        // GET: api/SubTaskings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubTask>> GetSubTasks(int id_task, int id_subtask)
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


        // POST: api/SubTaskings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Task_SubTask>> PostSubTasking(Task_SubTask subTasking)
        {
            _context.tasks_subtask.Add(subTasking);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubTaskingExists(subTasking.subtask_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubTasking", new { id = subTasking.subtask_id }, subTasking);
        }

        private bool SubTaskingExists(int id)
        {
            return _context.tasks_subtask.Any(e => e.subtask_id == id);
        }
    }
}