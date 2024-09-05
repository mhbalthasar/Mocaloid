#ifndef DBVSTATIONARY_H
#define DBVSTATIONARY_H

#include "chunkarray.h"

class ChunkDBVStationary : public ChunkChunkArray {
public:
    explicit ChunkDBVStationary() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "STA "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVStationary";
    }

    static BaseChunk* Make() { return new ChunkDBVStationary; }
};

#endif // DBVSTATIONARY_H
