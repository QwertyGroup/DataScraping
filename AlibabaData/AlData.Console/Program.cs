using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace AlData.C
{
    class Program
    {
        private static TimeSpan _totalTime = new TimeSpan();
        private static int _counter;

        static void Main(string[] args)
        {
            DownloadPages();
            Console.ReadLine();
        }

        private static void DownloadPages()
        {
            for (int i = 1; i <= 57; i++)
            {
                DownPage(i);
            }
        }

        private static async void DownPage(int i)
        {
            Console.WriteLine(i + " started!");
            var sw = new Stopwatch();
            sw.Start();
            //var rq = WebRequest.Create($"https://www.alibaba.com/corporations/agriculture_tyres/--CN------------------50/{i}.html");
            var rq = WebRequest.Create($"https://www.alibaba.com/corporations/agriculture_tyres/{i}.html");
            var rp = await rq.GetResponseAsync();
            sw.Stop();
            _totalTime = _totalTime.Add(sw.Elapsed);
            Console.WriteLine(sw.Elapsed);

            //var stream = rp.GetResponseStream();
            //var str = new StreamReader(stream).ReadToEnd();

            _counter++;
            Console.WriteLine("Counter: " + _counter);
            Console.WriteLine($"Page {i} downloaded!");

            //File.WriteAllText($"Test {i}.txt", str);

            if (i == 20) Console.WriteLine("Total time: " + _totalTime);
        }
    }
}
