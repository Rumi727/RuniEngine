using System;
using System.Collections;

namespace RuniEngine
{
    public interface ITypeList : IList
    {
        Type listType { get; }
    }
}
