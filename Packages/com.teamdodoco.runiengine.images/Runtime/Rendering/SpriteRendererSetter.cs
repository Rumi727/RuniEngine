#nullable enable
using UnityEngine;

namespace RuniEngine.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteRendererSetter : SpriteSetterBase
    {
        public SpriteRenderer? spriteRenderer => _spriteRenderer = this.GetComponentFieldSave(_spriteRenderer);
        SpriteRenderer? _spriteRenderer;

        bool isLocalLoad = true;
        public override void Refresh()
        {
            if (spriteRenderer == null)
                return;

            if (spriteRenderer.sprite != null && isLocalLoad)
            {
                Destroy(spriteRenderer.sprite.texture);
                Destroy(spriteRenderer.sprite);
            }

            if (Kernel.isPlaying)
                isLocalLoad = false;

            spriteRenderer.sprite = GetSprite();
        }
    }
}
