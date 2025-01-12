#nullable enable
using RuniEngine.Jsons;
using TMPro;

namespace RuniEngine.UI.Themes
{
    public class ThemeStyle
    {
        public RectTransformStyle rectTransform = new RectTransformStyle(RectOffset.zero, new JVector2(0.5f, 0.5f), new JVector2(0.5f, 0.5f), new JVector2(0.5f, 0.5f));

        public TextureStyle texture = new TextureStyle("", JColor.one);
        public TextStyle text = new TextStyle(16, TextAlignmentOptions.TopLeft, JColor.one, RectOffset.zero);

        public LayoutGroupStyle layoutGroup = new LayoutGroupStyle(RectOffset.zero, 0, UnityEngine.TextAnchor.UpperLeft, false);

        public SelectableStyle selectable = new SelectableStyle(SelectableStyle.Transition.None);
    }
}
