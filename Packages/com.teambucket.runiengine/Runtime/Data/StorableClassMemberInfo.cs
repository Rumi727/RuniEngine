#nullable enable
using System.Reflection;

namespace RuniEngine.Data
{
    /// <summary>
    /// 저장 가능한 클래스에 있는 멤버에 대한 정보를 가지고 있는 클래스 입니다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StorableClassMemberInfo<T> where T : MemberInfo
    {
        public T memberInfo { get; }

        public string name => memberInfo.Name;
        public object? defaultValue { get; }

        public StorableClassMemberInfo(T memberInfo, object? defaultValue)
        {
            this.memberInfo = memberInfo;
            this.defaultValue = defaultValue;
        }
    }
}
