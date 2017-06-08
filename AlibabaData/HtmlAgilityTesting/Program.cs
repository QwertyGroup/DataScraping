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

            //LoadFromPage(1);

            //Test404();

            //BikesStart();

            //CountModels();

            //GetAllFields();

            Console.ReadLine();
        }

        private static void CountModels()
        {
            var data = File.ReadAllLines($"{BikesDir}Specs/kwsk2.txt");
            var counter = 0;
            foreach (var line in data)
            {
                if (line.Contains(new string('-', 16)))
                    //if (line.Contains("->"))
                    counter++;
            }
            Console.WriteLine(counter);
        }

        private static void GetAllFields()
        {
            var files = Directory.GetFiles("Bikes/NEW/");
            var fields = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var data = File.ReadAllLines(file).ToList();
                data = data.Where(line => line.Contains("->")).ToList();
                foreach (var d in data)
                {
                    var f = d.SplitBy("->")[0].Trim();
                    f = f.Substring(0, f.Length - 1);
                    if (!fields.Keys.Contains(f)) fields.Add(f, 1);
                    else fields[f]++;
                }
            }

            List<(string key, int val)> flds = fields.OrderBy(f => f.Value).Select(f => (f.Key, f.Value)).Reverse().ToList();

            File.WriteAllLines($"{BikesDir}Fields.txt", fields.Keys);
            Console.WriteLine("Fields done!");

            var fieldsDict = new Dictionary<string, string>();
            foreach (var k in flds.Select(x => x.key).ToList())
                fieldsDict.Add(k, string.Empty);

            var ListOfEverything = new List<Dictionary<string, string>>();
            foreach (var file in files)
            {

                var data = File.ReadAllLines(file).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    var line = data[i];
                    if (line == new string('-', 16))
                    {
                        ListOfEverything.Add(new Dictionary<string, string>(fieldsDict));
                        continue;
                    }
                    if (!line.Contains("->")) continue;
                    var spl = line.SplitBy(" -> ");
                    if (spl.Count < 2) continue;
                    ListOfEverything.Last()[spl[0]] = spl[1];
                }
            }

            File.WriteAllText($"{BikesDir}ListOfEverything.csv", new Func<string>(() =>
            {
                var row = flds.Select(kv => kv.key).Aggregate((acc, k) => acc += $"{k}|");
                return row.Remove(row.Length - 1, 1);
            }).Invoke());

            File.AppendAllLines($"{BikesDir}ListOfEverything.csv", ListOfEverything.Select(line =>
            {
                var row = line.Select(kv => kv.Value).Aggregate((acc, field) => acc += $"{field}|");
                return row.Remove(row.Length - 1, 1);
            }
            ).ToList());

            Console.WriteLine("All DONE!");
        }

        private static string BikesDir
        {
            get
            {
                var dir = "Bikes/";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static async void BikesStart()
        {
            // http://bikez.com/brands/index.php

            // Aprilia motorcycles
            // ATK motorcycles
            // Bajaj motorcycles
            // Benelli motorcycles
            // Bimota motorcycles
            // BMW motorcycles
            // Cagiva motorcycles
            // CCM motorcycles
            // Ducati motorcycles
            // Enfield motorcycles
            // GAS GAS motorcycles
            // Gilera motorcycles
            // Harley-Davidson motorcycles
            // Honda motorcycles
            // Indian motorcycles
            // Kawasaki motorcycles
            // Keeway motorcycles
            // KTM motorcycles
            // Kymco motorcycles
            // MV Agusta motorcycles
            // Peugeot motorcycles
            // PGO motorcycles
            // Piaggio motorcycles
            // Suzuki motorcycles
            // Triumph motorcycles
            // Ural motorcycles
            // Vespa motorcycles
            // Victory motorcycles
            // Yamaha motorcycles

            // I would need all attributes except "further information" 
            // and I would just need a number of brands, not all 

            // --------------------------------------------------------

            // Interface
            //var test = await DownloadEM("http://bikez.com/motorcycles/bmw_kompressor_1939.php");
            //var validPairs = await GetAllBrandsTxt();
            //GatherAllBiLinks(validPairs);
            // UNCOMMENT
            var sw = new Stopwatch(); sw.Start();
            var files = Directory.GetFiles("Bikes/AllBikes");
            var counter = 0;
            //foreach (var file in files)
            //var file = $"{BikesDir}crackedPages.txt";
            var file = $"{BikesDir}CP.txt";
            //var file = files[1];
            {
                var fTime = new TimeSpan();
                var path = DirChecker(BikesDir + "Specs/");
                //List<(string name, string link)> models = File.ReadAllLines(file).Select(str =>
                //{
                //    var spl = str.SplitBy("->");
                //    return (spl[0].Trim(), spl[1].Trim());
                //}).ToList();

                var localCounter = 0;
                List<(string name, string link)> models = File.ReadAllLines(file).Select(x => ("CRCD", x)).ToList();
                foreach (var model in models)
                {
                    var lsw = new Stopwatch(); lsw.Start();
                    var specs = await DownloadEM(model.link);
                    if (specs == null) { Console.WriteLine("C-cracked"); ; continue; }
                    File.AppendAllLines(path + counter + ".txt", new[] { new string('-', 16), model.name });
                    File.AppendAllLines(path + counter + ".txt", specs.Select(sp => $"{sp.key} -> {sp.val}"));
                    File.AppendAllLines(path + counter + ".txt", new[] { Environment.NewLine });
                    localCounter++;
                    lsw.Stop();
                    fTime = fTime.Add(lsw.Elapsed);
                    Console.WriteLine($"{model.name} done. {localCounter} of {models.Count}. LTime: {lsw.Elapsed}. TTime: {sw.Elapsed}.");
                }

                counter++;
                Console.WriteLine($"{Environment.NewLine}>>_ {file} done. {counter} of {files.ToList().Count}. FTime: {fTime}{Environment.NewLine}{Environment.NewLine}");
            }
            sw.Stop();
            Console.WriteLine($"ALL done! Time: {sw.Elapsed}");
        }

        private static async Task<List<(string key, string val)>> DownloadEM(string url)
        {
            try
            {
                var doc = await GetHTMLDoc(url);
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table//tr");
                var node = doc.DocumentNode;
                var t1 = node.SelectNodes("//td[1]").Select(x => x.InnerText).ToList();
                t1.RemoveRange(0, t1.IndexOf("Model:"));
                try // ыы
                {
                    t1.RemoveRange(t1.IndexOf("Insurance costs"), t1.Count - t1.IndexOf("Insurance costs"));
                }
                catch
                {
                    Console.WriteLine("processing craked");
                    t1.RemoveRange(t1.IndexOf("Ask questions"), t1.Count - t1.IndexOf("Ask questions"));
                }

                var t2 = node.SelectNodes("//tr").ToList();
                var t3 = t2.Where(nd =>
                {
                    if (nd.ChildNodes.Count > 1)
                        if (t1.Contains(nd.ChildNodes[0].InnerText))
                            return true;
                    return false;
                }).Select(nd => nd.ChildNodes[1].InnerText).ToList();

                return t1.Zip(t3, (k, v) => (k, v)).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                File.AppendAllText(BikesDir + "crackedPages.txt", url + Environment.NewLine);
                return null;
            }
        }

        private static async void GatherAllBiLinks(List<(string Name, string Link)> validPairs)
        {
            var Total = 0;
            var sw = new Stopwatch();
            sw.Start();
            foreach (var pair in validPairs)
            {
                var maxPage = await GetMaxPage(pair.Link);

                var BrandBikes = new List<(string Model, string Link)>();
                for (int pn = 1; pn <= maxPage; pn++)
                {
                    var doc = await GetHTMLDoc(pair.Link + $"?page={pn}");
                    //var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table[3]/tr//td[1]/a");
                    var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table[3]/tr//td//a");
                    //var testnodes = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table[3]/tr//td//a");
                    //var testbikes = testnodes.Select(x => x.InnerHtml).ToList();//.Where(x => !x.Contains("img")).ToList();
                    // //*[@id="pagecontent"]/table[3]/tbody/tr[16]/td
                    List<(string Model, string Link)> bikes =
                        nodes.Select(node => (node.InnerText, node.GetAttributeValue("href", "def link"))).
                        Where(node => node.Item2 != "def link" && !node.Item1.Contains("<img src=")).
                        Select(node => (node.Item1.Trim(), ("http://bikez.com" + node.Item2.Remove(0, 2)).Trim())).ToList();

                    bikes = bikes.Where(bike => (bike.Model != string.Empty && bike.Model != "\r\n"
                    && !bike.Model.Contains("&lt;&lt;") && !bike.Model.Contains("http://"))).ToList();

                    if (pn == maxPage)
                        for (int i = 0; i < maxPage - 1; i++)
                            bikes.RemoveAt(bikes.Count - 1);
                    else
                        for (int i = 0; i < maxPage; i++)
                            bikes.RemoveAt(bikes.Count - 1);

                    BrandBikes.AddRange(bikes);
                    Console.WriteLine($"{pair.Name}: {pn} of {maxPage}");
                }

                File.WriteAllLines($"{DirChecker($"{BikesDir}AllBikes/")}{pair.Name}.txt",
                    BrandBikes.Select(bike => $"{bike.Model} -> {bike.Link}"));
                Console.WriteLine($"{pair.Name} ::done. Count: {BrandBikes.Count}{Environment.NewLine}");
                Total += BrandBikes.Count;
                await Task.Delay(200);
            }
            sw.Stop();
            Console.WriteLine($"All done! Total: {Total}; Time: {sw.Elapsed}");
        }

        private static string DirChecker(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        private static async Task<int> GetMaxPage(string link)
        {
            // if null then 1
            // //*[@id="pagecontent"]/table[3]/tr[122]/td/a[6]
            var doc = await GetHTMLDoc(link);
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table[3]/tr//td/a");
            if (nodes == null) return 1;
            var pages = nodes.Select(node => node.InnerText).ToList();
            if (!Int32.TryParse(pages[pages.Count - 2], out int maxPage)) return 1; // Convert.ToInt32(pages[pages.Count - 2]);
            return maxPage;
        }

        private static async Task<List<(string Name, string Link)>> GetAllBrandsTxt()
        {
            var doc = await GetHTMLDoc("http://bikez.com/brands/index.php");
            List<(string Name, string Link)> pairs = doc.DocumentNode.SelectNodes("//*[@id=\"pagecontent\"]/table[3]/tr[1]/td[1]/table/tr//td[1]/a").
                Select(node => (node.InnerText, node.GetAttributeValue("href", "def link"))).
                Where(node => node.Item2 != "def link").Select(node => (node.Item1, "http://bikez.com" + node.Item2)).
                ToList();

            File.WriteAllLines($"{BikesDir}AllBrands.txt", pairs.Select(pair => $"{pair.Name} -> {pair.Link}"));

            var validNames = File.ReadAllLines($"{BikesDir}ValidNames.txt").ToList();
            List<(string Name, string Link)> validParis = pairs.Where(pair => validNames.Contains(pair.Name)).ToList();
            File.WriteAllLines($"{BikesDir}ValidBrands.txt", validParis.Select(pair => $"{pair.Name} -> {pair.Link}"));

            Console.WriteLine("Got Brands.");
            return validParis;
        }

        private static int DelayMs { get { return _rnd.Next(200) + 200; } }
        private static async Task<HtmlDocument> GetHTMLDoc(string url)
        {
            var client = new HttpClient();
            var data = string.Empty;
            try
            {
                data = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            await Task.Delay(DelayMs);
            return doc;
        }

        private static async void Test404()
        {
            var client = new HttpClient();
            var data = await client.GetStringAsync(" https://www.cnet.com/products/lenovo-thinkpad-t460p-20fw-14-core-i7-6820hq-16-gb-ram-512kek-gb-ssd/");
        }

        private static async void LoadFromPage(int page)
        {
            List<(string Name, string Link)> nlpairs =
                File.ReadAllLines($"RawLinks/CLinks{page}.txt").Select(nl =>
            {
                var spl = nl.SplitBy("->");
                return (spl[0].Trim(), spl[1].Trim());
            }).ToList();

            foreach (var nlpair in nlpairs)
            {
                var dir = "TestProps";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var client = new HttpClient();
                var doc = new HtmlDocument();
                var data = string.Empty;
                try
                {
                    data = await client.GetStringAsync(nlpair.Link);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }

                doc.LoadHtml(data);
                var xpath = "//*[@id=\"editorReview\"]/ul";
                HtmlNode st = null;
                try
                {
                    st = doc.DocumentNode.SelectNodes(xpath)[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Console.WriteLine($"{nlpair.Name} :: null");
                    File.AppendAllText($"{dir}/null{page}.txt", nlpair.Name + " -> " + nlpair.Link + Environment.NewLine);
                    continue;
                }

                var props = st.ChildNodes.Select(node => node.InnerText).ToList();

                props.Insert(0, Environment.NewLine + nlpair.Name);

                File.AppendAllLines($"{dir}/page{page}.txt", props);

                await Task.Delay(_rnd.Next(800) + 600);
                Console.WriteLine(nlpair.Name + " done.");
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
