#nullable enable
using RuniEngine.Editor.Inspector.UI.Animating;
using RuniEngine.UI.Animating;
using UnityEditor;

namespace RuniEngine.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimationRectTransform))]
    public class UIAnimationRectTransformEditor : UIAnimationEditor<UIAnimationRectTransform>
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();
            Space();

            bool modeMixed = !TargetsIsEquals(x => x.mode);
            UseProperty("_mode", TryGetText("gui.mode"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY) || target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX) || modeMixed, "_anchoredPositionX", TryGetText("rect_transform.anchored_position.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY) || modeMixed, "_anchoredPositionY", TryGetText("rect_transform.anchored_position.y"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ) || modeMixed, "_anchoredPositionZ", TryGetText("rect_transform.anchored_position.z"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX) || target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX) || modeMixed, "_sizeDeltaX", TryGetText("rect_transform.size_delta.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY) || modeMixed, "_sizeDeltaY", TryGetText("rect_transform.size_delta.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinX) || target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinX) || modeMixed, "_offsetMinX", TryGetText("rect_transform.offset_min.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.offsetMinY) || modeMixed, "_offsetMinY", TryGetText("rect_transform.offset_min.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX) || target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX) || modeMixed, "_offsetMaxX", TryGetText("rect_transform.offset_max.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY) || modeMixed, "_offsetMaxY", TryGetText("rect_transform.offset_max.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinX) || modeMixed, "_anchorMinX", TryGetText("rect_transform.anchor_min.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchorMinY) || modeMixed, "_anchorMinY", TryGetText("rect_transform.anchor_min.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX) || target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX) || modeMixed, "_anchorMaxX", TryGetText("rect_transform.anchor_max.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY) || modeMixed, "_anchorMaxY", TryGetText("rect_transform.anchor_max.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.pivotX) || target.mode.HasFlag(UIAnimationRectTransformMode.pivotY))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.pivotX) || modeMixed, "_pivotX", TryGetText("rect_transform.pivot.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.pivotY) || modeMixed, "_pivotY", TryGetText("rect_transform.pivot.y"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.rotationX) || target.mode.HasFlag(UIAnimationRectTransformMode.rotationY) || target.mode.HasFlag(UIAnimationRectTransformMode.rotationZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.rotationX) || modeMixed, "_rotationX", TryGetText("transform.rotation.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.rotationY) || modeMixed, "_rotationY", TryGetText("transform.rotation.y"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.rotationZ) || modeMixed, "_rotationZ", TryGetText("transform.rotation.z"));

            if (target.mode.HasFlag(UIAnimationRectTransformMode.scaleX) || target.mode.HasFlag(UIAnimationRectTransformMode.scaleY) || target.mode.HasFlag(UIAnimationRectTransformMode.scaleZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.scaleX) || modeMixed, "_scaleX", TryGetText("transform.scale.x"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.scaleY) || modeMixed, "_scaleY", TryGetText("transform.scale.y"));
            AutoField(target.mode.HasFlag(UIAnimationRectTransformMode.scaleZ) || modeMixed, "_scaleZ", TryGetText("transform.scale.z"));
        }
    }
}
