using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_apis.Context;
using todo_apis.Models;

namespace todo_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoTasks : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoTasks(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MoTask>>> GetTasks()
        {
            return await _context.tasks.ToListAsync();
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MoTask>> GetTasks(long id)
        {
            var tasks = await _context.tasks.FindAsync(id);

            if (tasks == null)
            {
                return NotFound();
            }

            return tasks;
        }

        // PUT: api/tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasks(long id, MoTask tasks)
        {
            if (id != tasks.task_id)
            {
                return BadRequest();
            }

            _context.Entry(tasks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tasksExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MoTask>> Posttasks(MoTask tasks)
        {
            _context.tasks.Add(tasks);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTasks), new { id = tasks.task_id }, tasks);
            //return CreatedAtAction("Gettasks", new { id = tasks.Id }, tasks);
        }

 

        private bool tasksExists(long id)
        {
            return _context.tasks.Any(e => e.task_id == id);
        }
    }
}
