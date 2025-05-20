using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class CategoryDto
    {
        [Key]
        public int category_id { get; set; }

        public string category_name { get; set; }

        public CategoryDto(string category_name) {
            this.category_name = category_name;
        }
    }
}
