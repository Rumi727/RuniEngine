#nullable enable
using RuniEngine.Resource.Sounds;
using System;

namespace RuniEngine.Editor.ProjectSettings
{
    public sealed class NBSDataGUI : SoundDataGUI
    {
        public override Type targetType => typeof(NBSData);
        public override string folderName => NBSLoader.name;

        public override string soundsPropertyName => nameof(NBSData.nbses);
    }
}
