#ifndef CHUNKARRAY_H
#define CHUNKARRAY_H

#include "basechunk.h"
#include "chunkcreator.h"

class ChunkChunkArray : public BaseChunk {
public:
    explicit ChunkChunkArray() {

    }

    static QByteArray ClassSignature() { return "ARR "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE* file) {
        ReadBlockSignature(file);
        ReadArrayHead(file);
        ReadArrayBody(file, 0);
        ReadStringName(file);
    }

    virtual QString Description() {
        return "ChunkArray";
    }

    static BaseChunk* Make() {
        return new ChunkChunkArray();
    }

protected:
    void ReadArrayHead(FILE* file) {
        CHUNK_READPROP("unk4", 4);
        CHUNK_TREADPROP("UseEmptyChunk", 4, PropertyType::PropU32Int);
    }

    void ReadArrayBody(FILE* file, uint32_t maxCount = 1) {
        char signatureBuf[4];
        uint32_t count;
        BaseChunk::Read(file);

        // Read subchunk count
        CHUNK_TREADPROP("Count", 4, PropertyType::PropU32Int);
        STUFF_INTO(GetProperty("Count").data, count, uint32_t);


        //FIXME: JUST FOR TEST
        for(uint32_t ii = 0; ii < (maxCount == 0 ? count : maxCount); ii++) {

            if (BaseChunk::ArrayLeadingChunkName)
                ReadStringName(file); // HACK: Use current array's name as a temporary variable

            // Read signature first
            fpos_t pos;
            fgetpos(file, &pos);
            // Skip Leading QWORD if needed
            if(HasLeadingQword) fseek(file, 8, SEEK_CUR);
            fread(signatureBuf, 1, 4, file);
            fsetpos(file, &pos);

            auto sig = QByteArray(signatureBuf, 4);
            auto chk = ChunkCreator::Get()->ReadFor(sig, file);
            if(chk) {
                if (BaseChunk::ArrayLeadingChunkName)
                    chk->SetName(GetName());
                Children.append(chk);
            } else
                break;
        }
    }
};

#endif // CHUNKARRAY_H
