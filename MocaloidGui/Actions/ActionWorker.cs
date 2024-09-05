using Avalonia.Remote.Protocol;
using Mocaloid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MocaloidGui;

public class ActionWorker
{
    public static event Action<bool>? Completed;
    public static event Action<string>? Started;
    public static event Action<string>? Error;
    public static void StartBuildMocaloid(string inputDDI, string outputDir,bool isGCM,int selLang)
    {
        Task.Run(() => {
            Started?.Invoke("Building Mocaloid");
            string error = "";
            if (MocaloidBuilder.BuildUtau(inputDDI, outputDir, out error, true,isGCM,selLang))
            {
                Completed?.Invoke(true);
            }else
            {
                Error?.Invoke(error);
            }
        });
    }

    public static void StartCleanMocaloid(string outputDir)
    {

        Task.Run(() => {
            Started?.Invoke("Cleannig Mocaloid");
            string error = "";

            List<string> PitchList = new List<string>();
            using (FileStream fs = new FileStream(Path.Combine(outputDir, "mocaloid.ini"), FileMode.Open, FileAccess.ReadWrite))
            {
                using(TextReader tr = new StreamReader(fs))
                {
                    string? tl;
                    while((tl=tr.ReadLine())!=null)
                    {
                        if(tl.StartsWith("Pitchs"))
                        {
                            string pitl = tl.Substring(7);
                            PitchList.AddRange(pitl.Split(","));
                        }
                    }
                }
            }

            List<string> Dirs = new List<string>();
            List<string> Files = new List<string>();
            foreach(var p in PitchList)
            {
                if(Directory.Exists(Path.Combine(outputDir,p)))
                {
                    Dirs.Add(Path.Combine(outputDir, p));
                }
            }
            string[] bFiles = {"character.txt","g2pa_map.toml","mocaloid.ini","oto.ini","prefix.map" };
            foreach(var b in bFiles)
            {
                if(File.Exists(Path.Combine(outputDir,b)))
                {
                    Files.Add(Path.Combine(outputDir, b));
                }
            }
            try
            {
                foreach (string s in Dirs) Directory.Delete(s,true);
                foreach (string s in Files) File.Delete(s);
            }
            catch(Exception ex) { 
                Error?.Invoke(ex.Message);
                return;
            }
            Completed?.Invoke(true);
        });
    }
}
