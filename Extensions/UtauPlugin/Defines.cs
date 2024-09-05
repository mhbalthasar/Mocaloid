using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocaloid.UtauPlugin
{
    public class UnknownParts
    {
        public List<string> RawData { get; set; } = new List<string>();
        public string RawSection { get; set; } = "";
    }
    public class UstSettings : UnknownParts
    {
        public string VoiceDir { get; set; } = "";
        public double Tempo { get; set; } = 0;
    }

    public class UstNotes : UnknownParts
    {
        int mLength = 0;
        string mLyric = "";
        int mNoteNum = 0;
        public int Length { get { return mLength; } set { mLength = value; 
                for(int i=0;i<RawData.Count;i++)
                {
                    if (RawData[i].StartsWith("Length"))
                    {
                        RawData[i] = String.Format("Length={0}",value);
                    }
                }
            } }
        public string Lyric { get { return mLyric; } set { mLyric = value;
                for (int i = 0; i < RawData.Count; i++)
                {
                    if (RawData[i].StartsWith("Lyric"))
                    {
                        RawData[i] = String.Format("Lyric={0}", value);
                    }
                }
            } }
        public int NoteNum { get { return mNoteNum; } set { mNoteNum = value;
                for (int i = 0; i < RawData.Count; i++)
                {
                    if (RawData[i].StartsWith("NoteNum"))
                    {
                        RawData[i] = String.Format("NoteNum={0}", value);
                    }
                }
            } }

        public UstNotes Clone()
        {
            UstNotes ret = new UstNotes();
            ret.Length = Length;
            ret.Lyric= Lyric;
            ret.NoteNum= NoteNum;
            ret.RawSection = RawSection;
            ret.RawData.AddRange(RawData);
            return ret;
        }
    }

    public class UstInfo
    {
        public UstSettings Setting { get; set; } = new UstSettings();
        public UnknownParts Version { get; set; } = new UnknownParts();
        public List<List<UstNotes>> Notes { get; set; } = new List<List<UstNotes>>();
    }
}
