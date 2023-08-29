#nullable enable
using RuniEngine.UI;

namespace RuniEngine.Editor.Inspector.UI
{
    public abstract class UIAniBaseEditor<TTarget> : UIBaseEditor<TTarget> where TTarget : UIAniBase
    {
        /// <summary>
        /// Please put base.OnDisable() when overriding
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            UseProperty("_disableLerpAni", TryGetText("inspector.ui_ani.disable_lerp_ani"));
            if (TargetsIsEquals(x => x.disableLerpAni))
            {
                if (!target.disableLerpAni)
                {
                    UseProperty("_useCustomLerpSpeed", TryGetText("inspector.ui_ani.use_custom_lerp_speed"));
                    if (TargetsIsEquals(x => x.useCustomLerpSpeed))
                    {
                        if (target.useCustomLerpSpeed)
                            UseProperty("_lerpSpeed", TryGetText("inspector.ui_ani.lerp_speed"));
                    }
                    else
                        UseProperty("_lerpSpeed", TryGetText("inspector.ui_ani.lerp_speed"));
                }
            }
            else
            {
                UseProperty("_useCustomLerpSpeed", TryGetText("inspector.ui_ani.use_custom_lerp_speed"));
                UseProperty("_lerpSpeed", TryGetText("inspector.ui_ani.lerp_speed"));
            }
        }
    }
}
