#ifndef DBSINGER_H
#define DBSINGER_H

#include "chunkarray.h"
#include "phonemedict.h"
#include "chunkreaderguards.h"

#define DBTYPE_DAISY_VOCALOID3 0;
#define DBTYPE_DAISY_VOCALOID2 1;

class ChunkDBSinger : public ChunkChunkArray {
public:
    explicit ChunkDBSinger() : ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "DBSe"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE* file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        
        CHUNK_READPROP("unk6", 4);

        {
            // Emulate Vocaloid behavior
            LeadingQwordGuard qwg(false);

            CHUNK_READCHILD(ChunkPhonemeDict, this);
            
            CHUNK_READPROP("Hash store", 260);
        }

        ReadArrayBody(file, 0);

    }

    virtual QString Description() {
        return "DBSinger";
    }

    static BaseChunk* Make() {
        return new ChunkDBSinger();
    }
};

#endif // DBSINGER_H
