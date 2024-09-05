#ifndef DBVVQMORPHPHU_H
#define DBVVQMORPHPHU_H

#include "chunkarray.h"

class ChunkDBVVQMorphPhU : public ChunkChunkArray {
public:
    explicit ChunkDBVVQMorphPhU() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "VQMu"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        CHUNK_TREADPROP("Index", 4, PropertyType::PropU32Int);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "DBVVQMorphPhU";
    }

    static BaseChunk* Make() { return new ChunkDBVVQMorphPhU; }
};

#endif // DBVVQMORPHPHU_H
