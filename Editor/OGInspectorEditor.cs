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
            OGInspectorReflectionDrawer.DrawSerializedProperties(serializedObject);
            OGInspectorReflectionDrawer.DrawShowInInspectorMembers(target, targets, serializedObject);
            OGInspectorReflectionDrawer.DrawAttributeButtons(target, targets, serializedObject);
        }
    }

    [CustomEditor(typeof(UnityEngine.ScriptableObject), true)]
    [CanEditMultipleObjects]
    internal sealed class OGScriptableObjectEditor : OGEditor
    {
    }

    public class OGEditorWindow : EditorWindow
    {
        private static readonly UnityEngine.Object[] EmptyTargets = new UnityEngine.Object[0];

        protected virtual void OnGUI()
        {
            OGInspectorReflectionDrawer.DrawShowInInspectorMembers(this, EmptyTargets, null);
            OGInspectorReflectionDrawer.DrawAttributeButtons(this, EmptyTargets, null);
        }
    }

    internal static class OGInspectorReflectionDrawer
    {
        public static void DrawSerializedProperties(SerializedObject serializedObject)
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                object owner;
                System.Reflection.FieldInfo field = OGInspectorConditionalVisibility.FindField(
                    property.propertyPath, serializedObject.targetObject, out owner);
                if (field != null && !OGInspectorConditionalVisibility.ShouldDrawMember(
                    field, OGInspectorConditionalVisibility.GetPropertyOwners(
                        serializedObject.targetObjects, property.propertyPath)))
                {
                    continue;
                }

                bool readOnly = field != null && field.IsDefined(typeof(ReadOnlyAttribute), true);

                using (new EditorGUI.DisabledScope(readOnly))
                {
                    EditorGUILayout.PropertyField(property, true);
                }

                if (field != null && property.propertyType == SerializedPropertyType.Generic &&
                    !property.isArray && property.isExpanded && owner != null &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                {
                    object nested = field.GetValue(owner);
                    if (nested != null)
                    {
                        DrawAttributeButtons(nested, new UnityEngine.Object[0], serializedObject);
                    }
                }

                if (field != null && property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    PreviewFieldAttribute preview = (PreviewFieldAttribute)Attribute.GetCustomAttribute(
                        field, typeof(PreviewFieldAttribute), true);
                    if (preview != null)
                    {
                        DrawPreview(property.objectReferenceValue, preview);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
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

        private static object[] GetMemberOwners(object target, UnityEngine.Object[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                return target == null ? new object[0] : new[] { target };
            }

            var owners = new object[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                owners[i] = targets[i];
            }

            return owners;
        }

        private static void DrawPreview(UnityEngine.Object value, PreviewFieldAttribute attribute)
        {
            if (value == null)
            {
                return;
            }

            float height = Mathf.Max(1f, attribute.Height);
            Rect previewRect = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));

            if (value is Sprite sprite && DrawSpritePreview(previewRect, sprite))
            {
                return;
            }

            Texture preview = AssetPreview.GetAssetPreview(value);
            if (preview == null)
            {
                preview = AssetPreview.GetMiniThumbnail(value);
            }
            if (preview == null)
            {
                return;
            }

            // Draw with alpha blending so PNGs/textures keep their transparency instead of
            // being composited against the opaque preview material used by DrawPreviewTexture.
            GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit, true);
        }

        private static bool DrawSpritePreview(Rect previewRect, Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            if (texture == null)
            {
                return false;
            }

            Rect textureRect = sprite.textureRect;
            if (textureRect.width <= 0f || textureRect.height <= 0f)
            {
                return false;
            }

            // Only sample the sprite's own region of the atlas, not the whole packed texture.
            Rect texCoords = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);

            Rect fitRect = FitRect(previewRect, textureRect.width, textureRect.height);
            GUI.DrawTextureWithTexCoords(fitRect, texture, texCoords, true);
            return true;
        }

        private static Rect FitRect(Rect container, float width, float height)
        {
            if (width <= 0f || height <= 0f)
            {
                return container;
            }

            float scale = Mathf.Min(container.width / width, container.height / height);
            float fittedWidth = width * scale;
            float fittedHeight = height * scale;
            return new Rect(
                container.x + ((container.width - fittedWidth) * 0.5f),
                container.y + ((container.height - fittedHeight) * 0.5f),
                fittedWidth,
                fittedHeight);
        }

        public static void DrawShowInInspectorMembers(object target, UnityEngine.Object[] targets, SerializedObject serializedObject)
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
                    (serializedObject == null || serializedObject.FindProperty(field.Name) == null) &&
                    OGInspectorConditionalVisibility.ShouldDrawMember(field, GetMemberOwners(target, targets)))
                {
                    DrawField(target, targets, field);
                }
            }

            foreach (System.Reflection.PropertyInfo property in inspectedType.GetProperties(flags))
            {
                if (property.IsDefined(typeof(ShowInInspectorAttribute), true) &&
                    property.GetIndexParameters().Length == 0 &&
                    OGInspectorConditionalVisibility.ShouldDrawMember(property, GetMemberOwners(target, targets)))
                {
                    DrawProperty(target, targets, property);
                }
            }

            foreach (System.Reflection.MethodInfo method in inspectedType.GetMethods(flags))
            {
                if (method.IsDefined(typeof(ShowInInspectorAttribute), true) &&
                    !method.IsSpecialName &&
                    !method.IsGenericMethod &&
                    method.GetParameters().Length == 0 &&
                    OGInspectorConditionalVisibility.ShouldDrawMember(method, GetMemberOwners(target, targets)))
                {
                    DrawMethod(target, targets, method);
                }
            }
        }

        private static void DrawField(object target, UnityEngine.Object[] targets, System.Reflection.FieldInfo field)
        {
            object value = field.GetValue(target);
            bool readOnly = field.IsInitOnly || field.IsLiteral ||
                field.IsDefined(typeof(ReadOnlyAttribute), true);
            object updatedValue = DrawValue(field.Name, field.FieldType, value, readOnly);
            PreviewFieldAttribute preview = (PreviewFieldAttribute)Attribute.GetCustomAttribute(
                field, typeof(PreviewFieldAttribute), true);
            if (preview != null)
            {
                DrawPreview(updatedValue as UnityEngine.Object, preview);
            }

            if (!readOnly && !Equals(value, updatedValue))
            {
                if (targets.Length == 0)
                {
                    field.SetValue(target, updatedValue);
                }
                else
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
        }

        private static void DrawProperty(object target, UnityEngine.Object[] targets, System.Reflection.PropertyInfo property)
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
            PreviewFieldAttribute preview = (PreviewFieldAttribute)Attribute.GetCustomAttribute(
                property, typeof(PreviewFieldAttribute), true);
            if (preview != null)
            {
                DrawPreview(updatedValue as UnityEngine.Object, preview);
            }

            if (!readOnly && !Equals(value, updatedValue))
            {
                System.Reflection.MethodInfo setter = property.GetSetMethod(true);
                if (targets.Length == 0)
                {
                    setter.Invoke(target, new[] { updatedValue });
                }
                else
                {
                    foreach (UnityEngine.Object selectedTarget in targets)
                    {
                        Undo.RecordObject(selectedTarget, "Change " + property.Name);
                        setter.Invoke(selectedTarget, new[] { updatedValue });
                        EditorUtility.SetDirty(selectedTarget);
                    }
                }
            }
        }

        private static void DrawMethod(object target, UnityEngine.Object[] targets, System.Reflection.MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name)))
                {
                    if (targets.Length == 0)
                    {
                        method.Invoke(target, null);
                    }
                    else
                    {
                        foreach (UnityEngine.Object selectedTarget in targets)
                        {
                            method.Invoke(selectedTarget, null);
                            EditorUtility.SetDirty(selectedTarget);
                        }
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

            System.Type elementType;
            if (TryGetListElementType(type, out elementType))
            {
                System.Collections.IList list = value as System.Collections.IList;
                if (list == null && !readOnly)
                {
                    list = CreateListInstance(type, elementType);
                }

                if (list == null)
                {
                    EditorGUILayout.LabelField(label, "null");
                    return value;
                }

                return DrawListValue(label, list, elementType, readOnly);
            }

            if (IsSerializableObjectType(type))
            {
                if (value == null && !readOnly) value = CreateInstanceOrNull(type);
                if (value == null)
                {
                    EditorGUILayout.LabelField(label, "null");
                    return value;
                }

                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (System.Reflection.FieldInfo field in type.GetFields(
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic))
                {
                    if (field.IsStatic || field.IsNotSerialized) continue;
                    if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), true)) continue;
                    if (!OGInspectorConditionalVisibility.ShouldDrawMember(field, new[] { value })) continue;
                    field.SetValue(value, DrawValue(field.Name, field.FieldType, field.GetValue(value), readOnly));
                }

                EditorGUI.indentLevel--;
                return value;
            }

            EditorGUILayout.LabelField(label, value == null ? "null" : value.ToString());
            return value;
        }

        private static bool TryGetListElementType(System.Type type, out System.Type elementType)
        {
            elementType = null;
            if (type == null || !type.IsGenericType)
            {
                return false;
            }

            System.Type genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(System.Collections.Generic.List<>) ||
                genericType == typeof(System.Collections.Generic.IList<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        private static System.Collections.IList CreateListInstance(System.Type type, System.Type elementType)
        {
            System.Type listType = typeof(System.Collections.Generic.List<>).MakeGenericType(elementType);
            if (type.IsAssignableFrom(listType))
            {
                return (System.Collections.IList)Activator.CreateInstance(listType);
            }

            return null;
        }

        private static object DrawListValue(
            string label,
            System.Collections.IList list,
            System.Type elementType,
            bool readOnly)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            int removeIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                string elementLabel = "Element " + i;
                System.Type nestedElementType;
                bool composite = IsSerializableObjectType(elementType) ||
                    TryGetListElementType(elementType, out nestedElementType);
                if (composite)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                }

                object value = DrawValue(elementLabel, elementType, list[i], readOnly);
                if (!readOnly && !Equals(list[i], value))
                {
                    list[i] = value;
                }

                if (composite)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }

                if (!readOnly && GUILayout.Button("-", GUILayout.Width(24f)))
                {
                    removeIndex = i;
                }

                if (composite)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (removeIndex >= 0)
            {
                list.RemoveAt(removeIndex);
            }

            if (!readOnly && GUILayout.Button("Add Element"))
            {
                list.Add(GetDefaultValue(elementType));
            }

            EditorGUI.indentLevel--;
            return list;
        }

        private static bool IsSerializableObjectType(System.Type type)
        {
            return (type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum)) &&
                   type != typeof(string) &&
                   !typeof(UnityEngine.Object).IsAssignableFrom(type) &&
                   type.IsDefined(typeof(SerializableAttribute), false);
        }

        private static object CreateInstanceOrNull(System.Type type)
        {
            try { return Activator.CreateInstance(type, true); }
            catch (Exception) { return null; }
        }

        public static void DrawAttributeButtons(object target, UnityEngine.Object[] targets, SerializedObject serializedObject)
        {
            var methods = new System.Collections.Generic.List<System.Reflection.MethodInfo>();
            foreach (System.Reflection.MethodInfo method in target.GetType().GetMethods(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic))
            {
                var button = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute), true);
                if (button != null && !method.IsGenericMethod &&
                    OGInspectorConditionalVisibility.ShouldDrawMember(method, GetMemberOwners(target, targets)))
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

                System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                object[] args = GetButtonArguments(target, method, parameters);
                if (parameters.Length > 0)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                }

                DrawButtonParameters(parameters, args);

                if (GUILayout.Button(label, GUILayout.Height(height)))
                {
                    if (serializedObject != null)
                    {
                        serializedObject.ApplyModifiedProperties();
                    }

                    if (targets.Length == 0)
                    {
                        try
                        {
                            StoreButtonResult(target, method, method.Invoke(target, args));
                        }
                        catch (System.Reflection.TargetInvocationException exception)
                        {
                            Debug.LogException(exception.InnerException ?? exception, target as UnityEngine.Object);
                        }
                    }
                    else
                    {
                        foreach (UnityEngine.Object selectedTarget in targets)
                        {
                            if (button.DirtyOnClick)
                            {
                                Undo.RecordObject(selectedTarget, label);
                            }

                            try
                            {
                                StoreButtonResult(selectedTarget, method, method.Invoke(selectedTarget, args));
                            }
                            catch (System.Reflection.TargetInvocationException exception)
                            {
                                Debug.LogException(exception.InnerException ?? exception, selectedTarget);
                            }
                        }
                    }

                    if (serializedObject != null)
                    {
                        serializedObject.Update();
                    }
                }

                if (parameters.Length > 0)
                {
                    EditorGUILayout.EndVertical();
                }

                DrawButtonResult(target, method, parameters, args);
            }
        }

        private static readonly System.Collections.Generic.Dictionary<string, object[]> ButtonArguments =
            new System.Collections.Generic.Dictionary<string, object[]>();
        private static readonly System.Collections.Generic.Dictionary<string, object> ButtonResults =
            new System.Collections.Generic.Dictionary<string, object>();

        private static string GetButtonKey(object target, System.Reflection.MethodInfo method)
        {
            var unityObject = target as UnityEngine.Object;
            int id = unityObject != null ? unityObject.GetInstanceID() : System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(target);
            return id + ":" + method.DeclaringType + ":" + method.MetadataToken;
        }

        private static System.Type GetParameterType(System.Reflection.ParameterInfo parameter)
        {
            return parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
        }

        private static object GetDefaultValue(System.Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            if (type == typeof(string)) return string.Empty;
            System.Type elementType;
            if (TryGetListElementType(type, out elementType)) return CreateListInstance(type, elementType);
            if (IsSerializableObjectType(type)) return CreateInstanceOrNull(type);
            return null;
        }

        private static object[] GetButtonArguments(object target, System.Reflection.MethodInfo method, System.Reflection.ParameterInfo[] parameters)
        {
            string key = GetButtonKey(target, method);
            object[] args;
            if (!ButtonArguments.TryGetValue(key, out args) || args.Length != parameters.Length)
            {
                args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].HasDefaultValue && parameters[i].DefaultValue != null && !(parameters[i].DefaultValue is DBNull)
                        ? parameters[i].DefaultValue
                        : GetDefaultValue(GetParameterType(parameters[i]));
                }
                ButtonArguments[key] = args;
            }

            return args;
        }

        private static void DrawButtonParameters(System.Reflection.ParameterInfo[] parameters, object[] args)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut)
                {
                    continue;
                }

                args[i] = DrawValue(parameters[i].Name, GetParameterType(parameters[i]), args[i], false);
            }
        }

        private static void StoreButtonResult(object target, System.Reflection.MethodInfo method, object result)
        {
            if (method.ReturnType != typeof(void))
            {
                ButtonResults[GetButtonKey(target, method)] = result;
            }
        }

        private static void DrawButtonResult(object target, System.Reflection.MethodInfo method, System.Reflection.ParameterInfo[] parameters, object[] args)
        {
            object result;
            if (method.ReturnType != typeof(void) && ButtonResults.TryGetValue(GetButtonKey(target, method), out result))
            {
                DrawValue("Result", method.ReturnType, result, true);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut)
                {
                    DrawValue(parameters[i].Name, GetParameterType(parameters[i]), args[i], true);
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
