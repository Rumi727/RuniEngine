using RuniEngine.UI.Animating;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimationRectTransform))]
    public class UIAnimationRectTransformEditor : UIAnimationEditor<UIAnimationRectTransform>
    {
        AnimBool? anchoredPositionAnim;
        AnimBool? anchoredPositionXAnim;
        AnimBool? anchoredPositionYAnim;
        AnimBool? anchoredPositionZAnim;

        AnimBool? sizeDeltaAnim;
        AnimBool? sizeDeltaXAnim;
        AnimBool? sizeDeltaYAnim;

        AnimBool? offsetMinAnim;
        AnimBool? offsetMinXAnim;
        AnimBool? offsetMinYAnim;

        AnimBool? offsetMaxAnim;
        AnimBool? offsetMaxXAnim;
        AnimBool? offsetMaxYAnim;

        AnimBool? anchorMinAnim;
        AnimBool? anchorMinXAnim;
        AnimBool? anchorMinYAnim;

        AnimBool? anchorMaxAnim;
        AnimBool? anchorMaxXAnim;
        AnimBool? anchorMaxYAnim;

        AnimBool? pivotAnim;
        AnimBool? pivotXAnim;
        AnimBool? pivotYAnim;

        AnimBool? rotationAnim;
        AnimBool? rotationXAnim;
        AnimBool? rotationYAnim;
        AnimBool? rotationZAnim;

        AnimBool? scaleAnim;
        AnimBool? scaleXAnim;
        AnimBool? scaleYAnim;
        AnimBool? scaleZAnim;

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();
            Space();

            bool modeMixed = !TargetsIsEquals(x => x.mode);
            UseProperty("_mode", TryGetText("gui.mode"));

            FadeGroup(ref anchoredPositionAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY) || target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ), () => Space());


            AutoField(ref anchoredPositionXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX) || modeMixed, "_anchoredPositionX", TryGetText("rect_transform.anchored_position.x"));
            AutoField(ref anchoredPositionYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY) || modeMixed, "_anchoredPositionY", TryGetText("rect_transform.anchored_position.y"));
            AutoField(ref anchoredPositionZAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ) || modeMixed, "_anchoredPositionZ", TryGetText("rect_transform.anchored_position.z"));

            FadeGroup(ref sizeDeltaAnim, target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX) || target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY), () => Space());

            AutoField(ref sizeDeltaXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX) || modeMixed, "_sizeDeltaX", TryGetText("rect_transform.size_delta.x"));
            AutoField(ref sizeDeltaYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY) || modeMixed, "_sizeDeltaY", TryGetText("rect_transform.size_delta.y"));

            FadeGroup(ref offsetMinAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinX) || target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinY), () => Space());

            AutoField(ref offsetMinXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinX) || modeMixed, "_offsetMinX", TryGetText("rect_transform.offset_min.x"));
            AutoField(ref offsetMinYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinY) || modeMixed, "_offsetMinY", TryGetText("rect_transform.offset_min.y"));

            FadeGroup(ref offsetMaxAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX) || target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY), () => Space());

            AutoField(ref offsetMaxXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX) || modeMixed, "_offsetMaxX", TryGetText("rect_transform.offset_max.x"));
            AutoField(ref offsetMaxYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY) || modeMixed, "_offsetMaxY", TryGetText("rect_transform.offset_max.y"));

            FadeGroup(ref anchorMinAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinY), () => Space());

            AutoField(ref anchorMinXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinX) || modeMixed, "_anchorMinX", TryGetText("rect_transform.anchor_min.x"));
            AutoField(ref anchorMinYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinY) || modeMixed, "_anchorMinY", TryGetText("rect_transform.anchor_min.y"));

            FadeGroup(ref anchorMaxAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY), () => Space());

            AutoField(ref anchorMaxXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX) || modeMixed, "_anchorMaxX", TryGetText("rect_transform.anchor_max.x"));
            AutoField(ref anchorMaxYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY) || modeMixed, "_anchorMaxY", TryGetText("rect_transform.anchor_max.y"));

            FadeGroup(ref pivotAnim, target.mode.HasFlag(UIAnimationRectTransformMode.pivotX) || target.mode.HasFlag(UIAnimationRectTransformMode.pivotY), () => Space());

            AutoField(ref pivotXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.pivotX) || modeMixed, "_pivotX", TryGetText("rect_transform.pivot.x"));
            AutoField(ref pivotYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.pivotY) || modeMixed, "_pivotY", TryGetText("rect_transform.pivot.y"));

            FadeGroup(ref rotationAnim, target.mode.HasFlag(UIAnimationRectTransformMode.rotationX) || target.mode.HasFlag(UIAnimationRectTransformMode.rotationY) || target.mode.HasFlag(UIAnimationRectTransformMode.rotationZ), () => Space());

            AutoField(ref rotationXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.rotationX) || modeMixed, "_rotationX", TryGetText("transform.rotation.x"));
            AutoField(ref rotationYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.rotationY) || modeMixed, "_rotationY", TryGetText("transform.rotation.y"));
            AutoField(ref rotationZAnim, target.mode.HasFlag(UIAnimationRectTransformMode.rotationZ) || modeMixed, "_rotationZ", TryGetText("transform.rotation.z"));

            FadeGroup(ref scaleAnim, target.mode.HasFlag(UIAnimationRectTransformMode.scaleX) || target.mode.HasFlag(UIAnimationRectTransformMode.scaleY) || target.mode.HasFlag(UIAnimationRectTransformMode.scaleZ), () => Space());

            AutoField(ref scaleXAnim, target.mode.HasFlag(UIAnimationRectTransformMode.scaleX) || modeMixed, "_scaleX", TryGetText("transform.scale.x"));
            AutoField(ref scaleYAnim, target.mode.HasFlag(UIAnimationRectTransformMode.scaleY) || modeMixed, "_scaleY", TryGetText("transform.scale.y"));
            AutoField(ref scaleZAnim, target.mode.HasFlag(UIAnimationRectTransformMode.scaleZ) || modeMixed, "_scaleZ", TryGetText("transform.scale.z"));
        }
    }
}
