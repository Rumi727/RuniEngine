#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuniEngine.UI.Animating
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class UIAnimator : UIBase
    {
        public float time { get => _time; set => _time = value; }
        [SerializeField] float _time = 0;

        public float length => animations.Count > 0 ? animations.Max(x => x != null ? x.length : 0) : 0;

        public List<UIAnimation?> animations => _animations;
        [SerializeField] List<UIAnimation?> _animations = new List<UIAnimation?>();

        protected virtual void Update()
        {
            for (int i = 0; i < animations.Count; i++)
            {
                UIAnimation? animation = animations[i];
                if (animation == null)
                    continue;

                animation.Init(this);
                animation.LayoutUpdate();
            }
        }
    }
}
