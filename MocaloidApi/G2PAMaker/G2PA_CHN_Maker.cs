using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocaloidApi.G2PAMaker
{
    internal class G2PA_CHN_Maker
    {
        private static string TomlEncode(string str)
        {
            str = str.Replace("\\", "\\\\");
            return str;
        }
        private static readonly Dictionary<string, string[]> PhnStatic = new Dictionary<string, string[]>()
        {
            {"ju",["j","yu"]},
            {"qu",["q","yu"] },
            {"xu",["x","yu"] },
            {"nv",["n","yu"] },
            {"lv",["l","yu"] },


            {"jun",["j","yun"]},
            {"qun",["q","yun"] },
            {"xun",["x","yun"] },


            {"jue",["j","yue"]},
            {"que",["q","yue"] },
            {"xue",["x","yue"] },
            {"nve",["n","yue"] },
            {"lve",["l","yue"] },


            { "zi",["z","i@z"] },
            { "ci",["c","i@z"] },
            { "si",["s","i@z"] },

            { "zhi",["zh","i@r"] },
            { "chi",["ch","i@r"] },
            { "shi",["sh","i@r"] },
            { "ri",["r","i@r"] }
        };
        private static readonly Dictionary<string, string> G2PA_Static = new Dictionary<string, string>()
        {
            {"yi","i" },
            {"wu","u" },
            {"ya","ia" },
            {"ye","iE_r" },
            {"wa","ua" },
            {"wo","uo" },
            {"yao" ,"iAU"},
            {"you","i@U" },
            {"wai","uaI" },
            {"wei","uei" },
            {"yin","i_n" },
            {"yan","iE_n" },
            {"wan","ua_n" },
            {"wen","u@_n" },
            {"yuan","y{_n" },
            {"ying","iN" },
            {"yang","iAN" },
            {"wang","uAN" },
            {"weng","u@N" },
            {"yong","iUN" },

            {"yu","y" },
            {"ju","ts\\ y" },
            {"qu","ts\\_h y" },
            {"xu","s\\ y" },
            {"nv","n y" },
            {"lv","l y" },

            {"yun","y_n" },
            {"jun","ts\\ y_n" },
            {"qun","ts\\_h y_n" },
            {"xun","s\\ y_n" },

            { "yue","yE_r" },
            {"jue","ts\\ yE_r" },
            {"que","ts\\_h yE_r" },
            {"xue","s\\ yE_r" },
            {"nve","n yE_r" },
            { "lve","l yE_r" },

            { "zi","ts i\\" },
            {"ci","ts_h i\\" },
            {"si","s i\\" },

            {"zhi","ts` i`" },
            {"chi","ts`_h i`" },
            {"shi","s` i`" },
            {"ri","z` i`" }
        };
        private static readonly Dictionary<string, string> G2PA_Map = new Dictionary<string, string>()
        {
            { "a","a" },
            {"o","o" },
            {"7","e" },
            {"i`","i@r" },//zhi/chi/shi/ri
            {"i\\","i@z" },//zi,ci,si
            {"@`","er" },//er
            {"aI","ai" },
            {"ei","ei" },
            {"AU","ao" },
            {"@U","ou" },
            {"a_n","an" },
            {"@_n","en" },
            {"AN", "ang" },
            {"@N", "eng" },
            {"UN", "ong" },
            {"i", "i" },
            {"ia", "ia" },
            {"iE_r","ie" },
            {"iAU", "iao" },
            {"i@U", "iu" },
            {"iE_n", "ian" },
            {"i_n", "in" },
            {"iAN", "iang" },
            {"iN", "ing" },
            {"iUN", "iong" },
            {"u", "u" },
            {"ua", "ua" },
            {"uo", "uo" },
            {"uaI", "uai" },
            {"uei", "ui" },
            {"ua_n", "uan" },
            {"u@_n", "un" },
            {"uAN", "uang" },
            {"u@N", "ueng" },
            {"y", "v" },//ju,qu,xu,nv,lv,yu
            {"yE_r","ve" },//jue,que,xue,nve,lve,yue
            {"y{_n","uan"},
            {"un@x","y_n" },//xun,jun,jun
            //Consonants:
            {"ts`_h","ch" },
            {"ts_h","c" },
            {"t","d" },
            {"x","h" },
            {"ts\\","j" },
            {"k_h","k" },
            {"m","m" },
            {"n","n" },
            {"p_h","p" },
            {"ts\\_h","q" },
            {"z`","r" },
            {"s`","sh" },
            {"s","s" },
            {"t_h","t" },
            {"s\\","x" },
            {"ts","z" },
            {"ts`","zh" },
            {"l","l" },
            {"k","g" },
            {"p","b" },
            {"f","f" },
            {"Sil","R" }
        };
        public static void Build(string target_dir, Dictionary<string, List<string>> pstList, List<VoiceSample> Samples)
        {
            Dictionary<string, string> G2PA_Table = new Dictionary<string, string>();
            List<string> G2PA_Data = new List<string>();
            G2PA_Data.Add("[Lang]");
            G2PA_Data.Add("\"Type\" = \"Chinese\"");
            G2PA_Data.Add("");
            G2PA_Data.Add("[G2PA]");
            if (pstList.ContainsKey("V_CV")) foreach (var pl in pstList["V_CV"])
                {
                    string Lyric = "";
                    string PLC = "";
                    string PLV = "";
                    if (G2PA_Static.ContainsValue(pl)) foreach (var kv in G2PA_Static)
                        {
                            if (kv.Value == pl)
                            {
                                Lyric = kv.Key; PLC = kv.Key; PLV = kv.Key;
                                if (PhnStatic.ContainsKey(Lyric)) { PLC = PhnStatic[Lyric][0]; PLV = PhnStatic[Lyric][1]; }
                                break;
                            }
                        }
                    else
                    {
                        string[] c_v_a = pl.Split(" ");
                        string pli = c_v_a[c_v_a.Length - 1];
                        Lyric = "";
                        if (G2PA_Static.ContainsValue(pli))
                        {
                            foreach (var kv in G2PA_Static) { if (kv.Value == pli) { Lyric = kv.Key; break; } }
                        }
                        else if (G2PA_Map.ContainsKey(pli))
                        {
                            Lyric = G2PA_Map[pli];
                        }
                        PLC = Lyric;
                        PLV = Lyric;
                    }
                    if (Lyric != "" && !G2PA_Table.ContainsKey(Lyric)) G2PA_Table.Add(Lyric,String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", Lyric, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                }
            if (pstList.ContainsKey("CV")) foreach (var pl in pstList["CV"])
                {
                    string Lyric = "";
                    string PLC = "";
                    string PLV = "";
                    if (G2PA_Static.ContainsValue(pl)) foreach (var kv in G2PA_Static)
                        {
                            if (kv.Value == pl)
                            {
                                Lyric = kv.Key; PLC = kv.Key; PLV = kv.Key;
                                if (PhnStatic.ContainsKey(Lyric)) { PLC = PhnStatic[Lyric][0]; PLV = PhnStatic[Lyric][1]; }
                                break;
                            }
                        }
                    else
                    {
                        string[] c_v_a = pl.Split(" ");
                        string lr = "";
                        List<string> lc = new List<string>();
                        foreach (string pli in c_v_a)
                        {
                            if (G2PA_Static.ContainsValue(pli))
                            {
                                foreach (var kv in G2PA_Static) { if (kv.Value == pli) { lc.Add(kv.Key); break; } }
                            }
                            else if (G2PA_Map.ContainsKey(pli))
                            {
                                lc.Add(G2PA_Map[pli]);
                            }

                            if (G2PA_Map.ContainsKey(pli)) lr += G2PA_Map[pli];
                        }
                        PLC = lc[0];
                        PLV = lc[lc.Count - 1];
                        Lyric = lr;
                    }
                    if (Lyric != "" && !G2PA_Table.ContainsKey(Lyric)) G2PA_Table.Add(Lyric,String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", Lyric, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                }
            G2PA_Data.AddRange(G2PA_Table.Values);
            string G2PAFile = Path.Combine(target_dir, "g2pa_map.toml");
            using (FileStream fs = new FileStream(G2PAFile, FileMode.Create, FileAccess.Write))
            {
                string Content = String.Join("\r\n", G2PA_Data);
                byte[] arr = Encoding.ASCII.GetBytes(Content);
                fs.Position = 0;
                fs.Write(arr, 0, arr.Length);
            }
        }
    
        public static string GetSymbolChar(string Symbol,PhonemeSymbolType type)
        {
            string ret = Symbol;
            switch(type)
            {
                case PhonemeSymbolType.Vowel_Only:
                    {
                        string Lyric = "";
                        if (G2PA_Static.ContainsValue(Symbol)) foreach (var kv in G2PA_Static) { if (kv.Value == Symbol) { Lyric = kv.Key; break; } }
                        else if (G2PA_Map.ContainsKey(Symbol)) Lyric = G2PA_Map[Symbol];
                        ret = Lyric;
                    }
                    break;
                case PhonemeSymbolType.Vowel__Consonant_Vowel:
                    {
                        string Lyric = "";
                        if (G2PA_Static.ContainsValue(Symbol)) foreach (var kv in G2PA_Static) { if (kv.Value == Symbol) { Lyric = kv.Key; break; } }
                        else
                        {
                            string[] c_v_a = Symbol.Split(" ");
                            string lr = ""; 
                            if (G2PA_Static.ContainsValue(c_v_a[c_v_a.Length - 1])) 
                                foreach (var kv in G2PA_Static) { if (kv.Value == c_v_a[c_v_a.Length - 1]) { lr = kv.Key; } }
                            else if (G2PA_Map.ContainsKey(c_v_a[c_v_a.Length-1])) 
                                lr = G2PA_Map[c_v_a[c_v_a.Length - 1]];
                            Lyric = lr;
                        }
                        if(Lyric!="") ret = Lyric;
                    }
                    break;
                case PhonemeSymbolType.Consonant_Vowel:
                    {
                        string Lyric = "";
                        if (G2PA_Static.ContainsValue(Symbol)) foreach (var kv in G2PA_Static) { if (kv.Value == Symbol) { Lyric = kv.Key; break; } }
                        else
                        {
                            string[] c_v_a = Symbol.Split(" ");
                            string lr = "";
                            foreach (string pli in c_v_a)
                            {
                                if (G2PA_Map.ContainsKey(pli)) lr += G2PA_Map[pli];
                            }
                            Lyric = lr;
                        }
                        ret = Lyric;
                    }
                    break;
                case PhonemeSymbolType.Vowel__Vowel_Consonant:
                case PhonemeSymbolType.Vowel_Consonant:
                    {
                        string[] c_v_a = Symbol.Split(" ");
                        List<string> lr = new List<string>();
                        foreach (string pli in c_v_a)
                        {
                            if(G2PA_Static.ContainsValue(pli))
                            {
                                foreach (var kv in G2PA_Static) { if (kv.Value == pli) { lr.Add(kv.Key); break; } }
                            }
                            else if (G2PA_Map.ContainsKey(pli)) lr.Add(G2PA_Map[pli]);
                        }
                        ret = String.Join(" ", lr);
                    }
                    break;
                case PhonemeSymbolType.Vowel_Rest:
                    {
                        string[] c_v_a = Symbol.Split(" ");
                        List<string> lr = new List<string>();
                        if (G2PA_Static.ContainsValue(c_v_a[0]))
                        {
                            foreach (var kv in G2PA_Static) { if (kv.Value == c_v_a[0]) { lr.Add(kv.Key); break; } }
                        }
                        else if (G2PA_Map.ContainsKey(c_v_a[0])) lr.Add(G2PA_Map[c_v_a[0]]);
                        lr.Add("R");
                        ret = String.Join(" ", lr);
                    }
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}
