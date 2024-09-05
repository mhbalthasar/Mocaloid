using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;

namespace Mocaloid.UtauPlugin
{
    public class SplitUST
    {
        public static double Limit(double Value, double Min, double Max)
        {
            if (Value >= Max) return Max;
            if (Value <= Min) return Min;
            return Value;
        }

        public SplitUST() { }

        public UstInfo DoAction(UstInfo ustInfo)
        {
            UstInfo ret = ustInfo;
            List<List<UstNotes>> newList = new List<List<UstNotes>>();
            for (int i = 0; i < ret.Notes.Count; i++) newList.Add(new List<UstNotes>() { ret.Notes[i][0].Clone() });//DeepCopy

            string TomlG2p = Path.Combine(ustInfo.Setting.VoiceDir, "g2pa_map.toml");
            if(!File.Exists(TomlG2p)) { MessageBox.Show("Cannot load G2PA Map!");return ret; }
            string dictData = File.ReadAllText(TomlG2p);
            TomlTable? model;
            if (!Toml.TryToModel(dictData, out model, out var message))
            {
                MessageBox.Show("Cannot read G2PA Map!\r\n"+message);
                return ret;
            }
            string LangType = "";
            if (model.TryGetValue("Lang", out object langObj)) { if (((TomlTable)langObj).TryGetValue("Type", out object oLangType)) { LangType = ((string)oLangType).ToLower(); }; }
            else
            {
                MessageBox.Show("Cannot read G2PA Map!");
                return ret;
            }

            TomlTable g2paTable;
            if (model.TryGetValue("G2PA", out object g2paObj)) g2paTable = (TomlTable)g2paObj; else
            {
                MessageBox.Show("Cannot read G2PA Map!");
                return ret;
            }

            if (LangType == "chinese" || LangType == "japanese") for (int i = 0; i < ret.Notes.Count; i++)
                {
                    UstNotes? curNote = ret.Notes[i][0];
                    UstNotes? nextNote = i + 1 >= ret.Notes.Count ? null : ret.Notes[i + 1][0];

                    if (curNote.Lyric == "R") continue;
                    if (curNote.RawSection == "PREV" || curNote.RawSection == "NEXT") continue;
                    string NextC = "";
                    if (nextNote != null)
                    {
                        if (nextNote.Lyric == "R") NextC = "R";
                        else if (g2paTable.TryGetValue(nextNote.Lyric, out object gr))
                        {
                            TomlArray pnArray = (TomlArray)gr;
                            var gc = (string)pnArray[0];
                            var gv = (string)pnArray[2];
                            NextC = gc;
                        }
                    }
                    else
                    {
                        NextC = "R";
                    }
                    if (g2paTable.TryGetValue(curNote.Lyric, out object gCurN))
                    {
                        TomlArray pnArray = (TomlArray)gCurN;
                        var gc = (string)pnArray[0];
                        var gcv = (string)pnArray[1];
                        var gv = (string)pnArray[2];

                        string CVSymbol = gcv;
                        string VSymbol = gv;
                        string VCSymbol = gv + " " + NextC;

                        int totalLen = curNote.Length;

                        if (false && totalLen > 300 && CVSymbol!=VSymbol)
                        {
                            //Length Enouch && Not the VV
                            int CVLen = (int)Limit(totalLen * 0.3, 120.0, 240.0);
                            int sailyLen = totalLen - CVLen;
                            int VCLen = (int)Limit(sailyLen * 0.2, 60.0, 180.0);
                            int VLen = sailyLen - VCLen;

                            UstNotes partVNote = newList[i][0];
                            newList[i].Clear();

                            UstNotes partCVNote = partVNote.Clone();
                            partCVNote.Length = CVLen;
                            partCVNote.Lyric = CVSymbol;
                            partCVNote.RawSection = "INSERT";
                            newList[i].Add(partCVNote);

                            partVNote.Lyric = VSymbol;
                            partVNote.Length = VLen;
                            newList[i].Add(partVNote);

                            UstNotes partVCNote = partVNote.Clone();
                            partVCNote.Length = VCLen;
                            partVCNote.Lyric = VCSymbol;
                            partVCNote.RawSection = "INSERT";
                            newList[i].Add(partVCNote);
                        }else
                        {
                            int sailyLen = totalLen;
                            int VCLen = (int)Limit(sailyLen * 0.2, 0, 180.0);
                            int VLen = sailyLen - VCLen;

                            UstNotes partVNote = newList[i][0];
                            newList[i].Clear();

                            partVNote.Lyric = CVSymbol;
                            partVNote.Length = VLen;
                            newList[i].Add(partVNote);

                            UstNotes partVCNote = partVNote.Clone();
                            partVCNote.Length = VCLen;
                            partVCNote.Lyric = VCSymbol;
                            partVCNote.RawSection = "INSERT";
                            newList[i].Add(partVCNote);
                        }
                    }
                }

            ret.Notes = newList;
            return ret;
        }
    }
}
