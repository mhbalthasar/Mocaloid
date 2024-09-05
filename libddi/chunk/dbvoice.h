#ifndef DBVOICE_H
#define DBVOICE_H

#include "chunkarray.h"

class ChunkDBVoice : public ChunkChunkArray {
public:
    explicit ChunkDBVoice() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "DBV "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ChunkChunkArray::Read(file);
    }

    virtual QString Description() {
        return "DBVoice";
    }

    static BaseChunk* Make() { return new ChunkDBVoice; }
};

#endif // DBVOICE_H
