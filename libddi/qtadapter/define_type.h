#ifdef LIBDDI_QTMAKE
//QTMAKER
typedef long long LONG64;
typedef int BOOL;
#define TRUE 1
#define FALSE 0
#define max(a,b)            (((a) > (b)) ? (a) : (b))
#define min(a,b)            (((a) < (b)) ? (a) : (b))
#else
//VCMAKER
#ifndef NDEBUG
#pragma comment(lib, "../../libddi/base_library/lib/Qt5Cored.lib")
#else
#pragma comment(lib, "../../libddi/base_library/lib/Qt5Core.lib")
#endif
#pragma comment(lib, "Ws2_32.lib")
#pragma warning(disable:4204)

typedef unsigned char BYTE;
typedef unsigned __int8 uint8_t;
typedef unsigned __int16 uint16_t;
typedef unsigned __int32 uint32_t;
typedef unsigned __int64 uint64_t;
typedef __int8 int8_t;
typedef __int16 int16_t;
typedef __int32 int32_t;
typedef __int64 int64_t;
#endif

typedef unsigned long qsize_t;
#define PathSplitChar '\\'
