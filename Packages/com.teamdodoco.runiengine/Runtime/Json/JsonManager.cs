#nullable enable
using Newtonsoft.Json;
using System;
using System.IO;

namespace RuniEngine.Json
{
    public static class JsonManager
    {
        /// <summary>
        /// 텍스트 파일에서 Json을 읽고 반환합니다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">
        /// 텍스트 파일 경로
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// </param>
        /// <returns></returns>
        public static T? JsonRead<T>(string path) => (T?)JsonRead(typeof(T), path);

        /// <summary>
        /// 텍스트 파일에서 Json을 읽고 반환합니다
        /// </summary>
        /// <param name="path">
        /// 텍스트 파일 경로
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// </param>
        /// <returns></returns>
        public static object? JsonRead(Type type, string path)
        {
            string json = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(json))
                return ToObject(type, json);

            return default;
        }

        public static T? ToObject<T>(string value) => (T?)ToObject(typeof(T), value);
        public static object? ToObject(Type type, string value) => JsonConvert.DeserializeObject(value, type);

        public static string ToJson(object? value) => JsonConvert.SerializeObject(value, Formatting.Indented);
    }
}
