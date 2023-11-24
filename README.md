# Runi Engine

Language available: \[[**한국어 (대한민국)**](README.md)\] \[[English (US)](README-EN.md)\]  

**개발 중인 프로젝트 입니다**  
지금은 [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)을 사용해주세요

Unity 전용 만능 커널 시스템, 하지만 **누구나** 사용할 수 있는  
UI는 [osu!lazer](https://github.com/ppy/osu)에서 영감을 받았습니다

## 라이선스

[MIT License](https://opensource.org/licenses/MIT)

## 사용한 패키지와 DLL, 오픈 소스 출처

### 필수
- [UniTask](https://github.com/Cysharp/UniTask)
- [YoutubePlayer](https://github.com/iBicha/UnityYoutubePlayer)
- [Asynchronous Image Loader (Forked by TEAM Dodoco)](https://github.com/Rumi727/UnityAsyncImageLoader)
  - ``https://github.com/Rumi727/UnityAsyncImageLoader.git`` 링크를 사용해서 설치하세요
  - [원본](https://github.com/Looooong/UnityAsyncImageLoader)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)
### 내장됨
- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-project-vorbis)
  - 오디오를 비동기적으로 로드하기 위해 [VorbisPlugin.cs](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/src/VorbisPlugin.cs) 파일에 ToAudioClipAsync 메소드를 추가했으며 [어셈블리](Packages/com.teamdodoco.runiengine/Packages/VorbisPlugin/Impl/VorbisPluginImpl.asmdef)에 UniTask 패키지 의존성을 추가했습니다

제가 멍청해서 빼먹은 출처가 있을 수 있습니다...  
만약 그럴 경우, 이슈에 올려주세요

## 사용한 아이콘 출처

- 제가 직접 만들었거나, [여기](https://github.com/microsoft/fluentui-system-icons)에서 가져왔습니다

## 버전 표기 규칙

- [Semantic Versioning](https://semver.org/)
