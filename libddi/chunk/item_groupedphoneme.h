#ifndef ITEM_GROUPEDPHONEME_H
#define ITEM_GROUPEDPHONEME_H

#include "basechunk.h"

class ItemGroupedPhoneme : public BaseChunk {
public:
    explicit ItemGroupedPhoneme() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____GroupedPhoneme"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        BaseChunk::Read(file);
        // Read phoneme name
        CHUNK_READPROP("Length", 4);
        uint32_t length; STUFF_INTO(GetProperty("Length").data, length, uint32_t);
        QByteArray tmp(length, 0);
        fread(tmp.data(), 1, length, file);
        mName = tmp;

        CHUNK_READPROP("Phoneme No", 4);
    }

    virtual QString Description() {
        return "<Grouped Phoneme>";
    }

    static BaseChunk* Make() { return new ItemGroupedPhoneme; }
};

#endif // ITEM_GROUPEDPHONEME_H
