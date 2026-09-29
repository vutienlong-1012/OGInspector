using System;

namespace Mobione.MobioneInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Event)]
    public sealed class ShowInInspectorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class EnumToggleButtonsAttribute : Attribute
    {
        public EnumToggleButtonsAttribute(params object[] arguments) { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ReadOnlyAttribute : Attribute { }

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
    public sealed class InlinePropertyAttribute : Attribute { }

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
        public PreviewFieldAttribute(params object[] arguments)
        {
            Height = 100f;
            foreach (object argument in arguments)
            {
                if (argument is float height)
                {
                    Height = height;
                }
                else if (argument is int heightInPixels)
                {
                    Height = heightInPixels;
                }
            }
        }

        public float Height { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ShowInInlineEditorsAttribute : Attribute
    {
        public ShowInInlineEditorsAttribute(params object[] arguments) { }
    }
}
