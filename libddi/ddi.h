#pragma once
#include "qtadapter/define_type.h"
#ifdef LIBDDI_QTMAKE
#include <QtCore/qglobal.h>
#define DDI_API Q_DECL_EXPORT
#else
#define DDI_API __declspec(dllexport)
#endif

#define PST_VOWEL_ONLY 0
#define PST_CONSONANT_ONLY 1
#define PST_CONSONANT_VOWEL 2
#define PST_VOWEL_CONSONANT 3
#define PST_CONSONANT_CONSONANT 4
#define PST_VOWEL_VOWEL 5
#define PST_REST_CONSONANT 6
#define PST_REST_VOWEL 7
#define PST_VOWEL_REST 8
#define PST_CONSONANT_REST 9

struct VoiceSampleStruct {
    int type;//0:sta,1:diphone_art,2:triphone_art
    char phonemeBlocks[1024];//oneBlock is 256,maxBlockCount is 4;
    char phonemeEncodedBlocks[1024];//oneBlock is 256,maxBlockCount is 4;
    int phonemeCount;//blockCount;
    int phonemeType;
    float relPitch;
    float pitch;
    float tempo;
    int sndSampleRate;
    int sndChannel;
    LONG64 sndLength;
    LONG64 sndOffset;
    float sndLBound;
    float sndSectionData[16];//EachBlock 4;maxBlockCount is 4;
};
extern "C"{
	DDI_API VoiceSampleStruct* AnalysisDDI(char* ddiFile,char** error,int* vss_size);
	DDI_API void FreeDDI(VoiceSampleStruct* array);
    DDI_API BOOL HashDDI(char* ddiFile, char** ddiHash);
}

