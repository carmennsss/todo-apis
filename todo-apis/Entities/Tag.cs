namespace todo_apis.Models
{
    public class Tag
    {
        public int tag_id { get; set; }
        public string tag_name { get; set; }

        public string client_user { get; set; }

        public Tag(string tag_name, string client_user)
        {
            this.client_user = client_user;
            this.tag_name = tag_name;
        }

        public Tag() { }
    }
}
