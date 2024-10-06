using RuniEngine.Jsons;
using RuniEngine.Resource.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using static RuniEngine.Editor.EditorTool;

namespace RuniEngine.Editor.ProjectSettings
{
    public sealed class NBSProjectSetting : SoundProjectSetting
    {
        NBSProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider? instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => instance ??= new NBSProjectSetting("Runi Engine/Resource/NBS Setting", SettingsScope.Project);

        public override string folderPath => NBSLoader.name;

        public override string folderCreateButtonText => TryGetText("project_setting.nbs.nbses_folder_create");
        public override string jsonFileCreateButtonText => TryGetText("project_setting.nbs.nbses_file_create");

        public override Type targetDataType => typeof(NBSData);

        protected override SoundDataGUI CreateSoundDataGUI() => new NBSDataGUI();

        protected override IDictionary<string, SoundData>? GetSoundDatas(string path) => JsonManager.JsonRead<Dictionary<string, NBSData>>(path).ToDictionary(x => x.Key, x => (SoundData)x.Value);

        protected override SoundData CreateEmptySoundData() => new NBSData(string.Empty, true);
    }
}
