#nullable enable
using RuniEngine.Booting;
using RuniEngine.Resource;
using RuniEngine.Resource.Objects;
using RuniEngine.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine.Pooling
{
    public static class ObjectPoolingManager
    {
        public static Transform instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.Instantiate(ResourceUtility.emptyRectTransform);
                    Object.DontDestroyOnLoad(_instance);

                    _instance.name = "Object Pool";
                }

                return _instance;
            }
        }
        static Transform? _instance;

        

        [StaticResettable(false)] static readonly Dictionary<string, Dictionary<string, List<MonoBehaviour>>> pooledObjectList = new();




        /// <summary>
        /// 오브젝트를 미리 생성합니다
        /// </summary>
        /// <param name="key">미리 생성할 오브젝트 키</param>
        public static void CachingObject(string key, string nameSpace = "")
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            GameObject? gameObject = ObjectLoader.SearchGameObject(key, nameSpace);
            if (gameObject == null)
                return;

            MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
            if (monoBehaviour is not IObjectPooling)
                return;

            IObjectPooling poolingObject = (IObjectPooling)Object.Instantiate(monoBehaviour, instance.transform);

            poolingObject.poolingNameSpace = nameSpace;
            poolingObject.poolingKey = key;

            monoBehaviour.gameObject.SetActive(false);
            monoBehaviour.transform.SetParent(instance.transform, false);

            pooledObjectList.TryAdd(poolingObject.poolingNameSpace, new());
            pooledObjectList[poolingObject.poolingNameSpace].TryAdd(poolingObject.poolingKey, new());
            pooledObjectList[poolingObject.poolingNameSpace][poolingObject.poolingKey].Add(monoBehaviour);
        }

        /// <summary>
        /// 커스텀 오브젝트를 풀링 리스트에 추가합니다
        /// </summary>
        /// <param name="key">추가할 오브젝트의 키</param>
        /// <param name="poolingObject">추가할 오브젝트</param>
        public static void AddObject<T>(T poolingObject) where T : MonoBehaviour, IObjectPooling
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

            poolingObject.gameObject.SetActive(false);
            poolingObject.transform.SetParent(instance.transform, false);

            pooledObjectList.TryAdd(poolingObject.poolingNameSpace, new());
            pooledObjectList[poolingObject.poolingNameSpace].TryAdd(poolingObject.poolingKey, new());
            pooledObjectList[poolingObject.poolingNameSpace][poolingObject.poolingKey].Add(poolingObject);
        }

        /// <summary>
        /// 오브젝트가 리스트에 있는지 감지합니다 (커스텀 오브젝트으로 수동 풀링 시키지 않는이상 알아서 감지하고 생성하니, 이 메소드를 쓸 필요가 없습니다)
        /// </summary>
        /// <param name="key">감지할 오브젝트 키</param>
        /// <returns></returns>
        public static bool ContainsObject(string key, string nameSpace = "")
        {
            ResourceManager.SetDefaultNameSpace(ref nameSpace);
            return pooledObjectList.ContainsKey(nameSpace) && pooledObjectList.ContainsKey(key);
        }

        /// <summary>
        /// 오브젝트를 생성합니다
        /// </summary>
        /// <param name="key">생성할 오브젝트 키</param>
        /// <param name="parent">생성할 오브젝트가 자식으로갈 오브젝트</param>
        /// <returns></returns>
        public static T? ObjectClone<T>(string key, string nameSpace = "", Transform? parent = null) where T : MonoBehaviour, IObjectPooling
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            if (pooledObjectList.TryGetValue(nameSpace, out var value) && value.TryGetValue(key, out var poolingObjectList))
            {
                int poolingObjectIndex = -1;
                for (int i = 0; i < poolingObjectList.Count; i++)
                {
                    MonoBehaviour monoBehaviour = poolingObjectList[i];

                    //풀링 오브젝트가 맞는지만 감지하는 것이기 때문에 제너릭 타입을 사용해선 안됩니다
                    if (monoBehaviour is not IObjectPooling poolingObject || monoBehaviour == null)
                    {
                        poolingObjectList.RemoveAt(i);

                        i--;
                        continue;
                    }

                    if (!poolingObject.disableCreation)
                    {
                        poolingObjectIndex = i;
                        break;
                    }
                }

                if (poolingObjectIndex >= 0)
                {
                    if (poolingObjectList[poolingObjectIndex] is not T poolingObject)
                        return null;

                    poolingObject.transform.SetParent(parent, false);
                    poolingObject.gameObject.SetActive(true);

                    poolingObjectList.RemoveAt(poolingObjectIndex);

                    poolingObject.OnCreate();
                    return poolingObject;
                }
            }
            
            {
                GameObject? gameObject = ObjectLoader.SearchGameObject(key, nameSpace);
                if (gameObject == null)
                    return null;

                T? monoBehaviour = gameObject.GetComponent<T>();
                if (monoBehaviour == null)
                    return null;

                T poolingObject = Object.Instantiate(monoBehaviour, parent);

                poolingObject.poolingNameSpace = nameSpace;
                poolingObject.poolingKey = key;

                poolingObject.OnCreate();
                return poolingObject;
            }
        }
    }
}