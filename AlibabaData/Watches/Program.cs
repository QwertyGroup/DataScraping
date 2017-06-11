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
using BigDataCore;

namespace Watches
{
    class Program
    {
        static void Main(string[] args)
        {
            new Task(new Rolex().Start).Start();
            Console.ReadLine();
        }
    }

    class Rolex
    {
        private Random _rnd = new Random();
        private StrongholdLibrarian _lib = new StrongholdLibrarian();
        private ScrapingTools _tools = new ScrapingTools();

        public void Start()
        {
            //Lower();
            //Merge();

            //GatherLinks();

            //CountWatches();
        }

        private void CountWatches()
        {
            var files = _lib.GetFilesFromDirectory(StrongholdLibrarian.FileName.Name, "Links/");
            var selector = 1;
            var info = files.Select(file => _lib.ReadGeneralInfo(file, "Links/")).
                Aggregate((acc, list) =>
                {
                    list = list.Select(item => { item.field = files[selector]; return item; }).ToList();
                    acc.AddRange(list);
                    selector++;
                    return acc;
                }).ToList(); // Why acc starts with first element

            var total = info.Select(item => Convert.ToInt32(item.data)).Aggregate((acc, val) => acc += val);

            _lib.SaveFile("CountInfo.txt", info.Select(item => item.field + ": " + item.data).ToList(), "Info/");
            _lib.AppendFile("CountInfo.txt", $"Total: {total.ToString()}", "Info/");

            Console.WriteLine("Count done.");
        }

        private void Merge()
        {
            var brands = File.ReadAllLines("ToLoad.txt");
            var shrts = File.ReadAllLines("Lower.txt");
            for (int i = 0; i < brands.Length; i++)
            {
                brands[i] += " -> " + shrts[i];
            }
            File.WriteAllLines("RawBrands.txt", brands);
        }

        private async void GatherLinks()
        {
            List<(string fbrand, string shrt)> brands = _lib.ReadFile("RawBrands.txt").
                Select(line =>
                {
                    var spl = line.SplitByAndTrim("->");
                    return (spl[0], spl[1]);
                }).ToList();

            foreach (var brandpair in brands)
            {
                Console.WriteLine($"{brandpair.fbrand} starting...");
                var rootlink =
                    $"http://www.chrono24.com/{brandpair.shrt}/index.htm?man={brandpair.shrt}&pageSize=120&showpage=";

                var maxpages = await GetMaxPage(rootlink + 1);
                var pagelist = _tools.GenChunkSequence(maxpages, (maxpages < 8) ? maxpages : 8);
                var pairs = new List<(string model, string link)>();

                var pagesCounter = 0;
                while (pagelist.Any())
                {
                    var counter = 0;
                    var urls = new List<string>();
                    while (counter < _rnd.Next(3) + 3 && pagelist.Any())
                    {
                        urls.Add(rootlink + pagelist.First());
                        pagelist.RemoveAt(0);
                        counter++;
                    }
                    pairs.AddRange(await GetLinkPairs(urls));
                    pagesCounter += counter;
                    Console.WriteLine(pagesCounter + " of " + maxpages);
                    await Task.Delay(_tools.GenRndDelay(300, 400));
                }

                _lib.SaveLinksToFile(brandpair.fbrand, pairs);

                await Task.Delay(_tools.GenRndDelay(800, 1000));
            }

            Console.WriteLine("All done!");
        }

        private async Task<int> GetMaxPage(string url)
        {
            var data = _tools.GetXPathNodesAndProcess(
                await _tools.DownloadDocAsync(url), "//*[@id=\"watches\"]/div[2]/div[2]/ul/li").
                Select(line => line.Replace(" ", string.Empty).Replace("\n", string.Empty)).ToList();
            return Convert.ToInt32(data[data.Count - 2]);
        }

        private void Lower()
        {
            var lines = File.ReadAllLines("ToLoad.txt");
            lines = lines.Select(line => line.ToLower().Replace(" ", string.Empty).
            Replace("&", string.Empty).Replace(".", string.Empty)).ToArray();
            File.WriteAllLines("Lower.txt", lines);
        }

        public async Task<List<(string model, string link)>> GetLinkPairs(List<string> urls)
        {
            var docs = await _tools.DownloadMultipleDocsAsync(urls, _tools.GenRndDelay(100, 120));
            var pairs = new List<(string model, string link)>();

            foreach (var doc in docs)
            {
                var links = _tools.GetXPathNodesAndProcess(doc, "//*[@id=\"watches\"]/div[1]/div/a",
                nodes => nodes.Select(node => $"http://www.chrono24.com{node.GetAttributeValue("href", "def")}").ToList());

                var models = _tools.GetXPathNodesAndProcess(doc, "//*[@id=\"watches\"]/div[1]/div/a/span[2]/strong",
                    nodes => nodes.Select(node =>
                    node.InnerText.Replace("\r\n", string.Empty).Replace(" ", string.Empty)).ToList());

                if (links.Count != models.Count) throw new Exception("links.Count != models.Count");
                for (int i = 0; i < links.Count; i++)
                    pairs.Add((models[i], links[i]));
            }

            return pairs;
        }
    }
}
