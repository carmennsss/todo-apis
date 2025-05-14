using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities.Models
{
    public class TagDto
    {
        [Key]
        public int tag_id { get; set; }

        public string tag_name { get; set; }

        public string client_user { get; set; }
    }
}
