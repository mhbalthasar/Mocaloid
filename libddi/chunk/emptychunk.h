#ifndef EMPTYCHUNK_H
#define EMPTYCHUNK_H

#include "basechunk.h"

class ChunkEmptyChunk : public BaseChunk {
public:
    explicit ChunkEmptyChunk() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "EMPT"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "Empty chunk";
    }

    static BaseChunk* Make() { return new ChunkEmptyChunk; }
};

#endif // EMPTYCHUNK_H
