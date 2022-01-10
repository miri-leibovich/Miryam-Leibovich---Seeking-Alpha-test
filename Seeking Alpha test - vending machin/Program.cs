using System;
using System.IO;
using Newtonsoft.Json;

namespace Seeking_Alpha_test___vending_machin
{
    class Program
    {
        static void Main(string[] args)
        {

            VendingMachine machine = new VendingMachine();
            machine.On();
            Console.ReadKey();
        }
    }
}
