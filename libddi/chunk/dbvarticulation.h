#ifndef DBVARTICULATION_H
#define DBVARTICULATION_H

#include "chunkarray.h"

class ChunkDBVArticulation : public ChunkChunkArray {
public:
    explicit ChunkDBVArticulation() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "ART "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        CHUNK_TREADPROP("Index", 4, PropertyType::PropU32Int);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVArticulation";
    }

    static BaseChunk* Make() { return new ChunkDBVArticulation; }
};

#endif // DBVARTICULATION_H
