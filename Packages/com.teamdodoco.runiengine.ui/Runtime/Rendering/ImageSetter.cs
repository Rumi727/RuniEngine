#nullable enable
using RuniEngine.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.Rendering
{
    [RequireComponent(typeof(Image))]
    public class ImageSetter : SpriteSetterBase
    {
        public Image? image => _image = this.GetComponentFieldSave(_image);
        Image? _image;

        public override void Refresh()
        {
            if (image == null)
                return;
            
            if (ThreadManager.isMainThread)
                image.sprite = GetSprite();
            else
                ThreadDispatcher.Execute(() => image.sprite = GetSprite());
        }
    }
}
