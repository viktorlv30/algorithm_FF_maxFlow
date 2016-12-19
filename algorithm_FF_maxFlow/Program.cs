using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFlowExample;

namespace algorithm_FF_maxFlow
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Matrix matrix = new Matrix();
            //matrix.Run();

            new FordFulkerson().Run();
            FordFulkerson.PrintLn("Press key to exit ...");
            Console.ReadLine();
        }
    }
}
