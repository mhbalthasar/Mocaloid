#ifndef DBVSTATIONARYPHUPART_H
#define DBVSTATIONARYPHUPART_H

#include "chunkarray.h"
#include "item_audioframerefs.h"

class ChunkDBVStationaryPhUPart : public ChunkChunkArray {
public:
    explicit ChunkDBVStationaryPhUPart() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "STAp"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        uint32_t frameCount;//, unk15;
        ReadBlockSignature(file);
        ReadArrayHead(file);
        CHUNK_TREADPROP("unk1", 8, PropertyType::PropHex64);
        CHUNK_READPROP("unk2", 2);
        CHUNK_TREADPROP("mPitch", 4, PropertyType::PropF32);
        CHUNK_TREADPROP("Average pitch", 4, PropertyType::PropF32);
        CHUNK_READPROP("unk5", 4);
        CHUNK_TREADPROP("Dynamic", 4, PropertyType::PropF32);
        CHUNK_TREADPROP("Tempo", 4, PropertyType::PropF32);
        CHUNK_READPROP("unk8", 4);
        ReadArrayBody(file, 0);
        CHUNK_READPROP("unk9", 4);

        CHUNK_TREADPROP("Frame count", 4, PropertyType::PropU32Int);
        STUFF_INTO(GetProperty("Frame count").data, frameCount, uint32_t);
        {
            auto frameDir = new ItemAudioFrameRefs(frameCount);
            frameDir->SetName("<Frames>");
            frameDir->Read(file);
            Children.append(frameDir);
        }

        CHUNK_TREADPROP("SND Sample rate", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("SND Channel count", 2, PropertyType::PropU16Int);
        CHUNK_READPROP("SND Sample count", 4);
        CHUNK_TREADPROP("SND Sample offset", 8, PropertyType::PropHex64);
        CHUNK_READPROP("unk14", 4);
        CHUNK_READPROP("unk15", 4);
        CHUNK_READPROP("unk16", 4);
        CHUNK_READPROP("unk17", 4);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVStationaryPhUPart";
    }

    static BaseChunk* Make() { return new ChunkDBVStationaryPhUPart; }
};

#endif // DBVSTATIONARYPHUPART_H
