#nullable enable
using System;
using System.Reflection;
using UnityEditor;

namespace RuniEngine.Editor
{
    public partial class EditorTool
    {
        public static Assembly editorAssembly { get; } = typeof(UnityEditor.Editor).Assembly;

        static Type? _gameViewType;
        public static Type gameViewType
        {
            get
            {
                if (_gameViewType == null)
                    _gameViewType = editorAssembly.GetType("UnityEditor.GameView");

                return _gameViewType;
            }
        }

        static EditorWindow? _gameView;
        public static EditorWindow gameView
        {
            get
            {
                if (_gameView == null)
                    _gameView = EditorWindow.GetWindow(gameViewType, false, null);

                return _gameView;
            }
        }
    }
}
