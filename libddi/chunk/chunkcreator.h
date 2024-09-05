#ifndef CHUNKCREATOR_H
#define CHUNKCREATOR_H

#include <QtCore\qobject.h>
#include <QtCore\qmap.h>
#include "basechunk.h"

typedef BaseChunk*(*MakeMethod)() ;
/*
	chunkcreator.obj : error LNK2001: �޷��������ⲿ���� "public: virtual struct QMetaObject const * __thiscall ChunkCreator::metaObject(void)const " (?metaObject@ChunkCreator@@UBEPBUQMetaObject@@XZ)
    ������Ϊ��Դ�ļ���û�������moc_chunkcreator.cpp�ļ���
    ����������һ�chunkcreator.h��ѡ���Զ������ɲ��衱�������桱
    �����У�moc.exe chunkcreator.h -o moc_chunkcreator.cpp
    �����moc_chunkcreator.cpp
    ���������moc.exe chunkcreator.h
    ȷ����Ȼ���һ�chunkcreator.h��ѡ�� �����롱�������ļ���������moc_chunkcreator.cpp���ٽ�����ӵ�Դ�ļ��С�
*/
class ChunkCreator// : public QObject  
{
   // Q_OBJECT
public:
    explicit ChunkCreator(QObject *parent = nullptr);
    static ChunkCreator* Get();

    BaseChunk *ReadFor(QByteArray signature, FILE* file);
	
private:
    static ChunkCreator* mInstance;
	
    QMap<QByteArray, MakeMethod> FactoryMethods;
    template <typename T> void AddToFactory();

signals:

};

#endif // CHUNKCREATOR_H
