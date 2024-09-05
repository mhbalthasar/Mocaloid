using MocaloidApi;
using MocaloidApi.G2PAMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mocaloid
{
    public class MocaloidBuilder
    {
        public static bool BuildUtau(string src_ddi, string target_folder, out string error, bool overwrite = false, bool isGCM = false,int selLang=-1)
        {
            try
            {
                if (!Path.Exists(target_folder)) Directory.CreateDirectory(target_folder);
            }
            catch { error = "Create target folder failure!"; return false; }
            error = "";

            //CheckHash
            DDInterface di = new DDInterface(src_ddi);
            string hash = di.GetDDIHash();
            if (hash == "") { error = "Cannot load Hash, LibDDI initialize failure";return false; }
            string verifyFile = Path.Combine(target_folder, hash + ".dat");
            if (!overwrite && System.IO.File.Exists(verifyFile)) return true;

            if(di.GetDasiyFileVersion()!=DDInterface.EnumDaisyFileVersion.VOCALOID3)
            {
                error = "Unsupported Daisy VoiceBank Format!"; return false;
            }

            //Analysis
            if (!di.ParseDDI(out error)) return false;
            var Lang = di.GuessLanguage();
            if (selLang >= 0 && selLang <= 4) Lang = (VoiceBankLanguage)selLang;
            di.FixVowelTable(Lang);
            SampleFilterType sampleFilter = SampleFilterType.DEFAULT;
            switch(Lang)
            {
                case VoiceBankLanguage.Japanese:
                case VoiceBankLanguage.Chinese:
                    sampleFilter = SampleFilterType.CVVC;
                    break;
                default:
                    {
                        error = "Uninstantiated phonemizer for this language";
                        return false;
                    }
            }
            List<int> pitchGroupNumbers = di.FillAvaliableGroup(sampleFilter,isGCM);
            var samples = di.GetSamples(sampleFilter);

            //ExtractSample To Utau
            Dictionary<string, List<string>> otoList = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> pstList = new Dictionary<string, List<string>>();
            Dictionary<string, Tuple<int, int>> replaceRecordList = new Dictionary<string, Tuple<int, int>>();
            for (int i = 0; i < samples.Count; i++)
            {
                if (samples[i].PitchGroup.Count == 0) 
                    continue;
                string PST = "";
                switch (samples[i].SymbolType)
                {
                    case PhonemeSymbolType.Vowel_Only: PST = "VO"; break;
                    case PhonemeSymbolType.Vowel_Consonant: PST = "VC"; break;
                    case PhonemeSymbolType.Consonant_Vowel: PST = "CV"; break;
                    case PhonemeSymbolType.Vowel_Rest: PST = "VR"; break;
                    case PhonemeSymbolType.Rest_Vowel: PST = "RV"; break;
                    case PhonemeSymbolType.Rest_Consonant: PST = "RC"; break;
                    case PhonemeSymbolType.Consonant_Rest: PST = "CR"; break;
                    case PhonemeSymbolType.Vowel_Vowel: PST = "VV"; break;
                    case PhonemeSymbolType.Vowel__Consonant_Vowel: PST = "V_CV"; break;
                    case PhonemeSymbolType.Vowel__Vowel_Consonant: PST = "V_VC"; break;
                    default: PST = "OT"; break;
                }
                string FileName = PST + "__" + samples[i].PhonemeFileName + "__" + samples[i].PitchName.ToString() + ".wav";
                {
                    if (!pstList.ContainsKey(PST)) pstList.Add(PST, new List<string>());
                    if (!pstList[PST].Contains(samples[i].PhonemeName)) pstList[PST].Add(samples[i].PhonemeName);
                }

                foreach (string PitchGroup in samples[i].PitchGroup)
                {
                    string PitchDir = Path.Combine(target_folder, PitchGroup);
                    if (!Path.Exists(PitchDir)) Directory.CreateDirectory(PitchDir);

                    string SndPath = Path.Combine(PitchDir, FileName);
                    if (!Path.Exists(SndPath)) samples[i].ExportSnd(SndPath);

                    if (!otoList.ContainsKey(PitchGroup)) otoList.Add(PitchGroup, new List<string>());
                    switch (Lang)
                    {
                        case VoiceBankLanguage.Chinese:
                            {
                                string CVVCSymbol = G2PA_CHN_Maker.GetSymbolChar(samples[i].PhonemeName, samples[i].SymbolType) + "_" + PitchGroup;
                                string otoLine = FileName + "=" + CVVCSymbol + "," + samples[i].ExportLabel().ToString();
                                if (samples[i].SymbolType == PhonemeSymbolType.Vowel__Consonant_Vowel || samples[i].SymbolType == PhonemeSymbolType.Vowel_Only)
                                {
                                    int vvLevel = (samples[i].SymbolType == PhonemeSymbolType.Vowel_Only) ? 0 : (samples[i].SymbolType == PhonemeSymbolType.Vowel__Consonant_Vowel && samples[i].Phoneme[0].Symbol != samples[i].Phoneme[1].Symbol) ? 1 : 2;
                                    if (replaceRecordList.ContainsKey(CVVCSymbol))
                                    {
                                        var curTp = replaceRecordList[CVVCSymbol];
                                        if (curTp.Item2 < vvLevel)
                                        {
                                            otoList[PitchGroup][curTp.Item1] = otoLine;
                                            replaceRecordList[CVVCSymbol] = new Tuple<int, int>(curTp.Item1, vvLevel);
                                        }
                                    }
                                    else
                                    {
                                        otoList[PitchGroup].Add(otoLine);
                                        replaceRecordList.Add(CVVCSymbol, new Tuple<int, int>(otoList[PitchGroup].Count - 1, vvLevel));
                                    }
                                }
                                else
                                {
                                    otoList[PitchGroup].Add(otoLine);
                                }
                            }
                            break;
                        case VoiceBankLanguage.Japanese:
                            {
                                string CVVCSymbol = G2PA_JPN_Maker.GetSymbolChar(samples[i].PhonemeName, samples[i].SymbolType) + "_" + PitchGroup;
                                string otoLine = FileName + "=" + CVVCSymbol + "," + samples[i].ExportLabel().ToString();
                                if (samples[i].SymbolType == PhonemeSymbolType.Vowel__Consonant_Vowel || samples[i].SymbolType == PhonemeSymbolType.Vowel_Only)
                                {
                                    int vvLevel = (samples[i].SymbolType == PhonemeSymbolType.Vowel_Only) ? 0 : (samples[i].SymbolType == PhonemeSymbolType.Vowel__Consonant_Vowel && samples[i].Phoneme[0].Symbol != samples[i].Phoneme[1].Symbol) ? 1 : 2;
                                    if (replaceRecordList.ContainsKey(CVVCSymbol))
                                    {
                                        var curTp = replaceRecordList[CVVCSymbol];
                                        if (curTp.Item2 < vvLevel)
                                        {
                                            otoList[PitchGroup][curTp.Item1] = otoLine;
                                            replaceRecordList[CVVCSymbol] = new Tuple<int, int>(curTp.Item1, vvLevel);
                                        }
                                    }
                                    else
                                    {
                                        otoList[PitchGroup].Add(otoLine);
                                        replaceRecordList.Add(CVVCSymbol, new Tuple<int, int>(otoList[PitchGroup].Count - 1, vvLevel));
                                    }
                                }
                                else
                                {
                                    otoList[PitchGroup].Add(otoLine);
                                }
                            }
                            break;
                    }

                }
            }

            //BuildOto.ini
            {
                foreach (var otlKv in otoList)
                {
                    string otoFile = Path.Combine(target_folder, otlKv.Key, "oto.ini");
                    using (FileStream fs = new FileStream(otoFile, FileMode.Create, FileAccess.Write))
                    {
                        string Content = String.Join("\r\n", otlKv.Value);
                        byte[] arr = Encoding.ASCII.GetBytes(Content);
                        fs.Position = 0;
                        fs.Write(arr, 0, arr.Length);
                    }
                }
            }

            //BuildPrefix.Map
            //UTAU C1=24 C7=96
            {
                string prefixFile = Path.Combine(target_folder, "prefix.map");
                List<string> PrefixLines = new List<string>();
                List<int> pitchGroups = pitchGroupNumbers;
                int curIndex = 0;
                for (int p = 24; p <= 96; p++)
                {
                   /* if (curIndex + 1 < pitchGroups.Count && p >= pitchGroups[curIndex + 1])
                    {
                        curIndex++;
                    }*/
                    int distance = Math.Abs(p - pitchGroups[curIndex]);
                    for(int g = 1; g < pitchGroups.Count; g++)
                    {
                        int nt = Math.Abs(p - pitchGroups[g]);
                        if (nt < distance) { distance = nt; curIndex = g; }
                    }
                    string curPitch = GetPitchName(p);
                    string curSuffix = "_" + GetPitchName(pitchGroups[curIndex]);
                    PrefixLines.Add(curPitch + "\t\t" + curSuffix);
                }
                using (FileStream fs = new FileStream(prefixFile, FileMode.Create, FileAccess.Write))
                {
                    PrefixLines.Reverse();
                    string Content = String.Join("\r\n", PrefixLines);
                    byte[] arr = Encoding.ASCII.GetBytes(Content);
                    fs.Position = 0;
                    fs.Write(arr, 0, arr.Length);
                }
            }

            //BuildCharacter.txt
            {
                string characterFile = Path.Combine(target_folder, "character.txt");
                List<string> characterLines = new List<string>();
                characterLines.Add("name=" + di.GetName());
                characterLines.Add("author=Mocaloid Generator");
                characterLines.Add("language="+Lang.ToString());
                using (FileStream fs = new FileStream(characterFile, FileMode.Create, FileAccess.Write))
                {
                    string Content = String.Join("\r\n", characterLines);
                    byte[] arr = Encoding.ASCII.GetBytes(Content);
                    fs.Position = 0;
                    fs.Write(arr, 0, arr.Length);
                }
                string otoFile = Path.Combine(target_folder, "oto.ini");//Leave a empty
                using (FileStream fs = new FileStream(otoFile, FileMode.Create, FileAccess.Write)) { }
            }

            //Buildmocaloid.ini
            {
                string mocaloidFile = Path.Combine(target_folder, "mocaloid.ini");
                List<string> mocaloidLines = new List<string>();
                mocaloidLines.Add("[Info]");
                mocaloidLines.Add("Name=" + di.GetName());
                mocaloidLines.Add("Hash=" + di.GetDDIHash());
                mocaloidLines.Add("Language=" + Lang.ToString());
                List<int> pitchGroups = pitchGroupNumbers;
                List<string> pitchGroupStr= new List<string>();
                foreach (int i in pitchGroups) { pitchGroupStr.Add(GetPitchName(i)); }
                mocaloidLines.Add("Pitchs=" + String.Join(",", pitchGroupStr));
                mocaloidLines.Add("Pmode=" + (isGCM?"PIS":"SFS"));

                mocaloidLines.Add("");
                mocaloidLines.Add("[Phonemes]");
                List<string> Vowels = new List<string>();
                foreach(var i in di.VoiceSamples) { if(i.PhoneType==PhoneType.Stationary && !Vowels.Contains(i.Phoneme[0].Symbol)) { Vowels.Add(i.Phoneme[0].Symbol); } }
                List<string> Consonants = new List<string>();
                List<string> Rests = new List<string>();
                string[] def_rest = new string[] {"sil","asp","br","?","*" }; 
                foreach (var i in di.VoiceSamples) { if (i.PhoneType == PhoneType.DiPhoneArticulation) { foreach (var j in i.Phoneme)
                        {
                            if (Vowels.Contains(j.Symbol)) continue;
                            if (def_rest.Contains(j.Symbol.ToLower())) { if(!Rests.Contains(j.Symbol))Rests.Add(j.Symbol); }
                            else if(!Consonants.Contains(j.Symbol)) Consonants.Add(j.Symbol);
                        }
                    } }
                mocaloidLines.Add("Vowels="+string.Join(",",Vowels));
                mocaloidLines.Add("Consonants=" + string.Join(",", Consonants));
                mocaloidLines.Add("Rests=" + string.Join(",", Rests));

                using (FileStream fs = new FileStream(mocaloidFile, FileMode.Create, FileAccess.Write))
                {
                    string Content = String.Join("\r\n", mocaloidLines);
                    byte[] arr = Encoding.ASCII.GetBytes(Content);
                    fs.Position = 0;
                    fs.Write(arr, 0, arr.Length);
                }
            }
            
            switch(Lang)
            {
                case VoiceBankLanguage.Chinese:
                    G2PA_CHN_Maker.Build(target_folder, pstList, di.VoiceSamples);
                    break;

                case VoiceBankLanguage.Japanese:
                    G2PA_JPN_Maker.Build(target_folder, pstList, di.VoiceSamples);
                    break;
            }

            return true;
        }


        private static string GetPitchName(double pitchnumber)
        {
            bool isVoice = false;
            // 音高名称
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            // 计算给定频率与A4的音分数差
            double noteDiff = Math.Round(pitchnumber);

            // 计算最接近的音高
            int noteNumber = (int)Math.Round(noteDiff);
            noteNumber += (isVoice) ? 0 : +12;

            // 计算音高名称和八度
            int noteIndex = noteNumber % 12;
            int octave = -2 + (noteNumber) / 12;

            // 格式化输出
            return $"{noteNames[noteIndex]}{octave}";
        }
    }
}
