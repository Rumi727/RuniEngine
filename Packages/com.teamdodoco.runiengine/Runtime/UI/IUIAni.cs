#nullable enable

namespace RuniEngine.UI
{
    public interface IUIAni : IUI
    {
        bool disableLerpAni { get; set; }
        bool useCustomLerpSpeed { get; set; }
        float lerpSpeed { get; set; }

        float currentLerpSpeed { get; }

        void SetDirty();
        void LayoutUpdate();
    }
}
