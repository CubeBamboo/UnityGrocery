/**********************************************************************
 * source code written by: qingyiwebt(https://github.com/qingyiwebt)
 **********************************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace AutoInject.SelfFill
{
    public static class PropertyValues
    {
        public static GUIContent RepairLabel(GUIContent label, SerializedProperty property)
        {
            if (label.text == null) //even GUIContent.none has text = "" instead of null
                label = new GUIContent(property.name, property.tooltip);
            Debug.Assert(label.text != null);
            label.text = PropertyConversions.NameFormat(label.text);
            return label;
        }
    }

    public static class PropertyConversions
    {
        /// <summary></summary>
        /// <returns>removes underscores, starts uppercase, adds spaces</returns>
        public static string NameFormat(string name)
        {
            if (name is null)
                throw new ArgumentException("Null-String cannot be formatted");
            if (name == "")
                return "";

            //remove underscore start
            if (name[0] == '_')
                name = name[1..];
            if (name == "") //contained only the underscore
                return " "; //space so that label is still shown but only empty

            //first character always uppercase
            var res = char.ToUpper(name[0]).ToString();
            //add remaining but insert space before uppercases
            for (var i = 1; i < name.Length; i++)
                if (char.IsUpper(name[i]) &&
                    (i <= 0 || name[i - 1] != ' ')) //if uppercase letter and not already inserted
                    res += " " + name[i]; //to lowercase would be: res += " " + (char)(name[i] + ('a' - 'A'));
                else
                    res += name[i];
            return res;
        }
    }
}
