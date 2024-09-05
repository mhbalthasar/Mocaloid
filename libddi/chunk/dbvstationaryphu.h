#ifndef DBVSTATIONARYPHU_H
#define DBVSTATIONARYPHU_H

#include "chunkarray.h"

class ChunkDBVStationaryPhU : public ChunkChunkArray {
public:
    explicit ChunkDBVStationaryPhU() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "STAu"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        CHUNK_TREADPROP("Index", 4, PropertyType::PropU32Int);
        CHUNK_READPROP("unk2", 4);
        CHUNK_READPROP("unk3", 4);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVStationaryPhU";
    }

    static BaseChunk* Make() { return new ChunkDBVStationaryPhU; }
};

#endif // DBVSTATIONARYPHU_H
