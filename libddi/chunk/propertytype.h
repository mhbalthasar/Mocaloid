#ifndef PROPERTYTYPE_H
#define PROPERTYTYPE_H

#include <QtCore\qstring.h>

enum PropertyType {
    PropRawHex = 0,
    PropU8Int, PropS8Int, PropHex8,
    PropU16Int, PropS16Int, PropHex16,
    PropU32Int, PropS32Int, PropHex32,
    PropU64Int, PropS64Int, PropHex64,
    PropF32, PropF64,
    PropString,

    PropertyTypeCount
};
extern const char* PropertyTypeNames[];
struct ChunkProperty;
QString FormatProperty(const ChunkProperty &prop);

#endif // PROPERTYTYPE_H
