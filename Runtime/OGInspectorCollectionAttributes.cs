using System;

namespace Mobione.MobioneInspector
{
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Field | AttributeTargets.Property)]
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
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
        public string DrawElementMethod { get; set; }
        public string HorizontalTitle { get; set; }
        public bool SquareCells { get; set; }
        public bool HideColumnIndices { get; set; }
        public bool HideRowIndices { get; set; }
    }
}
