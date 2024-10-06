using System;

namespace RuniEngine.UI.Animating
{
    [Flags]
    public enum UIAnimationRectTransformMode
    {
        none = 0,

        anchoredPosition = anchoredPositionX | anchoredPositionY,
        anchoredPosition3D = anchoredPositionX | anchoredPositionY | anchoredPositionZ,
        anchoredPositionX = 1 << 0,
        anchoredPositionY = 1 << 1,
        anchoredPositionZ = 1 << 2,

        sizeDelta = sizeDeltaX | sizeDeltaY,
        sizeDeltaX = 1 << 3,
        sizeDeltaY = 1 << 4,

        offsets = offsetMin | offsetMax,

        offsetMin = offsetMinX | offsetMinY,
        offsetMinX = 1 << 5,
        offsetMinY = 1 << 6,

        offsetMax = offsetMaxX | offsetMaxY,
        offsetMaxX = 1 << 7,
        offsetMaxY = 1 << 8,

        anchors = anchorMin | anchorMax,

        anchorMin = anchorMinX | anchorMinY,
        anchorMinX = 1 << 9,
        anchorMinY = 1 << 10,

        anchorMax = anchorMaxX | anchorMaxY,
        anchorMaxX = 1 << 11,
        anchorMaxY = 1 << 12,

        pivot = pivotX | pivotY,
        pivotX = 1 << 13,
        pivotY = 1 << 14,

        rotation = rotationX | rotationY | rotationZ,
        rotationX = 1 << 15,
        rotationY = 1 << 16,
        rotationZ = 1 << 17,

        scale = scaleX | scaleY | scaleZ,
        scaleX = 1 << 18,
        scaleY = 1 << 19,
        scaleZ = 1 << 20
    }
}
