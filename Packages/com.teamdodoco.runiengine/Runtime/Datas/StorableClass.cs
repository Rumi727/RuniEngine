using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuniEngine.Resource.Texts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RuniEngine.Datas
{
    /// <summary>
    /// 클래스를 저장/로드 해주는 클래스 입니다
    /// </summary>
    public sealed class StorableClass
    {
        public Type type { get; }
        public string fullName => type.FullName;

        public object? instance { get; set; } = null;

        [StaticResettable] static readonly Dictionary<string, IReadOnlyList<StorableClassMemberInfo<PropertyInfo>>> cachedMemberInfos = new();
        public IReadOnlyList<StorableClassMemberInfo<PropertyInfo>> memberInfos { get; }

        /// <summary>
        /// 클래스 & 구조체에 대한 저장 가능한 오브젝트 생성
        /// </summary>
        /// <param name="type"></param>
        public StorableClass(object instance, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) : this(instance.GetType(), instance, bindingFlags) { }

        /// <summary>
        /// 정적 클래스 & 구조체에 대한 저장 가능한 오브젝트 생성
        /// </summary>
        /// <param name="type"></param>
        public StorableClass(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static) : this(type, null, bindingFlags) { }

        StorableClass(Type type, object? instance, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static)
        {
            this.type = type;
            this.instance = instance;

            if (cachedMemberInfos.TryGetValue(fullName, out var value))
                memberInfos = value;
            else
            {
                PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);
                List<StorableClassMemberInfo<PropertyInfo>> propertyInfoList = new List<StorableClassMemberInfo<PropertyInfo>>();

                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    PropertyInfo propertyInfo = propertyInfos[i];
                    if (propertyInfo.AttributeContains<JsonIgnoreAttribute>())
                        continue;

                    if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    {
                        string text = "";
                        if (!propertyInfo.CanRead && !propertyInfo.CanWrite)
                            text = "Get, Set";
                        else if (!propertyInfo.CanRead)
                            text = "Get";
                        else if (!propertyInfo.CanWrite)
                            text = "Set";

                        Debug.LogWarning
                        (
                            LanguageLoader.TryGetText("storable_class.warning.attribute").
                            Replace("{class}", fullName).
                            Replace("{property}", propertyInfo.Name).
                            Replace("{method}", text)
                        );
                        continue;
                    }

                    /*//JsonProperty 어트리뷰트가 없으면 경고 표시
                    if (!propertyInfo.AttributeContains<JsonPropertyAttribute>())
                    {
                        Debug.LogWarning
                        (
                            LanguageLoader.TryGetText("storable_class.warning.attribute").
                            Replace("{class}", fullName).
                            Replace("{property}", propertyInfo.Name)
                        );
                        continue;
                    }*/

                    propertyInfoList.Add(new StorableClassMemberInfo<PropertyInfo>(propertyInfo, propertyInfo.GetValue(instance)));
                }

                memberInfos = propertyInfoList.ToArray();
                cachedMemberInfos.Add(fullName, memberInfos);
            }
        }

        public void Save(string path)
        {
            JObject jObject = new JObject();
            for (int i = 0; i < memberInfos.Count; i++)
            {
                StorableClassMemberInfo<PropertyInfo> dataMemberInfo = memberInfos[i];
                jObject.Add(dataMemberInfo.name, JToken.FromObject(dataMemberInfo.memberInfo.GetValue(instance)));
            }

            string folderPath = PathUtility.GetParentPath(path);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            File.WriteAllText(path, jObject.ToString());
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
                Save(path);

            for (int i = 0; i < memberInfos.Count; i++)
            {
                StorableClassMemberInfo<PropertyInfo> memberInfo = memberInfos[i];
                memberInfo.memberInfo.SetValue(instance, null);
            }

            {
                try
                {
                    string json = File.ReadAllText(path);
                    JObject? jObject = JsonConvert.DeserializeObject<JObject>(json);

                    if (jObject != null)
                    {
                        foreach (var item in jObject)
                        {
                            for (int i = 0; i < memberInfos.Count; i++)
                            {
                                StorableClassMemberInfo<PropertyInfo> dataMemberInfo = memberInfos[i];
                                if (item.Key == dataMemberInfo.name)
                                    dataMemberInfo.memberInfo.SetValue(instance, item.Value?.ToObject(dataMemberInfo.memberInfo.PropertyType));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            for (int i = 0; i < memberInfos.Count; i++)
            {
                StorableClassMemberInfo<PropertyInfo> memberInfo = memberInfos[i];
                if (memberInfo.memberInfo.GetValue(instance) == null)
                    memberInfo.memberInfo.SetValue(instance, memberInfo.defaultValue);
            }
        }

        public void SetDefault()
        {
            for (int i = 0; i < memberInfos.Count; i++)
            {
                StorableClassMemberInfo<PropertyInfo> memberInfo = memberInfos[i];
                memberInfo.memberInfo.SetValue(instance, memberInfo.defaultValue);
            }
        }
    }
}
