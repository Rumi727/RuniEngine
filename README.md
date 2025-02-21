# Runi Engine

Languages available in README: \[[**한국어 (대한민국)**](README.md)\] \[[English (US)](README-EN.md)\]

**개발 중인 프로젝트입니다.**\
지금은 [SC KRM 1.0](https://github.com/Rumi727/SC-KRM-1.0/)을 사용해주세요.

## 소개

[nullable 참조 형식]: https://learn.microsoft.com/ko-kr/dotnet/csharp/language-reference/builtin-types/nullable-reference-types
[Visual Studio]: https://visualstudio.microsoft.com/ko/

Unity 전용 만능 커널 시스템, 하지만 **누구나** 사용할 수 있는\
UI는 [osu!lazer](https://github.com/ppy/osu)에서 영감을 받았습니다.

이 프로젝트는 [nullable 참조 형식]을 사용합니다.\
따라서 [Visual Studio]에서 이를 활성화할 수 있도록 하는 [패키지](https://github.com/Rumi727/Unity-Nullable-Enabler) 설치를 강력 추천합니다.

### 왜 만능 "커널" 시스템인가요?

[Kernel]: Packages/com.teamdodoco.runiengine/Runtime/Kernel.cs
[BootLoader]: Packages/com.teamdodoco.runiengine/Runtime/Booting/BootLoader.cs

글쎄요?\
[Kernel], [BootLoader] 같은 운영체제스러운 이름 때문일 수도 있고요.\
실제로 계정 시스템도 있어요. (서버가 없어서 로컬 저장이긴 하지만요)\
음... 그리고 아직 관련 게임을 만들 계획은 없지만 ~~자캐 설정에도 딱 맞기도 하고 하니 그냥?~~

뭐 아무렴 어때요. ~~내가 커널이라면 커널이라는 거지. 말이 많아 어쩔티비 저쩔티비~~

## 라이선스

[MPL-2.0](https://opensource.org/license/mpl-2-0)

### 외부 패키지
- [UniTask](https://github.com/Cysharp/UniTask)
- [Asynchronous Image Loader (Forked by Rumi)](https://github.com/Rumi727/UnityAsyncImageLoader) ([원본](https://github.com/Looooong/UnityAsyncImageLoader))
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)

### 내장된 라이브러리
- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-wrapper-vorbis) (비동기 오디오 로딩 기능 추가됨)
- [NVorbis](https://github.com/NVorbis/NVorbis)
- [NAudio](https://github.com/naudio/NAudio)
- [NAudio.Vorbis](https://github.com/naudio/Vorbis)

### 기타
- 2022 이하 버전에서 마이너한 오디오 관련 문제가 있을 수 있습니다.
- Runi Engine Installer, OOBE 음악 출처: Windows XP OOBE (문제시 제거하세요).
- 혹시 빠진 출처가 있다면 이슈로 제보해주세요.

## 아이콘 출처
- 직접 제작했거나, [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)에서 가져왔습니다.

## 버전 규칙
- [Semantic Versioning](https://semver.org/)을 따릅니다.

## 설치 방법

**유니티 버전:** 2022.3.0f1 이상 (특정 버전에서 오류 발생 시 제보해주세요.)

1. **입력 시스템 변경**
   - 프로젝트 설정 -> Player -> 기타 설정 -> "사용 중인 입력 처리"를 **"모두"**로 변경
   - "사용 중인 입력 처리"를 검색하여 바로 변경 가능

2. **Scoped Registry 추가**
   - 프로젝트 설정 -> 패키지 관리자 -> Scoped Registries
   - 이름: `package.openupm.com`
   - URL: `https://package.openupm.com`
   - Scopes:
     - `com.coffee.softmask-for-ugui`
     - `com.cysharp.unitask`
     - `com.unity.uiextensions`

3. **추천 패키지 설치**
   - [Unity Nullable Enabler](https://github.com/Rumi727/Unity-Nullable-Enabler)

4. **필수 패키지 및 추가 패키지 설치** (순서대로 설치 필요)
   - Runi Engine Internal API Bridge (필수): `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.internal-api-bridge`
   - Runi Engine (필수): `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine`
   - Runi Engine Object Pooling: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.pooling`
   - Runi Engine Sounds: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.sounds` (Object Pooling 먼저 설치!)
   - Runi Engine NBS: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.nbs` (Sounds 먼저 설치!)
   - Runi Engine Images: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.images`
     - **추가로 설치 필요:** `Asynchronous Image Loader (Forked by TEAM Dodoco)` → `https://github.com/Rumi727/UnityAsyncImageLoader.git`
   - Runi Engine Inputs: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.inputs` (미완성)
   - Runi Engine UI: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.ui` (미완성, 무거울 수 있음)

5. **최종 설치 (Installer 사용)**
   - 메뉴 모음 -> Runi Engine -> **Show Installer**에서 실행 가능

6. **최초 설치 후 에디터 재시작 권장**