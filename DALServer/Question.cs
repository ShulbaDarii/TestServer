using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALServer
{
    [Serializable]
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Difficulty { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public Question()
        {
            Answers = new List<Answer>();
        }
    }
}
