#nullable enable
using RuniEngine.Booting;
using System;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Data
{
    public static class StorableClassUtility
    {
        /// <summary>
        /// 조건에 맞는 모든 클래스에 대한 저장 가능한 클래스를 생성합니다
        /// </summary>
        /// <param name="attribute">클래스가 가지고 있어야하는 어트리뷰트</param>
        /// <returns></returns>
        public static StorableClass[] AutoInitialize<T>() => AutoInitialize(typeof(T));

        /// <summary>
        /// 조건에 맞는 모든 클래스에 대한 저장 가능한 클래스를 생성합니다
        /// </summary>
        /// <param name="attribute">클래스가 가지고 있어야하는 어트리뷰트</param>
        /// <returns></returns>
        public static StorableClass[] AutoInitialize(Type attribute)
        {
            List<StorableClass> storableObjectInfos = new List<StorableClass>();
            Type[] types = ReflectionManager.types;
            new StorableClass(typeof(StorableClass)).GetType();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (Attribute.GetCustomAttributes(type, attribute).Length <= 0)
                    continue;

                storableObjectInfos.Add(new StorableClass(type));
            }

            return storableObjectInfos.ToArray();
        }

        public static void AutoNameSave(this StorableClass storableObject, string folderPath) => storableObject.Save(Path.Combine(folderPath, storableObject.fullName + ".json"));

        /// <summary>
        /// 모든 저장 가능한 클래스를 로드합니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void AutoNameLoad(this StorableClass storableObject, string folderPath) => storableObject.Load(Path.Combine(folderPath, storableObject.fullName + ".json"));

        /// <summary>
        /// 모든 저장 가능한 클래스를 저장합니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void SaveAll(this IEnumerable<StorableClass> storableObjects, string folderPath)
        {
            IEnumerator<StorableClass> enumerator = storableObjects.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StorableClass storableObject = enumerator.Current;
                storableObject.Save(Path.Combine(folderPath, storableObject.fullName + ".json"));
            }
        }

        /// <summary>
        /// 모든 저장 가능한 클래스를 로드합니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void LoadAll(this IEnumerable<StorableClass> storableObjects, string folderPath)
        {
            IEnumerator<StorableClass> enumerator = storableObjects.GetEnumerator();
            while (enumerator.MoveNext())
            {
                StorableClass storableObject = enumerator.Current;
                storableObject.Load(Path.Combine(folderPath, storableObject.fullName + ".json"));
            }
        }
    }
}
