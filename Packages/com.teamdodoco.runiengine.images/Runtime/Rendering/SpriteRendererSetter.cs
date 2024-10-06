using RuniEngine.Threading;
using UnityEngine;

namespace RuniEngine.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteRendererSetter : SpriteSetterBase
    {
        public SpriteRenderer? spriteRenderer => _spriteRenderer = this.GetComponentFieldSave(_spriteRenderer);
        SpriteRenderer? _spriteRenderer;

        public override void Refresh()
        {
            if (spriteRenderer == null)
                return;

            if (ThreadTask.isMainThread)
                spriteRenderer.sprite = GetSprite();
            else
                ThreadDispatcher.Execute(() => spriteRenderer.sprite = GetSprite());
        }
    }
}
