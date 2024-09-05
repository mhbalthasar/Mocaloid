#ifndef CHUNKCREATOR_H
#define CHUNKCREATOR_H

#include <QtCore\qobject.h>
#include <QtCore\qmap.h>
#include "basechunk.h"

typedef BaseChunk*(*MakeMethod)() ;
/*
	chunkcreator.obj : error LNK2001: 无法解析的外部符号 "public: virtual struct QMetaObject const * __thiscall ChunkCreator::metaObject(void)const " (?metaObject@ChunkCreator@@UBEPBUQMetaObject@@XZ)
    这是因为在源文件中没有添加上moc_chunkcreator.cpp文件。
    解决方法：右击chunkcreator.h，选择“自定义生成步骤”，“常规”
    命令行：moc.exe chunkcreator.h -o moc_chunkcreator.cpp
    输出：moc_chunkcreator.cpp
    附加依赖项：moc.exe chunkcreator.h
    确定，然后，右击chunkcreator.h，选择 “编译”，则在文件夹中生成moc_chunkcreator.cpp，再将其添加到源文件中。
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
