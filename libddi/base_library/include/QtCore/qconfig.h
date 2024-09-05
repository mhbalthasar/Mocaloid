/* Everything */

/* Qt was configured for a static build */
#if !defined(QT_SHARED) && !defined(QT_STATIC)
# define QT_STATIC
#endif

/* License information */
#define QT_PRODUCT_LICENSEE "Open Source"
#define QT_PRODUCT_LICENSE "OpenSource"

// Qt Edition
#ifndef QT_EDITION
#  define QT_EDITION QT_EDITION_OPENSOURCE
#endif


// Compiler sub-arch support
#define QT_COMPILER_SUPPORTS_SSE2
#define QT_COMPILER_SUPPORTS_SSE3
#define QT_COMPILER_SUPPORTS_SSSE3
#define QT_COMPILER_SUPPORTS_SSE4_1
#define QT_COMPILER_SUPPORTS_SSE4_2
#define QT_COMPILER_SUPPORTS_AVX
#define QT_COMPILER_SUPPORTS_AVX2

// Compile time features
#if defined(QT_LARGEFILE_SUPPORT) && defined(QT_NO_LARGEFILE_SUPPORT)
# undef QT_LARGEFILE_SUPPORT
#elif !defined(QT_LARGEFILE_SUPPORT)
# define QT_LARGEFILE_SUPPORT 64
#endif

#if defined(QT_NO_ACCESSIBILITY) && defined(QT_ACCESSIBILITY)
# undef QT_NO_ACCESSIBILITY
#elif !defined(QT_NO_ACCESSIBILITY)
# define QT_NO_ACCESSIBILITY
#endif

#if defined(QT_NO_CUPS) && defined(QT_CUPS)
# undef QT_NO_CUPS
#elif !defined(QT_NO_CUPS)
# define QT_NO_CUPS
#endif

#if defined(QT_NO_DBUS) && defined(QT_DBUS)
# undef QT_NO_DBUS
#elif !defined(QT_NO_DBUS)
# define QT_NO_DBUS
#endif

#if defined(QT_NO_EVENTFD) && defined(QT_EVENTFD)
# undef QT_NO_EVENTFD
#elif !defined(QT_NO_EVENTFD)
# define QT_NO_EVENTFD
#endif

#if defined(QT_NO_FONTCONFIG) && defined(QT_FONTCONFIG)
# undef QT_NO_FONTCONFIG
#elif !defined(QT_NO_FONTCONFIG)
# define QT_NO_FONTCONFIG
#endif

#if defined(QT_NO_FREETYPE) && defined(QT_FREETYPE)
# undef QT_NO_FREETYPE
#elif !defined(QT_NO_FREETYPE)
# define QT_NO_FREETYPE
#endif

#if defined(QT_NO_GLIB) && defined(QT_GLIB)
# undef QT_NO_GLIB
#elif !defined(QT_NO_GLIB)
# define QT_NO_GLIB
#endif

#if defined(QT_NO_GUI) && defined(QT_GUI)
# undef QT_NO_GUI
#elif !defined(QT_NO_GUI)
# define QT_NO_GUI
#endif

#if defined(QT_NO_ICONV) && defined(QT_ICONV)
# undef QT_NO_ICONV
#elif !defined(QT_NO_ICONV)
# define QT_NO_ICONV
#endif

#if defined(QT_NO_IMAGEFORMAT_JPEG) && defined(QT_IMAGEFORMAT_JPEG)
# undef QT_NO_IMAGEFORMAT_JPEG
#elif !defined(QT_NO_IMAGEFORMAT_JPEG)
# define QT_NO_IMAGEFORMAT_JPEG
#endif

#if defined(QT_NO_IMAGEFORMAT_PNG) && defined(QT_IMAGEFORMAT_PNG)
# undef QT_NO_IMAGEFORMAT_PNG
#elif !defined(QT_NO_IMAGEFORMAT_PNG)
# define QT_NO_IMAGEFORMAT_PNG
#endif

#if defined(QT_NO_INOTIFY) && defined(QT_INOTIFY)
# undef QT_NO_INOTIFY
#elif !defined(QT_NO_INOTIFY)
# define QT_NO_INOTIFY
#endif

