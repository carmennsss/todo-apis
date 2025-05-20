using System.ComponentModel.DataAnnotations;

namespace todo_apis.Entities
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }

        public string category_name { get; set; }

        public string client_user { get; set; }

        public Category(string category_name, string username)
        {
            this.category_name = category_name;
            this.client_user = username;
        }

        public Category() { }
    }
}