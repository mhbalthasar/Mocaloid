#ifndef SMSREGION_H
#define SMSREGION_H

#include "basechunk.h"
#include "smsframe.h"
#include "skipchunk.h"

class ChunkSMSRegionChunk : public BaseChunk {
public:
    explicit ChunkSMSRegionChunk() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "RGN "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadBlockSignature(file);

        CHUNK_TREADPROP("unk1", 8, PropertyType::PropU64Int);
        CHUNK_TREADPROP("unk2", 1, PropertyType::PropU8Int);

        uint8_t flags1, flags2;
        CHUNK_TREADPROP("Flags1", 1, PropertyType::PropU8Int); STUFF_INTO(GetProperty("Flags1").data, flags1, uint8_t);
        if (flags1 & 0x01) {
            CHUNK_TREADPROP("unk4", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk5", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk6", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk7", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk8", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk9", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk10", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk11", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk12", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk13", 8, PropertyType::PropU64Int);
            CHUNK_TREADPROP("unk14", 8, PropertyType::PropU64Int);
            CHUNK_READPROP("unk15", 80);
            CHUNK_READPROP("unk16", 160);
            CHUNK_READPROP("unk17", 28);
        }
        if (flags1 & 0x02)
            CHUNK_TREADPROP("unk18", 1, PropertyType::PropU8Int);
        if (flags1 & 0x04)
            CHUNK_TREADPROP("unk19", 4, PropertyType::PropU32Int);
        if (flags1 & 0x08) {
            uint32_t unk20;
            CHUNK_TREADPROP("unk20", 4, PropertyType::PropU32Int); STUFF_INTO(GetProperty("unk20").data, unk20, uint32_t);
            CHUNK_READPROP("unk21", unk20 * 16);
        }
        if (flags1 & 0x10) {
            uint32_t unk22;
            CHUNK_TREADPROP("unk22", 4, PropertyType::PropU32Int); STUFF_INTO(GetProperty("unk22").data, unk22, uint32_t);
            CHUNK_READPROP("unk23", unk22 * 8);
        }
        if (flags1 & 0x20) {
            CHUNK_READPROP("unk24", 24);
            CHUNK_TREADPROP("unk25", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("unk26", 4, PropertyType::PropU32Int);
        }
        if (flags1 & 0x40) {
            CHUNK_READPROP("unk27", 48);
        }

        CHUNK_TREADPROP("Flags2", 1, PropertyType::PropU8Int); STUFF_INTO(GetProperty("Flags2").data, flags2, uint8_t);
        if (flags2 & 0x01) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x02) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x04) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x08) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x10) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x20) CHUNK_READCHILD(ChunkSkipChunk, this);
        if (flags2 & 0x40) {
            CHUNK_TREADPROP("Stable region begin", 4, PropertyType::PropU32Int);
            CHUNK_TREADPROP("Stable region end", 4, PropertyType::PropU32Int);
        }

        uint32_t frameCount;
        CHUNK_TREADPROP("Frame count", 4, PropertyType::PropU32Int); STUFF_INTO(GetProperty("Frame count").data, frameCount, uint32_t);
        for (size_t ii = 0; ii < frameCount; ii++) {
            auto frame = new ChunkSMSFrameChunk;
            frame->Read(file);
            frame->SetName(QString("Frame %1").arg(ii, 5, QChar('0')));
            Children.append(frame);
        }
    }

    virtual QString Description() {
        return "SMSRegion";
    }

    static BaseChunk* Make() { return new ChunkSMSRegionChunk; }
};

#endif // SMSREGION_H
