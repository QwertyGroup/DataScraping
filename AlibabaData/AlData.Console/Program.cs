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
        private const int MAX_PAGES = 20;
        private const string FILENAME = "Companies";
        private static TimeSpan _totalTime = new TimeSpan();
        private static int _counter;
        private static List<string> _companies = new List<string>();
        private static int _aCounter;

        static void Main(string[] args)
        {
            //DownloadPages();
            //OpenCompanies();
            //ProcessLinks();
            DataFromLinks();
            Console.ReadLine();
        }

        private static async void DataFromLinks()
        {
            var links = File.ReadLines("Links.txt").ToList();
            foreach (var topic in links)
            {
                DownloadPages(topic);
                await Task.Delay(6);
            }
        }

        private static void ProcessLinks()
        {
            var lnks = File.ReadAllText("ALinks.txt");
            var l1 = lnks.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            l1.RemoveAt(0);
            var res = new List<string>();
            foreach (var l2 in l1)
            {
                var l3 = l2.Split(new string[] { "\">" }, StringSplitOptions.RemoveEmptyEntries)[0];
                res.Add(l3);
            }

            File.WriteAllLines("Links.txt", res);
            Console.WriteLine("Done");
        }

        private static void OpenCompanies()
        {
            var text = File.ReadAllLines(FILENAME + ".txt").ToList();
            Console.WriteLine(text.Count());
        }

        private static void DownloadPages(string topic)
        {
            for (int i = 1; i <= MAX_PAGES; i++)
            {
                DownPage(i, topic);
            }
        }

        private static async void DownPage(int i, string topic)
        {
            Console.WriteLine(i + " started!");
            var sw = new Stopwatch();
            sw.Start();
            //                           https://www.alibaba.com/countrysearch/CN/agriculture-tyres-supplier.html
            //                           https://www.alibaba.com/countrysearch/CN/agriculture-tyres-supplier_2.html

            //var rq = WebRequest.Create($"https://www.alibaba.com/corporations/agriculture_tyres/--CN------------------50/{i}.html");

            var url = topic.Remove(topic.IndexOf(".html", 5)) + $"_{i}.html";
            var rq = WebRequest.Create(url);

            //var rq = WebRequest.Create($"https://www.alibaba.com/corporations/agriculture_tyres/{i}.html");

            var rp = await rq.GetResponseAsync();
            sw.Stop();
            _totalTime = _totalTime.Add(sw.Elapsed);
            Console.WriteLine(sw.Elapsed);

            //var stream = rp.GetResponseStream();
            //var str = new StreamReader(stream).ReadToEnd();

            var str = await new StreamReader(rp.GetResponseStream()).ReadToEndAsync();

            try
            {
                // <h2 class="title ellipsis">
                var c0 = str.Split(new string[] { "<h2 class=\"title ellipsis\">" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                c0.RemoveAt(0);
                foreach (var c1 in c0)
                {
                    // href="
                    var c2 = c1.Split(new string[] { "href=\"" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    // title="
                    var c3 = c2.Split(new string[] { "title=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    _companies.Add(c3);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            _counter++;
            Console.WriteLine("Counter: " + _counter);

            if (_counter == MAX_PAGES)
            {
                _aCounter++;
                File.AppendAllLines(FILENAME + _aCounter + ".txt", _companies);
                Console.WriteLine("Total time: " + _totalTime);
                Console.WriteLine("Finished");
            }

            //Console.WriteLine($"Page {i} downloaded!");

            //File.WriteAllText($"Test {i}.txt", str);

            //if (i == 20) Console.WriteLine("Total time: " + _totalTime);
            //if (i == 55) throw new Exception("Test");
        }
    }
}
