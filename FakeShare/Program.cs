using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeShare
{
    class Program
    {
        static Mutex singleton = new Mutex(true, "Fake Share");
        static void Main(string[] args)
        {
            if (singleton.WaitOne(TimeSpan.Zero, true))
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
