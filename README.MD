## 关于Mocaloid

Mocaloid是一个使用Moresample引擎取代VOCALOID合成引擎的传统拼接合成实验引擎。它将尝试一种集成的方法从Vocaloid Daisy VoiceBank中获取必要的采样和标记信息，并交付给Moresample合成声音。

## 知识库来源

本仓库引用了下列开源项目的研究成果：

DDB文件结构和采样解压方面：

1. https://github.com/641i130/ddb-extraction
2. https://github.com/yuukawahiroshi/ddb-tools

标记、音高、和TextGrideLabel信息方面：

3. https://github.com/shmishtopher/VAU
4. https://github.com/RigoLigoRLC/ddiview

其他参考资料：
5. https://github.com/praat/praat

## 其他
关于编译

- 1.程序主体使用AvaloniaUI + VS2022(.net8)编译。Windows平台使用IDE/VSCode配合Nuget直接编译，Linux/Mac平台使用dotnet命令/VSCode配合编译即可。
- 
- 2.libddi解析库使用基于QT5.15的环境编译，目前提供2种编译配置：
  - 2.1 VC110-windows编译
         为了简化发布，代码静态编译并携带了QTCore库，在windows下可以使用VC110直接编译32位版本。

  - 2.2 QTCreator编译
         考虑到跨平台需求，可下载安装QT5.15开发环境，使用QTCreator打开libddi.pro文件编译，支持MingGW/GCC/VC编译。编译完成后将libddi.dll复制到MocaloidApi.dll同目录即可。

  - PS：libddi的平台位数和程序主体应一致，要么的都是x86要么都是x64。

本仓库为分析实验仓库，禁止用于任何商业目的。
