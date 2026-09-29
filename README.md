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