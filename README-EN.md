# Runi Engine

Language available: \[[한국어 (대한민국)](README.md)\] \[[**English (US)**](README-EN.md)\]  

**This is a project in development**  
For now, please use [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)

Unity-only universal kernel system that **everyone** will use  
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

I may have been stupid and left out some sources...
If so, please post it in the issue.

## Source of icons used

- I made it myself, or got it from [here](https://github.com/microsoft/fluentui-system-icons)

## Version Notation Conventions

- [Semantic Versioning](https://semver.org/)
