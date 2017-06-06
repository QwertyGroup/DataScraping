using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HtmlAgilityTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            ////GetData();

            //GenPageList();

            //GetCLinks();

            LoadFromPage(1);

            Console.ReadLine();
        }

        private static async void LoadFromPage(int page)
        {
            var name_link = File.ReadAllLines($"RawLinks/CLinks{page}.txt").ToList();
            var names = name_link.Select(nl => nl.SplitBy("->")[0].Trim()).ToList();
            var links = name_link.Select(nl => nl.SplitBy("->")[1].Trim()).ToList();

            foreach (var link in links)
            {
                var client = new HttpClient();
                var data = await client.GetStringAsync(link);
                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                // ЫЫ
            }
        }

        private static Random _rnd = new Random();
        private static void GenPageList()
        {
            var okList = new List<List<int>>();
            var sixList = new List<int>();
            for (int i = 1; i <= 243; i++)
            {
                if (i % 6 - 1 == 0)
                {
                    okList.Add(sixList);
                    sixList = new List<int>();
                }
                sixList.Add(i);

                if (i == 243)
                    okList.Add(sixList);
            }

            okList.RemoveAt(0);

            var rndList = new List<int>();
            foreach (var six in okList)
            {
                while (six.Count != 0)
                {
                    var indexer = _rnd.Next(six.Count);
                    rndList.Add(six[indexer]);
                    six.RemoveAt(indexer);
                }
            }

            var rndString = string.Empty;
            foreach (var page in rndList)
            {
                if (page != rndList.Last())
                    rndString += $"{page}, ";
                else
                    rndString += $"{page}";
            }

            Console.WriteLine(rndString);
            File.WriteAllText("PageString.txt", rndString);
            Console.WriteLine("Count: " + rndList.Count + Environment.NewLine);
            Console.WriteLine("DONE");
        }

        private static async void GetCLinks()
        {
            var doneCounter = 0;
            var pageList = new List<int> { 4, 6, 2, 5, 3, 1, 9, 12, 8, 11, 10, 7, 14, 13, 18, 16, 17, 15,
                21, 22, 23, 20, 24, 19, 27, 26, 25, 28, 29, 30, 34, 35, 33, 36, 31, 32, 40, 41, 38, 42,
                37, 39, 46, 48, 43, 44, 45, 47, 50, 53, 51, 54, 49, 52, 55, 56, 60, 57, 58, 59, 62, 63,
                66, 61, 64, 65, 72, 69, 71, 68, 70, 67, 75, 77, 73, 78, 74, 76, 79, 80, 81, 83, 84, 82,
                90, 88, 87, 85, 89, 86, 95, 92, 91, 96, 94, 93, 102, 98, 100, 101, 97, 99, 107, 106,
                108, 103, 104, 105, 109, 111, 114, 112, 110, 113, 118, 120, 116, 117, 115, 119, 124,
                123, 126, 125, 122, 121, 127, 131, 128, 129, 132, 130, 136, 133, 137, 134, 138, 135,
                141, 144, 140, 142, 139, 143, 146, 145, 147, 150, 148, 149, 154, 152, 151, 153, 155,
                156, 162, 161, 160, 158, 159, 157, 163, 167, 165, 164, 166, 168, 172, 170, 169, 171,
                173, 174, 175, 176, 177, 179, 180, 178, 181, 183, 185, 184, 186, 182, 190, 187, 191,
                192, 188, 189, 194, 198, 197, 195, 193, 196, 204, 203, 202, 199, 200, 201, 208, 206,
                207, 210, 205, 209, 214, 213, 212, 211, 215, 216, 218, 221, 217, 222, 220, 219, 225,
                228, 227, 223, 224, 226, 231, 233, 234, 232, 229, 230, 240, 239, 235, 238, 237, 236,
                241, 243, 242 };

            foreach (var pageNum in pageList)
            {
                Console.WriteLine(" :: Page: " + pageNum + " :: Already done: " + $"{doneCounter}/{pageList.Count}");

                labelTryAgain:
                try
                {
                    await CNetRawLinks(pageNum);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Console.WriteLine("Trying: " + pageNum + " again.");
                    await Task.Delay(2000);
                    goto labelTryAgain;
                }

                doneCounter++;

                if (doneCounter == pageList.Count) Console.WriteLine("ALL DONE.");

                await Task.Delay(_rnd.Next(1500) + 1200);
            }
        }

        private static async Task CNetRawLinks(int pageNum) // min: 1; max: 243;
        {
            if (!Directory.Exists("RawLinks")) Directory.CreateDirectory("RawLinks");

            HtmlDocument doc = await GetDoc(pageNum);

            var xpath = "//*[@id=\"dfllResults\"]/div//section//div[2]/a"; // xpath: //*[@id=\"dfllResults\"]/div[1]/section[3]/div[2]/a
            // //*[@id=\"dfllResults\"]/div//section//div//a
            var cnodes = doc.DocumentNode.SelectNodes(xpath);
            var clinks = new List<(string Link, string Model)>();
            foreach (var cnode in cnodes)
            {
                var itext = cnode.InnerText.Trim();
                if (itext.Contains("\n")) itext = itext.Remove(itext.IndexOf("\n"), itext.Length - itext.IndexOf("\n"));

                var link = ("https://www.cnet.com" + cnode.GetAttributeValue("href", "none")).Trim();

                clinks.Add((link, itext));
                Console.WriteLine(itext + " -> " + link);
            }
            Console.WriteLine(Environment.NewLine + "Count: " + clinks.Count + Environment.NewLine);

            File.WriteAllLines($"RawLinks/CLinks{pageNum}.txt", clinks.Select(x => $"{x.Model} -> {x.Link}"));
        }

        private static async System.Threading.Tasks.Task<HtmlDocument> GetDoc(int pageNum)
        {
            // https://www.cnet.com/topics/laptops/products/2/
            var url = "https://www.cnet.com/topics/laptops/products/";
            if (pageNum > 1)
                url += "/" + pageNum + "/";
            var client = new HttpClient();
            var data = await client.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            return doc;
        }

        private static async void GetData()
        {
            var client = new HttpClient();

            var sw = new Stopwatch();
            sw.Start();

            // https://www.alibaba.com/corporations/agriculture_tyres/--CN------------------50/2.html
            var htmldata = await client.GetStringAsync(
                "https://www.alibaba.com/corporations/agriculture_tyres/--CN------------------50/7.html");

            sw.Stop();

            var doc = new HtmlDocument();
            doc.LoadHtml(htmldata);

            // //*[@id=\"J-items-content\"]/div//div/div//div//div//h2/a
            var compNodes = doc.DocumentNode.SelectNodes(
                "//*[@id=\"J-items-content\"]/div//div/div//div//div//h2/a");

            //// //*[@id=\"J-items-content\"]/div//div/div//div//div//div//ul/li/div//a
            //var ratingNodes = doc.DocumentNode.SelectNodes(
            //    "//*[@id=\"J-items-content\"]/div//div/div//div//div//div//ul/li/div//a");

            // //*[@id=\"J-items-content\"]/div//div/div//div//div
            var nodes = doc.DocumentNode.SelectNodes(
                "//*[@id=\"J-items-content\"]/div//div/div//div//div");

            foreach (var node in nodes)
            {
                var comps = node.SelectNodes(".//div//ul/li/div//a"); // //div[4]/ul/li/div[2]/a


                Console.WriteLine(node.InnerText);
            }
            Console.WriteLine($"{Environment.NewLine}Total: {compNodes.Count}");
            Console.WriteLine($"Time consumed: {sw.Elapsed}");
        }
    }

    internal static class StringExt
    {
        public static List<string> SplitBy(this string tString, string splitString)
        {
            return tString.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
