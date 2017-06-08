using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using HtmlAgilityPack;
using System.Net.Http;

namespace Watches
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = new Task(Start);
            task.Start();
            Console.ReadLine();
        }

        static async void Start()
        {
            Console.WriteLine("Downloading...");
            var page = await GetPage(@"http://www.chrono24.com/rolex/deepsea-d-blue-james-cameron--id4486491.htm");
            var table = GetTable(page);
            Console.WriteLine(table.Split('\n').Length);
        }

        static async Task<HtmlDocument> GetPage(string url)
        {
            var client = new HttpClient();
            string page = await client.GetStringAsync(url);
            page = WebUtility.HtmlDecode(page);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);
            return doc;
        }

        static string GetTable(HtmlDocument doc)
        {
            var xpath = "//*[@id=\"anti-flicker\"]/div[6]/div/section[2]/div/div[1]/div[1]";
            var cnodes = doc.DocumentNode.SelectSingleNode(xpath);
            Console.WriteLine(cnodes.InnerText);
            return cnodes.InnerText;
        }
    }
}
