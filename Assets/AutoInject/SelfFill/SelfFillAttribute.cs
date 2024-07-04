using System;
using System.Diagnostics;
using UnityEngine;

namespace AutoInject.SelfFill
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SelfFillAttribute : PropertyAttribute
    {
        public FillType Type { get; }
        public bool IncludeChildren { get; }

        public SelfFillAttribute(FillType fillType = FillType.Name, bool includeChildren = true)
        {
            Type = fillType;
            IncludeChildren = includeChildren;
        }
    }

    public enum FillType
    {
        Type,
        Name
    }
}
