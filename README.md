# Runi Engine

Language available in README: \[[**한국어 (대한민국)**](README.md)\] \[[English (US)](README-EN.md)\]

**개발 중인 프로젝트 입니다**\
지금은 [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)을 사용해주세요

## 소개

Unity 전용 만능 커널 시스템, 하지만 **누구나** 사용할 수 있는\
UI는 [osu!lazer](https://github.com/ppy/osu)에서 영감을 받았습니다

### 왜 만능 "커널" 시스템인가요?

[Kernel]: Packages/com.teamdodoco.runiengine/Runtime/Kernel.cs
[BootLoader]: Packages/com.teamdodoco.runiengine/Runtime/Booting/BootLoader.cs

글쎄요?\
[Kernel], [BootLoader] 같은 운영체제스럽게 메인 클래스 이름이 짜여져서 그런것 같기도 하고요?\
실제로 (서버가 없으니 로컬에 저장되긴 하지만) 계정 시스템도 있기도 하구요\
음... 그리고 아직 관련 게임을 만들 계획은 없지만 ~~자캐 설정에도 딱 맞기도 하고 하니 그냥?~~\
뭐 아무렴 어때요 ~~내가 커널이라면 커널이라는거지 말이 많아 어쩔티비 저쩔티비~~

## 라이선스

[MIT License](https://opensource.org/licenses/MIT)

## 사용한 패키지와 DLL, 오픈 소스 출처

- [UniTask](https://github.com/Cysharp/UniTask)
- [Asynchronous Image Loader (Forked by TEAM Dodoco)](https://github.com/Rumi727/UnityAsyncImageLoader)
  - ``https://github.com/Rumi727/UnityAsyncImageLoader.git`` 링크를 사용해서 설치하세요
  - [원본](https://github.com/Looooong/UnityAsyncImageLoader)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)

### 내장됨

- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-project-vorbis)
  - 오디오를 비동기적으로 로드하기 위해 [VorbisPlugin.cs](Packages/com.teamdodoco.runiengine.sounds/Packages/VorbisPlugin/Impl/src/VorbisPlugin.cs) 파일에 ToAudioClipAsync 메소드를 추가했으며 [어셈블리](Packages/com.teamdodoco.runiengine.sounds/Packages/VorbisPlugin/Impl/VorbisPluginImpl.asmdef)에 UniTask 패키지 의존성을 추가했습니다

제가 멍청해서 빼먹은 출처가 있을 수 있습니다...\
만약 그럴 경우, 이슈에 올려주세요

## 사용한 아이콘 출처

- 제가 직접 만들었거나, [여기](https://github.com/microsoft/fluentui-system-icons)에서 가져왔습니다

## 버전 표기 규칙

- [Semantic Versioning](https://semver.org/)

## 설치 방법

유니티 버전은 2022.3.0f1 이상이여야합니다\
만약 특정 버전에서 오류가 발생한다면 얘기해주세요

1. ``프로젝트 설정 -> Player -> 기타 설정 -> 설정 -> 사용 중인 입력 처리``를 ``모두``로 바꿔주세요
    - ``사용 중인 입력 처리``를 검색하는 것도 가능합니다
2. 프로젝트에 범위가 지정된 레지스트리를 추가합니다.\
  2.1 프로젝트 설정 -> 패키지 관리자 -> Scoped Registries
    - 이름 : ``package.openupm.com``
    - URL : ``https://package.openupm.com``
    - Scopes
      - ``com.coffee.softmask-for-ugui``
      - ``com.cysharp.unitask``
      - ``com.unity.uiextensions``
3. 패키지 관리자에서 git URL로 설치합니다 (* 표시된건 필수 패키지이며 꼭 순서대로 설치해야합니다)
    - \* Runi Engine Internal API Bridge : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.internal-api-bridge`
      - 유니티의 몇몇 내부 API를 손쉽게 접근할 수 있게 하는 패키지입니다
    - \* Runi Engine :  `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine`
      - 오로지 Runi Enigne을 위해, Runi Engine을 위한
    - Runi Engine Object Pooling : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.pooling`
      - 프리팹 로드를 오브젝트 풀링을 통해 쉽게 할 수 있게 만들어주며, 리소스팩에서 게임 오브젝트를 포함한 모든 유니티 오브젝트를 불러올 수 있게 해주는 API 입니다
    - Runi Engine Sounds : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.sounds`
      - 로컬 오디오 파일을 쉽게 로드할 수 있게 해주는 API가 있으며, 리소스팩에서 오디오를 불러올 수 있게 해줍니다
      - 전용 플레이어가 존재합니다
      - `Runi Engine Object Pooling` 패키지를 먼저 설치해야합니다!
    - Runi Engine NBS : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.nbs`
      - Minecraft Note Block Studio 프로그램에서 만들어진 음악을 쉽게 로드할 수 있게 해주는 API가 있으며, 리소스팩에서 NBS 파일을 불러올 수 있게 해줍니다
      - 전용 플레이어가 존재합니다
      - `Runi Engine Sounds` 패키지를 먼저 설치해야합니다!
    - Runi Engine Images : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.images`
      - 로컬 이미지 파일을 쉽게 로드할 수 있게 해주는 API가 있으며, 리소스팩에서 이미지를 불러올 수 있게 해줍니다
      - 전용 렌더러가 존재합니다
      - 설치 전에 다음 git URL 패키지도 같이 설치해주세요!
        - Asynchronous Image Loader (Forked by TEAM Dodoco) : `https://github.com/Rumi727/UnityAsyncImageLoader.git`
    - Runi Engine Inputs : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.inputs`
      - 아직 제대로 개발이 완료되지 않았습니다
      - 입력 처리를 쉽게해줍니다
    - Runi Engine UI : `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.ui`
      - 아직 제대로 개발이 완료되지 않았습니다
      - UGUI 관련한 편리한 API 및 컴포넌트가 있습니다
      - Runi Engine의 모든 UI를 담당합니다
        - 이로 인해 완성되면 꽤 무거울것이기 때문에 Runi Engine의 기본 UI가 필요하지 않다면 설치하지 않는걸 추천드립니다
        - 그리고... UI 코딩을 더럽게 못하는 편이라 완성되면 많이 무거울수도...
4. 인스톨러에서 최종 설치를 끝내세요\
  4.1 언제든지 ``메뉴 모음 -> Runi Engine -> Show Installer``로 가서 인스톨러를 실행할 수 있습니다
5. 최초 설치시에는 에디터를 재시작하는 것을 추천합니다
6. 끝!
