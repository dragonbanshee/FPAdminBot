using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPAdminBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection conn = new Connection();
            conn.Connect();
        }
    }
}
