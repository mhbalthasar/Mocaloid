#ifndef ITEM_PHONETICUNIT_H
#define ITEM_PHONETICUNIT_H

#include "basechunk.h"

class ItemPhoneticUnit : public BaseChunk {
public:
    explicit ItemPhoneticUnit() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____PhoneticUnit"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        BaseChunk::Read(file);
        QByteArray tmp(18, 0);
        fread(tmp.data(), 1, 18, file);
        mName = tmp;

        CHUNK_READPROP("unk1", 4);
        CHUNK_READPROP("unk2", 4);
        CHUNK_READPROP("unk3", 4);
        CHUNK_READPROP("Unvoiced", 1);
    }

    virtual QString Description() {
        return "<Phonetic unit>";
    }

    static BaseChunk* Make() { return new ItemPhoneticUnit; }
};

#endif // ITEM_PHONETICUNIT_H
