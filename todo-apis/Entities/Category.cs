using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }

        public string category_name { get; set; }

        public string client_user {  get; set; }
    }
}
