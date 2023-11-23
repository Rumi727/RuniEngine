#nullable enable
using Newtonsoft.Json;
using RuniEngine.Booting;
using RuniEngine.Data;
using RuniEngine.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Pooling
{
    public sealed class ObjectPoolingManager : MonoBehaviour
    {
        public static Transform instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Instantiate(ResourceUtility.emptyRectTransform);
                    DontDestroyOnLoad(_instance);

                    _instance.name = "Object Pool";
                }

                return _instance;
            }
        }
        static Transform? _instance;

        [ProjectData]
        public struct ProjectData
        {
            [JsonProperty] public static Dictionary<string, string> prefabList { get; set; } = new Dictionary<string, string>();
        }

        static readonly List<Instance> instanceList = new List<Instance>();
        class Instance
        {
            public string key = "";
            public (MonoBehaviour? monoBehaviour, IObjectPooling objectPooling) objectPooling;

            public Instance(string key, (MonoBehaviour? monoBehaviour, IObjectPooling objectPooling) objectPooling)
            {
                this.key = key;
                this.objectPooling = objectPooling;
            }
        }




        /// <summary>
        /// 오브젝트를 미리 생성합니다
        /// </summary>
        /// <param name="objectKey">미리 생성할 오브젝트 키</param>
        public static void ObjectAdvanceCreate(string objectKey)
        {
            MonoBehaviour? monoBehaviour = Resources.Load<MonoBehaviour>(ProjectData.prefabList[objectKey]);
            if (monoBehaviour is not IObjectPooling)
                return;

            ObjectAdd(objectKey, monoBehaviour);
        }

        /// <summary>
        /// 오브젝트를 리스트에 추가합니다
        /// </summary>
        /// <param name="objectKey">추가할 오브젝트의 키</param>
        /// <param name="monoBehaviour">추가할 오브젝트</param>
        public static void ObjectAdd(string objectKey, MonoBehaviour monoBehaviour)
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();
            BasicDataNotLoadedException.Exception();

            MonoBehaviour? instantiate = Instantiate(monoBehaviour, instance.transform);
            if (instantiate is not IObjectPooling objectPooling)
                return;

            objectPooling.objectKey = objectKey;
            ObjectRemove(objectKey, instantiate, objectPooling);
        }

        /// <summary>
        /// 오브젝트가 리스트에 있는지 감지합니다 (리소스 폴더에 있는 프리팹은 알아서 감지하고 생성하니, 이 함수를 쓸 필요가 없습니다)
        /// </summary>
        /// <param name="objectKey">감지할 오브젝트 키</param>
        /// <returns></returns>
        public static bool ObjectContains(string objectKey)
        {
            for (int i = 0; i < instanceList.Count; i++)
            {
                Instance instance = instanceList[i];
                if (instance.key == objectKey)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 오브젝트를 생성합니다
        /// </summary>
        /// <param name="objectKey">생성할 오브젝트 키</param>
        /// <param name="parent">생성할 오브젝트가 자식으로갈 오브젝트</param>
        /// <returns></returns>
        public static (MonoBehaviour? monoBehaviour, IObjectPooling? objectPooling) ObjectCreate(string objectKey, Transform? parent = null)
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();
            BasicDataNotLoadedException.Exception();

            int objectIndex = -1;
            for (int i = 0; i < instanceList.Count; i++)
            {
                Instance instance = instanceList[i];
                if (instance.key == objectKey)
                {
                    (MonoBehaviour? monoBehaviour, IObjectPooling objectPooling) = instance.objectPooling;
                    if (monoBehaviour == null)
                    {
                        instanceList.RemoveAt(i);

                        i--;
                        continue;
                    }

                    if (!objectPooling.disableCreation)
                    {
                        objectIndex = i;
                        break;
                    }
                }
            }

            if (objectIndex >= 0)
            {
                (MonoBehaviour? monoBehaviour, IObjectPooling objectPooling) = instanceList[objectIndex].objectPooling;
                if (monoBehaviour == null)
                    return (null, null);

                monoBehaviour.transform.SetParent(parent, false);
                monoBehaviour.gameObject.SetActive(true);

                objectPooling.objectKey = objectKey;
                instanceList.RemoveAt(objectIndex);

                objectPooling.OnCreate();
                return (monoBehaviour, objectPooling);
            }
            else if (ProjectData.prefabList.ContainsKey(objectKey))
            {
                GameObject? gameObject = Resources.Load<GameObject>(ProjectData.prefabList[objectKey]);
                if (gameObject == null)
                    return (null, null);

                if (!Instantiate(gameObject, parent).TryGetComponent<IObjectPooling>(out IObjectPooling? objectPooling))
                    return (null, null);

                MonoBehaviour monoBehaviour = (MonoBehaviour)objectPooling;

                monoBehaviour.name = objectKey;
                objectPooling.objectKey = objectKey;

                objectPooling.OnCreate();
                return (monoBehaviour, objectPooling);
            }

            return (null, null);
        }

        /// <summary>
        /// 오브젝트를 삭제합니다
        /// </summary>
        /// <param name="objectKey">지울 오브젝트 키</param>
        /// <param name="objectPooling">지울 오브젝트</param>
        public static bool ObjectRemove(string objectKey, MonoBehaviour monoBehaviour, IObjectPooling objectPooling)
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();
            BasicDataNotLoadedException.Exception();

            monoBehaviour.gameObject.SetActive(false);
            monoBehaviour.transform.SetParent(instance.transform, false);

            instanceList.Add(new Instance(objectKey, (monoBehaviour, objectPooling)));
            return true;
        }
    }
}