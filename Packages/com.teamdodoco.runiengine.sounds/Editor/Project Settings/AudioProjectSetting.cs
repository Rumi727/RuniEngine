using RuniEngine.Jsons;
using RuniEngine.Resource.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public sealed class AudioProjectSetting : SoundProjectSetting
    {
        AudioProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new AudioProjectSetting("Runi Engine/Resource/Audio Setting", SettingsScope.Project);

        public override string folderPath => AudioLoader.name;

        public override string folderCreateButtonText => TryGetText("project_setting.audio.audios_folder_create");
        public override string jsonFileCreateButtonText => TryGetText("project_setting.audio.audios_file_create");

        public override Type targetDataType => typeof(AudioData);

        protected override SoundDataGUI CreateSoundDataGUI() => new AudioDataGUI();

        protected override IDictionary<string, SoundData>? GetSoundDatas(string path) => JsonManager.JsonRead<Dictionary<string, AudioData>>(path).ToDictionary(x => x.Key, x => (SoundData)x.Value);

        protected override SoundData CreateEmptySoundData() => new AudioData(string.Empty, true);
    }
}
