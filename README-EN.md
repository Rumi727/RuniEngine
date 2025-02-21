# Runi Engine

Languages available in README: \[[**한국어 (대한민국)**](README.md)\] \[[English (US)](README-EN.md)\]

**This project is under development**\
For now, please use [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0/)

## Introduction

[Nullable reference types]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types
[Visual Studio]: https://visualstudio.microsoft.com/

A universal kernel system designed for Unity, but **anyone** can use it.\
The UI is inspired by [osu!lazer](https://github.com/ppy/osu).

This project uses [nullable reference types].\
Therefore, it is highly recommended to install this [package](https://github.com/Rumi727/Unity-Nullable-Enabler) that enables [nullable reference types] in [Visual Studio].

### Why is it called a universal "kernel" system?

[Kernel]: Packages/com.teamdodoco.runiengine/Runtime/Kernel.cs
[BootLoader]: Packages/com.teamdodoco.runiengine/Runtime/Booting/BootLoader.cs

Well...\
Maybe because the main class names, like [Kernel] and [BootLoader], resemble an operating system?\
It also has an account system (though it is stored locally since there is no server).\
Hmm... and while there are no plans to develop a related game yet, ~~it fits my OC settings perfectly, so why not?~~\
Anyway, who cares? ~~If I call it a kernel, then it's a kernel. Stop overthinking!~~

## License

[MPL-2.0](https://opensource.org/license/mpl-2-0)

## Packages, DLLs, and Open-Source References Used

- [UniTask](https://github.com/Cysharp/UniTask)
- [Asynchronous Image Loader (Forked by Rumi)](https://github.com/Rumi727/UnityAsyncImageLoader)
  - Install using the link: ``https://github.com/Rumi727/UnityAsyncImageLoader.git``
  - [Original Repository](https://github.com/Looooong/UnityAsyncImageLoader)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)

### Built-in

- [Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons)
- [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [VorbisPlugin](https://github.com/gindemit/unity-wrapper-vorbis)
  - Added the `ToAudioClipAsync` method to [VorbisPlugin.cs](Packages/com.teamdodoco.runiengine.sounds/Packages/VorbisPlugin/Impl/src/VorbisPlugin.cs) for asynchronous audio loading.
  - Added a UniTask package dependency to the [assembly](Packages/com.teamdodoco.runiengine.sounds/Packages/VorbisPlugin/Impl/VorbisPluginImpl.asmdef).
- [NVorbis](https://github.com/NVorbis/NVorbis)
- [NAudio](https://github.com/naudio/NAudio)
- [NAudio.Vorbis](https://github.com/naudio/Vorbis)

### Others

There seem to be minor audio-related issues in versions below 2022.

Runi Engine Installer and OOBE music source: Windows XP OOBE.\
Remove if necessary.

I might have forgotten to include some sources due to my carelessness...\
If you notice any missing credits, please report it in an issue.

## Icon Sources Used

- Created by me or sourced from [here](https://github.com/microsoft/fluentui-system-icons).

## Versioning Rules

- [Semantic Versioning](https://semver.org/)

## Installation Guide

Unity version must be **2022.3.0f1 or higher**.\
If you encounter errors in a specific version, please let me know.

1. Change the input processing method in **Project Settings -> Player -> Other Settings -> Active Input Handling** to **Both**.
    - You can also search for **Active Input Handling** in the settings.
2. Add a scoped registry to the project:
  2.1 **Project Settings -> Package Manager -> Scoped Registries**
    - Name: ``package.openupm.com``
    - URL: ``https://package.openupm.com``
    - Scopes:
      - ``com.coffee.softmask-for-ugui``
      - ``com.cysharp.unitask``
      - ``com.unity.uiextensions``
3. Recommended package before installation:
    - [Unity Nullable Enabler](https://github.com/Rumi727/Unity-Nullable-Enabler)
4. Install packages via Git URL in the **Package Manager** (* indicates required packages that must be installed in order):
    - \* **Runi Engine Internal API Bridge**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.internal-api-bridge`
      - Provides easy access to some internal Unity APIs.
    - \* **Runi Engine**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine`
      - Designed **exclusively** for Runi Engine.
    - **Runi Engine Object Pooling**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.pooling`
      - Simplifies prefab loading with object pooling and enables loading Unity objects from resource packs.
    - **Runi Engine Sounds**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.sounds`
      - Provides an API for easily loading local audio files and supports resource pack audio loading.
      - Includes a dedicated audio player.
      - Requires **Runi Engine Object Pooling** to be installed first!
    - **Runi Engine NBS**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.nbs`
      - Allows loading music created with Minecraft Note Block Studio and supports resource pack NBS files.
      - Includes a dedicated player.
      - Requires **Runi Engine Sounds** to be installed first!
    - **Runi Engine Images**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.images`
      - Provides an API for loading local image files and supports resource pack images.
      - Includes a dedicated renderer.
      - **Before installation, also install the following Git URL package:**
        - **Asynchronous Image Loader (Forked by TEAM Dodoco)**: `https://github.com/Rumi727/UnityAsyncImageLoader.git`
    - **Runi Engine Inputs**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.inputs`
      - Not yet fully developed.
      - Simplifies input processing.
    - **Runi Engine UI**: `https://github.com/Rumi727/RuniEngine.git?path=Packages/com.teamdodoco.runiengine.ui`
      - Not yet fully developed.
      - Includes convenient APIs and components for UGUI.
      - Handles all UI for Runi Engine.
        - It may be heavy when completed, so **do not install if you don't need the default Runi Engine UI**.
        - Also... my UI coding skills aren't great, so it might end up being quite heavy...
5. Complete the final installation via the **Installer**.\
  5.1 You can run the installer anytime via **Menu Bar -> Runi Engine -> Show Installer**.
6. **Restart the editor** after the initial installation.
7. Done!