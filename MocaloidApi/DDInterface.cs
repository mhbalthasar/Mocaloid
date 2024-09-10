using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MocaloidApi
{
    public class DDInterface
    {
        const string dllPath= "libddi.dll";


        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr AnalysisDDI([MarshalAs(UnmanagedType.LPStr)] string ddiFile, out IntPtr error, [Out] out int vss_size);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeDDI(IntPtr vss);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool HashDDI([MarshalAs(UnmanagedType.LPStr)] string ddiFile, out IntPtr ddiHash);

        private string DDIPath = "";
        private string DDBPath = "";
        private string DDIHash = "";

        public DDInterface(string DDIPath)
        {
            this.DDIPath = DDIPath;
            this.DDBPath = Path.Combine(Path.GetDirectoryName(DDIPath), Path.GetFileNameWithoutExtension(DDIPath) + ".ddb");
        }

        public string GetName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(this.DDIPath);
        }

        public string GetDDIHash()
        {
            if (DDIHash.Length == 32) return DDIHash;
            try
            {
                HashDDI(this.DDIPath, out IntPtr ddiHashPtr);
                if (ddiHashPtr != IntPtr.Zero) DDIHash = Marshal.PtrToStringAnsi(ddiHashPtr);
                return DDIHash;
            }
            catch { return ""; }
        }
                
        [StructLayout(LayoutKind.Sequential)]
        public struct VoiceSampleStruct
        {
            public int phoneType; // 0:sta, 1:diphone_art, 2:triphone_art
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public char[] phonemeBlocks; // oneBlock is 256, maxBlockCount is 4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public char[] phonemeEncodedBlocks; // oneBlock is 256, maxBlockCount is 4;
            public int phonemeCount; // blockCount;
            [MarshalAs(UnmanagedType.I4)]
            public PhonemeSymbolType phonemeType;
            public float relPitch;
            public float pitch;
            public float tempo;
            public int sndSampleRate;
            public int sndChannel;
            public long sndLength;
            public long sndOffset;
            public float sndLBound;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] sndSectionData; // EachBlock 4; maxBlockCount is 4;
        }

        public enum EnumDaisyFileVersion
        {
            VOCALOID3,
            VOCALOID2,
            VOCALOID_MOBILE,
            OTHER
        }
        public EnumDaisyFileVersion GetDasiyFileVersion()
        {
            EnumDaisyFileVersion ret = EnumDaisyFileVersion.OTHER;

            using (FileStream fs = new FileStream(DDIPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(BinaryReader br=new BinaryReader(fs))
                {
                    br.ReadBytes(8);
                    string cr4=new String(br.ReadChars(4));
                    if(cr4=="DBSe")
                    {
                        ret = EnumDaisyFileVersion.VOCALOID3;
                    }
                }
            }
            return ret;
        }
        public bool ParseDDI(out string error)
        {
            error = "";
            int vss_size;
            IntPtr ptr = AnalysisDDI(DDIPath, out IntPtr errorPtr, out vss_size);
            if (ptr == IntPtr.Zero)
            {
                if (errorPtr != IntPtr.Zero) error = Marshal.PtrToStringAnsi(errorPtr);
                return false;
            }

            int structSize = Marshal.SizeOf(typeof(VoiceSampleStruct));
            for (int i = 0; i < vss_size; i++)
            {
                IntPtr sPtr = new IntPtr(ptr.ToInt64() + i * structSize);
                VoiceSampleStruct vss = Marshal.PtrToStructure<VoiceSampleStruct>(sPtr);
                VoiceSample vs = CreateVS(vss);
                VoiceSamples.Add(vs);
                if(vs.PhoneType==PhoneType.DiPhoneArticulation && vs.SymbolType==PhonemeSymbolType.Vowel_Vowel)
                {
                    VoiceSample vs2 = CreateVS(vss);
                    vs2.SymbolType = PhonemeSymbolType.Vowel__Consonant_Vowel;
                    VoiceSamples.Add(vs2);

                    VoiceSample vs3 = CreateVS(vss);
                    vs3.SymbolType = PhonemeSymbolType.Vowel__Vowel_Consonant;
                    VoiceSamples.Add(vs3);
                }
                
            }
            FreeDDI(ptr);
            return true;
        }

        public void FixVowelTable(VoiceBankLanguage lang)
        {
            if (lang == VoiceBankLanguage.Japanese)
            {
                string[] avaVowel = {"a","i","M","e","o" };
                int pi = 0;
                while (pi < VoiceSamples.Count)
                {
                    if (VoiceSamples[pi].SymbolType==PhonemeSymbolType.Vowel_Vowel)
                    {
                        //AppendConsonantTable
                        string p1 = VoiceSamples[pi].Phoneme[0].Symbol;
                        string p2 = VoiceSamples[pi].Phoneme[1].Symbol;
                        if (p1.IndexOf('#') > 0) p1 = p1.Substring(0,p1.IndexOf('#'));
                        if (p2.IndexOf('#') > 0) p2 = p2.Substring(0,p2.IndexOf('#'));
                        bool p1_c = !avaVowel.Contains(p1);
                        bool p2_c = !avaVowel.Contains(p2);
                        VoiceSample vst = VoiceSamples[pi].Clone();
                        if (p1_c && !p2_c)
                        {
                            vst.SymbolType = PhonemeSymbolType.Consonant_Vowel;
                            VoiceSamples.Add(vst);
                        }
                        else if (!p1_c && p2_c)
                        {
                            vst.SymbolType = PhonemeSymbolType.Vowel_Consonant;
                            VoiceSamples.Add(vst);
                        }
                    }
                    pi++;
                }
            }
        }

        public List<int> FillAvaliableGroup(SampleFilterType vstype = SampleFilterType.ALL,bool isGCM=false)
        {
            List<VoiceSample> vsL = GetSamples(vstype);
            Dictionary<string, List<VoiceSample>> Symbols = new Dictionary<string, List<VoiceSample>>();
            for(int i=0;i<vsL.Count;i++)
            {
                VoiceSample vs = vsL[i];
                string ssKey = vs.SymbolType.ToString() + "__" + vs.PhonemeName;
                if (!Symbols.ContainsKey(ssKey))
                {
                    Symbols.Add(ssKey, new List<VoiceSample>() { vs });
                }
                else
                {
                    Symbols[ssKey].Add(vs);
                }
            }


            List<int> PitchGroup = new List<int>();
            if (isGCM)
            {
                int GroupCount = 0;
                foreach (var kv in Symbols)
                {
                    if(GroupCount<kv.Value.Count)
                    {
                        GroupCount = kv.Value.Count;
                        PitchGroup.Clear();
                        foreach(var kb in kv.Value)
                        {
                            PitchGroup.Add((int)kb.PitchNumber);
                        }
                    }
                }
            }
            else
            {
                //获取最大公约数(取最全的音阶)
                int GroupCount = int.MaxValue;
                foreach (var kv in Symbols) if (GroupCount > kv.Value.Count) GroupCount = kv.Value.Count;

                //获取音阶组
                for (int g = 0; g < GroupCount; g++)
                {
                    List<double> Pitch = new List<double>();
                    foreach (var kv in Symbols) { Pitch.Add(kv.Value[g].PitchNumber); }
                    PitchGroup.Add((int)Pitch.ToList().Average());
                }
            }

            //分配音阶组
            foreach (var kv in Symbols)
            {
                for (int i = 0; i < PitchGroup.Count; i++)
                {
                    VoiceSample? rgVs = null;
                    foreach (var vs in kv.Value)
                    {
                        if (rgVs == null) { rgVs = vs; continue; }
                        if (Math.Abs(PitchGroup[i] - vs.PitchNumber) < Math.Abs(PitchGroup[i] - rgVs.PitchNumber))
                        {
                            rgVs = vs;
                        }
                    }
                    if (rgVs != null)
                    {
                        rgVs.PitchGroup.Add(GetPitchName(PitchGroup[i]));
                    }
                }
            }

            return PitchGroup;
        }

        public VoiceBankLanguage GuessLanguage()
        {
            int jpn_inc = 0;
            int eng_inc = 0;
            int chn_inc = 0;
            int kor_inc = 0;
            int esp_inc = 0;
            string[] jpn_ind = { "N'", "N\\", "m'", "p\\", "C", "4", "4'", "p'", "t'", "k'" };
            string[] eng_ind = { "{", "V", "i:", "u:", "O:", "eI", "OI", "aU", "I@", "e@", "U@", "O@", "Q@", "@l", "e@0", "R" };
            string[] chn_ind = { "ei", "@`", "AU", "a_n", "@_n", "AN", "@N", "i\\", "i`", "ia", "iE_r", "iAU", "i@U", "iE_n", "i_n", "iAN", "iN", "iUN", "ua", "uo", "uaI", "uei", "ua_n", "u@_n", "uAN", "UN", "u@N", "y", "yE_r", "y{_n", "y_n", "z`", "k_h", "p_h", "t_h", "s\\", "s`", "ts\\_h", "ts\\", "ts`_h", "ts_h", "ts`" };
            string[] kor_ind = { "ja", "jo", "j7", "ju", "oa", "je", "u7", "ue", "ui", "Mi", "QNa", "QN7", "QNo", "QNu", "QNM", "Qra", "Np", "mp", "np", "Mp", "gp", "bp", "dp", "rp" };
            string[] esp_ind = { "B", "G", "rr", "j\\" };

            for (int i = 0; i < VoiceSamples.Count; i++)
            {
                foreach(var p in VoiceSamples[i].Phoneme)
                {
                    if (jpn_ind.Contains(p.Symbol)) 
                        jpn_inc++;
                    if (eng_ind.Contains(p.Symbol)) 
                        eng_inc++;
                    if (chn_ind.Contains(p.Symbol)) 
                        chn_inc++;
                    if (kor_ind.Contains(p.Symbol)) 
                        kor_inc++;
                    if (esp_ind.Contains(p.Symbol)) 
                        esp_inc++;
                }
            }
            if (chn_inc > 0) return VoiceBankLanguage.Chinese;//CHINESE
            if (eng_inc > 0) return VoiceBankLanguage.English;
            if (jpn_inc > 0) return VoiceBankLanguage.Japanese;
            if (esp_inc > 0) return VoiceBankLanguage.Espanhol;
            if (kor_inc > 0) return VoiceBankLanguage.Korean;

            return VoiceBankLanguage.English;
        }

        private string GetPitchName(double pitchnumber)
        {
            bool isVoice = false;//IS UTAU
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
        private VoiceSample CreateVS(VoiceSampleStruct vss)
        {
            VoiceSample vs = new VoiceSample(this.DDBPath);
            vs.PhoneType = (PhoneType)vss.phoneType;
            vs.PitchNumber = vss.pitch;
            vs.PitchName = GetPitchName((int)vss.pitch);
            vs.Tempo= vss.tempo;
            vs.Format.SampleRate = vss.sndSampleRate;
            vs.Format.Channel = vss.sndChannel;
            vs.RiffBody.Position = vss.sndOffset;
            vs.RiffBody.Length = vss.sndLength;
            vs.RiffBody.VoiceStartTime = vss.sndLBound;
            vs.SymbolType = vss.phonemeType;

            int partCount = vss.phonemeCount;
            for(int i=0; i < partCount;i++)
            {
                string phoneSymbol=new string(vss.phonemeBlocks.Skip(i * 256).Take(256).ToArray()).Split("\0")[0];
                string phoneEncodedSymbol = new string(vss.phonemeEncodedBlocks.Skip(i * 256).Take(256).ToArray()).Split("\0")[0];
                int SymbolIndex = vs.Phoneme.Count;
                vs.Phoneme.Add(new PhonemeSymbol() { Symbol = phoneSymbol,EncodedSymbol=phoneEncodedSymbol });

                float[] sectionArray = vss.sndSectionData.Skip(i * 4).Take(4).ToArray();
                LabelBlock labelBlock = new LabelBlock();
                labelBlock.phonemeSymbol = phoneSymbol;
                labelBlock.phonemeIndex = SymbolIndex;
                labelBlock.entireRange.startTime = sectionArray[0];
                labelBlock.entireRange.endTime= sectionArray[1];
                labelBlock.voicedRange.startTime = sectionArray[2];
                labelBlock.voicedRange.endTime= sectionArray[3];
                vs.LabelGrid.Add(labelBlock);
            }
            return vs;
        }

        public List<VoiceSample> VoiceSamples { get; private set; }= new List<VoiceSample>();

        public List<VoiceSample> GetSamples(SampleFilterType type=SampleFilterType.ALL)
        {
            if(type==SampleFilterType.ALL) return VoiceSamples;
            List<VoiceSample> ret = new List<VoiceSample>();
            foreach(var vs in VoiceSamples)
            {
                switch(type)
                {
                    case SampleFilterType.DEFAULT:
                        if (vs.SymbolType != PhonemeSymbolType.Vowel__Consonant_Vowel && vs.SymbolType != PhonemeSymbolType.Vowel__Vowel_Consonant) ret.Add(vs); 
                        break; 
                    case SampleFilterType.CVVC:
                        switch (vs.SymbolType)
                        {
                            case PhonemeSymbolType.Consonant_Vowel:
                            case PhonemeSymbolType.Vowel_Consonant:
                            case PhonemeSymbolType.Vowel_Only:
                            case PhonemeSymbolType.Vowel__Consonant_Vowel:
                            case PhonemeSymbolType.Vowel__Vowel_Consonant:
                            case PhonemeSymbolType.Vowel_Rest:
                                ret.Add(vs);
                                break;
                        }
                        break;
                    case SampleFilterType.VCV:
                        break;
                    case SampleFilterType.VCCV:
                        return VoiceSamples;//return all
                }
            }
            return ret;
        }
    }
    public enum SampleFilterType
    {
        CVVC,
        VCCV,
        VCV,
        DEFAULT,
        ALL
    }
    public enum VoiceBankLanguage
    {
        Japanese=0,
        English,
        Korean,
        Espanhol,
        Chinese
    }
    public enum PhonemeSymbolType
    {
        Vowel_Only = 0,
        Consonant_Only,
        Consonant_Vowel,
        Vowel_Consonant,
        Consonant_Consonant,
        Vowel_Vowel,
        Rest_Consonant,
        Rest_Vowel,
        Vowel_Rest,
        Consonant_Rest,
        Vowel__Consonant_Vowel=98,
        Vowel__Vowel_Consonant=99,
    }
    public class SndFormat
    {
        public int SampleRate { get; set; } = 44100;
        public int Channel { get; set; } = 1;
        public int Bit { get; set; } = 16;
    }
    public class SndBody
    {
        public long Position { get; set; } = 0;
        public long Length { get; set; } = 0;
        public double VoiceStartTime { get; set; } = 0;
    }
    public class LabelArea
    {
        public double startTime { get; set; } = 0;
        public double endTime { get; set; }=0;
    }
    public class LabelBlock
    {
        public string phonemeSymbol { get; set; }="";
        public int phonemeIndex { get; set; }=0;
        public LabelArea entireRange { get; set; }=new LabelArea();
        public LabelArea voicedRange { get; set; }=new LabelArea();
    }
    public class PhonemeSymbol
    {
        public string Symbol { get; set; } = "";
        public string EncodedSymbol { get; set; } = "";
    }
    public class VoiceSample
    {
        private string DDBPath = "";
        public VoiceSample(string DDBFile) { this.DDBPath = DDBFile; }
        public PhoneType PhoneType { get; set; } = PhoneType.Stationary;
        public List<PhonemeSymbol> Phoneme { get; set;} = new List<PhonemeSymbol>();
        public List<LabelBlock> LabelGrid { get; set; } = new List<LabelBlock>();
        public double PitchNumber { get; set; } = 0;
        public string PitchName { get; set; } = "";
        public List<string> PitchGroup { get; set; } = new List<string>();
        public double Tempo { get; set; } = 0;
        public SndFormat Format { get; set; } = new SndFormat();
        public SndBody RiffBody { get; set; }=new SndBody();
        public PhonemeSymbolType SymbolType { get; set; } = 0;

        public VoiceSample Clone()
        {
            VoiceSample ret = new VoiceSample(DDBPath);
            ret.PhoneType = PhoneType;
            ret.Phoneme.AddRange(Phoneme);
            ret.LabelGrid.AddRange(LabelGrid);
            ret.PitchNumber = PitchNumber;
            ret.PitchName = PitchName;
            ret.Tempo = Tempo;
            ret.Format = Format;
            ret.RiffBody= RiffBody;
            ret.SymbolType = SymbolType;
            return ret;
        }

        public string PhonemeName
        {
            get
            {
                List<string> phl = new List<string>();
                foreach (var p in Phoneme) phl.Add(p.Symbol);
                return String.Join(" ", phl);
            }
        }
        public string PhonemeFileName
        {
            get
            {
                List<string> phl = new List<string>();
                foreach (var p in Phoneme) phl.Add(p.EncodedSymbol);
                return String.Join("__", phl);
            }
        }

        public bool ExportSnd(Stream targetWavStream)
        {
            bool ret = true;
            using (FileStream fs = new FileStream(this.DDBPath, FileMode.Open, FileAccess.Read))
            {
                fs.Position = this.RiffBody.Position + 0x12;//去SND头
                using (BinaryReader br = new BinaryReader(fs))
                {
                    try
                    {
                        byte[] bs = br.ReadBytes((int)this.RiffBody.Length);
                        using (BinaryWriter bw2 = new BinaryWriter(targetWavStream))
                        {
                            try
                            {
                                //WriteHead
                                bw2.Write(new char[4] { 'R', 'I', 'F', 'F' });
                                bw2.Write((int)(this.RiffBody.Length + 0x44 - 8));
                                bw2.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                                bw2.Write((int)16);//PCM
                                bw2.Write((ushort)1);
                                bw2.Write((ushort)1);
                                bw2.Write((int)this.Format.SampleRate);
                                bw2.Write((int)this.Format.SampleRate * 2);//BitRate
                                bw2.Write((ushort)2);//BlockAlign
                                bw2.Write((ushort)this.Format.Bit);//Bits
                                bw2.Write(new char[4] { 'd', 'a', 't', 'a' });
                                bw2.Write((int)this.RiffBody.Length);
                                //WriteBody
                                bw2.Write(bs);
                            }
                            catch { ret = false; }
                        }
                    }
                    catch { ret = false; }
                }
            }
            return ret;
        }
        public bool ExportSnd(string targetFilePath)
        {
            bool ret = true;
            using (FileStream f2 = new FileStream(targetFilePath, FileMode.Create))
            {
                ret=ExportSnd(f2);
            }
            return ret;
        }

        public OtoLabel ExportLabel()
        {   
            OtoLabel ret = new OtoLabel();
            double lb = RiffBody.VoiceStartTime;
            
            ret.LeftPadding = lb*1000.0;
            if (this.PhoneType==PhoneType.Stationary)
            {
                ret.RightPadding = -(this.LabelGrid[0].voicedRange.endTime - lb) * 1000.0;
                ret.Preutter = Math.Min((this.LabelGrid[0].entireRange.endTime - lb) * 1000.0 / 2,20.0);
                ret.Overlap =  ret.Preutter;
                ret.FixedLength = 0;// ret.Overlap;
            }
            else if(this.PhoneType==PhoneType.DiPhoneArticulation)
            {
                ret.RightPadding = -(this.LabelGrid[1].entireRange.endTime-lb) * 1000.0;
                ret.FixedLength = (this.LabelGrid[1].voicedRange.startTime - this.LabelGrid[1].entireRange.startTime) * 1000.0;
                switch (this.SymbolType)
                {
                    case PhonemeSymbolType.Consonant_Vowel:
                        ret.Overlap = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = (this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0;
                        ret.FixedLength = ((this.LabelGrid[1].voicedRange.startTime + this.LabelGrid[1].voicedRange.endTime) / 2 - lb) * 1000.0;
                        break;
                    case PhonemeSymbolType.Vowel_Consonant:
                        ret.Overlap = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0);
                        ret.FixedLength = ((this.LabelGrid[1].voicedRange.startTime + this.LabelGrid[1].voicedRange.endTime) / 2 - lb) * 1000.0;
                        break;
                    case PhonemeSymbolType.Vowel_Vowel:
                        ret.Overlap = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = (this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0;
                        ret.FixedLength = ((this.LabelGrid[1].voicedRange.startTime + this.LabelGrid[1].voicedRange.endTime) / 2 - lb) * 1000.0;
                        break;
                    case PhonemeSymbolType.Vowel__Consonant_Vowel:
                        double tmpOvl = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = (this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0 - tmpOvl;
                        ret.FixedLength = ((this.LabelGrid[1].voicedRange.startTime + this.LabelGrid[1].voicedRange.endTime) / 2 - lb) * 1000.0 - tmpOvl;
                        ret.LeftPadding += tmpOvl;
                        ret.RightPadding += tmpOvl;
                        ret.Overlap = ret.Preutter;
                        break;
                    case PhonemeSymbolType.Vowel__Vowel_Consonant:
                        double pN10 = Math.Min(10.0, (this.LabelGrid[1].voicedRange.endTime - this.LabelGrid[1].voicedRange.startTime)*1000.0 / 5.0);
                        ret.Overlap = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = (this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0;
                        //-400 > -500
                        ret.RightPadding = ret.RightPadding < (- ret.Preutter - pN10) ? (- ret.Preutter - pN10):-ret.Preutter;
                        ret.FixedLength =  ret.Preutter;
                        break;
                    case PhonemeSymbolType.Vowel_Rest:
                    case PhonemeSymbolType.Consonant_Rest:
                    case PhonemeSymbolType.Consonant_Consonant:
                    default:
                        ret.Overlap = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0) / 2;
                        ret.Preutter = (this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0;
                        ret.FixedLength = ((this.LabelGrid[1].voicedRange.startTime - lb) * 1000.0);
                        break;
                }
            }
            return ret;
        }
    }

    public class OtoLabel {
        //ba_ba_ba.wav=- ba_B4,892.064,147.771,-401.198,14.637,4.747
        public double LeftPadding { get; set; } = 0;
        public double FixedLength { get; set; } = 0;
        public double RightPadding { get; set; } = 0;
        public double Preutter { get; set; } = 0;
        public double Overlap { get; set; } = 0;

        public override string ToString()
        {
            List<string> v=new List<string>();
            v.Add(LeftPadding.ToString("F3", CultureInfo.InvariantCulture));
            v.Add(FixedLength.ToString("F3", CultureInfo.InvariantCulture));
            v.Add(RightPadding.ToString("F3", CultureInfo.InvariantCulture));
            v.Add(Preutter.ToString("F3", CultureInfo.InvariantCulture));
            v.Add(Overlap.ToString("F3", CultureInfo.InvariantCulture));
            return String.Join(",", v);
        }
    }


    public enum PhoneType
    {
        Stationary=0,
        DiPhoneArticulation=1,
        TriPhoneArticulation= 2
    }
}
