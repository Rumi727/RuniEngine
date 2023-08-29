#nullable enable
using UnityEngine;

namespace RuniEngine.UI
{
    public abstract class UIAniBase : UIBase
    {
        public bool disableLerpAni { get => _disableLerpAni; set => disableLerpAni = value; }
        [SerializeField] bool _disableLerpAni = false;

        public bool useCustomLerpSpeed { get => _useCustomLerpSpeed; set => _useCustomLerpSpeed = value; }
        [SerializeField] bool _useCustomLerpSpeed = false;

        public float lerpSpeed { get => _lerpSpeed; set => _lerpSpeed = value; }

        [SerializeField, Range(0, 1)] float _lerpSpeed = 0.2f;

        public float currentLerpSpeed => useCustomLerpSpeed ? lerpSpeed : UIManager.UserData.defaultLerpAniSpeed;
    }
}
