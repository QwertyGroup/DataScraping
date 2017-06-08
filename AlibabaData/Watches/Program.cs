using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace Watches
{
    class Program
    {
        static void Main(string[] args)
        {
            //"col-md-12"
            var myThread = new Thread(Start);
            myThread.Start();
            for (;;)
                Console.WriteLine(5);
        }

        static async void Start()
        {
            var page = await GetPageTest(@"http://www.chrono24.com/rolex/daytona-steel--18k-yellow-gold-watch-black-dial--id3259523.htm");
            foreach (var line in page.Split('\n'))
            {
                Console.WriteLine(line);
            }
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
    }
}
