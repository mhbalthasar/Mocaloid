#ifndef DBVVQMORPH_H
#define DBVVQMORPH_H

#include "chunkarray.h"

class ChunkDBVVQMorph : public ChunkChunkArray {
public:
    explicit ChunkDBVVQMorph() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "VQM "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVVQMorph";
    }

    static BaseChunk* Make() { return new ChunkDBVVQMorph; }
};

#endif // DBVVQMORPH_H
