#nullable enable
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

            TargetsSetValue
            (
                x => x.nameSpace,
                x => UsePropertyAndDrawNameSpace(serializedObject, "_nameSpace", TryGetText("gui.namespace"), x.nameSpace),
                (x, y) => x.nameSpace = y,
                targets
            );

            TargetsSetValue
            (
                x => x.type,
                x => UsePropertyAndDrawStringArray(serializedObject, "_type", TryGetText("gui.type"), x.type, ImageLoader.GetTypes(x.nameSpace)),
                (x, y) => x.type = y,
                targets
            );

            TargetsSetValue
            (
                x => x.spriteName,
                x => UsePropertyAndDrawStringArray(serializedObject, "_path", TryGetText("gui.name"), x.spriteName, ImageLoader.GetSpriteNames(x.type, x.nameSpace)),
                (x, y) => x.spriteName = y,
                targets
            );

            UseProperty("_spriteTag", TryGetText("inspector.sprite_setter.spriteTag"));

            Space();

            UseProperty("_index", TryGetText("gui.index"));

            Space();

            UseProperty("_defaultSprite", TryGetText("inspector.sprite_setter.defaultSprite"));

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    SpriteSetterBase? value = targets[i];
                    if (value != null)
                        value.Refresh();
                }
            }
        }
    }
}
