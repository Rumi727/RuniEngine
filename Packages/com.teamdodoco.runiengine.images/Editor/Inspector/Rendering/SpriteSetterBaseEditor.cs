using RuniEngine.Rendering;
using RuniEngine.Resource.Images;
using UnityEditor;

namespace RuniEngine.Editor.Inspector.Rendering
{
    public class SpriteSetterBaseEditor<TTarget> : CustomInspectorBase<TTarget> where TTarget : SpriteSetterBase
    {
        public override void OnInspectorGUI()
        {
            if (target == null || targets == null || targets.Length <= 0)
                return;

            EditorGUI.BeginChangeCheck();

            UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), target.nameSpace);
            UsePropertyAndDrawStringArray(serializedObject, "_type", TryGetText("gui.type"), target.type, ImageLoader.GetTypes(target.nameSpace), true);
            UsePropertyAndDrawStringArray(serializedObject, "_path", TryGetText("gui.name"), target.spriteName, ImageLoader.GetSpriteNames(target.type, target.nameSpace));

            UseProperty("_spriteTag", TryGetText("inspector.sprite_setter.spriteTag"));

            Space();

            UseProperty("_index", TryGetText("gui.index"));

            Space();

            UseProperty("_defaultSprite", TryGetText("inspector.sprite_setter.defaultSprite"));

            if (EditorGUI.EndChangeCheck())
                TargetsInvoke(x => x.Refresh());
        }
    }
}
