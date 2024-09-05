#ifndef BASECHUNK_H
#define BASECHUNK_H

#include <QtCore\qobject.h>
#include <QtCore\qbytearray.h>
#include <QtCore\qvector.h>
#include <QtCore\qmap.h>
#include <stdio.h>
#include "propertytype.h"
#include "..\qtadapter\define_type.h"

#define CHUNK_READPROP(name,size)            \
    do {                                     \
		QByteArray tmp(size, 0);                      \
		size_t offset = ftell(file);             \
		fread(tmp.data(), 1, size, file);                  \
		mAdditionalProperties[name] = ChunkProperty::init(tmp,PropRawHex,offset);               \
	} while (0)


#define CHUNK_TREADPROP(name,size,type)            \
    do {                                     \
		QByteArray tmp(size, 0);                      \
		size_t offset = ftell(file);             \
		fread(tmp.data(), 1, size, file);                  \
		mAdditionalProperties[name] = ChunkProperty::init(tmp,type,offset);               \
	} while (0)

#define STUFF_INTO(from,to,type) \
    memcpy(&to, from.constData(), sizeof(type))

#define CHUNK_READCHILD(type,whose) \
    do { \
        type* child = new type; \
        child->Read(file); \
        whose->Children.append(child); \
    } while(0)

struct ChunkProperty {
    QByteArray data;
    PropertyType type;
    size_t offset;

    operator QByteArray() { return data; } // Implicit conversion to QByteArray
    ChunkProperty operator=(QByteArray _data) { data = _data; return *this; }
	static ChunkProperty init(QByteArray a,PropertyType b,size_t c) {ChunkProperty _tmp;	_tmp.data=a;_tmp.type=b;_tmp.offset=c;return _tmp;}
};

class BaseChunk
{
public:
    explicit BaseChunk() {
        mSignature = QByteArray(4, 0);
        mSize = 0;
    }

    explicit BaseChunk(BaseChunk& that) {
        this->mName = that.mName;
        this->mSize = that.mSize;
        this->mSignature = that.mSignature;
    }

    virtual ~BaseChunk() {
		for(auto i:Children){
            if(i) delete i;
		}
    }

    static bool ArrayLeadingChunkName;
    static bool HasLeadingQword;
    const static int ItemChunkRole,
                     ItemPropDataRole,
                     ItemOffsetRole;

    static QByteArray ClassSignature() { return "    "; }
    virtual QByteArray ObjectSignature() { return ClassSignature(); }
    virtual void Read(FILE* file) { }
    virtual QString Description() { return "..."; }
    static BaseChunk* Make() { return nullptr; }

    QString GetName() { return mName; }
    void SetName(QString name) { mName = name; }
    ChunkProperty GetProperty(QString name) { return mAdditionalProperties.value(name, ChunkProperty()); }
    void SetProperty(QString name, ChunkProperty data) { mAdditionalProperties[name] = data; }
    uint64_t GetOriginalOffset() { return mOriginalOffset; }
    uint32_t GetSize() { return mSize; }
    const QMap<QString, ChunkProperty>& GetPropertiesMap() { return mAdditionalProperties; }
    QByteArray GetSignature() { return mSignature; }
    BaseChunk* GetChildByName(QString name) {
        for(auto i : Children) if(i->mName == name) return i; return nullptr;
    }

    QVector<BaseChunk*> Children;

protected:
    QByteArray mSignature;
    uint32_t mSize;
    QString mName;
    uint64_t mOriginalOffset;

    QMap<QString, ChunkProperty> mAdditionalProperties;

    void ReadBlockSignature(FILE* file) {
        ReadOriginalOffset(file);
        if(HasLeadingQword)
			CHUNK_READPROP("LeadingQword", 8);
        QByteArray tmp(4, 0);
        fread(tmp.data(), 1, 4, file); mSignature = tmp;
        fread(tmp.data(), 1, 4, file); STUFF_INTO(tmp, mSize, uint32_t);
    }

    void ReadStringName(FILE* file) {
        uint32_t length;
        fread(&length, 1, 4, file);
        QByteArray name(length, 0);
        fread(name.data(), 1, length, file);
        SetName(name);
    }

    void ReadOriginalOffset(FILE* file) {
        mOriginalOffset = ftell(file);
    }

};

Q_DECLARE_METATYPE(BaseChunk*);

#endif // BASECHUNK_H
