using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALServer
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public User()
        {
            Groups = new List<Group>();
        }
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
