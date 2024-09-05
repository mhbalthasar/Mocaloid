#ifndef ITEMDIRECTORY_H
#define ITEMDIRECTORY_H

#include "basechunk.h"

class ItemDirectory : public BaseChunk {
public:
    explicit ItemDirectory() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____Directory"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadOriginalOffset(file);
        BaseChunk::Read(file);
    }

    virtual QString Description() {
        return "(Directory)";
    }

    static BaseChunk* Make() { return new ItemDirectory; }
};

#endif // ITEMDIRECTORY_H
