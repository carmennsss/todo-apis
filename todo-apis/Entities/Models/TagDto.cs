using System.ComponentModel.DataAnnotations;

namespace todo_apis.Models
{
    public class TagDto
    {
        [Key]
        public int tag_id { get; set; }
        public string tag_name { get; set; }

        public TagDto(string tag_name)
        {
            this.tag_name = tag_name;
        }

        public TagDto() { }
    }
}
