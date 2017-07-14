using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataCore
{
    public class StrongholdLibrarian // file manager
    {
        /* Data pattern: *****to**csv***********START*****************************************
        
         5mnK5K<:]tNjOeC_hljvo%¯  ->  fS[^7B|«t§QX'E8gw*­N*eIzuy       | field -> value     |
         5I lA¦9X¡_¡xp];dk6tNJ9®  ->  ~®\o4{okm_hZr*nztwPKN=[-£       | field -> value     |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | field -> value     |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xumye[qp¥x       | field -> value     |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | field -> value     |
         --------------------------------                             | new string('-',32) |
                                                                      | space              |
         D#WLjdD)0vO*h9>;("dBtdR  ->  fS[^7B|«t§QX'E8gw*­N*eI#b4       | field -> value     |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | field -> value     |
         5I lA¦9X¡_¡xp];dk6tNJCk  ->  \o4{okm_hZr*nztwPKN=Z^_f$       | field -> value     |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xu|N§ª¯;¦!       | field -> value     |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | field -> value     |
         --------------------------------                             | new string('-',32) |
                                                                      | space              |
         ********************************                             | new string('*',32) | *Stop parse data here*
         Models : 2                                                   | field : data       |  
         Unique fields : 10                                           | field : data       |
         And other useful data

        ****************************************END*******************************************/


        /* Data pattern: ***load**data**links***START*****************************************

         5mnK5K<:]tNjOeC_hljvo%¯  ->  fS[^7B|«t§QX'E8gw*­N*eIzuy       | model -> link      |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | model -> link      |
         5I lA¦9X¡_¡xp];dk6tNJ9®  ->  ~®\o4{okm_hZr*nztwPKN=[-£       | model -> link      |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xumye[qp¥x       | model -> link      |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | model -> link      |
         D#WLjdD)0vO*h9>;("dBtdR  ->  fS[^7B|«t§QX'E8gw*­N*eI#b4       | model -> link      |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | model -> link      |
         5I lA¦9X¡_¡xp];dk6tNJCk  ->  \o4{okm_hZr*nztwPKN=Z^_f$       | model -> link      |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xu|N§ª¯;¦!       | model -> link      |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | model -> link      |

         ********************************                             | new string('*',32) | *Stop parse data here*
         Models : 2                                                   | field : data       |  
         Unique fields : 10                                           | field : data       |
         And other useful data

        ****************************************END*******************************************/

        public string RootDirectory { get; set; } = "RootDir/";

        public void SaveLinksToFile(string brand, List<(string model, string link)> model_link)
        {
            var dir = $"{RootDirectory}/Links/".CheckDirectory();
            var path = $"{dir}{brand}.txt";

            File.WriteAllLines(path, model_link.Select(ml => $"{ml.model} -> {ml.link}"));
            File.AppendAllLines(path, new[] { string.Empty, new string('*', 32),
                $"Model count := {model_link.Count}" });
        }

        public List<(string brand, List<(string model, string link)> model_link)> LoadLinksFromFiles()
        {
            var dir = $"{RootDirectory}/Links/".CheckDirectory();
            var files = Directory.GetFiles(dir);
            var loadedData = new List<(string brand, List<(string model, string link)> model_link)>();

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var data = new List<(string model, string link)>();

                foreach (var line in lines)
                {
                    if (line == new string('*', 32)) break;
                    if (line == string.Empty) continue;

                    var spl = line.SplitByAndTrim(" -> ");
                    data.Add((spl[0], spl[1]));
                }

                loadedData.Add((name, data));
            }

            return loadedData;
        }

        public void SaveFieldsToFile(string brand, List<List<(string field, string value)>> models)
        {
            var dir = $"{RootDirectory}/Data/".CheckDirectory();
            var path = $"{dir}{brand}.txt";
            var tosave = new List<string>();
            var counter = 0;
            foreach (var model in models)
            {
                counter++;
                foreach (var line in model)
                    tosave.Add($"{line.field} -> {line.value}");
                tosave.Add(new string('-', 32));
            }
            if (!File.Exists(path))
                File.WriteAllLines(path, tosave);
            else
            {
                var lines = File.ReadAllLines(path).ToList();
                lines.RemoveRange(lines.IndexOf(new string('*', 32)), lines.Count - lines.IndexOf(new string('*', 32)));
                lines.AddRange(tosave);
                counter = 0;
                foreach (var line in lines) if (line == new string('-', 32)) counter++;
                File.WriteAllLines(path, lines);
            }

            File.AppendAllLines(path, new[] { new string('*', 32), $"Count: {counter}" });
        }

        public List<(string brand, List<List<(string field, string val)>> models)> LoadDataFromFiles()
        {
            var dir = $"{RootDirectory}/Data/";
            var files = GetFilesFromDirectory(FileName.NameWithoutExtension, "Data/");
            var data = new List<(string brand, List<List<(string field, string val)>>)>();
            foreach (var file in files)
            {
                var lines = ReadFile($"{file}.txt", "Data/");
                var allfields = new List<List<(string field, string val)>>();
                var localfields = new List<(string field, string val)>();
                foreach (var line in lines)
                {
                    if (line == new string('*', 32)) break;
                    if (line == new string('-', 32))
                    {
                        allfields.Add(localfields);
                        localfields = new List<(string field, string val)>();
                    }

                    if (line.Contains("->"))
                    {
                        var spl = line.SplitByAndTrim("->");
                        localfields.Add((spl[0], spl[1]));
                    }
                }
                data.Add((file, allfields));
                Debug.WriteLine($"{file} loaded.");
            }
            return data;
        }

        public List<string> ReadFile(string name, string pathfromRoot = "")
        {
            return File.ReadLines(RootDirectory + pathfromRoot + name).ToList();
        }

        public void SaveFile(string name, List<string> data, string pathfromRoot = "")
        {
            File.WriteAllLines((RootDirectory + pathfromRoot).CheckDirectory() + name, data, Encoding.Unicode);
        }

        public void AppendFile(string name, List<string> data, string pathfromRoot = "")
        {
            File.AppendAllLines((RootDirectory + pathfromRoot).CheckDirectory() + name, data);
        }

        public void SaveFile(string name, string data, string pathfromRoot = "")
        {
            File.WriteAllText((RootDirectory + pathfromRoot).CheckDirectory() + name, data);
        }

        public void AppendFile(string name, string data, string pathfromRoot = "")
        {
            File.AppendAllText((RootDirectory + pathfromRoot).CheckDirectory() + name, data);
        }

        public void AppendGeneralInfo(string fname, List<(string field, string data)> info, string pathfromRoot = "")
        {
            fname = RootDirectory + pathfromRoot + fname;
            var lines = File.ReadAllLines(fname).ToList();
            lines.AddRange(info.Select(item => $"{item.field}: {item.data}"));
            File.WriteAllLines(fname, lines);
        }

        public List<(string field, string data)> ReadGeneralInfo(string fname, string pathfromRoot = "")
        {
            fname = RootDirectory + pathfromRoot + fname;
            var lines = File.ReadAllLines(fname).ToList();
            var startindex = lines.IndexOf(new string('*', 32));
            lines.RemoveRange(0, startindex + 1);
            return lines.Select(line =>
            {
                var spl = line.SplitByAndTrim(":");
                return (spl[0], spl[1]);
            }).ToList();
        }

        public enum FileName { FullPath, Name, NameWithoutExtension }
        public List<string> GetFilesFromDirectory(FileName fn, string pathfromRoot = "")
        {
            pathfromRoot = RootDirectory + pathfromRoot;
            var files = Directory.GetFiles(pathfromRoot);

            if (fn == FileName.FullPath)
                return files.ToList();

            if (fn == FileName.Name)
                return files.Select(file => Path.GetFileName(file)).ToList();

            if (fn == FileName.NameWithoutExtension)
                return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();

            return null;
        }

        public int CountModels(string brand)
        {
            brand = $"{brand}.txt";
            var lines = ReadFile(brand, "Data/");
            lines.RemoveRange(0, lines.IndexOf(new string('*', 32)) + 1);
            var fields = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var spl = line.SplitByAndTrim(":");
                fields.Add(spl[0], spl[1]);
            }
            return Convert.ToInt32(fields["Count"]);
        }

        public string GetLastLink(string brand)
        {
            brand = $"{brand}.txt";
            var lines = ReadFile(brand, "Data/");
            var links = new List<string>();
            foreach (var line in lines)
            {
                if (!line.Contains("->")) continue;
                var spl = line.SplitByAndTrim("->");
                if (spl[0] == "Link") links.Add(spl[1]);
            }
            return links.Last();
        }

        public void SaveCSV(string name, List<string> data, string pathfromcsv = "")
        {
            var dir = $"{RootDirectory}/CSV/{pathfromcsv}".CheckDirectory();
            File.WriteAllLines($"{dir}{name}.csv", data, Encoding.Unicode);
        }
    }
}
