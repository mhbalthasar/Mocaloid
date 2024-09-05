#include "stdafx.h"
#include "ddi.h"
#include <QtCore\qstringlist.h>
#include <QtCore\qfile.h>
#include <QtCore\qdir.h>
#include "chunk/basechunk.h"
#include "chunk/chunkcreator.h"
#include <QtCore/qcryptographichash.h>

#define SETERROR(x) std::string err_str(x.toLatin1());*error=new char[err_str.size()+1];std::copy(err_str.begin(),err_str.end(),*error);(*error)[err_str.size()]='\0';
#define FRET(x) ddi.close();ddb.close();mTreeRoot->~BaseChunk();return x;

static BaseChunk* SearchForChunkByPath(QString path,
    BaseChunk* mTreeRoot = nullptr,BOOL bSlience=TRUE)
{
	QStringList paths=QString(path).split(',');
    BaseChunk* baseSearchChunk = mTreeRoot;
    if(baseSearchChunk == nullptr) {
		if(!bSlience)
			QMsgBox(nullptr, ("No root"), ("Tree root chunk is non existent."));
        return nullptr;
    }
    foreach(auto i, paths) {
        if(i.isEmpty()) {
			if(!bSlience)
				QMsgBox(nullptr, ("Invalid pattern"), ("Empty name in path chain."));
            return nullptr;
        }
        baseSearchChunk = baseSearchChunk->GetChildByName(i);
        if(baseSearchChunk == nullptr) {
			if(!bSlience)
				QMsgBox(nullptr, ("Specified chunk not found"), QString("Search for \"%1\" returned nothing.").arg(i).toLatin1());
            return nullptr;
        }
    }

    return baseSearchChunk;
}
static BaseChunk* ParseDdi(QString path)
{
    FILE *file;
	fopen_s(&file,path.toLocal8Bit(), "rb");
    if(!file) {
        return nullptr;
    }

    // Read signature first
    fpos_t pos;
    char signatureBuf[4];
    fgetpos(file, &pos);
    // Skip Leading QWORD if needed
    if(BaseChunk::HasLeadingQword) fseek(file, 8, SEEK_CUR);
    fread(signatureBuf, 1, 4, file);
    fsetpos(file, &pos);

    auto sig = QByteArray(signatureBuf, 4);
    auto chunk = ChunkCreator::Get()->ReadFor("DBSe", file);

    fclose(file);

    return chunk;
}

#define VSS_STA 0
#define VSS_DIPHONE 1
#define VSS_TRIPHONE 2
#define FILL256STR(x,y,z) strncpy(&x[256*z], y.toLatin1(),min(256,y.length()));


void FreeDDI(VoiceSampleStruct * array) {
    delete[] array;
}

#define BYTE2SEC(x) (float)(x * 8.0) / (vss.sndSampleRate * 16.0 * vss.sndChannel)
#define FRAME2BYTE(x) (256*x*2)


BOOL HashDDI(char* ddiFile, char** ddiHash) {
    QString ddiPath = QString(ddiFile);
    QString ddbPath = ddiPath.section('.', 0, -2) + ".ddb";
    {
        QFile ddi(ddiPath);
        if (!ddi.open(QFile::ReadOnly)) {
            return FALSE;
        }
        {
            QCryptographicHash hash(QCryptographicHash::Md5);
            if (hash.addData(&ddi)) {
                QByteArray result = hash.result();
                QString x = QString(result.toHex());
                std::string err_str(x.toLatin1());//, x.size());// = x.toLatin1();
                *ddiHash = new char[err_str.size() + 1];
                std::copy(err_str.begin(), err_str.end(), *ddiHash);
                (*ddiHash)[err_str.size()] = '\0';
            }
        }
        ddi.close();
    }
    return TRUE;
}

static QString PEncode(QString input)
{
    QString ret;
    for (QChar c : input) {
        if (c >= 'a' && c <= 'z')
        {
            ret += c;
        }
        else if (c >= 'A' && c <= 'Z')
        {
            ret += '[' + c + ']';
        }
        else if (c >= '0' && c <= '9')
        {
            ret += c;
        }
        else if (c == ' ')
        {
            ret += "[%20]";
        }
        else if (c < ' ' || c>'}' || 
            c == '\\' ||
            c == '/'  ||
            c == ':' ||
            c == '*' ||
            c == '?' ||
            c == '<' ||
            c == '>' ||
            c == '|' ||
            c == '[' ||
            c == ']' ||
            c == '-' ||
            c == '.' ||
            c == '"'
            )
        {
            ret += '[' + QString::number(c.toLatin1()) + ']';
        }
        else
            ret += c;
    }
    return ret;
}

