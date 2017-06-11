using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataCore
{
    public class StrongholdLibrarian // file manager
    {
        /* Data pattern: *****to**csv***********START*****************************************
        
         --------------------------------                             | new string('-',32) |
         5mnK5K<:]tNjOeC_hljvo%¯  ->  fS[^7B|«t§QX'E8gw*­N*eIzuy       | field -> value     |
         5I lA¦9X¡_¡xp];dk6tNJ9®  ->  ~®\o4{okm_hZr*nztwPKN=[-£       | field -> value     |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | field -> value     |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xumye[qp¥x       | field -> value     |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | field -> value     |
                                                                      | space              |
         --------------------------------                             | new string('-',32) |
         D#WLjdD)0vO*h9>;("dBtdR  ->  fS[^7B|«t§QX'E8gw*­N*eI#b4       | field -> value     |
         yX-SC«w!t3R'p}nkh;­lF8IE  ->  W,4OB¨9(a0pvhm\$j¡¦"7Qª¬}       | field -> value     |
         5I lA¦9X¡_¡xp];dk6tNJCk  ->  \o4{okm_hZr*nztwPKN=Z^_f$       | field -> value     |
         0!:`"¨¨v!!§)~8h>&Z0V¥Hk  ->  DJ)C£¨ZilU¦¯Yn{Xu|N§ª¯;¦!       | field -> value     |
         >As+.Gk}2¤|;,Ek;>N,xbZU  ->  @«^e&&¬B<Uf(l£{&@!v>g$=bj       | field -> value     |
                                                                      | space              |
         ********************************                             | new string('*',32) | *Stop parse data here*
         Models := 2                                                  | field := data      |  
         Unique fields := 10                                          | field := data      |
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
         Models := 2                                                  | field := data      |  
         Unique fields := 10                                          | field := data      |
         And other useful data

        ****************************************END*******************************************/

        public string RootDirectory { get; set; } = "RootDir/";

        public void SaveLinksToFile(string brand, List<(string model, string link)> model_link)
        {
            var dir = $"{RootDirectory}/Links/".CheckDirectory();
            var path = $"{dir}{brand}.txt";

            File.WriteAllLines(path, model_link.Select(ml => $"{ml.model} -> {ml.link}"));
            File.AppendAllLines(path, new[] { string.Empty, new string('*', 32),
                $"Model count: {model_link.Count}" });
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

        public void SaveFieldsToFile()
        {
            var dir = $"{RootDirectory}/Data/";
            throw new NotImplementedException();
        }

        public void LoadDataFromFiles()
        {
            var dir = $"{RootDirectory}/Data/";
            throw new NotImplementedException();
        }

        public List<string> ReadFile(string name)
        {
            return File.ReadLines(RootDirectory + name).ToList();
        }

        public void SaveFile(string name, List<string> data)
        {
            File.WriteAllLines(RootDirectory.CheckDirectory(), data);
        }

        public void SaveFile(string name, string data)
        {
            File.WriteAllText(RootDirectory.CheckDirectory(), data);
        }

    }
}
