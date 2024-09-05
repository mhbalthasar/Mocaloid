#ifndef ITEM_PHONEMEGROUP_H
#define ITEM_PHONEMEGROUP_H

#include "basechunk.h"
#include "item_groupedphoneme.h"

class ItemPhonemeGroup : public BaseChunk {
public:
    explicit ItemPhonemeGroup() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____PhonemeGroup"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        BaseChunk::Read(file);
        // Read group name
        CHUNK_READPROP("Name length", 4);
        uint32_t length; STUFF_INTO(GetProperty("Name length").data, length, uint32_t);
        QByteArray tmp(length, 0);
        fread(tmp.data(), 1, length, file);
        mName = tmp;

        CHUNK_READPROP("Phoneme count", 4);
        CHUNK_READPROP("unk1", 4);

        // Read phoneme items
        uint32_t count; STUFF_INTO(GetProperty("Phoneme count").data, count, uint32_t);
        for(uint32_t ii = 0; ii < count; ii++) {
            CHUNK_READCHILD(ItemGroupedPhoneme, this);
        }
    }

    virtual QString Description() {
        return "<Phoneme Group>";
    }

    static BaseChunk* Make() { return new ItemPhonemeGroup; }
};

#endif // ITEM_PHONEMEGROUP_H
