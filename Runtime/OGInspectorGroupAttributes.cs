using System;

namespace OGInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order) { }
    }
}
