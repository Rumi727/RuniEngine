#nullable enable
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RuniEngine.Data
{
    /// <summary>
    /// 클래스를 저장/로드 해주는 클래스 입니다 (정적 프로퍼티만 저장/로드 합니다)
    /// </summary>
    public sealed class StorableClass
    {
        public Type type { get; }
        public string fullName => type.FullName;

        public StorableClassMemberInfo<PropertyInfo>[] memberInfos { get; }

        /// <summary>
        /// 클래스에 대한 저장 가능한 오브젝트 생성
        /// </summary>
        /// <param name="type"></param>
        public StorableClass(Type type)
        {
            this.type = type;
            
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            List<StorableClassMemberInfo<PropertyInfo>> propertyInfoList = new List<StorableClassMemberInfo<PropertyInfo>>();

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(JsonIgnoreAttribute)).Length > 0)
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

                    Debug.LogWarning($"{fullName}.{propertyInfo.Name}" +
                        $@" 프로퍼티에 {text} 메소드가 없는 것 같습니다
이 변수는 로드되지 않을것입니다.
무시를 원하신다면 [JsonIgnore] 어트리뷰트를 추가해주세요");
                    continue;
                }

                //JsonProperty 어트리뷰트가 없으면 경고 표시
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(JsonPropertyAttribute)).Length <= 0)
                {
                    Debug.LogWarning($"{fullName}.{propertyInfo.Name}" +
                        @" 프로퍼티에 [JsonProperty] 어트리뷰트가 추가되어있지 않습니다.
이 변수는 로드되지 않을것입니다.
무시를 원하신다면 [JsonIgnore] 어트리뷰트를 추가해주세요");
                    continue;
                }
                else
                    propertyInfoList.Add(new StorableClassMemberInfo<PropertyInfo>(propertyInfo, propertyInfo.GetValue(null)));
            }

            memberInfos = propertyInfoList.ToArray();
        }

        public void Save(string path)
        {
            JObject jObject = new JObject();
            for (int i = 0; i < memberInfos.Length; i++)
            {
                StorableClassMemberInfo<PropertyInfo> dataMemberInfo = memberInfos[i];
                jObject.Add(dataMemberInfo.name, JToken.FromObject(dataMemberInfo.memberInfo.GetValue(null)));
            }

            File.WriteAllText(path, jObject.ToString());
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
                Save(path);

            for (int i = 0; i < memberInfos.Length; i++)
            {
                StorableClassMemberInfo<PropertyInfo> memberInfo = memberInfos[i];
                memberInfo.memberInfo.SetValue(null, null);
            }

            {
                try
                {
                    string json = File.ReadAllText(path);
                    JsonConvert.DeserializeObject(json, type);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            for (int i = 0; i < memberInfos.Length; i++)
            {
                StorableClassMemberInfo<PropertyInfo> memberInfo = memberInfos[i];
                if (memberInfo.memberInfo.GetValue(null) == null)
                    memberInfo.memberInfo.SetValue(null, memberInfo.defaultValue);
            }
        }
    }
}
