#ifndef CHUNKREADERGUARDS_H
#define CHUNKREADERGUARDS_H

#include "basechunk.h"

class LeadingQwordGuard {
public:
//	LeadingQwordGuard() = &delete;
    LeadingQwordGuard(bool enable) {
        prev = BaseChunk::HasLeadingQword;
        BaseChunk::HasLeadingQword = enable;
    }
    ~LeadingQwordGuard() {
        BaseChunk::HasLeadingQword = prev;
    }

private:
    bool prev;
};

class ArrayLeadingNameGuard {
public:
//    ArrayLeadingNameGuard() = delete;
    ArrayLeadingNameGuard(bool enable) {
        prev = BaseChunk::ArrayLeadingChunkName;
        BaseChunk::ArrayLeadingChunkName = enable;
    }
    ~ArrayLeadingNameGuard() {
        BaseChunk::ArrayLeadingChunkName = prev;
    }

private:
    bool prev;
};

#endif // CHUNKREADERGUARDS_H
