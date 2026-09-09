//namespace WebApplication1.Controllers
//{
//    public class ListController
//    {
//    }
//}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.DTO;



namespace WebApplication1.Controllers
{


    [Route("api/lists")]
    [ApiController]
    public class ListController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public ListController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetListDto>>> getlists()
        {
            

            // Approach 3 = mapper
            var lists = await _db.Lists.ToListAsync();
            return Ok(_mapper.Map<List<GetListDto>>(lists));
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
        public async Task<ActionResult<CreateListDto>> CreateList([FromBody] CreateListDto list)
        {
            

            // approach 3 = mapper
            var l = _mapper.Map<List>(list);
            await _db.Lists.AddAsync(l);
            await _db.SaveChangesAsync();
            return Ok(l);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<List>> UpdateList([FromBody] UpdateListDto listdto, int id)
        {
            

            // approach 3 = mapper
            var list = await _db.Lists.FirstOrDefaultAsync(b => b.Id == id);
            var updated_list = _mapper.Map(listdto, list);
            _db.Lists.Update(updated_list);
            await _db.SaveChangesAsync();
            return Ok(list);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<List>> DeleteList(int id)
        {
            
            // approach 3 = mapper
            var list = await _db.Lists.FirstOrDefaultAsync(b => b.Id == id);
            _db.Lists.Remove(list);
            _db.SaveChanges();
            return Ok(list);

        }
    }
}
