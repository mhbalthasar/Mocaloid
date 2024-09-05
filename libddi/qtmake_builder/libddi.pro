QT -= gui

TEMPLATE = lib
DEFINES += LIBDDI_QTMAKE

CONFIG += c++17

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
    ../chunk/basechunk.cpp \
    ../chunk/chunkcreator.cpp \
    ../chunk/propertytype.cpp \
    ../ddi.cpp \
    ../stdafx.cpp

HEADERS += \
    ../chunk/basechunk.h \
    ../chunk/chunkarray.h \
    ../chunk/chunkcreator.h \
    ../chunk/chunkreaderguards.h \
    ../chunk/dbsinger.h \
    ../chunk/dbtimbre.h \
    ../chunk/dbtimbremodel.h \
    ../chunk/dbvarticulation.h \
    ../chunk/dbvarticulationphu.h \
    ../chunk/dbvarticulationphupart.h \
    ../chunk/dbvarticulationphupart.h.orig \
    ../chunk/dbvoice.h \
    ../chunk/dbvstationary.h \
    ../chunk/dbvstationaryphu.h \
    ../chunk/dbvstationaryphupart.h \
    ../chunk/dbvvqmorph.h \
    ../chunk/dbvvqmorphphu.h \
    ../chunk/dbvvqmorphphupart.h \
    ../chunk/emptychunk.h \
    ../chunk/item_articulationsection.h \
    ../chunk/item_audioframerefs.h \
    ../chunk/item_directory.h \
    ../chunk/item_eprguides.h \
    ../chunk/item_groupedphoneme.h \
    ../chunk/item_phonemegroup.h \
    ../chunk/item_phoneticunit.h \
    ../chunk/phonemedict.h \
    ../chunk/phonemegroup.h \
    ../chunk/propertytype.h \
    ../chunk/skipchunk.h \
    ../chunk/smsframe.h \
    ../chunk/smsgenerictrack.h \
    ../chunk/smsregion.h \
    ../chunk/soundchunk.h \
    ../ddi.h \
    ../qtadapter/define_type.h \
    ../stdafx.h \
    ../targetver.h

# Default rules for deployment.

CURRENT_DIR = $$PWD
LIBDDIPROJECT_DIR = $$dirname(CURRENT_DIR)
MOCALOID_DIR = $$dirname(LIBDDIPROJECT_DIR)
DEST_DIR = $MOCALOID_DIR/Output

unix {
    target.path = /usr/lib
}
!isEmpty(target.path): INSTALLS += target
