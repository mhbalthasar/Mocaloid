#include "chunkcreator.h"

// Chunks
#include "chunkarray.h"
#include "dbsinger.h"
#include "dbvoice.h"
#include "emptychunk.h"
#include "dbvarticulation.h"
#include "dbvarticulationphu.h"
#include "dbvarticulationphupart.h"
#include "dbvstationary.h"
#include "dbvstationaryphu.h"
#include "dbvstationaryphupart.h"
#include "dbvvqmorph.h"
#include "dbvvqmorphphu.h"
#include "dbvvqmorphphupart.h"
#include "dbtimbre.h"
#include "dbtimbremodel.h"

#include "smsgenerictrack.h"
#include "smsregion.h"
#include "smsframe.h"
#include "soundchunk.h"

// Items
#include "item_directory.h"
#include "item_eprguides.h"
#include "item_groupedphoneme.h"
#include "item_phonemegroup.h"
#include "item_phoneticunit.h"

#include <QtCore/qdebug.h>

ChunkCreator *ChunkCreator::mInstance = nullptr;
bool BaseChunk::HasLeadingQword = true;
bool BaseChunk::ArrayLeadingChunkName = false;
const int BaseChunk::ItemChunkRole = Qt::UserRole + 1;
const int BaseChunk::ItemPropDataRole = Qt::UserRole + 2;
const int BaseChunk::ItemOffsetRole = Qt::UserRole + 2;

ChunkCreator::ChunkCreator(QObject *parent)
{
    if (!FactoryMethods.empty())
        return;

    AddToFactory<ChunkChunkArray>();
    AddToFactory<ChunkDBSinger>();
    AddToFactory<ChunkDBVoice>();
    AddToFactory<ChunkEmptyChunk>();
    AddToFactory<ChunkDBVArticulation>();
    AddToFactory<ChunkDBVArticulationPhU>();
    AddToFactory<ChunkDBVArticulationPhUPart>();
    AddToFactory<ChunkDBVStationary>();
    AddToFactory<ChunkDBVStationaryPhU>();
    AddToFactory<ChunkDBVStationaryPhUPart>();
    AddToFactory<ChunkDBVVQMorph>();
    AddToFactory<ChunkDBVVQMorphPhU>();
    AddToFactory<ChunkDBVVQMorphPhUPart>();
    AddToFactory<ChunkDBTimbre>();
    AddToFactory<ChunkDBTimbreModel>();

    AddToFactory<ChunkSMSGenericTrackChunk>();
    AddToFactory<ChunkSMSRegionChunk>();
    AddToFactory<ChunkSMSFrameChunk>();
    AddToFactory<ChunkSoundChunk>();

    AddToFactory<ItemDirectory>();
    AddToFactory<ItemEprGuide>();
    AddToFactory<ItemEprGuidesGroup>();
    AddToFactory<ItemGroupedPhoneme>();
    AddToFactory<ItemPhonemeGroup>();
    AddToFactory<ItemPhoneticUnit>();

    //for(auto i : FactoryMethods.keys())
    //    qDebug() << i << FactoryMethods[i];

}

ChunkCreator *ChunkCreator::Get()
{
    if (mInstance == nullptr)
        mInstance = new ChunkCreator;
    return mInstance;
}

BaseChunk *ChunkCreator::ReadFor(QByteArray signature, FILE *file)
{
    if(!FactoryMethods.contains(signature))
        return nullptr;
    auto ret = FactoryMethods[signature]();
    ret->Read(file);
    return ret;
}

template<typename T>
void ChunkCreator::AddToFactory()
{
    FactoryMethods[T::ClassSignature()] = &T::Make;
}
