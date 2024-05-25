#nullable enable
using System;

namespace RuniEngine
{
    /// <summary>
    /// 플레이 모드가 종료 되고 나서 모든 종료 이벤트가 실행되고 난 이후에 한번 멤버를 초기화합니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class StaticResettableAttribute : Attribute
    {
        public StaticResettableAttribute() { }
        public StaticResettableAttribute(bool isNullable) => this.isNullable = isNullable;
        public StaticResettableAttribute(object value) => this.value = value;



        public bool isNullable = false;
        public object? value = null;
    }
}
