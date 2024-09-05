#ifndef ITEM_EPRGUIDES_H
#define ITEM_EPRGUIDES_H

#include "basechunk.h"

class ItemEprGuide : public BaseChunk {
public:
    explicit ItemEprGuide() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____EprGuide"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        uint32_t paramCount;

        BaseChunk::Read(file);
        CHUNK_READPROP("Name", 32);
        CHUNK_READPROP("Parameter count", 4);
        STUFF_INTO(GetProperty("Parameter count").data, paramCount, uint32_t);
        for(uint32_t i = 0; i < paramCount; i++) {
            CHUNK_READPROP(QString("Offset %1 A").arg(i, 4, 10, QChar('0')), 8);
            CHUNK_READPROP(QString("Offset %1 B").arg(i, 4, 10, QChar('0')), 8);
        }
        SetName(GetProperty("Name").data);
    }

    virtual QString Description() {
        return "(EpR Guide)";
    }

    static BaseChunk* Make() { return new ItemEprGuide; }
};

class ItemEprGuidesGroup : public BaseChunk {
public:
    explicit ItemEprGuidesGroup() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____EprGuidesGroup"; }

    virtual void Read(FILE *file) {
        uint32_t groupCount;

        BaseChunk::Read(file);
        CHUNK_READPROP("Count", 4);
        STUFF_INTO(GetProperty("Count").data, groupCount, uint32_t);
        for(uint32_t i = 0; i < groupCount; i++) {
            CHUNK_READCHILD(ItemEprGuide, this);
        }

        SetName("<EpR Guides>");
    }

    virtual QString Description() {
        return "(EpR Guides Group)";
    }

    static BaseChunk* Make() { return new ItemEprGuidesGroup; }
};

#endif // ITEM_EPRGUIDES_H
