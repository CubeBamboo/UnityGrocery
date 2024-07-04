using System;
using AutoInject.SelfFill;
using UnityEditor;
using UnityEngine;

namespace AutoInject.Editor
{
    [CustomPropertyDrawer(typeof(SelfFillAttribute))]
    public class SelfFillAttributeDrawer : PropertyDrawer
    {
        private bool _success;
        private SelfFillAttribute _selfFillAttribute;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _selfFillAttribute = attribute as SelfFillAttribute;
            ProcessSelfFill(property, label);
            if (!_success)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void ProcessSelfFill(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogError($"SelfFill: type {property.propertyType} not supported. Use this attribute on objects (interfaces with RequireType or components)");
                _success = false;
                return;
            }

            label.text += " (auto-filled)";
            const string tooltipMessage =
                "SelfFill: This field will be automatically filled with the first matching component on this gameObject";
            label.tooltip = string.IsNullOrEmpty(label.tooltip) ? tooltipMessage : $"{label.tooltip}\n{tooltipMessage}";
            
            if (property.serializedObject.targetObject is not Component root)
            {
                Debug.LogError(property.serializedObject.targetObject is ScriptableObject
                    ? "SelfFillAttribute for ScriptableObjects not supported"
                    : $"SelfFillAttribute for {property.serializedObject.targetObject.GetType()} not supported");
                _success = false;
                return;
            }

            if (property.objectReferenceValue != null)
            {
                _success = true;
                return;
            }

            OnSelfFill(property, root, _selfFillAttribute.IncludeChildren);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void OnSelfFill(SerializedProperty property, Component root, bool includeChildren)
        {
            var findType = fieldInfo.FieldType;

            bool findIsGameObject = findType == typeof(GameObject);
            bool findIsComponent = findType == typeof(Component) ||
                                   findType.IsSubclassOf(typeof(Component));

            if(!findIsGameObject && !findIsComponent)
            {
                Debug.LogError($"SelfFill: type {fieldInfo.FieldType} not supported. Use this attribute on objects (interfaces with RequireType or components)");
                _success = false;
                return;
            }
            
            var toFillType = findIsGameObject ? ToFillType.GameObject : ToFillType.Component;
            
            if (_selfFillAttribute.Type == FillType.Type)
            {
                TypeBasedFill(property, root, includeChildren, toFillType);
            }
            else
            {
                NameBasedFill(property, root, toFillType);
            }
        }
        
        private void TypeBasedFill(SerializedProperty property, Component root, bool includeChildren, ToFillType toFillType)
        {
            if (toFillType == ToFillType.GameObject)
            {
                property.objectReferenceValue = root;
                _success = true;
            }
            else
            {
                FillComponent(property, root, fieldInfo.FieldType, includeChildren);
            }
        }
        
        private void NameBasedFill(SerializedProperty property, Component root, ToFillType toFillType)
        {
            var rootGameObject = root.gameObject;
            var name = fieldInfo.Name;
            
            var found = FindChildByName(rootGameObject, name);
            
            if (found == null)
            {
                Debug.LogError($"SelfFill: {name} not found on {rootGameObject.name}");
                _success = false;
                return;
            }

            property.objectReferenceValue =
                toFillType == ToFillType.GameObject ? found.gameObject : found.GetComponent(fieldInfo.FieldType);

            _success = true;
        }

        private static Transform FindChildByName(GameObject rootGameObject, string name, bool includeSelf = true)
        {
            if (includeSelf && rootGameObject.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return rootGameObject.transform;
            }
            return InternalFindChildByName(rootGameObject, name);
        }
        
        private static Transform InternalFindChildByName(GameObject rootGameObject, string name, int depth = 0)
        {
            if (depth > 100)
            {
                Debug.LogError("SelfFill: Max depth reached");
                return null;
            }
            
            for(int i = 0; i < rootGameObject.transform.childCount; i++)
            {
                var child = rootGameObject.transform.GetChild(i);
                if (child.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
                
                if(rootGameObject.transform.childCount > 0)
                {
                    var found = FindChildByName(child.gameObject, name);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }
        
        private void FillComponent(SerializedProperty property, Component root, Type componentType, bool includeChildren)
        {
            property.objectReferenceValue =
                includeChildren ? root.GetComponentInChildren(componentType) : root.GetComponent(componentType);

            if (property.objectReferenceValue == null)
            {
                Debug.LogError($"SelfFill: {fieldInfo.FieldType} not found on {root.name}");
                _success = false;
                return;
            }

            _success = true;
        }

        private enum ToFillType
        {
            GameObject,
            Component
        }
    }
}
