using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using HtmlAgilityPack;
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
            var page = await GetPageTest(@"http://www.chrono24.com/rolex/daytona-steel--18k-yellow-gold-watch-black-dial--id3259523.htm");
            var table = GetTable(page);
            Console.WriteLine(table.Split('\n').Length);
        }

        static async Task<string> GetPageTest(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string page = sr.ReadToEnd();
            page = WebUtility.HtmlDecode(page);

            return page;
        }

        static string GetTable(string htmlPage)
        {
            var xpath = "//*[@id=\"anti - flicker\"]/div";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlPage);
            var cnodes = doc.DocumentNode.SelectNodes(xpath);
            Console.WriteLine(cnodes.First().InnerText);
            return cnodes.First().InnerText;
        }
    }
}
