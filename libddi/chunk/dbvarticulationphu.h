#ifndef DBVARTICULATIONPHU_H
#define DBVARTICULATIONPHU_H

#include "chunkarray.h"

class ChunkDBVArticulationPhU : public ChunkChunkArray {
public:
    explicit ChunkDBVArticulationPhU() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "ARTu"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        CHUNK_READPROP("unk1", 4);
        CHUNK_READPROP("unk2", 4);
        CHUNK_READPROP("unk3", 4);
        CHUNK_READPROP("unk4", 4);
        CHUNK_READPROP("unk5", 4);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVArticulationPhU";
    }

    static BaseChunk* Make() { return new ChunkDBVArticulationPhU; }
};

#endif // DBVARTICULATIONPHU_H
