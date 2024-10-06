using System;
using System.Collections.Generic;

namespace RuniEngine
{
    public class TypeList<T> : List<T>, ITypeList
    {
        public TypeList() : base() { }
        public TypeList(int capacty) : base(capacty) { }
        public TypeList(IEnumerable<T> collection) : base(collection) { }

        public Type listType => typeof(T);
    }
}
