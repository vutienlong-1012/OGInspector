
using System;

namespace Sirenix.OdinInspector
{
    public class SerializedMonoBehaviour : UnityEngine.MonoBehaviour
    {
    }

    public class SerializedScriptableObject : UnityEngine.ScriptableObject
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ShowOGSerializedPropertiesInInspectorAttribute : Attribute
    {
    }

    public enum ButtonSizes
    {
        Small,
        Medium,
        Large,
        Gigantic
    }

    public enum TitleAlignments
    {
        Left,
        Centered,
        Right
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ButtonAttribute : Attribute
    {
        public ButtonAttribute(params object[] arguments)
        {
            ButtonSize = ButtonSizes.Medium;

            if (arguments == null)
            {
                return;
            }

            foreach (object argument in arguments)
            {
                if (argument is string name)
                {
                    Name = name;
                }
                else if (argument is ButtonSizes buttonSize)
                {
                    ButtonSize = buttonSize;
                }
            }
        }

        public string Name { get; set; }
        public ButtonSizes ButtonSize { get; set; }
        public float ButtonHeight { get; set; }
        public bool DirtyOnClick { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ButtonGroupAttribute : Attribute
    {
        public ButtonGroupAttribute(params object[] arguments) { }
        public string GroupID { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ShowInInspectorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class EnumToggleButtonsAttribute : Attribute
    {
        public EnumToggleButtonsAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ReadOnlyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class BoxGroupAttribute : Attribute
    {
        public BoxGroupAttribute(string group, bool centerLabel = false)
        {
            CenterLabel = centerLabel;
        }

        public BoxGroupAttribute(params object[] arguments) { }

        public bool CenterLabel { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class FoldoutGroupAttribute : Attribute
    {
        public FoldoutGroupAttribute(string group) { }
        public FoldoutGroupAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HideIfAttribute : Attribute
    {
        public HideIfAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ShowIfAttribute : Attribute
    {
        public ShowIfAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class DisableInEditorModeAttribute : Attribute
    {
        public DisableInEditorModeAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class HorizontalGroupAttribute : Attribute
    {
        public HorizontalGroupAttribute(params object[] arguments) { }
        public float Width { get; set; }
        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class VerticalGroupAttribute : Attribute
    {
        public VerticalGroupAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class TabGroupAttribute : Attribute
    {
        public TabGroupAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class TitleGroupAttribute : Attribute
    {
        public TitleGroupAttribute(string title, object subtitle = null, TitleAlignments alignment = TitleAlignments.Left) { }
        public TitleGroupAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class TitleAttribute : Attribute
    {
        public TitleAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InlineButtonAttribute : Attribute
    {
        public InlineButtonAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InlinePropertyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InlineEditorAttribute : Attribute
    {
        public InlineEditorObjectFieldModes ObjectFieldMode { get; set; }
    }

    public enum InlineEditorObjectFieldModes
    {
        Default,
        Hidden
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AssetListAttribute : Attribute
    {
        public bool AutoPopulate { get; set; }
        public string Path { get; set; }
    }

    public enum Units
    {
        None,
        Percent,
        Unitless,
        Seconds,
        Minutes,
        Hours,
        Meters,
        Kilometers,
        Feet,
        Miles,
        Degrees,
        Radians
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class UnitAttribute : Attribute
    {
        public UnitAttribute(params object[] arguments) { }
        public UnitAttribute(Units unit) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class LabelTextAttribute : Attribute
    {
        public LabelTextAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class LabelWidthAttribute : Attribute
    {
        public LabelWidthAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HideLabelAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class PreviewFieldAttribute : Attribute
    {
        public PreviewFieldAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ShowInInlineEditorsAttribute : Attribute
    {
        public ShowInInlineEditorsAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ValueDropdownAttribute : Attribute
    {
        public ValueDropdownAttribute(params object[] arguments) { }
        public bool ExpandAllMenuItems { get; set; }
    }

    public class ValueDropdownList<T> : System.Collections.Generic.List<ValueDropdownItem>
    {
        public void Add(string text, T value)
        {
            base.Add(new ValueDropdownItem(text, value));
        }
    }

    public class ValueDropdownItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public ValueDropdownItem(string text, object value)
        {
            Text = text;
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class PropertySpaceAttribute : Attribute
    {
        public PropertySpaceAttribute(float spaceBefore = 0f, float spaceAfter = 0f) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class GUIColorAttribute : Attribute
    {
        public GUIColorAttribute(float r, float g, float b) { }
        public GUIColorAttribute(float r, float g, float b, float a) { }
        public GUIColorAttribute(string color) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ProgressBarAttribute : Attribute
    {
        public ProgressBarAttribute(params object[] arguments) { }
        public bool Segmented { get; set; }
        public string ColorGetter { get; set; }
        public object GetValue { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class SearchableAttribute : Attribute
    {
        public SearchableAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FolderPathAttribute : Attribute
    {
        public bool AbsolutePath { get; set; }
        public FolderPathAttribute(params object[] arguments) { }
        public FolderPathAttribute(bool absolutePath = false) { AbsolutePath = absolutePath; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FilePathAttribute : Attribute
    {
        public FilePathAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class TableListAttribute : Attribute
    {
        public TableListAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TableColumnWidthAttribute : Attribute
    {
        public TableColumnWidthAttribute(float width) { }
        public bool Resizable { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DictionaryDrawerSettingsAttribute : Attribute
    {
        public string KeyLabel { get; set; }
        public string ValueLabel { get; set; }
        public string IsReadOnly { get; set; }
        public DictionaryDrawerSettingsAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ListDrawerSettingsAttribute : Attribute
    {
        public bool Expanded { get; set; }
        public bool IsReadOnly { get; set; }
        public ListDrawerSettingsAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OnValueChangedAttribute : Attribute
    {
        public OnValueChangedAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiredFieldAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiredComponentAttribute : Attribute
    {
        public RequiredComponentAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TypeColumnIdxAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TableMatrixAttribute : Attribute
    {
        public string HorizontalTitle { get; set; }
        public bool SquareCells { get; set; }
    }
}

namespace Sirenix.Serialization
{
}
namespace Sirenix.Utilities
{
}

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SerializedMonoBehaviour), true)]
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

    [CustomEditor(typeof(SerializedScriptableObject), true)]
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

namespace Sirenix.Utilities.Editor
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
#endif
