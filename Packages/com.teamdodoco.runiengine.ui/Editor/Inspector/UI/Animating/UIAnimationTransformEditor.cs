#nullable enable
using RuniEngine.UI.Animating;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.UI.Animating
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimationTransform))]
    public class UIAnimationTransformEditor : UIAnimationEditor<UIAnimationTransform>
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            base.OnInspectorGUI();
            Space();

            bool modeMixed = !TargetsIsEquals(x => x.mode);
            UseProperty("_mode", TryGetText("gui.mode"));

            if (target.mode.HasFlag(UIAnimationTransformMode.positionX) || target.mode.HasFlag(UIAnimationTransformMode.positionY) || target.mode.HasFlag(UIAnimationTransformMode.positionZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationTransformMode.positionX) || modeMixed, "_positionX", TryGetText("transform.position.x"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.positionY) || modeMixed, "_positionY", TryGetText("transform.position.y"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.positionZ) || modeMixed, "_positionZ", TryGetText("transform.position.z"));

            if (target.mode.HasFlag(UIAnimationTransformMode.rotationX) || target.mode.HasFlag(UIAnimationTransformMode.rotationY) || target.mode.HasFlag(UIAnimationTransformMode.rotationZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationTransformMode.rotationX) || modeMixed, "_rotationX", TryGetText("transform.rotation.x"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.rotationY) || modeMixed, "_rotationY", TryGetText("transform.rotation.y"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.rotationZ) || modeMixed, "_rotationZ", TryGetText("transform.rotation.z"));

            if (target.mode.HasFlag(UIAnimationTransformMode.scaleX) || target.mode.HasFlag(UIAnimationTransformMode.scaleY) || target.mode.HasFlag(UIAnimationTransformMode.scaleZ))
                Space();

            AutoField(target.mode.HasFlag(UIAnimationTransformMode.scaleX) || modeMixed, "_scaleX", TryGetText("transform.scale.x"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.scaleY) || modeMixed, "_scaleY", TryGetText("transform.scale.y"));
            AutoField(target.mode.HasFlag(UIAnimationTransformMode.scaleZ) || modeMixed, "_scaleZ", TryGetText("transform.scale.z"));
        }
    }
}
