using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace BigDataCore
{
    public class ScrapingTools
    {
        private Random _rnd = new Random();

        public async Task<HtmlDocument> GetDocAsync(string url)
        {
            var client = new HttpClient();
            string page = await client.GetStringAsync(url);
            page = WebUtility.HtmlDecode(page);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);
            return doc;
        }

        public List<int> GenPageSequence(int maxPageNum, int chunkSize = 6)
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

            Debug.WriteLine("ScrapingTools: Page sequence generated.");
            return rndList;
        }
    }
}
