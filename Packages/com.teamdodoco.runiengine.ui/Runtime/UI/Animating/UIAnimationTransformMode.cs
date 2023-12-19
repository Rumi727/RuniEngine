#nullable enable
using System;

namespace RuniEngine.UI.Animating
{
    [Flags]
    public enum UIAnimationTransformMode
    {
        none = 0,

        position = positionX | positionY | positionZ,
        positionX = 1 << 0,
        positionY = 1 << 1,
        positionZ = 1 << 2,

        rotation = rotationX | rotationY | rotationZ,
        rotationX = 1 << 3,
        rotationY = 1 << 4,
        rotationZ = 1 << 5,

        scale = scaleX | scaleY | scaleZ,
        scaleX = 1 << 6,
        scaleY = 1 << 7,
        scaleZ = 1 << 8
    }
}
