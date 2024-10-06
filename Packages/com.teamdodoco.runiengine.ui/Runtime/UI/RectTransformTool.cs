using UnityEngine;

namespace RuniEngine.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class RectTransformTool : UIBase
    {
        public static implicit operator RectTransform(RectTransformTool value) => value.rectTransform;
        public static explicit operator RectTransformTool(RectTransform value) => value.GetComponent<RectTransformTool>();
    }
}
