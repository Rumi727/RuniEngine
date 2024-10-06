using RuniEngine.UI;
using System;

namespace RuniEngine.Pooling
{
    public abstract class UIObjectPoolingBase : UIBase, IObjectPooling
    {
        public string poolingNameSpace { get; set; } = string.Empty;
        public string poolingKey { get; set; } = string.Empty;

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
        /// Please put <see cref="OnCreate"/> when overriding
        /// </summary>
        public virtual void OnCreate() => IObjectPooling.OnCreateDefault(transform, this);

        /// <summary>
        /// Please put <see cref="Remove"/> when overriding
        /// </summary>
        public virtual void Remove() => IObjectPooling.RemoveDefault(this);
    }
}
