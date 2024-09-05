#ifndef SMSGENERICTRACK_H
#define SMSGENERICTRACK_H

#include "basechunk.h"
#include "smsregion.h"

class ChunkSMSGenericTrackChunk : public BaseChunk {
public:
    explicit ChunkSMSGenericTrackChunk() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "GTRK"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);

        CHUNK_TREADPROP("unk1", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("unk2", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("unk3", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("unk4", 8, PropertyType::PropU64Int);
        CHUNK_TREADPROP("unk5", 4, PropertyType::PropU32Int);
        CHUNK_TREADPROP("unk8", 1, PropertyType::PropU8Int);
        CHUNK_TREADPROP("Region count", 4, PropertyType::PropU32Int);

        uint32_t rgnCount; STUFF_INTO(GetProperty("Region count").data, rgnCount, uint32_t);
        for (size_t ii = 0; ii < rgnCount; ii++) {
            auto rgn = new ChunkSMSRegionChunk;
            rgn->Read(file);
            rgn->SetName(QString("Region %1").arg(ii));
            Children.append(rgn);
        }
    }

    virtual QString Description() {
        return "SMSGenericTrack";
    }

    static BaseChunk* Make() { return new ChunkSMSGenericTrackChunk; }
};

#endif // SMSGENERICTRACK_H
