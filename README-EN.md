# Runi Engine

Language available in README: \[[한국어 (대한민국)](README.md)\] \[[**English (US)**](README-EN.md)\]  

**This is a project in development**\
For now, please use [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)

## Introduction

[Nullable reference types]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types
[Visual Studio]: https://visualstudio.microsoft.com

Unity-only universal kernel system that **everyone** will use\
UI inspired by [osu!lazer](https://github.com/ppy/osu)

This project uses [nullable reference types]\
Therefore, I strongly recommend installing the [package](https://github.com/Rumi727/Unity-Nullable-Enabler) that enables [nullable reference types] in [Visual Studio]

### Why a universal "kernel" system?

[Kernel]: Packages/com.teamdodoco.runiengine/Runtime/Kernel.cs
[BootLoader]: Packages/com.teamdodoco.runiengine/Runtime/Booting/BootLoader.cs

well?\
I think it's because the main class name is structured like an operating system, such as [Kernel] and [BootLoader]?\
In fact, there is also an account system (although it is stored locally since there is no server)\
Hmm... And I don't have any plans to make a related game yet, but it also fits ~~my character setup~~\
Well, whatever, ~~When I say kernel, it is kernel. There is so much talk. so whaTV~~

## License

[MIT License](https://opensource.org/licenses/MIT)

## Packages and DLLs used, open source sources

- [UniTask](https://github.com/Cysharp/UniTask)
- [Asynchronous Image Loader (Forked by Rumi)](https://github.com/Rumi727/UnityAsyncImageLoader)
  - Install using the ``https://github.com/Rumi727/UnityAsyncImageLoader.git`` link
  - [Original](https://github.com/Looooong/UnityAsyncImageLoader)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)

### Built-in

- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-wrapper-vorbis)
  - Added ToAudioClipAsync method to [VorbisPlugin.cs](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/src/VorbisPlugin.cs) file to load audio asynchronously and added UniTask package dependency to [assembly](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/VorbisPluginImpl.asmdef).
- [NVorbis](https://github.com/NVorbis/NVorbis)
- [NAudio](https://github.com/naudio/NAudio)
- [NAudio.Vorbis](https://github.com/naudio/Vorbis)

### etc

 Runi Engine Installer, OOBE music source: Windows XP OOBE\
 Remove in case of problem

I may have been stupid and left out some sources...\
If so, please post it in the issue.

## Source of icons used

- I made it myself, or got it from [here](https://github.com/microsoft/fluentui-system-icons)

## Version Notation Conventions

- [Semantic Versioning](https://semver.org/)

## How to install

Unity version must be 2022.3.0f1 or higher\
If an error occurs in a specific version, please let me know.

1. Please change ``Project Settings -> Player -> Other Settings -> Configuration -> Active Input Handling`` to ``Both``
    - It is also possible to search for ``Active Input Handling``
2. Add a scoped registry to your project.\
  2.1 Project Settings -> Package Manager -> Scoped Registries
    - Name : ``package.openupm.com``
    - URL : ``https://package.openupm.com``
    - Scopes
      - ``com.coffee.softmask-for-ugui``
      - ``com.cysharp.unitask``
      - ``com.unity.uiextensions``
3. Install from the git URL in the package manager (*marked packages are required packages and must be installed in order)
    - \* Runi Engine Internal API Bridge : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.internal-api-bridge`
      - This is a package that allows easy access to some of Unity’s internal APIs.
    - \* Runi Engine :  `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine`
      - Just for Runi Enigne, for Runi Engine
    - Runi Engine Object Pooling : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.pooling`
      - This is an API that makes it easy to load prefabs through object pooling and allows you to load all Unity objects, including game objects, from resource packs.
    - Runi Engine Sounds : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.sounds`
      - There is an API that allows you to easily load local audio files, and allows you to import audio from resource packs.
      - Dedicated players exist
      - You need to install the `Runi Engine Object Pooling` package first!
    - Runi Engine NBS : You need to install the `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.nbs`
      - There is an API that allows you to easily load music created in the Minecraft Note Block Studio program, and allows you to load NBS files from resource packs.
      - Dedicated players exist
      - `Runi Engine Sounds` package first!
    - Runi Engine Images : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.images`
      - There is an API that makes it easy to load local image files and allows you to load images from resource packs.
      - A dedicated renderer exists
      - Before installation, please also install the following git URL package!
        - Asynchronous Image Loader (Forked by TEAM Dodoco) : `https://github.com/Rumi727/UnityAsyncImageLoader.git`
    - Runi Engine Inputs : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.inputs`
      - Development has not been properly completed yet
      - Makes input processing easier
    - Runi Engine UI : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.ui`
      - Development has not been properly completed yet
      - There are convenient APIs and components related to UGUI.
      - Responsible for all UI of Runi Engine
        - Because of this, it will be quite heavy once completed, so I recommend not installing Runi Engine's default UI if you don't need it.
        - And... I'm not really good at UI coding, so it might be quite heavy once it's completed...
4. Complete the final installation in the installer\
  4.1 You can run the installer at any time by going to ``Menu Bar -> Runi Engine -> Show Installer``
5. It is recommended to restart the editor when installing for the first time.
6. End!