static bool IsEvecSymbol(QString Symbol)
{
    if(Symbol == "*") return TRUE;
    if(Symbol == "?") return TRUE;
    if (Symbol.length() >= 3)
    {
        QChar Si = Symbol.at(Symbol.length() - 1);
        QChar SS = Symbol.at(Symbol.length() - 2);
        if (SS != '#')return FALSE;
        if (Si == '1' ||
            Si == '2' ||
            Si == '3' ||
            Si == '4' ||
            Si == '5' ||
            Si == '6' ||
            Si == '+' ||
            Si == '-' ||
            Si == 'F'
            )return TRUE;
        return FALSE;;
    }
    return FALSE;
}

#define PST_IS_REST(x) x.toLower()=="sil" || x.toLower()=="asp"
VoiceSampleStruct * AnalysisDDI(char* ddiFile, char** error, int* vss_size) {
    QString ddiPath = QString(ddiFile);
    QString ddbPath = ddiPath.section('.', 0, -2) + ".ddb";

    //Check If File Readable
    {
        QFile ddi(ddiPath);
        if (!ddi.open(QFile::ReadOnly)) {
            SETERROR("Cannot open target DDI " + ddiPath);
            return NULL;
        }
        ddi.close();
        QFile ddb(ddbPath);
        if (!ddb.open(QFile::ReadOnly)) {
            SETERROR("Cannot open target DDB " + ddbPath);
            return NULL;
        }
        ddb.close();
    }

    auto mTreeRoot = ParseDdi(ddiPath);
    if (mTreeRoot == nullptr) {
            SETERROR("Parse DDI Failure" + ddiPath);
            return NULL;
    }

    std::vector<VoiceSampleStruct> totalMap;
    
    QFile ddi(ddiPath);
    ddi.open(QFile::ReadOnly);
    QFile ddb(ddbPath);
    ddb.open(QFile::ReadOnly);
    
    auto readPropertyInt16 = [&](ChunkProperty p) -> short {
			qsize_t curPos=ddi.pos();
			short ret=0;
			ddi.seek(p.offset);
			ddi.read((char*)&ret,sizeof(ret));
            ddi.seek(curPos);
            return ret;
        };
    auto readPropertyFloat= [&](ChunkProperty p) -> float {
			qsize_t curPos=ddi.pos();
			float ret=0;
			ddi.seek(p.offset);
			ddi.read((char*)&ret,sizeof(float));
            ddi.seek(curPos);
            return ret;
        };
    auto readPropertyInt32 = [&](ChunkProperty p) -> int {
			qsize_t curPos=ddi.pos();
			int ret=0;
			ddi.seek(p.offset);
			ddi.read((char*)&ret,sizeof(ret));
            ddi.seek(curPos);
            return ret;
        };
    auto readPropertyInt64 = [&](ChunkProperty p) -> long {
			qsize_t curPos=ddi.pos();
			long ret=0;
			ddi.seek(p.offset);
			ddi.read((char*)&ret,sizeof(ret));
            ddi.seek(curPos);
            return ret;
        };
    auto rel2pitch = [&](float relPitch)->float {
        double pnA4 = 69.0;
        double pnR = relPitch / 100.0;
        return pnA4+pnR;
        };

    bool JumpEVEC = TRUE;

    auto stationaryRoot = SearchForChunkByPath("voice,stationary",mTreeRoot);
    
    QStringList VowelList;
    // Iterate voice colors
    for (auto voiceColor : stationaryRoot->Children) {
        // Iterate stationary segments
        for (auto staSeg : voiceColor->Children) {
            //CHECK IF EVEC
            if (JumpEVEC && IsEvecSymbol(staSeg->GetName()))
            {
                continue;
            }
            // Iterate each pitch of the segment
            for (auto pitchSeg : staSeg->Children) {
                VoiceSampleStruct vss;
                vss.type = VSS_STA;//sta;
                memset(&vss.phonemeBlocks, 0, 1024);
                memset(&vss.phonemeEncodedBlocks, 0, 1024);
                memset(&vss.sndSectionData, 0, sizeof(float) * 16);
                VowelList.append(staSeg->GetName());
                FILL256STR(vss.phonemeBlocks, staSeg->GetName(), 0);
                FILL256STR(vss.phonemeEncodedBlocks, PEncode(staSeg->GetName()), 0);
                vss.phonemeCount = 1;
                vss.phonemeType = PST_VOWEL_ONLY;
                vss.relPitch = readPropertyFloat(pitchSeg->GetProperty("mPitch"));
                vss.pitch = rel2pitch(vss.relPitch);
                if (vss.pitch < 12 || vss.pitch>120)continue;//非法音高
                vss.tempo = readPropertyFloat(pitchSeg->GetProperty("Tempo"));
                vss.sndSampleRate = readPropertyInt32(pitchSeg->GetProperty("SND Sample rate"));
                vss.sndChannel = readPropertyInt16(pitchSeg->GetProperty("SND Channel count"));
                {
                    qsize_t sndOft = readPropertyInt64(pitchSeg->GetProperty("SND Sample offset")) - 0x12;
                    char TMP[5] = { 0 };
                    qsize_t seekOff = -1;
                    while (QString(TMP) != "SND ")
                    {
                        if (TMP[1] == 'S' && TMP[2] == 'N' && TMP[3] == 'D')
                            seekOff -= 1;
                        else if (TMP[2] == 'S' && TMP[3] == 'N')
                            seekOff -= 2;
                        else if (TMP[3] == 'S')
                            seekOff -= 3;
                        else if (TMP[0] == 'N' && TMP[1] == 'D' && TMP[2] == ' ')
                            seekOff += 1;
                        else if (TMP[0] == 'D' && TMP[1] == ' ')
                            seekOff += 2;
                        else if (TMP[0] == ' ')
                            seekOff += 3;
                        else
                            seekOff += 4;
                        ddb.seek(sndOft - seekOff);
                        ddb.read(TMP, 4);
                        TMP[4] = 0;
                    }
                    qsize_t SndHead = sndOft - seekOff;
                    ddb.seek(SndHead + 4);
                    int sndLen = 0;
                    ddb.read((char*)&sndLen, 4);
                    vss.sndOffset = SndHead + 0x12;//ȥͷ
                    vss.sndLength = sndLen - 0x12;//ȥͷ

                    int skipLength = seekOff - 0x12;

                    float secOft = BYTE2SEC(skipLength);//(float)(skipLength * 8.0) / (vss.sndSampleRate * 16.0 * vss.sndChannel);

                    vss.sndLBound = secOft;

                    vss.sndSectionData[0] = secOft;
                    vss.sndSectionData[1] = BYTE2SEC(sndLen - 0x12);

                    int frameCount = 0;
                    auto framesDir = pitchSeg->GetChildByName("<Frames>");
                    auto framesProps = framesDir->GetPropertiesMap();
                    framesProps.remove("Count");
                    for (auto i : framesProps) {
                        frameCount++;
                    }

                    int secBytes = min(skipLength + (frameCount * 256 * 2), sndLen);
                    vss.sndSectionData[2] = secOft;//(float)(secBytes * 8.0) / (vss.sndSampleRate * 16.0 * vss.sndChannel);
                    vss.sndSectionData[3] = BYTE2SEC(secBytes);
                }
                totalMap.push_back(vss);
            }
        }
    }

    auto articulationRoot = SearchForChunkByPath("voice,articulation",mTreeRoot);
    for (auto beginPhoneme : articulationRoot->Children) {
        if (JumpEVEC && IsEvecSymbol(beginPhoneme->GetName()))
        {
            continue;
        }
        // Iterate end phonemes
        for (auto endPhoneme : beginPhoneme->Children) {
            // If Triphonemes
            if (endPhoneme->ObjectSignature() == "ART ") {
                /*
                for (auto thirdPhoneme : endPhoneme->Children) {
                    for (auto pitch = 0; pitch < thirdPhoneme->Children.size(); pitch++) {
                        auto pitchSeg = thirdPhoneme->Children[pitch];
                        //三音阶无法被UTAU利用，不解压
                    }
                }
                */
                continue;
            }
            else
            {
                if (JumpEVEC && IsEvecSymbol(endPhoneme->GetName()))
                {
                    continue;
                }

                for (auto pitch = 0; pitch < endPhoneme->Children.size(); pitch++) {
                    auto pitchSeg = endPhoneme->Children[pitch];

                    VoiceSampleStruct vss;
                    vss.type = VSS_DIPHONE;//sta;
                    memset(&vss.phonemeBlocks, 0, 1024);
                    memset(&vss.phonemeEncodedBlocks, 0, 1024);
                    memset(&vss.sndSectionData, 0, sizeof(float) * 16);
                    FILL256STR(vss.phonemeBlocks, beginPhoneme->GetName(), 0);
                    FILL256STR(vss.phonemeBlocks, endPhoneme->GetName(), 1);
                    FILL256STR(vss.phonemeEncodedBlocks, PEncode(beginPhoneme->GetName()), 0);
                    FILL256STR(vss.phonemeEncodedBlocks, PEncode(endPhoneme->GetName()), 1);
                    vss.phonemeCount = 2;
                    bool p1_v = VowelList.contains(beginPhoneme->GetName());
                    bool p1_r = PST_IS_REST(beginPhoneme->GetName());
                    bool p2_v = VowelList.contains(endPhoneme->GetName());
                    bool p2_r = PST_IS_REST(endPhoneme->GetName());
                    vss.phonemeType = (p1_v && p2_r) ? PST_VOWEL_REST :
                        (p1_r && p2_v) ? PST_REST_VOWEL :
                        (!p1_v && p2_r) ? PST_CONSONANT_REST :
                        (p1_r && !p2_v) ? PST_REST_CONSONANT :
                        (p1_v && !p2_v) ? PST_VOWEL_CONSONANT :
                        (p2_v && !p1_v) ? PST_CONSONANT_VOWEL :
                        (p1_v && p2_v) ? PST_VOWEL_VOWEL :
                        PST_CONSONANT_CONSONANT;
                    vss.relPitch = readPropertyFloat(pitchSeg->GetProperty("mPitch"));
                    vss.pitch = rel2pitch(vss.relPitch);
                    if (vss.pitch < 12 || vss.pitch>120)
                        continue;//非法音高
                    vss.tempo = readPropertyFloat(pitchSeg->GetProperty("Tempo"));
                    vss.sndSampleRate = readPropertyInt32(pitchSeg->GetProperty("SND Sample rate"));
                    vss.sndChannel = readPropertyInt16(pitchSeg->GetProperty("SND Channel count"));
                    {
                        qsize_t sndOft = readPropertyInt64(pitchSeg->GetProperty("SND Sample offset")) - 0x12;
                        char TMP[5] = { 0 };
                        qsize_t seekOff = -1;
                        while (QString(TMP) != "SND ")
                        {
                            if (TMP[1] == 'S' && TMP[2] == 'N' && TMP[3] == 'D')
                                seekOff -= 1;
                            else if (TMP[2] == 'S' && TMP[3] == 'N')
                                seekOff -= 2;
                            else if (TMP[3] == 'S')
                                seekOff -= 3;
                            else if (TMP[0] == 'N' && TMP[1] == 'D' && TMP[2] == ' ')
                                seekOff += 1;
                            else if (TMP[0] == 'D' && TMP[1] == ' ')
                                seekOff += 2;
                            else if (TMP[0] == ' ')
                                seekOff += 3;
                            else
                                seekOff += 4;
                            ddb.seek(sndOft - seekOff);
                            ddb.read(TMP, 4);
                            TMP[4] = 0;
                        }
                        qsize_t SndHead = sndOft - seekOff;
                        ddb.seek(SndHead + 4);
                        int sndLen = 0;
                        ddb.read((char*)&sndLen, 4);
                        vss.sndOffset = SndHead + 0x12;
                        vss.sndLength = sndLen - 0x12;

                        sndOft = readPropertyInt64(pitchSeg->GetProperty("SND Sample offset"));
                        qsize_t sndOft2 = readPropertyInt64(pitchSeg->GetProperty("SND Sample offset+800"));
                        qsize_t sndOffset = sndOft2 - sndOft;
                        float secOffset = BYTE2SEC(sndOffset);//(float)(sndOffset * 8.0) / (vss.sndSampleRate * 16.0 * vss.sndChannel);

                        auto sectionDir = pitchSeg->GetChildByName("<sections>");
                        auto sec0 = sectionDir->GetChildByName("<section 0>");
                        auto sec1 = sectionDir->GetChildByName("<section 1>");

                        vss.sndLBound = secOffset;

                        int id = 0;
                        vss.sndSectionData[4 * id + 0] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec0->GetProperty("Entire section Begin")))));
                        vss.sndSectionData[4 * id + 1] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec0->GetProperty("Entire section End")))));
                        vss.sndSectionData[4 * id + 2] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec0->GetProperty("Stationary section Begin")))));
                        vss.sndSectionData[4 * id + 3] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec0->GetProperty("Stationary section End")))));
                        id = 1;
                        vss.sndSectionData[4 * id + 0] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec1->GetProperty("Entire section Begin")))));
                        vss.sndSectionData[4 * id + 1] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec1->GetProperty("Entire section End")))));
                        vss.sndSectionData[4 * id + 2] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec1->GetProperty("Stationary section Begin")))));
                        vss.sndSectionData[4 * id + 3] = secOffset + BYTE2SEC(FRAME2BYTE((readPropertyInt32(sec1->GetProperty("Stationary section End")))));

                    }
                    totalMap.push_back(vss);
                }
            }
        }
    }
    
    *vss_size = totalMap.size();
    VoiceSampleStruct* array = new VoiceSampleStruct[totalMap.size()];
    std::copy(totalMap.begin(), totalMap.end(), array);
    FRET(array);
}
