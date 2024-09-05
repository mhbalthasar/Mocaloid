// stdafx.h : 标准系统包含文件的包含文件，
// 或是经常使用但不常更改的
// 特定于项目的包含文件
//

#pragma once
#ifdef LIBDDI_QTMAKE
#define QMsgBox(a,b,c) {;}
#else
#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             //  从 Windows 头文件中排除极少使用的信息


// TODO: 在此处引用程序需要的其他头文件

#include <stdio.h>
#include <tchar.h>

// DEFINE QMsgBox
#include <Windows.h>
#define QMsgBox(a,b,c) MessageBoxA(0,b,c,0)
#endif
