using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI
{
    public interface IUI
    {
        public Canvas? canvas { get; }

        public RectTransform? parentRectTransform { get; }
        public RectTransform rectTransform { get; }

        public Graphic? graphic { get; }
    }
}
