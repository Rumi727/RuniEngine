#nullable enable
using RuniEngine.UI.Animating;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimationTransform))]
    public class UIAnimationTransformEditor : UIAnimationEditor<UIAnimationTransform>
    {
        AnimBool? positionAnim;
        AnimBool? positionXAnim;
        AnimBool? positionYAnim;
        AnimBool? positionZAnim;

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

            FadeGroup(ref positionAnim, target.mode.HasFlag(UIAnimationTransformMode.positionX) || target.mode.HasFlag(UIAnimationTransformMode.positionY) || target.mode.HasFlag(UIAnimationTransformMode.positionZ), () => Space());

            AutoField(ref positionXAnim, target.mode.HasFlag(UIAnimationTransformMode.positionX) || modeMixed, "_positionX", TryGetText("transform.position.x"));
            AutoField(ref positionYAnim, target.mode.HasFlag(UIAnimationTransformMode.positionY) || modeMixed, "_positionY", TryGetText("transform.position.y"));
            AutoField(ref positionZAnim, target.mode.HasFlag(UIAnimationTransformMode.positionZ) || modeMixed, "_positionZ", TryGetText("transform.position.z"));

            FadeGroup(ref rotationAnim, target.mode.HasFlag(UIAnimationTransformMode.rotationX) || target.mode.HasFlag(UIAnimationTransformMode.rotationY) || target.mode.HasFlag(UIAnimationTransformMode.rotationZ), () => Space());

            AutoField(ref rotationXAnim, target.mode.HasFlag(UIAnimationTransformMode.rotationX) || modeMixed, "_rotationX", TryGetText("transform.rotation.x"));
            AutoField(ref rotationYAnim, target.mode.HasFlag(UIAnimationTransformMode.rotationY) || modeMixed, "_rotationY", TryGetText("transform.rotation.y"));
            AutoField(ref rotationZAnim, target.mode.HasFlag(UIAnimationTransformMode.rotationZ) || modeMixed, "_rotationZ", TryGetText("transform.rotation.z"));

            FadeGroup(ref scaleAnim, target.mode.HasFlag(UIAnimationTransformMode.scaleX) || target.mode.HasFlag(UIAnimationTransformMode.scaleY) || target.mode.HasFlag(UIAnimationTransformMode.scaleZ), () => Space());

            AutoField(ref scaleXAnim, target.mode.HasFlag(UIAnimationTransformMode.scaleX) || modeMixed, "_scaleX", TryGetText("transform.scale.x"));
            AutoField(ref scaleYAnim, target.mode.HasFlag(UIAnimationTransformMode.scaleY) || modeMixed, "_scaleY", TryGetText("transform.scale.y"));
            AutoField(ref scaleZAnim, target.mode.HasFlag(UIAnimationTransformMode.scaleZ) || modeMixed, "_scaleZ", TryGetText("transform.scale.z"));
        }
    }
}
