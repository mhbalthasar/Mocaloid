using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocaloidApi.G2PAMaker
{
    internal class G2PA_ENG_Maker
    {
        private static string TomlEncode(string str)
        {
            str = str.Replace("\\", "\\\\");
            return str;
        }

        public static void Build(string target_dir, Dictionary<string, List<string>> pstList, List<VoiceSample> Samples)
        {
            Dictionary<string, string> G2PA_Table = new Dictionary<string, string>();
            List<string> G2PA_Data = new List<string>();
            G2PA_Data.Add("[Lang]");
            G2PA_Data.Add("\"Type\" = \"English\"");
            G2PA_Data.Add("");
            G2PA_Data.Add("[G2PA]");
            string G2PAFile = Path.Combine(target_dir, "g2pa_map.toml");
            using (FileStream fs = new FileStream(G2PAFile, FileMode.Create, FileAccess.Write))
            {
                string Content = String.Join("\r\n", G2PA_Data);
                byte[] arr = Encoding.UTF8.GetBytes(Content);
                fs.Position = 0;
                fs.Write(arr, 0, arr.Length);
            }
        }

        public static string GetSymbolChar(string Symbol, PhonemeSymbolType type)
        {
            string ret = Symbol;
            switch (type)
            {
                case PhonemeSymbolType.Vowel_Rest:
                    {
                        string[] c_v_a = Symbol.Split(" ");
                        List<string> lr = new List<string>();
                        lr.Add(c_v_a[0]);
                        lr.Add("-");
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
