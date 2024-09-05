#ifndef SMSFRAME_H
#define SMSFRAME_H

#include "basechunk.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qfile.h>

class ChunkSMSFrameChunk : public BaseChunk {
public:
    explicit ChunkSMSFrameChunk() : BaseChunk() {

    }

    static QByteArray ClassSignature() { return "FRM2"; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }

    virtual void Read(FILE *file) {
        auto originalOffset = ftell(file);

        ReadBlockSignature(file);

        QFile snd;
        if (snd.open(file, QFile::ReadOnly)) {
            snd.seek(originalOffset);
            rawData = snd.read(mSize);
            fseek(file, snd.pos(), SEEK_SET);
            snd.close();
        }
    }

    virtual QString Description() {
        return "SMSFrame";
    }

    QByteArray rawData;

    static BaseChunk* Make() { return new ChunkSMSFrameChunk; }
};

#endif // SMSFRAME_H
