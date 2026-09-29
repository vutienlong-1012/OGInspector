namespace OGInspector.Editor
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
            DrawAttributeButtons();
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

namespace OGInspector.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class OGInspectorEditorGUI
    {
        public static void BeginHorizontalToolbar() { EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); }
        public static bool ToolbarButton(string label) { return GUILayout.Button(label, EditorStyles.toolbarButton); }
        public static void EndHorizontalToolbar() { EditorGUILayout.EndHorizontal(); }
    }
}