#if defined(QT_NO_NATIVE_GESTURES) && defined(QT_NATIVE_GESTURES)
# undef QT_NO_NATIVE_GESTURES
#elif !defined(QT_NO_NATIVE_GESTURES)
# define QT_NO_NATIVE_GESTURES
#endif

#if defined(QT_NO_NIS) && defined(QT_NIS)
# undef QT_NO_NIS
#elif !defined(QT_NO_NIS)
# define QT_NO_NIS
#endif

#if defined(QT_NO_OPENGL) && defined(QT_OPENGL)
# undef QT_NO_OPENGL
#elif !defined(QT_NO_OPENGL)
# define QT_NO_OPENGL
#endif

#if defined(QT_NO_OPENSSL) && defined(QT_OPENSSL)
# undef QT_NO_OPENSSL
#elif !defined(QT_NO_OPENSSL)
# define QT_NO_OPENSSL
#endif

#if defined(QT_NO_OPENVG) && defined(QT_OPENVG)
# undef QT_NO_OPENVG
#elif !defined(QT_NO_OPENVG)
# define QT_NO_OPENVG
#endif

#if defined(QT_NO_SSL) && defined(QT_SSL)
# undef QT_NO_SSL
#elif !defined(QT_NO_SSL)
# define QT_NO_SSL
#endif

#if defined(QT_NO_STYLE_FUSION) && defined(QT_STYLE_FUSION)
# undef QT_NO_STYLE_FUSION
#elif !defined(QT_NO_STYLE_FUSION)
# define QT_NO_STYLE_FUSION
#endif

#if defined(QT_NO_STYLE_GTK) && defined(QT_STYLE_GTK)
# undef QT_NO_STYLE_GTK
#elif !defined(QT_NO_STYLE_GTK)
# define QT_NO_STYLE_GTK
#endif

#if defined(QT_NO_STYLE_S60) && defined(QT_STYLE_S60)
# undef QT_NO_STYLE_S60
#elif !defined(QT_NO_STYLE_S60)
# define QT_NO_STYLE_S60
#endif

#if defined(QT_NO_STYLE_WINDOWS) && defined(QT_STYLE_WINDOWS)
# undef QT_NO_STYLE_WINDOWS
#elif !defined(QT_NO_STYLE_WINDOWS)
# define QT_NO_STYLE_WINDOWS
#endif

#if defined(QT_NO_STYLE_WINDOWSCE) && defined(QT_STYLE_WINDOWSCE)
# undef QT_NO_STYLE_WINDOWSCE
#elif !defined(QT_NO_STYLE_WINDOWSCE)
# define QT_NO_STYLE_WINDOWSCE
#endif

#if defined(QT_NO_STYLE_WINDOWSMOBILE) && defined(QT_STYLE_WINDOWSMOBILE)
# undef QT_NO_STYLE_WINDOWSMOBILE
#elif !defined(QT_NO_STYLE_WINDOWSMOBILE)
# define QT_NO_STYLE_WINDOWSMOBILE
#endif

#if defined(QT_NO_STYLE_WINDOWSVISTA) && defined(QT_STYLE_WINDOWSVISTA)
# undef QT_NO_STYLE_WINDOWSVISTA
#elif !defined(QT_NO_STYLE_WINDOWSVISTA)
# define QT_NO_STYLE_WINDOWSVISTA
#endif

#if defined(QT_NO_STYLE_WINDOWSXP) && defined(QT_STYLE_WINDOWSXP)
# undef QT_NO_STYLE_WINDOWSXP
#elif !defined(QT_NO_STYLE_WINDOWSXP)
# define QT_NO_STYLE_WINDOWSXP
#endif

#if defined(QT_NO_WIDGETS) && defined(QT_WIDGETS)
# undef QT_NO_WIDGETS
#elif !defined(QT_NO_WIDGETS)
# define QT_NO_WIDGETS
#endif

#if defined(QT_QML_NO_DEBUGGER) && defined(QT_NO_QML_NO_DEBUGGER)
# undef QT_QML_NO_DEBUGGER
#elif !defined(QT_QML_NO_DEBUGGER)
# define QT_QML_NO_DEBUGGER
#endif

#define QT_QPA_DEFAULT_PLATFORM_NAME "windows"
