using System.Collections;
using todo_apis.Models;

namespace todo_apis.Utils.Comparers
{
    public class TagComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            if (x.tag_id == y.tag_id && x.tag_name == y.tag_name && x.client_user == y.client_user)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Tag obj)
        {
            return obj.tag_id.GetHashCode();
        }
    }
}
