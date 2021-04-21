using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALServer
{
    [Serializable]
    public class Group
    {
        public int Id { get; set; }
        public string NameGroup { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public Group()
        {
            Users = new List<User>();
            Tests = new List<Test>();
        }
        public override string ToString()
        {
            return NameGroup;
        }
    }
}
