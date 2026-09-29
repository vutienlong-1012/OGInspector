# OGInspector

`[Button]` adds a clickable control to the Inspector for a parameterless method on a
`SerializedMonoBehaviour` or `SerializedScriptableObject`:

```csharp
[Button("Reset", ButtonSizes.Large, DirtyOnClick = true)]
private void ResetValues()
{
    // Reset this object's values.
}
```

The button label defaults to the method name. `ButtonHeight` can override the
height selected by `ButtonSize`.