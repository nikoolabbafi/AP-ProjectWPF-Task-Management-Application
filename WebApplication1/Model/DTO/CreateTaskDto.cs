namespace WebApplication1.Model.DTO
{
    public class CreateTaskDto
    {
        public string? Name { get; set; }
        public bool Starred { get; set; }
        public bool Checked { get; set; }
        public string? DueDate { get; set; }
    }
}


