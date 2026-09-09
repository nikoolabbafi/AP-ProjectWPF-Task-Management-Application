namespace WebApplication1.Model.DTO
{
    public class UpdateTaskDto
    {
        public string? Name { get; set; }
        public bool Checked { get; set; }
        //public bool Starred { get; set; }
        public int ListId { get; set; }
        public string? DueDate { get; set; }
    }
}
