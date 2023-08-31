#nullable enable
using RuniEngine.SceneManagement;
using RuniEngine.UI;
using System;

namespace RuniEngine.Pooling
{
    public abstract class UIAniObjectPoolingBase : UIAniBase, IObjectPooling
    {
        public string objectKey { get; set; } = string.Empty;

        public bool isRemoved => !isActived;

        public bool isActived => transform.parent != ObjectPoolingManager.instance;

        public bool disableCreation { get; protected set; }
        bool IObjectPooling.disableCreation { get => disableCreation; set => disableCreation = value; }



        /*IRefreshable[] _refreshableObjects;
        public IRefreshable[] refreshableObjects => _refreshableObjects = this.GetComponentsInChildrenFieldSave(_refreshableObjects, true);*/

        public event Action removed { add => _removed += value; remove => _removed -= value; }
        Action? _removed = null;
        Action? IObjectPooling.removed { get => _removed; set => _removed = value; }



        /// <summary>
        /// Please put base.OnCreate() when overriding
        /// </summary>
        public virtual void OnCreate() => IObjectPooling.OnCreateDefault(transform, this);

        /// <summary>
        /// Please put base.Remove() when overriding
        /// </summary>
        public virtual void Remove() => IObjectPooling.RemoveDefault(this, this);

        /// <summary>
        /// Please put base.ActiveSceneChanged() when overriding
        /// </summary>
        public virtual void ActiveSceneChanged()
        {
            if (!isRemoved && gameObject.scene.name != "DontDestroyOnLoad")
                Remove();
        }

        /// <summary>
        /// Please put base.OnDestroy() when overriding
        /// </summary>
        protected override void OnDestroy() => SceneManager.activeSceneChanged -= ActiveSceneChanged;
    }
}
