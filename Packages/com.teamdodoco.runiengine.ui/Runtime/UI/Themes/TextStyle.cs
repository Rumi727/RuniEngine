#nullable enable
using RuniEngine.Jsons;
using TMPro;

namespace RuniEngine.UI.Themes
{
    public struct TextStyle
    {
        public float fontSize;
        public TextAlignmentOptions alignment;

        public AutoSizing autoSizing;
        public struct AutoSizing
        {
            public bool enableAutoSizing;

            public float fontSizeMin;
            public float fontSizeMax;

            public float characterWidthAdjustment;
            public float lineSpacingAdjustment;

            public AutoSizing(bool enableAutoSizing, float fontSizeMin, float fontSizeMax, float characterWidthAdjustment, float lineSpacingAdjustment)
            {
                this.enableAutoSizing = enableAutoSizing;

                this.fontSizeMin = fontSizeMin.Clamp(0, fontSizeMax);
                this.fontSizeMax = fontSizeMax.Clamp(fontSizeMin, 32767);

                this.characterWidthAdjustment = characterWidthAdjustment.Clamp(0, 50);
                this.lineSpacingAdjustment = lineSpacingAdjustment.Clamp(float.MinValue, 0);
            }
        }

        public JColor color;

        public Spacing spacing;
        public struct Spacing
        {
            public float character;
            public float word;
            public float line;
            public float paragraph;

            public Spacing(float character, float word, float line, float paragraph)
            {
                this.character = character;
                this.word = word;
                this.line = line;
                this.paragraph = paragraph;
            }
        }

        public RectOffset padding;

        public TextStyle(float fontSize, TextAlignmentOptions alignment, JColor color, RectOffset padding)
        {
            this.fontSize = fontSize.Clamp(0, 32767);
            this.alignment = alignment;

            autoSizing = new AutoSizing(false, 0, 32767, 0, 0);

            this.color = color;

            spacing = new Spacing(0, 0, 0, 0);
            this.padding = padding;
        }

        public TextStyle(float fontSize, TextAlignmentOptions alignment, JColor color, RectOffset padding, AutoSizing autoSizing, Spacing spacing)
        {
            this.fontSize = fontSize.Clamp(0, 32767);
            this.alignment = alignment;

            this.autoSizing = autoSizing;

            this.color = color;

            this.spacing = spacing;
            this.padding = padding;
        }
    }
}