#nullable enable
using System;

namespace RuniEngine.Booting
{
    /// <summary>
    /// 프로젝트 설정 불러오기가 끝나면 메소드를 호출 시켜주는 어트리뷰트 입니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StartenAttribute : Attribute { }
}
