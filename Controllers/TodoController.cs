using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public TodoController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var items = await _context.Items.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetItem(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemData data)
        {
            if (ModelState.IsValid)
            {
                await _context.Items.AddAsync(data);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetItem", new {data.Id}, data);
            }
            
            return new JsonResult("Something went wrong") { StatusCode=500 };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(int id, ItemData data)
        {
            if (id != data.Id)
            {
                return BadRequest();
            }
                
            var existsItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (existsItem == null)
            {
                return NotFound();
            }

            existsItem.Title = data.Title;
            existsItem.Description = data.Description;
            existsItem.Done = data.Done;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            var existsItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (existsItem == null)
            {
                return NotFound();
            }

            _context.Items.Remove(existsItem);
            await _context.SaveChangesAsync();

            return Ok(existsItem);

        }

    }
}