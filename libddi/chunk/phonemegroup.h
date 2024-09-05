#ifndef PHONEMEGROUP_H
#define PHONEMEGROUP_H

#include "basechunk.h"
#include "item_phonemegroup.h"
#include "item_directory.h"

class ChunkPhonemeGroup : public BaseChunk {
public:
    explicit ChunkPhonemeGroup() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "PHG2"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE* file) {
        ReadBlockSignature(file);
        CHUNK_READPROP("Group count", 4);

        // Groups
        uint32_t GroupCount;
        STUFF_INTO(GetProperty("Group count").data, GroupCount, uint32_t);
        for(uint32_t ii = 0; ii < GroupCount; ii++) {
            CHUNK_READCHILD(ItemPhonemeGroup, this);
        }

        SetName("<Phoneme Groups>");
    }

    virtual QString Description() {
        return "PhonemeGroup";
    }

    static BaseChunk* Make() {
        return new ChunkPhonemeGroup();
    }
};

#endif // PHONEMEGROUP_H
