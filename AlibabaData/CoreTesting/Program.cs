using BigDataCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBase test = new BotTesting();        // ToolsTesting | AsyncTesting | MultLoadTesting | Random_ascii
                                                     // LibrarianTesting | BotTesting
            test.Main();
            Console.ReadLine();
        }
    }

    abstract class TestBase
    {
        public abstract void Main();
    }

    class BotTesting : TestBase
    {
        public override void Main()
        {
            var eye = new EyeOfTheUniverse();
            eye.SendMessage("kek");
        }
    }

    class Random_ascii : TestBase
    {
        public override void Main()
        {
            Console.WriteLine("Random_ascii");

            int asciiCharacterStart = 33; // from which ascii character code the generation should start
            int asciiCharacterEnd = 175; // to which ascii character code the generation should end
            int characterCount = 10000; // count of characters to generate

            Random random = new Random(DateTime.Now.Millisecond);
            StringBuilder builder = new StringBuilder();

            // iterate, get random int between 'asciiCharacterStart' and 'asciiCharacterEnd', then convert it to (char), append to StringBuilder
            for (int i = 0; i < characterCount; i++)
                builder.Append((char)(random.Next(asciiCharacterStart, asciiCharacterEnd + 1) % 255));

            // voila!
            String text = builder.ToString();
        }
    }

    class LibrarianTesting : TestBase
    {
        private StrongholdLibrarian _lib = new StrongholdLibrarian();
        private ScrapingTools _tools = new ScrapingTools();

        public override void Main()
        {
            //Test1();
            Test2();
        }

        private async void Test2()
        {
            // http://www.chrono24.com/rolex/index.htm?man=rolex&pageSize=120&showpage=1
            // http://www.chrono24.com/rolex/index.htm?man=rolex&pageSize=120&showpage=2
            // //*[@id=\"watch-5793154\"]/span[2]/strong
            // //*[@id=\"watches\"]/div[1]/div/a/span[2]/strong
            // //*[@id="watch-6329862"]/span[2]/strong
            // //*[@id="watch-6329862"]

            var urls = new[] {
                "http://www.chrono24.com/rolex/index.htm?man=rolex&pageSize=120&showpage=1",
                "http://www.chrono24.com/rolex/index.htm?man=rolex&pageSize=120&showpage=2"
            };

            var docs = await _tools.DownloadMultipleDocsAsync(urls.ToList());

            var nodes1 = _tools.GetXPathNodesAndProcess(docs[0], "//*[@id=\"watches\"]/div[1]/div");

            var nodes2links = _tools.GetXPathNodesAndProcess(docs[1], "//*[@id=\"watches\"]/div[1]/div/a",
                nodes => nodes.Select(node => node.GetAttributeValue("href", "def")).ToList());

            var nodes2names = _tools.GetXPathNodesAndProcess(docs[1], "//*[@id=\"watches\"]/div[1]/div/a/span[2]/strong",
                nodes => nodes.Select(node =>
                node.InnerText.Replace("\r\n", string.Empty).Replace(" ", string.Empty)).ToList());
        }

        private void Test1()
        {
            _lib.RootDirectory = "TestRootDir";

            var pairs = new List<(string, string)> { ("model1", "link1"), ("model2", "link2"),
                ("model3", "link3"),("model4", "link4") };

            _lib.SaveLinksToFile("Brand1", pairs);
            _lib.SaveLinksToFile("Brand2", pairs);
            _lib.SaveLinksToFile("Brand3", pairs);

            var loaded = _lib.LoadLinksFromFiles();
        }
    }

    class MultLoadTesting : TestBase
    {
        private ScrapingTools _coreTools = new ScrapingTools();

        public override void Main()
        {
            Console.WriteLine("MultLoadTesting");

            //RunDL();
            RunBikes();
        }

        private void RunBikes()
        {

        }

        private async void RunDL()
        {
            var urls = new List<string> {
                "https://stackoverflow.com/questions/20056727/need-to-understand-the-usage-of-semaphoreslim",
                "https://translate.google.com.ua/#ru/en/%D0%BE%D0%B1%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D0%B0%D1%82%D1%8C%20%D1%82%D0%B5%D0%BA%D1%81%D1%82",
                "https://msdn.microsoft.com/en-us/library/dd449174(v=vs.110).aspx",
                "http://www.azlyrics.com/",
                "https://habrahabr.ru/post/213809/",
                "http://bikez.com/brand/aprilia_motorcycles.php",
                "http://www.chrono24.com/rolex/index.htm",
                "http://www.cyberforum.ru/csharp-beginners/thread1268162.html",
                "http://www.chrono24.com/rolex/datejust-ii-champagne18k-gold-o41mm---116333--id5772415.htm",
                "https://stackoverflow.com/questions/15149811/how-to-wait-for-async-method-to-complete",
                "https://www.visualstudio.com/ru/",
                "https://videosmile.ru/",
                "https://photoshop-master.org/disc183/"
            };
            var docs = await _coreTools.DownloadMultipleDocsAsync(urls);
            //var doc = await _coreTools.DownloadDocAsync("https://photoshop-master.org/disc183/");
        }
    }

    class ToolsTesting : TestBase
    {
        private ScrapingTools _coreTools = new ScrapingTools();

        public override void Main()
        {
            Console.WriteLine("ToolsTesting");

            var pages = _coreTools.GenChunkSequence(100, 4);
            var spl1 = "  abc <<->> cba         <<->> kekes     <<->>     ".SplitBy("<<->>");
            var spl2 = "  abc <<->> cba         <<->> kekes     <<->>     ".SplitByAndTrim("<<->>");
            AsyncMethod();
        }

        private async void AsyncMethod()
        {
            var data = _coreTools.GetXPathNodesAndProcess(await _coreTools.DownloadDocAsync(
                "https://stackoverflow.com/questions/2082615/pass-method-as-parameter-using-c-sharp"),
                "//*[@id=\"sidebar\"]/div/div/div/a",
                nc => nc.Select(node => node.InnerHtml).ToList());
        }
    }

    class AsyncTesting : TestBase
    {
        public override void Main()
        {
            Console.WriteLine("AsyncTesting");

            StartMultiple();
        }

        private Random _rnd = new Random();
        private async void StartMultiple()
        {
            for (int i = 0; i < 20; i++)
            {
                //await SingleLoad1(i);
                SingleLoad2(i);
            }
        }

        private int _RandomDelay { get { return _rnd.Next(500) + 500; } }
        private async Task SingleLoad1(int n)
        {
            await Task.Delay(_RandomDelay);
            Console.WriteLine($"Load{n}");
        }

        private async void SingleLoad2(int n)
        {
            await Task.Delay(_RandomDelay);
            Console.WriteLine($"Load{n}");
        }
    }
}
