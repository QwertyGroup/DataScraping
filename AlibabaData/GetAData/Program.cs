using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetAData
{
    class Program
    {
        private const string COMPANIES = "Companies";
        private const string TXT_EXTENSION = ".txt";
        private const string HTML_EXTENSION = ".html";
        private static int _topicCounter;
        private static string _folderPath = "Companies/";
        private static string FolderPath
        {
            get
            {
                if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
                return _folderPath;
            }
            set
            {
                _folderPath = value;
            }
        }

        static void Main(string[] args)
        {
            var links = GetLinks();
            ProcessLinks(links);
            Console.ReadLine();
        }

        private static async void ProcessLinks(List<string> links)
        {
            foreach (var link in links)
            {
                var maxPages = await GetMaxPages(link);
                _topicCounter++;
                Console.WriteLine("Starting: " + _topicCounter);
                for (int i = 1; i <= maxPages; i++)
                {
                    GetCompanies(link, i);
                    await Task.Delay(200);
                }
                Console.WriteLine(maxPages + " donwloaded.");
                Console.WriteLine(_topicCounter + ". " + link + " ::DONE");
                await Task.Delay(10000);
            }
        }

        private static async void GetCompanies(string link, int page)
        {
            var fullLink = link + $"_{page}{HTML_EXTENSION}";
            var resp = await GetResonse(fullLink);
            var companies = new List<string>();

            try
            {
                // <h2 class="title ellipsis">
                var c0 = resp.Split(new string[] { "<h2 class=\"title ellipsis\">" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                c0.RemoveAt(0);
                foreach (var c1 in c0)
                {
                    // href="
                    var c2 = c1.Split(new string[] { "href=\"" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    // title="
                    var c3 = c2.Split(new string[] { "title=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    companies.Add(c3);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Console.WriteLine("Topic: " + _topicCounter + ". Page: " + page + ". Done.");
            File.WriteAllLines(FolderPath + COMPANIES + "." + _topicCounter + "." + page + TXT_EXTENSION, companies);
        }

        private static async Task<int> GetMaxPages(string link)
        {
            try
            {
                var resp = await GetResonse(link + "_1.html");
                var danderResp = resp.SplitBy("rel=\"nofollow\">Next</a>");
                await Task.Delay(500);
                //if (danderResp[0].Contains("暂时无法处理您的请求")) { await Task.Delay(1000); goto danger; }

                var r1 = danderResp[1]; // rel="nofollow">Next</a>
                var r2 = r1.SplitBy("</script>")[0]; // </script>
                r2 = r2.Remove(0, r2.IndexOf("total:") + 6);
                r2 = r2.Remove(r2.IndexOf("}"), r2.Length - 1 - r2.IndexOf("}"));
                return Convert.ToInt32(r2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        private static List<string> GetLinks()
        {
            var path = "Links.txt";
            return File.ReadAllLines(path).ToList().Select(
                x => x.Replace(HTML_EXTENSION, string.Empty)).ToList();
        }

        private static async Task<string> GetResonse(string fullLink)
        {
            var req = WebRequest.Create(fullLink);
            var resp = await req.GetResponseAsync();
            return await new StreamReader(resp.GetResponseStream()).ReadToEndAsync();
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
