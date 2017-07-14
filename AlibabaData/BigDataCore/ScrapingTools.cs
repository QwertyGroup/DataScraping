using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net.Http;
using System.Net;

using HtmlAgilityPack;
using EyeOfTheUniverseCore;

namespace BigDataCore
{
    public class ScrapingTools
    {
        private Random _rnd = new Random();

        public HtmlNodeCollection GetXPathNodes(HtmlDocument doc, string xpath)
        {
            var cl = doc.DocumentNode.SelectNodes(xpath);
            if (cl == null) throw new Exception("Nothing found by xpath.");
            return cl;
        }

        public List<string> GetXPathNodesAndProcess(HtmlDocument doc, string xpath,
            Func<HtmlNodeCollection, List<string>> processor = null)
        {
            var collection = GetXPathNodes(doc, xpath);
            if (processor == null) processor = nodes => nodes.Select(node => node.InnerText).ToList();
            return processor(collection);
        }

        public List<List<string>> GetXPathNodesAndProcess(HtmlDocument doc, string xpath,
            Func<HtmlNodeCollection, List<List<string>>> processor)
        {
            var collection = GetXPathNodes(doc, xpath);
            return processor(collection);
        }

        public async Task<HtmlDocument> DownloadDocAsync(string url, int sleep_ms = 3)
        {
            var doc = (await DownloadMultipleDocsAsync(new List<string> { url }, sleep_ms)).First();
            if (sleep_ms != 0) await Task.Delay(sleep_ms);
            return doc;
        }

        public async Task<List<HtmlDocument>> DownloadMultipleDocsAsync(List<string> urls, int sleep_ms = 0)
        {
            return await new DocsLoader(urls).DownloadDocsAsync(sleep_ms);
        }

        private class DocsLoader
        {
            private SortedDictionary<int, HtmlDocument> _docs = new SortedDictionary<int, HtmlDocument>();
            private List<string> _urls;

            private DocsLoader() { }
            public DocsLoader(List<string> urls)
            {
                _urls = urls;
                for (int i = 0; i < _urls.Count; i++)
                    _docs.Add(i, null);

                //OnDocLoaded += (s, e) => alreadyLoaded++;
            }

            private int alreadyLoaded = 0;
            public async Task<List<HtmlDocument>> DownloadDocsAsync(int sleep_ms = 0)
            {
                var counter = 0;
                foreach (var url in _urls)
                {
                    if (sleep_ms != 0) await Task.Delay(sleep_ms);
                    DownloadDoc(url, counter);
                    counter++;
                }

                //await new AThingThatFreezesTask(this, _urls.Count).Freeze(); // NOT rly working((
                while (alreadyLoaded != _urls.Count) // Trying this
                    await Task.Delay(TimeSpan.FromMilliseconds(20));

                return _docs.Select(pair => pair.Value).ToList();
            }

            public event EventHandler<string> OnDocLoaded;

            private async void DownloadDoc(string url, int key)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Android 7.0; rv:41.0) Gecko/41.0 Firefox/41.0");
                client.Timeout = TimeSpan.FromSeconds(40);
                var page = string.Empty;
                tryagain:
                try
                {
                    page = await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("A task was canceled")) // Timeout
                    {
                        await new EyeApi().SpreadMessageAsync("40 sec timeout. Trying again...");
                        goto tryagain;
                    }

                    if (!ex.Message.Contains("410")) // filter
                    {
                        if (ex.Message.Contains("403"))
                            await new EyeApi().SpreadMessageAsync("403" + Environment.NewLine + ex.Message + Environment.NewLine + url);
                        //else
                        //    await new EyeApi().SpreadMessageAsync(ex.Message + Environment.NewLine + url);
                    }
                    if (ex.Message.Contains("403")) throw ex;
                    //OnDocLoaded?.Invoke(this, url);
                    alreadyLoaded++;
                    return;
                }
                page = WebUtility.HtmlDecode(page);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page);
                _docs[key] = doc;
                //OnDocLoaded?.Invoke(this, url);
                alreadyLoaded++;
            }

            private class AThingThatFreezesTask
            {
                private int _maxCount;
                private int _currentCount;

                private AThingThatFreezesTask() { }
                public AThingThatFreezesTask(DocsLoader dad, int maxCount)
                {
                    phosphor = new SemaphoreSlim(1, 1);
                    _maxCount = maxCount;
                    _currentCount = 0;
                    var releasedTimes = 0;
                    phosphor.Wait();
                    dad.OnDocLoaded += async (s, url) =>
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(5));
                            _currentCount++;
                            //Console.WriteLine($"{_currentCount} of {maxCount} released");
                            if (_currentCount == _maxCount)
                            {
                                releasedTimes++;
                                if (releasedTimes == 1)
                                {
                                    Console.WriteLine("Released");
                                    phosphor.Release();
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    };


                }

                private SemaphoreSlim phosphor;
                public async Task Freeze()
                {
                    await phosphor.WaitAsync();
                }
            }
        }

        public List<int> GenChunkSequence(int maxPageNum, int chunkSize = 6)
        {
            if (chunkSize < 1 || chunkSize > maxPageNum)
                throw new Exception($"Invalid chunk size. chS: {chunkSize}. maxP: {maxPageNum}");

            if (chunkSize == 1)
            {
                var chOne = new List<int>();
                for (int i = 1; i <= maxPageNum; i++)
                    chOne.Add(i);
                return chOne;
            }

            var okList = new List<List<int>>();
            var chunkList = new List<int>();
            for (int i = 1; i <= maxPageNum; i++)
            {
                if (i % chunkSize - 1 == 0)
                {
                    okList.Add(chunkList);
                    chunkList = new List<int>();
                }
                chunkList.Add(i);

                if (i == maxPageNum)
                    okList.Add(chunkList);
            }

            okList.RemoveAt(0);

            var rndList = new List<int>();
            foreach (var chunk in okList)
            {
                while (chunk.Count != 0)
                {
                    var indexer = _rnd.Next(chunk.Count);
                    rndList.Add(chunk[indexer]);
                    chunk.RemoveAt(indexer);
                }
            }

            Debug.WriteLine("ScrapingTools: Chunk int sequence generated.");
            return rndList;
        }

        public int GenRndDelay(int bottom_ms, int top_ms)
        {
            if (bottom_ms > top_ms)
                (bottom_ms, top_ms) = (top_ms, bottom_ms);

            var delay = _rnd.Next(top_ms - bottom_ms) + bottom_ms;
            Debug.WriteLine($"Delay gen.: {delay} ms");
            return delay;
        }
    }
}
