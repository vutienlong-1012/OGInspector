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

`[Button]` adds a clickable control to the Inspector for a parameterless method on a
`MonoBehaviour` or `ScriptableObject`:

```csharp
[Button("Reset", ButtonSizes.Large, DirtyOnClick = true)]
private void ResetValues()
{
}
```

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