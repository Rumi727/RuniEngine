#nullable enable
using System;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.Datas
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
            IReadOnlyList<Type> types = ReflectionManager.types;
            
            for (int i = 0; i < types.Count; i++)
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
        /// 모든 저장 가능한 클래스를 기본값으로 되돌립니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void AutoNameSetDefault(this StorableClass storableObject) => storableObject.SetDefault();

        /// <summary>
        /// 모든 저장 가능한 클래스를 저장합니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void SaveAll(this IEnumerable<StorableClass> storableObjects, string folderPath)
        {
            IEnumerator<StorableClass> enumerator = storableObjects.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.AutoNameSave(folderPath);
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
                enumerator.Current.AutoNameLoad(folderPath);
        }

        /// <summary>
        /// 모든 저장 가능한 클래스를 기본값으로 되돌립니다
        /// </summary>
        /// <param name="storableObjects">저장 가능한 클래스 리스트</param>
        /// <param name="folderPath">폴더 경로</param>
        public static void SetDefaultAll(this IEnumerable<StorableClass> storableObjects)
        {
            IEnumerator<StorableClass> enumerator = storableObjects.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.AutoNameSetDefault();
        }
    }
}
