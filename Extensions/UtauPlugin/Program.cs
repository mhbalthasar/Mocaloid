using Mocaloid.UtauPlugin;

internal class Program
{
    private static UstInfo AnalysisUST(List<string> ustfile)
    {
        int curLine = 0;

        UstSettings? setting = null;
        UstNotes? note = null;
        UnknownParts? version = null;
        List<List<UstNotes>> notes = new List<List<UstNotes>>();

        bool isSetting = false;
        bool isNote = false;
        bool isVersion = false;
        //FindSetting
        while (curLine < ustfile.Count)
        {
            if((isSetting || isNote || isVersion) && ustfile[curLine].StartsWith("[#") && ustfile[curLine].EndsWith("]")) {
                //CURPART IS END
                if (isSetting) isSetting = false;
                if (isVersion) isVersion = false;
                if (isNote) { isNote = false; if (note != null) notes.Add(new List<UstNotes>() { note }); }
                continue;
            }
            if (ustfile[curLine].StartsWith("[#") && ustfile[curLine].EndsWith("]"))
            {
                string titleSec = ustfile[curLine].Substring(2, ustfile[curLine].Length-3);
                if (titleSec == "VERSION")
                {
                    version = new UnknownParts();
                    version.RawSection = titleSec;
                    isVersion = true;
                }
                else if (titleSec=="SETTING")
                {
                    setting = new UstSettings();
                    setting.RawSection = titleSec;
                    isSetting = true;
                }else if(titleSec == "PREV" || titleSec=="NEXT" || int.TryParse(titleSec,out int noteIdx))
                {
                    note = new UstNotes();
                    note.RawSection = titleSec;
                    isNote = true;
                }
                curLine++;
                continue;
            }
            if (isVersion)
                version.RawData.Add(ustfile[curLine]);
            else if (isSetting)
            {
                setting.RawData.Add(ustfile[curLine]);
                if (ustfile[curLine].StartsWith("VoiceDir"))
                {
                    setting.VoiceDir = ustfile[curLine].Substring("VoiceDir".Length + 1);
                }
                else if (ustfile[curLine].StartsWith("Tempo"))
                {
                    if (double.TryParse(ustfile[curLine].Substring("Tempo".Length + 1), out double tempo)) setting.Tempo = tempo;
                }
            }
            else if (isNote)
            {
                note.RawData.Add(ustfile[curLine]);
                if (ustfile[curLine].StartsWith("Lyric"))
                {
                    note.Lyric = ustfile[curLine].Substring("Lyric".Length + 1);
                }
                else if (ustfile[curLine].StartsWith("NoteNum"))
                {
                    if (int.TryParse(ustfile[curLine].Substring("NoteNum".Length + 1), out int notenum)) note.NoteNum=notenum;
                }
                else if (ustfile[curLine].StartsWith("Length"))
                {
                    if (int.TryParse(ustfile[curLine].Substring("Length".Length + 1), out int length)) note.Length = length;
                }
            }
            curLine++;
        }
        if (isSetting) isSetting = false;
        if (isVersion) isVersion = false;
        if (isNote) { isNote = false; if (note != null) notes.Add(new List<UstNotes>() { note }); }

        UstInfo ret = new UstInfo();
        if(setting!=null)ret.Setting = setting;
        if(version!=null)ret.Version = version;
        ret.Notes = notes;
        return ret;
    }

    private static List<string> BuildUstInfo(UstInfo info)
    {
        List<string> ret = new List<string>();
        if(info.Version.RawData.Count>0)
        {
            ret.Add(String.Format("[#{0}]", info.Version.RawSection));
            ret.AddRange(info.Version.RawData);
        }
        if (info.Setting.RawData.Count > 0)
        {
            ret.Add(String.Format("[#{0}]", info.Setting.RawSection));
            ret.AddRange(info.Setting.RawData);
        }
        foreach (var nl in info.Notes)
        {
            foreach (var n in nl)
            {
                ret.Add(String.Format("[#{0}]", n.RawSection));
                ret.AddRange(n.RawData);
            }
        }
        return ret;
    }

    private static void Main(string[] args)
    {
        if (args.Length < 1) return;
        if (!File.Exists(args[0])) return;
        string filename = args[0];
        List<string> ustfile = new List<string>();
        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (TextReader sr = new StreamReader(fs,System.Text.CodePagesEncodingProvider.Instance.GetEncoding("Shift-JIS")))
            {
                string? ln;
                while ((ln = sr.ReadLine()) != null)
                {
                    ustfile.Add(ln);
                }
            }
        }
        var ustInput = AnalysisUST(ustfile);
        var splitust=new SplitUST();
        var ustOutput=splitust.DoAction(ustInput);

        using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (TextWriter sr = new StreamWriter(fs))
            {
                var tl = BuildUstInfo(ustOutput);
                foreach(var vl in tl)
                {
                    sr.WriteLine(vl);
                }
            }
        }
    }
}