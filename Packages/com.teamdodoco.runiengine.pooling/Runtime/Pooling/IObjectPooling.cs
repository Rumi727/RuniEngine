using System;
using UnityEngine;

namespace RuniEngine.Pooling
{
    public interface IObjectPooling
    {
        string poolingNameSpace { get; set; }
        string poolingKey { get; set; }

        bool isActived { get; }

        bool disableCreation { get; set; }

        //IRefreshable[] refreshableObjects { get; }

        Action? removed { get; set; }

        void OnCreate();

        bool IsDestroyed();

        public static void OnCreateDefault(Transform transform, IObjectPooling objectPooling)
        {
            transform.gameObject.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            transform.gameObject.name = objectPooling.poolingKey;

            transform.localPosition = Vector3.zero;

            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        public static bool RemoveDefault<T>(T poolingObject) where T : MonoBehaviour, IObjectPooling
        {
            if (!poolingObject.isActived)
                return false;

            poolingObject.removed?.Invoke();
            poolingObject.removed = null;

            ObjectPoolingManager.AddObject(poolingObject);
            poolingObject.name = poolingObject.poolingKey;

            /*monoBehaviour.transform.localPosition = Vector3.zero;

            monoBehaviour.transform.localEulerAngles = Vector3.zero;
            monoBehaviour.transform.localScale = Vector3.one;*/

            poolingObject.StopAllCoroutines();
            return true;
        }
    }
}
