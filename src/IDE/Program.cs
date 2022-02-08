using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE
{
    static class Program
    {

        [STAThread]
        public static void Main()
        {
            var app = new MainApp();
            app.Run();
        }
    }
}
