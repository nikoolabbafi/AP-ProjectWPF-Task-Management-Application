using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.DTO;
using Microsoft.Extensions.Logging;



namespace WebApplication1.Controllers
{


    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskController> _logger;
        public TaskController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetTaskDto>>> gettasks()
        {
            

            // Approach 3 = mapper
            var tasks = await _db.Tasks.ToListAsync();
            return Ok(_mapper.Map<List<GetTaskDto>>(tasks));
        }



        [HttpGet("filterlistid")]
        public async Task<ActionResult<IEnumerable<GetTaskDto>>> FilterListid([FromQuery] int listid)
        {
            var tasks = await _db.Tasks.Where(b => b.ListId == listid).ToListAsync();
            return Ok(_mapper.Map<List<GetTaskDto>>(tasks));
        }




        [HttpGet("filtercheck")]
        public async Task<ActionResult<IEnumerable<GetTaskDto>>> FilterTaskCheck([FromQuery] bool ischeck)
        {
            var tasks = await _db.Tasks.Where(b => b.Checked == ischeck).ToListAsync();
            return Ok(_mapper.Map<List<GetTaskDto>>(tasks));
        }

        [HttpGet("filterstarred")]
        public async Task<ActionResult<IEnumerable<GetTaskDto>>> FilterTaskStarred([FromQuery] bool isstarred)
        {
            var tasks = await _db.Tasks.Where(b => b.Starred == isstarred).ToListAsync();
            return Ok(_mapper.Map<List<GetTaskDto>>(tasks));
        }

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<Chef>> getOneChef(int id)
        //{
        //    // Approach 1
        //    //var chef = _db.Chefs.FirstOrDefault(b => b.Id == id);
        //    //return Ok(chef);

        //    // Approach 2 = DTO
        //    // var chef = _db.Chefs.FirstOrDefault(b => b.Id == id);
        //    // var chef_dto = new GetChefDto
        //    // {
        //    //     Name = chef.Name,
        //    //     PhoneNumber = chef.PhoneNumber,
        //    // };

        //    //return Ok(chef_dto);

        //    // Approach 3 = mapper
        //    Chef chef = await _db.Chefs.FirstOrDefaultAsync(b => b.Id == id);
        //    return Ok(_mapper.Map<GetChefDto>(chef));
        //}

        [HttpPost]
        public async Task<ActionResult<CreateTaskDto>> CreateTask([FromBody] CreateTaskDto task)
        {
            

            // approach 3 = mapper
            var t = _mapper.Map<Model.Task>(task);
            await _db.Tasks.AddAsync(t);
            await _db.SaveChangesAsync();
            return Ok(t);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Model.Task>> UpdateTask([FromBody] UpdateTaskDto taskdto, int id)
        {
            

            // approach 3 = mapper
            var task = await _db.Tasks.FirstOrDefaultAsync(b => b.Id == id);
            var updated_task = _mapper.Map(taskdto, task);
            _db.Tasks.Update(updated_task);
            await _db.SaveChangesAsync();
            return Ok(task);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Model.Task>> DeleteTask(int id)
        {
           

            // approach 3 = mapper
            var task = await _db.Tasks.FirstOrDefaultAsync(b => b.Id == id);
            _db.Tasks.Remove(task);
            _db.SaveChanges();
            return Ok(task);
            //try
            //{
            //    var task = await _db.Tasks.FirstOrDefaultAsync(b => b.Id == id);
            //    if (task == null)
            //    {
            //        _logger.LogWarning($"Task with id {id} not found.");
            //        return NotFound();
            //    }

            //    _db.Tasks.Remove(task);
            //    await _db.SaveChangesAsync();

            //    _logger.LogInformation($"Task with id {id} successfully deleted.");
            //    return Ok(task);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Error deleting task with id {id}: {ex.Message}");
            //    return StatusCode(500, "Internal server error");
            //}

        }
    }
}
