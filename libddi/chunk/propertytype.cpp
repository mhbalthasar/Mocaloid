#include "propertytype.h"
#include "basechunk.h"

const char* PropertyTypeNames[] = {
    "PropRawHex",
    "PropU8Int",
    "PropS8Int",
    "PropHex8",
    "PropU16Int",
    "PropS16Int",
    "PropHex16",
    "PropU32Int",
    "PropS32Int",
    "PropHex32",
    "PropU64Int",
    "PropS64Int",
    "PropHex64",
    "PropF32",
    "PropF64",
    "PropString",
};

QString FormatProperty(const ChunkProperty &prop)
{
    QString ret;
    auto type = prop.type;
    auto &data = prop.data;

    switch(type) {
    case PropRawHex: {
        ret = data.toHex();//' '
        break;
    }
    case PropU8Int: {
        uint8_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropS8Int: {
        int8_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropHex8: {
        uint8_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x, 16).toUpper();
        break;
    }
    case PropU16Int: {
        uint16_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropS16Int: {
        int16_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropHex16: {
        uint16_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x, 16).toUpper();
        break;
    }
    case PropU32Int: {
        uint32_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropS32Int: {
        int32_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropHex32: {
        uint32_t x; STUFF_INTO(data, x, decltype(x)); ret = ret = ret.number(x, 16).toUpper();
        break;
    }
    case PropU64Int: {
        uint64_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropS64Int: {
        int64_t x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropHex64: {
        uint64_t x; STUFF_INTO(data, x, decltype(x)); ret = ret = ret.number(x, 16).toUpper();
        break;
    }
    case PropF32: {
        float x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropF64: {
        double x; STUFF_INTO(data, x, decltype(x)); ret = ret.number(x);
        break;
    }
    case PropString: {
        QString x(data); ret = x;
        break;
    }
    }

    return ret;
}
