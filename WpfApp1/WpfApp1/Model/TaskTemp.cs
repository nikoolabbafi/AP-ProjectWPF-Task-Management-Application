using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;

namespace WpfApp1.Model
{
    public class TaskTemp
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("checked")]
        public bool Checked { get; set; }
        [JsonPropertyName("starred")]
        public bool Starred { get; set; }
        [JsonPropertyName("listId")]
        public int ListId { get; set; }
        [JsonPropertyName("dueDate")]
        public string duedate { get; set; }
        //public DateTime? DueDate { get; set; }
        //public DateTime DueDate { get; set; }
    }
}
