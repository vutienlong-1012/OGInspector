# OGInspector

## Installation

Install as a Unity package via the Package Manager:

1. Open **Window > Package Manager**.
2. Click the **+** button and choose **Add package from git URL...**.
3. Enter `https://github.com/vutienlong-1012/OGInspector.git` and click **Add**.

This installs the package under **Packages**, not **Assets**. If you want the source
files under **Assets**, download this repository as a ZIP and copy its `Runtime` and
`Editor` folders into a folder such as `Assets/OGInspector`. Keep the `.asmdef` files
in those folders. Do not use both installation methods in the same project, or Unity
will see duplicate type definitions.

To pin a specific version, append `#<tag>` to the URL, e.g.
`https://github.com/vutienlong-1012/OGInspector.git#1.0.0`.

## Usage

[ShowInInspector] displays non-serialized fields, properties, and parameterless methods
in the Inspector. Read-only members (including members marked `[ReadOnly]`) are
displayed but cannot be edited.

```csharp
[ShowInInspector]
private int health = 100;

[ShowInInspector]
private string Status => health > 0 ? "Alive" : "Defeated";
```

`[ReadOnly]` also disables editing for serialized fields, and `[PreviewField]`
shows a preview beneath a Unity object reference. The preview height defaults to
100 pixels and can be set with an integer or float:

```csharp
[ReadOnly] public int Id;
[PreviewField(120)] public GameObject Model;
```

`[ShowIf]` and `[HideIf]` conditionally display fields, properties, and methods.
Pass the name of a bool field/property/method to test whether it is `true`, or pass
an expected value to compare against:

```csharp
[ShowIf(nameof(mode), IdleRewardMode.FixedPerHour)]
public double ratePerHour;

[HideIf(nameof(isRewardEnabled))]
public string disabledReason;
```

[ShowIf] requires every condition to match; `[HideIf]` hides the member when any
condition matches. Both attributes can be applied multiple times.

`[Button]` adds a clickable control to the Inspector for a method on a
`MonoBehaviour` or `ScriptableObject`. Methods can take multiple supported inputs,
including editable `List<T>` parameters:

```csharp
[Button("Reset", ButtonSizes.Large, DirtyOnClick = true)]
private void ResetValues()
{
}

[Button]
private void NewLevel(float chapterCoefficient, List<int> allowedUnitIds)
{
}
```

`[Button]` also works in plain `[Serializable]` classes (not derived from
`MonoBehaviour`) used as serialized fields; the buttons appear under the
expanded field.

The button label defaults to the method name. `ButtonHeight` can override the
height selected by `ButtonSize`.

To show `[ShowInInspector]` members and `[Button]` methods in a custom
`EditorWindow`, derive from `OGEditorWindow` instead of `EditorWindow` (this is
the equivalent of Odin's `OdinEditorWindow`):

```csharp
public class UserDataManagerEditor : OGEditorWindow
{
    [MenuItem("Tools/User Data Editor")]
    private static void OpenWindow()
    {
        GetWindow<UserDataManagerEditor>().Show();
    }

    [ShowInInspector] public static UserData UserData;

    [Button(ButtonSizes.Gigantic)]
    void Refresh() { }
}
```

If you override `OnGUI` in a subclass, call `base.OnGUI()` to keep drawing
these members.