#nullable enable
using RuniEngine.Json;

namespace RuniEngine.UI.Themes
{
    public struct SelectableStyle
    {
        public Transition transition;

        public ColorBlock colors;
        public SpriteState spriteState;

        public SelectableStyle(Transition transition)
        {
            this.transition = transition;

            colors = new ColorBlock(JColor.one, new JColor(0.875f, 0.875f, 0.875f), new JColor(0.75f, 0.75f, 0.75f), new JColor(0.875f, 0.875f, 0.875f), new JColor(1, 1, 1, 0.5f));
            spriteState = new SpriteState("", "", "", "");
        }

        public SelectableStyle(Transition transition, ColorBlock colors, SpriteState spriteState)
        {
            this.transition = transition;

            this.colors = colors;
            this.spriteState = spriteState;
        }

        public struct ColorBlock
        {
            public JColor normalColor;
            public JColor highlightedColor;
            public JColor pressedColor;
            public JColor selectedColor;
            public JColor disabledColor;

            public ColorBlock(JColor normalColor, JColor highlightedColor, JColor pressedColor, JColor selectedColor, JColor disabledColor)
            {
                this.normalColor = normalColor;
                this.highlightedColor = highlightedColor;
                this.pressedColor = pressedColor;
                this.selectedColor = selectedColor;
                this.disabledColor = disabledColor;
            }
        }

        public struct SpriteState
        {
            public NameSpaceIndexTypeNamePair highlightedSprite;
            public NameSpaceIndexTypeNamePair pressedSprite;
            public NameSpaceIndexTypeNamePair selectedSprite;
            public NameSpaceIndexTypeNamePair disabledSprite;

            public SpriteState(NameSpaceIndexTypeNamePair highlightedSprite, NameSpaceIndexTypeNamePair pressedSprite, NameSpaceIndexTypeNamePair selectedSprite, NameSpaceIndexTypeNamePair disabledSprite)
            {
                this.highlightedSprite = highlightedSprite;
                this.pressedSprite = pressedSprite;
                this.selectedSprite = selectedSprite;
                this.disabledSprite = disabledSprite;
            }
        }

        public enum Transition
        {
            None = 0,
            ColorTint = 1,
            SpriteSwap = 2
        }
    }
}
