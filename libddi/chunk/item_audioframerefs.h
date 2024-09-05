#ifndef ITEM_AUDIOFRAMEREFS_H
#define ITEM_AUDIOFRAMEREFS_H

#include "basechunk.h"

class ItemAudioFrameRefs : public BaseChunk {
public:
	ItemAudioFrameRefs(){m_count=0;};
	ItemAudioFrameRefs(BaseChunk& that):BaseChunk(that){m_count=0;};
    explicit ItemAudioFrameRefs(uint32_t count) : BaseChunk(), m_count(count) {
		ChunkProperty _tmp;
		_tmp.data=QByteArray::fromRawData((const char *)&m_count,
                                        sizeof(m_count));
		_tmp.type=PropertyType::PropU32Int;
        mAdditionalProperties["Count"] = _tmp;
    }

    static QByteArray ClassSignature() { return "____AudioFrameRefs"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        ReadOriginalOffset(file);
        for (uint32_t i = 0; i < m_count; i++) {
            CHUNK_TREADPROP(QString("Frame %1").arg(i, 5, 10, QChar('0')), 8, PropertyType::PropHex64);
        }
    }

    virtual QString Description() {
        return "<Audio frame references>";
    }

    static BaseChunk* Make() { return new ItemAudioFrameRefs(0); }

protected:
    uint32_t m_count;
};

#endif // ITEM_AUDIOFRAMEREFS_H
