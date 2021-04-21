using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALServer
{
    [Serializable]
    public class UserAnswer
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Answer Answer { get; set; }
        public DateTime Date { get; set; }
    }
}
