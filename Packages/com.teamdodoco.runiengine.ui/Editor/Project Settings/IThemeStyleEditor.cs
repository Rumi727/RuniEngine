#nullable enable
using RuniEngine.UI.Themes;

namespace RuniEngine.Editor.ProjectSettings
{
    public interface IThemeStyleEditor
    {
        string label { get; }

        /// <returns>GUI.changed</returns>
        bool DrawGUI(ThemeStyle style);
    }
}
