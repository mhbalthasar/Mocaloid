#ifndef ITEM_ARTICULATIONSECTION_H
#define ITEM_ARTICULATIONSECTION_H

#include "basechunk.h"

class ItemArticulationSection : public BaseChunk {
public:
    explicit ItemArticulationSection() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "____ArticulationSection"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadOriginalOffset(file);
        CHUNK_TREADPROP("Entire section Begin", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("Entire section End", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("Stationary section Begin", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("Stationary section End", 4, PropertyType::PropU32Int);
    }

    virtual QString Description() {
        return "<Articulation section>";
    }

    static BaseChunk* Make() { return new ItemArticulationSection; }
};

#endif // ITEM_ARTICULATIONSECTION_H
