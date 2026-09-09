namespace WebApplication1.Model.DTO
{
    public class GetTaskDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Checked { get; set; }
        public bool Starred { get; set; }
        public string? DueDate { get; set; }
        public int ListId { get; set; }
    }
}
