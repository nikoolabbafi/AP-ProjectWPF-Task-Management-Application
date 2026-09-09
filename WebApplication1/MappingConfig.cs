using AutoMapper;
using WebApplication1.Model;
using WebApplication1.Model.DTO;
//using WebApplication1.Model.DTO;

namespace WebApplication1
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Model.Task, GetTaskDto>().ReverseMap();
            CreateMap<Model.Task, CreateTaskDto>().ReverseMap();
            CreateMap<Model.Task, UpdateTaskDto>().ReverseMap();

            CreateMap<List, GetListDto>().ReverseMap();
            CreateMap<List, CreateListDto>().ReverseMap();
            CreateMap<List, UpdateListDto>().ReverseMap();


        }
    }
}

