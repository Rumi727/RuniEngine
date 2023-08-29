#nullable enable
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace RuniEngine
{
    public static class ConverterUtility
    {
        public static T[]? ConvertObjects<T>(this IList<Object>? rawObjects) where T : Object
        {
            if (rawObjects == null)
                return null;
            
            T[] array = new T[rawObjects.Count];
            for (int i = 0; i < array.Length; i++)
                array[i] = (T)rawObjects[i];

            return array;
        }
    }
}
