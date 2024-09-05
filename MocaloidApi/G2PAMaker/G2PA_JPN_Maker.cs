using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MocaloidApi.G2PAMaker
{
    internal class G2PA_JPN_Maker
    {
        private static string TomlEncode(string str)
        {
            str = str.Replace("\\", "\\\\");
            return str;
        }
        private static readonly Dictionary<string, string[]> PhnStatic = new Dictionary<string, string[]>()
        {
            {"ki",["ky","i"] },
            {"ti",["ty","i"] },
            {"ni",["ny","i"] },
            {"hi",["hy","i"] },
            {"fi",["fy","i"] },
            {"mi",["my","i"] },
            {"ri",["ry","i"] },
            {"gi",["gy","i"] },
            {"g!i",["gy!","i"] },
            {"di",["dy","i"] },
            {"bi",["by","i"] },
            {"pi",["py","i"] }
        };
        private static readonly Dictionary<string, string> G2PA_Static = new Dictionary<string, string>()
        {
            {"ki","k' i" },
            {"ti","t' i" },
            {"ni","J i" },
            {"hi","C i" },
            {"fi","p\\ i" },
            {"mi","m' i" },
            {"ri","4' i" },
            {"gi","g' i" },
            {"g!i","N' i" },
            {"di","d' i" },
            {"bi","b' i" },
            {"pi","p' i" },
        };
        private static readonly Dictionary<string, string> G2PA_Map = new Dictionary<string, string>()
        {
            { "a","a" },
            { "i","i" },
            { "M","u" },
            { "e","e" },
            { "o","o" },


            {"k'","ky" },//only i列
            {"t'", "ty" },//only  i列
            {"J", "ny" },
            {"C", "hy" },
            {"p\\'", "fy" },
            {"m'", "my" },
            {"4'", "ry" },

            {"g'","gy" },//only i列
            {"N'","gy!" },//针对g行
            {"d'", "dy" },
            {"b'", "by" },
            {"p'", "py" },

            //Consonants
            {"k","k" },
            {"s","s" },
            {"t","t" },
            {"h", "h" },
            {"p\\", "f" },//fo列
            {"h\\", "h!" },//只有手输时有用
            {"m", "m" },
            {"j", "y" },
            {"4", "r" },
            {"w","w" },
            {"n","n"},
            {"N\\","n!"},
            //PConsoants
            {"g","g" },
            {"N","g!" },//只有手输时有用
            {"dz", "z" },
            {"z", "z!" },//只有手输时有用
            {"d", "d" },
            {"b", "b" },
            {"p", "p" },
            {"S", "sh" },
            {"Z", "j!" },//只有手输时有用
            {"dZ", "j" },
            {"ts", "ts" },
            {"tS", "ch" },

            {"Sil","R" }
        };
        private static readonly Dictionary<string, string[]> Translate = new Dictionary<string, string[]>()
        {
            { "a", ["あ","ア"]},
            { "i", ["い", "イ"] },
            { "u", ["う", "ウ"]},
            { "e", ["え", "エ"]},
            { "o", ["お", "オ", "を", "ヲ"]},

            { "n", ["ん", "ン"]},
            { "ba", ["ば","バ"]},
            { "bi", ["び", "ビ"]},
            { "bu", ["ぶ", "ブ"]},
            { "be", ["べ", "ベ"]},
            { "bo", ["ぼ", "ボ"]},

            { "cha", ["ちゃ", "チャ"]},
            { "chi", ["ち", "チ"]},

            { "chu", ["ちゅ", "チュ"]},
            { "che", ["ちぇ", "チェ"]},
            { "cho", ["ちょ", "チョ"]},
            { "da", ["だ", "ダ"]},
            { "di", ["でぃ", "ディ"]},
            { "du", ["どぅ", "ドゥ"]},
            { "de", ["で", "デ"]},
            { "do", ["ど", "ド"]},

            { "fa", ["ふぁ", "ファ"]},
            { "fi", ["ふぃ", "フィ"]},
            { "fu", ["ふ", "フ"]},
            { "fe", ["ふぇ", "フェ"]},
            { "fo", ["ふぉ", "フォ"]},

            { "ga", ["が", "ガ"]},
            { "gi", ["ぎ", "ギ"]},
            { "gu", ["ぐ", "グ"]},
            { "ge", ["げ", "ゲ"]},
            { "go", ["ご", "ゴ"]},

            { "ha", ["は", "ハ"]},
            { "hi", ["ひ", "ヒ"]},
            { "he", ["へ", "ヘ"]},
            { "ho", ["ほ", "ホ"]},

            { "ja", ["じゃ", "ジャ"]},
            { "ji", ["じ", "ジ"]},
            { "ju", ["じゅ", "ジュ"]},
            { "je", ["じぇ", "ジェ"]},
            { "jo", ["じょ", "ジョ"]},
            { "ka", ["か", "カ"]},
            { "ki", ["き",  "キ"]},
            { "ku", ["く", "ク"]},
            { "ke", ["け", "ケ"]},
            { "ko", ["こ", "コ"]},
            { "ma", ["ま", "マ"]},
            { "mi", ["み", "ミ"]},
            { "mu", ["む", "ム"]},
            { "me", ["め", "メ"]},
            { "mo", ["も", "モ"]},
            { "na", ["な", "ナ"]},
            { "ni", ["に", "ニ"]},
            { "nu", ["ぬ", "ヌ"]},
            { "ne", ["ね", "ネ"]},
            { "no", ["の", "ノ"]},
            { "pa", ["ぱ", "パ"]},
            { "pi", ["ぴ", "ピ"]},
            { "pu", ["ぷ", "プ"]},
            { "pe", ["ぺ", "ペ"]},
            { "po", ["ぽ", "ポ"]},
            { "ra", ["ら", "ラ"]},
            { "ri", ["り", "リ"]},
            { "ru", ["る", "ル"]},
            { "re", ["れ", "レ"]},
            { "ro", ["ろ","ロ"]},
            { "sa", ["さ", "サ"]},
            { "si", ["すぃ", "スィ"]},
            { "su", ["す", "ス"]},
            { "se", ["せ", "セ"]},
            { "so", ["そ", "ソ"]},
            { "sha", ["しゃ", "シャ"]},
            { "shi", ["し", "シ"]},
            { "shu", ["しゅ", "シュ"]},
            { "she", ["しぇ", "シェ"]},
            { "sho", ["しょ", "ショ"]},
            { "ta", ["た", "タ"]},
            { "ti", ["てぃ", "ティ"]},
            { "tu", ["とぅ", "トゥ"]},
            { "te", ["て", "テ"]},
            { "to", ["と", "ト"]},
            { "tsa", ["つぁ", "ツァ"]},
            { "tsi", ["つぃ", "ツィ"]},
            { "tsu", ["つ", "ツ"]},
            { "tse", ["つぇ", "ツェ"]},
            { "tso", ["つぉ", "ツォ"]},
            { "wa", ["わ", "ワ", "va"] },
            { "wi", ["うぃ", "ウィ", "vi"]},
            { "we", ["うぇ", "ウェ", "ve"] },
            { "wo", ["うぉ", "ウォ", "vo"] },
            { "ya", ["や", "ヤ"]},
            { "yu", ["ゆ", "ユ"]},
            { "ye", ["いぇ", "イェ"]},
            { "yo", ["よ", "ヨ"]},
            { "za", ["ざ", "ザ"]},
            { "zi", ["ずぃ", "ズィ"]},
            { "zu", ["ず", "ズ"]},
            { "ze", ["ぜ", "ゼ"]},
            { "zo", ["ぞ", "ゾ"]},
            { "bya", ["びゃ", "ビャ"]},
            { "byu", ["びゅ", "ビュ"]},
            { "bye", ["びぇ", "ビェ"]},
            { "byo", ["びょ", "ビョ"]},
            { "gya", ["ぎゃ", "ギャ"]},
            { "gyu", ["ぎゅ", "ギュ"]},
            { "gye", ["ぎぇ", "ギェ"]},
            { "gyo", ["ぎょ", "ギョ"]},
            { "hya", ["ひゃ", "ヒャ"]},
            { "hyu", ["ひゅ", "ヒュ"]},
            { "hye", ["ひぇ", "ヒェ"]},
            { "hyo", ["ひょ", "ヒョ"]},
            { "kya", ["きゃ", "キャ"]},
            { "kyu", ["きゅ", "キュ"]},
            { "kye", ["きぇ", "キェ"]},
            { "kyo", ["きょ", "キョ"]},
            { "mya", ["みゃ", "ミャ"]},
            { "myu", ["みゅ", "ミュ"]},
            { "mye", ["みぇ", "ミェ"]},
            { "myo", ["みょ", "ミョ"]},
            { "nya", ["にゃ", "ニャ"]},
            { "nyu", ["にゅ", "ニュ"]},
            { "nye", ["にぇ", "ニェ"]},
            { "nyo", ["にょ", "ニョ"]},
            { "pya", ["ぴゃ", "ピャ"]},
            { "pyu", ["ぴゅ", "ピュ"]},
            { "pye", ["ぴぇ", "ピェ"]},
            { "pyo", ["ぴょ", "ピョ"]},
            { "rya", ["りゃ", "リャ"]},
            { "ryu", ["りゅ", "リュ"]},
            { "rye", ["りぇ", "リェ"]},
            { "ryo", ["りょ", "リョ"]}
        };

        private static readonly List<string> EVECTable = new List<string>()
        {
            "#1","#2","#3",
            "#4","#5","#6",
            "#F","#+","#-"
        };


        public static void Build(string target_dir, Dictionary<string, List<string>> pstList, List<VoiceSample> Samples)
        {
            Dictionary<string, string> G2PA_Table = new Dictionary<string, string>();
            List<string> G2PA_Data = new List<string>();
            G2PA_Data.Add("[Lang]");
            G2PA_Data.Add("\"Type\" = \"Japanese\"");
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
                    if (Lyric != "" && !G2PA_Table.ContainsKey(Lyric))
                    {
                        G2PA_Table.Add(Lyric, String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", Lyric, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                        if (Translate.ContainsKey(Lyric)) foreach (string lrc in Translate[Lyric])
                            {
                                G2PA_Table.Add(lrc, String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", lrc, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                            };
                    }
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
                    if (Lyric != "" && !G2PA_Table.ContainsKey(Lyric))
                    {
                        G2PA_Table.Add(Lyric, String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", Lyric, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                        if (Translate.ContainsKey(Lyric)) foreach(string lrc in Translate[Lyric])
                            {
                                G2PA_Table.Add(lrc, String.Format("\"{0}\" = [\"{1}\", \"{2}\", \"{3}\"]", lrc, TomlEncode(PLC), TomlEncode(Lyric), TomlEncode(PLV)));
                            };
                    }
                }
            G2PA_Data.AddRange(G2PA_Table.Values);
            string G2PAFile = Path.Combine(target_dir, "g2pa_map.toml");
            using (FileStream fs = new FileStream(G2PAFile, FileMode.Create, FileAccess.Write))
            {
                string Content = String.Join("\r\n", G2PA_Data);
                byte[] arr = Encoding.UTF8.GetBytes(Content);
                fs.Position = 0;
                fs.Write(arr, 0, arr.Length);
            }
        }

        private static void miss()
        {
            //Assert
        }

        public static string GetSymbolChar(string Symbol, PhonemeSymbolType type)
        {
            string ret = Symbol;
            switch (type)
            {
                case PhonemeSymbolType.Vowel_Only:
                    {
                        string Lyric = "";
                        if (G2PA_Static.ContainsValue(Symbol)) foreach (var kv in G2PA_Static) { if (kv.Value == Symbol) { Lyric = kv.Key; break; } }
                        else if (G2PA_Map.ContainsKey(Symbol)) Lyric = G2PA_Map[Symbol];
                        else
                            miss();
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
                            else if (G2PA_Map.ContainsKey(c_v_a[c_v_a.Length - 1]))
                                lr = G2PA_Map[c_v_a[c_v_a.Length - 1]];
                            else
                                miss();
                            Lyric = lr;
                        }
                        if (Lyric != "") ret = Lyric;
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
                                else
                                    miss();
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
                            if (G2PA_Static.ContainsValue(pli))
                            {
                                foreach (var kv in G2PA_Static) { if (kv.Value == pli) { lr.Add(kv.Key); break; } }
                            }
                            else if (G2PA_Map.ContainsKey(pli)) lr.Add(G2PA_Map[pli]);
                            else
                                miss();
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
                        else
                            miss();
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
