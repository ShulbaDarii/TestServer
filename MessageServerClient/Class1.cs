using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageServerClient
{
    [Serializable]
    public class Mess
    {
        public string type { get; set; }
        public int id { get; set; }
        public Byte[] info { get; set; }

    }
}
