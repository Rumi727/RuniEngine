#nullable enable
using RuniEngine.Rendering;
using RuniEngine.Resource.Texts;
using RuniEngine.Threading;
using TMPro;
using UnityEngine;

namespace RuniEngine
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TextMeshProSetter : MonoBehaviour, IRenderer
    {
        public TMP_Text? text => _text = this.GetComponentFieldSave(_text);
        TMP_Text? _text;



        public string nameSpace { get => _nameSpace; set => _nameSpace = value; }
        [SerializeField] string _nameSpace = "";

        public string key { get => _path; set => _path = value; }

        string IRenderer.path { get => _path; set => _path = value; }
        [SerializeField] string _path = "";



        public ReplaceOldNewPair[] replaces { get => _replaces; set => _replaces = value; }
        [SerializeField] ReplaceOldNewPair[] _replaces = new ReplaceOldNewPair[0];

        public NameSpacePathPair pair
        {
            get => new NameSpacePathPair(nameSpace, key);
            set
            {
                nameSpace = value.nameSpace;
                key = value.path;
            }
        }

        public void Refresh()
        {
            if (text == null)
                return;

            if (ThreadManager.isMainThread)
                text.text = LanguageLoader.GetText(key, nameSpace);
            else
                ThreadDispatcher.Execute(() => text.text = LanguageLoader.GetText(key, nameSpace));
        }
    }
}
