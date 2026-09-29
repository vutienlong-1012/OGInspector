using System;

namespace Mobione.MobioneInspector
{
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InlineButtonAttribute : Attribute
    {
        public InlineButtonAttribute(params object[] arguments) { }
    }
}
