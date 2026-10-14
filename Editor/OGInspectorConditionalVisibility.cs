namespace Mobione.MobioneInspector.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    internal sealed class ShowIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OGInspectorConditionalVisibility.ShouldDrawCondition(
                property, (ShowIfAttribute)attribute) ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (OGInspectorConditionalVisibility.ShouldDrawCondition(property, (ShowIfAttribute)attribute))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }

    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    internal sealed class HideIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OGInspectorConditionalVisibility.ShouldDrawCondition(
                property, (HideIfAttribute)attribute) ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (OGInspectorConditionalVisibility.ShouldDrawCondition(property, (HideIfAttribute)attribute))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }

    internal static class OGInspectorConditionalVisibility
    {
        internal static System.Reflection.FieldInfo FindField(
            string propertyPath,
            object target,
            out object owner)
        {
            owner = null;
            if (target == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            string[] segments = propertyPath.Split('.');
            if (segments.Length >= 3 && segments[segments.Length - 2] == "Array" &&
                segments[segments.Length - 1].StartsWith("data[", StringComparison.Ordinal))
            {
                // Property drawers on collections run per element; resolve to the collection field itself.
                System.Array.Resize(ref segments, segments.Length - 2);
            }

            object current = target;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "Array" && i + 1 < segments.Length &&
                    segments[i + 1].StartsWith("data[", StringComparison.Ordinal))
                {
                    System.Collections.IList list = current as System.Collections.IList;
                    int index;
                    string indexText = segments[i + 1].Substring(5).TrimEnd(']');
                    if (list == null || !int.TryParse(indexText, out index) || index < 0 || index >= list.Count)
                    {
                        return null;
                    }

                    current = list[index];
                    if (current == null)
                    {
                        return null;
                    }

                    i++;
                    continue;
                }

                System.Reflection.FieldInfo field = FindField(current.GetType(), segments[i]);
                if (field == null)
                {
                    return null;
                }
                if (i == segments.Length - 1)
                {
                    owner = current;
                    return field;
                }

                current = field.GetValue(current);
                if (current == null)
                {
                    return null;
                }
            }

            return null;
        }

        internal static object[] GetPropertyOwners(UnityEngine.Object[] targets, string propertyPath)
        {
            var owners = new System.Collections.Generic.List<object>();
            if (targets == null)
            {
                return owners.ToArray();
            }

            foreach (UnityEngine.Object target in targets)
            {
                object owner;
                FindField(propertyPath, target, out owner);
                owners.Add(owner);
            }

            return owners.ToArray();
        }

        internal static bool ShouldDrawMember(System.Reflection.MemberInfo member, object[] owners)
        {
            object[] showConditions = Attribute.GetCustomAttributes(member, typeof(ShowIfAttribute), true);
            object[] hideConditions = Attribute.GetCustomAttributes(member, typeof(HideIfAttribute), true);
            if (showConditions.Length == 0 && hideConditions.Length == 0)
            {
                return true;
            }

            if (owners.Length == 0)
            {
                return false;
            }

            foreach (object owner in owners)
            {
                if (owner == null)
                {
                    return false;
                }

                foreach (ShowIfAttribute condition in showConditions)
                {
                    if (!ConditionMatches(owner, condition.Condition, condition.Value, condition.HasValue))
                    {
                        return false;
                    }
                }

                foreach (HideIfAttribute condition in hideConditions)
                {
                    if (ConditionMatches(owner, condition.Condition, condition.Value, condition.HasValue))
                    {
                        return false;
                    }
                }
            }

            return owners.Length > 0;
        }

        internal static bool ShouldDrawCondition(SerializedProperty property, ShowIfAttribute condition)
        {
            return ShouldDrawCondition(
                property,
                GetPropertyOwners(property.serializedObject.targetObjects, property.propertyPath),
                condition.Condition,
                condition.Value,
                condition.HasValue,
                true);
        }

        internal static bool ShouldDrawCondition(SerializedProperty property, HideIfAttribute condition)
        {
            return ShouldDrawCondition(
                property,
                GetPropertyOwners(property.serializedObject.targetObjects, property.propertyPath),
                condition.Condition,
                condition.Value,
                condition.HasValue,
                false);
        }

        private static bool ShouldDrawCondition(
            SerializedProperty property,
            object[] owners,
            string condition,
            object expectedValue,
            bool hasExpectedValue,
            bool showIf)
        {
            if (owners.Length == 0)
            {
                return false;
            }

            foreach (object owner in owners)
            {
                if (owner == null)
                {
                    return false;
                }

                object actualValue;
                bool matches = TryGetSerializedConditionValue(property, owner, condition, out actualValue)
                    ? CompareConditionValue(actualValue, expectedValue, hasExpectedValue)
                    : ConditionMatches(owner, condition, expectedValue, hasExpectedValue);
                if ((showIf && !matches) || (!showIf && matches))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetSerializedConditionValue(
            SerializedProperty property,
            object owner,
            string condition,
            out object value)
        {
            value = null;
            if (property == null || string.IsNullOrEmpty(condition))
            {
                return false;
            }

            int lastSeparator = property.propertyPath.LastIndexOf('.');
            string parentPath = lastSeparator < 0
                ? string.Empty
                : property.propertyPath.Substring(0, lastSeparator + 1);
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(parentPath + condition);
            if (conditionProperty == null)
            {
                return false;
            }

            switch (conditionProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    value = conditionProperty.boolValue;
                    return true;
                case SerializedPropertyType.Integer:
                    value = conditionProperty.intValue;
                    return true;
                case SerializedPropertyType.Float:
                    value = conditionProperty.floatValue;
                    return true;
                case SerializedPropertyType.String:
                    value = conditionProperty.stringValue;
                    return true;
                case SerializedPropertyType.ObjectReference:
                    value = conditionProperty.objectReferenceValue;
                    return true;
                case SerializedPropertyType.Enum:
                    System.Reflection.FieldInfo field = owner == null
                        ? null
                        : FindField(owner.GetType(), condition);
                    string[] enumNames = conditionProperty.enumNames;
                    int enumIndex = conditionProperty.enumValueIndex;
                    if (field == null || !field.FieldType.IsEnum ||
                        enumNames == null || enumIndex < 0 || enumIndex >= enumNames.Length)
                    {
                        return false;
                    }

                    try
                    {
                        value = Enum.Parse(field.FieldType, enumNames[enumIndex]);
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        private static bool ConditionMatches(object target, string condition, object expectedValue, bool hasExpectedValue)
        {
            if (target == null || string.IsNullOrEmpty(condition))
            {
                return false;
            }

            const System.Reflection.BindingFlags flags =
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic;

            for (System.Type type = target.GetType(); type != null; type = type.BaseType)
            {
                System.Reflection.FieldInfo field = type.GetField(condition, flags | System.Reflection.BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    return CompareConditionValue(field.GetValue(field.IsStatic ? null : target), expectedValue, hasExpectedValue);
                }

                System.Reflection.PropertyInfo property = type.GetProperty(condition, flags | System.Reflection.BindingFlags.DeclaredOnly);
                if (property != null && property.GetIndexParameters().Length == 0)
                {
                    System.Reflection.MethodInfo getter = property.GetGetMethod(true);
                    if (getter != null)
                    {
                        return CompareConditionValue(getter.Invoke(getter.IsStatic ? null : target, null), expectedValue, hasExpectedValue);
                    }
                }

                System.Reflection.MethodInfo method = type.GetMethod(
                    condition, flags | System.Reflection.BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (method != null)
                {
                    return CompareConditionValue(method.Invoke(method.IsStatic ? null : target, null), expectedValue, hasExpectedValue);
                }
            }

            return false;
        }

        private static bool CompareConditionValue(object actualValue, object expectedValue, bool hasExpectedValue)
        {
            return hasExpectedValue
                ? ValuesEqual(actualValue, expectedValue)
                : actualValue is bool && (bool)actualValue;
        }

        private static bool ValuesEqual(object actualValue, object expectedValue)
        {
            if (Equals(actualValue, expectedValue))
            {
                return true;
            }

            if (actualValue == null || expectedValue == null)
            {
                return false;
            }

            Type actualType = actualValue.GetType();
            Type expectedType = expectedValue.GetType();
            if (actualType.IsEnum && IsNumeric(expectedType))
            {
                actualValue = Convert.ChangeType(actualValue, Enum.GetUnderlyingType(actualType));
                actualType = actualValue.GetType();
            }

            if (IsNumeric(actualType) && IsNumeric(expectedType))
            {
                return Convert.ToDouble(actualValue) == Convert.ToDouble(expectedValue);
            }

            return false;
        }

        private static bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte: case TypeCode.Byte: case TypeCode.Int16: case TypeCode.UInt16:
                case TypeCode.Int32: case TypeCode.UInt32: case TypeCode.Int64: case TypeCode.UInt64:
                case TypeCode.Single: case TypeCode.Double: case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        private static System.Reflection.FieldInfo FindField(System.Type inspectedType, string fieldName)
        {
            const System.Reflection.BindingFlags flags =
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.DeclaredOnly;

            for (System.Type type = inspectedType; type != null; type = type.BaseType)
            {
                System.Reflection.FieldInfo field = type.GetField(fieldName, flags);
                if (field != null)
                {
                    return field;
                }
            }

            return null;
        }
    }
}
