# Runi Engine

Language available: \[[한국어 (대한민국)](README.md)\] \[[**English (US)**](README-EN.md)\]  

**This is a project in development**\
For now, please use [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)

Unity-only universal kernel system that **everyone** will use\
UI inspired by [osu!lazer](https://github.com/ppy/osu)

## License

[MIT License](https://opensource.org/licenses/MIT)

## Packages and DLLs used, open source sources

### Required

- [UniTask](https://github.com/Cysharp/UniTask)
- [Asynchronous Image Loader (Forked by TEAM Dodoco)](https://github.com/Rumi727/UnityAsyncImageLoader)
  - ``https://github.com/Rumi727/UnityAsyncImageLoader.git`` 링크를 사용해서 설치하세요
  - [Original](https://github.com/Looooong/UnityAsyncImageLoader)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)

### Built-in

- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-project-vorbis)
  - Added ToAudioClipAsync method to [VorbisPlugin.cs](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/src/VorbisPlugin.cs) file to load audio asynchronously and added UniTask package dependency to [assembly](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/VorbisPluginImpl.asmdef).

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
    - \* Runi Engine Internal API Bridge : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.internal-api-bridge`
    - \* Runi Engine :  `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine`
    - Runi Engine Object Pooling : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.pooling`
    - Runi Engine Sounds : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.sounds`
      - You need to install the `Runi Engine Object Pooling` package first!
    - Runi Engine NBS : You need to install the `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.nbs`
      - `Runi Engine Sounds` package first!
    - Runi Engine Images : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.images`
      - Before installation, please also install the following git URL package!
        - Asynchronous Image Loader (Forked by TEAM Dodoco) : `https://github.com/Rumi727/UnityAsyncImageLoader.git`
    - Runi Engine Inputs : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.inputs`
    - Runi Engine UI : `https://github.com/Rumi727/RuniEngine?path=Packages/com.teamdodoco.runiengine.ui`
4. Complete the final installation in the installer\
  4.1 You can run the installer at any time by going to ``Menu Bar -> Runi Engine -> Show Installer``
5. It is recommended to restart the editor when installing for the first time.
6. End!
