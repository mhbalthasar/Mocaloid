#ifndef DBVTIMBRE_H
#define DBVTIMBRE_H

#include "chunkarray.h"

class ChunkDBTimbre : public ChunkChunkArray {
public:
    explicit ChunkDBTimbre() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "TDB "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBTimbre";
    }

    static BaseChunk* Make() { return new ChunkDBTimbre; }
};

#endif // DBVTIMBRE_H
