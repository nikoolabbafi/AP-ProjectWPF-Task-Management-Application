using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Checked { get; set; }
        public bool Starred { get; set; }
        public int ListId { get; set; }
        public string? DueDate { get; set; }
            //public DateTime DueDate { get; set; }
     }
   
}
