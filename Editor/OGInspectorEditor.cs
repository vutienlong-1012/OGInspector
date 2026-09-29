namespace Mobione.MobioneInspector.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UnityEngine.MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class OGEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawShowInInspectorMembers();
            DrawAttributeButtons();
        }

        private void DrawShowInInspectorMembers()
        {
            System.Type inspectedType = target.GetType();
            System.Reflection.BindingFlags flags =
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic;

            foreach (System.Reflection.FieldInfo field in inspectedType.GetFields(flags))
            {
                if (field.IsDefined(typeof(ShowInInspectorAttribute), true) &&
                    serializedObject.FindProperty(field.Name) == null)
                {
                    DrawField(field);
                }
            }

            foreach (System.Reflection.PropertyInfo property in inspectedType.GetProperties(flags))
            {
                if (property.IsDefined(typeof(ShowInInspectorAttribute), true) &&
                    property.GetIndexParameters().Length == 0)
                {
                    DrawProperty(property);
                }
            }

            foreach (System.Reflection.MethodInfo method in inspectedType.GetMethods(flags))
            {
                if (method.IsDefined(typeof(ShowInInspectorAttribute), true) &&
                    !method.IsSpecialName &&
                    !method.IsGenericMethod &&
                    method.GetParameters().Length == 0)
                {
                    DrawMethod(method);
                }
            }
        }

        private void DrawField(System.Reflection.FieldInfo field)
        {
            object value = field.GetValue(target);
            bool readOnly = field.IsInitOnly || field.IsLiteral ||
                field.IsDefined(typeof(ReadOnlyAttribute), true);
            object updatedValue = DrawValue(field.Name, field.FieldType, value, readOnly);

            if (!readOnly && !Equals(value, updatedValue))
            {
                foreach (UnityEngine.Object selectedTarget in targets)
                {
                    if (!field.IsStatic)
                    {
                        Undo.RecordObject(selectedTarget, "Change " + field.Name);
                    }

                    field.SetValue(selectedTarget, updatedValue);
                    EditorUtility.SetDirty(selectedTarget);
                }
            }
        }

        private void DrawProperty(System.Reflection.PropertyInfo property)
        {
            System.Reflection.MethodInfo getter = property.GetGetMethod(true);
            if (getter == null)
            {
                return;
            }

            object value = getter.Invoke(target, null);
            bool readOnly = property.GetSetMethod(true) == null ||
                property.IsDefined(typeof(ReadOnlyAttribute), true);
            object updatedValue = DrawValue(property.Name, property.PropertyType, value, readOnly);

            if (!readOnly && !Equals(value, updatedValue))
            {
                foreach (UnityEngine.Object selectedTarget in targets)
                {
                    Undo.RecordObject(selectedTarget, "Change " + property.Name);
                    property.GetSetMethod(true).Invoke(selectedTarget, new[] { updatedValue });
                    EditorUtility.SetDirty(selectedTarget);
                }
            }
        }

        private void DrawMethod(System.Reflection.MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name)))
                {
                    foreach (UnityEngine.Object selectedTarget in targets)
                    {
                        method.Invoke(selectedTarget, null);
                        EditorUtility.SetDirty(selectedTarget);
                    }
                }
                return;
            }

            object value = method.Invoke(target, null);
            DrawValue(method.Name, method.ReturnType, value, true);
        }

        private static object DrawValue(string name, System.Type type, object value, bool readOnly)
        {
            string label = ObjectNames.NicifyVariableName(name);
            using (new EditorGUI.DisabledScope(readOnly))
            {
                if (type == typeof(bool)) return EditorGUILayout.Toggle(label, value != null && (bool)value);
                if (type == typeof(int)) return EditorGUILayout.IntField(label, value == null ? 0 : (int)value);
                if (type == typeof(float)) return EditorGUILayout.FloatField(label, value == null ? 0f : (float)value);
                if (type == typeof(double)) return EditorGUILayout.DoubleField(label, value == null ? 0d : (double)value);
                if (type == typeof(string)) return EditorGUILayout.TextField(label, value as string ?? string.Empty);
                if (type == typeof(Vector2)) return EditorGUILayout.Vector2Field(label, value == null ? Vector2.zero : (Vector2)value);
                if (type == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, value == null ? Vector3.zero : (Vector3)value);
                if (type == typeof(Vector4)) return EditorGUILayout.Vector4Field(label, value == null ? Vector4.zero : (Vector4)value);
                if (type == typeof(Color)) return EditorGUILayout.ColorField(label, value == null ? Color.white : (Color)value);
                if (type == typeof(UnityEngine.Object) || type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    return EditorGUILayout.ObjectField(label, value as UnityEngine.Object, type, true);
                }
                if (type.IsEnum)
                {
                    return EditorGUILayout.EnumPopup(label, value == null ? (System.Enum)System.Enum.GetValues(type).GetValue(0) : (System.Enum)value);
                }
            }

            EditorGUILayout.LabelField(label, value == null ? "null" : value.ToString());
            return value;
        }

        private void DrawAttributeButtons()
        {
            var methods = new System.Collections.Generic.List<System.Reflection.MethodInfo>();
            foreach (System.Reflection.MethodInfo method in target.GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic))
            {
                var button = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute), true);
                if (button != null && !method.IsGenericMethod && method.GetParameters().Length == 0)
                {
                    methods.Add(method);
                }
            }

            methods.Sort((left, right) => left.MetadataToken.CompareTo(right.MetadataToken));
            foreach (System.Reflection.MethodInfo method in methods)
            {
                var button = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute), true);
                string label = string.IsNullOrEmpty(button.Name) ? method.Name : button.Name;
                float height = button.ButtonHeight > 0f ? button.ButtonHeight : GetButtonHeight(button.ButtonSize);

                if (GUILayout.Button(label, GUILayout.Height(height)))
                {
                    serializedObject.ApplyModifiedProperties();

                    foreach (UnityEngine.Object selectedTarget in targets)
                    {
                        if (button.DirtyOnClick)
                        {
                            Undo.RecordObject(selectedTarget, label);
                        }

                        try
                        {
                            method.Invoke(selectedTarget, null);
                        }
                        catch (System.Reflection.TargetInvocationException exception)
                        {
                            Debug.LogException(exception.InnerException ?? exception, selectedTarget);
                        }
                    }

                    serializedObject.Update();
                }
            }
        }

        private static float GetButtonHeight(ButtonSizes size)
        {
            switch (size)
            {
                case ButtonSizes.Small:
                    return EditorGUIUtility.singleLineHeight;
                case ButtonSizes.Large:
                    return EditorGUIUtility.singleLineHeight * 2f;
                case ButtonSizes.Gigantic:
                    return EditorGUIUtility.singleLineHeight * 3f;
                default:
                    return EditorGUIUtility.singleLineHeight * 1.5f;
            }
        }
    }

    [CustomEditor(typeof(UnityEngine.ScriptableObject), true)]
    [CanEditMultipleObjects]
    internal sealed class OGScriptableObjectEditor : OGEditor
    {
    }

    public class OGEditorWindow : EditorWindow
    {
    }

    public class OGMenuEditorWindow : EditorWindow
    {
        protected OGMenuTree MenuTree { get; private set; }

        protected virtual OGMenuTree BuildMenuTree()
        {
            return new OGMenuTree();
        }

        protected virtual void OnBeginDrawEditors() { }

        protected virtual void OnGUI()
        {
            MenuTree = BuildMenuTree();
            OnBeginDrawEditors();
        }
    }

    public class OGMenuTree
    {
        public OGMenuTreeSelection Selection { get; } = new OGMenuTreeSelection();

        public void AddAllAssetsAtPath(string menuPath, string assetFolder, System.Type type) { }
        public void SortMenuItemsByName() { }
        public void Add(string menuPath, object value) { }
    }

    public class OGMenuTreeSelection
    {
        public object SelectedValue { get; set; }
    }
}

namespace Mobione.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class MobioneEditorGUI
    {
        public static void BeginHorizontalToolbar() { EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); }
        public static bool ToolbarButton(string label) { return GUILayout.Button(label, EditorStyles.toolbarButton); }
        public static void EndHorizontalToolbar() { EditorGUILayout.EndHorizontal(); }
    }
}
