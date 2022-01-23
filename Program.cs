using MinecraftServers.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServers
{   class versionConfig
    {
        public string javapath { get; set; }
        public string jarfile { get; set; }
    }
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>

        static public Form now = null;
        static public Dictionary<string, string[]> properties_show=new Dictionary<string, string[]>();
        [STAThread]
        static void Main()
        {
            useful.get_dir_or_create("../MinecraftServers-Datas");
            Directory.SetCurrentDirectory("../MinecraftServers-Datas");
            load_properties_show();
            load_version_mapping();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            now = new menu();
            while (now != null)
            {
                Form tmp = now;
                now = null;
                Application.Run(tmp);
            }
            save_proc_show();
            save_version_mapping();
        }
        static public string[] get_entry_setting(string entry_name)
        {
            if (properties_show.ContainsKey(entry_name))
            {
                return properties_show[entry_name];
            }
            else
            {
                return null;
            }
        }
        static private void load_properties_show()
        {
            FileInfo procfile = useful.get_file_or_create("./properties.txt");
            using (StreamReader sr = new StreamReader(procfile.FullName))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line == "") continue;
                    string[] sps = line.Split(' ');
                    string entry_name = sps[0];
                    if (sps[1] == "array")
                    {
                        do
                        {
                            line = sr.ReadLine();
                        } while (line.Length == 0);
                        sps = line.Split(' ');
                        properties_show[entry_name] = sps;
                    }
                    else
                    {
                        properties_show[entry_name] = new string[0];
                    }
                }
            }
        }
        static private void save_proc_show()
        {
            using(StreamWriter sw =new StreamWriter("./properties.txt"))
            {
                foreach (var p in properties_show)
                {
                    sw.Write(p.Key);
                    if (p.Value.Length == 0) sw.WriteLine(" input");
                    else
                    {
                        sw.WriteLine(" array");
                        for (int i = 0; i < p.Value.Length; i++)
                        {
                            sw.Write(p.Value[i]);
                            if (i < p.Value.Length - 1) sw.Write(' ');
                            else sw.WriteLine();
                        }
                    }
                }
            }
        }
        public static Dictionary<string, string> version_mapping; 
        static private void load_version_mapping()
        {
            if (File.Exists("./version_mapping.json"))
            {
                using (StreamReader sr=new StreamReader("./version_mapping.json"))
                {
                    string data = sr.ReadToEnd();
                    version_mapping = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
                }
            }
            else
            {
                version_mapping = new Dictionary<string, string>();
            }
        }
        static private void save_version_mapping()
        {
            using(StreamWriter sw =new StreamWriter("./version_mapping.json"))
            {
                string data = JsonSerializer.Serialize(version_mapping);
                sw.Write(data);
            }
        }
    }
}
