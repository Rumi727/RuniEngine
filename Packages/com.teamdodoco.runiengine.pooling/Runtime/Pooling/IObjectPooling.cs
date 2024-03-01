#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.Pooling
{
    public interface IObjectPooling
    {
        string objectKey { get; set; }
        bool isActived { get; }

        bool disableCreation { get; set; }

        //IRefreshable[] refreshableObjects { get; }

        Action? removed { get; set; }

        void OnCreate();

        bool IsDestroyed();

        public static void OnCreateDefault(Transform transform, IObjectPooling objectPooling)
        {
            transform.gameObject.name = objectPooling.objectKey;

            transform.localPosition = Vector3.zero;

            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        public static bool RemoveDefault(MonoBehaviour monoBehaviour, IObjectPooling objectPooling)
        {
            if (!objectPooling.isActived)
                return false;
            if (!Kernel.isPlaying)
                return false;

            objectPooling.removed?.Invoke();
            objectPooling.removed = null;

            ObjectPoolingManager.ObjectRemove(objectPooling.objectKey, monoBehaviour, objectPooling);
            monoBehaviour.name = objectPooling.objectKey;

            /*monoBehaviour.transform.localPosition = Vector3.zero;

            monoBehaviour.transform.localEulerAngles = Vector3.zero;
            monoBehaviour.transform.localScale = Vector3.one;*/

            monoBehaviour.StopAllCoroutines();
            return true;
        }
    }
}
