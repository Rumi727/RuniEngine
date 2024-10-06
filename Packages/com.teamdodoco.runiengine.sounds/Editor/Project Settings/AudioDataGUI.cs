using RuniEngine.Resource.Sounds;
using System;

namespace RuniEngine.Editor.ProjectSettings
{
    public class AudioDataGUI : SoundDataGUI
    {
        public override Type targetType => typeof(AudioData);
        public override string folderName => AudioLoader.name;

        public override string soundsPropertyName => nameof(AudioData.audios);
    }
}
