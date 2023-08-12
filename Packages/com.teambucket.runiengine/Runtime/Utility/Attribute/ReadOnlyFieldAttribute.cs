#nullable enable
using System;
using UnityEngine;

namespace RuniEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReadOnlyFieldAttribute : PropertyAttribute
    {

    }
}
