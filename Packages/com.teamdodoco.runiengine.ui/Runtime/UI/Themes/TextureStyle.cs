#nullable enable
using RuniEngine.Jsons;

namespace RuniEngine.UI.Themes
{
    public struct TextureStyle
    {
        public TextureStyle(NameSpaceIndexTypeNamePair pair, JColor color)
        {
            this.pair = pair;
            this.color = color;
        }

        public NameSpaceIndexTypeNamePair pair;
        public JColor color;
    }
}
